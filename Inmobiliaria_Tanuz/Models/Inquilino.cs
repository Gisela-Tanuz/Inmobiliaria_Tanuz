using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Codigo")]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        [Display(Name = "Nombre del Garante")]
        public string NombreGarante { get; set; }
        [Display(Name = "Direccion del Garante")]
        public string DireccionGarante { get; set; }
        [Display(Name = "Telefono del Garante")]
        public string TelGarante { get; set; }
        [Display(Name = "Lugar de trabajo")]
        public string LugarDeTrabajo { get; set; }

    }
}
