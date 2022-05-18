using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }

        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250)]
        public string Titulo { get; set; }
    }
}
