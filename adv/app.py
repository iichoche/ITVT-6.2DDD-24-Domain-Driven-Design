from flask import Flask, request, jsonify
import joblib
import numpy as np
#from dotenv import load_dotenv (for local API testing)
import os
from config import Config

#load_dotenv()  #for local checking API_KEY
EXPECTED_API_KEY = Config.API_KEY
model_dir = Config.MODEL_DIR


model_files = [f for f in os.listdir(model_dir) if f.endswith(".pkl")]

# S ort files by modification time, newest first
model_files.sort(key=lambda x: os.path.getmtime(os.path.join(model_dir, x)), reverse=True)

if model_files:
    latest_model_path = os.path.join(model_dir, model_files[0])
    model = joblib.load(latest_model_path)
else:
    raise FileNotFoundError("No model files found in the model directory.")
app = Flask(__name__)    

@app.route("/get_advice", methods=["POST"])
def get_advice():
    # Check API key in headers
    provided_key = request.headers.get("X-API-KEY")
    if not EXPECTED_API_KEY or provided_key != EXPECTED_API_KEY:
        return jsonify({"error": "Unauthorized. Invalid or missing API key."}), 401
    data = request.get_json()
    categories = data.get("Categories")

    if not categories or not isinstance(categories, list):
        return jsonify({"error": "Please provide 'CareNeeds' as a list of values."}), 400

    try:
        values = [int(val) for val in categories]

        expected_num_features = model.n_features_in_
        if len(values) < expected_num_features:
            values += [0] * (expected_num_features - len(values))
        elif len(values) > expected_num_features:
            return jsonify({"error": f"Too many care needs. Max allowed is {expected_num_features}"}), 400

        prediction = model.predict([values])
        recommended = int(prediction[0])

        probas = model.predict_proba([values])[0]
        classes = model.classes_
        ranking = [
            {"healthcareTech": int(tech), "percentage": round(prob * 100, 2)}
            for tech, prob in zip(classes, probas)
        ]
        ranking_sorted = sorted(ranking, key=lambda x: x["percentage"], reverse=True)

        return jsonify({
            "recommended_healthcareTech": recommended,
            "healthcareTech_ranking": ranking_sorted
        })

    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route("/ping", methods=["GET"])
def healthz():
    return "OK", 200


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=80)