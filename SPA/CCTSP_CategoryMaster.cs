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
    
    public partial class CCTSP_CategoryMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CCTSP_CategoryMaster()
        {
            this.CCTSP_CategoryDetails = new HashSet<CCTSP_CategoryDetails>();
            this.Prescription_Detail = new HashSet<Prescription_Detail>();
        }
    
        public long CatgId { get; set; }
        public string CatgName { get; set; }
        public string CatgDesc { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCTSP_CategoryDetails> CCTSP_CategoryDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prescription_Detail> Prescription_Detail { get; set; }
    }
}
