using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;

namespace Gestor_Inventario_H.Controllers
{
    // Consultas MIS orientadas al estado del inventario y stock (UC01, UC02, UC05, UC06, UC10)
    // UC01 — Inventario de insumos por almacén (JOIN 3 tablas)
    // UC02 — Insumos próximos a vencer (JOIN 3 tablas + ORDER BY)
    // UC05 — Almacenes sin insumos asignados (NOT EXISTS)
    // UC06 — Insumos más solicitados (GROUP BY + COUNT + SUM)
    // UC10 — Insumos sin distribución a ningún almacén (NOT EXISTS)
    [Route("api/[controller]")]
    [ApiController]
    public class MisInventarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MisInventarioController(AppDbContext context)
        {
            _context = context;
        }

        // UC01 — Inventario de insumos por almacén
        // ¿Qué insumos están asignados a cada almacén?
        [HttpGet("inventario-por-almacen")]
        public async Task<IActionResult> InventarioPorAlmacen()
        {
            var resultado = await (from i in _context.Insumos
                                   join d in _context.Distribuciones on i.Id equals d.InsumoId
                                   join a in _context.Almacenes on d.AlmacenId equals a.Id
                                   where i.Estado != "Inactivo" && d.Estado != "Inactivo" && a.Estado != "Inactivo"
                                   select new
                                   {
                                       CodigoAlmacen = a.Codigo,
                                       NombreAlmacen = a.Nombre,
                                       Ubicacion = a.Ubicacion,
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       Descripcion = i.Descripcion
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC02 — Insumos próximos a vencer (próximos 90 días)
        // ¿Qué lotes vencen pronto y en qué almacén están?
        [HttpGet("proximos-a-vencer")]
        public async Task<IActionResult> ProximosAVencer()
        {
            var limite = DateTime.UtcNow.AddDays(90);
            var resultado = await (from d in _context.DetalleMovimientos
                                   join i in _context.Insumos on d.InsumoId equals i.Id
                                   join a in _context.Almacenes on d.AlmacenId equals a.Id
                                   where d.Estado != "Inactivo" && i.Estado != "Inactivo" &&
                                         a.Estado != "Inactivo" && d.FechaVencimiento <= limite
                                   orderby d.FechaVencimiento ascending
                                   select new
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       CodigoAlmacen = a.Codigo,
                                       NombreAlmacen = a.Nombre,
                                       d.Lote,
                                       d.FechaVencimiento,
                                       d.Cantidad
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC05 — Almacenes sin insumos asignados
        // ¿Qué almacenes están vacíos (sin distribución activa)?
        [HttpGet("almacenes-sin-insumos")]
        public async Task<IActionResult> AlmacenesSinInsumos()
        {
            var resultado = await (from a in _context.Almacenes
                                   where a.Estado != "Inactivo" &&
                                         !_context.Distribuciones.Any(d => d.AlmacenId == a.Id && d.Estado != "Inactivo")
                                   select new
                                   {
                                       CodigoAlmacen = a.Codigo,
                                       NombreAlmacen = a.Nombre,
                                       Ubicacion = a.Ubicacion
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC06 — Insumos más solicitados
        // ¿Qué insumos tienen mayor frecuencia y volumen de movimientos?
        [HttpGet("insumos-mas-solicitados")]
        public async Task<IActionResult> InsumosMasSolicitados()
        {
            var resultado = await (from d in _context.DetalleMovimientos
                                   join i in _context.Insumos on d.InsumoId equals i.Id
                                   where d.Estado != "Inactivo" && i.Estado != "Inactivo"
                                   group d by new { i.Codigo, i.Nombre } into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       CodigoInsumo = g.Key.Codigo,
                                       NombreInsumo = g.Key.Nombre,
                                       TotalMovimientos = g.Count(),
                                       CantidadTotal = g.Sum(x => x.Cantidad)
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC10 — Insumos sin distribución a ningún almacén
        // ¿Qué insumos existen pero no están asignados a ningún almacén?
        [HttpGet("insumos-sin-distribucion")]
        public async Task<IActionResult> InsumosSinDistribucion()
        {
            var resultado = await (from i in _context.Insumos
                                   where i.Estado != "Inactivo" &&
                                         !_context.Distribuciones.Any(d => d.InsumoId == i.Id && d.Estado != "Inactivo")
                                   select new
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       Descripcion = i.Descripcion
                                   }).ToListAsync();
            return Ok(resultado);
        }
    }
}
