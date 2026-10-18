using Aplicada1.Core;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public interface IEntradasService : IServiceResult<Entradas, int>
{
    Task<bool> Existe(int entradaId);
    Task<Result> Insertar(Entradas entrada);
    Task<Result> Modificar(Entradas entrada);
    Task<Result> Validar(Entradas entrada);
    Task<List<EntradasDetalle>> GetDetalles(int entradaId);
}
