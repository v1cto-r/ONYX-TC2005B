namespace CosmicRunnerFront.Models.InicioModels{
    public class Comentario
    {
        public int comment_id { get; set; }
        public string mensaje { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
        public string autor_nombre { get; set; }
        public string autor_foto { get; set; } 
        public int dias_transcurridos { get; set; } 
    }
}