from flask import Blueprint, request, jsonify
from services.login_services import login_user

login_bp = Blueprint("login", __name__)

@login_bp.route("/login", methods=["POST"])
def login():
  data = request.get_json()
  email = data["email"]
  password = data["password"]
  
  result = login_user(email, password)
  
  return jsonify(result[0])