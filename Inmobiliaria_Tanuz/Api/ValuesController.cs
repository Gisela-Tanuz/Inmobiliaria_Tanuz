using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria_Tanuz.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext context;

        public ValuesController(DataContext dataContext) {
            this.context = dataContext;
        }

        //GET:api/<Valuescontroller>
        [HttpGet]
        public async Task<IActionResult> Get() {
            try
            {
                return Ok(new
                {

                    Mensaje = "Exito",
                    Error = 0,
                    Resultado = new
                    {
                        Clave = "key",
                        Valor = "value"

                    },
                });
            }
            catch (Exception ex) {

                return BadRequest(ex);
            };
        }
        //GET: api/<ValuesController>/5

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(context.Propietario.Find(id));
        }

        //GET: api/<ValuesController>
        [HttpGet("usuario/{id=0}")]
        public IActionResult GetUser(int id) 
        {
            return Ok(context.Usuario.ToList());
        }
        //GET: api/<ValuesController>
        [HttpGet("email/{id=0}")]
        public IActionResult Emails(int id) 
        {
            if (id > 0)

                return Ok(context.Propietario.Where(x => x.IdPropietario == id).Select(x => x.Email).Single());
            else
                return Ok(context.Propietario.Select(x => x.Email).ToList());
        }

        //GET: api/<ValuesController>
        [HttpGet("anonimo/{id}")]
        public IActionResult GetAnonimo(int id) 
        {
            return id > 0 ? 
                Ok(context.Propietario.Where(x => x.IdPropietario == id).Select(x => new { Id = x.IdPropietario, x.Email }).Single()) :
                Ok(context.Propietario.Select(x => new { Id = x.IdPropietario, x.Email }).ToList());
        }


        
        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromForm] string value, IFormFile file)
        {
          
        }


        // POST api/<ValuesController>/usuario/5
        [HttpPost("usuario/{id}")]
        public Usuario Post([FromForm] Usuario usuario, int id)
        {
            usuario.Id = id;
            return usuario;
        }

        // POST api/<ValuesController>/usuario/5
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromForm] Login login)
        {
            return Ok(login);
        }

        //PUT api/<ValuesControler>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) 
        { 
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
