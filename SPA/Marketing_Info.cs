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
    
    public partial class Marketing_Info
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Marketing_Info()
        {
            this.Platform_Marketing_SendHistory = new HashSet<Platform_Marketing_SendHistory>();
        }
    
        public long MInfo_id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.DateTime> created_on { get; set; }
        public Nullable<System.DateTime> Updated_on { get; set; }
        public string Status { get; set; }
        public Nullable<int> DStatus { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        public Nullable<long> Extra3 { get; set; }
        public Nullable<long> Extra4 { get; set; }
        public Nullable<long> Lang_id { get; set; }
        public string From_EmailId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Platform_Marketing_SendHistory> Platform_Marketing_SendHistory { get; set; }
    }
}