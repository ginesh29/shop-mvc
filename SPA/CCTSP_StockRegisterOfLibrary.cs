//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SPA
{
    using System;
    using System.Collections.Generic;
    
    public partial class CCTSP_StockRegisterOfLibrary
    {
        public long AccessionNo { get; set; }
        public long MediaType { get; set; }
        public Nullable<long> FacilityNo { get; set; }
        public long SchId { get; set; }
        public string NameOfBooks { get; set; }
        public Nullable<long> Edition { get; set; }
        public Nullable<int> NoOfPagesInBook { get; set; }
        public string NameOfPublisher { get; set; }
        public string NameOfAuthor { get; set; }
        public int copyNo { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> actualPrice { get; set; }
        public Nullable<long> SupplierId { get; set; }
        public Nullable<System.DateTime> BookReciptDate { get; set; }
        public long BookStatus { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> SubjectCatgId { get; set; }
        public Nullable<long> classCatgId { get; set; }
        public string BookLibraryStatus { get; set; }
        public Nullable<decimal> FineAmount { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string SchoolAcessionNumber { get; set; }
    }
}
