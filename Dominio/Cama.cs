using System.ComponentModel.DataAnnotations;

namespace Gestor_Inventario_H.Dominio
{
    public class Cama
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Sala { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Estado { get; set; } = "Activo";
    }
}
