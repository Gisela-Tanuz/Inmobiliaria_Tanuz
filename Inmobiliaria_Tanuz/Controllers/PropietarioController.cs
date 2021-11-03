
using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorio;
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration config;
        public PropietarioController(IConfiguration config, IWebHostEnvironment environment)
        {
            this.repositorio = new RepositorioPropietario(config);
            this.config = config;
            this.environment = environment;

        }
        // GET: PropietarioController
  
        public ActionResult Index()
        
            {
                IList<Propietario> lta = repositorio.Obtener();
                return View(lta);

            }


        // GET: PropietarioController/Details/5
       
        public ActionResult Details(int id)
        {
            Propietario propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
            
        }

        // GET: PropietarioController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: propietario.Contraseña,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    propietario.Contraseña = hashed;
                    repositorio.Alta(propietario);
                    TempData["Id"] = propietario.IdPropietario;
                    if (propietario.AvatarPropFile != null && propietario.IdPropietario > 0)
                    {
                        string wwwPath = environment.WebRootPath;
                        string path = Path.Combine(wwwPath, "Uploads");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                        string fileName = "avatar_" + propietario.IdPropietario + Path.GetExtension(propietario.AvatarPropFile.FileName);
                        string pathCompleto = Path.Combine(path, fileName);
                        propietario.AvatarProp = Path.Combine("/Uploads/", fileName);
                        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                        {
                            propietario.AvatarPropFile.CopyTo(stream);
                        }
                        repositorio.Modificar(propietario);
                    }
                        return RedirectToAction(nameof(Index));
                }
                else
                    return View(propietario);

            } catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View(propietario);
             }
         }

        // GET: PropietarioController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            return View(p);
            
        }

        // POST: PropietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                var p = repositorio.ObtenerPorId(id);
                propietario.IdPropietario = id;
             //   repositorio.Modificar(propietario);
                propietario.Contraseña = p.Contraseña;
                propietario.AvatarProp = p.AvatarProp;
                repositorio.Modificar(propietario);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropietarioController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            return View(p);
            
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Propietario propietario)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(propietario);
            }
        }
        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.BuscarPropietario(id);
            return View(lista);
        }

    }
}
