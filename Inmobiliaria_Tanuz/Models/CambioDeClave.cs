using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class CambioDeClave
    {
        [Required(ErrorMessage ="Campo obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string claveActual { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string claveNueva { get; set; }
        [Required (ErrorMessage = "Campo obligatorio") ]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar la nueva Contraseña")]
        [Compare ("claveNueva",ErrorMessage = "La contraseña nueva y su confirmación no coinciden ")]
        public string confirmarClave { get; set; }

    }
}
