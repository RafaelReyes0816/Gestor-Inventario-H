using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

namespace Gestor_Inventario_H.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlmacenesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Almacenes
        [HttpGet]
        public async Task<IActionResult> GetAlmacenes()
        {
            var almacenes = await (from a in _context.Almacenes
                                   where a.Estado != "Inactivo"
                                   select new
                                   {
                                       a.Codigo,
                                       a.Nombre,
                                       a.Ubicacion
                                   }).ToListAsync();
            return Ok(almacenes);
        }

        // GET: api/Almacenes/ALM-001
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetAlmacen(string codigo)
        {
            var almacen = await (from a in _context.Almacenes
                                 where a.Codigo == codigo && a.Estado != "Inactivo"
                                 select new
                                 {
                                     a.Codigo,
                                     a.Nombre,
                                     a.Ubicacion
                                 }).FirstOrDefaultAsync();

            if (almacen == null)
                return NotFound(new { mensaje = "Almacén no encontrado" });

            return Ok(almacen);
        }

        // POST: api/Almacenes
        [HttpPost]
        public async Task<IActionResult> PostAlmacen(string codigo, string nombre, string ubicacion)
        {
            bool existe = await _context.Almacenes.AnyAsync(a => a.Codigo == codigo);
            if (existe)
                return BadRequest(new { mensaje = "El código ya existe en la base de datos" });

            Almacen almacen = new Almacen()
            {
                Codigo = codigo,
                Nombre = nombre,
                Ubicacion = ubicacion,
                Estado = "Activo"
            };
            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAlmacen), new { codigo = almacen.Codigo },
                new { mensaje = "Almacén creado con éxito", almacen.Codigo });
        }

        // PUT: api/Almacenes/ALM-001
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutAlmacen(string codigo, string nuevoNombre, string nuevaUbicacion)
        {
            var almacen = await (from a in _context.Almacenes
                                 where a.Codigo == codigo && a.Estado != "Inactivo"
                                 select a).FirstOrDefaultAsync();

            if (almacen == null)
                return NotFound(new { mensaje = "Almacén no encontrado" });

            almacen.Nombre = nuevoNombre;
            almacen.Ubicacion = nuevaUbicacion;
            _context.Almacenes.Update(almacen);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Almacén actualizado con éxito" });
        }

        // DELETE: api/Almacenes/ALM-001
        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteAlmacen(string codigo)
        {
            var almacen = await (from a in _context.Almacenes
                                 where a.Codigo == codigo && a.Estado != "Inactivo"
                                 select a).FirstOrDefaultAsync();

            if (almacen == null)
                return NotFound(new { mensaje = "Almacén no encontrado" });

            almacen.Estado = "Inactivo";
            _context.Almacenes.Update(almacen);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Almacén desactivado correctamente" });
        }
    }
}
