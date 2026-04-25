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
        public DbSet<Suministro> Suministros { get; set; } = default!;
    }
}
