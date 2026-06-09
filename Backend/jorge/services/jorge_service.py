from database.conexion import obtener_conexion


def obtener_pregunta_aleatoria():
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    consulta_pregunta = """
        SELECT question_id, question_text
        FROM jorge_questions
        WHERE active = TRUE
        ORDER BY RAND()
        LIMIT 1
    """

    cursor.execute(consulta_pregunta)
    pregunta = cursor.fetchone()

    if pregunta is None:
        cursor.close()
        conexion.close()
        return None

    consulta_respuestas = """
        SELECT answer_id, answer_text, is_correct
        FROM jorge_answers
        WHERE question_id = %s
        ORDER BY RAND()
    """

    cursor.execute(consulta_respuestas, (pregunta["question_id"],))
    respuestas = cursor.fetchall()

    for respuesta in respuestas:
        respuesta["is_correct"] = bool(respuesta["is_correct"])

    cursor.close()
    conexion.close()

    return {
        "exito": True,
        "pregunta": {
            "question_id": pregunta["question_id"],
            "question_text": pregunta["question_text"],
            "answers": respuestas
        }
    }


# Creditos minigame
def agregar_creditos_jorge_minigame(user_id, credits_earned):
    conexion = obtener_conexion()
    cursor = conexion.cursor(dictionary=True)

    cursor.callproc("AgregarCreditosMarinoMinigame", [user_id, credits_earned])

    conexion.commit()

    cursor.close()
    conexion.close()

    return {
        "mensaje": "Creditos agregados correctamente",
        "user_id": user_id,
        "credits_earned": credits_earned
    }