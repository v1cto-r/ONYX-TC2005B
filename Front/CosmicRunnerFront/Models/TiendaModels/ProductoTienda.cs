namespace CosmicRunnerFront.Models.TiendaModels;

public class ProductoTienda
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = "";
    public int Precio { get; set; }
    public string ImagenUrl { get; set; } = "";
    public bool Disponible { get; set; } = true;
    public bool Comprado { get; set; }
}