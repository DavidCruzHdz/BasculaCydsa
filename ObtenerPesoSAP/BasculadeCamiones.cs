namespace ObtenerPesoSAP
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;

    public class BasculadeCamiones : DbContext
    {
        public BasculadeCamiones()
            : base("name=BasculadeCamiones")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<CPCatMateriales> CPCatMateriales { get; set; }

   

        //public virtual ObjectResult<ComboMateriales_Result> ComboMediciones(Nullable<int> idMaterial)
        //{
           
        //    var idMaterialParameter = idMaterial.HasValue ?
        //        new ObjectParameter("IdMaterial", idMaterial) :
        //        new ObjectParameter("IdMaterial", typeof(int));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ComboMateriales_Result>("ComboMateriales", idMaterialParameter);
        //}
    }

}