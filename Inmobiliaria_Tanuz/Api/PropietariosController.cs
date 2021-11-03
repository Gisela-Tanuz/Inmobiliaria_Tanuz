using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria_Tanuz.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietariosController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration config;

        public PropietariosController(DataContext context, IConfiguration config)
        {
            this.context = context;
            this.config = config;
        }
     
        //GET: api/<Controller>  
        [HttpGet]
        public async Task<ActionResult<Propietario>> Get() {
            try
            {
                var usuario = User.Identity.Name;
                return await context.Propietario.SingleOrDefaultAsync(x => x.Email == usuario);
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }

        // GET api/<Controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var entidad = await context.Propietario.SingleOrDefaultAsync(x => x.IdPropietario == id);
                return entidad != null ? Ok(entidad) : NotFound();
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
        //GET api/<Controller>/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await context.Propietario.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        // POST api/<Controller>/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] Login login)
        {
            //   Propietario p = null;
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                         password: login.Clave,
                         salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                         prf: KeyDerivationPrf.HMACSHA1,
                         iterationCount: 1000,
                         numBytesRequested: 256 / 8));
                var p = await context.Propietario.FirstOrDefaultAsync(x => x.Email == login.Email);

                if (p == null || p.Contraseña != hashed)
                {

                    return BadRequest("Email o clave incorrecta"); ;

                }
                else
                {
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                     {
                         new Claim(ClaimTypes.Name, p.Email),
                         new Claim("FullName", p.Nombre + " " + p.Apellido),
                         new Claim(ClaimTypes.Role, "Propietario"),
                     };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());

            }

        }
        // POST api/<PropietariosController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Propietario propietarios)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await context.Propietario.AddAsync(propietarios);
                    context.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = propietarios.IdPropietario }, propietarios);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/Controller>/5  Actualizar Perfil
        [HttpPut()]
        public async Task<IActionResult> Put ([FromBody] Propietario propietario)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    context.Propietario.Update(propietario);
                   
                    await context.SaveChangesAsync();
                    return Ok(propietario);
                }
                    return BadRequest();
                
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }


        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
