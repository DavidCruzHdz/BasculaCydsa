﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ObtenerPesoSAP.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class BDObtenerPesoSAPEntities : DbContext
    {
        public BDObtenerPesoSAPEntities()
            : base("name=BDObtenerPesoSAPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CPAutorizaciones> CPAutorizaciones { get; set; }
        public virtual DbSet<CPBitacora> CPBitacora { get; set; }
        public virtual DbSet<CPCatEmpresas> CPCatEmpresas { get; set; }
        public virtual DbSet<CPCatMateriales> CPCatMateriales { get; set; }
        public virtual DbSet<CPCatTipoCaptura> CPCatTipoCaptura { get; set; }
        public virtual DbSet<CPCatTiposDeVehiculos> CPCatTiposDeVehiculos { get; set; }
        public virtual DbSet<CPCatUnidades> CPCatUnidades { get; set; }
        public virtual DbSet<CPPantallas> CPPantallas { get; set; }
        public virtual DbSet<CPPantallasPermisos> CPPantallasPermisos { get; set; }
        public virtual DbSet<CPPartidas> CPPartidas { get; set; }
        public virtual DbSet<CPPermisosPlantas> CPPermisosPlantas { get; set; }
        public virtual DbSet<CPRol> CPRol { get; set; }
        public virtual DbSet<CPUsuario> CPUsuario { get; set; }
    }
}
