using ObtenerPesoSAP.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Management;
using System.IO;
using System.Collections;


namespace ObtenerPesoSAP.Controllers
{
    public class ConfigurarController : Controller
    {
        BDObtenerPesoSAPEntities db = new BDObtenerPesoSAPEntities();
        // GET: Configurar
        public ActionResult Configurar()
        {

            List<SelectListItem> Puertos = new List<SelectListItem>();
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                //Console.WriteLine(ports[i]);
                Puertos.Add(new SelectListItem() { Text = ports[i], Value = ports[i] });
            }

            ViewBag.dropdownPuertos = Puertos;


            ////creamos una lista de USBInfo

            //List<USBInfo> lstDispositivos = new List<USBInfo>();

            ////creamos un ManagementObjectCollection para obtener nuestros dispositivos
            //ManagementObjectCollection collection;

            ////utilizando la WMI clase Win32_USBHub obtenemos todos los dispositivos USB
            //using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))

            //    //asignamos los dispositivos a nuestra coleccion
            //    collection = searcher.Get();

            ////recorremos la colección
            //foreach (var device in collection)
            //{
            //    //asignamos el dispositivo a nuestra lista
            //    lstDispositivos.Add(new USBInfo(
            //    (string)device.GetPropertyValue("DeviceID"),
            //    (string)device.GetPropertyValue("PNPDeviceID"),
            //    (string)device.GetPropertyValue("Description")
            //    ));
            //}

            ////liberamos el objeto collection
            //collection.Dispose();
            ////regresamos la lista
            //return lstDispositivos;

            return View();
        }




        // POST: Bitacora/LeerBascula
        //[HttpPost]
        public ActionResult LeerBascula(int txtId)
        {

            StreamReader objReader = new StreamReader("c:\\GenerarPesos\\operaciones_1.txt");
            string sLine = "";
            ArrayList arrText = new ArrayList();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                    arrText.Add(sLine);
            }
            objReader.Close();

            //var contador = 1;
            var Col = 0;
            var resultado = "";
            var dato = "";
            foreach (string sOutput in arrText)
            {
                //resultado = resultado.Split(',').Last();
                //    Console.WriteLine(sOutput);
                //    Console.ReadLine();

                resultado = sOutput;
            }

            CPBitacora entity = new CPBitacora(); // Lerma se instancia el modelo
            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["VarConsecutivo"] = dato;
                ViewBag.VarConsecutivo = dato;
                //entity.CPIdTransporte = int.Parse(dato.ToString()); //lerma se mandan los valores al modelo
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);


            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPNumEconomico"] = resultado;
                //ViewBag.CPNumEconomico = dato;
                entity.CPNumEconomico = dato.ToString();
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);


            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPesoEntrada"] = resultado;
                entity.CPPesoEntrada = int.Parse(dato.ToString());
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);


            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPesoSalida"] = resultado;
                entity.CPPesoSalida = int.Parse(dato.ToString());
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);


            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPesoNeto"] = resultado;
                entity.CPPesoNeto = int.Parse(dato.ToString());
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);


            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPlaca"] = resultado;
                entity.CPPlaca = dato;
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);



            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPlaca"] = resultado;
                entity.CPPlaca = dato;
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);

            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPlaca"] = resultado;
                entity.CPPlaca = dato;
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);

            Col = resultado.IndexOf(",");
            //Col = Col - 1;
            dato = resultado.Substring(0, Col);
            if (dato != null)
            {
                //ViewData["CPPlaca"] = resultado;
                entity.CPPlaca = dato;
            }
            Col = Col + 1;
            resultado = resultado.Remove(0, Col);


            Col = resultado.IndexOf(",");
            dato = resultado;
            if (dato != null)
            {
                //ViewData["CPPlaca"] = resultado;
                entity.CPPlaca = dato;
            }


            //contador = (contador + 1);




            return View(entity);// lerma se manda el modelo a la vista 
        }





        public ActionResult ProbarPuerto()
        {
            // Instantiate the communications // port with some basic settings  
            SerialPort port = new SerialPort("COM1", 115200, Parity.None, 8, StopBits.One);

            // Open the port for communications  
            //ReadFromFile("c:\\GenerarPesos\\operaciones_1.txt", port);

            StreamReader objReader = new StreamReader("c:\\GenerarPesos\\operaciones_1.txt");
            string sLine = "";
            ArrayList arrText = new ArrayList();

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                    arrText.Add(sLine);
            }
            objReader.Close();

            foreach (string sOutput in arrText)
                Console.WriteLine(sOutput);
            Console.ReadLine();



            //SerialPort port = new SerialPort("COM 1", 9600);

            //port.Open();
            //int length = port.BytesToRead;
            //byte[] buffer = new byte[length];
            //port.Read(buffer, 0, length);
            //string curData = port.ReadExisting();  // varias lineas
            //string line = port.ReadLine(); //una sola linea


            return View();
        }
    }
}