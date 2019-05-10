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
    public class CPPermisosPlantasController : Controller
    {
        private BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

        // GET: CPPermisosPlantas
        public ActionResult Index()
        {

            int VarUsuario = int.Parse(Session["idUsuario"].ToString());
            if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 8 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }

            var cPPermisosPlantas = db.CPPermisosPlantas.Include(c => c.CPCatEmpresas).Include(c => c.CPUsuario);
            return View(cPPermisosPlantas.ToList());
        }

        // GET: CPPermisosPlantas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPPermisosPlantas cPPermisosPlantas = db.CPPermisosPlantas.Find(id);
            if (cPPermisosPlantas == null)
            {
                return HttpNotFound();
            }
            return View(cPPermisosPlantas);
        }

        // GET: CPPermisosPlantas/Create
        public ActionResult Create()
        {
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa");
            ViewBag.CPIdUsuario = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            return View();
        }

        // POST: CPPermisosPlantas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CPId,CPIdEmpresa,CPIdUsuario,CPFechaAlta,CPUsuarioAlta,CPFechaCambio,CPUsuarioCambio,CPPlantaDefault")] CPPermisosPlantas cPPermisosPlantas)
        {
            if (ModelState.IsValid)
            {
                db.CPPermisosPlantas.Add(cPPermisosPlantas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPPermisosPlantas.CPIdEmpresa);
            ViewBag.CPIdUsuario = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPPermisosPlantas.CPIdUsuario);
            return View(cPPermisosPlantas);
        }

        // GET: CPPermisosPlantas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPPermisosPlantas cPPermisosPlantas = db.CPPermisosPlantas.Find(id);
            if (cPPermisosPlantas == null)
            {
                return HttpNotFound();
            }
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPPermisosPlantas.CPIdEmpresa);
            ViewBag.CPIdUsuario = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPPermisosPlantas.CPIdUsuario);
            return View(cPPermisosPlantas);
        }

        // POST: CPPermisosPlantas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CPId,CPIdEmpresa,CPIdUsuario,CPFechaAlta,CPUsuarioAlta,CPFechaCambio,CPUsuarioCambio,CPPlantaDefault")] CPPermisosPlantas cPPermisosPlantas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cPPermisosPlantas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa", cPPermisosPlantas.CPIdEmpresa);
            ViewBag.CPIdUsuario = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario", cPPermisosPlantas.CPIdUsuario);
            return View(cPPermisosPlantas);
        }

        // GET: CPPermisosPlantas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPPermisosPlantas cPPermisosPlantas = db.CPPermisosPlantas.Find(id);
            if (cPPermisosPlantas == null)
            {
                return HttpNotFound();
            }
            return View(cPPermisosPlantas);
        }

        // POST: CPPermisosPlantas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CPPermisosPlantas cPPermisosPlantas = db.CPPermisosPlantas.Find(id);
            db.CPPermisosPlantas.Remove(cPPermisosPlantas);
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
