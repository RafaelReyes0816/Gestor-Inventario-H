using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gestor_Inventario_H.Data;
using Gestor_Inventario_H.Dominio;

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
        public async Task<IActionResult> GetSuministros()
        {
            var suministros = await (from s in _context.Suministros
                                     join i in _context.Insumos on s.InsumoId equals i.Id
                                     join p in _context.Proveedores on s.ProveedorId equals p.Id
                                     where s.Estado != "Inactivo"
                                     select new
                                     {
                                         s.Codigo,
                                         CodigoInsumo = i.Codigo,
                                         CodigoProveedor = p.Codigo
                                     }).ToListAsync();
            return Ok(suministros);
        }

        // GET: api/Suministros
        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetSuministro(string codigo)
        {
            var suministro = await (from s in _context.Suministros
                                    join i in _context.Insumos on s.InsumoId equals i.Id
                                    join p in _context.Proveedores on s.ProveedorId equals p.Id
                                    where s.Codigo == codigo && s.Estado != "Inactivo"
                                    select new
                                    {
                                        s.Codigo,
                                        CodigoInsumo = i.Codigo,
                                        NombreInsumo = i.Nombre,
                                        CodigoProveedor = p.Codigo,
                                        NombreProveedor = p.Nombre
                                    }).FirstOrDefaultAsync();

            if (suministro == null)
                return NotFound(new { mensaje = "Suministro no encontrado" });

            return Ok(suministro);
        }
        
        // Punto 7: Crear una consulta que utilice JOIN entre 3 tablas,
        // sin exponer el Id ni el Estado(Insumo + Suministro + Proveedor)
        // y devuelva información relevante del módulo.
        // GET: api/Suministros/Detalle
        [HttpGet("Detalle")]
        public async Task<IActionResult> GetSuministrosDetalle()
        {
            var detalle = await (from i in _context.Insumos
                                 join s in _context.Suministros on i.Id equals s.InsumoId
                                 join p in _context.Proveedores on s.ProveedorId equals p.Id
                                 where s.Estado != "Inactivo" && i.Estado != "Inactivo" && p.Estado != "Inactivo"
                                 select new
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
        public async Task<IActionResult> PostSuministro(string codigo, string codigoInsumo, string codigoProveedor)
        {
            bool codigoExiste = await _context.Suministros.AnyAsync(s => s.Codigo == codigo);
            if (codigoExiste)
                return BadRequest(new { mensaje = "El código de suministro ya existe" });

            var insumo = await (from i in _context.Insumos
                                where i.Codigo == codigoInsumo && i.Estado != "Inactivo"
                                select i).FirstOrDefaultAsync();

            if (insumo == null)
                return BadRequest(new { mensaje = "Insumo no encontrado o inactivo" });

            var proveedor = await (from p in _context.Proveedores
                                   where p.Codigo == codigoProveedor && p.Estado != "Inactivo"
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
                Codigo = codigo,
                InsumoId = insumo.Id,
                ProveedorId = proveedor.Id,
                Estado = "Activo"
            };
            _context.Suministros.Add(suministro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSuministro), new { codigo = suministro.Codigo },
                new { mensaje = "Relación de suministro creada con éxito", suministro.Codigo });
        }

        // DELETE: api/Suministros - Por código utilizando Soft Delete
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
