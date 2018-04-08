//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SPA
{
    using System;
    using System.Collections.Generic;
    
    public partial class Health_Insurance
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Health_Insurance()
        {
            this.Add_on_Product_Master = new HashSet<Add_on_Product_Master>();
            this.CCTSP_CategoryDetails = new HashSet<CCTSP_CategoryDetails>();
        }
    
        public long Insurance_Id { get; set; }
        public Nullable<int> Tarif_Number { get; set; }
        public string Rate_Name { get; set; }
        public string Chapter_Description { get; set; }
        public Nullable<int> Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<long> SchlId { get; set; }
        public Nullable<int> LangId { get; set; }
        public string ActiveStatus { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public Nullable<int> order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Add_on_Product_Master> Add_on_Product_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCTSP_CategoryDetails> CCTSP_CategoryDetails { get; set; }
    }
}
