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
    
    public partial class CCTSP_CalendarDetails
    {
        public long CalDetailId { get; set; }
        public long SchId { get; set; }
        public long CalID { get; set; }
        public Nullable<long> HolidayTypeId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string CalendarName { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
    }
}