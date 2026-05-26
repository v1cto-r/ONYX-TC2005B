using Front.Models;

namespace Front.Services
{
    public interface IClasificacionService
    {
        Task<List<UsuarioRanking>> ObtenerClasificacionGlobal();
        Task<List<UsuarioRanking>> ObtenerClasificacionDepartamental(string departamento);
    }
}