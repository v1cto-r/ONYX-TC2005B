using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Services.Prompts;

public interface IPromptService
{
	Task<(List<CategoryModel> Categories, List<DepartmentModel> Departments)> ObtenerOpcionesAsync();
	Task<List<PromptModel>?> ObtenerPromptAsync(int userId, string? searchText, int? categoryId, int? departmentId);
	Task<string> CrearPromptAsync(int userId, string promptTitle, string promptText, int categoryId, int departmentId);
	Task<string> CommentarPromptAsync(int promptId, int userId, string comment);
	Task<string> GuardarPromptAsync(int promptId, int userId);
	Task<string> CalificarPromptAsync(int promptId, int userId, int rating);
}
