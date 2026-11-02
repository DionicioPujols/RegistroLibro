using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.DATA.Context;
using RegistroLibro.DATA.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Service.Services;

public class PrestamosService(IDbContextFactory<Contexto> contextFactory)
    : Aplicada1.Core.IServiceResult<Prestamo, int>
{
    public async Task<Result> Guardar(Prestamo prestamo)
    {
        if (!await Existe(prestamo.PrestamoId))
        {
            var inserto = await Insertar(prestamo);
            return Result.Success(inserto);
        }

        var modifico = await Modificar(prestamo);
        return Result.Success(modifico);
    }

    public async Task<bool> Existe(int prestamoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Prestamos.AnyAsync(p => p.PrestamoId == prestamoId);
    }

    public async Task<bool> Insertar(Prestamo prestamo)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Prestamos.Add(prestamo);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Prestamo prestamo)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(prestamo);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Prestamo?>> Buscar(int prestamoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Prestamos
            .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);
    }

    public async Task<Result<List<Prestamo>>> GetList(Expression<Func<Prestamo, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Prestamos
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int prestamoId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var tieneDevoluciones = await contexto.Devoluciones
            .AnyAsync(d => d.PrestamoId == prestamoId);

        if (tieneDevoluciones)
        {
            return Result.Success(false);
        }

        var eliminado = await contexto.Prestamos
            .Where(p => p.PrestamoId == prestamoId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
