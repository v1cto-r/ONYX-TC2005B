from database.db import get_db_connection
from models.usuario import UsuarioInicio
from models.departamento import Departamento
from models.areaimpacto import AreaImpacto
from models.idea import IdeaFeed
import json

def obtener_perfil_usuario(user_id):
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)
    
    cursor.callproc("ObtenerPerfilUsuario", (user_id,))
    
    primer_query = next(cursor.stored_results())
    resultado = primer_query.fetchone()
    
    cursor.close()
    conexion.close()

    if resultado:
        return UsuarioInicio(
            user_id=resultado["user_id"],
            nombre=resultado["name"],
            apellido=resultado["lastname"],
            racha=resultado["streak"],
            foto_perfil=resultado["profile_picture_url"],
            top_global=resultado["global_ranking"],
            top_departamental=resultado["dept_ranking"]
        )
    return None

def obtener_departamentos():
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)
    cursor.execute("SELECT department_id, name FROM departments")
    resultados = cursor.fetchall()
    cursor.close()
    conexion.close()
    return [Departamento(r["department_id"], r["name"]) for r in resultados]

def obtener_areas_impacto():
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)
    cursor.execute("SELECT impact_area_id AS area_impacto_id, name FROM impact_areas")
    resultados = cursor.fetchall()
    cursor.close()
    conexion.close()
    return [AreaImpacto(r["area_impacto_id"], r["name"]) for r in resultados]


def obtener_ideas(user_id):
    conexion = get_db_connection()
    cursor = conexion.cursor(dictionary=True)
    
    # Pasamos el user_id al SP
    cursor.callproc("ObtenerTodasLasIdeas", (user_id,))
    
    primer_query = next(cursor.stored_results())
    resultados = primer_query.fetchall()

    cursor.close()
    conexion.close()

    lista_ideas = []
    for r in resultados:
        colaboradores_list = json.loads(r["colaboradores_json"])
        comentarios_list = json.loads(r["comentarios_json"])
        puntos_clave_list = json.loads(r["puntos_clave_json"]) 

        nombre_completo = f"{r['autor_nombre']} {r['autor_apellido']}"

        idea_obj = IdeaFeed(
            idea_id=r["idea_id"],
            titulo=r["titulo"],
            descripcion=r["descripcion"],
            dias_transcurridos=r["dias_transcurridos"],
            estado=r["estado"],
            likes=r["likes"],
            dislikes=r["dislikes"],
            cantidad_comentarios=r["cantidad_comentarios"],
            autor_id=r["autor_id"],
            autor_nombre=nombre_completo,
            autor_foto=r["autor_foto"],
            departamento=r["departamento"],
            area_impacto=r["area_impacto"],
            colaboradores=colaboradores_list,
            comentarios=comentarios_list,
            puntos_clave=puntos_clave_list,
            reaccion_usuario=r["reaccion_usuario"] 
        )
        lista_ideas.append(idea_obj)
    
    return lista_ideas



def ejecutar_sp_con_feedback(nombre_sp, parametros):
    try:
        conexion = get_db_connection()
        cursor = conexion.cursor(dictionary=True)
        
        cursor.callproc(nombre_sp, parametros)
        
        resultado = next(cursor.stored_results()).fetchone()

        conexion.commit() 
        
        cursor.close()
        conexion.close()
        
        return {"success": True, "data": resultado}
    except Exception as e:
        return {"success": False, "message": str(e)}


def crear_nueva_idea(titulo, descripcion, autor_id, depto_id, area_id, p1, p2, p3):
    parametros = (titulo, descripcion, autor_id, depto_id, area_id, p1, p2, p3)
    return ejecutar_sp_con_feedback("CrearIdea", parametros)

def unirse_a_proyecto(idea_id, user_id):
    return ejecutar_sp_con_feedback("UnirseProyecto", (idea_id, user_id))

def salir_de_proyecto(idea_id, user_id):
    return ejecutar_sp_con_feedback("SalirProyecto", (idea_id, user_id))

def gestionar_racha(user_id, accion):
    return ejecutar_sp_con_feedback("ActualizarRacha", (user_id, accion))

def reaccionar_idea(idea_id, user_id, es_like):
    return ejecutar_sp_con_feedback("ToggleReaccionIdea", (idea_id, user_id, es_like))

def reaccionar_comentario(comment_id, user_id, es_like):
    return ejecutar_sp_con_feedback("ToggleReaccionComentario", (comment_id, user_id, es_like))

def crear_comentario(idea_id, autor_id, mensaje):
    return ejecutar_sp_con_feedback("PublicarComentario", (idea_id, autor_id, mensaje))

def borrar_idea(idea_id):
    return ejecutar_sp_con_feedback("EliminarIdea", (idea_id,))

def borrar_comentario(comment_id):
    return ejecutar_sp_con_feedback("EliminarComentario", (comment_id,))
