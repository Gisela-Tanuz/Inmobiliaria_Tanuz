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
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino repositorio;
        private readonly IConfiguration config;
        public InquilinoController(IConfiguration config)
        {
            this.repositorio = new RepositorioInquilino(config);
            this.config = config;
        }
        // GET: InquilinoController
  
        public ActionResult Index()
        {
            IList<Inquilino> lta = repositorio.Obtener();
            return View(lta);
        }

        // GET: InquilinoController/Details/5
        
        public ActionResult Details(int id)
        {
            Inquilino inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);

        }

        // GET: InquilinoController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: InquilinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Inquilino inquilino)
        {
          
            repositorio.Alta(inquilino);
            return RedirectToAction(nameof(Index));
        }

        // GET: InquilinoController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            return View(i);
            
        }

        // POST: InquilinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                inquilino.Id = id;
                repositorio.Modificar(inquilino);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InquilinoController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            return View(i);
            
        }

        // POST: InquilinoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Inquilino inquilino)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(inquilino);
            }
        }
    }
}
