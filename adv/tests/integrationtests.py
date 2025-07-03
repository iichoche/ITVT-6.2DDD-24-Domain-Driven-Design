import unittest
import json
import os
import jwt

# Set env vars before importing app to ensure Config loads them
os.environ["TESTING"] = "1"
os.environ["JWT_SECRET"] = "super-secret-key"  # your JWT secret, if your app uses env var

from app import app

JWT_SECRET = "super-secret-key"
JWT_ALGORITHM = "HS256"

def generate_test_jwt():
    payload = {
        "sub": "testuser",
        "role": "admin"
    }
    token = jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)
    if isinstance(token, bytes):
        token = token.decode("utf-8")
    return token

class IntegrationTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        self.valid_token = generate_test_jwt()
        self.headers = {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {self.valid_token}"
        }

    def test_valid_chain_prediction_response(self):
        payload = {"Categories": [7, 11]} 
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)
        data = response.get_json()
        self.assertIn("recommended_healthcareTech", data)
        self.assertIn("healthcareTech_ranking", data)
        for item in data["healthcareTech_ranking"]:
            self.assertIn("healthcareTech", item)
            self.assertIn("percentage", item)
            self.assertIsInstance(item["percentage"], float)

if __name__ == "__main__":
    unittest.main()
