using Mapster;
using Microsoft.AspNetCore.Mvc;
using RegistroLibro.API.DTOs;
using RegistroLibro.DATA.Models;
using RegistroLibro.Service.Services;

namespace RegistroLibro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LibrosController(LibrosService librosService) : ControllerBase
{
    // GET: api/libros
    [HttpGet]
    public async Task<ActionResult<List<LibroDto>>> Get()
    {
        var result = await librosService.GetList(l => l.LibroId > 0);
        return Ok(result.Value.Adapt<List<LibroDto>>());
    }

    // GET: api/libros/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LibroDto>> GetById(int id)
    {
        var result = await librosService.Buscar(id);

        if (result.Value == null)
        {
            return NotFound();
        }

        return Ok(result.Value.Adapt<LibroDto>());
    }

    // POST: api/libros
    [HttpPost]
    public async Task<ActionResult<LibroDto>> Post([FromBody] Libro libro)
    {
        await librosService.Guardar(libro);
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

        var libroActual = await librosService.Buscar(id);
        if (libroActual.Value == null)
        {
            return NotFound();
        }

        await librosService.Guardar(libro);
        return NoContent();
    }

    // DELETE: api/libros/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await librosService.Buscar(id);
        if (libro.Value == null)
        {
            return NotFound();
        }

        var resultado = await librosService.Eliminar(id);
        var eliminacion = (Aplicada1.Core.Result<bool>)resultado;
        if (!eliminacion.Value)
        {
            return Conflict("No se puede eliminar el libro porque tiene préstamos o entradas asociados.");
        }

        return NoContent();
    }
}
