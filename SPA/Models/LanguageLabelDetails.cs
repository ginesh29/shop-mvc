using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class LanguageLabelDetails
    {
        public int Lang_id { get; set; }
        public string Value { get; set; }
        public string Page_Name { get; set; }
        public Nullable<int> Order_id { get; set; }
        public string English_Label { get; set; }
        public string Label_Name { get; set; }
        public string languageCulture { get; set; }
        public int? checkId { get; set; }
    }
    public class LanguageNewShop
    {
        public int Lang_id { get; set; }
        public string Value { get; set; }
        public string Page_Name { get; set; }
        public Nullable<long> Order_id { get; set; }
        public string English_Label { get; set; }
        public string SubPage { get; set;}
        public string Catg_Sep { get; set; }

    }
    public class ShopDetails
    {
        public int? LangId { get; set; }
        public int? Display_FreeCustomer { get; set;}
        public string Currency { get; set;}
    }
}