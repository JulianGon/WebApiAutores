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

        //Propiedad de navegacioon, permite hacer JOIN de una manera distinta 
        public List<Comentario> Comentarios { get; set; }
    }
}
