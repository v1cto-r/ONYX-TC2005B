from flask import Blueprint, jsonify, request
import services.onyx_services as servicios

clasificacion_bp = Blueprint("clasificacion", __name__)

@clasificacion_bp.route("/clasificacion/global", methods=["GET"])
def get_clasificacion_sql1():
    # Pedimos los datos al servicio y dejamos que la ruta solo los empaquete.
    datos = servicios.SP1_ClasificacionGlobal()
    # jsonify transforma la lista o diccionario en respuesta JSON.
    return jsonify(datos)

@clasificacion_bp.route("/clasificacion/departamental/<string:p_departamento>", methods=["GET"])
def get_clasificacion_sql2(p_departamento):
    # Pedimos los datos al servicio y dejamos que la ruta solo los empaquete.
    datos = servicios.SP2_ClasificacionDepartamental(p_departamento)
    # jsonify transforma la lista o diccionario en respuesta JSON.
    return jsonify(datos)

@clasificacion_bp.route("/clasificacion/ataqueEstelar/<int:p_user_id>", methods=["GET"])
def get_clasificacion_sql3(p_user_id):
    # Pedimos los datos al servicio y dejamos que la ruta solo los empaquete.
    datos = servicios.SP3_RankAtequeEstelar(p_user_id)
    # jsonify transforma la lista o diccionario en respuesta JSON.
    return jsonify(datos)

@clasificacion_bp.route("/clasificacion/ataqueEstelar/agregar", methods=["POST"])
def post_ataque_estelar():
    try:
        # Leemos el cuerpo JSON que mando el cliente.
        data = request.get_json()
        # Sacamos cada campo usando las mismas llaves que espera la API.
        p_user_id = data["p_user_id"]
        p_credits = data["p_credits"]

        # Llamamos al servicio que hace el INSERT mediante stored procedure.
        servicios.SP_AgregarCreditos(p_user_id, p_credits)

        # Respondemos con 201 porque se creo un recurso nuevo.
        return jsonify({"mensaje": "¡puntos agregados exitosamente!"}), 201
    except Exception as e:
        # Si algo falla, devolvemos el mensaje del error para debug.
        return jsonify({"error2": str(e)}), 500
    
@clasificacion_bp.route("/clasificacion/ataqueEstelar/<string:p_username>/<string:p_password>", methods=["GET"])
def get_ataque_estelar(p_username, p_password):
    # Pedimos los datos al servicio y dejamos que la ruta solo los empaquete.
    datos = servicios.SP_Login(p_username, p_password)
    # jsonify transforma la lista o diccionario en respuesta JSON.
    return jsonify(datos)