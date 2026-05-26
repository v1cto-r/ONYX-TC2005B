using System.Text;
using System.Text.Json;
using CosmicRunnerFront.Models.TiendaModels;

namespace CosmicRunnerFront.Services.Tienda;

public class TiendaApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TiendaApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        var apiBaseUrl = configuration["ApiUrls:JorgeApi"] ?? "https://127.0.0.1:5057/";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<TiendaViewModel> ObtenerTiendaAsync(int usuarioId)
    {
        var response = await _httpClient.GetAsync($"api/tienda/productos/{usuarioId}");
        var json = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<TiendaProductosApiResponse>(json, _jsonOptions);

        if (apiResponse == null || !apiResponse.Exito)
        {
            throw new Exception(apiResponse?.Mensaje ?? "No se pudo cargar la tienda desde el API.");
        }

        var categoriasApi = apiResponse.Productos
            .Select(producto => producto.Category)
            .Where(categoria => !string.IsNullOrWhiteSpace(categoria))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var categorias = categoriasApi
            .Select((categoria, index) => new CategoriaTienda
            {
                Id = index + 1,
                Nombre = ConvertirNombreCategoria(categoria)
            })
            .ToList();

        var categoriasPorNombre = categoriasApi
            .Select((categoria, index) => new
            {
                NombreOriginal = categoria,
                Id = index + 1,
                NombreBonito = ConvertirNombreCategoria(categoria)
            })
            .ToDictionary(categoria => categoria.NombreOriginal, StringComparer.OrdinalIgnoreCase);

        var productos = apiResponse.Productos.Select(producto =>
        {
            var categoria = categoriasPorNombre[producto.Category];

            var imagenes = producto.ImageUrls.Any()
                ? producto.ImageUrls
                : new List<string> { producto.ImageUrl };

            return new ProductoTienda
            {
                Id = producto.AssetId,
                Nombre = producto.Name,
                Descripcion = producto.Description,
                CategoriaId = categoria.Id,
                CategoriaNombre = categoria.NombreBonito,
                Precio = producto.Cost,
                ImagenUrl = imagenes.FirstOrDefault() ?? producto.ImageUrl,
                ImagenesUrl = imagenes,
                Disponible = producto.Available,
                Comprado = producto.Purchased
            };
        }).ToList();

        return new TiendaViewModel
        {
            Productos = productos,
            Categorias = categorias,
            CreditosUsuario = apiResponse.Usuario?.Credits ?? 0
        };
    }

    public async Task<CompraTiendaResponse> ComprarProductoAsync(int usuarioId, int productoId)
    {
        var compra = new
        {
            usuario_id = usuarioId,
            asset_id = productoId
        };

        var json = JsonSerializer.Serialize(compra);
        var contenido = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/tienda/comprar", contenido);
        var responseJson = await response.Content.ReadAsStringAsync();

        var apiResponse = JsonSerializer.Deserialize<ComprarTiendaApiResponse>(responseJson, _jsonOptions);

        if (apiResponse == null)
        {
            return new CompraTiendaResponse
            {
                Exito = false,
                Mensaje = "No se pudo leer la respuesta del API."
            };
        }

        return new CompraTiendaResponse
        {
            Exito = apiResponse.Exito,
            Mensaje = apiResponse.Mensaje,
            CreditosRestantes = apiResponse.CreditosRestantes,
            ProductoComprado = apiResponse.Producto == null ? null : new ProductoTienda
            {
                Id = apiResponse.Producto.AssetId,
                Nombre = apiResponse.Producto.Name,
                Precio = apiResponse.Producto.Cost
            }
        };
    }

    private static string ConvertirNombreCategoria(string categoria)
    {
        return categoria.ToLower() switch
        {
            "player_skins" => "Skins de jugador",
            "icons" => "Iconos",
            _ => categoria
        };
    }
}