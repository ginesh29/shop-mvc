using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace SPA.Models
{
    public class ProductList
    {
        public long CLIENT_UserId { get; set; }
        public long SchId { get; set; }
        public long CatgTypeId { get; set; }
        public string CatgDesc { get; set; }
        public string SectionName { get; set; }
        public string SectionDesc { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }       
        public long UserId { get; set; }
       
        
    }
    public class InvoiceInit
    {
        public long ReservationId { get; set; }
        public long Client_UserId { get; set; }
        public string ReservationList { get; set; }
    }
    public class AddonProduct
    {
        public long? AddOnProductId { get; set; }
        public string ProductName { get; set;}
        public string ProductDesc { get; set; }
        public Double? Quantity { get; set;}
        public Double? Buying_Price { get; set; }
        public Double? Selling_Price { get; set; }
        public string Manufacturer { get; set; }
        public string Dozes { get; set; }
        public long? SelectedAddOnPInsurance { get; set;}
        public string insurance { get; set; }
        public string SelectedVat { get; set; }
        public int? Tarif_Number { get; set; }
        public string Settlement_Number { get; set; }
        public string AddOn_C_Settlement_Text { get; set;}
        public string DefaultStatus { get; set;}
    }
    public class InsuranceList
    {
        public string Insurance { get; set;}
        public long InsuranceId { get; set;}
        public int? Tarif_Number { get; set;}
        public int? Settlement_Number { get; set;}
        public string DefaultStatus { get; set;}
    }
    public class productDetail
    {
        public string ItenName { get; set; }
        public string Amount { get; set; }
        public string Duration { get; set; }
        public string SectionDesc { get; set; }
        public string Image { get; set; }
    }
    public class settlementTxt
    {
        public long? Insurance_Id { get; set; }
        public int? Tarif_Number { get; set; }
        public int? Settlement_Number { get; set; }
        public string Settlement_text { get; set; }
        public bool checkbox { get; set; }
        public string Duration { get; set; }
        public string Amount { get; set; }
        public long? Health_Assign_id { get; set; }
        public int? order { get; set; }
        public double? Quantity { get; set; }
        public double? Rate { get; set; }
        public string flg { get; set; }
    }
}