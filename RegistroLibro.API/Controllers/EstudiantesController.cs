using Mapster;
using Microsoft.AspNetCore.Mvc;
using RegistroLibro.API.DTOs;
using RegistroLibro.DATA.Models;
using RegistroLibro.Service.Services;

namespace RegistroLibro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EstudiantesController(EstudianteService estudiantesService) : ControllerBase
{
    // GET: api/estudiantes
    [HttpGet]
    public async Task<ActionResult<List<EstudianteDto>>> Get()
    {
        var resultado = await estudiantesService.GetList(e => true);

        if (resultado.IsFailure)
        {
            return BadRequest(resultado.Error);
        }

        return Ok(resultado.Value.Adapt<List<EstudianteDto>>());
    }

    // GET: api/estudiantes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EstudianteDto>> GetById(int id)
    {
        var resultado = await estudiantesService.Buscar(id);

        if (resultado.IsFailure)
        {
            return BadRequest(resultado.Error);
        }

        if (resultado.Value == null)
        {
            return NotFound();
        }

        return Ok(resultado.Value.Adapt<EstudianteDto>());
    }

    // POST: api/estudiantes
    [HttpPost]
    public async Task<ActionResult<EstudianteDto>> Post([FromBody] Estudiante estudiante)
    {
        var resultado = await estudiantesService.Guardar(estudiante);

        if (resultado.IsFailure)
        {
            return BadRequest(resultado.Error);
        }

        return Ok(estudiante.Adapt<EstudianteDto>());
    }

    // PUT: api/estudiantes/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Estudiante estudiante)
    {
        if (id != estudiante.EstudianteId)
        {
            return BadRequest();
        }

        var estudianteActual = await estudiantesService.Buscar(id);

        if (estudianteActual.IsFailure)
        {
            return BadRequest(estudianteActual.Error);
        }

        if (estudianteActual.Value == null)
        {
            return NotFound();
        }

        var resultado = await estudiantesService.Guardar(estudiante);

        if (resultado.IsFailure)
        {
            return BadRequest(resultado.Error);
        }

        return NoContent();
    }

    // DELETE: api/estudiantes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await estudiantesService.Buscar(id);

        if (estudiante.IsFailure)
        {
            return BadRequest(estudiante.Error);
        }

        if (estudiante.Value == null)
        {
            return NotFound();
        }

        var resultado = await estudiantesService.Eliminar(id);

        if (resultado.IsFailure)
        {
            return BadRequest(resultado.Error);
        }

        var eliminacion = (Aplicada1.Core.Result<bool>)resultado;
        if (!eliminacion.Value)
        {
            return Conflict("No se puede eliminar el estudiante porque tiene préstamos asociados.");
        }

        return NoContent();
    }
}
