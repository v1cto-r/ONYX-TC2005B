using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Services
{
    public interface IInicioApiService
    {
        Task<UsuarioUsr> GetUsuarioByIdAsync(int userId);
        Task<List<Idea>> GetIdeasAsync();
        Task<List<Departamento>> GetDepartamentosAsync();
        Task<List<AreaImpacto>> GetAreasImpactoAsync();
        Task<bool> CrearIdeaAsync(FormularioIdeaViewModel nuevaIdea);
        Task<bool> ReaccionarIdeaAsync(int ideaId, int userId, string tipo);
        Task<bool> GuardarComentarioAsync(int ideaId, int userId, string mensaje);
        Task<bool> UnirseProyectoAsync(int ideaId, int userId);
        Task<bool> ReaccionarComentarioAsync(int commentId, int userId, string tipo);
    }
}