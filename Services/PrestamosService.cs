using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace RegistroLibro.Services
{
    public class PrestamosService(
        IDbContextFactory<Contexto> contextFactory)
        : Aplicada1.Core.IService<Prestamos, int>
    {
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
            contexto.Prestamos.Update(prestamos);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Prestamos?> Buscar(int prestamoId)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .Include(p => p.Estudiante)
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);
        }

        public async Task<bool> Eliminar(int prestamoId)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
            var prestamo = await contexto.Prestamos
                .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);

            if (prestamo == null) return false;

            contexto.Prestamos.Remove(prestamo);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<List<Prestamos>> GetList(Expression<Func<Prestamos, bool>> criterio)
        {
            await using var contexto = await contextFactory.CreateDbContextAsync();
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
