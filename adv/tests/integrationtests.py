import unittest
import json
import os

# Set env vars before importing app to ensure Config loads them
os.environ["TESTING"] = "1"
os.environ["API_KEY"] = "testkey123"

from app import app

class IntegrationTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        self.headers = {
            "Content-Type": "application/json",
            "X-API-KEY": "testkey123"
        }

    def test_valid_chain_prediction_response(self):
        payload = {"Categories": [7, 11]} 
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        #check for 200
        self.assertEqual(response.status_code, 200)
        data = response.get_json()
                              
        # check if the right structure is returned
        for item in data["healthcareTech_ranking"]:
            self.assertIn("healthcareTech", item)
            self.assertIn("percentage", item)
            self.assertIsInstance(item["percentage"], float)

if __name__ == "__main__":
    unittest.main()
