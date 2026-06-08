import random
from database.database import get_db_connection

connection = get_db_connection()

## Cantidad de prompts en la base de datos
AMOUNT_OF_PROMPTS = 10
pool = []

def next_prompt_id() -> int:
    global pool

    if not pool:
        pool = list(range(1, AMOUNT_OF_PROMPTS + 1))
        random.shuffle(pool)

    return pool.pop()


def get_game_prompt():
    cursor = connection.cursor(dictionary=True)
    cursor.execute("SELECT * FROM Ab_Prompt_Parts WHERE prompt = %s", [next_prompt_id()])
    result = cursor.fetchall()
    random.shuffle(result)
    cursor.close()
    return result


def get_user_credits(user_id: int):
    cursor = connection.cursor(dictionary=True)
    cursor.execute("SELECT credits FROM users WHERE user_id = %s", [user_id])
    result = cursor.fetchone()
    cursor.close()
    return result


def add_user_credits(user_id: int, credits: int):
    try:
        cursor = connection.cursor(dictionary=True)
        cursor.callproc("AgregarCreditos", [user_id, credits])
        connection.commit()
        cursor.close()
        return True
    except:
        return False