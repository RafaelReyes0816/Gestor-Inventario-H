using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

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
        public async Task<IActionResult> GetProveedores()
        {
            var proveedores = await (from p in _context.Proveedores
                                     where p.Estado != "Inactivo"
                                     select new
                                     {
                                         p.Codigo,
                                         p.Nombre
                                     }).ToListAsync();
            return Ok(proveedores);
        }

        // GET: api/Proveedores - Por código
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetProveedor(string codigo)
        {
            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigo && p.Estado != "Inactivo"
                                   select new
                                   {
                                       p.Codigo,
                                       p.Nombre
                                   }).FirstOrDefaultAsync();

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado" });

            return Ok(proveedor);
        }

        // POST: api/Proveedores
        [HttpPost]
        public async Task<IActionResult> PostProveedor(string codigo, string nombre)
        {
            bool existe = await _context.Proveedores.AnyAsync(p => p.Codigo == codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código ya existe en la base de datos" });

            Proveedor proveedor = new Proveedor()
            {
                Codigo = codigo,
                Nombre = nombre,
                Estado = "Activo"
            };
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedor), new { codigo = proveedor.Codigo },
                new { mensaje = "Proveedor creado con éxito", proveedor.Codigo });
        }

        // PUT: api/Proveedores - Por código
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutProveedor(string codigo, string nuevoNombre)
        {
            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigo && p.Estado != "Inactivo"
                                   select p).FirstOrDefaultAsync();

            if (proveedor == null)
                return NotFound(new { mensaje = "Proveedor no encontrado" });

            proveedor.Nombre = nuevoNombre;
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Proveedor actualizado con éxito" });
        }

        // DELETE: api/Proveedores - Por código utilizando Soft Delete
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
