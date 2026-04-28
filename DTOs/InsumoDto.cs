namespace Gestor_Inventario_H.DTOs
{
    public class InsumoRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string CodigoCategoria { get; set; } = null!;
    }

    public class InsumoUpdateDto
    {
        public string NuevoNombre { get; set; } = null!;
        public string NuevaDescripcion { get; set; } = null!;
        public string CodigoCategoria { get; set; } = null!;
    }

    public class InsumoResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }

    public class InsumoPorCategoriaDto
    {
        public string CodigoInsumo { get; set; } = null!;
        public string NombreInsumo { get; set; } = null!;
        public string DescripcionInsumo { get; set; } = null!;
        public string CodigoCategoria { get; set; } = null!;
        public string NombreCategoria { get; set; } = null!;
    }
}
