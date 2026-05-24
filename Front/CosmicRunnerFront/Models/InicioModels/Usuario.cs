using System;
using System.Collections.Generic;

namespace CosmicRunnerFront.Models.InicioModels
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Username { get; set; }
        public string Contrasena { get; set; }
        public string Biografia { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Puesto { get; set; }
        public string Ubicacion { get; set; }
        public string FotoPerfilUrl { get; set; } // Para mostrar el avatar
        
        // Gamificación (Racha, Posiciones, Créditos)
        public int Racha { get; set; }
        public int CreditosTiendita { get; set; }
        public int PosicionGlobalPorcentaje { get; set; }
        public int PosicionDepartamentalPorcentaje { get; set; }

        // Llave foránea
        public int DepartamentoId { get; set; }
        public Departamento Departamento { get; set; }

        // Relaciones
        public List<Idea> ListaProyectos { get; set; } = new List<Idea>();
        public List<Comentario> ListaComentarios { get; set; } = new List<Comentario>();
        public List<string> ListaPrompts { get; set; } = new List<string>();
        public List<string> Habilidades { get; set; } = new List<string>();
    }
}