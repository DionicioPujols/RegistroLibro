using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroLibro.Models;

public class Devoluciones
{
    [Key]
    public int DevolucionId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un préstamo")]
    public int PrestamoId { get; set; }

    [ForeignKey("PrestamoId")]
    public virtual Prestamos? Prestamo { get; set; }
}
