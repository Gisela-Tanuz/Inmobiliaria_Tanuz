using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InmueblesController : ControllerBase
    {
        private readonly DataContext context;

        public InmueblesController(DataContext context) {
            this.context = context;

        }
        //lista de inmuebles 
         [HttpGet("listaInm")]
         public async Task<IActionResult> Get()
         {
             try
             {
                 var usuarios = User.Identity.Name;
                 var list =  context.Inmueble.Include(x => x.Duenio).Where(x => x.Duenio.Email == usuarios);
                 return Ok(list);
             }
             catch (Exception ex)
             {
                 return BadRequest(ex);
             }
         }
        
        //inmuebles por id
        // GET api/<InmueblesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                
                var res = context.Inmueble.Include(x => x.Duenio)
                                          .Where(x => x.Duenio.Email == usuario)
                                          .Single(x => x.IdInmueble == id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //Actualiza disponibilidad
        // PUT api/<InmueblesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var inmueble = context.Inmueble.Include(x => x.Duenio)
                    .FirstOrDefault(x => x.IdInmueble == id && x.Duenio.Email == usuario);
                if (inmueble != null)
                {
                    inmueble.Estado = !inmueble.Estado;
                    context.Inmueble.Update(inmueble);
                    await context.SaveChangesAsync();
                    return Ok(inmueble);
                  
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
      
    }
}
