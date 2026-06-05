namespace CosmicRunnerFront.Models.TiendaModels;

public class CompraTiendaResponse
{
    // Resultado API
    public bool Exito { get; set; }

    public string Mensaje { get; set; } = "";

    public int CreditosRestantes { get; set; }
}