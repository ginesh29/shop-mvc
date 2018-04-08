using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class MarketingMaster
    {
       public long MID { get; set; }
        public string content { get; set; }
        public string Subject { get; set; }
        public string status { get; set; }
        public string Chkstatus { get; set; }
        public string[] Extra_1 { get; set; }

    }
}