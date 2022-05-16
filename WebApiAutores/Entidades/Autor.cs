using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor :IValidatableObject
    {
        public int Id { get; set; }  

        [Required(ErrorMessage ="El campo {0} es requerido")] //Convierte la propiedad Nombre como requerido. En caso de que venga vacio se devolcerá un 400
        [StringLength(maximumLength:120, ErrorMessage = "Maximo {1} caracteres para el campo {0}")]
        //[PrimeraLetraMayus] // Validaciones personalizada 
        public string Nombre { get; set; }
        
        [Range(18,120)] // Rango de valores 
        [NotMapped] // Especificamos a EF que no mapee esta propiedad a la BBDD 
        public int Edad { get; set; }

        [CreditCard] //Valida la numeración de la tarjeta 
        [NotMapped]
        public string TarjetaCredito { get; set; } 

        [Url] // Valida que sea una URL 
        [NotMapped]
        public string URL { get; set; }

        
        public List<Libro> Libros { get; set; }

        [NotMapped]
        public int Menor { get; set; }

        [NotMapped]
        public int Mayor { get; set; }

        // Para que las reglas del modelo se ejcuten, se debe de pasar como válido las validaciones por atributo 
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) //Validaciones por modelo
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    //vamos añadiendo las validaciones al IEnumerable
                    yield return new ValidationResult("La primera letra debe ser mayuscula", new string[] { nameof(Nombre)});
                }
            }
            if (Menor > Mayor)
            {
                yield return new ValidationResult("Es valor menos no puede ser mayor que el valor Mayor", new string[] { nameof(Menor) });
            }

        }
    }
}
