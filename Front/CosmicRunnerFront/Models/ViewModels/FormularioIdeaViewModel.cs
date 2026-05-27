using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CosmicRunnerFront.Models.ViewModels
{
    public class FormularioIdeaViewModel
    {
        [Required(ErrorMessage = "El titulo de la iniciativa es obligatorio.")]
        public string titulo { get; set; }

        [Required(ErrorMessage = "La descripcion detallada es obligatoria.")]
        public string descripcion { get; set; }
        public int autor_id { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un departamento de origen.")]
        public int? departamento_id { get; set; }

        [Required(ErrorMessage = "Debes seleccionar un area de impacto.")]
        public int? area_impacto_id { get; set; }

        [Required(ErrorMessage = "El punto clave 1 es obligatorio.")]
        public string punto_1 { get; set; }
        public string punto_2 { get; set; }
        public string punto_3 { get; set; }

        public SelectList? CatalogoDepartamentos { get; set; }
        public SelectList? CatalogoAreasImpacto { get; set; }
    }
}