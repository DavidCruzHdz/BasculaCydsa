using ObtenerPesoSAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Security.Cryptography;

namespace CYDSA_Sustentabilidad.Controllers
{
    public class ApiLoginController : Controller
    {

        public static byte[] GetKey(string siteSecret)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(siteSecret)).Take(16).ToArray();
        }

        [System.Web.Http.HttpPost]
        public string GetUsuario(string texto)
        {
            using (AesManaged myAes = new AesManaged())
            {
                try
                {
                    DateTime today = DateTime.Now;
                    string llave = "";
                    llave = today.ToString("yyyyMMdd");

                    int[] asciiArray = llave.Select(r => (int)r).ToArray();
                    int suma = 0;

                    for (int i = 0; i < llave.Length; i++) { suma += (int)llave[i]; }

                    llave = suma.ToString() + llave;

                    int diadehoy = today.Day;
                    suma = suma % diadehoy;
                    llave = suma.ToString() + llave;
                    llave = "BasculaSAP" + llave;

                    myAes.Key = GetKey(llave);
                    myAes.IV = myAes.Key;


                    //const string s = "Dot Net Perls is about Dot Net.";
                    //Console.WriteLine(s);

                    //// We must assign the result to a variable.
                    //// ... Every instance is replaced.
                    //string v = s.Replace("Net", "Basket");
                    //Console.WriteLine(v);

                    string textoades = texto;
                    textoades = textoades.Replace("-", "+");
                    textoades = textoades.Replace("_", "/");
                    textoades = textoades.Replace(",", "=");


                    byte[] encrypted = Convert.FromBase64String(textoades);
                    string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
                    return roundtrip;

                }
                catch
                {

                    Session["idUsuario"] = 0;
                    Session["logeado"] = false;
                    ViewBag.error = "usuario incorrecto";
                    //return Redirect("/Usuarios/Login");

                    string roundtrip = "";
                    return roundtrip;
                }


            }
        }



        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException("cipherText");

            if (Key == null || Key.Length <= 0) throw new ArgumentNullException("Key");

            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");

            string plaintext = null;
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();

                        }
                    }
                }
            }
            return plaintext;
        }



        public ActionResult TextoDesEncryptado(string texto)
        {
            string result = GetUsuario(texto);
            Session["NombreUsuario"] = result;

            bool existe = false;
            BDObtenerPesoSAPEntities context = new BDObtenerPesoSAPEntities();
            var ususarios = context.CPUsuario.ToList().Where(x => x.CPNombreUsuario == result).FirstOrDefault();
            if (ususarios != null)
                existe = true;

            if (existe)
            {
                //if (!context.IdsPantallasPermisos.Any(x => x.IdPantalla == 9 && x.IdUsuario == ususarios.IdsIdUsuario))
                //{
                var empresa = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == ususarios.CPIdUsuario && x.CPPlantaDefault == true).FirstOrDefault().CPIdEmpresa;
                Session["logeado"] = true;
                Session["idUsuario"] = ususarios.CPIdUsuario;
                Session["idPlantaDF"] = context.CPPermisosPlantas.Where(x => x.CPIdUsuario == ususarios.CPIdUsuario && x.CPPlantaDefault == true).FirstOrDefault().CPIdEmpresa;
                Session["NombrePlanta"] = context.CPCatEmpresas.Where(x => x.CPIdEmpresa == empresa).FirstOrDefault().CPDescripcionEmpresa;

                return Redirect("/Home/Index");
                //}

            }
            else
            {
                Session["idUsuario"] = 0;
                Session["logeado"] = false;
                ViewBag.error = "usuario incorrecto";
                return Redirect("/Usuarios/Login");

            }


            var entity = context.CPUsuario.ToList().Where(x => x.Estatus == true);
            return View(entity);
        }


        public string GetEncripatar(string ususario)
        {

            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(ususario);
            result = Convert.ToBase64String(encryted);
            return result;
        }





        /// Encripta una cadena
        //public static string Encriptar(this string _cadenaAencriptar)
        //{
        //    string result = string.Empty;
        //    byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
        //    result = Convert.ToBase64String(encryted);
        //    return result;
        //}



        ///// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        //public static string DesEncriptar(this string _cadenaAdesencriptar)
        //{
        //    string result = string.Empty;
        //    byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
        //    //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
        //    result = System.Text.Encoding.Unicode.GetString(decryted);
        //    return result;
        //}


    }
}
