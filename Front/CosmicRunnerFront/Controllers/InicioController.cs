using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.DataInicio;

namespace CosmicRunnerFront.Controllers;

public class InicioController : Controller
{
    // GET: Carga la página principal y envía las ideas guardadas
    public IActionResult Index()
    {
        // 1. Obtenemos las ideas de la "base de datos" ordenadas por la más reciente
        // Ordenamos por Id de menor a mayor (1, 2, 3...)
var ideasGuardadas = MockDatabase.Ideas.OrderBy(i => i.Id).ToList();
        
        // 2. Pasamos las ideas a la vista mediante ViewBag para iterarlas en el HTML
        ViewBag.ListaIdeas = ideasGuardadas;

        // 3. Inicializamos el modelo vacío para el formulario de crear idea
        // Nota: Cambié CrearIdeaViewModel por Idea para simplificar, pero puedes usar tu ViewModel si lo prefieres.
        ViewData["NuevaIdea"] = new Idea(); 
        
        return View();
    }

    [HttpPost]
    public IActionResult CrearIdea(Idea nuevaIdea)
    {
        // Asignamos valores base
        nuevaIdea.Id = MockDatabase.Ideas.Any() ? MockDatabase.Ideas.Max(i => i.Id) + 1 : 1; 
        nuevaIdea.FechaPublicacion = DateTime.Now;
        nuevaIdea.Estado = EstadoIniciativa.EnRevisionInicial;
        
        // Simulamos que el usuario logueado es César (Id = 1)
        nuevaIdea.AutorId = 1; 
        nuevaIdea.Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1);

        // Enlazamos los objetos completos para que la vista pueda leer sus nombres
        nuevaIdea.Departamento = MockDatabase.Departamentos.FirstOrDefault(d => d.Id == nuevaIdea.DepartamentoId);
        nuevaIdea.AreaImpacto = MockDatabase.AreasImpacto.FirstOrDefault(a => a.Id == nuevaIdea.AreaImpactoId);

        // Inicializamos las listas vacías para evitar errores nulos
        nuevaIdea.ListaColaboradores = new List<Usuario>();
        nuevaIdea.ListaComentarios = new List<Comentario>();

        // Guardamos en la memoria RAM
        MockDatabase.Ideas.Add(nuevaIdea);

        return RedirectToAction("Index");
    }

    // POST: Ejemplo para simular la acción de unirse a un proyecto sin JS
    [HttpPost]
    public IActionResult UnirseProyecto(int ideaId)
    {
        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        if (idea != null)
        {
            var usuarioActual = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1);
            if (usuarioActual != null && !idea.ListaColaboradores.Contains(usuarioActual))
            {
                idea.ListaColaboradores.Add(usuarioActual);
            }
        }
        
        return RedirectToAction("Index");
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