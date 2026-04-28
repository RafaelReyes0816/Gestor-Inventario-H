namespace Gestor_Inventario_H.DTOs
{
    public class CategoriaRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }

    public class CategoriaUpdateDto
    {
        public string NuevoNombre { get; set; } = null!;
    }

    public class CategoriaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
