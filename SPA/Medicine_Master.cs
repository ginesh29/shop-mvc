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
    
    public partial class Medicine_Master
    {
        public long Medicine_Id { get; set; }
        public long Prescription_Id { get; set; }
        public long UserId { get; set; }
        public System.DateTime created_on { get; set; }
        public int BookingId { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string NtimesDay { get; set; }
        public string Times { get; set; }
        public string Remarks { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<long> SchId { get; set; }
        public Nullable<long> MasterResId { get; set; }
    
        public virtual CCTSP_SchoolMaster CCTSP_SchoolMaster { get; set; }
        public virtual CCTSP_User CCTSP_User { get; set; }
        public virtual SPA_EmployeeScheduler SPA_EmployeeScheduler { get; set; }
        public virtual Prescription_Master Prescription_Master { get; set; }
        public virtual spa_Master_Reservation spa_Master_Reservation { get; set; }
    }
}