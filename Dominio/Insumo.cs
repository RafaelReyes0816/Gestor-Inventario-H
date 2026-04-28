using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gestor_Inventario_H.Dominio
{
    public class Insumo
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int CategoriaId { get; set; }
        public string Estado { get; set; } = "Activo";

        [ForeignKey("CategoriaId")]
        [JsonIgnore]
        public Categoria? Categoria { get; set; }

        public List<Suministro>? Suministros { get; set; }
        public List<Distribucion>? Distribuciones { get; set; }
        public List<DetalleMovimiento>? DetalleMovimientos { get; set; }
    }
}
