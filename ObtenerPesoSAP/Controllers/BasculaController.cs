using ObtenerPesoSAP.ADO;
using ObtenerPesoSAP.Models;
//using ObtenerPesoSAP.SAPEnviarPeso;
using ObtenerPesoSAP.DocEntrega;
using ObtenerPesoSAP.PartidasMateriales;
using ObtenerPesoSAP.ValidaTransporte;
using ObtenerPesoSAP.EnviaPesos;
using ObtenerPesoSAP.CierreProceso;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Mvc.Ajax;
using System.Web.UI;

namespace ObtenerPesoSAP.Controllers
{
    public class BasculaController : Controller
    {
        //public bool EstatusEntrada { get; private set; }
        BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();

        public ActionResult Bascula()
        {
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            int VarUsuario = int.Parse(Session["idUsuario"].ToString());


            if (!db.CPPantallasPermisos.Any(x => x.IdPantalla == 1 && x.IdUsuario == VarUsuario))
            {
                return Redirect("/Home/Index");
            }

            ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");
            if (Session["idUsuario"] == null)
            {
                ViewBag.Message = "";
                return Redirect("/Usuarios/Login");
            }
            else
            {
                if (Session["IdUserAutoriza"] != "1")
                {
                    Session["IdUserAutoriza"] = 0;
                }

                ViewData["PlantaId"] = int.Parse(Session["idPlantaDF"].ToString());
                Session["Validacion"] = 1;
                ViewBag.Message = "";


                return View();
            }
        }

        public ContentResult GetText()
        {
            throw new Exception("Este es un error");
        }


        // POST: Bitacora/Grabar
        [HttpPost]
        public ActionResult Bascula(CPBitacora entity)
        {
            var AutorizacionSAP = 0;
            var ProcesoSAP = 0;
            //int IdTransporte = 0;
            var IdPlanta = int.Parse(Session["idPlantaDF"].ToString());
            var IdTransporte = decimal.Parse(Session["IdTransporte"].ToString());

            var VarEntrada = 0;
            var VarSalida = 0;
            long VarPesoNeto = 0;
            decimal MontoPesoNeto = 0;

            decimal Material = 0;
            var NumEco = "";
            var Placas = "";
            var Conductor = "";
            var Carta = "";
            var Celular = "";
            var Autorizacion = Session["IdUserAUtoriza"];
            decimal Transporte = 0;

            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();

            try
            {
                var Busqueda = context.CPBitacora.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdTransporte == IdTransporte)).FirstOrDefault();

