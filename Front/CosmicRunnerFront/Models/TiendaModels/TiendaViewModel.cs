namespace CosmicRunnerFront.Models.TiendaModels;

public class TiendaViewModel
{
    // Productos
    public List<ProductoTienda> Productos { get; set; } = new List<ProductoTienda>();
    // Categorias del filtro de la tienda
    public List<CategoriaTienda> Categorias { get; set; } = new List<CategoriaTienda>();
    // Creditos  del usuario
    public int CreditosUsuario { get; set; }
    // Texto que el usuario escribio en el buscador
    public string? SearchText { get; set; }
    public int? FilterCategoriaId { get; set; }
}