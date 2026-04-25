using System.ComponentModel.DataAnnotations;

namespace Gestor_Inventario_H.Dominio
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Estado { get; set; } = "Activo";

        public List<Insumo>? Insumos { get; set; }
    }
}
