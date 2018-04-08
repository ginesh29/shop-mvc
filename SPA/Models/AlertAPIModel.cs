using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class AlertAPIModel
    {
        public string username { get; set; }
        public string Password { get; set; }
        public string ToWhom { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public string DateTime { get; set; }
        public int type_id { get; set; }
        public string greetingMessage { get; set; }
        public bool display_name { get; set; }
        public string bodyMessage { get; set; }
        public string endMessage { get; set; }
        public string endName { get; set; }
        public string Birthday_wish { get; set; }
    }
    public class Error
    {
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Extra { get; set; }
        public long? Schlid { get; set;}
        public long? UserId { get; set;}
    }
}