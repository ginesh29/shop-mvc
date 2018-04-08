using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class EmployeeLeaves
    {
        public long userid { get; set; }
        public long schid { get; set; }
        public long roleid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string holidaydesc { get; set; }
        public string calendarname { get; set; }
        public string DateConcat { get; set; }
    }
    public class INVOICE_MASTER_LOCAL
    {
        public long CustomerId { get; set; }
        public long EmployeeId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public long SchlId { get; set; }
        public int? ReservationId { get; set; }
        public long? Product_Id { get; set; }
        public string ProductName { get; set; }
        public double? Amount { get; set; }
        public int? Duration { get; set; }
        public Double? VAT { get; set; }
        public string BookingDate { get; set; }
        public string Invoice_Service { get; set; }
        public string ZSR_No { get; set; }
        public DateTime DOB { get; set; }
        public int? Pincode { get; set; }
    }
    public class twodMatrix
    {
        public DateTime invoiceDate { get; set; }
        public string ZSRNO { get; set; }
        public DateTime? CustBdate { get; set; }
        public long? custZip { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? FirstTreatment { get; set; }
        public long InvoiceId { get; set; }
        public string ESRCODE { get; set; }
    }
}