using System.Collections.Generic;

namespace CosmicRunnerFront.Models.InicioModels
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
        // Relación inversa
        public List<Usuario> ListaUsuarios { get; set; } = new List<Usuario>();
    }
}