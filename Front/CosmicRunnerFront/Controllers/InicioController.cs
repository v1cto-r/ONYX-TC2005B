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
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        var viewModel = new InicioViewModel();
        
        viewModel.UsuarioActual = await _inicioApiService.GetUsuarioByIdAsync(currentUserId.Value);
        viewModel.ListaIdeas = await _inicioApiService.GetIdeasAsync(currentUserId.Value);

        var departamentos = await _inicioApiService.GetDepartamentosAsync();
        var areas = await _inicioApiService.GetAreasImpactoAsync();

        viewModel.NuevaIdea.CatalogoDepartamentos = new SelectList(departamentos, "department_id", "name");
        viewModel.NuevaIdea.CatalogoAreasImpacto = new SelectList(areas, "area_impacto_id", "name");

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CrearIdea(FormularioIdeaViewModel NuevaIdea)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            NuevaIdea.autor_id = currentUserId.Value;
            
            var exito = await _inicioApiService.CrearIdeaAsync(NuevaIdea);
            if (exito) 
            {
                TempData["MensajeExito"] = "¡Tu iniciativa ha sido publicada con éxito y ya está en el Top!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar la idea en el servidor. Intenta de nuevo.");
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "No pudimos publicar tu idea. Por favor, revisa los campos en rojo.");
        }
        var viewModel = new InicioViewModel
        {
            NuevaIdea = NuevaIdea, 
            UsuarioActual = await _inicioApiService.GetUsuarioByIdAsync(currentUserId.Value),
            ListaIdeas = await _inicioApiService.GetIdeasAsync(currentUserId.Value)
        };

        var departamentos = await _inicioApiService.GetDepartamentosAsync();
        var areas = await _inicioApiService.GetAreasImpactoAsync();

        viewModel.NuevaIdea.CatalogoDepartamentos = new SelectList(departamentos, "department_id", "name");
        viewModel.NuevaIdea.CatalogoAreasImpacto = new SelectList(areas, "area_impacto_id", "name");

        return View("Index", viewModel);
    }

    [HttpGet]
    public IActionResult LimpiarFormulario()
    {
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DarLike(int ideaId)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        await _inicioApiService.ReaccionarIdeaAsync(ideaId, currentUserId.Value, "like");
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DarDislike(int ideaId)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        await _inicioApiService.ReaccionarIdeaAsync(ideaId, currentUserId.Value, "dislike");
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> GuardarComentario(int ideaId, string Mensaje)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        if (!string.IsNullOrWhiteSpace(Mensaje))
        {
            await _inicioApiService.GuardarComentarioAsync(ideaId, currentUserId.Value, Mensaje);
        }
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UnirseProyecto(int ideaId)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        await _inicioApiService.UnirseProyectoAsync(ideaId, currentUserId.Value);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DarLikeComentario(int commentId)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        await _inicioApiService.ReaccionarComentarioAsync(commentId, currentUserId.Value, "like");
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DarDislikeComentario(int commentId)
    {
        int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId");
        if (currentUserId == null) return RedirectToAction("Index", "Home");

        await _inicioApiService.ReaccionarComentarioAsync(commentId, currentUserId.Value, "dislike");
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}