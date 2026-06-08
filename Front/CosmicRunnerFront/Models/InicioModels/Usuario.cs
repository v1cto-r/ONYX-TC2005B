using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // <-- Asegúrate de agregar esta línea

namespace CosmicRunnerFront.Models.InicioModels
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es un campo obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es un campo obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un formato de correo válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El teléfono de contacto es obligatorio.")]
        public string Telefono { get; set; }

        public string Username { get; set; }
        public string Contrasena { get; set; }
        public string Biografia { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un tema de interfaz.")]
        public string Tema { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaIngreso { get; set; }

        [Required(ErrorMessage = "El puesto de trabajo es obligatorio.")]
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