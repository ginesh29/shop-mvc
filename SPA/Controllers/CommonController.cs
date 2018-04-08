using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA.Controllers
{
    public class CommonController : Controller
    {
        Common.Function com = new Common.Function();
        // GET: Common
        public string DownloadFILE(string url)
        {
            string path = (url); //get physical file path from server
            string name = Path.GetFileName(path); //get file name
            string ext = Path.GetExtension(path); //get file extension
            string type = "";
            // set known types based on file extension
            if (ext != null)
            {
                switch (ext.ToLower())
                {
                    case ".htm":
                    case ".html":
                        type = "text/HTML";
                        break;

                    case ".txt":
                        type = "text/plain";
                        break;

                    case ".GIF":
                        type = "image/GIF";
                        break;

                    case ".pdf":
                        type = "Application/pdf";
                        break;

                    case ".doc":
                    case ".rtf":
                        type = "Application/msword";
                        break;
                    case ".xls":
                        type = "application/ms-excel";
                        break;
                }
            }
            Response.AppendHeader("content-disposition", "attachment; filename=" + name);
            if (type != "")
                Response.ContentType = type;
            Response.WriteFile(path);
            Response.End(); //give POP to user for file downlaod
            return "";
        }
        [ValidateInput(false)]
        public string DownloadPDF(long schId, int Year, int grt, int sml)
        {
            var APIURL = ConfigurationManager.AppSettings["APIUrl"] + "Logics/PaymentHistoryPDF?schId=" + schId + "&Year=" + Year + "&grt=" + grt + "&sml=" + sml;
            //var APIURL = ConfigurationManager.AppSettings["EmailUrl"] + URL;
            var api = new HtmlToPdfOrImage.Api("6732e433-91a4-4540-a837-fd425291b3b5", "sTh41407");
            string requested = HttpContext.Request.Url.Authority;
            var result = api.Convert(com.GetAnyLinkHtml(APIURL));
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            var FileName = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/NewFolder1/"), randomNumber + ".pdf");
            bool IsExists = Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/NewFolder1/"));
            if (!IsExists)
            {
                System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/NewFolder1/"));
            }
            System.IO.File.WriteAllBytes(FileName, (byte[])result.model);
            DownloadFILE(FileName);
            return FileName;
        }

        public string ConvertPartialViewToString(PartialViewResult partialView)
        {
            using (var sw = new StringWriter())
            {
                partialView.View = ViewEngines.Engines
                  .FindPartialView(ControllerContext, partialView.ViewName).View;

                var vc = new ViewContext(
                  ControllerContext, partialView.View, partialView.ViewData, partialView.TempData, sw);
                partialView.View.Render(vc, sw);

                var partialViewString = sw.GetStringBuilder().ToString();

                return partialViewString;
            }
        }
        public ActionResult Image()
        {
            return View();
        }

    }
}