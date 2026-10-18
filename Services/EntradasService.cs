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
            return await Insertar(entrada);
        }
        else
        {
            return await Modificar(entrada);
        }
    }

    public async Task<bool> Existe(int entradaId)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Entradas.AnyAsync(e => e.EntradaId == entradaId);
    }

    public async Task<Result> Validar(Entradas entrada)
    {
        if (entrada == null)
            return Result.Failure(new Error("Entrada.Nula", "La entrada no puede ser nula."));

        if (string.IsNullOrWhiteSpace(entrada.Concepto))
            return Result.Failure(new Error("Entrada.ConceptoRequerido", "El concepto de la entrada es obligatorio."));

        if (entrada.Fecha == default)
            return Result.Failure(new Error("Entrada.FechaRequerida", "La fecha de la entrada es obligatoria."));

        if (entrada.EntradasDetalle == null || !entrada.EntradasDetalle.Any())
            return Result.Failure(new Error("Entrada.SinDetalles", "Debe agregar al menos un libro en el detalle de la entrada."));

        var librosDuplicados = entrada.EntradasDetalle
            .GroupBy(d => d.LibroId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (librosDuplicados.Any())
            return Result.Failure(new Error("Entrada.LibrosDuplicados", "No se permiten registros duplicados del mismo libro en el detalle de la entrada."));

        await using var contexto = await contextFactory.CreateDbContextAsync();

        foreach (var detalle in entrada.EntradasDetalle)
        {
            if (detalle.LibroId <= 0)
                return Result.Failure(new Error("Entrada.LibroNoSeleccionado", "Debe seleccionar un libro válido en el detalle."));

            if (detalle.Cantidad <= 0)
                return Result.Failure(new Error("Entrada.CantidadInvalida", "La cantidad de libros debe ser mayor que cero."));

            var existeLibro = await contexto.Libros.AnyAsync(l => l.LibroId == detalle.LibroId);
            if (!existeLibro)
                return Result.Failure(new Error("Entrada.LibroNoExiste", $"El libro seleccionado con ID {detalle.LibroId} no existe en el catálogo."));
        }

        return Result.Success();
    }

    public async Task<Result> Insertar(Entradas entrada)
    {
        var validacion = await Validar(entrada);
        if (validacion.IsFailure)
            return validacion;

        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        try
        {
            if (entrada.EntradaId > 0 && await contexto.Entradas.AnyAsync(e => e.EntradaId == entrada.EntradaId))
            {
                return Result.Failure(new Error("Entrada.IdExistente", $"Ya existe una entrada con el ID #{entrada.EntradaId}."));
            }

            foreach (var detalle in entrada.EntradasDetalle)
            {
                var libro = await contexto.Libros.FirstOrDefaultAsync(l => l.LibroId == detalle.LibroId);
                if (libro == null)
                {
                    await transaccion.RollbackAsync();
                    return Result.Failure(new Error("Entrada.LibroNoExiste", $"El libro con ID {detalle.LibroId} no existe."));
                }

                libro.CantidadDisponible += detalle.Cantidad;
                detalle.Libro = null;
            }

            entrada.Total = entrada.EntradasDetalle.Sum(d => d.Cantidad);

            contexto.Entradas.Add(entrada);
            var guardo = await contexto.SaveChangesAsync() > 0;

            if (!guardo)
            {
                await transaccion.RollbackAsync();
                return Result.Failure(new Error("Entrada.ErrorGuardar", "Ocurrió un error al intentar guardar la entrada."));
            }

            await transaccion.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaccion.RollbackAsync();
            return Result.Failure(new Error("Entrada.Excepcion", $"Error inesperado al guardar la entrada: {ex.Message}"));
        }
    }

    public async Task<Result> Modificar(Entradas entrada)
    {
        var validacion = await Validar(entrada);
        if (validacion.IsFailure)
            return validacion;

        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();
        try
        {
            var entradaExistente = await contexto.Entradas
                .Include(e => e.EntradasDetalle)
                .FirstOrDefaultAsync(e => e.EntradaId == entrada.EntradaId);

            if (entradaExistente == null)
                return Result.Failure(new Error("Entrada.NoEncontrada", $"No se encontró la entrada #{entrada.EntradaId} para modificar."));

            var librosInvolucradosIds = entradaExistente.EntradasDetalle.Select(d => d.LibroId)
                .Union(entrada.EntradasDetalle.Select(d => d.LibroId))
                .Distinct()
                .ToList();

            foreach (var libroId in librosInvolucradosIds)
            {
                var libro = await contexto.Libros.FirstOrDefaultAsync(l => l.LibroId == libroId);
                if (libro == null)
                {
                    await transaccion.RollbackAsync();
                    return Result.Failure(new Error("Entrada.LibroNoExiste", $"El libro con ID {libroId} no existe en el catálogo."));
                }

                int cantidadAnterior = entradaExistente.EntradasDetalle
                    .Where(d => d.LibroId == libroId)
                    .Sum(d => d.Cantidad);

                int cantidadNueva = entrada.EntradasDetalle
                    .Where(d => d.LibroId == libroId)
                    .Sum(d => d.Cantidad);

                int diferencia = cantidadNueva - cantidadAnterior;
                int nuevoStock = libro.CantidadDisponible + diferencia;

                if (nuevoStock < 0)
                {
                    await transaccion.RollbackAsync();
                    return Result.Failure(new Error("Entrada.StockNegativo",
                        $"No se puede modificar la entrada: la cantidad disponible del libro '{libro.Titulo}' quedaría en negativo ({nuevoStock}). Verifique los préstamos asociados."));
                }

                libro.CantidadDisponible = nuevoStock;
            }

            entradaExistente.Fecha = entrada.Fecha;
            entradaExistente.Concepto = entrada.Concepto;
            entradaExistente.Total = entrada.EntradasDetalle.Sum(d => d.Cantidad);

            contexto.EntradasDetalle.RemoveRange(entradaExistente.EntradasDetalle);

            foreach (var detalle in entrada.EntradasDetalle)
            {
                entradaExistente.EntradasDetalle.Add(new EntradasDetalle
                {
                    EntradaId = entradaExistente.EntradaId,
                    LibroId = detalle.LibroId,
                    Cantidad = detalle.Cantidad
                });
            }

            var guardo = await contexto.SaveChangesAsync() > 0;
            if (!guardo)
            {
                await transaccion.RollbackAsync();
                return Result.Failure(new Error("Entrada.ErrorModificar", "No se realizaron cambios en la entrada."));
            }

            await transaccion.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaccion.RollbackAsync();
            return Result.Failure(new Error("Entrada.Excepcion", $"Error inesperado al modificar la entrada: {ex.Message}"));
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
                return Result.Failure(new Error("Entrada.NoEncontrada", $"No se encontró la entrada #{entradaId} para eliminar."));

            foreach (var detalle in entrada.EntradasDetalle)
            {
                var libro = await contexto.Libros.FirstOrDefaultAsync(l => l.LibroId == detalle.LibroId);
                if (libro == null)
                {
                    await transaccion.RollbackAsync();
                    return Result.Failure(new Error("Entrada.LibroNoExiste", $"El libro con ID {detalle.LibroId} no existe en el catálogo."));
                }

                int nuevoStock = libro.CantidadDisponible - detalle.Cantidad;
                if (nuevoStock < 0)
                {
                    await transaccion.RollbackAsync();
                    return Result.Failure(new Error("Entrada.StockNegativo",
                        $"No se puede eliminar la entrada: la cantidad disponible del libro '{libro.Titulo}' quedaría en negativo ({nuevoStock}). Existen préstamos asociados."));
                }

                libro.CantidadDisponible = nuevoStock;
            }

            contexto.EntradasDetalle.RemoveRange(entrada.EntradasDetalle);
            contexto.Entradas.Remove(entrada);

            var eliminado = await contexto.SaveChangesAsync() > 0;
            if (!eliminado)
            {
                await transaccion.RollbackAsync();
                return Result.Failure(new Error("Entrada.ErrorEliminar", "No se pudo eliminar la entrada de la base de datos."));
            }

            await transaccion.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaccion.RollbackAsync();
            return Result.Failure(new Error("Entrada.Excepcion", $"Error inesperado al eliminar la entrada: {ex.Message}"));
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
}
