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
    
    public partial class CCTSP_TeacherScheduler
    {
        public long TeacherSchedulerId { get; set; }
        public long DetailsId { get; set; }
        public long UserId { get; set; }
        public long SubjId { get; set; }
        public long ClassSecId { get; set; }
        public Nullable<long> Type { get; set; }
        public long FacilityId { get; set; }
        public string Remarks { get; set; }
        public string BatchName { get; set; }
        public Nullable<long> SchId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<long> AcdYearId { get; set; }
        public Nullable<System.TimeSpan> NStartTime { get; set; }
        public Nullable<System.TimeSpan> NEndTime { get; set; }
        public Nullable<long> BatchId { get; set; }
    }
}
