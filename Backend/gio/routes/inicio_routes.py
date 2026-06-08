from flask import Blueprint, request, jsonify
from services.inicio_services import (obtener_perfil_usuario, obtener_departamentos, 
    obtener_areas_impacto, obtener_ideas, crear_nueva_idea, unirse_a_proyecto, salir_de_proyecto, gestionar_racha,
    reaccionar_idea, reaccionar_comentario, crear_comentario, borrar_comentario, borrar_idea
    )
inicio_bp = Blueprint("inicio", __name__)

@inicio_bp.route("/user/<int:user_id>", methods=["GET"])
def get_user_profile(user_id):
    usuario = obtener_perfil_usuario(user_id)
    if usuario:
        return usuario.to_dict(), 200
    return {"error": "Usuario no encontrado"}, 404


@inicio_bp.route("/departamentos", methods=["GET"])
def get_departamentos():
    lista = obtener_departamentos()
    return [item.to_dict() for item in lista], 200

@inicio_bp.route("/areasimpacto", methods=["GET"])
def get_areas_impacto():
    lista = obtener_areas_impacto()
    return [item.to_dict() for item in lista], 200

@inicio_bp.route("/ideas", methods=["GET"])
def get_todas_las_ideas():
    user_id = request.args.get("user_id", type=int) 
    
    ideas_base = obtener_ideas(user_id)
    ideas_json = [idea.to_dict() for idea in ideas_base]
        
    return jsonify(ideas_json), 200

@inicio_bp.route("/idea", methods=["POST"])
def post_idea():
    data = request.get_json()
    
    titulo = data.get("titulo")
    descripcion = data.get("descripcion")
    autor_id = data.get("autor_id")
    depto_id = data.get("departamento_id")
    area_id = data.get("area_impacto_id")
    p1 = data.get("punto_1", "")
    p2 = data.get("punto_2", "")
    p3 = data.get("punto_3", "")

    resultado = crear_nueva_idea(titulo, descripcion, autor_id, depto_id, area_id, p1, p2, p3)
    
    if resultado["success"]:
        return jsonify(resultado["data"]), 201
    return jsonify({"error": "No se pudo publicar la idea", "detalle": resultado["message"]}), 500


@inicio_bp.route("/idea/<int:idea_id>/colaborador", methods=["POST"])
def post_unirse_proyecto(idea_id):
    data = request.get_json()
    user_id = data.get("user_id")
    
    resultado = unirse_a_proyecto(idea_id, user_id)
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al unirse", "detalle": resultado["message"]}), 500


@inicio_bp.route("/idea/<int:idea_id>/colaborador", methods=["DELETE"])
def delete_salir_proyecto(idea_id):
    data = request.get_json()
    user_id = data.get("user_id")
    
    resultado = salir_de_proyecto(idea_id, user_id)
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al salir del proyecto", "detalle": resultado["message"]}), 500


@inicio_bp.route("/usuario/<int:user_id>/racha", methods=["PUT"])
def put_racha(user_id):
    data = request.get_json()
    accion = data.get("accion") 
    
    if accion not in [0, 1]:
        return jsonify({"error": "Acción inválida. Usa 1 para sumar o 0 para reiniciar."}), 400

    resultado = gestionar_racha(user_id, accion)
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al actualizar racha", "detalle": resultado["message"]}), 500



@inicio_bp.route("/idea/<int:idea_id>/reaccion", methods=["PUT"])
def put_reaccion_idea(idea_id):
    data = request.get_json()
    tipo = data.get("tipo")
    user_id = data.get("user_id")
    
    if not user_id:
        return jsonify({"error": "Se requiere el ID del usuario"}), 400
        
    if tipo not in ["like", "dislike"]:
        return jsonify({"error": "Tipo de reacción inválida"}), 400
        
    es_like = True if tipo == "like" else False
    resultado = reaccionar_idea(idea_id, user_id, es_like)
    
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al procesar reacción", "detalle": resultado["message"]}), 500


@inicio_bp.route("/comentario/<int:comment_id>/reaccion", methods=["PUT"])
def put_reaccion_comentario(comment_id):
    data = request.get_json()
    tipo = data.get("tipo")
    user_id = data.get("user_id") 
    
    if not user_id:
        return jsonify({"error": "Se requiere el ID del usuario"}), 400
        
    if tipo not in ["like", "dislike"]:
        return jsonify({"error": "Tipo de reacción inválida"}), 400
        
    es_like = True if tipo == "like" else False
    resultado = reaccionar_comentario(comment_id, user_id, es_like)
    
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al procesar reacción", "detalle": resultado["message"]}), 500


@inicio_bp.route("/idea/<int:idea_id>/comentario", methods=["POST"])
def post_comentario(idea_id):
    data = request.get_json()
    autor_id = data.get("autor_id")
    mensaje = data.get("mensaje")

    if not autor_id or not mensaje:
        return jsonify({"error": "Faltan datos requeridos (autor_id o mensaje)"}), 400
    
    resultado = crear_comentario(idea_id, autor_id, mensaje)
    
    if resultado["success"]:
        return jsonify(resultado["data"]), 201
    return jsonify({"error": "Error al publicar comentario", "detalle": resultado["message"]}), 500



@inicio_bp.route("/idea/<int:idea_id>", methods=["DELETE"])
def delete_idea(idea_id):
    resultado = borrar_idea(idea_id)
    
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al eliminar idea", "detalle": resultado["message"]}), 500


@inicio_bp.route("/comentario/<int:comment_id>", methods=["DELETE"])
def delete_comentario(comment_id):
    resultado = borrar_comentario(comment_id)
    
    if resultado["success"]:
        return jsonify(resultado["data"]), 200
    return jsonify({"error": "Error al eliminar comentario", "detalle": resultado["message"]}), 500