using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class EmployeeBooking
    {
        public int date { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string bookingdate { get; set; }
        public string fromslotmasterid { get; set; }
        public string toslotmasterid { get; set; }
        public long emp_userid { get; set; }
        public long client_userid { get; set; }
        public long product_id { get; set; }
        public string product_price { get; set; }
        public string ConcateDate { get; set; }
    }
    public class TimeSlots
    {
        public string T1 { get; set; }
        public TimeSpan T2 { get; set; }
        public long Scheduler_Time_Id { get; set; }
        public int Status { get; set; }
        public string SlotDue { get; set; }
    }
}