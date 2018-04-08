using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using System.Configuration;

namespace SPA.Controllers
{
    public class ErrorController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        Function fu = null;
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public ErrorController()
        {
            fu = new Function();
            if (!fu.CheckClickandgo(link, null, 0).status)
            {
                schlId = Convert.ToInt32(fu.GetShopId(link));               
            }

        }
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error_List(string error)
        {
            var Lang_Id = 1;
            if (schlId > 251)
                Lang_Id = SPA.CCTSP_SchoolMaster.Where(c =>c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Error_Page" && c.Lang_id == Lang_Id).Select(c => new Models.LanguageLabelDetails
            {
                English_Label = c.English_Label,
                Value = c.Value,
                Lang_id = c.Lang_id,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id
            }).ToList();
            return View();
        } 
        public ActionResult AddError(FormCollection Error)
        {
            SPA_Error ErrorDetails = new SPA_Error();
            ErrorDetails.Action_Name = Error["Name"];
            ErrorDetails.ErrorMsg = Error["ErrorMessage"];
            ErrorDetails.Created_Date = fu.ZonalDate(null);
            ErrorDetails.ReqColumn1 = schlId.ToString();
            SPA.SPA_Error.Add(ErrorDetails);
            SPA.SaveChanges();
            return RedirectToAction("Home", "Home");
        }
        public void check_error(string hello)
        {
            try
            {
                int id = Convert.ToInt32(hello);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
    }
}