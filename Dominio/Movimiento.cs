using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gestor_Inventario_H.Dominio
{
    public class Movimiento
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = null!;
        public int UsuarioId { get; set; }
        public string Estado { get; set; } = "Activo";

        [ForeignKey("UsuarioId")]
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        public List<DetalleMovimiento>? Detalles { get; set; }
    }
}
