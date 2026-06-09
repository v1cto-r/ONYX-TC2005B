from flask import Flask, jsonify
from flask_cors import CORS
from routes.inicio_routes import inicio_bp
from routes.minijuego_routes import minijuego_bp

app = Flask(__name__)
CORS(app)
app.json.sort_keys = False

@app.route("/")
def inicio():
    return jsonify({"mensaje": "API del proyecto iniciada correctamente"})

app.register_blueprint(inicio_bp, url_prefix='/api')
app.register_blueprint(minijuego_bp, url_prefix='/api')

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=12001, ssl_context="adhoc", debug=True)