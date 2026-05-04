using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;

namespace Gestor_Inventario_H.Controllers
{
    // Consultas MIS orientadas a movimientos y operaciones hospitalarias (UC03, UC07, UC08)
    // UC03 — Historial de movimientos por usuario (JOIN 2 tablas)
    // UC07 — Entradas y salidas por insumo (GROUP BY TipoMovimiento)
    // UC08 — Camas disponibles por sala (GROUP BY + COUNT)
    [Route("api/[controller]")]
    [ApiController]
    public class MisOperacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MisOperacionesController(AppDbContext context)
        {
            _context = context;
        }

        // UC03 — Historial de movimientos por usuario
        // ¿Quién registró cada movimiento y cuándo?
        [HttpGet("movimientos-por-usuario")]
        public async Task<IActionResult> MovimientosPorUsuario()
        {
            var resultado = await (from m in _context.Movimientos
                                   join u in _context.Usuarios on m.UsuarioId equals u.Id
                                   where m.Estado != "Inactivo" && u.Estado != "Inactivo"
                                   orderby u.Nombre, m.Fecha descending
                                   select new
                                   {
                                       CodigoUsuario = u.Codigo,
                                       NombreUsuario = u.Nombre,
                                       Rol = u.Rol,
                                       CodigoMovimiento = m.Codigo,
                                       m.Fecha,
                                       m.TipoMovimiento
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC07 — Entradas y salidas por insumo
        // ¿Cuántas unidades entraron y salieron de cada insumo?
        [HttpGet("entradas-salidas-por-insumo")]
        public async Task<IActionResult> EntradasSalidasPorInsumo()
        {
            var resultado = await (from d in _context.DetalleMovimientos
                                   join m in _context.Movimientos on d.MovimientoId equals m.Id
                                   join i in _context.Insumos on d.InsumoId equals i.Id
                                   where d.Estado != "Inactivo" && m.Estado != "Inactivo" && i.Estado != "Inactivo"
                                   group new { d, m } by new { i.Codigo, i.Nombre, m.TipoMovimiento } into g
                                   select new
                                   {
                                       CodigoInsumo = g.Key.Codigo,
                                       NombreInsumo = g.Key.Nombre,
                                       TipoMovimiento = g.Key.TipoMovimiento,
                                       TotalMovimientos = g.Count(),
                                       CantidadTotal = g.Sum(x => x.d.Cantidad)
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC08 — Stock total de camas disponibles
        // ¿Cuántas camas hay en total en el hospital?
        [HttpGet("total-camas")]
        public async Task<IActionResult> TotalCamas()
        {
            var resultado = await (from c in _context.Camas
                                   where c.Estado != "Inactivo"
                                   select new
                                   {
                                       c.Codigo,
                                       c.Cantidad
                                   }).ToListAsync();

            var totalGeneral = resultado.Sum(c => c.Cantidad);
            return Ok(new { TotalCamas = totalGeneral, Registros = resultado });
        }
    }
}
