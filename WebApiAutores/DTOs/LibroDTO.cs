using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entidades;

namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [Required]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion
        {
            get; set;
        }

        //public List<ComentarioDTO> Comentarios { get; set; }
    }
}
