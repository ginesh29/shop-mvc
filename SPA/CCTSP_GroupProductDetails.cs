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
    
    public partial class CCTSP_GroupProductDetails
    {
        public long GroupProductDetailsId { get; set; }
        public Nullable<long> GroupIdDefault { get; set; }
        public Nullable<long> GroupIdByShop { get; set; }
        public long ProductId { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<long> schlId { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public Nullable<System.DateTime> Update_Date { get; set; }
        public Nullable<int> Attributeorder { get; set; }
    
        public virtual CCTSP_CategoryDetails CCTSP_CategoryDetails { get; set; }
        public virtual CCTSP_CategoryDetails CCTSP_CategoryDetails1 { get; set; }
        public virtual CCTSP_CategoryDetails CCTSP_CategoryDetails2 { get; set; }
        public virtual CCTSP_SchoolMaster CCTSP_SchoolMaster { get; set; }
    }
}
