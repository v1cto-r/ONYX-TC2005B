import mysql.connector

# Configuracion de MySQL
def get_db_connection():
    conexion = mysql.connector.connect(
        host="localhost",
        user="root",
        password="rootroot",
        database="cosmic_runner"
    )
    return conexion