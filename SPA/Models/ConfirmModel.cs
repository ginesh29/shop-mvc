using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class ConfirmModel
    {
        public int EmpSchDetailsId { get; set; }
        public string BookingDate { get; set; }
        public string ProductName { get; set; }
        public string Productdesc { get; set; }
        public string FromSlotMasterId { get; set; }
        public string ToSlotMasterId { get; set; }
        public int Duration { get; set; }
        public decimal Amount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public string ActiveStatus { get; set; }
        public string BookedStatus { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhoneNo { get; set; }
        public string ShopOwnerName { get; set; }
        public string ShopOwnerLastName { get; set; }
        public string CreatedDate { get; set; }
        public string Product_price { get; set; }
        public string comment { get; set; }
        public long? CLIENT_UserId { get; set; }
        public long? EMP_UserId { get; set; }
        public long? ProductId { get; set; }
        public string EmailId { get; set; }
        public string Reg_Status { get; set; }
        public string AccessToData { get; set; }
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public string FlowStatus { get; set;}
        public long? Invoice_id { get; set;}
        public string Invoice_Status { get; set;}
    }
    public class ReservationInfo
    {
        public DateTime? BookedDate { get; set; }
        public string ProductName { get; set; }
        public string Productdesc { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string EmpFName { get; set; }
        public string EmpLName { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientEmail { get; set; }
        public string ShopOwnerName { get; set; }
        public string ShopOwnerLastName { get; set; }
        public string ShopOwnerEmail { get; set; }
        public string ShopUrl { get; set; }
        public string ShopAddress { get; set; }
        public string ShopCity { get; set; }
        public string ShopState { get; set; }
        public string ShopPincode { get; set; }
    }
    public class CheckInvoiceStatus
    {
        public long InvoiceId { get; set; }
        public string InvoiceStatus { get; set;}
    }

  
}