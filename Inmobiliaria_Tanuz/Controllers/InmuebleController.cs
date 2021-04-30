using Inmobiliaria_Tanuz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repoPropietario;
        private readonly IConfiguration config;

        public InmuebleController(IConfiguration config)
        {
            this.repositorio = new RepositorioInmueble(config);
            this.repoPropietario = new RepositorioPropietario(config);
            this.config = config;
        }

        // GET: InmuebleController
        
        public ActionResult Index()
        {
            var lista = repositorio.Obtener();
            return View(lista);
        }

        // GET: Inmuebles/Details/6
   
        public ActionResult Disponibles(int id)
        {
            var lista = repositorio.ObtenerDisponibles();
            return View(lista);
        }

        // GET: InmuebleController/Details/5

        public ActionResult Details(int id)
        {
       
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                ViewBag.Estado = Inmueble.ObtenerEstado();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
            
        }

        // GET: InmuebleController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Propietario = repoPropietario.Obtener();
            return View();
           
        }

        // POST: InmuebleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Inmueble i)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int res = repositorio.Alta(i);
                    TempData["Id"] = i.IdInmueble;
                    repositorio.EstadoDisponible(i);
                    TempData["Mensaje"] = "Se ha creado un nuevo inmueble";
                    return RedirectToAction(nameof(Index));
                   
                }
                else
                {
                    ViewBag.Propietario = repoPropietario.Obtener();
                    return View(i);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(i);
            }
        }

        // GET: InmuebleController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
          
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Propietario = repoPropietario.Obtener();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }


        // POST: InmuebleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Inmueble entidad)
        { 
            try
            {   
                entidad.IdInmueble = id;
                repositorio.Modificar(entidad);
                ViewBag.Inmueble = Inmueble.ObtenerEstado();
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietario = repoPropietario.Obtener();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // GET: InmuebleController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            Inmueble i = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(i);
           
        }

        // POST: InmuebleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inmueble entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Id"] = "eliminó el inmueble";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The DELETE statement conflicted with the REFERENCE"))
                {
                    var i = repositorio.ObtenerPorId(id);
                    ViewBag.Error = "No se puede eliminar el inmueble, ya que tiene contratos a su nombre";
                }
                else
                {
                    ViewBag.Error = ex.Message;
                    ViewBag.StackTrate = ex.StackTrace;
                }
                return View(entidad);
            }
        }
        
       
    }
}
