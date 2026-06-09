using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Services.Login;

namespace CosmicRunnerFront.Controllers;

public class HomeController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";
    private readonly ILoginService _loginService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ILoginService loginService)
    {
        _logger = logger;
        _loginService = loginService;
    }

    public IActionResult Index()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is not null)
            return RedirectToAction("Index", "Inicio");
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var userId = await _loginService.LoginUser(model.email, model.password);

        if (userId == -1)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View("Index", model);
        }
        Console.WriteLine("User id after login is: "+userId);
        
        HttpContext.Session.SetInt32(CurrentUserSessionKey, userId);
        return RedirectToAction("Index", "Inicio");
    }
    
    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
