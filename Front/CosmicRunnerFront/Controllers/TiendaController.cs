using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.TiendaModels;
using CosmicRunnerFront.Services.Tienda;

namespace CosmicRunnerFront.Controllers;

public class TiendaController : Controller
{
    private const string CurrentUserSessionKey = "CurrentUserId";

    private readonly ILogger<TiendaController> _logger;
    private readonly TiendaApiService _tiendaApiService;

    public TiendaController(ILogger<TiendaController> logger, TiendaApiService tiendaApiService)
    {
        _logger = logger;
        _tiendaApiService = tiendaApiService;
    }

    public async Task<IActionResult> Index(string? SearchText, int? FilterCategoriaId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var viewModel = await _tiendaApiService.ObtenerTiendaAsync(currentUserId.Value);

            viewModel.Productos = FiltrarProductos(viewModel.Productos, SearchText, FilterCategoriaId);
            viewModel.SearchText = SearchText;
            viewModel.FilterCategoriaId = FilterCategoriaId;
            viewModel.TiendaDisponible = true;

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar la tienda");

            TempData["MensajeTiendaTitulo"] = "Tienda no disponible";
            TempData["MensajeTienda"] = "Por el momento no se pudieron cargar los productos. Intenta de nuevo mas tarde.";
            TempData["TipoMensajeTienda"] = "error";

            return View(new TiendaViewModel
            {
                Productos = new List<ProductoTienda>(),
                Categorias = new List<CategoriaTienda>(),
                CreditosUsuario = 0,
                SearchText = SearchText,
                FilterCategoriaId = FilterCategoriaId,
                TiendaDisponible = false
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Comprar(int productoId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (productoId <= 0)
        {
            TempData["MensajeTiendaTitulo"] = "Producto no encontrado";
            TempData["MensajeTienda"] = "No se encontro el producto seleccionado.";
            TempData["TipoMensajeTienda"] = "error";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            var resultado = await _tiendaApiService.ComprarProductoAsync(currentUserId.Value, productoId);

            GuardarResultadoCompra(resultado);

            if (resultado.Exito)
            {
                await GuardarProductoCompradoAsync(currentUserId.Value, productoId);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comprar producto");

            TempData["MensajeTiendaTitulo"] = "Compra no disponible";
            TempData["MensajeTienda"] = "No se pudo procesar la compra en este momento. Intenta de nuevo mas tarde.";
            TempData["TipoMensajeTienda"] = "error";

            return RedirectToAction(nameof(Index));
        }
    }

    private int? GetCurrentUserId()
    {
        return HttpContext.Session.GetInt32(CurrentUserSessionKey);
    }

    private static List<ProductoTienda> FiltrarProductos(
        List<ProductoTienda> productos,
        string? searchText,
        int? filterCategoriaId)
    {
        var productosFiltrados = productos.AsEnumerable();
        var busqueda = searchText?.Trim();
        var categoriaId = filterCategoriaId.GetValueOrDefault();

        // Filtros de busqueda y categoria.
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
        TempData["MensajeTiendaTitulo"] = resultado.Exito
            ? "Compra realizada con exito"
            : "No se pudo completar la compra";

        TempData["MensajeTienda"] = resultado.Mensaje;
        TempData["TipoMensajeTienda"] = resultado.Exito ? "success" : "error";
        TempData["CreditosRestantes"] = resultado.CreditosRestantes;
    }

    private async Task GuardarProductoCompradoAsync(int usuarioId, int productoId)
    {
        // Se vuelve a consultar la tienda para mostrar el producto comprado en el modal.
        var tiendaActualizada = await _tiendaApiService.ObtenerTiendaAsync(usuarioId);
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