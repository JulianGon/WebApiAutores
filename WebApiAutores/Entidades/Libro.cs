namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor  Autor { get; set; } // Propiedad de Navegación -> Habilita cargar desde un libro los datos del autor 

    }
}
