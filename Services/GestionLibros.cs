using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace RegistroLibro.Services
{
    public class GestionLibros(IDbContextFactory<Contexto> dbContext) : Aplicada1.Core.IService<Libros, int>
    {
        private async Task<bool> Existe(int libroId)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Libros.AllAsync(l => l.LibroId == libroId);
        }

        private async Task<bool> Insertar(Libros libro)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            contexto.Libros.Add(libro);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Libros libro)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            contexto.Libros.Update(libro);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> Guardar(Libros libro)
        {
            if(!await Existe(libro.LibroId))
            {
                return await Insertar(libro);
            }
            else
            {
                return await Modificar(libro);
            }
        }

        public async Task<Libros?> Buscar(int id)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Libros.FirstOrDefaultAsync(l => l.LibroId == id);
        }

        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            var libro = await contexto.Libros.FirstOrDefaultAsync(l => l.LibroId == id);

            if(libro == null) return false;

            contexto.Libros.Remove(libro);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<List<Libros>> GetList(Expression<Func<Libros, bool>> criterio)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Libros.
                Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }
 
    }
}
