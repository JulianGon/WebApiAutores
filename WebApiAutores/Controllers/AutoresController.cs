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

        [HttpGet] // Especifica la función que se ejecuta con la peticion GET 
        public async Task<ActionResult<List<Autor>>> Get() 
        {
            return await context.Autores.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor) 
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
