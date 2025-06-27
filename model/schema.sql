DROP TABLE IF EXISTS model_log;
CREATE TABLE model_log (
    model_id    INTEGER PRIMARY KEY,
    model_blob  BLOB    NOT NULL,
    viz_blob    BLOB    NOT NULL
);