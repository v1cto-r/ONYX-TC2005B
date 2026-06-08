class UsuarioInicio:
    def __init__(self, user_id, nombre, apellido, racha, foto_perfil, top_global, top_departamental):
        self.user_id = user_id
        self.nombre = nombre
        self.apellido = apellido
        self.racha = racha
        self.foto_perfil = foto_perfil
        self.top_global = top_global
        self.top_departamental = top_departamental

    def to_dict(self):
        return {
            "user_id": self.user_id,
            "nombre_completo": f"{self.nombre} {self.apellido}",
            "racha_whirlpool": self.racha,
            "foto_perfil": self.foto_perfil,
            "top_global_pct": self.top_global,
            "top_departamental_pct": self.top_departamental
        }