class Departamento:
    def __init__(self, id_dept, nombre):
        self.id_dept = id_dept
        self.nombre = nombre

    def to_dict(self):
        return {"department_id": self.id_dept, "name": self.nombre}