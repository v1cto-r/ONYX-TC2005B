from flask import Blueprint, jsonify, request
from services.tienda_service import obtener_productos_tienda, comprar_producto

tienda_bp = Blueprint("tienda_bp", __name__)

@tienda_bp.route("/api/tienda/productos/<int:usuario_id>", methods=["GET"])
def buscar_productos(usuario_id):
    resultado = obtener_productos_tienda(usuario_id)

    if resultado is None:
        return jsonify({
            "exito": False,
            "mensaje": "No se encontro el usuario"
        }), 404

    return jsonify(resultado), 200


@tienda_bp.route("/api/tienda/comprar", methods=["POST"])
def comprar():
    datos = request.get_json()

    if datos is None:
        return jsonify({
            "exito": False,
            "mensaje": "No se enviaron datos"
        }), 400

    usuario_id = datos.get("usuario_id")
    asset_id = datos.get("asset_id")

    if usuario_id is None or asset_id is None:
        return jsonify({
            "exito": False,
            "mensaje": "Falta usuario_id o asset_id"
        }), 400

    resultado, codigo = comprar_producto(usuario_id, asset_id)

    return jsonify(resultado), codigo
