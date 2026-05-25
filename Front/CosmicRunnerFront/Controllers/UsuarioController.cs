using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.UsuarioModels;
using CosmicRunnerFront.Services.Usuario;

namespace CosmicRunnerFront.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioService _usuarioService;
    private const int CurrentUserId = 1;

    public UsuarioController(ILogger<UsuarioController> logger, IUsuarioService usuarioService)
    {
        _logger = logger;
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildUsuarioViewModelAsync(nameof(Index)));
    }

    public async Task<IActionResult> Actividades()
    {
        return View("Actividades", await BuildUsuarioViewModelAsync(nameof(Actividades)));
    }

    public async Task<IActionResult> Prompts()
    {
        return View("Prompts", await BuildUsuarioViewModelAsync(nameof(Prompts)));
    }

    public async Task<IActionResult> Configuracion()
    {
        return View("Configuracion", await BuildUsuarioViewModelAsync(nameof(Configuracion)));
    }

    [HttpPost]
    public async Task<IActionResult> GuardarConfiguracion(UsuarioViewModel usuarioViewModel, string? habilidadesTexto)
    {
        if (usuarioViewModel.Usuario.Id == 0)
        {
            usuarioViewModel.Usuario.Id = CurrentUserId;
        }

        var success = await _usuarioService.ActualizarPerfilAsync(usuarioViewModel);
        if (!success)
        {
            return StatusCode(502, new { mensaje = "No se pudo actualizar el perfil" });
        }

        var habilidades = ParseHabilidades(habilidadesTexto);
        foreach (var habilidad in habilidades)
        {
            await _usuarioService.AgregarHabilidadAsync(usuarioViewModel.Usuario.Id, habilidad);
        }

        return RedirectToAction(nameof(Configuracion));
    }

    private async Task<UsuarioViewModel> BuildUsuarioViewModelAsync(string seccionActiva)
    {
        var usuarioViewModel = await _usuarioService.ObtenerPerfilAsync(CurrentUserId) ?? new UsuarioViewModel
        {
            Usuario = new Models.InicioModels.Usuario { Id = CurrentUserId },
            Habilidades = new List<string>(),
            PromptsRecientes = new List<PromptModel>(),
            Contactos = new List<UsuarioViewModel.Contacto>(),
            ActividadGeneral = new List<UsuarioViewModel.ActividadMetrica>()
        };

        EnsureUsuarioDefaults(usuarioViewModel);
        usuarioViewModel.SeccionActiva = seccionActiva;
        usuarioViewModel.Secciones = new List<UsuarioViewModel.Pestana>
        {
            new() { Etiqueta = "Información General", Accion = nameof(Index) },
            new() { Etiqueta = "Actividad Reciente", Accion = nameof(Actividades) },
            new() { Etiqueta = "Prompts", Accion = nameof(Prompts) },
            new() { Etiqueta = "Configuración", Accion = nameof(Configuracion) }
        };

        usuarioViewModel.EsPerfilPropio = usuarioViewModel.Usuario.Id == CurrentUserId;

        return usuarioViewModel;
    }

    private static void EnsureUsuarioDefaults(UsuarioViewModel usuarioViewModel)
    {
        var usuario = usuarioViewModel.Usuario;

        if (string.IsNullOrWhiteSpace(usuario.Nombre))
        {
            usuario.Nombre = string.Empty;
        }

        usuario.Puesto = usuario.Puesto ?? string.Empty;
        usuario.Telefono = usuario.Telefono ?? string.Empty;
        usuario.Ubicacion = usuario.Ubicacion ?? string.Empty;
        usuario.Correo = usuario.Correo ?? string.Empty;
        usuario.FotoPerfilUrl = string.IsNullOrWhiteSpace(usuario.FotoPerfilUrl) ? "/assets/random/user.png" : usuario.FotoPerfilUrl;
        usuario.Biografia = usuario.Biografia ?? string.Empty;
        usuario.Tema = usuario.Tema ?? "Claro";
        usuario.Habilidades ??= new List<string>();
        usuario.Departamento ??= new CosmicRunnerFront.Models.InicioModels.Departamento();

        if (string.IsNullOrWhiteSpace(usuario.Departamento.Nombre) && usuario.DepartamentoId > 0)
        {
            usuario.Departamento.Nombre = $"Departamento {usuario.DepartamentoId}";
        }

        usuarioViewModel.Contactos = new List<UsuarioViewModel.Contacto>
        {
            new() { Etiqueta = "Teléfono", Valor = usuario.Telefono, IconoSvg = "~/assets/icons/phone.svg" },
            new() { Etiqueta = "Correo", Valor = usuario.Correo, IconoSvg = "~/assets/icons/mail.svg" },
            new() { Etiqueta = "Ubicación", Valor = usuario.Ubicacion, IconoSvg = "~/assets/icons/map-pin.svg" },
            new() { Etiqueta = "Fecha de ingreso", Valor = usuario.FechaIngreso == default ? string.Empty : usuario.FechaIngreso.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-MX")), IconoSvg = "~/assets/icons/calendar-days.svg" }
        };

        if (usuarioViewModel.ActividadGeneral.Count == 0)
        {
            usuarioViewModel.ActividadGeneral = new List<UsuarioViewModel.ActividadMetrica>
            {
                new() { Etiqueta = "Proyectos", Valor = usuario.ListaProyectos.Count.ToString() },
                new() { Etiqueta = "Prompts", Valor = usuario.ListaPrompts.Count.ToString() },
                new() { Etiqueta = "Comentarios", Valor = usuario.ListaComentarios.Count.ToString() }
            };
        }

        if (usuarioViewModel.ActividadesRecientes.Count == 0 && usuarioViewModel.PromptsRecientes.Count > 0)
        {
            usuarioViewModel.ActividadesRecientes = usuarioViewModel.PromptsRecientes
                .Select(prompt => new UsuarioViewModel.ActividadReciente
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
