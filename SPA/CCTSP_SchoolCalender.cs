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
    
    public partial class CCTSP_SchoolCalender
    {
        public long CalId { get; set; }
        public long SchId { get; set; }
        public long ClassSecId { get; set; }
        public string AcdYearName { get; set; }
        public System.DateTime CalStartDate { get; set; }
        public System.DateTime CalEndDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
    }
}