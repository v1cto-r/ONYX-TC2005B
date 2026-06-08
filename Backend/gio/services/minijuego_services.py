from database.db import get_db_connection
from models.minijuego import Prompts

def obtener_parejas_prompts():
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)
    
    query = """
        SELECT 
            c.category AS categoria,
            c.difficulty AS dificultad,
            c.prompt_fragment AS prompt_correcto,
            i.prompt_fragment AS prompt_incorrecto
        FROM gio_minigame_catalog c
        JOIN gio_minigame_catalog i 
          ON c.category = i.category 
          AND c.difficulty = i.difficulty 
          AND i.is_correct = FALSE
        WHERE c.is_correct = TRUE
        ORDER BY RAND()
        LIMIT 10;
    """

    cursor.execute(query)
    resultados = cursor.fetchall()
    
    cursor.close()
    conexion.close()
    
    lista_parejas = []
    for r in resultados:
        pareja = Prompts(
            categoria=r["categoria"],
            dificultad=r["dificultad"],
            prompt_correcto=r["prompt_correcto"],
            prompt_incorrecto=r["prompt_incorrecto"]
        )
        lista_parejas.append(pareja)
        
    return lista_parejas