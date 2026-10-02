using Microsoft.EntityFrameworkCore;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services
{
    public class LibrosService(IDbContextFactory<Contexto> 
        contextFactory) : Aplicada1.Core.IService<Libros, int>
    {

        public async Task<bool> Guardar(Libros libro)
        {
            if (!await Existe(libro.LibroId))
            {
                return await Insertar(libro);
            }
            else
            {
                return await Modificar(libro);
            }
        }

        private async Task<bool> Existe(int libroId)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Libros
                .AnyAsync(l => l.LibroId == libroId);
        }

        private async Task<bool> Insertar(Libros libro)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Libros.Add(libro);
            return await contexto
                    .SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Libros libro)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            contexto.Update(libro);
            return await contexto
                .SaveChangesAsync() > 0;
        }

        public async Task<Libros?> Buscar(int librosId)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Libros
                .FirstOrDefaultAsync(l => l.LibroId == librosId);
        }

        public async Task<List<Libros>> GetList(Expression<Func<Libros, bool>> criterio)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Libros.
                Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Eliminar(int libroId)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Libros
                .Where(p => p.LibroId == libroId)
                .ExecuteDeleteAsync() > 0;
        }

    }
}
