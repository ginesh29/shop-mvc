using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ISDATE : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime dt;
            if(value != null)
            {
                if (DateTime.TryParse(Convert.ToString(value), out dt))
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Not a Date");
            }
            else
                return ValidationResult.Success;
        }
    }
    public class ImportCustomer
    {
        public long Identity { get; set; }
        public string ColumnName { get; set; }
        public string AliasName { get; set; }
        public string CustColName { get; set; }
        public string Datatype { get; set; }
        public string ISNULL { get; set; }
        public bool ISSAME { get; set; }
    }
    public class SucImportCust
    {
        public int RowNumber { get; set; }
        public bool IsValid { get; set; }
        public bool IsRepeated { get; set; }
        public bool IsOther { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DownloadExcel { get; set; }
    }
    public class combineSucImport : SucImportCust
    {
        public userTable UserData { get; set; } = new userTable();
    }

    public class userTable
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        //[Phone]
        [RegularExpression(@"^([0-9]{10,12})$")]
        public string PhoneNo { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailId { get; set; }
        [RegularExpression(@"^(\d{1,16})$")]
        public string PasswordQuerry2 { get; set; }
        [ISDATE]
        public string DOB { get; set; }
        [RegularExpression(@"^(\d{1,8})$")]
        public string Pincode { get; set; }
        public string ActiveStatus = "A";
        [MaxLength(20)]
        public string VEKA_Number { get; set; }
        public string GLN_No { get; set; }
        [StringLength(7, MinimumLength = 7)]
        public string ZSR_No { get; set; }
        [MaxLength(16)]
        public string AHV_Number { get; set; }
        [MaxLength(20)]
        public string InsuranceNumber { get; set; }
        public string street { get; set; }
        public string StreetNumber { get; set; }
        public string CITY { get; set; }
        public string State { get; set; }
    }
}