using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        
        [Required]
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }

        public DateTime? fechaPublicacion { get; set; }
        //Propiedad de navegacion, permite hacer JOIN de una manera distinta 
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; } // Relaciona el Libro con sus autores. FK de Libro y Autor
    }
}
