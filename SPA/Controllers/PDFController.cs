using SPA.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA.Controllers
{
    public class PDFController : Controller
    {
        //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        Function fu = new Function();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        Sql sql = new Sql();
        PushEmail Email = new PushEmail();
        CultureInfo enGB = new CultureInfo("en-GB");
        // GET: PDF
        public ActionResult PDFFILE(long? schId)
        {
            ViewBag.ShopInfo = SPA.CCTSP_User.Where(c => c.SchId == schId && c.ActiveStatus == "A" && c.RoleId == 1).FirstOrDefault();
            return View();
        }
        public ActionResult PDFTERMSCondition(long schId)
        {
            var shopInfo = SPA.CCTSP_User.Where(c => c.SchId == schId && c.ActiveStatus == "A" && c.RoleId == 1).FirstOrDefault();
            var shopId = "";
            if (shopInfo.CCTSP_SchoolMaster.Schcountry != null)
            {
                if (shopInfo.CCTSP_SchoolMaster.Schcountry == "INDIA")
                    shopId = "2";
                else
                    shopId = "1";
            }
            ViewBag.Language = SPA.Language_NewShop.Where(c => c.Page_Name == "Term_Condition_page" && c.Lang_id == shopInfo.CCTSP_SchoolMaster.Lang_id && c.col2 == "A" && c.Schid == shopId).ToList();
            return View();
        }
        public ActionResult InvoicePrint(string InvoiceId, bool IsPrint, string Url, string Status, long? schlId, string RStatus)
        {
            try
            {
                if (schlId == null)
                    schlId = fu.GetShopId(link);
                if (!string.IsNullOrEmpty(InvoiceId))
                {
                    var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                                    .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, TimeZone = c.TimeZone, Currency = c.Currency, OverDue = c.OverDue, IbanNo = c.IBANNO, AccountNumber = c.BANKACCOUNT, ExtendDuration = c.Extend_Pay_Duration, Display_Invoice = c.Display_Invoice })
                                    .FirstOrDefault();
                    DateTime CurrentDate = fu.ZonalDate(ShopInfo.TimeZone);
                    var VatFinalDropDown = new List<Models.CatgDropDownList>();
                    var VatDropDown = SPA.CCTSP_CategoryDetails
                                          .Where(c => c.CCTSP_CategoryMaster.CatgName == "Vat_Cal" && c.ActiveStatus == "A" && c.CCTSP_CategoryMaster.ActiveStatus == "A")
                                          .Select(c => new Models.CatgDropDownList { CatgId = c.CatgId, CatgDesc = c.CatgDesc, CatgName = c.CCTSP_CategoryMaster.CatgName, CatgType = c.CatgType, CatgTypeId = c.CatgTypeId })
                                          .ToList();
                    List<long> InvoiceIdList = new List<long>();
                    InvoiceIdList = InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).ToList();
                    var Details = fu.PrintMailManualInvoiveList(InvoiceId);
                    decimal Amount = 0;
                    int OverDue = ShopInfo.OverDue != null ? ShopInfo.OverDue.Value : 30;
                    int ExtOverDue = ShopInfo.ExtendDuration != null ? ShopInfo.ExtendDuration.Value : 10;
                    int ConsiderOverDue = RStatus == "YES" ? ExtOverDue : OverDue;
                    int DisplayInvoice = ShopInfo.Display_Invoice.Value;
                    var Link = fu.GetCurrenturl();
                    ViewBag.link = Link;
                    ViewBag.RStatus = RStatus;
                    if (Details != null)
                    {
                        Details.All(c => { c.ShopImage = !string.IsNullOrEmpty(c.ShopImage) ? Link + c.ShopImage : ""; c.CurrentDate = CurrentDate; c.DisplayInvoice = DisplayInvoice; return true; });
                        foreach (var item in InvoiceIdList)
                        {
                            VatFinalDropDown.AddRange(VatDropDown
                            .Select(c => new Models.CatgDropDownList
                            {
                                Invoice_Id = item,
                                CatgId = c.CatgId,
                                OverDue = ConsiderOverDue,
                                CatgDesc = c.CatgDesc,
                                CatgName = c.CatgName,
                                CatgType = c.CatgType,
                                CatgTypeId = c.CatgTypeId,
                                Currency = ShopInfo.Currency,
                                TotalAmount = string.Format("{0:0.00}", Details.Where(d => d.Print_Vat.ToString().Trim() == c.CatgDesc.Trim() && d.RealAmount != null && d.InvoiceNo == item)
                                              .GroupBy(d => d.Invoice_Detail_Id)
                                              .Select(e => decimal.Parse(
                                              (e.FirstOrDefault().RealAmount.Value +
                                              (e.FirstOrDefault().RealAmount.Value * double.Parse(e.FirstOrDefault().Print_Vat.ToString())) / 100).ToString())).Sum()),
                                Vat = string.Format("{0:0.00}", Details.Where(d => d.Print_Vat.ToString().Trim() == c.CatgDesc.Trim() && d.RealAmount != null && d.InvoiceNo == item)
                                              .GroupBy(d => d.Invoice_Detail_Id)
                                              .Select(e => decimal.Parse(
                                                          ((e.FirstOrDefault().RealAmount.Value *
                                                          double.Parse(e.FirstOrDefault().Print_Vat.ToString())) / 100).ToString())
                                                     ).Sum()),
                                TreatFromDate = DateTime.Parse(Details.Where(d => d.InvoiceNo == item).OrderBy(d => DateTime.Parse(d.BookingDate, enGB)).Select(d => d.BookingDate).FirstOrDefault(), enGB).ToString("dd.MM.yyyy"),
                                TreatToDate = DateTime.Parse(Details.Where(d => d.InvoiceNo == item).OrderBy(d => DateTime.Parse(d.BookingDate, enGB)).Select(d => d.BookingDate).LastOrDefault(), enGB).ToString("dd.MM.yyyy"),
                                AccountNumber = ShopInfo.AccountNumber,
                                Ibanno = ShopInfo.IbanNo
                            }).OrderBy(c => c.CatgType).ToList());
                        }
                    }

                    ViewBag.VatDropDown = VatFinalDropDown;
                    ViewBag.viewStatus = Status;
                    ViewBag.Status = IsPrint;
                    ViewBag.Url = Url;
                    ViewBag.Amount = Amount;
                    var CatgDetails = SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgDesc == "Invoice Link" && c.ActiveStatus == "A").Select(c => new Models.CategoryDetails { CatgDesc = c.Gender, CatgName = c.Banner_Image, Group_orderdata = c.Group_orderdata }).ToList();
                    var getPageName = CatgDetails.Select(d => d.CatgName).Distinct().ToList();
                    ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && (getPageName.Contains(c.Page_Name) || (c.Page_Name == "Invoice_Page" && c.Order_id == 92)) && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                    {
                        Order_id = c.Order_id,
                        Lang_id = c.Lang_id,
                        English_Label = c.English_Label,
                        Value = c.Value,
                        Page_Name = c.Page_Name
                    }).ToList();
                    ViewBag.partialPage = CatgDetails;
                    return View(Details);
                }
                else
                    return Redirect("/Invoice/Invoicing#" + Url);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult NotesDisplay(long Reservationid, bool IsPrint, string Display, long schlId, string Url, long? UserId)
        {
            try
            {
                /*GET data to Print,Display or Email*/
                var Details = fu.getNotesSectionWithData(Reservationid);
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                                   .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, Logo = c.ImageLogo ,SchlStudentStrength=c.SchlStudentStrength})
                                   .FirstOrDefault();
                ViewBag.Status = IsPrint;
                ViewBag.Display = Display;
                ViewBag.Url = Url;
                ViewBag.Logo = fu.LogoImg(ShopInfo.Logo);
                ViewBag.ShopInfo = ShopInfo;
                bool PopupStatus = true;
                if(Details.Where(c => c.ActiveStatus != null).Select(c => c.ActiveStatus.Trim()).FirstOrDefault() == "C")
                    PopupStatus = false;
                ViewBag.CheckPopupStatus = PopupStatus;
                Models.getAccess Access = new Models.getAccess();
                if (UserId > 0)
                    Access = fu.CheckAccessofPage("For Invoicing", UserId.Value);
                ViewBag.CheckAccessRight = Access;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Therapist_Notes" || c.Page_Name == "Therapist_Notes_Default") && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Label_Name = c.Label_Name
                }).ToList();
                return View(Details);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult GI_Customer()
        {
            return View();
        }
        public ActionResult Therapists_print()
        {
            return View();
        }
        public ActionResult Therapists1_print(long Reservationid, bool IsPrint, string Display, long schlId, string Url, long? UserId)
        {

            /*GET data to Print,Display or Email*/
            var Details = fu.getNotesSectionWithData(Reservationid);
            bool PopupStatus = true;
            if (Details.Where(c => c.ActiveStatus != null).Select(c => c.ActiveStatus.Trim()).FirstOrDefault() == "C")
                PopupStatus = false;
            ViewBag.CheckPopupStatus = PopupStatus;
            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                              .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, Logo = c.ImageLogo,SchlStudentStrength=c.SchlStudentStrength })
                              .FirstOrDefault();
            ViewBag.HistoryList = fu.GetNoteHistoryList("", Details.Select(c=>c.ClientUserId).FirstOrDefault().Value, ShopInfo.SchlStudentStrength.Value);
            ViewBag.ShopInfo = ShopInfo;
            ViewBag.Status = IsPrint;
            ViewBag.Display = Display;
            ViewBag.Url = Url;
            ViewBag.Logo = fu.LogoImg(ShopInfo.Logo);
            Models.getAccess Access = new Models.getAccess();
            if (UserId > 0)
                Access = fu.CheckAccessofPage("For Invoicing", UserId.Value);
            ViewBag.CheckAccessRight = Access;
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Therapist_Notes" || c.Page_Name == "Therapist_Notes_Default") && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
            {
                Lang_id = c.Lang_id,
                English_Label = c.English_Label,
                Value = c.Value,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id,
                Label_Name = c.Label_Name
            }).ToList();
            return View(Details);
        }
    }
}