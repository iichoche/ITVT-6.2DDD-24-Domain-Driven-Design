import os

class Config:
    API_KEY = os.getenv("API_KEY")
    MODEL_DIR = "model"
    SERVICEBUS_SEND_CONN_STR   = os.getenv("SERVICEBUS_SEND_CONN_STR")
    SERVICEBUS_LISTEN_CONN_STR = os.getenv("SERVICEBUS_LISTEN_CONN_STR")
    REQUEST_QUEUE_NAME         = os.getenv("SERVICEBUS_REQUEST_QUEUE",  "request-queue")
    RESPONSE_QUEUE_NAME        = os.getenv("SERVICEBUS_RESPONSE_QUEUE", "response-queue")
