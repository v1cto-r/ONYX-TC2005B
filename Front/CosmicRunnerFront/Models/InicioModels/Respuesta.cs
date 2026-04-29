namespace CosmicRunnerFront.Models.InicioModels
{
    // El símbolo ":" indica que hereda todos los atributos de Comentario
    public class Respuesta : Comentario 
    {
        // Solo necesitamos añadir lo que es exclusivo de una respuesta
        public int ComentarioPadreId { get; set; }
        public Comentario ComentarioPadre { get; set; }
    }
}