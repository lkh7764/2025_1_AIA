from flask import Flask, request, jsonify
from dotenv import load_dotenv
import os, requests, logging

load_dotenv()

app = Flask(__name__)
logging.basicConfig(level=logging.DEBUG)

OLLAMA_API_URL  = "http://localhost:11436/api/generate"
MODEL_NAME      = "gemma3:latest"



# Persona vector JSON
import json

with open("Persona.json", "r", encoding="utf-8") as f:
    PERSONA = json.load(f)



# Evaluation
from Services.Evaluate import evaluate_picture_internal

@app.route("/evaluate_picture", methods=["POST"])
def evaluate_picture():
    payload = request.json
    result = evaluate_picture_internal(payload)
    return jsonify(result)



# LLM 
from Services.LLM import getRequestFromDeepSeek

@app.route("/generate_request", methods=["POST"])
def generate_request():
    data = request.get_json()
    theme = data.get("theme")
    if not theme:
        return jsonify({"error": "missing theme"}), 400
    
    print(theme)
    text = getRequestFromDeepSeek(theme)
    return jsonify({"request_text": text}), 200
    


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5001)