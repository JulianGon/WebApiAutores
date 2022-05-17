namespace WebApiAutores.Middleware
{
    // Los metodos de extensiones son estaticos 
    public static class WebApiAutoresMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP ( this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>(); 
        }
    }
    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente,
            ILogger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        // REGLA para utilizar esta clase como Middleware. Debe de tener el metodo publico Invoke o InvokeAsync.
        // que debe devolver una tarea y recoger como primer parametro un HttpContext

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream()) // Creo un Buffer para guardar el Stream de la respuesta 
            {
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = ms; // Cambio el cuerpo original de la respuesta por el Buffer MemoryStream
                await siguiente(context); // LLamo al siguiente Middleware 

                ms.Seek(0, SeekOrigin.Begin); // Colocamos el Stream al inicio 
                string respuesta = new StreamReader(ms).ReadToEnd(); // Guardamos la respuesta en un string
                ms.Seek(0, SeekOrigin.Begin); // Volvemos a colocar el Stream al inicio

                await ms.CopyToAsync(cuerpoOriginalRespuesta); // Lo copiamos al cuerpo original 
                context.Response.Body = cuerpoOriginalRespuesta; // Lo dejamos para que el cliente pueda leer el Stream 

                logger.LogInformation(respuesta); // Con esta linea Logger podrá crear logs de todas las peticiones HTTP 

            }
        }
    }
}
