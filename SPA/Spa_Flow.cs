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
    
    public partial class Spa_Flow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Spa_Flow()
        {
            this.Spa_Assign_FieldDetails = new HashSet<Spa_Assign_FieldDetails>();
            this.Spa_FlowDetails = new HashSet<Spa_FlowDetails>();
        }
    
        public long Flow_Id { get; set; }
        public string Flow_Name { get; set; }
        public string ActiveStatus { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string Extra_1 { get; set; }
        public string Extra_2 { get; set; }
        public string Extra_3 { get; set; }
        public string Extra_4 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spa_Assign_FieldDetails> Spa_Assign_FieldDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spa_FlowDetails> Spa_FlowDetails { get; set; }
    }
}
