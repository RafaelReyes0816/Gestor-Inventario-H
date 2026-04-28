using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleMovimientosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DetalleMovimientosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/DetalleMovimientos
        [HttpGet]
        public async Task<IActionResult> GetDetalles()
        {
            var detalles = await (from d in _context.DetalleMovimientos
                                  join m in _context.Movimientos on d.MovimientoId equals m.Id
                                  join i in _context.Insumos on d.InsumoId equals i.Id
                                  where d.Estado != "Inactivo"
                                  select new
                                  {
                                      d.Codigo,
                                      CodigoMovimiento = m.Codigo,
                                      CodigoInsumo = i.Codigo,
                                      NombreInsumo = i.Nombre,
                                      d.Lote,
                                      d.FechaVencimiento,
                                      d.Cantidad
                                  }).ToListAsync();
            return Ok(detalles);
        }

        // GET: api/DetalleMovimientos/DET-001
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetDetalle(string codigo)
        {
            var detalle = await (from d in _context.DetalleMovimientos
                                 join m in _context.Movimientos on d.MovimientoId equals m.Id
                                 join i in _context.Insumos on d.InsumoId equals i.Id
                                 join p in _context.Proveedores on d.ProveedorId equals p.Id
                                 join a in _context.Almacenes on d.AlmacenId equals a.Id
                                 where d.Codigo == codigo && d.Estado != "Inactivo"
                                 select new
                                 {
                                     d.Codigo,
                                     CodigoMovimiento = m.Codigo,
                                     TipoMovimiento = m.TipoMovimiento,
                                     CodigoInsumo = i.Codigo,
                                     NombreInsumo = i.Nombre,
                                     CodigoProveedor = p.Codigo,
                                     NombreProveedor = p.Nombre,
                                     CodigoAlmacen = a.Codigo,
                                     NombreAlmacen = a.Nombre,
                                     d.Lote,
                                     d.FechaVencimiento,
                                     d.Cantidad
                                 }).FirstOrDefaultAsync();

            if (detalle == null)
                return NotFound(new { mensaje = "Detalle de movimiento no encontrado" });

            return Ok(detalle);
        }

        // GET: api/DetalleMovimientos/PorMovimiento/MOV-001
        [HttpGet("PorMovimiento/{codigoMovimiento}")]
        public async Task<IActionResult> GetDetallesPorMovimiento(string codigoMovimiento)
        {
            var detalles = await (from m in _context.Movimientos
                                  join d in _context.DetalleMovimientos on m.Id equals d.MovimientoId
                                  join i in _context.Insumos on d.InsumoId equals i.Id
                                  join p in _context.Proveedores on d.ProveedorId equals p.Id
                                  join a in _context.Almacenes on d.AlmacenId equals a.Id
                                  where m.Codigo == codigoMovimiento && d.Estado != "Inactivo" && m.Estado != "Inactivo"
                                  select new
                                  {
                                      CodigoDetalle = d.Codigo,
                                      CodigoInsumo = i.Codigo,
                                      NombreInsumo = i.Nombre,
                                      CodigoProveedor = p.Codigo,
                                      NombreProveedor = p.Nombre,
                                      CodigoAlmacen = a.Codigo,
                                      NombreAlmacen = a.Nombre,
                                      d.Lote,
                                      d.FechaVencimiento,
                                      d.Cantidad
                                  }).ToListAsync();
            return Ok(detalles);
        }

        // POST: api/DetalleMovimientos
        [HttpPost]
        public async Task<IActionResult> PostDetalle(
            string codigo,
            string codigoMovimiento,
            string codigoInsumo,
            string codigoProveedor,
            string codigoAlmacen,
            string lote,
            DateTime fechaVencimiento,
            int cantidad)
        {
            bool existe = await _context.DetalleMovimientos.AnyAsync(d => d.Codigo == codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código de detalle ya existe" });

            var movimiento = await (from m in _context.Movimientos
                                    where m.Codigo == codigoMovimiento && m.Estado != "Inactivo"
                                    select m).FirstOrDefaultAsync();
            if (movimiento == null)
                return BadRequest(new { mensaje = "Movimiento no encontrado o inactivo" });

            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigoInsumo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();
            if (insumo == null)
                return BadRequest(new { mensaje = "Insumo no encontrado o inactivo" });

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

            if (cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a cero" });

            DetalleMovimiento detalle = new DetalleMovimiento()
            {
                Codigo = codigo,
                MovimientoId = movimiento.Id,
                InsumoId = insumo.Id,
                ProveedorId = proveedor.Id,
                AlmacenId = almacen.Id,
                Lote = lote,
                FechaVencimiento = fechaVencimiento,
                Cantidad = cantidad,
                Estado = "Activo"
            };
            _context.DetalleMovimientos.Add(detalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDetalle), new { codigo = detalle.Codigo },
                new { mensaje = "Detalle registrado con éxito", detalle.Codigo });
        }

        // DELETE: api/DetalleMovimientos/DET-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteDetalle(string codigo)
        {
            var detalle = await (from d in _context.DetalleMovimientos
                                 where d.Codigo == codigo && d.Estado != "Inactivo"
                                 select d).FirstOrDefaultAsync();

            if (detalle == null)
                return NotFound(new { mensaje = "Detalle de movimiento no encontrado" });

            detalle.Estado = "Inactivo";
            _context.DetalleMovimientos.Update(detalle);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Detalle desactivado correctamente" });
        }
    }
}
