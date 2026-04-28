using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CosmicRunnerFront.Models;

public class PromptsViewModel
{
    public List<PromptModel> Prompts { get; set; } = new List<PromptModel>();
    public SelectList? Categories { get; set; }
    public SelectList? Departments { get; set; }

    public string? SearchText { get; set; }
    public int? FilterCategoryId { get; set; }
    public int? FilterDepartmentId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string NewPromptTitle { get; set; }

    [Required(ErrorMessage = "El contenido es obligatorio.")]
    [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
    public string NewPromptDescription { get; set; }

    [Required(ErrorMessage = "Selecciona una categoría.")]
    public int? NewPromptCategoryId { get; set; }

    [Required(ErrorMessage = "Selecciona un departamento.")]
    public int? NewPromptDepartmentId { get; set; }
}