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
    
    public partial class SPA_LeaveMaster
    {
        public long LeaveId { get; set; }
        public Nullable<long> UserId { get; set; }
        public long SchId { get; set; }
        public Nullable<long> CalID { get; set; }
        public string HolidayDesc { get; set; }
        public Nullable<long> HolidayTypeId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string CalendarName { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.TimeSpan> StartTime { get; set; }
        public Nullable<System.TimeSpan> EndTime { get; set; }
        public Nullable<long> Created_By { get; set; }
    }
}
