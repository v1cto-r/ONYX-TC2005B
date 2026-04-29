using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.DataInicio;
using Microsoft.VisualBasic.FileIO; // Asegúrate de que apunte a tu carpeta DataInicio

namespace CosmicRunnerFront.Controllers;

public class InicioController : Controller
{
    public IActionResult Index()
    {
        // Obtenemos las ideas ordenadas por ID para mantener el orden del mockup
        var ideasGuardadas = MockDatabase.Ideas.OrderBy(i => i.Id).ToList();
        
        ViewBag.ListaIdeas = ideasGuardadas;
        ViewData["NuevaIdea"] = new Idea(); 
        
        return View();
    }

    [HttpPost]
    public IActionResult CrearIdea(Idea nuevaIdea, bool? cancelar)
    {
        if (cancelar == true)
        { 
            return RedirectToAction("Index");
        }
        nuevaIdea.Id = MockDatabase.Ideas.Any() ? MockDatabase.Ideas.Max(i => i.Id) + 1 : 1; 
        nuevaIdea.FechaPublicacion = DateTime.Now;
        nuevaIdea.Estado = EstadoIniciativa.EnRevisionInicial;
        
        nuevaIdea.AutorId = 1; 
        nuevaIdea.Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1);
        nuevaIdea.Departamento = MockDatabase.Departamentos.FirstOrDefault(d => d.Id == nuevaIdea.DepartamentoId);
        nuevaIdea.AreaImpacto = MockDatabase.AreasImpacto.FirstOrDefault(a => a.Id == nuevaIdea.AreaImpactoId);

        nuevaIdea.ListaColaboradores = new List<Usuario>();
        nuevaIdea.ListaComentarios = new List<Comentario>();

        MockDatabase.Ideas.Add(nuevaIdea);
        return RedirectToAction("Index");
    }
   
    [HttpPost]
    public IActionResult UnirseProyecto(int ideaId)
    {
        // idea en nuestra "Base de Datos"
        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        
        if (idea != null)
        {
            // usuario logueado (César - Id 1)
            var usuarioActual = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1);
            
            // 3. Verificamos que el usuario no sea ya un colaborador para no duplicarlo
            if (usuarioActual != null && !idea.ListaColaboradores.Any(c => c.Id == usuarioActual.Id) && usuarioActual.Id != idea.AutorId)
            {
                idea.ListaColaboradores.Add(usuarioActual);
            }
        }
        
        // Devolvemos al usuario al feed para que vea su pastilla de colaborador
        return RedirectToAction("Index");
    }

    // PASO 2: Lógica de Likes
    [HttpPost]
    public IActionResult DarLike(int ideaId)
    {
        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        if (idea != null)
        {
            idea.Likes++;
        }
        return RedirectToAction("Index");
    }

    // PASO 2: Lógica de Dislikes
    [HttpPost]
    public IActionResult DarDislike(int ideaId)
    {
        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        if (idea != null)
        {
            idea.Dislikes++;
        }
        return RedirectToAction("Index");
    }

    // PASO 3: Lógica para Guardar Comentarios
    [HttpPost]
    public IActionResult GuardarComentario(int ideaId, string Mensaje)
    {
        if (!string.IsNullOrWhiteSpace(Mensaje))
        {
            var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
            if (idea != null)
            {
                var nuevoComentario = new Comentario
                {
                    Id = idea.ListaComentarios.Any() ? idea.ListaComentarios.Max(c => c.Id) + 1 : 1,
                    IdeaId = ideaId,
                    Mensaje = Mensaje,
                    FechaCreacion = DateTime.Now,
                    // Asignamos a César (Id = 1) como el autor de la recomendación
                    AutorId = 1,
                    Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1),
                    Likes = 0,
                    Dislikes = 0
                };
                
                idea.ListaComentarios.Add(nuevoComentario);
            }
        }
        return RedirectToAction("Index");
    }

    // PASO 5: Lógica para Guardar Respuestas anidadas
    [HttpPost]
    public IActionResult GuardarRespuesta(int comentarioPadreId, string Mensaje)
    {
        if (!string.IsNullOrWhiteSpace(Mensaje))
        {
            // Buscamos el comentario padre dentro de las ideas
            foreach (var idea in MockDatabase.Ideas)
            {
                var padre = idea.ListaComentarios.FirstOrDefault(c => c.Id == comentarioPadreId);
                if (padre != null)
                {
                    var nuevaRespuesta = new Respuesta
                    {
                        Id = padre.ListaRespuestas.Any() ? padre.ListaRespuestas.Max(r => r.Id) + 1 : 1,
                        ComentarioPadreId = comentarioPadreId,
                        Mensaje = Mensaje,
                        FechaCreacion = DateTime.Now,
                        AutorId = 1, // Simulamos a César
                        Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == 1),
                        Likes = 0,
                        Dislikes = 0
                    };
                    
                    padre.ListaRespuestas.Add(nuevaRespuesta);
                    break; // Salimos del ciclo porque ya encontramos el comentario
                }
            }
        }
        return RedirectToAction("Index");
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}