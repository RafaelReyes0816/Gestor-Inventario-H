using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;
using Gestor_Inventario_H.DTOs;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsumosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InsumosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Insumos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InsumoResponseDto>>> GetInsumos()
        {
            var insumos = await (from i in _context.Insumos
                                 where i.Estado != "Inactivo"
                                 select new InsumoResponseDto
                                 {
                                     Codigo = i.Codigo,
                                     Nombre = i.Nombre,
                                     Descripcion = i.Descripcion
                                 }).ToListAsync();
            return Ok(insumos);
        }

        // GET: api/Insumos/INS-001
        [HttpGet("{codigo}")]
        public async Task<ActionResult<InsumoResponseDto>> GetInsumo(string codigo)
        {
            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigo && i.Estado != "Inactivo"
                                select new InsumoResponseDto
                                {
                                    Codigo = i.Codigo,
                                    Nombre = i.Nombre,
                                    Descripcion = i.Descripcion
                                }).FirstOrDefaultAsync();

            if (insumo == null)
                return NotFound(new { mensaje = "Insumo no encontrado" });

            return Ok(insumo);
        }

        // GET: api/Insumos/PorCategoria  (JOIN 2 tablas: Insumo + Categoria)
        [HttpGet("PorCategoria")]
        public async Task<ActionResult<IEnumerable<InsumoPorCategoriaDto>>> GetInsumosPorCategoria()
        {
            var resultado = await (from i in _context.Insumos
                                   join c in _context.Categorias on i.CategoriaId equals c.Id
                                   where i.Estado != "Inactivo" && c.Estado != "Inactivo"
                                   select new InsumoPorCategoriaDto
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       DescripcionInsumo = i.Descripcion,
                                       CodigoCategoria = c.Codigo,
                                       NombreCategoria = c.Nombre
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // POST: api/Insumos
        [HttpPost]
        public async Task<ActionResult<InsumoResponseDto>> PostInsumo([FromBody] InsumoRequestDto dto)
        {
            bool insumoExiste = await _context.Insumos.AnyAsync(i => i.Codigo == dto.Codigo);
            if (insumoExiste)
                return BadRequest(new { mensaje = "El código de insumo ya existe en la base de datos" });

            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == dto.CodigoCategoria && c.Estado != "Inactivo"
                                   select c).FirstOrDefaultAsync();

            if (categoria == null)
                return BadRequest(new { mensaje = "Categoría no encontrada o inactiva" });

            Insumo insumo = new Insumo()
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                CategoriaId = categoria.Id,
                Estado = "Activo"
            };
            _context.Insumos.Add(insumo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInsumo), new { codigo = insumo.Codigo },
                new { mensaje = "Insumo creado con éxito", insumo.Codigo });
        }

        // PUT: api/Insumos/INS-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutInsumo(string codigo, [FromBody] InsumoUpdateDto dto)
        {
            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();

            if (insumo == null)
                return NotFound(new { mensaje = "Insumo no encontrado" });

            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == dto.CodigoCategoria && c.Estado != "Inactivo"
                                   select c).FirstOrDefaultAsync();

            if (categoria == null)
                return BadRequest(new { mensaje = "Categoría no encontrada o inactiva" });

            insumo.Nombre = dto.NuevoNombre;
            insumo.Descripcion = dto.NuevaDescripcion;
            insumo.CategoriaId = categoria.Id;
            _context.Insumos.Update(insumo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Insumo actualizado con éxito" });
        }

        // DELETE: api/Insumos/INS-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteInsumo(string codigo)
        {
            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();

            if (insumo == null)
                return NotFound(new { mensaje = "Insumo no encontrado" });

            insumo.Estado = "Inactivo";
            _context.Insumos.Update(insumo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Insumo desactivado correctamente" });
        }
    }
}
