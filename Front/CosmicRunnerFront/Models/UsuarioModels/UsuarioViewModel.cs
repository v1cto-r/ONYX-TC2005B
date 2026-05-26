using System.Globalization;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Models.UsuarioModels;

public class UsuarioViewModel
{
    public Usuario Usuario { get; set; } = new();
    public string SeccionActiva { get; set; } = string.Empty;
    public string TextoSeccionEnDesarrollo { get; set; } = "Sección en desarrollo.";
    public string TituloAcercaDeMi { get; set; } = "Acerca de Mí";
    public string TituloHabilidadesClave { get; set; } = "Habilidades clave";
    public string TituloActividadGeneral { get; set; } = "Actividad general";
    public string TituloUltimosMovimientos { get; set; } = "Últimos movimientos";
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
    public string SeccionActivaEtiqueta => Secciones.FirstOrDefault(x => x.Accion == SeccionActiva)?.Etiqueta ?? SeccionActiva;

    public class ActividadMetrica
    {
        public string Etiqueta { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }

    public class ActividadReciente
    {
        public string IconoSvg { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Subtitulo { get; set; } = string.Empty;
    }

    public class Contacto
    {
        public string Etiqueta { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string IconoSvg { get; set; } = string.Empty;
    }

    public class Pestana
    {
        public string Etiqueta { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
    }

}