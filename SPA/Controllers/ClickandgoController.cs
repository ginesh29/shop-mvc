using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;


namespace SPA.Controllers
{
    [CheckClickandgo]
    public class ClickandgoController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]); 
        Function fu = new Function();
        JavaScriptSerializer js = new JavaScriptSerializer();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        // GET: Clickandgo
        public ClickandgoController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }

        public ActionResult Index(int? lang)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
            if (clickShop.status)
            {
                Session["Registration"] = new Models.Registration();
                ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "HOME_PAGE" || c.Page_Name == "Layout_Header" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                ViewBag.LangId = clickShop.LangId;
                ViewBag.SchId = clickShop.schId;
                ViewBag.status = clickShop.stat;
                return View();
            }
            else
                return RedirectToAction("Error_List", "Error");
        }
        public ActionResult About(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "About_Page" || c.Page_Name == "Layout_Header_Two" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.SchId = clickShop.schId;
                    ViewBag.status = clickShop.stat;
                    ViewBag.Message = "Your application description page.";
                    return View();
                }
                else { return RedirectToAction("Error_List", "Error"); }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult FUNKTIONEN(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "FEATURES_PAGE" || c.Page_Name == "Layout_Header" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.SchId = clickShop.schId;
                    ViewBag.status = clickShop.stat;
                    return View();
                }
                else { return RedirectToAction("Error_List", "Error"); }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult LiveDemo(int? lang)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
            if (clickShop.status)
            {
                ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "ADVANTAGES_PAGE" || c.Page_Name == "Layout_Header" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                string MainList = "select a.CatgTypeId as MainCatgId,a.CatgDesc as MainCategory ,b.SectionDesc as ShopTypeName,b.SolutionId as ShopTypeId from cctsp_categoryDetails a join ctsp_solutionMaster b on a.CatgTypeId = b.CatgTypeId and b.ActiveStatus = 'A' where a.catgId = 127 and a.ActiveStatus = 'A' and a.Lang_Id = " +lang;
                var MainCategoryList = SPA.Database.SqlQuery<Models.ShopTypeDetails>(MainList).ToList();
                ViewBag.ShopTypeList = new SelectList(MainCategoryList, "ShopTypeId", "ShopTypeName");
                ViewBag.LangId = clickShop.LangId;
                ViewBag.SchId = clickShop.schId;
                ViewBag.status = clickShop.stat;
                return View();
            }
            else { return RedirectToAction("Error_List", "Error"); }
        }
        public ActionResult AddLiveDemo(spa_DemoShop Details,int?Lang)
        {
            PushEmail Email = new PushEmail();
            DateTime CurrentDate= fu.ZonalDate(null);
            Details.ActiveStatus = "A";
            Details.created_on = CurrentDate;
            Details.Updated_on = CurrentDate;
            SPA.spa_DemoShop.Add(Details);
            SPA.SaveChanges();
            Email.EmailForLiveDemo(Details,Lang);
            return RedirectToAction("LiveDemo", "Clickandgo");
        }
        public ActionResult VORTEILE(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "ADVANTAGES_PAGE" || c.Page_Name == "Layout_Header" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.SchId = clickShop.schId;
                    ViewBag.status = clickShop.stat;
                    return View();
                }
                else { return RedirectToAction("Error_List", "Error"); }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult SMARTWAREPLUS(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "SMARTWAREPLUS_Page" || c.Page_Name == "Layout_Header_Two" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.SchId = clickShop.schId;
                    ViewBag.status = clickShop.stat;
                    return View();
                }
                else { return RedirectToAction("Error_List", "Error"); }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult PREISE(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Prices_Page" || c.Page_Name == "Layout_Header" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.SchId = clickShop.schId;
                    ViewBag.status = clickShop.stat;
                    return View();
                }
                else { return RedirectToAction("Error_List", "Error"); }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult KONTAKTREGISTRIERUNG(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Client = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == clickShop.LangId).OrderBy(c=>c.CatgType), "CatgType", "CatgType");
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "CONTACT_REGIST_PAGE" || c.Page_Name == "Layout_Header" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.SchId = clickShop.schId;
                    ViewBag.status = clickShop.stat;
                    return View();
                }
                else
                    return RedirectToAction("Error_List", "Error");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public JsonResult GetLocation(int Location, int Id)
        {
            List<Models.LocationStatus> LocationData = new List<Models.LocationStatus>();
            if (Location == 1)
                LocationData = SPA.spa_Location.Where(c => c.country_id == Id).Select(c => new Models.LocationStatus { LocId = c.ID, name = c.name }).ToList();
            else
                LocationData = SPA.spa_Location.Where(c => c.state_id == Id).Select(c => new Models.LocationStatus { LocId = c.ID, name = c.name }).ToList();
            return Json(LocationData, JsonRequestBehavior.AllowGet);
        }
    }
}