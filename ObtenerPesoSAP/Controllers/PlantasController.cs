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
    public class PlantasController : Controller
    {
        private BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

        // GET: Plantas
        public ActionResult Index()
        {
            return View(db.CPCatEmpresas.ToList());
        }

        // GET: Plantas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPCatEmpresas cPCatEmpresas = db.CPCatEmpresas.Find(id);
            if (cPCatEmpresas == null)
            {
                return HttpNotFound();
            }
            return View(cPCatEmpresas);
        }

        // GET: Plantas/Create
        public ActionResult Create()
       {
        //    List<SelectListItem> CPTipo = new List<SelectListItem>()
        //            {
        //                new SelectListItem { Text = "Manual", Value = "1" },
        //                new SelectListItem { Text = "Semiautomatico", Value = "2" },
        //                new SelectListItem { Text = "Automatico", Value = "3" },
        
        //            };
        //    CPTipo.Add(new SelectListItem() { Text = "Seleccione el Tipo de Captura", Value = "NC" });
        //    ViewBag.TipoDeCaptura = CPTipo;

            //ViewBag.dropdownTipos = new SelectList(db.CPCatTipoCaptura.ToList(), "CPIdTipoCaptura", "CPDescripcion");

            return View();
        }

        // POST: Plantas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CPIdEmpresa,CPIdCia,CPIdPlanta,CPDescripcionEmpresa,CPFechaAlta,CPUsuarioAlta,CPFechaCambio,CPUsuarioCambio,TipoDeCaptura")] CPCatEmpresas cPCatEmpresas)
        {
            if (ModelState.IsValid)
            {


                db.CPCatEmpresas.Add(cPCatEmpresas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cPCatEmpresas);
        }

        // GET: Plantas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.dropdownTipos = new SelectList(db.CPCatTipoCaptura.ToList(), "CPIdTipoCaptura", "CPDescripcion");

            CPCatEmpresas cPCatEmpresas = db.CPCatEmpresas.Find(id);
            if (cPCatEmpresas == null)
            {
                return HttpNotFound();
            }
            return View(cPCatEmpresas);
        }

        // POST: Plantas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "CPIdEmpresa,CPIdCia,CPIdPlanta,CPDescripcionEmpresa,CPFechaAlta,CPUsuarioAlta,CPFechaCambio,CPUsuarioCambio,CPTipoCaptura")] CPCatEmpresas cPCatEmpresas)
        public ActionResult Edit(CPCatEmpresas entity)
         {
            if (ModelState.IsValid)
            {
                CPCatEmpresas CmbCatEmpresas = db.CPCatEmpresas.Find(entity.CPIdEmpresa);
                if (CmbCatEmpresas != null)
                {
                    //cPCatEmpresas.CPIdCia = CmbCatEmpresas.CPIdCia;
                    //cPCatEmpresas.CPIdPlanta = CmbCatEmpresas.CPIdPlanta;
                    //cPCatEmpresas.CPFechaAlta = CmbCatEmpresas.CPFechaAlta;
                    //cPCatEmpresas.CPUsuarioAlta = CmbCatEmpresas.CPUsuarioAlta;

                    CmbCatEmpresas.CPDescripcionEmpresa = entity.CPDescripcionEmpresa;
                    CmbCatEmpresas.CPIdTipoCaptura = entity.CPIdTipoCaptura;
                    CmbCatEmpresas.CPFechaCambio = DateTime.Now;
                    CmbCatEmpresas.CPUsuarioCambio = int.Parse(Session["idUsuario"].ToString());
                    
                }
                

                //db.Entry(cPCatEmpresas).State = EntityState.Modified;
                //db.SaveChanges();

                db.CPCatEmpresas.Attach(CmbCatEmpresas);
                db.Entry(CmbCatEmpresas).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        // GET: Plantas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CPCatEmpresas cPCatEmpresas = db.CPCatEmpresas.Find(id);
            if (cPCatEmpresas == null)
            {
                return HttpNotFound();
            }
            return View(cPCatEmpresas);
        }

        // POST: Plantas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CPCatEmpresas cPCatEmpresas = db.CPCatEmpresas.Find(id);
            db.CPCatEmpresas.Remove(cPCatEmpresas);
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
