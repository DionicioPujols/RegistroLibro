namespace RegistroLibro.API.DTOs;

public class EstudianteDto
{
    public int EstudianteId { get; set; }
    public string? Nombre { get; set; }
    public string? Direccion { get; set; }
    public string? Correo { get; set; }
    public DateTime? FechaNacimeinto { get; set; }
}
