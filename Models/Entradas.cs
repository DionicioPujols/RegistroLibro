using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroLibro.Models;

public class Entradas
{
    [Key]
    public int EntradaId { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "El concepto es obligatorio")]
    public string? Concepto { get; set; }

    public int Total { get; set; }

    [InverseProperty("Entrada")]
    public virtual ICollection<EntradasDetalle> EntradasDetalle { get; set; } = new List<EntradasDetalle>();
}
