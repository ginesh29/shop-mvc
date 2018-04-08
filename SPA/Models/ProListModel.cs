using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class ProListModel
    {
        public long CatgTypeId { get; set; }
        public long SolutionId { get; set; }
        public long OrderData { get; set; }
        public string ProductName { get; set; }
        public string ProductDetail { get; set; }
        public int Duration { get; set; }
        public decimal Amount { get; set; }        
        public string Gender { get; set; }

    }
}