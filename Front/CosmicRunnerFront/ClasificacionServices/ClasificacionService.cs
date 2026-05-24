using System.Text.Json;
using System.Text.Json.Serialization;
using Front.Models;

namespace Front.Services
{
    public class ClasificacionService : IClasificacionService
    {
        private readonly HttpClient _httpClient;

        public ClasificacionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UsuarioRanking>> ObtenerClasificacionGlobal()
        {
            var url = "https://192.168.1.25:5000//clasificacion/global";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<UsuarioRanking>();

            var json = await response.Content.ReadAsStringAsync();

            var datos = JsonSerializer.Deserialize<List<ApiUsuario>>(json) ?? new();

            return datos.Select((u, i) => new UsuarioRanking
            {
                Id        = u.Id,
                Nombre     = u.Nombre,        
                Departamento = u.Departamento,
                Prompts    = u.Prompts,
                Puntaje    = u.Puntaje,
                Posicion   = u.Posicion
            }).ToList();
        }

        public async Task<List<UsuarioRanking>> ObtenerClasificacionDepartamental(string departamento)
        {
            var url = $"https://192.168.1.25:5000//clasificacion/departamental/{departamento}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<UsuarioRanking>();

            var json = await response.Content.ReadAsStringAsync();

            var datos = JsonSerializer.Deserialize<List<ApiUsuarioDepartamental>>(json) ?? new();

            return datos.Select(u => new UsuarioRanking
            {
                Id        = u.Id,
                Nombre     = u.Nombre, 
                Prompts    = u.Prompts,
                Puntaje    = u.Puntaje,
                Posicion   = u.Posicion
            }).ToList();
        }

        private class ApiUsuario
        {
            public int    Id          { get; set; }
            public string Nombre      { get; set; } = string.Empty;
            public int    Prompts     { get; set; }
            public int    Puntaje     { get; set; }
            public string Departamento { get; set; } = string.Empty;
            public int    Posicion    { get; set; }
        }

        private class ApiUsuarioDepartamental
        {
            public int    Id        { get; set; }
            public string Nombre    { get; set; } = string.Empty;
            public int    Prompts   { get; set; }
            public int    Puntaje   { get; set; }
            public int    Posicion  { get; set; }
        }
    }
}