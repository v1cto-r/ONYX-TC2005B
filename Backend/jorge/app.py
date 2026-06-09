from flask import Flask
from flask_cors import CORS
from routes.tienda_routes import tienda_bp
from routes.jorge_routes import jorge_bp

app = Flask(__name__)
CORS(app)
app.json.sort_keys = False

app.register_blueprint(tienda_bp)
app.register_blueprint(jorge_bp)

@app.route("/", methods=["GET"])
def inicio():
    return {
        "mensaje": "API Cosmic Runner Jorge funcionando en HTTPS"
    }

if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0', ssl_context="adhoc", port=12004)

