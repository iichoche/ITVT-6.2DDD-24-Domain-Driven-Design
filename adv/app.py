import os, json
from flask import (
    Flask, session, render_template,
    request, redirect, url_for, flash
)
import requests
from dotenv import load_dotenv

load_dotenv()
AZURE_URL = os.getenv("AZURE_URL")
if not AZURE_URL:
    raise RuntimeError("Set AZURE_URL in .env")

app = Flask(__name__)
app.secret_key = os.urandom(24)


@app.route("/", methods=["GET", "POST"])
def home():
    if request.method == "POST":
        api_key = request.form.get("api_key", "").strip()
        body_text = request.form.get("body", "").strip()

        # validate
        if not api_key:
            flash("API key is required.", "danger")
        elif not body_text:
            flash("JSON body is required.", "danger")
        else:
            # parse JSON
            try:
                payload = json.loads(body_text)
            except json.JSONDecodeError as e:
                flash(f"Invalid JSON: {e}", "danger")
                return render_template(
                    "input.html",
                    api_key=api_key,
                    body_text=body_text
                )

            # call Azure
            headers = {
                "Content-Type": "application/json",
                "X-API-KEY": api_key
            }
            try:
                resp = requests.post(
                    AZURE_URL,
                    json=payload,
                    headers=headers,
                    timeout=5
                )
                resp.raise_for_status()
            except requests.RequestException as e:
                flash(f"Error calling API: {e}", "danger")
                return render_template(
                    "input.html",
                    api_key=api_key,
                    body_text=body_text
                )

            # success → stash response and redirect
            session["api_key"]  = api_key
            session["payload"]  = payload
            session["response"] = resp.json()
            return redirect(url_for("result"))

    # GET: prefill
    sample = json.dumps({"Categories": [1, 0, 2, 5]}, indent=2)
    return render_template(
        "input.html",
        api_key=session.get("api_key", ""),
        body_text=session.get("body_text", sample)
    )


@app.route("/result")
def result():
    if "response" not in session:
        return redirect(url_for("home"))
    return render_template("result.html", result=session["response"])


if __name__ == "__main__":
    app.run(debug=True)