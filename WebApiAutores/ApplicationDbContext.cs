using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : DbContext
    {
        // Las opciones recibicdas en el constructor pueden recibir la cadena de conexión por ejemplo 
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Autor> Autores { get; set; } //Esto hace que se cree una tabla con los campos de la case. Se creará una tabla en SqlServer con las propiedades de la clase Autor 

        public DbSet<Libro> Libros { get; set; } // Hace que la tabla de libros pueda ser consultado por EF
    }
}
