using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.InicioModels;
using CosmicRunnerFront.DataInicio;
using Microsoft.VisualBasic.FileIO; 

namespace CosmicRunnerFront.Controllers;

public class InicioController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";

    public IActionResult Index()
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Obtenemos las ideas ordenadas por ID para mantener el orden del mockup
        var ideasGuardadas = MockDatabase.Ideas.OrderBy(i => i.Id).ToList();
        var usuarioActual = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == currentUserId.Value);

        if (usuarioActual is null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        ViewBag.ListaIdeas = ideasGuardadas;
        ViewBag.UsuarioActual = usuarioActual;
        ViewData["NuevaIdea"] = new Idea();
        
        return View();
    }

    [HttpPost]
    public IActionResult CrearIdea(Idea nuevaIdea)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        nuevaIdea.Id = MockDatabase.Ideas.Any() ? MockDatabase.Ideas.Max(i => i.Id) + 1 : 1; 
        nuevaIdea.FechaPublicacion = DateTime.Now;
        nuevaIdea.Estado = EstadoIniciativa.EnRevisionInicial;
        
        nuevaIdea.AutorId = currentUserId.Value; 
        nuevaIdea.Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == currentUserId.Value);
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
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        // idea en nuestra "Base de Datos"
        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        
        if (idea != null)
        {
            var usuarioActual = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == currentUserId.Value);
            
            // 3. Verificamos que el usuario no sea ya un colaborador para no duplicarlo
            if (usuarioActual != null && !idea.ListaColaboradores.Any(c => c.Id == usuarioActual.Id) && usuarioActual.Id != idea.AutorId)
            {
                idea.ListaColaboradores.Add(usuarioActual);
            }
        }
        
        // Devolvemos al usuario al feed para que vea su pastilla de colaborador
        return RedirectToAction("Index");
    }

    // Likes
    [HttpPost]
    public IActionResult DarLike(int ideaId)
    {
        if (GetCurrentUserId() is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        if (idea != null)
        {
            idea.Likes++;
        }
        return RedirectToAction("Index");
    }

    // Dislikes
    [HttpPost]
    public IActionResult DarDislike(int ideaId)
    {
        if (GetCurrentUserId() is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var idea = MockDatabase.Ideas.FirstOrDefault(i => i.Id == ideaId);
        if (idea != null)
        {
            idea.Dislikes++;
        }
        return RedirectToAction("Index");
    }

    // Guardar Comentarios
    [HttpPost]
    public IActionResult GuardarComentario(int ideaId, string Mensaje)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

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
                    AutorId = currentUserId.Value,
                    Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == currentUserId.Value),
                    Likes = 0,
                    Dislikes = 0
                };
                
                idea.ListaComentarios.Add(nuevoComentario);
            }
        }
        return RedirectToAction("Index");
    }

    // Guardar Respuestas anidadas
    [HttpPost]
    public IActionResult GuardarRespuesta(int comentarioPadreId, string Mensaje)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

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
                        AutorId = currentUserId.Value,
                        Autor = MockDatabase.Usuarios.FirstOrDefault(u => u.Id == currentUserId.Value),
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

    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }
}