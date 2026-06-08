from flask import Blueprint, request, jsonify
from services.game_services import get_game_prompt, get_user_credits, add_user_credits

game_bp = Blueprint("game", __name__)

@game_bp.route("/game/prompt", methods=["GET"])
def game_prompt():
    result = get_game_prompt()
    return jsonify(result)

@game_bp.route("/game/credits", methods=["GET"])
def get_game_credits():
    try:
        user_id = int(request.args.get("user_id"))
    except (TypeError, ValueError):
        return jsonify({"Failed": "Missing parameters"}), 400

    result = get_user_credits(user_id)
    if result is None:
        return jsonify({"Failed": "User not found"}), 404
    return jsonify(result)

@game_bp.route("/game/credits", methods=["PATCH"])
def add_game_credits():
    data = request.get_json()
    user_id = data.get("user_id")
    credits = data.get("credits")

    if user_id is None or credits is None:
        return jsonify({"Failed": "Missing parameters"}), 400

    success = add_user_credits(user_id, credits)
    if success:
        return jsonify({"Status": "Success"})
    return jsonify({"Status": "Failed"}), 500