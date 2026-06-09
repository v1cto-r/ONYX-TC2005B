import mysql.connector
import os

def obtener_conexion():
    conexion = mysql.connector.connect(
        host=os.getenv("HOST"),
        user=os.getenv("USER"),
        password=os.getenv("PASSWORD"),
        database=os.getenv("DATABASE"),
        port=int(os.getenv("PORT"))
    )
    return conexion
