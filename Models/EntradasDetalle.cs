using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroLibro.Models;

public class EntradasDetalle
{
    [Key]
    public int Id { get; set; }

    public int EntradaId { get; set; }

    [ForeignKey("EntradaId")]
    [InverseProperty("EntradasDetalle")]
    public virtual Entradas? Entrada { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un libro")]
    public int LibroId { get; set; }

    [ForeignKey("LibroId")]
    public virtual Libros? Libro { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }
}
