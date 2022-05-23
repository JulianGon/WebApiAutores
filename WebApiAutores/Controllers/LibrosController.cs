﻿using AutoMapper;
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> Get(int id)
        {

            var libro = await context.Libros.Include(libroDb => libroDb.Comentarios).FirstOrDefaultAsync(x => x.Id.Equals(id));
            return mapper.Map<LibroDTO>(libro);
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

            if (libro.AutoresLibros != null)    // Se crea un orden aleatorio de los libros 
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }


            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();

        }

    }
}
