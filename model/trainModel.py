import pandas as pd
from sklearn.tree import DecisionTreeClassifier, plot_tree
from sklearn.metrics import accuracy_score
from sklearn.model_selection import train_test_split
import random
import json
import joblib
import matplotlib.pyplot as plt
import json
import requests
from datetime import datetime
import os


def generate_dataset(num_samples=5000, output_path="model/healthcare_dataset.json"):
    records = []
    
    # Define a mapping of healthcare technology to a correlated list of possible care need numbers, this is to add realism to the dataset.
    categories_mapping = {
        1: list(range(1, 31)),            # Zorgtech 1: volledig random, in dit geval Kraamzorg
        2: [5, 10, 15, 20, 28],            # Zorgtech 2: Rolstoel
        3: [3, 7, 11, 16, 22],             # Zorgtech 3: Bed
        4: [8, 12, 17, 20, 25, 28],         # Zorgtech 4: Traplift
        5: [7, 12, 16, 22, 30],            # Zorgtech 5: Krukken
        6: [9, 14, 19, 25, 29]             # Zorgtech 6: brees
    }

    for _ in range(num_samples):
        tech_code = random.randint(1, 6)
        record = {"healthcareTech": tech_code}

        possible_needs = categories_mapping[tech_code]
        num_needs = random.randint(1, min(5, len(possible_needs)))
        chosen_needs = sorted(random.sample(possible_needs, num_needs))

        for i, need in enumerate(chosen_needs, start=1):
            record[f"Category{i}"] = need

        records.append(record)

    # Save the records directly to JSON, avoiding the use of a DataFrame.
    with open(output_path, "w") as f:
        json.dump(records, f, indent=2)
    
    print(f"Generated dataset saved to {output_path}")
    return records

# getting data from Implement database, however as that part isn't implemented yet, i still use the generated dataset for prod
def fetch_dataset(
    endpoint: str = "http://localhost:5050/get_history",
    output_path: str = "model/healthcare_dataset.json",
    save_to_file: bool = False
):
    try:
        resp = requests.get(endpoint, timeout=10)
        resp.raise_for_status()
        records = resp.json()
    except requests.RequestException as e:
        print(f"Failed to fetch dataset from {endpoint!r}: {e}")
        return []

    if save_to_file:
        try:
            with open(output_path, "w", encoding="utf-8") as f:
                json.dump(records, f, indent=2, ensure_ascii=False)
            print(f"Fetched dataset saved to {output_path}")
        except IOError as e:
            print(f"Couldn’t write to {output_path!r}: {e}")

    return records

def train_healthcare_model(json_file_path='model/healthcare_dataset.json'):
    # Step 1: get the dataset from the implement database, this doesn't work yet because immanuel isn't done yet
    data = fetch_dataset()  # Uncomment this line to fetch the dataset from the endpoint
    # as it's not don yet, ill get the dataset from the generated dataset
    # Step 1: Read the dataset from the file
    with open(json_file_path, 'r') as file:
        data = json.load(file)
    df = pd.DataFrame(data)

    # Step 2: preprocessing
    target = 'healthcareTech'
    if target not in df.columns:
        raise ValueError(f"Target column '{target}' not found in dataset.")
    feature_cols = [col for col in df.columns if col != target]
    X = df[feature_cols]
    y = df[target]
    X = df[feature_cols].fillna(0)

    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42
    )

    # Step 3: training the model
    clf = DecisionTreeClassifier(random_state=42, max_depth=4)
    clf.fit(X_train, y_train)

    train_acc = accuracy_score(y_train, clf.predict(X_train))
    test_acc  = accuracy_score(y_test,  clf.predict(X_test))

    # Step 4: logging the model in a dated log file
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    logdir    = os.path.join("model/log", f"Model-{timestamp}")
    os.makedirs(logdir, exist_ok=True)

    # add trained model to dated log file
    model_filename = f"healthcare_model_{timestamp}.pkl"
    model_path = os.path.join(logdir, model_filename)
    joblib.dump(clf, model_path)
    
    # visualisation of the model, adding the date and accuracy to the image
    fig = plt.figure(figsize=(20, 10))
    plot_tree(
        clf,
        feature_names=X.columns,
        class_names=[str(c) for c in sorted(y.unique())],
        filled=True, rounded=True, fontsize=10
    )
    # text
    footer = f"Date of training: {timestamp}    Train Accuracy: {train_acc:.4f}    Test Accuracy: {test_acc:.4f}"
    fig.text(0.5, 0.01, footer, ha='center', va='bottom',
             fontsize=12, color='gray')
    #adding to dated log file
    viz_path = os.path.join(logdir, "decision_tree_visualization.png")
    fig.savefig(viz_path, bbox_inches='tight')
    plt.close(fig)
    
    print(f" training completed, saved in:{viz_path}")
    print(f"\n▶ Training accuracy: {train_acc:.4f}")
    print(f"▶ Test     accuracy: {test_acc:.4f}")
    
if __name__ == "__main__":
    #get dataset, disabled as this is not implemented yet by immanuel
    #fetch_dataset()
    
    generate_dataset()
    train_healthcare_model()
