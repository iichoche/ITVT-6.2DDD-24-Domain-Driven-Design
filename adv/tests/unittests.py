import unittest
import json
import jwt
from unittest.mock import patch
from app import app

# Constants for JWT
JWT_SECRET = "super-secret-key"
JWT_ALGORITHM = "HS256"

def generate_test_jwt():
    payload = {
        "sub": "testuser",
        "role": "admin"
    }
    token = jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)
    # jwt.encode returns bytes in PyJWT 2.x, so decode if needed
    if isinstance(token, bytes):
        token = token.decode("utf-8")
    return token

class GetAdviceTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        self.valid_token = generate_test_jwt()
        self.auth_header = {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {self.valid_token}"
        }

    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_get_advice_success(self):
        payload = {"Categories": [7, 11]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.auth_header
        )
        self.assertEqual(response.status_code, 200)
        data = response.get_json()
        self.assertIn("recommended_healthcareTech", data)
        self.assertIn("healthcareTech_ranking", data)

    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_get_advice_unauthorized_missing_token(self):
        payload = {"Categories": [7, 11]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers={"Content-Type": "application/json"}  # No Authorization header
        )
        self.assertEqual(response.status_code, 401)
        data = response.get_json()
        self.assertIn("error", data)

    @patch('app.JWT_SECRET', JWT_SECRET)
    def test_get_advice_unauthorized_invalid_token(self):
        payload = {"Categories": [7, 11]}
        invalid_auth_header = {
            "Content-Type": "application/json",
            "Authorization": "Bearer invalid.token.here"
        }
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=invalid_auth_header
        )
        self.assertEqual(response.status_code, 401)
        data = response.get_json()
        self.assertIn("error", data)

if __name__ == "__main__":
    unittest.main()
