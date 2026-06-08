using System.Text.Json.Serialization;

namespace CosmicRunnerFront.Models.UsuarioModels;

public class HabilidadModel
{
    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;
}