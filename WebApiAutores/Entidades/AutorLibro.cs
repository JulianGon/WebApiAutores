namespace WebApiAutores.Entidades
{
    public class AutorLibro
    {
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set; }

        // Propiedades de Nav egacion
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }

    }
}