                if (Busqueda != null)
                {

                    var ValidaPasos = context.CPBitacora.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdTransporte == IdTransporte) & (x.CPNumDePasos != 4) & (x.CPEstatus != 2)).FirstOrDefault();
                    if (ValidaPasos != null)
                    {
                        BDObtenerPesoSAPEntities dbSalida = new BDObtenerPesoSAPEntities();
                        CPBitacora Peso = new CPBitacora();
                        Peso.CPIdTransporte = IdTransporte;
                        Peso.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                        Peso.CPIdMaterial = decimal.Parse(ValidaPasos.CPIdMaterial.ToString());

                        NumEco = entity.CPNumEconomico.ToString();
                        Placas = entity.CPPlaca.ToString();
                        Conductor = entity.CPNomConductor.ToString();
                        Carta = entity.CPNumPorte;
                        Celular = entity.CPNumCelular;
                        Material = decimal.Parse(ValidaPasos.CPIdMaterial.ToString());

                        Peso.CPNumEconomico = NumEco;
                        Peso.CPNumPorte = Carta;
                        Peso.CPPlaca = Placas;
                        Peso.CPNomConductor = Conductor;
                        Peso.CPNumCelular = Celular;
                        Peso.CPIdMaterial = ValidaPasos.CPIdMaterial;
                        Peso.CPIdDocumento = ValidaPasos.CPIdDocumento;

                        if (Busqueda.CPFechaEntrada != entity.CPFechaEntrada)
                        {
                            if (entity.CPFechaEntrada != null)
                            {
                                Peso.CPFechaEntrada = entity.CPFechaEntrada;
                            }
                        }
                        else
                        {
                            Peso.CPFechaEntrada = Busqueda.CPFechaEntrada;
                        }

                        Peso.CPFechaEntrada = Busqueda.CPFechaEntrada;
                        Peso.CPEntrada = true;
                        Peso.CPId = Busqueda.CPId;

                        if (Busqueda.CPPesoEntrada != entity.CPPesoEntrada & entity.CPPesoEntrada != null)
                        {
                            ProcesoSAP = 3;
                            VarEntrada = int.Parse(entity.CPPesoEntrada.ToString());
                        }
                        else
                        {
                            VarEntrada = int.Parse(Busqueda.CPPesoEntrada.ToString());
                        }

                        Peso.CPPesoEntrada = VarEntrada;


                        if (Busqueda.CPPesoSalida != entity.CPPesoSalida & entity.CPPesoSalida != null)
                        {
                            ProcesoSAP = 4;
                            AutorizacionSAP = 1;
                            VarSalida = int.Parse(entity.CPPesoSalida.ToString());
                            Peso.CPPesoSalida = VarSalida;
                            Peso.CPSalida = true;
                            Peso.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());


                            DocEntrega.SI_OS_Consulta_NumCargaClient ClientDoc = new DocEntrega.SI_OS_Consulta_NumCargaClient();
                            DocEntrega.DT_Consulta_NumCarga contextWsDoc = new DocEntrega.DT_Consulta_NumCarga();
                            DocEntrega.DT_Consulta_NumCargaData datosWsDoc = new DocEntrega.DT_Consulta_NumCargaData();

                            ClientDoc.ClientCredentials.UserName.UserName = "POCYDSAINT";
                            ClientDoc.ClientCredentials.UserName.Password = "Cydsa2019$";

                            datosWsDoc.Numero_Transporte = Session["IdTransporte"].ToString();

                            contextWsDoc.data = datosWsDoc;

                            var resultadoDoc = ClientDoc.SI_OS_Consulta_NumCarga(contextWsDoc);
                            var RegDoc = resultadoDoc.OPTIONS.item.Count();

                            decimal SumaDeProd = 0;
                            var elemetomayor = "";
                            decimal valoromayor = 0;
                            decimal VarDocumento = 0;
                            var Pda = 1;

                            for (int i = 0; i < RegDoc; i++)
                            {
                                PartidasMateriales.SI_OA_TM101_Consulta_EntregaClient ClientMat = new PartidasMateriales.SI_OA_TM101_Consulta_EntregaClient();
                                PartidasMateriales.DT_TM101_Consulta_Entrega_Req contextWsMat = new PartidasMateriales.DT_TM101_Consulta_Entrega_Req();
                                PartidasMateriales.DT_TM101_Consulta_Entrega_ReqItem datosWsMat = new PartidasMateriales.DT_TM101_Consulta_Entrega_ReqItem();

                                ClientMat.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                ClientMat.ClientCredentials.UserName.Password = "Cydsa2019$";

                                if (resultadoDoc.OPTIONS.item[i].Response.ToString() != null)
                                {
                                    datosWsMat.DELIV_NUMB = resultadoDoc.OPTIONS.item[i].Response.ToString();
                                    VarDocumento = decimal.Parse(resultadoDoc.OPTIONS.item[i].Response.ToString());
                                }
                                else
                                {
                                    datosWsMat.DELIV_NUMB = "";
                                }


                                contextWsMat.item = datosWsMat;
                                var resultadoMat = ClientMat.SI_OA_TM101_Consulta_Entrega(contextWsMat);
                                var RegPda = resultadoMat.ET_DELIVERY_ITEM.Count();
                                var ColPda = 4;
                                decimal idMaterial = 0;
                                decimal elemento = 0;

                                for (int f = 0; f < RegPda; f++)
                                {
                                    if (resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString() != null)
                                    {
                                        idMaterial = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());
                                    }
                                    else
                                    {
                                        idMaterial = 0;
                                    }

                                    var ValidaCodigo = db.CPCatMateriales.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdMaterialSAP == idMaterial)).FirstOrDefault();
                                    if (ValidaCodigo != null)
                                    {
                                        var Requerido = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString());
                                    }


                                    if (valoromayor < decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString()))
                                    {
                                        elemento = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());
                                        valoromayor = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString());
                                        elemetomayor = elemento.ToString();
                                    }

                                    SumaDeProd = SumaDeProd + decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString());

                                }
                            }

                            SumaDeProd = (SumaDeProd * 1000);
                            Session["idMaterial"] = elemetomayor;
                            Session["ValMaterial"] = SumaDeProd;

                        }
                        else
                        {
                            VarSalida = int.Parse(Busqueda.CPPesoSalida.ToString());
                            Peso.CPPesoSalida = VarSalida;
                            Peso.CPSalida = false;
                            Peso.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());
                        }


                        if (Busqueda.CpFechaSalida != null)
                        {
                            Peso.CpFechaSalida = Busqueda.CpFechaSalida;
                        }
                        else
                        {
                            Peso.CpFechaSalida = DateTime.Now;
                        }


                        VarPesoNeto = (VarSalida - VarEntrada);

                        if (VarPesoNeto < 0)
                        {
                            VarPesoNeto = 0;
                        }

                        Peso.CPIdUsuarioEnt = Busqueda.CPIdUsuarioEnt;
                        Peso.CPIdMaterial = entity.CPIdMaterial;
                        Peso.CPPesoNeto = VarPesoNeto;

                        if (Busqueda.CPNumDePasos == 4 && Busqueda.CPEstatus >= 1)
                        {
                            Peso.CPNumDePasos = Busqueda.CPNumDePasos;
                            Peso.CPEstatus = Busqueda.CPEstatus;
                        }
                        else
                        {
                            Peso.CPNumDePasos = ProcesoSAP;
                            Peso.CPEstatus = 1;
                        }

                        Peso.CPEstatus = 1;
                        Peso.CPDescripcionMaterial = "";
                        Peso.CPIdDocumento = Busqueda.CPIdDocumento;
                        Peso.CPPartida = Busqueda.CPPartida;
                        Peso.CPIdMaterial = Busqueda.CPIdMaterial;

                        dbSalida.CPBitacora.Attach(Peso);
                        dbSalida.Entry(Peso).State = System.Data.Entity.EntityState.Modified;
                        dbSalida.SaveChanges();
                    }
                }

                else
                {
                    BDObtenerPesoSAPEntities dbEntrada = new BDObtenerPesoSAPEntities();
                    CPBitacora medicion = new CPBitacora();
                    var Planta = int.Parse(Session["idPlantaDF"].ToString());


                    //////////////////////////////////////////////////////////////////////////////////
                    //                                                                                
                    // Aqui vamos a grabar todas la partidas y vamos a  calcular los factores
                    //
                    //////////////////////////////////////////////////////////////////////////////////

                    var existenteDoc = "";
                    var mensajeDoc = "";
                    var existente7 = "";
                    var MsjError = "";

                    List<Tabla> lsitTabla = new List<Tabla>();
                    lsitTabla.Clear();

                    DocEntrega.SI_OS_Consulta_NumCargaClient ClientDoc = new DocEntrega.SI_OS_Consulta_NumCargaClient();
                    DocEntrega.DT_Consulta_NumCarga contextWsDoc = new DocEntrega.DT_Consulta_NumCarga();
                    DocEntrega.DT_Consulta_NumCargaData datosWsDoc = new DocEntrega.DT_Consulta_NumCargaData();

                    ClientDoc.ClientCredentials.UserName.UserName = "POCYDSAINT";
                    ClientDoc.ClientCredentials.UserName.Password = "Cydsa2019$";

                    datosWsDoc.Numero_Transporte = Session["IdTransporte"].ToString();

                    contextWsDoc.data = datosWsDoc;

                    var resultadoDoc = ClientDoc.SI_OS_Consulta_NumCarga(contextWsDoc);
                    var RegDoc = resultadoDoc.OPTIONS.item.Count();

                    decimal SumaDeProd = 0;
                    var elemetomayor = "";
                    decimal valoromayor = 0;
                    decimal VarDocumento = 0;
                    var Pda = 1;

                    for (int i = 0; i < RegDoc; i++)
                    {
                        PartidasMateriales.SI_OA_TM101_Consulta_EntregaClient ClientMat = new PartidasMateriales.SI_OA_TM101_Consulta_EntregaClient();
                        PartidasMateriales.DT_TM101_Consulta_Entrega_Req contextWsMat = new PartidasMateriales.DT_TM101_Consulta_Entrega_Req();
                        PartidasMateriales.DT_TM101_Consulta_Entrega_ReqItem datosWsMat = new PartidasMateriales.DT_TM101_Consulta_Entrega_ReqItem();

                        ClientMat.ClientCredentials.UserName.UserName = "POCYDSAINT";
                        ClientMat.ClientCredentials.UserName.Password = "Cydsa2019$";

                        if (resultadoDoc.OPTIONS.item[i].Response.ToString() != null)
                        {
                            datosWsMat.DELIV_NUMB = resultadoDoc.OPTIONS.item[i].Response.ToString();
                            VarDocumento = decimal.Parse(resultadoDoc.OPTIONS.item[i].Response.ToString());
                        }
                        else
                        {
                            datosWsMat.DELIV_NUMB = "";
                        }


                        contextWsMat.item = datosWsMat;
                        var resultadoMat = ClientMat.SI_OA_TM101_Consulta_Entrega(contextWsMat);

                        var RegPda = resultadoMat.ET_DELIVERY_ITEM.Count();
                        var ColPda = 4;
                        long idMaterial = 0;

                        for (int f = 0; f < RegPda; f++)
                        {


                            if (resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString() != null)
                            {
                                idMaterial = long.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());
                            }
                            else
                            {
                                idMaterial = 0;
                            }

                            var ValidaCodigo = db.CPCatMateriales.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdMaterialSAP == idMaterial)).FirstOrDefault();
                            if (ValidaCodigo != null)
                            {
                                BDObtenerPesoSAPEntities dbAnexo = new BDObtenerPesoSAPEntities();
                                CPPartidas Partidas = new CPPartidas();
                                var Requerido = double.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString());

                                Partidas.CPIdEmpresa = Planta;
                                Partidas.CPIdTransporte = long.Parse(Session["IdTransporte"].ToString());
                                Partidas.CPIdDocumento = long.Parse(datosWsMat.DELIV_NUMB.ToString());


                                // Partidas.CPIdCodigo = resultadoMat.ET_DELIVERY_ITEM[f].POSNR.ToString();
                                Partidas.CPIdCodigo = long.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString()).ToString();
                                Material = long.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());
                                Partidas.CPIdMaterial = Material.ToString();
                                Partidas.CPDescripcion = resultadoMat.ET_DELIVERY_ITEM[f].ARKTX.ToString();

                                var TipoPeso = resultadoMat.ET_DELIVERY_ITEM[f].MEINS.ToString();
                                TipoPeso = TipoPeso.Substring(0, 1);

                                if (TipoPeso == "T")
                                {
                                    Partidas.CPPesoRequerido = (Requerido * 1000);
                                }
                                else
                                {
                                    Partidas.CPPesoRequerido = (Requerido);
                                }

                                if (TipoPeso == "K")
                                {
                                    Partidas.CPIdUnidadMedida = 1;
                                }
                                else
                                {
                                    Partidas.CPIdUnidadMedida = 2;
                                }


                                Partidas.CPPartida = Pda;
                                Partidas.CPFecha = DateTime.Now;
                                Partidas.CPIdUsuario = int.Parse(Session["idUsuario"].ToString());
                                Partidas.CPEstatus = 1;

                                dbAnexo.CPPartidas.Add(Partidas);
                                dbAnexo.SaveChanges();

                                Pda = Pda + 1;

                                if (valoromayor < decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString()))
                                {
                                    valoromayor = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString());
                                    elemetomayor = resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString();

                                }
                                SumaDeProd = SumaDeProd + decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].POSNR.ToString());
                            }
                        }


                        NumEco = entity.CPNumEconomico.ToString();
                        Placas = entity.CPPlaca.ToString();
                        Conductor = entity.CPNomConductor.ToString();
                        Carta = entity.CPNumPorte.ToString();
                        Celular = entity.CPNumCelular.ToString();

                        medicion.CPIdEmpresa = Planta;
                        medicion.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());
                        medicion.CPPesoEntrada = VarEntrada;
                        medicion.CPNumEconomico = NumEco;
                        medicion.CPNumPorte = Carta;
                        medicion.CPPlaca = Placas;
                        medicion.CPNomConductor = Conductor;
                        medicion.CPFechaEntrada = DateTime.Now;
                        medicion.CPIdUsuarioEnt = int.Parse(Session["idUsuario"].ToString());
                        medicion.CPIdMaterial = Material;
                        medicion.CPNumCelular = Celular;
                        medicion.CPPartida = RegPda;

                        VarSalida = 0;
                        medicion.CPPesoSalida = VarSalida;
                        medicion.CpFechaSalida = DateTime.Now;
                        medicion.CPSalida = false;
                        medicion.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());

                        VarPesoNeto = (VarSalida - VarEntrada);

                        if (VarPesoNeto < 0)
                        {
                            VarPesoNeto = 0;
                        }

                        medicion.CPPesoNeto = VarPesoNeto;
                        var VarCaptura = decimal.Parse(Session["TipoCaptura"].ToString());

                        medicion.CPEntrada = false;
                        medicion.CPPesoEntrada = VarEntrada;
                        ProcesoSAP = 2;

                        medicion.CPNumDePasos = ProcesoSAP;
                        medicion.CPEstatus = 1;
                        medicion.CPDescripcionMaterial = "";
                        medicion.CPIdDocumento = VarDocumento;
                        //medicion.CPPartida = 1;
                        medicion.CPIdMaterial = decimal.Parse(elemetomayor.ToString());

                        dbEntrada.CPBitacora.Add(medicion);
                        dbEntrada.SaveChanges();

                        SumaDeProd = (SumaDeProd * 1000);
                        Session["idMaterial"] = elemetomayor;
                        Session["ValMaterial"] = SumaDeProd;
                    }




                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //
                    //
                    // aqui debe de ir if para saber si este material no se pesa, si es asi hay que correr aqui mismo los demas webservice
                    //
                    //
                    //
                    //
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    var VarMaterial = decimal.Parse(Session["idMaterial"].ToString());

                    var MaterialLiquido = context.CPCatMateriales.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdMaterialSAP == VarMaterial)).FirstOrDefault();
                    if (MaterialLiquido != null)
                    {

                        if (MaterialLiquido.CPSePesa == "0")
                        {

                            for (int x = 2; x < 5; x += 1)
                            {
                                EnviaPesos.SI_OA_Peso_VehiculoClient Client7 = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                                EnviaPesos.DT_Peso_Vehiculo contextWs7 = new EnviaPesos.DT_Peso_Vehiculo();
                                EnviaPesos.DT_Peso_VehiculoData datosWs7 = new EnviaPesos.DT_Peso_VehiculoData();

                                Client7.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                Client7.ClientCredentials.UserName.Password = "Cydsa2019$";
                                //ws.Open();

                                datosWs7.Vehicle_Number = Session["IdTransporte"].ToString();
                                datosWs7.Weight_Initial = VarEntrada.ToString();
                                datosWs7.Weight_Final = VarSalida.ToString();
                                datosWs7.Economic_Number = NumEco;
                                datosWs7.Licence_Plate = Placas;
                                datosWs7.Driver_Name = Conductor;
                                datosWs7.Carriage_Number = entity.CPNumPorte;

                                ProcesoSAP = x;

                                switch (ProcesoSAP)
                                {
                                    case 2:
                                        datosWs7.Check_In = "X";
                                        datosWs7.Load_Start = "";
                                        datosWs7.Load_End = "";
                                        break;
                                    case 3:
                                        datosWs7.Check_In = "";
                                        datosWs7.Load_Start = "X";
                                        datosWs7.Load_End = "";
                                        break;
                                    case 4:
                                        datosWs7.Check_In = "";
                                        datosWs7.Load_Start = "";
                                        datosWs7.Load_End = "";
                                        break;
                                }

                                contextWs7.data = datosWs7;
                                var resultado7 = Client7.SI_OA_Peso_Vehiculo(contextWs7);
                                //var existente7 = resultado7[0].TYPE;

                                var Errores = resultado7.Count();
                                //var existente1 = resultado1[0].TYPE;


                                for (int r = 0; r < Errores; r += 1)
                                {
                                    existente7 = resultado7[r].TYPE;

                                    if (existente7 == "E")
                                    {
                                        MsjError = resultado7[r].MESSAGE;
                                        break;
                                    }

                                }


                                if (existente7 != "E" && existente7 != "A")
                                {
                                    var existente8 = "";
                                    var mensaje8 = "";
                                    Transporte = decimal.Parse(Session["IdTransporte"].ToString());


                                    var VarPicking = context.CPBitacora.Where(y => (y.CPIdEmpresa == IdPlanta) && (y.CPIdTransporte == Transporte)).FirstOrDefault();
                                    if (VarPicking != null)
                                    {
                                        BDObtenerPesoSAPEntities NoPicking = new BDObtenerPesoSAPEntities();
                                        CPBitacora SinPicking = new CPBitacora();
                                        SinPicking.CPId = VarPicking.CPId;
                                        SinPicking.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());
                                        SinPicking.CPPesoEntrada = VarPicking.CPPesoEntrada;
                                        SinPicking.CPPesoSalida = VarPicking.CPPesoSalida;
                                        SinPicking.CPNumEconomico = VarPicking.CPNumEconomico;
                                        SinPicking.CPPlaca = VarPicking.CPPlaca;
                                        SinPicking.CPNomConductor = VarPicking.CPNomConductor;
                                        SinPicking.CPNumPorte = VarPicking.CPNumPorte;
                                        SinPicking.CPPesoNeto = VarPicking.CPPesoNeto;
                                        SinPicking.CPSalida = VarPicking.CPSalida;
                                        SinPicking.CPEntrada = VarPicking.CPEntrada;
                                        SinPicking.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                                        SinPicking.CPFechaEntrada = DateTime.Now;
                                        SinPicking.CPIdUsuarioEnt = int.Parse(Session["idUsuario"].ToString());
                                        SinPicking.CPIdMaterial = VarPicking.CPIdMaterial;
                                        SinPicking.CPNumCelular = VarPicking.CPNumCelular;

                                        SinPicking.CPPesoSalida = VarPicking.CPPesoSalida;
                                        SinPicking.CpFechaSalida = VarPicking.CpFechaSalida;
                                        SinPicking.CPSalida = VarPicking.CPSalida;
                                        SinPicking.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());

                                        SinPicking.CPNumDePasos = ProcesoSAP;
                                        SinPicking.CPEstatus = AutorizacionSAP;

                                        NoPicking.CPBitacora.Attach(SinPicking);
                                        NoPicking.Entry(SinPicking).State = System.Data.Entity.EntityState.Modified;
                                        NoPicking.SaveChanges();
                                    }

                                    if (ProcesoSAP == 4)
                                    {
                                        var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                                        if (CierrePasos != null)
                                        {
                                            BDObtenerPesoSAPEntities CmbPasos = new BDObtenerPesoSAPEntities();
                                            CPBitacora Pasos = new CPBitacora();
                                            Pasos.CPId = CierrePasos.CPId;
                                            Pasos.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());
                                            Pasos.CPPesoEntrada = CierrePasos.CPPesoEntrada;
                                            Pasos.CPPesoSalida = CierrePasos.CPPesoSalida;
                                            Pasos.CPNumEconomico = CierrePasos.CPNumEconomico;
                                            Pasos.CPPlaca = CierrePasos.CPPlaca;
                                            Pasos.CPNomConductor = CierrePasos.CPNomConductor;
                                            Pasos.CPNumPorte = CierrePasos.CPNumPorte;
                                            Pasos.CPPesoNeto = CierrePasos.CPPesoNeto;
                                            Pasos.CPSalida = CierrePasos.CPSalida;
                                            Pasos.CPEntrada = CierrePasos.CPEntrada;
                                            Pasos.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                                            Pasos.CPFechaEntrada = DateTime.Now;
                                            Pasos.CPIdUsuarioEnt = int.Parse(Session["idUsuario"].ToString());
                                            Pasos.CPIdMaterial = CierrePasos.CPIdMaterial;
                                            Pasos.CPNumCelular = CierrePasos.CPNumCelular;

                                            Pasos.CPPesoSalida = CierrePasos.CPPesoSalida;
                                            Pasos.CpFechaSalida = CierrePasos.CpFechaSalida;
                                            Pasos.CPSalida = CierrePasos.CPSalida;
                                            Pasos.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());

                                            Pasos.CPNumDePasos = 4;
                                            Pasos.CPEstatus = 3;

                                            CmbPasos.CPBitacora.Attach(Pasos);
                                            CmbPasos.Entry(Pasos).State = System.Data.Entity.EntityState.Modified;
                                            CmbPasos.SaveChanges();

                                            ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                            ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                            ViewBag.IsOK = 1;
                                            ViewBag.Message = MsjError;
                                            return View();
                                            //return Redirect("~/Bascula/Bascula");
                                        }

                                        Session["SinPicking"] = 0;
                                        CierreProceso.SI_OS_UpdatePickingClient Client8 = new CierreProceso.SI_OS_UpdatePickingClient();
                                        CierreProceso.DT_UpdatePicking contextWs8 = new CierreProceso.DT_UpdatePicking();
                                        CierreProceso.DT_UpdatePickingData datosWs8 = new CierreProceso.DT_UpdatePickingData();

                                        Client8.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                        Client8.ClientCredentials.UserName.Password = "Cydsa2019$";

                                        datosWs8.Vehicle_Number = Session["IdTransporte"].ToString();

                                        contextWs8.data = datosWs8;
                                        var resultado8 = Client8.SI_OS_UpdatePicking(contextWs8);
                                        existente8 = resultado8[0].Response;
                                        MsjError = resultado8[0].Message;

                                        if (existente8 != "E")
                                        {
                                            EnviaPesos.SI_OA_Peso_VehiculoClient Client5 = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                                            EnviaPesos.DT_Peso_Vehiculo contextWs5 = new EnviaPesos.DT_Peso_Vehiculo();
                                            EnviaPesos.DT_Peso_VehiculoData datosWs5 = new EnviaPesos.DT_Peso_VehiculoData();

                                            Client5.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                            Client5.ClientCredentials.UserName.Password = "Cydsa2019$";
                                            var existente5 = "";
                                            var Errores5 = 0;

                                            for (int c = 0; c < 1; c += 1)
                                            {
                                                datosWs5.Vehicle_Number = Session["IdTransporte"].ToString();
                                                datosWs5.Weight_Initial = VarEntrada.ToString();
                                                datosWs5.Weight_Final = VarSalida.ToString();
                                                datosWs5.Economic_Number = NumEco;
                                                datosWs5.Licence_Plate = Placas;
                                                datosWs5.Driver_Name = Conductor;
                                                datosWs5.Carriage_Number = entity.CPNumPorte;

                                                datosWs5.Check_In = "";
                                                datosWs5.Load_Start = "";

                                                if (c == 0)
                                                {
                                                    datosWs5.Load_End = "X";
                                                }

                                                //if (c == 1)
                                                //{
                                                //    datosWs5.Shipment_Complete = "X";
                                                //}

                                                //if (c == 2)
                                                //{
                                                //    datosWs5.Shipment_Start = "X";
                                                //}

                                                //if (c == 3)
                                                //{
                                                //    datosWs5.Shipment_End = "X";
                                                //}


                                                contextWs5.data = datosWs5;
                                                var resultado5 = Client5.SI_OA_Peso_Vehiculo(contextWs5);

                                                if (resultado5.Count() > 0)
                                                {
                                                    Errores5 = resultado5.Count();
                                                    for (int r = 0; r < Errores5; r += 1)
                                                    {
                                                        existente5 = resultado5[r].TYPE;

                                                        if (existente5 == "E")
                                                        {
                                                            MsjError = resultado5[r].MESSAGE;
                                                            break;
                                                        }

                                                    }

                                                    break;
                                                }
                                            }





                                            if (existente5 != "E" && existente5 != "A")
                                            {
                                                ViewBag.IsOK = 1;
                                                ViewBag.Message = "Transaccion Completada con exito";
                                                return View();
                                                //return Redirect("~/Bascula/Bascula");

                                            }
                                            else
                                            {

                                                //string yourId = "QVtSqdk3okeC1yrqpmGecWRhdmlkX2RvdF9jcnV6X2F0X2dydXBvdml0ZWtfZG90X2NvbV9kb3RfbXg=";
                                                //string yourMobile = "+528111269428";
                                                //string yourMessage = "pruebas de comunicacion para IQUISA";

                                                //try
                                                //{
                                                //    string url = "https://NiceApi.net/API";
                                                //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                                                //    request.Method = "POST";
                                                //    request.ContentType = "application/x-www-form-urlencoded";
                                                //    request.Headers.Add("X-APIId", yourId);
                                                //    request.Headers.Add("X-APIMobile", yourMobile);
                                                //    using (StreamWriter streamOut = new StreamWriter(request.GetRequestStream()))
                                                //    {
                                                //        streamOut.Write(yourMessage);
                                                //    }
                                                //    using (StreamReader streamIn = new StreamReader(request.GetResponse().GetResponseStream()))
                                                //    {
                                                //        Console.WriteLine(streamIn.ReadToEnd());
                                                //    }
                                                //}
                                                //catch (SystemException se)
                                                //{
                                                //    Console.WriteLine(se.Message);
                                                //}
                                                //Console.ReadLine();

                                                BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                                CPBitacora Regresar = new CPBitacora();
                                                Regresar.CPId = CierrePasos.CPId;
                                                Regresar.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());

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
                                                        CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                                        CmbRegresa.CPBitacora.Remove(cPBitacora);
                                                        CmbRegresa.SaveChanges();

                                                        Transporte = decimal.Parse(Session["IdTransporte"].ToString());

                                                        var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                                ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                                ViewBag.IsOK = 0;
                                                ViewBag.Message = MsjError;
                                                return View();


                                            }
                                        }
                                        else
                                        {
                                            ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                            ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                            if (mensaje8 == "Material No Relvante para Picking")
                                            {
                                                Session["SinPicking"] = 1;
                                                ViewBag.IsOK = 1;
                                                ViewBag.Message = mensaje8;
                                                return View();
                                            }
                                            else
                                            {
                                                BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                                CPBitacora Regresar = new CPBitacora();
                                                Regresar.CPId = CierrePasos.CPId;
                                                Regresar.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());

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
                                                        CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                                        CmbRegresa.CPBitacora.Remove(cPBitacora);
                                                        CmbRegresa.SaveChanges();

                                                        Transporte = decimal.Parse(Session["IdTransporte"].ToString());

                                                        var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                                ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                                ViewBag.IsOK = 0;
                                                ViewBag.Message = MsjError;
                                                return View();
                                            }

                                            //ViewBag.IsOK = 0;
                                            //ViewBag.Message = MsjError;
                                            //return View();
                                        }
                                    }

                                }
                                else
                                {

                                    var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                                    if (CierrePasos != null)
                                    {
                                        BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                        CPBitacora Regresar = new CPBitacora();
                                        Regresar.CPId = CierrePasos.CPId;
                                        Regresar.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());

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

                                                Transporte = long.Parse(Session["IdTransporte"].ToString());

                                                var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                        ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                        ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                        ViewBag.IsOK = 0;
                                        ViewBag.Message = MsjError;
                                        return View();
                                    }

                                    //ViewBag.IsOK = 1;
                                    //ViewBag.Message = "Transaccion Completada con exito";
                                    //ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                    //ViewBag.TempData();
                                    //return View();
                                }
                            }
                        }

                    }


                }




                // ESTA ES LA PARTE DE LOS FACTORES SE TIENE QUE INCLUIR
                var RequiereAutorizar = context.CPBitacora.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdTransporte == IdTransporte) & (x.CPNumDePasos == 4) & (x.CPEstatus == 1)).FirstOrDefault();
                if (RequiereAutorizar != null)
                {
                    var idMaterial = decimal.Parse(Session["elemetomayor"].ToString());
                    var CantSolicitada = decimal.Parse(Session["ValMaterial"].ToString());

                    //var CantSolicitada = 0;
                    //if (Session["ValMaterial"] != null)
                    //{

                    //     CantSolicitada = int.Parse(Session["ValMaterial"].ToString());
                    //}
                    //else
                    //{
                    //    var variable = Session["ValMaterial"];
                    //    CantSolicitada = int.Parse(Session["ValMaterial"].ToString());
                    //}




                    var Autoriza = context.CPCatMateriales.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdMaterialSAP == idMaterial)).FirstOrDefault();
                    if (Autoriza != null)
                    {
                        if (Autoriza.CPRequiereAutoriza == "1")
                        {
                            if (CantSolicitada > 0)
                            {
                                MontoPesoNeto = (int.Parse(entity.CPPesoSalida.ToString()) - int.Parse(entity.CPPesoEntrada.ToString()));

                            }
                            else
                            {
                                MontoPesoNeto = CantSolicitada;
                            }

                            decimal FactorMin = 0;
                            decimal FactorMax = 0;
                            FactorMin = (CantSolicitada / Autoriza.CPFactorMin);
                            FactorMax = (CantSolicitada * Autoriza.CPFactorMax);

                            if (MontoPesoNeto >= FactorMin && MontoPesoNeto <= FactorMax)
                            {
                                ViewBag.Message = "";
                                Session["IdUserAutoriza"] = 1;

                            }


                            // SI EL MATERIAL NO CUMPLE CON LOS FACTORE MANDO UN ERROR
                            if (Session["IdUserAutoriza"].ToString() != "1")
                            {
                                Session["IdUserAutoriza"] = 0;
                                Session["SinPicking"] = 0;

                                ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                ViewBag.IsOK = 0;
                                ViewBag.Message = "Error en los limites de peso permitido, Requiere autorizacion de un supervisor";
                                return View();
                            }
                        }
                    }
                }
                else
                {
                    // EN CASO QUE EL MATERIAL NO ESTE DADO DE ALTA EN EL CATALOGO DEL PORTAL DEBEMOS DE DEJAR PASAR AQUI EL PROCESO
                    Session["IdUserAutoriza"] = 1;
                }




                var AfectaSAP = context.CPBitacora.Where(y => (y.CPIdEmpresa == IdPlanta) && (y.CPIdTransporte == IdTransporte) && (y.CPEstatus != 2)).FirstOrDefault();
                if (AfectaSAP != null)
                {
                    var Contar = 0;
                    var PesoEntrada = 0;
                    var PesoSalida = 0;
                    var existente1 = "";
                    var MsjError = "";

                    var VarCaptura = int.Parse(Session["TipoCaptura"].ToString());
                    Contar = 1;


                    for (int x = 0; x < Contar; x += 1)
                    {
                        EnviaPesos.SI_OA_Peso_VehiculoClient Client1 = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                        EnviaPesos.DT_Peso_Vehiculo contextWs1 = new EnviaPesos.DT_Peso_Vehiculo();
                        EnviaPesos.DT_Peso_VehiculoData datosWs1 = new EnviaPesos.DT_Peso_VehiculoData();

                        Client1.ClientCredentials.UserName.UserName = "POCYDSAINT";
                        Client1.ClientCredentials.UserName.Password = "Cydsa2019$";

                        if (ProcesoSAP > 2)
                        {
                            PesoEntrada = int.Parse(entity.CPPesoEntrada.ToString());

                            if (entity.CPPesoSalida != null)
                            {
                                PesoSalida = int.Parse(entity.CPPesoSalida.ToString());
                            }
                            else
                            {
                                PesoSalida = 0;
                            }
                        }
                        else
                        {
                            PesoEntrada = int.Parse(AfectaSAP.CPPesoEntrada.ToString());
                            PesoSalida = int.Parse(AfectaSAP.CPPesoSalida.ToString());
                        }


                        datosWs1.Vehicle_Number = Session["IdTransporte"].ToString();
                        datosWs1.Weight_Initial = PesoEntrada.ToString();
                        datosWs1.Weight_Final = PesoSalida.ToString();
                        datosWs1.Economic_Number = NumEco;
                        datosWs1.Licence_Plate = Placas;
                        datosWs1.Driver_Name = Conductor;
                        datosWs1.Carriage_Number = entity.CPNumPorte;

                        switch (ProcesoSAP)
                        {
                            case 2:
                                datosWs1.Check_In = "X";
                                datosWs1.Load_Start = "";
                                datosWs1.Load_End = "";
                                break;
                            case 3:
                                datosWs1.Check_In = "";
                                datosWs1.Load_Start = "X";
                                datosWs1.Load_End = "";
                                break;
                            case 4:
                                datosWs1.Check_In = "";
                                datosWs1.Load_Start = "";
                                datosWs1.Load_End = "";
                                break;
                        }

                        if (ProcesoSAP != 4)
                        {
                            contextWs1.data = datosWs1;
                            var resultado1 = Client1.SI_OA_Peso_Vehiculo(contextWs1);
                            var Errores = resultado1.Count();


                            for (int r = 0; r < Errores; r += 1)
                            {
                                existente1 = resultado1[r].TYPE;

                                if (existente1 == "E")
                                {
                                    MsjError = resultado1[r].MESSAGE;
                                    break;
                                }

                            }


                        }
                        else
                        {

                            if (Session["IdUserAutoriza"] != "0")
                            {
                                contextWs1.data = datosWs1;
                                var resultado1 = Client1.SI_OA_Peso_Vehiculo(contextWs1);
                                var Errores = resultado1.Count();
                                var existente2 = "";
                                var mensaje2 = "";

                                for (int r = 0; r < Errores; r += 1)
                                {
                                    existente1 = resultado1[r].TYPE;

                                    if (existente1 == "E")
                                    {
                                        MsjError = resultado1[r].MESSAGE;
                                        break;
                                    }

                                }

                                if (existente1 != "E" && existente1 != "A")
                                {

                                    CierreProceso.SI_OS_UpdatePickingClient Client2 = new CierreProceso.SI_OS_UpdatePickingClient();
                                    CierreProceso.DT_UpdatePicking contextWs2 = new CierreProceso.DT_UpdatePicking();
                                    CierreProceso.DT_UpdatePickingData datosWs2 = new CierreProceso.DT_UpdatePickingData();

                                    Client2.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                    Client2.ClientCredentials.UserName.Password = "Cydsa2019$";

                                    datosWs2.Vehicle_Number = Session["IdTransporte"].ToString();

                                    contextWs2.data = datosWs2;
                                    var resultado2 = Client2.SI_OS_UpdatePicking(contextWs2);
                                    existente2 = resultado2[0].Response;
                                    MsjError = resultado2[0].Message;

                                    if (existente2 != "E")
                                    {
                                        var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                                        if (CierrePasos != null)
                                        {
                                            EnviaPesos.SI_OA_Peso_VehiculoClient Client5 = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                                            EnviaPesos.DT_Peso_Vehiculo contextWs5 = new EnviaPesos.DT_Peso_Vehiculo();
                                            EnviaPesos.DT_Peso_VehiculoData datosWs5 = new EnviaPesos.DT_Peso_VehiculoData();

                                            Client5.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                            Client5.ClientCredentials.UserName.Password = "Cydsa2019$";
                                            var existente5 = "";
                                            var Errores5 = 0;

                                            for (int c = 0; c < 1; c += 1)
                                            {
                                                datosWs5.Vehicle_Number = Session["IdTransporte"].ToString();
                                                datosWs5.Weight_Initial = VarEntrada.ToString();
                                                datosWs5.Weight_Final = VarSalida.ToString();
                                                datosWs5.Economic_Number = NumEco;
                                                datosWs5.Licence_Plate = Placas;
                                                datosWs5.Driver_Name = Conductor;
                                                datosWs5.Carriage_Number = entity.CPNumPorte;

                                                datosWs5.Check_In = "";
                                                datosWs5.Load_Start = "";

                                                if (c == 0)
                                                {
                                                    datosWs5.Load_End = "X";
                                                }

                                                //if (c == 1)
                                                //{
                                                //    datosWs5.Shipment_Complete = "X";
                                                //}

                                                //if (c == 2)
                                                //{
                                                //    datosWs5.Shipment_Start = "X";
                                                //}

                                                //if (c == 3)
                                                //{
                                                //    datosWs5.Shipment_End = "X";
                                                //}


                                                contextWs5.data = datosWs5;
                                                var resultado5 = Client5.SI_OA_Peso_Vehiculo(contextWs5);

                                                if (resultado5.Count() > 0)
                                                {
                                                    Errores5 = resultado5.Count();
                                                    for (int r = 0; r < Errores5; r += 1)
                                                    {
                                                        existente5 = resultado5[r].TYPE;

                                                        if (existente5 == "E")
                                                        {
                                                            MsjError = resultado5[r].MESSAGE;
                                                            break;
                                                        }

                                                    }

                                                    //break;
                                                }
                                            }

                                            if (existente5 != "E" && existente5 != "A")
                                            {
                                                BDObtenerPesoSAPEntities CmbPasos = new BDObtenerPesoSAPEntities();
                                                CPBitacora Pasos = new CPBitacora();
                                                Pasos.CPId = CierrePasos.CPId;
                                                Pasos.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());
                                                Pasos.CPPesoEntrada = CierrePasos.CPPesoEntrada;
                                                Pasos.CPPesoSalida = CierrePasos.CPPesoSalida;
                                                Pasos.CPNumEconomico = CierrePasos.CPNumEconomico;
                                                Pasos.CPPlaca = CierrePasos.CPPlaca;
                                                Pasos.CPNomConductor = CierrePasos.CPNomConductor;
                                                Pasos.CPNumPorte = CierrePasos.CPNumPorte;

                                                Pasos.CPSalida = CierrePasos.CPSalida;
                                                Pasos.CPEntrada = CierrePasos.CPEntrada;
                                                Pasos.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                                                Pasos.CPFechaEntrada = DateTime.Now;
                                                Pasos.CPIdUsuarioEnt = int.Parse(Session["idUsuario"].ToString());
                                                Pasos.CPIdMaterial = CierrePasos.CPIdMaterial;
                                                Pasos.CPNumCelular = CierrePasos.CPNumCelular;

                                                Pasos.CPPesoSalida = VarSalida;
                                                Pasos.CpFechaSalida = DateTime.Now;
                                                Pasos.CPSalida = true;
                                                Pasos.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());
                                                Pasos.CPPesoNeto = VarPesoNeto;

                                                Pasos.CPNumDePasos = 4;
                                                Pasos.CPEstatus = 2;

                                                CmbPasos.CPBitacora.Attach(Pasos);
                                                CmbPasos.Entry(Pasos).State = System.Data.Entity.EntityState.Modified;
                                                CmbPasos.SaveChanges();

                                                ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                                ViewBag.IsOK = 1;
                                                ViewBag.Message = "Transaccion Completada con exito";
                                                return View();
                                                //return Redirect("~/Bascula/Bascula");

                                            }
                                            else
                                            {

                                                //var CierreFinal = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                                                //if (CierreFinal != null)
                                                //{
                                                BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                                CPBitacora Regresar = new CPBitacora();
                                                Regresar.CPId = CierrePasos.CPId;
                                                Regresar.CPIdTransporte = long.Parse(Session["IdTransporte"].ToString());

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
                                                        CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                                        CmbRegresa.CPBitacora.Remove(cPBitacora);
                                                        CmbRegresa.SaveChanges();

                                                        Transporte = decimal.Parse(Session["IdTransporte"].ToString());

                                                        var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                                ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");


                                                /// REVISA MAÑANA ESTA FUNCION BIEN
                                                ViewBag.IsOK = 0;
                                                ViewBag.Message = MsjError;
                                                return View();


                                            }

                                        }

                                    }
                                    else
                                    {
                                        var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();

                                        if (MsjError != "Material No Relvante para Picking")
                                        {
                                            if (CierrePasos != null)
                                            {

                                                BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                                CPBitacora Regresar = new CPBitacora();
                                                Regresar.CPId = CierrePasos.CPId;
                                                Regresar.CPIdTransporte = long.Parse(Session["IdTransporte"].ToString());

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

                                                        CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                                        CmbRegresa.CPBitacora.Remove(cPBitacora);
                                                        CmbRegresa.SaveChanges();

                                                        Transporte = long.Parse(Session["IdTransporte"].ToString());

                                                        var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                                ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                                ViewBag.IsOK = 0;
                                                ViewBag.Message = MsjError;
                                                return View();


                                                //ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                                                //ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");
                                                //ViewBag.Message = mensaje2;
                                                //return View();
                                            }
                                        }
                                        else
                                        {
                                            BDObtenerPesoSAPEntities CmbPasos = new BDObtenerPesoSAPEntities();
                                            CPBitacora Pasos = new CPBitacora();
                                            Pasos.CPId = CierrePasos.CPId;
                                            Pasos.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());
                                            Pasos.CPPesoEntrada = CierrePasos.CPPesoEntrada;
                                            Pasos.CPPesoSalida = CierrePasos.CPPesoSalida;
                                            Pasos.CPNumEconomico = CierrePasos.CPNumEconomico;
                                            Pasos.CPPlaca = CierrePasos.CPPlaca;
                                            Pasos.CPNomConductor = CierrePasos.CPNomConductor;
                                            Pasos.CPNumPorte = CierrePasos.CPNumPorte;

                                            Pasos.CPSalida = CierrePasos.CPSalida;
                                            Pasos.CPEntrada = CierrePasos.CPEntrada;
                                            Pasos.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                                            Pasos.CPFechaEntrada = DateTime.Now;
                                            Pasos.CPIdUsuarioEnt = int.Parse(Session["idUsuario"].ToString());
                                            Pasos.CPIdMaterial = CierrePasos.CPIdMaterial;
                                            Pasos.CPNumCelular = CierrePasos.CPNumCelular;

                                            Pasos.CPPesoSalida = VarSalida;
                                            Pasos.CpFechaSalida = DateTime.Now;
                                            Pasos.CPSalida = true;
                                            Pasos.CPIdUsuarioSal = int.Parse(Session["idUsuario"].ToString());
                                            Pasos.CPPesoNeto = VarPesoNeto;

                                            Pasos.CPNumDePasos = 4;
                                            Pasos.CPEstatus = 2;

                                            CmbPasos.CPBitacora.Attach(Pasos);
                                            CmbPasos.Entry(Pasos).State = System.Data.Entity.EntityState.Modified;
                                            CmbPasos.SaveChanges();



                                            EnviaPesos.SI_OA_Peso_VehiculoClient Client5 = new EnviaPesos.SI_OA_Peso_VehiculoClient();
                                            EnviaPesos.DT_Peso_Vehiculo contextWs5 = new EnviaPesos.DT_Peso_Vehiculo();
                                            EnviaPesos.DT_Peso_VehiculoData datosWs5 = new EnviaPesos.DT_Peso_VehiculoData();

                                            Client5.ClientCredentials.UserName.UserName = "POCYDSAINT";
                                            Client5.ClientCredentials.UserName.Password = "Cydsa2019$";
                                            var existente5 = "";
                                            var Errores5 = 0;

                                            for (int c = 0; c < 1; c += 1)
                                            {
                                                datosWs5.Vehicle_Number = Session["IdTransporte"].ToString();
                                                datosWs5.Weight_Initial = VarEntrada.ToString();
                                                datosWs5.Weight_Final = VarSalida.ToString();
                                                datosWs5.Economic_Number = NumEco;
                                                datosWs5.Licence_Plate = Placas;
                                                datosWs5.Driver_Name = Conductor;
                                                datosWs5.Carriage_Number = entity.CPNumPorte;

                                                //datosWs5.Check_In = "";
                                                //datosWs5.Load_Start = "";

                                                if (c == 0)
                                                {
                                                    datosWs5.Load_End = "X";
                                                }

                                                //if (c == 1)
                                                //{
                                                //    datosWs5.Shipment_Complete = "X";
                                                //}

                                                //if (c == 2)
                                                //{
                                                //    datosWs5.Shipment_Start = "X";
                                                //}

                                                //if (c == 3)
                                                //{
                                                //    datosWs5.Shipment_End = "X";
                                                //}


                                                contextWs5.data = datosWs5;
                                                var resultado5 = Client5.SI_OA_Peso_Vehiculo(contextWs5);

                                                if (resultado5.Count() > 0)
                                                {
                                                    Errores5 = resultado5.Count();
                                                    for (int r = 0; r < Errores5; r += 1)
                                                    {
                                                        existente5 = resultado5[r].TYPE;

                                                        if (existente5 == "E")
                                                        {
                                                            MsjError = resultado5[r].MESSAGE;
                                                            break;
                                                        }

                                                    }

                                                    //break;
                                                }
                                            }



                                            if (existente5 != "E" && existente5 != "A")
                                            {
                                                ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                                ViewBag.IsOK = 1;
                                                ViewBag.Message = "Transaccion Completada con exito";
                                                return View();
                                                //return Redirect("~/Bascula/Bascula");
                                            }
                                            else
                                            {
                                                BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                                CPBitacora Regresar = new CPBitacora();
                                                Regresar.CPId = CierrePasos.CPId;
                                                Regresar.CPIdTransporte = long.Parse(Session["IdTransporte"].ToString());

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
                                                        CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                                        CmbRegresa.CPBitacora.Remove(cPBitacora);
                                                        CmbRegresa.SaveChanges();

                                                        Transporte = decimal.Parse(Session["IdTransporte"].ToString());

                                                        var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                                ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                                                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                                ViewBag.IsOK = 0;
                                                ViewBag.Message = MsjError;
                                                return View();
                                            }


                                        }

                                    }
                                }
                                else
                                {
                                    var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                                    if (CierrePasos != null)
                                    {
                                        BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                                        CPBitacora Regresar = new CPBitacora();
                                        Regresar.CPId = CierrePasos.CPId;
                                        Regresar.CPIdTransporte = long.Parse(Session["IdTransporte"].ToString());

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
                                                CPBitacora cPBitacora = CmbRegresa.CPBitacora.Find(CierrePasos.CPId);
                                                CmbRegresa.CPBitacora.Remove(cPBitacora);
                                                CmbRegresa.SaveChanges();

                                                Transporte = decimal.Parse(Session["IdTransporte"].ToString());

                                                var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                                        ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                                        ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                                        ViewBag.IsOK = 0;
                                        ViewBag.Message = MsjError;
                                        return View();

                                    }

                                    //ViewBag.IsOK = 0;
                                    //ViewBag.Message = MsjError;
                                    //return View();
                                }
                            }
                        }
                    }



                    if (existente1 == "E")
                    {

                        var CierrePasos = context.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                        if (CierrePasos != null)
                        {
                            BDObtenerPesoSAPEntities CmbRegresa = new BDObtenerPesoSAPEntities();
                            CPBitacora Regresar = new CPBitacora();
                            Regresar.CPId = CierrePasos.CPId;
                            Regresar.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());

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

                                    Transporte = decimal.Parse(Session["IdTransporte"].ToString());

                                    var BuscaPartidas = context.CPPartidas.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPId == Transporte)).FirstOrDefault();
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

                            ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                            ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                            ViewBag.IsOK = 0;
                            ViewBag.Message = MsjError;
                            return View();
                        }

                    }

                }


                ViewBag.VarConsecutivo = decimal.Parse(Session["IdTransporte"].ToString());
                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                MaterialesADO cargaMaterialADO = new MaterialesADO();
                ViewBag.dropdownMateriales = new SelectList(cargaMaterialADO.cmbMateriales(int.Parse(Session["idPlantaDF"].ToString())), "CPIdMaterial", "CPDescripcionMaterial");

                ViewBag.IsOK = 1;
                ViewBag.Message = "Transaccion Completada con exito";
                return View();
                //return Redirect("~/Bascula/Bascula");



            }
            catch (Exception e)
            {
                ViewBag.VarConsecutivo = Session["IdTransporte"].ToString();
                ViewBag.dropdownUsuario = new SelectList(context.CPUsuario.ToList(), "CPIdUsuario", "CPNombreUsuario");

                MaterialesADO cargaMaterialADO = new MaterialesADO();
                ViewBag.dropdownMateriales = new SelectList(cargaMaterialADO.cmbMateriales(int.Parse(Session["idPlantaDF"].ToString())), "CPIdMaterial", "CPDescripcionMaterial");

                ViewBag.IsOK = 0;
                ViewBag.Message = "Se produjo un error al momento guardar la informacion";
                return View();
            }



        }


        public JsonResult Autorizacion(int IdAutoriza = 0, string IdClave = "", string MotivoAuto = "")
        {
            CPUsuario Usuarios = new CPUsuario();

            var Usuario = IdAutoriza;
            var Clave = IdClave;
            var MotivoAutorizacion = MotivoAuto;
            var IdAutorizacion = 0;

            Session["IdUserAutoriza"] = "";

            try
            {
                var Busca = db.CPUsuario.Where(x => (x.CPIdUsuario == Usuario && x.CPPassword == Clave && x.CPRol_id == 1)).FirstOrDefault();

                if (Busca != null)
                {
                    IdAutorizacion = Busca.CPIdUsuario;
                    Session["IdUserAutoriza"] = Busca.CPIdUsuario;
                    var IdPlanta = int.Parse(Session["idPlantaDF"].ToString());
                    var IdTransporte = decimal.Parse(Session["IdTransporte"].ToString());



                    var CmbStatus = db.CPBitacora.Where(w => (w.CPIdEmpresa == IdPlanta) && (w.CPIdTransporte == IdTransporte)).FirstOrDefault();
                    if (CmbStatus != null)
                    {
                        CPBitacora Estatus = new CPBitacora();
                        Estatus.CPId = CmbStatus.CPId;
                        Estatus.CPIdEmpresa = IdPlanta;
                        Estatus.CPIdTransporte = IdTransporte;
                        Estatus.CPNumEconomico = CmbStatus.CPNumEconomico;
                        Estatus.CPPlaca = CmbStatus.CPPlaca;
                        Estatus.CPNumPorte = CmbStatus.CPNumPorte;
                        Estatus.CPNomConductor = CmbStatus.CPNomConductor;
                        Estatus.CPNumCelular = CmbStatus.CPNumCelular;
                        Estatus.CPPesoEntrada = CmbStatus.CPPesoEntrada;
                        Estatus.CPPesoSalida = CmbStatus.CPPesoSalida;
                        Estatus.CPPesoNeto = CmbStatus.CPPesoNeto;
                        Estatus.CPIdDocumento = CmbStatus.CPIdDocumento;
                        Estatus.CPIdMaterial = CmbStatus.CPIdMaterial;
                        Estatus.CPDescripcionMaterial = CmbStatus.CPDescripcionMaterial;
                        Estatus.CPPartida = CmbStatus.CPPartida;
                        Estatus.CPFechaEntrada = CmbStatus.CPFechaEntrada;
                        Estatus.CpFechaSalida = CmbStatus.CpFechaSalida;
                        Estatus.CPEntrada = CmbStatus.CPEntrada;
                        Estatus.CPSalida = CmbStatus.CPSalida;
                        Estatus.CPIdUsuarioEnt = CmbStatus.CPIdUsuarioEnt;
                        Estatus.CPIdUsuarioSal = CmbStatus.CPIdUsuarioSal;
                        Estatus.CPNumDePasos = CmbStatus.CPNumDePasos;
                        Estatus.CPEstatus = 2;

                        BDObtenerPesoSAPEntities dbAutorizacion = new BDObtenerPesoSAPEntities();
                        dbAutorizacion.CPBitacora.Attach(Estatus);
                        dbAutorizacion.Entry(Estatus).State = System.Data.Entity.EntityState.Modified;
                        dbAutorizacion.SaveChanges();

                        CPAutorizaciones motivos = new CPAutorizaciones();
                        motivos.CPIdEmpresa = int.Parse(Session["idPlantaDF"].ToString());
                        motivos.CPIdTransporte = decimal.Parse(Session["IdTransporte"].ToString());
                        motivos.CPMotivoAutorizacion = MotivoAutorizacion;
                        motivos.CpFechaAutorizacion = DateTime.Now;
                        motivos.CPIdUsuario = int.Parse(Session["idUsuario"].ToString());
                        motivos.CPIdDocumento = decimal.Parse(CmbStatus.CPIdDocumento.ToString());
                        motivos.CPIdPartidas = int.Parse(CmbStatus.CPPartida.ToString());

                        dbAutorizacion.CPAutorizaciones.Add(motivos);
                        dbAutorizacion.SaveChanges();
                    }

                    return Json(db.CPUsuario.ToList().Where(x => (x.CPIdUsuario == IdAutorizacion)).Select(x => new { CPIdUsuario = x.CPIdUsuario }), JsonRequestBehavior.AllowGet);

                }
                else
                {
                    IdAutorizacion = -1;
                }

                return Json(IdAutorizacion);
            }
            catch (Exception e)
            {

                return Json(e);
            }
        }


        public JsonResult WebSer(int IdPlanta = 0, long Id = 0, string IdTipo = "")
        {
            CPBitacora Bitacora = new CPBitacora();
            var Planta = int.Parse(Session["idPlantaDF"].ToString());

            Session["IdTransporte"] = Id;
            Session["TipoVehiculo"] = IdTipo;

            var Consecutivo = long.Parse(Id.ToString());


            var existenteDoc = "";
            var mensajeDoc = "";


            List<Tabla> lsitTabla = new List<Tabla>();
            lsitTabla.Clear();

            DocEntrega.SI_OS_Consulta_NumCargaClient ClientDoc = new DocEntrega.SI_OS_Consulta_NumCargaClient();
            DocEntrega.DT_Consulta_NumCarga contextWsDoc = new DocEntrega.DT_Consulta_NumCarga();
            DocEntrega.DT_Consulta_NumCargaData datosWsDoc = new DocEntrega.DT_Consulta_NumCargaData();

            ClientDoc.ClientCredentials.UserName.UserName = "POCYDSAINT";
            ClientDoc.ClientCredentials.UserName.Password = "Cydsa2019$";

            datosWsDoc.Numero_Transporte = Session["IdTransporte"].ToString();

            contextWsDoc.data = datosWsDoc;

            var resultadoDoc = ClientDoc.SI_OS_Consulta_NumCarga(contextWsDoc);

            var RegDoc = resultadoDoc.OPTIONS.item.Count();

            decimal SumaDeProd = 0;
            decimal elemetomayor = 0;
            decimal valoromayor = 0;
            for (int i = 0; i < RegDoc; i++)
            {
                PartidasMateriales.SI_OA_TM101_Consulta_EntregaClient ClientMat = new PartidasMateriales.SI_OA_TM101_Consulta_EntregaClient();
                PartidasMateriales.DT_TM101_Consulta_Entrega_Req contextWsMat = new PartidasMateriales.DT_TM101_Consulta_Entrega_Req();
                PartidasMateriales.DT_TM101_Consulta_Entrega_ReqItem datosWsMat = new PartidasMateriales.DT_TM101_Consulta_Entrega_ReqItem();

                ClientMat.ClientCredentials.UserName.UserName = "POCYDSAINT";
                ClientMat.ClientCredentials.UserName.Password = "Cydsa2019$";

                if (resultadoDoc.OPTIONS.item[i].Response.ToString() != null)
                {
                    datosWsMat.DELIV_NUMB = resultadoDoc.OPTIONS.item[i].Response.ToString();
                }
                else
                {
                    datosWsMat.DELIV_NUMB = "";
                }


                contextWsMat.item = datosWsMat;
                var resultadoMat = ClientMat.SI_OA_TM101_Consulta_Entrega(contextWsMat);
                decimal VarRequerido = 0;
                var RegPda = resultadoMat.ET_DELIVERY_ITEM.Count();
                var ColPda = 4;
                decimal idMaterial = 0;

                for (int f = 0; f < RegPda; f++)
                {
                    Tabla tabla = new Tabla();

                    if (resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString() != null)
                    {
                        idMaterial = long.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());
                    }
                    else
                    {
                        idMaterial = 0;
                    }

                    var ValidaCodigo = db.CPCatMateriales.Where(x => (x.CPIdEmpresa == IdPlanta) & (x.CPIdMaterialSAP == idMaterial)).FirstOrDefault();
                    if (ValidaCodigo != null)
                    {
                        if (resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString() != null)
                        {
                            idMaterial = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());
                            tabla.Id = idMaterial.ToString();
                        }
                        else
                        {
                            tabla.Id = "0";
                        }

                        if (resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString() != null)
                        {
                            VarRequerido = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString());


                            if (valoromayor < decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].LFIMG.ToString()))
                            {

                                var TipoPeso = resultadoMat.ET_DELIVERY_ITEM[f].MEINS.ToString();
                                TipoPeso = TipoPeso.Substring(0, 1);

                                if (TipoPeso == "K")
                                {
                                    VarRequerido = (VarRequerido * 1000);
                                    tabla.Requerido = VarRequerido.ToString();
                                }
                                else
                                {
                                    tabla.Requerido = VarRequerido.ToString();
                                }

                                valoromayor = VarRequerido;
                                elemetomayor = decimal.Parse(resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString());

                            }

                            SumaDeProd = (SumaDeProd + VarRequerido);
                        }
                        else
                        {
                            tabla.Requerido = "0";
                        }

                        if (resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString() != null)
                        {
                            tabla.Codigo = resultadoMat.ET_DELIVERY_ITEM[f].MATNR.ToString();
                        }
                        else
                        {
                            tabla.Codigo = "";
                        }


                        if (resultadoMat.ET_DELIVERY_ITEM[f].ARKTX.ToString() != null)
                        {
                            tabla.Descripcion = resultadoMat.ET_DELIVERY_ITEM[f].ARKTX.ToString();
                        }
                        else
                        {
                            tabla.Descripcion = "";
                        }

                        tabla.Requerido = VarRequerido.ToString();

                        BDObtenerPesoSAPEntities contex = new BDObtenerPesoSAPEntities();
                        lsitTabla.Add(tabla);
                    }





                }


                Session["valoromayor"] = valoromayor;
                Session["elemetomayor"] = elemetomayor;
            }

            return Json(lsitTabla, JsonRequestBehavior.AllowGet);
        }




        public JsonResult Index3(int IdPlanta = 0, long Id = 0, string IdTipo = "")
        {
            CPBitacora Bitacora = new CPBitacora();
            var Planta = int.Parse(Session["idPlantaDF"].ToString());

            Session["IdTransporte"] = Id;
            Session["TipoVehiculo"] = IdTipo;

            var Consecutivo = long.Parse(Id.ToString());
            long ValorSAP = 0;
            var RespBusqueda = 0;



            try
            {
                var Busca = db.CPBitacora.Where(x => (x.CPIdTransporte == Consecutivo)).FirstOrDefault();

                if (Busca != null)
                {
                    if (Busca.CPSalida == null)
                    {
                        RespBusqueda = int.Parse(("2").ToString());
                    }
                    else
                    {
                        RespBusqueda = int.Parse(("3").ToString());
                    }

                    //ViewBag.selecto = db.CPCatMateriales.Find(Busca.CPIdMaterial).CPIdMaterial;


                    return Json(db.CPBitacora.ToList().Where(x => (x.CPIdTransporte == Consecutivo)).Select(x => new { CPIdEmpresa = x.CPIdEmpresa, CPIdMaterial = x.CPIdMaterial, CPPesoEntrada = x.CPPesoEntrada, CPPesoSalida = x.CPPesoSalida, CPNumEconomico = x.CPNumEconomico, CPPlaca = x.CPPlaca, CPNomConductor = x.CPNomConductor, CPFechaEntrada = x.CPFechaEntrada, CpFechaSalida = x.CpFechaSalida, CPEntrada = x.CPEntrada, CPSalida = x.CPSalida, CPNumPorte = x.CPNumPorte, CPNumCelular = x.CPNumCelular, CPPesoNeto = x.CPPesoNeto, CPEstatus = x.CPEstatus }), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var PlantaSAP = 0;
                    RespBusqueda = int.Parse(("1").ToString());

                    ValidaTransporte.SI_OS_Consulta_TransporteClient Client3 = new ValidaTransporte.SI_OS_Consulta_TransporteClient();
                    ValidaTransporte.DT_Consulta_Tranporte contextWs3 = new ValidaTransporte.DT_Consulta_Tranporte();
                    ValidaTransporte.DT_Consulta_TranporteData datosWs3 = new ValidaTransporte.DT_Consulta_TranporteData();

                    Client3.ClientCredentials.UserName.UserName = "POCYDSAINT";
                    Client3.ClientCredentials.UserName.Password = "Cydsa2019$";

                    datosWs3.Numero_Transporte = Session["IdTransporte"].ToString();
                    datosWs3.Centro_Logistico = Session["idPlantaDF"].ToString();

                    contextWs3.data = datosWs3;
                    var resultado3 = Client3.SI_OS_Consulta_Transporte(contextWs3);
                    var existente3 = resultado3.OPTIONS[0].Type;
                    var Respuesta = resultado3.OPTIONS[0].Response;
                    long IdTransporteSAP = 0;

                    if (existente3 != "E")
                    {
                        var Col = 0;

                        Col = Respuesta.IndexOf(",");
                        var Cons = Respuesta.Substring(0, Col);

                        
                        if (Cons != null)
                        {
                            IdTransporteSAP = long.Parse(Cons.ToString());
                        }
                        Col = Col + 1;
                        Respuesta = Respuesta.Remove(0, Col);



                        Col = Respuesta.IndexOf(",");

                        if (PlantaSAP == 0)
                        {
                            PlantaSAP = int.Parse(Respuesta.ToString());
                        }


                        ValorSAP = Consecutivo;
                    }
                    else
                    {
                        IdTransporteSAP = 0;
                    }
             

                    if (existente3 != "E")
                    {
                        return Json(db.CPBitacora.ToList().Where(x => (x.CPIdTransporte == Consecutivo)).Select(x => new { CPIdEmpresa = x.CPIdEmpresa, CPIdMaterial = x.CPIdMaterial, CPPesoEntrada = x.CPPesoEntrada, CPPesoSalida = x.CPPesoSalida, CPNumEconomico = x.CPNumEconomico, CPPlaca = x.CPPlaca, CPNomConductor = x.CPNomConductor, CPFechaEntrada = x.CPFechaEntrada, CpFechaSalida = x.CpFechaSalida, CPEntrada = x.CPEntrada, CPSalida = x.CPSalida, CPNumPorte = x.CPNumPorte, CPNumCelular = x.CPNumCelular, CPPesoNeto = x.CPPesoNeto, CPEstatus = x.CPEstatus, ErrorSAP = x.CPIdUsuarioSal }), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (IdTransporteSAP == Consecutivo)
                        {
                            if (IdTransporteSAP == 0 && Consecutivo == 0)
                            {
                                ValorSAP = -4;
                                //ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "successalert();", true); esta linea solo funciona en una aplicacion de escritorio
                            }
                            else
                            {
                                if (Respuesta != "")
                                {

                                    ValorSAP = -2;
                                }
                                else
                                {
                                    ValorSAP = -3;
                                }

                            }
                        }
                        else
                        {
                            ValorSAP = -1;
                        }

                        return Json(ValorSAP);
                    }
                }


            }
            catch (Exception e)
            {

                return Json(e);
            }

        }

        /// no esta cambiando el id
        /// esta agarrando el que tenia antes
        /// seguro que ese es el control?
        /// 



        public ActionResult ObtenPesos()
        {
            List<SelectListItem> Puertos = new List<SelectListItem>();
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                Console.WriteLine(ports[i]);
                //item.Empresa = ports[i];
                Puertos.Add(new SelectListItem() { Text = ports[i], Value = ports[i] });
            }


            ViewBag.dropdownPuertos = Puertos;


            //SerialPort port = new SerialPort();
            //SerialPort port = new SerialPort("COM 1"); ;
            SerialPort port = new SerialPort("COM 1", 9600);

            int length = port.BytesToRead;
            //Note that you can swap out a byte-array for a char-array if you prefer.
            byte[] buffer = new byte[length];
            port.Read(buffer, 0, length);

            string curData = port.ReadExisting();  // varias lineas

            string line = port.ReadLine(); //una sola linea


            //            The easiest way to write data over the SerialPort is:

            //port.Write("here is some text to be sent over the serial port.");
            //            However you can also send data over like this when needed:

            ////Note that you can swap out the byte-array with a char-array if you so choose.
            //byte[] data = new byte[1] { 255 };
            //            port.Write(data, 0, data.Length);

            return View();


        }


    }



}