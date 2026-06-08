class Prompts:
    def __init__(self, categoria, dificultad, prompt_correcto, prompt_incorrecto):
        self.categoria = categoria
        self.dificultad = dificultad
        self.prompt_correcto = prompt_correcto
        self.prompt_incorrecto = prompt_incorrecto

    def to_dict(self):
        return {
            "categoria": self.categoria,
            "dificultad": self.dificultad,
            "prompt_correcto": self.prompt_correcto,
            "prompt_incorrecto": self.prompt_incorrecto
        }