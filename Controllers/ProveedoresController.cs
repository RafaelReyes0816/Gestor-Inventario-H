using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;
using Gestor_Inventario_H.DTOs;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorResponseDto>>> GetProveedores()
        {
            var proveedores = await (from p in _context.Proveedores
                                     where p.Estado != "Inactivo"
                                     select new ProveedorResponseDto
                                     {
                                         Codigo = p.Codigo,
                                         Nombre = p.Nombre
                                     }).ToListAsync();
            return Ok(proveedores);
        }

        // GET: api/Proveedores/PROV-001
        [HttpGet("{codigo}")]
        public async Task<ActionResult<ProveedorResponseDto>> GetProveedor(string codigo)
        {
            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigo && p.Estado != "Inactivo"
                                   select new ProveedorResponseDto
                                   {
                                       Codigo = p.Codigo,
                                       Nombre = p.Nombre
                                   }).FirstOrDefaultAsync();

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado" });

            return Ok(proveedor);
        }

        // POST: api/Proveedores
        [HttpPost]
        public async Task<ActionResult<ProveedorResponseDto>> PostProveedor([FromBody] ProveedorRequestDto dto)
        {
            bool existe = await _context.Proveedores.AnyAsync(p => p.Codigo == dto.Codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código ya existe en la base de datos" });

            Proveedor proveedor = new Proveedor()
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Estado = "Activo"
            };
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedor), new { codigo = proveedor.Codigo },
                new { mensaje = "Proveedor creado con éxito", proveedor.Codigo });
        }

        // PUT: api/Proveedores/PROV-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutProveedor(string codigo, [FromBody] ProveedorUpdateDto dto)
        {
            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigo && p.Estado != "Inactivo"
                                   select p).FirstOrDefaultAsync();

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado" });

            proveedor.Nombre = dto.NuevoNombre;
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Proveedor actualizado con éxito" });
        }

        // DELETE: api/Proveedores/PROV-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteProveedor(string codigo)
        {
            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigo && p.Estado != "Inactivo"
                                   select p).FirstOrDefaultAsync();

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado" });

            proveedor.Estado = "Inactivo";
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Proveedor desactivado correctamente" });
        }
    }
}
