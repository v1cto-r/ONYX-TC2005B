// Simulamos la base de datos mientras se desarrolla el API

using CosmicRunnerFront.Models.TiendaModels;

namespace CosmicRunnerFront.DataTienda
{
    public static class TiendaMockDatabase
    {
        // Creditos
        public static int CreditosUsuario = 200000;

        // filtros de la tienda
        public static List<CategoriaTienda> Categorias = new List<CategoriaTienda>
        {
            new CategoriaTienda { Id = 1, Nombre = "Cosméticos del jugador" },
            new CategoriaTienda { Id = 2, Nombre = "Cosméticos de la nave" },
            new CategoriaTienda { Id = 3, Nombre = "Íconos de cuenta" }
        };

        // Productos de prueba
        public static List<ProductoTienda> Productos = new List<ProductoTienda>
        {
            new ProductoTienda
            {
                Id = 1,
                Nombre = "Skin jugador neón",
                Descripcion = "Cosmético visual para cambiar la apariencia del jugador dentro del juego.",
                CategoriaId = 1,
                CategoriaNombre = "Cosméticos del jugador",
                Precio = 45000,
                ImagenUrl = "/assets/tienda/jugador_neon.png",
                Disponible = true
            },
            new ProductoTienda
            {
                Id = 2,
                Nombre = "Skin jugador sombra",
                Descripcion = "Cosmético visual con un estilo más oscuro para el personaje.",
                CategoriaId = 1,
                CategoriaNombre = "Cosméticos del jugador",
                Precio = 60000,
                ImagenUrl = "/assets/tienda/jugador_sombra.png",
                Disponible = true
            },
            new ProductoTienda
            {
                Id = 3,
                Nombre = "Nave amarilla",
                Descripcion = "Cosmético para cambiar el aspecto de la nave durante la partida.",
                CategoriaId = 2,
                CategoriaNombre = "Cosméticos de la nave",
                Precio = 75000,
                ImagenUrl = "/assets/tienda/nave_amarilla.png",
                Disponible = true
            },
            new ProductoTienda
            {
                Id = 4,
                Nombre = "Nave azul",
                Descripcion = "Cosmético de nave con estilo espacial azul.",
                CategoriaId = 2,
                CategoriaNombre = "Cosméticos de la nave",
                Precio = 80000,
                ImagenUrl = "/assets/tienda/nave_azul.png",
                Disponible = true
            },
            new ProductoTienda
            {
                Id = 5,
                Nombre = "Ícono estrella",
                Descripcion = "Ícono decorativo para mostrar en la cuenta del jugador.",
                CategoriaId = 3,
                CategoriaNombre = "Íconos de cuenta",
                Precio = 25000,
                ImagenUrl = "/assets/tienda/icono_estrella.png",
                Disponible = true
            },
            new ProductoTienda
            {
                Id = 6,
                Nombre = "Ícono galaxia",
                Descripcion = "Ícono de cuenta con estilo de galaxia.",
                CategoriaId = 3,
                CategoriaNombre = "Íconos de cuenta",
                Precio = 30000,
                ImagenUrl = "/assets/tienda/icono_galaxia.png",
                Disponible = true
            }
        };

        public static List<int> ProductosComprados = new List<int>();

        public static List<ProductoTienda> ObtenerProductos()
        {

            return Productos.Select(producto => new ProductoTienda
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = producto.CategoriaNombre,
                Precio = producto.Precio,
                ImagenUrl = producto.ImagenUrl,
                Disponible = producto.Disponible,
                Comprado = ProductosComprados.Contains(producto.Id)
            }).ToList();
        }

        public static CompraTiendaResponse ComprarProducto(int productoId)
        {

            var producto = Productos.FirstOrDefault(p => p.Id == productoId);

            if (producto == null)
            {
                return new CompraTiendaResponse
                {
                    Exito = false,
                    Mensaje = "No se encontró el producto seleccionado.",
                    CreditosRestantes = CreditosUsuario
                };
            }

            if (!producto.Disponible)
            {
                return new CompraTiendaResponse
                {
                    Exito = false,
                    Mensaje = "Este producto no está disponible por el momento.",
                    CreditosRestantes = CreditosUsuario
                };
            }

            if (ProductosComprados.Contains(producto.Id))
            {
                return new CompraTiendaResponse
                {
                    Exito = false,
                    Mensaje = "Este producto ya fue comprado anteriormente.",
                    CreditosRestantes = CreditosUsuario
                };
            }

            if (CreditosUsuario < producto.Precio)
            {
                return new CompraTiendaResponse
                {
                    Exito = false,
                    Mensaje = "No tienes créditos suficientes para comprar este producto.",
                    CreditosRestantes = CreditosUsuario
                };
            }

            CreditosUsuario -= producto.Precio;
            ProductosComprados.Add(producto.Id);

            return new CompraTiendaResponse
            {
                Exito = true,
                Mensaje = "Compra realizada con éxito.",
                CreditosRestantes = CreditosUsuario,
                ProductoComprado = producto
            };
        }
    }
}