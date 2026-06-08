from flask import Flask, jsonify
from routes.ususario_routes import usuario_bp
from routes.minigame_routes import minigame_bp

app = Flask(__name__)
app.json.sort_keys = False

# End-Point base
@app.route("/")
def inicio():
    return jsonify({"mensaje": "API funcionando"})

# Obtener End-Points en el Blueprint
app.register_blueprint(usuario_bp)
app.register_blueprint(minigame_bp)

if __name__ == "__main__":
    # HTTPS (Uso de SSL)
    app.run(host="0.0.0.0", port=12003, ssl_context="adhoc", debug=True)
