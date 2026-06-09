using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosmicRunnerFront.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Services.Login;

public class PromptApiResponse
{
    public int Id { get; set; } = -1;
}


public class LoginService : ILoginService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://victor.onyx.14082006.xyz/api";
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public LoginService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> LoginUser(string _correo, string _password)
    {
        var body = JsonSerializer.Serialize(new
        {
            email = _correo,
            password = _password
        });
        var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{BaseUrl}/login", content);
        if (!response.IsSuccessStatusCode) return -1;
        var result = await response.Content.ReadFromJsonAsync<PromptApiResponse>(_jsonOptions);
        return result?.Id ?? -1;
    }
}
