using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using RegistroLibro.Context;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public class EntradasService(
    IDbContextFactory<Contexto> contextFactory) : IEntradasService
{
    public async Task<Result> Guardar(Entradas entrada)
    {
        if (!await Existe(entrada.EntradaId))
        {
            var inserto = await Insertar(entrada);
            return Result.Success(inserto);
        }
        else
        {
            var modifico = await Modificar(entrada);
            return Result.Success(modifico);
        }
    }

    public async Task<bool> Existe(int entradaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Entradas.AnyAsync(e => e.EntradaId == entradaId);
    }

    public async Task<bool> Insertar(Entradas entrada)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        try
        {
            if (!await ValidarLibros(contexto, entrada))
                return false;

            // Evitar que EF intente reinsertar instancias de libros si vienen asociadas
            foreach (var detalle in entrada.EntradasDetalle)
            {
                detalle.Libro = null;
            }

            entrada.Total = entrada.EntradasDetalle.Sum(d => d.Cantidad);

            contexto.Entradas.Add(entrada);
            var guardo = await contexto.SaveChangesAsync() > 0;

            await transaccion.CommitAsync();
            return guardo;
        }
        catch
        {
            await transaccion.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> Modificar(Entradas entrada)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        try
        {
            var entradaExistente = await contexto.Entradas
                .Include(e => e.EntradasDetalle)
                .FirstOrDefaultAsync(e => e.EntradaId == entrada.EntradaId);

            if (entradaExistente == null)
                return false;

            if (!await ValidarLibros(contexto, entrada))
                return false;

            // Actualizar campos de la cabecera
            entradaExistente.Fecha = entrada.Fecha;
            entradaExistente.Concepto = entrada.Concepto;
            entradaExistente.Total = entrada.EntradasDetalle.Sum(d => d.Cantidad);

            // 1. Eliminar detalles que ya no existen en la entrada entrante
            var idsDetallesActuales = entrada.EntradasDetalle
                .Where(d => d.Id != 0)
                .Select(d => d.Id)
                .ToList();

            var detallesAEliminar = entradaExistente.EntradasDetalle
                .Where(d => !idsDetallesActuales.Contains(d.Id))
                .ToList();

            contexto.EntradasDetalle.RemoveRange(detallesAEliminar);

            // 2. Modificar existentes y agregar nuevos
            foreach (var detalle in entrada.EntradasDetalle)
            {
                if (detalle.Id == 0)
                {
                    entradaExistente.EntradasDetalle.Add(new EntradasDetalle
                    {
                        EntradaId = entrada.EntradaId,
                        LibroId = detalle.LibroId,
                        Cantidad = detalle.Cantidad
                    });
                }
                else
                {
                    var detalleExistente = entradaExistente.EntradasDetalle
                        .FirstOrDefault(d => d.Id == detalle.Id);

                    if (detalleExistente != null)
                    {
                        detalleExistente.LibroId = detalle.LibroId;
                        detalleExistente.Cantidad = detalle.Cantidad;
                    }
                }
            }

            var guardo = await contexto.SaveChangesAsync() > 0;
            await transaccion.CommitAsync();
            return guardo;
        }
        catch
        {
            await transaccion.RollbackAsync();
            return false;
        }
    }

    public async Task<Result<Entradas?>> Buscar(int entradaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Entradas
            .Include(e => e.EntradasDetalle)
            .ThenInclude(d => d.Libro)
            .FirstOrDefaultAsync(e => e.EntradaId == entradaId);
    }

    public async Task<Result<List<Entradas>>> GetList(Expression<Func<Entradas, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Entradas
            .Include(e => e.EntradasDetalle)
            .ThenInclude(d => d.Libro)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int entradaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        try
        {
            var entrada = await contexto.Entradas
                .Include(e => e.EntradasDetalle)
                .FirstOrDefaultAsync(e => e.EntradaId == entradaId);

            if (entrada == null)
                return Result.Success(false);

            contexto.EntradasDetalle.RemoveRange(entrada.EntradasDetalle);
            contexto.Entradas.Remove(entrada);

            var eliminado = await contexto.SaveChangesAsync() > 0;
            await transaccion.CommitAsync();
            return Result.Success(eliminado);
        }
        catch
        {
            await transaccion.RollbackAsync();
            return Result.Success(false);
        }
    }

    public async Task<List<EntradasDetalle>> GetDetalles(int entradaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.EntradasDetalle
            .Include(d => d.Libro)
            .Where(d => d.EntradaId == entradaId)
            .AsNoTracking()
            .ToListAsync();
    }

    private async Task<bool> ValidarLibros(Contexto contexto, Entradas entrada)
    {
        if (entrada.EntradasDetalle == null || !entrada.EntradasDetalle.Any())
            return false;

        foreach (var detalle in entrada.EntradasDetalle)
        {
            if (detalle.LibroId <= 0 || detalle.Cantidad <= 0)
                return false;

            var existeLibro = await contexto.Libros.AnyAsync(l => l.LibroId == detalle.LibroId);
            if (!existeLibro)
                return false;
        }

        return true;
    }
}
