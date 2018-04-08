using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using SPA.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Data;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.Script.Serialization;

namespace SPA.Controllers
{
    [checkShop]
    public class LoginController : Controller
    {
        int schId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string EmailFrom = (ConfigurationManager.AppSettings["ApplicationVariableEmailBCC"]).ToString();
        CultureInfo enGB = new CultureInfo("en-GB");
        PushEmail Email = new PushEmail();
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        PuchSMS SMS = new PuchSMS();
        Function fu = new Function();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        string ControllerName = "";
        string ActionName = "";
        JavaScriptSerializer js = new JavaScriptSerializer();
        public LoginController()
        {
            ControllerName = "Login";
            schId = Convert.ToInt32(fu.GetShopId(link));
        }
        private const string RememberMeCookieName = "MyCookieName";
        [AllowAnonymous]
        //Language according to user
        public ActionResult Login(string returnUrl, int? Edit, string alertMessage)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                ViewBag.Edit = 0;
                if (CheckForCookieUserName() != "")
                {
                    string getCookie = CheckForCookieUserName();
                    long ActualCookie = Convert.ToInt64(getCookie);
                    var data = SPA.CCTSP_User.Where(b => b.UserId == ActualCookie).FirstOrDefault();
                    ViewBag.EmailId = data.EmailId;
                    ViewBag.password = data.Password;
                }
                if (alertMessage != null && alertMessage != "")
                {
                    ViewBag.alertMessage = "This User already exists.";
                }
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.Edit = Edit;
                var Lang_Id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).Select(c => c.Lang_id).FirstOrDefault();
                var LanguageId = fu.getLanguageId(Lang_Id.Value, null);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Login_Reservation" || (c.Page_Name == "Title" && c.Order_id == 3) || (c.Page_Name == "back_button")) && c.Lang_id == LanguageId).ToList();
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
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult SignIn(LoginModels model)
        {
            ActionName = "SignIn";
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                var result = SignInStatus.Failure;
                var SchoolInformation = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).Select(c => new ShopMasterDetail { Lang_id = c.Lang_id.Value, Color_Id = c.Color_Id, SchlWebsite = c.SchlWebsite, TimeZone = c.TimeZone, timezone_id = c.timezone_id, Flow_Id = c.Flow_Id }).FirstOrDefault();
                //Not Valid
                if (!ModelState.IsValid)
                {
                   
                    //Language Information
                    var LanguageId = fu.getLanguageId(SchoolInformation.Lang_id.Value, null);
                    ViewBag.SchoolInformation = SchoolInformation;
                    ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "login page" || (c.Page_Name == "Title" && c.Order_id == 7) || (c.Page_Name == "back_button")) && c.Lang_id == LanguageId).ToList();
                    ViewBag.LangNameList = fu.LanguageNameList();
                    ViewBag.Colorclass = fu.Colorclass(SchoolInformation.Color_Id);
                    return View(model);
                }
                // return View(model);
                //Valid
                else
                {
                    string password = model.password;
                    if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrEmpty(password))
                    {
                        var data = new CCTSP_User();
                        var dataList = SPA.CCTSP_User.Where(b => b.PhoneNo == model.PhoneNumber && b.Password == password && b.ActiveStatus == "A" && (b.SchId == schId || b.RoleId == 4)).OrderBy(c => c.RoleId).ToList();
                        var CheckPriority = dataList.Where(c => c.RoleId != 4).FirstOrDefault();
                        if (CheckPriority != null)
                            data = CheckPriority;
                        else
                        {
                            var CurrentShop = dataList.Where(c => c.SchId == schId).FirstOrDefault();
                            if (CurrentShop != null)
                                data = CurrentShop;
                            else
                                data = dataList.FirstOrDefault();
                        }
                        if (data != null)
                        {
                            #region ExpiryandTimeZone
                            if (data.CCTSP_SchoolMaster.RegDate != null)
                            {
                                DateTime EuropeDate = fu.ZonalDate(data.CCTSP_SchoolMaster.TimeZone);
                                if (data.CCTSP_SchoolMaster.RegDate < EuropeDate)
                                {
                                    Session["Message"] = "Expire Account";
                                    return RedirectToAction("SignIn", "Login");

                                }
                                if (data.CCTSP_SchoolMaster.RegDate != null)
                                {
                                    if (data.CCTSP_SchoolMaster.RegDate.Value.AddDays(-7) < EuropeDate && (data.RoleId == 1 || data.RoleId == 4))
                                    {
                                        Session["Expire"] = "Temp Expire";
                                    }
                                }
                            }
                            #endregion
                            #region Authentication
                            //RoleId set
                            Session["role_id"] = data.RoleId;
                            if (!string.IsNullOrEmpty(model.password) && !string.IsNullOrEmpty(password))
                                result = SignInStatus.Success;
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    #region Success
                                    //for session
                                    if (data.RoleId >= 1)
                                    {
                                        Session["UserId"] = data.UserId;
                                        Session["SchoolId"] = schId;
                                        Session["RoleId"] = data.RoleId;
                                        Session["UserEmailId"] = data.EmailId;
                                        Session["UserFirstName"] = data.FirstName;
                                        Session["UserLastName"] = data.LastName;
                                        Session["UserGender"] = data.Gender;
                                        Session["UserName"] = data.LoginName;
                                        Session["PhoneNumber"] = data.PhoneNo;
                                        /*Cookie Data*/
                                        //CookieModel CookieData = new CookieModel();
                                        //CookieData.UserId = data.RoleId;
                                        //CookieData.SchoolId = schId;
                                        //CookieData.RoleId = data.RoleId;
                                        //CookieData.UserEmailId = data.EmailId;
                                        //CookieData.UserFirstName = data.FirstName;
                                        //CookieData.UserLastName = data.LastName;
                                        //CookieData.UserGender = data.Gender;
                                        //CookieData.UserName = data.LoginName;
                                        //CookieData.PhoneNumber = data.PhoneNo;
                                        //fu.SetCookie("Auth", js.Serialize(CookieData), TimeSpan.FromDays(30));
                                    }

                                    if (data.RoleId == 4)
                                    {
                                        bool ReservationTabAccess = true;
                                        if (SchoolInformation.Flow_Id > 1)
                                            ReservationTabAccess = fu.CheckTabAccess("Reservation", SchoolInformation.Flow_Id.Value);
                                        if(ReservationTabAccess==true)
                                        {
                                            string name = data.Gender + " " + data.FirstName + " " + data.LastName;
                                            var checkEmail = SPA.CCTSP_User.Where(c => c.UserId == data.UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                                            if (schId != data.SchId)
                                            {
                                                List<string> SchlidList = new List<string>();
                                                if (checkEmail.Other_Schid != null)
                                                    SchlidList = checkEmail.Other_Schid.Split(',').ToList();
                                                if (!SchlidList.Contains(schId.ToString()))
                                                {
                                                    SchlidList.Add(schId.ToString());
                                                    checkEmail.Other_Schid = string.Join(",", SchlidList);
                                                    UpdateModel(checkEmail);
                                                    SPA.SaveChanges();
                                                }
                                            }
                                            //start Activity Log for Customer

                                            fu.setActivityLogAsync(ActionName, ControllerName, schId, "Logged in as Customer", model);
                                            if (checkEmail != null)
                                                Email.MailForLogin(name, data.EmailId);
                                            // Email.Loginmail(name, null, data.EmailId);
                                            Session["Message"] = "Success";
                                            return RedirectToAction("ChooseYourProduct", "Home");
                                        }
                                        else
                                        {
                                            Session["Message"] = "Error";
                                            return RedirectToAction("SignIn", "Login");
                                        }
                                       
                                    }
                                    else if (data.RoleId != 4)
                                    {
                                        Session["Message"] = "Success";
                                        long FlowId = SchoolInformation.Flow_Id != null ? SchoolInformation.Flow_Id.Value : 0;
                                        var RedirctionLink = fu.AccessRightRedirectLink(data.RoleId, FlowId);
                                        //start Activity log for Shopowner
                                        fu.setActivityLogAsync(ActionName, ControllerName, schId, "Logged in as " + data.CCTSP_Role.RoleName, model);
                                        //return RedirectToAction("Reservation", "Reservation");
                                        if (RedirctionLink != null)
                                            return Redirect(RedirctionLink);
                                        else
                                        {
                                            Session["Message"] = "No Access";
                                            return RedirectToAction("SignIn", "Login");
                                        }

                                    }
                                    else
                                    {
                                        Session["Message"] = "Error";
                                        return RedirectToAction("SignIn", "Login");
                                    }
                                #endregion
                                case SignInStatus.Failure:
                                    #region Failure
                                    Session["Message"] = "Error";
                                    return RedirectToAction("SignIn", "Login");
                                #endregion
                                default:
                                    #region Default
                                    Session["Message"] = "Error";
                                    return RedirectToAction("SignIn", "Login");
                                    #endregion
                            }
                            #endregion
                            //return View();
                        }
                        else
                        {
                            fu.setActivityLogAsync(ActionName, ControllerName, schId, "Failed to Login", model);
                            Session["Message"] = "Error";
                            return RedirectToAction("SignIn", "Login");
                        }
                    }
                    return View();
                }
            }
            catch (Exception e)
            {
                fu.setActivityLogAsync(ActionName, ControllerName, schId, "Error while Login", model);
                //error Exception
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModels model)
        {
            var ActionName = "Login";
            try
            {

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                var result = SignInStatus.Failure;
                if (!ModelState.IsValid)
                {
                    var Lang_Id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).Select(c => c.Lang_id).FirstOrDefault();
                    var LanguageId = fu.getLanguageId(Lang_Id.Value, null);
                    ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Login_Reservation" || (c.Page_Name == "Title" && c.Order_id == 3) || (c.Page_Name == "back_button")) && c.Lang_id == LanguageId).ToList();
                    ViewBag.LanguageId = LanguageId;
                    return View(model);
                }
                else
                {
                    string password = model.password;
                    if (!string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrEmpty(password))
                    {
                        var dataList = SPA.CCTSP_User.Where(b => b.PhoneNo == model.PhoneNumber && b.Password == password && b.ActiveStatus == "A" && (b.RoleId == 1 && b.SchId == schId || b.RoleId == 4)).ToList();
                        var CurrentShop = dataList.Where(c => c.SchId == schId).FirstOrDefault();
                        var data = dataList.FirstOrDefault();
                        if (CurrentShop != null)
                            data = CurrentShop;
                        if (data != null)
                        {
                            if (data.CCTSP_SchoolMaster.RegDate != null)
                            {
                                DateTime EuropeDate = fu.ZonalDate(null);
                                if (data.CCTSP_SchoolMaster.RegDate < EuropeDate)
                                {
                                    Session["Message"] = "Expire Account";
                                    return RedirectToAction("Login", "Login");

                                }
                                if (data.CCTSP_SchoolMaster.RegDate != null)
                                {
                                    if (data.CCTSP_SchoolMaster.RegDate.Value.AddDays(-7) < EuropeDate && (data.RoleId == 1 || data.RoleId == 4))
                                    {
                                        Session["Expire"] = "Temp Expire";
                                    }

                                }
                            }
                            Session["role_id"] = data.RoleId;
                            if (!string.IsNullOrEmpty(model.password) && !string.IsNullOrEmpty(password))
                                result = SignInStatus.Success;
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    #region Success
                                    //for session
                                    if (data.RoleId == 4 || data.RoleId == 1)
                                    {
                                        Session["UserId"] = data.UserId;
                                        Session["SchoolId"] = schId;
                                        Session["RoleId"] = data.RoleId;
                                        Session["UserEmailId"] = data.EmailId;
                                        Session["UserFirstName"] = data.FirstName;
                                        Session["UserLastName"] = data.LastName;
                                        Session["UserGender"] = data.Gender;
                                        Session["UserName"] = data.LoginName;
                                        Session["PhoneNumber"] = data.PhoneNo;
                                        /*Cookie Data*/
                                        //CookieModel CookieData = new CookieModel();
                                        //CookieData.UserId = data.RoleId;
                                        //CookieData.SchoolId = schId;
                                        //CookieData.RoleId = data.RoleId;
                                        //CookieData.UserEmailId = data.EmailId;
                                        //CookieData.UserFirstName = data.FirstName;
                                        //CookieData.UserLastName = data.LastName;
                                        //CookieData.UserGender = data.Gender;
                                        //CookieData.UserName = data.LoginName;
                                        //CookieData.PhoneNumber = data.PhoneNo;
                                        //fu.SetCookie("Auth", js.Serialize(CookieData), TimeSpan.FromDays(30));
                                    }

                                    if (data.RoleId == 4)
                                    {
                                        string name = data.Gender + " " + data.FirstName + " " + data.LastName;
                                        var checkEmail = SPA.CCTSP_User.Where(c => c.UserId == data.UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                                        if (schId != data.SchId)
                                        {
                                            List<string> SchlidList = new List<string>();
                                            if (checkEmail.Other_Schid != null)
                                                SchlidList = checkEmail.Other_Schid.Split(',').ToList();
                                            if (!SchlidList.Contains(schId.ToString()))
                                            {
                                                SchlidList.Add(schId.ToString());
                                                checkEmail.Other_Schid = string.Join(",", SchlidList);
                                                UpdateModel(checkEmail);
                                                SPA.SaveChanges();
                                            }
                                        }
                                        //if (checkEmail != null)
                                        //    Email.Loginmail(name, null, data.EmailId);
                                        if (checkEmail != null)
                                            Email.MailForLogin(name, data.EmailId);

                                        Session["Message"] = "Success";
                                        fu.setActivityLogAsync(ActionName, ControllerName, schId, "Logged in as Customer", model);
                                        //Client
                                        return RedirectToAction("ChooseYourProduct", "Home");
                                    }
                                    else if (data.RoleId == 1)
                                    {
                                        Session["Message"] = "Success";
                                        fu.setActivityLogAsync(ActionName, ControllerName, schId, "Logged in as Shopowner", model);
                                        return RedirectToAction("Reservation", "Reservation");
                                    }
                                    else
                                    {
                                        Session["Message"] = "Error";
                                        return RedirectToAction("Login", "Login");
                                    }
                                #endregion
                                case SignInStatus.Failure:
                                    #region Failure
                                    Session["Message"] = "Error";
                                    return RedirectToAction("Login", "Login");
                                #endregion
                                default:
                                    #region Default
                                    Session["Message"] = "Error";
                                    return RedirectToAction("Login", "Login");
                                    #endregion
                            }
                            //return View();


                        }
                        else
                        {
                            fu.setActivityLogAsync(ActionName, ControllerName, schId, "Failed to Login", model);
                            Session["Message"] = "Error";
                            return RedirectToAction("Login", "Login");
                        }
                    }
                    return View();
                }
            }
            catch (Exception e)
            {
                fu.setActivityLogAsync(ActionName, ControllerName, schId, "Error while Login", model);
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        private string CheckForCookieUserName()
        {
            try
            {
                string returnValue = string.Empty;
                HttpCookie rememberMeUserNameCookie = Request.Cookies.Get(RememberMeCookieName);
                if (null != rememberMeUserNameCookie)
                {
                    /* Note, the browser only sends the name/value to the webserver, and not the expiration date */
                    returnValue = rememberMeUserNameCookie.Value;
                }
                return returnValue;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return string.Empty;
            }
        }
        //Language according to user
        public ActionResult Registration()
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                //ViewBag.GenderList = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgName == "Gender" && c.Lang_Id == SchoolInfo.Lang_id).OrderBy(c => c.CatgType), "CatgTypeId", "CatgType");
                var CatgNameList = "Title";
                ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, SchoolInfo.Lang_id.Value);
                var LngLocal = "";
                if (SchoolInfo.Lang_id == 1) { LngLocal = "en"; }
                if (SchoolInfo.Lang_id == 2) { LngLocal = "de"; }
                if (SchoolInfo.Lang_id == 3) { LngLocal = "fr-ca"; }
                ViewBag.LngLocal = LngLocal;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Login_Reservation" && c.Lang_id == SchoolInfo.Lang_id).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == SchoolInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                ViewBag.SchoolInfo = SchoolInfo;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        [HttpPost]
        public ActionResult RegistrationAdd(CCTSP_User user)
        {
            var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).Select(c => c.Lang_id).FirstOrDefault();
            var UserLog = fu.GetRegistrationDetail(user);
            DateTime EuropeDate = fu.ZonalDate(null);
            ActionName = "RegistrationAdd";
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                int LanguageId = Lang_id.Value;
                //  ViewBag.GenderList = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgName == "Gender" && c.Lang_Id == LanguageId).OrderBy(c => c.CatgType), "CatgTypeId", "CatgType");
                var CatgNameList = "Title";
                ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, LanguageId);
                var CheckUserExist = SPA.CCTSP_User.Where(c => c.PhoneNo == user.PhoneNo && c.ActiveStatus == "A" && c.RoleId == 4).FirstOrDefault();
                if (CheckUserExist == null)
                {
                    //int CatTypeIdGender = Convert.ToInt32(user.Gender);
                    #region UserAdd
                    user.ActiveStatus = "A";
                    user.CreatedOn = EuropeDate;
                    user.LoginName = user.PhoneNo;
                    user.RoleId = 4;
                    user.SchId = schId;
                    if (user.DOB == null)
                        user.DOB = DateTime.Parse("0001-01-01", enGB);
                    user.SMS_Service = 1;
                    user.Email_Service = 3;
                    SPA.CCTSP_User.Add(user);
                    SPA.SaveChanges();
                    #endregion
                    Session["UserId"] = user.UserId;
                    Session["SchoolId"] = user.SchId;
                    Session["RoleId"] = user.RoleId;
                    Session["UserEmailId"] = user.EmailId;
                    Session["UserFirstName"] = user.FirstName;
                    Session["UserLastName"] = user.LastName;
                    Session["UserGender"] = user.Gender;
                    Session["UserName"] = user.LoginName;
                    Session["PhoneNumber"] = user.PhoneNo;
                    /*Cookie Data*/
                    //CookieModel CookieData = new CookieModel();
                    //CookieData.UserId = user.RoleId;
                    //CookieData.SchoolId = schId;
                    //CookieData.RoleId = user.RoleId;
                    //CookieData.UserEmailId = user.EmailId;
                    //CookieData.UserFirstName = user.FirstName;
                    //CookieData.UserLastName = user.LastName;
                    //CookieData.UserGender = user.Gender;
                    //CookieData.UserName = user.LoginName;
                    //CookieData.PhoneNumber = user.PhoneNo;
                    //fu.SetCookie("Auth", js.Serialize(CookieData), TimeSpan.FromDays(30));
                    string name = user.Gender + " " + user.FirstName + " " + user.LastName;
                    //SMSCode
                    var checkSMSRegistration = SPA.CCTSP_User.Where(c => c.UserId == user.UserId && c.SchId == schId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                    if (checkSMSRegistration != null)
                        SMS.UserRegistration(name, null, user.LoginName);
                    //EmailCode
                    var checkEMAILRegistration = SPA.CCTSP_User.Where(c => c.UserId == user.UserId && c.SchId == schId).FirstOrDefault();
                    if (checkEMAILRegistration != null)
                        Email.MailForUserRegistration(user.UserId, user.EmailId);
                    //Email.UserRegistration(user.UserId, null, user.EmailId);

                    fu.setActivityLogAsync(ActionName, ControllerName, schId, "Registered Successfully", UserLog);
                    return RedirectToAction("ChooseYourProduct", "Home");
                }
                else
                {
                    fu.setActivityLogAsync(ActionName, ControllerName, schId, "This User is Already Registered", UserLog);
                    Session["Message"] = "Repeat";
                }
                return RedirectToAction("Login", "Login");
            }
            catch (Exception e)
            {
                fu.setActivityLogAsync(ActionName, ControllerName, schId, "Error in Registration Process", UserLog);
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        private void CreateRememberMeCookie(string userName)
        {
            try
            {
                HttpCookie rememberMeCookie = new HttpCookie(RememberMeCookieName, userName);
                rememberMeCookie.Expires = DateTime.MaxValue;
                Response.SetCookie(rememberMeCookie);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        private void RemoveRememberMeCookie()
        {
            try
            {
                /* k1ll the cookie ! */
                HttpCookie rememberMeUserNameCookie = Request.Cookies[RememberMeCookieName];
                if (null != rememberMeUserNameCookie)
                {
                    Response.Cookies.Remove(RememberMeCookieName);
                    rememberMeUserNameCookie.Expires = DateTime.Now.AddYears(-1);
                    rememberMeUserNameCookie.Value = null;
                    Response.SetCookie(rememberMeUserNameCookie);
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        //Language according to user
        [AllowAnonymous]
        public ActionResult SignIn(string returnUrl, int? Edit, string alertMessage)
        {
            try
            {
                ViewBag.Edit = 0;
                var SchoolInformation = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).Select(c => new ShopMasterDetail { Lang_id = c.Lang_id.Value, Color_Id = c.Color_Id, SchlWebsite = c.SchlWebsite, TimeZone = c.TimeZone, timezone_id = c.timezone_id }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "login page" || (c.Page_Name == "Title" && c.Order_id == 7) || (c.Page_Name == "back_button")) && c.Lang_id == SchoolInformation.Lang_id).ToList();
                ViewBag.LangNameList = fu.LanguageNameList();
                ViewBag.Colorclass = fu.Colorclass(SchoolInformation.Color_Id);
                ViewBag.SchoolInformation = SchoolInformation;
                if (alertMessage != null && alertMessage != "")
                {
                    ViewBag.alertMessage = "This User already exists.";
                }
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.Edit = Edit;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        public ActionResult SignUp()
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                var SchoolInformation = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                ViewBag.GenderList = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgName == "Gender" && c.Lang_Id == SchoolInformation.Lang_id).OrderBy(c => c.CatgType), "CatgTypeId", "CatgType");
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "login page" && c.Lang_id == SchoolInformation.Lang_id).ToList();
                ViewBag.Colorclass = fu.Colorclass(SchoolInformation.Color_Id);
                ViewBag.SchoolInformation = SchoolInformation;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        [HttpPost]
        public ActionResult SignUp(CCTSP_User user)
        {
            ActionName = "SignUp";
            var UserLog = fu.GetRegistrationDetail(user);
            try
            {
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                DateTime EuropeDate = fu.ZonalDate(SchoolInfo.TimeZone);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                ViewBag.GenderList = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgName == "Gender" && c.Lang_Id == SchoolInfo.Lang_id).OrderBy(c => c.CatgType), "CatgTypeId", "CatgType");
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "login page" && c.Lang_id == SchoolInfo.Lang_id).ToList();
                ViewBag.Colorclass = fu.Colorclass(SchoolInfo.Color_Id);
                ViewBag.SchoolInformation = SchoolInfo;
                var CheckUserExist = SPA.CCTSP_User.Where(c => c.PhoneNo == user.PhoneNo && c.ActiveStatus == "A" && c.RoleId == 4).FirstOrDefault();
                if (CheckUserExist == null)
                {
                    int CatTypeIdGender = Convert.ToInt32(user.Gender);

                    #region UserAdd
                    user.ActiveStatus = "A";
                    user.CreatedOn = EuropeDate;
                    user.Gender = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatTypeIdGender).Select(c => c.CatgType).FirstOrDefault();
                    user.LoginName = user.PhoneNo;
                    user.RoleId = 4;
                    user.SchId = schId;
                    user.SMS_Service = 1;
                    user.Email_Service = 3;
                    user.DOB = DateTime.Parse("01/01/0001");
                    SPA.CCTSP_User.Add(user);
                    SPA.SaveChanges();
                    #endregion
                    #region Intialization
                    Session["UserId"] = user.UserId;
                    Session["SchoolId"] = user.SchId;
                    Session["RoleId"] = user.RoleId;
                    Session["UserEmailId"] = user.EmailId;
                    Session["UserFirstName"] = user.FirstName;
                    Session["UserLastName"] = user.LastName;
                    Session["UserGender"] = user.Gender;
                    Session["UserName"] = user.LoginName;
                    Session["PhoneNumber"] = user.PhoneNo;
                    /*Cookie Data*/
                    //CookieModel CookieData = new CookieModel();
                    //CookieData.UserId = user.RoleId;
                    //CookieData.SchoolId = schId;
                    //CookieData.RoleId = user.RoleId;
                    //CookieData.UserEmailId = user.EmailId;
                    //CookieData.UserFirstName = user.FirstName;
                    //CookieData.UserLastName = user.LastName;
                    //CookieData.UserGender = user.Gender;
                    //CookieData.UserName = user.LoginName;
                    //CookieData.PhoneNumber = user.PhoneNo;
                    //fu.SetCookie("Auth", js.Serialize(CookieData), TimeSpan.FromDays(30));
                    #endregion
                    string name = user.Gender + " " + user.FirstName + " " + user.LastName;
                    var checkSMSRegistration = SPA.CCTSP_User.Where(c => c.UserId == user.UserId && c.SchId == schId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                    if (checkSMSRegistration != null)
                        SMS.UserRegistration(name, null, user.LoginName);
                    var checkEMAILRegistration = SPA.CCTSP_User.Where(c => c.UserId == user.UserId && c.SchId == schId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                    if (checkEMAILRegistration != null)
                        Email.MailForUserRegistration(user.UserId, user.EmailId);
                    // Email.UserRegistration(user.UserId, null, user.EmailId);
                    fu.setActivityLogAsync(ActionName, ControllerName, schId, "Registered Successfully", UserLog);
                    return RedirectToAction("ChooseYourProduct", "Home");
                }
                else
                {
                    fu.setActivityLogAsync(ActionName, ControllerName, schId, "This User is Already Registered", UserLog);
                    Session["Message"] = "Repeat";
                }
                return RedirectToAction("SignUp", "Login");
            }
            catch (Exception e)
            {
                fu.setActivityLogAsync(ActionName, ControllerName, schId, "Error in Registration Process", UserLog);
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        public ActionResult ForgotPassword()
        {
            try
            {
                ViewBag.ErrorMessage = "";
                var SchoolInformation = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Forgot_Password" || (c.Page_Name == "Title" && c.Order_id == 18)) && c.Lang_id == SchoolInformation.Lang_id).ToList();
                ViewBag.LangNameList = fu.LanguageNameList();
                ViewBag.Colorclass = fu.Colorclass(SchoolInformation.Color_Id);
                ViewBag.ShopInfo = SchoolInformation;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //Language according to user
        //ankit code
        [HttpPost]
        public ActionResult ForgotPassword(UserForgetPassword user)
        {
            ActionName = "ForgotPassword";
            try
            {
                string phoneno = user.PhoneNo;
                var SchoolInformation = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Forgot_Password" && c.Lang_id == SchoolInformation.Lang_id).ToList();
                ViewBag.LangNameList = fu.LanguageNameList();
                ViewBag.Colorclass = fu.Colorclass(SchoolInformation.Color_Id);
                ViewBag.ShopInfo = SchoolInformation;
                var ForgotUpdate = new CCTSP_User();
                var ForgotDetails = SPA.CCTSP_User.Where(c => c.PhoneNo == phoneno && c.ActiveStatus == "A").ToList();
                if (ForgotDetails.Count > 0)
                {
                    ForgotUpdate = ForgotDetails.Where(c => c.PhoneNo == phoneno && c.ActiveStatus == "A" && c.SchId == schId).FirstOrDefault();
                    if (ForgotUpdate == null)
                        ForgotUpdate = ForgotDetails.FirstOrDefault();
                }
                if (ForgotUpdate != null)
                {
                    ForgotUpdate.nint = 1;
                    SPA.SaveChanges();
                    // Email.ForgotPassword(ForgotUpdate.EmailId, ForgotUpdate.UserId);
                    Email.ForgotPassword(ForgotUpdate.EmailId, ForgotUpdate.UserId);
                    ViewBag.AlreadyRegistered = "NO";
                    ViewBag.Emailid = ForgotUpdate.EmailId;
                    fu.setActivityLogAsync(ActionName, ControllerName, schId, "Requested For Forgot Password,Wanted To change Password", user);
                }
                else
                {
                    fu.setActivityLogAsync(ActionName, ControllerName, schId, "Requested For Forgot Password,Wanted To change Password", user);
                    ViewBag.AlreadyRegistered = "YES";
                }
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }

        public ActionResult ChangePassword(long User)
        {
            try
            {
                if (User != 0)
                {
                    var UserData = SPA.CCTSP_User.Where(c => c.UserId == User && c.ActiveStatus == "A" && c.SchId == schId).FirstOrDefault();
                    var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                    ViewBag.Language = fu.getLanguageData("Change_Password", 0, ShopInfo.Lang_id.Value);
                    ViewBag.LangNameList = fu.LanguageNameList();
                    ViewBag.ShopInfo = ShopInfo;
                    if (UserData != null)
                    {
                        if (UserData.nint == 1)
                        {
                            ViewBag.UserId = UserData.UserId;
                            return View();
                        }
                        else
                            return RedirectToAction("LinkExpired", "Login");
                    }
                    else
                        return RedirectToAction("ForgotPassword", "Login", new { User = User });
                }
                else
                    return RedirectToAction("SignIn", "Login");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }

        public void ChangeNewPassword(UserForgetPassword Change)
        {
            ActionName = "ChangeNewPassword";
            try
            {
                string Password = Change.Password;
                string ConfirmPassword = Change.confirmPassword;
                long UserId = Convert.ToInt64(Change.UserIdEdit);
                var UpdatePassword = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schId).FirstOrDefault();
                UpdatePassword.Password = Password;
                UpdatePassword.nint = null;
                SPA.SaveChanges();
                fu.setActivityLogAsync(ActionName, ControllerName, schId, "changed Password", Change);
                ViewBag.changepassword = "YES";
            }
            catch (Exception e)
            {
                fu.setActivityLogAsync(ActionName, ControllerName, schId, "error in changing Password", Change);
                fu.ErrorSend(RouteData, e);
            }
        }
        //public void ChangeNewPassword(FormCollection Change)
        //{
        //    try
        //    {
        //        string Password = Change["Password"];
        //        string ConfirmPassword = Change["confirmPassword"];
        //        long UserId = Convert.ToInt64(Change["UserIdEdit"]);
        //        var UpdatePassword = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schId).FirstOrDefault();
        //        UpdatePassword.Password = Password;
        //        UpdatePassword.nint = null;
        //        SPA.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //    }
        //}
        public ActionResult LinkExpired()
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId).FirstOrDefault();
                ViewBag.ShopInfo = ShopInfo;
                int LangId = ShopInfo.Lang_id.Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "LinkExpired_page" && c.Lang_id == LangId).ToList();
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