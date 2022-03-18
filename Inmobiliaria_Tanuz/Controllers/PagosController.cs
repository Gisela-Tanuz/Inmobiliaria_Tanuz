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
    public class PagosController : Controller
    {
        private readonly RepositorioPagos repositorio;
        private readonly RepositorioContrato repoContrato;
        private readonly RepositorioInmueble repoInmueble;
        private readonly IConfiguration config;

        public PagosController(IConfiguration config)
        {
            this.repositorio = new RepositorioPagos(config);
            this.repoContrato = new RepositorioContrato(config);
            this.repoInmueble = new RepositorioInmueble(config);
            this.config = config;
        }
        // GET: PagosController
      
        public ActionResult Index()
        {

            var lista = repositorio.Obtener();
            return View(lista);
        }

        // GET: PagosController/Details/5

        public ActionResult Details(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            return View(pago);

        }

        // GET: PagosController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(int id)
        {
            try
            {
                
                ViewBag.Contrato = repoContrato.ObtenerPorId(id);
                Contrato c = repoContrato.ObtenerPorId(id);
                IList<Pagos> pagos = repositorio.ObtenerPagoxContrato(c.IdContrato);
               
                if (pagos.Count == 0)
                {
                    ViewBag.NroDePago = 1;
                }
                else
                {
                    int nrop = pagos.Count;
                    ViewBag.NroDePago = nrop + 1;
                }

                return View();

            }
            catch (Exception ex)
            {
                throw ;
            }
            

        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Pagos p)
        {

            try
            {
                int res = repositorio.Alta(p);
              //  IList<Pagos> pagos = repositorio.ObtenerPagoxContrato(res);
                
                TempData["Mensaje"] = "Se ha creado un nuevo pago";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: PagosController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {   
            var p = repositorio.ObtenerPorId(id);
            return View(p);

            
        }

        // POST: PagosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Pagos pagos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    pagos.IdPago = id;
                    int res = repositorio.Modificar(pagos);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: PagosController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var p = repositorio.ObtenerPorId(id);
            
            return View(p);
            
        }

        // POST: PagosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Pagos pagos)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(pagos);
            }
        }
        
        public ActionResult VerPagos(int id)
        {
            ViewBag.Contrato = repoContrato.ObtenerPorId(id);
            IList<Pagos> pago = repositorio.ObtenerPagoxContrato(id);
            return View(pago);
           
            
        }
    }
}
