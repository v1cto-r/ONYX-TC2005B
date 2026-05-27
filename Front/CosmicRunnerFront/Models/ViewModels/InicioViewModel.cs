using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Models.ViewModels
{
    public class InicioViewModel
    {
        public Usuario UsuarioActual { get; set; } = new Usuario();
        public List<Idea> ListaIdeas { get; set; } = new List<Idea>();
        public FormularioIdeaViewModel NuevaIdea { get; set; } = new FormularioIdeaViewModel();
    }
}