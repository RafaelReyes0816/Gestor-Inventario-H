using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gestor_Inventario_H.Dominio
{
    public class Suministro
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public int InsumoId { get; set; }
        public int ProveedorId { get; set; }
        public string Estado { get; set; } = "Activo";

        [ForeignKey("InsumoId")]
        [JsonIgnore]
        public Insumo? Insumo { get; set; }

        [ForeignKey("ProveedorId")]
        [JsonIgnore]
        public Proveedor? Proveedor { get; set; }
    }
}
