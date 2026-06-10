using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CosmicRunnerFront.Models.InicioModels
{
    public class FormularioIdeaViewModel
    {
        [Required(ErrorMessage = "El titulo de la iniciativa es obligatorio.")]
        [MaxLength(255, ErrorMessage = "El título no puede exceder los 255 caracteres.")]
        public string titulo { get; set; }

        [Required(ErrorMessage = "La descripcion detallada es obligatoria.")]
        public string descripcion { get; set; }
        public int autor_id { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un departamento de origen.")]
        public int? departamento_id { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un area de impacto.")]
        public int? area_impacto_id { get; set; }

        [Required(ErrorMessage = "El punto clave 1 es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El punto clave debe ser breve (máx 150 caracteres).")]
        public string punto_1 { get; set; }
        [MaxLength(150, ErrorMessage = "El punto clave debe ser breve (máx 150 caracteres).")]
        public string punto_2 { get; set; }
        [MaxLength(150, ErrorMessage = "El punto clave debe ser breve (máx 150 caracteres).")]
        public string punto_3 { get; set; }

        public SelectList? CatalogoDepartamentos { get; set; }
        public SelectList? CatalogoAreasImpacto { get; set; }
    }
}