from flask import Blueprint, jsonify, request
from services.jorge_service import obtener_pregunta_aleatoria, agregar_creditos_jorge_minigame

jorge_bp = Blueprint("jorge_bp", __name__)


@jorge_bp.route("/api/jorge/pregunta-aleatoria", methods=["GET"])
def buscar_pregunta_aleatoria():
    resultado = obtener_pregunta_aleatoria()

    if resultado is None:
        return jsonify({
            "exito": False,
            "mensaje": "No hay preguntas activas registradas"
        }), 404

    return jsonify(resultado), 200


@jorge_bp.route("/api/jorge/minigame/creditos", methods=["POST"])
def post_agregar_creditos_jorge_minigame():
    try:
        data = request.get_json() or {}

        usuario_id = data["UserId"]
        creditos_earned = data["CreditsEarned"]

        if creditos_earned <= 0:
            return jsonify({
                "error": "Los creditos ganados deben ser mayores a 0"
            }), 400

        resultado = agregar_creditos_jorge_minigame(usuario_id, creditos_earned)

        return jsonify(resultado), 201

    except Exception as e:
        return jsonify({"error": str(e)}), 500