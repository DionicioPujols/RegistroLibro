using System;
using System.Collections.Generic;

namespace RegistroLibro.DATA.Models;

public partial class Libro
{
    public int LibroId { get; set; }

    public string Titulo { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public int AnoPublicacion { get; set; }

    public int CantidadPrestamos { get; set; }

    public int CantidadDisponible { get; set; }

    public virtual ICollection<EntradasDetalle> EntradasDetalles { get; set; } = new List<EntradasDetalle>();

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
