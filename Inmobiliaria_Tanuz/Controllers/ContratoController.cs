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
    public class ContratoController : Controller
    {
        private readonly RepositorioContrato repositorio;
        private readonly RepositorioInquilino repoInquilino;
        private readonly RepositorioInmueble repoInmueble;
        private readonly IConfiguration config;

        public ContratoController(IConfiguration config)
        {
            this.repositorio = new RepositorioContrato(config);
            this.repoInquilino = new RepositorioInquilino(config);
            this.repoInmueble = new RepositorioInmueble(config);
            this.config = config;
        }

        // GET: ContratoController

        public ActionResult Index()
        {
            var lista = repositorio.Obtener();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: ContratoController/Details/5

        public ActionResult Details(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            return View(contrato);

        }

        // GET: ContratoController/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Inquilino = repoInquilino.Obtener();
            ViewBag.Inmueble = repoInmueble.Obtener();
            return View();
        }


        // POST: ContratoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Contrato c)
        {
            try
            {

                int res = repositorio.Alta(c);
                Inquilino i = repoInquilino.ObtenerPorId(c.InquilinoId);
                TempData["Id"] = c.Id;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Inquilino = repoInquilino.Obtener();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(c);
            }
        }

        // GET: ContratoController/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            var con = repositorio.ObtenerPorId(id);
            ViewBag.Inquilino = repoInquilino.Obtener();
            ViewBag.Inmueble = repoInmueble.Obtener();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(con);

        }

        // POST: ContratoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id, Contrato contrato)
        {
            try
            {
                contrato.Id = id;
                repositorio.Modificar(contrato);
                TempData["Mensaje"] = "Los datos han sido actualizados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inquilino = repoInquilino.Obtener();
                ViewBag.Inmueble = repoInmueble.Obtener();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(contrato);
            }
        }

        // GET: ContratoController/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var con = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(con);

        }

        // POST: ContratoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Contrato contrato)
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
                return View(contrato);
            }
        }
        public ActionResult BuscarContratoPorFecha(BuscarPorFechas porFechas)
        {
            try
            {
                List<Contrato> lista = repositorio.ObtenerContratoVigente(porFechas.FechaInicio, porFechas.FechaFinal);
                if (lista != null)
                {
                    ViewData["Title"] = "Contratos vigentes";
                    return View(nameof(Index),lista);
                }
                else
                {
                    TempData["Mensaje"] = "No hay contratos disponibles en esas fechas";
                    return RedirectToAction(nameof(Index));
                }
           }
            catch (Exception ex)
            {
             //   TempData["Mensaje"] = "Error";
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }

        }
    }
}


        
            
     