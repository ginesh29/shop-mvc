using Newtonsoft.Json.Linq;
using SPA.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SPA.Controllers
{
    [CheckClickandgo]
    public class RegistrationController : Controller
    {
        cctspDatabaseEntities22 SPA = null;
        JavaScriptSerializer js = null;
        int schlId = Convert.ToInt16(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string StPay = ConfigurationManager.AppSettings["StPayment"];
        string ErrorClick = ConfigurationManager.AppSettings["ErrorClick"];
        string link = null;
        Function fu = null;
        public RegistrationController()
        {
            SPA = new cctspDatabaseEntities22();
            js = new JavaScriptSerializer();
            fu = new Function();
            link = System.Web.HttpContext.Current.Request.Url.Host;
            schlId = Convert.ToInt16(fu.GetShopId(link).ToString());
        }
        // GET: Registration
        public ActionResult Welcome(int? lang)
        {
            Session["Registration"] = new Models.Registration();
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
            if (clickShop.status)
            {
                ViewBag.Language = SPA.Language_NewShop.Where(c => c.Page_Name == "WelCome_Page" && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                ViewBag.LangId = clickShop.LangId;
                return View();
            }
            else
                return Redirect(ErrorClick);
        }
        public ActionResult Login(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.Client = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == clickShop.LangId).OrderBy(c=>c.CatgType), "CatgType", "CatgType");
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Registration_Page") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangNameList = fu.LanguageNameList();
                    if (Session["Registration"] != "" && Session["Registration"] != null)
                    {
                        Models.Registration model;
                        model = (Models.Registration)Session["Registration"];
                        return View(model);
                    }
                    return View();
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [HttpPost]
        public ActionResult Login(Models.Registration model)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, model.Langid, schlId);
                if (clickShop.status)
                {
                    int Language = clickShop.LangId;
                    Session["Registration"] = model;
                    if (Session["Registration"] != null && Session["Registration"] != "")
                        return RedirectToAction("Registration", "Registration", new { lang = Language });
                    else
                        return RedirectToAction("Login", "Registration", new { lang = Language });
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Registration(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.LangId = clickShop.LangId;
                    if (Session["Registration"] != null && Session["Registration"] != "")
                    {
                        ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Registration_Page") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                        ViewBag.Client = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == clickShop.LangId).OrderBy(c=>c.CatgType), "CatgType", "CatgType");
                        var countrylist = new List<spatime_zone>();
                        ViewBag.LangNameList = fu.LanguageNameList();
                        Models.Registration model;
                        if (clickShop.stat == 2)
                        {
                            countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A" && c.name_country != "INDIA").ToList();
                        }
                        else
                            countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A" && c.name_country == "INDIA").ToList();
                        ViewBag.countrylist = countrylist;
                        model = (Models.Registration)Session["Registration"];
                        return View(model);
                    }
                    else
                        return RedirectToAction("Login", "Registration", new { lang = clickShop.LangId });
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [HttpPost]
        public ActionResult Registration(Models.Registration model)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, model.Langid, schlId);
                if (clickShop.status)
                {
                    int Language = clickShop.LangId;
                    if (Session["Registration"] != null && Session["Registration"] != "")
                    {
                        Models.Registration logindetails;
                        logindetails = (Models.Registration)Session["Registration"];
                        model.password = logindetails.password;
                        model.ConfirmPassword = logindetails.password;
                        Session["Registration"] = model;
                        return RedirectToAction("RegistrationDetail", "Registration", new { lang = Language });
                    }
                    else
                        return RedirectToAction("Login", "Registration", new { lang = Language });
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }

        public ActionResult RegistrationDetail(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    if (Session["Registration"] != null && Session["Registration"] != "")
                        ViewBag.LangId = lang;
                    if (clickShop.stat == 2)
                        ViewBag.countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A" && c.name_country != "INDIA").ToList();
                    else
                        ViewBag.countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A" && c.name_country == "INDIA").ToList();
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Registration_Page") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangNameList = fu.LanguageNameList();
                    string MainList = "select a.CatgTypeId as MainCatgId,a.CatgDesc as MainCategory ,b.SectionDesc as ShopTypeName,b.SolutionId as ShopTypeId from cctsp_categoryDetails a join ctsp_solutionMaster b on a.CatgTypeId = b.CatgTypeId and b.ActiveStatus = 'A' where a.catgId = 127 and a.ActiveStatus = 'A' and a.Lang_Id = " + lang;
                    var MainCategoryList = SPA.Database.SqlQuery<Models.ShopTypeDetails>(MainList).ToList();
                    var MainCategory = MainCategoryList.FirstOrDefault();
                    ViewBag.ShopDetails = js.Serialize(MainCategoryList);
                    ViewBag.ShopTypeList = new SelectList(MainCategoryList, "ShopTypeId", "ShopTypeName");
                    if (Session["Registration"] != null && Session["Registration"] != "")
                    {
                        Models.Registration model;
                        model = (Models.Registration)Session["Registration"];
                        model.State_hidden = model.State_hidden;
                        model.City_hidden = model.City_hidden;
                        model.Country_hidden = model.Country_hidden;
                        model.City = model.City;
                        model.State = model.State;
                        model.Country = model.Country;
                        model.shopaddress = model.Address;
                        model.shopzipcode = model.Zipcode;
                        model.maincategory = MainCategory.MainCategory;
                        model.MainCatgId = MainCategory.MainCatgId;
                        model.ShopTypeName = Convert.ToString(MainCategory.ShopTypeId);
                        if (string.IsNullOrEmpty(model.shopcountry_hidden)) { model.shopcountry_hidden = model.Country_hidden; model.shopcountry = model.Country; }
                        if (string.IsNullOrEmpty(model.shopState_hidden)) { model.shopState_hidden = model.State_hidden; model.shopState = model.State; }
                        if (string.IsNullOrEmpty(model.shopcity_hidden)) { model.shopcity_hidden = model.City_hidden; model.shopcity = model.City; }
                        Session["Registration"] = model;
                        return View(model);
                    }
                    else
                        return RedirectToAction("Login", "Registration", new { lang = clickShop.LangId });
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [HttpPost]
        public ActionResult RegistrationDetail(Models.Registration model)
        {
            try
            {
                int Language = model.Langid;
                if (Session["Registration"] != null && Session["Registration"] != "")
                {
                    Models.Registration Registration;
                    Registration = (Models.Registration)Session["Registration"];
                    model.FirstName = Registration.FirstName;
                    model.FamilyName = Registration.FamilyName;
                    model.emailid = Registration.emailid;
                    model.password = Registration.password;
                    model.phoneno = Registration.phoneno;
                    model.Country = Registration.Country;
                    model.City = Registration.City;
                    model.State = Registration.State;
                    model.Country = Registration.Country;
                    model.shopState = Registration.shopState;
                    model.shopcity = Registration.shopcity;
                    model.shopcountry = Registration.shopcountry;
                    model.State_hidden = Registration.State_hidden;
                    model.City_hidden = Registration.City_hidden;
                    model.Country_hidden = Registration.Country_hidden;
                    model.shopcity_hidden = Registration.shopcity_hidden;
                    model.shopState_hidden = Registration.shopState_hidden;
                    model.shopcountry_hidden = Registration.shopcountry_hidden;
                    model.Address = Registration.Address;
                    model.shopname = Registration.shopname;
                    model.Title = Registration.Title;
                    model.Zipcode = Registration.Zipcode;
                    model.ZsrNo = Registration.ZsrNo;
                    if (model.MainCatgId == 0 && model.ShopTypeName == null)
                    {
                        model.ShopTypeName = Registration.ShopTypeName;
                        model.MainCatgId = Registration.MainCatgId;
                        model.maincategory = Registration.maincategory;
                    }
                    Session["Registration"] = model;
                    return RedirectToAction("Various", "Registration", new { lang = Language });
                }
                else
                    return RedirectToAction("Login", "Registration", new { lang = Language });
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Various(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.LangId = clickShop.LangId;
                    if (Session["Registration"] != null && Session["Registration"] != "")
                    {
                        ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Registration_Page") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                        ViewBag.LangNameList = fu.LanguageNameList();
                        ViewBag.patmentType = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 155 && c.ActiveStatus == "A").ToList();
                        ViewBag.Status = clickShop.stat;
                        ViewBag.TimeSlot = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 112 && c.ActiveStatus == "A" && c.CatgDesc.Contains("Minute") && c.DomainType == 236).Select(c=> new Models.Timeslot {CatgDesc=c.CatgDesc.Replace("Minutes","").Replace("Minute","") }).ToList();
                        Models.Registration model;
                        model = (Models.Registration)Session["Registration"];
                        var timezonelist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A" && c.name_country == model.shopcountry_hidden).ToList();
                        ViewBag.timezone = new SelectList(timezonelist, "timezone_id", "name_timezone");
                        ViewBag.ShopStatus = ConfigurationManager.AppSettings["WebsiteName"];
                        ViewBag.DynamicLang = fu.LanguageNameList();
                        var SalesPersonList = SPA.CCTSP_User.Where(c => c.ActiveStatus == "A" && c.RoleId == 20 && c.SchId == 389).ToList();
                        ViewBag.SalesPersonList = new SelectList(SalesPersonList, "UserId", "FirstName");
                        ViewBag.StPay = StPay;
                        return View(model);
                    }
                    else
                        return RedirectToAction("Login", "Registration", new { lang = clickShop.LangId });
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [HttpPost]
        public ActionResult Various(Models.Registration model)
        {
            try
            {
                int Language = model.Langid;
                if (Session["Registration"] != null && Session["Registration"] != "")
                {
                    Models.Registration VariousDetails;
                    VariousDetails = (Models.Registration)Session["Registration"];
                    VariousDetails.Timezone = model.Timezone;
                   // VariousDetails.ShopLanguage = model.ShopLanguage;
                    VariousDetails.ShopLangId = model.ShopLangId;
                    VariousDetails.PaymentType = model.PaymentType;
                    VariousDetails.TimeSlot = model.TimeSlot;
                    if (model.ShopLink == "YES")
                    {
                        VariousDetails.Linktomywebsite = model.Linktomywebsite;
                        VariousDetails.HostingProvider = model.HostingProvider;
                        VariousDetails.HostingUserName = model.HostingUserName;
                        VariousDetails.HostingPassword = model.HostingPassword;
                        VariousDetails.ShopUrl = null;
                    }
                    else
                    {
                        VariousDetails.Linktomywebsite = null;
                        VariousDetails.HostingProvider = null;
                        VariousDetails.HostingUserName = null;
                        VariousDetails.HostingPassword = null;
                        VariousDetails.ShopUrl = model.ShopUrl;
                        VariousDetails.HShopUrl = model.HShopUrl;
                    }
                    if (model.salesLink == "YES")
                        VariousDetails.Salesid = model.Salesid;
                    else
                        VariousDetails.Salesid = 0;
                    Session["Registration"] = VariousDetails;
                    ViewBag.LangId = Language;
                    //if (StPay == "1")
                    //    return RedirectToAction("PaymentGateway", "Registration", new { lang = Language });
                    //else
                        return RedirectToAction("TermsCondition", "Registration", new { lang = Language });
                }
                else
                    return RedirectToAction("Login", "Registration", new { lang = Language });
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }

        public ActionResult ThanksPopup(int? lang)
        {
            try
            {
                Session["Registration"] = "";
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    lang = clickShop.LangId;
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Thanks_Page" || c.Page_Name == "Registration_Page") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangId = lang;
                    return View();
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult TermsCondition(int? lang)
        {
            try
            {
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    ViewBag.LangId = clickShop.LangId;
                    ViewBag.Language = SPA.Language_NewShop.Where(c => (c.Page_Name == "Registration_Page" || c.Page_Name == "Term_Condition_page") && c.Lang_id == clickShop.LangId && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                    ViewBag.LangNameList = fu.LanguageNameList();
                    if (Session["Registration"] != null && Session["Registration"] != "")
                        return View();
                    else
                        return RedirectToAction("Login", "Registration", new { lang = lang });
                }
                else
                    return Redirect(ErrorClick);

            }
            catch (Exception e)
            {
                return RedirectToAction("Error_List", "Error");
            }

        }
        //public ActionResult TermsConditionAdd(int? LangId)
        //{
        //    Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, LangId, schlId);
        //    if (clickShop.status)
        //    {
        //        CCTSP_SchoolMaster ShopDetails = new CCTSP_SchoolMaster();
        //        CCTSP_User ShopOwnerDetails = new CCTSP_User();
        //        CCTSP_CategoryDetails ShopMaincategory = new CCTSP_CategoryDetails();
        //        CCTSP_LendingPageMaster LandingData = new CCTSP_LendingPageMaster();
        //        var CurrentDate = fu.ZonalDate(null);
        //        TimeZoneInfo EuropeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        //        DateTime EuropeDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EuropeZone);
        //        if (Session["Registration"] != null && Session["Registration"] != "")
        //        {
        //            Models.Registration RegistrationDetails;
        //            RegistrationDetails = (Models.Registration)Session["Registration"];
        //            //add code for shop details
        //            if (RegistrationDetails != null)
        //            {
        //                var currency = SPA.spa_currency.Where(c => c.name_country == RegistrationDetails.shopcountry && c.ActiveStatus == "A").Select(c => c.currency).FirstOrDefault();
        //                //Shop Info
        //                #region ShopDetails
        //                ShopDetails.SchlName = RegistrationDetails.shopname;
        //                ShopDetails.SchPin = RegistrationDetails.shopzipcode;
        //                ShopDetails.Schcountry = RegistrationDetails.shopcountry_hidden;
        //                ShopDetails.SchlAddress = RegistrationDetails.shopaddress;
        //                ShopDetails.SchlWebsite = RegistrationDetails.Linktomywebsite;
        //                ShopDetails.SchlCity = RegistrationDetails.shopcity_hidden;
        //                ShopDetails.SchlState = RegistrationDetails.shopState_hidden;
        //                ShopDetails.SHOPTYPE = RegistrationDetails.ShopTypeName;
        //                ShopDetails.SchlEmployeeStrength = Convert.ToInt32(RegistrationDetails.Numberofemployees);
        //                ShopDetails.HostingProvider = RegistrationDetails.HostingProvider;
        //                ShopDetails.Hosting_UserName = RegistrationDetails.HostingUserName;
        //                ShopDetails.Hosting_Password = RegistrationDetails.HostingPassword;
        //                ShopDetails.ActiveStatus = "R";
        //                // ShopDetails.Terms_conditions = Status;
        //                ShopDetails.timezone_id = RegistrationDetails.Timezone;
        //                ShopDetails.Currency = currency;
        //                ShopDetails.SchlLandline2 = RegistrationDetails.phoneno;
        //                ShopDetails.SchlEmail = RegistrationDetails.emailid;
        //                ShopDetails.CreatedOn = EuropeDate;
        //                ShopDetails.PaymentType = RegistrationDetails.PaymentType;
        //                if (RegistrationDetails.ShopLanguage == "LangEnglish")
        //                {
        //                    ShopDetails.Lang_id = 1;
        //                }
        //                if (RegistrationDetails.ShopLanguage == "LangGerman")
        //                {
        //                    ShopDetails.Lang_id = 2;
        //                }
        //                if (RegistrationDetails.ShopLanguage == "LangFrench")
        //                {
        //                    ShopDetails.Lang_id = 3;
        //                }
        //                ShopDetails.MainCategory = RegistrationDetails.MainCatgId;
        //                SPA.CCTSP_SchoolMaster.Add(ShopDetails);
        //                SPA.SaveChanges();
        //                #endregion
        //                //Add LandingData
        //                #region
        //                string ShopStatus = ConfigurationManager.AppSettings["WebsiteName"];
        //                LandingData.Schid = ShopDetails.SchlId;
        //                LandingData.Original_Website = RegistrationDetails.ShopUrl + "." + ShopStatus;
        //                LandingData.Azure_Website = RegistrationDetails.ShopUrl + "." + ShopStatus;
        //                SPA.CCTSP_LendingPageMaster.Add(LandingData);
        //                SPA.SaveChanges();
        //                #endregion
        //                //Add Payment data 
        //                #region Payment Section
        //                if (StPay == "1")
        //                {                                                    
        //                    //Get All Id of one whose Input is not taken
        //                    var AllCatgDetailId = RegistrationDetails.PaymentDetails_id;
        //                    //Get All information for one whose UserInput is not taken
        //                    var Result = fu.getClickPaymentDetails(AllCatgDetailId, RegistrationDetails.Timezone);
        //                    //List<Models.DebitPayment> UserSpecific = new List<Models.DebitPayment>();
        //                    //For One whose input is taken
        //                    //if (!string.IsNullOrEmpty(frm["PayDetail"]))
        //                        //UserSpecific = js.Deserialize<List<Models.DebitPayment>>(Convert.ToString(frm["PayDetail"]));
        //                    var TotAmount = Result.Where(c => (c.CatPayment == "2" || c.CatPayment == "3") && c.PaymentType != null).Select(c => c.Amount).Sum() /*+ UserSpecific.Where(c => (c.CatPayment == "2" || c.CatPayment == "3")).Select(c => c.Amount).Sum()*/;
        //                    var TotalCredit = Result.Where(c => (c.CatPayment == "1" || c.CatPayment == "2") && c.PaymentType != null).Select(c => c.Amount).Sum() /*+ UserSpecific.Where(c => (c.CatPayment == "1" || c.CatPayment == "2")).Select(c => c.credit).Sum()*/;
        //                    //Billing Master information
        //                    #region BillMaster
        //                    spa_BillingMaster BillMaster = new spa_BillingMaster();
        //                    BillMaster.ShopId = ShopDetails.SchlId;
        //                    BillMaster.TotalAmount = TotAmount;
        //                    BillMaster.ActiveStatus = "A";
        //                    BillMaster.country_id = Result.Select(c => c.country_id).FirstOrDefault();
        //                    BillMaster.TotalCredit = TotalCredit;
        //                    BillMaster.created_on = EuropeDate;
        //                    BillMaster.Updated_on = EuropeDate;
        //                    if (Result.Select(c => c.Duration).Sum() > 0)
        //                    {
        //                        BillMaster.DurationStatus = "Y";
        //                        BillMaster.Duration = Convert.ToString(Result.Select(c => c.Duration).Sum());
        //                        BillMaster.StartDate = EuropeDate;
        //                    }
        //                    else
        //                        BillMaster.DurationStatus = "N";
        //                    BillMaster.RemainingCredit = TotalCredit;
        //                    SPA.spa_BillingMaster.Add(BillMaster);
        //                    SPA.SaveChanges();
        //                    #endregion
        //                    //Categories of Payment
        //                    #region BillingDetails
        //                    List<spa_BillingDetails> BillDetails = new List<spa_BillingDetails>();
        //                    foreach (var ReBillDetail in Result)
        //                    {
        //                        string PayDetailId = Convert.ToString(ReBillDetail.PaymentDetail_Id);
        //                        spa_BillingDetails bDetails = new spa_BillingDetails();
        //                        bDetails.ReceiptId = BillMaster.Receipt_Id;
        //                        bDetails.created_on = EuropeDate;
        //                        bDetails.Updated_on = EuropeDate;
        //                        bDetails.ShopId = ShopDetails.SchlId;
        //                        bDetails.ActiveStatus = "A";
        //                        bDetails.RechargeId = ReBillDetail.PaymentCatgId;
        //                        if ((ReBillDetail.CatPayment == "2" || ReBillDetail.CatPayment == "3") && ReBillDetail.PaymentType != null)
        //                            bDetails.Amount = ReBillDetail.Amount;
        //                        if ((ReBillDetail.CatPayment == "1" || ReBillDetail.CatPayment == "2") && ReBillDetail.PaymentType != null)
        //                            bDetails.Credit = ReBillDetail.Amount;
        //                        //if ((ReBillDetail.CatPayment == "2" || ReBillDetail.CatPayment == "3") && ReBillDetail.PaymentType == null)
        //                        //    bDetails.Amount = UserSpecific.Where(c => c.CatgId == PayDetailId).Select(c => c.Amount).FirstOrDefault();
        //                        //if ((ReBillDetail.CatPayment == "1" || ReBillDetail.CatPayment == "2") && ReBillDetail.PaymentType == null)
        //                        //    bDetails.Credit = UserSpecific.Where(c => c.CatgId == PayDetailId).Select(c => c.credit).FirstOrDefault();
        //                        bDetails.receivedAmount = bDetails.Amount;
        //                        if (ReBillDetail.Duration != 0)
        //                            bDetails.Duration = Convert.ToString(ReBillDetail.Duration);
        //                        BillDetails.Add(bDetails);
        //                    }
        //                    SPA.spa_BillingDetails.AddRange(BillDetails);
        //                    SPA.SaveChanges();
        //                    #endregion
        //                }
        //                #endregion
        //                //add code for shopowner details
        //                #region ShopownerDetail
        //                ShopOwnerDetails.FirstName = RegistrationDetails.FirstName;
        //                ShopOwnerDetails.LastName = RegistrationDetails.FamilyName;
        //                ShopOwnerDetails.PhoneNo = RegistrationDetails.phoneno;
        //                ShopOwnerDetails.EmailId = RegistrationDetails.emailid;
        //                ShopOwnerDetails.Gender = RegistrationDetails.Title;
        //                ShopOwnerDetails.Pincode = Convert.ToInt32(RegistrationDetails.Zipcode.ToString());
        //                ShopOwnerDetails.RoleId = 1;
        //                ShopOwnerDetails.ActiveStatus = "R";
        //                ShopOwnerDetails.Address1 = RegistrationDetails.Address;
        //                if (RegistrationDetails.password != null)
        //                {
        //                    ShopOwnerDetails.Password = RegistrationDetails.password;
        //                }
        //                else
        //                {
        //                    ShopOwnerDetails.Password = "Admin@12345";
        //                }
        //                ShopOwnerDetails.LoginName = RegistrationDetails.phoneno;
        //                ShopOwnerDetails.CreatedOn = EuropeDate;
        //                ShopOwnerDetails.Updated_Date = EuropeDate;
        //                ShopOwnerDetails.Country = RegistrationDetails.Country_hidden;
        //                ShopOwnerDetails.City = RegistrationDetails.City_hidden;
        //                ShopOwnerDetails.State = RegistrationDetails.State_hidden;
        //                ShopOwnerDetails.SchId = ShopDetails.SchlId;
        //                SPA.CCTSP_User.Add(ShopOwnerDetails);
        //                SPA.SaveChanges();
        //                #endregion
        //                //Email
        //                fu.SendEmail(RegistrationDetails, schlId, LangId.Value);
        //            }
        //            return RedirectToAction("ThanksPopup", "Registration", new { lang = LangId });
        //        }
        //        else
        //            return RedirectToAction("Login", "Registration", new { lang = LangId });
        //    }
        //    else
        //        return Redirect(ErrorClick);

        //}
        public ActionResult TermsConditionAdd(int? LangId)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, LangId, schlId);
            if (clickShop.status)
            {
                if (Session["Registration"] != null && Session["Registration"] != "")
                {
                    if (StPay == "1")
                        return RedirectToAction("PaymentGateway", "Registration", new { lang = LangId });
                    else
                    {
                        // CommonTermCondition(LangId);
                        fu.CommonAddRegisterationDetails(LangId,null);
                        return RedirectToAction("ThanksPopup", "Registration", new { lang = LangId });
                    }                      
                }
                else
                    return RedirectToAction("Login", "Registration", new { lang = LangId });
            }
            else
                return Redirect(ErrorClick);

        }
     
        public ActionResult FillMainCategory(long? CatgtypeID)
        {
            var MainCatgList = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgTypeId == CatgtypeID).Select(c => c.CatgDesc).FirstOrDefault();
            return Json(MainCatgList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PaymentGateway(int? lang)
        {
            try
            {
                string schid = ConfigurationManager.AppSettings["EnterpriseId"];
                var PayDispData = new List<Models.PaySection>();
                Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, lang, schlId);
                if (clickShop.status)
                {
                    lang = clickShop.LangId;
                    if (StPay == "1")
                    {
                        if (Session["Registration"] != null && Session["Registration"] != "")
                        {
                            Models.Registration model;
                            model = (Models.Registration)Session["Registration"];
                            if (model.PaymentDetails_id != "" && model.PaymentDetails_id != null)
                                ViewBag.GetAllChk = model.PaymentDetails_id;
                            int countryId = model.Timezone;
                           
                            PayDispData = fu.getClickPaymentDetails(null, countryId);

                        }
                        ViewBag.language = SPA.Language_NewShop.Where(c => c.Lang_id == lang && (c.Page_Name == "PaymentGateway" || c.Page_Name == "Registration_Page") && c.col2 == "A" && c.Schid == clickShop.schId).ToList();
                        ViewBag.LangNameList = fu.LanguageNameList();
                        ViewBag.LangId = lang;
                        return View(PayDispData);
                    }
                    else
                    {
                        return RedirectToAction("TermsCondition", "Registration", new { lang = lang });
                    }
                }
                else
                    return Redirect(ErrorClick);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public JsonResult ClickgoCheckPayment(Models.DebitPayment debit)
        {
            try
            {
                long UserShopId = schlId;
                var Result = fu.getClickPaymentDetails(debit.ListofCatgId, Convert.ToInt32(debit.shopid));
                long paydetailId = Convert.ToInt32(debit.CatgId);
                debit.TotalAmount = Result.Where(c => (c.CatPayment == "2" || c.CatPayment == "3")).Select(c => c.Amount).Sum() + debit.AmountUser;
                debit.TotalCredit = Result.Where(c => c.CatPayment == "1" || c.CatPayment == "2").Select(c => c.Amount).Sum() + debit.CreditUser;
                var CurrentResult = Result.Where(c => c.PaymentDetail_Id == paydetailId).FirstOrDefault();
                if (CurrentResult != null)
                {
                    if ((CurrentResult.CatPayment == "1" || CurrentResult.CatPayment == "2") && CurrentResult.PaymentType != null)
                        debit.credit = CurrentResult.Amount;
                    if ((CurrentResult.CatPayment == "2" || CurrentResult.CatPayment == "3") && CurrentResult.PaymentType != null)
                        debit.Amount = CurrentResult.Amount;
                    if (CurrentResult.PaymentType == null)
                    {
                        debit.User = "1";
                        if ((CurrentResult.CatPayment == "1" || CurrentResult.CatPayment == "2"))
                            debit.credit = Convert.ToInt32(debit.Amount);
                        debit.TotalAmount = debit.TotalAmount + Convert.ToInt32(debit.Amount);
                        debit.TotalCredit = debit.TotalCredit + debit.credit;
                    }
                    debit.CatPayment = CurrentResult.CatPayment;
                }
                return Json(debit, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public void AddPaymentdata(string getall)
        {
            if (Session["Registration"] != null && Session["Registration"] != "")
            {
                Models.Registration model;
                model = (Models.Registration)Session["Registration"];
                model.PaymentDetails_id = getall;
                Session["Registration"] = model;
            }
            // RedirectToAction("TermsCondition", "Registration", new { lang = 1 });
        }
        public JObject PayDetails(long PayDetailId, string getallChk)
        {
            JObject PayReturn = null;
            if (Session["Registration"] != null && Session["Registration"] != "")
            {
                Models.Registration model;
                model = (Models.Registration)Session["Registration"];
                int Countryid = model.Timezone;
                var Result = fu.getClickPaymentDetails(getallChk, Countryid);
                var FinalResult = Result.Where(c => c.PaymentDetail_Id == PayDetailId).FirstOrDefault();
                if (FinalResult != null)
                    FinalResult.status = "YES";
                else
                {
                    FinalResult = Result.FirstOrDefault();
                    FinalResult.status = "NO";
                }
                FinalResult.SumTotal = Result.Select(c => c.Amount).ToList().Sum();
                PayReturn = JObject.Parse(js.Serialize(FinalResult));

                return PayReturn;
            }
            else
                return PayReturn;
        }
        public bool CheckWebsiteExist(string Website)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            var WebsiteCount = SPA.CCTSP_LendingPageMaster.Where(c => c.Original_Website.StartsWith(Website + "." + clickShop.Website) || c.Azure_Website.StartsWith(Website + "." + clickShop.Website)).Count() > 0 || Website == "";
            return WebsiteCount;
        }
    }
}