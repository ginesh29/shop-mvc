using Newtonsoft.Json.Linq;
using SPA.Common;
using SPA.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SPA.Controllers
{
    [checkShop]
    public class ShopController : Controller
    {
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        // GET: Employee
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        CultureInfo enGB = new CultureInfo("en-GB");
        Function fu = new Function();
        PuchSMS Sms = new PuchSMS();
        PushEmail Email = new PushEmail();
        JavaScriptSerializer js = new JavaScriptSerializer();
        Sql sql = new Sql();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public ShopController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: Shop
        [CustomAutohrize(null)]
        public ActionResult Shop()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4 && Convert.ToInt32(Session["RoleId"].ToString()) > 0)
                //{
                var ShopInfo = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schlId).Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.CCTSP_SchoolMaster.Lang_id,
                    Display_FreeCustomer = c.CCTSP_SchoolMaster.Display_FreeCustomer,
                    Display_Invoice = c.CCTSP_SchoolMaster.Display_Invoice,
                    DisplayMarketing = c.Display_Marketing,
                    BookingApproval = c.CCTSP_SchoolMaster.BookingApproval,
                    SchlWebsite = c.CCTSP_SchoolMaster.SchlWebsite,
                    Flow_Id=c.CCTSP_SchoolMaster.Flow_Id
                }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Tab" && c.Lang_id == ShopInfo.Lang_id && c.ActiveStatus == "A").Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Value = c.Value
                }).ToList();
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 8 && c.Lang_id == ShopInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                ViewBag.Schlid = schlId;
                var MTab = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schlId).Select(c => c.Display_Marketing).FirstOrDefault();
                ViewBag.DisplayTab = MTab;
                ViewBag.SubMenu = fu.AccessSubMenu(ShopInfo.Lang_id.Value, Convert.ToInt32(Session["RoleId"].ToString()), "Shop_Tab",ShopInfo.Flow_Id);
                List<Models.ConditionList> ConditionList = new List<Models.ConditionList>();
                //Models.ConditionList Condition = new ConditionList();
                //Condition.Condition = "Language";
                //Condition.ConditionValue = schlId == 251 || schlId == 273 ? "YES" : "NO";
                //ConditionList.Add(Condition);
                //Condition = new Models.ConditionList();
                //Condition.Condition = "Marketing";
                //Condition.ConditionValue = MTab == "1" ? "YES" : "NO";
                //ConditionList.Add(Condition);
                //Condition = new Models.ConditionList();
                //Condition.Condition = "Payment";
                //Condition.ConditionValue = schlId == 254 || schlId == 257 ? "YES" : "NO";
                //ConditionList.Add(Condition);
                ViewBag.ConditionList = fu.LayoutCondition(ShopInfo); ;
                return View();
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
        [CustomAutohrize(null)]
        public ActionResult ShopOwnerSetup()
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                Response.Cache.SetNoServerCaching();
                Response.Cache.SetNoStore();
                Response.Cookies.Clear();
                Request.Cookies.Clear();
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                string ShopDetails = "SELECT a.SchlName as SHOPNAME,a.Lang_id as LangId,a.latitude,a.longitude, " +
                                     "a.location as Location,c.ShopDesc, a.Schlid as ShopId, d.CatgTypeId as MainCatgId, " +
                                     "e.SolutionId as ShopTypeId,d.CatgDesc as MAINCATEGORY,e.SectionName as SHOPTYPE,b.Gender as TITLE, " +
                                     "b.firstname as OWNERNAME,b.Lastname as OWNERSURNAME,b.Phoneno as MOBILENUMBER, a.SchlCity as CITY ,  " +
                                     "a.SchPin as ZIPCODE,a.SchlEmail as ShopEmail,b.EmailId as OwnerEmail,a.SchlAddress as ADDRESS ,  " +
                                     "a.ImageLogo as ImageLogo,a.SchlWebsite as ShopWebsite,a.BANKNAME ,a.BANKACCOUNT,a.IBANNO " +
                                     ",b.Salutation as SALUTATION,b.PasswordQuerry2 as LANDLINENUMBER,b.[state] as State," +
                                     "b.GLN_No as GLN_NO,b.ZSR_No as ZSR_NO,b.Title as GENDER,a.Vat as VAT,a.VatNumber as VATNUMBER," +
                                     "a.Currency as Currency,a.OverDue as PaymentDuration,a.Extend_Pay_Duration as ExtendDuration,a.street, "+
                                     "a.streetNumber,a.Display_Invoice,a.Invoice_FreeText as InvoiceFreeText,a.SchCountry as Country,a.SchlStudentStrength,a.Flow_Id as Flow_Id " +
                                     ", InvoiceEmailTxt = Case When a.Invoice_Email_Txt != '' then a.Invoice_Email_Txt else f.value end "+
                                     "FROM cctsp_Schoolmaster a " +
                                     "JOIN cctsp_User b on a.Schlid = b.schid  " +
                                     "JOIN CCTSP_LendingPageMaster c on c.Schid = a.Schlid " +
                                     "JOIN CCTSP_CategoryDetails d on d.CatgTypeId = a.MainCategory " +
                                     "JOIN ctsp_SolutionMaster e on e.SolutionId = convert(bigint, a.ShopType) " +
                                     "LEFT JOIN Language_Label_Detail f on f.Lang_Id=a.Lang_Id and f.ActiveStatus='A' and f.Page_Name='Shop_Owner_Text' and f.Order_Id=29 "+
                                     "WHERE a.Schlid = " + schlId + " and b.RoleId = 1 and a.ActiveStatus = 'A' and b.ActiveStatus = 'A'";
                var ShopInfo = SPA.Database.SqlQuery<Models.shopMaster>(ShopDetails).FirstOrDefault();
                ViewBag.CheckInvoice = fu.CheckInvoice(ShopInfo.Display_Invoice);
                var CatgNameList = "Salutation,New_Gender,Title";
                ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, ShopInfo.LangId.Value);
                bool TextTabAccess = true;
                if(ShopInfo.Flow_Id>1)
                    TextTabAccess = fu.CheckSubTabAccess("Texts", ShopInfo.Flow_Id.Value);
                ViewBag.TextTabAccess = TextTabAccess;
                ViewBag.ShopInfo = ShopInfo;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Owner_setup" && c.Lang_id == ShopInfo.LangId).Select(c => new LanguageLabelDetails { Value = c.Value, Order_id = c.Order_id, Lang_id = c.Lang_id, Page_Name = c.Page_Name }).ToList();
                //ViewBag.Title = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == ShopInfo.LangId).Select(c => c.CatgType).OrderBy(c => c).ToList();
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
        public ActionResult Shopschedule()
        {
            var Days = SPA.CCTSP_SchedulerMaster.Where(c => (c.ActiveStatus == "A" || c.ActiveStatus == "R") && c.SchId == schlId).OrderBy(c => c.PeriodNumber).Distinct().ToList();
            ViewBag.Language = fu.getstartandEndLangList("Create_Employee", 18, 29, Days.FirstOrDefault().CCTSP_SchoolMaster.Lang_id.Value);
            return View(Days);
        }
        public ActionResult Marketing()
        {
            var Marketingdata = SPA.Marketing_Master.Where(c => c.schId == schlId && c.ActiveStatus == "A").ToList();
            ViewBag.Marketingdata = Marketingdata;
            int LangId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
            ViewBag.language = fu.getLanguageData("Marketing_page", 0, LangId);
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public JObject MarketingAdd(Marketing_Master MM)
        {
            Marketing_Master CheckData = new Marketing_Master();
            Models.MarketingMaster EditData = new MarketingMaster();
            CheckData = SPA.Marketing_Master.Where(c => c.MID == MM.MID && c.ActiveStatus == "A" && c.schId == schlId).FirstOrDefault();
            var Status = "I";
            if (CheckData != null)
            {
                CheckData.content = MM.content.Replace("\r\n", "<br>");
                CheckData.Subject = MM.Subject;
                UpdateModel(CheckData);
                SPA.SaveChanges();
                Status = "U";
                EditData.MID = CheckData.MID;
                EditData.Subject = CheckData.Subject;
                EditData.content = CheckData.content;
                EditData.status = CheckData.status;
                EditData.Chkstatus = Status;
            }
            else
            {
                MM.ActiveStatus = "A";
                MM.CreatedOn = DateTime.Now;
                MM.content = MM.content.Replace("\r\n", "<br>");
                MM.schId = schlId;
                SPA.Marketing_Master.Add(MM);
                SPA.SaveChanges();
                EditData.MID = MM.MID;
                EditData.Subject = MM.Subject;
                EditData.content = MM.content;
                EditData.status = MM.status;
                EditData.Chkstatus = Status;
            }

            return JObject.Parse(js.Serialize(EditData));
        }
        public ActionResult MCustomerDetails(string Status, string TxtStatus)
        {
            List<Models.CustomerTabInfo> CustomerList = new List<Models.CustomerTabInfo>();
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            CustomerList = fu.getCustomerList("", 1, UserId, "Marketing");
            if (Status == "All")
                CustomerList = CustomerList.ToList();
            if (Status == "Men")
                CustomerList = CustomerList.Where(c => c.Gender == "Mr" || c.Gender == "Herr" || c.Gender == "Monsieur").ToList();
            if (Status == "Women")
                CustomerList = CustomerList.Where(c => c.Gender != "Mr" && c.Gender != "Herr" && c.Gender != "Monsieur").ToList();
            if (TxtStatus == "SMS")
                CustomerList = CustomerList.Where(c => !string.IsNullOrEmpty(c.Phoneno)).ToList();
            else
                CustomerList = CustomerList.Where(c => !string.IsNullOrEmpty(c.Emailid)).ToList();
            ViewBag.CustomerList = CustomerList;
            return View();
        }
        public JObject EditMarketingData(long MID)
        {
            JObject send = null;
            string jsondata = "";
            var EditData = SPA.Marketing_Master.Where(c => c.schId == schlId && c.ActiveStatus == "A" && c.MID == MID).FirstOrDefault();
            //var LinkList = js.Deserialize<Models.MarketingAttachment>(EditData.Extra_1);
            Models.MarketingMaster MData = new Models.MarketingMaster();
            MData.content = EditData.content.Replace("<br>", "\r\n");
            MData.Subject = EditData.Subject;
            MData.status = EditData.status;
            MData.MID = EditData.MID;
            //MData.Extra_1 = LinkList.Extra_1;          
            jsondata = js.Serialize(MData);
            send = JObject.Parse(jsondata);
            return send;
        }
        public void DeleteMarketing(long id)
        {
            var DQuery = "update marketing_master set ActiveStatus='D' where MID=" + id;
            SPA.Database.ExecuteSqlCommand(DQuery);
        }
        public void SendDetails(string UserList, long ContentId)
        {
            try
            {
                var Content = SPA.Marketing_Master.Where(c => c.schId == schlId && c.ActiveStatus == "A" && c.MID == ContentId).FirstOrDefault();
                Marketing_SendHistory MHistory = new Marketing_SendHistory();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                MHistory.MID = ContentId;
                MHistory.schId = schlId;
                MHistory.GroupUserId = UserList;
                MHistory.ActiveStatus = "A";
                MHistory.Extra_1 = Content.content;
                MHistory.SendDate = fu.ZonalDate(ShopInfo.TimeZone);
                SPA.Marketing_SendHistory.Add(MHistory);
                SPA.SaveChanges();
                var List = (from q in SPA.CCTSP_User where q.ActiveStatus == "A" && q.RoleId == 4 && q.SchId == schlId && UserList.Contains(q.UserId.ToString()) select q).ToList();
                if (Content.status == "EMAIL")
                {
                    var Link = "http://" + System.Web.HttpContext.Current.Request.Url.Host + "/";
                    var LinkList = new Models.MarketingAttachment();
                    List<string> AttchmentLinks = new List<string>();
                    List<string> Filenames = new List<string>();
                    if (!string.IsNullOrEmpty(Content.Extra_1))
                    {
                        LinkList = js.Deserialize<Models.MarketingAttachment>(Content.Extra_1);
                        foreach (var item in LinkList.Extra_1)
                        {
                            var MLink = Link + item;
                            Filenames.Add(item.Replace("/MarketingUpload/" + schlId + "/", ""));
                            AttchmentLinks.Add(MLink);
                        }
                    }
                    var dynamicContent = Regex.Replace(Content.content, "<img src=\"", "<img src=\"" + Link, RegexOptions.IgnoreCase);
                    Email.EmailWithNewsletter(List.Select(c => c.EmailId).ToList(), schlId, dynamicContent, AttchmentLinks, Filenames, Content.Subject);
                }
                else
                {
                    var SmsCountry = "EuropeSms";
                    if (ShopInfo.Schcountry == "INDIA")
                        SmsCountry = "IndiaSms";
                    var SmsUrl = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CCTSP_CategoryDetails.CatgId == 145 && c.CCTSP_CategoryDetails.CatgDesc == SmsCountry).Select(c => c.SectionDesc).FirstOrDefault();
                    Sms.SendMarketingSms(Content.content.Replace("<br>", "\r\n"), List.Where(c => c.PhoneNo != null).Select(c => c.PhoneNo).ToList(), SmsUrl, ShopInfo.Schcountry);
                }

            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
            }

        }
        public string MarketingFileUpload(HttpPostedFileBase file, string filename)
        {
            try
            {
                string PathDetails = "";
                if (file.FileName != "")
                {

                    var postedFile = file;
                    //string subPath = "MarketingUpload/";
                    string subPath = "MarketingUpload/" + schlId + "/";
                    bool IsExists = Directory.Exists(Path.Combine(HttpRuntime.AppDomainAppPath, subPath, file.FileName));
                    //bool IsExists = Directory.Exists(Server.MapPath(subPath));
                    // var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1).ToLower();
                    string savedFileName = Path.Combine(HttpRuntime.AppDomainAppPath, subPath) + file.FileName + "";
                    FileInfo temp = new FileInfo(savedFileName);
                    if (temp.Exists) temp.Delete();
                    IsExists = Directory.Exists(Path.Combine(HttpRuntime.AppDomainAppPath, subPath, file.FileName));
                    if (postedFile.ContentLength > 5145728)
                    {
                        // ViewBag.message = "File Size Not Valid";
                        PathDetails = "File Size Not Valid";
                    }
                    //if (!supportedTypes.Contains(fileExt))
                    //{
                    //    PathDetails = "Only image";
                    //}
                    if (PathDetails == "")
                    {
                        if (!IsExists)
                        {
                            System.IO.Directory.CreateDirectory(Path.Combine(HttpRuntime.AppDomainAppPath, subPath));
                            postedFile.SaveAs(Path.Combine(HttpRuntime.AppDomainAppPath, subPath) + file.FileName + "");
                            PathDetails = Path.Combine("/" + subPath + file.FileName);
                        }
                        else
                        {
                            postedFile.SaveAs(Path.Combine(HttpRuntime.AppDomainAppPath, subPath, file.FileName));
                            PathDetails = Path.Combine("/" + subPath + file.FileName);
                        }
                    }
                }
                return PathDetails;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }
        }
        //public void ScheduleAdd(FormCollection frmSchedule)
        //{
        //    var NoTInclude = new List<string>();
        //    var ShopSchedule = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && (c.ActiveStatus == "A" || c.ActiveStatus == "R")).ToList();
        //    var WeekSchedule = ShopSchedule.Select(c => c.WeekDay).Distinct().ToList();
        //    foreach (var Week in WeekSchedule)
        //    {
        //        var ListSchedule = ShopSchedule.Where(c => c.WeekDay == Week).OrderBy(c => c.PeriodNumber).Take(2);
        //        var Tempcount = 0;
        //        if (!string.IsNullOrEmpty(frmSchedule["1" + Week]))
        //        {
        //            foreach (var Shop in ListSchedule)
        //            {
        //                if (Tempcount == 0)
        //                {
        //                    Shop.StartTime = DateTime.Parse(frmSchedule["1" + Shop.WeekDay]).TimeOfDay;
        //                    Shop.EndTime = DateTime.Parse(frmSchedule["2" + Shop.WeekDay]).TimeOfDay;
        //                    Shop.ActiveStatus = "A";
        //                    UpdateModel(Shop);
        //                }
        //                if (Tempcount == 1)
        //                {
        //                    Shop.StartTime = DateTime.Parse(frmSchedule["3" + Shop.WeekDay]).TimeOfDay;
        //                    Shop.EndTime = DateTime.Parse(frmSchedule["4" + Shop.WeekDay]).TimeOfDay;
        //                    Shop.ActiveStatus = "A";
        //                    UpdateModel(Shop);
        //                }
        //                Tempcount = Tempcount + 1;
        //            }
        //        }
        //        else
        //        {
        //            NoTInclude.Add(Week);
        //        }
        //    }
        //    foreach (var No in ShopSchedule.Where(c => NoTInclude.Contains(c.WeekDay)))
        //    {
        //        No.ActiveStatus = "R";
        //        UpdateModel(No);
        //    }
        //    SPA.SaveChanges();
        //}
        public ActionResult LendingPage()
        {
            var lendingDetails = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schlId).Select(c => new Models.LandingDetails
            {
                Logo = c.Logo,
                Logotext = c.Logotext,
                ShopName = c.ShopName,
                Logotext_Color = c.Logotext_Color,
                Shopimg1 = c.Shopimg1,
                Shopimg2 = c.Shopimg2,
                Shopimg3 = c.Shopimg3,
                Shopimg4 = c.Shopimg4,
                Schid = c.Schid,
                Field1 = c.Field1,
                Field2 = c.Field2,
                Field3 = c.Field3,
                Field4 = c.Field4,
                Field5 = c.Field5,
                Address = c.Address,
                Email_id = c.Email_id,
                PhoneNo = c.PhoneNo,
                ZipCode = c.ZipCode,
                ShopCity = c.ShopCity,
                Color_Id = c.CCTSP_SchoolMaster.Color_Id
            }).FirstOrDefault();
            ViewBag.lendingDetails = lendingDetails;
            int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
            string WeekDay = " select a.CatgTypeId,b.Value as Day,c.CatgDesc,c.Gender from cctsp_CategoryDetails a " +
                            " join Language_Label_Detail b on a.Catgdesc = b.English_Label " +
                            " left join cctsp_CategoryDetails c on c.value = a.CatgTypeId and c.DomainType = " + schlId + " and c.ActiveStatus = 'A' and c.CatgId = 148 " +
                            " Where a.CatgId = 30 and a.ActiveStatus = 'A' and a.DomainType = 236 " +
                            " and b.Lang_Id = " + LanguageId + " and b.ActiveStatus = 'A' and b.Page_Name = 'Create_Employee'";
            ViewBag.WeekDetails = SPA.Database.SqlQuery<Models.LendingWeekScheduleDetails>(WeekDay).ToList();
            var colordata = SPA.CCTSP_ColorMaster.ToList();
            ViewBag.colordata = colordata;
            ViewBag.LendingList = SPA.Database.SqlQuery<LendingWeekScheduleDetails>("select a.Value,a.CatgDesc, a.Gender, c.value  as [Day]  from cctsp_categoryDetails a join cctsp_categoryDetails b on a.value = b.CatgTypeId join  Language_Label_Detail c on c.English_label = b.catgDesc where a.catgId = 148 and a.DomainType = " + schlId + " and a.ActiveStatus = 'A' and c.Lang_id = " + LanguageId + " and c.ActiveStatus = 'A' and c.Page_name = 'Create_Employee'").ToList();
            ViewBag.SerialzeLendingList = js.Serialize(ViewBag.WeekDetails);
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.ActiveStatus == "A" && c.Lang_id == LanguageId).Select(c => new Models.LanguageNewShop
            {
                Lang_id = c.Lang_id,
                English_Label = c.English_Label,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id,
                Value = c.Value
            }).ToList();
            return View();
        }
        public ActionResult AddLendingPageDetails(FormCollection Lending)
        {
            try
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                long LenSchid = 0;
                CCTSP_LendingPageMaster LendingData = new CCTSP_LendingPageMaster();
                if (!string.IsNullOrEmpty(Lending["LendSchid"]))
                    LenSchid = Convert.ToInt64(Lending["LendSchid"]);
                if (LenSchid > 0)
                    LendingData = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == LenSchid).FirstOrDefault();
                if (!string.IsNullOrEmpty(Lending["Hidden_Logoimg"]))
                {
                    if (!string.IsNullOrEmpty(Request.Files["Logoimg_Name"].FileName))
                    {
                        HttpPostedFileBase Logo = Request.Files["Logoimg_Name"];

                        LendingData.Logo = fu.CommonImageUpload(Logo, supportedTypes);

                    }
                }
                else
                    LendingData.Logo = null;
                if (!string.IsNullOrEmpty(Lending["Hidden_FirstImg"]))
                {
                    if (!string.IsNullOrEmpty(Request.Files["FirstImg_Name"].FileName))
                    {
                        HttpPostedFileBase Shopimg1 = Request.Files["FirstImg_Name"];
                        LendingData.Shopimg1 = fu.CommonImageUpload(Shopimg1, supportedTypes);
                    }
                }
                else
                    LendingData.Shopimg1 = null;
                if (!string.IsNullOrEmpty(Lending["Hidden_SecondImg"]))
                {
                    if (!string.IsNullOrEmpty(Request.Files["SecondImg_Name"].FileName))
                    {
                        HttpPostedFileBase Shopimg2 = Request.Files["SecondImg_Name"];
                        LendingData.Shopimg2 = fu.CommonImageUpload(Shopimg2, supportedTypes);
                    }
                }
                else
                    LendingData.Shopimg2 = null;
                if (!string.IsNullOrEmpty(Lending["Hidden_ThirdImg"]))
                {
                    if (!string.IsNullOrEmpty(Request.Files["ThirdImg_Name"].FileName))
                    {
                        HttpPostedFileBase Shopimg3 = Request.Files["ThirdImg_Name"];
                        LendingData.Shopimg3 = fu.CommonImageUpload(Shopimg3, supportedTypes);
                    }
                }
                else
                    LendingData.Shopimg3 = null;
                if (!string.IsNullOrEmpty(Lending["Hidden_FourthImg"]))
                {
                    if (!string.IsNullOrEmpty(Request.Files["FourthImg_Name"].FileName))
                    {
                        HttpPostedFileBase Shopimg4 = Request.Files["FourthImg_Name"];
                        LendingData.Shopimg4 = fu.CommonImageUpload(Shopimg4, supportedTypes);
                    }
                }
                else
                    LendingData.Shopimg4 = null;
                LendingData.Logotext = Lending["Logotext"];
                LendingData.ShopName = Lending["ShopName"];
                LendingData.Address = Lending["LendingAddress"];
                LendingData.ZipCode = Lending["Zipcode"];
                LendingData.Email_id = Lending["LendingEmail"];
                LendingData.PhoneNo = Lending["PhoneNumber"];
                LendingData.ShopCity = Lending["ShopCity"];
                LendingData.Logotext_Color = Lending["LogoTextColor"];
                LendingData.CCTSP_SchoolMaster.Color_Id = Convert.ToInt16(Lending["ColorSelection"].ToString());
                // LendingData.ShopDesc = Lending["ShopDesc"];
                if (!string.IsNullOrEmpty(Lending["ChkRequired"]))
                {
                    LendingData.Field1 = Lending["FImgText"];
                    LendingData.Field2 = Lending["SImgText"];
                    LendingData.Field3 = Lending["TImgText"];
                    LendingData.Field4 = Lending["FHImgText"];
                    LendingData.Field5 = "1";
                }
                else
                    LendingData.Field5 = "0";
                LendingData.Schid = Convert.ToInt64(schlId);
                if (LenSchid > 0)
                    UpdateModel(LendingData);
                else
                    SPA.CCTSP_LendingPageMaster.Add(LendingData);
                SPA.SaveChanges();
                var query = "update cctsp_categoryDetails set ActiveStatus='D' where CatgId=148 and Domaintype=" + schlId;
                SPA.Database.ExecuteSqlCommand(query);
                if (Lending["LendingObject"] != "")
                {
                    var WeekSch = js.Deserialize<List<Models.LendingWeekScheduleDetails>>(Lending["LendingObject"].ToString());
                    List<CCTSP_CategoryDetails> List = new List<CCTSP_CategoryDetails>();
                    CCTSP_CategoryDetails Detail = new CCTSP_CategoryDetails();
                    int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                    var Closed = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.ActiveStatus == "A" && c.Order_id == 41 && c.Lang_id == LanguageId).FirstOrDefault();
                    var StatusClosed = Closed.Value;
                    var EnLabelStatusClosed = Closed.English_Label;
                    foreach (var item in WeekSch)
                    {
                        Detail = new CCTSP_CategoryDetails();
                        Detail.CatgId = 148;
                        Detail.CatgType = "LendingDay";
                        Detail.ActiveStatus = "A";
                        Detail.DomainType = schlId;
                        Detail.CatgDesc = item.Catgdesc != null && item.Catgdesc != EnLabelStatusClosed ? item.Catgdesc : EnLabelStatusClosed;
                        Detail.Gender = item.Gender != null && item.Gender != EnLabelStatusClosed ? item.Gender : EnLabelStatusClosed;
                        Detail.Value = item.CatgTypeId;
                        List.Add(Detail);
                    }
                    SPA.CCTSP_CategoryDetails.AddRange(List);
                    SPA.SaveChanges();
                }
                //return RedirectToAction("Home", "Home");
                return Redirect("/Shop/shop#Lending");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public long AddLendingWeekSchedule(CCTSP_CategoryDetails LendingSchedule)
        {
            CCTSP_CategoryDetails AddLendingSchedule = new CCTSP_CategoryDetails();
            int countcheck = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 148 && c.ActiveStatus == "A" && c.Value == LendingSchedule.Value).Count();
            if (countcheck > 0)
            {
                return 0;
            }
            else
            {
                AddLendingSchedule.CatgId = 148;
                AddLendingSchedule.CatgDesc = LendingSchedule.CatgDesc;
                AddLendingSchedule.ActiveStatus = "A";
                AddLendingSchedule.CatgType = "Lending";
                AddLendingSchedule.Value = LendingSchedule.Value;
                AddLendingSchedule.DomainType = schlId;
                SPA.CCTSP_CategoryDetails.Add(AddLendingSchedule);
                SPA.SaveChanges();
                return AddLendingSchedule.CatgTypeId;
            }

        }
        public void DeleteLendingDay(long CatgTypeId)
        {
            var query = "update cctsp_categoryDetails set ActiveStatus='D' where CatgId=148 and Domaintype=" + schlId + " and CatgTypeId=" + CatgTypeId;
            SPA.Database.ExecuteSqlCommand(query);
        }
        [CustomAutohrize(null)]
        public ActionResult ChangePassword()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Owner_Chg_Pwd" && c.Lang_id == LanguageId).ToList();
                return View();
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
        public string ChangePasswordEdit(FormCollection edit)
        {
            try
            {
                string Result = "";
                long UserId = Convert.ToInt64(Session["UserId"].ToString());
                string Password = edit["CTPASSWORD"];
                CCTSP_User user = new CCTSP_User();
                user = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.Password == Password && c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                if (edit["CTPASSWORD"] == user.Password)
                {
                    user.Password = edit["NWPASSWORD"];
                    UpdateModel(user);
                    SPA.SaveChanges();
                    Result = "Success";
                    return Result;
                }
                else
                {
                    Result = "UnSuccessful";
                    return Result;
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "UnSuccessful";
            }
        }
        [CustomAutohrize(null)]
        public ActionResult Holiday()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var CalendarType = "select a.CatgDesc as CalendarName,c.AccessToData,c.insertAccess,c.DeleteAccess,c.UpdateAccess ,b.ItenName as FlowStatus " +
                                   "from CCTSP_CategoryDetails a " +
                                   " join ctsp_SolutionMaster b on b.SectionName = 'Holiday' " +
                                   " join cctsp_user d on d.UserId = " + UserId + "  " +
                                   " join cctsp_Role e on e.RoleId = d.RoleId and(e.Schlid = " + schlId + " or e.Schlid = 236) and e.ActiveStatus = 'A' " +
                                   " join cctsp_RoleDetails c on c.ActiveStatus = 'A' and e.RoleId = c.RoleId " +
                                   " and c.Schid = e.Schlid    and b.SolutionId = convert(bigint, c.AccesstoSub) " +
                                   " Where a.CatgId = 119  and a.ActiveStatus = 'A'";
                var HolidayList = SPA.Database.SqlQuery<Models.HolidayList>(CalendarType).ToList();
                // ViewBag.Dataholidaytype = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 119 && c.ActiveStatus == "A").Select(c => c.CatgDesc).ToList();
                ViewBag.Dataholidaytype = HolidayList;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Owner_Holiday" && c.Lang_id == LanguageId).ToList();
                string LngLocal = "";
                if (LanguageId == 1) { LngLocal = "en"; }
                if (LanguageId == 2) { LngLocal = "de"; }
                if (LanguageId == 3) { LngLocal = "fr-ca"; }
                ViewBag.LngLocal = LngLocal;
                return View();
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
        [CustomAutohrize(null)]
        public ActionResult Texts()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                // ViewBag.EventList = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgId == 120 || c.CatgId == 121 && c.DomainType == schlId).Select(c => c.CatgDesc).Distinct().ToList();
                var Text = "select a.ItenName,a.CatgTypeId,a.[Group],a.SolutionId,b.Catgdesc,b.CatgId,a.SectionDesc, " +
                           "c.AccessToData,c.insertAccess,c.DeleteAccess,c.UpdateAccess ,f.ItenName as FlowStatus " +
                           "from CTSP_SolutionMaster a " +
                           "join cctsp_CategoryDetails b on a.CatgtypeId = b.CatgTypeId " +
                           "join ctsp_SolutionMaster f on f.SectionName = 'Texts' " +
                           "join cctsp_user d on d.UserId = " + UserId + " " +
                           "join cctsp_Role e on e.RoleId = d.RoleId and(e.Schlid = " + schlId + " or e.Schlid = 236) and e.ActiveStatus = 'A' " +
                           "join cctsp_RoleDetails c on c.ActiveStatus = 'A' and e.RoleId = c.RoleId " +
                           "and c.Schid = e.Schlid    and f.SolutionId = convert(bigint, c.AccesstoSub) " +
                           "Where a.Schid = " + schlId + " and a.Activestatus = 'A' and(b.CatgId = 120 or b.CatgId = 121)";
                var TextList = SPA.Database.SqlQuery<Models.TextDisplay>(Text).ToList();
                ViewBag.EventList = TextList;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Owner_Text" && c.Lang_id == LanguageId).ToList();
                return View();
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
        public void AddText(FormCollection Text)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                string SelectedEvent = Text["SelectedEvent"];
                #region Variable
                long catgTypeId = 0;
                string sectionName = "";
                if (Text["EmailId"].ToString() == "SMSFIELD")
                {
                    catgTypeId = SPA.CCTSP_CategoryDetails.Where(c => c.CatgDesc == SelectedEvent && c.CatgId == 121 && c.ActiveStatus == "A").Select(c => c.CatgTypeId).FirstOrDefault();
                    sectionName = "SMS" + Text["SelectedEvent"];

                }
                else
                {
                    catgTypeId = SPA.CCTSP_CategoryDetails.Where(c => c.CatgDesc == SelectedEvent && c.CatgId == 120 && c.ActiveStatus == "A").Select(c => c.CatgTypeId).FirstOrDefault();
                    sectionName = "Email" + Text["SelectedEvent"];
                }
                CTSP_SolutionMaster TextInsert = new CTSP_SolutionMaster();
                #endregion
                #region Logic

                if (string.IsNullOrEmpty(Text["EditUserId"]))
                {
                    //This is the code to deactivate multiple Text
                    var query = "update CTSP_SolutionMaster set [Group]='D' where CatgTypeId=" + catgTypeId + "and SchId=" + schlId;
                    SPA.Database.ExecuteSqlCommand(query);
                    TextInsert.CatgTypeId = catgTypeId;
                    TextInsert.SectionName = sectionName;
                    TextInsert.SectionDesc = Text["MsgArea"];
                    TextInsert.CraetedOn = EuropeDate;
                    TextInsert.Activestatus = "A";
                    TextInsert.SchId = schlId;
                    SPA.CTSP_SolutionMaster.Add(TextInsert);
                }
                else
                {
                    long SolutionId = Convert.ToInt32(Text["EditUserId"]);
                    TextInsert = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == SolutionId && c.Activestatus == "A").FirstOrDefault();
                    if (TextInsert != null && TextInsert.Group != "D")
                    {
                        var query = "update CTSP_SolutionMaster set [Group]='D' where CatgTypeId=" + catgTypeId + "and SchId=" + schlId + " and SolutionId!=" + SolutionId;
                        SPA.Database.ExecuteSqlCommand(query);
                    }
                    TextInsert.CatgTypeId = catgTypeId;
                    TextInsert.SectionName = sectionName;
                    TextInsert.SectionDesc = Text["MsgArea"];
                    UpdateModel(TextInsert);
                }
                SPA.SaveChanges();

                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        [CustomAutohrize(null)]
        public ActionResult TextsDisplay(List<Language_Label_Detail> Language)
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //var data = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.SchId == schlId && (c.CCTSP_CategoryDetails.CatgId == 120 || c.CCTSP_CategoryDetails.CatgId == 121) && c.CCTSP_CategoryDetails.ActiveStatus == "A").ToList();
                var Text = "select a.ItenName,a.CatgTypeId,a.[Group],a.SolutionId,b.Catgdesc,b.CatgId,a.SectionDesc, " +
                           "c.AccessToData,c.insertAccess,c.DeleteAccess,c.UpdateAccess ,f.ItenName as FlowStatus " +
                           "from CTSP_SolutionMaster a " +
                           "join cctsp_CategoryDetails b on a.CatgtypeId = b.CatgTypeId " +
                           "join ctsp_SolutionMaster f on f.SectionName = 'Texts' " +
                           "join cctsp_user d on d.UserId = " + UserId + " " +
                           "join cctsp_Role e on e.RoleId = d.RoleId and(e.Schlid = " + schlId + " or e.Schlid = 236) and e.ActiveStatus = 'A' " +
                           "join cctsp_RoleDetails c on c.ActiveStatus = 'A' and e.RoleId = c.RoleId " +
                           "and c.Schid = e.Schlid    and f.SolutionId = convert(bigint, c.AccesstoSub) " +
                           "Where a.Schid = " + schlId + " and a.Activestatus = 'A' and(b.CatgId = 120 or b.CatgId = 121)";
                var data = SPA.Database.SqlQuery<Models.TextDisplay>(Text).ToList();
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                if (Language == null)
                    Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Owner_Text" && c.Lang_id == LanguageId).ToList();
                ViewBag.Language = Language;
                ViewBag.DataDisplay = data;
                ViewBag.DataDisplayCount = data.Count();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public JObject EditTextDisplay(long SolutionId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                string Media = "";
                var Data = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == SolutionId && c.Activestatus == "A" && c.SchId == schlId).FirstOrDefault();
                if (Data.CCTSP_CategoryDetails.CatgId == 121)
                    Media = "SMS";
                if (Data.CCTSP_CategoryDetails.CatgId == 120)
                    Media = "Email";
                Models.EditShopDetails TextDisplay = new Models.EditShopDetails()
                { SolutionId = SolutionId, CatgDesc = Data.CCTSP_CategoryDetails.CatgDesc, SectionDesc = Data.SectionDesc, Media = Media };
                jsondata = js.Serialize(TextDisplay);
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
        public ActionResult ShopAdd(FormCollection form)
        {
            try
            {
                string UploadPath = "";
                try
                {
                    DateTime EuropeDate = fu.ZonalDate(null);
                    long schoolId = Convert.ToInt64(form["CUSTOMERNO"]);
                    if (form["TempImage"] == "" || form["TempImage"] == null)
                    {
                        #region UploadImage
                        HttpPostedFileBase file = Request.Files["ImageLogo"];
                        if (file.FileName != "")
                        {
                            var postedFile = file;
                            string subPath = "~/Upload/" + schoolId + "/";
                            bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                            var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                            if (postedFile.ContentLength > 16780000)
                            {
                                ViewBag.message = "File Size Not Valid";
                            }
                            if (!supportedTypes.Contains(fileExt.ToLower()))
                            {
                                ModelState.AddModelError("photo", "Invalid type. Only the following types (jpg, jpeg, png) are supported.");
                            }
                            if (!IsExists)
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                                string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                                int numFiles = files1.Length + Convert.ToInt32(schoolId);
                                postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(numFiles + ".jpg"));
                                UploadPath = "/Upload/" + schoolId + "/" + numFiles + ".jpg";
                            }
                            else
                            {
                                string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                                int numFiles = files1.Length;
                                long Change = numFiles + schoolId;
                                //System.IO.File.Move(Server.MapPath(subPath) + "1.jpg", Server.MapPath(subPath) + "old-" + numFiles + ".jpg");
                                postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(Change + ".jpg"));
                                UploadPath = "/Upload/" + schoolId + "/" + Change + ".jpg";
                            }
                        }
                        else
                        {
                            UploadPath = "";
                        }
                        #endregion
                    }
                    else
                    {
                        UploadPath = form["TempImage"];
                    }
                    if (form["TempImage"] == "Delete")
                    {
                        UploadPath = "";
                    }
                    #region UpdateShopInfo
                    CCTSP_SchoolMaster School = new CCTSP_SchoolMaster();
                    School = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schoolId).FirstOrDefault();
                    School.SchlName = form["SHOPNAME"];
                    School.SchlMobile1 = form["MOBILENUMBER"];
                    School.SchlCity = form["CITY"];
                    School.SchPin = form["ZIPCODE"];
                    School.SchlAddress = form["ADDRESS"];
                    //School.SHOPTYPE = form["SHOPTYPE"];
                    School.CUSTOMERNO = schoolId;
                    School.BANKACCOUNT = form["BANKACCOUNT"];
                    School.IBANNO = form["IBANNO"];
                    /*Removed in View side*/
                    //School.SchlWebsite = form["Website"];
                    School.SchlEmail = form["Email"];
                    School.BANKNAME = form["BANKNAME"];
                    School.StreetNumber = form["StreetNumber"];
                    School.street = form["Street"];
                    School.VatNumber = form["VATNUMBER"];
                    School.Vat = form["VAT"];
                    School.Invoice_FreeText = form["Invoice_FreeText"];
                    School.Invoice_Email_Txt = form["InvoiceEmailTxt"];
                    if (!string.IsNullOrEmpty(form["latitude"]))
                        School.latitude = Convert.ToDouble(form["latitude"]);
                    else
                        School.latitude = null;
                    if (!string.IsNullOrEmpty(form["longitude"]))
                        School.longitude = Convert.ToDouble(form["longitude"]);
                    else
                        School.longitude = null;
                    if (!string.IsNullOrEmpty(form["PaymentDuration"]))
                        School.OverDue = Convert.ToInt32(form["PaymentDuration"]);
                    else
                        School.OverDue = null;
                    if (!string.IsNullOrEmpty(form["ExtendDuration"]))
                        School.Extend_Pay_Duration = Convert.ToInt32(form["ExtendDuration"]);
                    else
                        School.Extend_Pay_Duration = null;
                    School.location = form["Location"];
                    SPA.SaveChanges();
                    #endregion
                    #region UpdateLandinInfo
                    CCTSP_LendingPageMaster LandingInfo = new CCTSP_LendingPageMaster();
                    LandingInfo = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schoolId).FirstOrDefault();
                    LandingInfo.ShopDesc = form["ShopDesc"].Replace("\r\n", "<br>");
                    #endregion
                    #region UpdateOwnerInfo
                    CCTSP_User User = new CCTSP_User();
                    User = SPA.CCTSP_User.Where(c => c.SchId == schoolId && c.ActiveStatus == "A" && c.CCTSP_SchoolMaster.SchlId == schoolId && c.RoleId == 1).FirstOrDefault();
                    User.PhoneNo = form["MOBILENUMBER"];
                    User.Gender = form["TITLE"];
                    User.Salutation = form["SALUTATION"];
                    User.Title = form["GENDER"];
                    User.LastName = form["OWNERSURNAME"];
                    User.FirstName = form["OWNERNAME"];
                    User.EmailId = form["ShopOwnerEmail"];
                    User.State = form["State"];
                    User.City = form["CITY"];
                    User.GLN_No = form["GLN_NO"];
                    User.ZSR_No = form["ZSR_NO"];
                    User.PasswordQuerry2 = form["LANDLINENUMBER"];
                    User.CCTSP_SchoolMaster.ImageLogo = UploadPath;
                    SPA.SaveChanges();
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetExpires(DateTime.Now);
                    Response.Cache.SetNoServerCaching();
                    Response.Cache.SetNoStore();
                    Response.Cookies.Clear();
                    Request.Cookies.Clear();

                    #endregion
                }
                catch (Exception e)
                {
                    fu.ErrorSend(RouteData, e);
                    return RedirectToAction("Error_List", "Error");
                }

                return RedirectToAction("Shop", "Shop");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public string UploadImage(FormCollection form)
        {

            var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "ico" };
            string UploadedImagePath = "";
            var file = Request.Files["Uploadmedia"];
            var postedFile = file;
            if (file.FileName != "")
            {
                //string subPath = "/images/Marketing/";
                string subPath = "/MarketingUpload/" + schlId + "/";
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                var fileExt = Path.GetExtension(file.FileName).Substring(1).ToLower();
                var fileWithoutExt = Path.GetFileNameWithoutExtension(file.FileName).ToLower();
                if (postedFile.ContentLength > 16780000)
                {
                    return "File Size Not Valid";
                }
                if (!supportedTypes.Contains(fileExt))
                {
                    return "It Accept only Image format";
                }
                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                }
                string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                int numFiles = files1.Length;
                postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(numFiles + ".jpg"));
                return UploadedImagePath = subPath + numFiles + ".jpg";
            }

            return "0";
        }
        public JObject AddHolidayShop(FormCollection holiday)
        {

            try
            {
                JObject send = null;
                string jsondata = "";
                DateTime EuropeDate = fu.ZonalDate(null);
                long LeaveId = 0;
                SPA_LeaveMaster leaveShop = new SPA_LeaveMaster();
                if (holiday["EditLeaveId"] != "")
                {
                    LeaveId = Convert.ToInt64(holiday["EditLeaveId"]);
                    leaveShop = SPA.SPA_LeaveMaster.Where(c => c.LeaveId == LeaveId).FirstOrDefault();
                }
                #region AddShop
                leaveShop.HolidayDesc = holiday["HolidayName"];
                leaveShop.CalendarName = holiday["HolidayType"];
                // string dt1 = vacation["StartDate"].ToString();
                // DateTime start = DateTime.ParseExact(dt1, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);
                string startDate = holiday["StartDate"].ToString();
                leaveShop.StartDate = DateTime.Parse(startDate, enGB);
                string endDate = holiday["EndDate"].ToString();
                leaveShop.EndDate = DateTime.Parse(endDate, enGB);
                var ChkShopLeaves = "select top 1* from SPA_EmployeeScheduler b join CCTSP_CategoryDetails d on d.CatgTypeId=b.Product_Id  where ((b.ActiveStatus = 'DA' and b.BookedStatus = 'B') or (b.ActiveStatus = 'A' and b.BookedStatus = 'B') or (b.ActiveStatus = 'C' and b.BookedStatus = 'C')) and convert(date, b.BookingDate) BETWEEN convert(date, '" + DateTime.Parse(startDate.ToString(), enGB).ToString("yyyy/MM/dd") + "') AND convert(date, '" + DateTime.Parse(endDate.ToString(), enGB).ToString("yyyy/MM/dd") + "') and b.schId=" + schlId;
                var ShopLeavesList = SPA.Database.SqlQuery<SPA_EmployeeScheduler>(ChkShopLeaves).FirstOrDefault();
                Models.vacation VacationDetails = new Models.vacation();
                if (ShopLeavesList == null)
                {
                    if (holiday["EditLeaveId"] == "")
                    {
                        leaveShop.ActiveStatus = "A";
                        leaveShop.CreatedOn = EuropeDate;
                        leaveShop.UserId = (SPA.CCTSP_User.Where(c => c.RoleId == 1 && c.ActiveStatus == "A" && c.SchId == schlId)
                                           .Select(c => c.UserId).FirstOrDefault());
                        leaveShop.SchId = schlId;
                        leaveShop.HolidayTypeId = 11154;
                        SPA.SPA_LeaveMaster.Add(leaveShop);
                    }
                    SPA.SaveChanges();
                }
                else
                {
                    VacationDetails.StartDate = startDate;
                    VacationDetails.EndDate = endDate;
                    VacationDetails.Status = "ErrorLeaves";
                }
                #endregion
                ViewBag.MsgType = "holiday";
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
        public ActionResult HolidayDetail()
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //var DataHoliday = SPA.SPA_LeaveMaster.Where(c => c.HolidayTypeId == 11154 && c.SchId == schlId && c.ActiveStatus == "A").OrderBy(c => c.StartDate).ToList();
                var HolidayList = " select a.HolidayDesc,a.Schid,a.UserId,a.LeaveId,a.StartDate,a.EndDate,a.CalendarName,a.CreatedOn, " +
                                  " c.AccessToData,c.insertAccess,c.DeleteAccess,c.UpdateAccess,b.ItenName as FlowStatus " +
                                  " from SPA_LeaveMaster a " +
                                  " join ctsp_SolutionMaster b on b.SectionName = 'Holiday' " +
                                  " join cctsp_user d on d.UserId = " + UserId + " " +
                                  " join cctsp_Role e on e.RoleId = d.RoleId and(e.Schlid = " + schlId + " or e.Schlid = 236) and e.ActiveStatus = 'A' " +
                                  " join cctsp_RoleDetails c on c.ActiveStatus = 'A' and e.RoleId = c.RoleId " +
                                  " and c.Schid = e.Schlid    and b.SolutionId = convert(bigint, c.AccesstoSub) " +
                                  " Where a.HolidayTypeId = 11154 and a.SchId = " + schlId + " and a.ActiveStatus = 'A' order by a.StartDate ";
                var DataHoliday = SPA.Database.SqlQuery<Models.HolidayList>(HolidayList).ToList();
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Shop_Owner_Holiday" && c.Lang_id == LanguageId).Select(c => new LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Order_id = c.Order_id,
                    Page_Name = c.Page_Name,
                    Lang_id = c.Lang_id
                }).ToList();
                ViewBag.DataHoliday = DataHoliday;
                //if (DataHoliday.Count == 0)
                //    ViewBag.CountHoliday = 0;
                //else
                //    ViewBag.CountHoliday = DataHoliday.Count;

                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public JObject HolidayShopEdit(long? LeaveId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                var Data = SPA.SPA_LeaveMaster.Where(c => c.LeaveId == LeaveId && c.ActiveStatus == "A" && c.SchId == schlId && c.HolidayTypeId == 11154).FirstOrDefault();
                string stdate = DateTime.Parse(Data.StartDate.ToString()).Date.ToString("dd/MM/yyyy");
                if (Data.StartTime != null)
                    stdate = DateTime.Parse(Data.StartDate.Value.Date.ToString("dd/MM/yyyy") + " " + Data.StartTime).ToString("dd/MM/yyyy");
                string enddt = DateTime.Parse(Data.EndDate.ToString()).Date.ToString("dd/MM/yyyy");
                if (Data.EndTime != null)
                    enddt = DateTime.Parse(Data.EndDate.Value.Date.ToString("dd/MM/yyyy") + " " + Data.EndTime.ToString()).ToString("dd/MM/yyyy");
                Models.EditShopDetails EditHoliday = new Models.EditShopDetails()
                { HolidayName = Data.HolidayDesc, HolidayType = Data.CalendarName, StartDate = stdate, EndDate = enddt, EditLeaveId = Data.LeaveId };
                jsondata = js.Serialize(EditHoliday);
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
        public void DeleteShop(long? LeaveId)
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

        public void DeleteText(long? SolutionId)
        {
            try
            {
                CTSP_SolutionMaster sol = new CTSP_SolutionMaster();
                sol = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == SolutionId).FirstOrDefault();
                sol.Activestatus = "D";
                UpdateModel(sol);
                SPA.SaveChanges();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public string DeactivateText(long? SolutionId, string status)
        {

            string TextSend = "";
            try
            {
                CTSP_SolutionMaster sol = new CTSP_SolutionMaster();
                if (status == null || status == "")
                {
                    sol = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == SolutionId).FirstOrDefault();
                    sol.Group = status;
                    var catgtypeid = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == SolutionId).Select(c => c.CatgTypeId).FirstOrDefault();
                    var data = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == catgtypeid && c.SchId == schlId && c.Activestatus == "A" && c.SolutionId != SolutionId).Select(c => c.SolutionId).ToList();
                    if (data.Count > 0)
                    {
                        TextSend = fu.updateTextRange(data);
                    }
                }
                else
                {
                    sol = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == SolutionId).FirstOrDefault();
                    sol.Group = status;
                }
                UpdateModel(sol);
                SPA.SaveChanges();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

            return TextSend.Replace(",", "~");
        }
        [HttpPost]
        public ActionResult ActiveText(string SoultionIdList)
        {
            if (SoultionIdList != null && SoultionIdList != "")
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                ViewBag.Access=fu.CheckAccessofPage("Texts", UserId);
                int LangId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                long Id = Convert.ToInt64(SoultionIdList);
                var data = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.SolutionId == Id && c.SchId == schlId && (c.CCTSP_CategoryDetails.CatgId == 120 || c.CCTSP_CategoryDetails.CatgId == 121) && c.CCTSP_CategoryDetails.ActiveStatus == "A").FirstOrDefault();
                ViewBag.Data = data;
                ViewBag.Language = fu.getLanguageData("Shop_Owner_Text", 0, LangId);
            }
            return View();
        }
        [CustomAutohrize(null)]
        public ActionResult Various()
        {
            try
            {

                var ReNotification = new Models.ShopInfoJson();
                List<int> alertNumber = new List<int>();
                for (int i = 0; i <= 23; i++) { alertNumber.Add(i + 1); }
                var Shopdata = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.variousShopDetails
                {
                    jsonModel = c.jsonModel,
                    Lang_id = c.Lang_id,
                    send_sms = c.send_sms,
                    AlertRemainder = c.AlertRemainder,
                    AdvBookingRestrict = c.AdvBookingRestrict,
                    Color_Id = c.Color_Id,
                    Cancel_Res_Duration = c.Cancel_Res_Duration,
                    Invoice_FreeText = c.Invoice_FreeText,
                    BookingApproval = c.BookingApproval,
                    ShowPrice = c.ShowPrice,
                    Display_Invoice = c.Display_Invoice
                }).FirstOrDefault();
                if (!string.IsNullOrEmpty(Shopdata.jsonModel))
                    ReNotification = js.Deserialize<Models.ShopInfoJson>(Shopdata.jsonModel);
                ViewBag.ReNotification = ReNotification;
                ViewBag.ShopData = Shopdata;
                ViewBag.Alert = alertNumber;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Various" && c.Lang_id == Shopdata.Lang_id).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }

        public void VariousAdd(FormCollection various)
        {
            try
            {
                var NReNotification = new Models.ShopInfoJson();
                var Enterprise = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                if (various["Language"] == "LangGerman")
                    Enterprise.Lang_id = 2;
                if (various["Language"] == "LangEnglish")
                    Enterprise.Lang_id = 1;
                if (various["Language"] == "LangFrench")
                    Enterprise.Lang_id = 3;
                Enterprise.BookingApproval = various["BookingApproval"].ToString();
                Enterprise.AlertRemainder = Convert.ToInt16(various["reminderInput"].ToString());
                Enterprise.Cancel_Res_Duration = Convert.ToInt16((various["CancelReservationInput"]));
                Enterprise.AdvBookingRestrict = Convert.ToInt16(various["AdvanceBooking"].ToString());
                Enterprise.Invoice_FreeText = various["Invoice_FreeText"];
                //Enterprise.Color_Id = Convert.ToInt16(various["ColorSelection"].ToString());
                var Shopdata = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new ShopMasterDetail { JsonModel = c.jsonModel, Display_Invoice = c.Display_Invoice }).FirstOrDefault();
                if (!string.IsNullOrEmpty(Shopdata.JsonModel))
                    NReNotification = js.Deserialize<Models.ShopInfoJson>(Shopdata.JsonModel);
                Models.ShopInfoJson ResevationNotification = new Models.ShopInfoJson()
                {
                    ReNotification = various["NotificationOfReservation"].ToString(),
                    DemoShop = NReNotification.DemoShop
                };
                Enterprise.jsonModel = js.Serialize(ResevationNotification);
                if (various["CusomerPrice"].ToString() == "YES")
                    Enterprise.ShowPrice = 1;
                else
                    Enterprise.ShowPrice = null;
                UpdateModel(Enterprise);
                SPA.SaveChanges();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void BackgroundColorChange(long ColorId)
        {
            try
            {
                CCTSP_SchoolMaster Colordata = new CCTSP_SchoolMaster();
                Colordata = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                Colordata.Color_Id = ColorId;
                UpdateModel(Colordata);
                SPA.SaveChanges();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }

        public ActionResult RestrictionAddLeaves(string Status, string Startdate, string EndDate, long? Empuserid)
        {
            try
            {
                var SchlLanguage = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                var LangaugeContent = SPA.Language_Label_Detail.Where(c => c.Order_id == 53 && c.Page_Name == "AlertMsg" && c.Lang_id == SchlLanguage.Lang_id).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Reservation_update" && c.Lang_id == SchlLanguage.Lang_id).ToList();
                string ChkShopLeaves = "";
                if (Status == "ShopLeave")
                    ChkShopLeaves = "select b.BookingDate,b.FromSlotMasterId,c.firstname as ClientName,c.Lastname as ClientLastName,c.PhoneNo as ClientPhoneNo , d.catgdesc as ProductName from SPA_EmployeeScheduler b join CCTSP_User c on c.Userid=b.CLIENT_Userid join CCTSP_CategoryDetails d on d.CatgTypeId=b.Product_Id where((b.ActiveStatus = 'DA' and b.BookedStatus = 'B')or(b.ActiveStatus = 'A' and b.BookedStatus = 'B') or (b.ActiveStatus = 'C' and b.BookedStatus = 'C')) and convert(date, b.BookingDate) BETWEEN convert(date, '" + DateTime.Parse(Startdate.ToString(), enGB).ToString("yyyy/MM/dd") + "') AND convert(date, '" + DateTime.Parse(EndDate.ToString(), enGB).ToString("yyyy/MM/dd") + "') and b.schId = " + schlId + " and d.DomainType=" + schlId + " and c.ActiveStatus = 'A' and d.ActiveStatus='A'";
                if (Status == "EmployeeLeave")
                    ChkShopLeaves = "select b.BookingDate,b.FromSlotMasterId,c.firstname as ClientName,c.Lastname as ClientLastName,c.PhoneNo as ClientPhoneNo , d.catgdesc as ProductName from SPA_EmployeeScheduler b join CCTSP_User c on c.Userid=b.CLIENT_Userid join CCTSP_CategoryDetails d on d.CatgTypeId=b.Product_Id where((b.ActiveStatus = 'DA' and b.BookedStatus = 'B')or(b.ActiveStatus = 'A' and b.BookedStatus = 'B') or (b.ActiveStatus = 'C' and b.BookedStatus = 'C')) and convert(date, b.BookingDate) BETWEEN convert(date, '" + DateTime.Parse(Startdate.ToString(), enGB).ToString("yyyy/MM/dd") + "') AND convert(date, '" + DateTime.Parse(EndDate.ToString(), enGB).ToString("yyyy/MM/dd") + "') and b.schId = " + schlId + " and d.DomainType=" + schlId + " and c.ActiveStatus = 'A' and d.ActiveStatus='A' and b.Emp_UserId =" + Empuserid;

                var ShopLeavesList = SPA.Database.SqlQuery<ConfirmModel>(ChkShopLeaves).ToList();
                ViewBag.ShopLeavesList = ShopLeavesList;
                ViewBag.LangaugeContent = LangaugeContent.Value;
                ViewBag.Startdate = Startdate;
                ViewBag.EndDate = EndDate;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult Language()
        {
            try
            {
                if (Session["RoleId"].ToString() == "1")
                {
                    var PageName = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A").ToList();
                    ViewBag.PageList = PageName;
                    return View();
                }
                else
                {
                    return RedirectToAction("SignIn", "Login");
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Language_LabelDetail(string PageName)
        {
            try
            {
                string Page = PageName.Replace("li_", "");
                var LanguageDetail = SPA.Language_Label_Detail.Where(c => c.Page_Name == Page && c.ActiveStatus == "A").OrderBy(c => c.Lang_Detail_Id).ToList();
                ViewBag.LanguageDetailList = LanguageDetail;
                return View();

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult ShopImageLanguage(string PageName)
        {
            try
            {
                string Page = "/LanguageUpload/" + PageName + ".png";
                ViewBag.DataImage = Page;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public string CheckWhether()
        {
            try
            {
                string Result = "";
                if (Session["RoleId"].ToString() == "1")
                {
                    Result = "Yes";
                }
                else
                {
                    Result = "No";
                }
                return Result;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }
        }

        //public string EditDisplayLangauge(long? Id)
        //{
        //    try
        //    {
        //        Language_Label_Detail LangDetail = new Language_Label_Detail();
        //        LangDetail = SPA.Language_Label_Detail.Where(c => c.Lang_Detail_Id == Id).FirstOrDefault();
        //        return LangDetail.Lang_Detail_Id.ToString() + "~" + LangDetail.Value;
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return "";
        //    }
        //}
        public JObject EditDisplayLangauge(long? Id)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                Language_Label_Detail LangDetail = new Language_Label_Detail();
                LangDetail = SPA.Language_Label_Detail.Where(c => c.Lang_Detail_Id == Id).FirstOrDefault();
                Models.EditShopDetails EditLang = new Models.EditShopDetails()
                { LangDetailId = LangDetail.Lang_Detail_Id, LangValue = LangDetail.Value };
                jsondata = js.Serialize(EditLang);
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
        public string LanguageEdit(FormCollection form)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                Language_Label_Detail LabelDetail = new Language_Label_Detail();
                long Id = Convert.ToInt64(form["LanguageId"]);
                LabelDetail = SPA.Language_Label_Detail.Where(c => c.Lang_Detail_Id == Id).FirstOrDefault();
                LabelDetail.Value = form["LanguageText"];
                LabelDetail.newdata = "0";
                LabelDetail.UpdatedDate = EuropeDate;
                SPA.SaveChanges();
                return LabelDetail.Page_Name;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }
        }
        public ActionResult Payment()
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    TimeZone = c.TimeZone
                }).FirstOrDefault();
                DateTime CurrentDate = fu.ZonalDate(ShopInfo.TimeZone);
                var PaymentDetails = fu.getPaymentDetails(null, schlId);
                ViewBag.BillingHistory = fu.BookingHistory(null, null, false);
                ViewBag.RemainingStatus = fu.RemainingCredit(schlId, CurrentDate);
                ViewBag.RemaingCredit = SPA.spa_BillingMaster.Where(c => c.ActiveStatus == "A" && c.ShopId == schlId).Select(c => c.RemainingCredit).FirstOrDefault();
                ViewBag.Currency = fu.currency(null);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Payment" && c.Lang_id == ShopInfo.Lang_id && c.ActiveStatus == "A").Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Value = c.Value
                }).ToList();
                return View(PaymentDetails);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public JsonResult CheckPayment(Models.DebitPayment debit)
        {
            try
            {
                long UserShopId = schlId;
                var Result = fu.getPaymentDetails(debit.ListofCatgId, UserShopId);
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

        public ActionResult DebitPay(FormCollection frm)
        {
            try
            {
                var ShopInfo = SPA.CCTSP_User.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.RoleId == 1)
                             .Select(c => new Models.CustomerDisplay
                             {
                                 FirstName = c.FirstName,
                                 FamilyName = c.LastName,
                                 PhoneNumber = c.PhoneNo,
                                 Timezone = c.CCTSP_SchoolMaster.TimeZone,
                                 LangId = c.CCTSP_SchoolMaster.Lang_id.Value,
                                 Email = c.EmailId
                             }).FirstOrDefault();
                DateTime EuropeDate = fu.ZonalDate(ShopInfo.Timezone);
                long UserShopId = schlId;
                //Get All Id of one whose Input is not taken
                var AllCatgDetailId = frm.AllKeys.Where(c => c != "PayDetail").ToList();
                //Get All information for one whose UserInput is not taken
                var Result = fu.getPaymentDetails(string.Join(",", AllCatgDetailId), UserShopId);
                List<Models.DebitPayment> UserSpecific = new List<Models.DebitPayment>();
                //For One whose input is taken
                if (!string.IsNullOrEmpty(frm["PayDetail"]))
                    UserSpecific = js.Deserialize<List<Models.DebitPayment>>(Convert.ToString(frm["PayDetail"]));
                var TotAmount = Result.Where(c => (c.CatPayment == "2" || c.CatPayment == "3") && c.PaymentType != null).Select(c => c.Amount).Sum() + UserSpecific.Where(c => (c.CatPayment == "2" || c.CatPayment == "3")).Select(c => c.Amount).Sum();
                var TotalCredit = Result.Where(c => (c.CatPayment == "1" || c.CatPayment == "2") && c.PaymentType != null).Select(c => c.Amount).Sum() + UserSpecific.Where(c => (c.CatPayment == "1" || c.CatPayment == "2")).Select(c => c.credit).Sum();
                //Billing Master information
                #region BillMaster
                spa_BillingMaster BillMaster = new spa_BillingMaster();
                BillMaster.ShopId = UserShopId;
                BillMaster.TotalAmount = TotAmount;
                BillMaster.ActiveStatus = "A";
                BillMaster.country_id = Result.Select(c => c.country_id).FirstOrDefault();
                BillMaster.TotalCredit = TotalCredit;
                BillMaster.created_on = EuropeDate;
                BillMaster.Updated_on = EuropeDate;
                if (Result.Select(c => c.Duration).Sum() > 0)
                {
                    BillMaster.DurationStatus = "Y";
                    BillMaster.Duration = Convert.ToString(Result.Select(c => c.Duration).Sum());
                    BillMaster.StartDate = EuropeDate;
                }
                else
                    BillMaster.DurationStatus = "N";
                BillMaster.RemainingCredit = TotalCredit;
                var oldBilling = SPA.spa_BillingMaster.Where(c => c.ActiveStatus == "A" && c.ShopId == UserShopId).FirstOrDefault();
                if (oldBilling != null)
                {
                    BillMaster.RemainingCredit = BillMaster.RemainingCredit + oldBilling.RemainingCredit;
                    oldBilling.ActiveStatus = "T";
                    if (oldBilling.DurationStatus == "Y" && oldBilling.DurationStatus != null)
                    {
                        BillMaster.DurationStatus = "Y";
                        BillMaster.Duration = Convert.ToString(Convert.ToInt16(BillMaster.Duration) + Convert.ToInt16(oldBilling.Duration));
                        BillMaster.StartDate = oldBilling.StartDate;
                    }
                    UpdateModel(oldBilling);
                }
                SPA.spa_BillingMaster.Add(BillMaster);
                SPA.SaveChanges();
                #endregion
                //Categories of Payment
                #region BillingDetails
                List<spa_BillingDetails> BillDetails = new List<spa_BillingDetails>();
                foreach (var ReBillDetail in Result)
                {
                    string PayDetailId = Convert.ToString(ReBillDetail.PaymentDetail_Id);
                    spa_BillingDetails bDetails = new spa_BillingDetails();
                    bDetails.ReceiptId = BillMaster.Receipt_Id;
                    bDetails.created_on = EuropeDate;
                    bDetails.Updated_on = EuropeDate;
                    bDetails.ShopId = UserShopId;
                    bDetails.ActiveStatus = "A";
                    bDetails.RechargeId = ReBillDetail.PaymentCatgId;
                    if ((ReBillDetail.CatPayment == "2" || ReBillDetail.CatPayment == "3") && ReBillDetail.PaymentType != null)
                        bDetails.Amount = ReBillDetail.Amount;
                    if ((ReBillDetail.CatPayment == "1" || ReBillDetail.CatPayment == "2") && ReBillDetail.PaymentType != null)
                        bDetails.Credit = ReBillDetail.Amount;
                    if ((ReBillDetail.CatPayment == "2" || ReBillDetail.CatPayment == "3") && ReBillDetail.PaymentType == null)
                        bDetails.Amount = UserSpecific.Where(c => c.CatgId == PayDetailId).Select(c => c.Amount).FirstOrDefault();
                    if ((ReBillDetail.CatPayment == "1" || ReBillDetail.CatPayment == "2") && ReBillDetail.PaymentType == null)
                        bDetails.Credit = UserSpecific.Where(c => c.CatgId == PayDetailId).Select(c => c.credit).FirstOrDefault();
                    bDetails.receivedAmount = bDetails.Amount;
                    if (ReBillDetail.Duration != 0)
                        bDetails.Duration = Convert.ToString(ReBillDetail.Duration);
                    BillDetails.Add(bDetails);
                }
                SPA.spa_BillingDetails.AddRange(BillDetails);
                SPA.SaveChanges();
                #endregion
                /*Payment Section*/
                Uri uri = HttpContext.Request.Url;
                var GetHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                DateTime CurrentDate = fu.ZonalDate(null);
                List<string> UDEFINED = new List<string>();
                spa_Payment_Gateway PayGateway = new spa_Payment_Gateway()
                {
                    ActiveStatus = "A",
                    amount_ = TotAmount.ToString(),
                    email_ = ShopInfo.Email,
                    firstname_ = ShopInfo.FirstName,
                    lastname_ = ShopInfo.FamilyName,
                    furl = GetHost + "/Error/Error_List",
                    surl = GetHost + "/payment/ResultPay",
                    phone = ShopInfo.PhoneNumber,
                    productinfo = "Shop Setup Fees",
                    Created_Date = CurrentDate,
                    Updated_Date = CurrentDate
                };
                SPA.spa_Payment_Gateway.Add(PayGateway);
                SPA.SaveChanges();
                if (PayGateway.PaymentGatewayId != 0)
                {
                    UDEFINED.Add(Convert.ToString(PayGateway.PaymentGatewayId));
                    UDEFINED.Add(Convert.ToString(ShopInfo.LangId));
                    UDEFINED.Add("/Shop/Shop#Payment");
                    Models.Pay payu = new Models.Pay()
                    {
                        amount = PayGateway.amount_,
                        firstname = PayGateway.firstname_,
                        email = PayGateway.email_,
                        furl = PayGateway.furl,
                        surl = PayGateway.surl,
                        productinfo = PayGateway.productinfo,
                        lastname = PayGateway.lastname_,
                        phone = ShopInfo.PhoneNumber,
                        udf1 = UDEFINED,
                        payId = PayGateway.PaymentGatewayId,
                        Lang_id = ShopInfo.LangId
                    };
                    new paymentController().GoToPayu(payu);
                }
                return Redirect("Shop/Shop#Payment");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return Redirect("/Login/Login");
            }
        }
        public ActionResult Timeslot()
        {
            return View();
        }
        public ActionResult PaymentHistory()
        {
            var QryPayHistory = sql.getPayHistory(schlId);
            var QryExecute = SPA.Database.SqlQuery<PaymentHistory>(QryPayHistory).ToList();
            ViewBag.currency = QryExecute.Where(c => c.Currency != null).Select(c => c.Currency).FirstOrDefault();
            return View(QryExecute);
        }
        public ViewResult PaymentHistoryPrint(List<Models.BillingHistory> Billing, string Currency, int? Months, int? Year, bool status)
        {
            if (Billing == null || Billing.Count == 0)
            {
                Billing = fu.BookingHistory(Year, Months, status);
                if (Currency == null)
                    Currency = fu.currency(null);
            }
            ViewBag.status = status;
            ViewBag.BillingHistory = Billing;
            ViewBag.Currency = Currency;
            return View();
        }

        public ActionResult Image_crop()
        {
            return View();
        }
        public ActionResult Permissions()
        {
            return new AccessRightController().Permissions();
        }
        public ActionResult Roles()
        {
            return new AccessRightController().Roles();
        }
    }
}
