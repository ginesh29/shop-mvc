using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class FreeCustomer
    {
        public string phoneno { get; set; }
        public long? ShopId { get; set; }
        public long? ShopTypeId { get; set; }
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
        public int? Zipcode { get; set; }
        public string Address { get; set; }
        public string shopcountry { get; set; }
        public string shopState { get; set; }
        public string shopcity { get; set; }
        public string OwnerGender { get; set; }
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
        public string HShopUrl { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string ShopDesc { get; set; }
        public string Location { get; set; }
        public string EmpFName { get; set; }
        public string EmpLName { get; set; }
        public string EmpGender { get; set; }
        public string EmpNumber { get; set; }
        public string EmpEmail { get; set; }
        public string EmpCharge { get; set; }
        public string EmpSkypeId { get; set; }
        public string EmpDegree { get; set; }
        public int? EmpExpMonth { get; set; }
        public int? EmpExpYear { get; set; }
        public long? HSpecialization_1 { get; set; }
        public long? HSpecialization_2 { get; set; }
        public long? HSpecialization_3 { get; set; }
        public long? HSpecialization_4 { get; set; }
        public string LendingObject { get; set; }
        public string DoctorStatus { get; set; }
        public string EmpJson { get; set;}
    }
    public class EditFreeCustomer
    {
        public string phoneno { get; set; }
        public long? ShopId { get; set; }
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
        public int? Zipcode { get; set; }
        public string Address { get; set; }
        public string shopcountry { get; set; }
        public string OwnerGender { get; set; }
        public string shopState { get; set; }
        public string shopcity { get; set; }
        public string shopState_hidden { get; set; }
        public string shopcity_hidden { get; set; }
        public string shopcountry_hidden { get; set; }
        public string shopzipcode { get; set; }
        public string shopaddress { get; set; }
        public string ShopType { get; set; }
        public long? ShopTypeId { get; set; }
        public string maincategory { get; set; }
        public long MainCatgId { get; set; }
        public string ShopTypeName { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string ShopDesc { get; set; }
        public string Location { get; set; }
        public string LendingObject { get; set; }
        public int? DoctorFlowStatus { get; set; }
        public string specialization { get; set; }
        public List<long> EmpUserId { get; set; }
        public List<string> EmpFName { get; set; }
        public List<string> EmpLName { get; set; }
        public List<string> EmpGender { get; set; }
        public List<string> EmpNumber { get; set; }
        public List<string> EmpEmail { get; set; }
        public List<string> EmpCharge { get; set; }
        public List<string> EmpSkypeId { get; set; }
        public List<string> EmpDegree { get; set; }
        public List<int> EmpExpMonth { get; set; }
        public List<int> EmpExpYear { get; set; }
        public List<long> HSpecialization_1 { get; set; }
        public List<long> HSpecialization_2 { get; set; }
        public List<long> HSpecialization_3 { get; set; }
        public List<long> HSpecialization_4 { get; set; }
        public List<HttpPostedFileBase> EmpImage_Name { get; set; }
    }
    public class EmployeeJsonModel
    {
        public long EmpId { get; set;}
        public string EmpFName { get; set;}
        public string EmpImage_Name { get; set;}
        public string Hidden_EmpImage { get; set; }
        public string EmpImage { get; set;}
        public string EmpLName { get; set; }
        public string EmpGender { get; set; }
        public string EmpNumber { get; set; }
        public string EmpEmail { get; set; }
        public string EmpCharge { get; set; }
        public string EmpSkypeId { get; set; }
        public string EmpDegree { get; set; }
        public int? EmpExpMonth { get; set; }
        public int? EmpExpYear { get; set; }
        public long? HSpecialization_1 { get; set; }
        public long? HSpecialization_2 { get; set; }
        public long? HSpecialization_3 { get; set; }
        public long? HSpecialization_4 { get; set; }
    }
    public class FreeCustomerList
    {
        public long ShopId { get; set;}
        public string ShopName { get; set; }
        public string ShopCity { get; set; }
        public string OwnerName { get; set; }
        public string OwnerLName { get; set; }
        public string MainCategory { get; set; }
        public string ShopType { get; set; }
    }
    public class FreecustomerImage
    {
        public long EmpId { get; set; }
        public string EmpImage { get; set; }
    }
    public class FreeCustomerEmployeeInfo
    {
        public long EmpUserId { get; set; }
        public string specialization { get; set; }
        public string EmpFName { get; set; }
        public string EmpLName { get; set; }
        public string EmpGender { get; set; }
        public string EmpNumber { get; set; }
        public string EmpEmail { get; set; }
        public string EmpCharge { get; set; }
        public string EmpSkypeId { get; set; }
        public string EmpDegree { get; set; }
        public int? EmpExpMonth { get; set; }
        public int? EmpExpYear { get; set; }
        public long? HSpecialization_1 { get; set; }
        public long? HSpecialization_2 { get; set; }
        public long? HSpecialization_3 { get; set; }
        public long? HSpecialization_4 { get; set; }
        public string EmpImage_Name { get; set; }
        public string Hidden_EmpImage { get; set; }
        public string EmpImage { get; set; }
        public string BaseUrl { get; set;}
    }
}