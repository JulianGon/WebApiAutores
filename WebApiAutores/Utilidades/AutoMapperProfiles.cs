﻿using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>(); // Mapeamos las DTO con la clase de EF a mapear 
            CreateMap<Autor, AutorDTO>();   // El origen es el Autor de la BBDD y el destino es devolverle la información al cliente  
            CreateMap<LibroCreacionDTO, Libro>()
                // Especificamos el mapeo con la funcion MapAutoresLibros() para incluir el mapeo de la tabla AutorLibro (M:M) al crear un Libro Nuevo en su Controller
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDTO>()
                // Especificamos el mapeo para que traiga el nombre del Autor
                .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<ComentarioCreacionDTO,Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

        }

        // Mapeo el List<AutorDTO> q es campo de LibroDTO con los datos relacionados en AutorLibro para traerme sus nombres
        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO) 
        { 
            var resultado = new List<AutorDTO>();   
            if (libro.AutoresLibros == null) { return resultado; }
            foreach ( var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO() {
                    id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }
            return resultado;
        }

        // Transforma el List<int> de la DTO de LibroCreacionDTO a el List<AutorLibros> de la entidad Libro
        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (libroCreacionDTO.AutoresIds == null) { return resultado; } // No se han especificado autores del nuevo libro 
            foreach (var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId }); // Como estoy creando un Libro no mapeo el LibroId
            }
            return resultado;
        }
    }
}
