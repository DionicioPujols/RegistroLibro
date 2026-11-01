using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.DATA.Context;
using RegistroLibro.DATA.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Service.Services;

public class LibrosService(IDbContextFactory<Contexto> contextFactory)
    : Aplicada1.Core.IServiceResult<Libro, int>
{
    public async Task<Result> Guardar(Libro libro)
    {
        if (!await Existe(libro.LibroId))
        {
            var inserto = await Insertar(libro);
            return Result.Success(inserto);
        }

        var modifico = await Modificar(libro);
        return Result.Success(modifico);
    }

    public async Task<bool> Existe(int libroId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Libros.AnyAsync(l => l.LibroId == libroId);
    }

    public async Task<bool> Insertar(Libro libro)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Libros.Add(libro);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Libro libro)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        contexto.Update(libro);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Libro?>> Buscar(int libroId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Libros.FirstOrDefaultAsync(l => l.LibroId == libroId);
    }

    public async Task<Result<List<Libro>>> GetList(Expression<Func<Libro, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Libros
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int libroId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();

        var tienePrestamos = await contexto.Prestamos.AnyAsync(p => p.LibroId == libroId);
        var tieneEntradas = await contexto.EntradasDetalles.AnyAsync(e => e.LibroId == libroId);

        if (tienePrestamos || tieneEntradas)
        {
            return Result.Success(false);
        }

        var eliminado = await contexto.Libros
            .Where(l => l.LibroId == libroId)
            .ExecuteDeleteAsync() > 0;

        return Result.Success(eliminado);
    }
}
