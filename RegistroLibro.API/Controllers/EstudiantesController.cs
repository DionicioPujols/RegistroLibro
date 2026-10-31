using Microsoft.AspNetCore.Mvc;
using RegistroLibro.DATA.Models;
using RegistroLibro.Service.Services;

namespace RegistroLibro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EstudiantesController : ControllerBase
{
    private readonly EstudianteService _estudianteService;

    public EstudiantesController(EstudianteService estudianteService)
    {
        _estudianteService = estudianteService;
    }

    // GET: api/estudiantes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Estudiante>>> GetEstudiantes()
    {
        var result = await _estudianteService.GetList(e => true);
        return Ok(result.Value);
    }

    // GET: api/estudiantes/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Estudiante>> GetEstudiante(int id)
    {
        var result = await _estudianteService.Buscar(id);

        if (!result.IsSuccess || result.Value == null)
        {
            return NotFound();
        }

        return Ok(result.Value);
    }

    // POST: api/estudiantes
    [HttpPost]
    public async Task<ActionResult<Estudiante>> PostEstudiante(Estudiante estudiante)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _estudianteService.Guardar(estudiante);
        if (!result.IsSuccess)
        {
            return BadRequest("No se pudo guardar el estudiante.");
        }

        return CreatedAtAction(nameof(GetEstudiante), new { id = estudiante.EstudianteId }, estudiante);
    }

    // PUT: api/estudiantes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstudiante(int id, Estudiante estudiante)
    {
        if (id != estudiante.EstudianteId)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del estudiante.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _estudianteService.Existe(id))
        {
            return NotFound();
        }

        var result = await _estudianteService.Guardar(estudiante);
        if (!result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar el estudiante.");
        }

        return NoContent();
    }

    // DELETE: api/estudiantes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstudiante(int id)
    {
        if (!await _estudianteService.Existe(id))
        {
            return NotFound();
        }

        var result = await _estudianteService.Eliminar(id);
        if (!result.IsSuccess)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "No se pudo eliminar el estudiante.");
        }

        return NoContent();
    }
}
