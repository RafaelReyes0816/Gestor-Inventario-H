using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;

namespace Gestor_Inventario_H.Controllers
{
    // Consultas MIS orientadas a la gestión y cobertura de proveedores (UC04, UC09)
    // UC04 — Proveedores que surten cada insumo (JOIN 3 tablas)
    // UC09 — Proveedores con mayor cobertura de insumos (GROUP BY + COUNT)
    [Route("api/[controller]")]
    [ApiController]
    public class MisProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MisProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // UC04 — Proveedores que surten cada insumo
        // ¿Qué proveedor está autorizado para suministrar cada insumo?
        [HttpGet("proveedores-por-insumo")]
        public async Task<IActionResult> ProveedoresPorInsumo()
        {
            var resultado = await (from s in _context.Suministros
                                   join i in _context.Insumos on s.InsumoId equals i.Id
                                   join p in _context.Proveedores on s.ProveedorId equals p.Id
                                   where s.Estado != "Inactivo" && i.Estado != "Inactivo" && p.Estado != "Inactivo"
                                   select new
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       CodigoProveedor = p.Codigo,
                                       NombreProveedor = p.Nombre
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // UC09 — Proveedores con mayor cobertura de insumos
        // ¿Qué proveedor suministra la mayor variedad de insumos?
        [HttpGet("proveedores-mayor-cobertura")]
        public async Task<IActionResult> ProveedoresMayorCobertura()
        {
            var resultado = await (from s in _context.Suministros
                                   join p in _context.Proveedores on s.ProveedorId equals p.Id
                                   where s.Estado != "Inactivo" && p.Estado != "Inactivo"
                                   group s by new { p.Codigo, p.Nombre } into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       CodigoProveedor = g.Key.Codigo,
                                       NombreProveedor = g.Key.Nombre,
                                       TotalInsumosSuministrados = g.Count()
                                   }).ToListAsync();
            return Ok(resultado);
        }
    }
}
