using SPA.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA.Controllers
{
    [CheckClickandgo]
    public class ClickGoController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        Function fu = new Function();
        // GET: Clickgo
        public ClickGoController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        public ActionResult Home(int? Lang)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, Lang, schlId);
            if (clickShop.status)
            {
                ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Layout_Header" || c.Page_Name == "New_Home_Page" || c.Page_Name == "Layout_Footer") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).Select(c => new Models.LanguageNewShop
                {
                    Value = c.Value,
                    Order_id = c.Order_id,
                    Page_Name = c.Page_Name,
                    Lang_id = c.Lang_id,
                    SubPage = c.col1
                }).ToList();
                ViewBag.LangId = clickShop.LangId;
                ViewBag.SchId = clickShop.schId;
                ViewBag.status = clickShop.stat;
                string MainList = "select a.CatgTypeId as MainCatgId,a.CatgDesc as MainCategory ,b.SectionDesc as ShopTypeName,b.SolutionId as ShopTypeId from cctsp_categoryDetails a join ctsp_solutionMaster b on a.CatgTypeId = b.CatgTypeId and b.ActiveStatus = 'A' where a.catgId = 127 and a.ActiveStatus = 'A' and a.Lang_Id = " + clickShop.LangId;
                var MainCategoryList = SPA.Database.SqlQuery<Models.ShopTypeDetails>(MainList).ToList();
                ViewBag.ShopTypeList = new SelectList(MainCategoryList, "ShopTypeName", "ShopTypeName");
                return View();
            }
            else
                return RedirectToAction("Error_List", "Error");
        }
        public ActionResult AddLiveDemo(Models.LiveDemoList Details, int? Lang)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, Lang, schlId);
            try
            {
                if (clickShop.status && ModelState.IsValid)
                {
                    DateTime CurrentDate = fu.ZonalDate(null);
                    spa_DemoShop demoshop = new spa_DemoShop()
                    {
                        ActiveStatus = "A",
                        created_on = CurrentDate,
                        Updated_on = CurrentDate,
                        first_name = Details.first_name,
                        LastName = Details.LastName,
                        mobilenumber=Details.mobilenumber,
                        email=Details.email,
                        ShopType=Details.ShopType,
                        Extra1=Details.Extra1,
                        Extra2=Details.Extra2
                    };
                    PushEmail Email = new PushEmail();
                    SPA.spa_DemoShop.Add(demoshop);
                    SPA.SaveChanges();
                    Email.EmailForLiveDemo(demoshop, Lang);
                    Session["Message"] = "MailSentSuccessFully";
                    return RedirectToAction("Home", "ClickGo", new { Lang = Lang });
                }
                else
                {
                    Session["Message"] = "MailNotSent";
                    return RedirectToAction("Home", "ClickGo", new { Lang = Lang });
                }
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        //Smartwareplus
        public ActionResult CLICKANDGO(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "SMARTWAREPLUS_Page" || c.Page_Name == "Layout_Footer" || c.Page_Name == "Layout_Header") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).Select(c => new Models.LanguageNewShop
                    {
                        Value = c.Value,
                        Order_id = c.Order_id,
                        Page_Name = c.Page_Name,
                        Lang_id = c.Lang_id,
                        SubPage = c.col1,
                        Catg_Sep = c.Catg_Sep
                    }).ToList();
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
        public ActionResult Aboutus(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "About_Page" || c.Page_Name == "Layout_Footer" || c.Page_Name == "Layout_Header") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).Select(c => new Models.LanguageNewShop
                    {
                        Value = c.Value,
                        Order_id = c.Order_id,
                        Page_Name = c.Page_Name,
                        Lang_id = c.Lang_id,
                        SubPage = c.col1,
                        Catg_Sep = c.Catg_Sep
                    }).ToList(); ;
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
        public void AddLogDetails()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request != null)
                {
                    var HostAddress = !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.UserHostAddress) ? System.Web.HttpContext.Current.Request.UserHostAddress : "";
                    if (SPA.CCTSP_CategoryDetails.Where(c => c.CatgDesc == HostAddress && c.ActiveStatus == "A" && c.CCTSP_CategoryMaster.CatgName == "NEW WEBSITE LOG").Count() == 0)
                    {
                        CCTSP_CategoryDetails catgDetails = new CCTSP_CategoryDetails();
                        catgDetails.CreatedOn = DateTime.Now;
                        /*CallerIp*/
                        catgDetails.CatgDesc = HostAddress;
                        catgDetails.CatgType = "";
                        /*Caller Agent*/
                        catgDetails.Email_Image = !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.UserAgent) ? System.Web.HttpContext.Current.Request.UserAgent : "";
                        /*client Url*/
                        catgDetails.Search_Image = !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Url.OriginalString) ? System.Web.HttpContext.Current.Request.Url.OriginalString : "";
                        catgDetails.Banner_Image = System.Web.HttpContext.Current.Request.UserLanguages.Count() > 0 ? string.Join(",", System.Web.HttpContext.Current.Request.UserLanguages) : "";
                        catgDetails.ActiveStatus = "A";
                        catgDetails.Update_Date = DateTime.Now;
                        catgDetails.CatgId = SPA.CCTSP_CategoryMaster.Where(c => c.CatgName == "NEW WEBSITE LOG").Select(c => c.CatgId).FirstOrDefault();
                        SPA.CCTSP_CategoryDetails.Add(catgDetails);
                        SPA.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

            }

        }
        public ActionResult Impressum(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "IMPRESSUM" || c.Page_Name == "Layout_Footer" || c.Page_Name == "Layout_Header") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).Select(c => new Models.LanguageNewShop
                    {
                        Value = c.Value,
                        Order_id = c.Order_id,
                        Page_Name = c.Page_Name,
                        Lang_id = c.Lang_id,
                        SubPage = c.col1,
                        Catg_Sep = c.Catg_Sep
                    }).ToList();
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
    }
}