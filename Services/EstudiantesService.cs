using Microsoft.EntityFrameworkCore;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public class EstudiantesService(
    IDbContextFactory<Contexto> contextFactory
    ) : Aplicada1.Core.IService<Estudiantes, int>
{

    public async Task<bool> Guardar(Estudiantes estudiante)
    {
        if (!await Existe(estudiante.EstudiantesId))
        {
            return await Insetar(estudiante);
        }
        else
        {
            return await Modificar(estudiante);
        }
    }

    private async Task<bool> Existe(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
        .AnyAsync(e => e.EstudiantesId == estudianteId);
    }

    private async Task<bool> Insetar(Estudiantes estudiantes)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Estudiantes.Add(estudiantes);
        return await contexto
            .SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Estudiantes estudiantes)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(estudiantes);
        return await contexto
            .SaveChangesAsync() > 0;
    }

    public async Task<Estudiantes?> Buscar(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .FirstOrDefaultAsync(e => e.EstudiantesId == estudianteId);
    }

    public async Task<List<Estudiantes>> GetList(Expression<Func<Estudiantes, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Eliminar(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .Where(d => d.EstudiantesId == estudianteId)
            .ExecuteDeleteAsync() > 0;
    }
}
