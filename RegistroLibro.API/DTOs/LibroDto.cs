namespace RegistroLibro.API.DTOs;

public class LibroDto
{
    public int LibroId { get; set; }
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public int AnoPublicacion { get; set; }
    public int CantidadPrestamos { get; set; }
    public int CantidadDisponible { get; set; }
}
