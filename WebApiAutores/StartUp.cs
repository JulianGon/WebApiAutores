using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middleware;
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


            services.AddControllers(opciones =>{ 
                opciones.Filters.Add(typeof(FiltroDeExcepcion));    // Filtro de excepcion global 
            
            }).AddJsonOptions (x =>
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

            services.AddTransient<MiFiltroDeAccion>();

            services.AddEndpointsApiExplorer();

            services.AddResponseCaching();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartUp> servLogger) {
            // Primer MiddelWare a ejecutar 

            // Crear log de las respuestas HTTP 
            // siguiente permite continuar con la tubería 
            app.UseLoguearRespuestaHTTP();


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

            app.UseResponseCaching();

            app.UseAuthorization(); // Utilizo el middleware de autenticación antes de llegar a los controllers 

            // Ultimo Middleware 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
