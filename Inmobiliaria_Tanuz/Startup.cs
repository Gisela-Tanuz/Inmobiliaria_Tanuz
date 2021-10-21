using Inmobiliaria_Tanuz.Models;
using LinqToDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
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
                .AddCookie(options => //el sitio web valida con cookie
                {
                    options.LoginPath = "/Usuario/Login";  // redirige para el login
                    options.LogoutPath = "/Usuario/Logout"; // redirige para el logout
                    options.AccessDeniedPath = "/Home/Restringido"; // para accesos denegados
                })
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
                   // opción extra para usar el token el hub
                  /* options.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           // lee el token de la consulta
                           var accessToken = context.Request.Query["access_token"];
                           // la solicitud para el hub
                           var path = context.HttpContext.Request.Path;
                           if (!string.IsNullOrEmpty(accessToken) &&
                               path.StartsWithSegments("/chatsegurohub"))
                           {//reemplazar la url por la usada en la ruta ⬆
                               context.Token = accessToken;
                           }
                           return Task.CompletedTask;
                       }
                   };*/
               });

            //services.AddControllersWithViews();
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
           // services.AddSingleton<IUserIdProvider, UsetIdProvider>();
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
           

            services.AddDbContext<Models.DataContext>(options => options.UseSqlServer(configuration["ConnectionString:DefaultConnection"]));
        
    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.None, });
            //Habilita la autorizacion y autenticacion
            app.UseAuthentication();
            app.UseAuthorization();
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
