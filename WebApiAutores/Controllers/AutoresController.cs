using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController] // Permite validaciones automaticas respecto a los datos recibidos por el controlador
    [Route("api/autores")] // Especifica la ruta del controlador, 
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]    // Configurado en StartUp 
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController (ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        

        [HttpGet] // Especifica la función que se ejecuta con la peticion GET. Utilizando la ruta del controlador api/autores
        [AllowAnonymous] // Saca al Endpoint del Authorize
        // Al final de toda la autenticación,a la petición se le añade el siguiente Header:
        //      Authorization: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Imp1bGlhbkBob2xhbWFpbC5jb20iLCJsbyBxdWUgcXVpZXJhIjoib3RybyBWYWxvciIsImV4cCI6MTY1OTExNzU1OX0.bvZb7ugHkQqVdSmA67bE8ql9IVBuPDkPt6ad_VUumKY"
        public async Task<ActionResult<List<AutorDTO>>> Get() 
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);
        }

        
        [HttpGet("{id:int}",Name = "obtenerAutor")] // Devuelve un recurso en específico api/autores/1 -> Devuelve el autor específico .
        // Si no especificamos la restricción de :int (HttpGet("{id}") el error al enviar un string será un 400 en lugar del 404 programado en el EndPoint
        public async Task<ActionResult<AutorDTOConLibros>> Get (int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTOConLibros>(autor);
        }

        [HttpGet("{nombre}")] // No existe la restricción String
        public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();
            
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("model")] //api/autores/model?tematica=terror&valor=x
        public async Task<ActionResult<AutorDTO>> modelBinding([FromHeader] string nombre, [FromQuery] string tematica , [FromQuery] string valor)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDTO>(autor);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO) // El parametro de la funcion será el Request Body 
        {
            //Validaciones desde el controlador
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
            if (existeAutor)
            {
                return BadRequest($"Ya existe un autor con el nombre: {autorCreacionDTO.Nombre}");
            }
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync(); // Salva los cambios en la BBDD

            var autorDTO = mapper.Map<AutorDTO>(autor);
            // Con el nombre mapeado del GET, el recurso a devolver (Con un objeto anónimo) y la DTO del autor 
            return CreatedAtRoute("obtenerAutor", new {id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}")] //api/autores/algo {} -> parámetro de ruta 
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)    // la variable id coincide con el parametro de ruta en el Http
        {

            var existe = await context.Autores.AnyAsync(x => x.Equals(id)); // Busco el id en la BBDD mediante el objeto EF y métodos LinQ (DbSet)
            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor); // Este objeto de autor ya tiene todos los cambios a realizar en la BBDD 
            await context.SaveChangesAsync();
            return NoContent(); // 204 No Content
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
