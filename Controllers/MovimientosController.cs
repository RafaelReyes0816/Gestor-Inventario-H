using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet]
        public async Task<IActionResult> GetMovimientos()
        {
            var movimientos = await (from m in _context.Movimientos
                                     join u in _context.Usuarios on m.UsuarioId equals u.Id
                                     where m.Estado != "Inactivo"
                                     select new
                                     {
                                         m.Codigo,
                                         m.Fecha,
                                         m.TipoMovimiento,
                                         CodigoUsuario = u.Codigo,
                                         NombreUsuario = u.Nombre
                                     }).ToListAsync();
            return Ok(movimientos);
        }

        // GET: api/Movimientos/MOV-001
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetMovimiento(string codigo)
        {
            var movimiento = await (from m in _context.Movimientos
                                    join u in _context.Usuarios on m.UsuarioId equals u.Id
                                    where m.Codigo == codigo && m.Estado != "Inactivo"
                                    select new
                                    {
                                        m.Codigo,
                                        m.Fecha,
                                        m.TipoMovimiento,
                                        CodigoUsuario = u.Codigo,
                                        NombreUsuario = u.Nombre
                                    }).FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound(new { mensaje = "Movimiento no encontrado" });

            return Ok(movimiento);
        }

        // POST: api/Movimientos
        [HttpPost]
        public async Task<IActionResult> PostMovimiento(string codigo, string tipoMovimiento, string codigoUsuario)
        {
            bool existe = await _context.Movimientos.AnyAsync(m => m.Codigo == codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código de movimiento ya existe" });

            if (tipoMovimiento != "Entrada" && tipoMovimiento != "Salida")
                return BadRequest(new { mensaje = "El tipo de movimiento debe ser 'Entrada' o 'Salida'" });

            var usuario = await (from u in _context.Usuarios
                                 where u.Codigo == codigoUsuario && u.Estado != "Inactivo"
                                 select u).FirstOrDefaultAsync();

            if (usuario == null)
                return BadRequest(new { mensaje = "Usuario no encontrado o inactivo" });

            Movimiento movimiento = new Movimiento()
            {
                Codigo = codigo,
                Fecha = DateTime.UtcNow,
                TipoMovimiento = tipoMovimiento,
                UsuarioId = usuario.Id,
                Estado = "Activo"
            };
            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovimiento), new { codigo = movimiento.Codigo },
                new { mensaje = "Movimiento registrado con éxito", movimiento.Codigo });
        }

        // PUT: api/Movimientos/MOV-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutMovimiento(string codigo, string nuevoTipo, string codigoUsuario)
        {
            var movimiento = await (from m in _context.Movimientos
                                    where m.Codigo == codigo && m.Estado != "Inactivo"
                                    select m).FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound(new { mensaje = "Movimiento no encontrado" });

            if (nuevoTipo != "Entrada" && nuevoTipo != "Salida")
                return BadRequest(new { mensaje = "El tipo de movimiento debe ser 'Entrada' o 'Salida'" });

            var usuario = await (from u in _context.Usuarios
                                 where u.Codigo == codigoUsuario && u.Estado != "Inactivo"
                                 select u).FirstOrDefaultAsync();

            if (usuario == null)
                return BadRequest(new { mensaje = "Usuario no encontrado o inactivo" });

            movimiento.TipoMovimiento = nuevoTipo;
            movimiento.UsuarioId = usuario.Id;
            _context.Movimientos.Update(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Movimiento actualizado con éxito" });
        }

        // DELETE: api/Movimientos/MOV-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteMovimiento(string codigo)
        {
            var movimiento = await (from m in _context.Movimientos
                                    where m.Codigo == codigo && m.Estado != "Inactivo"
                                    select m).FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound(new { mensaje = "Movimiento no encontrado" });

            movimiento.Estado = "Inactivo";
            _context.Movimientos.Update(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Movimiento desactivado correctamente" });
        }
    }
}
