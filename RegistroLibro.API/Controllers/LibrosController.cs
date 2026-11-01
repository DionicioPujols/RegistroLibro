using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.API.DTOs;
using RegistroLibro.DATA.Context;
using RegistroLibro.DATA.Models;

namespace RegistroLibro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LibrosController(Contexto contexto) : ControllerBase
{
    // GET: api/libros
    [HttpGet]
    public async Task<ActionResult<List<LibroDto>>> GetLibros()
    {
        var libros = await contexto.Libros.AsNoTracking().ToListAsync();
        return Ok(libros.Adapt<List<LibroDto>>());
    }

    // GET: api/libros/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LibroDto>> GetLibro(int id)
    {
        var libro = await contexto.Libros.FindAsync(id);

        if (libro == null)
        {
            return NotFound();
        }

        return Ok(libro.Adapt<LibroDto>());
    }

    // POST: api/libros
    [HttpPost]
    public async Task<ActionResult<LibroDto>> PostLibro(Libro libro)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        contexto.Libros.Add(libro);
        await contexto.SaveChangesAsync();

        return Ok(libro.Adapt<LibroDto>());
    }

    // PUT: api/libros/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutLibro(int id, Libro libro)
    {
        if (id != libro.LibroId)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del libro.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var libroExiste = await contexto.Libros.AnyAsync(l => l.LibroId == id);
        if (!libroExiste)
        {
            return NotFound();
        }

        contexto.Entry(libro).State = EntityState.Modified;
        await contexto.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/libros/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLibro(int id)
    {
        var libro = await contexto.Libros.FindAsync(id);
        if (libro == null)
        {
            return NotFound();
        }

        var tienePrestamos = await contexto.Prestamos.AnyAsync(p => p.LibroId == id);
        var tieneEntradas = await contexto.EntradasDetalles.AnyAsync(e => e.LibroId == id);
        if (tienePrestamos || tieneEntradas)
        {
            return Conflict("No se puede eliminar el libro porque tiene préstamos o entradas asociados.");
        }

        contexto.Libros.Remove(libro);
        await contexto.SaveChangesAsync();

        return NoContent();
    }
}
