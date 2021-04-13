using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_Tanuz.Models
{
  
        public abstract class RepositorioBase
        {
            protected readonly IConfiguration configuration;
            protected readonly string connectionString;

            protected RepositorioBase(IConfiguration configuration)
            {
                this.configuration = configuration;
                connectionString = configuration["ConnectionStrings:DefaultConnection"];
            }
        }
    
}