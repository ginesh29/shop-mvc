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
    
    public partial class CCTSP_InstallmetDetails
    {
        public long InstallmentID { get; set; }
        public long InstallCatgId { get; set; }
        public System.DateTime DateFrom { get; set; }
        public System.DateTime DateTo { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
        public long SchId { get; set; }
        public long AcdYear { get; set; }
        public Nullable<decimal> FineAmount { get; set; }
        public System.DateTime FineDate { get; set; }
        public string InstallmentName { get; set; }
    }
}
