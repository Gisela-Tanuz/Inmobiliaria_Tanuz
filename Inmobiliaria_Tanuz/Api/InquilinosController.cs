using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext context;
        

        public InquilinosController(DataContext context){ 
            this.context = context;
            
        }
        // GET: api/<InquilinosController>  lista de inquilinos por propietario
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuarios = User.Identity.Name;
              
                var res = context.Contrato.Include(x => x.Inquilino)
                         .Include(x => x.Inmueble.Duenio)
                         .Where(x => x.Inmueble.Duenio.Email == usuarios)
                         .Select(x => x.Inquilino).Distinct()
                         .ToList();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<InquilinosController>/5  obtener inquilino por id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return NotFound();

            var res = context.Inquilino.FirstOrDefault(x => x.IdInquilino == id);

            if (res != null)
                return Ok(res);
            else
                return NotFound();
        }

    }
}
