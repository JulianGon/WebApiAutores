using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor 
    {
        public int Id { get; set; }  

        [Required(ErrorMessage ="El campo {0} es requerido")] //Convierte la propiedad Nombre como requerido. En caso de que venga vacio se devolcerá un 400
        [StringLength(maximumLength:120, ErrorMessage = "Maximo {1} caracteres para el campo {0}")]
        [PrimeraLetraMayus]
        public string Nombre { get; set; }
              

    }
}
