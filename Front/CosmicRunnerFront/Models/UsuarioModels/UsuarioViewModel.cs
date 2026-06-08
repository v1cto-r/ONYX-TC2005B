using System.Globalization;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Models.UsuarioModels;

public class UsuarioViewModel
{
    public Usuario Usuario { get; set; } = new();
    public string SeccionActiva { get; set; } = string.Empty;
    public List<Pestana> Secciones { get; set; } = new();
    public List<Contacto> Contactos { get; set; } = new();
    public List<string> Habilidades { get; set; } = new();
    public List<ActividadMetrica> ActividadGeneral { get; set; } = new();
    public List<ActividadReciente> ActividadesRecientes { get; set; } = new();
    public List<PromptModel> PromptsRecientes { get; set; } = new();
    public bool EsPerfilPropio { get; set; }

    public string NombreCompleto => $"{Usuario.Nombre} {Usuario.Apellido}".Trim();
    public string Puesto => Usuario.Puesto;
    public string Correo => Usuario.Correo;
    public string Ubicacion => Usuario.Ubicacion;
    public string FotoPerfilUrl => Usuario.FotoPerfilUrl;
    public string Biografia => Usuario.Biografia;
    public string FechaIngresoTexto => Usuario.FechaIngreso.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("es-MX"));
    public string Departamento => Usuario.Departamento?.Nombre ?? string.Empty;
    public List<HabilidadModel> HabilidadesDisponibles { get; set; } = new();
    public string SeccionActivaEtiqueta => Secciones.FirstOrDefault(x => x.Accion == SeccionActiva)?.Etiqueta ?? SeccionActiva;
}