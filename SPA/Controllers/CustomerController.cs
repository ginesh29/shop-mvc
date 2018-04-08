using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using SPA.Models;
using System.Globalization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using System.Data;

namespace SPA.Controllers
{
    [checkShop]

    public class CustomerController : Controller
    {
        //schlId
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        CultureInfo enGB = new CultureInfo("en-GB");
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        PuchSMS SMS = new PuchSMS();
        PushEmail Email = new PushEmail();
        Function fu = new Function();
        JavaScriptSerializer js = new JavaScriptSerializer();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public CustomerController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        int TempVariable = 0;
        // GET: Customer
        [CustomAutohrize(null)]
        public ActionResult Customer()
        {
            try
            {
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                                .Select(c => new Models.ShopMasterDetail
                                {
                                    Currency = c.Currency,
                                    Lang_id = c.Lang_id.Value,
                                    TimeZone = c.TimeZone,
                                    Flow_Id = c.Flow_Id
                                }).FirstOrDefault();
                ViewBag.Language = fu.getLanguageData("Customer_Tab", 0, SchoolInfo.Lang_id.Value);
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Lang_id == SchoolInfo.Lang_id.Value && c.Order_id == 11).Select(c => c.Value).FirstOrDefault();
                ViewBag.SubMenu = fu.AccessSubMenu(SchoolInfo.Lang_id.Value, Convert.ToInt32(Session["RoleId"].ToString()), "Customer_Tab", SchoolInfo.Flow_Id);
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult ListView(string OrderBy, int? Sorting)
        {
            try
            {
                //Language Detail
                #region Langauge               
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(d => d.SchlId == schlId && d.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    SchlStudentStrength = c.SchlStudentStrength,
                    TimeZone = c.TimeZone,
                    Display_Invoice = c.Display_Invoice,
                    Flow_Id = c.Flow_Id
                }).FirstOrDefault();
                ViewBag.LangcusList = fu.getLanguageData("Customer_List_view", 0, SchoolInfo.Lang_id.Value);
                ViewBag.LangMonth = fu.getLanguageData("Small_calander", 8, SchoolInfo.Lang_id.Value);
                #endregion
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                ViewBag.CustomerInfo = fu.getCustomerList(OrderBy, Sorting, UserId, "List view");
                //Access of New customer as it is used in this page
                ViewBag.AcesofNewCust = fu.CheckAccessofPage("New Customer", UserId);
                bool ReservationTabAccess = true;
                if (SchoolInfo.Flow_Id > 1)
                    ReservationTabAccess = fu.CheckTabAccess("Reservation", SchoolInfo.Flow_Id.Value);
                ViewBag.CheckReservationTabAccess = ReservationTabAccess;
                ViewBag.OrderBy = OrderBy;
                ViewBag.Sorting = Sorting;
                ViewBag.ShopId = schlId;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult NewCustomer(long? UserId, string Status)
        {
            try
            {
                var CountryVersion = ConfigurationManager.AppSettings["CountryVersionShop"];
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(d => d.SchlId == schlId && d.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    SchlStudentStrength = c.SchlStudentStrength,
                    TimeZone = c.TimeZone,
                    Display_Invoice = c.Display_Invoice,
                    Flow_Id = c.Flow_Id,
                    country = c.Schcountry
                }).FirstOrDefault();
                Models.CustomerDisplay UserInfo = new CustomerDisplay();
                if (UserId > 0)
                {
                    UserInfo = SPA.CCTSP_User.Where(c => c.UserId == UserId)
                        .Select(c => new Models.CustomerDisplay
                        {
                            Title = c.Title,
                            UserId = c.UserId,
                            Salutation = c.Salutation,
                            Gender = c.Gender,
                            FirstName = c.FirstName,
                            FamilyName = c.LastName,
                            PhoneNumber = c.PhoneNo,
                            landlinenumber = c.PasswordQuerry2,
                            Email = c.EmailId,
                            CustPostalCode = c.Pincode,
                            City = c.City,
                            State = c.State,
                            Street = c.Street,
                            Status = Status,
                            StreetNumber = c.StreetNumber,
                            Address = c.Address1,
                            EmailService = c.Email_Service.Value,
                            SMSService = c.SMS_Service.Value,
                            DisplayInvoice = c.Display_Invoice,
                            Password = c.Password == "BCS@1234" ? "" : c.Password,
                            InvoiceService = c.Invoice_Service,
                            CustomerDOB = c.DOB,
                            AHV_Number = c.AHV_Number,
                            VEKA_Number = c.VEKA_Number,
                            InsuranceNumber = c.InsuranceNumber,
                            CusCreatedOn = c.CreatedOn,
                            GLN_No = c.GLN_No,
                            ZSR_No = c.ZSR_No,
                            Country = c.Country
                        }).FirstOrDefault();
                }
                ViewBag.UserInfo = UserInfo;
                ViewBag.CheckInvoice = fu.CheckInvoice(SchoolInfo.Display_Invoice);
                ViewBag.DisplayInvoice = SchoolInfo.Display_Invoice != null ? SchoolInfo.Display_Invoice : 0;
                DateTime EuropeDate = fu.ZonalDate(SchoolInfo.TimeZone);
                var CatgNameList = "Salutation,New_Gender,Title";
                ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, SchoolInfo.Lang_id.Value);
                long FlowId = SchoolInfo.Flow_Id != null ? SchoolInfo.Flow_Id.Value : 0;
                ViewBag.Language = fu.DynamicFieldsAccordingFlow(SchoolInfo.Lang_id.Value, FlowId, "Customer_New_Customer");
                ViewBag.LngLocal = fu.SmallCalLangTranslate(SchoolInfo.Lang_id.Value);
                var countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A").Select(c => new Models.CountryList { Country = c.name_country }).ToList();
                if (CountryVersion == "INDIA")
                    countrylist = countrylist.Where(c => c.Country == "INDIA").ToList();
                else
                    countrylist = countrylist.Where(c => c.Country != "INDIA").ToList();
                ViewBag.countrylist = countrylist;
                bool ReservationTabAccess = true;
                if (SchoolInfo.Flow_Id > 1)
                    ReservationTabAccess = fu.CheckTabAccess("Reservation", SchoolInfo.Flow_Id.Value);
                ViewBag.CheckReservationTabAccess = ReservationTabAccess;
                ViewBag.SchoolInfo = SchoolInfo;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public void NewCustomerAdd(FormCollection user)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                var BirthdayUpdate = "";
                string Status = "No";
                CCTSP_User UserInfo = new CCTSP_User();
                var phoneno = user["NwPhoneNo"];
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(d => d.SchlId == schlId && d.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    SchlStudentStrength = c.SchlStudentStrength,
                    TimeZone = c.TimeZone,
                    Display_Invoice = c.Display_Invoice,
                    Flow_Id = c.Flow_Id
                }).FirstOrDefault();
                if (user["NWEditUserId"] != "")
                {
                    UserInfo.UserId = Convert.ToInt64(user["NWEditUserId"]);
                    UserInfo = SPA.CCTSP_User.Where(c => c.UserId == UserInfo.UserId).FirstOrDefault();
                    UserInfo.Updated_Date = EuropeDate;
                    if (UserInfo.DOB != DateTime.Parse("0001-01-01") && UserInfo.DOB != null)
                        BirthdayUpdate = "A";
                    var usereditphoneno = SPA.CCTSP_User.Where(c => c.UserId == UserInfo.UserId && c.SchId == UserInfo.SchId && c.PhoneNo == phoneno && c.ActiveStatus == "A").Count();
                    var UserPhoneNo = SPA.CCTSP_User.Where(c => c.RoleId == 4 && c.PhoneNo == phoneno && c.ActiveStatus == "A" && c.PhoneNo != "").Count();
                    if (usereditphoneno == 0 && UserPhoneNo > 0)
                        Status = "Yes";
                }
                else
                {
                    var UserPhoneNo = SPA.CCTSP_User.Where(c => c.RoleId == 4 && c.SchId == schlId && c.PhoneNo == phoneno && c.ActiveStatus == "A" && c.PhoneNo != "").Count();
                    if (UserPhoneNo > 0)
                        Status = "Yes";
                }
                if (Status == "No")
                {
                    UserInfo.PhoneNo = phoneno;
                    UserInfo.PasswordQuerry2 = user["NwLandLineNo"];
                    UserInfo.Gender = user["NWTitle"];
                    UserInfo.FirstName = user["NWFirstName"];
                    UserInfo.LastName = user["NWLastName"];
                    UserInfo.EmailId = user["NWEmail"];
                    var Password = user["NWPassword"];
                    if (Password != null && Password != "")
                        UserInfo.Password = Password;
                    else
                        UserInfo.Password = "BCS@1234";
                    if (user["NWPostal"].ToString() != "")
                        UserInfo.Pincode = Convert.ToInt32(user["NWPostal"]);
                    UserInfo.City = user["NWCITY"];
                    UserInfo.Address1 = user["NWAddress"];
                    UserInfo.CreatedOn = EuropeDate;
                    UserInfo.RoleId = 4;
                    if (user["NWEditUserId"] == "")
                        UserInfo.SchId = schlId;
                    UserInfo.Street = user["Street"];
                    UserInfo.AHV_Number = user["AHV_Number"];
                    UserInfo.VEKA_Number = user["VEKA_Number"];
                    UserInfo.InsuranceNumber = user["InsuranceNumber"];
                    UserInfo.Salutation = user["NWSalutation"];
                    UserInfo.Title = user["NWGender"];
                    UserInfo.StreetNumber = user["Street_Number"];
                    UserInfo.Street = user["Street"];
                    UserInfo.State = user["NWState"];
                    UserInfo.GLN_No = user["NW_GLNNo"];
                    UserInfo.ZSR_No = user["NW_ZSRNo"];
                    UserInfo.Country = user["NWCountry"];
                    if (!string.IsNullOrEmpty(user["status"]))
                        UserInfo.ActiveStatus = user["status"];
                    else
                        UserInfo.ActiveStatus = "A";
                    if (UserInfo.PhoneNo != "" && UserInfo.PasswordQuerry2 == "")
                        UserInfo.LoginName = UserInfo.PhoneNo;
                    if (UserInfo.PhoneNo == "" && UserInfo.PasswordQuerry2 != "")
                        UserInfo.LoginName = UserInfo.PasswordQuerry2;
                    if (UserInfo.PhoneNo != "" && UserInfo.PasswordQuerry2 != "")
                        UserInfo.LoginName = UserInfo.PhoneNo;
                    if (UserInfo.PhoneNo == "" && UserInfo.PasswordQuerry2 == "")
                        UserInfo.LoginName = "";
                    string startDate = user["NWDOB"].ToString();
                    if (startDate != null && startDate != "")
                        UserInfo.DOB = DateTime.Parse(startDate, enGB);
                    else
                        UserInfo.DOB = DateTime.Parse("0001-01-01", enGB);
                    if (!string.IsNullOrEmpty(user["DisplayInvoice"]))
                        UserInfo.Display_Invoice = Convert.ToInt32(user["DisplayInvoice"]);
                    else
                        UserInfo.Display_Invoice = null;
                    #region CheckEmailServiceActivatedOrNot
                    if (user["SmsConfirm"] == null) { user["SmsConfirm"] = ""; }
                    if (user["SmsRemain"] == null) { user["SmsRemain"] = ""; }
                    if (user["EmailConfirm"] == null) { user["EmailConfirm"] = ""; }
                    if (user["EmailRemain"] == null) { user["EmailRemain"] = ""; }
                    if (user["EmailInvoice"] == null) { user["EmailInvoice"] = ""; }
                    if (user["PrintInvoice"] == null) { user["PrintInvoice"] = ""; }
                    if (user["SmsConfirm"].ToString().Contains("on") || user["SmsRemain"].ToString().Contains("on"))
                    {
                        if (user["SmsRemain"].ToString().Contains("on") && user["SmsConfirm"].ToString().Contains("on"))
                            UserInfo.SMS_Service = 2;
                        else if (user["SmsRemain"].ToString().Contains("on"))
                            UserInfo.SMS_Service = 1;
                        else if (user["SmsConfirm"].ToString().Contains("on"))
                            UserInfo.SMS_Service = 3;
                    }
                    else
                        UserInfo.SMS_Service = null;
                    if (user["EmailConfirm"].ToString().Contains("on") || user["EmailRemain"].ToString().Contains("on"))
                    {
                        if (user["EmailRemain"].ToString().Contains("on") && user["EmailConfirm"].ToString().Contains("on"))
                            UserInfo.Email_Service = 2;
                        else if (user["EmailRemain"].ToString().Contains("on"))
                            UserInfo.Email_Service = 1;
                        else if (user["EmailConfirm"].ToString().Contains("on"))
                            UserInfo.Email_Service = 3;
                    }
                    else
                        UserInfo.Email_Service = null;
                    if (user["EmailInvoice"].ToString().Contains("on") || user["PrintInvoice"].ToString().Contains("on"))
                    {
                        if (user["EmailInvoice"].ToString().Contains("on") && user["PrintInvoice"].ToString().Contains("on"))
                            UserInfo.Invoice_Service = "2";
                        else if (user["EmailInvoice"].ToString().Contains("on"))
                            UserInfo.Invoice_Service = "1";
                        else if (user["PrintInvoice"].ToString().Contains("on"))
                            UserInfo.Invoice_Service = "3";
                    }
                    else
                        UserInfo.Invoice_Service = null;
                    #endregion
                    if (user["NWEditUserId"] == "")
                        SPA.CCTSP_User.Add(UserInfo);
                    else
                        UpdateModel(UserInfo);
                    SPA.SaveChanges();
                    if (user["NWEditUserId"] == "")
                    {
                        UserInfo.Updated_Date = EuropeDate;
                        string name = UserInfo.FirstName + " " + UserInfo.LastName;
                        var checkSMSService = SPA.CCTSP_User.Where(c => c.UserId == UserInfo.UserId && c.SchId == schlId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                        if (checkSMSService != null)
                            SMS.UserRegistration(name, null, UserInfo.PhoneNo.ToString());
                        var checkEMAILService = SPA.CCTSP_User.Where(c => c.UserId == UserInfo.UserId && c.SchId == schlId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                        if (checkEMAILService != null && SchoolInfo.Flow_Id != 2)
                            Email.MailForUserRegistration(UserInfo.UserId, UserInfo.EmailId);
                        //Email.UserRegistration(UserInfo.UserId, null, UserInfo.EmailId);
                    }
                    if (BirthdayUpdate == "A")
                        Email.AddBirthdayReminder(UserInfo.UserId);
                }
                Session["CustomerValidationMsg"] = Status;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        [CustomAutohrize(null)]
        public ActionResult BlackList(string OrderBy, int? Sorting)
        {
            try
            {
                var Desc = "";
                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = "a.Updated_Date";
                if (Sorting == 1)
                    Desc = "DESC";
                //Logged in User
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var TopTenUser = "  select a.UserId, a.Firstname,a.LastName,a.Emailid,a.Gender,a.Phoneno,a.Updated_Date as LastVisited ," +
                                 "  b.ItenName as FlowStatus,e.AccessToData,e.insertAccess,e.DeleteAccess ,e.UpdateAccess " +
                                 "  from cctsp_User a " +
                                 "  join ctsp_SolutionMaster b on b.SectionName = 'Black List' " +
                                 "  left join SPA_EmployeeScheduler c on c.Client_UserId = a.UserId " +
                                 "  join cctsp_User d on d.UserId = " + UserId + " " +
                                 "  join cctsp_Role f on f.Roleid=d.Roleid and (f.Schlid=" + schlId + " or f.Schlid=236) and f.ActiveStatus='A' " +
                                 "  join cctsp_RoleDetails e on e.RoleId = f.RoleId and  b.SolutionId = convert(bigint, e.AccesstoSub) " +
                                 "  and((d.UserId = " + UserId + " and e.Accesstodata = 'Own' and c.Emp_UserId = d.UserId)or(e.Accesstodata != 'Own')) " +
                                 "  and e.Schid=f.Schlid and e.activeStatus = 'A' " +
                                 "  Where a.RoleId = 4 and a.ActiveStatus = 'L' and a.Schid = " + schlId + " " +
                                 "  group by a.UserId, a.Firstname,a.LastName,a.Emailid,a.Gender,a.Phoneno,a.Updated_Date,b.ItenName ,e.AccessToData,e.insertAccess,e.DeleteAccess ,e.UpdateAccess " +
                                 "  order by " + OrderBy + " " + Desc;
                ViewBag.BlackList = SPA.Database.SqlQuery<Models.CustomerTabInfo>(TopTenUser).ToList();
                #region Language
                int LangId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = fu.getLanguageData("Customer_Black_List", 0, LangId);
                ViewBag.MonthList = fu.getLanguageData("Small_calander", 8, LangId);
                #endregion
                //check Access for New customer as edit button is there in Black List
                ViewBag.AcesofNewCust = fu.CheckAccessofPage("New Customer", UserId);
                ViewBag.OrderBy = OrderBy;
                ViewBag.Sorting = Sorting;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public void AddBlackList(int? UserId, string status)
        {
            DateTime EuropeDate = fu.ZonalDate(null);
            try
            {
                if (status == null)
                {
                    status = "L";
                }
                var query = SPA.CCTSP_User.Where(c => c.UserId == UserId).FirstOrDefault();
                query.ActiveStatus = status;
                query.Updated_Date = EuropeDate;
                SPA.SaveChanges();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        [CustomAutohrize(null)]
        public ActionResult TopTenCustomer(string OrderBy, int? Sorting)
        {
            try
            {
                var Desc = "";
                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = "Sum(Cast(a.Product_price as float)) DESC";
                if (Sorting == 1)
                    Desc = "DESC";
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var ToptenCustomer = " Select top 10b.Firstname,b.LastName,b.Emailid,b.Gender,b.Phoneno, " +
                                    " d.ItenName as FlowStatus,f.AccessToData  ,f.insertAccess ,f.DeleteAccess,f.UpdateAccess," +
                                    " Sum(Cast(a.Product_price as float)) as Revenue,b.UserId, " +
                                    " ( " +
                                    " select max(x.updatedOn) " +
                                    " from SPA_EmployeeScheduler x " +
                                    " where x.ActiveStatus = 'C' and x.BookedStatus = 'C'and x.CLIENT_UserId = b.UserId ) as LastVisited " +
                                    " from SPA_EmployeeScheduler a " +
                                    " join ctsp_SolutionMaster d on d.SectionName = 'Top Ten Customer' " +
                                    " join CCTSP_User b on a.Client_UserId = b.UserId " +
                                    " join cctsp_User e on e.UserId = " + UserId + " " +
                                    " join cctsp_Role g on g.Roleid=e.Roleid and (g.Schlid=" + schlId + " or g.Schlid=236)" +
                                    " join cctsp_RoleDetails f on f.RoleId = g.RoleId and  d.SolutionId = convert(bigint, f.AccesstoSub) " +
                                    "  and((e.UserId = " + UserId + " and f.Accesstodata = 'Own' and a.Emp_UserId = e.UserId)or(f.Accesstodata != 'Own'))	" +
                                    "  and f.Schid=g.Schlid and f.ActiveStatus='A' " +
                                    "  Where a.ActiveStatus = 'C' and a.BookedStatus = 'C' And a.Schid = " + schlId + " " +
                                    "  and b.ActiveStatus = 'A' and b.SchId = " + schlId + " " +
                                    "  Group by b.Firstname,b.LastName,b.Emailid,b.Gender,b.Phoneno,b.UserId, " +
                                    "  f.AccessToData  ,f.insertAccess ,f.DeleteAccess,f.UpdateAccess, d.ItenName  " +
                                    "  order by " + OrderBy + " " + Desc;
                ViewBag.UserInfo = SPA.Database.SqlQuery<Models.CustomerTabInfo>(ToptenCustomer).ToList();
                ViewBag.OrderBy = OrderBy;
                ViewBag.Sorting = Sorting;
                //Feeding on to ViewBag 
                var shopinfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                ViewBag.currency = fu.currency(shopinfo.Currency);
                var languagedata = fu.getLanguageData("Customer_Top_Ten", 0, shopinfo.Lang_id.Value);
                languagedata.AddRange(fu.getLanguageData("Small_calander", 8, shopinfo.Lang_id.Value));
                ViewBag.Language = languagedata;
                return View();
            }
            catch (Exception e)
            {
                //Check For Error
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public JObject EditCustomerDisplay(int? UserId, string Status)
        {
            try
            {
                JObject send = null;
                string jasondata = "";
                var data = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.ActiveStatus == Status && c.RoleId == 4).FirstOrDefault();
                string crDate = data.CreatedOn.Value.ToShortDateString().Replace("-", "/");
                string crDOB = data.DOB.ToString("dd-MM-yyyy");
                var Post = fu.NullToString(Convert.ToString(data.Pincode));
                if (data.Email_Service == null) data.Email_Service = 0;
                if (data.SMS_Service == null) data.SMS_Service = 0;
                if (data.Display_Invoice == null) data.Display_Invoice = 0;
                if (data.Password == "BCS@1234") { data.Password = ""; }
                Models.CustomerDisplay EditCustomer = new Models.CustomerDisplay();
                EditCustomer.UserId = UserId.Value;
                EditCustomer.FirstName = fu.NullToString(data.FirstName);
                EditCustomer.PhoneNumber = fu.NullToString(data.PhoneNo);
                EditCustomer.FamilyName = fu.NullToString(data.LastName);
                EditCustomer.Email = fu.NullToString(data.EmailId);
                EditCustomer.Title = fu.NullToString(data.Gender);
                EditCustomer.PostalCode = Post;
                EditCustomer.City = fu.NullToString(data.City);
                EditCustomer.Address = fu.NullToString(data.Address1);
                EditCustomer.EmailService = data.Email_Service.Value;
                EditCustomer.InvoiceService = data.Invoice_Service;
                EditCustomer.SMSService = data.SMS_Service.Value;
                EditCustomer.DisplayInvoice = data.Display_Invoice;
                EditCustomer.Password = fu.NullToString(data.Password);
                EditCustomer.DOB = crDOB;
                EditCustomer.AHV_Number = fu.NullToString(data.AHV_Number);
                EditCustomer.VEKA_Number = fu.NullToString(data.VEKA_Number);
                EditCustomer.InsuranceNumber = fu.NullToString(data.InsuranceNumber);
                EditCustomer.Street = fu.NullToString(data.Street);
                EditCustomer.CreatedOn = crDate;
                EditCustomer.Status = fu.NullToString(Status);
                EditCustomer.landlinenumber = fu.NullToString(data.PasswordQuerry2);
                EditCustomer.Gender = fu.NullToString(data.Title);
                EditCustomer.Salutation = fu.NullToString(data.Salutation);
                EditCustomer.StreetNumber = fu.NullToString(data.StreetNumber);
                EditCustomer.GLN_No = fu.NullToString(data.GLN_No);
                EditCustomer.ZSR_No = fu.NullToString(data.ZSR_No);
                EditCustomer.State = fu.NullToString(data.State);
                EditCustomer.Country = fu.NullToString(data.Country);
                jasondata = js.Serialize(EditCustomer);
                send = JObject.Parse(jasondata);
                return send;
            }
            catch (Exception e)
            {
                JObject send = null;
                fu.ErrorSend(RouteData, e);
                return send;
            }

        }
        public ActionResult DisplayCustomerHistory(int? UserId)
        {
            try
            {
                #region BasicReq
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                                 .Select(c => new Models.shopMaster { LangId = c.Lang_id })
                                .FirstOrDefault();
                int LanguageId = SchoolInfo.LangId.Value;
                var LanguageCustomerHistory = fu.getLanguageData("Customer_New_Customer", 0, LanguageId);
                LanguageCustomerHistory.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Common" && (c.Order_id == 1 || c.Order_id == 2) && c.Lang_id == LanguageId).ToList());
                LanguageCustomerHistory.AddRange(fu.getLanguageData("Small_calander", 8, LanguageId));
                ViewBag.LagCustomerHistory = LanguageCustomerHistory;

                #endregion
                List<Models.PendingApproval> displaycustomerhistory = new List<PendingApproval>();
                if (UserId != null && UserId != 0)
                {
                    //string customerhistory = "select c.currency, ISNULL(c.SchlStudentStrength, 0 ) as SchlStudentStrength ,a.empschdetailsid,a.bookingdate,cast(a.fromslotmasterid as time) as fromslotmasterid,b.firstname as empfirstname,a.reg_status,d.catgdesc, cast(case when isnumeric(e.amount) = 1 then e.amount else null end as decimal(38, 2)) as amount,cast(e.duration as int) as duration,e.sectiondesc as comment,stdoctnotes = (select count(x.Prescription_Id) +(select count(x.Prescription_Id) from Medicine_Master z ,Prescription_Master X where x.Prescription_Id = z.Prescription_Id and z.BookingId = a.empschdetailsid and z.ActiveStatus = 'A' and x.BookingId = a.empschdetailsid and diff = 1 and x.ActiveStatus = 'A')  from Prescription_Master x, Prescription_Detail y where x.BookingId = a.empschdetailsid and y.BookingId = a.empschdetailsid and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 1 and y.Prescription_Id = x.Prescription_Id), stprescription = (select count(x.Prescription_Id) + (select count(x.Prescription_Id) from Medicine_Master z ,Prescription_Master x where x.Prescription_Id = z.Prescription_Id and z.BookingId = a.empschdetailsid and z.ActiveStatus = 'A' and x.BookingId = a.empschdetailsid and diff = 2 and x.ActiveStatus = 'A') from Prescription_Master x, Prescription_Detail y where x.BookingId = a.empschdetailsid and y.BookingId = a.empschdetailsid and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 2 and y.Prescription_Id = x.Prescription_Id) , StMerge = (select count(x.Prescription_Id) + (select count(x.Prescription_Id) from Medicine_Master z ,Prescription_Master x where x.Prescription_Id = z.Prescription_Id and z.BookingId = a.empschdetailsid and z.ActiveStatus = 'A' and x.BookingId = a.empschdetailsid and diff = 3 and x.ActiveStatus = 'A') from Prescription_Master x, Prescription_Detail y where x.BookingId = a.empschdetailsid and y.BookingId = a.empschdetailsid and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 3 and y.Prescription_Id = x.Prescription_Id) from spa_employeescheduler a, cctsp_user b,cctsp_schoolmaster c, cctsp_categorydetails d, ctsp_solutionmaster e where a.schid = " + schlId + " and a.client_userid = " + UserId + " and a.emp_userid = b.userid and c.schlid = a.schid and a.activestatus = 'c' and a.bookedstatus = 'c' and c.activestatus = 'A' and d.catgtypeid = a.product_id and e.catgtypeid = d.catgtypeid";
                    displaycustomerhistory = fu.CustomerHistory(UserId.Value);
                }
                ViewBag.displaycustomerhistory = displaycustomerhistory;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public void DeleteCustomer(int? UserId)
        {
            try
            {
                var query = "update CCTSP_User set ActiveStatus='D' where UserId=" + UserId + "and SchId=" + schlId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public ActionResult ShowReport()
        {
            int Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Import" && c.ActiveStatus == "A" && c.Lang_id == Lang_id && c.Order_id > 7).Select(c => new Models.LanguageLabelDetails
            {
                English_Label = c.English_Label,
                Value = c.Value,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id,
                Lang_id = c.Lang_id
            }).ToList();
            return View(this.ImportExcel());
        }
        public List<Models.combineSucImport> ImportExcel()
        {
            try
            {
                var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
                string url = SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgDesc == "Import Excel Url" && c.ActiveStatus == "A")
                                .OrderByDescending(c => c.CatgTypeId)
                                .Select(c => c.CatgDesc).FirstOrDefault();

                string path = Path.Combine(HttpRuntime.AppDomainAppPath).ToString();
                if (url != null)
                {
                    url = path + url;
                    #region Import Excel
                    //get Column names
                    var getColName = fu.getAllColInfo();
                    FileInfo info = new FileInfo(url);
                    ExcelPackage package = new ExcelPackage(info);
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                    List<int> IndexColumns = new List<int>();
                    if (workSheet != null)
                    {
                        DataTable dataTable = new DataTable();
                        int columns = workSheet.Dimension.End.Column;
                        int rows = workSheet.Dimension.End.Row;
                        if (rows > 1)
                        {
                            #region Rows
                            for (int i = 1; i <= rows; i++)
                            {
                                DataRow dataRow = dataTable.NewRow();
                                var k = 0;
                                #region Column
                                for (int j = 1; j <= columns; j++)
                                {
                                    if (i == 1)
                                    {
                                        #region Header
                                        var ColumnNames = Convert.ToString(workSheet.Cells[i, j].Text).Trim();
                                        if (!string.IsNullOrEmpty(ColumnNames))
                                        {
                                            var ColumnName = getColName.Where(c => (c.ColumnName == ColumnNames || c.AliasName == ColumnNames || (c.CustColName == ColumnNames && c.CustColName != null && c.CustColName != ""))).FirstOrDefault();
                                            if (ColumnName != null)
                                            {
                                                dataTable.Columns.Add(ColumnNames);
                                                IndexColumns.Add(j);
                                            }
                                        }

                                        #endregion
                                    }
                                    else if (IndexColumns.Contains(j) && i != 1)
                                    {
                                        #region value
                                        if (string.IsNullOrEmpty(workSheet.Cells[1, j].Text) && !IndexColumns.Contains(j))
                                            break;
                                        k = 1;
                                        dataRow[IndexColumns.IndexOf(j)] = Convert.ToString(workSheet.Cells[i, j].Text).Trim();
                                        #endregion
                                    }
                                }
                                #endregion
                                if (k == 1)
                                {
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                            #endregion
                            //Add Data to Customer List and also gives Report
                            //Also Write Note that this process will take time accroding to your data.
                            return fu.AddDataTableToDatabase(dataTable, getColName);
                        }
                    }
                    #endregion
                }
                return new List<combineSucImport>();
            }
            catch (Exception ex)
            {
                if (TempVariable < 2)
                {
                    TempVariable += 1;
                    return this.ImportExcel();
                }
                else
                {
                    fu.ErrorSend(RouteData, ex);
                    return new List<combineSucImport>();
                }

            }
        }
        public ActionResult Import()
        {
            var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => new Models.shopMaster { LangId = c.Lang_id, ShopId = c.SchlId }).FirstOrDefault();
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Import" && c.ActiveStatus == "A" && c.Lang_id == SchoolInfo.LangId && c.Order_id < 7).Select(c => new Models.LanguageLabelDetails
            {
                English_Label = c.English_Label,
                Value = c.Value,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id,
                Lang_id = c.Lang_id
            }).ToList();
            return View(SchoolInfo);
        }
        public string UploadExcel(HttpPostedFileBase file, string filename)
        {
            try
            {
                if (file.FileName != "")
                {
                    var supportedTypes = new[] { "xlsx", "xls" };
                    return fu.AddUrlofExcel(fu.CommonImageUpload(file, supportedTypes));
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
            return "";
        }
        [HttpPost]
        public JsonResult ImportCustomer(FormCollection frm)
        {
            var url = "";
            var jobj = new JObject();
            var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
            var LangText = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Import" && c.ActiveStatus == "A" && c.Lang_id == Lang_id && c.Order_id == 7).Select(c => c.Value).FirstOrDefault();
            Models.AlertModel AM = new AlertModel();
            AM.ISERROR = true;
            AM.Message = LangText;
            //AM.Data = "/Customer/Customer#Cimport";
            try
            {
                if (frm.Count > 0)
                    url = frm["ExcelUrl"];
                else
                    url = SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgDesc == "Import Excel Url" && c.ActiveStatus == "A").OrderByDescending(c => c.CatgTypeId).Select(c => c.CatgDesc).FirstOrDefault();
                string path = Path.Combine(HttpRuntime.AppDomainAppPath).ToString();
                url = path + url;

                var getColName = SPA.CCTSP_CategoryDetails
                             .Where(c => c.CCTSP_CategoryMaster.CatgDesc == "Import Excel" && c.Lang_Id == Lang_id
                                    && c.ActiveStatus == "A"
                                    && c.CatgDesc == "cctsp_user").Select(c => new Models.ImportCustomer
                                    {
                                        ColumnName = c.Gender,
                                        AliasName = c.Banner_Image,
                                        Identity = c.CatgTypeId
                                    }).ToList();
                //var getColName = fu.getAllColInfo();
                List<string> getAll = fu.GetAllHeaders(url);
                if (getColName.Count > 0 && getAll.Count > 0)
                {
                    getColName.Where(c => (getAll.Contains(c.ColumnName) || getAll.Contains(c.AliasName) || (c.CustColName != null && getAll.Contains(c.CustColName)))).All(c => { c.ISSAME = true; return true; });
                    AM.ISERROR = false;
                    if (getColName.Where(c => c.ISSAME == false).Count() == 0)
                        AM.Data = "/Customer/ImportExcel";
                    else
                        AM.Data = "/Customer/DynamicImport";
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
            return Json(AM, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DynamicImport()
        {
            var getList = new List<Models.ImportCustomer>();
            try
            {
                /*It does not include Customer Column*/
                //getList = SPA.CCTSP_CategoryDetails
                //            .Where(c => c.CCTSP_CategoryMaster.CatgDesc == "Import Excel"
                //                   && c.ActiveStatus == "A"
                //                   && c.CatgDesc == "cctsp_user").Select(c => new Models.ImportCustomer
                //                   {
                //                       ColumnName = c.Gender,
                //                       AliasName = c.Banner_Image,
                //                       Identity = c.CatgTypeId
                //                   }).ToList();
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => new Models.shopMaster { LangId = c.Lang_id, ShopId = c.SchlId }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Import" && c.ActiveStatus == "A" && c.Lang_id == SchoolInfo.LangId && c.Order_id > 7).Select(c => new Models.LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id
                }).ToList();
                /*It include Customer Accepted Last Column*/
                getList = fu.getAllColInfo();
                var url = SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgDesc == "Import Excel Url" && c.ActiveStatus == "A").OrderByDescending(c => c.CatgTypeId).Select(c => c.CatgDesc).FirstOrDefault();
                if (url != null)
                {
                    string path = Path.Combine(HttpRuntime.AppDomainAppPath).ToString();
                    url = path + url;
                    ViewBag.getExcelColumn = fu.GetAllHeaders(url);
                }

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
            return View(getList);
        }
        public JsonResult AddDynamicColumn(FormCollection DynamicFields)
        {
            Models.AlertModel AM = new AlertModel();
            var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
            var LangText = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Import" && c.ActiveStatus == "A" && c.Lang_id == Lang_id && c.Order_id == 21).Select(c => c.Value).FirstOrDefault();
            AM.ISERROR = true;
            AM.Data = "/Customer/ShowReport";
            AM.Message = LangText;
            try
            {
                List<CTSP_SolutionMaster> DynamiFieldsList = new List<CTSP_SolutionMaster>();
                CTSP_SolutionMaster DynamicField = new CTSP_SolutionMaster();
                //Delete Dynamic Fields Entry
                var updateQuery = "UPDATE ctsp_SolutionMaster SET Activestatus='D' WHERE Schid=" + schlId + " AND CatgtypeId IN ( " +
                                 "SELECT a.CatgtypeId FROM cctsp_CategoryDetails a JOIN " +
                                 "cctsp_Categorymaster b ON a.CatgId = b.CatgId " +
                                 "JOIN cctsp_schoolMaster c on c.SchlId=" + schlId + " " +
                                 "WHERE a.CatgDesc = 'cctsp_user' AND b.CatgDesc = 'Import Excel' AND a.Lang_id=c.Lang_id " +
                                 "AND a.ActiveStatus = 'A' " +
                                 ") AND ActiveStatus = 'A'";
                SPA.Database.ExecuteSqlCommand(updateQuery);
                foreach (var Item in DynamicFields.AllKeys.Where(c => !c.Contains("X-Requested-With")).ToList())
                {
                    DynamicField = new CTSP_SolutionMaster();
                    DynamicField.CatgTypeId = Convert.ToInt64(Item);
                    DynamicField.SectionName = DynamicFields[Item];
                    DynamicField.SectionDesc = DynamicFields[Item];
                    DynamicField.Activestatus = "A";
                    DynamicField.CraetedOn = DateTime.Now;
                    DynamicField.Update_Date = DateTime.Now;
                    DynamicField.SchId = schlId;
                    DynamiFieldsList.Add(DynamicField);
                }
                if (DynamiFieldsList.Count > 0)
                {
                    SPA.CTSP_SolutionMaster.AddRange(DynamiFieldsList);
                    SPA.SaveChanges();
                }
                AM.ISERROR = false;
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
            }
            return Json(AM, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Anamnese(long?UserId)
        {
            Models.anamnese AnamneseDetails = new anamnese();
            if(UserId>0)
            {
                AnamneseDetails = SPA.CCTSP_Anamnese.Where(c => c.UserId == UserId && c.ActiveStatus=="A").Select(c => new Models.anamnese
                {
                    Profession=c.Profession,
                    Hobbies=c.Hobbies,
                    Date_1=c.Date_1,
                    Doctor_Name=c.Doctor_Name,
                    Date_2=c.Date_2,
                    Main_Diagnosis=c.Main_Diagnosis,
                    Secondary_Diagnosis=c.Secondary_Diagnosis,
                    Main_issues=c.Main_issues,
                    Symptom_Behaviour=c.Symptom_Behaviour,
                    History=c.History,
                    Previous_Therapies=c.Previous_Therapies,
                    Az=c.Az,
                    Sicknesses=c.Sicknesses,
                    Surgery=c.Surgery,
                    Accidents=c.Accidents,
                    Dizziness=c.Dizziness,
                    Medicine=c.Medicine,
                    Social_Situation=c.Social_Situation,
                    Exp_Therapy=c.Exp_Therapy,
                    Immediate_objective=c.Immediate_objective,
                }).FirstOrDefault();
            }
            return View();
        }
        public void Addanamnese(Models.anamnese Anamnese)
        {
            if (ModelState.IsValid)
            {
                var ShopInfo = fu.ShopInfo();
                CCTSP_Anamnese AddAnamnese = new CCTSP_Anamnese();
                AddAnamnese.Profession = Anamnese.Profession;
                AddAnamnese.Hobbies = Anamnese.Hobbies;
                AddAnamnese.Date_1 = Anamnese.Date_1;
                AddAnamnese.Doctor_Name = Anamnese.Doctor_Name;
                AddAnamnese.Date_2 = Anamnese.Date_2;
                AddAnamnese.Main_Diagnosis = Anamnese.Main_Diagnosis;
                AddAnamnese.Secondary_Diagnosis = Anamnese.Main_Diagnosis;
                AddAnamnese.Main_issues = Anamnese.Main_issues;
                AddAnamnese.Symptom_Behaviour = Anamnese.Symptom_Behaviour;
                AddAnamnese.History = Anamnese.History;
                AddAnamnese.Previous_Therapies = Anamnese.Previous_Therapies;
                AddAnamnese.Az = Anamnese.Az;
                AddAnamnese.Sicknesses = Anamnese.Sicknesses;
                AddAnamnese.Surgery = Anamnese.Surgery;
                AddAnamnese.Accidents = Anamnese.Accidents;
                AddAnamnese.Dizziness = Anamnese.Dizziness;
                AddAnamnese.Medicine = Anamnese.Medicine;
                AddAnamnese.Social_Situation = Anamnese.Social_Situation;
                AddAnamnese.Exp_Therapy = Anamnese.Exp_Therapy;
                AddAnamnese.Immediate_objective = Anamnese.Immediate_objective;
                AddAnamnese.LongTerm_objective = Anamnese.LongTerm_objective;
                AddAnamnese.MidTerm_objective = Anamnese.MidTerm_objective;
                AddAnamnese.UserId = Anamnese.UserId;
                AddAnamnese.SchId = Anamnese.SchId;
                AddAnamnese.ActiveStatus = "A";
                AddAnamnese.Created_Date = fu.ZonalDate(ShopInfo.TimeZone);
                AddAnamnese.Updated_Date = fu.ZonalDate(ShopInfo.TimeZone);
                SPA.CCTSP_Anamnese.Add(AddAnamnese);
                SPA.SaveChanges();
            }
        }
    }
}