using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ObtenerPesoSAP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Verificacion de Pesos en Bascula";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Nuestros Contactos";

            return View();
        }
    }
}