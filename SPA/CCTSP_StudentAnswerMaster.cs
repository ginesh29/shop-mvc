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
    
    public partial class CCTSP_StudentAnswerMaster
    {
        public long StudentAnswerMasterId { get; set; }
        public long SchId { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<int> TotalTime { get; set; }
        public Nullable<int> TimeTaken { get; set; }
        public Nullable<int> TotalMark { get; set; }
        public Nullable<int> MaximumMark { get; set; }
        public Nullable<int> NoOfQuestionAttempted { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }
}