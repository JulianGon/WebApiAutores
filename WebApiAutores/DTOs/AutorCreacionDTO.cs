using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")] //Convierte la propiedad Nombre como requerido. En caso de que venga vacio se devolcerá un 400
        [StringLength(maximumLength: 120, ErrorMessage = "Maximo {1} caracteres para el campo {0}")]
        [PrimeraLetraMayus]
        public string Nombre { get; set; }
    }
}
