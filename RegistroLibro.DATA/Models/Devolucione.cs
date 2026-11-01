using System;
using System.Collections.Generic;

namespace RegistroLibro.DATA.Models;

public partial class Devolucione
{
    public int DevolucionId { get; set; }

    public DateTime Fecha { get; set; }

    public int PrestamoId { get; set; }

    public virtual Prestamo Prestamo { get; set; } = null!;
}
