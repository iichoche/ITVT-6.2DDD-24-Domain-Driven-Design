import pandas as pd
from sklearn.tree import DecisionTreeClassifier, plot_tree
from sklearn.metrics import accuracy_score
from sklearn.model_selection import train_test_split
import random
import json
import joblib
import matplotlib.pyplot as plt

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




def train_healthcare_model(json_file_path='model/healthcare_dataset.json', model_file_path='model/log/healthcare_model.pkl'):
    # Step 1: Read the dataset
    with open(json_file_path, 'r') as file:
        data = json.load(file)
    df = pd.DataFrame(data)
    print("Original DataFrame:")
    print(df.head())
    print(f"\nTotal records in the dataset: {len(df)}")

    
    target = 'healthcareTech'
    if target not in df.columns:
        raise ValueError(f"Target column '{target}' not found in dataset.")
    
    # Use all other columns as features
    feature_cols = [col for col in df.columns if col != target]
    X = df[feature_cols]
    y = df[target]
    X = df[feature_cols].fillna(0)

    # Create a train-test split (80% training, 20% testing)
    X_train, X_test, y_train, y_test = train_test_split(
        X, y, test_size=0.2, random_state=42
    )
    
    model = DecisionTreeClassifier(random_state=42, max_depth=4)
    model.fit(X_train, y_train)
    
    # Evaluate the model on both training and test sets
    train_predictions = model.predict(X_train)
    test_predictions = model.predict(X_test)
    
    train_accuracy = accuracy_score(y_train, train_predictions)
    test_accuracy = accuracy_score(y_test, test_predictions)
    
    print(f"\nTraining Accuracy: {train_accuracy:.4f}")
    print(f"Test Accuracy: {test_accuracy:.4f}")
    
    joblib.dump(model, model_file_path)
    print(f"\nDecision Tree model trained and saved to '{model_file_path}'.")
    
    plt.figure(figsize=(20, 10))
    class_names = [str(label) for label in sorted(y.unique())]
    
    plot_tree(model, feature_names=feature_cols, class_names=class_names, filled=True)
    plt.savefig("model/log/decision_tree_visualization.png")
    plt.close()  
    print("Decision tree visualization saved as 'decision_tree_visualization.png'.")

if __name__ == "__main__":
    generate_dataset()
    train_healthcare_model()
