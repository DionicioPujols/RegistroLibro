using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Security.Cryptography.Xml;

namespace RegistroLibro.Services;

public class EstudiantesService(
    IDbContextFactory<Contexto> contextFactory
    ) : Aplicada1.Core.IService<Estudiantes, int>
{

    private async Task<bool> Existe(int estudianteId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
        .AnyAsync(e => e.EstudiantesID == estudianteId);
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
        contexto.Estudiantes.Update(estudiantes);
        return await contexto
            .SaveChangesAsync() > 0;
    }

    public async Task<Estudiantes?> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .FirstOrDefaultAsync(e => e.EstudiantesID == id);
    }

    public async Task<bool> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .Where(d => d.EstudiantesID == id)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<List<Estudiantes>> GetList(Expression<Func<Estudiantes, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> Guardar(Estudiantes estudiantes)
    {
        if (!await Existe(estudiantes.EstudiantesID))
        {
            return await Insetar(estudiantes);
        }
        else
        {
            return await Modificar(estudiantes);
        }
    }
}

