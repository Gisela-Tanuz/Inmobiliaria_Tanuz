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
    public class PagosController : ControllerBase
    {
        private readonly DataContext context;

        public PagosController(DataContext context) {
            this.context = context;
        }

        // GET api/<PagosController>/5   lista de pagos por id
        [HttpGet("listaPagos/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var usuario = User.Identity.Name;
                var lista = await context.Pago.Include(x => x.contrato)
                                              .Where(x =>x.contrato.Inmueble.Duenio.Email== usuario && x.contrato.IdContrato == id)
                                              .ToListAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
