using SPA.Common;
using SPA.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA.Controllers
{
    [checkShop]
    public class BookOrderController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        Function fu = new Function();
        PushEmail Email = new PushEmail();
        PuchSMS SMS = new PuchSMS();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public BookOrderController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: BookOrder
        public ActionResult Index()
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
                //fu.RemoveCookie("Auth");
                return RedirectToAction("Customer", "Customer");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult OrderList(int id,string Status,string Url)
        {
            try
            {
                string Product = "select d.CatgTypeId as ProductId,a.SectionDesc as ProductName,d.orderData,b.Gender as Gender, d.SectionDesc,cast(a.Duration as int) as Duration,CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38, 2)) as Amount,f.CatgDesc as GroupName,e.GroupIdByShop from CTSP_SolutionMaster a join CCTSP_CategoryDetails b on a.sectionDesc = b.CatgDesc join cctsp_user c on a.UserId = c.UserId join CTSP_SolutionMaster d on d.CatgTypeId = b.CatgTypeId join SPA_SchedulerSlotMaster s on s.UserId = c.UserId join CCTSP_GroupProductDetails e on e.ProductId = b.CatgTypeId join CCTSP_CategoryDetails f on f.CatgTypeId = e.GroupIdByShop where a.activestatus = 'A' and d.activestatus = 'A' and c.activestatus = 'A'and b.activestatus = 'A' and s.ActiveStatus = 'A' and e.ActiveStatus = 'A' and f.ActiveStatus = 'A' and a.SchId = " + schlId + " and d.SchId = a.SchId and c.SchId = a.SchId and s.schId = a.SchId and e.SchlId = a.SchId and f.DomainType = a.SchId and a.SectionName = 'EmployeeProduct' and b.catgId = 111 and b.catgDesc = a.SectionDesc and d.Duration = a.Duration and d.amount = a.amount group by d.CatgTypeId,a.SectionDesc,d.orderData,b.Gender, d.SectionDesc,a.Duration,a.Amount,f.CatgDesc,e.GroupIdByShop,f.Group_orderdata order by f.Group_orderdata,d.orderdata";
                ViewBag.Product = SPA.Database.SqlQuery<Models.GroupProductList>(Product).ToList();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.Status = Status;
                ViewBag.id = id;
                ViewBag.Url = Url;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Choose_Product"|| (c.Page_Name == "Title" && c.Order_id == 2)) && c.Lang_id == ShopInfo.Lang_id.Value).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == ShopInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public long OrderListBooked(FormCollection form)
        {
            try
            {
                long UserId = Convert.ToInt32(form["UserId"]);
                var updateAllHistory = "update SPA_EmployeeScheduler set ActiveStatus='DE',BookedStatus='DE' where CLIENT_UserId=" + UserId + " and ActiveStatus='DA' and BookedStatus='T' and SchId=" + schlId;
                SPA.Database.ExecuteSqlCommand(updateAllHistory);
                var updateAllHistoryDetail = "update SPA_EmployeeScheduler set ActiveStatus='DE',BookedStatus='DE' where CLIENT_UserId=" + UserId + " and ActiveStatus='DA' and BookedStatus='BT' and SchId=" + schlId;
                SPA.Database.ExecuteSqlCommand(updateAllHistoryDetail);
                foreach (var item in form.Keys)
                {
                    if (item.ToString() != "UserId")
                    {
                        SPA_EmployeeScheduler Employee = new SPA_EmployeeScheduler();
                        Employee.CLIENT_UserId = UserId;
                        long CatgTypeId = Convert.ToInt32(item);
                        Employee.Product_Id = CatgTypeId;
                        var data = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == Employee.Product_Id).FirstOrDefault();
                        Employee.Product_price = data.Amount;
                        Employee.SchId = schlId;
                        DateTime NowDate = fu.ZonalDate(null);
                        Employee.CreatedOn = NowDate;
                        Employee.ActiveStatus = "DA";
                        Employee.BookedStatus = "T";
                        Employee.reg_status = "M";
                        Employee.UpdatedOn = NowDate;
                        SPA.SPA_EmployeeScheduler.Add(Employee);
                        SPA.SaveChanges();
                        Session["ResId"] = Employee.EmpSchDetailsId;
                    }
                }
                return UserId;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return 0;
            }
        }
        #region new code for choose_employee
        [CustomAutohrize(null)]
        public ActionResult ChooseEmployee(long? Id,string Status,string Url)
        {
            try
            {
                ViewBag.UserId = Id;
                ViewBag.Status = Status;
                ViewBag.Url = Url;
                if (Id == 0)
                {
                    //When there is no Users Selected then it returns back To Selection of customer
                    return RedirectToAction("Customer", "Customer");
                }
                else
                {
                    //Language
                    #region Language
                    var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                    ViewBag.ShopInfo = ShopInfo;
                    int LanguageId = ShopInfo.Lang_id.Value;
                    ViewBag.currency = fu.currency(ShopInfo.Currency);
                    ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Choose_Employee"||(c.Page_Name== "Title" && c.Order_id== 4)) && c.Lang_id == LanguageId).ToList();
                    ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                    if (LanguageId == 1) { ViewBag.LocalLanguage = "en"; }
                    if (LanguageId == 2) { ViewBag.LocalLanguage = "de"; }
                    if (LanguageId == 3) { ViewBag.LocalLanguage = "fr"; }
                    #endregion
                    //Check whether the User is Shop Owner or not
                    #region LoginCheck
                    //if (Convert.ToInt32(Session["RoleId"].ToString()) !=4)
                    //{
                        long UserId = Convert.ToInt64(Id);
                        int ReservationId = Convert.ToInt32(Session["ResId"]);
                        //Products
                        string ProListShow = "select a.CLIENT_UserId,a.SchId,b.CatgTypeId,b.CatgDesc,c.SectionName,c.SectionDesc,CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN Amount ELSE NULL END AS decimal(38,2)) as Amount,cast(c.Duration as int) as Duration  from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on a.Product_Id=b.CatgTypeId  join ctsp_solutionMaster c on c.CatgTypeId=b.CatgTypeId  where a.ActiveStatus='DA' and a.BookedStatus='T' and a.SchId=" + schlId + " and a.EmpSchDetailsId=" + ReservationId + "";
                        var Prodlist = SPA.Database.SqlQuery<Models.ProductList>(ProListShow).ToList();
                        //Employee Information
                        //string EmpInformation = "select d.UserId,e.FirstName,e.LastName,e.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on a.Product_Id = b.CatgTypeId join Ctsp_solutionMaster c on c.CatgTypeId = b.CatgTypeId join CTSp_solutionMaster d on d.SectionDesc = b.CatgDesc join CCTSP_User e on e.UserId = d.UserId join SPA_SchedulerSlotMaster f on f.UserId=e.UserId where a.EmpSchDetailsId = " + ReservationId + " and d.SchId = " + schlId + " and b.DomainType = " + schlId + " and e.SchId = " + schlId + " and c.SchId = " + schlId + " and a.schId = " + schlId + " and c.SectionDesc = d.Image and c.Duration = d.Duration and c.Amount = d.Amount and d.ActiveStatus = 'A' and e.ActiveStatus = 'A' and b.ActiveStatus = 'A' and b.ActiveStatus = 'A' and f.schId=" + schlId + " and f.ActiveStatus='A' group by d.UserId,e.FirstName,e.LastName,e.Answer2";
                        string EmpInformation = "select d.UserId,e.FirstName,e.LastName,e.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on a.Product_Id = b.CatgTypeId join Ctsp_solutionMaster c on c.CatgTypeId = b.CatgTypeId join CTSp_solutionMaster d on d.SectionDesc = b.CatgDesc join CCTSP_User e on e.UserId = d.UserId join SPA_SchedulerSlotMaster f on f.UserId=e.UserId where a.EmpSchDetailsId = " + ReservationId + " and e.RoleId not in (1,4)  and d.SchId = " + schlId + " and b.DomainType = " + schlId + " and e.SchId = " + schlId + " and c.SchId = " + schlId + " and a.schId = " + schlId + " and c.SectionDesc = d.Image and c.Duration = d.Duration and c.Amount = d.Amount and d.ActiveStatus = 'A' and e.ActiveStatus = 'A' and b.ActiveStatus = 'A' and b.ActiveStatus = 'A' group by d.UserId,e.FirstName,e.LastName,e.Answer2";
                        var EmpList = SPA.Database.SqlQuery<Models.EmployeeList>(EmpInformation).ToList();
                        if (Prodlist.Count > 0 && EmpList.Count > 0)
                        {
                            ViewBag.ProductInfo = Prodlist;
                            ViewBag.Employees = EmpList;
                            return View();
                        }
                        else
                            return RedirectToAction("OrderList", "BookOrder", new { Id = Id });
                    //}
                    #endregion
                    //Login
                    #region GoToLogin
                    //else
                    //    return RedirectToAction("SignIn", "Login");

                    #endregion
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        #endregion
        [CustomAutohrize(null)]
        public ActionResult ConfirmBooking(long? Id,string Status,string Url)
        {
            try
            {
                ViewBag.UserId = Id;
                ViewBag.Status = Status;
                ViewBag.Url = Url;
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                    int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                    ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Confirm_Booking" || c.Page_Name=="" || (c.Page_Name == "Title" && c.Order_id == 5)) && c.Lang_id == LanguageId).ToList();
                    ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
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
        public ActionResult DisplayBookedProduct(long? Id,string Status,string Url)
        {
            try
            {
                ViewBag.UserId = Id;
                ViewBag.Status = Status;
                ViewBag.Url = Url;
                int ReservationId = Convert.ToInt32(Session["ResId"]);
                #region NewCode               
                int clientUserId = Convert.ToInt32(Id);
                string DisplayBkPro = "select a.EmpSchDetailsId,a.BookingDate,b.CatgDesc as ProductName,a.FromSlotMasterId ,c.sectiondesc as Productdesc,cast(c.Duration as int) as Duration, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id  join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId where a.BookedStatus = 'BT' and a.ActiveStatus = 'DA' and a.EmpSchDetailsId=" + ReservationId + " and a.schid = " + schlId + " and b.domaintype = " + schlId + " and b.activestatus = 'A' and b.catgid = 111 and c.activestatus = 'A' and c.SchId =" + schlId + " and d.activestatus='A' and d.schid=" + schlId + "";
                ViewBag.DisplayBookProduct = SPA.Database.SqlQuery<Models.ConfirmModel>(DisplayBkPro).ToList();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Confirm_Booking" || c.Page_Name == "Small_calander") && c.ActiveStatus=="A" && c.Lang_id == ShopInfo.Lang_id).ToList();
                return View();
                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public string BookMyOrder(long Id, string Comments,string Status)
        {
            try
            {
                ViewBag.UserId = Id;  
                var query = "";
                if (!string.IsNullOrEmpty(Convert.ToString(Session["ResId"])))
                {
                    long ClientUserId = Convert.ToInt64(Id);
                    int ReservationId = Convert.ToInt32(Session["ResId"]);
                    var EnterpriseInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                    // var BookingDetails = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                    string BookedData = "select a.EmpSchDetailsId,a.comment,a.ActiveStatus,a.BookedStatus,a.BookingDate,b.CatgTypeid as ProductId ,b.CatgDesc as ProductName,a.Product_price,a.FromSlotMasterId ,a.ToSlotMasterId,a.Emp_UserId as EMP_UserId ,a.CLIENT_UserId as CLIENT_UserId, c.sectiondesc as Productdesc, cast(c.Duration as int) as Duration,e.FirstName as ClientName, e.LastName as ClientLastName ,f.FirstName as ShopOwnerName , f.LastName as ShopOwnerLastName,e.EmailId as ClientEmail,e.PhoneNo as ClientPhoneNo, FORMAT(a.CreatedOn ,'dd-MM-yyyy hh:mm') as CreatedDate, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId  join CCTSP_User e on e.UserId = a.CLIENT_UserId join CCTSP_User f on f.schid = a.schid where a.schid =" + schlId + "  and b.catgid = 111 and c.SchId = " + schlId + " and d.schid = " + schlId + " and f.schid = " + schlId + " and f.RoleId = 1  and f.ActiveStatus = 'A' and a.EmpSchDetailsId = " + ReservationId + "";
                    var BookingDetails = SPA.Database.SqlQuery<Models.ConfirmModel>(BookedData).FirstOrDefault();
                    string startdate = DateTime.Parse(BookingDetails.BookingDate + " " + BookingDetails.FromSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                    string EndDate = DateTime.Parse(BookingDetails.BookingDate + " " + BookingDetails.ToSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                    var ValidationCheck = fu.CheckReservationExistOrNot(startdate, EndDate, BookingDetails.EMP_UserId.Value);
                    #region AllowPastDateCheck
                    //For Allow PastDate
                    var ActiveStatus = "A";
                    var BookedStatus = "B";
                    bool PastDate = false;
                    PastDate = fu.CheckPastDate(EnterpriseInfo.TimeZone, DateTime.Parse(BookingDetails.BookingDate + " " + BookingDetails.FromSlotMasterId));
                    if (PastDate == true)
                    {
                        ActiveStatus = "C";
                        BookedStatus = "C";
                    }
                    #endregion
                    if (ValidationCheck == "Yes")
                    {
                        Session["Result"] = Status+"BookedError";
                        return Status+"error";
                    }
                    else
                    {
                        var ReservationInfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId && c.BookedStatus.Trim() == "BT" && c.ActiveStatus.Trim() == "DA" && c.SchId == schlId && c.Product_Id != null).FirstOrDefault();
                        if(ReservationInfo != null)
                        {
                            ReservationInfo.BookedStatus = BookedStatus;
                            ReservationInfo.ActiveStatus = ActiveStatus;
                            ReservationInfo.reg_status = "M";
                            if (!string.IsNullOrEmpty(Comments))
                                ReservationInfo.Comment = Comments;
                            SPA.SaveChanges();

                        }
                        //if (string.IsNullOrEmpty(Comments))
                        //    query = "update SPA_EmployeeScheduler set BookedStatus='B',ActiveStatus='A',reg_status='M' where EmpSchDetailsId=" + ReservationId + " and BookedStatus='BT' and ActiveStatus='DA' and SchId=" + schlId + " and product_id is not null";
                        //else
                        //    query = "update SPA_EmployeeScheduler set BookedStatus='B',ActiveStatus='A',reg_status='M',Comment='" + Comments + "' where EmpSchDetailsId=" + ReservationId + " and BookedStatus='BT' and ActiveStatus='DA' and SchId=" + schlId + " and product_id is not null";
                        //SPA.Database.ExecuteSqlCommand(query);
                       
                        //PaymentDeduction
                        fu.PaymentDeduction(BookingDetails.Product_price, ReservationId, EnterpriseInfo,"A");
                        //Add MasterData
                        fu.AddMasterData(BookingDetails, ActiveStatus, BookedStatus, "M");
                        // New Code
                        // var data = SPA.SPA_EmployeeScheduler.Where(c => c.CLIENT_UserId == ClientUserId && c.BookedStatus == "B" && c.ActiveStatus == "A" && c.SchId == schlId).OrderByDescending(c => c.EmpSchDetailsId).FirstOrDefault();
                        if (BookingDetails != null)
                        {
                            var data1 = SPA.CCTSP_User.Where(c => c.UserId == BookingDetails.CLIENT_UserId).FirstOrDefault();
                            string name = data1.Gender + " " + data1.FirstName + " " + data1.LastName;
                            var checkSMSTEMP = SPA.CCTSP_User.Where(c => c.UserId == BookingDetails.CLIENT_UserId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                            if (checkSMSTEMP != null && PastDate == false)
                                SMS.Approvebooking(name, BookingDetails.BookingDate, data1.LoginName);
                            var checkEMAILTEMP = SPA.CCTSP_User.Where(c => c.UserId == BookingDetails.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                            if (checkEMAILTEMP != null && PastDate==false)
                                Email.Approvebooking(name, BookingDetails.BookingDate, data1.EmailId, BookingDetails.FromSlotMasterId, BookingDetails.EmpSchDetailsId);
                        }
                        return "";
                    }
                }
                else
                {
                    Session["Result"] = Status+"BookedError";
                    return Status+"error";
                }
                    
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return Status+"error";
            }
        }
        public ActionResult GoBackEmployee(long? Id)
        {
            try
            {
                long User = Convert.ToInt32(Id);
                var GoBackEmployeeQuery = "update SPA_EmployeeScheduler set ActiveStatus ='DA',BookedStatus ='T' where CLIENT_UserId=" + User + " and BookedStatus='BT' and SchId='" + schlId + "' and ActiveStatus = 'DA'";
                SPA.Database.ExecuteSqlCommand(GoBackEmployeeQuery);
                return RedirectToAction("ChooseEmployee", "BookOrder", new { Id = User });
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult ThanksMessage(long? Id,string Status,string Url)
        {
            try
            {
                ViewBag.UserId = Id;
                ViewBag.Status = Status;
                ViewBag.Url = Url;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                    int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                    ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Booking_Done" || (c.Page_Name == "Title" && c.Order_id == 6)) && c.Lang_id == LanguageId).ToList();
                    ViewBag.LanguageAlertBox = SPA.Language_Label_Detail.Where(c => c.Page_Name == "AlertMsg" && c.Lang_id == LanguageId && (c.Order_id == 26 || c.Order_id == 27 || c.Order_id == 28)).ToList();
                    return View();
                //}
                //else
                //    return RedirectToAction("Login", "Login");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
    }
}