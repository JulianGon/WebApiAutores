using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }  

        [Required(ErrorMessage ="El campo {0} es requerido")] //Convierte la propiedad Nombre como requerido. En caso de que venga vacio se devolcerá un 400
        [StringLength(maximumLength:5, ErrorMessage = "Maximo {1} caracteres para el campo {0}")]
        public string Nombre { get; set; }
        
        [Range(18,120)] // Rango de valores 
        [NotMapped] // Especificamos a EF que no mapee esta propiedad a la BBDD 
        public int Edad { get; set; }

        [CreditCard] //Valida la numeración de la tarjeta 
        [NotMapped]
        public string TarjetaCredito { get; set; } 

        [Url] // Valida que sea una URL 
        [NotMapped]
        public string URL { get; set; }

        
        public List<Libro> Libros { get; set; }
    }
}
