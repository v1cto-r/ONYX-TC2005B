namespace CosmicRunnerFront.Models.InicioModels
{
    public class Idea
    {
        public int idea_id { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; } 
        public int dias_transcurridos { get; set; }
        public int estado { get; set; } 
        public MetricasIdea metricas { get; set; }
        public Participante autor { get; set; }
        public EtiquetasIdea etiquetas { get; set; } 
        public List<Participante> colaboradores { get; set; }
        public List<Comentario> comentarios { get; set; }
        public List<string> puntos_clave { get; set; }
    }
}