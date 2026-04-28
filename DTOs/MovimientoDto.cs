namespace Gestor_Inventario_H.DTOs
{
    public class MovimientoRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string TipoMovimiento { get; set; } = null!;
        public string CodigoUsuario { get; set; } = null!;
    }

    public class MovimientoUpdateDto
    {
        public string NuevoTipo { get; set; } = null!;
        public string CodigoUsuario { get; set; } = null!;
    }

    public class MovimientoResponseDto
    {
        public string Codigo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = null!;
        public string CodigoUsuario { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
    }
}
