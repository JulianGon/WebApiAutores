using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion
        {
            get; set;
        }

        //public List<ComentarioDTO> Comentarios { get; set; }
    }
}
