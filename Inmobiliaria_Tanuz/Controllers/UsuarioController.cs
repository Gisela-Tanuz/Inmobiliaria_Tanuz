using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Controllers
{
    public class UsuarioController : Controller
    {
        private RepositorioUsuario repositorio;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration config;



        public UsuarioController(IConfiguration config, IWebHostEnvironment environment)
        {

            repositorio = new RepositorioUsuario(config);
            this.environment = environment;
            this.config = config;

        }
        // GET: UsuarioController
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Index()
        {
            var usuarios = repositorio.Obtener();
            return View(usuarios);

        }

        // GET: UsuarioController/Details/5
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Details(int id)
        {
            var e = repositorio.ObtenerPorId(id);
            return View(e);

        }

        // GET: UsuarioController/Create
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();

        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                u.Clave = hashed;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(u);
                TempData["Id"] = u.Id;
                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificar(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(u);
            }
        }


        // GET: UsuarioController/Edit/5
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Edit";
            var u = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        // POST: UsuarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = "Edit";
            try
            {
                var usuario = repositorio.ObtenerPorId(id);
                if (User.IsInRole("Usuario"))
                {

                    var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)
                    {
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    else
                    {
                        repositorio.Modificar(u);
                        TempData["Mensaje"] = "Datos actualizados correctamente";
                        return RedirectToAction(nameof(Index));
                    }

                }
                // TODO: Add update logic here
                u.Clave = usuario.Clave;
                u.Avatar = usuario.Avatar;
                repositorio.Modificar(u);
                TempData["Mensaje"] = "Datos actualizados correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(vista, u);
            }
        }

        // GET: UsuarioController/Delete/5
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Delete(int id)
        {
            ViewBag.Usuario = repositorio.ObtenerPorId(id);
            var i = repositorio.ObtenerPorId(id);
            return View(i);

        }

        // POST: UsuarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SuperAdministrador")]
        public ActionResult Delete(int id, Usuario u)
        {
            try
            {
                int res = repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        //  [AllowAnonymous]
        // GET: Usuarios/Login/
        /*public ActionResult Login()
        {
            return PartialView("Login", new Login());
        }*/
        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                         password: login.Clave,
                         salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                         prf: KeyDerivationPrf.HMACSHA1,
                         iterationCount: 1000,
                         numBytesRequested: 256 / 8));

                var e = repositorio.ObtenerPorEmail(login.Email);
                if (e == null || e.Clave != hashed)
                {

                    TempData["error"] = "El email o la clave no son correctos";
                    return RedirectToAction(nameof(Index), "Home");
                }

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        //new Claim("fotoUrl", e.Avatar),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));


                if (TempData[" returnUrl"] != null)
                {
                    return Redirect(TempData["returnUrl"].ToString());
                }
                else
                {
                    return RedirectToAction(nameof(Index), "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

        }


        // GET: Usuarios/Edit/5
        // [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }
        [Authorize]
        public IActionResult Avatar()
        {
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);

            //leer el archivo
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            //devolverlo
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        // GET: Usuarios/Create
        [Authorize]
        public ActionResult Foto()
        {
            try
            {
                var u = repositorio.ObtenerPorEmail(User.Identity.Name);
                var stream = System.IO.File.Open(
                    Path.Combine(environment.WebRootPath, u.Avatar.Substring(1)),
                    FileMode.Open,
                    FileAccess.Read);
                var ext = Path.GetExtension(u.Avatar);
                return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // GET: Usuarios/Create
        [Authorize]
        public ActionResult Datos()
        {
            try
            {
                var u = repositorio.ObtenerPorEmail(User.Identity.Name);
                string buffer = "Nombre;Apellido;Email" + Environment.NewLine +
                    $"{u.Nombre};{u.Apellido};{u.Email}";
                var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(buffer));
                var res = new FileStreamResult(stream, "text/plain");
                res.FileDownloadName = "Datos.csv";
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // GET: /salir
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
       // [HttpGet]
       // [Route("Usuario/CambioDeClave")]
        /*public IActionResult CambioDeClave() {
            return View();
        }*/
        [HttpPost]
        [Route("Usuario/CambioDeClave")]
        public async Task<IActionResult> CambioDeClave(CambioDeClave cambioDeClave)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var usuarioA =  repositorio.ObtenerPorEmail(User.Identity.Name);
                    
                    if (usuarioA == null)
                    {
                        return RedirectToAction(nameof(Index), "Home");
                    }


                    string hashedActual = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                      password: cambioDeClave.claveActual,
                      salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                      prf: KeyDerivationPrf.HMACSHA1,
                      iterationCount: 1000,
                      numBytesRequested: 256 / 8));
                    if (usuarioA.Clave == hashedActual)
                    {
                        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: cambioDeClave.claveNueva,
                            salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
                        usuarioA.Clave = hashed;
                        repositorio.CambiarClave(usuarioA);
                        TempData["Mensaje"] = "Contraseña Actualizada con éxito.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Mensaje"] = "No sé puedo cambiar la contraseña.";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrate"] = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }

    }

    
