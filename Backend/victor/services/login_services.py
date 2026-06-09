import random
from database.database import get_db_connection

connection = get_db_connection()

def login_user(email, password):
    cursor = connection.cursor(dictionary=True)
    cursor.execute("SELECT user_id FROM users WHERE email = %s AND password_hash = %s LIMIT 1", [email, password])
    result = cursor.fetchall()
    cursor.close()
    
    if len(result) == 0:
        return None
    
    return result[0]