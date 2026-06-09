from database.conexion import obtener_conexion


def obtener_productos_tienda(usuario_id):
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    consulta_usuario = """
        SELECT user_id, username, credits
        FROM users
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
        FROM assets a
        INNER JOIN assets_type at ON a.asset_type_id = at.asset_type_id
        INNER JOIN jorge_store_assetdetails d ON a.asset_id = d.asset_id
        LEFT JOIN users_assets ua
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
            FROM users
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
            FROM assets a
            INNER JOIN jorge_store_assetdetails d ON a.asset_id = d.asset_id
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
            FROM users_assets
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
            UPDATE users
            SET credits = %s
            WHERE user_id = %s
        """

        insertar_asset_usuario = """
            INSERT INTO users_assets (user_id, asset_id, equipped)
            VALUES (%s, %s, FALSE)
        """

        insertar_transaccion = """
            INSERT INTO transactions (amount, user_id)
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


def obtener_customizacion_usuario(usuario_id):
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    consulta_usuario = """
        SELECT user_id, username, credits
        FROM users
        WHERE user_id = %s
    """

    cursor.execute(consulta_usuario, (usuario_id,))
    usuario = cursor.fetchone()

    if usuario is None:
        cursor.close()
        conexion.close()
        return None

    consulta_assets = """
        SELECT
            ua.asset_id,
            a.name,
            a.cost,
            at.name AS category,
            d.description,
            d.image_url,
            d.image_url_2,
            d.image_url_3,
            d.available,
            ua.equipped
        FROM users_assets ua
        INNER JOIN assets a ON ua.asset_id = a.asset_id
        INNER JOIN assets_type at ON a.asset_type_id = at.asset_type_id
        INNER JOIN jorge_store_assetdetails d ON a.asset_id = d.asset_id
        WHERE ua.user_id = %s
          AND at.name IN ('player_skins', 'icons')
          AND d.available = TRUE
        ORDER BY at.asset_type_id, a.asset_id
    """

    cursor.execute(consulta_assets, (usuario_id,))
    assets = cursor.fetchall()

    skins = []
    icons = []

    for asset in assets:
        asset["available"] = bool(asset["available"])
        asset["equipped"] = bool(asset["equipped"])

        imagenes = []

        if asset.get("image_url"):
            imagenes.append(asset["image_url"])

        if asset.get("image_url_2"):
            imagenes.append(asset["image_url_2"])

        if asset.get("image_url_3"):
            imagenes.append(asset["image_url_3"])

        asset["image_urls"] = imagenes

        if asset["category"] == "player_skins":
            skins.append(asset)
        elif asset["category"] == "icons":
            icons.append(asset)

    cursor.close()
    conexion.close()

    return {
        "exito": True,
        "usuario": usuario,
        "skins": skins,
        "icons": icons
    }


def equipar_asset_usuario(usuario_id, asset_id):
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    try:
        consulta_usuario = """
            SELECT user_id, username
            FROM users
            WHERE user_id = %s
        """

        cursor.execute(consulta_usuario, (usuario_id,))
        usuario = cursor.fetchone()

        if usuario is None:
            return {
                "exito": False,
                "mensaje": "No se encontro el usuario"
            }, 404

        consulta_asset = """
            SELECT
                a.asset_id,
                a.name,
                at.name AS category,
                d.image_url,
                d.image_url_2,
                d.image_url_3,
                d.available
            FROM assets a
            INNER JOIN assets_type at ON a.asset_type_id = at.asset_type_id
            INNER JOIN jorge_store_assetdetails d ON a.asset_id = d.asset_id
            WHERE a.asset_id = %s
        """

        cursor.execute(consulta_asset, (asset_id,))
        asset = cursor.fetchone()

        if asset is None:
            return {
                "exito": False,
                "mensaje": "No se encontro el asset"
            }, 404

        if asset["category"] not in ("player_skins", "icons"):
            return {
                "exito": False,
                "mensaje": "Este asset no se puede equipar"
            }, 409

        if not bool(asset["available"]):
            return {
                "exito": False,
                "mensaje": "Este asset no esta disponible"
            }, 409

        consulta_compra = """
            SELECT user_id, asset_id
            FROM users_assets
            WHERE user_id = %s AND asset_id = %s
        """

        cursor.execute(consulta_compra, (usuario_id, asset_id))
        compra = cursor.fetchone()

        if compra is None:
            return {
                "exito": False,
                "mensaje": "El usuario no ha comprado este asset"
            }, 409

        quitar_equipados = """
            UPDATE users_assets ua
            INNER JOIN assets a ON ua.asset_id = a.asset_id
            INNER JOIN assets_type at ON a.asset_type_id = at.asset_type_id
            SET ua.equipped = FALSE
            WHERE ua.user_id = %s
              AND at.name = %s
        """

        equipar_nuevo = """
            UPDATE users_assets
            SET equipped = TRUE
            WHERE user_id = %s
              AND asset_id = %s
        """

        cursor.execute(quitar_equipados, (usuario_id, asset["category"]))
        cursor.execute(equipar_nuevo, (usuario_id, asset_id))

        conexion.commit()

        asset["available"] = bool(asset["available"])
        asset["equipped"] = True

        imagenes = []

        if asset.get("image_url"):
            imagenes.append(asset["image_url"])

        if asset.get("image_url_2"):
            imagenes.append(asset["image_url_2"])

        if asset.get("image_url_3"):
            imagenes.append(asset["image_url_3"])

        asset["image_urls"] = imagenes

        return {
            "exito": True,
            "mensaje": "Asset equipado correctamente",
            "asset": asset
        }, 200

    except Exception as error:
        conexion.rollback()

        return {
            "exito": False,
            "mensaje": "Ocurrio un error al equipar el asset",
            "detalle": str(error)
        }, 500

    finally:
        cursor.close()
        conexion.close()