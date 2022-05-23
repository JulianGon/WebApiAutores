namespace WebApiAutores.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public int LibroId { get; set; } // Con esto creamos la FK con EF 

        //Propiedad de navegacioon, permite hacer JOIN de una manera distinta. **Con el include
        public  Libro Libro { get; set; }

    }
}
