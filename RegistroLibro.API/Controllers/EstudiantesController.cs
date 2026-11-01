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
        var result = await estudiantesService.GetList(d => d.EstudianteId > 0);
        return Ok(result.Value.Adapt<List<EstudianteDto>>());
    }

    // GET: api/estudiantes/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EstudianteDto>> GetById(int id)
    {
        var result = await estudiantesService.Buscar(id);

        if (result.Value == null)
        {
            return NotFound();
        }

        return Ok(result.Value.Adapt<EstudianteDto>());
    }

    // POST: api/estudiantes
    [HttpPost]
    public async Task<ActionResult<EstudianteDto>> Post([FromBody] Estudiante estudiante)
    {
        await estudiantesService.Guardar(estudiante);
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
        if (estudianteActual.Value == null)
        {
            return NotFound();
        }

        await estudiantesService.Guardar(estudiante);
        return NoContent();
    }

    // DELETE: api/estudiantes/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var estudiante = await estudiantesService.Buscar(id);
        if (estudiante.Value == null)
        {
            return NotFound();
        }

        var resultado = await estudiantesService.Eliminar(id);
        var eliminacion = (Aplicada1.Core.Result<bool>)resultado;
        if (!eliminacion.Value)
        {
            return Conflict("No se puede eliminar el estudiante porque tiene préstamos asociados.");
        }

        return NoContent();
    }
}
