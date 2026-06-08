from database.db import get_db_connection


# Stored Procedures


# GET: SP - ObtenerPalabrasMarinoMinigame
def obtener_palabras_marinominigame():
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("ObtenerPalabrasMarinoMinigame")

    primer_query = next(cursor.stored_results())
    resultados = primer_query.fetchall()

    cursor.close()
    conexion.close()

    return resultados


# GET: SP - ObtenerPromptsMarinoMinigame
def obtener_prompts_marinominigame():
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("ObtenerPromptsMarinoMinigame")

    primer_query = next(cursor.stored_results())
    resultados = primer_query.fetchall()

    cursor.close()
    conexion.close()

    return resultados


# POST: SP - AgregarCreditosMarinoMinigame
def agregar_creditos_marinominigame(user_id, credits_earned):
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("AgregarCreditosMarinoMinigame", [user_id, credits_earned])

    conexion.commit()

    cursor.close()
    conexion.close()

    return {"mensaje": "Creditos agregados correctamente"}