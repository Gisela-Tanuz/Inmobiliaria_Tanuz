using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Inmueble
    {
        [Display(Name = "Codigo")]
        public int IdInmueble { get; set; }
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey("IdPropietario")]
        public Propietario Duenio { get; set; }
        public string Direccion { get; set; }
        public string Uso { get; set; }
        public string Tipo { get; set; }
        [Display(Name= "Cantidad de ambientes")]
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
    }
}
