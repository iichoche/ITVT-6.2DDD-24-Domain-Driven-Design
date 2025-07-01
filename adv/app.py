# app.py

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


if os.getenv("TESTING") != "1":
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


# old HTTP system
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


@app.route("/ping", methods=["GET"])
def healthz():
    return "OK", 200

#service bus version
def servicebus_worker():
    """
    Background thread that:
      1. Listens on REQ_QUEUE for incoming JSON messages
      2. Runs prediction
      3. Sends result to RES_QUEUE with the same correlation_id
    """
    with sb_receiver_client, sb_sender_client:
        receiver = sb_receiver_client.get_queue_receiver(
            queue_name=REQ_QUEUE, max_wait_time=5
        )
        sender = sb_sender_client.get_queue_sender(
            queue_name=RES_QUEUE
        )

        for msg in receiver:
            try:
                body = json.loads(str(msg))
                categories = body.get("Categories", [])
                result = run_prediction(categories)

                reply = ServiceBusMessage(
                    json.dumps(result),
                    correlation_id=msg.correlation_id
                )
                sender.send_messages(reply)
                receiver.complete_message(msg)

            except Exception as err:
                # move invalid or failed messages to the dead-letter queue
                receiver.dead_letter_message(msg, reason=str(err))


# start listener in a daemon thread
threading.Thread(target=servicebus_worker, daemon=True).start()


# ─── App Runner ───────────────────────────────────────────────────────────────
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)
