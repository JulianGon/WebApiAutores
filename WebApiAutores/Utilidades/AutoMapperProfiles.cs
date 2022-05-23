using AutoMapper;
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
            CreateMap<LibroCreacionDTO, Libro>();
            CreateMap<Libro, LibroDTO>();
        }
    }
}
