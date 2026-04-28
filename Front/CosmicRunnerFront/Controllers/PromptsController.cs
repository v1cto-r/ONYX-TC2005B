using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Controllers;

public class PromptsController : Controller
{
    private readonly ILogger<PromptsController> _logger;

    // Los valores mock de manera estatica, para simular una base de datos en memoria. Es solo de demo, se saca de la base de datos despues.
    private static List<CategoryModel> _dbCategories = new List<CategoryModel> { 
        new CategoryModel { CategoryId = 1, CategoryName = "Excel" }, 
        new CategoryModel { CategoryId = 2, CategoryName = "Resumen" } 
    };

    private static List<DepartmentModel> _dbDepartments = new List<DepartmentModel> { 
        new DepartmentModel { DepartmentId = 1, DepartmentName = "Innovación" }, 
        new DepartmentModel { DepartmentId = 2, DepartmentName = "Finanzas" } 
    };

    private static List<PromptModel> _mockPrompts = new List<PromptModel>
    {
        new PromptModel { promptId = 1, promptTitle = "Mi prompt de excel", promptDescription = "Actúa como un analista financiero especializado en la industria de electrodomésticos (Whirlpool). Necesito editar un archivo de Excel con datos financieros de la empresa. Guíame paso a paso para realizar las siguientes tareas, asumiendo que tengo un nivel intermedio en Excel. No inventes datos reales de Whirlpool, sino estructura y fórmulas genéricas que pueda aplicar a mi archivo", promptCategoryId = 1, promptCategory = "Excel", promptDepartmentId = 1, promptDepartment = "Innovación" },
        new PromptModel { promptId = 2, promptTitle = "Para generar un reporte mensual", promptDescription = "Actúa como un consultor senior en innovación corporativa. Trabajo en el departamento de innovación de una empresa de electrodomésticos (similar a Whirlpool). Necesito redactar un resumen administrativo (executive summary) de máximo una página para presentar a la dirección general. El resumen debe basarse en los siguientes elementos (puedes inventar datos realistas del sector, pero mantenlos coherentes): ...", promptCategoryId = 2, promptCategory = "Resumen", promptDepartmentId = 2, promptDepartment = "Finanzas" }
    };

    // Comentarios de mock
    private static List<PromptComment> _mockComments = new List<PromptComment> {
        new PromptComment { commentId = 1, commentPromptId = 1, commentUserId = 1, commentContent = "¡Excelente prompt, muy útil!" },
        new PromptComment { commentId = 2, commentPromptId = 1, commentUserId = 2, commentContent = "Lo usé y me ahorró mucho tiempo." }
    };
    private static List<int> _mockSavedPrompts = new List<int>(); // Prompts guardados (vacio inicialmente)
    private static List<PromptRating> _mockRatings = new List<PromptRating>(); // Ratings de prompts (vacio inicialmente)

    public PromptsController(ILogger<PromptsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string? SearchText, int? FilterCategoryId, int? FilterDepartmentId)
    {
        // Crear las SelectLists para las opciones del select
        var categories = new SelectList(_dbCategories, "CategoryId", "CategoryName");
        var departments = new SelectList(_dbDepartments, "DepartmentId", "DepartmentName");

        var promptsToList = _mockPrompts.AsEnumerable();

        // Filtro falso solo para demostración
        if (!string.IsNullOrEmpty(SearchText))
        {
            promptsToList = promptsToList.Where(p => 
                (p.promptTitle != null && p.promptTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) || 
                (p.promptDescription != null && p.promptDescription.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        var finalPrompts = promptsToList.ToList();
        
        // Agregar los datos de comentarios, guardados y ratings a cada prompt antes de enviarlos a la vista
        foreach(var p in finalPrompts)
        {
            p.Comments = _mockComments.Where(c => c.commentPromptId == p.promptId).ToList();
            p.IsSaved = _mockSavedPrompts.Contains(p.promptId);
            p.Likes = _mockRatings.Count(r => r.ratingPromptId == p.promptId && r.ratingValue == 1);
            p.Dislikes = _mockRatings.Count(r => r.ratingPromptId == p.promptId && r.ratingValue == -1);
            
            // Obtener el rating del usuario actual (mock user id = 1)
            p.CurrentUserRating = _mockRatings.Find(r => r.ratingPromptId == p.promptId && r.ratingUserId == 1)?.ratingValue ?? 0;
        }

        // Enviar a la vista
        var vm = new PromptsViewModel
        {
            Categories = categories,
            Departments = departments,
            Prompts = finalPrompts,
            SearchText = SearchText,
            FilterCategoryId = FilterCategoryId,
            FilterDepartmentId = FilterDepartmentId
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult CreatePrompt(PromptsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Si el modelo no es válido, necesitamos recargar las listas y los prompts para mostrar la vista correctamente
            model.Categories = new SelectList(_dbCategories, "CategoryId", "CategoryName");
            model.Departments = new SelectList(_dbDepartments, "DepartmentId", "DepartmentName");
            model.Prompts = _mockPrompts.ToList();
            
            return View("Index", model); 
        }

        // Sacar el nombre de la categoría/departamento en base al id
        var selectedCategory = _dbCategories.Find(c => c.CategoryId == model.NewPromptCategoryId);
        var selectedDepartment = _dbDepartments.Find(d => d.DepartmentId == model.NewPromptDepartmentId);

        // Añadirlo a la "base de datos" (en este caso, la lista estática)
        var newPrompt = new PromptModel
        {
            promptId = _mockPrompts.Count > 0 ? _mockPrompts.Max(p => p.promptId) + 1 : 1,
            promptTitle = model.NewPromptTitle,
            promptDescription = model.NewPromptDescription,
            promptCategoryId = model.NewPromptCategoryId ?? 0,
            promptCategory = selectedCategory?.CategoryName,
            promptDepartmentId = model.NewPromptDepartmentId ?? 0,
            promptDepartment = selectedDepartment?.DepartmentName
        };

        // Agregar al principio de la lista para que aparezca primero
        _mockPrompts.Insert(0, newPrompt);

        // Regresar a la vista
        return RedirectToAction("Index");
    }

    // ACTION: Comment
    [HttpPost]
    public IActionResult AddComment(int promptId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText)) return RedirectToAction("Index");

        var newComment = new PromptComment
        {
            commentId = _mockComments.Count > 0 ? _mockComments.Max(c => c.commentId) + 1 : 1,
            commentPromptId = promptId,
            commentUserId = 1, // mock del usuario actual
            commentContent = commentText
        };
        
        _mockComments.Add(newComment);
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult SavePrompt(int promptId)
    {
        if (_mockSavedPrompts.Contains(promptId))
            _mockSavedPrompts.Remove(promptId); // Quitar de guardados
        else
            _mockSavedPrompts.Add(promptId);    // Agregar a guardados

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RatePrompt(int promptId, int ratingValue)
    {
        var existingRating = _mockRatings.Find(r => r.ratingPromptId == promptId && r.ratingUserId == 1);

        if (existingRating != null)
        {
            if (existingRating.ratingValue == ratingValue)
            {
                // Si el usuario hace click en el mismo rating, se quita el rating (toggle)
                _mockRatings.Remove(existingRating);
            }
            else
            {
                // Actualizar el rating existente
                existingRating.ratingValue = ratingValue;
            }
        }
        else
        {
            // Agregar nuevo rating
            _mockRatings.Add(new PromptRating { ratingPromptId = promptId, ratingUserId = 1, ratingValue = ratingValue });
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
