using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class GroupProductList
    {
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public string SectionDesc { get; set; }
        public string GroupName { get; set; }
        public long ProductId { get; set; }
        public string Gender { get; set; }
        public long GroupIdByShop { get; set; }
        public long OrderData { get; set; }
        public long SolutionId { get; set; }
        public string DefaultGroupName { get; set; }
        public long Group_orderdata { get; set; }
        public long GroupIdByDefault{ get; set; }
        public long ? Work_Flow_Id { get; set;}
        public string AccessToData { get;set;}
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set;}
        public string Insurance { get; set;}
        public long? InsuranceId { get; set;}
        public int? Tarif_Number { get; set; }
        public string Settlement_Number { get; set; }
        public string vat { get; set; }
        public string CustomText { get; set; }
        public string DefaultStatus { get; set;}
    }
    public class CatgDetails
    {
        public long CatgId { get; set; }
        public string CatgType { get; set; }
        public string ActiveStatus { get; set; }
        public long DomainType { get; set; }
        public string CatgDesc { get; set; }
        public string Gender { get; set; }
        public long Value { get; set; }
    }
}