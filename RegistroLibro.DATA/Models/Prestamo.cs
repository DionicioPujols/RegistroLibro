using System;
using System.Collections.Generic;

namespace RegistroLibro.DATA.Models;

public partial class Prestamo
{
    public int PrestamoId { get; set; }

    public DateTime Fecha { get; set; }

    public int EstudianteId { get; set; }

    public int LibroId { get; set; }

    public bool Devuelto { get; set; }

    public virtual ICollection<Devolucione> Devoluciones { get; set; } = new List<Devolucione>();

    public virtual Estudiante? Estudiante { get; set; }

    public virtual Libro? Libro { get; set; }
}
