using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Services
{
    public interface IInicioApiService
    {
        Task<Usuario> GetUsuarioByIdAsync(int userId);
        Task<List<Idea>> GetIdeasAsync();
        Task<List<Departamento>> GetDepartamentosAsync();
        Task<List<AreaImpacto>> GetAreasImpactoAsync();
        Task<bool> CrearIdeaAsync(FormularioIdeaViewModel nuevaIdea);
    }
}