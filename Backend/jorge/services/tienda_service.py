from database.conexion import obtener_conexion


def obtener_productos_tienda(usuario_id):
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    consulta_usuario = """
        SELECT user_id, username, credits
        FROM Users
        WHERE user_id = %s
    """

    cursor.execute(consulta_usuario, (usuario_id,))
    usuario = cursor.fetchone()

    if usuario is None:
        cursor.close()
        conexion.close()
        return None

    consulta_productos = """
        SELECT
            a.asset_id,
            a.name,
            a.cost,
            at.name AS category,
            d.description,
            d.image_url,
            d.image_url_2,
            d.image_url_3,
            d.available,
            CASE
                WHEN ua.asset_id IS NULL THEN FALSE
                ELSE TRUE
            END AS purchased
        FROM Assets a
        INNER JOIN Assets_Type at ON a.asset_type_id = at.asset_type_id
        INNER JOIN Jorge_Store_AssetDetails d ON a.asset_id = d.asset_id
        LEFT JOIN Users_Assets ua
            ON a.asset_id = ua.asset_id
            AND ua.user_id = %s
        WHERE d.available = TRUE
        ORDER BY a.asset_id
    """

    cursor.execute(consulta_productos, (usuario_id,))
    productos = cursor.fetchall()

    for producto in productos:
        producto["available"] = bool(producto["available"])
        producto["purchased"] = bool(producto["purchased"])

        imagenes = []

        if producto.get("image_url"):
            imagenes.append(producto["image_url"])

        if producto.get("image_url_2"):
            imagenes.append(producto["image_url_2"])

        if producto.get("image_url_3"):
            imagenes.append(producto["image_url_3"])

        producto["image_urls"] = imagenes

    cursor.close()
    conexion.close()

    return {
        "exito": True,
        "usuario": usuario,
        "productos": productos
    }


def comprar_producto(usuario_id, asset_id):
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    try:
        consulta_usuario = """
            SELECT user_id, username, credits
            FROM Users
            WHERE user_id = %s
        """

        cursor.execute(consulta_usuario, (usuario_id,))
        usuario = cursor.fetchone()

        if usuario is None:
            return {
                "exito": False,
                "mensaje": "No se encontro el usuario"
            }, 404

        consulta_producto = """
            SELECT
                a.asset_id,
                a.name,
                a.cost,
                d.available
            FROM Assets a
            INNER JOIN Jorge_Store_AssetDetails d ON a.asset_id = d.asset_id
            WHERE a.asset_id = %s
        """

        cursor.execute(consulta_producto, (asset_id,))
        producto = cursor.fetchone()

        if producto is None:
            return {
                "exito": False,
                "mensaje": "No se encontro el producto"
            }, 404

        if not bool(producto["available"]):
            return {
                "exito": False,
                "mensaje": "El producto no esta disponible"
            }, 409

        consulta_compra = """
            SELECT user_id, asset_id
            FROM Users_Assets
            WHERE user_id = %s AND asset_id = %s
        """

        cursor.execute(consulta_compra, (usuario_id, asset_id))
        compra_existente = cursor.fetchone()

        if compra_existente is not None:
            return {
                "exito": False,
                "mensaje": "El usuario ya compro este producto"
            }, 409

        if usuario["credits"] < producto["cost"]:
            return {
                "exito": False,
                "mensaje": "No tienes creditos suficientes para comprar este producto",
                "creditos_actuales": usuario["credits"],
                "costo_producto": producto["cost"]
            }, 409

        nuevos_creditos = usuario["credits"] - producto["cost"]

        actualizar_creditos = """
            UPDATE Users
            SET credits = %s
            WHERE user_id = %s
        """

        insertar_asset_usuario = """
            INSERT INTO Users_Assets (user_id, asset_id, equipped)
            VALUES (%s, %s, FALSE)
        """

        insertar_transaccion = """
            INSERT INTO Transactions (amount, user_id)
            VALUES (%s, %s)
        """

        cursor.execute(actualizar_creditos, (nuevos_creditos, usuario_id))
        cursor.execute(insertar_asset_usuario, (usuario_id, asset_id))
        cursor.execute(insertar_transaccion, (-producto["cost"], usuario_id))

        conexion.commit()

        return {
            "exito": True,
            "mensaje": "Compra realizada con exito",
            "creditos_restantes": nuevos_creditos,
            "producto": {
                "asset_id": producto["asset_id"],
                "name": producto["name"],
                "cost": producto["cost"]
            }
        }, 200

    except Exception as error:
        conexion.rollback()

        return {
            "exito": False,
            "mensaje": "Ocurrio un error al realizar la compra",
            "detalle": str(error)
        }, 500

    finally:
        cursor.close()
        conexion.close()