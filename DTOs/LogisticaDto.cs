namespace Gestor_Inventario_H.DTOs
{
    public class LogisticaRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
    }

    public class LogisticaResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
    }

    public class LogisticaDetalleDto
    {
        public string CodigoLogistica { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
        public string NombreProveedor { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
        public string NombreAlmacen { get; set; } = null!;
        public string UbicacionAlmacen { get; set; } = null!;
    }
}
