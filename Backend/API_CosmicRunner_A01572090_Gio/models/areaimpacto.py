class AreaImpacto:
    def __init__(self, id_area, nombre):
        self.id_area = id_area
        self.nombre = nombre

    def to_dict(self):
        return {"area_impacto_id": self.id_area, "name": self.nombre}