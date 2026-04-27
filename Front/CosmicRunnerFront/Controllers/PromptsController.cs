using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Controllers;

public class PromptsController : Controller
{
    private readonly ILogger<PromptsController> _logger;

    public PromptsController(ILogger<PromptsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
