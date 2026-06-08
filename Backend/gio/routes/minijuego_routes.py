from flask import Blueprint, request, jsonify
from services.minijuego_services import obtener_parejas_prompts

minijuego_bp = Blueprint("minijuego", __name__)

@minijuego_bp.route("/prompts", methods=["GET"])
def get_prompts_minijuego():    
    parejas_base = obtener_parejas_prompts()
    parejas_json = [pareja.to_dict() for pareja in parejas_base]
    
    return jsonify(parejas_json), 200