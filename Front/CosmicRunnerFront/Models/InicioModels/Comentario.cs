using System;
using System.Collections.Generic;

namespace CosmicRunnerFront.Models.InicioModels
{
    public class Comentario
    {
        public int Id { get; set; }
        public int AutorId { get; set; }
        public Usuario Autor { get; set; }
        
        public int IdeaId { get; set; }
        public Idea IdeaAsociada { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string Mensaje { get; set; }
        
        public int Likes { get; set; }
        public int Dislikes { get; set; }

        // Relación con las respuestas
        public List<Respuesta> ListaRespuestas { get; set; } = new List<Respuesta>();
    }
}