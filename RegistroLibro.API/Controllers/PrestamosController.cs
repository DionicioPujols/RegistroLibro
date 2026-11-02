using Mapster;
using Microsoft.AspNetCore.Mvc;
using RegistroLibro.API.DTOs;
using RegistroLibro.DATA.Models;
using RegistroLibro.Service.Services;

namespace RegistroLibro.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrestamosController(PrestamosService prestamosService) : ControllerBase
{
    // GET: api/prestamos
    [HttpGet]
    public async Task<ActionResult<List<PrestamoDto>>> Get()
    {
        var result = await prestamosService.GetList(p => p.PrestamoId > 0);
        return Ok(result.Value.Adapt<List<PrestamoDto>>());
    }

    // GET: api/prestamos/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrestamoDto>> GetById(int id)
    {
        var result = await prestamosService.Buscar(id);

        if (result.Value == null)
        {
            return NotFound();
        }

        return Ok(result.Value.Adapt<PrestamoDto>());
    }

    // POST: api/prestamos
    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> Post([FromBody] Prestamo prestamo)
    {
        await prestamosService.Guardar(prestamo);
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

        var prestamoActual = await prestamosService.Buscar(id);
        if (prestamoActual.Value == null)
        {
            return NotFound();
        }

        await prestamosService.Guardar(prestamo);
        return NoContent();
    }

    // DELETE: api/prestamos/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var prestamo = await prestamosService.Buscar(id);
        if (prestamo.Value == null)
        {
            return NotFound();
        }

        var resultado = await prestamosService.Eliminar(id);
        var eliminacion = (Aplicada1.Core.Result<bool>)resultado;
        if (!eliminacion.Value)
        {
            return Conflict("No se puede eliminar el préstamo porque tiene devoluciones asociadas.");
        }

        return NoContent();
    }
}
