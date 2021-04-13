using Inmobiliaria_Tanuz.Models;
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: InquilinoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inquilino inquilino)
        {
            /* try
             {
                 return RedirectToAction(nameof(Index));
             }
             catch
             {
                 return View();
             }*/
            repositorio.Alta(inquilino);
            return RedirectToAction(nameof(Index));
        }

        // GET: InquilinoController/Edit/5
        public ActionResult Edit(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            return View(i);
            
        }

        // POST: InquilinoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                inquilino.IdInquilino = id;
                repositorio.Modificar(inquilino);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: InquilinoController/Delete/5
        public ActionResult Delete(int id)
        {
            var i = repositorio.ObtenerPorId(id);
            return View(i);
            
        }

        // POST: InquilinoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Inquilino inquilino)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(inquilino);
            }
        }
    }
}
