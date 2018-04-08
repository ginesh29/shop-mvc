using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class ReminderCountry
    {
        public string BookingDate { get; set; }
        public string FromSlotMasterId { get; set; }
        public string ToSlotMasterId { get; set; }
        public string CatgDesc { get; set; }
        public string SchlName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientGender { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLastName { get; set; }
        public long SchId { get; set; }
        public string Country { get; set; }
        public int EmpSchDetailsId { get; set; }
        public string Email_Reg { get; set; }
        public string Sms_reg { get; set; }
        public string send_sms { get; set; }
        public string TimeZone { get; set; }
        public DateTime BookedDate { get; set; }
    }
}