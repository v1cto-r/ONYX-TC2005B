using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Services
{
    public class InicioApiService : IInicioApiService
    {
        private readonly HttpClient _httpClient;
        
        public InicioApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UsuarioUsr> GetUsuarioByIdAsync(int userId)
        {
            var url = $"https://gio.onyx.14082006.xyz/api/user/{userId}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new UsuarioUsr();
                
            var usuario = await response.Content.ReadFromJsonAsync<UsuarioUsr>();
            return usuario ?? new UsuarioUsr();
        }

        public async Task<List<Idea>> GetIdeasAsync(int userId)
        {
            var url = $"https://gio.onyx.14082006.xyz/api/ideas?user_id={userId}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<Idea>();
                
            var listaIdeas = await response.Content.ReadFromJsonAsync<List<Idea>>();
            return listaIdeas ?? new List<Idea>();
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            var url = "https://gio.onyx.14082006.xyz/api/departamentos";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<Departamento>();
                
            var listaDepartamentos = await response.Content.ReadFromJsonAsync<List<Departamento>>();
            return listaDepartamentos ?? new List<Departamento>();
        }

        public async Task<List<AreaImpacto>> GetAreasImpactoAsync()
        {
            var url = "https://gio.onyx.14082006.xyz/api/areasimpacto";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<AreaImpacto>();
                
            var listaAreas = await response.Content.ReadFromJsonAsync<List<AreaImpacto>>();
            return listaAreas ?? new List<AreaImpacto>();
        }

        public async Task<bool> CrearIdeaAsync(FormularioIdeaViewModel nuevaIdea)
        {
            var url = "https://gio.onyx.14082006.xyz/api/idea";
            var response = await _httpClient.PostAsJsonAsync(url, nuevaIdea);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            return false;
        }

        public async Task<bool> ReaccionarIdeaAsync(int ideaId, int userId, string tipo)
        {
            var url = $"https://gio.onyx.14082006.xyz/api/idea/{ideaId}/reaccion";
            var payload = new { user_id = userId, tipo = tipo };           
            var response = await _httpClient.PutAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> GuardarComentarioAsync(int ideaId, int userId, string mensaje)
        {
            // URL actualizada para coincidir con Python
            var url = $"https://gio.onyx.14082006.xyz/api/idea/{ideaId}/comentario";
            var payload = new { autor_id = userId, mensaje = mensaje }; 
            
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UnirseProyectoAsync(int ideaId, int userId)
        {
            // URL actualizada para coincidir con Python
            var url = $"https://gio.onyx.14082006.xyz/api/idea/{ideaId}/colaborador";
            var payload = new { user_id = userId };
            
            var response = await _httpClient.PostAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReaccionarComentarioAsync(int commentId, int userId, string tipo)
        {
            var url = $"https://gio.onyx.14082006.xyz/api/comentario/{commentId}/reaccion";
            var payload = new { user_id = userId, tipo = tipo };
            
            var response = await _httpClient.PutAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }
    }
}