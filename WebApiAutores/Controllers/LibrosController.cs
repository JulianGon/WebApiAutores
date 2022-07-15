using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController :ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {

            var libro = await context.Libros
                .Include(libroDb => libroDb.AutoresLibros) // JOIN AutorLibro
                .ThenInclude(autorLibroDb => autorLibroDb.Autor) // JOIN Autor para traerme el nombre del autor (q lo muestro con AutoMapper)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList(); // Ordeno
            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await context.Autores
                .Where(autorDB => libroCreacionDTO.AutoresIds.Contains(autorDB.Id))
                .Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe alguno de los autores");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);  // En el mapeo de LibroCreacionDTO a Libro se ha especificado la transformación del List<int> de la DTO a List<AutorLibro> de la entidad Libro
            AsignarOrdenAutores(libro);    


            context.Add(libro);
            await context.SaveChangesAsync();
            var libroDTO = mapper.Map<LibroDTO>(libro);
            return CreatedAtRoute("obtenerLibro", new {id = libro.Id}, libroDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put (int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDB = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libroDB == null)
            {
                return NotFound();
            }
            // Devuelve:
            //     The mapped destination object, same instance as the destination object
            libroDB = mapper.Map(libroCreacionDTO, libroDB); // Con esto conseguimos modificar los autores del libro y los libros de los autores 
            AsignarOrdenAutores(libroDB);
            s
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrdenAutores(Libro libro)
        {
            
            if (libro.AutoresLibros != null)    // Se crea un orden aleatorio de los libros 
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

    }
}
