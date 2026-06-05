namespace CosmicRunnerFront.Models.TiendaModels;

public class TiendaViewModel
{
    public List<ProductoTienda> Productos { get; set; } = new();

    public List<CategoriaTienda> Categorias { get; set; } = new();

    public int CreditosUsuario { get; set; }

    public string? SearchText { get; set; }

    public int? FilterCategoriaId { get; set; }
}