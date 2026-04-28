namespace Gestor_Inventario_H.DTOs
{
    public class AlmacenRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Ubicacion { get; set; } = null!;
    }

    public class AlmacenUpdateDto
    {
        public string NuevoNombre { get; set; } = null!;
        public string NuevaUbicacion { get; set; } = null!;
    }

    public class AlmacenResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Ubicacion { get; set; } = null!;
    }
}
