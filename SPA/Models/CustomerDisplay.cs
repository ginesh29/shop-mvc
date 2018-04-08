using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class CustomerDisplay
    {
        public long UserId { get; set; }
        public string Title { get; set; }
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastVisit { get; set; }
        public string EmployeeName { get; set; }
        public string EmpImg { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string DOB { get; set; }
        public string CreatedOn { get; set; }
        public string Status { get; set; }
        public int? SMSService { get; set; }
        public int? EmailService { get; set; }
        public string InvoiceService { get; set;}
        public string landlinenumber { get; set; }
        public string Timezone { get; set; }
        public int LangId { get; set; }
        public string AccessToData { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set; }
        public string BaseUrl { get; set;}
        public string Street { get; set; }
        public string AHV_Number { get; set; }
        public string VEKA_Number { get; set; }
        public string InsuranceNumber { get; set; }
        public string Gender { get;set;}
        public string StreetNumber { get; set; }
        public string Salutation { get; set; }
        public string State { get; set; }
        public string GLN_No { get; set; }
        public string ZSR_No { get; set; }
        public string Country { get; set; }
        public int? DisplayInvoice { get; set; }
        public DateTime? CustomerDOB { get; set; }
        public int? CustPostalCode { get; set; }
        public DateTime? CusCreatedOn { get; set; }
    }
    public class UserForgetPassword
    {
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string confirmPassword { get; set; }
        public string UserIdEdit { get; set; }

    }
    public class GetCustomerSpecialization
    {
        public string specialization { get; set; }
        public string degree { get; set; }
        public int? ExperienceYear { get; set; }
        public int? ExperienceMonth { get; set; }
        public string skypeId { get; set; }
        public long? UserId { get; set; }
        public long? HSpecialization_1 { get; set; }
        public long? HSpecialization_2 { get; set; }
        public long? HSpecialization_3 { get; set; }
        public long? HSpecialization_4 { get; set; }
    }
    public class UserRoleDetails
    {
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public long RoleId { get; set;}
        public long UserId { get; set;}
    }

}



