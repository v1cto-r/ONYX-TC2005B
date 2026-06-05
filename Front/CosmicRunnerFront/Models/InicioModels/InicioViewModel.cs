namespace CosmicRunnerFront.Models.InicioModels
{
    public class InicioViewModel
    {
        public UsuarioUsr UsuarioActual { get; set; } = new UsuarioUsr();
        public List<Idea> ListaIdeas { get; set; } = new List<Idea>();
        public FormularioIdeaViewModel NuevaIdea { get; set; } = new FormularioIdeaViewModel();
    }
}