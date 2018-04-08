using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class CalendarMonthModel
    {

        public List<int> day_number { get; set; }
        public List<string> day_name { get; set; }
        public int week_number { get; set; }
        public List<string> times { get; set; }
        
        public int colSpan { get; set; }

        public string CurrentMonth { get; set; }
        public string Currentyear { get; set; }

    }
}