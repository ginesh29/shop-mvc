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
    
    public partial class GYM_Member_Dept
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GYM_Member_Dept()
        {
            this.CCTSP_User = new HashSet<CCTSP_User>();
            this.CCTSP_User1 = new HashSet<CCTSP_User>();
        }
    
        public long MemberDetailsID { get; set; }
        public long PrimaryMemberUserID { get; set; }
        public string ActiveStatus { get; set; }
        public System.DateTime CreateOn { get; set; }
        public long Schid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCTSP_User> CCTSP_User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCTSP_User> CCTSP_User1 { get; set; }
    }
}
