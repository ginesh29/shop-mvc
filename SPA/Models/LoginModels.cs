using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace SPA.Models
{
    public class LoginModels
    {

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        public string password { get; set; }
        public bool RememberMe { get; set; }
        public int UserId { get; set; }

    }
    public class  CookieModel
    {
        public long UserId { get; set; }
        public int SchoolId { get; set; }
        public long RoleId { get; set; }
        public string UserEmailId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserGender { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}