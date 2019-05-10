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
    public class MaterialesController : Controller
    {
        private BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

        // GET: Materiales
        public ActionResult Index()
        {
            var Planta = int.Parse(Session["idPlantaDF"].ToString());
            var cPCatMateriales = db.CPCatMateriales.Include(c => c.CPCatEmpresas).Include(c => c.CPCatUnidades).Include(c => c.CPUsuario).Include(c => c.CPUsuario1);
            return View(cPCatMateriales.Where(x => x.CPIdEmpresa == Planta).ToList());
        }

        // GET: Materiales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPCatMateriales cPCatMateriales = db.CPCatMateriales.Find(id);
            if (cPCatMateriales == null)
            {
                return HttpNotFound();
            }
            return View(cPCatMateriales);
        }

        // GET: Materiales/Create
        public ActionResult Create()
        {
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa");
            ViewBag.CPIdUnidadMedida = new SelectList(db.CPCatUnidades, "CPIdUnidadMedida", "CPDescripcionUnidadMedida");
            ViewBag.CPUsuarioAlta = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            ViewBag.CPUsuarioCambio = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            return View();
        }

        // POST: Materiales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CPIdMaterial,CPIdEmpresa,CPIdMaterialAnt,CPIdMaterialSAP,CPDescripcionMaterial,CPPesoRequerido,CPFactorMin,CPFactorMax,CPSePesa,CPRequiereAutoriza,CPIdUnidadMedida,CPFechaAlta,CPUsuarioAlta,CPFechaCambio,CPUsuarioCambio")] CPCatMateriales cPCatMateriales)
        {
            if (ModelState.IsValid)
            {
                CPCatMateriales Materiales = new CPCatMateriales();

                Materiales.CPIdEmpresa = cPCatMateriales.CPIdEmpresa;
                Materiales.CPIdMaterialAnt = cPCatMateriales.CPIdMaterialAnt;
                Materiales.CPIdMaterialSAP = cPCatMateriales.CPIdMaterialSAP;
                Materiales.CPDescripcionMaterial = cPCatMateriales.CPDescripcionMaterial;
                Materiales.CPPesoRequerido = cPCatMateriales.CPPesoRequerido;
                Materiales.CPFactorMin = cPCatMateriales.CPFactorMin;
                Materiales.CPFactorMax = cPCatMateriales.CPFactorMax;
                Materiales.CPSePesa = cPCatMateriales.CPSePesa;
                Materiales.CPRequiereAutoriza = cPCatMateriales.CPRequiereAutoriza;
                Materiales.CPIdUnidadMedida = cPCatMateriales.CPIdUnidadMedida;
                Materiales.CPFechaAlta = DateTime.Now;
                Materiales.CPUsuarioAlta = int.Parse(Session["idUsuario"].ToString());
                Materiales.CPFechaCambio = DateTime.Now;
                Materiales.CPUsuarioCambio = int.Parse(Session["idUsuario"].ToString());
                Materiales.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                db.CPCatMateriales.Add(Materiales);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPCatMateriales.CPIdEmpresa);
            ViewBag.CPIdUnidadMedida = new SelectList(db.CPCatUnidades, "CPIdUnidadMedida", "CPDescripcionUnidadMedida", cPCatMateriales.CPIdUnidadMedida);
            ViewBag.CPUsuarioAlta = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPCatMateriales.CPUsuarioAlta);
            ViewBag.CPUsuarioCambio = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPCatMateriales.CPUsuarioCambio);
            return View(cPCatMateriales);
        }

        // GET: Materiales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPCatMateriales cPCatMateriales = db.CPCatMateriales.Find(id);
            if (cPCatMateriales == null)
            {
                return HttpNotFound();
            }
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPCatMateriales.CPIdEmpresa);
            ViewBag.CPIdUnidadMedida = new SelectList(db.CPCatUnidades, "CPIdUnidadMedida", "CPDescripcionUnidadMedida", cPCatMateriales.CPIdUnidadMedida);
            ViewBag.CPUsuarioAlta = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPCatMateriales.CPUsuarioAlta);
            ViewBag.CPUsuarioCambio = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPCatMateriales.CPUsuarioCambio);
            return View(cPCatMateriales);
        }

        // POST: Materiales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CPIdMaterial,CPIdEmpresa,CPIdMaterialAnt,CPIdMaterialSAP,CPDescripcionMaterial,CPPesoRequerido,CPFactorMin,CPFactorMax,CPSePesa,CPRequiereAutoriza,CPIdUnidadMedida,CPFechaAlta,CPUsuarioAlta,CPFechaCambio,CPUsuarioCambio")] CPCatMateriales cPCatMateriales)
        {
            if (ModelState.IsValid)
            {
                cPCatMateriales.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                db.Entry(cPCatMateriales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPCatMateriales.CPIdEmpresa);
            ViewBag.CPIdUnidadMedida = new SelectList(db.CPCatUnidades, "CPIdUnidadMedida", "CPDescripcionUnidadMedida", cPCatMateriales.CPIdUnidadMedida);
            ViewBag.CPUsuarioAlta = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPCatMateriales.CPUsuarioAlta);
            ViewBag.CPUsuarioCambio = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPCatMateriales.CPUsuarioCambio);
            return View(cPCatMateriales);
        }

        // GET: Materiales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPCatMateriales cPCatMateriales = db.CPCatMateriales.Find(id);
            if (cPCatMateriales == null)
            {
                return HttpNotFound();
            }
            return View(cPCatMateriales);
        }

        // POST: Materiales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CPCatMateriales cPCatMateriales = db.CPCatMateriales.Find(id);
            db.CPCatMateriales.Remove(cPCatMateriales);
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
