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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            // Primer MiddelWare a ejecutar 

            app.Run(async context => // Run corta la ejecución del resto de Middelware 
            {
                await context.Response.WriteAsync("Estoy al inicio de la tubería ");
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
