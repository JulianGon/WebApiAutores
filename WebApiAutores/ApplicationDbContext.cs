using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Las opciones recibicdas en el constructor pueden recibir la cadena de conexión por ejemplo 
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Esto debe de estar aqui, sino EF Identity no funciona

            modelBuilder.Entity<AutorLibro>()
                .HasKey(al => new { al.AutorId, al.LibroId }); // Creamos la PK de AutorLibro con las PK de Autor y de Libro
        }
        public DbSet<Autor> Autores { get; set; } //Esto hace que se cree una tabla con los campos de la case. Se creará una tabla en SqlServer con las propiedades de la clase Autor 

        public DbSet<Libro> Libros { get; set; } // Hace que la tabla de libros pueda ser consultado por EF

        public DbSet<Comentario> Comentarios { get; set; } 

        public DbSet<AutorLibro> AutoresLibros { get; set;}
    }
}
