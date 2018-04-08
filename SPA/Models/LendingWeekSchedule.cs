using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class LendingWeekSchedule
    {
        public long? Value { get; set;}
        public string CatgDesc { get; set; }
        public string Day { get; set; }
        public long CatgTypeid { get; set; }
        public string Gender { get; set; }
    }
    public class LendingWeekScheduleDetails
    {       
        public string Catgdesc { get; set; }
        public string Day { get; set; }
        public long CatgTypeId { get; set; }    
        public string Gender { get; set; }
    }
    public class LandingDetails
    {
        public string Logo { get; set; }
        public string Logotext { get; set; }
        public string ShopName { get; set; }
        public string Logotext_Color { get; set;}
        public string Shopimg1 { get; set; }
        public string Shopimg2 { get; set; }
        public string Shopimg3 { get; set; }
        public string Shopimg4 { get; set; }
        public Nullable<long> Schid { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }              
        public string Address { get; set; }
        public string Email_id { get; set; }
        public string PhoneNo { get; set; }
        public string ZipCode { get; set; }
        public string ShopCity { get; set; }
        public long? Color_Id { get; set;}
    }
    public class EmployeeSpecializationList
    {
        public long EmpId { get; set; }
        public List<Models.MainCategoryDetails> SpecializationList { get; set; }
    }
    public class FreecustomerShoptypeDetails
    {
        public string ShopTypeName { get; set; }
        public long ShopTypeId { get; set; }
        public long MainCatgId { get; set; }
        public string MainCategory { get; set; }

    }

}