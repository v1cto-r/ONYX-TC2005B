using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Services.Prompts;

namespace CosmicRunnerFront.Controllers;

public class PromptsController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";
    private readonly IPromptService _promptService;
    private readonly ILogger<PromptsController> _logger;

    public PromptsController(ILogger<PromptsController> logger, IPromptService promptService)
    {
        _logger = logger;
        _promptService = promptService;
    }

    public async Task<IActionResult> Index(string? SearchText, int? FilterCategoryId, int? FilterDepartmentId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return RedirectToAction("Index", "Home");

        var prompts = await _promptService.ObtenerPromptAsync(
            currentUserId.Value,
            SearchText,
            FilterCategoryId,
            FilterDepartmentId
        ) ?? new List<PromptModel>();

        var (categories, departments) = await GetSelectListsAsync();
        var vm = new PromptsViewModel
        {
            Categories = categories,
            Departments = departments,
            Prompts = prompts,
            SearchText = SearchText,
            FilterCategoryId = FilterCategoryId,
            FilterDepartmentId = FilterDepartmentId
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrompt(PromptsViewModel model)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            var (cats, depts) = await GetSelectListsAsync();
            model.Categories = cats;
            model.Departments = depts;
            model.Prompts = await _promptService.ObtenerPromptAsync(currentUserId.Value, null, null, null) ?? new List<PromptModel>();
            return View("Index", model);
        }

        var status = await _promptService.CrearPromptAsync(
            currentUserId.Value,
            model.NewPromptTitle,
            model.NewPromptDescription,
            model.NewPromptCategoryId ?? 0,
            model.NewPromptDepartmentId ?? 0
        );

        TempData["SuccessMessage"] = status;
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int promptId, string? commentText)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return RedirectToAction("Index", "Home");

        if (!string.IsNullOrWhiteSpace(commentText))
            await _promptService.CommentarPromptAsync(promptId, currentUserId.Value, commentText);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> SavePrompt(int promptId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return RedirectToAction("Index", "Home");

        await _promptService.GuardarPromptAsync(promptId, currentUserId.Value);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RatePrompt(int promptId, int ratingValue)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return RedirectToAction("Index", "Home");

        await _promptService.CalificarPromptAsync(promptId, currentUserId.Value, ratingValue);
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }

    private async Task<(SelectList Categories, SelectList Departments)> GetSelectListsAsync()
    {
        var (categories, departments) = await _promptService.ObtenerOpcionesAsync();
        return (
            new SelectList(categories, "CategoryId", "CategoryName"),
            new SelectList(departments, "DepartmentId", "DepartmentName")
        );
    }
}
