using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using System.Collections;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Text;
using SPA.Models;
using System.Web.Script.Serialization;

namespace SPA.Controllers
{
    [Compress]
    [checkShop]
    public class HomeController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        Function fu = null;
        PushEmail Email = null;
        CultureInfo enGB = null;
        JavaScriptSerializer js = null;
        PuchSMS SMS = null;
        string ControllerName = "";
        string ActionName = "";
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public HomeController()
        {
            fu = new Function();
            if (!fu.CheckClickandgo(link, null, 0).status)
            {
                ControllerName = "Home";
                schlId = Convert.ToInt32(fu.GetShopId(link));
                Email = new PushEmail();
                enGB = new CultureInfo("en-GB");
                js = new JavaScriptSerializer();
                SMS = new PuchSMS();
            }

        }
        //Language according to user
        public ActionResult Home(string status)
        {
            try
            {
                var ShopInfoDetail = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schlId && c.CCTSP_SchoolMaster.ActiveStatus == "A").Select(c => new ShopMasterDetail
                {
                    Field1 = c.Field1,
                    Field2 = c.Field2,
                    Field3 = c.Field3,
                    Field4 = c.Field4,
                    Field5 = c.Field5,
                    Shopimg1 = c.Shopimg1,
                    Shopimg2 = c.Shopimg2,
                    Shopimg3 = c.Shopimg3,
                    Shopimg4 = c.Shopimg4,
                    ShopName = c.ShopName,
                    Address = c.Address,
                    Email_id = c.Email_id,
                    PhoneNo = c.PhoneNo,
                    ShopCity = c.ShopCity,
                    ZipCode = c.ZipCode,
                    Color_Id = c.CCTSP_SchoolMaster.Color_Id,
                    Lang_id = c.CCTSP_SchoolMaster.Lang_id.Value,
                    SchlWebsite = c.CCTSP_SchoolMaster.SchlWebsite,
                    TimeZone = c.CCTSP_SchoolMaster.TimeZone,
                    timezone_id = c.CCTSP_SchoolMaster.timezone_id,
                    SchlStudentStrength = c.CCTSP_SchoolMaster.SchlStudentStrength.Value,
                    Shop_Address = c.CCTSP_SchoolMaster.SchlAddress,
                    Shop_Email_id = c.CCTSP_SchoolMaster.SchlEmail,
                    Shop_ShopCity = c.CCTSP_SchoolMaster.SchlCity,
                    Shop_ZipCode = c.CCTSP_SchoolMaster.SchPin,
                    Shop_PhoneNo = c.CCTSP_SchoolMaster.CCTSP_User.Where(d => d.RoleId == 1).Select(d => d.PhoneNo).FirstOrDefault()
                }).FirstOrDefault();
                //Language according to user
                var LangId = fu.getLanguageId(ShopInfoDetail.Lang_id.Value, null);
                if (ShopInfoDetail != null)
                {
                    if (string.IsNullOrEmpty(ShopInfoDetail.SchlWebsite) || ShopInfoDetail.SchlWebsite.Contains(HttpContext.Request.Url.Host) || !string.IsNullOrEmpty(status))
                    {
                        //var LendingData = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schlId).FirstOrDefault();
                        ViewBag.lendingDetails = ShopInfoDetail;
                        //domain type 236 and catgid=30 for week days 
                        string WeekDay = " select a.CatgTypeId,b.Value as Day,c.CatgDesc,c.Gender from cctsp_CategoryDetails a " +
                                   " join Language_Label_Detail b on a.Catgdesc = b.English_Label " +
                                   " left join cctsp_CategoryDetails c on c.value = a.CatgTypeId and c.DomainType = " + schlId + " and c.ActiveStatus = 'A' and c.CatgId = 148 " +
                                   " Where a.CatgId = 30 and a.ActiveStatus = 'A' and a.DomainType = 236 " +
                                   " and b.Lang_Id = " + LangId + " and b.ActiveStatus = 'A' and b.Page_Name = 'Create_Employee'";
                        ViewBag.WeekDetails = SPA.Database.SqlQuery<Models.LendingWeekScheduleDetails>(WeekDay).ToList();
                        ViewBag.WeekSchedule = SPA.Database.SqlQuery<LendingWeekSchedule>("select b.Value,b.CatgDesc,b.Gender,a.CatgDesc as [Day], a.CatgTypeid from cctsp_categoryDetails a left join cctsp_categoryDetails b on b.value = a.CatgTypeId and b.catgId = 148 and b.DomainType = " + schlId + " and b.ActiveStatus = 'A' where a.CatgId = 30 and a.DomainType = 236").ToList();
                        //var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                        var Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "HomePage" || (c.Page_Name == "Title" && c.Order_id == 1) || (c.Page_Name == "Lending_Page") || (c.Page_Name == "Create_Employee")) && c.Lang_id == LangId)
                            .Select(c => new LanguageLabelDetails { Lang_id = c.Lang_id, Order_id = c.Order_id, Page_Name = c.Page_Name, Value = c.Value, English_Label = c.English_Label })
                            .ToList();
                        //var Language = fu.getLanguageData("HomePage", 0, ShopInfoDetail.Lang_id.Value);
                        ViewBag.Language = Language;
                        var DoctorFlow = ShopInfoDetail.SchlStudentStrength != null ? ShopInfoDetail.SchlStudentStrength : 0;
                        var LanguageHome = Language.Where(c => c.Page_Name == "HomePage" && c.Order_id >= 11).ToList();
                        Models.HomeDefaultStructure HomePageDefult = new Models.HomeDefaultStructure()
                        {
                            Dimg1 = DoctorFlow == 3 ? "/images/doctor default1.jpg" : "/images/spa01.jpg",
                            Dimg2 = DoctorFlow == 3 ? "/images/doctor default2.jpg" : "/images/spa02.jpg",
                            Dimg3 = DoctorFlow == 3 ? "/images/doctor default3.jpg" : "/images/spa03.jpg",
                            Dimg4 = DoctorFlow == 3 ? "/images/doctor default4.jpg" : "/images/spa04.jpg",
                            Dimgtext1 = DoctorFlow == 3 ? LanguageHome.Where(c => c.Order_id == 15 && c.Page_Name == "HomePage").Select(c => c.Value).FirstOrDefault() : LanguageHome.Where(c => c.Order_id == 11).Select(c => c.Value).FirstOrDefault(),
                            Dimgtext2 = DoctorFlow == 3 ? LanguageHome.Where(c => c.Order_id == 16).Select(c => c.Value).FirstOrDefault() : LanguageHome.Where(c => c.Order_id == 12).Select(c => c.Value).FirstOrDefault(),
                            Dimgtext3 = DoctorFlow == 3 ? LanguageHome.Where(c => c.Order_id == 17).Select(c => c.Value).FirstOrDefault() : LanguageHome.Where(c => c.Order_id == 13).Select(c => c.Value).FirstOrDefault(),
                            Dimgtext4 = DoctorFlow == 3 ? LanguageHome.Where(c => c.Order_id == 18).Select(c => c.Value).FirstOrDefault() : LanguageHome.Where(c => c.Order_id == 14).Select(c => c.Value).FirstOrDefault()
                        };
                        ViewBag.DeFault = HomePageDefult;
                        return View();
                    }
                    else
                    {
                        if (ShopInfoDetail.SchlWebsite.Contains("http://") || ShopInfoDetail.SchlWebsite.Contains("https://"))
                            return new RedirectResult(ShopInfoDetail.SchlWebsite);
                        else
                            return new RedirectResult("http://" + ShopInfoDetail.SchlWebsite);
                    }
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
        public JsonResult GetAllLangDays()
        {
            var LangId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
            var Days = SPA.Language_Label_Detail.Where(c => c.Lang_id == LangId && c.Order_id <= 7 && c.Page_Name == "Small_calander").Select(c => c.Value).ToList();
            Models.DateAndTimeArray LangDetails = new Models.DateAndTimeArray() { BlockData = Days.ToArray() };
            return Json(LangDetails, JsonRequestBehavior.AllowGet);
        }
        public string GetAllMonths(string month)
        {
            string ArrayMonths = "";
            try
            {
                var LangId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
                var partmonth = SPA.Language_Label_Detail.Where(c => c.Order_id >= 8 && c.Value == month && c.Page_Name == "Small_calander").Select(c => c.English_Label).FirstOrDefault();
                var Months = SPA.Language_Label_Detail.Where(c => c.Lang_id == LangId && c.Order_id >= 8 && c.English_Label == partmonth && c.Page_Name == "Small_calander").Select(c => c.Value).FirstOrDefault();
                ArrayMonths = Months.ToString();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return string.Empty;
            }
            return ArrayMonths;
        }
        public ActionResult LogutAfterAllBooking()
        {
            try
            {
                Session["UserId"] = "";
                Session["SchoolId"] = 0;
                Session["RoleId"] = 0;
                Session["UserEmailId"] = "";
                Session["UserFirstName"] = "";
                Session["UserLastName"] = "";
                Session["UserGender"] = "";
                Session["UserName"] = "";
                Session["PhoneNumber"] = "";
                Session["Remember"] = "";
                Session["Message"] = "";
                //fu.RemoveCookie("Auth");
                var GetWebsite = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                if (!string.IsNullOrEmpty(GetWebsite.SchlWebsite))
                {
                    if (GetWebsite.SchlWebsite.Contains("http://"))
                        return Redirect(GetWebsite.SchlWebsite);
                    else
                        return Redirect("http://" + GetWebsite.SchlWebsite);
                }
                else
                {
                    return RedirectToAction("Home", "Home");
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Contact()
        {
            try
            {
                ViewBag.Message = "Your contact page.";
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        public ActionResult ChooseYourProduct(string Head, string Back)
        {
            try
            {
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                    .Select(c => new ShopMasterDetail
                    {
                        Lang_id = c.Lang_id.Value,
                        SchlWebsite = c.SchlWebsite,
                        TimeZone = c.TimeZone,
                        timezone_id = c.timezone_id,
                        Currency = c.Currency,
                        ShowPrice = c.ShowPrice
                    }).FirstOrDefault();
                #region Language
                var LanguageId = fu.getLanguageId(SchoolInfo.Lang_id.Value, null);
                //int LanguageId = SchoolInfo.Lang_id.Value;
                ViewBag.SchoolInfo = SchoolInfo;
                ViewBag.Language = fu.getLanguageData("Choose_Product", 0, LanguageId);
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 2 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                #endregion
                DateTime EuropeDate = fu.ZonalDate(SchoolInfo.TimeZone);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                ViewBag.GetPastProductCount = 0;
                ViewBag.currency = fu.currency(SchoolInfo.Currency);
                #region Logic
                if (Convert.ToInt32(Session["RoleId"].ToString()) == 4)
                {
                    long UserId = Convert.ToInt64(Session["UserId"].ToString());
                    if (Back == null)
                    {
                        var DeleteAllData = "update SPA_EmployeeScheduler set ActiveStatus='DE',BookedStatus='DE' where CLIENT_UserId=" + UserId + "and (ActiveStatus='DA' and BookedStatus='T') or (BookedStatus='BT' and ActiveStatus='T') or (BookedStatus = 'BT' and ActiveStatus = 'DA') ";
                        SPA.Database.ExecuteSqlCommand(DeleteAllData);
                    }
                    #region SessionContainsValue
                    if (!string.IsNullOrEmpty(Session["ProductChoosed"].ToString()))
                    {
                        List<long> ProductsList = Session["ProductChoosed"].ToString().Split('~').Select(c => (long.Parse(c))).ToList();
                        var GetCatgTypeId = (from g in SPA.CTSP_SolutionMaster where ProductsList.Contains(g.CatgTypeId) select g).ToList();
                        foreach (var prod in ProductsList)
                        {
                            long CatgTypeId = prod;
                            var data1 = SPA.SPA_EmployeeScheduler.Where(c => c.CLIENT_UserId == UserId && c.ActiveStatus == "DA" && c.BookedStatus == "T" && c.Product_Id == CatgTypeId).FirstOrDefault();
                            if (data1 == null)
                            {
                                SPA_EmployeeScheduler Employee = new SPA_EmployeeScheduler();
                                Employee.CLIENT_UserId = UserId;
                                Employee.Product_Id = CatgTypeId;
                                var data = GetCatgTypeId.Where(c => c.CatgTypeId == CatgTypeId).FirstOrDefault();
                                Employee.Product_price = data.Amount;
                                Employee.SchId = schlId;
                                Employee.CreatedOn = EuropeDate;
                                Employee.ActiveStatus = "DA";
                                Employee.BookedStatus = "T";
                                Employee.UpdatedOn = EuropeDate;
                                SPA.SPA_EmployeeScheduler.Add(Employee);
                                SPA.SaveChanges();
                                Session["ResId"] = Employee.EmpSchDetailsId;
                            }
                        }
                        Session["ProductChoosed"] = "";
                        return RedirectToAction("ChooseEmployee", "Home");
                    }
                    #endregion
                    #region NormalFlow
                    else
                    {
                        var GetPastProduct = SPA.SPA_EmployeeScheduler.Where(c => c.ActiveStatus == "DA" && c.BookedStatus == "T" && c.CLIENT_UserId == UserId && c.SchId == schlId).Select(c => c.Product_Id).ToList();
                        ViewBag.GetPastProduct = GetPastProduct;
                        ViewBag.GetPastProductCount = GetPastProduct.Count;
                        #region new code
                        string Product = "select d.CatgTypeId as ProductId,a.SectionDesc as ProductName,d.orderData,b.Gender as Gender, d.SectionDesc,cast(a.Duration as int) as Duration,CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38, 2)) as Amount,f.CatgDesc as GroupName,e.GroupIdByShop from CTSP_SolutionMaster a join CCTSP_CategoryDetails b on a.sectionDesc = b.CatgDesc join cctsp_user c on a.UserId = c.UserId join CTSP_SolutionMaster d on d.CatgTypeId = b.CatgTypeId join SPA_SchedulerSlotMaster s on s.UserId = c.UserId join CCTSP_GroupProductDetails e on e.ProductId = b.CatgTypeId join CCTSP_CategoryDetails f on f.CatgTypeId = e.GroupIdByShop where a.activestatus = 'A' and d.activestatus = 'A' and c.activestatus = 'A'and b.activestatus = 'A' and s.ActiveStatus = 'A' and e.ActiveStatus = 'A' and f.ActiveStatus = 'A' and a.SchId = " + schlId + " and d.SchId = a.SchId and c.SchId = a.SchId and s.schId = a.SchId and e.SchlId = a.SchId and f.DomainType = a.SchId and a.SectionName = 'EmployeeProduct' and b.catgId = 111 and b.catgDesc = a.SectionDesc and d.Duration = a.Duration and d.amount = a.amount group by d.CatgTypeId,a.SectionDesc,d.orderData,b.Gender, d.SectionDesc,a.Duration,a.Amount,f.CatgDesc,e.GroupIdByShop,f.Group_orderdata order by f.Group_orderdata,d.orderdata";
                        ViewBag.Product = SPA.Database.SqlQuery<Models.GroupProductList>(Product).ToList();
                        fu.setActivityLogAsync("ChooseYourProduct", ControllerName, schlId, "Choose Your product", ((List<Models.GroupProductList>)ViewBag.Product).Select(c => c.ProductId).ToList());
                        return View();
                        #endregion
                    }
                }
                #endregion
                else
                {
                    Session["heading"] = "a";
                    string Product = "select d.CatgTypeId as ProductId,a.SectionDesc as ProductName,d.orderData,b.Gender as Gender, d.SectionDesc,cast(a.Duration as int) as Duration,CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38, 2)) as Amount,f.CatgDesc as GroupName,e.GroupIdByShop from CTSP_SolutionMaster a join CCTSP_CategoryDetails b on a.sectionDesc = b.CatgDesc join cctsp_user c on a.UserId = c.UserId join CTSP_SolutionMaster d on d.CatgTypeId = b.CatgTypeId join SPA_SchedulerSlotMaster s on s.UserId = c.UserId join CCTSP_GroupProductDetails e on e.ProductId = b.CatgTypeId join CCTSP_CategoryDetails f on f.CatgTypeId = e.GroupIdByShop where a.activestatus = 'A' and d.activestatus = 'A' and c.activestatus = 'A'and b.activestatus = 'A' and s.ActiveStatus = 'A' and e.ActiveStatus = 'A' and f.ActiveStatus = 'A' and a.SchId = " + schlId + " and d.SchId = a.SchId and c.SchId = a.SchId and s.schId = a.SchId and e.SchlId = a.SchId and f.DomainType = a.SchId and a.SectionName = 'EmployeeProduct' and b.catgId = 111 and b.catgDesc = a.SectionDesc and d.Duration = a.Duration and d.amount = a.amount group by d.CatgTypeId,a.SectionDesc,d.orderData,b.Gender, d.SectionDesc,a.Duration,a.Amount,f.CatgDesc,e.GroupIdByShop,f.Group_orderdata order by f.Group_orderdata,d.orderdata";
                    ViewBag.Product = SPA.Database.SqlQuery<Models.GroupProductList>(Product).ToList();
                    fu.setActivityLogAsync("ChooseYourProduct", ControllerName, schlId, "Choose Your product-With Out Login", ((List<Models.GroupProductList>)ViewBag.Product).Select(c => c.ProductId).ToList());
                    return View();
                }
                #endregion
            }
            catch (Exception e)
            {
                Models.Error Error = new Models.Error();
                Error.Schlid = schlId;
                Error.Exception = e.Message;
                fu.ErrorSend(RouteData, e);
                fu.setActivityLogAsync("ChooseYourProduct", ControllerName, schlId, "Choose Your product-Error", Error);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        [CustomAutohrize("4")]
        public ActionResult ChooseEmployee()
        {
            ActionName = "ChooseEmployee";
            long UserId = 0;
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                               .FirstOrDefault();
                #region Language
                var LanguageId = fu.getLanguageId(ShopInfo.Lang_id.Value, null);
                ViewBag.ShopInfo = ShopInfo;
                ViewBag.Currency = fu.currency(ShopInfo.Currency);
                ViewBag.BlockMonths = ShopInfo.AdvBookingRestrict;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Choose_Employee" || (c.Page_Name == "Title" && c.Order_id == 4)) && c.Lang_id == LanguageId).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                if (LanguageId == 1 || LanguageId == 0) { ViewBag.LocalLanguage = "en"; }
                if (LanguageId == 2) { ViewBag.LocalLanguage = "de"; }
                if (LanguageId == 3) { ViewBag.LocalLanguage = "fr"; }
                #endregion
                //if (Convert.ToInt32(Session["RoleId"].ToString()) == 4)
                //{
                int ReservationId = Convert.ToInt32(Session["ResId"]);
                List<string> ItemProductDetail = new List<string>();
                List<string> ProductsToSearch = new List<string>();
                UserId = Convert.ToInt64(Session["UserId"].ToString());
                string ProListShow = "select a.CLIENT_UserId,a.SchId,b.CatgTypeId,b.CatgDesc,c.SectionName,c.SectionDesc,CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN Amount ELSE NULL END AS decimal(38,2)) as Amount,cast(c.Duration as int) as Duration  from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on a.Product_Id=b.CatgTypeId  join ctsp_solutionMaster c on c.CatgTypeId=b.CatgTypeId  where a.ActiveStatus='DA' and a.BookedStatus='T' and a.SchId=" + schlId + " and a.EmpSchDetailsId=" + ReservationId + "";
                var Prodlist = SPA.Database.SqlQuery<Models.ProductList>(ProListShow).ToList();
                //Employee Information
                //string EmpInformation = "select d.UserId,e.FirstName,e.LastName,e.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on a.Product_Id = b.CatgTypeId join Ctsp_solutionMaster c on c.CatgTypeId = b.CatgTypeId join CTSp_solutionMaster d on d.SectionDesc = b.CatgDesc join CCTSP_User e on e.UserId = d.UserId join SPA_SchedulerSlotMaster f on f.UserId=e.UserId where a.EmpSchDetailsId = " + ReservationId + " and d.SchId = " + schlId + " and b.DomainType = " + schlId + " and e.SchId = " + schlId + " and c.SchId = " + schlId + " and a.schId = " + schlId + " and c.SectionDesc = d.Image and c.Duration = d.Duration and c.Amount = d.Amount and d.ActiveStatus = 'A' and e.ActiveStatus = 'A' and b.ActiveStatus = 'A' and b.ActiveStatus = 'A' and f.schId=" + schlId + " and f.ActiveStatus='A' group by d.UserId,e.FirstName,e.LastName,e.Answer2";
                string EmpInformation = "select d.UserId,e.FirstName,e.LastName,e.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on a.Product_Id = b.CatgTypeId join Ctsp_solutionMaster c on c.CatgTypeId = b.CatgTypeId join CTSp_solutionMaster d on d.SectionDesc = b.CatgDesc join CCTSP_User e on e.UserId = d.UserId and e.RoleId not in (1,4) join SPA_SchedulerSlotMaster f on f.UserId=e.UserId where a.EmpSchDetailsId = " + ReservationId + " and d.SchId = " + schlId + " and b.DomainType = " + schlId + " and e.SchId = " + schlId + " and c.SchId = " + schlId + " and a.schId = " + schlId + " and c.Duration = d.Duration and c.Amount = d.Amount and d.ActiveStatus = 'A' and e.ActiveStatus = 'A' and b.ActiveStatus = 'A' and b.ActiveStatus = 'A'  group by d.UserId,e.FirstName,e.LastName,e.Answer2";
                var EmpList = SPA.Database.SqlQuery<Models.EmployeeList>(EmpInformation).ToList();
                if (Prodlist.Count > 0 && EmpList.Count > 0)
                {
                    //fu.setActivityLogAsync(ActionName, ControllerName, schlId, "Choose Employee- Employee List", EmpList);
                    fu.setActivityLogAsync(ActionName, ControllerName, schlId, "Choose Employee - product List", Prodlist);
                    ViewBag.ProductInfo = Prodlist;
                    ViewBag.Employees = EmpList;
                    return View();
                }
                else
                {
                    var Pending = new Models.PendingApproval() { CLIENT_UserId = UserId, EmpSchDetailsId = ReservationId };
                    fu.setActivityLogAsync(ActionName, ControllerName, schlId, "Choose Employee- Redirect to Choose your prooduct as there is no Data Related to Product or Employee", Pending);
                    return RedirectToAction("ChooseYourProduct", "Home");
                }
                //}
                //else { return RedirectToAction("Login", "Login"); }
            }
            catch (Exception e)
            {
                Models.Error Error = new Models.Error();
                Error.Schlid = schlId;
                Error.UserId = UserId;
                Error.Exception = e.Message;
                fu.setActivityLogAsync("ChooseEmployee", "Home", schlId, "Choose Employee - Error", Error);

                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        [CustomAutohrize("4")]
        public ActionResult ThanksMessage()
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                var Lang_Id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
                var LanguageId = fu.getLanguageId(Lang_Id.Value, null);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Booking_Done" || (c.Page_Name == "Title" && c.Order_id == 6)) && c.Lang_id == LanguageId).ToList();
                ViewBag.LanguageAlertBox = SPA.Language_Label_Detail.Where(c => c.Page_Name == "AlertMsg" && c.Lang_id == LanguageId && (c.Order_id == 26 || c.Order_id == 27 || c.Order_id == 28)).ToList();
                ViewBag.LanguageId = LanguageId;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        [CustomAutohrize("4")]
        public ActionResult ConfirmBooking()
        {
            try
            {
                #region Langauge
                var Lang_Id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
                var LanguageId = fu.getLanguageId(Lang_Id.Value, null);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Confirm_Booking" || (c.Page_Name == "Title" && c.Order_id == 5)) && c.Lang_id == LanguageId).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                #endregion
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

                //if (Convert.ToInt32(Session["RoleId"].ToString()) == 4)
                return View();
                //else
                //    return RedirectToAction("Login", "Login");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        public ActionResult DisplayBookedProduct()
        {
            int clientUserId = 0;
            try
            {
                #region Language
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.ShopInfo = ShopInfo;
                var LangId = fu.getLanguageId(ShopInfo.Lang_id.Value, null);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Confirm_Booking" || c.Page_Name == "Small_calander") && c.ActiveStatus == "A" && c.Lang_id == LangId).ToList();
                #endregion
                if (Session["RoleId"].ToString() == "4")
                {
                    clientUserId = Convert.ToInt32(Session["UserId"].ToString());
                    int ReservationId = Convert.ToInt32(Session["ResId"]);
                    #region NewCode               
                    string DisplayBkPro = "select a.EmpSchDetailsId,a.BookingDate,b.CatgDesc as ProductName,a.FromSlotMasterId ,c.sectiondesc as Productdesc,cast(c.Duration as int) as Duration, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id  join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId where a.BookedStatus = 'BT' and a.ActiveStatus = 'DA' and a.EmpSchDetailsId = " + ReservationId + " and a.schid = " + schlId + " and b.domaintype = " + schlId + " and b.activestatus = 'A' and b.catgid = 111 and c.activestatus = 'A' and c.SchId =" + schlId + " and d.activestatus='A' and d.schid=" + schlId + "";
                    ViewBag.DisplayBookProduct = SPA.Database.SqlQuery<Models.ConfirmModel>(DisplayBkPro).ToList();
                    if (ShopInfo.Currency != null && ShopInfo.Currency != "")
                        ViewBag.currency = ShopInfo.Currency;
                    else
                        ViewBag.currency = "CHF";
                    fu.setActivityLogAsync("DisplayBookedProduct", ControllerName, schlId, "ConfirmBooking - success", ViewBag.DisplayBookProduct);
                    return View();
                    #endregion
                }
                else { return RedirectToAction("Login", "Login"); }
            }
            catch (Exception e)
            {
                Models.Error Error = new Models.Error();
                Error.Schlid = schlId;
                Error.UserId = clientUserId;
                Error.Message = e.Message;
                fu.setActivityLogAsync("DisplayBookedProduct", "Home", schlId, "ConfirmBooking - failure", Error);
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public string BookMyOrder(string Comments, long? ReservationId)
        {
            try
            {
                var query = "";
                //Comments to be added in all the section
                //long ClientUserId = Convert.ToInt64(Session["UserId"].ToString());
                // var BookingDetails = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                string BookedData = "select a.EmpSchDetailsId,a.comment,a.ActiveStatus,a.BookedStatus,a.BookingDate,b.CatgTypeid as ProductId ,b.CatgDesc as ProductName,a.Product_price,a.FromSlotMasterId ,a.ToSlotMasterId,a.Emp_UserId as EMP_UserId ,a.CLIENT_UserId as CLIENT_UserId, c.sectiondesc as Productdesc, cast(c.Duration as int) as Duration,e.FirstName as ClientName, e.LastName as ClientLastName ,f.FirstName as ShopOwnerName , f.LastName as ShopOwnerLastName,e.EmailId as ClientEmail,e.PhoneNo as ClientPhoneNo, FORMAT(a.CreatedOn ,'dd-MM-yyyy hh:mm') as CreatedDate, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId  join CCTSP_User e on e.UserId = a.CLIENT_UserId join CCTSP_User f on f.schid = a.schid where a.schid =" + schlId + "  and b.catgid = 111 and c.SchId = " + schlId + " and d.schid = " + schlId + " and f.schid = " + schlId + " and f.RoleId = 1  and f.ActiveStatus = 'A' and a.EmpSchDetailsId = " + ReservationId + "";
                var BookingDetails = SPA.Database.SqlQuery<Models.ConfirmModel>(BookedData).FirstOrDefault();
                string startdate = DateTime.Parse(BookingDetails.BookingDate + " " + BookingDetails.FromSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                string EndDate = DateTime.Parse(BookingDetails.BookingDate + " " + BookingDetails.ToSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                var ValidationCheck = fu.CheckReservationExistOrNot(startdate, EndDate, BookingDetails.EMP_UserId.Value);
                if (ValidationCheck == "Yes")
                {
                    Session["Remember"] = "Error";
                    return "error";
                }
                else
                {
                    var EnterpriseInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                    //if (!string.IsNullOrEmpty(Comments))
                    //{
                    //    query = "update SPA_EmployeeScheduler set BookedStatus='B',reg_status='D',Comment='" + Comments + "' where CLIENT_UserId=" + BookingDetails.CLIENT_UserId + " and BookedStatus='BT' and ActiveStatus='DA' and SchId=" + schlId + " and product_id is not null";
                    //    if (EnterpriseInfo.BookingApproval == "NO")
                    //        query = "update SPA_EmployeeScheduler set BookedStatus='B',ActiveStatus='A',reg_status='D',Comment='" + Comments + "' where CLIENT_UserId=" + BookingDetails.CLIENT_UserId + " and BookedStatus='BT' and ActiveStatus='DA' and SchId=" + schlId + " and product_id is not null";
                    //}
                    //else
                    //{
                    //    query = "update SPA_EmployeeScheduler set BookedStatus='B',reg_status='D' where CLIENT_UserId=" + BookingDetails.CLIENT_UserId + " and BookedStatus='BT' and ActiveStatus='DA' and SchId=" + schlId + " and product_id is not null";
                    //    if (EnterpriseInfo.BookingApproval == "NO")
                    //        query = "update SPA_EmployeeScheduler set BookedStatus='B',ActiveStatus='A',reg_status='D' where CLIENT_UserId=" + BookingDetails.CLIENT_UserId + " and BookedStatus='BT' and ActiveStatus='DA' and SchId=" + schlId + " and product_id is not null";
                    //}
                    //SPA.Database.ExecuteSqlCommand(query);
                    var Resinfo = SPA.SPA_EmployeeScheduler.Where(c => c.CLIENT_UserId == BookingDetails.CLIENT_UserId && c.BookedStatus.Trim() == "BT" && c.ActiveStatus.Trim() == "DA" && c.SchId == schlId && c.Product_Id != null).FirstOrDefault();
                    if (Resinfo != null)
                    {
                        Resinfo.reg_status = "D";
                        Resinfo.BookedStatus = "B";
                        if (EnterpriseInfo.BookingApproval == "NO")
                            Resinfo.ActiveStatus = "A";
                        if (!string.IsNullOrEmpty(Comments))
                            Resinfo.Comment = Comments;
                        SPA.SaveChanges();
                    }
                    //PaymentDeduction
                    var Status = "DA";
                    if (EnterpriseInfo.BookingApproval != "YES")
                        Status = "A";
                    fu.PaymentDeduction(Convert.ToString(BookingDetails.Amount), ReservationId.Value, EnterpriseInfo, Status);
                    //Add MasterData
                    fu.AddMasterData(BookingDetails, Status, "B", "D");

                    // New Code
                    try
                    {
                        // var data = SPA.SPA_EmployeeScheduler.Where(c => c.CLIENT_UserId == ClientUserId && c.BookedStatus == "B" && c.ActiveStatus == "DA" && c.SchId == schlId).OrderByDescending(c => c.EmpSchDetailsId).FirstOrDefault();
                        var data1 = SPA.CCTSP_User.Where(c => c.UserId == BookingDetails.CLIENT_UserId).FirstOrDefault();
                        string name = Session["UserGender"].ToString() + " " + Session["UserFirstName"].ToString() + " " + Session["UserLastName"].ToString();
                        var checkSMSTEMP = SPA.CCTSP_User.Where(c => c.UserId == BookingDetails.CLIENT_UserId && c.SchId == schlId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                        if (checkSMSTEMP != null)
                        {
                            if (EnterpriseInfo.BookingApproval != "NO")
                                SMS.TemporaryApprove(name, BookingDetails.BookingDate, data1.LoginName);
                            else
                                SMS.Approvebooking(name, BookingDetails.BookingDate, data1.LoginName);
                        }
                        var checkEMAILTEMP = SPA.CCTSP_User.Where(c => c.UserId == BookingDetails.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                        if (checkEMAILTEMP != null)
                        {
                            if (EnterpriseInfo.BookingApproval != "NO")
                                Email.TemporaryApprove(name, BookingDetails.BookingDate, Session["UserEmailId"].ToString(), BookingDetails.EmpSchDetailsId, BookingDetails.FromSlotMasterId);
                            else
                                Email.Approvebooking(name, BookingDetails.BookingDate, Session["UserEmailId"].ToString(), BookingDetails.FromSlotMasterId, BookingDetails.EmpSchDetailsId);
                        }
                        var ReNotification = new Models.ShopInfoJson();
                        if (!string.IsNullOrEmpty(EnterpriseInfo.jsonModel))
                            ReNotification = js.Deserialize<Models.ShopInfoJson>(EnterpriseInfo.jsonModel);
                        if (ReNotification.ReNotification == "YES")
                            Email.ShopOwnerEmail(BookingDetails);
                        fu.setActivityLogAsync("BookMyOrder", "Home", schlId, "BookMyOrder - Success", BookingDetails);
                    }
                    catch (Exception e)
                    {
                        Models.Error Error = new Models.Error();
                        Error.Schlid = schlId;
                        Error.Message = e.Message;
                        fu.setActivityLogAsync("BookMyOrder", "Home", schlId, "BookMyOrder - failure", Error);
                        fu.ErrorSend(RouteData, e);
                    }
                    return "";
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "error";
            }
        }
        public string AddClientProductData(FormCollection user)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                string Msg = "";
                string ProductsTemp = "";
                if (user.Keys.Count != 0)
                {
                    if (Convert.ToInt32(Session["RoleId"].ToString()) == 4)
                    {
                        long UserId1 = Convert.ToInt32(Session["UserId"].ToString());
                        var query = "update SPA_EmployeeScheduler set ActiveStatus='D' where CLIENT_UserId=" + UserId1 + " and ActiveStatus='DA' and BookedStatus='T'";
                        SPA.Database.ExecuteSqlCommand(query);
                    }
                    foreach (var key in user.Keys)
                    {
                        if (Convert.ToInt32(Session["RoleId"].ToString()) == 4)
                        {
                            long UserId = Convert.ToInt32(Session["UserId"].ToString());
                            SPA_EmployeeScheduler Employee = new SPA_EmployeeScheduler();
                            long CatgTypeId = Convert.ToInt64(key);
                            Employee.CLIENT_UserId = UserId;
                            Employee.Product_Id = CatgTypeId;
                            var data = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == CatgTypeId).FirstOrDefault();
                            Employee.Product_price = data.Amount;
                            Employee.SchId = schlId;
                            Employee.CreatedOn = EuropeDate;
                            Employee.ActiveStatus = "DA";
                            Employee.BookedStatus = "T";
                            Employee.reg_status = "D";
                            Employee.UpdatedOn = EuropeDate;
                            SPA.SPA_EmployeeScheduler.Add(Employee);
                            SPA.SaveChanges();
                            Msg = "GoToEmployee";
                            Session["ResId"] = Employee.EmpSchDetailsId;
                        }
                        else
                        {
                            ProductsTemp = ProductsTemp + key + "~";
                            Msg = "GoToLogin";
                        }
                    }
                }
                if (Msg == "GoToLogin")
                {
                    Session["ProductChoosed"] = ProductsTemp.Remove(ProductsTemp.Length - 1, 1);
                }
                return Msg;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return string.Empty;
            }
        }
        public ActionResult GetUserTimeSlot(int? userId, string Day)
        {
            try
            {
                string DayFinal = (DateTime.Parse(Day).DayOfWeek).ToString();
                var data = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == userId && (c.ActiveStatus).Trim() == "A" && c.SchId == schlId && c.CCTSP_SchedulerMaster.WeekDay == DayFinal && (c.CCTSP_SchedulerMaster.ActiveStatus).Trim() == "A" && c.SchId == schlId).ToList().OrderBy(c => c.StartTime).Distinct().ToList();
                ViewBag.DataCount = data.Count;
                return View(data);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult GetAnyTimeSlotData(string Day)
        {
            try
            {
                List<CCTSP_User> userDataListOriginal = new List<CCTSP_User>();
                #region Variable
                List<string> ItemProductDetail = new List<string>();
                List<string> ProductsToSearch = new List<string>();
                List<SPA_SchedulerSlotMaster> TimeSlotPerUser = new List<SPA_SchedulerSlotMaster>();
                long UserId = Convert.ToInt64(Session["UserId"].ToString());
                //get All product
                var GetAllProductBooked = SPA.SPA_EmployeeScheduler.Where(c => c.ActiveStatus == "DA" && c.BookedStatus == "T" && c.CLIENT_UserId == UserId && c.SchId == schlId).ToList();
                long TotalDuration = 0;
                long TotalPrice = 0;
                #endregion
                #region PriceAndAmount
                foreach (var item in GetAllProductBooked)
                {
                    var dataItem = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == item.CCTSP_CategoryDetails.CatgTypeId).FirstOrDefault();
                    TotalDuration = TotalDuration + Convert.ToInt64(dataItem.Duration);
                    TotalPrice = TotalPrice + Convert.ToInt64(dataItem.Amount);
                    if (dataItem.Amount == null)
                    {
                        dataItem.Amount = " ";
                    }
                    if (dataItem.Duration == null)
                    {
                        dataItem.Duration = " ";
                    }
                    ItemProductDetail.Add(dataItem.Amount + "~" + dataItem.Duration);
                    ProductsToSearch.Add(item.CCTSP_CategoryDetails.CatgDesc);
                }
                ViewBag.TotalDuration = TotalDuration;
                ViewBag.TotalPrice = TotalPrice;
                ViewBag.DataProduct = ItemProductDetail;
                #endregion
                #region GetUser
                List<long> UserFinalId = new List<long>();
                string USrIdSyntax = "";
                var UserIdList = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11145 && c.SectionName == "EmployeeProduct" && c.SchId == schlId).Select(c => c.UserId).Distinct();
                String ExistOrNot = "";
                foreach (var itemUser in UserIdList)
                {
                    foreach (var ProductbckUser in ProductsToSearch)
                    {
                        var UserCheck = SPA.CTSP_SolutionMaster.Where(c => c.UserId == itemUser && c.SectionDesc == ProductbckUser && c.Activestatus == "A" && c.CatgTypeId == 11145 && c.SectionName == "EmployeeProduct" && c.SchId == schlId).Select(c => c.UserId).FirstOrDefault();
                        if (UserCheck == null)
                        {
                            ExistOrNot = "No";
                        }
                    }
                    if (ExistOrNot != "No")
                    {
                        UserFinalId.Add(Convert.ToInt64(itemUser));
                        USrIdSyntax = USrIdSyntax + " UserId=" + itemUser + " or";
                    }
                    ExistOrNot = "";
                }
                string FinalWord = "";
                if (USrIdSyntax.Length > 6)
                {
                    FinalWord = USrIdSyntax.Remove(USrIdSyntax.Length - 2);
                }
                if (FinalWord != null && FinalWord != "")
                {
                    userDataListOriginal = SPA.CCTSP_User.SqlQuery("select * from dbo.CCTSP_User where(" + FinalWord + ") and RoleId not in (1,4) and ActiveStatus='A' and SchId=" + schlId).ToList();
                    ViewBag.DataImage = userDataListOriginal;
                    if (userDataListOriginal == null)
                    {
                        ViewBag.DataImageCount = 0;
                    }
                    else
                    {
                        ViewBag.DataImageCount = userDataListOriginal.Count();
                    }
                }
                else
                {
                    ViewBag.DataImage = null;
                    ViewBag.DataImageCount = 0;
                }
                #endregion
                #region CalculateTimeSlotCount
                foreach (var ui in userDataListOriginal)
                {
                    string DayFinal = (DateTime.Parse(Day).DayOfWeek).ToString();
                    TimeSlotPerUser.AddRange(SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == ui.UserId && c.ActiveStatus == "A" && c.SchId == schlId && c.CCTSP_SchedulerMaster.WeekDay == DayFinal && c.CCTSP_SchedulerMaster.ActiveStatus == "A" && c.SchId == schlId).OrderBy(c => c.SchedulerId).ToList());
                }
                ViewBag.DataCount = TimeSlotPerUser.Count;
                #endregion
                return View(GetAllProductBooked);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public String GoToNextBookedStatus(string DateBooked, long? EmpUserId, long? ClientUserId, string slotTime)
        {
            try
            {
                string Day = DateTime.Parse(DateBooked).DayOfWeek.ToString();
                TimeSpan startTime = DateTime.Parse(slotTime).TimeOfDay;
                DateTime StartDate = DateTime.Parse(DateBooked + " " + slotTime);
                ReservationController cn = new ReservationController();
                //All Data of Client
                var ClinetAllData = SPA.SPA_EmployeeScheduler.Where(c => c.ActiveStatus == "DA" && c.BookedStatus == "T" && c.CLIENT_UserId == ClientUserId && c.SchId == schlId).ToList();
                //int TempVariable = 0;
                foreach (var item in ClinetAllData)
                {
                    var DataProduct = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == item.Product_Id && c.SchId == schlId).FirstOrDefault();
                    TimeSpan TempTime = new TimeSpan(0, 0, Convert.ToInt32(DataProduct.Duration), 0, 0);
                    TimeSpan FinalTempTime = (startTime + TempTime);
                    DateTime time = DateTime.Today.Add(FinalTempTime);
                    DateTime EndDate = DateTime.Parse(DateBooked + " " + FinalTempTime);
                    var Checkdata = fu.CheckReservationExistOrNot(StartDate.ToString("yyyy-MM-dd HH:mm:ss"), EndDate.ToString("yyyy-MM-dd HH:mm:ss"), EmpUserId.Value);
                    //Further Check Exist or not or Insert Reservation
                    #region Existornot
                    if (Checkdata.ToLower() == "yes")
                    {
                        Session["Remember"] = "Error";
                        return "Error";
                    }
                    else
                    {
                        var TimeOfEmployee = SPA.SPA_SchedulerSlotMaster.Where(c => c.CCTSP_SchedulerMaster.WeekDay == Day && c.UserId == EmpUserId && c.SchId == schlId && c.ActiveStatus == "A" && c.CCTSP_SchedulerMaster.ActiveStatus == "A" && c.StartTime >= FinalTempTime).OrderBy(c => c.StartTime).FirstOrDefault();
                        TimeSpan EndTime = TimeOfEmployee.StartTime.Value;
                        fu.BookedDataUpdate(startTime.ToString(), EndTime.ToString(), TimeOfEmployee.SchedulerId, EmpUserId, ClientUserId, item.Product_Id, StartDate.ToString("dd MMMM, yyyy"));
                        //var TimeOfEmployee = SPA.SPA_SchedulerSlotMaster.Where(c => c.CCTSP_SchedulerMaster.WeekDay == Day && c.UserId == EmpUserId && c.SchId == schlId && c.ActiveStatus == "A" && c.CCTSP_SchedulerMaster.ActiveStatus == "A").OrderBy(c => c.StartTime).ToList();
                        //string status = "No";
                        //foreach (var TimeTempData in TimeOfEmployee)
                        //{
                        //    if (TimeTempData.StartTime >= FinalTempTime && status == "No" && TempVariable != 0)
                        //    {
                        //        fu.BookedDataUpdate(startTime.ToString(), TimeTempData.StartTime.ToString(), TimeTempData.SchedulerId, EmpUserId, ClientUserId, item.Product_Id, DateBooked);
                        //        startTime = DateTime.Parse(TimeTempData.StartTime.ToString()).TimeOfDay;
                        //        status = "yes";
                        //    }
                        //    if (TimeTempData.StartTime == startTime && TempVariable == 0)
                        //    {
                        //        fu.BookedDataUpdate(startTime.ToString(), FinalTempTime.ToString(), TimeTempData.SchedulerId, EmpUserId, ClientUserId, item.Product_Id, DateBooked);
                        //        startTime = DateTime.Parse(FinalTempTime.ToString()).TimeOfDay;
                        //        status = "yes";
                        //        TempVariable = 1;
                        //    }
                        //}

                    }

                    #endregion
                }
                return "Done";
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "Error";
            }
        }
        public string BlockDateofCalander(string Status, string PartMonYear, long UserId, int? StatusTxt)
        {
            try
            {
                bool? AllowPast = Convert.ToInt32(Session["RoleId"].ToString()) == 4?false:SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.AllOW_PastDate).FirstOrDefault();
                int statusText = 0;
                if (StatusTxt == null || StatusTxt == 1) { statusText = 1; }
                var daysForUser = SPA.CTSP_SolutionMaster.Where(c => c.UserId == UserId && c.SchId == schlId && c.SectionName == "EmployeeDays" && c.CatgTypeId == 11143 && c.Activestatus == "A").ToList();

                string MSg = "";
                string[] ListOfPart = PartMonYear.Split(',');
                string Month = ListOfPart[0].Trim();
                long Year = Convert.ToInt64(ListOfPart[1].Trim());
                List<string> blockdate = new List<string>();
                int CatgtypeId = 0;
                if (Status == "cmp")
                {
                    CatgtypeId = 11154;
                }
                var data = SPA.SPA_LeaveMaster.Where(c => c.HolidayTypeId == CatgtypeId && c.ActiveStatus == "A" && c.SchId == schlId);
                if (Status == "Emp")
                {
                    CatgtypeId = 11147;
                    data = SPA.SPA_LeaveMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId && (c.HolidayTypeId == 11154 || c.UserId == UserId));
                }
                if (Status == "cmpEmp")
                {
                    data = SPA.SPA_LeaveMaster.Where(c => c.ActiveStatus == "A" && (c.HolidayTypeId == 11154 || c.UserId == UserId) && c.SchId == schlId);
                }
                if (data.Count() > 0)
                {
                    List<SPA_LeaveMaster> Result = data.ToList().Where(c => (enGB.DateTimeFormat.GetMonthName(c.StartDate.Value.Month) == Month && c.StartDate.Value.Year == Year) || (enGB.DateTimeFormat.GetMonthName(c.EndDate.Value.Month) == Month && c.EndDate.Value.Year == Year)).ToList();
                    foreach (var item in Result)
                    {
                        List<DateTime> DTCHK = new List<DateTime>();
                        #region CheckStartDateandEndDate
                        DateTime startdateRTemp = DateTime.Parse(item.StartDate.ToString());
                        DateTime EndDateRTemp = DateTime.Parse(item.EndDate.ToString());
                        while (startdateRTemp <= EndDateRTemp)
                        {
                            if (enGB.DateTimeFormat.GetMonthName(startdateRTemp.Month) == Month && DateTime.Parse(item.StartDate.ToString()).Year == Year)
                            {
                                MSg = MSg + (DateTime.Parse(startdateRTemp.ToString()).Day).ToString() + "~";
                            }
                            startdateRTemp = startdateRTemp.AddDays(1);
                        }
                        #endregion
                    }
                }
                if (daysForUser != null)
                {
                    DateTime BlockPastDate = DateTime.Parse("01 " + Month + "," + Convert.ToInt32(Year));
                    string DateTemp = DateTime.DaysInMonth(BlockPastDate.Year, BlockPastDate.Month).ToString();
                    DateTime EndBlockPasteDate = DateTime.Parse(DateTemp + " " + Month + "," + BlockPastDate.Year);
                    //By Default 3 month restriction is given
                    DateTime setDateTime = new DateTime();
                    if (daysForUser.FirstOrDefault().CCTSP_SchoolMaster.AdvBookingRestrict != null)
                        setDateTime = DateTime.Now.AddMonths(daysForUser.FirstOrDefault().CCTSP_SchoolMaster.AdvBookingRestrict.Value);
                    else
                        setDateTime = DateTime.Now.AddMonths(3);

                    while (BlockPastDate <= EndBlockPasteDate)
                    {
                        string week_Day = DateTime.Parse(BlockPastDate.ToString()).DayOfWeek.ToString();
                        var AvailableDays = daysForUser.Where(c => c.SectionDesc == week_Day).Select(c => c.SectionDesc);
                        if (AvailableDays.Count() == 0)
                        {
                            MSg = MSg + (DateTime.Parse(BlockPastDate.ToString()).Day).ToString() + "~";
                        }
                        if (statusText == 1)
                        {
                            if (setDateTime < BlockPastDate)
                            {
                                MSg = MSg + (DateTime.Parse(BlockPastDate.ToString()).Day).ToString() + "~";
                            }
                        }
                        BlockPastDate = BlockPastDate.AddDays(1);
                    }
                }
                DateTime BlockPastDateForCurrent = DateTime.Parse("01 " + Month + "," + Convert.ToInt32(Year));
                if (AllowPast != true)
                {
                    while (BlockPastDateForCurrent < DateTime.Today)
                    {
                        if (enGB.DateTimeFormat.GetMonthName(BlockPastDateForCurrent.Month) == Month && DateTime.Parse(BlockPastDateForCurrent.ToString()).Year == Year)
                        {
                            MSg = MSg + (DateTime.Parse(BlockPastDateForCurrent.ToString()).Day).ToString() + "~";
                        }
                        BlockPastDateForCurrent = BlockPastDateForCurrent.AddDays(1);
                    }
                }
                if (MSg.Length > 0)
                {
                    MSg = MSg.Remove(MSg.Length - 1, 1);
                }
                return MSg;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return string.Empty;
            }
        }
        //public JObject GetTimeBlocking(long? UserId, string Date)
        //{
        //    JObject send = null;
        //    StringBuilder sb = null;
        //    SPA_LeaveMaster user = new SPA_LeaveMaster();
        //    string Day = DateTime.Parse(Date).DayOfWeek.ToString();
        //    string MSG = "";
        //    List<string> MsgList = new List<string>();
        //    var data = SPA.SPA_EmployeeScheduler.Where(c => c.EMP_UserId == UserId && c.BookingDate == Date && c.SchId == schlId && ((c.ActiveStatus == "DA" && c.BookedStatus == "B") || (c.ActiveStatus == "A" && c.BookedStatus == "B") || (c.ActiveStatus == "C" && c.BookedStatus == "C")));
        //    var MSG1Temp = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.CCTSP_SchedulerMaster.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId).ToList();
        //    if (data.Count() != 0)
        //    {
        //        foreach (var item in data)
        //        {
        //            MsgList.Add(item.FromSlotMasterId.Remove(item.FromSlotMasterId.Length - 3, 3));
        //            TimeSpan Start = DateTime.Parse(item.FromSlotMasterId).TimeOfDay;
        //            TimeSpan END = DateTime.Parse(item.ToSlotMasterId).TimeOfDay;
        //            var MsgTemp = MSG1Temp.Where(d => d.StartTime >= Start && d.StartTime < END).Select(c => c.StartTime).ToList();
        //            foreach (var TempSlot in MsgTemp)
        //            {
        //                MsgList.Add(TempSlot.ToString().Remove(TempSlot.ToString().Length - 3, 3));
        //            }

        //        }
        //        foreach (var DtTimeSlot in MsgList.Distinct()) { MSG = MSG + DtTimeSlot + "~"; }
        //    }
        //    DateTime CurrentDate = fu.ZonalDate(null);
        //    if (DateTime.Parse(Date).Day == CurrentDate.Day && DateTime.Parse(Date).Month == CurrentDate.Month && DateTime.Parse(Date).Year == CurrentDate.Year)
        //    {
        //        TimeSpan CurrentStart = DateTime.Parse(CurrentDate.ToString()).TimeOfDay;
        //        var CurrentDateTemp = MSG1Temp.Where(d => d.StartTime < CurrentStart).Select(c => c.StartTime).ToList();
        //        foreach (var CurrentTimeSlot in CurrentDateTemp)
        //        {
        //            MsgList.Add(CurrentTimeSlot.ToString().Remove(CurrentTimeSlot.ToString().Length - 3, 3));
        //        }
        //        foreach (var DtTimeSlot in MsgList.Distinct()) { MSG = MSG + DtTimeSlot + "~"; }
        //    }
        //    if (MSG != "" && MSG != null)
        //    {
        //        MSG = MSG.Remove(MSG.Length - 1, 1);
        //    }
        //    sb = new StringBuilder();
        //    sb.Append("{");
        //    sb.Append("\"startTime\":\"" + MSG + "\"");
        //    sb.Append("}");
        //    send = JObject.Parse(sb.ToString());
        //    return send;
        //}
        public JObject GetTimeBlocking(long? UserId, string Date)
        {
            JObject send = fu.Timeblocking(UserId, Date, 0);
            return send;
        }
        //public JObject GetTimeBlockingUpdate(long? UserId, string Date, int ReservationId)
        //{
        //    JObject send = null;
        //    StringBuilder sb = null;
        //    SPA_LeaveMaster user = new SPA_LeaveMaster();
        //    string Day = DateTime.Parse(Date).DayOfWeek.ToString();
        //    string MSG = "";
        //    List<string> MsgList = new List<string>();
        //    var data = SPA.SPA_EmployeeScheduler.Where(c => c.EMP_UserId == UserId && c.BookingDate == Date && c.SchId == schlId && c.EmpSchDetailsId != ReservationId && ((c.ActiveStatus == "DA" && c.BookedStatus == "B") || (c.ActiveStatus == "A" && c.BookedStatus == "B") || (c.ActiveStatus == "C" && c.BookedStatus == "C")));
        //    var MSG1Temp = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.CCTSP_SchedulerMaster.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId).ToList();
        //    if (data.Count() != 0)
        //    {
        //        foreach (var item in data)
        //        {
        //            MsgList.Add(item.FromSlotMasterId.Remove(item.FromSlotMasterId.Length - 3, 3));
        //            TimeSpan Start = DateTime.Parse(item.FromSlotMasterId).TimeOfDay;
        //            TimeSpan END = DateTime.Parse(item.ToSlotMasterId).TimeOfDay;
        //            var MsgTemp = MSG1Temp.Where(d => d.StartTime >= Start && d.StartTime < END).Select(c => c.StartTime).ToList();
        //            foreach (var TempSlot in MsgTemp)
        //            {
        //                MsgList.Add(TempSlot.ToString().Remove(TempSlot.ToString().Length - 3, 3));
        //            }
        //        }
        //        foreach (var DtTimeSlot in MsgList.Distinct())
        //        {
        //            MSG = MSG + DtTimeSlot + "~";
        //        }
        //    }
        //    DateTime CurrentDate = fu.ZonalDate(null);
        //    if (DateTime.Parse(Date).Day == CurrentDate.Day && DateTime.Parse(Date).Month == CurrentDate.Month && DateTime.Parse(Date).Year == CurrentDate.Year)
        //    {
        //        TimeSpan CurrentStart = DateTime.Parse(CurrentDate.ToString()).TimeOfDay;
        //        var CurrentDateTemp = MSG1Temp.Where(d => d.StartTime < CurrentStart).Select(c => c.StartTime).ToList();
        //        foreach (var CurrentTimeSlot in CurrentDateTemp)
        //        {
        //            MsgList.Add(CurrentTimeSlot.ToString().Remove(CurrentTimeSlot.ToString().Length - 3, 3));
        //        }
        //        foreach (var DtTimeSlot in MsgList.Distinct())
        //        {
        //            MSG = MSG + DtTimeSlot + "~";
        //        }
        //    }
        //    if (MSG != "" && MSG != null)
        //    {
        //        MSG = MSG.Remove(MSG.Length - 1, 1);
        //    }
        //    sb = new StringBuilder();
        //    sb.Append("{");
        //    sb.Append("\"startTime\":\"" + MSG + "\"");
        //    sb.Append("}");
        //    send = JObject.Parse(sb.ToString());
        //    return send;
        //}
        public JObject GetTimeBlockingUpdate(long? UserId, string Date, int ReservationId)
        {
            JObject send = fu.Timeblocking(UserId, Date, ReservationId);
            return send;
        }
        public void DeleteProductFromBook(long? Id)
        {
            try
            {
                var query = "update SPA_EmployeeScheduler set ActiveStatus='D' where EmpSchDetailsId=" + Id + " and SchId=" + schlId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public ActionResult GoBackEmployee(string UserId)
        {
            try
            {
                long User = Convert.ToInt32(UserId);
                var GoBackEmployeeQuery = "update SPA_EmployeeScheduler set ActiveStatus ='DA',BookedStatus ='T' where CLIENT_UserId=" + User + " and BookedStatus='BT' and SchId='" + schlId + "' and ActiveStatus = 'DA'";
                SPA.Database.ExecuteSqlCommand(GoBackEmployeeQuery);
                return RedirectToAction("ChooseEmployee", "Home");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public long CheckDurationDifference(long? EmpUserId, string Date)
        {
            try
            {
                long Difference = 0;
                string Day = DateTime.Parse(Date).DayOfWeek.ToString();
                var CheckData = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == EmpUserId && c.ActiveStatus == "A" && c.SchId == schlId && c.CCTSP_SchedulerMaster.WeekDay == Day).Take(4).ToList();
                #region CheckCount
                if (CheckData.Count() >= 2)
                {
                    var StartDiff = CheckData.Select(c => c.StartTime).Distinct().Take(2).OrderBy(c => c).ToList();
                    Difference = Convert.ToInt64((StartDiff.Last() - StartDiff.First()).Value.TotalMinutes);
                    return Difference;
                }
                #endregion
                else
                {
                    string SlotDuration = CheckData.First().SlotDue;
                    #region Half
                    if (SlotDuration.Contains("Half"))
                    {
                        if (SlotDuration.Contains("1"))
                        {
                            var CheckDataParameter = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == EmpUserId);
                            if (CheckDataParameter.Count() != 0)
                            {
                                Difference = Convert.ToInt64((CheckDataParameter.First().EndTime - CheckDataParameter.First().StartTime).Value.TotalMinutes);
                            }
                            else
                            {
                                var checkDataPara = SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId);
                                Difference = Convert.ToInt64((checkDataPara.First().EndTime - checkDataPara.First().StartTime).Value.TotalMinutes);
                            }
                            return Difference;
                        }
                        if (SlotDuration.Contains("2"))
                        {
                            var CheckDataParameter = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == EmpUserId);
                            if (CheckDataParameter.Count() != 0)
                            {
                                Difference = Convert.ToInt64((CheckDataParameter.Last().EndTime - CheckDataParameter.Last().StartTime).Value.TotalMinutes);
                            }
                            else
                            {
                                var checkDataPara = SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId);
                                Difference = Convert.ToInt64((checkDataPara.Last().EndTime - checkDataPara.Last().StartTime).Value.TotalMinutes);
                            }
                            return Difference;
                        }
                    }
                    #endregion
                    #region Day
                    if (SlotDuration.Contains("Day"))
                    {
                        var CheckDataParameter = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == EmpUserId);
                        if (CheckDataParameter.Count() != 0)
                        {
                            Difference = Convert.ToInt64((CheckDataParameter.Last().EndTime - CheckDataParameter.First().StartTime).Value.TotalMinutes);
                        }
                        else
                        {
                            var checkDataPara = SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == Day && c.ActiveStatus == "A" && c.SchId == schlId);
                            Difference = Convert.ToInt64((checkDataPara.Last().EndTime - checkDataPara.First().StartTime).Value.TotalMinutes);
                        }
                        return Difference;
                    }
                    #endregion
                    return Difference;
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return 0;
            }
        }
        /*still left to implement*/
        public string GetAllAvailableforANY(string Month, string Year)
        {
            List<string> DaysofWeek = new List<string>();
            List<DateTime> Date = new List<DateTime>();
            var USERLIST = SPA.CCTSP_User.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.RoleId == 3);
            var UserIdList = USERLIST.Select(c => c.UserId);
            var SlotMaster = SPA.SPA_SchedulerSlotMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId && UserIdList.Contains(c.UserId));
            var LeaveList = SPA.SPA_LeaveMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId && UserIdList.Contains(c.UserId.Value));
            var HolidayList = SPA.SPA_LeaveMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.HolidayTypeId == 11154);
            foreach (var item in DaysofWeek)
            {
                var GetUsersforDay = SlotMaster.Where(c => c.CCTSP_SchedulerMaster.WeekDay == item).Select(c => c.UserId).Distinct();
                foreach (var User in GetUsersforDay)
                {

                }
            }
            return "";
        }
        public string DefaultTime()
        {
            DateTime EuropeDate = fu.ZonalDate(null);
            string Result = "No";
            string OfficalDate = "";
            DateTime DateCheck = EuropeDate;
            do
            {
                string DayofWeekCheck = DateCheck.DayOfWeek.ToString();
                var User = (from a in SPA.CCTSP_User where a.ActiveStatus == "A" && a.SchId == schlId && a.RoleId == 3 select a.UserId).ToList();
                var UserLeaves = (from c in SPA.SPA_LeaveMaster where User.Contains(c.UserId.Value) && c.SchId == schlId && c.ActiveStatus == "A" && c.StartDate >= DateCheck && c.EndDate <= DateCheck select c).ToList();
                var SlotMaster = (from b in SPA.SPA_SchedulerSlotMaster where b.ActiveStatus == "A" && User.Contains(b.UserId) && b.CCTSP_SchedulerMaster.WeekDay == DayofWeekCheck select b.SchedulerId).Distinct().ToList();
                if (UserLeaves.Count == 0 && SlotMaster.Count > 0)
                {
                    Result = "Yes";
                    OfficalDate = DateCheck.ToShortDateString();
                }
                else
                    DateCheck = DateCheck.AddDays(1);
            } while (Result == "No");
            return OfficalDate;
        }
        public int RestrictionBooking()
        {
            var SchoolMaster = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.AdvBookingRestrict).FirstOrDefault();
            if (SchoolMaster == null)
                SchoolMaster = 3;
            return SchoolMaster.Value;
        }
        public ActionResult Payment()
        {
            return View();
        }
        public ActionResult GetTimeForEmployee(string startDateTime, long? UserId, string Durations, long? ReservationId)
        {
            try
            {
                var startDate = Convert.ToDateTime(startDateTime).ToString("yyyy-MM-dd");
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    TimeZone = c.TimeZone,
                    AllowPastDate = c.AllOW_PastDate
                }).FirstOrDefault();
                string Currentdate = fu.ZonalDate(ShopInfo?.TimeZone).AddMinutes(30).ToString("yyyy-MM-dd HH:mm:ss");
                //string TimeSlot = "select distinct(format(convert(datetime,b.StartTime),'HH:mm')) as T1,b.StartTime as T2,d.Scheduler_Time_Id,b.SlotDue " +
                //                   " , CASE WHEN b.StartTime>=(format(convert(datetime, '" + startDateTime + "'), 'HH:mm')) THEN 1 ELSE 0 END as Status " +
                //                   "from SPA_SchedulerSlotMaster b " +
                //                    "join cctsp_schedulerMaster c on c.WeekDay = datename(dw, convert(Date, '" + startDate + "')) and c.SchId = b.SchId " +
                //                    " and c.ActiveStatus = 'A' and c.SchedulerId = b.SchedulerId " +
                //                    " join spa_employeeSchedulers d on d.WeekDay = datename(dw, convert(Date, '" + startDate + "')) and d.ActiveStatus = 'A' and " +
                //                    " d.UserId = " + UserId + " and b.Starttime between d.starttime and d.endtime " +
                //                    " where b.UserId = " + UserId + " and b.ActiveStatus = 'A' " +
                //                   " and cast(concat('" + DateTime.Parse(startDate).ToString("yyyy-MM-dd") + "', ' ', cast(b.StartTime as nvarchar(8))) as Datetime) > convert(datetime, '" + Currentdate + "') " +
                //                   " and b.StartTime not in " +
                //                   " ( " +
                //                   " SELECT b.Starttime from spa_employeeScheduler a " +
                //                   " where convert(Date, a.BookingDate) = convert(Date, '" + startDate + "') and " +
                //                   " ( " +
                //                   " (a.ActiveStatus = 'A' and a.BookedStatus = 'B') or(a.ActiveStatus = 'C' and a.BookedStatus = 'C') or " +
                //                   " (a.ActiveStatus = 'DA' and a.BookedStatus = 'B')) ";
                //if (ReservationId > 0 || ReservationId != null)
                //    TimeSlot = TimeSlot + "and a.EmpSchDetailsId!=" + ReservationId;
                //TimeSlot = TimeSlot + " and b.StartTime >= convert(time, a.FromSlotMasterId) and b.StartTime < convert(time, a.ToSlotMasterId) and a.Emp_UserId = " + UserId +
                //" )";
                //TimeSlot = TimeSlot + " order by b.StartTime";
                bool AllowPastDate = ShopInfo.AllowPastDate;
                if (Convert.ToInt32(Session["RoleId"].ToString()) == 4)
                    AllowPastDate = false;
                string TimeSlot = new Sql().GetTimeForEmployee(UserId, startDateTime, startDate, Currentdate, AllowPastDate, ReservationId);
                var GetEmpTime = new List<Models.TimeSlots>();
                if (!string.IsNullOrEmpty(TimeSlot))
                    GetEmpTime = SPA.Database.SqlQuery<Models.TimeSlots>(TimeSlot).ToList();
                List<int> DurationList = Durations.Split(',').Select(c => int.Parse(c)).ToList();
                List<int> FinalDuration = new List<int>();

                if (GetEmpTime.Count > 0)
                {
                    int ETimeSlot = GetEmpTime.Where(c => c.SlotDue != null && c.SlotDue.Contains("Minutes") || c.SlotDue.Contains("Minute"))
                                              .Select(c => int.Parse(c.SlotDue.Replace(" Minutes", "").Replace(" Minute", ""))).FirstOrDefault();
                    int TotalDuration = DurationList.Sum();
                    if (DurationList.Count > 1)
                    {
                        foreach (var Slot in DurationList)
                        {
                            var Remaining = (double)Slot / ETimeSlot;
                            if (Remaining.ToString().Contains("."))
                                FinalDuration.Add(ETimeSlot * ((Slot / ETimeSlot) + 1));
                            else
                                FinalDuration.Add(Convert.ToInt16(ETimeSlot * Remaining));
                        }
                        TotalDuration = FinalDuration.Sum();
                    }
                    DateTime EXPDate = DateTime.Parse(startDateTime).AddMinutes(TotalDuration);
                    DateTime tDate = DateTime.Parse(startDateTime);
                    if (EXPDate.ToShortDateString() == tDate.ToShortDateString())
                    {
                        #region Old Code
                        foreach (var item in GetEmpTime.Select(c => c.Scheduler_Time_Id).Distinct().ToList())
                        {
                            var SelectTime = GetEmpTime.Where(c => c.Scheduler_Time_Id == item).Select(c => c.T2).ToList();
                            foreach (var TimeEmp in SelectTime)
                            {
                                TimeSpan EXP = DateTime.Parse(TimeEmp.ToString()).AddMinutes(TotalDuration).TimeOfDay;
                                TimeSpan t = DateTime.Parse(TimeEmp.ToString()).TimeOfDay;
                                //int ETimeSlot = Convert.ToInt32(ProductDetails.Timeslot.Replace("Minutes", ""));
                                var ExpTime = t;
                                var count = 0;
                                if (SelectTime.Where(c => c >= EXP).Count() > 0)
                                {
                                    foreach (var ExpectedTime in SelectTime.Where(c => c > t))
                                    {
                                        if (ExpectedTime == DateTime.Parse(ExpTime.ToString()).AddMinutes(ETimeSlot).TimeOfDay)
                                        {
                                            ExpTime = DateTime.Parse(ExpTime.ToString()).AddMinutes(ETimeSlot).TimeOfDay;
                                            count = count + 1;
                                            if (ExpTime >= EXP)
                                                break;
                                            if (SelectTime.Where(c => c > ExpTime).Count() == 0 && ExpTime != EXP)
                                                GetEmpTime.Remove(GetEmpTime.Where(c => c.T2 == t).FirstOrDefault());

                                        }
                                        else
                                        {
                                            if (count == 0)
                                            {
                                                if (!(t <= EXP && DateTime.Parse(t.ToString()).AddMinutes(ETimeSlot).TimeOfDay >= EXP))
                                                    GetEmpTime.Remove(GetEmpTime.Where(c => c.T2 == t).FirstOrDefault());
                                                break;
                                            }
                                            else
                                            {
                                                if (!(ExpTime <= EXP && DateTime.Parse(ExpTime.ToString()).AddMinutes(ETimeSlot).TimeOfDay >= EXP))
                                                    GetEmpTime.Remove(GetEmpTime.Where(c => c.T2 == t).FirstOrDefault());
                                                break;
                                            }
                                        }
                                    }
                                    if (SelectTime.Where(c => c > t).Count() == 0)
                                    {
                                        if (!(t <= EXP && DateTime.Parse(t.ToString()).AddMinutes(ETimeSlot).TimeOfDay >= SelectTime.Where(c => c >= EXP).Select(c => c).FirstOrDefault()))
                                            GetEmpTime.Remove(GetEmpTime.Where(c => c.T2 == t).FirstOrDefault());
                                    }
                                }
                                else
                                    GetEmpTime.Remove(GetEmpTime.Where(c => c.T2 == t).FirstOrDefault());
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        GetEmpTime = new List<Models.TimeSlots>();
                    }
                }
                else
                {
                    GetEmpTime = new List<Models.TimeSlots>();
                }
                return View(GetEmpTime);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                //return Json("", JsonRequestBehavior.AllowGet);
                return RedirectToAction("Error_List", "Error");
            }

        }
        //public string CheckSum(string number)
        public string CheckSum(string Currency, Decimal Amount, string AccountNo)
        {
            //AccountNo = "01-39139-1";
            //int digits = number.Length + 1;
            return fu.MakeESRCODE(Currency, Amount, AccountNo);
            //Convert.ToInt64(number + fu.getCheckDigit(number)).ToString("D" + digits);
        }
    }
}