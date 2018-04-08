using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class shopMaster
    {
        public string MAINCATEGORY { get; set; }
        public string SHOPNAME { get; set; }
        public string TITLE { get; set; }
        public string OWNERNAME { get; set; }
        public string OWNERSURNAME { get; set; }
        public string MOBILENUMBER { get; set; }
        public string CITY { get; set; }
        public string ZIPCODE { get; set; }
        public string ADDRESS { get; set; }
        public string SHOPTYPE { get; set; }
        public string CUSTOMERNO { get; set; }
        public string ImageLogo { get; set; }
        public string BANKNAME { get; set; }
        public string BANKACCOUNT { get; set; }
        public string IBANNO { get; set; }
        public string ShopEmail { get; set; }
        public string OwnerEmail { get; set; }
        public string ShopWebsite { get; set; }
        public string ShopDesc { get; set; }
        public long ShopId { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string Location { get; set; }
        public int? LangId { get; set; }
        public string GENDER { get; set; }
        public string SALUTATION { get; set; }
        public string LANDLINENUMBER { get; set; }
        public string State { get; set; }
        public string VAT { get; set; }
        public string VATNUMBER { get; set; }
        public string GLN_NO { get; set; }
        public string ZSR_NO { get; set; }
        public string Currency { get; set; }
        public long? MainCatgId { get; set; }
        public int? SchlStudentStrength { get; set; }
        public int? PaymentDuration { get; set; }
        public int? ExtendDuration { get; set; }
        public string streetNumber { get; set; }
        public string street { get; set; }
        public int? Display_Invoice { get; set; }
        public long? Flow_Id { get; set; }
        public string InvoiceFreeText { get; set; }
        public string Country { get; set;}
        public string InvoiceEmailTxt { get; set;}
        public bool Allow_PastDate { get; set; }
        public string TimeZone { get; set; }
}
    public class ShopClickandgo
    {
        public int LangId { get; set; }
        public bool status { get; set; }
        public string schId { get; set; }
        public int stat { get; set; }
        public string Website { get; set; }
        public string WorkFlowStatus { get; set; }
    }
    public class ShopMasterDetail
    {
        public Nullable<int> Lang_id { get; set; }
        public Nullable<long> Color_Id { get; set; }
        public string SchlWebsite { get; set; }
        public string TimeZone { get; set; }
        public Nullable<int> timezone_id { get; set; }
        public Nullable<int> SchlStudentStrength { get; set; }
        public string ShopName { get; set; }
        public string Shopimg1 { get; set; }
        public string Shopimg2 { get; set; }
        public string Shopimg3 { get; set; }
        public string Shopimg4 { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string Email_id { get; set; }
        public string PhoneNo { get; set; }
        public string ShopCity { get; set; }
        public string Shop_Address { get; set; }
        public string Shop_ZipCode { get; set; }
        public string Shop_Email_id { get; set; }
        public string Shop_PhoneNo { get; set; }
        public string Shop_ShopCity { get; set; }
        public string country { get; set; }
        public string Logo { get; set; }
        public string Logotext_Color { get; set; }
        public string Logotext { get; set; }
        public string colorclass { get; set; }
        public string Shopwebsite { get; set; }
        public string Currency { get; set; }
        public int? ShowPrice { get; set; }
        public string DisplayMarketing { get; set; }
        public long? Schlid { get; set; }
        public int? OverDue { get; set; }
        public int? Display_FreeCustomer { get; set; }
        public int? Display_Invoice { get; set; }
        public string IbanNo { get; set; }
        public string AccountNumber { get; set; }
        public string BookingApproval { get; set; }
        public int? ExtendDuration { get; set; }
        public int? PaymentDuration { get; set; }
        public int? CancelResvHours { get; set; }
        public string JsonModel { get; set; }
        public string Invoice_FreeText { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? Flow_Id { get; set;}
        public bool AllowPastDate { get; set; }
    }
    public class variousShopDetails
    {
        public string jsonModel { get; set; }
        public int? Lang_id { get; set; }
        public string send_sms { get; set; }
        public int? AlertRemainder { get; set; }
        public int? AdvBookingRestrict { get; set; }
        public long? Color_Id { get; set; }
        public int? Cancel_Res_Duration { get; set; }
        public string Invoice_FreeText { get; set; }
        public string BookingApproval { get; set; }
        public int? ShowPrice { get; set; }
        public string ReNotification { get; set; }
        public int? Display_Invoice { get; set; }
    }
}