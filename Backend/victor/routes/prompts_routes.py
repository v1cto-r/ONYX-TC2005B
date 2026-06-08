from flask import Blueprint, request, jsonify
from services.prompts_services import add_prompt, get_prompts_and_comments, rate_prompt, add_comment, toggle_save

prompts_bp = Blueprint("prompts", __name__)

@prompts_bp.route("/prompts/full", methods=["GET"])
def get_prompts():
    try:
        user_id = int(request.args.get("user_id"))
    except (TypeError, ValueError):
        return jsonify({"Failed": "Mising parameters"}), 400

    search_text = request.args.get("search")


    try:
        department_id = int(request.args.get("department_id"))
    except (TypeError, ValueError):
        department_id = None

    try:
        category_id = int(request.args.get("category_id"))
    except (TypeError, ValueError):
        category_id = None

    result = get_prompts_and_comments(user_id, search_text, category_id, department_id)

    return jsonify(result)

@prompts_bp.route("/prompts", methods=["POST"])
def new_prompt():
    data = request.get_json()
    # user_id: int, prompt_title: str, prompt_text: str, category_id: int, department_id: int
    user_id = data["user_id"]
    prompt_title = data["prompt_title"]
    prompt_text = data["prompt_text"]
    category_id = data["category_id"]
    department_id = data["department_id"]

    success = add_prompt(user_id, prompt_title, prompt_text, category_id, department_id)

    if success:
        return jsonify({"Status": "Success"}), 201
    else :
        return jsonify({"Status": "Failed"}), 500

@prompts_bp.route("/prompts/<int:prompt_id>/rate", methods=["POST"])
def rate_prompt_endpoint(prompt_id: int):
    data = request.get_json()
    user_id = data.get("user_id")
    rating = data.get("rating")

    if user_id is None or rating is None:
        return jsonify({"Failed": "Missing parameters"}), 400

    success = rate_prompt(prompt_id, user_id, rating)
    if success:
        return jsonify({"Status": "Success"})
    return jsonify({"Status": "Failed"}), 500

@prompts_bp.route("/prompts/<int:prompt_id>/comment", methods=["POST"])
def comment_prompt(prompt_id: int):
    data = request.get_json()
    user_id = data.get("user_id")
    comment = data.get("comment")

    if user_id is None or comment is None:
        return jsonify({"Failed": "Missing parameters"}), 400

    success = add_comment(prompt_id, user_id, comment)
    if success:
        return jsonify({"Status": "Success"}), 201
    return jsonify({"Status": "Failed"}), 500

@prompts_bp.route("/prompts/<int:prompt_id>/save", methods=["POST"])
def save_prompt(prompt_id: int):
    data = request.get_json()
    user_id = data.get("user_id")

    if user_id is None:
        return jsonify({"Failed": "Missing parameters"}), 400

    action = toggle_save(prompt_id, user_id)
    if action:
        return jsonify({"Status": action})
    return jsonify({"Status": "Failed"}), 500