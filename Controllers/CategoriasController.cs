using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;
using Gestor_Inventario_H.DTOs;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> GetCategorias()
        {
            var categorias = await (from c in _context.Categorias
                                    where c.Estado != "Inactivo"
                                    select new CategoriaResponseDto
                                    {
                                        Codigo = c.Codigo,
                                        Nombre = c.Nombre
                                    }).ToListAsync();
            return Ok(categorias);
        }

        // GET: api/Categorias/CAT-001
        [HttpGet("{codigo}")]
        public async Task<ActionResult<CategoriaResponseDto>> GetCategoria(string codigo)
        {
            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == codigo && c.Estado != "Inactivo"
                                   select new CategoriaResponseDto
                                   {
                                       Codigo = c.Codigo,
                                       Nombre = c.Nombre
                                   }).FirstOrDefaultAsync();

            if (categoria == null)
                return NotFound(new { mensaje = "Categoría no encontrada" });

            return Ok(categoria);
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<CategoriaResponseDto>> PostCategoria([FromBody] CategoriaRequestDto dto)
        {
            bool existe = await _context.Categorias.AnyAsync(c => c.Codigo == dto.Codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código ya existe en la base de datos" });

            Categoria categoria = new Categoria()
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Estado = "Activo"
            };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoria), new { codigo = categoria.Codigo },
                new { mensaje = "Categoría creada con éxito", categoria.Codigo });
        }

        // PUT: api/Categorias/CAT-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutCategoria(string codigo, [FromBody] CategoriaUpdateDto dto)
        {
            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == codigo && c.Estado != "Inactivo"
                                   select c).FirstOrDefaultAsync();

            if (categoria == null)
                return NotFound(new { mensaje = "Categoría no encontrada" });

            categoria.Nombre = dto.NuevoNombre;
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Categoría actualizada con éxito" });
        }

        // DELETE: api/Categorias/CAT-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteCategoria(string codigo)
        {
            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == codigo && c.Estado != "Inactivo"
                                   select c).FirstOrDefaultAsync();

            if (categoria == null)
                return NotFound(new { mensaje = "Categoría no encontrada" });

            categoria.Estado = "Inactivo";
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Categoría desactivada correctamente" });
        }
    }
}
