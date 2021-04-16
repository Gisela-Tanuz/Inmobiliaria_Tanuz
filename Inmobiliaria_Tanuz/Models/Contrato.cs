using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Contrato
    {
        [Display(Name="Codigo")]
        public int IdContrato { get; set;}
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }
        [ForeignKey("IdInquilino")]
        public Inquilino Inquilino { get; set; }
        [Display(Name = "Dueño")]
        public int InmuebleId { get; set; }
        [ForeignKey("IdInmueble")]
        public Inmueble Duenio { get; set; }
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
        
    }
}
