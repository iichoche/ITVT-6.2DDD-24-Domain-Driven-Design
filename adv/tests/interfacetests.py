import unittest
import json
from unittest.mock import patch
from app import app
# interface tests to check correct and incorrect usage.
class InterfaceTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        self.test_api_key = "testkey"
        self.headers = {
            "Content-Type": "application/json",
            "X-API-KEY": self.test_api_key
        }
    #test invalid type (in this case a string)
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_invalid_categories_type(self):
        payload = {"Categories": "invalid_type"}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 400)
        self.assertIn("error", response.get_json())
    #test a valid request but the wrong name 
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_missing_categories_field(self):
        payload = {"NotCategory": [7,11]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 400)
        self.assertIn("error", response.get_json())
        
    #test a valid request but there is a string that are uncovertable into a int
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_wrong_categories_field(self):
        payload = {"Categories": [7,"eleven"]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 500)
        self.assertIn("error", response.get_json())

        
    #test a valid request but there is a string in there with numbers, the code should still work as it converts it to an int.
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_string_categories_field(self):
        payload = {"Categories": [7,"11"]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)
        data = response.get_json()
        self.assertIn("recommended_healthcareTech", data)
        self.assertIn("healthcareTech_ranking", data)

    #added too many features.
    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_too_many_features(self):
        payload = {"Categories": [7,11,23,14,2,3]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 500)
        self.assertIn("error", response.get_json())

if __name__ == "__main__":
    unittest.main()
