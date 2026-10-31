using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public class DevolucionesService(
    IDbContextFactory<Contexto> contextFactory)
    : Aplicada1.Core.IServiceResult<Devoluciones, int>
{

    public async Task<Result> Guardar(Devoluciones devoluciones)
    {
        if (!await Existe(devoluciones.DevolucionId))
        {
            var inserto = await Insertar(devoluciones);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(devoluciones);
            return Result.Success(modifico);
        }
    }

    public async Task<bool> Existe(int devolucionId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Devoluciones.AnyAsync(d => d.DevolucionId == devolucionId);
    }

    public async Task<bool> Insertar(Devoluciones devoluciones)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Devoluciones.Add(devoluciones);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Devoluciones devoluciones)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(devoluciones);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Devoluciones?>> Buscar(int devolucionId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Devoluciones
            .Include(d => d.Prestamo)
            .ThenInclude(p => p!.Estudiante)
            .Include(d => d.Prestamo)
            .ThenInclude(p => p!.Libro)
            .FirstOrDefaultAsync(d => d.DevolucionId == devolucionId);
    }

    public async Task<Result<List<Devoluciones>>> GetList(Expression<Func<Devoluciones, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Devoluciones
            .Include(d => d.Prestamo)
            .ThenInclude(p => p!.Estudiante)
            .Include(d => d.Prestamo)
            .ThenInclude(p => p!.Libro)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int devolucionId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.Devoluciones
            .Where(d => d.DevolucionId == devolucionId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }
}
