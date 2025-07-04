import os
import io
import json
import random
import sqlite3
import pickle
from datetime import datetime
import joblib
import pandas as pd
import matplotlib.pyplot as plt
import requests
from sklearn.tree import DecisionTreeClassifier, plot_tree
from sklearn.metrics import accuracy_score
from sklearn.model_selection import train_test_split

# because the stuff from implementation isn't ready, i am generating a mock dataset, 
# imagine in the future the healthare_dataset.json is the return i get from the azure service bus from immanuel
def generate_dataset(
    num_samples: int = 5000,
    output_path: str = "model/healthcare_dataset.json",
) -> list:
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    # Map each tech code to realistic care-need categories
    categories_mapping = {
        1: list(range(1, 31)),            # Kraamzorg
        2: [5, 10, 15, 20, 28],            # Rolstoel
        3: [3, 7, 11, 16, 22],             # Bed
        4: [8, 12, 17, 20, 25, 28],        # Traplift
        5: [7, 12, 16, 22, 30],            # Krukken
        6: [9, 14, 19, 25, 29],            # Brees
    }

    records = []
    for _ in range(num_samples):
        tech_code = random.randint(1, 6)
        possible_needs = categories_mapping[tech_code]
        chosen = sorted(random.sample(possible_needs, random.randint(1, min(5, len(possible_needs)))))
        rec = {"healthcareTech": tech_code}
        for idx, need in enumerate(chosen, start=1):
            rec[f"Category{idx}"] = need
        records.append(rec)

    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(records, f, indent=2, ensure_ascii=False)
    print(f"✔ Generated dataset with {len(records)} records → {output_path}")
    return records

# mockup for data collection, i added this so at least the idea is there 
# ideally for the future i want to use azure service bus to get the data
def fetch_dataset(
    endpoint: str = "http://localhost:5050/get_history",
    output_path: str = "model/healthcare_dataset.json",
    save_to_file: bool = False,
) -> list:
    try:
        resp = requests.get(endpoint, timeout=5)
        resp.raise_for_status()
        data = resp.json()
        if save_to_file:
            with open(output_path, "w", encoding="utf-8") as f:
                json.dump(data, f, indent=2, ensure_ascii=False)
            print(f"✔ Fetched dataset → {output_path}")
        return data

    except requests.RequestException as exc:
        print(f"Failed to fetch dataset from {endpoint!r}: {exc}")
        return []


def train_healthcare_model(
    json_path: str = "model/healthcare_dataset.json",
    sqlite_path: str = "model/models.sqlite",
):
    # Load data (again, data from immanuael not there, so am using the generated dataset)
    data = fetch_dataset(save_to_file=False)
    if not data:
        with open(json_path, "r", encoding="utf-8") as f:
            data = json.load(f)

    df = pd.DataFrame(data)
    if "healthcareTech" not in df.columns:
        raise ValueError("`healthcareTech` column missing in JSON data")

    # Preprocess
    X = df.drop(columns=["healthcareTech"]).fillna(0)
    y = df["healthcareTech"]
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42
    )

    # Train
    clf = DecisionTreeClassifier(max_depth=4, random_state=42)
    clf.fit(X_train, y_train)

    train_acc = accuracy_score(y_train, clf.predict(X_train))
    test_acc  = accuracy_score(y_test,  clf.predict(X_test))
    print(f"\n▶ Train accuracy: {train_acc:.4f}")
    print(f"▶ Test  accuracy: {test_acc:.4f}")

    # Serialize model
    model_blob = pickle.dumps(clf)

    # Build and serialize the tree visualization
    fig = plt.figure(figsize=(20, 10))
    plot_tree(
        clf,
        feature_names=X.columns,
        class_names=[str(c) for c in sorted(y.unique())],
        filled=True,
        rounded=True,
        fontsize=10,
    )
    footer = (
        f"Trained: {datetime.now().isoformat(sep=' ')}   "
        f"Train ACC: {train_acc:.3f}   Test ACC: {test_acc:.3f}"
    )
    fig.text(0.5, 0.01, footer, ha="center", va="bottom", fontsize=12, color="gray")

    buf = io.BytesIO()
    fig.savefig(buf, format="png", bbox_inches="tight")
    plt.close(fig)
    viz_blob = buf.getvalue()
    buf.close()

    # Store BLOBs in SQLite
    os.makedirs(os.path.dirname(sqlite_path), exist_ok=True)
    conn = sqlite3.connect(sqlite_path)
    cur  = conn.cursor()

    # Ensure table matches schema.sql
    cur.execute("""
    CREATE TABLE IF NOT EXISTS model_log (
      model_id   INTEGER PRIMARY KEY AUTOINCREMENT,
      model_blob BLOB    NOT NULL,
      viz_blob   BLOB    NOT NULL
    )
    """)
    cur.execute(
        "INSERT INTO model_log (model_blob, viz_blob) VALUES (?, ?)",
        (sqlite3.Binary(model_blob), sqlite3.Binary(viz_blob)),
    )
    conn.commit()
    conn.close()
    # logging the model in a dated log file
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
    # adding to dated log file
    viz_path = os.path.join(logdir, "decision_tree_visualization.png")
    fig.savefig(viz_path, bbox_inches='tight')
    plt.close(fig)

if __name__ == "__main__":
    generate_dataset()
    train_healthcare_model()
    