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
    
    public partial class CCTSP_ClassSection
    {
        public long ClassSecId { get; set; }
        public long SchId { get; set; }
        public long ClassCatId { get; set; }
        public long SecCatId { get; set; }
        public Nullable<long> FacilityCatgId { get; set; }
        public Nullable<long> Capacity { get; set; }
        public Nullable<long> OccupiedCapacity { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<int> WeeklyMaxPeriod { get; set; }
        public Nullable<long> FacilityCategory { get; set; }
        public Nullable<long> Location { get; set; }
        public string Remarks { get; set; }
    }
}