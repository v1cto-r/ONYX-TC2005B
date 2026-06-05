using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.Models.ViewModels;

namespace CosmicRunnerFront.Services
{
    public class InicioApiService : IInicioApiService
    {
        private readonly HttpClient _httpClient;
        
        public InicioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int userId)
        {
            var url = $"https://127.0.0.1:12001/api/user/{userId}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new Usuario();
                
            var usuario = await response.Content.ReadFromJsonAsync<Usuario>();
            return usuario ?? new Usuario();
        }

        public async Task<List<Idea>> GetIdeasAsync()
        {
            var url = "https://127.0.0.1:12001/api/ideas";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<Idea>();
                
            var listaIdeas = await response.Content.ReadFromJsonAsync<List<Idea>>();
            return listaIdeas ?? new List<Idea>();
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            var url = "https://127.0.0.1:12001/api/departamentos";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<Departamento>();
                
            var listaDepartamentos = await response.Content.ReadFromJsonAsync<List<Departamento>>();
            return listaDepartamentos ?? new List<Departamento>();
        }

        public async Task<List<AreaImpacto>> GetAreasImpactoAsync()
        {
            var url = "https://127.0.0.1:12001/api/areasimpacto";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<AreaImpacto>();
                
            var listaAreas = await response.Content.ReadFromJsonAsync<List<AreaImpacto>>();
            return listaAreas ?? new List<AreaImpacto>();
        }

        public async Task<bool> CrearIdeaAsync(FormularioIdeaViewModel nuevaIdea)
        {
            var url = "https://localhost:12001/api/idea";
            var response = await _httpClient.PostAsJsonAsync(url, nuevaIdea);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            return false;
        }
    }
}