using System.Collections.Generic;

namespace Front.Models.ViewModels
{
    public class ClasificacionViewModel
    {
        public List<UsuarioRanking> Global { get; set; } = new();
        public List<UsuarioRanking> Departamental { get; set; } = new();
        public List<string> Departamentos { get; set; } = new();

        public string DepartamentoSeleccionado { get; set; } = string.Empty;
        public string NombreBuscado { get; set; } = string.Empty;

        public string? Mensaje { get; set; }
        public string? ErrorNombre { get; set; }

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int ResultadosPorPagina { get; set; } = 5;
        public int TotalResultados { get; set; } = 0;
    }
}