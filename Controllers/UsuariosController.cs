using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await (from u in _context.Usuarios
                                  where u.Estado != "Inactivo"
                                  select new
                                  {
                                      u.Codigo,
                                      u.Nombre,
                                      u.Rol
                                  }).ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/Usuarios/USR-001
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetUsuario(string codigo)
        {
            var usuario = await (from u in _context.Usuarios
                                 where u.Codigo == codigo && u.Estado != "Inactivo"
                                 select new
                                 {
                                     u.Codigo,
                                     u.Nombre,
                                     u.Rol
                                 }).FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            return Ok(usuario);
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<IActionResult> PostUsuario(string codigo, string nombre, string rol)
        {
            bool existe = await _context.Usuarios.AnyAsync(u => u.Codigo == codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código ya existe en la base de datos" });

            Usuario usuario = new Usuario()
            {
                Codigo = codigo,
                Nombre = nombre,
                Rol = rol,
                Estado = "Activo"
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { codigo = usuario.Codigo },
                new { mensaje = "Usuario creado con éxito", usuario.Codigo });
        }

        // PUT: api/Usuarios/USR-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutUsuario(string codigo, string nuevoNombre, string nuevoRol)
        {
            var usuario = await (from u in _context.Usuarios
                                 where u.Codigo == codigo && u.Estado != "Inactivo"
                                 select u).FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            usuario.Nombre = nuevoNombre;
            usuario.Rol = nuevoRol;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario actualizado con éxito" });
        }

        // DELETE: api/Usuarios/USR-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteUsuario(string codigo)
        {
            var usuario = await (from u in _context.Usuarios
                                 where u.Codigo == codigo && u.Estado != "Inactivo"
                                 select u).FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            usuario.Estado = "Inactivo";
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario desactivado correctamente" });
        }
    }
}
