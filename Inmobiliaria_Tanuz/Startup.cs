using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Inmobiliaria_Tanuz
{
    public class Startup

    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>//el sitio web valida con cookie
                {
                    options.LoginPath = "/Usuarios/Login";
                    options.LogoutPath = "/Usuarios/Logout";
                    options.AccessDeniedPath = "/Home/Restringido";
                })
            /*services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //el sitio web valida con cookie
                {
                    options.LoginPath = "/Usuario/Login";  // redirige para el login
                    options.LogoutPath = "/Usuario/Logout"; // redirige para el logout
                    options.AccessDeniedPath = "/Home/Restringido"; // para accesos denegados
                })*/
               .AddJwtBearer(options =>
               //la api web valida con token
               {
                   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = configuration["TokenAuthentication:Issuer"],
                       ValidAudience = configuration["TokenAuthentication:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
                           configuration["TokenAuthentication:SecretKey"])),
                   };
               });
                   services.AddControllersWithViews();
                   services.AddControllers();
                   services.AddAuthorization(options =>
                   {
                       options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
                       options.AddPolicy("SuperAdministrador", policy => policy.RequireClaim(ClaimTypes.Role, "SuperAdministrador"));
                       options.AddPolicy("Administrador", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador"));
                       options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Empleado"));


                   });
                   services.AddMvc();
                   services.AddSignalR();//añade signalR
                                         //IUserIdProvider permite cambiar el ClaimType usado para obtener el UserIdentifier en Hub
                   //services.AddSingleton<IUserIdProvider, UsetIdProvider>();
                   /*
                   Transient objects are always different; a new instance is provided to every controller and every service.
                   Scoped objects are the same within a request, but different across different requests.
                   Singleton objects are the same for every object and every request.
                   */

                   services.AddTransient<IRepositorio<Propietario>, RepositorioPropietario>();
                   services.AddTransient<IRepositorio<Inquilino>, RepositorioInquilino>();
                   services.AddTransient<IRepositorio<Inmueble>, RepositorioInmueble>();
                   services.AddTransient<IRepositorio<Usuario>, RepositorioUsuario>();
                   services.AddTransient<IRepositorio<Contrato>, RepositorioContrato>();
                   services.AddTransient<IRepositorio<Pagos>, RepositorioPagos>();


            services.AddDbContext<DataContext>(
                  options => options.UseSqlServer(
                      Configuration.GetConnectionString("DefaultConnection")));
                     
               }
        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Todos estos métodos permiten manejar errores 404
            // En lugar de devolver el error, devuelve el código
            //app.UseStatusCodePages();
            // Hace un redirect cuando ocurren errores
            //app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
            // Hace una reejecución cuando ocurren errores
            //app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            // Habilitar CORS
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            // Uso de archivos estáticos (*.html, *.css, *.js, etc.)
            app.UseStaticFiles();
            app.UseRouting();
            // Permitir cookies
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
            });
            // Habilitar autenticación
            app.UseAuthentication();
            app.UseAuthorization();
            // App en ambiente de desarrollo?
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();//página amarilla de errores
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("login", "login/{**accion}", new { controller = "Usuario", action = "Login" });

                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
