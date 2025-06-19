from flask import Flask, request, jsonify
from dotenv import load_dotenv
import os, requests, logging
from LLM import getRequestFromDeepSeek


load_dotenv()

app = Flask(__name__)
logging.basicConfig(level=logging.DEBUG)

OLLAMA_API_URL  = "http://localhost:11436/api/generate"
MODEL_NAME      = "gemma3:latest"
    

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