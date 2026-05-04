using System.ComponentModel.DataAnnotations;

namespace Gestor_Inventario_H.Dominio
{
    public class Cama
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public int Cantidad { get; set; }
        public string Estado { get; set; } = "Activo";
    }
}
