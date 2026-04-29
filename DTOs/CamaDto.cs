namespace Gestor_Inventario_H.DTOs
{
    public class CamaRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Sala { get; set; } = null!;
        public string Tipo { get; set; } = null!;
    }

    public class CamaUpdateDto
    {
        public string NuevoNumero { get; set; } = null!;
        public string NuevaSala { get; set; } = null!;
        public string NuevoTipo { get; set; } = null!;
    }

    public class CamaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string Numero { get; set; } = null!;
        public string Sala { get; set; } = null!;
        public string Tipo { get; set; } = null!;
    }
}
