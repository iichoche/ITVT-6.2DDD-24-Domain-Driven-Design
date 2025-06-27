import os

class Config:
    API_KEY = os.getenv("API_KEY")
    MODEL_DIR = "model"
