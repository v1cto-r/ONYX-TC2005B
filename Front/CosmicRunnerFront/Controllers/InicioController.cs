using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using System.ComponentModel.Design;
using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.Controllers;

public class InicioController : Controller
{

    public IActionResult Index()
    {
        ViewData["NuevaIdea"] = new CrearIdeaViewModel();
        return View();
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
