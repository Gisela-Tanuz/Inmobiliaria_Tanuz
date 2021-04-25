using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Pagos
    {
        [Display(Name = "Codigo")]
        public int IdPago { get; set; }
        [Display(Name= "Contrato ")]
        public int ContratoId { get; set; }
        [ForeignKey(nameof(ContratoId))]
        public Contrato contrato { get; set; }
        [Display(Name ="Nro de pago")]
        public int NroDePago { get; set; }
        [Display(Name = "Fecha de pago")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
    }
}
