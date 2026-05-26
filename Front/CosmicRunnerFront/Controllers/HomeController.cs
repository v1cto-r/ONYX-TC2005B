using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;

namespace CosmicRunnerFront.Controllers;

public class HomeController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login()
    {
        HttpContext.Session.SetInt32(CurrentUserSessionKey, 1);
        return RedirectToAction("Index", "Inicio");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
