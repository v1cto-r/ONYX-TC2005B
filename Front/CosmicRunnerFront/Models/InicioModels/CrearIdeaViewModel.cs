using System.ComponentModel.DataAnnotations;

namespace CosmicRunnerFront.Models.InicioModels
{
    public class CrearIdeaViewModel
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [Display(Name = "Título del Proyecto")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Selecciona el departamento de origen.")]
        [Display(Name = "Departamento de Origen")]
        public string DepartamentoOrigen { get; set; }

        [Required(ErrorMessage = "Selecciona el área de impacto.")]
        [Display(Name = "Área de Impacto")]
        public string AreaImpacto { get; set; }

        [Required(ErrorMessage = "Proporciona al menos un punto clave.")]
        [Display(Name = "Puntos Clave")]
        public string PuntosClave { get; set; } 

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [Display(Name = "Descripción del Proyecto")]
        public string Descripcion { get; set; }

        // Campos opcionales según el mockup
        [Display(Name = "Cantidad de colaboradores necesarios")]
        public int? ColaboradoresNecesarios { get; set; }

        [Display(Name = "Departamento de colaboradores")]
        public string DepartamentoColaboradores { get; set; }
    }
}