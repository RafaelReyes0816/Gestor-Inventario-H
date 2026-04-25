using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

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
        public async Task<IActionResult> GetInsumos()
        {
            var insumos = await (from i in _context.Insumos
                                 where i.Estado != "Inactivo"
                                 select new
                                 {
                                     i.Codigo,
                                     i.Nombre,
                                     i.Descripcion
                                 }).ToListAsync();
            return Ok(insumos);
        }

        // GET: api/Insumos - Por código
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetInsumo(string codigo)
        {
            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigo && i.Estado != "Inactivo"
                                select new
                                {
                                    i.Codigo,
                                    i.Nombre,
                                    i.Descripcion
                                }).FirstOrDefaultAsync();

            if (insumo == null)
                return NotFound(new { mensaje = "Insumo no encontrado" });

            return Ok(insumo);
        }

        // Punto 6: Crear una consulta que utilice JOIN entre 2 tablas,
        // sin exponer el Id ni el Estado(Insumo + Categoria)
        // y devuelva información relevante del módulo.
        // GET: api/Insumos/PorCategoria
        [HttpGet("PorCategoria")]
        public async Task<IActionResult> GetInsumosPorCategoria()
        {
            var resultado = await (from i in _context.Insumos
                                   join c in _context.Categorias on i.CategoriaId equals c.Id
                                   where i.Estado != "Inactivo" && c.Estado != "Inactivo"
                                   select new
                                   {
                                       CodigoInsumo = i.Codigo,
                                       NombreInsumo = i.Nombre,
                                       DescripcionInsumo = i.Descripcion,
                                       NombreCategoria = c.Nombre,
                                       CodigoCategoria = c.Codigo
                                   }).ToListAsync();
            return Ok(resultado);
        }

        // POST: api/Insumos
        [HttpPost]
        public async Task<IActionResult> PostInsumo(string codigo, string nombre, string descripcion, string codigoCategoria)
        {
            bool insumoExiste = await _context.Insumos.AnyAsync(i => i.Codigo == codigo);
            if (insumoExiste)
                return BadRequest(new { mensaje = "El código de insumo ya existe en la base de datos" });

            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == codigoCategoria && c.Estado != "Inactivo"
                                   select c).FirstOrDefaultAsync();

            if (categoria == null)
                return BadRequest(new { mensaje = "Categoría no encontrada o inactiva" });

            Insumo insumo = new Insumo()
            {
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                CategoriaId = categoria.Id,
                Estado = "Activo"
            };
            _context.Insumos.Add(insumo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInsumo), new { codigo = insumo.Codigo },
                new { mensaje = "Insumo creado con éxito", insumo.Codigo });
        }

        // PUT: api/Insumos - Por código
        [HttpPut("{codigo}")]
        public async Task<IActionResult> PutInsumo(string codigo, string nuevoNombre, string nuevaDescripcion, string codigoCategoria)
        {
            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();

            if (insumo == null)
                return NotFound(new { mensaje = "Insumo no encontrado" });

            var categoria = await (from c in _context.Categorias
                                   where c.Codigo == codigoCategoria && c.Estado != "Inactivo"
                                   select c).FirstOrDefaultAsync();

            if (categoria == null)
                return BadRequest(new { mensaje = "Categoría no encontrada o inactiva" });

            insumo.Nombre = nuevoNombre;
            insumo.Descripcion = nuevaDescripcion;
            insumo.CategoriaId = categoria.Id;
            _context.Insumos.Update(insumo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Insumo actualizado con éxito" });
        }

        // DELETE: api/Insumos - Por código utilizando Soft Delete
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
