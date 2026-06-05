using CosmicRunnerFront.Models.UsuarioModels;

namespace CosmicRunnerFront.Services.Usuario;

public interface IUsuarioService
{
	Task<UsuarioViewModel?> ObtenerPerfilAsync(int usuarioId, CancellationToken cancellationToken = default);
	Task<bool> ActualizarPerfilAsync(UsuarioViewModel usuarioViewModel, CancellationToken cancellationToken = default);
	Task<bool> AgregarHabilidadAsync(int usuarioId, string habilidad, CancellationToken cancellationToken = default);
}
