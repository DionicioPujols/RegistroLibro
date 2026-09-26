using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroLibro.Models
{
    public class Prestamos
    {
        [Key]
        public int PrestamoId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Debe seleccionar un estudiante")]
        public int EstudiantesID { get; set; }

        public bool Devuelto { get; set; } = false;

        [Required(ErrorMessage = "Debe seleccionar un libro")]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        public virtual Libros? Libro { get; set; }

        [ForeignKey("EstudiantesID")]
        [InverseProperty("Prestamos")]
        public virtual Estudiantes Estudiante { get; set; } = null!;
    }
}
