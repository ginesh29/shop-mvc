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
    
    public partial class Spa_BookingDetails
    {
        public long ResDetailId { get; set; }
        public Nullable<int> ReservationId { get; set; }
        public Nullable<long> productId { get; set; }
        public string Product_price { get; set; }
        public Nullable<long> schId { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string Reqcol1 { get; set; }
        public string Reqcol2 { get; set; }
        public string Reqcol3 { get; set; }
    
        public virtual CCTSP_CategoryDetails CCTSP_CategoryDetails { get; set; }
        public virtual CCTSP_SchoolMaster CCTSP_SchoolMaster { get; set; }
        public virtual SPA_EmployeeScheduler SPA_EmployeeScheduler { get; set; }
    }
}
