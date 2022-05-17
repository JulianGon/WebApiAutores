using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Servicios;

namespace WebApiAutores
{
    public class StartUp
    {
        public StartUp(IConfiguration configuration)
        {
            
            Configuration = configuration;

        }
        
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {


            services.AddControllers().AddJsonOptions (x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles    // Eliminamos el bucle de libro-Autor
            );

            //Configuración para el DbContext 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
                );

            // Tabién podemos instanciar la clase específica como servicio 
            //services.AddTransient<ServicioA>()
            
            // Aqui estoy especificandole a la aplicación que cuando una clase requiera IServicio se le pase ServicioA
            services.AddTransient<IServicio, ServicioA>();

            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartUp> servLogger) {
            // Primer MiddelWare a ejecutar 

            // Crear log de las respuestas HTTP 
            // siguiente permite continuar con la tubería 
            app.Use(async (context, siguiente) =>
            {
                using (var ms = new MemoryStream()) // Creo un Buffer para guardar el Stream de la respuesta 
                {
                    var cuerpoOriginalRespuesta = context.Response.Body;
                    context.Response.Body = ms; // Cambio el cuerpo original de la respuesta por el Buffer MemoryStream
                    await siguiente.Invoke(); // LLamo al siguiente Middleware 
                    // -> a partir de aquí se ejecuta cuando los middleware siguientes hayan devuelto su respuesta 

                    ms.Seek(0, SeekOrigin.Begin); // Colocamos el Stream al inicio 
                    string respuesta = new StreamReader(ms).ReadToEnd(); // Guardamos la respuesta en un string
                    ms.Seek(0, SeekOrigin.Begin); // Volvemos a colocar el Stream al inicio

                    await ms.CopyToAsync(cuerpoOriginalRespuesta); // Lo copiamos al cuerpo original 
                    context.Response.Body = cuerpoOriginalRespuesta; // Lo dejamos para que el cliente pueda leer el Stream 

                    servLogger.LogInformation(respuesta); // Con esta linea Logger podrá crear logs de todas las peticiones HTTP 

                }
            });
            // Hago una bifurcación de la tubería 
            app.Map("/ruta1", app =>
            {
                app.Run(async context => // Run corta la ejecución del resto de Middelware 
                {
                    await context.Response.WriteAsync("Estoy al inicio de la tubería ");
                });
            });


            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // Ultimo Middleware 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
