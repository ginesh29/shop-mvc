using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class DoctorNotes
    {
        public long UserId { get; set; }
        public long Prescription_Id { get; set; }
        public string CatgIDPatient { get; set; }
        public string FreePatient { get; set; }
        public string Temp { get; set; }
        public string pulse { get; set; }
        public string BP { get; set; }
        public string RepositoryRate { get; set; }
        public string DiagFree { get; set; }
        public string CatgIDMedication { get; set; }
        public string FreeMedication { get; set; }
        public string CatgIDInvestigation { get; set; }
        public string FreeInvestigation { get; set; }
        public HttpPostedFileBase UploadReport { get; set; }
        public int BookingId { get; set; }
        public int diff { get; set; }
        public int Medicationdiff { get; set; }
        public int Investigationdiff { get; set; }
        public List<SpecialInsertForDoctor> specialMedication { get; set; }
        public List<SpecialInsertForDoctor> AdviceList { get; set; }
        public List<string> UploadImageList { get; set; }                    
        public string FreeFollowUp { get; set; }
        public string FreeAllergies { get; set; }
        public string FreepreDiag { get; set; }
        public string FreeAdvise { get; set; }
        public string PreDiagnosisId { get; set; }
    }
    public class InvoiceStatus
    {
        public bool status { get; set; }
        public List<long> ReservationList { get; set; }
        public DateTime? InvoiceDate { get; set;}
        public string url { get; set; }
    }
   
}