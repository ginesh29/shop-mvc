using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class ActivityLog
    {
        public string Action { get; set; }
        public string controller { get; set; }
        public long schId { get; set; }
        public string Message { get; set; }
        public object Activity { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
    }
}