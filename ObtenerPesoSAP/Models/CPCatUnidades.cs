//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class CPCatUnidades
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CPCatUnidades()
        {
            this.CPCatMateriales = new HashSet<CPCatMateriales>();
            this.CPCatTiposDeVehiculos = new HashSet<CPCatTiposDeVehiculos>();
            this.CPPartidas = new HashSet<CPPartidas>();
            this.CPPartidas1 = new HashSet<CPPartidas>();
        }

        public int CPIdUnidadMedida { get; set; }
        [DisplayName("Unidad de Medida")]
        public string CPDescripcionUnidadMedida { get; set; }
        public Nullable<System.DateTime> CPFechaAlta { get; set; }
        public Nullable<int> CPUsuarioAlta { get; set; }
        public Nullable<System.DateTime> CPFechaCambio { get; set; }
        public Nullable<int> CPUsuarioCambio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CPCatMateriales> CPCatMateriales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CPCatTiposDeVehiculos> CPCatTiposDeVehiculos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CPPartidas> CPPartidas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CPPartidas> CPPartidas1 { get; set; }
    }
}