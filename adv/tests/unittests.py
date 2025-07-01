import unittest
import json
from unittest.mock import patch, MagicMock
from app import app


class GetAdviceTestCase(unittest.TestCase):
    def setUp(self):
        self.client = app.test_client()
        self.test_api_key = "testkey"
        self.headers = {
            "Content-Type": "application/json",
            "X-API-KEY": self.test_api_key
        }

    @patch('app.EXPECTED_API_KEY', new='testkey')
    @patch('app.model')
    def test_get_advice_success(self, mock_model):
        # Setup mock model
        mock_model.n_features_in_ = 2
        mock_model.predict.return_value = [1]
        mock_model.predict_proba.return_value = [[0.3, 0.7]]
        mock_model.classes_ = [0, 1]

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

    @patch('app.EXPECTED_API_KEY', new='testkey')
    def test_get_advice_unauthorized(self):
        payload = {"Categories": [7, 11]}
        response = self.client.post(
            "/get_advice",
            data=json.dumps(payload),
            headers={"Content-Type": "application/json"}  # no API key
        )
        self.assertEqual(response.status_code, 401)
        data = response.get_json()
        self.assertIn("error", data)


if __name__ == "__main__":
    unittest.main()
