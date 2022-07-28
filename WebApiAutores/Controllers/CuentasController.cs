using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentasController(UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager) 
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }


        [HttpPost("registrar")] //api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsiarioDTO credencialesUsiario)
        {
            var usuario = new IdentityUser {
                UserName = credencialesUsiario.Email,
                Email = credencialesUsiario.Email
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsiario.Password);
            if (resultado.Succeeded)
            {
                //************ -->  JSON WEB TOKEN  RFC 7519.
                //En la práctica, se trata de una cadena de texto que tiene tres partes codificadas en Base64, cada una de ellas separadas por un punto
                //Podemos ver el contenido del token sin necesidad de saber la clave con la cual se ha generado,
                //aunque no podremos validarlo sin la misma.
                //
                //Header: encabezado dónde se indica, al menos, el algoritmo y el tipo de token, algoritmo HS256 y un token JWT.
                //Payload: donde aparecen los datos de usuario y privilegios, así como toda la información que queramos añadir,
                //todos los datos que creamos convenientes.
                //Signature: una firma que nos permite verificar si el token es válido, 
                return ConstruirToken(credencialesUsiario);
            }
            else{
                return BadRequest(resultado.Errors);
            }

        }


        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsiarioDTO credencialesUsiarioDTO) 
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsiarioDTO.Email,
                credencialesUsiarioDTO.Password,
                isPersistent: false, // Cookie de autenticación, OFF
                lockoutOnFailure: false // Bloquea a los usuarios cuyos login sean incorrectos, lo dejo Off
                );
            if (resultado.Succeeded)
            {
                return ConstruirToken(credencialesUsiarioDTO);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        
        }
         
        /// <summary>
        /// Genera el token que se le devolverá al usuario para las siguientes peticiones. Se generan los claims y el resto de información necesaria para 
        /// 
        /// </summary>
        /// <param name="credencialesUsiarioDTO"></param>
        /// <returns></returns>
        private RespuestaAutenticacionDTO ConstruirToken(CredencialesUsiarioDTO credencialesUsiarioDTO)
        {
            // Un claim es informacion del usuario emitida por una fuente confiable
            var claims = new List<Claim>()
            {
                // La gracia del claim es QUE SIEMPRE ES KEY/VALUE
                // Los claim NO SON SECRETOS, LA INFO LA DEBE DE VER EL USUARIO
                new Claim("email", credencialesUsiarioDTO.Email),
                new Claim("lo que quiera","otro Valor")
            };

            // Genero la llave con el string que hay en AppSettings. 
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJWT"]));

            // Algoritmo para generar las credenciales 
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddDays(1);

            // De aqui por el momento solo nos interesa los claims, las creds y la expiracion
            var jwt = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),    // Devuelve el string que representa al TOKEN
                Expiracion = expiracion
            };
        }
    }
}
