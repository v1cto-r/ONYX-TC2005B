from flask import Flask, jsonify
from flask_cors import CORS
from routes.onyx_routes import clasificacion_bp

app = Flask(__name__)
CORS(app)
app.json.sort_keys = False

@app.route("/")
def inicio():
    return jsonify({"mensaje": "API funcionando"})

app.register_blueprint(clasificacion_bp)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=12005, ssl_context="adhoc", debug=True)