using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;
using Gestor_Inventario_H.DTOs;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CamasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Camas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CamaResponseDto>>> GetCamas()
        {
            var camas = await (from c in _context.Camas
                               where c.Estado != "Inactivo"
                               select new CamaResponseDto
                               {
                                   Codigo = c.Codigo,
                                   Numero = c.Numero,
                                   Sala = c.Sala,
                                   Tipo = c.Tipo
                               }).ToListAsync();
            return Ok(camas);
        }

        // GET: api/Camas/CAM-001
        [HttpGet("{codigo}")]
        public async Task<ActionResult<CamaResponseDto>> GetCama(string codigo)
        {
            var cama = await (from c in _context.Camas
                              where c.Codigo == codigo && c.Estado != "Inactivo"
                              select new CamaResponseDto
                              {
                                  Codigo = c.Codigo,
                                  Numero = c.Numero,
                                  Sala = c.Sala,
                                  Tipo = c.Tipo
                              }).FirstOrDefaultAsync();

            if (cama == null)
                return NotFound(new { mensaje = "Cama no encontrada" });

            return Ok(cama);
        }

        // GET: api/Camas/PorSala/Cirugia
        [HttpGet("PorSala/{sala}")]
        public async Task<ActionResult<IEnumerable<CamaResponseDto>>> GetCamasPorSala(string sala)
        {
            var camas = await (from c in _context.Camas
                               where c.Sala == sala && c.Estado != "Inactivo"
                               select new CamaResponseDto
                               {
                                   Codigo = c.Codigo,
                                   Numero = c.Numero,
                                   Sala = c.Sala,
                                   Tipo = c.Tipo
                               }).ToListAsync();
            return Ok(camas);
        }

        // POST: api/Camas
        [HttpPost]
        public async Task<ActionResult<CamaResponseDto>> PostCama([FromBody] CamaRequestDto dto)
        {
            bool existe = await _context.Camas.AnyAsync(c => c.Codigo == dto.Codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código ya existe en la base de datos" });

            Cama cama = new Cama()
            {
                Codigo = dto.Codigo,
                Numero = dto.Numero,
                Sala = dto.Sala,
                Tipo = dto.Tipo,
                Estado = "Activo"
            };
            _context.Camas.Add(cama);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCama), new { codigo = cama.Codigo },
                new { mensaje = "Cama registrada con éxito", cama.Codigo });
        }

        // PUT: api/Camas/CAM-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutCama(string codigo, [FromBody] CamaUpdateDto dto)
        {
            var cama = await (from c in _context.Camas
                              where c.Codigo == codigo && c.Estado != "Inactivo"
                              select c).FirstOrDefaultAsync();

            if (cama == null)
                return NotFound(new { mensaje = "Cama no encontrada" });

            cama.Numero = dto.NuevoNumero;
            cama.Sala = dto.NuevaSala;
            cama.Tipo = dto.NuevoTipo;
            _context.Camas.Update(cama);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cama actualizada con éxito" });
        }

        // DELETE: api/Camas/CAM-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteCama(string codigo)
        {
            var cama = await (from c in _context.Camas
                              where c.Codigo == codigo && c.Estado != "Inactivo"
                              select c).FirstOrDefaultAsync();

            if (cama == null)
                return NotFound(new { mensaje = "Cama no encontrada" });

            cama.Estado = "Inactivo";
            _context.Camas.Update(cama);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cama desactivada correctamente" });
        }
    }
}
