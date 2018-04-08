using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class translateModel
    {

        public string Page_Name { get; set; }
        public string Label_Name { get; set; }
        public string Value { get; set; }
        public string GermanValue { get; set; }
        public string FrenchValue { get; set; }
        public Nullable<int> Order_id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string Lan_Source { get; set; }
    }
}