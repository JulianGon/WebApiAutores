﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middleware;

namespace WebApiAutores
{
    public class StartUp
    {
        public StartUp(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // Limpia el mapeo de Claims para que vengas los datos más limpios. del mapero automático 
            Configuration = configuration;

        }
        
        public IConfiguration Configuration { get; } // Por defecto

        public void ConfigureServices(IServiceCollection services) {


            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));    // Filtro de excepcion global 

            }).AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles    // Eliminamos el bucle de libro-Autor
            ).AddNewtonsoftJson(); 

            //Configuración para el DbContext 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
                );
            services.AddEndpointsApiExplorer();

            // Authorize JwtBearerDefaults es el AuthenticationSkin
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                // Configuración de JwtBearer
                .AddJwtBearer(opciones => opciones.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, // Tiempo de validez
                    ValidateIssuerSigningKey = true, // Validamos la firma del Token
                    IssuerSigningKey = new SymmetricSecurityKey(    // Metemos la firma del token
                        Encoding.UTF8.GetBytes(Configuration["llaveJWT"])),
                    ClockSkew = TimeSpan.Zero // Para no tener problemas de tiempo
                });

            services.AddSwaggerGen(c =>
            // Configuración de Swagger
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebApiAutores", Version = "v1" });
                // Configuración para el Token Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme //Microsoft.OpenApi.Models
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddAutoMapper(typeof(StartUp));

            // Arrancamos EF para el DbContext de usuarios y logueo 
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Si el usuario tiene un claim de "EsAdmin" entonces será admin 
            services.AddAuthorization(opciones =>
            {
                // se especifica el claim en el endpoint o en su controller en la etiqueta Authorize -> Policy = "EsAdmin"
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
                // Podemos añadir más como vendedores, comerciales...
                //opciones.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor")); // Este claim solo es para los vendedores

            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<StartUp> servLogger) {
            // Primer MiddelWare a ejecutar 

            // Crear log de las respuestas HTTP 
            // siguiente permite continuar con la tubería 
            app.UseLoguearRespuestaHTTP();

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization(); // Utilizo el middleware de autenticación antes de llegar a los controllers 

            // Ultimo Middleware 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
