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

    // POST: Este método es llamado por el formulario HTML al hacer "Publicar Idea"
    [HttpPost]
    public IActionResult CrearIdea(Idea nuevaIdea)
    {
        // Asignamos los valores que no vienen del formulario directamente
        // Simulamos un autoincremental para el ID
        nuevaIdea.Id = MockDatabase.Ideas.Any() ? MockDatabase.Ideas.Max(i => i.Id) + 1 : 1; 
        nuevaIdea.FechaPublicacion = DateTime.Now;
        nuevaIdea.Estado = EstadoIniciativa.EnRevisionInicial;
        
        // Simulamos que el usuario logueado (César) es el autor
        nuevaIdea.AutorId = 1; 

        // Guardamos en la memoria RAM
        MockDatabase.Ideas.Add(nuevaIdea);

        // Patrón PRG (Post-Redirect-Get): Redirigimos al Index para refrescar la pantalla y evitar reenvíos de formulario
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