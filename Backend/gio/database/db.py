import mysql.connector 

def get_db_connection():
    conexion = mysql.connector.connect(
        host="localhost",
        user="magio",
        password="GioMax102._.",
        database="cosmic_runner"
    )
    return conexion