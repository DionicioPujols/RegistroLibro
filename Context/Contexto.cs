using Microsoft.EntityFrameworkCore;
using RegistroLibro.Models;

namespace RegistroLibro.Context
{
    public class Contexto  : DbContext
    {
        public Contexto(DbContextOptions<Contexto>options) : base(options) { } 
        public DbSet<Libros> Libros { get; set; }
        public DbSet<Estudiantes> Estudiates { get; set; }

        public DbSet<Prestamos> Prestamos { get; set; }
        public DbSet<PrestamosDetalle> PrestamosDetalle { get; set; }
    }
}
