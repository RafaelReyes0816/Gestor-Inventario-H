using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;
using Gestor_Inventario_H.DTOs;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuministrosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuministrosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Suministros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuministroResponseDto>>> GetSuministros()
        {
            var suministros = await (from s in _context.Suministros
                                     join i in _context.Insumos on s.InsumoId equals i.Id
                                     join p in _context.Proveedores on s.ProveedorId equals p.Id
                                     where s.Estado != "Inactivo"
                                     select new SuministroResponseDto
                                     {
                                         Codigo = s.Codigo,
                                         CodigoInsumo = i.Codigo,
                                         CodigoProveedor = p.Codigo
                                     }).ToListAsync();
            return Ok(suministros);
        }

        // GET: api/Suministros/SUM-001
        [HttpGet("{codigo}")]
        public async Task<ActionResult<SuministroDetalleDto>> GetSuministro(string codigo)
        {
            var suministro = await (from s in _context.Suministros
                                    join i in _context.Insumos on s.InsumoId equals i.Id
                                    join p in _context.Proveedores on s.ProveedorId equals p.Id
                                    where s.Codigo == codigo && s.Estado != "Inactivo"
                                    select new SuministroDetalleDto
                                    {
                                        CodigoSuministro = s.Codigo,
                                        CodigoInsumo = i.Codigo,
                                        NombreInsumo = i.Nombre,
                                        DescripcionInsumo = i.Descripcion,
                                        CodigoProveedor = p.Codigo,
                                        NombreProveedor = p.Nombre
                                    }).FirstOrDefaultAsync();

            if (suministro == null)
                return NotFound(new { mensaje = "Suministro no encontrado" });

            return Ok(suministro);
        }

        // GET: api/Suministros/Detalle  (JOIN 3 tablas: Insumo + Suministro + Proveedor)
        [HttpGet("Detalle")]
        public async Task<ActionResult<IEnumerable<SuministroDetalleDto>>> GetSuministrosDetalle()
        {
            var detalle = await (from i in _context.Insumos
                                 join s in _context.Suministros on i.Id equals s.InsumoId
                                 join p in _context.Proveedores on s.ProveedorId equals p.Id
                                 where s.Estado != "Inactivo" && i.Estado != "Inactivo" && p.Estado != "Inactivo"
                                 select new SuministroDetalleDto
                                 {
                                     CodigoSuministro = s.Codigo,
                                     CodigoInsumo = i.Codigo,
                                     NombreInsumo = i.Nombre,
                                     DescripcionInsumo = i.Descripcion,
                                     CodigoProveedor = p.Codigo,
                                     NombreProveedor = p.Nombre
                                 }).ToListAsync();
            return Ok(detalle);
        }

        // POST: api/Suministros/crear
        [HttpPost("crear")]
        public async Task<ActionResult<SuministroResponseDto>> PostSuministro([FromBody] SuministroRequestDto dto)
        {
            bool codigoExiste = await _context.Suministros.AnyAsync(s => s.Codigo == dto.Codigo);
            if (codigoExiste)
                return BadRequest(new { mensaje = "El código de suministro ya existe" });

            var insumo = await (from i in _context.Insumos
                                where i.Codigo == dto.CodigoInsumo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();

            if (insumo == null)
                return BadRequest(new { mensaje = "Insumo no encontrado o inactivo" });

            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == dto.CodigoProveedor && p.Estado != "Inactivo"
                                   select p).FirstOrDefaultAsync();

            if (proveedor == null)
                return BadRequest(new { mensaje = "Proveedor no encontrado o inactivo" });

            bool relacionExiste = await _context.Suministros.AnyAsync(s =>
                s.InsumoId == insumo.Id &&
                s.ProveedorId == proveedor.Id &&
                s.Estado != "Inactivo");

            if (relacionExiste)
                return BadRequest(new { mensaje = "Este proveedor ya suministra este insumo" });

            Suministro suministro = new Suministro()
            {
                Codigo = dto.Codigo,
                InsumoId = insumo.Id,
                ProveedorId = proveedor.Id,
                Estado = "Activo"
            };
            _context.Suministros.Add(suministro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSuministro), new { codigo = suministro.Codigo },
                new { mensaje = "Relación de suministro creada con éxito", suministro.Codigo });
        }

        // DELETE: api/Suministros/SUM-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteSuministro(string codigo)
        {
            var suministro = await (from s in _context.Suministros
                                    where s.Codigo == codigo && s.Estado != "Inactivo"
                                    select s).FirstOrDefaultAsync();

            if (suministro == null)
                return NotFound(new { mensaje = "Suministro no encontrado" });

            suministro.Estado = "Inactivo";
            _context.Suministros.Update(suministro);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Suministro desactivado correctamente" });
        }
    }
}
