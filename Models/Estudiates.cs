using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace RegistroLibro.Models
{
    public class Estudiates
    {
        [Key]
        public int EstudiantesID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string? nombre { get; set; }
        [Required(ErrorMessage = "La direccion es obligatoria.")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage = "El Email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe contener un correo valido")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimeinto es obligatorio")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimeinto { get; set; } = DateTime.Today;

    }
}
