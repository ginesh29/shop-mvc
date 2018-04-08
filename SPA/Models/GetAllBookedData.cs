using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class GetAllBookedData
    {
        public string [] DataSlot { get; set;}
        public string [] WeekdaysBlock { get; set;}
        public string [] employeelive { get; set;}
        public string [] NotAvailableDay { get; set; }
        public string [] NotAvailableTime { get; set; }
        public string [] TimeSlotAvailable { get; set; }
        public string [] QuickBlock { get; set; }
        public string TotalRev { get; set; }        
    }
 
}