class IdeaFeed:
    def __init__(self, idea_id, titulo, descripcion, dias_transcurridos, estado, 
                 likes, dislikes, cantidad_comentarios, autor_nombre, autor_foto, 
                 departamento, area_impacto, colaboradores, comentarios, puntos_clave, reaccion_usuario, autor_id):
        self.idea_id = idea_id
        self.titulo = titulo
        self.descripcion = descripcion
        self.dias_transcurridos = dias_transcurridos
        self.estado = estado
        self.likes = likes
        self.dislikes = dislikes
        self.cantidad_comentarios = cantidad_comentarios
        self.autor_id = autor_id
        self.autor_nombre = autor_nombre
        self.autor_foto = autor_foto
        self.departamento = departamento
        self.area_impacto = area_impacto
        self.colaboradores = colaboradores 
        self.comentarios = comentarios 
        self.puntos_clave = puntos_clave 
        self.reaccion_usuario = reaccion_usuario 

    def to_dict(self):
        return {
            "idea_id": self.idea_id,
            "titulo": self.titulo,
            "descripcion": self.descripcion,
            "dias_transcurridos": self.dias_transcurridos,
            "estado": self.estado,
            "metricas": {
                "likes": self.likes,
                "dislikes": self.dislikes,
                "cantidad_comentarios": self.cantidad_comentarios
            },
            "autor": {
                "autor_id": self.autor_id,
                "nombre": self.autor_nombre,
                "foto": self.autor_foto
            },
            "etiquetas": {
                "departamento": self.departamento,
                "area_impacto": self.area_impacto
            },
            "puntos_clave": self.puntos_clave, 
            "reaccion_usuario": self.reaccion_usuario,
            "colaboradores": self.colaboradores,
            "comentarios": self.comentarios
        }