using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class PaySection
    {
        public long PaymentDetail_Id { get; set; }
        public int PaymentCatgId { get; set; }
        public string PaymentCatgName { get; set; }
        public string SchCountry { get; set; }
        public double Amount { get; set; }
        public double specificshop { get; set; }
        public double Duration { get; set; }
        public string PaymentType { get; set; }
        public int country_id { get; set; }
        public string CatPayment { get; set; }
        public string Currency { get; set; }
        public long schlId { get; set; }
        public string status { get; set; }
        public double? SumTotal { get; set; }
    }
    public class RemainingShopCredit
    {
        public double? TotalReservation { get; set; }
        public double? RemainingDays { get; set; }
    }
    public class DebitPayment
    {
        public string CatgId { get; set; }
        public double Amount { get; set; }
        public long shopid { get; set; }
        public string User { get; set; }
        public string ListofCatgId { get; set; }
        public double TotalAmount { get; set; }
        public double credit { get; set; }
        public double TotalCredit { get; set; }
        public double AmountUser { get; set; }
        public double CreditUser { get; set; }
        public string CatPayment { get; set; }

    }
    public class BillingHistory
    {
        public double? Credit { get; set; }
        public double? Amount { get; set; }
        public string ProductName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime created_on { get; set; }
        public string PaymentCatgName { get; set; }
        public int? ReservationId { get; set; }
        public int status { get; set; }
    }
    public class Pay
    {
        public string key { get; set; }
        public string txnid { get; set; }
        public string amount { get; set; }
        public string productinfo { get; set; }
        [Required(ErrorMessage = "First Name required")]
        public string firstname { get; set; }
        public string lastname { get; set; }
        [Required(ErrorMessage = "Email Id is required")]
        public string email { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        public string phone { get; set; }
        public string surl { get; set; }
        public string furl { get; set; }
        public string hash { get; set; }
        public string service_provider { get; set; }
        public List<string> udf1 { get; set; }
        public int? online { get; set; }
        public int Lang_id { get; set; }
        public long? payId { get; set; }
    }
    public class PaymentHistory
    {
        public long ShopId { get; set; }
        public string Monthd { get; set; }
        public int Months { get; set; }
        public int years { get; set; }
        public string Currency { get; set; }
        public double CreditRemaining { get; set; }
        public string MonthNames { get; set; }
    }
}