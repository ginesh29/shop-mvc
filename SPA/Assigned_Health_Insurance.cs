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
    
    public partial class Assigned_Health_Insurance
    {
        public long Health_Assign_id { get; set; }
        public Nullable<long> Insurance_Id { get; set; }
        public long SchlId { get; set; }
        public System.DateTime Created_Date { get; set; }
        public System.DateTime Updated_Date { get; set; }
        public string ActiveStatus { get; set; }
        public string Amount { get; set; }
        public string Duration { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public Nullable<long> Field3 { get; set; }
        public Nullable<long> field4 { get; set; }
        public Nullable<int> SettlementNumber { get; set; }
        public Nullable<int> order { get; set; }
        public Nullable<int> Tarif_Number { get; set; }
        public string Settlement_text { get; set; }
    
        public virtual CCTSP_SchoolMaster CCTSP_SchoolMaster { get; set; }
    }
}