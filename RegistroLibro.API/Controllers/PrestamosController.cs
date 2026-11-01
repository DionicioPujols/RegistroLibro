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
    public async Task<ActionResult<List<PrestamoDto>>> GetPrestamos()
    {
        var prestamos = await contexto.Prestamos
            .Include(p => p.Estudiante)
            .Include(p => p.Libro)
            .AsNoTracking()
            .ToListAsync();

        return Ok(prestamos.Adapt<List<PrestamoDto>>());
    }

    // GET: api/prestamos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PrestamoDto>> GetPrestamo(int id)
    {
        var prestamo = await contexto.Prestamos
            .Include(p => p.Estudiante)
            .Include(p => p.Libro)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PrestamoId == id);

        if (prestamo == null)
        {
            return NotFound();
        }

        return Ok(prestamo.Adapt<PrestamoDto>());
    }

    // POST: api/prestamos
    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> PostPrestamo(Prestamo prestamo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var estudianteExiste = await contexto.Estudiantes
            .AnyAsync(e => e.EstudianteId == prestamo.EstudianteId);
        var libroExiste = await contexto.Libros
            .AnyAsync(l => l.LibroId == prestamo.LibroId);

        if (!estudianteExiste || !libroExiste)
        {
            return BadRequest("El estudiante o el libro indicado no existe.");
        }

        contexto.Prestamos.Add(prestamo);
        await contexto.SaveChangesAsync();

        return Ok(prestamo.Adapt<PrestamoDto>());
    }

    // PUT: api/prestamos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPrestamo(int id, Prestamo prestamo)
    {
        if (id != prestamo.PrestamoId)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del préstamo.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var prestamoExistente = await contexto.Prestamos.FindAsync(id);
        if (prestamoExistente == null)
        {
            return NotFound();
        }

        var estudianteExiste = await contexto.Estudiantes
            .AnyAsync(e => e.EstudianteId == prestamo.EstudianteId);
        var libroExiste = await contexto.Libros
            .AnyAsync(l => l.LibroId == prestamo.LibroId);

        if (!estudianteExiste || !libroExiste)
        {
            return BadRequest("El estudiante o el libro indicado no existe.");
        }

        prestamoExistente.Fecha = prestamo.Fecha;
        prestamoExistente.EstudianteId = prestamo.EstudianteId;
        prestamoExistente.LibroId = prestamo.LibroId;
        prestamoExistente.Devuelto = prestamo.Devuelto;

        await contexto.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/prestamos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePrestamo(int id)
    {
        var prestamo = await contexto.Prestamos.FindAsync(id);
        if (prestamo == null)
        {
            return NotFound();
        }

        var tieneDevoluciones = await contexto.Devoluciones
            .AnyAsync(d => d.PrestamoId == id);
        if (tieneDevoluciones)
        {
            return Conflict("No se puede eliminar el préstamo porque tiene devoluciones asociadas.");
        }

        contexto.Prestamos.Remove(prestamo);
        await contexto.SaveChangesAsync();

        return NoContent();
    }
}
