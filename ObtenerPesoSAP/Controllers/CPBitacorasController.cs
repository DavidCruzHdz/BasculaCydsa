using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ObtenerPesoSAP.Models;

namespace ObtenerPesoSAP.Controllers
{
    public class CPBitacorasController : Controller
    {
        private BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

        // GET: CPBitacoras
        public ActionResult Index()
        {
            var cPBitacora = db.CPBitacora.Include(c => c.CPCatEmpresas).Include(c => c.CPUsuario).Include(c => c.CPUsuario1);
            return View(cPBitacora.ToList());
        }

        // GET: CPBitacoras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPBitacora cPBitacora = db.CPBitacora.Find(id);
            if (cPBitacora == null)
            {
                return HttpNotFound();
            }
            return View(cPBitacora);
        }

        // GET: CPBitacoras/Create
        public ActionResult Create()
        {
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa");
            ViewBag.CPIdTipoVehiculo = new SelectList(db.CPCatTiposDeVehiculos, "CPIdMaterial", "CPDescripcionMaterial");
            ViewBag.CPIdUsuarioEnt = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            ViewBag.CPIdUsuarioSal = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            return View();
        }

        // POST: CPBitacoras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CPId,CPIdEmpresa,CPIdMaterial,CPNumEconomico,CPPlaca,CPNumPorte,CPNomConductor,CPPesoEntrada,CPPesoSalida,CPPesoNeto,CPIdTipoVehiculo,CPFechaEntrada,CpFechaSalida,CPEntrada,CPSalida,CPIdUsuarioEnt,CPIdUsuarioSal")] CPBitacora cPBitacora)
        {
            if (ModelState.IsValid)
            {
                db.CPBitacora.Add(cPBitacora);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPBitacora.CPIdEmpresa);
            ViewBag.CPIdTipoVehiculo = new SelectList(db.CPCatMateriales, "CPIdMaterial", "CPDescripcionMaterial", cPBitacora.CPIdMaterial);
            ViewBag.CPIdUsuarioEnt = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPBitacora.CPIdUsuarioEnt);
            ViewBag.CPIdUsuarioSal = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPBitacora.CPIdUsuarioSal);
            return View(cPBitacora);
        }

        // GET: CPBitacoras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPBitacora cPBitacora = db.CPBitacora.Find(id);
            if (cPBitacora == null)
            {
                return HttpNotFound();
            }
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPBitacora.CPIdEmpresa);
            ViewBag.CPIdTipoVehiculo = new SelectList(db.CPCatMateriales, "CPIdMaterial", "CPDescripcionMaterial", cPBitacora.CPIdMaterial);
            ViewBag.CPIdUsuarioEnt = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPBitacora.CPIdUsuarioEnt);
            ViewBag.CPIdUsuarioSal = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPBitacora.CPIdUsuarioSal);
            return View(cPBitacora);
        }

        // POST: CPBitacoras/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CPId,CPIdEmpresa,CPIdMaterial,CPNumEconomico,CPPlaca,CPNumPorte,CPNomConductor,CPPesoEntrada,CPPesoSalida,CPPesoNeto,CPIdTipoVehiculo,CPFechaEntrada,CpFechaSalida,CPEntrada,CPSalida,CPIdUsuarioEnt,CPIdUsuarioSal")] CPBitacora cPBitacora)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cPBitacora).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPBitacora.CPIdEmpresa);
            ViewBag.CPIdTipoVehiculo = new SelectList(db.CPCatMateriales, "CPIdMaterial", "CPDescripcionMaterial", cPBitacora.CPIdMaterial);
            ViewBag.CPIdUsuarioEnt = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPBitacora.CPIdUsuarioEnt);
            ViewBag.CPIdUsuarioSal = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPBitacora.CPIdUsuarioSal);
            return View(cPBitacora);
        }

        // GET: CPBitacoras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPBitacora cPBitacora = db.CPBitacora.Find(id);
            if (cPBitacora == null)
            {
                return HttpNotFound();
            }
            return View(cPBitacora);
        }

        // POST: CPBitacoras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CPBitacora cPBitacora = db.CPBitacora.Find(id);
            db.CPBitacora.Remove(cPBitacora);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
