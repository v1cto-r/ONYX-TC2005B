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
            // Conexion API
            var viewModel = await _tiendaApiService.ObtenerTiendaAsync(UsuarioPruebaId);

            viewModel.Productos = FiltrarProductos(viewModel.Productos, SearchText, FilterCategoriaId);
            viewModel.SearchText = SearchText;
            viewModel.FilterCategoriaId = FilterCategoriaId;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la tienda");

            TempData["MensajeTienda"] = "No se pudo cargar la tienda. Revisa que el API esté encendido.";
            TempData["TipoMensajeTienda"] = "error";

            return View(new TiendaViewModel
            {
                Productos = new List<ProductoTienda>(),
                Categorias = new List<CategoriaTienda>(),
                CreditosUsuario = 0,
                SearchText = SearchText,
                FilterCategoriaId = FilterCategoriaId
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Comprar(int productoId)
    {
        if (productoId <= 0)
        {
            TempData["MensajeTienda"] = "No se encontró el producto seleccionado.";
            TempData["TipoMensajeTienda"] = "error";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            // Compra API
            var resultado = await _tiendaApiService.ComprarProductoAsync(UsuarioPruebaId, productoId);

            GuardarResultadoCompra(resultado);

            if (resultado.Exito)
            {
                await GuardarProductoCompradoAsync(productoId);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprar producto");

            TempData["MensajeTienda"] = "No se pudo conectar con el API para completar la compra.";
            TempData["TipoMensajeTienda"] = "error";

            return RedirectToAction(nameof(Index));
        }
    }

private static List<ProductoTienda> FiltrarProductos(
    List<ProductoTienda> productos,
    string? searchText,
    int? filterCategoriaId)
{
    var productosFiltrados = productos.AsEnumerable();
    var busqueda = searchText?.Trim();
    var categoriaId = filterCategoriaId.GetValueOrDefault();

    // Filtros
    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        productosFiltrados = productosFiltrados.Where(producto =>
            producto.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
            producto.Descripcion.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
            producto.CategoriaNombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase));
    }

    if (categoriaId > 0)
    {
        productosFiltrados = productosFiltrados.Where(producto =>
            producto.CategoriaId == categoriaId);
    }

    return productosFiltrados.ToList();
}

    private void GuardarResultadoCompra(CompraTiendaResponse resultado)
    {
        // Resultado compra
        TempData["MensajeTienda"] = resultado.Mensaje;
        TempData["TipoMensajeTienda"] = resultado.Exito ? "success" : "error";
        TempData["CreditosRestantes"] = resultado.CreditosRestantes;
    }

    private async Task GuardarProductoCompradoAsync(int productoId)
    {
        // Producto para modal
        var tiendaActualizada = await _tiendaApiService.ObtenerTiendaAsync(UsuarioPruebaId);
        var productoComprado = tiendaActualizada.Productos.FirstOrDefault(producto => producto.Id == productoId);

        if (productoComprado == null)
        {
            return;
        }

        TempData["ProductoNombre"] = productoComprado.Nombre;
        TempData["ProductoCategoria"] = productoComprado.CategoriaNombre;
        TempData["ProductoImagen"] = productoComprado.ImagenUrl;
        TempData["ProductoPrecio"] = productoComprado.Precio;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}