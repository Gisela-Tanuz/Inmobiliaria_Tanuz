﻿using Inmobiliaria_Tanuz.Models;
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
        // GET: api/<PropietariosController>
        /*[HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/
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
                var entidad = await context.Propietario.SingleOrDefaultAsync(x => x.Id == id);
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
        [HttpPost("login")]
        public async Task<IActionResult>Login([FromForm] Login login)
        {
            try 
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                var p = await context.Propietario.FirstOrDefaultAsync(x => x.Email == login.Email);
                if (p != null || p.Contraseña != hashed)
                {
                    return BadRequest("Usuario o Clave incorrecto");
                }
                else {
                    var Key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim> {
                       new Claim (ClaimTypes.Name, p.Email),
                       new Claim("FullName", p.Nombre + " "+p.Apellido),
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
                return BadRequest(ex);
            }
        }

        // PUT api/Controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
