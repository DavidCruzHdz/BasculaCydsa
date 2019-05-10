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
    using System.ComponentModel.DataAnnotations;

    public partial class CPCatMateriales
    {
        public int CPIdMaterial { get; set; }
        [DisplayName("Planta")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "La informacion es requerida")]
        public int CPIdEmpresa { get; set; }
        [DisplayName("Codigo Anterior")]
        public string CPIdMaterialAnt { get; set; }
        [DisplayFormat(DataFormatString = "{0:F0}")]
        [DisplayName("Codigo SAP")]
        [RegularExpression(@"[0-9]*\.?[0-9]*", ErrorMessage = "La cantidad debe contener s�lo n�meros")]
        public Nullable<decimal> CPIdMaterialSAP { get; set; }
        [DisplayName("Descripcion")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "La informacion es requerida")]
        public string CPDescripcionMaterial { get; set; }
        [DisplayName("Peso Solicitado")]
        [RegularExpression(@"[0-9]*\.?[0-9]*", ErrorMessage = "La cantidad debe contener s�lo n�meros")]
        public Nullable<double> CPPesoRequerido { get; set; }
        [DisplayName("Factor Minimo")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        [RegularExpression(@"[0-9]*\.?[0-9]*", ErrorMessage = "La cantidad debe contener s�lo n�meros")]
        public decimal CPFactorMin { get; set; }
        [DisplayName("Factor Maximo")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        [RegularExpression(@"[0-9]*\.?[0-9]*", ErrorMessage = "La cantidad debe contener s�lo n�meros")]
        public decimal CPFactorMax { get; set; }
        [DisplayName("Se Requiere Pesar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Seleccione una opcion")]
        public string CPSePesa { get; set; }
        [DisplayName("Se Autoriza")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Seleccione una opcion")]
        public string CPRequiereAutoriza { get; set; }
        [DisplayName("Unidad de Medida")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Seleccione una unidad de medida por favor")]
        public int CPIdUnidadMedida { get; set; }
        public Nullable<System.DateTime> CPFechaAlta { get; set; }
        public Nullable<int> CPUsuarioAlta { get; set; }
        public Nullable<System.DateTime> CPFechaCambio { get; set; }
        public Nullable<int> CPUsuarioCambio { get; set; }

        public virtual CPCatEmpresas CPCatEmpresas { get; set; }
        public virtual CPCatUnidades CPCatUnidades { get; set; }
        public virtual CPUsuario CPUsuario { get; set; }
        public virtual CPUsuario CPUsuario1 { get; set; }
    }
}