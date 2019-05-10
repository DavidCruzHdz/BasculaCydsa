using ObtenerPesoSAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CYDSA_Sustentabilidad.Controllers
{
    public class UsuariosController : Controller
    {
        public ActionResult Index()
        {

            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();

            int VarUsuario = int.Parse(Session["idUsuario"].ToString());

            if (!context.CPPantallasPermisos.Any(x => x.IdPantalla == 3 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }


            var entity = context.CPUsuario.ToList().Where(x => x.Estatus == true);
            return View(entity);
        }
        // GET: Usuarios
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Pantallas(int id)

        {
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();

            int VarUsuario = int.Parse(Session["idUsuario"].ToString());

            if (!context.CPPantallasPermisos.Any(x => x.IdPantalla == 7 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }


            Session["IdUsuarioPermisos"] = id;

            var entity = context.CPPantallas.ToList();
            var permisos = context.CPPantallasPermisos.Where(x => x.IdUsuario == id);

            foreach (var item in permisos)
            {
                entity.Find(x => x.Id == item.IdPantalla).checkeado = true;

            }
            return View(context.CPPantallas.ToList());
        }


        // POST: Usuarios/Create
        [HttpPost]
        public ActionResult Pantallas(IEnumerable<CPPantallas> entity)
        {
        //[HttpPost]
        //public ActionResult Pantallas(IEnumerable<CPPantallas> entity)
        //{
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            BDObtenerPesoSAPEntities context2 = new BDObtenerPesoSAPEntities();

            int idusuario = (int)Session["IdUsuarioPermisos"];
            var aa = context.CPPantallasPermisos.Where(x => x.IdUsuario == idusuario);
            if (aa.Count() >= 1)
            {
                context.CPPantallasPermisos.RemoveRange(aa);
                context.SaveChanges();
            }


            //entity2.IdPantalla = entity.FirstOrDefault().Id;
            //entity2.idUsuario = (int)Session["IdUsuario"];

            //context2.IdsPantallasPermisos.Add(entity2);
            //context2.SaveChanges();

            foreach (var item in entity)
            {
                if (item.checkeado == true) // si esta chequeado
                {
                    var idPantalla = item.Id;
                    var idUsuario = (int)Session["IdUsuarioPermisos"];
                    CPPantallasPermisos entity2 = new CPPantallasPermisos();

                    entity2.IdPantalla = idPantalla;
                    entity2.IdUsuario = (int)Session["IdUsuarioPermisos"];

                    context2.CPPantallasPermisos.Add(entity2);
                    context2.SaveChanges();

                }


            }



            return Redirect("/Usuarios/Index");

        }



        [HttpPost]
        public ActionResult Login(CPUsuario entity)
        {
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            var exist = context.CPUsuario.Where(x => x.CPNombreUsuario == entity.CPNombreUsuario && x.CPPassword == entity.CPPassword && x.Estatus == true).FirstOrDefault();
            if (exist != null)
            {
                var empresa = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == exist.CPIdUsuario && x.CPPlantaDefault == true).FirstOrDefault().CPIdEmpresa;

                Session["logeado"] = true;
                Session["idUsuario"] = exist.CPIdUsuario;
                Session["idPlantaDF"] = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == exist.CPIdUsuario && x.CPPlantaDefault == true).FirstOrDefault().CPIdEmpresa;
                
                ///Session["TipoCaptura"] = context.CPCatEmpresas.Where(x => x.CPIdEmpresa == empresa).FirstOrDefault().CPIdTipoCaptura;
                Session["TipoCaptura"] = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == exist.CPIdUsuario && x.CPPlantaDefault == true).FirstOrDefault().CPIdTipoCaptura;

                Session["NombrePlanta"] = context.CPCatEmpresas.Where(x => x.CPIdEmpresa == empresa).FirstOrDefault().CPDescripcionEmpresa;
                Session["IdUserAutoriza"] = 0;
                Session.Timeout = 50000;
                //Session["NombrePlanta"] = context.CPCatEmpresas.Where(x => x.CPIdEmpresa == exist.CPIdEmpresa).FirstOrDefault().CPDescripcionEmpresa;
                return Redirect("/Home/Index");
            }
            else
            {
                Session["idUsuario"] = 0;
                Session["logeado"] = false;
                ViewBag.error = "usuario incorrecto";
                return View();
            }

        }
        // GET: Usuarios/Details/5
        public ActionResult DesLogear()
        {

            Session["logeado"] = false;
            Session["idUsuario"] = 0;
            return Redirect("/Usuarios/Login");
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();

            int VarUsuario = int.Parse(Session["idUsuario"].ToString());

            if (!context.CPPantallasPermisos.Any(x => x.IdPantalla == 4 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }


            ViewBag.dropdownPlanta = new SelectList(context.CPCatEmpresas.ToList(), "CPIdEmpresa", "CPDescripcionEmpresa");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        public ActionResult Create(CPUsuario entity)
        {
            //entity.IdsIdCia = 1;
            entity.CPRol_id = 1;
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            try
            {
                entity.Estatus = true;
                // TODO: Add insert logic here
                context.CPUsuario.Add(entity);
                context.SaveChanges();


                CPPermisosPlantas CPPlantas = new CPPermisosPlantas();
                CPPlantas.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                CPPlantas.CPIdUsuario = entity.CPIdUsuario;
                CPPlantas.CPFechaAlta = System.DateTime.Now;
                CPPlantas.CPUsuarioAlta = int.Parse(Session["idUsuario"].ToString());
                CPPlantas.CPPlantaDefault = true;

                context.CPPermisosPlantas.Add(CPPlantas);
                context.SaveChanges();


                for (int j = 1; j < 4; j++)
                {
                    CPPantallasPermisos CPPantallas = new CPPantallasPermisos();
                    CPPantallas.IdPantalla = j;
                    CPPantallas.IdUsuario = entity.CPIdUsuario;

                    context.CPPantallasPermisos.Add(CPPantallas);
                    context.SaveChanges();
                }


                //ViewBag.dropdownPlanta = new SelectList(context.IdsCatEmpresas.ToList(), "IdsIdEmpresa", "IdsDescripcionEmpresa");
                //return View();

                return Redirect("/usuarios");
            }
            catch
            {
                ViewBag.dropdownPlanta = new SelectList(context.CPCatEmpresas.ToList(), "CPIdEmpresa", "CPDescripcionEmpresa");
                return View();
            }
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int id)
        {

            BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();


            int VarUsuario = int.Parse(Session["idUsuario"].ToString());

            if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 4 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }



            ViewBag.dropdownPlanta = new SelectList(db.CPCatEmpresas.ToList(), "CPIdEmpresa", "CPDescripcionEmpresa");
            var list = db.CPUsuario.Find(id);
            return View(list);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CPUsuario collection)
        {
            try
            {
                BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

                int VarUsuario = int.Parse(Session["idUsuario"].ToString());

                if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 4 && x.IdUsuario == VarUsuario))
                {
                    return Redirect("/Home/Index");
                }


                BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
                    var Busqueda = context.CPUsuario.Where(x => (x.CPIdUsuario == collection.CPIdUsuario)).FirstOrDefault();

                    if (Busqueda != null)
                    {
                            collection.CPNombreUsuario = Busqueda.CPNombreUsuario;
                    }
                       

                db.CPUsuario.Attach(collection);
                db.Entry(collection).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                // TODO: Add update logic here

                return RedirectToAction("/Index");
            }
            catch (Exception e)
            {
                return View(e);
            }
        }

        // GET: Usuarios/Delete/5


        // POST: Usuarios/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

                int VarUsuario = int.Parse(Session["idUsuario"].ToString());

                if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 4 && x.IdUsuario == VarUsuario))
                {
                    return Redirect("/Home/Index");
                }
                CPUsuario usuario = new CPUsuario();
                usuario = db.CPUsuario.Where(x => x.CPIdUsuario == id).FirstOrDefault();
                usuario.Estatus = false;

                db.CPUsuario.Attach(usuario);

                db.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(e);
            }
        }

        public ActionResult Permisos(int id)
        {
            BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

            int VarUsuario = int.Parse(Session["idUsuario"].ToString());

            if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 4 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }

            Session["IdUsuarioEmpresa"] = id;
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            var entity = context.CPCatEmpresas.ToList();
            var permisos = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == id);

            foreach (var item in permisos)
            {
                entity.Find(x => x.CPIdEmpresa == item.CPIdEmpresa).checkeado = true;

            }
            return View(entity);
        }

        [HttpPost]
        public ActionResult Permisos(IEnumerable<CPCatEmpresas> entity)
        {

            Session["DefaultEmpresa"] = 0;
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            var idUsuarioEmpresa = (int)Session["IdUsuarioEmpresa"];
            var aa = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == idUsuarioEmpresa);
            if (aa.Count() >= 1)
            {
                if (aa.Where(x => x.CPPlantaDefault == true).Count() >= 1)
                {
                    Session["DefaultEmpresa"] = aa.Where(x => x.CPPlantaDefault == true).FirstOrDefault().CPIdEmpresa;
                }

                context.CPPermisosPlantas.RemoveRange(aa);
                context.SaveChanges();
            }
            foreach (var item in entity)
            {
                if (item.checkeado == true) // si esta chequeado
                {
                    var CPIdEmpresa = item.CPIdEmpresa;
                    var idEmpresa = (int)Session["idUsuarioEmpresa"];
                    CPPermisosPlantas entity2 = new CPPermisosPlantas();
                    entity2.CPIdEmpresa = CPIdEmpresa;
                    entity2.CPIdUsuario = idEmpresa;

                    context.CPPermisosPlantas.Add(entity2);
                    context.SaveChanges();

                }


            }


            BDObtenerPesoSAPEntities contex2 = new BDObtenerPesoSAPEntities();
            var cc = contex2.CPPermisosPlantas.ToList();
            var a = Session["DefaultEmpresa"];
            var PlantasEncontradas = cc.Where(x => x.CPIdUsuario == idUsuarioEmpresa && x.CPIdEmpresa == (Int32)Session["DefaultEmpresa"]);

            // si la planta que estaba como defaul aun existe, se buelve a poner default 
            if (PlantasEncontradas.Count() >= 1)
            {
                CPPermisosPlantas editar = new CPPermisosPlantas();
                editar = context.CPPermisosPlantas.ToList().Where(x => x.CPIdEmpresa == (Int32)Session["DefaultEmpresa"] && x.CPIdUsuario == idUsuarioEmpresa).FirstOrDefault();
                editar.CPPlantaDefault = true;
                context.CPPermisosPlantas.Add(editar);

                context.Entry(editar).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

            }
            else// si no
            {
                if (!context.CPPermisosPlantas.Any(x => x.CPIdUsuario == idUsuarioEmpresa))// si quito todas, se vuelbe  a agregar la planta default
                {
                    CPPermisosPlantas insertar = new CPPermisosPlantas();
                    insertar.CPFechaAlta = System.DateTime.Now;
                    insertar.CPIdEmpresa = (Int32)Session["DefaultEmpresa"];
                    insertar.CPIdUsuario = idUsuarioEmpresa;


                }
                CPPermisosPlantas editar = new CPPermisosPlantas();
                editar = context.CPPermisosPlantas.ToList().Where(x => x.CPIdUsuario == idUsuarioEmpresa).FirstOrDefault();
                editar.CPPlantaDefault = true;
                context.CPPermisosPlantas.Attach(editar);
                context.Entry(editar).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

            }

            return Redirect("/Usuarios/Index");
        }
    }
}
