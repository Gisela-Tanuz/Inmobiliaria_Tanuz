using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Propietario
    {
        [Display(Name = "Codigo")]
        public int IdPropietario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        [Required, DataType(DataType.Password)]
        public string Contraseña { get; set; }

    }
}
