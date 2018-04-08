using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class DoctorPagesDisplay
    {
        public long? CatgTypeId { get; set; }
        public long? CatgId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string CatgDesc { get; set; }        
        public string EmpFirstName { get; set; }
        public string EmpLastName { get; set; }
        public int? EmpSchDetailsId { get; set; }
        public long? UserId { get; set; }
        public long? ClientUserId { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string Address { get; set;}
        public string EmailId { get; set; }
        public string comment { get; set; }
        public string ShopImg { get; set; }
        public int? Type { get; set; }
        public int? Age { get; set; }
        public string Medicine { get; set; }
        public string HowManyTimes { get; set; }
        public string When { get; set; }
        public string FreeText { get; set; }
        public string status { get; set; }
        public string Product_Price { get; set; }
        public string Productname { get; set; }
        public  int? Lang_id { get; set; }
        public string Title { get; set; }
        public long? SectionCatgId { get; set; }
        public string SectionName  { get; set; }
        public string EnglishSection { get; set; }
        public string SectionType { get; set; }
        public string BookingDate { get; set; }
        public long? flg { get; set; }
        public int? SchlStudentStrength { get; set;}
        public DateTime? Bookings { get; set; }
        public string PhoneNo { get; set;}
        public string ActiveStatus { get; set; }
    }
    public class getHistoryNotes
    {
        public string BookingDate { get; set; }
        public int? EmpSchDetailsId { get; set; }
        public long schId { get; set; }
        public long Prescription_Id { get; set; }
        public DateTime? Bookings { get; set; }
    }
    public class GetNoteHistoryDetails
    {
        public long? CatgId { get; set; }
        public string CatgDesc { get; set;}
        public string BookingDate { get; set;}
        public DateTime? Bookings { get; set; }
        public long? SectionCatgId { get; set; }
        public int? ReservationId { get; set; }
    }
}