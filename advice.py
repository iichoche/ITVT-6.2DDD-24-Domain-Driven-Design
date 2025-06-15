from flask import Flask, request, jsonify
import joblib
import numpy as np

# Load the trained model
model = joblib.load("healthcare_model.pkl")

app = Flask(__name__)

@app.route("/get_advice", methods=["POST"])
def get_advice():
    data = request.get_json()
    categories = data.get("Categories")
    
    if not categories or not isinstance(categories, list):
        return jsonify({"error": "Please provide 'CareNeeds' as a list of values."}), 400

    try:
        values = [int(val) for val in categories]
        
        # Determine how many features the model was trained with.
        expected_num_features = model.n_features_in_
        print(expected_num_features)
        if len(values) < expected_num_features:
            values += [np.nan] * (expected_num_features - len(values))
            print(values)
        elif len(values) > expected_num_features:
            return jsonify({"error": f"Too many care needs given to generate a proper advice. Max amount of care needs it can make a advice for is {expected_num_features}"}), 400
            # Truncate the list to match the expected number of features if you still want a prediction, but it will be wrong
            #values = values[:expected_num_features]
            #print(values)
        
     # Get the recommended prediction.
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