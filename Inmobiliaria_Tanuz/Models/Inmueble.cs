﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    /*public enum estado
    {
        NoDisponible = 0,
        Disponible = 1,

    }*/

    public class Inmueble
       
    {
        [Key]
        [Display(Name = "Codigo")]
        public int IdInmueble { get; set; }
        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }
        [ForeignKey(nameof(PropietarioId))]
        public Propietario Duenio { get; set; }
        public string Direccion { get; set; }
        public string Uso { get; set; }
        public string Tipo { get; set; }
        [Display(Name= "Ambientes")]
        public int Ambientes { get; set; }
        public decimal Precio { get; set; }
        [Display(Name ="Disponible")]
        public bool Estado { get; set; }
        public string Imagen { get; set; }
        [NotMapped]
        public IFormFile ImagenFile { get; set; }
       
    }
}
