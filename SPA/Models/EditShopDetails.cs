using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class EditShopDetails
    {
        public string HolidayName { get; set; }
        public string HolidayType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set;}
        public long EditLeaveId { get; set;}
        public long SolutionId { get; set;}
        public string CatgDesc { get; set; }
        public string SectionDesc { get; set;}
        public string Media { get; set; }
        public int LangDetailId { get; set;}
        public string LangValue { get; set;}
        public long Catgtypeid { get; set; }
    }
}