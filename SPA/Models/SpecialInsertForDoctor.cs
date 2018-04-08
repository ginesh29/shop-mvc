using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class SpecialInsertForDoctor
    {
        public int SpId { get; set; }
        public string Medicine { get; set; }
        public string HowManyTimes { get; set; }
        public string When { get; set; }
        public string AddPreDetails { get; set; }
        public string CountSpecial { get; set; }
        public string FreeText { get; set; }
        public string catgDesc { get; set; }
        public string MedicinecatgDesc { get; set; }
        public string HowManyTimescatgDesc { get; set; }
        public string WhencatgDesc { get; set; }
        public string status { get; set; }
        public long CatgId { get; set; }
    }
    public class NoteSectionList
    {
        public string CatgType { get; set;}
        public string Label_Name { get; set;}
        public string English_Label { get; set;}
        public string LangCatgId { get; set;}
        public long? CatgId { get; set; }
    }
    public class CategoryDetails
    {
        public long? CatgId { get; set; }
        public long? CatgTypeId { get; set; }
        public string CatgDesc { get; set; }
        public int? Lang_Id { get; set; }
        public long? Group_orderdata { get; set; }
        public string CatgName { get; set; }
    }
}