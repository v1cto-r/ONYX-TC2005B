using System.ComponentModel.DataAnnotations;

namespace Front.Models
{
    public class UsuarioRanking
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Departamento { get; set; } = string.Empty;

        public int Prompts { get; set; }

        public int Puntaje { get; set; }

        public int Posicion { get; set; }
        public string Picture { get; set; } = string.Empty;
    }
}