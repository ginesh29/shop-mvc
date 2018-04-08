using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using System.Globalization;
using SPA.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace SPA.Controllers
{
    [checkShop]
    public class ReservationController : Controller
    {
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string EmailFrom = (ConfigurationManager.AppSettings["ApplicationVariableEmailBCC"]).ToString();
        PushEmail Email = new PushEmail();
        PuchSMS SMS = new PuchSMS();
        Function fu = new Function();
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        JavaScriptSerializer js = new JavaScriptSerializer();
        CultureInfo enGB = new CultureInfo("en-GB");
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public ReservationController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: Reservation
        [CustomAutohrize(null)]
        public ActionResult Reservation()
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    Flow_Id = c.Flow_Id,
                    BookingApproval=c.BookingApproval                  
                }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Reservation_Tab" || (c.Page_Name == "Title" && c.Order_id == 12)) && c.Lang_id == ShopInfo.Lang_id).ToList();
                ViewBag.BookingApproval = ShopInfo.BookingApproval;
                ViewBag.SubMenu = fu.AccessSubMenu(ShopInfo.Lang_id.Value, Convert.ToInt32(Session["RoleId"].ToString()), "Reservation_Tab",ShopInfo.Flow_Id);
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult DisplayCalbookingDetails(int? ReservationId, string view, string AllView)
        {
            try
            {
                ViewBag.view = view;
                ViewBag.AllView = AllView;
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, SchlStudentStrength = c.SchlStudentStrength }).FirstOrDefault();
               ViewBag.SchlStudentStrength = ShopInfo.SchlStudentStrength != null ? ShopInfo.SchlStudentStrength.Value : 0;
                var TimeZone = ShopInfo.TimeZone;
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                DateTime EuropeDate = fu.ZonalDate(TimeZone);
                ViewBag.Europdate = EuropeDate;
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //string DisplayCalbookingDetails = "select a.EmpSchDetailsId,a.comment,a.ActiveStatus,a.BookedStatus,a.BookingDate,b.CatgDesc as ProductName,a.Product_price,a.FromSlotMasterId ,a.ToSlotMasterId, c.sectiondesc as Productdesc, cast(c.Duration as int) as Duration,e.FirstName as ClientName, e.LastName as ClientLastName ,f.FirstName as ShopOwnerName , f.LastName as ShopOwnerLastName,e.EmailId as ClientEmail,e.PhoneNo as ClientPhoneNo, FORMAT(a.CreatedOn ,'dd-MM-yyyy hh:mm') as CreatedDate, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId  join CCTSP_User e on e.UserId = a.CLIENT_UserId join CCTSP_User f on f.schid = a.schid where a.schid =" + schlId + "  and b.catgid = 111 and c.SchId = " + schlId + " and d.schid = " + schlId + " and e.schid = " + schlId + " and f.schid = " + schlId + " and f.RoleId = 1  and f.ActiveStatus = 'A' and a.EmpSchDetailsId = " + ReservationId + "";
                string DisplayCalbookingDetails = "select a.EmpSchDetailsId,a.comment,a.ActiveStatus,a.BookedStatus,a.Reg_Status, " +
                                                  "a.BookingDate,b.CatgDesc as ProductName,a.Product_price,a.FromSlotMasterId ,a.ToSlotMasterId, " +
                                                  "c.sectiondesc as Productdesc, cast(c.Duration as int) as Duration,e.FirstName as ClientName,  " +
                                                  "e.LastName as ClientLastName ,f.FirstName as ShopOwnerName , f.LastName as ShopOwnerLastName, " +
                                                  "e.EmailId as ClientEmail,e.PhoneNo as ClientPhoneNo, FORMAT(a.CreatedOn ,'dd-MM-yyyy hh:mm') as CreatedDate, " +
                                                  "CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName, " +
                                                  "d.LastName,d.Answer2 as Image, a.Emp_UserId as EMP_UserId, " +
                                                  "j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName as FlowStatus,k.Invoice_id as Invoice_id,l.Invoice_Status as Invoice_Status " +
                                                  "from SPA_EmployeeScheduler a  " +
                                                  "join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id " +
                                                  "join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId  " +
                                                  "join CCTSP_User d on d.UserId = a.Emp_UserId  " +
                                                  "join CCTSP_User e on e.UserId = a.CLIENT_UserId " +
                                                  "join CCTSP_User f on f.schid = a.schid " +
                                                  "join ctsp_SolutionMaster g on g.SectionName = 'Calendar Month' and g.Activestatus='A' " +
                                                  "join cctsp_user h on h.UserId = " + UserId + " " +
                                                 "join cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236) " +
                                                 "and i.ActiveStatus = 'A' " +
                                                 "join cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                                                 " and j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub) " +
                                                 " and j.SchId = i.Schlid " +
                                                 " and((j.AccessToData = 'Own' and a.Emp_UserId = h.UserId) or(j.AccessToData != 'Own')) " +
                                                 "left join SPA_INVOICE_Detail k on k.ReservationId=a.EmpSchDetailsId and k.ActiveStatus='A' "+
                                                 "left join SPA_INVOICE_STATUS l on l.Invoice_Id = k.invoice_Id and l.ActiveStatus = 'A' " +
                                                  "where a.schid =" + schlId + "  and b.catgid = 111 and c.SchId = " + schlId + " and " +
                                                  "d.schid = " + schlId + "and f.schid = " + schlId + " and f.RoleId = 1  and f.ActiveStatus = 'A' " +
                                                  " and a.EmpSchDetailsId = " + ReservationId + "";
                var DisplayBookedDetail = SPA.Database.SqlQuery<Models.ConfirmModel>(DisplayCalbookingDetails).FirstOrDefault();
                ViewBag.DisplayBookedDetail = DisplayBookedDetail;
                ViewBag.CheckAccessRight =fu.CheckAccessofPage("For Invoicing", UserId);
                //All Language of Page
                var Language = new List<Language_Label_Detail>();
                Language.AddRange(fu.getLanguageData("Reservation_update", 0, ShopInfo.Lang_id.Value));
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == ShopInfo.Lang_id && c.Order_id >= 8 && c.ActiveStatus == "A" && c.Page_Name == "Small_calander").ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Common" && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.Lang_id).ToList());
                ViewBag.Language_Reservation_Update = Language;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult PendingforApproval()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.Language = fu.getLanguageData("Reservation_Pending", 0, ShopInfo.Lang_id.Value);
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
        public void PendingDataReply(string status, int Id, string BookingDate, string ViewStatus, string AllView)
        {
            try
            {
                var BookinInfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Id).FirstOrDefault();
                long Empuserid = BookinInfo.EMP_UserId.Value;
                if (AllView != null && AllView == "All" && ViewStatus == "Day")
                    Session["AllView"] = "All";
                if (ViewStatus != null && ViewStatus != "")
                {
                    Session["View"] = ViewStatus;
                    Session["ViewUserId"] = Empuserid.ToString();
                }
                if (BookingDate != null && BookingDate != "")
                {
                    var BDate = Convert.ToDateTime(BookingDate, enGB);
                    Session["ViewDate"] = BDate.Day + "-" + BDate.AddMonths(-1).ToString("MM") + "-" + BDate.Year;
                }
                status = status.Trim();
                //var query = "update SPA_EmployeeScheduler set ActiveStatus='" + status + "' where EmpSchDetailsId=" + Id;
                //SPA.Database.ExecuteSqlCommand(query);
                // var UpdateResinfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Id).FirstOrDefault();
                BookinInfo.ActiveStatus = status;
                UpdateModel(BookinInfo);
                if (BookinInfo.MasterResId > 0 && BookinInfo.MasterResId != null)
                {
                    var UpdateMasterinfo = SPA.spa_Master_Reservation.Where(c => c.MasterResId == BookinInfo.MasterResId).FirstOrDefault();
                    UpdateMasterinfo.ActiveStatus = status;
                    UpdateModel(BookinInfo);
                }
                SPA.SaveChanges();
                if (status == "A")
                {
                    var EnterpriseInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                    fu.PaymentDeduction(Convert.ToString(BookinInfo.Product_price), Id, EnterpriseInfo, "A");
                }
                #region EmailandSMSCode
                var qry = "";
                var qryemail = "";
                string MsgSubject = "";
                if (status == "A")
                {
                    MsgSubject = "App";
                    qry = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == MsgSubject && c.Activestatus == "A" && c.SchId == schlId && c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgDesc == "SPA SMS").Select(c => c.SectionDesc).FirstOrDefault();
                    qryemail = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == MsgSubject && c.Activestatus == "A" && c.SchId == schlId && c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgDesc == "SPA Email").Select(c => c.SectionDesc).FirstOrDefault();
                }
                else
                {
                    MsgSubject = "Can";
                    qry = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == MsgSubject && c.Activestatus == "A" && c.SchId == schlId && c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgDesc == "SPA SMS").Select(c => c.SectionDesc).FirstOrDefault();
                    qryemail = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == MsgSubject && c.Activestatus == "A" && c.SchId == schlId && c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgDesc == "SPA Email").Select(c => c.SectionDesc).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(qry))
                {
                    var qry1 = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Id && c.SchId == schlId).FirstOrDefault();
                    var qry2 = SPA.CCTSP_User.Where(a => a.UserId == qry1.CLIENT_UserId).FirstOrDefault();
                    if (qry2 != null)
                    {
                        string name = qry2.Gender + " " + qry2.FirstName + " " + qry2.LastName;
                        string date = qry1.BookingDate;
                        if (MsgSubject == "App")
                        {
                            var checkSMSApprove = SPA.CCTSP_User.Where(c => c.UserId == qry1.CLIENT_UserId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                            if (checkSMSApprove != null)
                                SMS.Approvebooking(name, date, qry2.LoginName);
                        }
                        else
                        {
                            var checkSMSCancel = SPA.CCTSP_User.Where(c => c.UserId == qry1.CLIENT_UserId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                            if (checkSMSCancel != null)
                                SMS.Cancelbooking(name, date, qry2.LoginName);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(qryemail))
                {
                    var qry3 = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Id && c.SchId == schlId).FirstOrDefault();
                    var qry4 = SPA.CCTSP_User.Where(a => a.UserId == qry3.CLIENT_UserId).FirstOrDefault();
                    if (qry4 != null)
                    {
                        string name = qry4.Gender + " " + qry4.FirstName + " " + qry4.LastName;
                        string date = qry3.BookingDate;
                        if (MsgSubject == "App")
                        {
                            var checkEMAILApprove = SPA.CCTSP_User.Where(c => c.UserId == qry3.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                            if (checkEMAILApprove != null)
                                Email.Approvebooking(name, date, qry4.EmailId, qry3.FromSlotMasterId, qry3.EmpSchDetailsId);
                        }
                        else
                        {
                            var checkEMAILCancel = SPA.CCTSP_User.Where(c => c.UserId == qry3.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                            if (checkEMAILCancel != null)
                                Email.Cancelbooking(name, qry4.EmailId, qry3.FromSlotMasterId, date, qry3.EmpSchDetailsId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

            #endregion
        }
        public void StatusFinalChange(long? Id, string Status, string checkornot)
        {
            try
            {
                string status1 = "";
                if (checkornot == "Checked")
                    status1 = Status;
                else
                {
                    Status = "A";
                    status1 = "B";
                }
                SPA_EmployeeScheduler employeeBook = new SPA_EmployeeScheduler();
                employeeBook = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Id).FirstOrDefault();
                employeeBook.ActiveStatus = Status;
                employeeBook.BookedStatus = status1;
                employeeBook.UpdatedOn = DateTime.Today;
                UpdateModel(employeeBook);
                if (employeeBook.MasterResId > 0 && employeeBook.MasterResId != null)
                {
                    var UpdateMasterinfo = SPA.spa_Master_Reservation.Where(c => c.MasterResId == employeeBook.MasterResId).FirstOrDefault();
                    UpdateMasterinfo.ActiveStatus = Status;
                    UpdateMasterinfo.BookedStatus = status1;
                    UpdateModel(UpdateMasterinfo);
                }
                SPA.SaveChanges();                
                if (Status == "D")
                {
                    var BookedDetails = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Id && c.SchId == schlId).FirstOrDefault();
                    var UserDetails = SPA.CCTSP_User.Where(a => a.UserId == BookedDetails.CLIENT_UserId).FirstOrDefault();
                    if (UserDetails != null)
                    {
                        string name = UserDetails.Gender + " " + UserDetails.FirstName + " " + UserDetails.LastName;
                        string date = BookedDetails.BookingDate;
                        var checkEMAILCancel = SPA.CCTSP_User.Where(c => c.UserId == BookedDetails.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                        if (checkEMAILCancel != null)
                            Email.Cancelbooking(name, UserDetails.EmailId, BookedDetails.FromSlotMasterId, date, BookedDetails.EmpSchDetailsId);
                        var checkSMSCancel = SPA.CCTSP_User.Where(c => c.UserId == BookedDetails.CLIENT_UserId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                        if (checkSMSCancel != null)
                            SMS.Cancelbooking(name, date, UserDetails.LoginName);
                    }

                }

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void BookingStatusChange(long? Id, string Status, string checkornot,string View,string AllView,string Date,long? EmpUserId)
        {
            //Redirection to calendar
            fu.setRedirectionofCalendar(View, AllView, Date, EmpUserId);
            //StatusofAppClosed
            StatusFinalChange( Id,  Status, checkornot);
        }
        #region NewCode PendingForApprovalDetail
        [CustomAutohrize(null)]
        public ActionResult PendingForApprovalDetail(string Status, string OrderBy, int? Sorting)
        {
            try
            {
                string BookedData = "";
                string PendingData = "";
                var Desc = "";
                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = "cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
                if (OrderBy == "Date")
                    OrderBy = "cast(a.BookingDate as date)";
                //if (OrderBy == "Employee")
                //    OrderBy = "e.firstName";
                if (OrderBy == "Service")
                    OrderBy = "b.CatgDesc";
                //if (OrderBy == "Customer")
                //    OrderBy = "d.firstname";
                if (Sorting == 1)
                    Desc = "DESC";
                if (Status == "Pending" || Status == null)
                {
                    BookedData = "B";
                    PendingData = "DA";
                    ViewBag.StatusApproval = "Pending";
                }
                if (Status == "Accepted")
                {
                    BookedData = "B";
                    PendingData = "A";
                    ViewBag.StatusApproval = Status;
                }
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var subAccess = fu.AccessDetails(Convert.ToInt32(Session["RoleId"].ToString()), "Pending For Approval");
                string Data = "  select a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName,a.EmpSchDetailsId," +
                              "  b.CatgDesc as CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId,cast(a.ToSlotMasterId as time) as ToSlotMasterId, " +
                              "  cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff, " +
                              "  CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38,2)) as Amount," +
                              "  a.CreatedOn as CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone, " +
                              "  g.AccessToData ,g.insertAccess,g.DeleteAccess,g.UpdateAccess,f.ItenName as FlowStatus" +
                              "  from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id=b.catgtypeId " +
                              "  join ctsp_solutionMaster c on b.catgtypeId=c.catgtypeId " +
                              "  join cctsp_user d on a.client_userid=d.UserId " +
                              "  join cctsp_user e on e.UserId=a.Emp_UserId" +
                              "  join ctsp_SolutionMaster f on f.SectionName = 'Pending For Approval' and f.ActiveStatus='A' " +
                              "  join cctsp_user h on h.UserId = " + UserId + " " +
                              "  join cctsp_Role i on i.Roleid=h.Roleid and (i.schlid=" + schlId + " or i.schlid=236) and i.ActiveStatus='A' " +
                              "  join cctsp_RoleDetails g on g.RoleId = i.RoleId and g.ActiveStatus = 'A' and f.SolutionId = convert(bigint, g.AccesstoSub) " +
                              "  and g.SchId = i.schlid and f.SolutionId = convert(bigint, g.AccesstoSub) and " +
                              " ((g.AccessToData = 'Own' and a.Emp_UserId = h.UserId) or(g.AccessToData != 'Own'))" +
                              "  where a.BookedStatus='" + BookedData + "' and a.ActiveStatus='" + PendingData + "' and a.SchId=" + schlId + "" +
                              "  and d.RoleId=4 " +
                              "  group by a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName,e.lastName ,f.ItenName ," +
                              "  a.EmpSchDetailsId, b.CatgDesc ,c.Duration ,a.Product_price,a.CreatedOn,d.firstname,d.lastName,d.phoneNo, " +
                              "  a.BookingDate,a.FromSlotMasterId,a.ToSlotMasterId,a.Emp_UserId,g.AccessToData ,g.insertAccess,g.DeleteAccess,g.UpdateAccess" +
                              "  order by " + OrderBy + " " + Desc;
                if (Status == "Declined")
                {
                    PendingData = "D";
                    Data = "select a.BookedStatus,a.ActiveStatus,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName,a.EmpSchDetailsId,b.CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId,cast(a.ToSlotMasterId as time) as ToSlotMasterId,cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff,CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38,2)) as Amount,a.CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id=b.catgtypeId join ctsp_solutionMaster c on b.catgtypeId=c.catgtypeId join cctsp_user d on a.client_userid=d.UserId join cctsp_user e on e.UserId=a.Emp_UserId where a.BookedStatus='B' and a.ActiveStatus='" + PendingData + "' and a.SchId=" + schlId + " and d.RoleId=4 and e.RoleId=3 order by cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
                    ViewBag.StatusApproval = Status;
                }
                if (Status == "All")
                {
                    Data = "select a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName,a.EmpSchDetailsId,b.CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId,cast(a.ToSlotMasterId as time) as ToSlotMasterId,cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff,CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38,2)) as Amount,a.CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id=b.catgtypeId join ctsp_solutionMaster c on b.catgtypeId=c.catgtypeId join cctsp_user d on a.client_userid=d.UserId join cctsp_user e on e.UserId=a.Emp_UserId where a.BookedStatus='B' and a.SchId=" + schlId + " and d.RoleId=4 and e.RoleId=3 and (a.ActiveStatus='A' or a.ActiveStatus='DA' or a.ActiveStatus='D') order by cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
                    ViewBag.StatusApproval = Status;
                }
                //var AllData1= SPA.Database.SqlQuery<Models.PendingApproval>(Data).ToList();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.AllData = SPA.Database.SqlQuery<Models.PendingApproval>(Data).ToList();
                var Language = new List<Language_Label_Detail>();
                Language.AddRange(fu.getLanguageData("Reservation_Pending", 0, ShopInfo.Lang_id.Value));
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Common" && (c.Order_id == 1 || c.Order_id == 2) && c.Lang_id == ShopInfo.Lang_id).ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == ShopInfo.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander").ToList());
                ViewBag.OpenLang = Language;
                if (OrderBy == "cast(a.BookingDate as date)")
                    OrderBy = "Date";
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
        #endregion
        #region NewCode PendingForListViewDetail
        //public ActionResult OpenListView(string Status)
        //{
        //    try
        //    {
        //        if (Convert.ToInt32(Session["RoleId"].ToString()) == 1)
        //        {
        //            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
        //            ViewBag.currency = fu.currency(ShopInfo.Currency);
        //            ViewBag.DoctorPage = ShopInfo.SchlStudentStrength;
        //            ViewBag.status = Status;
        //            return View();
        //        }
        //        else
        //            return RedirectToAction("SignIn", "Login");
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return RedirectToAction("Error_List", "Error");
        //    }
        //}
        //public ActionResult PendingForListViewDetail(string Status)
        //{
        //    try
        //    {
        //        //AS is For ActiveStatus
        //        string AS = "";
        //        //BS is for BookedStatus
        //        string BS = "";
        //        string ResultOpenListView = "";
        //        //Pending Data
        //        if (Status == "Pending" || Status == null)
        //        {
        //            ViewBag.status = "Pending";
        //            //string data = "select a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId, cast(a.ToSlotMasterId as time) as ToSlotMasterId,cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff,CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount, a.CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone ,StDoctNotes=(select count(y.Prescription_DetailId) + count(z.Medicine_Id) from Prescription_Master x, Prescription_Detail y, Medicine_Master z where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 1 and y.Prescription_Id = x.Prescription_Id and z.Prescription_Id = y.Prescription_Id and z.BookingId = y.BookingId and z.ActiveStatus = 'A'), StPrescription = (select count(y.Prescription_DetailId) + count(z.Medicine_Id) from Prescription_Master x,Prescription_Detail y, Medicine_Master z  where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 2 and y.Prescription_Id = x.Prescription_Id and z.Prescription_Id = y.Prescription_Id and z.BookingId = y.BookingId and z.ActiveStatus = 'A') ,StMerge=(select count(y.Prescription_DetailId) + count(z.Medicine_Id) from Prescription_Master x, Prescription_Detail y, Medicine_Master z where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 3 and y.Prescription_Id = x.Prescription_Id and z.Prescription_Id = y.Prescription_Id and z.BookingId = y.BookingId and z.ActiveStatus = 'A') from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id = b.catgtypeId join ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId join cctsp_user d on a.client_userid = d.UserId join cctsp_user e on e.UserId = a.Emp_UserId where (a.BookedStatus = 'B' or a.BookedStatus = 'Z')   and(a.ActiveStatus = 'A' or a.ActiveStatus = 'Z') and a.SchId = " + schlId + " and d.RoleId = 4 and e.RoleId = 3 order by cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
        //            ResultOpenListView = "select a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate, cast(a.FromSlotMasterId as time) as FromSlotMasterId, cast(a.ToSlotMasterId as time) as ToSlotMasterId, cast((datediff(minute, cast(a.FromSlotMasterId as time), cast(a.ToSlotMasterId as time))) as int) as diff, CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount, a.CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone, StDoctNotes = (select count(x.Prescription_Id) + (select count(x.Prescription_Id) from Medicine_Master z ,Prescription_Master x where x.Prescription_Id = z.Prescription_Id and z.BookingId = a.EmpSchDetailsId and z.ActiveStatus = 'A' and x.BookingId = a.EmpSchDetailsId and diff = 1 and x.ActiveStatus = 'A')  from Prescription_Master x, Prescription_Detail y where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 1 and y.Prescription_Id = x.Prescription_Id), StPrescription = (select count(x.Prescription_Id) + (select count(x.Prescription_Id) from Medicine_Master z ,Prescription_Master x where x.Prescription_Id = z.Prescription_Id and z.BookingId = a.EmpSchDetailsId and z.ActiveStatus = 'A' and x.BookingId = a.EmpSchDetailsId and diff = 2 and x.ActiveStatus = 'A') from Prescription_Master x, Prescription_Detail y where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 2 and y.Prescription_Id = x.Prescription_Id), StMerge = (select count(x.Prescription_Id) +(select count(x.Prescription_Id) from Medicine_Master z ,Prescription_Master x where x.Prescription_Id = z.Prescription_Id and z.BookingId = a.EmpSchDetailsId and z.ActiveStatus = 'A' and x.BookingId = a.EmpSchDetailsId and diff = 3 and x.ActiveStatus = 'A') from Prescription_Master x, Prescription_Detail y where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A'and x.ActiveStatus = 'A' and x.diff = 3 and y.Prescription_Id = x.Prescription_Id) from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id = b.catgtypeId join ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId join cctsp_user d on a.client_userid = d.UserId join cctsp_user e on e.UserId = a.Emp_UserId where (a.BookedStatus = 'B' or a.BookedStatus = 'Z') and(a.ActiveStatus = 'A' or a.ActiveStatus = 'Z') and a.SchId = " + schlId + " and d.RoleId = 4 and e.RoleId = 3 order by cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
        //            ViewBag.AllData = SPA.Database.SqlQuery<Models.PendingApproval>(ResultOpenListView).ToList();
        //        }
        //        else
        //        {
        //            //Appointment Closed
        //            if (Status == "Appointment Closed")
        //            {
        //                AS = "C";
        //                BS = "C";
        //                ViewBag.status = Status;
        //            }
        //            //Declined
        //            if (Status == "Declined")
        //            {
        //                AS = "D";
        //                BS = "D";
        //                ViewBag.status = Status;
        //            }
        //            ResultOpenListView = "select a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId, cast(a.ToSlotMasterId as time) as ToSlotMasterId,cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff,CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount, a.CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id = b.catgtypeId join ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId join cctsp_user d on a.client_userid = d.UserId join cctsp_user e on e.UserId = a.Emp_UserId where (a.BookedStatus = '" + BS + "')   and(a.ActiveStatus ='" + AS + "') and a.SchId = " + schlId + " and d.RoleId = 4 and e.RoleId = 3 order by cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
        //            ViewBag.AllData = SPA.Database.SqlQuery<Models.PendingApproval>(ResultOpenListView).ToList();
        //        }
        //        var shopinfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
        //        #region Language
        //        var Language = new List<Language_Label_Detail>();
        //        Language.AddRange(fu.getLanguageData("Reservation_Open_List", 0, shopinfo.Lang_id.Value));
        //        Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Common" && (c.Order_id == 1 || c.Order_id == 2) && c.Lang_id == shopinfo.Lang_id).ToList());
        //        Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == shopinfo.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander").ToList());
        //        ViewBag.OpenLang = Language;
        //        #endregion
        //        ViewBag.currency = fu.currency(shopinfo.Currency);
        //        ViewBag.DoctorPage = shopinfo.SchlStudentStrength;
        //        return View();
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return RedirectToAction("Error_List", "Error");
        //    }
        //}
        #endregion
        [CustomAutohrize(null)]
        public ActionResult OpenListView(string Status, string OrderBy, int? Sorting)
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, SchlStudentStrength = c.SchlStudentStrength, Color_Id = c.Color_Id, Currency = c.Currency }).FirstOrDefault();
                #region Variable
                //AS is For ActiveStatus
                string AS = "";
                //BS is for BookedStatus
                string BS = "";
                var AccessTab = "";
                var SubAccess = new Models.TabInfo();
                var Desc = "";
                string ResultOpenListView = "";
                if (string.IsNullOrEmpty(OrderBy))
                    OrderBy = "cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
                if (OrderBy == "Date")
                    OrderBy = "cast(a.BookingDate as date)";
                if (Sorting == 1)
                    Desc = "DESC";
                #endregion
                #region Language
                var Language = new List<Language_Label_Detail>();
                Language.AddRange(fu.getLanguageData("Reservation_Open_List", 0, ShopInfo.Lang_id.Value));
                Language.AddRange(SPA.Language_Label_Detail.Where(c => ((c.Page_Name == "Common" && (c.Order_id == 1 || c.Order_id == 2)) || (c.Order_id >= 8 && c.Page_Name == "Small_calander")) && c.Lang_id == ShopInfo.Lang_id).ToList());
                //Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == ShopInfo.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander").ToList());
                ViewBag.OpenLang = Language;
                ViewBag.Url = "openlistview";
                #endregion
                #region Logic
                if (Status == "Pending" || Status == null)
                {
                    ViewBag.status = "Pending";                   
                    SubAccess = fu.AccessDetails(Convert.ToInt32(Session["RoleId"].ToString()), "Open List View");
                    //string data = "select a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName,e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc,cast(c.Duration as int) as Duration,a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId, cast(a.ToSlotMasterId as time) as ToSlotMasterId,cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff,CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount, a.CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone ,StDoctNotes=(select count(y.Prescription_DetailId) + count(z.Medicine_Id) from Prescription_Master x, Prescription_Detail y, Medicine_Master z where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 1 and y.Prescription_Id = x.Prescription_Id and z.Prescription_Id = y.Prescription_Id and z.BookingId = y.BookingId and z.ActiveStatus = 'A'), StPrescription = (select count(y.Prescription_DetailId) + count(z.Medicine_Id) from Prescription_Master x,Prescription_Detail y, Medicine_Master z  where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 2 and y.Prescription_Id = x.Prescription_Id and z.Prescription_Id = y.Prescription_Id and z.BookingId = y.BookingId and z.ActiveStatus = 'A') ,StMerge=(select count(y.Prescription_DetailId) + count(z.Medicine_Id) from Prescription_Master x, Prescription_Detail y, Medicine_Master z where x.BookingId = a.EmpSchDetailsId and y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' and x.ActiveStatus = 'A' and x.diff = 3 and y.Prescription_Id = x.Prescription_Id and z.Prescription_Id = y.Prescription_Id and z.BookingId = y.BookingId and z.ActiveStatus = 'A') from spa_employeeScheduler a join cctsp_categoryDetails b on a.product_id = b.catgtypeId join ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId join cctsp_user d on a.client_userid = d.UserId join cctsp_user e on e.UserId = a.Emp_UserId where (a.BookedStatus = 'B' or a.BookedStatus = 'Z')   and(a.ActiveStatus = 'A' or a.ActiveStatus = 'Z') and a.SchId = " + schlId + " and d.RoleId = 4 and e.RoleId = 3 order by cast(a.BookingDate as date) ,cast(a.FromSlotMasterId as time)";
                    ResultOpenListView = " SELECT a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status," +
                    " e.firstName as EmpfirstName,e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc as CatgDesc," +
                    " cast(c.Duration as int) as Duration,  a.BookingDate," +
                    " cast(a.FromSlotMasterId as time) as FromSlotMasterId," +
                    " cast(a.ToSlotMasterId as time) as ToSlotMasterId," +
                    " cast((datediff(minute, cast(a.FromSlotMasterId as time)," +
                    " cast(a.ToSlotMasterId as time))) as int) as diff," +
                    " CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1" +
                    " THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount," +
                    " a.CreatedOn as CreatedOn ,d.firstname as CustomerFirstName,d.lastName as CustomerLastName," +
                    " d.phoneNo as CustomerPhone,  g.AccessToData ,g.insertAccess,g.DeleteAccess,g.UpdateAccess," +
                    " f.ItenName as FlowStatus," +
                    " stMerge=ISNULL((select top 1 case when y.prescription_Id is not null or " +
                    " z.prescription_Id is not null then 1 else 0 end " +
                    " from Prescription_Master x" +
                    " LEFT JOIN Prescription_Detail y on y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A'" +
                    " and y.Prescription_Id = x.Prescription_Id" +
                    " LEFT JOIN Medicine_Master z on z.BookingId = a.EmpSchDetailsId and z.ActiveStatus = 'A'" +
                    " and z.Prescription_Id = x.Prescription_Id" +
                    " where x.BookingId = a.EmpSchDetailsId and x.ActiveStatus = 'A' and x.diff = j.SchlStudentStrength),0)" +
                    " from spa_employeeScheduler a" +
                    " JOIN cctsp_categoryDetails b on a.product_id = b.catgtypeId" +
                    " JOIN ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId" +
                    " JOIN cctsp_user d on a.client_userid = d.UserId" +
                    " JOIN cctsp_user e on e.UserId = a.Emp_UserId" +
                    " JOIN ctsp_SolutionMaster f on f.SectionName = 'Open List View' and f.ActiveStatus = 'A'" +
                    " JOIN cctsp_user h on h.UserId =" + UserId +
                    " JOIN cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236) and i.ActiveStatus = 'A'" +
                    " JOIN cctsp_RoleDetails g on g.RoleId = i.RoleId and g.ActiveStatus = 'A'" +
                    " and f.SolutionId = convert(bigint, g.AccesstoSub)  and g.SchId = i.Schlid" +
                    " and f.SolutionId = convert(bigint, g.AccesstoSub)" +
                    " and((g.AccessToData = 'Own' and a.Emp_UserId = h.UserId) or(g.AccessToData != 'Own'))" +
                    " join cctsp_SchoolMaster j on j.SchlId = a.SchId and j.ActiveStatus = 'A'" +
                    " where (a.BookedStatus = 'B' or a.BookedStatus = 'Z') and " +
                    " (a.ActiveStatus = 'A' or a.ActiveStatus = 'Z')" +
                    " and a.SchId = " + schlId + " and d.RoleId = 4 and e.RoleId not in(1,4) " +
                    " GROUP BY a.Client_UserId,a.BookedStatus,a.ActiveStatus,a.Comment,a.reg_Status,e.firstName," +
                    " e.lastName,a.EmpSchDetailsId,b.CatgDesc,c.Duration,a.BookingDate,a.FromSlotMasterId," +
                    " a.ToSlotMasterId,a.Product_price,a.CreatedOn,d.firstname,d.lastName,d.phoneNo,g.AccessToData," +
                    " g.insertAccess,g.DeleteAccess,g.UpdateAccess,f.ItenName,j.SchlStudentStrength" +
                    " order by " + OrderBy + " " + Desc;
                    ViewBag.AllData = SPA.Database.SqlQuery<Models.PendingApproval>(ResultOpenListView).ToList();
                }
                else
                {
                    if (Status == "Appointment Closed")
                    {
                        AS = "C";
                        BS = "C";
                        ViewBag.status = Status;
                        ViewBag.Url = "Appclosed";
                        AccessTab = "AppClosed";
                    }
                    //Declined
                    if (Status == "Declined")
                    {
                        AS = "D";
                        BS = "D";
                        ViewBag.status = Status;
                        AccessTab = "Declined";
                    }
                    SubAccess = fu.AccessDetails(Convert.ToInt32(Session["RoleId"].ToString()), AccessTab);
                    ResultOpenListView = " select a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status,e.firstName as EmpfirstName," +
                                         " e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc as CatgDesc,cast(c.Duration as int) as Duration, " +
                                         " a.BookingDate,cast(a.FromSlotMasterId as time) as FromSlotMasterId, cast(a.ToSlotMasterId as time) as ToSlotMasterId," +
                                         " cast((datediff(minute,cast(a.FromSlotMasterId as time),cast(a.ToSlotMasterId as time))) as int) as diff," +
                                         " CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount, " +
                                         " a.CreatedOn as CreatedOn,d.firstname as CustomerFirstName,d.lastName as CustomerLastName,d.phoneNo as CustomerPhone ," +
                                         " g.AccessToData ,g.insertAccess,g.DeleteAccess,g.UpdateAccess ,f.ItenName as FlowStatus, " +
                                         " stMerge=ISNULL((select top 1 case when y.prescription_Id is not null or "+ 
                                         " z.prescription_Id is not null then 1 else 0 end "+
                                         " from Prescription_Master x "+
                                         " LEFT JOIN Prescription_Detail y on y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A' "+
                                         " and y.Prescription_Id = x.Prescription_Id "+
                                         " LEFT JOIN Medicine_Master z on z.BookingId = a.EmpSchDetailsId "+
                                         " and z.ActiveStatus = 'A' and z.Prescription_Id = x.Prescription_Id "+
                                         " where x.BookingId = a.EmpSchDetailsId and x.ActiveStatus = 'A' and x.diff = j.SchlStudentStrength),0) "+
                                         " from spa_employeeScheduler a " +
                                         " join cctsp_categoryDetails b on a.product_id = b.catgtypeId " +
                                         " join ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId " +
                                         " join cctsp_user d on a.client_userid = d.UserId " +
                                         " join cctsp_user e on e.UserId = a.Emp_UserId " +
                                         " join ctsp_SolutionMaster f on f.SectionName = '" + AccessTab + "' and f.ActiveStatus = 'A' " +
                                         " join cctsp_user h on h.UserId = " + UserId + " " +
                                         " join cctsp_Role i on i.Roleid=h.Roleid and (i.schlid=" + schlId + " or i.schlid=236) and i.ActiveStatus='A' " +
                                         " join cctsp_RoleDetails g on g.RoleId = i.RoleId and g.ActiveStatus = 'A' and f.SolutionId = convert(bigint, g.AccesstoSub) " +
                                         " and g.SchId = i.Schlid and f.SolutionId = convert(bigint, g.AccesstoSub) and " +
                                         " ((g.AccessToData = 'Own' and a.Emp_UserId = h.UserId) or(g.AccessToData != 'Own'))" +
                                         " join cctsp_SchoolMaster j on j.SchlId = a.SchId and j.ActiveStatus = 'A' "+
                                         " where (a.BookedStatus = '" + BS + "')   and(a.ActiveStatus ='" + AS + "') and a.SchId = " + schlId + " " +
                                         " and d.RoleId = 4 and e.RoleId not in (1,4)";
                    ResultOpenListView = ResultOpenListView + "order by " + OrderBy + " " + Desc;
                    ViewBag.AllData = SPA.Database.SqlQuery<Models.PendingApproval>(ResultOpenListView).ToList();
                }
                #endregion
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.DoctorPage = ShopInfo.SchlStudentStrength;
                ViewBag.status = Status;
                if (OrderBy == "cast(a.BookingDate as date)")
                    OrderBy = "Date";
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
        [CustomAutohrize(null)]
        public ActionResult AppointMentClosed()
        {
            try
            {
                return View("OpenListView", new { Status = "Appointment Closed" });
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult Declined()
        {
            try
            {
                return Redirect("/Reservation/OpenListView?Status=Declined");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public string ReservationValidationCheck(long? id)
        {
            string Check = "";
            var Validationdata = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == id).FirstOrDefault();
            if (Validationdata != null)
            {
                var FromDate = DateTime.Parse(Validationdata.BookingDate + " " + Validationdata.FromSlotMasterId).ToString("yyyy-MM-dd hh:mm:ss");
                var ToDate = DateTime.Parse(Validationdata.BookingDate + " " + Validationdata.ToSlotMasterId).ToString("yyyy-MM-dd hh:mm:ss");
                var validationCheck = "select * from spa_employeeScheduler where schId=" + schlId + "  and ((cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) <= '" + FromDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + FromDate + "') or (cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) <= '" + ToDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + ToDate + "')) and ((ActiveStatus = 'DA' and BookedStatus = 'B') or(ActiveStatus = 'A' and BookedStatus = 'B') or(ActiveStatus = 'C' and BookedStatus = 'C')) and Emp_UserId = " + Validationdata.EMP_UserId;
                var Checkdata = SPA.Database.SqlQuery<SPA_EmployeeScheduler>(validationCheck).ToList();
                if (Checkdata.Count > 0)
                    Check = "AlreadyRegistered";
                else
                {
                    var ChkShopLeaves = "select top 1* from SPA_LeaveMaster where schId=" + schlId + " and activeStatus='A' and (HolidayTypeId=11154 or UserId=" + Validationdata.EMP_UserId + ") and convert(date, '" + DateTime.Parse(Validationdata.BookingDate).ToString("yyyy/MM/dd") + "') between startdate and enddate";
                    var LeavesList = SPA.Database.SqlQuery<SPA_LeaveMaster>(ChkShopLeaves).FirstOrDefault();
                    if (LeavesList != null)
                        Check = "Leaves";
                }
            }
            return Check;
        }
        [CustomAutohrize(null)]
        public ActionResult Month()
        {
            try
            {
                //string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId join ctsp_solutionMaster c on c.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName " +
                                      "from SPA_SchedulerSlotMaster a " +
                                      " join cctsp_user b on a.UserId = b.UserId " +
                                      " join ctsp_solutionMaster c on c.UserId = b.UserId " +
                                      " join ctsp_SolutionMaster d on d.SectionName = 'Calendar Month' and d.ActiveStatus = 'A' " +
                                      " join cctsp_User e on e.UserId = " + UserId + " " +
                                      " join cctsp_Role g on g.RoleId=e.Roleid and (g.Schlid=" + schlId + " or g.Schlid=236) and g.ActiveStatus='A'" +
                                       " join cctsp_RoleDetails f on f.RoleId = g.RoleId and f.ActiveStatus = 'A' and d.SolutionId = convert(bigint, f.AccesstoSub) " +
                                       " and f.SchId =g.Schlid and d.SolutionId = convert(bigint, f.AccesstoSub) and " +
                                       " ((f.AccessToData = 'Own' and b.UserId = e.UserId) or(f.AccessToData != 'Own')) " +
                                      " where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' " +
                                      " and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 " +
                                      " group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                var UserData = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(UserDataquery).ToList();
                ViewBag.UserData = UserData;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Calender_view" && c.Lang_id == LanguageId).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    Value = c.Value,
                    English_Label = c.English_Label,
                    Order_id = c.Order_id,
                    Page_Name = c.Page_Name
                }).ToList();
                ViewBag.CalenderLanguage = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Small_calander" && c.Lang_id == LanguageId).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    Value = c.Value,
                    English_Label = c.English_Label,
                    Order_id = c.Order_id,
                    Page_Name = c.Page_Name
                }).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult BlockNewCalenderMonthDetail(long? UserId, string Monthname, string yearname)
        {
            try
            {
                long LoginUserId = Convert.ToInt32(Session["UserId"].ToString());
                string queryWeekdaysBlock = "select a.UserId,a.weekDay,max(a.EndTime)as MaxTime,min(a.StartTime) as MinTime,b.SlotDue, " +
                                             "j.AccessToData,g.ItenName as FlowStatus,j.insertAccess,j.DeleteAccess,j.UpdateAccess " +
                                            "from SPA_EmployeeSchedulers a " +
                                            "join SPA_SchedulerSlotMaster b on a.UserId=b.UserId  " +
                                            "join cctsp_schedulerMaster c on c.SchedulerId=b.SchedulerId  " +
                                            "join cctsp_user d on d.UserId=a.UserId  " +
                                            "join ctsp_SolutionMaster g on g.SectionName = 'Calendar Month' and g.Activestatus='A' " +
                                            "join cctsp_user h on h.UserId = " + LoginUserId + " " +
                                            "join cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236)  " +
                                            "and i.ActiveStatus = 'A' " +
                                            "join cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                                            "and j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub)  " +
                                            "and j.SchId = i.Schlid " +
                                            "and((j.AccessToData = 'Own' and a.UserId = h.UserId) or(j.AccessToData != 'Own')) " +
                                            "where a.SchId=" + schlId + " and a.WeekDay=c.weekday and d.activeStatus='A' " +
                                            "and a.activestatus='A' and b.ActiveStatus='A' and c.ActiveStatus = 'A' and a.UserId = " + UserId + " " +
                                            "and b.schId=" + schlId + " and d.schId=" + schlId + " " +
                                            "group by a.weekDay,b.slotDue,a.UserId ,j.AccessToData,g.ItenName ,j.insertAccess,j.DeleteAccess,j.UpdateAccess";
                var WeekdaysBlockList = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(queryWeekdaysBlock).ToList();
                ViewBag.WeekdaysBlockList = WeekdaysBlockList;
                ViewBag.AllowPastDate = fu.ShopInfo().Allow_PastDate;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult BlockWeekDetails(long? UserId)
        {
            try
            {
                long LoginUserId = Convert.ToInt32(Session["UserId"].ToString());
                string queryWeekdays = "select a.UserId,a.weekDay,max(a.EndTime)as MaxTime,min(a.StartTime) as MinTime,b.SlotDue, " +
                                       "j.AccessToData,g.ItenName as FlowStatus,j.insertAccess,j.DeleteAccess,j.UpdateAccess " +
                                       "from SPA_EmployeeSchedulers a " +
                                       "join SPA_SchedulerSlotMaster b on a.UserId=b.UserId  " +
                                       "join cctsp_schedulerMaster c on c.SchedulerId=b.SchedulerId  " +
                                       "join cctsp_user d on d.UserId=a.UserId  " +
                                       "join ctsp_SolutionMaster g on g.SectionName = 'Calendar Month' and g.Activestatus='A' " +
                                       "join cctsp_user h on h.UserId = " + LoginUserId + " " +
                                       "join cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236)  " +
                                       "and i.ActiveStatus = 'A' " +
                                       "join cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                                       "and j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub)  " +
                                       "and j.SchId = i.Schlid " +
                                       "and((j.AccessToData = 'Own' and a.UserId = h.UserId) or(j.AccessToData != 'Own')) " +
                                       "where a.SchId=" + schlId + " and a.WeekDay=c.weekday and d.activeStatus='A' " +
                                       "and a.activestatus='A' and b.ActiveStatus='A' and c.ActiveStatus = 'A' and a.UserId = " + UserId + " " +
                                       "and d.schId=" + schlId + " and b.schId=" + schlId + " " +
                                       "group by a.weekDay,b.slotDue,a.UserId ,j.AccessToData,g.ItenName ,j.insertAccess,j.DeleteAccess,j.UpdateAccess";
                var WeekBlockList = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(queryWeekdays).ToList();
                ViewBag.WeekBlockList = WeekBlockList;
                ViewBag.AllowPastDate = fu.ShopInfo().Allow_PastDate;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult BlockDayDetail(long? UserId, string GetDate)
        {
            try
            {
                Sql sql = new Sql();
                int pMonth = Convert.ToInt32(GetDate.Split('-')[1]);
                int pdate = Convert.ToInt32(GetDate.Split('-')[0]);
                int pyear = Convert.ToInt32(GetDate.Split('-')[2]);
                var SetDay = new DateTime(pyear, pMonth, pdate);
                string pday = SetDay.DayOfWeek.ToString();
                long LoginUserId = Convert.ToInt32(Session["UserId"].ToString());
                var DayBlockList = new List<Models.WeekDayBlockDetail>();
                var count = SPA.SPA_EmployeeSchedulers.Where(c => c.ActiveStatus == "A" && c.WeekDay == pday && c.UserId == UserId).Count();
                if (count > 0)
                    DayBlockList = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(sql.BlockDay(UserId, LoginUserId, schlId, pday)).ToList();
                else
                    DayBlockList = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(sql.BlockDay(UserId, schlId, LoginUserId)).ToList();
                ViewBag.DayBlockList = DayBlockList;
                ViewBag.AllowPastDate = fu.ShopInfo().Allow_PastDate;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult BlockAllEmployeeDayDetail(string GetDate)
        {
            try
            {
                int pMonth = Convert.ToInt32(GetDate.Split('-')[1]);
                int pdate = Convert.ToInt32(GetDate.Split('-')[0]);
                int pyear = Convert.ToInt32(GetDate.Split('-')[2]);
                var SetDay = new DateTime(pyear, pMonth, pdate);
                string pday = SetDay.DayOfWeek.ToString();
                ViewBag.pday = pday;
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //string UserDataquery = "select a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId join ctsp_solutionMaster c on c.UserId = b.UserId join CCTSP_SchedulerMaster d on a.SchedulerId = d.SchedulerId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 group by a.UserId,a.slotDue,b.firstName,b.lastName";
                //string UserDataquery = "select a.UserId,max(a.EndTime)as MaxTime,min(a.StartTime) as MinTime,b.SlotDue,d.firstName,d.lastName from SPA_EmployeeSchedulers a join SPA_SchedulerSlotMaster b on a.UserId=b.UserId join cctsp_schedulerMaster c on c.SchedulerId=b.SchedulerId join cctsp_user d on d.UserId=a.UserId join ctsp_solutionMaster e on e.UserId=d.UserId where a.SchId=" + schlId + " and a.WeekDay=c.weekday and e.activestatus='A' and e.catgTypeId = 11145 and d.activeStatus='A' and a.activestatus='A' and b.activestatus='A' and d.schId=" + schlId + " group by a.UserId,b.slotDue,d.firstName,d.lastName";
                string UserDataquery = " select a.UserId,max(a.EndTime)as MaxTime,min(a.StartTime) as MinTime,b.SlotDue,d.firstName,d.lastName," +
                                       " f.ItenName as FlowStatus,h.AccessToData" +
                                       " from SPA_EmployeeSchedulers a " +
                                       " join cctsp_schedulerMaster c on c.ActiveStatus='A' and c.SchId=a.SchId" +
                                       " join SPA_SchedulerSlotMaster b on a.UserId = b.UserId and SlotMasterId=(select top 1SlotMasterId from SPA_SchedulerSlotMaster where SchId=a.SchId and UserId=a.UserId and SchedulerId=c.SchedulerId and ActiveStatus='A')" +
                                       " join cctsp_user d on d.UserId = a.UserId " +
                                       " join ctsp_solutionMaster e on e.UserId = d.UserId " +
                                       " join ctsp_SolutionMaster f on f.solutionId = (select top 1solutionId" +
                                       " from ctsp_SolutionMaster where SectionName = 'Calendar Month' and ActiveStatus = 'A')" +
                                       " join cctsp_User g on g.UserId = " + UserId + " " +
                                       " join cctsp_Role i on i.RoleId=g.Roleid and (i.Schlid=" + schlId + " or i.Schlid=236) and i.ActiveStatus='A'" +
                                       " join cctsp_RoleDetails h on h.RoleId = i.RoleId and h.ActiveStatus = 'A' and f.SolutionId = convert(bigint, h.AccesstoSub) " +
                                       " and h.SchId = i.Schlid and convert(bigint, h.AccesstoSub)=f.SolutionId " +
                                       " where a.SchId = " + schlId + " and a.WeekDay = c.weekday and e.activestatus = 'A' and e.catgTypeId = 11145 " +
                                       " and d.activeStatus = 'A' and a.activestatus = 'A' and b.activestatus = 'A' and d.schId = " + schlId +
                                       " group by a.UserId,b.slotDue,d.firstName,d.lastName,h.AccessToData,g.UserId,f.Itenname " +
                                       " having ((h.AccessToData = 'Own' and a.UserId = g.UserId) or(h.AccessToData != 'Own'))";
                //"  ,f.ItenName,h.insertAccess,h.AccessToData,h.DeleteAccess,h.UpdateAccess";
                var Employeeinfo = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(UserDataquery).ToList();
                ViewBag.Employeeinfo = Employeeinfo;
                ViewBag.AllowPastDate = fu.ShopInfo().Allow_PastDate;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }

        public JsonResult GetAllBookedData(long UserId, string Monthname, string yearname, string Day, string FirstDate, string Finaldate, string Status)
        {
            try
            {
                string EmployeeBooking = "";
                string queryWeekdaysBlock = "";
                string employeeleaves = "";
                string NotAvilable = "";
                string QuickBlocking = "";
                string Fdate = "";
                string sdate = "";
                if (FirstDate != null && Finaldate != null && FirstDate != "" && Finaldate != "")
                {
                    DateTime fdt = Convert.ToDateTime(FirstDate, enGB);
                    DateTime sdt = Convert.ToDateTime(Finaldate, enGB);
                    Fdate = fdt.ToString("dd MMMM, yyyy", enGB);
                    sdate = sdt.ToString("dd MMMM, yyyy", enGB);
                }
                //When User is there
                if (UserId != 0)
                {
                    // EmployeeBooking = "select concat(datepart(dd,a.bookingdate),' ',CONVERT(nvarchar,a.fromslotmasterid),' ',CONVERT(nvarchar,a.toslotmasterid),' ',RTrim(a.ActiveStatus),' ',b.gender,'<br>',b.firstname,'<br>',b.lastname,' ',a.EmpSchdetailsId) as ConcateDate,b.firstName,datepart(dd,a.bookingdate) as date,datename(mm,a.BookingDate) as Month,datepart(yyyy,a.bookingdate) as Year,a.bookingdate,a.fromslotmasterid,a.toslotmasterid,a.emp_userid,a.client_userid,a.product_id,a.product_price from spa_employeeScheduler a join cctsp_user b on a.Client_UserId=b.UserId where a.EMP_UserId = '" + UserId + "' and datename(mm, cast(a.BookingDate as date))= '" + Monthname + "' and ((a.BookedStatus = 'B' and a.ActiveStatus = 'A') or (a.BookedStatus = 'B' and a.ActiveStatus = 'DA') or (a.BookedStatus = 'C' and a.ActiveStatus = 'C')) and datepart(yyyy, cast(a.BookingDate as date))= '" + yearname + "' and a.schid='" + schlId + "'";
                    EmployeeBooking = "select concat(datepart(dd,a.bookingdate),' ',CONVERT(nvarchar,a.fromslotmasterid),' ',CONVERT(nvarchar,a.toslotmasterid),' ',RTrim(a.ActiveStatus),' ',b.gender,'<br>',replace(b.firstname,' ',''),'<br>',replace(b.lastname,' ',''),'<br>',replace(c.catgdesc,' ','*'),' ',a.EmpSchdetailsId) as ConcateDate,b.firstName,datepart(dd,a.bookingdate) as date,datename(mm,a.BookingDate) as Month,datepart(yyyy,a.bookingdate) as Year,a.bookingdate,a.fromslotmasterid,a.toslotmasterid,a.emp_userid,a.client_userid,a.product_id,a.product_price from spa_employeeScheduler a join cctsp_user b on a.Client_UserId=b.UserId join  cctsp_categorydetails c  on a.Product_Id=c.catgtypeid where a.EMP_UserId = '" + UserId + "' and datename(mm, cast(a.BookingDate as date))= '" + Monthname + "' and ((a.BookedStatus = 'B' and a.ActiveStatus = 'A') or (a.BookedStatus = 'B' and a.ActiveStatus = 'DA') or (a.BookedStatus = 'C' and a.ActiveStatus = 'C')) and datepart(yyyy, cast(a.BookingDate as date))= '" + yearname + "' and a.schid='" + schlId + "'";
                    queryWeekdaysBlock = "select a.UserId,b.weekDay,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,concat(a.UserId,' ',REPLACE(REPLACE( a.SlotDue, ' Minutes', '' ),' Minute','')) as SlotDue from SPA_SchedulerSlotMaster a join CCTSP_SchedulerMaster b on a.SchedulerId = b.SchedulerId  where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and a.UserId = " + UserId + " group by b.WeekDay,a.UserId,a.slotDue";
                    //employeeleaves = "select concat(datepart(dd,b.startdate),' ',datepart(dd,b.enddate)) as DateConcat,b.userid,a.schid,a.roleid,a.firstname,a.lastname,b.startdate,b.enddate,b.holidaydesc,b.calendarname from cctsp_user a join Spa_leaveMaster b on a.userid = b.userid where a.Roleid not in (4) and a.schid = '" + schlId + "' and b.schid ='" + schlId + "' and a.activestatus = 'A' and b.activestatus = 'A' and(b.UserId = '" + UserId + "' or b.UserId in (select userid from cctsp_user where roleid = 1 and activestatus = 'A'))and (datename(mm,b.StartDate)= '" + Monthname + "'or datename(mm,b.EndDate)='" + Monthname + "') and  (Datepart(yyyy,b.StartDate)='" + yearname + "' or Datepart(yyyy,b.EndDate)='" + yearname + "')";
                    employeeleaves = "select (SELECT concat(min(datestring),' ',max(datestring)) FROM dbo.DateRange_To_Table ((format(b.StartDate,'dd/MM/yyyy')),(format(b.enddate,'dd/MM/yyyy')),'" + Monthname + "')) as DateConcat,b.userid,a.schid,a.roleid,a.firstname,a.lastname,b.startdate,b.enddate,b.holidaydesc,b.calendarname from cctsp_user a join Spa_leaveMaster b on a.userid = b.userid where a.Roleid not in (4) and a.schid = '" + schlId + "' and b.schid ='" + schlId + "' and a.activestatus = 'A' and b.activestatus = 'A' and(b.UserId = '" + UserId + "' or b.UserId in (select userid from cctsp_user where roleid = 1 and activestatus = 'A'))and (datename(mm,b.StartDate)= '" + Monthname + "'or datename(mm,b.EndDate)='" + Monthname + "') and  (Datepart(yyyy,b.StartDate)='" + yearname + "' or Datepart(yyyy,b.EndDate)='" + yearname + "')";
                    //NotAvilable = "select concat(CONVERT(varchar,Min(a.starttime),120), ' ',CONVERT(varchar,Max(a.starttime),120))as SlotDue, b.weekDay from SPA_SchedulerSlotMaster a join cctsp_schedulerMaster b on a.schedulerId = b.schedulerId where a.UserId = " + UserId + " and a.Activestatus = 'A' and a.schId=" + schlId + " group by b.weekday";
                    NotAvilable = "select concat(CONVERT(varchar,Min(a.starttime),120), ' ',CONVERT(varchar,Max(a.EndTime),120)) as SlotDue ,a.weekDay from SPA_EmployeeSchedulers a join SPA_SchedulerSlotMaster b on a.UserId=b.UserId join cctsp_schedulerMaster c on c.SchedulerId=b.SchedulerId join cctsp_user d on d.UserId=a.UserId where a.SchId=" + schlId + " and a.WeekDay=c.weekday and d.activeStatus='A' and a.activestatus='A' and a.UserId = " + UserId + " and d.schId=" + schlId + " group by a.weekDay";
                    QuickBlocking = "select concat(datepart(dd,a.bookingdate),' ',CONVERT(nvarchar,a.fromslotmasterid),' ',CONVERT(nvarchar, a.toslotmasterid),' ',RTrim(a.ActiveStatus),' ',a.EmpSchdetailsId,' ',a.emp_userid,' ',CONVERT(DATE,a.bookingdate),' ',replace(a.Comment,' ','*'))  as ConcateDate from spa_employeeScheduler a where a.EMP_UserId =" + UserId + "  and datename(mm, cast(a.BookingDate as date))= '" + Monthname + "' and((a.BookedStatus = 'B' and a.ActiveStatus = 'A') or(a.BookedStatus = 'B' and a.ActiveStatus = 'DA') or(a.BookedStatus = 'C' and a.ActiveStatus = 'C'))  and datepart(yyyy, cast(a.BookingDate as date))= '" + yearname + "' and a.schid = '" + schlId + "'and a.Reg_Status = 'B'";
                }
                //When user might not be
                else
                {
                    // EmployeeBooking = "select concat(datepart(dd,a.bookingdate),' ',CONVERT(nvarchar,a.fromslotmasterid),' ',CONVERT(nvarchar,a.toslotmasterid),' ',RTrim(a.ActiveStatus),' ',b.gender,'<br>',b.firstname,'<br>',b.lastname,' ',a.EmpSchdetailsId,' ',a.emp_userid) as ConcateDate,b.firstName,datepart(dd,a.bookingdate) as date,datename(mm,a.BookingDate) as Month,datepart(yyyy,a.bookingdate) as Year,a.bookingdate,a.fromslotmasterid,a.toslotmasterid,a.emp_userid,a.client_userid,a.product_id,a.product_price from spa_employeeScheduler a join cctsp_user b on a.Client_UserId=b.UserId where a.EMP_UserId in (select a.UserId from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A')  and datename(mm, cast(a.BookingDate as date))= '" + Monthname + "' and ((a.BookedStatus = 'B' and a.ActiveStatus = 'A') or (a.BookedStatus = 'B' and a.ActiveStatus = 'DA') or (a.BookedStatus = 'C' and a.ActiveStatus = 'C')) and datepart(yyyy, cast(a.BookingDate as date))= '" + yearname + "' and a.schid='" + schlId + "'";
                    EmployeeBooking = "select concat(datepart(dd,a.bookingdate),' ',CONVERT(nvarchar,a.fromslotmasterid),' ',CONVERT(nvarchar,a.toslotmasterid),' ',RTrim(a.ActiveStatus),' ',b.gender,'<br>',replace(b.firstname,' ',''),'<br>',replace(b.lastname,' ',''),'<br>',replace(c.catgdesc,' ','*'),' ',a.EmpSchdetailsId,' ',a.emp_userid) as ConcateDate,b.firstName,datepart(dd,a.bookingdate) as date,datename(mm,a.BookingDate) as Month,datepart(yyyy,a.bookingdate) as Year,a.bookingdate,a.fromslotmasterid,a.toslotmasterid,a.emp_userid,a.client_userid,a.product_id,a.product_price from spa_employeeScheduler a join cctsp_user b on a.Client_UserId=b.UserId join  cctsp_categorydetails c  on a.Product_Id=c.catgtypeid where a.EMP_UserId in (select a.UserId from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId  where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A')  and datename(mm, cast(a.BookingDate as date))= '" + Monthname + "' and ((a.BookedStatus = 'B' and a.ActiveStatus = 'A') or (a.BookedStatus = 'B' and a.ActiveStatus = 'DA') or (a.BookedStatus = 'C' and a.ActiveStatus = 'C')) and datepart(yyyy, cast(a.BookingDate as date))= '" + yearname + "' and a.schid='" + schlId + "'";
                    queryWeekdaysBlock = "select a.UserId,concat(b.weekDay,' ', a.UserId) as weekDay,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,concat(a.UserId,' ',REPLACE(REPLACE( a.SlotDue, ' Minutes', '' ),' Minute','')) as SlotDue from SPA_SchedulerSlotMaster a join CCTSP_SchedulerMaster b on a.SchedulerId = b.SchedulerId  where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and a.UserId in (select a.UserId from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A') group by b.WeekDay,a.UserId,a.slotDue";
                    employeeleaves = "select (SELECT concat(min(datestring),' ',max(datestring),' ',b.userid,' ',a.roleid) FROM dbo.DateRange_To_Table ((format(b.StartDate,'dd/MM/yyyy')),(format(b.enddate,'dd/MM/yyyy')),'" + Monthname + "')) as DateConcat,b.userid,a.schid,a.roleid,a.firstname,a.lastname,b.startdate,b.enddate,b.holidaydesc,b.calendarname from cctsp_user a join Spa_leaveMaster b on a.userid = b.userid where a.Roleid not in (4) and a.schid = '" + schlId + "' and b.schid ='" + schlId + "' and a.activestatus = 'A' and b.activestatus = 'A' and(b.UserId in ((select a.UserId from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A')) or b.UserId in (select userid from cctsp_user where roleid = 1 and activestatus = 'A'))and (datename(mm,b.StartDate)= '" + Monthname + "'or datename(mm,b.EndDate)='" + Monthname + "') and  (Datepart(yyyy,b.StartDate)='" + yearname + "' or Datepart(yyyy,b.EndDate)='" + yearname + "')";
                    // employeeleaves = "select concat(datepart(dd,b.startdate),' ',datepart(dd,b.enddate),' ',b.userid,' ',a.roleid) as DateConcat,b.userid,a.schid,a.roleid,a.firstname,a.lastname,b.startdate,b.enddate,b.holidaydesc,b.calendarname from cctsp_user a join Spa_leaveMaster b on a.userid = b.userid where a.Roleid not in (4) and a.schid = '" + schlId + "' and b.schid ='" + schlId + "' and a.activestatus = 'A' and b.activestatus = 'A' and(b.UserId in ((select a.UserId from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A')) or b.UserId in (select userid from cctsp_user where roleid = 1 and activestatus = 'A'))and (datename(mm,b.StartDate)= '" + Monthname + "'or datename(mm,b.EndDate)='" + Monthname + "') and  (Datepart(yyyy,b.StartDate)='" + yearname + "' or Datepart(yyyy,b.EndDate)='" + yearname + "')";
                    NotAvilable = "select concat(min(CONVERT(varchar,a.starttime,120)),' ',max(CONVERT(varchar,a.EndTime,120)))as SlotDue,concat(a.weekDay,' ',b.UserId) as weekDay from SPA_EmployeeSchedulers a join cctsp_User b on a.UserId=b.UserId   where a.schId=" + schlId + " and b.schId=" + schlId + " and a.ActiveStatus='A' and b.ActiveStatus='A' group by a.weekDay,b.UserId order by b.UserId";
                    QuickBlocking = "select concat(datepart(dd,a.bookingdate),' ',CONVERT(nvarchar,a.fromslotmasterid),' ',CONVERT(nvarchar, a.toslotmasterid),' ',RTrim(a.ActiveStatus),' ',a.EmpSchdetailsId,' ',a.emp_userid,' ',CONVERT(DATE,a.bookingdate),' ',replace(a.Comment,' ','*'))  as ConcateDate from spa_employeeScheduler a where a.EMP_UserId in (select a.UserId from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId  where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A')  and datename(mm, cast(a.BookingDate as date))= '" + Monthname + "' and((a.BookedStatus = 'B' and a.ActiveStatus = 'A') or(a.BookedStatus = 'B' and a.ActiveStatus = 'DA') or(a.BookedStatus = 'C' and a.ActiveStatus = 'C'))  and datepart(yyyy, cast(a.BookingDate as date))= '" + yearname + "' and a.schid = '" + schlId + "'and a.Reg_Status = 'B'";
                }
                var EmployeeBookinglist = SPA.Database.SqlQuery<Models.EmployeeBooking>(EmployeeBooking).ToList();
                var WeekdaysBlockList = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(queryWeekdaysBlock).ToList();
                var employeelivelist = SPA.Database.SqlQuery<Models.EmployeeLeaves>(employeeleaves).ToList();
                var NotAvailableList = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(NotAvilable).ToList();
                var QuickBloCkingList = SPA.Database.SqlQuery<Models.EmployeeBooking>(QuickBlocking).ToList();
                //Revenue Functionality
                #region Revenue
                string EmployeeRevenue = "";
                if (Status == "Daily")
                    EmployeeRevenue = "select cast(sum(cast(Product_price as float)) as nvarchar) as product_price from spa_employeeScheduler where schid=" + schlId + "and BookedStatus = 'C' and ActiveStatus = 'C' and datename(mm, cast(BookingDate as date))= '" + Monthname + "' and Datepart(dd, cast(BookingDate as date))='" + Day + "' and Datepart(yyyy, cast(BookingDate as date))='" + yearname + "' and Emp_userId='" + UserId + "'";
                if (Status == "Week")
                    EmployeeRevenue = "select cast(sum(cast(Product_price as float)) as nvarchar) as product_price from spa_employeeScheduler where schid=" + schlId + " and BookedStatus = 'C' and ActiveStatus = 'C' and cast(BookingDate as date) >= cast('" + Fdate + "' as date) and cast(BookingDate as date)<=cast('" + sdate + "' as date)  and Emp_userId='" + UserId + "'";
                if (Status == "AllEmployee")
                    EmployeeRevenue = "select cast(sum(cast(Product_price as float)) as nvarchar) as product_price from spa_employeeScheduler where schid=" + schlId + "and BookedStatus = 'C' and ActiveStatus = 'C' and datename(mm, cast(BookingDate as date))= '" + Monthname + "' and Datepart(dd, cast(BookingDate as date))='" + Day + "' and Datepart(yyyy, cast(BookingDate as date))='" + yearname + "'";
                if (Status == "Month")
                    EmployeeRevenue = "select cast(sum(cast(Product_price as float)) as nvarchar) as product_price from spa_employeeScheduler where schid=" + schlId + "and BookedStatus = 'C' and ActiveStatus = 'C' and datename(mm, cast(BookingDate as date))= '" + Monthname + "' and Datepart(yyyy, cast(BookingDate as date))='" + yearname + "' and Emp_userId='" + UserId + "'";
                var totalRevenue = SPA.Database.SqlQuery<Models.EmployeeBooking>(EmployeeRevenue).FirstOrDefault();
                string Currency = "";
                var currencyCheck = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Currency).FirstOrDefault();
                if (currencyCheck != null && currencyCheck != "")
                    Currency = currencyCheck;
                else
                    Currency = "CHF";
                string TotalRev = "0";
                if (totalRevenue.product_price != null)
                    TotalRev = totalRevenue.product_price + "*" + Currency;
                else
                    TotalRev = "0" + "*" + Currency;
                #endregion
                Models.GetAllBookedData AllDetails = new Models.GetAllBookedData()
                {
                    DataSlot = EmployeeBookinglist.Select(c => c.ConcateDate).ToList().ToArray(),
                    WeekdaysBlock = WeekdaysBlockList.Select(c => c.weekDay).Distinct().ToList().ToArray(),
                    employeelive = employeelivelist.Select(c => c.DateConcat).Distinct().ToList().ToArray(),
                    NotAvailableDay = NotAvailableList.Select(c => c.weekDay).Distinct().ToList().ToArray(),
                    NotAvailableTime = NotAvailableList.Select(c => c.SlotDue).ToList().ToArray(),
                    TimeSlotAvailable = WeekdaysBlockList.Select(c => c.SlotDue).Distinct().ToList().ToArray(),
                    QuickBlock = QuickBloCkingList.Select(c => c.ConcateDate).Distinct().ToList().ToArray(),
                    TotalRev = TotalRev
                };
                return Json(AllDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                JsonResult send = null;
                return send;
            }
        }
        [CustomAutohrize(null)]
        public ActionResult Day()
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId join ctsp_solutionMaster c on c.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' and c.ActiveStatus='A' and c.schId=" + schlId + " and c.catgTypeId=11145 group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName " +
                                       "from SPA_SchedulerSlotMaster a " +
                                       " join cctsp_user b on a.UserId = b.UserId " +
                                       " join ctsp_solutionMaster c on c.UserId = b.UserId " +
                                       " join ctsp_SolutionMaster d on d.SectionName = 'Calendar Month' and d.ActiveStatus = 'A' " +
                                       " join cctsp_User e on e.UserId = " + UserId + " " +
                                       " join cctsp_Role g on g.RoleId=e.Roleid and (g.Schlid=" + schlId + " or g.Schlid=236) and g.ActiveStatus='A'" +
                                       " join cctsp_RoleDetails f on f.RoleId = g.RoleId and f.ActiveStatus = 'A' and d.SolutionId = convert(bigint, f.AccesstoSub) " +
                                       " and f.SchId =g.Schlid and d.SolutionId = convert(bigint, f.AccesstoSub) and " +
                                       " ((f.AccessToData = 'Own' and b.UserId = e.UserId) or(f.AccessToData != 'Own')) " +
                                       " where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' " +
                                       " and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 " +
                                       " group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                var UserData = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(UserDataquery).ToList();
                ViewBag.DailyUserData = UserData;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Calender_view" && c.Lang_id == LanguageId).Select(c => new Models.LanguageLabelDetails
                {
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value
                }).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult Week()
        {
            try
            {
                //Query For Original Data
                //string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId join ctsp_solutionMaster c on c.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' and c.ActiveStatus='A' and c.schId=" + schlId + " and c.catgTypeId=11145 group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName " +
                                      "from SPA_SchedulerSlotMaster a " +
                                      " join cctsp_user b on a.UserId = b.UserId " +
                                      " join ctsp_solutionMaster c on c.UserId = b.UserId " +
                                      " join ctsp_SolutionMaster d on d.SectionName = 'Calendar Month' and d.ActiveStatus = 'A' " +
                                      " join cctsp_User e on e.UserId = " + UserId + " " +
                                       " join cctsp_Role g on g.RoleId=e.Roleid and (g.Schlid=" + schlId + " or g.Schlid=236) and g.ActiveStatus='A'" +
                                       " join cctsp_RoleDetails f on f.RoleId = g.RoleId and f.ActiveStatus = 'A' and d.SolutionId = convert(bigint, f.AccesstoSub) " +
                                       " and f.SchId =g.Schlid and d.SolutionId = convert(bigint, f.AccesstoSub) and " +
                                       " ((f.AccessToData = 'Own' and b.UserId = e.UserId) or(f.AccessToData != 'Own')) " +
                                      " where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' " +
                                      " and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 " +
                                      " group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                var UserData = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(UserDataquery).ToList();
                ViewBag.WeeklyUserData = UserData;
                //Langauge Translation
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Calender_view" && c.Lang_id == LanguageId).Select(c => new Models.LanguageLabelDetails
                {
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value
                }).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult CalendarMonth()
        {
            try
            {
                var TotalRevenue = SPA.SPA_EmployeeScheduler.Where(c => c.SchId == schlId && c.ActiveStatus == "c" && c.BookedStatus == "c" && c.Product_price != null).ToList().Select(c => float.Parse(c.Product_price)).Sum();
                ViewBag.TotalRevenue = TotalRevenue;
                string Employeelist = "select a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId join ctsp_solutionMaster c on c.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 group by a.UserId,a.slotDue,b.firstName,b.lastName";
                var EmployeeCount = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(Employeelist).ToList();
                ViewBag.Employeelist = EmployeeCount.Count();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopDetails
                {
                    LangId = c.Lang_id,
                    Currency = c.Currency
                }).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Calender_view" && c.Lang_id == ShopInfo.LangId).Select(c => new Models.LanguageLabelDetails
                {
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value
                }).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult QuickBlockPopup(long? UserId, string Status, int? ReservationId)
        {
            try
            {
                var schoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, TimeZone = c.TimeZone }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Quick_Block" && c.ActiveStatus == "A" && c.Lang_id == schoolInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Value = c.Value,
                    English_Label = c.English_Label
                }).ToList();
                var StartDate = fu.ZonalDate(schoolInfo.TimeZone);
                var ReservationInfo = new Models.EditQuickBlockDetails();
                if (ReservationId > 0)
                {
                    ReservationInfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).Select(c => new Models.EditQuickBlockDetails
                    {
                        BookingDate = c.BookingDate,
                        ReservationId = c.EmpSchDetailsId,
                        StartTime = c.FromSlotMasterId,
                        EndTime = c.ToSlotMasterId,
                        ShortLeaveComment = c.Comment
                    }).FirstOrDefault();
                    StartDate = DateTime.Parse(ReservationInfo.BookingDate, enGB);
                    ReservationInfo.BookingDate = StartDate.ToString("yyyy/MM/dd");
                }
                var WeekDay = StartDate.DayOfWeek.ToString();
                var BlockQuery = fu.QuickBlockQuery(WeekDay, UserId.Value, schlId, StartDate.ToString("yyyy/MM/dd"));
                ViewBag.UserId = UserId;
                ViewBag.ReservationInfo = ReservationInfo;
                if (BlockQuery == null)
                    BlockQuery = new quickBlockSlot();
                BlockQuery.ViewStatus = Status;
                return View(BlockQuery);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult QuickBlock(DateTime? StartDate, long? UserId, Models.quickBlockSlot BlockQuery, Models.EditQuickBlockDetails EditDetails)
        {
            try
            {
                var schoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, TimeZone = c.TimeZone }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Quick_Block" && c.ActiveStatus == "A" && c.Lang_id == schoolInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Value = c.Value,
                    English_Label = c.English_Label
                }).ToList();
                var WeekDay = "";
                if (StartDate != null)
                    WeekDay = StartDate.Value.DayOfWeek.ToString();
                var OutPut = BlockQuery;
                if (BlockQuery.StartTime == null && StartDate != null)
                    OutPut = fu.QuickBlockQuery(WeekDay, UserId.Value, schlId, StartDate.Value.ToString("yyyy/MM/dd"));
                Models.QuickBlockSlotDetails QuickDetails = new QuickBlockSlotDetails();
                if (OutPut != null)
                {
                    QuickDetails.Stepping = OutPut.SlotDue != null ? OutPut.SlotDue.Replace("Minutes", "").Replace("Minute", "") : "5";
                    if (OutPut.StartTime != null && QuickDetails.Stepping != null)
                        OutPut.StartTime = OutPut.StartTime.Value.Add(TimeSpan.FromMinutes(-Convert.ToInt32(QuickDetails.Stepping)));
                    QuickDetails.StartMin = OutPut.StartTime != null ? Convert.ToDateTime(Convert.ToString(OutPut.StartTime)).Minute : 0;
                    QuickDetails.StartHour = OutPut.StartTime != null ? Convert.ToDateTime(Convert.ToString(OutPut.StartTime)).Hour : 0;
                    QuickDetails.Endmin = OutPut.EndTime != null ? Convert.ToDateTime(Convert.ToString(OutPut.EndTime)).Minute : 0;
                    QuickDetails.EndHour = OutPut.EndTime != null ? Convert.ToDateTime(Convert.ToString(OutPut.EndTime)).Hour : 0;
                    if (OutPut.StartTime != null && QuickDetails.Stepping != null)
                        OutPut.EndTime = OutPut.StartTime.Value.Add(TimeSpan.FromMinutes(Convert.ToInt32(QuickDetails.Stepping)));
                    QuickDetails.EndStartMin = OutPut.EndTime != null ? Convert.ToDateTime(Convert.ToString(OutPut.EndTime)).Minute : 0;
                    QuickDetails.EndStartHour = OutPut.EndTime != null ? Convert.ToDateTime(Convert.ToString(OutPut.EndTime)).Hour : 0;
                    if (StartDate != null && OutPut.StartTime != null)
                    {
                        QuickDetails.StartDateTime = Convert.ToDateTime(StartDate.Value.ToString("yyyy/MM/dd") + " " + new DateTime(OutPut.StartTime.Value.Ticks).ToString("HH:mm")).AddMinutes(Convert.ToInt32(QuickDetails.Stepping)).ToString("yyyy/MM/dd HH:mm");
                        QuickDetails.startTime = new DateTime(OutPut.StartTime.Value.Ticks).AddMinutes(Convert.ToInt32(QuickDetails.Stepping)).ToString("HH:mm");
                        QuickDetails.EndTime = Convert.ToDateTime(QuickDetails.startTime).AddMinutes(Convert.ToInt32(QuickDetails.Stepping)).ToString("HH:mm");
                    }
                    else
                    {
                        var CurrentDate = fu.ZonalDate(schoolInfo.TimeZone);
                        QuickDetails.StartDateTime = CurrentDate.ToString("yyyy/MM/dd HH:mm");
                        QuickDetails.startTime = CurrentDate.ToString("HH:mm");
                        QuickDetails.EndTime = Convert.ToDateTime(QuickDetails.startTime).AddMinutes(Convert.ToInt32(QuickDetails.Stepping)).ToString("HH:mm");
                    }
                    if (EditDetails.StartTime != null && EditDetails.EndTime != null)
                    {
                        QuickDetails.EditStartTime = EditDetails.StartTime;
                        QuickDetails.EditEndTime = EditDetails.EndTime;
                    }
                }
                return View(QuickDetails);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [ValidateInput(false)]
        public void AddQuickBlockDetails(Models.QuickBlockDetails Details)
        {
            try
            {
                if (Details.BlockEndTime != null && Details.BlockStartTime != null && Details.BlockDatePicker != null)
                {

                    string StartDate = DateTime.Parse(Details.BlockDatePicker + " " + Details.BlockStartTime).ToString("yyyy-MM-dd HH:mm:ss");
                    string EndDate = DateTime.Parse(Details.BlockDatePicker + " " + Details.BlockEndTime).ToString("yyyy-MM-dd HH:mm:ss");
                    var StartTime = DateTime.Parse(Details.BlockStartTime).ToString("HH:mm:ss");
                    var EndTime = DateTime.Parse(Details.BlockEndTime).ToString("HH:mm:ss");
                    long UserId = Convert.ToInt64(Details.BlockUserId);
                    DateTime CurrentDate = fu.ZonalDate(null);

                    Session["View"] = Details.ViewStatus;
                    Session["ViewUserId"] = UserId.ToString();
                    var Bdate = Convert.ToDateTime(Details.BlockDatePicker, enGB);
                    Session["ViewDate"] = Bdate.Day + "-" + Bdate.AddMonths(-1).ToString("MM") + "-" + Bdate.Year;
                    if (Details.ReservationId == null)
                    {
                        var Check = fu.CheckReservationExistOrNot(StartDate, EndDate, UserId);
                        if (Check.ToLower() == "no")
                        {

                            spa_Master_Reservation MasterDetails = new spa_Master_Reservation();
                            MasterDetails.BookingDate = Bdate.ToString("dd MMMM, yyyy", enGB);
                            MasterDetails.StartTime = StartTime;
                            MasterDetails.EndTime = EndTime;
                            MasterDetails.ActiveStatus = "DA";
                            MasterDetails.ActiveStatus = "B";
                            MasterDetails.EmpId = UserId;
                            MasterDetails.created_date = CurrentDate;
                            MasterDetails.Updated_date = CurrentDate;
                            MasterDetails.Reg_Status = "B";
                            MasterDetails.Schlid = schlId;
                            MasterDetails.Comment = Details.ShortLeaveComment;
                            SPA.spa_Master_Reservation.Add(MasterDetails);
                            SPA.SaveChanges();
                            SPA_EmployeeScheduler BlockDetails = new SPA_EmployeeScheduler();
                            BlockDetails.BookingDate = Bdate.ToString("dd MMMM, yyyy", enGB);
                            BlockDetails.FromSlotMasterId = StartTime;
                            BlockDetails.ToSlotMasterId = EndTime;
                            BlockDetails.ActiveStatus = "DA";
                            BlockDetails.BookedStatus = "B";
                            BlockDetails.EMP_UserId = UserId;
                            BlockDetails.CreatedOn = CurrentDate;
                            BlockDetails.UpdatedOn = CurrentDate;
                            BlockDetails.reg_status = "B";
                            BlockDetails.SchId = schlId;
                            BlockDetails.Comment = Details.ShortLeaveComment;
                            BlockDetails.MasterResId = MasterDetails.MasterResId;
                            SPA.SPA_EmployeeScheduler.Add(BlockDetails);
                            SPA.SaveChanges();
                            Session["Result"] = "LeaveSucessfully";
                        }
                        else
                            Session["Result"] = "LeaveError";
                    }
                    else
                    {

                        var Check = fu.CheckUpdateReservationExistOrNot(StartDate, EndDate, UserId, Details.ReservationId.Value);
                        if (Check.ToLower() == "no")
                        {
                            var UpdateReservationInfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == Details.ReservationId).FirstOrDefault();
                            if (UpdateReservationInfo != null)
                            {
                                UpdateReservationInfo.BookingDate = Bdate.ToString("dd MMMM, yyyy", enGB);
                                UpdateReservationInfo.FromSlotMasterId = StartTime;
                                UpdateReservationInfo.ToSlotMasterId = EndTime;
                                UpdateReservationInfo.Comment = Details.ShortLeaveComment;
                                UpdateModel(UpdateReservationInfo);
                                var MasterInfo = SPA.spa_Master_Reservation.Where(c => c.MasterResId == UpdateReservationInfo.MasterResId).FirstOrDefault();
                                MasterInfo.BookingDate = Bdate.ToString("dd MMMM, yyyy", enGB);
                                MasterInfo.StartTime = StartTime;
                                MasterInfo.EndTime = EndTime;
                                MasterInfo.Comment = Details.ShortLeaveComment;
                                UpdateModel(MasterInfo);
                                SPA.SaveChanges();
                                Session["Result"] = "LeaveSucessfully";
                            }
                        }
                        else
                            Session["Result"] = "LeaveError";
                    }

                }
                else
                    Session["Result"] = "LeaveError";
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public int AddBasicReservation(long? EmpUserId, string Date, string Time, string View, string AllView)
        {
            //When whole process of Reservation is completed you will be redirected to that location
            fu.setRedirectionofCalendar(View, AllView, Date, EmpUserId);
            //get Shop date
            var UniversalDate = fu.ZonalDate(null);
            try
            {
                DateTime dt = Convert.ToDateTime(Date + " " + Time, enGB);
                var WeekDay = dt.DayOfWeek.ToString();
                var BookingDate = dt.ToString("dd MMMM, yyyy", enGB);
                var Timeslot = dt.TimeOfDay.ToString();
                SPA_EmployeeScheduler CalBookedDetails = new SPA_EmployeeScheduler();
                CalBookedDetails.EMP_UserId = EmpUserId;
                CalBookedDetails.BookingDate = dt.ToString("dd MMMM, yyyy", enGB);
                CalBookedDetails.FromSlotMasterId = Timeslot;
                CalBookedDetails.SchedulerId = SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == WeekDay && c.SchId == schlId && c.ActiveStatus == "A").Select(c => c.SchedulerId).FirstOrDefault();
                CalBookedDetails.ActiveStatus = "BA";
                CalBookedDetails.BookedStatus = "BA";
                CalBookedDetails.CreatedOn = UniversalDate;
                CalBookedDetails.UpdatedOn = UniversalDate;
                CalBookedDetails.SchId = schlId;
                SPA.SPA_EmployeeScheduler.Add(CalBookedDetails);
                SPA.SaveChanges();
                return CalBookedDetails.EmpSchDetailsId;

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return 0;
            }

        }
        [CustomAutohrize(null)]
        public ActionResult CustomerList(int? ReservationId, int? OrderBy, int? Sorting)
        {
            try
            {
                string[] ColumnList = { "Gender", "LastName", "Firstname", "Phoneno", "PasswordQuerry2", "Emailid" };
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                #region PastCodewithoutconsideringothershopcustomer

                //var CustomerList = "select Tbl1.UserId,Tbl1.LandLineNumber,Tbl1.Firstname,Tbl1.LastName,Tbl1.Emailid, Tbl1.Gender, " +
                //                   " Tbl1.Phoneno,Tbl1.LastVisited,Tbl1.EmpfirstName " +
                //                   " from( " +
                //                   " select a.UserId, a.PasswordQuerry2 as LandLineNumber, a.Firstname, a.LastName, " +
                //                   " a.Emailid, a.Gender, a.Phoneno, b.ActiveStatus, a.SchId, " +
                //                   " (select max(x.updatedOn) from SPA_EmployeeScheduler x " +
                //                   "  where x.ActiveStatus = 'C' and x.BookedStatus = 'C' and x.CLIENT_UserId = a.UserId " +
                //                   " ) as LastVisited, " +
                //                   " (select top 1y.firstName from cctsp_User y " +
                //                   "  join SPA_EmployeeScheduler o on y.UserId = o.EMP_UserId " +
                //                   " where o.ActiveStatus = 'C' and o.BookedStatus = 'C' and o.CLIENT_UserId = a.UserId order by o.updatedOn " +
                //                   " ) as EmpfirstName , " +
                //                   " (select b.AccessToData from cctsp_Role a " +
                //                   "  join cctsp_RoleDetails b on a.Roleid = b.RoleId and a.Schlid = b.Schid " +
                //                   "   where a.RoleId = c.RoleId and a.ActiveStatus = 'A' " +
                //                   "  and b.ActiveStatus = 'A' and(a.schlid = " + schlId + " or a.schlid = 236) " +
                //                   "  and d.SolutionId = convert(bigint, IsNull(b.AccesstoSub, '0'))) as AccessToData, " +
                //                  "c.UserId as EmpUserId " +
                //                  " from cctsp_User a " +
                //                  " join ctsp_SolutionMaster d on d.SectionName = 'List view' " +
                //                  " left join SPA_EmployeeScheduler b on b.Client_UserId = a.UserId and b.ActiveStatus = 'C' " +
                //                  " and b.BookedStatus = 'C' and b.SchId = a.SchId and b.Emp_UserId = " + UserId + " " +
                //                  " join cctsp_User c on c.ActiveStatus = 'A'  and c.SchId = a.SchId and c.UserId = " + UserId + " " +
                //                  " Where a.RoleId = 4 and a.ActiveStatus = 'A' and a.Schid = " + schlId + " " +
                //                  "  group by a.UserId,a.PasswordQuerry2, a.Firstname,a.LastName,a.Emailid,a.Gender,a.Phoneno,b.EMP_UserId, " +
                //                  "  a.Updated_Date,a.USerId,b.ActiveStatus,a.SchId,c.RoleId,d.SolutionId,c.UserId " +
                //                  ") as Tbl1 " +
                //                  " where((Tbl1.ActiveStatus = 'C' and Tbl1.EmpUserId = " + UserId + " and Tbl1.AccessToData = 'Own') or(Tbl1.AccessToData != 'Own')) " +
                //                 " group by Tbl1.UserId,Tbl1.LandLineNumber,Tbl1.Firstname,Tbl1.LastName,Tbl1.Emailid, Tbl1.Gender, " +
                //                 " Tbl1.Phoneno,Tbl1.LastVisited,Tbl1.EmpfirstName,Tbl1.ActiveStatus ,Tbl1.SchId,Tbl1.AccessToData,Tbl1.EmpUserId ";
                //if (OrderBy == 0 || OrderBy == null)
                //{
                //    CustomerList = CustomerList + " order by " + ColumnList[ColumnList.Length - 1];
                //    ViewBag.Sorting = null;
                //}
                //else
                //{
                //    CustomerList = CustomerList + " order by " + ColumnList[OrderBy.Value];
                //    ViewBag.Sorting = OrderBy + "," + Sorting;
                //}
                //if (Sorting == 1)
                //    CustomerList = CustomerList + " DESC";
                //ViewBag.CustomerList = SPA.Database.SqlQuery<Models.CustomerTabInfo>(CustomerList).ToList();
                #endregion
                ViewBag.CustomerList = fu.getCustomerList((OrderBy == 0 || OrderBy == null) ? ColumnList[ColumnList.Length - 1] : ColumnList[OrderBy.Value], Sorting, UserId, "Calendar Month");
                ViewBag.ReservationId = ReservationId;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                /*Language Queries*/
                var LanguageDetail = SPA.Language_Label_Detail.Where(c => c.Lang_id == LanguageId &&
                (
                  (c.Page_Name == "back_button" && c.Order_id == 1) || (c.Page_Name == "Title" && c.Order_id == 15) || (c.Page_Name == "Customer_List_view")
                )).ToList();
                ViewBag.LanguageBack = LanguageDetail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                ViewBag.Language = LanguageDetail.Where(c => c.Page_Name == "Customer_List_view" && c.Lang_id == LanguageId).ToList();
                ViewBag.Title = LanguageDetail.Where(c => c.Page_Name == "Title" && c.Order_id == 15 && c.Lang_id == LanguageId).Select(c => c.Value).FirstOrDefault();
                return View();

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult CustomerBookDetail(string CustomerUserId, int? ReservationId)
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                SPA_EmployeeScheduler customerdetails = new SPA_EmployeeScheduler();
                customerdetails = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                customerdetails.CLIENT_UserId = Convert.ToInt64(CustomerUserId);
                customerdetails.ActiveStatus = "DA";
                customerdetails.BookedStatus = "T";
                UpdateModel(customerdetails);
                SPA.SaveChanges();
                return RedirectToAction("CalChooseProduct", "Reservation", new { ReservationId = ReservationId });
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
        [CustomAutohrize(null)]
        public ActionResult CalChooseProduct(int? ReservationId)
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var EmpData = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                var EmpId = EmpData.EMP_UserId;
                string Product = "select d.CatgTypeId as ProductId,a.SectionDesc  as ProductName,d.orderData,b.Gender as Gender, d.SectionDesc,cast(a.Duration as int) as Duration,CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38, 2)) as Amount,f.CatgDesc as GroupName,e.GroupIdByShop from CTSP_SolutionMaster a join CCTSP_CategoryDetails b on a.sectionDesc = b.CatgDesc join cctsp_user c on a.UserId = c.UserId join CTSP_SolutionMaster d on d.CatgTypeId = b.CatgTypeId join SPA_SchedulerSlotMaster s on s.UserId = c.UserId join CCTSP_GroupProductDetails e on e.ProductId = b.CatgTypeId join CCTSP_CategoryDetails f on f.CatgTypeId = e.GroupIdByShop where a.activestatus = 'A' and d.activestatus = 'A' and c.activestatus = 'A'and b.activestatus = 'A' and s.ActiveStatus = 'A' and e.ActiveStatus = 'A' and f.ActiveStatus = 'A' and a.SchId = " + schlId + " and d.SchId = a.SchId and c.SchId = a.SchId and s.schId = a.SchId and e.SchlId = a.SchId and f.DomainType = a.SchId and c.UserId = " + EmpId + " and a.SectionName = 'EmployeeProduct' and b.catgId = 111 and b.catgDesc = a.SectionDesc and d.Duration = a.Duration and d.amount = a.amount group by d.CatgTypeId,a.SectionDesc,d.orderData,b.Gender, d.SectionDesc,a.Duration,a.Amount,f.CatgDesc,e.GroupIdByShop,f.Group_orderdata order by f.Group_orderdata,d.orderdata";
                ViewBag.productData = SPA.Database.SqlQuery<Models.GroupProductList>(Product).ToList();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Choose_Product" || (c.Page_Name == "Title" && c.Order_id == 2)) && c.Lang_id == ShopInfo.Lang_id).ToList();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == ShopInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                ViewBag.ReservationId = ReservationId;
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
        [CustomAutohrize(null)]
        public ActionResult CalProductBooksDetail(FormCollection Booking)
        {

            //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
            //{
            int ReservationID = Convert.ToInt32(Booking["ReservationId"].ToString());
            var ProductBookedDetail = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationID).FirstOrDefault();
            #region AddProduct
            foreach (var item in Booking.Keys)
            {
                if (item.ToString() != "ReservationId") { ProductBookedDetail.Product_Id = Convert.ToInt64(item); }
            }
            #endregion
            var productDetails = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == ProductBookedDetail.Product_Id && c.Activestatus == "A" && c.SchId == schlId).FirstOrDefault();
            ProductBookedDetail.Product_price = productDetails.Amount;
            var SlotMasterIDetail = DateTime.Parse(ProductBookedDetail.FromSlotMasterId).AddMinutes(Convert.ToDouble(productDetails.Duration)).ToString("HH:mm:ss");
            var SlotMstreID = "select top 1convert(nvarchar,StartTime) as toslotmasterid from SPA_SchedulerSlotMaster where ActiveStatus='A' and SchedulerId=" + ProductBookedDetail.SchedulerId + " and schId=" + schlId + " and StartTime>='" + SlotMasterIDetail + "' order by StartTime";
            var SlotMstrID = SPA.Database.SqlQuery<Models.EmployeeBooking>(SlotMstreID).FirstOrDefault();
            var SlotMasterID = "";
            if (SlotMstrID != null) { SlotMasterID = DateTime.Parse(SlotMstrID.toslotmasterid.ToString()).ToString("HH:mm:ss"); }
            else
            {
                Session["Result"] = "BookedError";
                return Redirect(Url.Action("Reservation", "Reservation") + "#calendarmonth");
            }
            ProductBookedDetail.ToSlotMasterId = SlotMasterID;
            ProductBookedDetail.ActiveStatus = "DA";
            ProductBookedDetail.BookedStatus = "BT";
            ProductBookedDetail.UpdatedOn = DateTime.Today;
            string wkDayselDate = DateTime.Parse(ProductBookedDetail.BookingDate).ToString("dddd", enGB);
            var Check = "select min(startTime) as MinTime,max(EndTime) as MaxTime from SPA_EmployeeSchedulers where UserId=" + ProductBookedDetail.EMP_UserId + " and WeekDay='" + wkDayselDate + "' and ActiveStatus='A' and SchId=" + schlId;
            var CheckData = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(Check).FirstOrDefault();
            if ((CheckData.MinTime <= DateTime.Parse(ProductBookedDetail.FromSlotMasterId).TimeOfDay && CheckData.MaxTime >= DateTime.Parse(ProductBookedDetail.FromSlotMasterId).TimeOfDay) && (CheckData.MinTime <= DateTime.Parse(ProductBookedDetail.ToSlotMasterId).TimeOfDay && CheckData.MaxTime >= DateTime.Parse(ProductBookedDetail.ToSlotMasterId).TimeOfDay))
            {
                string StartDate = DateTime.Parse(ProductBookedDetail.BookingDate + " " + ProductBookedDetail.FromSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                string EndDate = DateTime.Parse(ProductBookedDetail.BookingDate + " " + ProductBookedDetail.ToSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                //check validation for timeslot available or not
                var checkbooking = fu.CheckReservationExistOrNot(StartDate, EndDate, ProductBookedDetail.EMP_UserId.Value);
                if (checkbooking.ToLower() == "yes")
                {
                    //Failed
                    Session["Result"] = "BookedError";
                    return Redirect(Url.Action("Reservation", "Reservation") + "#calendarmonth");
                }
                else
                {
                    UpdateModel(ProductBookedDetail);
                    SPA.SaveChanges();
                    long ClientUserID = Convert.ToInt64(ProductBookedDetail.CLIENT_UserId);
                    return RedirectToAction("CalProductBookedDetails", "Reservation", new { ReservationId = ReservationID });
                }
            }
            else
            {
                Session["Result"] = "BookedError";
                return Redirect(Url.Action("Reservation", "Reservation") + "#calendarmonth");
            }
            //}
            //else { return RedirectToAction("SignIn", "Login"); }
        }
        public ActionResult CalProductBookedDetails(int? ReservationId)
        {
            try
            {
                var ReservationInfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                string BookedDetails = "select a.EmpSchDetailsId,a.BookingDate,b.CatgDesc as ProductName,a.FromSlotMasterId ,c.sectiondesc as Productdesc,cast(c.Duration as int) as Duration, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id  join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId where a.BookedStatus = 'BT' and a.ActiveStatus = 'DA' and a.CLIENT_UserId = " + ReservationInfo.CLIENT_UserId + " and a.schid = " + schlId + " and b.domaintype = " + schlId + " and b.activestatus = 'A' and b.catgid = 111 and c.activestatus = 'A' and c.SchId = " + schlId + " and d.activestatus = 'A' and d.schid = " + schlId + " and a.EmpSchDetailsId=" + ReservationId;
                var BookedDetail = SPA.Database.SqlQuery<Models.ConfirmModel>(BookedDetails).ToList();
                var shopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(shopInfo.Currency);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Confirm_Booking" || c.Page_Name == "Small_calander" || (c.Page_Name == "Title" && c.Order_id == 5)) && c.Lang_id == shopInfo.Lang_id).ToList();
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == shopInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                ViewBag.BookedDetail = BookedDetail;
                ViewBag.ReservationId = ReservationId;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult CalNewCustomer(int? ReservationId)
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var CountryVersion = ConfigurationManager.AppSettings["CountryVersionShop"];
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(d => d.SchlId == schlId && d.ActiveStatus == "A").Select(c => new Models.shopMaster
                {
                    LangId = c.Lang_id,
                    Display_Invoice = c.Display_Invoice,
                    SchlStudentStrength=c.SchlStudentStrength,
                    Country=c.Schcountry                    
                }).FirstOrDefault();
                ViewBag.CheckInvoice = fu.CheckInvoice(ShopInfo.Display_Invoice);
                ViewBag.DisplayInvoice = ShopInfo.Display_Invoice != null ? ShopInfo.Display_Invoice : 0;
                // ViewBag.GenderList = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == LanguageId).Select(c => c.CatgType).Distinct().OrderBy(c => c).ToList();
                var CatgNameList = "Salutation,New_Gender,Title";
                ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, ShopInfo.LangId.Value);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Customer_New_Customer" && c.Lang_id == ShopInfo.LangId.Value).ToList();
                ViewBag.ReservationId = ReservationId;
                ViewBag.LanguageBack = SPA.Language_Label_Detail.Where(c => c.Page_Name == "back_button" && c.Order_id == 1 && c.Lang_id == ShopInfo.LangId.Value).Select(c => c.Value).FirstOrDefault();
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Lang_id == ShopInfo.LangId && c.Order_id == 11).Select(c => c.Value).FirstOrDefault();
                var countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A").Select(c => new Models.CountryList { Country = c.name_country }).ToList();
                if (CountryVersion == "INDIA")
                    countrylist = countrylist.Where(c => c.Country == "INDIA").ToList();
                else
                    countrylist = countrylist.Where(c => c.Country != "INDIA").ToList();
                ViewBag.countrylist = countrylist;
                ViewBag.LngLocal = fu.SmallCalLangTranslate(ShopInfo.LangId.Value);
                ViewBag.Shopinfo = ShopInfo;
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
        [HttpPost]
        [CustomAutohrize(null)]
        public void CalNewCustomerAdd(FormCollection user)
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                int ReservationID = Convert.ToInt32(user["ReservationId"].ToString());
                DateTime EuropeDate = fu.ZonalDate(null);
                string Status = "No";
                CCTSP_User UserInfo = new CCTSP_User();
                SPA_EmployeeScheduler CustomerInfo = new SPA_EmployeeScheduler();
                var phoneno = user["NwPhoneNo"];
                var UserPhoneNo = SPA.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 4 && c.PhoneNo == phoneno && c.ActiveStatus == "A" && c.PhoneNo != "").Count();
                if (UserPhoneNo > 0)
                    Status = "Yes";
                if (Status == "No")
                {
                    UserInfo.PhoneNo = phoneno;
                    UserInfo.PasswordQuerry2 = user["NwLandLineNo"];
                    UserInfo.Gender = user["NWTitle"];
                    UserInfo.FirstName = user["NWFirstName"];
                    UserInfo.LastName = user["NWLastName"];
                    UserInfo.EmailId = user["NWEmail"];
                    UserInfo.Password = user["NWPassword"];
                    if (user["NWPostal"].ToString() != "")
                        UserInfo.Pincode = Convert.ToInt32(user["NWPostal"]);
                    UserInfo.City = user["NWCITY"];
                    UserInfo.Address1 = user["NWAddress"];
                    UserInfo.CreatedOn = EuropeDate;
                    UserInfo.RoleId = 4;
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
                    UserInfo.ActiveStatus = "A";
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
                    SPA.CCTSP_User.Add(UserInfo);
                    SPA.SaveChanges();
                    //Add Reservation data in SPA_EmployeeScheduler
                    var CustomerUserId = SPA.CCTSP_User.Where(c => c.FirstName == UserInfo.FirstName && c.LastName == UserInfo.LastName && c.PhoneNo == UserInfo.PhoneNo && c.EmailId == UserInfo.EmailId).Select(c => c.UserId).FirstOrDefault();
                    CustomerInfo.CLIENT_UserId = CustomerUserId;
                    UserInfo.Updated_Date = EuropeDate;
                    string name = UserInfo.FirstName + " " + UserInfo.LastName;
                    var checkSMSService = SPA.CCTSP_User.Where(c => c.UserId == UserInfo.UserId && c.SchId == schlId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                    if (checkSMSService != null)
                        SMS.UserRegistration(name, null, UserInfo.PhoneNo.ToString());
                    var checkEMAILService = SPA.CCTSP_User.Where(c => c.UserId == UserInfo.UserId && c.SchId == schlId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                    if (checkEMAILService != null)
                        Email.MailForUserRegistration(UserInfo.UserId, UserInfo.EmailId);
                    //Email.UserRegistration(UserInfo.UserId, null, UserInfo.EmailId);

                }
                Session["CustomerValidationMsg"] = Status;
                //}
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }

        public void CalBookOrder(int? ReservationId, string Comments)
        {
            try
            {
                var Reservation = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                string StartDate = DateTime.Parse(Reservation.BookingDate + " " + Reservation.FromSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                string EndDate = DateTime.Parse(Reservation.BookingDate + " " + Reservation.ToSlotMasterId).ToString("yyyy-MM-dd HH:mm:ss");
                var Check = fu.CheckReservationExistOrNot(StartDate, EndDate, Reservation.EMP_UserId.Value);
                var EnterpriseInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                #region AllowPastDateCheck
                //For Allow PastDate
                var ActiveStatus = "A";
                var BookedStatus = "B";
                bool PastDate = false;
                PastDate = fu.CheckPastDate(EnterpriseInfo.TimeZone, DateTime.Parse(Reservation.BookingDate + " " + Reservation.FromSlotMasterId));
                if (PastDate==true)
                {
                    ActiveStatus = "C";
                    BookedStatus = "C";
                }
                #endregion
                if (Check.ToLower() == "no")
                {
                    if (!string.IsNullOrEmpty(Comments))
                        Reservation.Comment = Comments;
                    Reservation.BookedStatus = BookedStatus;
                    Reservation.ActiveStatus = ActiveStatus;
                    Reservation.reg_status = "M";
                    UpdateModel(Reservation);
                    SPA.SaveChanges();
                    //Successful               
                    //string CalCustomerBookedDetails = "select concat(b.Gender, ' ', b.firstName, ' ', b.lastName) as FirstName,a.BookingDate,b.EmailId as Image,a.FromSlotMasterId,a.EmpSchDetailsId from spa_EmployeeScheduler a join cctsp_user b on a.Client_UserId = b.UserId where a.EmpSchdetailsId = '" + ReservationId + "'";
                    //var CalCustomerBooked = SPA.Database.SqlQuery<Models.ConfirmModel>(CalCustomerBookedDetails).FirstOrDefault();
                    string BookedData = "select a.EmpSchDetailsId,a.comment,a.ActiveStatus,a.BookedStatus,a.BookingDate,b.CatgTypeid as ProductId ,b.CatgDesc as ProductName,a.Product_price,a.FromSlotMasterId ,a.ToSlotMasterId,a.Emp_UserId as EMP_UserId ,a.CLIENT_UserId as CLIENT_UserId, c.sectiondesc as Productdesc, cast(c.Duration as int) as Duration,concat(e.Gender, ' ', e.firstName, ' ', e.lastName) as ClientName ,e.EmailId,f.FirstName as ShopOwnerName , f.LastName as ShopOwnerLastName,e.EmailId as ClientEmail,e.PhoneNo as ClientPhoneNo, FORMAT(a.CreatedOn ,'dd-MM-yyyy hh:mm') as CreatedDate, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId  join CCTSP_User e on e.UserId = a.CLIENT_UserId join CCTSP_User f on f.schid = a.schid where a.schid =" + schlId + "  and b.catgid = 111 and c.SchId = " + schlId + " and d.schid = " + schlId + " and f.schid = " + schlId + " and f.RoleId = 1  and f.ActiveStatus = 'A' and a.EmpSchDetailsId = " + ReservationId + "";
                    var BookingDetails = SPA.Database.SqlQuery<Models.ConfirmModel>(BookedData).FirstOrDefault();                  
                    //PaymentDeduction
                    fu.PaymentDeduction(Reservation.Product_price, Convert.ToInt64(ReservationId), EnterpriseInfo, "A");
                    //Add MasterData
                    fu.AddMasterData(BookingDetails, ActiveStatus, BookedStatus, "M");
                    if (BookingDetails != null && PastDate==false)
                        Email.Approvebooking(BookingDetails.ClientName, BookingDetails.BookingDate, BookingDetails.EmailId, BookingDetails.FromSlotMasterId, BookingDetails.EmpSchDetailsId);
                    Session["Result"] = "BookedSucessfully";
                }
                else
                {
                    Session["Result"] = "BookedError";
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public ActionResult EmployeeDetails()
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //string UserDataquery = "select a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName from SPA_SchedulerSlotMaster a join cctsp_user b on a.UserId = b.UserId join ctsp_solutionMaster c on c.UserId = b.UserId where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' and c.ActiveStatus='A' and c.schId=" + schlId + " and c.catgTypeId=11145 group by a.UserId,a.slotDue,b.firstName,b.lastName";
                string UserDataquery = "select b.Answer2 as Image,a.UserId,max(a.starttime) as MaxTime,min(a.starttime) as MinTime,a.SlotDue,b.firstName,b.lastName " +
                                       "from SPA_SchedulerSlotMaster a " +
                                       " join cctsp_user b on a.UserId = b.UserId " +
                                       " join ctsp_solutionMaster c on c.UserId = b.UserId " +
                                       " join ctsp_SolutionMaster d on d.SectionName = 'Calendar Month' and d.ActiveStatus = 'A' " +
                                       " join cctsp_User e on e.UserId = " + UserId + " " +
                                       " join cctsp_Role g on g.RoleId=e.Roleid and (g.Schlid=" + schlId + " or g.Schlid=236) and g.ActiveStatus='A'" +
                                       " join cctsp_RoleDetails f on f.RoleId = g.RoleId and f.ActiveStatus = 'A' and d.SolutionId = convert(bigint, f.AccesstoSub) " +
                                       " and f.SchId =g.Schlid and d.SolutionId = convert(bigint, f.AccesstoSub) and " +
                                       " ((f.AccessToData = 'Own' and b.UserId = e.UserId) or(f.AccessToData != 'Own')) " +
                                       " where a.ActiveStatus = 'A' and a.SchId = " + schlId + "  and b.ActiveStatus = 'A' " +
                                       " and c.ActiveStatus = 'A' and c.schId = " + schlId + " and c.catgTypeId = 11145 " +
                                       " group by a.UserId,a.slotDue,b.firstName,b.lastName,b.Answer2";
                var UserData = SPA.Database.SqlQuery<Models.WeekDayBlockDetail>(UserDataquery).ToList();
                ViewBag.EmployeeDetails = UserData;
                var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault();
                ViewBag.LanguageLabel = SPA.Language_Label_Detail.Where(c => c.Lang_id == Lang_id && c.Page_Name == "Calender_view" && c.Order_id == 7).FirstOrDefault();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public void DeleteCalProduct(long? Id)
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
        public void CancelCalBookedDetail(int? ReservationId, string status, string Viewstatus, string bookingdate, string Allview)
        {
            try
            {
                var BookedDetails = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == ReservationId && c.SchId == schlId).FirstOrDefault();
                long Empuserid = BookedDetails.EMP_UserId.Value;
                if (Allview != null && Allview == "All")
                {
                    Session["AllView"] = "All";
                }
                if (Viewstatus != null && Viewstatus != "")
                {
                    Session["View"] = Viewstatus;
                    Session["ViewUserId"] = Empuserid.ToString();
                }
                if (bookingdate != "" && bookingdate != null)
                {
                    var BDate = Convert.ToDateTime(bookingdate, enGB);
                    Session["ViewDate"] = BDate.Day + "-" + BDate.AddMonths(-1).ToString("MM") + "-" + BDate.Year;
                }
                if (status == "TemBooking")
                {
                    //var query = "update SPA_EmployeeScheduler set ActiveStatus='D' where EmpSchDetailsId=" + ReservationId + " and SchId=" + schlId;
                    //SPA.Database.ExecuteSqlCommand(query);                   
                    BookedDetails.ActiveStatus = "D";
                    BookedDetails.UpdatedOn = DateTime.Today;
                    UpdateModel(BookedDetails);
                    if (BookedDetails.MasterResId > 0 && BookedDetails.MasterResId != null)
                    {
                        var UpdateMasterinfo = SPA.spa_Master_Reservation.Where(c => c.MasterResId == BookedDetails.MasterResId).FirstOrDefault();
                        UpdateMasterinfo.ActiveStatus = "D";
                        UpdateModel(UpdateMasterinfo);
                    }
                    SPA.SaveChanges();

                }
                if (status == "Booking" || status == "QuickBlocker")
                {
                    //var query = "update SPA_EmployeeScheduler set ActiveStatus='D',BookedStatus='D' where EmpSchDetailsId=" + ReservationId + " and SchId=" + schlId;
                    //SPA.Database.ExecuteSqlCommand(query);
                    BookedDetails.ActiveStatus = "D";
                    BookedDetails.BookedStatus = "D";
                    BookedDetails.UpdatedOn = DateTime.Today;
                    UpdateModel(BookedDetails);
                    if (BookedDetails.MasterResId > 0 && BookedDetails.MasterResId != null)
                    {
                        var UpdateMasterinfo = SPA.spa_Master_Reservation.Where(c => c.MasterResId == BookedDetails.MasterResId).FirstOrDefault();
                        UpdateMasterinfo.ActiveStatus = "D";
                        UpdateMasterinfo.BookedStatus = "D";
                        UpdateModel(UpdateMasterinfo);
                    }
                    SPA.SaveChanges();
                }
                if (status != null && status != "QuickBlocker")
                {

                    var UserDetails = SPA.CCTSP_User.Where(a => a.UserId == BookedDetails.CLIENT_UserId).FirstOrDefault();
                    if (UserDetails != null)
                    {
                        string name = UserDetails.Gender + " " + UserDetails.FirstName + " " + UserDetails.LastName;
                        string date = BookedDetails.BookingDate;
                        var checkEMAILCancel = SPA.CCTSP_User.Where(c => c.UserId == BookedDetails.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();
                        if (checkEMAILCancel != null)
                            Email.Cancelbooking(name, UserDetails.EmailId, BookedDetails.FromSlotMasterId, date, BookedDetails.EmpSchDetailsId);
                        var checkSMSCancel = SPA.CCTSP_User.Where(c => c.UserId == BookedDetails.CLIENT_UserId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                        if (checkSMSCancel != null)
                            SMS.Cancelbooking(name, date, UserDetails.LoginName);
                    }

                }


            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public ActionResult MonthCalendar(long? id, string monthChange, string yearChange, string status)
        {
            try
            {
                CalendarMonthModel cmm;
                int year = 0;
                int month = 0;
                if (status != null)
                {
                    if (status == "yearBack")
                    {
                        year = Convert.ToInt32(yearChange) - 1;
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month;
                    }
                    else if (status == "monthBack")
                    {
                        year = Convert.ToInt32(yearChange);
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month - 1;
                        if (month == 0)
                        {
                            month = 12;
                            year = year - 1;
                        }

                    }
                    else if (status == "yearNext")
                    {
                        year = Convert.ToInt32(yearChange) + 1;
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month;
                    }
                    else
                    {
                        year = Convert.ToInt32(yearChange);
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month + 1;
                        if (month == 13)
                        {
                            month = 1;
                            year = year + 1;
                        }
                    }
                }
                else
                {
                    year = Convert.ToInt32(yearChange);
                    month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month;
                }
                List<CalendarMonthModel> calendarMonthmodel = new List<CalendarMonthModel>();
                var UserData = SPA.CCTSP_User.Where(c => c.UserId == id).FirstOrDefault();
                var ShpDays = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == schlId && c.ActiveStatus == "A").Select(c => c.WeekDay).Distinct();
                List<string> daysName = new List<string>();
                List<int> daysNumber = new List<int>();
                DateTime date = new DateTime(year, month, 1);
                do
                {
                    string Days = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek);
                    if (ShpDays.Contains(Days))
                    {
                        daysNumber.Add(date.Day);
                        daysName.Add(Days);
                    }
                    date = date.AddDays(1);
                }
                while (date.Month == month);
                cmm = new CalendarMonthModel();
                // cmm.user_name = UserData.FirstName + " " + UserData.LastName;
                cmm.day_number = daysNumber;
                cmm.day_name = daysName;
                double a = Convert.ToDouble(daysNumber.Count()) / Convert.ToDouble(ShpDays.Count());
                int ans = Convert.ToInt32(Math.Ceiling(a));
                cmm.week_number = ans;
                cmm.CurrentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                cmm.Currentyear = year.ToString();
                cmm.colSpan = ShpDays.Count();
                calendarMonthmodel.Add(cmm);

                var Adi = calendarMonthmodel;

                return View(Adi);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public string GetNameMonthAndYear(string monthChange, string yearChange, string status)
        {
            try
            {
                int year = 0;
                int month = 0;
                if (status != null)
                {
                    if (status == "yearBack")
                    {
                        year = Convert.ToInt32(yearChange) - 1;
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month;
                    }
                    else if (status == "monthBack")
                    {
                        year = Convert.ToInt32(yearChange);
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month - 1;
                        if (month == 0)
                        {
                            month = 12;
                            year = year - 1;
                        }

                    }
                    else if (status == "yearNext")
                    {
                        year = Convert.ToInt32(yearChange) + 1;
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month;
                    }
                    else
                    {
                        year = Convert.ToInt32(yearChange);
                        month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month + 1;
                        if (month == 13)
                        {
                            month = 1;
                            year = year + 1;
                        }
                    }
                }
                else
                {
                    year = Convert.ToInt32(yearChange);
                    month = DateTime.ParseExact(monthChange, "MMMM", new CultureInfo("en-US")).Month;
                }
                string MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

                return MonthName + " - " + year;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }

        }
        public ActionResult DayChange(long UserId, string DateandDay, string status)
        {
            try
            {
                var DateArray = DateandDay.Split(' ');
                if (status == "back")
                {
                    DateTime PreviousDate = DateTime.Parse(DateArray[1]).AddDays(-1);
                    string PreviousDayName = PreviousDate.DayOfWeek.ToString();
                    var GetDataEx = SPA.SPA_SchedulerSlotMaster.Where(c => c.CCTSP_SchedulerMaster.WeekDay == PreviousDayName && c.ActiveStatus == "A" && c.SchId == schlId && c.CCTSP_SchedulerMaster.ActiveStatus == "A").OrderBy(c => c.SchedulerId);
                    if (GetDataEx.Count() > 0)
                        ViewBag.DayTime = GetDataEx.Select(c => c.StartTime);
                    else
                    {
                        var GetAllData = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == PreviousDayName && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId).OrderBy(c => c.Scheduler_Time_Id);
                        ViewBag.DayTime = GetAllData.Select(c => c.StartTime);
                    }
                }
                else
                {
                    DateTime NextDate = DateTime.Parse(DateArray[1]).AddDays(1);
                    string NextDateDayName = NextDate.DayOfWeek.ToString();
                    var GetDataEx = SPA.SPA_SchedulerSlotMaster.Where(c => c.CCTSP_SchedulerMaster.WeekDay == NextDateDayName && c.ActiveStatus == "A" && c.SchId == schlId && c.CCTSP_SchedulerMaster.ActiveStatus == "A").OrderBy(c => c.SchedulerId); ;
                    if (GetDataEx.Count() > 0)
                        ViewBag.DayTime = GetDataEx.Select(c => c.StartTime);
                    else
                    {
                        var GetAllData = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == NextDateDayName && c.ActiveStatus == "A" && c.SchId == schlId && c.UserId == UserId).OrderBy(c => c.Scheduler_Time_Id);
                        ViewBag.DayTime = GetAllData.Select(c => c.StartTime);
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
        public void GoBackToPending(long? EmpSchDetailsId)
        {
            try
            {
                var query = "update SPA_EmployeeScheduler set ActiveStatus='DA',BookedStatus='B' where EmpSchDetailsId=" + EmpSchDetailsId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        [CustomAutohrize(null)]
        public ActionResult UpdateReservedData(int? ReservationId, string Status, string Viewstatus, string AllView)
        {
            try
            {
                if (AllView != null && AllView == "All" && Viewstatus == "Day")
                {
                    Session["AllView"] = "All";
                }
                if (Viewstatus != null && Viewstatus != "")
                {
                    Session["View"] = Viewstatus;
                }

                SPA_EmployeeScheduler SPADATA = new SPA_EmployeeScheduler();
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                SPADATA = SPA.SPA_EmployeeScheduler.Where(c => c.SchId == schlId && c.EmpSchDetailsId == ReservationId).FirstOrDefault();
                if (Viewstatus != null && Viewstatus != "")
                {
                    Session["ViewUserId"] = SPADATA.EMP_UserId.ToString();
                    if (!string.IsNullOrEmpty(SPADATA.BookingDate))
                    {
                        var BDate = Convert.ToDateTime(SPADATA.BookingDate, enGB);
                        Session["ViewDate"] = BDate.Day + "-" + BDate.AddMonths(-1).ToString("MM") + "-" + BDate.Year;
                    }
                }
                ViewBag.EmpName = SPA.CCTSP_User.Where(c => c.UserId == SPADATA.EMP_UserId).Select(c => c.FirstName).FirstOrDefault();
                ViewBag.ReservedData = SPADATA;
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).FirstOrDefault();
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                #region Language
                var Language = new List<Language_Label_Detail>();
                Language.AddRange(fu.getLanguageData("Choose_Product", 0, ShopInfo.Lang_id.Value));
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 16 && c.Lang_id == ShopInfo.Lang_id).ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Reservation_update" && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.Lang_id).ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == ShopInfo.Lang_id && c.ActiveStatus == "A" && c.Order_id >= 8 && c.Page_Name == "Small_calander").ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == ShopInfo.Lang_id && c.ActiveStatus == "A" && c.Order_id == 1 && c.Page_Name == "back_button").ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == ShopInfo.Lang_id && c.ActiveStatus == "A" && c.Order_id == 38 && c.Page_Name == "AlertMsg").ToList());
                ViewBag.OpenLang = Language;
                ViewBag.ShopInfo = ShopInfo;
                #endregion
                #region newcode
                string Product = "select d.CatgTypeId as ProductId,a.SectionDesc as ProductName,d.orderData,b.Gender as Gender, d.SectionDesc,cast(a.Duration as int) as Duration,CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38, 2)) as Amount,f.CatgDesc as GroupName,e.GroupIdByShop from CTSP_SolutionMaster a join CCTSP_CategoryDetails b on a.sectionDesc = b.CatgDesc join cctsp_user c on a.UserId = c.UserId join CTSP_SolutionMaster d on d.CatgTypeId = b.CatgTypeId join SPA_SchedulerSlotMaster s on s.UserId = c.UserId join CCTSP_GroupProductDetails e on e.ProductId = b.CatgTypeId join CCTSP_CategoryDetails f on f.CatgTypeId = e.GroupIdByShop where a.activestatus = 'A' and d.activestatus = 'A' and c.activestatus = 'A'and b.activestatus = 'A' and s.ActiveStatus = 'A' and e.ActiveStatus = 'A' and f.ActiveStatus = 'A' and a.SchId = " + schlId + " and d.SchId = a.SchId and c.SchId = a.SchId and s.schId = a.SchId and e.SchlId = a.SchId and f.DomainType = a.SchId and a.SectionName = 'EmployeeProduct' and b.catgId = 111 and b.catgDesc = a.SectionDesc and d.Duration = a.Duration and d.amount = a.amount group by d.CatgTypeId,a.SectionDesc,d.orderData,b.Gender, d.SectionDesc,a.Duration,a.Amount,f.CatgDesc,e.GroupIdByShop,f.Group_orderdata order by f.Group_orderdata,d.orderdata";
                ViewBag.DataProductAssigned = SPA.Database.SqlQuery<Models.GroupProductList>(Product).ToList();
                #endregion
                ViewBag.ReservationId = ReservationId;
                if (Status != null) { ViewBag.ReservationStatus = Status; }
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
        #region Newcode of ChooseEmployeeUpdates
        public ActionResult ChooseEmployeeUpdates(int? CatgTypeId, int? ReservationId, string Status)
        {
            try
            {
                ViewBag.CatgTypeId = CatgTypeId;
                ViewBag.ReservationId = ReservationId;
                ViewBag.ReservationStatus = Status;
                var EmployeeData = "select distinct(a.UserId),d.CatgTypeId,c.FirstName,c.LastName,c.answer2 as Image,a.SectionDesc as ProductName,d.sectionDesc as ProductDetail,cast(a.Duration as int)as Duration,CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38,2)) as Amount from CTSP_SolutionMaster a join CCTSP_CategoryDetails b on  a.sectionDesc=b.CatgDesc join cctsp_user c on a.UserId=c.UserId join CTSP_SolutionMaster d on d.CatgTypeId=b.CatgTypeId join SPA_SchedulerSlotMaster e on e.UserId=a.UserId where  a.activestatus='A' and c.RoleId not in (1,4)  and a.SchId=" + schlId + " and d.activestatus='A' and d.SchId=" + schlId + " and a.SectionName = 'EmployeeProduct'  and c.activestatus='A' and c.SchId=" + schlId + " and b.activestatus='A' and d.Duration=a.Duration and d.amount=a.amount and d.CatgTypeId=" + CatgTypeId + " and e.ActiveStatus='A' and e.SchId=" + schlId + "";
                var EmployeeList = SPA.Database.SqlQuery<Models.EmployeeList>(EmployeeData).ToList();
                ViewBag.DataEmployee = EmployeeList;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                var Language = new List<Language_Label_Detail>();
                Language.AddRange(fu.getLanguageData("Choose_Employee", 0, LanguageId));
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Page_Name == "Reservation_update" && c.Lang_id == LanguageId).ToList());
                Language.AddRange(SPA.Language_Label_Detail.Where(c => c.Lang_id == LanguageId && c.Order_id == 1 && c.Page_Name == "back_button").ToList());
                ViewBag.OpenLang = Language;
                if (LanguageId == 1 || LanguageId == 0) { ViewBag.LocalLanguage = "en"; }
                if (LanguageId == 2) { ViewBag.LocalLanguage = "de"; }
                if (LanguageId == 3) { ViewBag.LocalLanguage = "fr"; }
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        #endregion
        public void UpdateReservationInformation(FormCollection UpdateReservation)
        {
            try
            {
                long CatgTypeId = Convert.ToInt64(UpdateReservation["ChoosedProduct"]);
                int EmpSchId = Convert.ToInt32(UpdateReservation["ChangeReservation"]);
                var EnterpriseInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                long EmpUserId = 0;
                if (UpdateReservation["UserIdOrder"] != "")
                {
                    EmpUserId = Convert.ToInt64(UpdateReservation["UserIdOrder"]);
                }
                TimeSpan ChoosedTime = DateTime.Parse(UpdateReservation["choosedTime"]).TimeOfDay;
                DateTime ChoosedDate = DateTime.Parse(UpdateReservation["ChoosedDate"]);
                var WeekOfChoosedDate = DateTime.Parse(ChoosedDate.ToString()).DayOfWeek.ToString();
                var GetAllEmpTime = SPA.SPA_SchedulerSlotMaster.Where(c => c.CCTSP_SchedulerMaster.WeekDay == WeekOfChoosedDate && c.UserId == EmpUserId && c.SchId == schlId && c.ActiveStatus == "A" && c.CCTSP_SchedulerMaster.ActiveStatus == "A");
                var GetBookedProductInfo = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == CatgTypeId && c.SchId == schlId && c.Activestatus == "A").FirstOrDefault();
                int TotalDuration = Convert.ToInt32(GetBookedProductInfo.Duration);
                TimeSpan TempTime = new TimeSpan(0, 0, Convert.ToInt32(TotalDuration), 0, 0);
                TimeSpan FinalTempTime = (ChoosedTime + TempTime);
                TimeSpan EndTime = GetAllEmpTime.Where(c => c.StartTime >= FinalTempTime).OrderBy(c => c.StartTime).Select(c => c.StartTime).FirstOrDefault().Value;
                //var query = "update SPA_EmployeeScheduler set ActiveStatus='U',BookedStatus='U' where EmpSchDetailsId="+ EmpSchId + " and SchId=" + schlId;
                // SPA.Database.ExecuteSqlCommand(query);
                var UpdateResinfo = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                UpdateResinfo.ActiveStatus = "U";
                UpdateResinfo.BookedStatus = "U";
                UpdateModel(UpdateResinfo);
                if (UpdateResinfo.MasterResId > 0 && UpdateResinfo.MasterResId != null)
                {
                    var UpdateMasterinfo = SPA.spa_Master_Reservation.Where(c => c.MasterResId == UpdateResinfo.MasterResId).FirstOrDefault();
                    UpdateMasterinfo.ActiveStatus = "U";
                    UpdateMasterinfo.BookedStatus = "U";
                    UpdateModel(UpdateMasterinfo);
                }
                SPA.SaveChanges();
                long CustomerId = UpdateResinfo.CLIENT_UserId.Value;
                #region AllowPastDateCheck
                //For Allow PastDate
                var ActiveStatus = "A";
                var BookedStatus = "B";
                bool PastDate = false;
                PastDate = fu.CheckPastDate(EnterpriseInfo.TimeZone, DateTime.Parse((ChoosedDate +ChoosedTime)+""));
                if (PastDate == true)
                {
                    ActiveStatus = "C";
                    BookedStatus = "C";
                }
                #endregion
                SPA_EmployeeScheduler EmployeeScheduler = new SPA_EmployeeScheduler();
                // EmployeeScheduler = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                EmployeeScheduler.CLIENT_UserId = CustomerId;
                EmployeeScheduler.EMP_UserId = EmpUserId;
                EmployeeScheduler.FromSlotMasterId = ChoosedTime.ToString();
                EmployeeScheduler.ToSlotMasterId = EndTime.ToString();
                EmployeeScheduler.BookingDate = ChoosedDate.ToString("dd MMMM, yyyy");
                EmployeeScheduler.Product_Id = CatgTypeId;
                EmployeeScheduler.ActiveStatus = ActiveStatus;
                EmployeeScheduler.BookedStatus = BookedStatus;
                EmployeeScheduler.reg_status = "M";
                EmployeeScheduler.SchId = schlId;
                EmployeeScheduler.Product_price = GetBookedProductInfo.Amount;
                DateTime CurrentDate = fu.ZonalDate(null);
                EmployeeScheduler.CreatedOn = CurrentDate;
                EmployeeScheduler.UpdatedOn = CurrentDate;
                EmployeeScheduler.SchedulerId = SPA.CCTSP_SchedulerMaster.Where(c => c.WeekDay == WeekOfChoosedDate && c.SchId == schlId && c.ActiveStatus == "A").Select(c => c.SchedulerId).FirstOrDefault();
                SPA.SPA_EmployeeScheduler.Add(EmployeeScheduler);
                SPA.SaveChanges();
                //Payment Add And Payment Deduction
               
                fu.RefundPayment(EmpSchId);
                fu.PaymentDeduction(GetBookedProductInfo.Amount, EmployeeScheduler.EmpSchDetailsId, EnterpriseInfo, "A");
                //Add MasterData
                string BookedData = "select a.EmpSchDetailsId,a.comment,a.ActiveStatus,a.BookedStatus,a.BookingDate,b.CatgTypeid as ProductId ,b.CatgDesc as ProductName,a.Product_price,a.FromSlotMasterId ,a.ToSlotMasterId,a.Emp_UserId as EMP_UserId ,a.CLIENT_UserId as CLIENT_UserId, c.sectiondesc as Productdesc, cast(c.Duration as int) as Duration,e.FirstName as ClientName, e.LastName as ClientLastName ,f.FirstName as ShopOwnerName , f.LastName as ShopOwnerLastName,e.EmailId as ClientEmail,e.PhoneNo as ClientPhoneNo, FORMAT(a.CreatedOn ,'dd-MM-yyyy hh:mm') as CreatedDate, CAST(CASE WHEN ISNUMERIC(c.Amount) = 1 THEN c.Amount ELSE NULL END AS decimal(38, 2)) as Amount,d.FirstName,d.LastName,d.Answer2 as Image from SPA_EmployeeScheduler a join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId join CCTSP_User d on d.UserId = a.Emp_UserId  join CCTSP_User e on e.UserId = a.CLIENT_UserId join CCTSP_User f on f.schid = a.schid where a.schid =" + schlId + "  and b.catgid = 111 and c.SchId = " + schlId + " and d.schid = " + schlId + " and f.schid = " + schlId + " and f.RoleId = 1  and f.ActiveStatus = 'A' and a.EmpSchDetailsId = " + EmployeeScheduler.EmpSchDetailsId + "";
                var BookingDetails = SPA.Database.SqlQuery<Models.ConfirmModel>(BookedData).FirstOrDefault();
                fu.AddMasterData(BookingDetails, ActiveStatus, BookedStatus, "M");
                if (!string.IsNullOrEmpty(Session["View"] as string))
                {
                    Session["ViewUserId"] = EmpUserId.ToString();
                    if (!string.IsNullOrEmpty(EmployeeScheduler.BookingDate))
                    {
                        var BDate = Convert.ToDateTime(EmployeeScheduler.BookingDate, enGB);
                        Session["ViewDate"] = BDate.Day + "-" + BDate.AddMonths(-1).ToString("MM") + "-" + BDate.Year;
                    }
                }
                var qry = "";
                var qryemail = "";
                qry = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "App" && c.Activestatus == "A" && c.SchId == schlId && c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgDesc == "SPA SMS").Select(c => c.SectionDesc).FirstOrDefault();
                qryemail = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "App" && c.Activestatus == "A" && c.SchId == schlId && c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgDesc == "SPA Email").Select(c => c.SectionDesc).FirstOrDefault();
                if (!string.IsNullOrEmpty(qry))
                {
                    var qry1 = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmployeeScheduler.EmpSchDetailsId && c.SchId == schlId).FirstOrDefault();
                    var qry2 = SPA.CCTSP_User.Where(a => a.UserId == qry1.CLIENT_UserId).FirstOrDefault();
                    if (qry2 != null)
                    {
                        string name = qry2.Gender + " " + qry2.FirstName + " " + qry2.LastName;
                        string date = qry1.BookingDate;
                        var checkSMSApprove = SPA.CCTSP_User.Where(c => c.UserId == qry1.CLIENT_UserId && (c.SMS_Service == 3 || c.SMS_Service == 2)).FirstOrDefault();
                        if (checkSMSApprove != null)
                            SMS.Approvebooking(name, date, qry2.LoginName);
                    }
                }
                if (!string.IsNullOrEmpty(qryemail))
                {
                    var qry3 = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmployeeScheduler.EmpSchDetailsId && c.SchId == schlId).FirstOrDefault();
                    var qry4 = SPA.CCTSP_User.Where(a => a.UserId == qry3.CLIENT_UserId).FirstOrDefault();
                    if (qry4 != null)
                    {
                        string name = qry4.Gender + " " + qry4.FirstName + " " + qry4.LastName;
                        string date = qry3.BookingDate;
                        var checkEMAILApprove = SPA.CCTSP_User.Where(c => c.UserId == qry3.CLIENT_UserId && (c.Email_Service == 3 || c.Email_Service == 2)).FirstOrDefault();

                        if (checkEMAILApprove != null && PastDate==false)
                            Email.Approvebooking(name, date, qry4.EmailId, qry3.FromSlotMasterId, qry3.EmpSchDetailsId);
                    }
                }

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public JObject GetAllDayDisplay(string date, string status, string from)
        {
            try
            {
                JObject send = null;
                StringBuilder sb = null;
                string DisplayHeading = "";
                string DisplayMainHeading = "";
                if (from == "DayIn")
                {
                    var DateArray = date.Split(' ');
                    DateTime GivenDate = DateTime.Parse(DateArray[1]);
                    DateTime ChangeDate = new DateTime();
                    if (status == "back")
                        ChangeDate = GivenDate.AddDays(-1);
                    else
                        ChangeDate = GivenDate.AddDays(1);
                    DisplayHeading = ChangeDate.DayOfWeek + " " + ChangeDate.Day + "-" + ChangeDate.Month + "-" + ChangeDate.Year;
                    if (GivenDate.Month != ChangeDate.Month || GivenDate.Year != ChangeDate.Year)
                        DisplayMainHeading = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(ChangeDate.Month) + "-" + ChangeDate.Year;

                    sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"DisplayHeading\":\"" + DisplayHeading + "\",");
                    sb.Append("\"DisplaySubheading\":\"" + DisplayMainHeading + "\",");
                    sb.Append("}");
                    send = JObject.Parse(sb.ToString());
                }
                return send;
            }
            catch (Exception e)
            {
                JObject send = null;
                StringBuilder sb = null;
                sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"error\":\"" + "" + "\",");
                sb.Append("}");
                fu.ErrorSend(RouteData, e);
                send = JObject.Parse(sb.ToString());
                return send;
            }

        }
        public ActionResult CancelBooking(int CancelBookingId)
        {
            try
            {
                var CancelBooking = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == CancelBookingId && c.SchId == schlId).FirstOrDefault();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                                   .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, TimeZone = c.TimeZone, CancelResvHours = c.Cancel_Res_Duration })
                                   .FirstOrDefault();
                ViewBag.Language = fu.getLanguageData("Cancel_Link", 0, ShopInfo.Lang_id.Value);
                int ConsiderHours = ShopInfo.CancelResvHours != null ? ShopInfo.CancelResvHours.Value : 24;
                ViewBag.ConsiderHours = ConsiderHours;
                if (CancelBooking.ActiveStatus.ToString().Trim() != "Z" && CancelBooking.BookedStatus.ToString().Trim() != "Z")
                {
                    DateTime CurrentDate = fu.ZonalDate(ShopInfo.TimeZone);
                    DateTime BookingDate = DateTime.Parse(CancelBooking.BookingDate + " " + CancelBooking.FromSlotMasterId, enGB);
                    double Hours = Math.Round((BookingDate - CurrentDate).TotalHours);
                    if (Hours >= ConsiderHours)
                    {
                        var cancelbookingDetails = SPA.CCTSP_User.Where(a => a.UserId == CancelBooking.CLIENT_UserId).FirstOrDefault();
                        CancelBooking.ActiveStatus = "Z";
                        CancelBooking.BookedStatus = "Z";
                        if (CancelBooking.MasterResId != null && CancelBooking.MasterResId > 0)
                        {
                            CancelBooking.spa_Master_Reservation.ActiveStatus = "Z";
                            CancelBooking.spa_Master_Reservation.BookedStatus = "Z";
                        }
                        SPA.SaveChanges();
                        ViewBag.StatusCheck = "Yes";
                        //Payment Refund
                        fu.RefundPayment(CancelBookingId);
                        string name = cancelbookingDetails.Gender + " " + cancelbookingDetails.FirstName + " " + cancelbookingDetails.LastName;
                        string date = CancelBooking.BookingDate;
                        if (cancelbookingDetails != null)
                        {
                            Email.CancelbookingCustomer(name, cancelbookingDetails.EmailId, CancelBooking.FromSlotMasterId, date, CancelBooking.EmpSchDetailsId);
                        }
                    }
                    else
                        ViewBag.StatusCheck = "ContactOwner";
                }
                else
                    ViewBag.StatusCheck = "No";
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult CancelBookingAlert(int BookingId)
        {
            try
            {
                ViewBag.BookingId = BookingId;
                var CancelBookingAlert = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == BookingId).FirstOrDefault();
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = fu.getLanguageData("Cancel_Link", 0, LanguageId);
                if (CancelBookingAlert.ActiveStatus.ToString().Trim() == "Z" && CancelBookingAlert.BookedStatus.ToString().Trim() == "Z")
                {
                    ViewBag.CancelBookingStatus = "Yes";
                    return View();
                }
                else
                {
                    ViewBag.CancelBookingStatus = "No";
                    return View();
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult AcceptBookingAlert(int BookingId)
        {
            try
            {
                ViewBag.BookingId = BookingId;
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Accept_link" && c.Lang_id == LanguageId).ToList();
                var Check = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == BookingId && c.ActiveStatus == "A" && c.BookedStatus == "B").FirstOrDefault();
                if (Check != null)
                    ViewBag.AcceptStatus = "Yes";
                else
                    ViewBag.AcceptStatus = "No";

                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult AcceptBooking(int BookingId)
        {
            try
            {
                var AcceptBooking = SPA.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == BookingId && c.SchId == schlId && c.ActiveStatus == "DA" && c.BookedStatus == "B").FirstOrDefault();
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Accept_link" && c.Lang_id == LanguageId).ToList();
                if (AcceptBooking != null)
                {
                    AcceptBooking.ActiveStatus = "A";
                    AcceptBooking.BookedStatus = "B";
                    if (AcceptBooking.MasterResId != null && AcceptBooking.MasterResId > 0)
                    {
                        AcceptBooking.spa_Master_Reservation.ActiveStatus = "A";
                        AcceptBooking.spa_Master_Reservation.BookedStatus = "B";
                    }
                    SPA.SaveChanges();
                    ViewBag.StatusCheck = "Yes";
                    var AcceptBookingDetails = SPA.CCTSP_User.Where(a => a.UserId == AcceptBooking.CLIENT_UserId).FirstOrDefault();
                    string name = AcceptBookingDetails.Gender + " " + AcceptBookingDetails.FirstName + " " + AcceptBookingDetails.LastName;
                    string date = AcceptBooking.BookingDate;
                    if (AcceptBookingDetails != null)
                        Email.Approvebooking(name, date, AcceptBookingDetails.EmailId, AcceptBooking.FromSlotMasterId, AcceptBooking.EmpSchDetailsId);
                }
                else
                    ViewBag.StatusCheck = "No";

                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult CancelFeedback()
        {
            try
            {
                ViewBag.CancelFeedback = "Yes";
                int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Cancel_Link" && c.Lang_id == LanguageId).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public void ApproveAllBooking()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var query = "update SPA_EmployeeScheduler set ActiveStatus='A',BookedStatus='B' where ActiveStatus='DA' and BookedStatus='B' and SchId=" + schlId;
                SPA.Database.ExecuteSqlCommand(query);
                //}
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        /*Khatri code starts*/
        public void SelectAllAppointment(string EmpschIdList)
        {
            DateTime EuropeDate = fu.ZonalDate(null);
            try
            {
                if (EmpschIdList != "")
                {
                    List<int> EMPIDLIST = new List<int>();
                    EMPIDLIST = EmpschIdList.Split(' ').Select(i => int.Parse(i)).ToList();
                    List<SPA.SPA_EmployeeScheduler> BookingData = SPA.SPA_EmployeeScheduler.Where(c => c.SchId == schlId && EMPIDLIST.Contains(c.EmpSchDetailsId)).ToList();
                    foreach (var item in BookingData)
                    {
                        item.ActiveStatus = "A";
                        item.BookedStatus = "B";
                        item.UpdatedOn = EuropeDate;
                        if (item.MasterResId != null && item.MasterResId > 0)
                        {
                            item.spa_Master_Reservation.ActiveStatus = "A";
                            item.spa_Master_Reservation.BookedStatus = "B";
                        }
                    }
                    SPA.SaveChanges();
                    var EnterpriseInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                    fu.MultiplePaymentDeduction(BookingData, EnterpriseInfo, "A");
                    fu.SendMultipleEmail(EmpschIdList);
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void BackToPending(int? EmpSchDetailsId)
        {
            string EuropeDate = fu.ZonalDate(null).ToString("yyyy-MM-dd hh:mm:ss");
            try
            {
                var query = "update SPA_EmployeeScheduler set ActiveStatus='A',BookedStatus='B', UpdatedOn='" + EuropeDate + "' where EmpSchDetailsId=" + EmpSchDetailsId;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        /*Khatri Code Ends*/
        //Code For Checking Whether data Exist or not
        public string CheckReservation(string EmpUserId, string Dates)
        {
            string Result = "";
            try
            {
                string originalDate = Dates.Split(' ')[0].Split('-')[2] + "-" + Dates.Split(' ')[0].Split('-')[1] + "-" + Dates.Split(' ')[0].Split('-')[0] + " " + Dates.Split(' ')[1];
                var Query = "select * from spa_employeeScheduler where schId = " + schlId + "  and cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) <= '" + originalDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + originalDate + "' and ((ActiveStatus = 'DA' and BookedStatus = 'B') or(ActiveStatus = 'A' and BookedStatus = 'B') or(ActiveStatus = 'C' and BookedStatus = 'C')) and Emp_UserId=" + EmpUserId;
                var GetAllData = SPA.Database.SqlQuery<SPA_EmployeeScheduler>(Query).ToList();
                if (GetAllData.Count > 0)
                    Result = "No";
                else
                    Result = "yes";
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
            return Result;
        }
        public JsonResult getCalendarConverted()
        {
            var langId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault();
            var qryLang = "select * from Language_Label_Detail where Page_name='Calender_view' and Lang_id=" + langId + " and (order_id between 1 and 6 or order_id=27 or order_id=29 ) order by order_id";
            var Output = SPA.Database.SqlQuery<Language_Label_Detail>(qryLang).ToList();
            return Json(Output, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReservationPopup()
        {
            return View();
        }
        public ActionResult HolidayCalendar()
        {
            return View();
        }
        public ActionResult AppClosedAndRedirectToInvoice(long ReservationId,string Url)
        {
            try
            {
                if (ReservationId > 0)
                {
                    StatusFinalChange(ReservationId, "C", "Checked");
                    return Redirect("/Invoice/Invoice1?ReservationIdList=" + ReservationId + "&Url="+ Url);
                }
                else
                    return RedirectToAction("Error_List", "Error");

            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult InvoiceRedirectFromCalendar(string View, string AllView, string Date, long? EmpUserId,long ReservationId,long?InvoiceId,string InvoiceStatus)
        {
            try
            {
                //Redirection to calendar
                fu.setRedirectionofCalendar(View, AllView, Date, EmpUserId);
                //Go to InvoiceCreate
                var Url = "/Invoice/Invoice1?ReservationIdList=" + ReservationId + "&Url=calendarmonth";
                if(!string.IsNullOrEmpty(InvoiceStatus))
                {
                    if (InvoiceStatus == "CI")
                        Url = "/Invoice/Invoice1?InvoiceId=" + InvoiceId + "&Url=calendarmonth";
                    else
                        Url = "/PDF/InvoicePrint?InvoiceId=" + InvoiceId + "&IsPrint=" + false + "&Url=calendarmonth&Status=D";
                }
                return Redirect(Url);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData,e);
                return RedirectToAction("Error_List", "Error");
            }
        } 
        public ActionResult CheckInvoiceStatusAndRedirectToInvoice(long Reservationid,string Url)
        {

            var RedirectUrl = "/Invoice/Invoice1?ReservationIdList=" + Reservationid + "&Url="+ Url;
            var InvoiceDetails = fu.CheckInvoiceStatus(Reservationid);
            if (!string.IsNullOrEmpty(InvoiceDetails.InvoiceStatus))
            {
                if (InvoiceDetails.InvoiceStatus == "CI")
                    RedirectUrl = "/Invoice/Invoice1?InvoiceId=" + InvoiceDetails.InvoiceId + "&Url=" + Url;
                else
                    RedirectUrl = "/PDF/InvoicePrint?InvoiceId=" + InvoiceDetails.InvoiceId + "&IsPrint=" + false + "&Url=" + Url + "&Status=D";
            }
            return Redirect(RedirectUrl);
        }
        public ActionResult NoteRedirectFromCalendar(string View, string AllView, string Date, long? EmpUserId, long ReservationId)
        {
            try
            {
                //Redirection to calendar
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail{SchlStudentStrength = c.SchlStudentStrength}).FirstOrDefault();
                fu.setRedirectionofCalendar(View, AllView, Date, EmpUserId);
                return Redirect("/Doctor/DocPre?Reservationid="+ ReservationId + "&diff="+ShopInfo.SchlStudentStrength+ "&Url=calendarmonth");
            }
            catch(Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }       
    }
}