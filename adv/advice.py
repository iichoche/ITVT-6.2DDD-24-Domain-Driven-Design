from flask import Flask, request, jsonify
import joblib
import numpy as np
from dotenv import load_dotenv
import os

load_dotenv()  #for checking API_KEY
API_KEY = os.getenv("API_KEY")

# Load the trained model
model = joblib.load("healthcare_model.pkl")

app = Flask(__name__)



def verify_api_key(provided_key):
    if provided_key != API_KEY:
        return jsonify({"error": "Unauthorized. Invalid or missing API key."}), 401

@app.route("/get_advice", methods=["POST"])
def get_advice():
    # Check for API Key in headers
    provided_key = request.headers.get("SECRET_API_KEY")
    verify_api_key(provided_key)

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


if __name__ == "__main__":
    app.run(debug=True)