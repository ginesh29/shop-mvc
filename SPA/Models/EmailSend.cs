using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class EmailSend
    {
        public string EmailId { get; set; }
        public string TitleHead { get; set; }
        public string DatabaseBody { get; set; }
        public string ShopName { get; set; }
        public long schid { get; set; }
        public string SMstext { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string LogoText { get; set; }
        public long? MainCategory { get; set; }
    }
}