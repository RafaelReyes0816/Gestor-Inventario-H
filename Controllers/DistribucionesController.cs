using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;
using Gestor_Inventario_H.DTOs;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistribucionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DistribucionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Distribuciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistribucionResponseDto>>> GetDistribuciones()
        {
            var distribuciones = await (from d in _context.Distribuciones
                                        join i in _context.Insumos on d.InsumoId equals i.Id
                                        join a in _context.Almacenes on d.AlmacenId equals a.Id
                                        where d.Estado != "Inactivo"
                                        select new DistribucionResponseDto
                                        {
                                            Codigo = d.Codigo,
                                            CodigoInsumo = i.Codigo,
                                            CodigoAlmacen = a.Codigo
                                        }).ToListAsync();
            return Ok(distribuciones);
        }

        // GET: api/Distribuciones/DIS-001
        [HttpGet("{codigo}")]
        public async Task<ActionResult<DistribucionDetalleDto>> GetDistribucion(string codigo)
        {
            var distribucion = await (from d in _context.Distribuciones
                                      join i in _context.Insumos on d.InsumoId equals i.Id
                                      join a in _context.Almacenes on d.AlmacenId equals a.Id
                                      where d.Codigo == codigo && d.Estado != "Inactivo"
                                      select new DistribucionDetalleDto
                                      {
                                          CodigoDistribucion = d.Codigo,
                                          CodigoInsumo = i.Codigo,
                                          NombreInsumo = i.Nombre,
                                          CodigoAlmacen = a.Codigo,
                                          NombreAlmacen = a.Nombre,
                                          UbicacionAlmacen = a.Ubicacion
                                      }).FirstOrDefaultAsync();

            if (distribucion == null)
                return NotFound(new { mensaje = "Distribución no encontrada" });

            return Ok(distribucion);
        }

        // GET: api/Distribuciones/Detalle  (JOIN 3 tablas: Insumo + Distribucion + Almacen)
        [HttpGet("Detalle")]
        public async Task<ActionResult<IEnumerable<DistribucionDetalleDto>>> GetDistribucionesDetalle()
        {
            var detalle = await (from i in _context.Insumos
                                 join d in _context.Distribuciones on i.Id equals d.InsumoId
                                 join a in _context.Almacenes on d.AlmacenId equals a.Id
                                 where d.Estado != "Inactivo" && i.Estado != "Inactivo" && a.Estado != "Inactivo"
                                 select new DistribucionDetalleDto
                                 {
                                     CodigoDistribucion = d.Codigo,
                                     CodigoInsumo = i.Codigo,
                                     NombreInsumo = i.Nombre,
                                     CodigoAlmacen = a.Codigo,
                                     NombreAlmacen = a.Nombre,
                                     UbicacionAlmacen = a.Ubicacion
                                 }).ToListAsync();
            return Ok(detalle);
        }

        // POST: api/Distribuciones/crear
        [HttpPost("crear")]
        public async Task<ActionResult<DistribucionResponseDto>> PostDistribucion([FromBody] DistribucionRequestDto dto)
        {
            bool codigoExiste = await _context.Distribuciones.AnyAsync(d => d.Codigo == dto.Codigo);
            if (codigoExiste)
                return BadRequest(new { mensaje = "El código de distribución ya existe" });

            var insumo = await (from i in _context.Insumos
                                where i.Codigo == dto.CodigoInsumo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();

            if (insumo == null)
                return BadRequest(new { mensaje = "Insumo no encontrado o inactivo" });

            var almacen = await (from a in _context.Almacenes
                                 where a.Codigo == dto.CodigoAlmacen && a.Estado != "Inactivo"
                                 select a).FirstOrDefaultAsync();

            if (almacen == null)
                return BadRequest(new { mensaje = "Almacén no encontrado o inactivo" });

            bool relacionExiste = await _context.Distribuciones.AnyAsync(d =>
                d.InsumoId == insumo.Id &&
                d.AlmacenId == almacen.Id &&
                d.Estado != "Inactivo");

            if (relacionExiste)
                return BadRequest(new { mensaje = "Este insumo ya está distribuido en ese almacén" });

            Distribucion distribucion = new Distribucion()
            {
                Codigo = dto.Codigo,
                InsumoId = insumo.Id,
                AlmacenId = almacen.Id,
                Estado = "Activo"
            };
            _context.Distribuciones.Add(distribucion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDistribucion), new { codigo = distribucion.Codigo },
                new { mensaje = "Distribución creada con éxito", distribucion.Codigo });
        }

        // DELETE: api/Distribuciones/DIS-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteDistribucion(string codigo)
        {
            var distribucion = await (from d in _context.Distribuciones
                                      where d.Codigo == codigo && d.Estado != "Inactivo"
                                      select d).FirstOrDefaultAsync();

            if (distribucion == null)
                return NotFound(new { mensaje = "Distribución no encontrada" });

            distribucion.Estado = "Inactivo";
            _context.Distribuciones.Update(distribucion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Distribución desactivada correctamente" });
        }
    }
}
