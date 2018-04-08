using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class TopTen
    {
        public long UserId { get; set; }
        public long TotalPrice { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public long Mobile { get; set; }
        public string Email { get; set; }
        public string Employee { get; set; }
        public string LastVisited { get; set; }

    }
    public class CustomerTabInfo
    {
        public long? UserId { get; set; }
        public double? Revenue { get; set; }
        public string Gender { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Phoneno { get; set; }
        public string Emailid { get; set; }
        public DateTime? LastVisited { get; set; }
        public string EmpfirstName { get; set;}
        public string LandLineNumber { get; set;}
        public long? SchId { get; set;}
        public string AccessToData { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set; }
        public string ActiveStatus { get; set;}
        public long? CustomerId { get; set;}
    }
    public class CountryList
    {
        public string Country { get; set;}
    }
}