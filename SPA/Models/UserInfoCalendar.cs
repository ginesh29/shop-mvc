using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class UserInfoCalendar
    {
        public long user_id { get; set; }
        public string user_name { get; set; }
        public string CurrentMonth { get; set; }
        public string Currentyear { get; set; }
        public int occupied { get; set; }
        public int rating { get; set; }
        public int revenue { get; set; }
    }
}