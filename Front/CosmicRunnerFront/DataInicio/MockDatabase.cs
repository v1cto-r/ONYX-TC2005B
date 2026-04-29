using System;
using System.Collections.Generic;
using CosmicRunnerFront.Models.InicioModels;

namespace CosmicRunnerFront.DataInicio
{
    public static class MockDatabase
    {
        public static List<Usuario> Usuarios = new List<Usuario>();
        public static List<Idea> Ideas = new List<Idea>();
        public static List<Departamento> Departamentos = new List<Departamento>();
        public static List<AreaImpacto> AreasImpacto = new List<AreaImpacto>();

        static MockDatabase()
        {
            // 1. Departamentos
            Departamentos.Add(new Departamento { Id = 1, Nombre = "Calidad" });
            Departamentos.Add(new Departamento { Id = 2, Nombre = "Finanzas" });
            Departamentos.Add(new Departamento { Id = 3, Nombre = "Innovación" });

            // 2. Áreas de Impacto
            AreasImpacto.Add(new AreaImpacto { Id = 1, Nombre = "Optimización" });
            AreasImpacto.Add(new AreaImpacto { Id = 2, Nombre = "Sostenibilidad" });

            // 3. Usuarios (Basados en el mockup)
            var cesar = new Usuario { Id = 1, Nombre = "César", Apellido = "Alexandro", DepartamentoId = 1, Racha = 24, PosicionGlobalPorcentaje = 50, PosicionDepartamentalPorcentaje = 10, FotoPerfilUrl = "/assets/random/user.png" };
            var adriana = new Usuario { Id = 2, Nombre = "Adriana", Apellido = "Díaz", DepartamentoId = 1, FotoPerfilUrl = "/assets/random/user.png" };
            var jacobo = new Usuario { Id = 3, Nombre = "Jacobo", Apellido = "Pérez", DepartamentoId = 2, FotoPerfilUrl = "/assets/random/user.png" };
            var julia = new Usuario { Id = 4, Nombre = "Julia", Apellido = "Fernández", DepartamentoId = 3, FotoPerfilUrl = "/assets/random/user.png" };
            var mario = new Usuario { Id = 5, Nombre = "Mario", Apellido = "López", DepartamentoId = 3, FotoPerfilUrl = "/assets/random/user.png" };

            Usuarios.AddRange(new[] { cesar, adriana, jacobo, julia, mario });

            // 4. Ideas
            var idea1 = new Idea
            {
                Id = 1,
                Titulo = "Optimización de Línea de Producción",
                Descripcion = "Optimizar una línea de producción es similar a dirigir una sinfonía; cada elemento debe estar en perfecta armonía. Este proceso implica un examen y ajuste meticulosos de cada faceta del proceso para reducir el desperdicio y aumentar la eficiencia.",
                FechaPublicacion = DateTime.Now.AddDays(-5), // Para que marque "5 días"
                Estado = EstadoIniciativa.Reclutamiento, // El estado "negro" en el mockup
                Likes = 784,
                Dislikes = 40,
                AutorId = 2,
                Autor = adriana,
                DepartamentoId = 1,
                Departamento = Departamentos[0],
                AreaImpactoId = 1,
                AreaImpacto = AreasImpacto[0],
                PuntosClave = new List<string> 
                { 
                    "Análisis de cuellos de botella en la fase de ensamblaje B.",
                    "Revisión de capacidades de la maquinaria actual.",
                    "Estandarización de tiempos de ciclo por estación."
                },
                ListaColaboradores = new List<Usuario> { jacobo, julia, mario },
                // Simulamos los 40 comentarios (creando una lista ficticia con 40 elementos o forzando un count. Para simplificar, llenamos una pequeña lista)
                ListaComentarios = new List<Comentario>(new Comentario[40]) 
            };
            
            // Idea secundaria para comprobar el bucle
            var idea2 = new Idea
            {
                Id = 2,
                Titulo = "Plataforma Gamificada Codex",
                Descripcion = "Implementación de dinámicas de juego para incentivar la participación corporativa en la plataforma Codex, priorizando una paleta de colores Light Mode.",
                FechaPublicacion = DateTime.Now.AddDays(-2),
                Estado = EstadoIniciativa.EnRevisionInicial,
                Likes = 120,
                Dislikes = 5,
                AutorId = 5,
                Autor = mario,
                DepartamentoId = 3,
                Departamento = Departamentos[2],
                AreaImpactoId = 2,
                AreaImpacto = AreasImpacto[1],
                PuntosClave = new List<string> { "Diseño de insignias", "Integración de MVC" },
                ListaColaboradores = new List<Usuario>(), // Sin colaboradores aún
                ListaComentarios = new List<Comentario>()
            };

            Ideas.Add(idea1);
            Ideas.Add(idea2);
        }
    }
}