using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class EmployeeList
    {
        public long CatgTypeId { get; set; }
        public string SectionName { get; set; }
        public string SectionDesc { get; set; }
        public string ProductName { get; set; }
        public string ProductDetail { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string CatgDesc { get; set; }
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public long RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { set; get; }
        public int Available { get; set; }
        public string Gender { get; set; }

    }
}