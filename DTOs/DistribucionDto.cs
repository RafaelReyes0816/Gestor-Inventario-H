namespace Gestor_Inventario_H.DTOs
{
    public class DistribucionRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
    }

    public class DistribucionResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
    }

    public class DistribucionDetalleDto
    {
        public string CodigoDistribucion { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string NombreInsumo { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
        public string NombreAlmacen { get; set; } = null!;
        public string UbicacionAlmacen { get; set; } = null!;
    }
}
