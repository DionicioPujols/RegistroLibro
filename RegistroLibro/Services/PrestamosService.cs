using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public class PrestamosService(
    IDbContextFactory<Contexto> contextFactory)
    : Aplicada1.Core.IServiceResult<Prestamos, int>
{

    public async Task<Result> Guardar(Prestamos prestamos)
    {
        if (!await Existe(prestamos.PrestamoId))
        {
            var inserto = await Insertar(prestamos);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(prestamos);
            return Result.Success(modifico);
        }
    }

    public async Task<bool> Existe(int PrestamosId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Prestamos.AnyAsync(p => p.PrestamoId == PrestamosId);
    }

    public async Task<bool> Insertar(Prestamos prestamos)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Prestamos.Add(prestamos);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Prestamos prestamos)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(prestamos);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Prestamos?>> Buscar(int prestamoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Prestamos
            .Include(p => p.Estudiante)
            .Include(p => p.Libro)
            .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);
    }

    public async Task<Result<List<Prestamos>>> GetList(Expression<Func<Prestamos, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Prestamos
            .Include(p => p.Estudiante)
            .Include(p => p.Libro)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int prestamoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.Prestamos
            .Where(p => p.PrestamoId == prestamoId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }
}
