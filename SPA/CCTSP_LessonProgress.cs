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
    
    public partial class CCTSP_LessonProgress
    {
        public long LessonProgressId { get; set; }
        public long TeacherSchedulerId { get; set; }
        public Nullable<long> UserId { get; set; }
        public long SchId { get; set; }
        public Nullable<long> SubjId { get; set; }
        public Nullable<long> SchduleType { get; set; }
        public string ReasonForLeave { get; set; }
        public Nullable<long> CurrentTopic { get; set; }
        public string HomeWork { get; set; }
        public Nullable<long> PercentageOfCompletion { get; set; }
        public Nullable<long> NextTopic { get; set; }
        public string ActivityDetails { get; set; }
        public string Assignment { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreateDay { get; set; }
        public long AcdYearId { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.DateTime> RescheduleDate { get; set; }
    }
}
