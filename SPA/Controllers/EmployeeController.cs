using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using SPA.Common;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Data.Objects.SqlClient;

namespace SPA.Controllers
{
    [checkShop]
    public class EmployeeController : Controller
    {
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        CultureInfo enGB = new CultureInfo("en-GB");
        // GET: Employee
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        JavaScriptSerializer js = new JavaScriptSerializer();
        Function fu = new Function();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public EmployeeController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        public ActionResult EmployeeHome()
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
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CustomAutohrize(null)]
        public ActionResult CreateEmployee(int? UserId)
        {
            try
            {
                #region CacheRemove
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                #endregion
                #region Logic
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(d => d.SchlId == schlId && d.ActiveStatus == "A").Select(c => new Models.shopMaster
                {
                    MainCatgId = c.MainCategory,
                    LangId = c.Lang_id,
                    SchlStudentStrength = c.SchlStudentStrength,
                    Display_Invoice = c.Display_Invoice,
                    Flow_Id=c.Flow_Id,
                    Country=c.Schcountry
                }).FirstOrDefault();
                ViewBag.CheckInvoice = fu.CheckInvoice(ShopInfo.Display_Invoice);
                long? MainCatgId = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == ShopInfo.MainCatgId && c.CatgId == 127).Select(c => c.Work_Flow_Id).FirstOrDefault();
                var WorkflowStatus = "";
                if (MainCatgId > 0)
                    WorkflowStatus = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == MainCatgId && c.CatgId == 153).Select(c => c.CatgDesc).FirstOrDefault();
                ViewBag.WorkflowStatus = WorkflowStatus;
                int LanguageId = ShopInfo.LangId.Value;
                ViewBag.ShopInfo = ShopInfo;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == LanguageId).Select(c => new Models.LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Lang_id = c.Lang_id,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Value = c.Value
                }).ToList();
                long FlowId = ShopInfo.Flow_Id != null ? ShopInfo.Flow_Id.Value : 0;
                ViewBag.EmpSubTabAccess = SPA.Database.SqlQuery<Models.AccessSubMenu>(new Sql().CheckSubTabAccessFlowVise(FlowId, "Employee", "Employee",ShopInfo.LangId.Value, "Create_Employee")).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                //ViewBag.GenderList = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == LanguageId).Select(c => c.CatgType).OrderBy(c => c).ToList();
                //DropDown Details
                var CatgNameList = "Salutation,New_Gender,Title";
                ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, ShopInfo.LangId.Value);
                var LngLocal = "";
                if (LanguageId == 1)
                    LngLocal = "en";
                if (LanguageId == 2)
                    LngLocal = "de";
                if (LanguageId == 3)
                    LngLocal = "fr-ca";
                ViewBag.LngLocal = LngLocal;
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                if (UserId == null)
                {
                    UserId = 0;
                }
                if (UserId != 0)
                {
                    var data = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId).Select(c => new Models.UserDetails
                    {
                        UserId = c.UserId,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Password = c.Password,
                        PhoneNo = c.PhoneNo,
                        EmailId = c.EmailId,
                        Answer2 = c.Answer2,
                        DOB = c.DOB,
                        Gender = c.Gender,
                        City = c.City,
                        State = c.State,
                        Country = c.Country,
                        Address1 = c.Address1,
                        PasswordQuerry2 = c.PasswordQuerry2,
                        Pincode = c.Pincode,
                        BaseUrl = c.BaseUrl,
                        Salutation = c.Salutation,
                        ETitle = c.Title,
                        GLN_No = c.GLN_No,
                        ZSR_No = c.ZSR_No,
                        Street = c.Street,
                        StreetNumber = c.StreetNumber
                    }).FirstOrDefault();
                    ViewBag.UserData = data;
                }
                else
                    ViewBag.UserData = null;
                ViewBag.UserId = UserId;
                return View();
                //}
                //else
                //    return RedirectToAction("SignIn", "Login");
                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult createEmployeeUper(int? UserId)
        {
            try
            {
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == LanguageId).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public JObject GetEmployeeData(int? UserId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                CCTSP_User user = new CCTSP_User();
                user = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId).FirstOrDefault();
                Models.CustomerDisplay EmployeeDetails = new Models.CustomerDisplay()
                { UserId = UserId.Value, FirstName = user.FirstName, PhoneNumber = user.PhoneNo, FamilyName = user.LastName, Email = user.EmailId, Title = user.Gender, EmpImg = user.Answer2 };
                jsondata = js.Serialize(EmployeeDetails);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch (Exception e)
            {
                JObject send = null;
                fu.ErrorSend(RouteData, e);
                return send;
            }

        }
        //public string GetDetailEmployeeData(int? UserId)
        //{
        //    try
        //    {
        //        var data = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == 845864568468468).ToList();
        //        //var data = SPA.CTSP_SolutionMaster.Where(c => c.UserId == UserId && c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SchId == schlId && c.SectionName == "EmployeeDays").ToList();
        //        string Days = "";
        //        if (data.Count != 0)
        //        {
        //            foreach (var item in data)
        //            {
        //                Days = Days + item.SectionDesc + "~";
        //            }
        //        }
        //        else
        //        {
        //            Days = "0";
        //        }
        //        return Days;
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return "";
        //    }
        //}
        public JObject GetDetailEmployeeData(int? UserId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                var data = SPA.CTSP_SolutionMaster.Where(c => c.UserId == UserId && c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SchId == schlId && c.SectionName == "EmployeeDays").Select(c => c.SectionDesc).ToList();
                Models.DateAndTimeArray Details = new Models.DateAndTimeArray()
                {
                    BlockData = data.ToArray()
                };
                jsondata = js.Serialize(Details);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                JObject send = null;
                return send;
            }
        }
        public string GetEmployeeTimeSlot(int? UserId)
        {
            try
            {
                var data = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.SchId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                if (data != null)
                {
                    var DataFinal = SPA.CCTSP_CategoryDetails.Where(c => c.CatgDesc == data.SlotDue && c.ActiveStatus == "A" && c.DomainType == schlId).Select(c => c.CatgTypeId).FirstOrDefault();
                    return DataFinal + "";
                }
                else
                {
                    var DataFinal = SPA.CCTSP_CategoryDetails.Where(c => c.CatgDesc == "15 minutes" && c.ActiveStatus == "A" && c.DomainType == schlId).Select(c => c.CatgTypeId).FirstOrDefault();
                    return DataFinal + "";
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public int CreateEmployeeAdd(Models.UserDetails UserDetails)
        {
            try
            {

                var SchoolDetails = SPA.CCTSP_SchoolMaster.Where(d => d.SchlId == schlId && d.ActiveStatus == "A").FirstOrDefault();
                DateTime EuropeDate = fu.ZonalDate(SchoolDetails.TimeZone);
                #region CacheCode
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                Response.Cache.SetNoServerCaching();
                Response.Cache.SetNoStore();
                Response.Cookies.Clear();
                Request.Cookies.Clear();
                #endregion
                #region Logic
                string PathTemp = null;
                CCTSP_User user = new CCTSP_User();
                if (UserDetails.UserId != null)
                    user.UserId = UserDetails.UserId.Value;
                user.FirstName = UserDetails.FirstName;
                user.LastName = UserDetails.LastName;
                user.EmailId = UserDetails.EmailId;
                user.PhoneNo = UserDetails.PhoneNo;
                user.Answer2 = UserDetails.Answer2;
                user.PasswordQuerry2 = UserDetails.PasswordQuerry2;
                user.Salutation = UserDetails.Salutation;
                user.Pincode = UserDetails.Pincode;
                user.Title = UserDetails.ETitle;
                user.Gender = UserDetails.Gender;
                user.City = UserDetails.City;
                user.State = UserDetails.State;
                user.Street = UserDetails.Street;
                user.StreetNumber = UserDetails.StreetNumber;
                user.ZSR_No = UserDetails.ZSR_No;
                user.GLN_No = UserDetails.GLN_No;
                user.Address1 = UserDetails.Address1;
                if (UserDetails.DOB != null)
                    user.DOB = DateTime.Parse(Convert.ToString(UserDetails.DOB), enGB);
                else
                    user.DOB = DateTime.Parse("0001-01-01", enGB);
                var StatusToUpdate = user.Answer2;
                int LanguageId = SchoolDetails.Lang_id.Value;
                ViewBag.GenderList = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == LanguageId).Select(c => c.CatgType).OrderBy(c => c).ToList();
                //Checking Whether data Exist or Not
                //Just Now Check Only on Basis Of Number
                //var data = SPA.CCTSP_User.Where(c => c.PhoneNo == user.PhoneNo && c.RoleId == 3 && c.ActiveStatus == "A" && c.SchId == schlId).FirstOrDefault();
                //if (user.UserId != 0)
                //{
                #region EmployeeAdd
                int CheckAlreadyExist = 0;
                //Add Data
                if (user.UserId == 0)
                {
                    CheckAlreadyExist = SPA.CCTSP_User.Where(c => c.PhoneNo == user.PhoneNo && c.RoleId != 1 && c.RoleId != 4 && c.ActiveStatus == "A" && c.SchId == schlId).Count();
                    if (CheckAlreadyExist == 0)
                    {
                        user.CreatedOn = EuropeDate;
                        user.SchId = schlId;
                        user.ActiveStatus = "A";
                        user.RoleId = 3;
                        if (user.LastName == null)
                        {
                            user.LastName = "";
                        }
                        if (user.PhoneNo != "" && user.PhoneNo != null)
                        {
                            user.LoginName = UserDetails.PhoneNo;
                        }
                        else
                        {
                            user.LoginName = "";
                        }

                        if (user.Answer2 == "Already Exist")
                        {
                            user.Answer2 = "";
                        }
                        user.Password = UserDetails.Password != null ? UserDetails.Password : "Emp@12345";
                        user.BaseUrl = "http://" + HttpContext.Request.Url.Host;
                        SPA.CCTSP_User.Add(user);
                        SPA.SaveChanges();
                        //Assign Product to Employee
                        fu.AssignProductToEmp(user.UserId, EuropeDate);
                    }
                    else
                        Session["Message"] = "EmpExist";
                }
                else
                {
                    //Edit Data
                    CheckAlreadyExist = SPA.CCTSP_User.Where(c => c.PhoneNo == user.PhoneNo && c.UserId != user.UserId && c.RoleId != 1 && c.RoleId != 4 && c.ActiveStatus == "A" && c.SchId == schlId).Count();
                    if (CheckAlreadyExist == 0)
                    {
                        var UserModel = SPA.CCTSP_User.Where(c => c.UserId == user.UserId).FirstOrDefault();
                        PathTemp = UserModel.Answer2;
                        UserModel.FirstName = user.FirstName;
                        if (user.LastName == null)
                        {
                            UserModel.LastName = "";
                        }
                        else
                        {
                            UserModel.LastName = user.LastName;
                        }
                        if (user.Answer2 == "Already Exist")
                        {
                            user.Answer2 = UserModel.Answer2;
                            UserModel.Answer2 = user.Answer2;
                        }

                        UserModel.LastName = user.LastName;
                        UserModel.Gender = user.Gender;
                        UserModel.EmailId = user.EmailId;
                        UserModel.PhoneNo = user.PhoneNo;
                        UserModel.Address1 = user.Address1;
                        UserModel.GLN_No = user.GLN_No;
                        UserModel.ZSR_No = user.ZSR_No;
                        UserModel.Salutation = user.Salutation;
                        UserModel.DOB = user.DOB;
                        UserModel.StreetNumber = user.StreetNumber;
                        UserModel.Street = user.Street;
                        UserModel.Pincode = user.Pincode;
                        UserModel.City = user.City;
                        UserModel.State = user.State;
                        UserModel.Title = user.Title;
                        UserModel.PasswordQuerry2 = user.PasswordQuerry2;
                        UserModel.BaseUrl = "http://" + HttpContext.Request.Url.Host;
                        UserModel.Password = user.Password != null ? user.Password : "Emp@12345";
                        //UpdateModel(UserModel);
                        SPA.SaveChanges();
                        user = new CCTSP_User();
                        user = UserModel;

                    }
                    else
                        Session["Message"] = "EmpExist";
                }
                #endregion
                #region UploadImage
                HttpPostedFileBase file = Request.Files["ProfilePic"];
                var ImgePathProfile = StatusToUpdate;
                if (file.FileName != "" && CheckAlreadyExist == 0)
                {
                    var postedFile = file;
                    string subPath = "~/Upload/" + user.UserId + "/";
                    bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                    var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                    if (postedFile.ContentLength > 16780000)
                    {
                        ViewBag.message = "File Size Not Valid";
                        return Convert.ToInt32(user.UserId);
                    }
                    if (!supportedTypes.Contains(fileExt.ToLower()))
                    {
                        ModelState.AddModelError("photo", "Invalid type. Only the following types (jpg, jpeg, png) are supported.");
                        return Convert.ToInt32(user.UserId);
                    }
                    if (!IsExists)
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                        string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                        long numFiles = files1.Length + user.UserId;
                        postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(numFiles + ".jpg"));
                        ImgePathProfile = "/Upload/" + user.UserId + "/" + numFiles + ".jpg";
                    }
                    else
                    {
                        string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                        int numFiles = files1.Length;
                        long Change = numFiles + user.UserId;
                        //System.IO.File.Move(Server.MapPath(subPath) + "1.jpg", Server.MapPath(subPath) + "old-" + numFiles + ".jpg");
                        postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(Change + ".jpg"));
                        ImgePathProfile = "/Upload/" + user.UserId + "/" + Change + ".jpg";
                    }

                    var query = "update CCTSP_User set Answer2='" + ImgePathProfile + "' where UserId=" + user.UserId;
                    SPA.Database.ExecuteSqlCommand(query);
                }
                else
                {
                    if (StatusToUpdate != "Already Exist" && StatusToUpdate != null && StatusToUpdate != "")
                    {
                        var query = "update CCTSP_User set Answer2='" + ImgePathProfile + "' where UserId=" + user.UserId;
                        SPA.Database.ExecuteSqlCommand(query);
                    }
                    else
                    {
                        if (CheckAlreadyExist == 0)
                        {
                            var query = "update CCTSP_User set Answer2='" + null + "' where UserId=" + user.UserId;
                            SPA.Database.ExecuteSqlCommand(query);
                        }
                    }


                }
                if (CheckAlreadyExist == 0)
                    return Convert.ToInt32(user.UserId);
                else
                    return 1;
                #endregion
                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return 0;
            }
        }
        public void DeleteEmployee(int? UserId)
        {
            try
            {
                var query = "update CCTSP_User set ActiveStatus='D' where UserId=" + UserId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public ActionResult Vacation(long? UserId)
        {
            try
            {
                string LngLocal = "";
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == LanguageId).ToList();
                if (LanguageId == 1)
                    LngLocal = "en";
                if (LanguageId == 2)
                    LngLocal = "de";
                if (LanguageId == 3)
                    LngLocal = "fr-ca";
                ViewBag.LngLocal = LngLocal;
                ViewBag.UserId = UserId;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult TimeSlot(long?UserId)
        {
            try
            {
                var data = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 112 && c.ActiveStatus == "A" && c.DomainType == schlId).OrderBy(c => c.CatgTypeId).ToList();
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == LanguageId).ToList();
                var UserTimeSlot = "15 Minutes";
                if (UserId > 0)
                    UserTimeSlot = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.ActiveStatus == "A").Select(c => c.SlotDue).FirstOrDefault();
                ViewBag.UserTimeSlot = UserTimeSlot;
                ViewBag.UserId = UserId;
                return View(data);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult WeekSchedule(long? UserId)
        {
            try
            {
                #region Logic
                List<Models.WeekSchedule> userWiseSchedule = new List<Models.WeekSchedule>();
                List<Models.WeekSchedule> FinalWeek = new List<Models.WeekSchedule>();
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == LanguageId).ToList();
                ViewBag.UserId = UserId;
                var days = SPA.CCTSP_SchedulerMaster
                              .Where(c => c.ActiveStatus == "A" && c.SchId == schlId).OrderBy(c => c.PeriodNumber)
                              .Distinct()
                              .Select(c => new Models.WeekSchedule
                              {
                                  WeekDay = c.WeekDay,
                                  starttime = c.StartTime,
                                  endTime = c.EndTime,
                                  periodNumber = c.PeriodNumber
                              }).ToList();
                var day = days.Select(c => c.WeekDay).Distinct().ToList();
                if (UserId != null && UserId != 0)
                    userWiseSchedule = SPA.SPA_EmployeeSchedulers
                                          .Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.UserId == UserId)
                                          .Select(c => new Models.WeekSchedule { WeekDay = c.WeekDay, starttime = c.StartTime, endTime = c.EndTime, flg = 1 }).ToList();
                var highestvalue = 0;
                foreach (var item in days.Select(c => c.WeekDay).Distinct())
                {
                    var data = new List<Models.WeekSchedule>();
                    if (UserId != null && UserId != 0 && userWiseSchedule.Where(c => c.WeekDay == item).Count() > 0)
                        data = userWiseSchedule.Where(c => c.WeekDay == item).ToList();
                    else
                        data = days.Where(c => c.WeekDay == item).ToList();
                    FinalWeek.AddRange(data);
                    if (data.Count > highestvalue)
                        highestvalue = data.Count;
                }
                ViewBag.highestcount = highestvalue;
                ViewBag.day = day;
                ViewBag.userday = userWiseSchedule.Count > 0 ? userWiseSchedule.Select(c => c.WeekDay).Distinct().ToList() : new List<string>();
                //If breaks are kept as per slot
                var Slot = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId).Select(c => c.SlotDue).FirstOrDefault();
                if (!string.IsNullOrEmpty(Slot))
                    Slot = Slot.Replace(" Minutes", "").Replace(" Minute", "");
                else
                    Slot = "15";
                ViewBag.slot = Slot;
                return View(FinalWeek);
                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult AddWeek()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                return View();
                //}
                //else
                //{
                //    return RedirectToAction("SignIn", "Login");
                //}
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [HttpPost]
        public ActionResult AddWeek(CCTSP_CategoryDetails Week)
        {
            try
            {
                var TimeZone = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                DateTime EuropeDate = fu.ZonalDate(TimeZone.TimeZone);
                if (Week.CatgTypeId == 0)
                {
                    var FindMobile = SPA.CCTSP_CategoryDetails.Where(c => c.CatgDesc == Week.CatgDesc && c.CatgId == 30 && c.DomainType == schlId && c.ActiveStatus == "A").FirstOrDefault();
                    if (FindMobile == null)
                    {
                        Week.CatgId = 30;
                        Week.Lang_Id = TimeZone.Lang_id;
                        Week.CreatedOn = EuropeDate;
                        Week.ActiveStatus = "A";
                        Week.DomainType = schlId;
                        SPA.CCTSP_CategoryDetails.Add(Week);
                        SPA.SaveChanges();
                    }
                }
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult WeekDetail()
        {
            try
            {
                var data = SPA.CCTSP_CategoryDetails.Where(C => C.CatgId == 30 && C.DomainType == schlId && C.ActiveStatus == "A").OrderBy(c => c.CreatedOn).ThenBy(c => c.CatgDesc).ToList();
                ViewBag.DataCount = data.Count;
                return View(data);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult DeletedDayScheduler()
        {
            try
            {
                var data = SPA.CCTSP_CategoryDetails.Where(C => C.CatgId == 30 && C.DomainType == schlId && C.ActiveStatus == "D").OrderBy(c => c.CreatedOn).ThenBy(c => c.CatgDesc).ToList();
                ViewBag.DataCount = data.Count;
                return View(data);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //public string displayEditWeek(int? CatTid)
        //{
        //    try
        //    {
        //        CCTSP_CategoryDetails CatgDetails = new CCTSP_CategoryDetails();
        //        string Result = "";
        //        CatgDetails = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.DomainType == schlId && c.CatgTypeId == CatTid).FirstOrDefault();
        //        Result = CatgDetails.CatgDesc + "~" + CatgDetails.CatgType;
        //        return Result;
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return "";
        //    }
        //}
        public JObject displayEditWeek(int? CatTid)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                var Data = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.DomainType == schlId && c.CatgTypeId == CatTid).FirstOrDefault();
                Models.DateAndTimeArray Details = new Models.DateAndTimeArray()
                {
                    CatgDesc = Data.CatgDesc,
                    CatgType = Data.CatgType
                };
                jsondata = js.Serialize(Details);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                JObject send = null;
                return send;
            }
        }
        public void updateWeek(FormCollection Weeks)
        {
            try
            {
                var query = "update CCTSP_CategoryDetails set CatgDesc = '" + Weeks["EditCatgDesc"] + "',CatgType='" + Weeks["EditCatgType"] + "' where CatgTypeId='" + Convert.ToInt32(Weeks["EditItemId"]) + "'";
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void DeleteWeek(int? CatTid)
        {
            try
            {
                var query = "update CCTSP_CategoryDetails set ActiveStatus='D' where CatgTypeId=" + CatTid;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void ActivateWeek(int? CatTid)
        {
            try
            {
                var query = "update CCTSP_CategoryDetails set ActiveStatus='A' where CatgTypeId=" + CatTid;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        [CustomAutohrize(null)]
        public ActionResult AddWeekPeriod()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                ViewBag.WeekDay = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgId == 30 && c.DomainType == 236).ToList(), "CatgDesc", "CatgDesc");
                return View();
                //}
                //else
                //{
                //    return RedirectToAction("SignIn", "Login");
                //}

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }


        }
        [HttpPost]
        public ActionResult AddWeekPeriod(FormCollection user)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                ViewBag.WeekDay = new SelectList(SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgId == 30 && c.DomainType == 236).ToList(), "CatgDesc", "CatgDesc");
                CCTSP_SchedulerMaster Week = new CCTSP_SchedulerMaster();
                if (user.Count != 0)
                {
                    Week.WeekDay = user["WeekDayAdd"].ToString();
                    DateTime start = DateTime.Parse(user["StartTime"], CultureInfo.CurrentCulture, DateTimeStyles.None);
                    DateTime End = DateTime.Parse(user["EndTime"], CultureInfo.CurrentCulture, DateTimeStyles.None);
                    Week.StartTime = start.TimeOfDay;
                    Week.EndTime = End.TimeOfDay;
                    Week.Duration = Week.EndTime - Week.StartTime;
                    Week.SchId = schlId;
                    Week.ActiveStatus = "A";
                    Week.CreatedOn = EuropeDate;
                    Week.PeriodNumber = Convert.ToInt32(user["orderPeriod"].ToString());
                    SPA.CCTSP_SchedulerMaster.Add(Week);
                    SPA.SaveChanges();
                }

                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");

            }

        }
        public ActionResult DetailWeekPeriod()
        {
            try
            {
                var data = SPA.CCTSP_SchedulerMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId).OrderBy(c => c.PeriodNumber);
                ViewBag.DataCount = data.Count();
                return View(data);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        //public string DisplayEditPeriod(int? SchedulerId)
        //{
        //    try
        //    {
        //        string DisplayPeriod = "";
        //        var data = SPA.CCTSP_SchedulerMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId && c.SchedulerId == SchedulerId).FirstOrDefault();
        //        DisplayPeriod = data.WeekDay + "~" + data.StartTime + "~" + data.EndTime + "~" + SchedulerId;
        //        return DisplayPeriod;
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return "";

        //    }

        //}
        public JObject DisplayEditPeriod(int? SchedulerId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                var data = SPA.CCTSP_SchedulerMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId && c.SchedulerId == SchedulerId).FirstOrDefault();
                Models.EditPeriod EditDetails = new Models.EditPeriod()
                {
                    WeekDay = data.WeekDay,
                    StartTime = data.StartTime.Value,
                    EndTime = data.EndTime.Value,
                    SchedulerId = SchedulerId
                };
                jsondata = js.Serialize(EditDetails);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                JObject send = null;
                return send;

            }

        }
        public void EditPeriod(FormCollection Period)
        {
            try
            {
                CCTSP_SchedulerMaster schedule = new CCTSP_SchedulerMaster();
                int schedulerId = Convert.ToInt32(Period["SchedulerId"]);
                string startTime = Period["StartTime"].ToString();
                string EndTime = Period["EndTime"].ToString();
                schedule = SPA.CCTSP_SchedulerMaster.Where(c => c.SchedulerId == schedulerId && c.SchId == schlId).FirstOrDefault();
                if (schedule != null)
                {
                    schedule.WeekDay = Period["WeekDayEdit"];
                    schedule.StartTime = DateTime.Parse(startTime).TimeOfDay;
                    schedule.EndTime = DateTime.Parse(EndTime).TimeOfDay;
                    schedule.Duration = schedule.EndTime - schedule.StartTime;
                    UpdateModel(schedule);
                    SPA.SaveChanges();
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public void DeletePeriod(int? SchedulerId)
        {
            try
            {
                var query = "update CCTSP_SchedulerMaster set ActiveStatus='D' where SchedulerId=" + SchedulerId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);

            }

        }
        public void ReplicateDays(int? SchedulerId)
        {
            DateTime EuropeDate = fu.ZonalDate(null);
            try
            {
                string days = "";
                var data = SPA.CCTSP_SchedulerMaster.Where(c => c.SchedulerId == SchedulerId).FirstOrDefault();
                #region ExistingDaysInDataBase
                var SameData = SPA.CCTSP_SchedulerMaster.Where(c => c.StartTime == data.StartTime && c.EndTime == data.EndTime && c.ActiveStatus == "A").ToList();
                foreach (var item in SameData)
                {
                    days = days + "~" + item.WeekDay;
                }
                #endregion
                #region originallyHowMuchDays
                string DaysOriginal = "";
                DateTime now = DateTime.Today;
                for (int i = 0; i < 7; i++)
                {
                    if (DaysOriginal != "")
                    {
                        DaysOriginal = DaysOriginal + "," + now.AddDays(1).ToString("dddd", enGB);
                    }
                    else
                    {
                        DaysOriginal = now.AddDays(1).ToString("dddd");
                    }
                    now = now.AddDays(1);
                }
                #endregion
                #region AddingDays
                int countDays = DaysOriginal.Split(',').Count();
                string[] DaysCurrent = DaysOriginal.Split(',');
                for (int i = 0; i < countDays; i++)
                {
                    if (!days.Contains(DaysCurrent[i]))
                    {
                        CCTSP_SchedulerMaster Week = new CCTSP_SchedulerMaster();
                        Week.WeekDay = DaysCurrent[i];
                        Week.StartTime = data.StartTime;
                        Week.EndTime = data.EndTime;
                        Week.SchId = schlId;
                        Week.ActiveStatus = "A";
                        Week.CreatedOn = EuropeDate;
                        Week.PeriodNumber = Convert.ToInt32(SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == Week.WeekDay && c.SchId == schlId).Select(c => c.PeriodNumber).Max()) + 1;
                        SPA.CCTSP_SchedulerMaster.Add(Week);
                        SPA.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

            #endregion
        }
        public ActionResult AddTimeSlot()
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
        [HttpPost]
        public ActionResult AddTimeSlot(CCTSP_CategoryDetails category)
        {
            DateTime EuropeDate = fu.ZonalDate(null);
            try
            {
                category.CatgType = "TimeSlot";
                category.ActiveStatus = "A";
                category.CreatedOn = EuropeDate;
                category.CatgId = 112;
                category.DomainType = schlId;
                SPA.CCTSP_CategoryDetails.Add(category);
                SPA.SaveChanges();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        #region Akash Code 
        //public JObject AddUserDays(FormCollection user)
        //{
        //    var Language = "";
        //    try
        //    {
        //        var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
        //        Language = SPA.Language_Label_Detail.Where(c => c.Lang_id == SchoolInfo.Lang_id && c.ActiveStatus == "A" && c.Page_Name == "AlertMsg" && c.Order_id == 58).Select(c => c.Value).FirstOrDefault();
        //        int Error = 0;
        //        JObject send = null;
        //        StringBuilder sb = null;
        //        DateTime EuropeDate = fu.ZonalDate(SchoolInfo.TimeZone);
        //        #region VariableInitialization
        //        CTSP_SolutionMaster Days = new CTSP_SolutionMaster();
        //        var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A").ToList();
        //        List<string> DaysList = new List<string>();
        //        List<SPA_EmployeeSchedulers> BulkAddUserDays = new List<SPA_EmployeeSchedulers>();
        //        int UserId = 0;
        //        #endregion
        //        #region CheckDataValidation
        //        var History = "";
        //        foreach (var key in user.Keys)
        //        {
        //            if (key.ToString() == "WeekUserId")
        //                UserId = Convert.ToInt32(user["WeekUserId"]);
        //            if (key.ToString() != "WeekUserId" && (key.ToString().Contains("1") || key.ToString().Contains("2") || key.ToString().Contains("3") || key.ToString().Contains("4")))
        //            {
        //                if (History == "")
        //                    History = user[key.ToString()];
        //                else
        //                {
        //                    if (DateTime.Parse(History).TimeOfDay <= DateTime.Parse(user[key.ToString()]).TimeOfDay)
        //                        History = user[key.ToString()];
        //                    else
        //                    {
        //                        sb = new StringBuilder();
        //                        sb.Append("{");
        //                        sb.Append("\"UserId\":\"" + UserId + "\",");
        //                        sb.Append("\"Error\":\"" + 1 + "\",");
        //                        sb.Append("\"ErrorMessage\":\"" + Language + "\",");
        //                        sb.Append("}");
        //                        send = JObject.Parse(sb.ToString());
        //                        return send;
        //                    }
        //                    if (key.ToString().Contains("4"))
        //                        History = "";
        //                }
        //            }
        //        }
        //        #endregion
        //        #region InsertAndUpdateDays
        //        #region updateAlldays
        //        var query = "update CTSP_SolutionMaster set ActiveStatus='D' where SectionName='EmployeeDays' and SchId='" + schlId + "' and ActiveStatus='A' and UserId=" + UserId;
        //        SPA.Database.ExecuteSqlCommand(query);
        //        #endregion
        //        #region AddWorkingDays
        //        foreach (var key in user.Keys)
        //        {
        //            if (key.ToString() != "WeekUserId" && !key.ToString().Contains("1") && !key.ToString().Contains("2") && !key.ToString().Contains("3") && !key.ToString().Contains("4"))
        //            {
        //                Days = SPA.CTSP_SolutionMaster.Where(c => c.UserId == UserId && c.SchId == schlId && c.SectionName == "EmployeeDays" && c.CatgTypeId == 11143 && c.SectionDesc == key.ToString() && c.Activestatus == "A").FirstOrDefault();
        //                if (Days == null)
        //                {
        //                    Days = new CTSP_SolutionMaster();
        //                    Days.SchId = schlId;
        //                    Days.CatgTypeId = 11143;
        //                    Days.UserId = UserId;
        //                    Days.CraetedOn = EuropeDate;
        //                    Days.SectionName = "EmployeeDays";
        //                    Days.Activestatus = "A";
        //                    Days.SectionDesc = key.ToString();
        //                    SPA.CTSP_SolutionMaster.Add(Days);
        //                    SPA.SaveChanges();
        //                    DaysList.Add(key.ToString());
        //                }
        //                else
        //                {
        //                    if (Days.Activestatus != "A")
        //                    {
        //                        Days.Activestatus = "A";
        //                        UpdateModel(Days);
        //                        SPA.SaveChanges();
        //                        DaysList.Add(key.ToString());
        //                    }
        //                }
        //            }
        //        }

        //        #endregion
        //        #region AddTimingsForSelectedDate
        //        var updateAllTimeSlotExisting = "update SPA_EmployeeSchedulers set ActiveStatus='D' where UserId=" + UserId + " and ActiveStatus='A' and schid=" + schlId + "";
        //        SPA.Database.ExecuteSqlCommand(updateAllTimeSlotExisting);
        //        foreach (var Daysadded in DaysList)
        //        {
        //            #region Section1
        //            SPA_EmployeeSchedulers EmployeeSchedule = new SPA_EmployeeSchedulers();
        //            EmployeeSchedule.ActiveStatus = "A";
        //            EmployeeSchedule.SchId = schlId;
        //            EmployeeSchedule.UserId = UserId;
        //            EmployeeSchedule.WeekDay = Daysadded;
        //            EmployeeSchedule.StartTime = DateTime.Parse(user["1" + Daysadded.ToString()]).TimeOfDay;
        //            EmployeeSchedule.EndTime = DateTime.Parse(user["2" + Daysadded.ToString()]).TimeOfDay;
        //            EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
        //            EmployeeSchedule.CreatedOn = EuropeDate;
        //            BulkAddUserDays.Add(EmployeeSchedule);
        //            #endregion
        //            #region Section2
        //            EmployeeSchedule = new SPA_EmployeeSchedulers();
        //            EmployeeSchedule.ActiveStatus = "A";
        //            EmployeeSchedule.SchId = schlId;
        //            EmployeeSchedule.UserId = UserId;
        //            EmployeeSchedule.WeekDay = Daysadded;
        //            EmployeeSchedule.StartTime = DateTime.Parse(user["3" + Daysadded.ToString()]).TimeOfDay;
        //            EmployeeSchedule.EndTime = DateTime.Parse(user["4" + Daysadded.ToString()]).TimeOfDay;
        //            EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
        //            EmployeeSchedule.CreatedOn = EuropeDate;
        //            BulkAddUserDays.Add(EmployeeSchedule);
        //            #endregion
        //        }
        //        if (BulkAddUserDays.Count > 0)
        //        {
        //            SPA.SPA_EmployeeSchedulers.AddRange(BulkAddUserDays);
        //            SPA.SaveChanges();
        //        }

        //        #endregion
        //        #endregion
        //        #region AddTime
        //        List<SPA_SchedulerSlotMaster> BulkSlotMaster = new List<SPA_SchedulerSlotMaster>();
        //        //Change
        //        var TimeDurationTemp = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId && c.SlotDue != null).Select(c => c.SlotDue).FirstOrDefault();
        //        if (TimeDurationTemp == null)
        //            TimeDurationTemp = "15 Minutes";
        //        var updateTimeSlotExisting = "update SPA_SchedulerSlotMaster set ActiveStatus='D' where UserId=" + UserId + " and ActiveStatus='A' and SchId=" + schlId;
        //        SPA.Database.ExecuteSqlCommand(updateTimeSlotExisting);
        //        SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
        //        string time = "";

        //        TimeSpan OriginalMinutes;
        //        #region MinuteCode
        //        if (TimeDurationTemp.Contains("Minute"))
        //        {
        //            time = TimeDurationTemp.Replace("Minute", "").Replace("s", "");
        //            OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(time.Trim()));
        //            var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == schlId && c.UserId == UserId).ToList();
        //            #region GetDays of Employee
        //            foreach (var Term in UserDays)
        //            {
        //                var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId).ToList();
        //                #region SlotEntry
        //                if (SlotTimeNew.Count() == 0)
        //                {
        //                    var SlotTime = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.WeekDay == Term.SectionDesc).OrderByDescending(c => c.StartTime);
        //                    foreach (var item in SlotTime)
        //                    {
        //                        TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
        //                        while (TempTime <= item.EndTime)
        //                        {
        //                            if (BulkSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = item.SchedulerId;
        //                                schedule.StartTime = TempTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                            TempTime = TempTime + OriginalMinutes;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var item in SlotTimeNew)
        //                    {
        //                        TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
        //                        while (TempTime <= item.EndTime)
        //                        {
        //                            var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
        //                            if (BulkSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = WeekDayForSelected;
        //                                schedule.StartTime = TempTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                            TempTime = TempTime + OriginalMinutes;
        //                        }
        //                    }
        //                }
        //                #endregion
        //            }
        //            #endregion
        //        }
        //        #endregion
        //        #region Day
        //        if (TimeDurationTemp.Contains("Day"))
        //        {
        //            var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == schlId);
        //            foreach (var Term in UserDays)
        //            {
        //                int i = 0;
        //                var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId);
        //                #region SlotEntry
        //                if (SlotTimeNew.Count() == 0)
        //                {
        //                    var SlotTime = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.WeekDay == Term.SectionDesc).OrderByDescending(c => c.StartTime);
        //                    foreach (var item in SlotTime)
        //                    {
        //                        #region OneDay
        //                        if (!TimeDurationTemp.Contains("Half"))
        //                        {
        //                            if (i == 0 && (BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0))
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = item.SchedulerId;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                                i++;
        //                            }
        //                        }
        //                        #endregion
        //                        #region HalfDay
        //                        else
        //                        {
        //                            if (BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = item.SchedulerId;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var item in SlotTimeNew)
        //                    {
        //                        var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc && c.SchId == schlId && c.ActiveStatus == "A").Select(c => c.SchedulerId).FirstOrDefault();
        //                        #region OneDay
        //                        if (!TimeDurationTemp.Contains("Half"))
        //                        {

        //                            if (i == 0 && (BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0))
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = WeekDayForSelected;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                                i++;
        //                            }
        //                        }
        //                        #endregion
        //                        #region HalfDay
        //                        else
        //                        {
        //                            if (BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == Term.SectionDesc && c.SchId == schlId && c.ActiveStatus == "A").Select(c => c.SchedulerId).FirstOrDefault();
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion
        //            }
        //        }
        //        #endregion
        //        #region half
        //        if (TimeDurationTemp.Contains("half"))
        //        {
        //            var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == schlId && c.UserId == UserId).ToList();
        //            foreach (var Term in UserDays)
        //            {
        //                //int i = 0;
        //                var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId).OrderBy(c => c.Scheduler_Time_Id).ToList();

        //                #region SlotEntry
        //                if (SlotTimeNew.Count() == 0)
        //                {
        //                    var SlotTime = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.WeekDay == Term.SectionDesc).OrderByDescending(c => c.StartTime);
        //                    foreach (var item in SlotTime)
        //                    {
        //                        #region 1st Half
        //                        if (!TimeDurationTemp.Contains("Half"))
        //                        {
        //                            if (SlotTime.First().SchedulerId == item.SchedulerId && BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = item.SchedulerId;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                        }
        //                        #endregion
        //                        #region 2nd Half
        //                        else
        //                        {
        //                            if (SlotTime.Last().SchedulerId == item.SchedulerId && BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = item.SchedulerId;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                else
        //                {
        //                    foreach (var item in SlotTimeNew)
        //                    {
        //                        var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc && c.SchId == schlId && c.ActiveStatus == "A").Select(c => c.SchedulerId).FirstOrDefault();
        //                        #region 1st Half
        //                        if (TimeDurationTemp.Contains("AM"))
        //                        {
        //                            if (SlotTimeNew.First().Scheduler_Time_Id == item.Scheduler_Time_Id && BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = WeekDayForSelected;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                        }
        //                        #endregion
        //                        #region 2nd Half
        //                        else
        //                        {
        //                            if (SlotTimeNew.Last().Scheduler_Time_Id == item.Scheduler_Time_Id && BulkSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                            {
        //                                schedule = new SPA_SchedulerSlotMaster();
        //                                schedule.SchedulerId = WeekDayForSelected;
        //                                schedule.StartTime = item.StartTime;
        //                                schedule.UserId = UserId;
        //                                schedule.CreatedOn = EuropeDate;
        //                                schedule.ActiveStatus = "A";
        //                                schedule.SlotDue = TimeDurationTemp;
        //                                schedule.SchId = schlId;
        //                                BulkSlotMaster.Add(schedule);
        //                            }
        //                        }
        //                        #endregion
        //                    }
        //                }
        //                #endregion
        //            }
        //        }

        //        #endregion
        //        fu.ScheduleSlotAdd(BulkSlotMaster);
        //        #endregion             
        //        sb = new StringBuilder();
        //        sb.Append("{");
        //        sb.Append("\"UserId\":\"" + UserId + "\",");
        //        sb.Append("\"Error\":\"" + Error + "\",");
        //        sb.Append("\"ErrorMessage\":\"" + "" + "\",");
        //        sb.Append("}");
        //        send = JObject.Parse(sb.ToString());
        //        return send;
        //    }
        //    catch (Exception e)
        //    {
        //        int error = 1;
        //        JObject send = null;
        //        StringBuilder sb = null;
        //        sb = new StringBuilder();
        //        sb.Append("{");
        //        sb.Append("\"Error\":\"" + error + "\"");
        //        sb.Append("\"ErrorMessage\":\"" + Language + "\",");
        //        sb.Append("}");
        //        fu.ErrorSend(RouteData, e);
        //        send = JObject.Parse(sb.ToString());
        //        return send;
        //    }
        //}
        public JObject AddUserDays(FormCollection user)
        {
            var Language = "";
            var Transaction = SPA.Database.BeginTransaction();
            JObject send = null;
            StringBuilder sb = null;
            try
            {
                /*Initialization Start*/
                int Error = 0;
                List<Models.WeekScheduleAdd> wekSchedAddList = new List<Models.WeekScheduleAdd>();
                Models.WeekScheduleAdd wekSchedAdd = new Models.WeekScheduleAdd();
                List<CTSP_SolutionMaster> dayslist = new List<CTSP_SolutionMaster>();
                CTSP_SolutionMaster Days = new CTSP_SolutionMaster();
                SPA_EmployeeSchedulers EmployeeSchedule = new SPA_EmployeeSchedulers();
                List<SPA_EmployeeSchedulers> EmployeeScheduleList = new List<SPA_EmployeeSchedulers>();
                SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
                List<SPA_SchedulerSlotMaster> schSlot = new List<SPA_SchedulerSlotMaster>();
                //Shop Information
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, TimeZone = c.TimeZone }).FirstOrDefault();
                //alert Message Language wise
                Language = SPA.Language_Label_Detail.Where(c => c.Lang_id == SchoolInfo.Lang_id && c.ActiveStatus == "A" && c.Page_Name == "AlertMsg" && c.Order_id == 58).Select(c => c.Value).FirstOrDefault();
                DateTime EuropeDate = fu.ZonalDate(SchoolInfo.TimeZone);
                var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A").ToList();
                var WeekDays = WeekDaysSchool.Select(c => c.WeekDay).ToList();

                /*Initialization End*/
                var UserId = Convert.ToInt32(user["WeekUserId"]);
                var DaysCal = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId && c.SlotDue != null).Select(c => c.SlotDue).FirstOrDefault();
                DaysCal = DaysCal != null && DaysCal.Contains("Minute") ? DaysCal : "15 Minutes";
                /*Logic start*/
                if (UserId != 0)
                {
                    if (user.AllKeys.Where(c => c.Contains("Day_")).Count() > 0)
                    {
                        //Remove All History Data
                        var query = "";
                        query += "update CTSP_SolutionMaster set ActiveStatus='D' where SectionName='EmployeeDays' and SchId='" + schlId + "' and ActiveStatus='A' and UserId=" + UserId + " ";
                        query += "update SPA_EmployeeSchedulers set ActiveStatus='D' where UserId=" + UserId + " and ActiveStatus='A' and schid=" + schlId + " ";
                        query += "update SPA_SchedulerSlotMaster set ActiveStatus='D' where UserId=" + UserId + " and ActiveStatus='A' and SchId=" + schlId;
                        SPA.Database.ExecuteSqlCommand(query);
                    }
                    foreach (var key in user.AllKeys.Where(c => c.Contains("Day_")))
                    {
                        wekSchedAdd = new Models.WeekScheduleAdd();
                        wekSchedAdd.WeekDay = key.ToString().Replace("Day_", "");
                        foreach (var daytime in user.AllKeys.Where(c => c.Contains(wekSchedAdd.WeekDay) && c != key))
                        {
                            wekSchedAdd.Time.Add(user[daytime].ToString());
                            wekSchedAdd.Times.Add(DateTime.Parse(user[daytime].ToString()).TimeOfDay);
                        }
                        wekSchedAddList.Add(wekSchedAdd);
                        //Days add
                        Days = new CTSP_SolutionMaster();
                        Days.SchId = schlId;
                        Days.CatgTypeId = 11143;
                        Days.UserId = UserId;
                        Days.CraetedOn = EuropeDate;
                        Days.SectionName = "EmployeeDays";
                        Days.Activestatus = "A";
                        Days.SectionDesc = wekSchedAdd.WeekDay;
                        //if (Days != null)
                        //{
                        //    SPA.CTSP_SolutionMaster.Add(Days);
                        //    SPA.SaveChanges();
                        //}
                        dayslist.Add(Days);
                        TimeSpan Lasttime = new TimeSpan();
                        int flg = 0;
                        foreach (var WeekSch in wekSchedAdd.Times.OrderBy(c => c).ToList())
                        {
                            var Firsttime = WeekSch;
                            flg = flg + 1;
                            if (flg % 2 == 0)
                                continue;
                            Lasttime = wekSchedAdd.Times.ElementAtOrDefault(flg);
                            if (Lasttime != null)
                            {
                                EmployeeSchedule = new SPA_EmployeeSchedulers();
                                EmployeeSchedule.ActiveStatus = "A";
                                EmployeeSchedule.SchId = schlId;
                                EmployeeSchedule.UserId = UserId;
                                EmployeeSchedule.WeekDay = wekSchedAdd.WeekDay;
                                EmployeeSchedule.StartTime = Firsttime;
                                EmployeeSchedule.EndTime = Lasttime;
                                EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
                                EmployeeSchedule.SchedulerId = WeekDaysSchool.Where(c => c.WeekDay == EmployeeSchedule.WeekDay).Select(c => c.SchedulerId).FirstOrDefault();
                                EmployeeSchedule.CreatedOn = EuropeDate;
                                //if (EmployeeSchedule != null)
                                //{
                                //    SPA.SPA_EmployeeSchedulers.Add(EmployeeSchedule);
                                //    SPA.SaveChanges();
                                //}
                                EmployeeScheduleList.Add(EmployeeSchedule);
                                var TempTime = Firsttime;
                                TimeSpan OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(DaysCal.ToLower().Replace(" minutes", "").Replace(" minute", "").Trim()));
                                while (TempTime >= Firsttime && TempTime <= Lasttime)
                                {
                                    schedule = new SPA_SchedulerSlotMaster();
                                    schedule.SchedulerId = EmployeeSchedule.SchedulerId.Value;
                                    schedule.StartTime = TempTime;
                                    schedule.UserId = UserId;
                                    schedule.CreatedOn = EuropeDate;
                                    schedule.ActiveStatus = "A";
                                    schedule.SlotDue = DaysCal;
                                    schedule.SchId = schlId;
                                    schSlot.Add(schedule);
                                    TempTime = TempTime + OriginalMinutes;
                                }
                                //if (schSlot.Count > 0)
                                //{
                                //    SPA.SPA_SchedulerSlotMaster.AddRange(schSlot);
                                //    SPA.SaveChanges();
                                //}
                                //schSlot = new List<SPA_SchedulerSlotMaster>();
                            }

                        }
                    }
                }
                if (dayslist.Count > 0)
                {
                    SPA.CTSP_SolutionMaster.AddRange(dayslist);
                    SPA.SaveChanges();
                    SPA.SPA_EmployeeSchedulers.AddRange(EmployeeScheduleList);
                    SPA.SaveChanges();
                    SPA.SPA_SchedulerSlotMaster.AddRange(schSlot);
                    SPA.SaveChanges();
                }
                Transaction.Commit();
                sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"UserId\":\"" + UserId + "\",");
                sb.Append("\"Error\":\"" + Error + "\",");
                sb.Append("\"ErrorMessage\":\"" + "" + "\",");
                sb.Append("}");
                send = JObject.Parse(sb.ToString());
                return send;
            }
            catch (Exception e)
            {
                Transaction.Rollback();
                int error = 1;
                sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"Error\":\"" + error + "\"");
                sb.Append("\"ErrorMessage\":\"" + Language + "\",");
                sb.Append("}");
                fu.ErrorSend(RouteData, e);
                send = JObject.Parse(sb.ToString());
                return send;
            }

        }
        //public void EmployeeTimeSlotingAccordingToShopTime(FormCollection user)
        //{
        //    try
        //    {
        //        DateTime EuropeDate = fu.ZonalDate(null);
        //        List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
        //        string time = "";
        //        TimeSpan OriginalMinutes;
        //        SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
        //        if (user.Keys.Count > 1)
        //        {
        //            int UserId = Convert.ToInt32(user["TimeSlotUserId"].Replace(",", ""));
        //            fu.CheckEmployeeTimeExist(UserId);
        //            var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A").ToList();
        //            foreach (var key in user.Keys)
        //            {
        //                //form should not be equal to TimeslotUserId
        //                if (key.ToString() != "TimeSlotUserId")
        //                {

        //                    #region Variable
        //                    int CatgTypeId = Convert.ToInt32(key);
        //                    //getCatgDesc from CategoryDetails for Getting Time Of User
        //                    var data = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatgTypeId && c.DomainType == schlId).FirstOrDefault();

        //                    #endregion
        //                    #region MinuteCode
        //                    if (data.CatgDesc.Contains("Minute"))
        //                    {
        //                        time = data.CatgDesc.Replace("Minute", "").Replace("s", "");

        //                        OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(time.Trim()));
        //                        var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == schlId && c.UserId == UserId).ToList();
        //                        #region GetDays of Employee
        //                        foreach (var Term in UserDays)
        //                        {
        //                            var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId).ToList();
        //                            #region SlotEntry
        //                            if (SlotTimeNew.Count() == 0)
        //                            {
        //                                var SlotTime = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.WeekDay == Term.SectionDesc).OrderByDescending(c => c.StartTime);
        //                                foreach (var item in SlotTime)
        //                                {
        //                                    TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
        //                                    while (TempTime <= item.EndTime)
        //                                    {
        //                                        if (BulkSchedulerSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = item.SchedulerId;
        //                                            schedule.StartTime = TempTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                        TempTime = TempTime + OriginalMinutes;
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                foreach (var item in SlotTimeNew)
        //                                {
        //                                    TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
        //                                    while (TempTime <= item.EndTime)
        //                                    {
        //                                        var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
        //                                        if (BulkSchedulerSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = WeekDayForSelected;
        //                                            schedule.StartTime = TempTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                        TempTime = TempTime + OriginalMinutes;
        //                                    }
        //                                }
        //                            }
        //                            #endregion
        //                        }
        //                        #endregion

        //                    }
        //                    #endregion
        //                    #region Day
        //                    if (data.CatgDesc.Contains("Day"))
        //                    {
        //                        var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == schlId);
        //                        foreach (var Term in UserDays)
        //                        {
        //                            int i = 0;
        //                            var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId);
        //                            #region SlotEntry
        //                            if (SlotTimeNew.Count() == 0)
        //                            {
        //                                var SlotTime = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.WeekDay == Term.SectionDesc).OrderByDescending(c => c.StartTime);
        //                                foreach (var item in SlotTime)
        //                                {
        //                                    #region OneDay
        //                                    if (!data.CatgDesc.Contains("Half"))
        //                                    {
        //                                        if (i == 0 && BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = item.SchedulerId;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                            i++;
        //                                        }
        //                                    }
        //                                    #endregion
        //                                    #region HalfDay
        //                                    else
        //                                    {
        //                                        if (BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = item.SchedulerId;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                            }
        //                            else
        //                            {
        //                                foreach (var item in SlotTimeNew)
        //                                {
        //                                    var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
        //                                    #region OneDay
        //                                    if (!data.CatgDesc.Contains("Half"))
        //                                    {
        //                                        if (i == 0 && BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = WeekDayForSelected;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                            i++;
        //                                        }
        //                                    }
        //                                    #endregion
        //                                    #region HalfDay
        //                                    else
        //                                    {
        //                                        if (BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = WeekDayForSelected;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                            }
        //                            #endregion
        //                        }
        //                    }
        //                    #endregion
        //                    #region half
        //                    if (data.CatgDesc.Contains("Half"))
        //                    {
        //                        var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == schlId && c.UserId == UserId).ToList();
        //                        foreach (var Term in UserDays)
        //                        {
        //                            //int i = 0;
        //                            var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId).OrderBy(c => c.Scheduler_Time_Id).ToList();
        //                            #region SlotEntry
        //                            if (SlotTimeNew.Count() == 0)
        //                            {
        //                                var SlotTime = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.WeekDay == Term.SectionDesc).OrderByDescending(c => c.StartTime);
        //                                foreach (var item in SlotTime)
        //                                {
        //                                    #region 1st Half
        //                                    if (data.CatgDesc.Contains("Half"))
        //                                    {
        //                                        if (SlotTime.First().SchedulerId == item.SchedulerId && BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = item.SchedulerId;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                    }
        //                                    #endregion
        //                                    #region 2nd Half
        //                                    else
        //                                    {
        //                                        if (SlotTime.Last().SchedulerId == item.SchedulerId && BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == item.SchedulerId).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = item.SchedulerId;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                            }
        //                            else
        //                            {
        //                                foreach (var item in SlotTimeNew)
        //                                {
        //                                    var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
        //                                    #region 1st Half
        //                                    if (data.CatgDesc.Contains("AM"))
        //                                    {
        //                                        if (SlotTimeNew.First().Scheduler_Time_Id == item.Scheduler_Time_Id && BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = WeekDayForSelected;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                    }
        //                                    #endregion
        //                                    #region 2nd Half
        //                                    else
        //                                    {
        //                                        if (SlotTimeNew.Last().Scheduler_Time_Id == item.Scheduler_Time_Id && BulkSchedulerSlotMaster.Where(c => c.StartTime == item.StartTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                                        {
        //                                            schedule = new SPA_SchedulerSlotMaster();
        //                                            schedule.SchedulerId = WeekDayForSelected;
        //                                            schedule.StartTime = item.StartTime;
        //                                            schedule.UserId = UserId;
        //                                            schedule.CreatedOn = EuropeDate;
        //                                            schedule.ActiveStatus = "A";
        //                                            schedule.SlotDue = data.CatgDesc;
        //                                            schedule.SchId = schlId;
        //                                            BulkSchedulerSlotMaster.Add(schedule);
        //                                        }
        //                                    }
        //                                    #endregion
        //                                }
        //                            }
        //                            #endregion
        //                        }
        //                    }
        //                    #endregion
        //                    if (BulkSchedulerSlotMaster.Count > 0)
        //                    {
        //                        fu.ScheduleSlotAdd(BulkSchedulerSlotMaster);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //    }
        //}
        public void EmployeeTimeSlotingAccordingToShopTime(FormCollection user)
        {
            var Transaction = SPA.Database.BeginTransaction();
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
                TimeSpan OriginalMinutes;
                SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
                if (user.Keys.Count > 1)
                {
                    int UserId = Convert.ToInt32(user["TimeSlotUserId"].Replace(",", ""));
                    fu.CheckEmployeeTimeExist(UserId);
                    var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A").ToList();
                    var DaysList = WeekDaysSchool.OrderBy(c => c.PeriodNumber).Select(c => c.WeekDay).Distinct().ToList();
                    foreach (var key in user.Keys)
                    {
                        //form should not be equal to TimeslotUserId
                        if (key.ToString() != "TimeSlotUserId")
                        {
                            #region Variable
                            int CatgTypeId = Convert.ToInt32(key);
                            //getCatgDesc from CategoryDetails for Getting Time Of User
                            var GetEmployeeSlot = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatgTypeId && c.DomainType == schlId).FirstOrDefault();
                            #endregion
                            if (GetEmployeeSlot != null)
                            {
                                var DaysCal = GetEmployeeSlot.CatgDesc;
                                DaysCal = DaysCal != null && DaysCal.Contains("Minute") ? DaysCal : "15 Minutes";
                                //All the timings of Employee as per all day
                                var getAllEmployeeDays = SPA.SPA_EmployeeSchedulers.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId).ToList();
                                foreach (var days in DaysList)
                                {
                                    //All the timings of Employee as per particular day
                                    var getDays = getAllEmployeeDays.Where(c => c.WeekDay == days).ToList();
                                    foreach (var EmpDays in getDays.OrderBy(c => c.StartTime).ToList())
                                    {
                                        var TempTime = EmpDays.StartTime;
                                        var schedulerId = EmpDays.SchedulerId != null && EmpDays.SchedulerId != 0 ? EmpDays.SchedulerId.Value : WeekDaysSchool.Where(c => c.WeekDay == days).Select(c => c.SchedulerId).FirstOrDefault();
                                        OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(DaysCal.ToLower().Replace(" minutes", "").Replace(" minute", "").Trim()));
                                        while (TempTime >= EmpDays.StartTime && TempTime <= EmpDays.EndTime)
                                        {
                                            //Saving Time according to slot as per particular employee
                                            if (BulkSchedulerSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == schedulerId).Count() == 0)
                                            {
                                                schedule = new SPA_SchedulerSlotMaster();
                                                schedule.SchedulerId = schedulerId;
                                                schedule.StartTime = TempTime;
                                                schedule.UserId = UserId;
                                                schedule.CreatedOn = EuropeDate;
                                                schedule.ActiveStatus = "A";
                                                schedule.SlotDue = DaysCal;
                                                schedule.SchId = schlId;
                                                BulkSchedulerSlotMaster.Add(schedule);
                                            }
                                            TempTime = TempTime + OriginalMinutes;
                                        }
                                    }
                                }
                                if (BulkSchedulerSlotMaster.Count > 0)
                                {
                                    fu.ScheduleSlotAdd(BulkSchedulerSlotMaster);
                                }
                                Transaction.Commit();
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Transaction.Rollback();
                fu.ErrorSend(RouteData, e);
            }
        }
        #endregion
        public ActionResult Logout()
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
                Session["Message"] = "";
                Session["Remember"] = "";
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
        public void AddEmployeeProduct(FormCollection user)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                int userId = Convert.ToInt32(user["ProductUserId"]);
                List<long> CatgTypeIdlist = new List<long>();
                #region Logic
                foreach (var key in user.Keys)
                {
                    if (key.ToString() != "ProductUserId")
                    {
                        long catgType = 0;
                        Int64.TryParse(Convert.ToString(key), out catgType);
                        if (catgType != 0)
                            CatgTypeIdlist.Add(Convert.ToInt32(key));
                    }
                }
                #endregion
                //Assign Product to Employee
                if (CatgTypeIdlist.Count > 0)
                    fu.AssignProductToEmp(new List<long> { userId }, CatgTypeIdlist, EuropeDate);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }


        }
        public ActionResult TestForm()
        {
            try
            {
                int? UserId = 111376;
                List<CTSP_SolutionMaster> CatDetails = new List<CTSP_SolutionMaster>();
                var CatDetailsTemp = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgName == "SPA Products" && c.CCTSP_CategoryDetails.ActiveStatus == "A" && c.Activestatus == "A" && c.SchId == schlId).OrderByDescending(c => c.OrderData).ToList();
                var CatEmpDetails = SPA.CTSP_SolutionMaster.Where(c => c.UserId == UserId && c.SchId == schlId && c.SectionName == "EmployeeProduct" && c.Activestatus == "A");
                foreach (var Item in CatDetailsTemp)
                {
                    var DataAdd = CatEmpDetails.Where(c => c.SectionDesc == Item.CCTSP_CategoryDetails.CatgDesc && c.Amount == Item.Amount && c.Duration == Item.Duration).FirstOrDefault();
                    if (DataAdd == null)
                    {
                        CatDetails.Add(Item);
                    }
                }
                ViewBag.EmployeeProducts = CatDetails.Count;
                if (UserId != null)
                {
                    var subquery = (from p in SPA.CTSP_SolutionMaster where p.SectionName == "EmployeeProduct" && p.SchId == schlId && p.UserId != UserId select p.UserId).Distinct();
                    var outerquery = (from q in SPA.CCTSP_User where q.SchId == schlId && q.ActiveStatus == "A" && q.RoleId == 3 && subquery.Contains(q.UserId) select q);
                    ViewBag.EmployeeInfoDisplay = outerquery;
                }
                else
                {
                    var subquery = (from p in SPA.CTSP_SolutionMaster where p.SectionName == "EmployeeProduct" && p.SchId == schlId select p.UserId).Distinct();
                    var outerquery = (from q in SPA.CCTSP_User where q.SchId == schlId && q.ActiveStatus == "A" && q.RoleId == 3 && subquery.Contains(q.UserId) select q);
                    ViewBag.EmployeeInfoDisplay = outerquery;
                }
                return View(CatDetails);

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public string TestForm1(FormCollection user)
        {
            try
            {
                var UserID = user["ResId"];
                return UserID;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }

        }
        public ActionResult AssignProduct(long? UserId)
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.Language = fu.getLanguageData("Create_Employee", 0, ShopInfo.Lang_id.Value);
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                //List<CTSP_SolutionMaster> CatDetails = new List<CTSP_SolutionMaster>();
                ///*Main SPA Products*/
                //var CatDetailsTemp = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgName == "SPA Products" && c.CCTSP_CategoryDetails.ActiveStatus == "A" && c.Activestatus == "A" && c.SchId == schlId).OrderByDescending(c => c.OrderData).ToList();
                ///*Employee Products*/
                //var CatEmpDetails = SPA.CTSP_SolutionMaster.Where(c => c.UserId == UserId && c.SchId == schlId && c.SectionName == "EmployeeProduct" && c.Activestatus == "A");
                ///*checks Employee Product and shop Products, if any product not assigned then that will be shown*/
                //foreach (var Item in CatDetailsTemp)
                //{
                //    var DataAdd = CatEmpDetails.Where(c => c.SectionDesc == Item.CCTSP_CategoryDetails.CatgDesc && c.Amount == Item.Amount && c.Duration == Item.Duration).FirstOrDefault();
                //    if (DataAdd == null) { CatDetails.Add(Item); }
                //}
                ///*Get ProductId List*/
                //var prodIdList = CatDetails.Select(c => c.CatgTypeId).Distinct().ToList();
                ///*get all Groupings from Product*/
                //var GroupList = (from Grup in SPA.CCTSP_GroupProductDetails where Grup.schlId == schlId && Grup.ActiveStatus == "A" && prodIdList.Contains(Grup.ProductId) select Grup).ToList();
                //var Groupidlist = GroupList.Select(c => c.GroupIdByShop).Distinct().ToList();
                ///*get all Details for Groupings*/
                //var Groupname = (from GroupDetails in SPA.CCTSP_CategoryDetails where GroupDetails.DomainType == schlId && GroupDetails.ActiveStatus == "A" && GroupDetails.CatgId == 126 && Groupidlist.Contains(GroupDetails.CatgTypeId) select GroupDetails).ToList();
                //ViewBag.Groupname = Groupname;
                //ViewBag.GroupList = GroupList;

                //ViewBag.EmployeeProducts = CatDetails.Count;

                var AllProductExceptEmp = "select a.CatgDesc as ProductName,a.Gender,b.SectionDesc," +
                    "CAST(CASE WHEN ISNUMERIC(b.Amount) = 1 THEN b.Amount ELSE NULL END AS decimal(38, 2)) as Amount," +
                    "cast(b.Duration as int) as Duration,c.catgTypeId as GroupIdByShop,a.CatgTypeId as ProductId," +
                    "c.CatgDesc as GroupName from " +
                    /*get Product Info*/
                    "cctsp_categoryDetails a " +
                    /*get Product Detail Info*/
                    "join ctsp_solutionMaster b on a.CatgTypeId = b.CatgTypeId " +
                    /*get Group details*/
                    "join CCTSP_CategoryDetails c on c.CatgId = 126 and c.ActiveStatus = 'A' " +
                    "where a.CatgId = 111 and a.ActiveStatus = 'A' and a.DomainType = " + schlId + " " +
                    "and b.ActiveStatus = 'A' and b.schId = a.DomainType ";
                if (UserId != null)
                {
                    AllProductExceptEmp = AllProductExceptEmp + " and a.CatgTypeId not in " +
                   "(select y.CatgTypeId from ctsp_solutionMaster x " +
                   "join cctsp_categoryDetails y on y.CatgDesc = x.SectionDesc " +
                   "join ctsp_solutionMaster z on z.CatgTypeId = y.CatgTypeId and z.Amount = x.Amount and z.Duration = x.Duration " +
                   "where x.SectionName = 'EmployeeProduct' and x.ActiveStatus = 'A' and y.ActiveStatus = 'A' and z.ActiveStatus = 'A' and " +
                   "x.SchId = a.DomainType and y.DomainType = a.DomainType and z.SchId = a.DomainType  and x.UserId = " + UserId + ")";
                }
                AllProductExceptEmp = AllProductExceptEmp + " and c.CatgTypeId in (select GroupIdbyShop from CCTSP_GroupProductDetails where schlId = a.DomainType and " +
                  "ProductId = a.CatgTypeId and ActiveStatus = 'A') " +
                  "order by c.Group_orderdata,b.orderdata";
                var ExeAllProductExceptEmp = SPA.Database.SqlQuery<Models.GroupProductList>(AllProductExceptEmp).ToList();
                //if (UserId != null)
                //{
                //    var subquery = (from p in SPA.CTSP_SolutionMaster where p.SectionName == "EmployeeProduct" && p.SchId == schlId && p.UserId != UserId && p.Activestatus == "A" select p.UserId).Distinct();
                //    var outerquery = (from q in SPA.CCTSP_User where q.SchId == schlId && q.ActiveStatus == "A" && q.RoleId == 3 && subquery.Contains(q.UserId) select q);
                //    ViewBag.EmployeeInfoDisplay = outerquery;
                //}
                //else
                //{
                //    var subquery = (from p in SPA.CTSP_SolutionMaster where p.SectionName == "EmployeeProduct" && p.SchId == schlId && p.Activestatus == "A" select p.UserId).Distinct();
                //    var outerquery = (from q in SPA.CCTSP_User where q.SchId == schlId && q.ActiveStatus == "A" && q.RoleId == 3 && subquery.Contains(q.UserId) select q);
                //    ViewBag.EmployeeInfoDisplay = outerquery;
                //}
                return View(ExeAllProductExceptEmp);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult UserProductAssigned(int? UserId)
        {
            try
            {
                var querProd = "";
                var querProdList = new List<Models.GroupProductList>();
                if (UserId != null)
                {
                    querProd = "select a.SolutionId,b.CatgDesc as ProductName,b.Gender," +
                    "CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,cast(c.Duration as int) as Duration,c.SectionDesc," +
                    "d.catgTypeId as GroupIdByShop,b.CatgTypeId as ProductId,d.CatgDesc as GroupName " +
                    /*Employee Assigned Product*/
                    "from ctsp_solutionMaster a " +
                    /*Product Table*/
                    "join cctsp_categoryDetails b on a.SectionDesc = b.CatgDesc " +
                    /*Product Detail Table*/
                    "join ctsp_solutionMaster c on c.CatgTypeId = b.CatgTypeId " +
                    "join CCTSP_CategoryDetails d on d.CatgId = 126 " +
                    /*join CCTSP_GroupProductDetails d on d.productId = c.CatgTypeId
                    join CCTSP_CategoryDetails e on e.CatgTypeId = d.GroupIdByShop*/
                    "where a.ActiveStatus = 'A' and a.schId = " + schlId + " and a.Amount = c.Amount and a.Duration = c.Duration and a.CatgTypeId = 11145 and a.UserId = " + UserId + " " +
                    "and b.ActiveStatus = 'A' and b.DomainType = a.schId and b.CatgId = 111 and c.ActiveStatus = 'A' and c.schId = a.schId and c.SectionDesc = a.Image " +
                    "and d.ActiveStatus = 'A' and d.DomainType = a.schId and d.CatgTypeId in (select GroupIdbyShop from CCTSP_GroupProductDetails where schlId = a.schId " +
                    "and ProductId = b.CatgTypeId and ActiveStatus = 'A') " +
                    /*and e.ActiveStatus = 'A' and e.DomainType = a.schId*/
                    "order by d.Group_orderdata,c.orderdata";
                    querProdList = SPA.Database.SqlQuery<Models.GroupProductList>(querProd).ToList();
                }
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == ShopInfo.Lang_id).ToList();
                ViewBag.querProdList = querProdList;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        //public string VacationAdd(FormCollection vacation)
        //{
        //    var Result = "";
        //    string dt1 = "";
        //    string dt2 = "";
        //    DateTime EuropeDate = fu.ZonalDate(null);
        //    try
        //    {
        //        SPA_LeaveMaster Leave = new SPA_LeaveMaster();
        //        if (vacation["EditVacationId"] == "")
        //        {
        //            Leave.SchId = schlId;
        //            Leave.UserId = Convert.ToInt32(vacation["VacationId"].ToString());
        //            dt1 = vacation["StartDate"].ToString();
        //            DateTime start = DateTime.Parse(dt1, enGB);
        //            Leave.StartDate = start;
        //            dt2 = vacation["EndDate"].ToString();
        //            DateTime End = DateTime.Parse(dt2, enGB);
        //            Leave.EndDate = End;
        //            Leave.CreatedOn = EuropeDate;
        //            Leave.HolidayDesc = vacation["VacationDesc"];
        //            Leave.HolidayTypeId = 11147;
        //            Leave.ActiveStatus = "A";
        //            SPA.SPA_LeaveMaster.Add(Leave);
        //        }
        //        else
        //        {
        //            long LeaveId = Convert.ToInt64(vacation["EditVacationId"]);
        //            Leave = SPA.SPA_LeaveMaster.Where(c => c.LeaveId == LeaveId).FirstOrDefault();
        //            dt1 = vacation["StartDate"].ToString();
        //            DateTime St = DateTime.Parse(dt1, enGB);
        //            Leave.StartDate = St;
        //            dt2 = vacation["EndDate"].ToString();
        //            DateTime Edt = DateTime.Parse(dt2, enGB);
        //            Leave.EndDate = Edt;
        //            Leave.HolidayDesc = vacation["VacationDesc"].ToString();
        //        }
        //        var ChkShopLeaves = "select top 1* from SPA_EmployeeScheduler b join CCTSP_CategoryDetails d on d.CatgTypeId=b.Product_Id  where ((b.ActiveStatus = 'DA' and b.BookedStatus = 'B') or(b.ActiveStatus = 'A' and b.BookedStatus = 'B') or (b.ActiveStatus = 'C' and b.BookedStatus = 'C')) and convert(date, b.BookingDate) BETWEEN convert(date, '" + DateTime.Parse(dt1.ToString(), enGB).ToString("yyyy/MM/dd") + "') AND convert(date, '" + DateTime.Parse(dt2.ToString(), enGB).ToString("yyyy/MM/dd") + "') and b.schId=" + schlId + " and b.Emp_UserId =" + Leave.UserId;
        //        var ShopLeavesList = SPA.Database.SqlQuery<SPA_EmployeeScheduler>(ChkShopLeaves).FirstOrDefault();
        //        if (ShopLeavesList != null)
        //            Result = "ErrorLeaves" + "~" + dt1 + "~" + dt2 + "~" + Leave.UserId;
        //        else
        //            SPA.SaveChanges();

        //        return Result;
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return "Error";
        //    }

        //}
        public JObject VacationAdd(FormCollection vacation)
        {
            DateTime EuropeDate = fu.ZonalDate(null);
            try
            {
                string dt1 = "";
                string dt2 = "";
                JObject send = null;
                string jsondata = "";
                SPA_LeaveMaster Leave = new SPA_LeaveMaster();
                if (vacation["EditVacationId"] == "")
                {
                    Leave.SchId = schlId;
                    Leave.UserId = Convert.ToInt32(vacation["VacationId"].ToString());
                    dt1 = vacation["StartDate"].ToString();
                    DateTime start = DateTime.Parse(dt1, enGB);
                    Leave.StartDate = start;
                    dt2 = vacation["EndDate"].ToString();
                    DateTime End = DateTime.Parse(dt2, enGB);
                    Leave.EndDate = End;
                    Leave.CreatedOn = EuropeDate;
                    Leave.HolidayDesc = vacation["VacationDesc"];
                    Leave.HolidayTypeId = 11147;
                    Leave.ActiveStatus = "A";
                    SPA.SPA_LeaveMaster.Add(Leave);
                }
                else
                {
                    long LeaveId = Convert.ToInt64(vacation["EditVacationId"]);
                    Leave = SPA.SPA_LeaveMaster.Where(c => c.LeaveId == LeaveId).FirstOrDefault();
                    dt1 = vacation["StartDate"].ToString();
                    DateTime St = DateTime.Parse(dt1, enGB);
                    Leave.StartDate = St;
                    dt2 = vacation["EndDate"].ToString();
                    DateTime Edt = DateTime.Parse(dt2, enGB);
                    Leave.EndDate = Edt;
                    Leave.HolidayDesc = vacation["VacationDesc"].ToString();
                }
                var ChkShopLeaves = "select top 1* from SPA_EmployeeScheduler b join CCTSP_CategoryDetails d on d.CatgTypeId=b.Product_Id  where ((b.ActiveStatus = 'DA' and b.BookedStatus = 'B') or(b.ActiveStatus = 'A' and b.BookedStatus = 'B') or (b.ActiveStatus = 'C' and b.BookedStatus = 'C')) and convert(date, b.BookingDate) BETWEEN convert(date, '" + DateTime.Parse(dt1.ToString(), enGB).ToString("yyyy/MM/dd") + "') AND convert(date, '" + DateTime.Parse(dt2.ToString(), enGB).ToString("yyyy/MM/dd") + "') and b.schId=" + schlId + " and b.Emp_UserId =" + Leave.UserId;
                var ShopLeavesList = SPA.Database.SqlQuery<SPA_EmployeeScheduler>(ChkShopLeaves).FirstOrDefault();
                Models.vacation VacationDetails = new Models.vacation();
                if (ShopLeavesList != null)
                {
                    VacationDetails.StartDate = dt1;
                    VacationDetails.EndDate = dt2;
                    VacationDetails.UserId = Leave.UserId;
                    VacationDetails.Status = "ErrorLeaves";
                }
                else
                    SPA.SaveChanges();
                jsondata = js.Serialize(VacationDetails);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                JObject send = null;
                return send;
            }

        }


        public ActionResult VacationDetail(long? UserId)
        {
            try
            {
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Create_Employee" && c.Lang_id == LanguageId).ToList();
                var data = SPA.SPA_LeaveMaster.Where(c => c.ActiveStatus == "A" && c.HolidayTypeId == 11147 && c.SchId == schlId && c.UserId == UserId);
                if (data.Count() != 0)
                    ViewBag.dataCount = data.Count();
                return View(data);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult EmployeeMaster()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4 && Convert.ToInt32(Session["RoleId"].ToString()) > 0)
                //{
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var UserData = " select a.UserId,a.Emailid as Email,a.PhoneNo as PhoneNumber,a.Answer2 as EmpImg,a.FirstName , a.BaseUrl," +
                               " a.LastName as FamilyName, " +
                               " b.ItenName as FlowStatus,c.AccessToData,c.insertAccess,c.DeleteAccess,c.UpdateAccess" +
                               " from cctsp_User a " +
                               " join ctsp_SolutionMaster b on b.SectionName = 'Employee' " +
                               " join cctsp_user d on d.UserId = " + UserId + " " +
                               " join cctsp_Role e on e.RoleId=d.RoleId and (e.Schlid=" + schlId + " or e.Schlid=236) and e.ActiveStatus='A'" +
                               " join cctsp_RoleDetails c on c.ActiveStatus = 'A' and e.RoleId = c.RoleId " +
                               " and c.Schid = e.Schlid  and " +
                               " ((c.AccessToData = 'Own' and a.UserId = d.UserId) or (c.AccessToData != 'Own')) " +
                               " and b.SolutionId = convert(bigint, c.AccesstoSub) " +
                               " where a.ActiveStatus = 'A' and a.Schid = " + schlId + " and a.RoleId not in (1,4) " +
                               " group by  a.UserId,a.BaseUrl,a.Emailid,a.PhoneNo,a.Answer2,a.FirstName,a.LastName,b.ItenName, " +
                               " c.AccessToData,c.insertAccess,c.DeleteAccess,c.UpdateAccess";
                var data = SPA.Database.SqlQuery<Models.CustomerDisplay>(UserData).ToList();
                if (data.Count == 0)
                {
                    ViewBag.AddAccess = SPA.Database.SqlQuery<int>(new Sql().checkinsertAccess(UserId, "Employee")).FirstOrDefault();
                }
                //var data = SPA.CCTSP_User.Where(c => c.RoleId == 3 && c.ActiveStatus == "A" && c.SchId == schlId).OrderByDescending(c => c.UserId);
                //ViewBag.DataCount = data.Count();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, SchlStudentStrength = c.SchlStudentStrength }).FirstOrDefault();
                int LanguageId = ShopInfo.Lang_id.Value;
                int Doctorflow = 0;
                if (ShopInfo.SchlStudentStrength > 0)
                    Doctorflow = ShopInfo.SchlStudentStrength.Value;
                ViewBag.Doctorflow = Doctorflow;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Employee_Tab" || (c.Page_Name == "Title" && c.Order_id == 10)) && c.Lang_id == LanguageId).Select(c => new Models.LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Lang_id = c.Lang_id,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id
                }).ToList();
                return View(data);
                //}
                //else
                //    return RedirectToAction("SignIn", "Login");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }


        }
        public void DeleteEmployeeProduct(long? solutionId)
        {
            try
            {
                var query = "update CTSP_SolutionMaster set ActiveStatus='D' where SolutionId=" + solutionId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public void DeleteLeave(long? LeaveId)
        {
            try
            {
                var query = "update SPA_LeaveMaster set ActiveStatus='D' where LeaveId=" + LeaveId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        //public JObject EditLeave(long? LeaveId)
        //{
        //    try
        //    {
        //        JObject send = null;
        //        StringBuilder sb = null;
        //        SPA_LeaveMaster user = new SPA_LeaveMaster();
        //        user = SPA.SPA_LeaveMaster.Where(c => c.LeaveId == LeaveId).FirstOrDefault();
        //        sb = new StringBuilder();
        //        sb.Append("{");
        //        sb.Append("\"HolidayName\":\"" + user.HolidayDesc + "\",");
        //        sb.Append("\"From\":\"" + DateTime.Parse(user.StartDate.ToString()).ToString("dd/MM/yyyy") + "\",");
        //        sb.Append("\"To\":\"" + DateTime.Parse(user.EndDate.ToString()).ToString("dd/MM/yyyy") + "\"");
        //        sb.Append("}");
        //        send = JObject.Parse(sb.ToString());
        //        return send;
        //    }
        //    catch (Exception e)
        //    {
        //        JObject send = null;
        //        StringBuilder sb = null;
        //        sb = new StringBuilder();
        //        sb.Append("{");
        //        sb.Append("\"Error\":\"" + "" + "\"");
        //        sb.Append("}");
        //        fu.ErrorSend(RouteData, e);
        //        send = JObject.Parse(sb.ToString());
        //        return send;
        //    }

        //}
        public JObject EditLeave(long? LeaveId)
        {
            try
            {
                JObject send = null;
                string Jasondata = "";
                SPA_LeaveMaster user = new SPA_LeaveMaster();
                user = SPA.SPA_LeaveMaster.Where(c => c.LeaveId == LeaveId).FirstOrDefault();
                string stdate = DateTime.Parse(user.StartDate.ToString()).Date.ToString("dd/MM/yyyy");
                if (user.StartTime != null)
                    stdate = DateTime.Parse(user.StartDate.Value.Date.ToString("dd/MM/yyyy") + " " + user.StartTime).ToString("dd/MM/yyyy");
                string enddt = DateTime.Parse(user.EndDate.ToString()).Date.ToString("dd/MM/yyyy");
                if (user.EndTime != null)
                    enddt = DateTime.Parse(user.EndDate.Value.Date.ToString("dd/MM/yyyy") + " " + user.EndTime.ToString()).ToString("dd/MM/yyyy");
                Models.EditShopDetails EmpLeave = new Models.EditShopDetails()
                { HolidayName = user.HolidayDesc, StartDate = stdate, EndDate = enddt };
                Jasondata = js.Serialize(EmpLeave);
                send = JObject.Parse(Jasondata);
                return send;
            }
            catch (Exception e)
            {
                JObject send = null;
                fu.ErrorSend(RouteData, e);
                return send;
            }

        }
        public void TransferEmployeeProduct(long? EmpUserId, long? CopyFrom)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                var query = "update CTSP_SolutionMaster set ActiveStatus='D' where UserId=" + EmpUserId + "and SchId=" + schlId + "and SectionName='EmployeeProduct'";
                SPA.Database.ExecuteSqlCommand(query);
                var EmpData = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "EmployeeProduct" && c.UserId == CopyFrom && c.Activestatus == "A").ToList();
                foreach (var item in EmpData)
                {
                    #region Solution
                    CTSP_SolutionMaster Solution = new CTSP_SolutionMaster();
                    Solution.CatgTypeId = item.CatgTypeId;
                    Solution.SectionName = item.SectionName;
                    Solution.SectionDesc = item.SectionDesc;
                    Solution.CraetedOn = EuropeDate;
                    Solution.Activestatus = "A";
                    Solution.UserId = EmpUserId;
                    Solution.SchId = schlId;
                    Solution.Amount = item.Amount;
                    Solution.Duration = item.Duration;
                    SPA.CTSP_SolutionMaster.Add(Solution);
                    SPA.SaveChanges();
                    #endregion
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public void GetLatestTime()
        {
            try
            {
                var Time = SPA.CCTSP_SchedulerMaster.Where(c => c.ActiveStatus == "A" && c.SchId == schlId);
                List<string> TimeList = new List<string>();
                TimeSpan T = Time.First().StartTime.Value;
                while (T < Time.Last().EndTime)
                {
                    TimeList.Add(Time.First().StartTime.Value.ToString("HH:mm"));
                }

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }

        public ActionResult specialization(long? UserId)
        {
            int Langid = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                         .Select(c => c.Lang_id).FirstOrDefault().Value;
            var CategoryList = SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgName == "Specialization"
                               && c.ActiveStatus == "A" && c.Lang_Id == Langid)
                               .Select(c => new Models.MainCategoryDetails
                               { MainCategory = c.CatgDesc, MainCatgId = c.CatgTypeId, WorkFlowId = c.Work_Flow_Id })
                               .OrderBy(c => c.MainCategory).ToList();
            ViewBag.Category = CategoryList;
            var SelectedData = new List<Models.MainCategoryDetails>();
            var Default = CategoryList.Where(c => c.WorkFlowId == 1).FirstOrDefault();
            //Get Employee Basic Info
            Models.GetCustomerSpecialization EmployeeInfo = new Models.GetCustomerSpecialization();
            if (UserId > 0)
            {
                EmployeeInfo = SPA.CCTSP_User
               .Where(c => c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId)
               .Select(c => new Models.GetCustomerSpecialization
               {
                   degree = c.Degree,
                   ExperienceYear = c.ExperienceYear,
                   ExperienceMonth = c.ExperienceMonth,
                   skypeId = c.skypeId,
                   specialization = c.specialization,
                   UserId = UserId
               }).FirstOrDefault();
                var CatgList = new List<long>();
                if (EmployeeInfo.specialization != null && EmployeeInfo.specialization != "")
                    CatgList = EmployeeInfo.specialization.Split(',').Select(long.Parse).ToList();
                SelectedData = CategoryList.Where(c => CatgList.Contains(c.MainCatgId)).ToList();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Specialization" && c.ActiveStatus == "A" && c.Lang_id == Langid).Select(c => new Models.LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Lang_id = c.Lang_id,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id
                }).ToList();
                //ViewBag.CatgList = CatgList;
            }
            var Count = 4 - SelectedData.Count;
            if (Count != 0)
            {
                for (int i = 0; i <= Count - 1; i++)
                    SelectedData.Add(Default);
            }
            ViewBag.CatgList = SelectedData;
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Specialization" && c.ActiveStatus == "A" && c.Lang_id == Langid).Select(c => new Models.LanguageLabelDetails
            {
                English_Label = c.English_Label,
                Value = c.Value,
                Lang_id = c.Lang_id,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id
            }).ToList();
            return View(EmployeeInfo);
        }

        [HttpPost]
        public ActionResult specialization(Models.GetCustomerSpecialization specializationDetails)
        {
            if (specializationDetails != null)
            {
                var UserData = SPA.CCTSP_User.Where(c => c.UserId == specializationDetails.UserId).FirstOrDefault();
                if (UserData != null)
                {
                    UserData.Degree = specializationDetails.degree;
                    UserData.ExperienceYear = specializationDetails.ExperienceYear;
                    UserData.ExperienceMonth = specializationDetails.ExperienceMonth;
                    UserData.skypeId = specializationDetails.skypeId;
                    List<long> CatgTypeIdlist = new List<long>();
                    if (specializationDetails.HSpecialization_1 > 0)
                        CatgTypeIdlist.Add(specializationDetails.HSpecialization_1.Value);
                    if (specializationDetails.HSpecialization_2 > 0)
                        CatgTypeIdlist.Add(specializationDetails.HSpecialization_2.Value);
                    if (specializationDetails.HSpecialization_3 > 0)
                        CatgTypeIdlist.Add(specializationDetails.HSpecialization_3.Value);
                    if (specializationDetails.HSpecialization_4 > 0)
                        CatgTypeIdlist.Add(specializationDetails.HSpecialization_4.Value);
                    if (CatgTypeIdlist.Count > 0)
                        UserData.specialization = string.Join(",", CatgTypeIdlist.Distinct());
                    else
                        UserData.specialization = null;
                    SPA.SaveChanges();
                    return RedirectToAction("CreateEmployee", "Employee", new { UserId = UserData.UserId });
                }
                else
                    return RedirectToAction("EmployeeMaster", "Employee");
            }
            else
                return RedirectToAction("EmployeeMaster", "Employee");

        }
        //public ActionResult getDynamicLane(string Label1, string Label2, int index)
        public ActionResult getDynamicLane(string Label1, string Label2, int index, string slot)
        {
            ViewBag.lbl1 = Label1;
            ViewBag.lbl2 = Label2;
            ViewBag.index = index;
            var days = SPA.CCTSP_SchedulerMaster
                              .Where(c => c.ActiveStatus == "A" && c.SchId == schlId).OrderBy(c => c.PeriodNumber)
                              .Distinct()
                              .Select(c => new Models.WeekSchedule
                              {
                                  WeekDay = c.WeekDay,
                                  starttime = c.StartTime,
                                  endTime = c.EndTime,
                                  periodNumber = c.PeriodNumber
                              }).ToList();
            var day = days.Select(c => c.WeekDay).Distinct().ToList();
            ViewBag.slot = slot;
            return View("WeekScheduleDyn", day);
        }
    }
}