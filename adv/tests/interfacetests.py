import unittest
import json
import jwt
from unittest.mock import patch
from app import app

# Constants for JWT
JWT_SECRET = "test-key"
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

class InterfaceTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        self.valid_token = generate_test_jwt()
        self.auth_headers = {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {self.valid_token}"
        }
# test 1: invalid categories type, string
    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_invalid_categories_type(self):
        payload = {"Categories": "invalid_type"}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.auth_headers
        )
        self.assertEqual(response.status_code, 400)
        self.assertIn("error", response.get_json())
#test 2: incorrect name of categories field
    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_missing_categories_field(self):
        payload = {"NotCategory": [7,11]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.auth_headers
        )
        self.assertEqual(response.status_code, 400)
        self.assertIn("error", response.get_json())
#test 3: string in categories field, unconvertable to int
    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_wrong_categories_field(self):
        payload = {"Categories": [7,"eleven"]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.auth_headers
        )
        self.assertEqual(response.status_code, 500)
        self.assertIn("error", response.get_json())
#test 3: string in categories field, convertable to int
    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_string_categories_field(self):
        payload = {"Categories": [7,"11"]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.auth_headers
        )
        self.assertEqual(response.status_code, 200)
        data = response.get_json()
        self.assertIn("recommended_healthcareTech", data)
        self.assertIn("healthcareTech_ranking", data)
#test 4: too many features, more than 5
    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_too_many_features(self):
        payload = {"Categories": [7,11,23,14,2,3]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.auth_headers
        )
        self.assertEqual(response.status_code, 500)
        self.assertIn("error", response.get_json())

if __name__ == "__main__":
    unittest.main()
