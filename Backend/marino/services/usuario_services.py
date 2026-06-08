from database.db import get_db_connection


# Stored Procedures


# GET: SP - GetUsuarioPerfil
def obtener_perfil_usuario(usuario_id):
	conexion = get_db_connection()
	cursor = conexion.cursor(dictionary=True)

	cursor.callproc("GetUsuarioPerfil", [usuario_id])

	resultados = list(cursor.stored_results())

	perfil = resultados[0].fetchall() if len(resultados) > 0 else []
	prompts_recientes = resultados[1].fetchall() if len(resultados) > 1 else []

	cursor.close()
	conexion.close()

	return {
		"perfil": perfil[0] if perfil else None,
		"prompts_recientes": prompts_recientes,
	}


# PUT: SP - ActualizarUsuarioPerfil
def actualizar_usuario_perfil(
	usuario_id,
	nombre,
	apellido,
	correo,
	telefono,
	puesto,
	departamento_id,
	biografia,
	ubicacion,
	foto_perfil_url,
	tema,
):
	conexion = get_db_connection()
	cursor = conexion.cursor(dictionary=True)

	cursor.callproc(
		"ActualizarUsuarioPerfil",
		[
			usuario_id,
			nombre,
			apellido,
			correo,
			telefono,
			puesto,
			departamento_id,
			biografia,
			ubicacion,
			foto_perfil_url,
			tema,
		],
	)

	conexion.commit()

	cursor.close()
	conexion.close()

	return {"mensaje": "Perfil de usuario actualizado correctamente"}


# POST: SP - AgregarHabilidadUsuario
def agregar_habilidad_usuario(usuario_id, skill_name):
	conexion = get_db_connection()
	cursor = conexion.cursor(dictionary=True)

	cursor.callproc("AgregarHabilidadUsuario", [usuario_id, skill_name])

	conexion.commit()

	cursor.close()
	conexion.close()

	return {"mensaje": "Habilidad agregada correctamente"}

# GET: SP - ObtenerListaHabilidades
def obtener_lista_habilidades():
	conexion = get_db_connection()
	cursor = conexion.cursor(dictionary=True)

	cursor.callproc("ObtenerListaHabilidades")

	resultados = list(cursor.stored_results())
	habilidades = resultados[0].fetchall() if len(resultados) > 0 else []

	cursor.close()
	conexion.close()

	return habilidades
