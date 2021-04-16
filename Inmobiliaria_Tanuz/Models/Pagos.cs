using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class Pagos
    {
        public int IdPago { get; set; }
        public int ContratoId { get; set; }
        public int NroDePago { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
    }
}
