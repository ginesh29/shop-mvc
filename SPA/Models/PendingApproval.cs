using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class PendingApproval
    {
        public string EmpfirstName { get; set; }
        public string EmpLastName { get; set; }
        public int EmpSchDetailsId { get; set; }
        public string CatgDesc { get; set; }
        public int Duration { get; set; }
        public string BookingDate { get; set; }
        public TimeSpan FromSlotMasterId { get; set; }
        public TimeSpan ToSlotMasterId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhone { get; set; }
        public string reg_status { set; get; }
        public string BookedStatus { set; get; }
        public string ActiveStatus { set; get; }
        public int diff { get; set; }
        public string comment { set; get; }   
        public long CLIENT_UserId { set; get; }
        public string currency { get; set; }
        public int StDoctNotes { get; set; }
        public int StPrescription { get; set; }
        public int StMerge { get; set; }
        public int SchlStudentStrength { get; set; }
        public string AccessToData { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set;}
    }
}