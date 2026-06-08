from database.db import get_db_connection

def SP1_ClasificacionGlobal():
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("SP1_ClasificacionGlobal")
    primerQuery = next(cursor.stored_results())
    resultados = primerQuery.fetchall()

    cursor.close()
    conexion.close()

    return resultados

def SP2_ClasificacionDepartamental(p_departamento):
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("SP2_ClasificacionDepartamental",[p_departamento])
    primerQuery = next(cursor.stored_results())
    resultados = primerQuery.fetchall()

    cursor.close()
    conexion.close()
    return resultados

def SP3_RankAtequeEstelar(p_user_id):
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("SP3_RankAtequeEstelar",[p_user_id])
    primerQuery = next(cursor.stored_results())
    resultados = primerQuery.fetchall()

    cursor.close()
    conexion.close()
    return resultados

def SP_AgregarCreditos(p_user_id,p_credits):
    # Abrimos conexion igual que en la consulta anterior.
    conexion = get_db_connection()
    # Seguimos usando diccionarios para que el JSON final salga mas limpio.
    cursor = conexion.cursor()

    # callproc ejecuta el procedimiento almacenado por su nombre.
    cursor.callproc("SP_AgregarCreditos",[p_user_id,p_credits])
    conexion.commit()

    # Cerramos cursor y conexion una vez ya tenemos todo en memoria.
    cursor.close()
    conexion.close()

def SP_Login(p_username, p_password):
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("Sp_Login", [p_username, p_password])
    primerQuery = next(cursor.stored_results())
    resultados = primerQuery.fetchall()

    cursor.close()
    conexion.close()

    return resultados