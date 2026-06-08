using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Services.Prompts;

public class PromptApiResponse
{
    public string Status { get; set; } = string.Empty;
}

file class CategoryOption
{
    [JsonPropertyName("category_id")] public int CategoryId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

file class DepartmentOption
{
    [JsonPropertyName("department_id")] public int DepartmentId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

file class PromptOptionsResponse
{
    public List<CategoryOption> Categories { get; set; } = new();
    public List<DepartmentOption> Departments { get; set; } = new();
}

public class PromptService : IPromptService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private const string BaseUrl = "https://localhost:12002";

    public PromptService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(List<CategoryModel> Categories, List<DepartmentModel> Departments)> ObtenerOpcionesAsync()
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}/api/prompts/options");
        if (!response.IsSuccessStatusCode)
            return (new List<CategoryModel>(), new List<DepartmentModel>());

        var opts = await response.Content.ReadFromJsonAsync<PromptOptionsResponse>(_jsonOptions);
        if (opts is null)
            return (new List<CategoryModel>(), new List<DepartmentModel>());

        var categories = opts.Categories
            .Select(c => new CategoryModel { CategoryId = c.CategoryId, CategoryName = c.Name })
            .ToList();
        var departments = opts.Departments
            .Select(d => new DepartmentModel { DepartmentId = d.DepartmentId, DepartmentName = d.Name })
            .ToList();

        return (categories, departments);
    }

    public async Task<List<PromptModel>?> ObtenerPromptAsync(int userId, string? searchText, int? categoryId, int? departmentId)
    {
        var url = $"{BaseUrl}/api/prompts/full?user_id={userId}";

        if (!string.IsNullOrWhiteSpace(searchText))
            url += $"&search={Uri.EscapeDataString(searchText)}";
        if (categoryId.HasValue)
            url += $"&category_id={categoryId.Value}";
        if (departmentId.HasValue)
            url += $"&department_id={departmentId.Value}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PromptModel>>(json, _jsonOptions) ?? new List<PromptModel>();
    }

    public async Task<string> CrearPromptAsync(int userId, string promptTitle, string promptText, int categoryId, int departmentId)
    {
        var body = JsonSerializer.Serialize(new
        {
            user_id = userId,
            prompt_title = promptTitle,
            prompt_text = promptText,
            category_id = categoryId,
            department_id = departmentId
        });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/api/prompts", content);
        if (!response.IsSuccessStatusCode)
            return "Error al crear el prompt";
        var result = await response.Content.ReadFromJsonAsync<PromptApiResponse>(_jsonOptions);
        return result?.Status ?? "Error";
    }

    public async Task<string> CommentarPromptAsync(int promptId, int userId, string comment)
    {
        var body = JsonSerializer.Serialize(new { user_id = userId, comment });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/api/prompts/{promptId}/comment", content);
        if (!response.IsSuccessStatusCode)
            return "Error al agregar el comentario";
        var result = await response.Content.ReadFromJsonAsync<PromptApiResponse>(_jsonOptions);
        return result?.Status ?? "Error";
    }

    public async Task<string> GuardarPromptAsync(int promptId, int userId)
    {
        var body = JsonSerializer.Serialize(new { user_id = userId });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/api/prompts/{promptId}/save", content);
        if (!response.IsSuccessStatusCode)
            return "Error al guardar el prompt";
        var result = await response.Content.ReadFromJsonAsync<PromptApiResponse>(_jsonOptions);
        return result?.Status ?? "Error";
    }

    public async Task<string> CalificarPromptAsync(int promptId, int userId, int rating)
    {
        var body = JsonSerializer.Serialize(new { user_id = userId, rating });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/api/prompts/{promptId}/rate", content);
        if (!response.IsSuccessStatusCode)
            return "Error al calificar el prompt";
        var result = await response.Content.ReadFromJsonAsync<PromptApiResponse>(_jsonOptions);
        return result?.Status ?? "Error";
    }
}
