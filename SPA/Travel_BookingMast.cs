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
    
    public partial class Travel_BookingMast
    {
        public long BookingID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public long UserID { get; set; }
        public long TravelFrom { get; set; }
        public long TravelTo { get; set; }
        public string Remarks { get; set; }
        public string ActiveStatus { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public long SchId { get; set; }
        public string OtherDetail { get; set; }
        public string OtherInfo { get; set; }
        public Nullable<long> OtherValue { get; set; }
        public Nullable<long> OtherOrderNo { get; set; }
        public Nullable<long> BookingUniq { get; set; }
        public string BookingVeNumber { get; set; }
        public string DriverDetails { get; set; }
        public string RequestFor { get; set; }
        public string RequestProcessStatus { get; set; }
        public string VisaDetail { get; set; }
        public string PassPortDetail { get; set; }
    }
}
