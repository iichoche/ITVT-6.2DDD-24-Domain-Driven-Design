import os
import json
import threading
import logging

from flask import Flask, request, jsonify
from azure.servicebus import ServiceBusClient, ServiceBusMessage
import joblib
import jwt
from jwt import InvalidTokenError
from dotenv import load_dotenv

load_dotenv()

from config import Config

# Logging setup
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(threadName)s %(message)s"
)
logger = logging.getLogger(__name__)

# JWT settings
JWT_SECRET = Config.JWT_SECRET
JWT_ALGORITHM = Config.JWT_ALGORITHM

# Service Bus configs
SB_SEND_CONN_STR = Config.SERVICEBUS_SEND_CONN_STR
SB_LISTEN_CONN_STR = Config.SERVICEBUS_LISTEN_CONN_STR
REQ_QUEUE = Config.REQUEST_QUEUE_NAME
RES_QUEUE = Config.RESPONSE_QUEUE_NAME
TESTING = Config.TESTING

# Load model
model_dir = Config.MODEL_DIR
model_files = [f for f in os.listdir(model_dir) if f.endswith(".pkl")]
model_files.sort(key=lambda fn: os.path.getmtime(os.path.join(model_dir, fn)), reverse=True)

if not model_files:
    raise FileNotFoundError(f"No .pkl files found in {model_dir}")

latest_model_path = os.path.join(model_dir, model_files[0])
model = joblib.load(latest_model_path)

# Flask app
app = Flask(__name__)

# Health check
@app.route("/ping", methods=["GET"])
def healthz():
    return "OK", 200

# JWT verification function
def verify_jwt(token):
    try:
        payload = jwt.decode(token, JWT_SECRET, algorithms=[JWT_ALGORITHM])
        return payload
    except InvalidTokenError as e:
        logger.warning(f"JWT verification failed: {e}")
        return None

# Prediction logic
def run_prediction(categories):
    values = [int(v) for v in categories]
    n_feats = model.n_features_in_

    if len(values) < n_feats:
        values += [0] * (n_feats - len(values))
    elif len(values) > n_feats:
        raise ValueError(f"Too many features: got {len(values)}, max is {n_feats}")

    logger.info(f"Input values: {values}")
    pred = int(model.predict([values])[0])
    probas = model.predict_proba([values])[0]
    classes = model.classes_

    ranking = sorted(
        [
            {"healthcareTech": int(c), "percentage": round(p * 100, 2)}
            for c, p in zip(classes, probas)
        ],
        key=lambda x: x["percentage"],
        reverse=True
    )

    logger.info(f"Prediction: {pred}")
    logger.info(f"Ranking: {ranking}")

    return {
        "recommended_healthcareTech": pred,
        "healthcareTech_ranking": ranking
    }

# HTTP endpoint
@app.route("/get_advice", methods=["POST"])
def get_advice():
    auth_header = request.headers.get("Authorization")
    if not auth_header or not auth_header.startswith("Bearer "):
        return jsonify({"error": "Missing or invalid Authorization header"}), 401

    token = auth_header.split(" ")[1]
    jwt_payload = verify_jwt(token)
    if not jwt_payload:
        return jsonify({"error": "Invalid or expired JWT token"}), 401

    payload = request.get_json(force=True)
    categories = payload.get("Categories")

    if not isinstance(categories, list):
        return jsonify({"error": "Please provide 'Categories' as a list of ints."}), 400

    try:
        result = run_prediction(categories)
        return jsonify(result)
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# Azure Service Bus clients
if TESTING != "1":
    sb_sender_client = ServiceBusClient.from_connection_string(SB_SEND_CONN_STR)
    sb_receiver_client = ServiceBusClient.from_connection_string(SB_LISTEN_CONN_STR)

# Background Service Bus worker
def servicebus_worker():
    if TESTING == "1":
        return

    with sb_receiver_client, sb_sender_client:
        while True:
            with sb_receiver_client.get_queue_receiver(queue_name=REQ_QUEUE, max_wait_time=5) as receiver, \
                 sb_sender_client.get_queue_sender(queue_name=RES_QUEUE) as sender:

                for msg in receiver:
                    jwt_token = msg.correlation_id
                    jwt_payload = verify_jwt(jwt_token)
                    if not jwt_payload:
                        receiver.dead_letter_message(
                            msg,
                            reason="Unauthorized: invalid JWT token",
                            error_description="Invalid or expired JWT in correlation_id"
                        )
                        continue

                    try:
                        body_bytes = b"".join(msg.body)
                        payload = json.loads(body_bytes.decode("utf-8"))
                    except Exception as e:
                        receiver.dead_letter_message(
                            msg,
                            reason="Invalid JSON",
                            error_description=str(e)
                        )
                        continue

                    categories = payload.get("Categories")
                    if not isinstance(categories, list) or not categories or not all(isinstance(x, int) for x in categories):
                        receiver.dead_letter_message(
                            msg,
                            reason="Bad payload: 'Categories'",
                            error_description="Must be non-empty list of ints"
                        )
                        continue

                    try:
                        result = run_prediction(categories)
                        reply = ServiceBusMessage(
                            json.dumps(result),
                            correlation_id=msg.correlation_id
                        )
                        sender.send_messages(reply)
                        receiver.complete_message(msg)
                    except Exception as err:
                        receiver.dead_letter_message(
                            msg,
                            reason="Prediction error",
                            error_description=str(err)
                        )

threading.Thread(target=servicebus_worker, daemon=True).start()

# Run Flask app
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)
