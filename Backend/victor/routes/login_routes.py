from flask import Blueprint, request, jsonify
from services.login_services import login_user

login_bp = Blueprint("login", __name__)

@login_bp.route("/login", methods=["POST"])
def login():
  data = request.get_json()
  email = data["email"]
  password = data["password"]
  
  result = login_user(email, password)
  
  if result is None:
    return jsonify({"message": "Login failed"}), 400
  
  return jsonify(result)