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
    public class ListadoPesosController : Controller
    {
        private BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

        // GET: EditarPesos
        public ActionResult Index()
        {
            var PlantaDF = int.Parse(Session["idPlantaDF"].ToString());
            var cPBitacora = db.CPBitacora.Include(c => c.CPCatEmpresas).Include(c => c.CPUsuario).Include(c => c.CPUsuario1);
            return View(cPBitacora.Where(x => x.CPIdEmpresa == PlantaDF).ToList());
        }

        // GET: EditarPesos/Details/5
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

        // GET: EditarPesos/Create
        public ActionResult Create()
        {
            ViewBag.CPIdEmpresa = new SelectList(db.CPCatEmpresas, "CPIdEmpresa", "CPDescripcionEmpresa");
            ViewBag.CPIdTipoVehiculo = new SelectList(db.CPCatTiposDeVehiculos, "CPIdTipoVehiculo", "CPDescripcionVehiculo");
            ViewBag.CPIdUsuarioEnt = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            ViewBag.CPIdUsuarioSal = new SelectList(db.CPUsuario, "CPIdUsuario", "CPNombreUsuario");
            return View();
        }

        // POST: EditarPesos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CPId,CPIdEmpresa,CPIdTransporte,CPNumEconomico,CPPlaca,CPNumPorte,CPNomConductor,CPPesoEntrada,CPPesoSalida,CPPesoNeto,CPIdTipoVehiculo,CPFechaEntrada,CpFechaSalida,CPEntrada,CPSalida,CPIdUsuarioEnt,CPIdUsuarioSal")] CPBitacora cPBitacora)
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

        // GET: EditarPesos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                Session["Id"] = id;
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

        // POST: EditarPesos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CPBitacora entity)
        {
            var Id = int.Parse(Session["Id"].ToString());

            var VarEntrada = 0;
            var VarSalida = 0;
            decimal IdTransporte = 0;

            var Material = 0;
            var NumEco = "";
            var Placas = "";
            var Conductor = "";
            var Carta = "";

            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();

            try
            {
                var ProcesoSAP = 0;
                var Busqueda = context.CPBitacora.Where(x => (x.CPId == Id)).FirstOrDefault();

                if (Busqueda != null)
                {
                    BDObtenerPesoSAPEntities dbCambios = new BDObtenerPesoSAPEntities();
                    CPBitacora Peso = new CPBitacora();
                    Peso.CPIdEmpresa = Busqueda.CPIdEmpresa;
                    Peso.CPIdTransporte = Busqueda.CPIdTransporte;
                    Peso.CPNumEconomico = Busqueda.CPNumEconomico.ToString();
                    Peso.CPNumPorte = Busqueda.CPNumPorte;
                    Peso.CPPlaca = Busqueda.CPPlaca.ToString();
                    Peso.CPNomConductor = Busqueda.CPNomConductor.ToString();
                    Peso.CPNumCelular = Busqueda.CPNumCelular;
                    Peso.CPFechaEntrada = Busqueda.CPFechaEntrada;
                    Peso.CPEntrada = Busqueda.CPEntrada;
                    Peso.CPId = Busqueda.CPId;
                    Peso.CPPesoEntrada = Busqueda.CPPesoEntrada;
                    Peso.CPPesoSalida = Busqueda.CPPesoSalida;
                    Peso.CPSalida = Busqueda.CPSalida;
                    Peso.CPIdUsuarioSal = Busqueda.CPIdUsuarioSal;
                    Peso.CpFechaSalida = Busqueda.CpFechaSalida;
                    Peso.CPIdUsuarioEnt = Busqueda.CPIdUsuarioEnt;
                    Peso.CPIdMaterial = Busqueda.CPIdMaterial;
                    Peso.CPNumDePasos = Busqueda.CPNumDePasos;
                    Peso.CPEstatus = Busqueda.CPEstatus;
                    Peso.CPIdDocumento = Busqueda.CPIdDocumento;
                    Peso.CPPartida = Busqueda.CPPartida;


                    if (Busqueda.CPPesoEntrada != entity.CPPesoEntrada)
                    {
                        VarEntrada = int.Parse(entity.CPPesoEntrada.ToString());
                    }
                    else
                    {
                        VarEntrada = int.Parse(Busqueda.CPPesoEntrada.ToString());
                    }

                    Peso.CPPesoEntrada = VarEntrada;


                    if (Busqueda.CPPesoSalida != entity.CPPesoSalida)
                    {
                        VarSalida = int.Parse(entity.CPPesoSalida.ToString());
                    }
                    else
                    {
                        VarSalida = int.Parse(Busqueda.CPPesoSalida.ToString());
                    }

                    Peso.CPPesoSalida = VarSalida;


                    double VarPesoNeto = (VarSalida - VarEntrada);

                    if (VarPesoNeto < 0)
                    {
                        VarPesoNeto = 0;
                    }

                    Peso.CPPesoNeto = VarPesoNeto;


                    if (Peso.CPPesoSalida == 0)
                    {
                        // esto es para darle reversa siempre y cuando el peso de salida lo teclemos en ceros
                        Peso.CPPesoNeto = 0;
                        Peso.CPSalida = false;
                        Peso.CPNumDePasos = 3;
                        Peso.CPEstatus = 1;
                    }


                    if (Peso.CPPesoEntrada == 0)
                    {
                        // esto es para darle reversa siempre y cuando el peso de salida lo teclemos en ceros
                        Peso.CPPesoSalida = 0;
                        Peso.CPEntrada = false;
                        Peso.CPPesoNeto = 0;
                        Peso.CPSalida = false;
                        Peso.CPNumDePasos = 2;
                        Peso.CPEstatus = 1;
                    }


                    dbCambios.CPBitacora.Attach(Peso);
                    dbCambios.Entry(Peso).State = System.Data.Entity.EntityState.Modified;
                    dbCambios.SaveChanges();

                    if (Peso.CPPesoSalida == 0)
                    {
                        EnviaPesos.SI_OA_Peso_VehiculoClient Client = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                        EnviaPesos.DT_Peso_Vehiculo contextWs = new EnviaPesos.DT_Peso_Vehiculo();
                        EnviaPesos.DT_Peso_VehiculoData datosWs = new EnviaPesos.DT_Peso_VehiculoData();

                        Client.ClientCredentials.UserName.UserName = "POCYDSAINT";
                        Client.ClientCredentials.UserName.Password = "Cydsa2019$";

                        datosWs.Vehicle_Number = Peso.CPIdTransporte.ToString();
                        datosWs.Weight_Initial = Peso.CPPesoEntrada.ToString();
                        datosWs.Weight_Final = "0";
                        datosWs.Economic_Number = Peso.CPNumEconomico.ToString();
                        datosWs.Licence_Plate = Peso.CPPlaca.ToString();
                        datosWs.Driver_Name = Peso.CPNomConductor.ToString();
                        datosWs.Carriage_Number = Peso.CPNumPorte.ToString();

                        datosWs.Check_In = "";
                        datosWs.Load_Start = "";
                        datosWs.Load_End = "X";


                        contextWs.data = datosWs;
                        var resultado = Client.SI_OA_Peso_Vehiculo(contextWs);

                        // esto es para darle reversa siempre y cuando el peso de salida lo teclemos en ceros
                        ViewBag.VarConsecutivo = int.Parse(Session["IdTransporte"].ToString());
                        ViewBag.Message = "Se cancelo la salida del peso bruto, vuelva a realizar nuevamente este paso";
                        return RedirectToAction("Index");
                    }


                    if (Peso.CPPesoEntrada == 0)
                    {
                        EnviaPesos.SI_OA_Peso_VehiculoClient Client = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                        EnviaPesos.DT_Peso_Vehiculo contextWs = new EnviaPesos.DT_Peso_Vehiculo();
                        EnviaPesos.DT_Peso_VehiculoData datosWs = new EnviaPesos.DT_Peso_VehiculoData();

                        Client.ClientCredentials.UserName.UserName = "POCYDSAINT";
                        Client.ClientCredentials.UserName.Password = "Cydsa2019$";

                        datosWs.Vehicle_Number = Peso.CPIdTransporte.ToString();
                        datosWs.Weight_Initial = "0";
                        datosWs.Weight_Final = "0";
                        datosWs.Economic_Number = Peso.CPNumEconomico.ToString();
                        datosWs.Licence_Plate = Peso.CPPlaca.ToString();
                        datosWs.Driver_Name = Peso.CPNomConductor.ToString();
                        datosWs.Carriage_Number = Peso.CPNumPorte.ToString();

                        datosWs.Check_In = "";
                        datosWs.Load_Start = "X";
                        datosWs.Load_End = "X";


                        contextWs.data = datosWs;
                        var resultado = Client.SI_OA_Peso_Vehiculo(contextWs);

                        // esto es para darle reversa siempre y cuando el peso de salida lo teclemos en ceros
                        ViewBag.VarConsecutivo = int.Parse(Session["IdTransporte"].ToString());
                        ViewBag.Message = "Se cancelo el peso de la Tara y el peso bruto, vuelva a realizar nuevamente el pesaje";
                        return RedirectToAction("Index");
                    }

                    IdTransporte = decimal.Parse(Busqueda.CPIdTransporte.ToString());
                    NumEco = Busqueda.CPNumEconomico.ToString();
                    Placas = Busqueda.CPPlaca.ToString();
                    Conductor = Busqueda.CPNomConductor.ToString();
                    Carta = Busqueda.CPNumPorte;

                }
                else
                {
                    ViewBag.VarConsecutivo = int.Parse(Session["IdTransporte"].ToString());
                    ViewBag.Message = "No se encontro el nnumero de transporte";
                    return View();
                }




                var existente = "";
                var mensaje = "";
                ProcesoSAP = 0;

                for (int x = 2; x < 5; x += 1)
                {
                    EnviaPesos.SI_OA_Peso_VehiculoClient Client = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                    EnviaPesos.DT_Peso_Vehiculo contextWs = new EnviaPesos.DT_Peso_Vehiculo();
                    EnviaPesos.DT_Peso_VehiculoData datosWs = new EnviaPesos.DT_Peso_VehiculoData();

                    Client.ClientCredentials.UserName.UserName = "POCYDSAINT";
                    Client.ClientCredentials.UserName.Password = "Cydsa2019$";

                    datosWs.Vehicle_Number = IdTransporte.ToString();
                    datosWs.Weight_Initial = VarEntrada.ToString();
                    datosWs.Weight_Final = VarSalida.ToString();
                    datosWs.Economic_Number = NumEco.ToString();
                    datosWs.Licence_Plate = Placas.ToString();
                    datosWs.Driver_Name = Conductor.ToString();
                    datosWs.Carriage_Number = Carta.ToString();

                    ProcesoSAP = x;

                    switch (ProcesoSAP)
                    {
                        case 2:
                            datosWs.Check_In = "X";
                            datosWs.Load_Start = "";
                            datosWs.Load_End = "";
                            break;
                        case 3:
                            datosWs.Check_In = "";
                            datosWs.Load_Start = "X";
                            datosWs.Load_End = "";
                            break;
                        case 4:
                            datosWs.Check_In = "";
                            datosWs.Load_Start = "";
                            datosWs.Load_End = "X";
                            break;
                    }

                    contextWs.data = datosWs;
                    var resultado = Client.SI_OA_Peso_Vehiculo(contextWs);


                    if (existente == "E" && existente == "A")
                    {

                    }

                }

                var existente2 = "";
                var mensaje2 = "";

                CierreProceso.SI_OS_UpdatePickingClient Client2 = new CierreProceso.SI_OS_UpdatePickingClient();
                CierreProceso.DT_UpdatePicking contextWs2 = new CierreProceso.DT_UpdatePicking();
                CierreProceso.DT_UpdatePickingData datosWs2 = new CierreProceso.DT_UpdatePickingData();

                Client2.ClientCredentials.UserName.UserName = "POCYDSAINT";
                Client2.ClientCredentials.UserName.Password = "Cydsa2019$";

                datosWs2.Vehicle_Number = IdTransporte.ToString();
                contextWs2.data = datosWs2;
                var resultado2 = Client2.SI_OS_UpdatePicking(contextWs2);
                existente2 = resultado2[0].Response;
                mensaje2 = resultado2[0].Message;

                if (existente2 != "E")
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    var PlantaDF = int.Parse(Session["idPlantaDF"].ToString());
                    var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == PlantaDF) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                    if (CierrePasos != null)
                    {
                        BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                        CPBitacora Regresar = new CPBitacora();
                        Regresar.CPId = CierrePasos.CPId;
                        Regresar.CPIdTransporte = int.Parse(Session["IdTransporte"].ToString());

                        Regresar.CPNumEconomico = CierrePasos.CPNumEconomico;
                        Regresar.CPPlaca = CierrePasos.CPPlaca;
                        Regresar.CPNomConductor = CierrePasos.CPNomConductor;
                        Regresar.CPNumPorte = CierrePasos.CPNumPorte;

                        Regresar.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                        Regresar.CPIdUsuarioEnt = int.Parse(Session["idUsuario"].ToString());
                        Regresar.CPIdMaterial = CierrePasos.CPIdMaterial;
                        Regresar.CPNumCelular = CierrePasos.CPNumCelular;
                        Regresar.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());

                        switch (ProcesoSAP)
                        {
                            case 2:
                                //CPPartidas cPPartida = CmbRegresa.CPPartidas.Find(CierrePasos.CPId);
                                //CmbRegresa.CPPartidas.Remove(cPPartida);
                                //CmbRegresa.SaveChanges();

                                CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                CmbRegresa.CPBitacora.Remove(cPBitacora);
                                CmbRegresa.SaveChanges();


                                var Transporte = int.Parse(Session["IdTransporte"].ToString());

                                var BuscaPartidas = context.CPPartidas.Where(w => (w.CPId == Transporte)).FirstOrDefault();
                                if (BuscaPartidas != null)
                                {
                                    CPPartidas Partidas = context.CPPartidas.Find(BuscaPartidas.CPId);
                                    context.CPPartidas.Remove(Partidas);
                                    context.SaveChanges();

                                }
                                break;
                            case 3:
                                Regresar.CPPesoEntrada = CierrePasos.CPPesoEntrada;
                                Regresar.CPEntrada = CierrePasos.CPEntrada;

                                Regresar.CPFechaEntrada = CierrePasos.CPFechaEntrada;
                                Regresar.CPPesoEntrada = 0;
                                Regresar.CPEntrada = false;

                                Regresar.CpFechaSalida = CierrePasos.CpFechaSalida;
                                Regresar.CPPesoSalida = 0;
                                Regresar.CPSalida = false;

                                Regresar.CPPesoNeto = 0;
                                Regresar.CPNumDePasos = 2;
                                Regresar.CPEstatus = 1;
                                CmbRegresa.CPBitacora.Attach(Regresar);
                                CmbRegresa.Entry(Regresar).State = System.Data.Entity.EntityState.Modified;
                                CmbRegresa.SaveChanges();
                                break;
                            case 4:
                                Regresar.CPPesoEntrada = CierrePasos.CPPesoEntrada;
                                Regresar.CPEntrada = CierrePasos.CPEntrada;
                                Regresar.CPFechaEntrada = CierrePasos.CPFechaEntrada;

                                Regresar.CpFechaSalida = CierrePasos.CpFechaSalida;
                                Regresar.CPPesoSalida = 0;
                                Regresar.CPSalida = false;

                                Regresar.CPPesoNeto = 0;
                                Regresar.CPNumDePasos = 3;
                                Regresar.CPEstatus = 1;
                                CmbRegresa.CPBitacora.Attach(Regresar);
                                CmbRegresa.Entry(Regresar).State = System.Data.Entity.EntityState.Modified;
                                CmbRegresa.SaveChanges();
                                break;
                        }

                        ViewBag.VarConsecutivo = int.Parse(Session["IdTransporte"].ToString());
                        ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                        //MaterialesADO cargaMaterialADO = new MaterialesADO();
                        //ViewBag.dropdownMateriales = new SelectList(cargaMaterialADO.cmbMateriales(int.Parse(Session["idPlantaDF"].ToString())), "CPIdMaterial", "CPDescripcionMaterial");

                        ViewBag.Message = mensaje2;
                        return View();
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error al agregar o modificar datos", e);
            }
            return View();
        }


        // GET: EditarPesos/Delete/5
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

        // POST: EditarPesos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var IdPlanta = int.Parse(Session["idPlantaDF"].ToString());


            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            CPBitacora cPBitacora = context.CPBitacora.Find(id);
            context.CPBitacora.Remove(cPBitacora);
            context.SaveChanges();

            //CPBitacora cPBitacora = db.CPBitacora.Find(id);
            //db.CPBitacora.Remove(cPBitacora);
            //db.SaveChanges();

            var CierrePasos = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == id)).FirstOrDefault();
            if (CierrePasos != null)
            {
                CPPartidas cPPartida = context.CPPartidas.Find(CierrePasos.CPId);
                context.CPPartidas.Remove(cPPartida);
                context.SaveChanges();

            }


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
