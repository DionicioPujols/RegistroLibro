using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.API.DTOs;
using RegistroLibro.DATA.Context;
using RegistroLibro.DATA.Models;

namespace RegistroLibro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrestamosController(Contexto contexto) : ControllerBase
{
    // GET: api/prestamos
    [HttpGet]
    public async Task<ActionResult<List<PrestamoDto>>> Get()
    {
        var prestamos = await contexto.Prestamos
            .AsNoTracking()
            .ToListAsync();

        return Ok(prestamos.Adapt<List<PrestamoDto>>());
    }

    // GET: api/prestamos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrestamoDto>> GetById(int id)
    {
        var prestamo = await contexto.Prestamos.FindAsync(id);

        if (prestamo == null)
        {
            return NotFound();
        }

        return Ok(prestamo.Adapt<PrestamoDto>());
    }

    // POST: api/prestamos
    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> Post([FromBody] Prestamo prestamo)
    {
        if (!await contexto.Estudiantes.AnyAsync(e => e.EstudianteId == prestamo.EstudianteId) ||
            !await contexto.Libros.AnyAsync(l => l.LibroId == prestamo.LibroId))
        {
            return BadRequest("El estudiante o el libro indicado no existe.");
        }

        contexto.Prestamos.Add(prestamo);
        await contexto.SaveChangesAsync();

        return Ok(prestamo.Adapt<PrestamoDto>());
    }

    // PUT: api/prestamos/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Prestamo prestamo)
    {
        if (id != prestamo.PrestamoId)
        {
            return BadRequest();
        }

        var prestamoActual = await contexto.Prestamos.FindAsync(id);
        if (prestamoActual == null)
        {
            return NotFound();
        }

        if (!await contexto.Estudiantes.AnyAsync(e => e.EstudianteId == prestamo.EstudianteId) ||
            !await contexto.Libros.AnyAsync(l => l.LibroId == prestamo.LibroId))
        {
            return BadRequest("El estudiante o el libro indicado no existe.");
        }

        prestamoActual.Fecha = prestamo.Fecha;
        prestamoActual.EstudianteId = prestamo.EstudianteId;
        prestamoActual.LibroId = prestamo.LibroId;
        prestamoActual.Devuelto = prestamo.Devuelto;

        await contexto.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/prestamos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var prestamo = await contexto.Prestamos.FindAsync(id);

        if (prestamo == null)
        {
            return NotFound();
        }

        if (await contexto.Devoluciones.AnyAsync(d => d.PrestamoId == id))
        {
            return Conflict("No se puede eliminar el préstamo porque tiene devoluciones asociadas.");
        }

        contexto.Prestamos.Remove(prestamo);
        await contexto.SaveChangesAsync();

        return NoContent();
    }
}
