using System.Net.Http.Json;
using System.Text.Json;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.Models.UsuarioModels;
using UsuarioEntity = CosmicRunnerFront.Models.InicioModels.Usuario;

namespace CosmicRunnerFront.Services.Usuario;

public class UsuarioService : IUsuarioService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions ApiJsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DictionaryKeyPolicy = null
    };

    public UsuarioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UsuarioViewModel?> ObtenerPerfilAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"/usuario/perfil/{usuarioId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        var root = document.RootElement;
        var perfil = GetProperty(root, "perfil");
        var promptsRecientes = GetProperty(root, "prompts_recientes");

        if (perfil.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return null;
        }

        var usuario = new UsuarioEntity
        {
            Id = GetInt(perfil, "UsuarioId", "Id", "usuario_id"),
            Nombre = GetString(perfil, "Nombre"),
            Apellido = GetString(perfil, "Apellido"),
            Correo = GetString(perfil, "Correo"),
            Telefono = GetString(perfil, "Telefono"),
            Username = GetString(perfil, "Username"),
            Contrasena = GetString(perfil, "Contrasena"),
            Biografia = GetString(perfil, "Biografia"),
            Tema = GetString(perfil, "Tema"),
            Puesto = GetString(perfil, "Puesto"),
            Ubicacion = GetString(perfil, "Ubicacion"),
            FotoPerfilUrl = GetString(perfil, "FotoPerfilUrl"),
            DepartamentoId = GetInt(perfil, "DepartamentoId"),
            FechaNacimiento = GetDateTime(perfil, "FechaNacimiento"),
            FechaIngreso = GetDateTime(perfil, "FechaIngreso"),
            Racha = GetInt(perfil, "Racha", "Streak"),
            CreditosTiendita = GetInt(perfil, "CreditosTiendita"),
            PosicionGlobalPorcentaje = GetInt(perfil, "PosicionGlobalPorcentaje"),
            PosicionDepartamentalPorcentaje = GetInt(perfil, "PosicionDepartamentalPorcentaje"),
            Habilidades = GetStringList(perfil, "Habilidades")
        };

        var departamentoNombre = GetString(perfil, "Departamento", "DepartamentoNombre", "DepartamentoName");
        if (string.IsNullOrWhiteSpace(departamentoNombre))
        {
            departamentoNombre = GetString(perfil, "department_name");
        }

        if (!string.IsNullOrWhiteSpace(departamentoNombre) || usuario.DepartamentoId > 0)
        {
            usuario.Departamento = new Departamento
            {
                Id = usuario.DepartamentoId,
                Nombre = departamentoNombre
            };
        }

        var promptModels = new List<PromptModel>();
        if (promptsRecientes.ValueKind == JsonValueKind.Array)
        {
            foreach (var promptElement in promptsRecientes.EnumerateArray())
            {
                var promptId = GetInt(promptElement, "Id", "id", "prompt_id", "promptId");
                var promptCategory = GetString(promptElement, "Categoria", "categoria", "promptCategory", "category");
                var promptDescription = GetString(promptElement, "PromptTexto", "prompt", "text", "description", "descripcion");

                promptModels.Add(new PromptModel
                {
                    promptId = promptId,
                    promptUserId = GetInt(promptElement, "promptUserId", "usuario_id", "user_id"),
                    promptCategoryId = GetInt(promptElement, "promptCategoryId", "category_id"),
                    promptDepartmentId = GetInt(promptElement, "promptDepartmentId", "department_id"),
                    promptCategory = promptCategory,
                    promptDepartment = GetString(promptElement, "promptDepartment", "department", "departamento"),
                    promptTitle = BuildPromptTitle(promptId, promptCategory),
                    promptDescription = promptDescription,
                    promptCreatedAt = GetDateTime(promptElement, "FechaPublicacion", "fecha_publicacion", "promptCreatedAt", "created_at"),
                    Likes = GetInt(promptElement, "Score", "score", "Likes", "likes")
                });
            }
        }

        var viewModel = new UsuarioViewModel
        {
            Usuario = usuario,
            Habilidades = usuario.Habilidades,
            PromptsRecientes = promptModels,
            ActividadGeneral = new List<ActividadMetrica>
            {
                new() { Etiqueta = "Proyectos", Valor = GetCountAsString(perfil, "ProyectosCount", "ListaProyectos", "lista_proyectos") },
                new() { Etiqueta = "Prompts", Valor = GetCountAsString(perfil, "PromptsCount", "ListaPrompts", "lista_prompts") },
                new() { Etiqueta = "Comentarios", Valor = GetCountAsString(perfil, "ComentariosCount", "ListaComentarios", "lista_comentarios") }
            },
            ActividadesRecientes = BuildRecentActivities(promptModels),
            Contactos = new List<Contacto>
            {
                new() { Etiqueta = "Teléfono", Valor = usuario.Telefono, IconoSvg = "~/assets/icons/phone.svg" },
                new() { Etiqueta = "Correo", Valor = usuario.Correo, IconoSvg = "~/assets/icons/mail.svg" },
                new() { Etiqueta = "Ubicación", Valor = usuario.Ubicacion, IconoSvg = "~/assets/icons/map-pin.svg" },
                new() { Etiqueta = "Fecha de ingreso", Valor = usuario.FechaIngreso == default ? string.Empty : usuario.FechaIngreso.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-MX")), IconoSvg = "~/assets/icons/calendar-days.svg" }
            }
        };

        return viewModel;
    }

    public async Task<bool> ActualizarPerfilAsync(UsuarioViewModel usuarioViewModel, CancellationToken cancellationToken = default)
    {
        var usuario = usuarioViewModel.Usuario;

        var payload = new
        {
            usuario.Nombre,
            usuario.Apellido,
            usuario.Correo,
            usuario.Telefono,
            usuario.Puesto,
            usuario.DepartamentoId,
            usuario.Biografia,
            usuario.Ubicacion,
            usuario.FotoPerfilUrl,
            usuario.Tema
        };

        using var response = await _httpClient.PutAsJsonAsync("/usuario/perfil/" + usuario.Id, payload, ApiJsonOptions, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AgregarHabilidadAsync(int usuarioId, string habilidad, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            UsuarioId = usuarioId,
            Habilidad = habilidad
        };

        using var response = await _httpClient.PostAsJsonAsync("/usuario/habilidades", payload, ApiJsonOptions, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    private static JsonElement GetProperty(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            if (TryGetPropertyIgnoreCase(element, name, out var value))
            {
                return value;
            }
        }

        return default;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string GetString(JsonElement element, params string[] names)
    {
        var property = GetProperty(element, names);
        return property.ValueKind == JsonValueKind.String ? property.GetString() ?? string.Empty : property.ToString();
    }

    private static int GetInt(JsonElement element, params string[] names)
    {
        var property = GetProperty(element, names);
        if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var number))
        {
            return number;
        }

        return int.TryParse(property.ToString(), out var parsed) ? parsed : 0;
    }

    private static DateTime GetDateTime(JsonElement element, params string[] names)
    {
        var property = GetProperty(element, names);
        return DateTime.TryParse(property.ToString(), out var dateTime) ? dateTime : default;
    }

    private static List<string> GetStringList(JsonElement element, params string[] names)
    {
        var property = GetProperty(element, names);
        if (property.ValueKind == JsonValueKind.Array)
        {
            return property.EnumerateArray()
                .Select(item => item.ValueKind == JsonValueKind.String ? item.GetString() ?? string.Empty : item.ToString())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
        }

        var text = property.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        var separators = new[] { ',', '|', ';' };
        return text.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private static string GetCountAsString(JsonElement element, params string[] names)
    {
        var property = GetProperty(element, names);
        return property.ValueKind switch
        {
            JsonValueKind.Array => property.GetArrayLength().ToString(),
            JsonValueKind.Number when property.TryGetInt32(out var number) => number.ToString(),
            JsonValueKind.String when int.TryParse(property.GetString(), out var parsed) => parsed.ToString(),
            _ => "0"
        };
    }

    private static string BuildPromptTitle(int promptId, string promptCategory)
    {
        if (!string.IsNullOrWhiteSpace(promptCategory))
        {
            return promptCategory;
        }

        return promptId > 0 ? $"Prompt #{promptId}" : "Prompt";
    }

    private static List<ActividadReciente> BuildRecentActivities(IEnumerable<PromptModel> promptModels)
    {
        return promptModels
            .Take(5)
            .Select(prompt => new ActividadReciente
            {
                IconoSvg = "~/assets/icons/book.svg",
                Titulo = "Compartió un Prompt",
                Subtitulo = string.IsNullOrWhiteSpace(prompt.promptCategory)
                    ? "Categoría: Sin categoría"
                    : $"Categoría: {prompt.promptCategory}"
            })
            .ToList();
    }

    public async Task<List<HabilidadModel>> ObtenerHabilidadesDisponiblesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<HabilidadModel>>("/habilidades", ApiJsonOptions, cancellationToken);
            return response ?? new List<HabilidadModel>();
        }
        catch (Exception)
        {
            return new List<HabilidadModel>();
        }
    }
}