using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gestor_Inventario_H.Dominio
{
    public class DetalleMovimiento
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public int MovimientoId { get; set; }
        public int InsumoId { get; set; }
        public int ProveedorId { get; set; }
        public int AlmacenId { get; set; }
        public string Lote { get; set; } = null!;
        public DateTime? FechaVencimiento { get; set; }
        public int Cantidad { get; set; }
        public string Estado { get; set; } = "Activo";

        [ForeignKey("MovimientoId")]
        [JsonIgnore]
        public Movimiento? Movimiento { get; set; }

        [ForeignKey("InsumoId")]
        [JsonIgnore]
        public Insumo? Insumo { get; set; }

        [ForeignKey("ProveedorId")]
        [JsonIgnore]
        public Proveedor? Proveedor { get; set; }

        [ForeignKey("AlmacenId")]
        [JsonIgnore]
        public Almacen? Almacen { get; set; }
    }
}
