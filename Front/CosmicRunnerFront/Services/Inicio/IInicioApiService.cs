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
    }
}