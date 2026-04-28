using System.ComponentModel.DataAnnotations;

namespace Gestor_Inventario_H.Dominio
{
    public class Almacen
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Ubicacion { get; set; } = null!;
        public string Estado { get; set; } = "Activo";

        public List<Distribucion>? Distribuciones { get; set; }
        public List<Logistica>? Logisticas { get; set; }
        public List<DetalleMovimiento>? DetalleMovimientos { get; set; }
    }
}
