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
    
    public partial class spatime_zone
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public spatime_zone()
        {
            this.CCTSP_SchoolMaster = new HashSet<CCTSP_SchoolMaster>();
            this.spa_BillingMaster = new HashSet<spa_BillingMaster>();
            this.spa_Payment_plan = new HashSet<spa_Payment_plan>();
            this.spa_PaymentDeduction = new HashSet<spa_PaymentDeduction>();
        }
    
        public int timezone_id { get; set; }
        public string name_timezone { get; set; }
        public string name_country { get; set; }
        public System.DateTime created_date { get; set; }
        public string ActiveStatus { get; set; }
        public string currency { get; set; }
        public Nullable<int> geonameid { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CCTSP_SchoolMaster> CCTSP_SchoolMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<spa_BillingMaster> spa_BillingMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<spa_Payment_plan> spa_Payment_plan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<spa_PaymentDeduction> spa_PaymentDeduction { get; set; }
    }
}
