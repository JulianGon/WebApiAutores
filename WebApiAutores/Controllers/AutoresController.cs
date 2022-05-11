using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController] // Permite validaciones automaticas respecto a los datos recibidos por el controlador
    [Route("api/autores")] // Especifica la ruta del controlador, 
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController (ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet] // Especifica la función que se ejecuta con la peticion GET. Utilizando la ruta del controlador api/autores
        [HttpGet("listado")] // Especifico la ruta del recurso -> api/autores/listado
        [HttpGet("/listado")] // Especifico la ruta saltándome la ruta del controller.->  /listado
        public async Task<ActionResult<List<Autor>>> Get() 
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("primero")]    // Especifica otra URL para el mismo metodo HTTP -> api/autores/primero
        public async Task<ActionResult<Autor>> PrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("{id:int}")] // Devuelve un recurso en específico api/autores/1 -> Devuelve el autor específico .
        // Si no especificamos la restricción de :int (HttpGet("{id}") el error al enviar un string será un 400 en lugar del 404 programado en el EndPoint
        public async Task<ActionResult<Autor>> Get (int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpGet("{nombre}")] // No existe la restricción String
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpGet("model")] //api/autores/model?tematica=terror&valor=x
        public async Task<ActionResult<Autor>> modelBinding([FromHeader] string nombre, [FromQuery] string tematica , [FromQuery] string valor)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        //[HttpGet("{id:int}/{param2}")] //Varios parametros de ruta
        //[HttpGet("{id:int}/{param2?}")] //param2 será opcional, sino se especifica sera null
        //[HttpGet("{id:int}/{param2=persona}")] //param2 cogerá el valor por defecto

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor) // El parametro de la funcion será el Request Body 
        {
            context.Add(autor);
            await context.SaveChangesAsync(); // Salva los cambios en la BBDD
            return Ok();
        }

        [HttpPut("{id:int}")] //api/autores/algo {} -> parámetro de ruta 
        public async Task<ActionResult> Put(Autor autor, int id)    // la variable id coincide con el parametro de ruta en el Http
        {
            
            if (autor.Id != id) // El Id del autor del Request body no coincide con el id que se envía por la URL 
            {
                return BadRequest("El id no coincide"); //Error 400
            }

            var existe = await context.Autores.AnyAsync(x => x.Equals(id)); // Busco el id en la BBDD mediante el objeto EF y métodos LinQ (DbSet)
            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor); // Este objeto de autor ya tiene todos los cambios a realizar en la BBDD 
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete (int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound();
            }

            context.Remove (new Autor { Id = id }); //Instancio un autor para pasarle al Remove
            await context.SaveChangesAsync();
            return Ok();

        }

    }
}
