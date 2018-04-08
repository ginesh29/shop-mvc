using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using SPA.Models;
using System.Configuration;

namespace SPA.Controllers
{
    [checkShop]
    public class FooterController : Controller
    {      
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);       
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        PushEmail Email = new PushEmail();
        Function fu = new Function();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public FooterController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: Footer
        [HttpGet]
        public ActionResult Contact()
        {
            try
            {
                var Lang_Id = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c=>c.Lang_id).FirstOrDefault();
                List<LanguageLabelDetails> ContactLangauge = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.Lang_id == Lang_Id && c.ActiveStatus=="A")
                    .Select(c => new LanguageLabelDetails { Lang_id = c.Lang_id, Order_id = c.Order_id, Page_Name = c.Page_Name, Value = c.Value,English_Label=c.English_Label })
                    .OrderBy(c => c.Order_id)
                    .ToList();
                ViewBag.ContactLangauge = ContactLangauge;
               // ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == ShopInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [HttpPost]
        public ActionResult Contact(FormCollection Get)
        {
            try
            {
                spa_contact contact = new spa_contact();
                contact.FirstName = Get["ConFirstName"].ToString();
                //contact.Street = Get["ConStreet"].ToString();
                //contact.place = Get["Conplace"].ToString();
                contact.Email = Get["ConEmail"].ToString();
               // contact.Firm = Get["ConFirm"].ToString();
                //contact.plz = Get["ConPLZ"].ToString();
                //contact.Mobile = Get["ConTelephone"].ToString();
                contact.communication = Get["ConCommunication"].ToString();
                contact.schid = schlId;
                contact.Activestatus = "A";
                SPA.spa_contact.Add(contact);
                SPA.SaveChanges();
                Email.EmailForContact(contact);
                return RedirectToAction("Home", "Home");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult PROFILE()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Links()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
    }
}