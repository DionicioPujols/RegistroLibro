using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.DATA.Context;
using RegistroLibro.DATA.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Service.Services;

public class EstudianteService(
    IDbContextFactory<Contexto> contextFactory
    ) : Aplicada1.Core.IServiceResult<Estudiante, int>
{
    public async Task<Result> Guardar(Estudiante estudiante)
    {
        if (!await Existe(estudiante.EstudianteId))
        {
            var inserto = await Insetar(estudiante);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(estudiante);
            return Result.Success(modifico);
        }
    }

    public async Task<bool> Existe(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .AnyAsync(e => e.EstudianteId == estudianteId);
    }

    public async Task<bool> ExisteNombre(string nombre)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .AnyAsync(e => e.Nombre == nombre);
    }

    public async Task<bool> Insetar(Estudiante estudiantes)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Estudiantes.Add(estudiantes);
        return await contexto
            .SaveChangesAsync() > 0;
    }

    public async Task<bool> Insertar(Estudiante estudiantes)
    {
        return await Insetar(estudiantes);
    }

    public async Task<bool> Modificar(Estudiante estudiantes)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(estudiantes);
        return await contexto
            .SaveChangesAsync() > 0;
    }

    public async Task<Result<Estudiante?>> Buscar(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .FirstOrDefaultAsync(e => e.EstudianteId == estudianteId);
    }

    public async Task<Result<List<Estudiante>>> GetList(Expression<Func<Estudiante, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        var eliminado = await contexto.Estudiantes
            .Where(d => d.EstudianteId == estudianteId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }
}
