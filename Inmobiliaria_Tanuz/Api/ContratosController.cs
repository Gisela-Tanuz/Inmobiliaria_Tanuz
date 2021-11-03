using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ContratosController : ControllerBase
    {
        private readonly DataContext context;

        public ContratosController(DataContext context)
        {
            this.context = context;
        }

        // GET api/<ContratosController>/5  obtener contratos vigentes
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                var list = await context.Contrato
                                .Include(x => x.Inquilino)
                                .Include(x => x.Inmueble)
                                .ThenInclude(x=> x.Duenio)
                                .Where(c => c.Inmueble.Duenio.Email == usuario && c.FechaFin> DateTime.Now).ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<ContratosController>/5    obtener contrato por id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var res =  context.Contrato
                    .Include(x => x.Inquilino)
                    .Include(x => x.Inmueble)
                    .ThenInclude(x => x.Duenio)
                    .Where(x => x.Inmueble.Duenio.Email == usuario)
                    .Single(x => x.IdContrato == id);
                  

                if (res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound();
                }
                
                }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
      

    }
}
