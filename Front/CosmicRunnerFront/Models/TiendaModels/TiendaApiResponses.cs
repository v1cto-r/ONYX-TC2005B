using System.Text.Json.Serialization;

namespace CosmicRunnerFront.Models.TiendaModels;

public class TiendaProductosApiResponse
{
    [JsonPropertyName("exito")]
    public bool Exito { get; set; }

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = "";

    [JsonPropertyName("usuario")]
    public TiendaUsuarioApi? Usuario { get; set; }

    [JsonPropertyName("productos")]
    public List<TiendaProductoApi> Productos { get; set; } = new();
}

public class TiendaUsuarioApi
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("credits")]
    public int Credits { get; set; }
}

public class TiendaProductoApi
{
    [JsonPropertyName("asset_id")]
    public int AssetId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("cost")]
    public int Cost { get; set; }

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = "";

    [JsonPropertyName("image_urls")]
    public List<string> ImageUrls { get; set; } = new();

    [JsonPropertyName("available")]
    public bool Available { get; set; }

    [JsonPropertyName("purchased")]
    public bool Purchased { get; set; }
}

public class ComprarTiendaApiResponse
{
    [JsonPropertyName("exito")]
    public bool Exito { get; set; }

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = "";

    [JsonPropertyName("creditos_restantes")]
    public int CreditosRestantes { get; set; }

    [JsonPropertyName("producto")]
    public ProductoCompraApi? Producto { get; set; }
}

public class ProductoCompraApi
{
    [JsonPropertyName("asset_id")]
    public int AssetId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("cost")]
    public int Cost { get; set; }
}