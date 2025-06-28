import unittest
import json
from unittest.mock import patch
from app import app

class GetAdviceTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        #bypassing API check with hardcoded API key, as i set EXPECTED_API_KEY in app.py to the test api_key
        self.test_api_key = "testkey"
        self.headers = {
            "Content-Type": "application/json",
            "X-API-KEY": self.test_api_key
        }
    #test for a successful response with a valid API key
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_get_advice_success(self):
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

    #test for a missing API key response
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_get_advice_unauthorized(self):
        payload = {"Categories": [7, 11]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            #we don't add a header to inidcate no API key
            headers={"Content-Type": "application/json"}
        )
        self.assertEqual(response.status_code, 401)
        data = response.get_json()
        self.assertIn("error", data)

if __name__ == "__main__":
    unittest.main()
