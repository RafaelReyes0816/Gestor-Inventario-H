namespace Gestor_Inventario_H.DTOs
{
    public class SuministroRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
    }

    public class SuministroResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
    }

    public class SuministroDetalleDto
    {
        public string CodigoSuministro { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string NombreInsumo { get; set; } = null!;
        public string DescripcionInsumo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
        public string NombreProveedor { get; set; } = null!;
    }
}
