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
    
    public partial class CCTSP_AttendenceMaster
    {
        public long AttendenceMasterId { get; set; }
        public long SchId { get; set; }
        public long TeacherSchedulerId { get; set; }
        public Nullable<long> AttendenceBy { get; set; }
        public Nullable<long> SubjId { get; set; }
        public long ClassSecId { get; set; }
        public Nullable<long> TotalNoOfStrength { get; set; }
        public Nullable<long> TotalNoOfPresent { get; set; }
        public Nullable<long> TotalNoOfAbsent { get; set; }
        public string RemarksByTeacher { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }
}
