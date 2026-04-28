using System.ComponentModel.DataAnnotations;

namespace Gestor_Inventario_H.Dominio
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string Estado { get; set; } = "Activo";

        public List<Movimiento>? Movimientos { get; set; }
    }
}
