using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CosmicRunnerFront.Models;
using CosmicRunnerFront.Models.TiendaModels;
using CosmicRunnerFront.DataTienda;

namespace CosmicRunnerFront.Controllers;

public class TiendaController : Controller
{
    private readonly ILogger<TiendaController> _logger;

    public TiendaController(ILogger<TiendaController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string? SearchText, int? FilterCategoriaId)
    {
        // Traemos los productos desde la base de datos fake
        var productos = TiendaMockDatabase.ObtenerProductos().AsEnumerable();

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

        // Modelo que se manda a view
        var viewModel = new TiendaViewModel
        {
            Productos = productos.ToList(),
            Categorias = TiendaMockDatabase.Categorias,
            CreditosUsuario = TiendaMockDatabase.CreditosUsuario,
            SearchText = SearchText,
            FilterCategoriaId = FilterCategoriaId
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Comprar(int productoId)
    {
        // Intentamos comprar el producto]
        var resultado = TiendaMockDatabase.ComprarProducto(productoId);

        // Guardamos el resultado]
        TempData["MensajeTienda"] = resultado.Mensaje;
        TempData["TipoMensajeTienda"] = resultado.Exito ? "success" : "error";
        TempData["CreditosRestantes"] = resultado.CreditosRestantes;

        // Si la compra sale bien mandamos los datos del producto al pop up final
        if (resultado.ProductoComprado != null)
        {
            TempData["ProductoNombre"] = resultado.ProductoComprado.Nombre;
            TempData["ProductoCategoria"] = resultado.ProductoComprado.CategoriaNombre;
            TempData["ProductoImagen"] = resultado.ProductoComprado.ImagenUrl;
            TempData["ProductoPrecio"] = resultado.ProductoComprado.Precio;
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}