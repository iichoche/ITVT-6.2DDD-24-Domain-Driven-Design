import os
import json
import threading

from flask import Flask, request, jsonify
import joblib
from azure.servicebus import ServiceBusClient, ServiceBusMessage
from dotenv import load_dotenv

load_dotenv()

from config import Config
import logging

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(threadName)s %(message)s"
)
logger = logging.getLogger(__name__)

# Load configs
EXPECTED_API_KEY = Config.API_KEY
SB_SEND_CONN_STR = Config.SERVICEBUS_SEND_CONN_STR
SB_LISTEN_CONN_STR = Config.SERVICEBUS_LISTEN_CONN_STR
REQ_QUEUE = Config.REQUEST_QUEUE_NAME
RES_QUEUE = Config.RESPONSE_QUEUE_NAME
TESTING = Config.TESTING

# model direction loading
model_dir = Config.MODEL_DIR
model_files = [f for f in os.listdir(model_dir) if f.endswith(".pkl")]
model_files.sort(
    key=lambda fn: os.path.getmtime(os.path.join(model_dir, fn)), 
    reverse=True
)

if not model_files:
    raise FileNotFoundError(f"No .pkl files found in {model_dir}")
latest_model_path = os.path.join(model_dir, model_files[0])
model = joblib.load(latest_model_path)

#flask app
app = Flask(__name__)


#ping test for health check
@app.route("/ping", methods=["GET"])
def healthz():
    return "OK", 200



# if testing is on, disable service bus clients
if TESTING != "1":
    sb_sender_client   = ServiceBusClient.from_connection_string(SB_SEND_CONN_STR)
    sb_receiver_client = ServiceBusClient.from_connection_string(SB_LISTEN_CONN_STR)


#prediction function
def run_prediction(categories):

    # ensure ints
    values = [int(v) for v in categories]
    n_feats = model.n_features_in_
    
    # pad or reject
    if len(values) < n_feats:
        values += [0] * (n_feats - len(values))
    elif len(values) > n_feats:
        raise ValueError(f"Too many features: got {len(values)}, max is {n_feats}")
    logger.info(f"Input values: {values}")
    pred    = int(model.predict([values])[0])
    probas  = model.predict_proba([values])[0]
    classes = model.classes_

    ranking = sorted(
        [
            {"healthcareTech": int(c), "percentage": round(p * 100, 2)}
            for c, p in zip(classes, probas)
        ],
        key=lambda x: x["percentage"], reverse=True
    )
    logger.info(f"values {pred}")
    logger.info(f"values {ranking}")

    return {
        "recommended_healthcareTech": pred,
        "healthcareTech_ranking":     ranking
    }


# HTTP system for testing and demo. in deployment this will be replaced by service bus but it's here for the demo
@app.route("/get_advice", methods=["POST"])
def get_advice():
    # API-Key check
    provided_key = request.headers.get("X-API-KEY")
    if not EXPECTED_API_KEY or provided_key != EXPECTED_API_KEY:
        return jsonify({"error": "Unauthorized. Invalid or missing API key."}), 401

    payload = request.get_json(force=True)
    categories = payload.get("Categories")
    if not isinstance(categories, list):
        return jsonify({"error": "Please provide 'Categories' as a list of ints."}), 400

    try:
        result = run_prediction(categories)
        return jsonify(result)
    except Exception as e:
        return jsonify({"error": str(e)}), 500


#service bus application for deployment and messaging
def servicebus_worker():
    with sb_receiver_client, sb_sender_client:
        while True:
            with sb_receiver_client.get_queue_receiver(
                    queue_name=REQ_QUEUE, max_wait_time=5
                ) as receiver, sb_sender_client.get_queue_sender(
                    queue_name=RES_QUEUE
                ) as sender:

                for msg in receiver:
                    # 1) Auth: use correlation_id as API-KEY
                    provided_key = msg.correlation_id
                    if provided_key != EXPECTED_API_KEY:
                        receiver.dead_letter_message(
                            msg,
                            reason="Unauthorized: invalid API key",
                            error_description="Check correlation_id"
                        )
                        continue

                    # 2) Parse JSON from msg.body
                    try:
                        # msg.body may be a bytes generator
                        body_bytes = b"".join(msg.body)
                        payload = json.loads(body_bytes.decode("utf-8"))
                    except Exception as e:
                        receiver.dead_letter_message(
                            msg,
                            reason="Invalid JSON",
                            error_description=str(e)
                        )
                        continue

                    # 3) Validate Categories field
                    categories = payload.get("Categories")
                    if (
                        not isinstance(categories, list)
                        or not categories
                        or not all(isinstance(x, int) for x in categories)
                    ):
                        receiver.dead_letter_message(
                            msg,
                            reason="Bad payload: 'Categories'",
                            error_description="Must be non-empty list of ints"
                        )
                        continue

                    # 4) Run prediction and reply
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


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)
