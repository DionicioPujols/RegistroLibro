using Microsoft.EntityFrameworkCore;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public class DevolucionesService(
    IDbContextFactory<Contexto> contextFactory)
    : Aplicada1.Core.IService<Devoluciones, int>
{

    public async Task<bool> Guardar(Devoluciones devoluciones)
    {
        if (!await Existe(devoluciones.DevolucionId))
        {
            return await Insertar(devoluciones);
        }
        else
        {
            return await Modificar(devoluciones);
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

    public async Task<Devoluciones?> Buscar(int devolucionId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Devoluciones
            .Include(d => d.Prestamo)
            .FirstOrDefaultAsync(d => d.DevolucionId == devolucionId);
    }

    public async Task<List<Devoluciones>> GetList(Expression<Func<Devoluciones, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Devoluciones
            .Include(d => d.Prestamo)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int devolucionId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var devolucion = await contexto.Devoluciones
            .FirstOrDefaultAsync(d => d.DevolucionId == devolucionId);

        if (devolucion == null) return false;

        contexto.Devoluciones.Remove(devolucion);
        return await contexto.SaveChangesAsync() > 0;
    }
}
