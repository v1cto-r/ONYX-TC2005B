from flask import Flask
from routes.tienda_routes import tienda_bp
from routes.jorge_routes import jorge_bp

app = Flask(__name__)

app.register_blueprint(tienda_bp)
app.register_blueprint(jorge_bp)

@app.route("/", methods=["GET"])
def inicio():
    return {
        "mensaje": "API Cosmic Runner Jorge funcionando en HTTPS"
    }

if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0', ssl_context="adhoc", port=12004)

