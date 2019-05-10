using ObtenerPesoSAP.ADO;
using ObtenerPesoSAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ObtenerPesoSAP.ADO
{
    public class MaterialesADO
    {
        BDObtenerPesoSAPEntities Context;   // esta es la alias de la base de datos
        public MaterialesADO()
        {
            Context = new BDObtenerPesoSAPEntities();
        }

        public IEnumerable<CPCatMateriales> cmbMateriales(int Planta) // este metodo carga combo
        {
            return Context.CPCatMateriales.Where(x=> x.CPIdEmpresa == Planta);
        }
    }
}