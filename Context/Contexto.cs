using Microsoft.EntityFrameworkCore;
using RegistroLibro.Models;

namespace RegistroLibro.Context;

public class Contexto  : DbContext
{
    public Contexto(DbContextOptions<Contexto>options) : base(options) { } 
    public DbSet<Libros> Libros { get; set; }
    public DbSet<Estudiantes> Estudiantes { get; set; }
    public DbSet<Prestamos> Prestamos { get; set; }
    public DbSet<Devoluciones> Devoluciones { get; set; }
    public DbSet<Entradas> Entradas { get; set; }
    public DbSet<EntradasDetalle> EntradasDetalle { get; set; }
}
