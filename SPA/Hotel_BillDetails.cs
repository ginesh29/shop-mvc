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
    
    public partial class Hotel_BillDetails
    {
        public long BillDetailsID { get; set; }
        public long BillMasterID { get; set; }
        public string ServiceDesc { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Advance { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<int> Unit { get; set; }
        public Nullable<decimal> UnitCost { get; set; }
    }
}
