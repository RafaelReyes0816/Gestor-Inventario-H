namespace Gestor_Inventario_H.DTOs
{
    public class CamaRequestDto
    {
        public string Codigo { get; set; } = null!;
        public int Cantidad { get; set; }
    }

    public class CamaUpdateDto
    {
        public int NuevaCantidad { get; set; }
    }

    public class CamaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}
