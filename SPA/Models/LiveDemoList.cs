using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class LiveDemoList
    {
        [Required]
        public string first_name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$")]
        public string mobilenumber { get; set; }
        [Required]
        public string Extra1 { get; set; }
        [Required]
        public string Extra2 { get; set; }
        [Required]
        public string ShopType { get; set; }
    }
}