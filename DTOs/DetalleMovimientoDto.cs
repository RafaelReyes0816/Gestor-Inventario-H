namespace Gestor_Inventario_H.DTOs
{
    public class DetalleMovimientoRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoMovimiento { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
        public string Lote { get; set; } = null!;
        public DateTime? FechaVencimiento { get; set; }
        public int Cantidad { get; set; }
    }

    public class DetalleMovimientoResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoMovimiento { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string NombreInsumo { get; set; } = null!;
        public string Lote { get; set; } = null!;
        public DateTime? FechaVencimiento { get; set; }
        public int Cantidad { get; set; }
    }

    public class DetalleMovimientoCompletoDto
    {
        public string Codigo { get; set; } = null!;
        public string CodigoMovimiento { get; set; } = null!;
        public string TipoMovimiento { get; set; } = null!;
        public string CodigoInsumo { get; set; } = null!;
        public string NombreInsumo { get; set; } = null!;
        public string CodigoProveedor { get; set; } = null!;
        public string NombreProveedor { get; set; } = null!;
        public string CodigoAlmacen { get; set; } = null!;
        public string NombreAlmacen { get; set; } = null!;
        public string Lote { get; set; } = null!;
        public DateTime? FechaVencimiento { get; set; }
        public int Cantidad { get; set; }
    }
}
