using System;
using System.Collections.Generic;

namespace CosmicRunnerFront.Models.InicioModels
{
    public class Idea
    {
        public int Id { get; set; }
        public string Titulo { get; set; } 
        public string Descripcion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        
        // Limitar a 3 puntos clave como sugeriste
        public List<string> PuntosClave { get; set; } = new List<string>(); 

        public EstadoIniciativa Estado { get; set; }
        
        public int Likes { get; set; }
        public int Dislikes { get; set; }

        // Llaves foráneas
        public int AutorId { get; set; }
        public Usuario Autor { get; set; }

        public int DepartamentoId { get; set; }
        public Departamento Departamento { get; set; }

        public int AreaImpactoId { get; set; }
        public AreaImpacto AreaImpacto { get; set; }

        // Relaciones
        public List<Usuario> ListaColaboradores { get; set; } = new List<Usuario>();
        public List<Comentario> ListaComentarios { get; set; } = new List<Comentario>();
        
        // Propiedad calculada para la vista (ej. "Hace 5 días")
        public int DiasTranscurridos => (DateTime.Now - FechaPublicacion).Days;
    }
}