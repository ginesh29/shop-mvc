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
    
    public partial class Marketing_SendHistory
    {
        public long MDID { get; set; }
        public long MID { get; set; }
        public Nullable<long> schId { get; set; }
        public string ActiveStatus { get; set; }
        public string status { get; set; }
        public System.DateTime SendDate { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string Extra_1 { get; set; }
        public string Extra_2 { get; set; }
        public string Extra_3 { get; set; }
        public string GroupUserId { get; set; }
    
        public virtual CCTSP_SchoolMaster CCTSP_SchoolMaster { get; set; }
        public virtual Marketing_Master Marketing_Master { get; set; }
    }
}