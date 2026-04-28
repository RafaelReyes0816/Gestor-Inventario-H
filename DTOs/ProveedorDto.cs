namespace Gestor_Inventario_H.DTOs
{
    public class ProveedorRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }

    public class ProveedorUpdateDto
    {
        public string NuevoNombre { get; set; } = null!;
    }

    public class ProveedorResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}
