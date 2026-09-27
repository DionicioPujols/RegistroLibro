using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace RegistroLibro.Services
{
    public class PrestamosServices(IDbContextFactory<Contexto> dbContext) : Aplicada1.Core.IService<Prestamos, int>
    {
        public async Task<bool> Existe(int PrestamosId)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Prestamos.AllAsync(p => p.PrestamoId == PrestamosId);
        }

        public async Task<bool> Insertar(Prestamos prestamos)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            contexto.Prestamos.Add(prestamos);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> Modificar(Prestamos prestamos)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            contexto.Prestamos.Update(prestamos);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Prestamos?> Buscar(int id)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Prestamos
                .Include(p => p.Estudiante)
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.PrestamoId == id);
        }

        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            var prestamo = await contexto.Prestamos
                .FirstOrDefaultAsync(p => p.PrestamoId == id);

            if (prestamo == null) return false;

            contexto.Prestamos.Remove(prestamo);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<List<Prestamos>> GetList(Expression<Func<Prestamos, bool>> criterio)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Prestamos
                .Include(p => p.Estudiante)
                .Include(p => p.Libro)
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Guardar(Prestamos prestamos)
        {
            if(!await Existe(prestamos.PrestamoId))
            {
                return await Insertar(prestamos);
            }
            else
            {
                return await Modificar(prestamos);
            }
        }
    }
}
