using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.TiendaModels;
using CosmicRunnerFront.Services.Tienda;

namespace CosmicRunnerFront.Controllers;

public class TiendaController : Controller
{
    private readonly ILogger<TiendaController> _logger;
    private readonly TiendaApiService _tiendaApiService;

    private const int UsuarioPruebaId = 1;

    public TiendaController(ILogger<TiendaController> logger, TiendaApiService tiendaApiService)
    {
        _logger = logger;
        _tiendaApiService = tiendaApiService;
    }

    public async Task<IActionResult> Index(string? SearchText, int? FilterCategoriaId)
    {
        try
        {
            // Traemos los productos desde el API de Jorge
            var viewModel = await _tiendaApiService.ObtenerTiendaAsync(UsuarioPruebaId);

            var productos = viewModel.Productos.AsEnumerable();

            // Si el usuario escribio algo buscamos en nombre descripcion o categoria
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                productos = productos.Where(producto =>
                    producto.Nombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    producto.Descripcion.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    producto.CategoriaNombre.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Si el usuario selecciona una categoria dejamos solo esos productos
            if (FilterCategoriaId.HasValue && FilterCategoriaId.Value > 0)
            {
                productos = productos.Where(producto => producto.CategoriaId == FilterCategoriaId.Value);
            }

            viewModel.Productos = productos.ToList();
            viewModel.SearchText = SearchText;
            viewModel.FilterCategoriaId = FilterCategoriaId;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la tienda desde el API");

            TempData["MensajeTienda"] = "No se pudo cargar la tienda. Revisa que el API esté encendido.";
            TempData["TipoMensajeTienda"] = "error";

            var viewModel = new TiendaViewModel
            {
                Productos = new List<ProductoTienda>(),
                Categorias = new List<CategoriaTienda>(),
                CreditosUsuario = 0,
                SearchText = SearchText,
                FilterCategoriaId = FilterCategoriaId
            };

            return View(viewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Comprar(int productoId)
    {
        try
        {
            // Mandamos la compra al API
            var resultado = await _tiendaApiService.ComprarProductoAsync(UsuarioPruebaId, productoId);

            TempData["MensajeTienda"] = resultado.Mensaje;
            TempData["TipoMensajeTienda"] = resultado.Exito ? "success" : "error";
            TempData["CreditosRestantes"] = resultado.CreditosRestantes;

            // Si la compra sale bien buscamos el producto completo para mostrarlo en el pop up
            if (resultado.Exito)
            {
                var tiendaActualizada = await _tiendaApiService.ObtenerTiendaAsync(UsuarioPruebaId);
                var productoComprado = tiendaActualizada.Productos.FirstOrDefault(producto => producto.Id == productoId);

                if (productoComprado != null)
                {
                    TempData["ProductoNombre"] = productoComprado.Nombre;
                    TempData["ProductoCategoria"] = productoComprado.CategoriaNombre;
                    TempData["ProductoImagen"] = productoComprado.ImagenUrl;
                    TempData["ProductoPrecio"] = productoComprado.Precio;
                }
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprar producto desde la tienda");

            TempData["MensajeTienda"] = "No se pudo conectar con el API para completar la compra.";
            TempData["TipoMensajeTienda"] = "error";

            return RedirectToAction("Index");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}