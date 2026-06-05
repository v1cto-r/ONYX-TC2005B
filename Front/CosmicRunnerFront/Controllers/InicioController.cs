using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.Models.ViewModels;
using CosmicRunnerFront.Services;

namespace CosmicRunnerFront.Controllers;

public class InicioController : Controller
{
    private readonly IInicioApiService _inicioApiService;

    public InicioController(IInicioApiService inicioApiService)
    {
        _inicioApiService = inicioApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var viewModel = new InicioViewModel();
        viewModel.UsuarioActual = await _inicioApiService.GetUsuarioByIdAsync(1);
        viewModel.ListaIdeas = await _inicioApiService.GetIdeasAsync();

        var departamentos = await _inicioApiService.GetDepartamentosAsync();
        var areas = await _inicioApiService.GetAreasImpactoAsync();

        viewModel.NuevaIdea.CatalogoDepartamentos = new SelectList(departamentos, "department_id", "name");
        viewModel.NuevaIdea.CatalogoAreasImpacto = new SelectList(areas, "area_impacto_id", "name");

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CrearIdea(FormularioIdeaViewModel NuevaIdea)
    {
        if (ModelState.IsValid)
        {
            NuevaIdea.autor_id = 1;

            var exito = await _inicioApiService.CrearIdeaAsync(NuevaIdea);
            
            if (exito)
            {
                return RedirectToAction("Index");
            }
        }

        var viewModel = new InicioViewModel
        {
            NuevaIdea = NuevaIdea, 
            UsuarioActual = await _inicioApiService.GetUsuarioByIdAsync(1),
            ListaIdeas = await _inicioApiService.GetIdeasAsync()
        };

        var departamentos = await _inicioApiService.GetDepartamentosAsync();
        var areas = await _inicioApiService.GetAreasImpactoAsync();

        viewModel.NuevaIdea.CatalogoDepartamentos = new SelectList(departamentos, "department_id", "name");
        viewModel.NuevaIdea.CatalogoAreasImpacto = new SelectList(areas, "area_impacto_id", "name");

        return View("Index", viewModel);
    }


    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }
}