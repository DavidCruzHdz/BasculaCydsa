using ObtenerPesoSAP.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;

namespace ObtenerPesoSAP.Controllers
{
    public class BitacoraController : Controller
    {
        // GET: Bitacora
        public ActionResult Bitacora()
        {
            BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();
            //var IdPlanta = (int)Session["idPlantaDF"];
            int VarUsuario = int.Parse(Session["idUsuario"].ToString());


            if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 2 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }

            if (Session["idUsuario"] == null)
            {
                return Redirect("/Usuarios/Login");
                //ViewBag.Message = "Mensuales";

                //return View();
            }
            else
            {
                var idplanta = (int)Session["idPlantaDF"];
                var a = db.CPBitacora.Where(x => x.CPIdEmpresa == idplanta);
                return View(a);
            }
        }

        // GET: Bitacora/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Bitacora/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bitacora/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Bitacora/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Bitacora/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Bitacora/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Bitacora/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
