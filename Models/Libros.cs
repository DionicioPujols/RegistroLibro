using System.ComponentModel.DataAnnotations;
namespace RegistroLibro.Models;

public class Libros
{
    [Key]
    public int LibroId { get; set; }

    [Required(ErrorMessage = "Necesitas agregar un Titulo Obligatorio")]
    public String? Titulo { get; set; }
    [Required(ErrorMessage = "Colocar Nombre del Autor")]
    public String? Autor { get; set; }
    [Required(ErrorMessage = "Debes colocar el año de publicacion")]
    public int anoPublicacion { get; set; }

}
