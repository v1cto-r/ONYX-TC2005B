using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.UsuarioModels;
using CosmicRunnerFront.Services.Usuario;

namespace CosmicRunnerFront.Controllers;

public class UsuarioController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioService usuarioService)
    {
        _logger = logger;
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Index)));
    }

    public async Task<IActionResult> Actividades()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View("Actividades", await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Actividades)));
    }

    public async Task<IActionResult> Prompts()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View("Prompts", await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Prompts)));
    }

    public async Task<IActionResult> Configuracion()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View("Configuracion", await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Configuracion)));
    }

    [HttpPost]
    public async Task<IActionResult> GuardarConfiguracion(UsuarioViewModel usuarioViewModel, string? habilidadesTexto)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        // NOTA: Ya no hay validaciones manuales aquí. 
        // ASP.NET valida automáticamente los campos usando los [Required] de tu modelo Usuario.cs

        // Si faltan campos (el ModelState es inválido), recargar la vista mostrando los mensajes de error
        if (!ModelState.IsValid)
        {
            var modelConErrores = await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Configuracion));
            modelConErrores.Usuario = usuarioViewModel.Usuario; // Conserva lo que escribió el usuario
            modelConErrores.Habilidades = ParseHabilidades(habilidadesTexto); // Conserva las habilidades cargadas
            return View("Configuracion", modelConErrores);
        }

        if (usuarioViewModel.Usuario.Id == 0)
        {
            usuarioViewModel.Usuario.Id = currentUserId.Value;
        }

        // CONTROL DE CAÍDA DE API AL GUARDAR
        try
        {
            var success = await _usuarioService.ActualizarPerfilAsync(usuarioViewModel);
            if (!success)
            {
                TempData["ApiError"] = "Error de API"; // Envía el error a la vista
                var modelConErrores = await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Configuracion));
                modelConErrores.Usuario = usuarioViewModel.Usuario;
                return View("Configuracion", modelConErrores);
            }

            var habilidades = ParseHabilidades(habilidadesTexto);
            foreach (var habilidad in habilidades)
            {
                await _usuarioService.AgregarHabilidadAsync(usuarioViewModel.Usuario.Id, habilidad);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API no disponible o error al guardar");
            TempData["ApiError"] = "Error de API"; // Envía el error a la vista
            
            var modelConErrores = await BuildUsuarioViewModelAsync(currentUserId.Value, nameof(Configuracion));
            modelConErrores.Usuario = usuarioViewModel.Usuario;
            return View("Configuracion", modelConErrores);
        }

        return RedirectToAction(nameof(Configuracion));
    }

    private async Task<UsuarioViewModel> BuildUsuarioViewModelAsync(int currentUserId, string seccionActiva)
    {
        UsuarioViewModel? usuarioViewModel = null;

        // 1. OBTENER PERFIL
        try
        {
            usuarioViewModel = await _usuarioService.ObtenerPerfilAsync(currentUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API no disponible al obtener perfil");
        }

        // Si hubo error, cargar defaults
        if (usuarioViewModel == null)
        {
            TempData["ApiError"] = "Error de API"; 
            usuarioViewModel = new UsuarioViewModel
            {
                Usuario = new Models.InicioModels.Usuario { Id = currentUserId },
                Habilidades = new List<string>(),
                PromptsRecientes = new List<PromptModel>(),
                Contactos = new List<Contacto>(),
                ActividadGeneral = new List<ActividadMetrica>()
            };
        }

        // 2. OBTENER HABILIDADES DISPONIBLES (NUEVO)
        try
        {
            usuarioViewModel.HabilidadesDisponibles = await _usuarioService.ObtenerHabilidadesDisponiblesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la lista de habilidades disponibles");
            usuarioViewModel.HabilidadesDisponibles = new List<HabilidadModel>(); // Lista vacía de respaldo
        }

        EnsureUsuarioDefaults(usuarioViewModel);
        usuarioViewModel.SeccionActiva = seccionActiva;
        usuarioViewModel.Secciones = new List<Pestana>
        {
            new() { Etiqueta = "Información General", Accion = nameof(Index) },
            new() { Etiqueta = "Actividad Reciente", Accion = nameof(Actividades) },
            new() { Etiqueta = "Prompts", Accion = nameof(Prompts) },
            new() { Etiqueta = "Configuración", Accion = nameof(Configuracion) }
        };

        usuarioViewModel.EsPerfilPropio = usuarioViewModel.Usuario.Id == currentUserId;

        return usuarioViewModel;
    }

    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }

    private static void EnsureUsuarioDefaults(UsuarioViewModel usuarioViewModel)
    {
        var usuario = usuarioViewModel.Usuario;

        usuario.Nombre ??= string.Empty;
        usuario.Puesto ??= string.Empty;
        usuario.Telefono ??= string.Empty;
        usuario.Ubicacion ??= string.Empty;
        usuario.Correo ??= string.Empty;
        usuario.FotoPerfilUrl = string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl) ? "/assets/random/user.png" : usuario.FotoPerfilUrl;
        usuario.Biografia ??= string.Empty;
        usuario.Tema ??= "Claro";
        usuario.Habilidades ??= new List<string>();
        usuario.Departamento ??= new CosmicRunnerFront.Models.InicioModels.Departamento();

        if (string.IsNullOrWhiteSpace(usuario.Departamento.Nombre) && usuario.DepartamentoId > 0)
        {
            usuario.Departamento.Nombre = $"Departamento {usuario.DepartamentoId}";
        }

        usuarioViewModel.Contactos = new List<Contacto>
        {
            new() { Etiqueta = "Teléfono", Valor = usuario.Telefono, IconoSvg = "~/assets/icons/phone.svg" },
            new() { Etiqueta = "Correo", Valor = usuario.Correo, IconoSvg = "~/assets/icons/mail.svg" },
            new() { Etiqueta = "Ubicación", Valor = usuario.Ubicacion, IconoSvg = "~/assets/icons/map-pin.svg" },
            new() { Etiqueta = "Fecha de ingreso", Valor = usuario.FechaIngreso == default ? string.Empty : usuario.FechaIngreso.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-MX")), IconoSvg = "~/assets/icons/calendar-days.svg" }
        };

        if (usuarioViewModel.ActividadGeneral.Count == 0)
        {
            usuarioViewModel.ActividadGeneral = new List<ActividadMetrica>
            {
                new() { Etiqueta = "Proyectos", Valor = usuario.ListaProyectos?.Count.ToString() ?? "0" },
                new() { Etiqueta = "Prompts", Valor = usuario.ListaPrompts?.Count.ToString() ?? "0" },
                new() { Etiqueta = "Comentarios", Valor = usuario.ListaComentarios?.Count.ToString() ?? "0" }
            };
        }

        if (usuarioViewModel.ActividadesRecientes.Count == 0 && usuarioViewModel.PromptsRecientes.Count > 0)
        {
            usuarioViewModel.ActividadesRecientes = usuarioViewModel.PromptsRecientes
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
    }

    private static List<string> ParseHabilidades(string? habilidadesTexto)
    {
        if (string.IsNullOrWhiteSpace(habilidadesTexto))
        {
            return new List<string>();
        }

        return habilidadesTexto
            .Split(new[] { '|', ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(habilidad => !string.IsNullOrWhiteSpace(habilidad))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}