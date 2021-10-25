using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz
{
    public class Program
    {
	/*	public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}*/
		public static void Main(string[] args)
		{

			CreateWebHostBuilder(args).Build().Run();
			
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args)
		{
			var host = WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(logging =>
				{
					logging.ClearProviders();//limpia los proveedores x defecto de log (consola+depuración)
					logging.AddConsole();//agrega log de consola
			       // logging.AddConfiguration(new LoggerConfiguration().WriteTo.File("serilog.txt").CreateLogger())
				})
				.UseStartup<Startup>();
			return host;
		}

		public static IWebHostBuilder CreateKestrel(string[] args)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.Build();
			var host = new WebHostBuilder()
				.UseConfiguration(config)
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseUrls("http://localhost:5000", "https://localhost:5001")//permite escuchar SOLO peticiones locales
				.UseUrls("http://*:5000", "https://*:5001")//permite escuchar peticiones locales y remotas
				.UseIISIntegration()
				.UseStartup<Startup>();
			return host;
		}
	}
}

