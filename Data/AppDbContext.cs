using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Dominio;

namespace Gestor_Inventario_H.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; } = default!;
        public DbSet<Insumo> Insumos { get; set; } = default!;
        public DbSet<Proveedor> Proveedores { get; set; } = default!;
        public DbSet<Almacen> Almacenes { get; set; } = default!;
        public DbSet<Suministro> Suministros { get; set; } = default!;
        public DbSet<Distribucion> Distribuciones { get; set; } = default!;
        public DbSet<Logistica> Logisticas { get; set; } = default!;
        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<Movimiento> Movimientos { get; set; } = default!;
        public DbSet<DetalleMovimiento> DetalleMovimientos { get; set; } = default!;
        public DbSet<Cama> Camas { get; set; } = default!;
    }
}
