namespace Gestor_Inventario_H.DTOs
{
    public class UsuarioRequestDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }

    public class UsuarioUpdateDto
    {
        public string NuevoNombre { get; set; } = null!;
        public string NuevoRol { get; set; } = null!;
    }

    public class UsuarioResponseDto
    {
        public string Codigo { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}
