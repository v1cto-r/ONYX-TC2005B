using Microsoft.AspNetCore.Mvc;
using Front.Models;
using Front.Models.ViewModels;
using Front.Services;
using System.Text.RegularExpressions;

namespace Front.Controllers
{
    public class ClasificacionController : Controller
    {
        private readonly IClasificacionService _clasificacion;

        public ClasificacionController(IClasificacionService clasificacion)
        {
            _clasificacion = clasificacion;
        }

        public async Task<IActionResult> Index(string? departamento, string? nombre, int page = 1)
        {
            const int resultadosPorPagina = 5;

            var nombreBuscado = nombre?.Trim() ?? string.Empty;
            string? errorNombre = null;

            if (!string.IsNullOrWhiteSpace(nombreBuscado) && !EsNombreValido(nombreBuscado))
            {
                errorNombre = "El nombre debe tener entre 3 y 50 caracteres y solo puede incluir letras, espacios, guiones y apóstrofes.";
                nombreBuscado = string.Empty;
            }

            var departamentoSeleccionado = string.IsNullOrWhiteSpace(departamento) ? string.Empty : departamento;

            var globalCompleto = await _clasificacion.ObtenerClasificacionGlobal();
            var global         = globalCompleto.Take(3).ToList();

            List<UsuarioRanking> todosResultados;

            if (string.IsNullOrWhiteSpace(departamentoSeleccionado))
                todosResultados = globalCompleto;
            else
                todosResultados = await _clasificacion.ObtenerClasificacionDepartamental(departamentoSeleccionado);

            if (!string.IsNullOrWhiteSpace(nombreBuscado))
            {
                todosResultados = todosResultados
                    .Where(u => u.Nombre.Contains(nombreBuscado, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalResultados = todosResultados.Count;
            var totalPaginas    = (int)Math.Ceiling((double)totalResultados / resultadosPorPagina);

            if (page < 1) page = 1;
            if (page > totalPaginas && totalPaginas > 0) page = totalPaginas;

            var departamental = todosResultados
                .Skip((page - 1) * resultadosPorPagina)
                .Take(resultadosPorPagina)
                .ToList();

            var departamentos = globalCompleto
                .Select(u => u.Departamento)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d)
                .ToList();

            var mensajeDepto = string.IsNullOrWhiteSpace(departamentoSeleccionado)
                ? "todos los departamentos"
                : departamentoSeleccionado;

            var mensaje = errorNombre ??
                (string.IsNullOrWhiteSpace(nombreBuscado)
                    ? $"Datos cargados correctamente. Mostrando {mensajeDepto}."
                    : $"Mostrando resultados para \"{nombreBuscado}\" en {mensajeDepto}.");

            var vm = new ClasificacionViewModel
            {
                Global                  = global,
                Departamental           = departamental,
                Departamentos           = departamentos,
                DepartamentoSeleccionado = departamentoSeleccionado,
                NombreBuscado           = nombreBuscado,
                Mensaje                 = mensaje,
                ErrorNombre             = errorNombre,
                PaginaActual            = page,
                TotalPaginas            = totalPaginas,
                ResultadosPorPagina     = resultadosPorPagina,
                TotalResultados         = totalResultados
            };

            return View("~/Views/Clasificacion/Index.cshtml", vm);
        }

        private static bool EsNombreValido(string nombre) =>
            nombre.Length is >= 3 and <= 50 && Regex.IsMatch(nombre, @"^[\p{L}\s'-]+$");
    }
}