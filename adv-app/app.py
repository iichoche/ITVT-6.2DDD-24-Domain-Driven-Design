import os, json
from flask import (
    Flask, session, render_template,
    request, redirect, url_for, flash
)
import requests
from dotenv import load_dotenv

load_dotenv()
AZURE_URL = "https://acradvice20250622213824.azurewebsites.net/get_advice"

app = Flask(__name__)
app.secret_key = os.urandom(24)


@app.route("/", methods=["GET", "POST"])
def home():
    if request.method == "POST":
        jwt_token = request.form.get("jwt_token", "").strip()
        body_text = request.form.get("body", "").strip()

        # validate
        if not jwt_token:
            flash("JWT token is required.", "danger")
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
                    jwt_token=jwt_token,
                    body_text=body_text
                )

            # call Azure with Authorization header
            headers = {
                "Content-Type": "application/json",
                "Authorization": f"Bearer {jwt_token}"
            }
            try:
                resp = requests.post(
                    AZURE_URL,
                    json=payload,
                    headers=headers,
                    timeout=5
                )
            except requests.RequestException as e:
                flash(f"Request error: {e}", "danger")
                return render_template(
                    "input.html",
                    jwt_token=jwt_token,
                    body_text=body_text
                )

            # get the json body
            try:
                data = resp.json()
            except ValueError:
                data = {}

            if resp.status_code != 200:
                err_msg = data.get("error", resp.text)
                flash(f"API error: {err_msg}", "danger")
                return render_template(
                    "input.html",
                    jwt_token=jwt_token,
                    body_text=body_text
                )

            # success
            session["response"] = data
            return redirect(url_for("result"))

    # GET: prefill
    return render_template(
        "input.html",
        jwt_token=session.get("jwt_token", "")
    )


@app.route("/result")
def result():
    if "response" not in session:
        return redirect(url_for("home"))
    return render_template("result.html", result=session["response"])


if __name__ == "__main__":
    app.run(debug=True)
