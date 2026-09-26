using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroLibro.Models
{
    public class PrestamosDetalle
    {
        [Key]
        public int DetalleId { get; set; }

        public int PrestamoId { get; set; }

        public int LibroId { get; set; }

        [ForeignKey("PrestamoId")]
        [InverseProperty("PrestamosDetalle")]
        public virtual Prestamos Prestamo { get; set; } = null!;

        [ForeignKey("LibroId")]
        public virtual Libros Libro { get; set; } = null!;
    }
}