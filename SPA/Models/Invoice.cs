using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class Invoice
    {
        public int ReservationId { get; set; }
        public string CustomerFName { get; set; }
        public string CustomerLName { get; set; }
        public string ProductName { get; set; }
        public string CustomerEmail { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string BookingDate { get; set; }
        public int? Duration { get; set; }
        public decimal? ProductPrice { get; set; }
        public double? Amount { get; set; }
        public long? CustomerId { get; set; }
        public long? EmployeeId { get; set; }
        public string Currency { get; set; }
        public long? Invoice_Id { get; set; }
        public string Invoice_Service { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CountReservation { get; set; }
        public DateTime? INVOICINGDATE { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? overdue { get; set; }
        public long Index { get; set; }
        public DateTime? Paid_Date { get; set; }
        public DateTime? Reminderdate1 { get; set; }
        public DateTime? Reminderdate2 { get; set; }
        public DateTime? Reminderdate3 { get; set; }
        public bool DisplayStatus { get; set; }
        public double? Remaining_Amount { get; set; }
        public string AccessToData { get; set;}
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set; }
    }
    public class ManualInvoice
    {
        public string Cust_FirstName { get; set; }
        public string Cust_LastName { get; set; }
        public string Cust_Street { get; set; }
        public int? Cust_Pincode { get; set; }
        public string Cust_City { get; set; }
        public DateTime? DOB { get; set; }
        public string Cust_Gender { get; set; }
        public string Cust_Title { get; set; }
        public string Short_Cust_Title { get; set; }
        public string Cust_AHV_Number { get; set; }
        public string Cust_VEKA_Number { get; set; }
        public string Cust_InsuranceNumber { get; set; }
        public string Cust_State { get; set; }
        public string Cust_PhoneNo { get; set; }
        public string Cust_GLN_No { get; set; }
        public string Cust_ZSR_No { get; set; }
        public string CustEmail { get; set; }
        public string CustAddress { get; set; }
        public string Own_GLN_No { get; set; }
        public string Own_ZSR_No { get; set; }
        public string Own_Fname { get; set; }
        public string Own_Lname { get; set; }
        public string OwnAddress { get; set; }
        public string OwnEmail { get; set; }
        public string OwnPhoneno { get; set; }
        public string SchlName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopCity { get; set; }
        public string ShopZipcode { get; set; }
        public string ShopCountry { get; set; }
        public string TimeZone { get; set; }
        public string ShopState { get; set; }
        public string ShopImage { get; set; }
        public string ShopStreetNumber { get; set; }
        public string Shopstreet { get; set; }
        public string Original_Website { get; set; }
        public string BookingDate { get; set; }
        public string ProductPrice { get; set; }
        public DateTime? Casedate { get; set; }
        public long? PatientId { get; set; }
        public string Kind_of_reimbursement { get; set; }
        public string Law { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public long? InvoiceNo { get; set; }
        public DateTime? InvoiceDateAndTime { get; set; }
        public long? Reminder_No { get; set; }
        public DateTime? Reminder_DateAndTime { get; set; }
        public string Treatment { get; set; }
        public string ContractNo { get; set; }
        public string Reason_For_Treatment { get; set; }
        public string Invoice_Referer { get; set; }
        public string Invoice_Referer_Name { get; set; }
        public string Invoice_Gln { get; set; }
        public string Invoice_Zsr { get; set; }
        public string Invoice_Diagnosis { get; set; }
        public string Therapie { get; set; }
        public string Invoice_Remarks { get; set; }
        public string ReservationIdList { get; set; }
        public string EmpFName { get; set; }
        public string EmpLName { get; set; }
        public string Emp_GLN_No { get; set; }
        public string Emp_ZSR_No { get; set; }
        public string EmpPhoneNo { get; set; }
        public string EmpEmail { get; set; }
        public string EmpAddress { get; set; }
        public long? EmpId { get; set; }
        public long? InvoiceId { get; set; }
        public DateTime? TreatFromdate { get; set; }
        public DateTime? TreatTodate { get; set; }
        public string Value { get; set; }
        public string CatgDetail { get; set; }
        public string CategoryName { get; set; }
        public int? ReservationId { get; set; }
        public long? Assign_HealthInsurance_Id { get; set; }
        public int? Duration { get; set; }
        public double? Quantity { get; set; }
        public double? Rate { get; set; }
        public double? RealAmount { get; set; }
        public List<int> PReservationid { get; set; }
        public List<int> PDuaration { get; set; }
        public List<int> SettlementTextDuaration { get; set; }
        public List<string> ISettlementText { get; set; }
        public List<long> AddOnProductId { get; set; }
        public List<long> AddOnSettlementTextId { get; set; }
        public List<double> AddOnQuantity { get; set; }
        public List<DateTime> AddOnDate { get; set; }
        public List<DateTime> SettlementTextDate { get; set; }
        public List<TimeSpan> AddOnTime { get; set; }
        public string ProductName { get; set; }
        public string Currency { get; set; }
        public double? PrintQuantity { get; set; }
        public string PrintProductName { get; set; }
        public double? PrintRate { get; set; }
        public double? PrintTotalPrice { get; set; }
        public int? PrintDuration { get; set; }
        public long? AddOnPId { get; set; }
        public DateTime? EAddOnDate { get; set; }
        public TimeSpan? EAddOnTime { get; set; }
        public long? Invoice_Detail_Id { get; set; }
        public int? Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public int? Tarif_Number { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string Manufacturer { get; set; }
        public string Dozes { get; set; }
        public string Image_barcode { get; set; }
        public decimal Vat { get; set; }
        public int? Lang_id { get; set; }
        public string Cust_StreetNumber { get; set; }
        public string Cust_Country { get; set; }
        public string UrlStatus { get; set; }
        public string LongNumber { get; set; }
        public string InvoiceStatus { get; set; }
        public string txtAreaGInvoice { get; set; }
        public double? TotalPricewithvat { get; set; }
        public string Emp_Title { get; set; }
        public string Emp_Street { get; set; }
        public string Emp_StreetNumber { get; set; }
        public int? Emp_Pincode { get; set; }
        public string Emp_City { get; set; }
        public int? Cust_Display_Invoice { get; set; }
        public string Invoice_Service { get; set; }
        public DateTime? CurrentDate { get; set; }
        public decimal? Print_Vat { get; set; }
        public int? DisplayInvoice { get; set; }
        public string Invoice_Settlement_text { get; set; }
        public int? Invoice_Settlement_Number { get; set; }
        public int? Invoice_Tarif_Number { get; set;}
    }
    public class ValueList
    {
        public long Id { get; set; }
        public double value { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public bool isReservation { get; set; } = false;
        public string SettlementText { get; set;}
    }
    public class getDataforProduct
    {
        public long? Add_On_Pid { get; set; }
        public int? ReservationId { get; set; }
        public long? Product_Id { get; set; }
        public string ProductName { get; set; }
        public double? Amount { get; set; }
        public int? Duration { get; set; }
        public string Manufacturer { get; set; }
        public string Dozes { get; set; }
        public string BookingDate { get; set; }
        public double? VAT { get; set; }
        public int? Tarif_Number { get; set; }
        public int? Settlement_Number { get; set;}
        public string Settlement_text { get; set;}
        public string Flag { get; set;}
        public string DefaultText { get; set; }
    }
    public class AddOnProductList
    {
        public long AddOnProductId { get; set; }
        public string ProductName { get; set; }
        public double? Quantity { get; set; }
        public double? Selling_Price { get; set; }
        public double? Amount { get; set; }
        public int? Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public int? Tarif_Number { get; set; }
        public string Manufacturer { get; set; }
        public string Dozes { get; set; }
        public double? vat { get; set; }
    }
    public class AddOnProductInvoiceAddList
    {
        public List<long> IAddOnProductId { get; set; }
        public List<int> PReservationid { get; set; }
        public List<string> ProductName { get; set; }
        public List<int> Tarif_Number { get; set; }
        public List<string> Settlement_Number { get; set; }
        public List<string> Settlement_text { get; set; }
        public List<double> Add_OnQuantity { get; set; }
        public List<double> Rate { get; set; }
        public List<DateTime> Add_OnDate { get; set; }
        public List<TimeSpan> Add_OnTime { get; set; }
        public List<int> Duration { get; set; }
        public List<Double> vat { get; set; }
        public List<long> EmpId { get; set; }
        public List<long> CustomerId { get; set; }
        public List<DateTime> InvoiceDate { get; set; }
    }
    public class DisplayAddOnProductInvoiceAddList
    {
        public long? AddOnProductId { get; set; }
        public long? PReservationid { get; set; }
        public long? AddOnSettlementTextId { get; set; }
        public string ProductName { get; set; }
        public double Add_OnQuantity { get; set; }
        public double Rate { get; set; }
        public DateTime Add_OnDate { get; set; }
        public TimeSpan Add_OnTime { get; set; }
        public int? Duration { get; set; }
        public string Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public int? Tarif_Number { get; set; }
        public double? vat { get; set; }
        public long? EmpId { get; set; }
        public long? CustomerId { get; set; }
    }
    public class InvoiceCustomerDetails
    {
        public int? Lang_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZone { get; set; }
        public string EmailId { get; set; }
        public long? Invoice_Id { get; set; }
        public long? UserId { get; set; }
        public string status { get; set; }
        public string PhoneNo { get; set;}
    }
    public class InvoiceReservationList
    {
        public int? ReservtionId { get; set; }
        public string BookingDate { get; set; }
        public double? Amount { get; set; }
        public string ProductName { get; set; }
        public long? Emp_UserId { get; set; }
        public long? CLIENT_UserId { get; set; }
        public int? Duration { get; set; }
        public double? Quantity { get; set; }
        public double? Rate { get; set; }
        public int? Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public int? Tarif_Number { get; set; }
        public string Currency { get; set; }
        public decimal? Vat { get; set; }
    }
    public class AmountList
    {
        public double Amount { get; set; }
        public long invoice_DetailId { get; set; }
    }
    public class CatgDropDownList
    {
        public string CatgType { get; set; }
        public string CatgDesc { get; set; }
        public long? CatgId { get; set; }
        public long? CatgTypeId { get; set; }
        public string CatgName { get; set; }
        public string Vat { get; set; }
        public string TotalAmount { get; set; }
        public string Currency { get; set; }
        public int? OverDue { get; set; }
        public long? Group_OrderData { get; set; }
        public long Invoice_Id { get; set; }
        public string ImageBarcode { get; set; }
        public string TreatFromDate { get; set; }
        public string TreatToDate { get; set; }
        public string Ibanno { get; set; }
        public string AccountNumber { get; set; }
    }

    public class UserDetails
    {
        public long? UserId { get; set; }
        public long? SchId { get; set; }
        public long? RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PasswordQuerry2 { get; set; }
        public string Answer2 { get; set; }
        public DateTime? DOB { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public string Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? Pincode { get; set; }
        public string BaseUrl { get; set; }
        public string Salutation { get; set; }
        public string ETitle { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string GLN_No { get; set; }
        public string ZSR_No { get; set; }
        public string AHV_Number { get; set; }
        public string VEKA_Number { get; set; }
        public string InsuranceNumber { get; set; }
    }
    public class InvoiceServicesList
    {
        public long? ProductId { get; set; }
        public long? EmpId { get; set; }
        public string ProductName { get; set; }
        public int? Duration { get; set; }
        public decimal? Amount { get; set; }
        public double? Quantity { get; set; }
        public double? Rate { get; set; }
        public int? Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public int? Tarif_Number { get; set; }
        public decimal? Vat { get; set; }
        public double? RealAmount { get; set; }
    }
    public class SelectedCustomerEmployeeId
    {
        public long? SelectedSearchMIEmployee { get; set; }
        public long? SelectedSearchMICustomer { get; set; }
    }
    public class InvoiceReport
    {
        public string ShopName { get; set; }
        public string ShopUrl { get; set; }
        public string ShopWebsite { get; set; }
        public string ShopAddress { get; set; }
        public string ShopCity { get; set; }
        public string ShopState { get; set; }
        public string ShopZipcode { get; set; }
        public string Currency { get; set; }
        public string ShopCountry { get; set; }
        public string TimeZone { get; set; }
        public DateTime? BookingDate { get; set; }
        public long? ProductId { get; set; }
        public decimal? Amount { get; set; }
        public long? ReservationId { get; set; }
        public string Title { get; set; }
        public string Owner_FName { get; set; }
        public string Owner_LName { get; set; }
        public string Month_Name { get; set; }
        public string ShopImage { get; set; }
        public string ServiceName { get; set; }
        public string ShopStreet { get; set; }
        public int? ShopStreetNumber { get; set; }
        public double? Monthly_Amount { get; set; }
        public int? Number_Of_Service { get; set; }
        public int? Addon { get; set; }
        public decimal? VAT { get; set; }
        public decimal? TotalWithVAT { get; set; }
        public double? qty { get; set; }
        public int? year { get; set; }
        public string MonthName { get; set; }
        public int? Months { get; set; }
        public string Invoice_SettlementText { get; set;}
        public int? Invoice_SettleMentNumber { get; set;}
        public int? Invoice_TarifNumber { get; set; }
    }
    public class AbstractInvoiceReport
    {
        public string Month { get; set; }
        public int? year { get; set; }
        public decimal? Price { get; set; }
        public DateTime? dates { get; set; }
        public string Culture { get; set; }
        public DateTime? MINDATE { get; set; }
        public DateTime? MAXDATE { get; set; }
        public string currency { get; set; }
    }
    public class PartialPaymentList
    {
        public long? Invoice_Id { get; set; }
        public double? Total_Amount { get; set; }
        public double? Remaining_Amount { get; set; }
        public double? TotalPrice { get; set; }
        public long? Invoice_Payment_id { get; set; }
        public DateTime? Pay_Date { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Payment_Catg { get; set; }
        public double? Discount_Amount { get; set; }
        public double? Pay_Amount { get; set; }
    }
    public class AddpartialPayment
    {
        public long? Invoice_Id { get; set; }
        public double? Remaining_Amount { get; set; }
        public double? Pay_Amount { get; set; }
        public double? Discount { get; set; }
        public string Pay_Date { get; set; }
        public double? Total_Amount { get; set; }
        public string UrlStatus { get; set; }
        public string Invoice_Service { get; set; }
    }
    public class BasicInfo_PartialPopup
    {
        public string LngLocal { get; set; }
        public string UrlStatus { get; set; }
        public string Invoice_Service { get; set; }
        public DateTime? ShopDate { get; set; }
        public long? Invoice_Id { get; set; }
        public string Msg { get; set; }
        public int? FullPayment { get; set; }
    }
    public class PaymentNtryDetails
    {
        public string EndToEndId { get; set; }
        public string Amt { get; set;}
        public string Ref { get; set; }
    }
    public class PaymentNtry
    {
        public string Amt { get; set; }
        public string BookgDt { get; set;}
        public string Sts { get; set;}
        public List<Models.PaymentNtryDetails> NtryDetails { get; set;}
    }
    public class dropdownfilter
    {
        public int? id { get; set;}
        public string filterName { get; set; }
    }
}