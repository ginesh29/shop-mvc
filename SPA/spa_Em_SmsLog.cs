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
    
    public partial class spa_Em_SmsLog
    {
        public long Em_Sms_Id { get; set; }
        public int ReservationId { get; set; }
        public Nullable<int> Temp { get; set; }
        public Nullable<int> Book { get; set; }
        public Nullable<int> ApClosed { get; set; }
        public Nullable<int> Decline_Temp { get; set; }
        public Nullable<int> Decline_Openlist { get; set; }
        public Nullable<int> Reminder_Sms { get; set; }
        public Nullable<int> Reminder_Email { get; set; }
        public Nullable<int> Login { get; set; }
        public Nullable<int> Registration { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
    
        public virtual SPA_EmployeeScheduler SPA_EmployeeScheduler { get; set; }
    }
}