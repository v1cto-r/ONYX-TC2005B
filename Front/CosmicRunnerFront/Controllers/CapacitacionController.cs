using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CosmicRunnerFront.Services.Prompts;

namespace CosmicRunnerFront.Controllers;

public class CapacitacionController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";
    private readonly IPromptService _promptService;
    private readonly ILogger<CapacitacionController> _logger;

    public CapacitacionController(ILogger<CapacitacionController> logger, IPromptService promptService)
    {
        _logger = logger;
        _promptService = promptService;
    }

    public IActionResult Index()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            return RedirectToAction("Index", "Home");
        
        return View();
    }
    
    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }
}
