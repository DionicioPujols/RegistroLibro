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
    public async Task<ActionResult<List<LibroDto>>> Get()
    {
        var libros = await contexto.Libros.AsNoTracking().ToListAsync();
        return Ok(libros.Adapt<List<LibroDto>>());
    }

    // GET: api/libros/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LibroDto>> GetById(int id)
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
    public async Task<ActionResult<LibroDto>> Post([FromBody] Libro libro)
    {
        contexto.Libros.Add(libro);
        await contexto.SaveChangesAsync();

        return Ok(libro.Adapt<LibroDto>());
    }

    // PUT: api/libros/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Libro libro)
    {
        if (id != libro.LibroId)
        {
            return BadRequest();
        }

        if (!await contexto.Libros.AnyAsync(l => l.LibroId == id))
        {
            return NotFound();
        }

        contexto.Entry(libro).State = EntityState.Modified;
        await contexto.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/libros/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await contexto.Libros.FindAsync(id);

        if (libro == null)
        {
            return NotFound();
        }

        if (await contexto.Prestamos.AnyAsync(p => p.LibroId == id) ||
            await contexto.EntradasDetalles.AnyAsync(e => e.LibroId == id))
        {
            return Conflict("No se puede eliminar el libro porque tiene préstamos o entradas asociados.");
        }

        contexto.Libros.Remove(libro);
        await contexto.SaveChangesAsync();

        return NoContent();
    }
}
