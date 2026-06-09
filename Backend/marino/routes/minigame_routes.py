from flask import Blueprint, request, jsonify

# Funciones Stored Procedures
from services.minigame_services import (
    obtener_palabras_marinominigame,
    obtener_prompts_marinominigame,
    agregar_creditos_marinominigame,
)

# Generar Blueprint (Conjunto de End-Points)
minigame_bp = Blueprint("minigame", __name__)


# End-Point para SP ObtenerPalabrasMarinoMinigame
@minigame_bp.route("/minigame/palabras", methods=["GET"])
def get_palabras_marinominigame():
    try:
        palabras = obtener_palabras_marinominigame()
        return jsonify(palabras)

    except Exception as e:
        return jsonify({"error": str(e)}), 500


# End-Point para SP ObtenerPromptsMarinoMinigame
@minigame_bp.route("/minigame/prompts", methods=["GET"])
def get_prompts_marinominigame():
    try:
        prompts = obtener_prompts_marinominigame()
        return jsonify(prompts)

    except Exception as e:
        return jsonify({"error": str(e)}), 500


# End-Point para SP AgregarCreditosMarinoMinigame
@minigame_bp.route("/minigame/creditos", methods=["POST"])
def post_agregar_creditos_marinominigame():
    try:
        data = request.get_json() or {}

        usuario_id = data["UserId"]
        creditos_earned = data["CreditsEarned"]

        mensaje = agregar_creditos_marinominigame(usuario_id, creditos_earned)

        return jsonify(mensaje), 201

    except Exception as e:
        print(str(e))
        return jsonify({"error": str(e)}), 500