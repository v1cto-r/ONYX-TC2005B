namespace CosmicRunnerFront.Models.TiendaModels;

public class CompraTiendaResponse
{
    // Indica si la compra paso o no
    public bool Exito { get; set; }
    // Mensaje que se muestra al usuario despues de intentar comprar
    public string Mensaje { get; set; } = "";
    public int CreditosRestantes { get; set; }
    public ProductoTienda? ProductoComprado { get; set; }
}