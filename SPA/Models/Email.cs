using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class Email
    {
        public string pdfLink { get; set; }
        public string PDFName { get; set; }
        public string FileArray { get; set; }
    }
    public class PDFDownload
    {
        public string Html { get; set; }
    }
}