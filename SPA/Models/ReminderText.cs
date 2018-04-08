using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class ReminderText
    {
        public string SectionDesc { get; set;}
        public long? Schid { get; set;}
        public string ItenName { get; set; }
        public long?CatgId { get; set; }
        public int? LangId { get; set; }
        public long? MainCategory { get; set; }
    }
}