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
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorio;
        private readonly IConfiguration config;
        public PropietarioController(IConfiguration config)
        {
            this.repositorio = new RepositorioPropietario(config);
            this.config = config;
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            /*  try
              {
                  return RedirectToAction(nameof(Index));
              }
              catch
              {
                  return View();
              }*/
            repositorio.Alta(propietario);
            return RedirectToAction(nameof(Index));
        }

        // GET: PropietarioController/Edit/5
        public ActionResult Edit(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            return View(p);
            
        }

        // POST: PropietarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                propietario.IdPropietario = id;
                repositorio.Modificar(propietario);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropietarioController/Delete/5
        public ActionResult Delete(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            return View(p);
            
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}
