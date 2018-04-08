using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class Registration
    {
        public string phoneno { get; set; }
        public int Langid { get; set; }
        public string emailid { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string FamilyName { get; set; }
        public string password { get; set; }
        public string ConfirmPassword { get; set; }
        public string shopname { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string State_hidden { get; set; }
        public string City_hidden { get; set; }
        public string Country_hidden { get; set; }
        public string Zipcode { get; set; }
        public string Address { get; set; }
        public string shopcountry { get; set; }
        public string shopState { get; set; }
        public string shopcity { get; set; }
        public string shopState_hidden { get; set; }
        public string shopcity_hidden { get; set; }
        public string shopcountry_hidden { get; set; }
        public string shopzipcode { get; set; }
        public string shopaddress { get; set; }
        public string Linktomywebsite { get; set; }
        public string ShopUrl { get; set; }
        public long Salesid { get; set; }
        public string ShopType { get; set; }
        public string specialization { get; set; }
        public string maincategory { get; set; }
        public string ShopLanguage { get; set; }
        public int? ShopLangId { get; set; }
        public int Timezone { get; set; }
        public string currency { get; set; }
        public string HostingProvider { get; set; }
        public string HostingUserName { get; set; }
        public string HostingPassword { get; set; }
        public string Numberofemployees { get; set; }
        public long MainCatgId { get; set; }
        public string ShopTypeName { get; set; }
        public string PaymentDetails_id { get; set; }
        public string ShopLink { get; set; }
        public string salesLink { get; set; }
        public string PaymentType { get; set; }
        public string HShopUrl { get; set;}
        public double? latitude { get; set;}
        public double? longitude { get; set; }
        public string ShopDesc { get; set;}
        public string Location { get; set;}
        public string TimeSlot { get; set;}
        public string ZsrNo { get; set;}
    }
    public class ShopTypeDetails
    {
        public string ShopTypeName { get; set; }
        public long ShopTypeId { get; set; }
        public long MainCatgId { get; set; }
        public string MainCategory { get; set; }

    }
    public class PaymentGateway
    {
        public string PaymentCatgName { get; set; }
        public double quantity_or_price { get; set; }
        public string Calculation { get; set; }
        public double Percharge { get; set; }
        public double discount { get; set; }
        public string name_country { get; set; }
        public int PaymentCatgId { get; set; }
        public long PaymentDetail_Id { get; set; }
        public string Where_to_display { get; set; }
        public int country_id { get; set; }
        public long schlId { get; set; }
        public string currency { get; set; }
        public double LegalAmount { get; set; }
        public double TotalAmount { get; set; }
        public double SumTotal { get; set; }
        public string status { get; set; }
    }
    public class MainCategoryDetails
    {
        public long MainCatgId { get; set; }
        public string MainCategory { get; set; }
        public long? WorkFlowId { get; set; }
    }
    public class CheckShopUrl
    {
        public string AzureWebsite { get; set; }
    }
    public class ShopJsonModel
    {
        public string ReNotification { get; set; }
        public string DemoShop { get; set; }
        public long SalesUserId { get; set; }
    }
    public class LocationStatus
    {
        public string name { get; set; }
        public long LocId { get; set; }
    }
    public class Timeslot
    {
        public string CatgDesc { get; set;}
    }
}