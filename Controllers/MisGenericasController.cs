using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;

namespace Gestor_Inventario_H.Controllers
{
    // Consultas genéricas obligatorias (5)
    // G1 — JOIN entre 2 tablas
    // G2 — GROUP BY + COUNT
    // G3 — GROUP BY + SUM
    // G4 — Búsqueda filtrada por código
    // G5 — NOT EXISTS
    [Route("api/[controller]")]
    [ApiController]
    public class MisGenericasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MisGenericasController(AppDbContext context)
        {
            _context = context;
        }

        // G1 — Listado general con JOIN entre 2 tablas
        // Insumos con el nombre de su categoría asignada
        [HttpGet("listado-insumos-con-categoria")]
        public async Task<IActionResult> ListadoInsumosConCategoria()
        {
            var resultado = await (from i in _context.Insumos
                                   join c in _context.Categorias on i.CategoriaId equals c.Id
                                   where i.Estado != "Inactivo" && c.Estado != "Inactivo"
                                   select new
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       Descripcion = i.Descripcion,
                                       CodigoCategoria = c.Codigo,
                                       NombreCategoria = c.Nombre
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // G2 — Agrupación con conteo (GROUP BY + COUNT)
        // Cantidad de insumos registrados por categoría
        [HttpGet("insumos-por-categoria-conteo")]
        public async Task<IActionResult> InsumosPorCategoriaConteo()
        {
            var resultado = await (from i in _context.Insumos
                                   join c in _context.Categorias on i.CategoriaId equals c.Id
                                   where i.Estado != "Inactivo" && c.Estado != "Inactivo"
                                   group i by new { c.Codigo, c.Nombre } into g
                                   select new
                                   {
                                       CodigoCategoria = g.Key.Codigo,
                                       NombreCategoria = g.Key.Nombre,
                                       TotalInsumos = g.Count()
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // G3 — Agrupación con suma (GROUP BY + SUM)
        // Cantidad total de unidades movidas agrupada por almacén
        [HttpGet("cantidad-total-por-almacen")]
        public async Task<IActionResult> CantidadTotalPorAlmacen()
        {
            var resultado = await (from d in _context.DetalleMovimientos
                                   join a in _context.Almacenes on d.AlmacenId equals a.Id
                                   where d.Estado != "Inactivo" && a.Estado != "Inactivo"
                                   group d by new { a.Codigo, a.Nombre } into g
                                   select new
                                   {
                                       CodigoAlmacen = g.Key.Codigo,
                                       NombreAlmacen = g.Key.Nombre,
                                       CantidadTotal = g.Sum(x => x.Cantidad)
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // G4 — Búsqueda filtrada por código
        // Buscar un insumo por su código con el detalle de su categoría
        [HttpGet("buscar-insumo/{codigo}")]
        public async Task<IActionResult> BuscarInsumoPorCodigo(string codigo)
        {
            var resultado = await (from i in _context.Insumos
                                   join c in _context.Categorias on i.CategoriaId equals c.Id
                                   where i.Codigo == codigo && i.Estado != "Inactivo"
                                   select new
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       Descripcion = i.Descripcion,
                                       CodigoCategoria = c.Codigo,
                                       NombreCategoria = c.Nombre
                                   }).FirstOrDefaultAsync();

            if (resultado == null)
                return NotFound(new { mensaje = "Insumo no encontrado" });

            return Ok(resultado);
        }

        // G5 — Consulta de registros sin relación en otra tabla (NOT EXISTS)
        // Insumos que no tienen ningún proveedor asignado en Suministros
        [HttpGet("insumos-sin-proveedor")]
        public async Task<IActionResult> InsumosSinProveedor()
        {
            var resultado = await (from i in _context.Insumos
                                   where i.Estado != "Inactivo" &&
                                         !_context.Suministros.Any(s => s.InsumoId == i.Id && s.Estado != "Inactivo")
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
