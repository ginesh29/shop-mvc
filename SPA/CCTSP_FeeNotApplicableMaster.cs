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
    
    public partial class CCTSP_FeeNotApplicableMaster
    {
        public long FeeDiscountRuleId { get; set; }
        public long FeeHead { get; set; }
        public Nullable<long> StudentId { get; set; }
        public Nullable<int> PerDisAmt { get; set; }
        public Nullable<int> FixedAmt { get; set; }
        public Nullable<decimal> AmountPayable { get; set; }
        public long GroupId { get; set; }
        public long CatgId { get; set; }
        public string MasterFlag { get; set; }
        public long FeeType { get; set; }
        public long AcdYear { get; set; }
        public long SchId { get; set; }
        public string ActiveStatus { get; set; }
        public System.DateTime CreatedOn { get; set; }
    }
}
