using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.DataInicio;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.UsuarioModels;

namespace CosmicRunnerFront.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(ILogger<UsuarioController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(BuildUsuarioViewModel(nameof(Index)));
    }

    public IActionResult Actividades()
    {
        return View("Index", BuildUsuarioViewModel(nameof(Actividades)));
    }

    public IActionResult Prompts()
    {
        return View("Index", BuildUsuarioViewModel(nameof(Prompts)));
    }

    public IActionResult Configuracion()
    {
        return View("Index", BuildUsuarioViewModel(nameof(Configuracion)));
    }

    private static UsuarioViewModel BuildUsuarioViewModel(string seccionActiva)
    {
        var usuario = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1) ?? MockDatabase.Usuarios.First();
        var departamento = MockDatabase.Departamentos.FirstOrDefault(d => d.Id == usuario.DepartamentoId);

        if (departamento is not null)
        {
            usuario.Departamento = departamento;
        }

        usuario.Puesto = usuario.Puesto ?? "Ingeniero industrial";
        usuario.Ubicacion = usuario.Ubicacion ?? "Monterrey, Nuevo León";
        usuario.Correo = usuario.Correo ?? "usuario.temporal@whirlpool.com";
        usuario.FotoPerfilUrl = usuario.FotoPerfilUrl ?? "/assets/random/user.png";
        usuario.FechaIngreso = usuario.FechaIngreso == default ? new DateTime(2011, 4, 16) : usuario.FechaIngreso;
        usuario.Biografia = string.IsNullOrWhiteSpace(usuario.Biografia)
            ? "Director de calidad e ingeniero industrial con 15 años de trayectoria."
            : usuario.Biografia;

        return new UsuarioViewModel
        {
            Usuario = usuario,
            SeccionActiva = seccionActiva,
            Secciones = new List<UsuarioViewModel.Pestana>
            {
                new() { Etiqueta = "Información General", Accion = nameof(Index) },
                new() { Etiqueta = "Actividad reciente", Accion = nameof(Actividades) },
                new() { Etiqueta = "Prompts", Accion = nameof(Prompts) },
                new() { Etiqueta = "Configuración", Accion = nameof(Configuracion) }
            },
            Contactos = new List<UsuarioViewModel.Contacto>
            {
                new() { Etiqueta = "Teléfono", Valor = "+52 123 456 7890", IconoSvg = "~/assets/icons/phone.svg" },
                new() { Etiqueta = "Correo", Valor = usuario.Correo, IconoSvg = "~/assets/icons/mail.svg" },
                new() { Etiqueta = "Ubicación", Valor = usuario.Ubicacion, IconoSvg = "~/assets/icons/map-pin.svg" },
                new() { Etiqueta = "Fecha de ingreso", Valor = usuario.FechaIngreso.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-MX")), IconoSvg = "~/assets/icons/calendar-days.svg" }
            },
            Habilidades = new List<string>
            {
                "Gestión de calidad",
                "Liderazgo",
                "Planificación estratégica",
                "Eficiencia",
                "Control de riesgos"
            }
        };
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
