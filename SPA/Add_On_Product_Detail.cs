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
    
    public partial class Add_On_Product_Detail
    {
        public long Pdetail_Id { get; set; }
        public long Add_On_Pid { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<double> Quantity { get; set; }
        public Nullable<double> Buying_Price { get; set; }
        public Nullable<double> Selling_Price { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public Nullable<System.DateTime> updated_Date { get; set; }
        public string Created_By { get; set; }
        public long SchlId { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
    
        public virtual Add_on_Product_Master Add_on_Product_Master { get; set; }
        public virtual CCTSP_SchoolMaster CCTSP_SchoolMaster { get; set; }
    }
}