using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entidades;

namespace WebApiAutores.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250)]
        [Required]
        public string Titulo { get; set; }

        public List<int> AutoresIds { get; set; } // Añadimos a la DTO de libros un listado de Id de Autores 

        public DateTime FechaPublicacion { get; set; }

    }
}
