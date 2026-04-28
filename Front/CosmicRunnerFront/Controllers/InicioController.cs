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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CrearIdea(CrearIdeaViewModel modelo)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }
        TempData["ReabrirModalNuevaIdea"] = true;
        ViewData["NuevaIdea"] = modelo;
        return View(Index);
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
