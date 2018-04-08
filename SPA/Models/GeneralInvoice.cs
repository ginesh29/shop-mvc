using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class GeneralInvoice
    {
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopCity { get; set; }
        public string ShopZipcode { get; set; }
        public string Currency { get; set; }
        public string ShopCountry { get; set; }
        public string TimeZone { get; set; }
        public string ShopState { get; set; }
        public string ShopImage { get; set; }
        public DateTime? BookingDate { get; set; }
        public Decimal? TotalPrice { get; set; }
        public int? Duration { get; set; }
        public string ProductName { get; set; }
        public long? product_id { get; set; }
        public long PatientId { get; set; }
        public DateTime? DOB { get; set; }
        public string Cust_Title { get; set; }
        public string Cust_Gender { get; set; }
        public string Cust_FirstName { get; set; }
        public string Cust_LastName { get; set; }
        public string Cust_Street { get; set; }
        public string Cust_StreetNumber { get; set; }
        public int? Cust_Pincode { get; set; }
        public string Cust_City { get; set; }
        public string Cust_State { get; set; }
        public string EmpFName { get; set; }
        public long EmpId { get; set; }
        public string EmpLName { get; set; }
        public string Salutation { get; set; }
        public string StreetNumber { get; set;}
        public string street { get; set; }
        public string Original_Website { get; set; }
        public double? VAT { get; set; }
        public Decimal? TotalPricewithvat { get; set; }
        public int? ReservationId { get; set; }
        public long? Add_On_Pid { get; set; }
        public double? Quantity { get; set; }
        public string URL { get; set; }
        public long? InvoiceId { get; set; }
        public double? Rate { get; set;}
        public string txtAreaGInvoice { get; set;}
        public DateTime? InvoiceDate { get; set;}
        public string ShopIban_Number { get; set;}
        public string Emp_Title { get; set;}
        public int? DueDays { get; set;}
        public long? Invoice_Detail_Id { get; set; }
        public string Invoice_Service { get; set; }
    }
}