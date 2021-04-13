using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Inquilino
    {
        [Display(Name = "Codigo")]
        public int IdInquilino { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string NombreGarante { get; set; }
        public string DireccionGarante { get; set; }
        public string TelGarante { get; set; }
        public string LugarDeTrabajo { get; set; }

    }
}
