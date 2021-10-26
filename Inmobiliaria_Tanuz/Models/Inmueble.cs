using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public enum estado
    {
        NoDisponible = 0,
        Disponible = 1,

    }

    public class Inmueble
       
    {
        [Key]
        [Display(Name = "Codigo")]
        public int Id { get; set; }
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey(nameof(PropietarioId))]
        public Propietario Duenio { get; set; }
        public string Direccion { get; set; }
        public string Uso { get; set; }
        public string Tipo { get; set; }
        [Display(Name= "Cantidad de ambientes")]
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        public int Estado { get; set; }

        public string estado => Estado == 1 ? "Disponible" : "No Disponible";

        public static IDictionary<int, string> ObtenerEstado()
        {
            SortedDictionary<int, string> estados = new SortedDictionary<int, string>();
            Type tipoEnumEstado = typeof(estado);
            foreach (var valor in Enum.GetValues(tipoEnumEstado))
            {
                estados.Add((int)valor, Enum.GetName(tipoEnumEstado, valor));
            }
            return estados;
        }
    }
}
