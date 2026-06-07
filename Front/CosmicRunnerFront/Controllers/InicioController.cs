using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.Services;
using CosmicRunnerFront.Models;

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


     [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult LimpiarFormulario()
    {
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DarLike(int ideaId)
    {
        await _inicioApiService.ReaccionarIdeaAsync(ideaId, 1, "like");
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DarDislike(int ideaId)
    {
        await _inicioApiService.ReaccionarIdeaAsync(ideaId, 1, "dislike");
        return RedirectToAction("Index");
    }
}