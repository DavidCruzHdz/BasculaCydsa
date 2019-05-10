using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Ports;
using System.Threading;

namespace ObtenerPesoSAP.Controllers
{
    public class LeerBasculaController : Controller
    {

     //   public class SerialPort : System.ComponentModel.Component

        // GET: LeerBascula
        public ActionResult Index()
        {

            SerialPort Port = new SerialPort();
     



            return View();
        }
    }
}