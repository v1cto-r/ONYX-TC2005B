from flask import Blueprint, request, jsonify

# Funciones Stored Procedures
from services.usuario_services import (
	obtener_perfil_usuario,
	actualizar_usuario_perfil,
	agregar_habilidad_usuario,
	obtener_lista_habilidades
)

# Generar Blueprint (Conjunto de End-Points)
usuario_bp = Blueprint("usuario", __name__)


# End-Point para SP GetUsuarioPerfil
@usuario_bp.route("/usuario/perfil/<int:usuario_id>", methods=["GET"])
def get_usuario_perfil(usuario_id):
	try:
		perfil = obtener_perfil_usuario(usuario_id)

		if not perfil:
			return jsonify({"error": "Usuario no encontrado"}), 404

		return jsonify(perfil)

	except Exception as e:
		return jsonify({"error": str(e)}), 500


# End-Point para SP ActualizarUsuarioPerfil
@usuario_bp.route("/usuario/perfil/<int:usuario_id>", methods=["PUT"])
def put_usuario_perfil(usuario_id):
	try:
		data = request.get_json() or {}

		mensaje = actualizar_usuario_perfil(
			usuario_id,
			data.get("Nombre"),
			data.get("Apellido"),
			data.get("Correo"),
			data.get("Telefono"),
			data.get("Puesto"),
			data.get("DepartamentoId"),
			data.get("Biografia"),
			data.get("Ubicacion"),
			data.get("FotoPerfilUrl"),
			data.get("Tema"),
		)

		return jsonify(mensaje)

	except Exception as e:
		return jsonify({"error": str(e)}), 500


# End-Point para SP AgregarHabilidadUsuario
@usuario_bp.route("/usuario/habilidades", methods=["POST"])
def post_agregar_habilidad_usuario():
	try:
		data = request.get_json() or {}

		usuario_id = data["UsuarioId"]
		habilidad = data["Habilidad"]

		mensaje = agregar_habilidad_usuario(usuario_id, habilidad)

		return jsonify(mensaje), 201

	except Exception as e:
		return jsonify({"error": str(e)}), 500
	
# End-Point para SP ObtenerListaHabilidades
@usuario_bp.route("/habilidades", methods=["GET"])
def get_lista_habilidades():
	try:
		habilidades = obtener_lista_habilidades()
		return jsonify(habilidades)
	except Exception as e:
		return jsonify({"error": str(e)}), 500
