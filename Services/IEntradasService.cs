using Aplicada1.Core;
using RegistroLibro.Models;
using System.Linq.Expressions;

namespace RegistroLibro.Services;

public interface IEntradasService : IServiceResult<Entradas, int>
{
    Task<bool> Existe(int entradaId);
    Task<bool> Insertar(Entradas entrada);
    Task<bool> Modificar(Entradas entrada);
    Task<List<EntradasDetalle>> GetDetalles(int entradaId);
}
