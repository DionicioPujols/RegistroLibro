namespace RegistroLibro.API.DTOs;

public class PrestamoDto
{
    public int PrestamoId { get; set; }
    public DateTime Fecha { get; set; }
    public int EstudianteId { get; set; }
    public int LibroId { get; set; }
    public bool Devuelto { get; set; }
}
