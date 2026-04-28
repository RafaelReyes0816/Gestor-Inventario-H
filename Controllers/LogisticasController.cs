using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogisticasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Logisticas
        [HttpGet]
        public async Task<IActionResult> GetLogisticas()
        {
            var logisticas = await (from l in _context.Logisticas
                                    join p in _context.Proveedores on l.ProveedorId equals p.Id
                                    join a in _context.Almacenes on l.AlmacenId equals a.Id
                                    where l.Estado != "Inactivo"
                                    select new
                                    {
                                        l.Codigo,
                                        CodigoProveedor = p.Codigo,
                                        CodigoAlmacen = a.Codigo
                                    }).ToListAsync();
            return Ok(logisticas);
        }

        // GET: api/Logisticas/LOG-001
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetLogistica(string codigo)
        {
            var logistica = await (from l in _context.Logisticas
                                   join p in _context.Proveedores on l.ProveedorId equals p.Id
                                   join a in _context.Almacenes on l.AlmacenId equals a.Id
                                   where l.Codigo == codigo && l.Estado != "Inactivo"
                                   select new
                                   {
                                       l.Codigo,
                                       CodigoProveedor = p.Codigo,
                                       NombreProveedor = p.Nombre,
                                       CodigoAlmacen = a.Codigo,
                                       NombreAlmacen = a.Nombre,
                                       UbicacionAlmacen = a.Ubicacion
                                   }).FirstOrDefaultAsync();

            if (logistica == null)
                return NotFound(new { mensaje = "Logística no encontrada" });

            return Ok(logistica);
        }

        // GET: api/Logisticas/Detalle  (JOIN 3 tablas: Proveedor + Logistica + Almacen)
        [HttpGet("Detalle")]
        public async Task<IActionResult> GetLogisticasDetalle()
        {
            var detalle = await (from p in _context.Proveedores
                                 join l in _context.Logisticas on p.Id equals l.ProveedorId
                                 join a in _context.Almacenes on l.AlmacenId equals a.Id
                                 where l.Estado != "Inactivo" && p.Estado != "Inactivo" && a.Estado != "Inactivo"
                                 select new
                                 {
                                     CodigoLogistica = l.Codigo,
                                     CodigoProveedor = p.Codigo,
                                     NombreProveedor = p.Nombre,
                                     CodigoAlmacen = a.Codigo,
                                     NombreAlmacen = a.Nombre,
                                     UbicacionAlmacen = a.Ubicacion
                                 }).ToListAsync();
            return Ok(detalle);
        }

        // POST: api/Logisticas/crear
        [HttpPost("crear")]
        public async Task<IActionResult> PostLogistica(string codigo, string codigoProveedor, string codigoAlmacen)
        {
            bool codigoExiste = await _context.Logisticas.AnyAsync(l => l.Codigo == codigo);
            if (codigoExiste)
                return BadRequest(new { mensaje = "El código de logística ya existe" });

            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigoProveedor && p.Estado != "Inactivo"
                                   select p).FirstOrDefaultAsync();

            if (proveedor == null)
                return BadRequest(new { mensaje = "Proveedor no encontrado o inactivo" });

            var almacen = await (from a in _context.Almacenes
                                 where a.Codigo == codigoAlmacen && a.Estado != "Inactivo"
                                 select a).FirstOrDefaultAsync();

            if (almacen == null)
                return BadRequest(new { mensaje = "Almacén no encontrado o inactivo" });

            bool relacionExiste = await _context.Logisticas.AnyAsync(l =>
                l.ProveedorId == proveedor.Id &&
                l.AlmacenId == almacen.Id &&
                l.Estado != "Inactivo");

            if (relacionExiste)
                return BadRequest(new { mensaje = "Este proveedor ya opera en ese almacén" });

            Logistica logistica = new Logistica()
            {
                Codigo = codigo,
                ProveedorId = proveedor.Id,
                AlmacenId = almacen.Id,
                Estado = "Activo"
            };
            _context.Logisticas.Add(logistica);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogistica), new { codigo = logistica.Codigo },
                new { mensaje = "Logística creada con éxito", logistica.Codigo });
        }

        // DELETE: api/Logisticas/LOG-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteLogistica(string codigo)
        {
            var logistica = await (from l in _context.Logisticas
                                   where l.Codigo == codigo && l.Estado != "Inactivo"
                                   select l).FirstOrDefaultAsync();

            if (logistica == null)
                return NotFound(new { mensaje = "Logística no encontrada" });

            logistica.Estado = "Inactivo";
            _context.Logisticas.Update(logistica);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Logística desactivada correctamente" });
        }
    }
}
