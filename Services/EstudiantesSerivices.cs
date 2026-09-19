using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Security.Cryptography.Xml;

namespace RegistroLibro.Services
{
    public class EstudiantesSerivices(IDbContextFactory<Contexto>
        dbContext) : Aplicada1.Core.IService<Estudiantes, int>
    {

        private async Task<bool> Existe(int estudianteId)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Estudiates
            .AllAsync(e => e.EstudiantesID == estudianteId);
        }

        private async Task<bool> Insetar(Estudiantes estudiates)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            contexto.Estudiates.Add(estudiates);
            return await contexto
                .SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Estudiantes estudiates)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            contexto.Estudiates.Update(estudiates);
            return await contexto
                .SaveChangesAsync() > 0;
        }

        public async Task<Estudiantes?> Buscar(int id) 
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Estudiates
                .FirstOrDefaultAsync(e => e.EstudiantesID == id);
        }

        public async Task<bool> Eliminar(int id)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Estudiates
                .Where(d => d.EstudiantesID == id)
                .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<Estudiantes>> GetList(Expression<Func<Estudiantes, bool>> criterio)
        {
            await using var contexto = await dbContext.CreateDbContextAsync();
            return await contexto.Estudiates
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Guardar(Estudiantes estudiates)
        {
            if(!await Existe(estudiates.EstudiantesID))
            {
                return await Insetar(estudiates);
            }
            else
            {
                return await Modificar(estudiates);
            }
        }
    }
}
