using SPA.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Xml;
using Newtonsoft.Json;
using System.Web.Hosting;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace SPA.Controllers
{
    [checkShop]
    [CustomAutohrize(null)]
    public class InvoiceController : Controller
    {
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        CultureInfo enGB = new CultureInfo("en-GB");
        Function fu = new Function();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        Sql sql = new Sql();
        JavaScriptSerializer js = new JavaScriptSerializer();
        PushEmail Email = new PushEmail();
        public InvoiceController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: Invoice
        public ActionResult Invoicing()
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.shopMaster
                {
                    LangId = c.Lang_id,
                    Flow_Id = c.Flow_Id

                }).FirstOrDefault();
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Invoicing_Tab" && c.Lang_id == ShopInfo.LangId.Value).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                ViewBag.SubMenu = fu.AccessSubMenu(ShopInfo.LangId.Value, Convert.ToInt32(Session["RoleId"].ToString()), "Invoicing_Tab", ShopInfo.Flow_Id);
                return View();
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult Forinvoicing()
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var InvoiceList = fu.InvoiceList(null, UserId);
                if (InvoiceList.Count == 0)
                {
                    ViewBag.AddAccess = SPA.Database.SqlQuery<int>(new Sql().checkinsertAccess(UserId, "For Invoicing")).FirstOrDefault();
                }
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.shopMaster
                {
                    LangId = c.Lang_id
                }).FirstOrDefault();
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "For_Invoicing" && c.Lang_id == ShopInfo.LangId.Value).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                return View(InvoiceList);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult InvoiceGenerated(string FromDate, string ToDate)
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    CreateDate = c.CreatedOn,
                    Flow_Id=c.Flow_Id

                }).FirstOrDefault();
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Invoice_Generated" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                bool ReservationTabAccess = true;
                if (ShopInfo.Flow_Id > 1)
                    ReservationTabAccess = fu.CheckTabAccess("Reservation", ShopInfo.Flow_Id.Value);
                ViewBag.ReservationTabAccess = ReservationTabAccess;
                ViewBag.LngLocal = fu.SmallCalLangTranslate(ShopInfo.Lang_id);
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.CreateDate = ShopInfo.CreateDate;
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                return View(SPA.Database.SqlQuery<Models.Invoice>(sql.InvoiceGenerated("CI", schlId, FromDate, ToDate, UserId)).ToList());
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult OpenInvoices(string FromDate, string ToDate)
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    TimeZone = c.TimeZone,
                    ExtendDuration = c.Extend_Pay_Duration,
                    CreateDate = c.CreatedOn,
                    Flow_Id=c.Flow_Id

                }).FirstOrDefault();
                DateTime CurrentDate = fu.ZonalDate(ShopInfo.TimeZone);
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Open Invoices" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                ViewBag.LngLocal = fu.SmallCalLangTranslate(ShopInfo.Lang_id);
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.CreateDate = ShopInfo.CreateDate;
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var getInvoice = SPA.Database.SqlQuery<Models.Invoice>(sql.OpenInvoice("'CIP','CIPS','CIS','REMEP','REME','REMP'", schlId, FromDate, ToDate, UserId)).ToList();
                getInvoice = getInvoice.Select(c => new Models.Invoice
                {
                    ReservationId = c.ReservationId,
                    CustomerFName = c.CustomerFName,
                    CustomerLName = c.CustomerLName,
                    ProductName = c.ProductName,
                    CustomerEmail = c.CustomerEmail,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    BookingDate = c.BookingDate,
                    Duration = c.Duration,
                    ProductPrice = c.ProductPrice,
                    Amount = c.Amount,
                    CustomerId = c.CustomerId,
                    EmployeeId = c.EmployeeId,
                    Currency = c.Currency,
                    Invoice_Id = c.Invoice_Id,
                    Invoice_Service = c.Invoice_Service,
                    CreatedDate = c.CreatedDate,
                    CountReservation = c.CountReservation,
                    INVOICINGDATE = c.INVOICINGDATE,
                    DueDate = c.DueDate,
                    overdue = c.overdue,
                    Index = c.Index,
                    Paid_Date = c.Paid_Date,
                    Reminderdate1 = c.Reminderdate1,
                    Reminderdate2 = c.Reminderdate2,
                    Reminderdate3 = c.Reminderdate3,
                    Remaining_Amount = c.Remaining_Amount,
                    insertAccess = c.insertAccess,
                    UpdateAccess = c.UpdateAccess,
                    DeleteAccess = c.DeleteAccess,
                    FlowStatus = c.FlowStatus,
                    DisplayStatus = c.DisplayStatus = c.Reminderdate1 == null && c.Reminderdate2 == null && c.Reminderdate3 == null && c.DueDate > CurrentDate ? true : fu.ShowReminderButton(c, CurrentDate, ShopInfo.ExtendDuration)
                }).ToList();
                //getInvoice.All(c => {c.DisplayStatus= c.Reminderdate1 == null && c.Reminderdate2 == null && c.Reminderdate3 == null && c.DueDate > CurrentDate ?true:fu.ShowReminderButton(c,CurrentDate,ShopInfo.ExtendDuration); return false; });
                return View(getInvoice);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult ClosedInvoices(int? Month, int? year, string FromDate, string ToDate)
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.Lang_id,
                    Schlid = c.SchlId,
                    Flow_Id=c.Flow_Id
                }).FirstOrDefault();
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && (c.Page_Name == "Closed_Invoices" || c.Page_Name == "Short_Month_Name") && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Label_Name = c.Label_Name
                }).ToList();
                int Year = year != null ? year.Value : DateTime.Now.Year;
                int IMonth = Month != null ? Month.Value : DateTime.Now.Month;
                ViewBag.year = Year;
                ViewBag.Month = IMonth;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.LngLocal = fu.SmallCalLangTranslate(ShopInfo.Lang_id);
                bool ReservationTabAccess = true;
                if (ShopInfo.Flow_Id > 1)
                    ReservationTabAccess = fu.CheckTabAccess("Reservation", ShopInfo.Flow_Id.Value);
                ViewBag.ReservationTabAccess = ReservationTabAccess;
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                return View(SPA.Database.SqlQuery<Models.Invoice>(sql.ClosedInvoice("'CMP','CMPEP','CMPE','CMPP','DCMP'", schlId, IMonth, Year, FromDate, ToDate, UserId)).ToList());
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        //public Models.InvoiceStatus AddInvoice(string Reservations, int? LangId, string Zonal)
        public Models.InvoiceStatus AddInvoice(string Reservations, Models.ShopMasterDetail ShopInfo)
        {
            Models.InvoiceStatus InvStatus = new Models.InvoiceStatus();
            InvStatus.ReservationList = new List<long>();
            List<SPA_INVOICE_MASTER> InvoiceMASTERList = new List<SPA_INVOICE_MASTER>();
            List<SPA_INVOICE_Detail> InvoiceDetailList = new List<SPA_INVOICE_Detail>();
            InvStatus.status = false;
            string Status = null;
            try
            {

                var Date = fu.ZonalDate(ShopInfo.TimeZone);
                if (string.IsNullOrEmpty(Reservations))
                    return InvStatus;
                var Customerqry = " select a.EmpSchDetailsId as ReservationId,a.Client_UserId as CustomerId,a.BookingDate," +
                                    " a.Emp_UserId as EmployeeId,b.FirstName as CustomerFirstName," +
                                    " b.LastName as CustomerLastName,c.FirstName as EmployeeFirstName," +
                                    " c.LastName as EmployeeLastName,a.schId as SchlId,ISNULL(c.ZSR_No,'0') as ZSR_No,b.DOB,ISNULL(b.Pincode,0) as Pincode," +
                                    " a.Product_Id,d.CatgDesc as ProductName,convert(float, e.Amount) as Amount,convert(int,e.Duration) as Duration,ISNULL(d.VAT,0.0) as VAT,b.Invoice_Service" +
                                    " from SPA_EmployeeScheduler a" +
                                    " JOIN CCTSP_USER b on b.UserId = a.CLIENT_USERId" +
                                    " JOIN CCTSP_USER c on c.USERID = a.Emp_USerId" +
                                    " JOIN cctsp_CategoryDetails d on d.CatgTypeId = a.Product_Id" +
                                    " JOIN Ctsp_SolutionMaster e on e.CatgTypeId = d.CatgTypeId" +
                                    " where a.SchId = " + schlId + " and a.ActiveStatus = 'C' and a.BookedStatus = 'C'" +
                                    " and a.EmpSchDetailsId in (" + Reservations + ")";
                var CustomerList = SPA.Database.SqlQuery<Models.INVOICE_MASTER_LOCAL>(Customerqry).ToList();
                if (CustomerList.Select(c => c.CustomerId).Distinct().Count() > 1 || CustomerList.Select(c => c.EmployeeId).Distinct().Count() > 1)
                    Status = "CI";
                foreach (var item in CustomerList.Select(c => c.CustomerId).Distinct().ToList())
                {
                    #region Customercode
                    var EmployeeWiseData = CustomerList.Where(c => c.CustomerId == item).ToList();
                    foreach (var Data in EmployeeWiseData.Select(c => c.EmployeeId).Distinct().ToList())
                    {
                        #region EmployeeCode
                        var EmpFirst = CustomerList.Where(c => c.CustomerId == item && c.EmployeeId == Data).FirstOrDefault();
                        SPA.SPA_INVOICE_MASTER Invoice = new SPA_INVOICE_MASTER()
                        {
                            ActiveStatus = "A",
                            SchlId = schlId,
                            CreatedDate = Date,
                            UpdatedDate = Date,
                            CustomerId = item,
                            EmployeeId = Data,
                            CustomerFirstName = EmpFirst.CustomerFirstName,
                            CustomerLastName = EmpFirst.CustomerLastName,
                            EmployeeFirstName = EmpFirst.EmployeeFirstName,
                            EmployeeLastName = EmpFirst.EmployeeLastName,
                            Invoice_Status = Status
                        };
                        InvoiceMASTERList.Add(Invoice);

                        //SPA.SPA_INVOICE_MASTER.Add(Invoice);
                        //SPA.SaveChanges();
                        #endregion
                    }
                    #endregion
                }
                if (InvoiceMASTERList.Count > 0)
                {
                    SPA.SPA_INVOICE_MASTER.AddRange(InvoiceMASTERList);
                    SPA.SaveChanges();
                    if (CustomerList.Select(c => c.CustomerId).Distinct().Count() > 1 || CustomerList.Select(c => c.EmployeeId).Distinct().Count() > 1)
                    {
                        /*Add Data to Invoice Detail list*/
                        var ModelsData = CustomerList
                                        .Select(c => new SPA_INVOICE_Detail
                                        {
                                            ActiveStatus = "A",
                                            ReservationId = c.ReservationId,
                                            CreatedDate = Date,
                                            UpdatedDate = Date,
                                            SchlId = schlId,
                                            ProductName = c.ProductName,
                                            Product_Id = c.Product_Id,
                                            Duration = c.Duration,
                                            Quantity = c.Duration / 5,
                                            FiveMinuteChargeorPercharge = c.Amount * 5 / c.Duration,
                                            TotalPrice = c.Duration / 5 * c.Amount * 5 / c.Duration,
                                            Invoice_Id = InvoiceMASTERList.Where(d => d.EmployeeId == c.EmployeeId && d.CustomerId == c.CustomerId).Select(d => d.Invoice_Id).FirstOrDefault(),
                                            Date_Of_Buying = c.BookingDate != null ? DateTime.Parse(c.BookingDate, enGB) : new Nullable<DateTime>(),
                                            TotalPricewithvat = (c.Duration / 5 * c.Amount * 5 / c.Duration) + (((c.Duration / 5 * c.Amount * 5 / c.Duration) * c.VAT) / 100),
                                            VAT = c.VAT
                                        }).ToList();
                        if (ModelsData.Count > 0)
                        {
                            SPA.SPA_INVOICE_Detail.AddRange(ModelsData);
                            SPA.SaveChanges();
                            fu.GenerateGroup2dMatrix(InvoiceMASTERList.Select(c => new Models.twodMatrix
                            {
                                ZSRNO = CustomerList.Where(d => d.EmployeeId == c.EmployeeId).Select(d => d.ZSR_No).FirstOrDefault(),
                                CustBdate = CustomerList.Where(d => d.CustomerId == c.CustomerId).Select(d => d.DOB).FirstOrDefault(),
                                custZip = CustomerList.Where(d => d.CustomerId == c.CustomerId).Select(d => d.Pincode).FirstOrDefault(),
                                FirstTreatment = ModelsData.Where(d => d.SPA_INVOICE_MASTER.CustomerId == c.CustomerId && d.SPA_INVOICE_MASTER.EmployeeId == c.EmployeeId && d.ReservationId != null).Select(d => d.Date_Of_Buying).Min(),
                                invoiceDate = InvoiceMASTERList.Where(d => d.CustomerId == c.CustomerId && d.EmployeeId == c.EmployeeId).Select(d => d.UpdatedDate).FirstOrDefault(),
                                Amount = Convert.ToDecimal(ModelsData.Where(d => d.SPA_INVOICE_MASTER.CustomerId == c.CustomerId && d.SPA_INVOICE_MASTER.EmployeeId == c.EmployeeId && d.TotalPricewithvat != null).Select(d => d.TotalPricewithvat.Value).Sum()),
                                InvoiceId = InvoiceMASTERList.Where(d => d.CustomerId == c.CustomerId && d.EmployeeId == c.EmployeeId).Select(d => d.Invoice_Id).FirstOrDefault(),
                                ESRCODE = fu.MakeESRCODE(ShopInfo.Currency, Convert.ToDecimal(ModelsData.Where(d => d.Invoice_Id == c.Invoice_Id).Select(d => d.TotalPricewithvat).Sum().Value), ShopInfo.AccountNumber)

                            }).ToList());
                            fu.AddDefaultDropValues(ShopInfo.Lang_id, string.Join(",", InvoiceMASTERList.Select(c => c.Invoice_Id).Distinct().ToList()));
                        }
                        InvStatus.url = "/Invoice/Invoicing#Forinvoicing";
                    }
                }
                List<long> InvoiceList = new List<long>();
                InvoiceList = InvoiceMASTERList.Select(c => c.Invoice_Id).Distinct().ToList();
                fu.UpdateInvoicestatus(Status, InvoiceList, Date, false);
                InvStatus.status = true;
                InvStatus.ReservationList.AddRange(InvoiceMASTERList.Select(c => c.Invoice_Id).Distinct().ToList());
                InvStatus.InvoiceDate = Date;
            }
            catch (Exception e)
            {
                return InvStatus;
            }
            return InvStatus;
        }
        public void RemoveReservation(List<long> ReservationList)
        {
            try
            {
                if (ReservationList.Count > 0)
                    SPA.Database.ExecuteSqlCommand("update spa_employeeScheduler set Invoice_Id=null where EmpSchDetailsId in ('" + string.Join(",", ReservationList) + "')");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
            }
        }
        public ActionResult Invoice1(string ReservationIdList, long? InvoiceId, string Url)
        {
            try
            {
                var Invoice = new Models.InvoiceStatus();
                List<Models.ManualInvoice> ManualInvoiceList = new List<Models.ManualInvoice>();
                if (!string.IsNullOrEmpty(ReservationIdList) || InvoiceId != null)
                {
                    var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
                    {
                        Lang_id = c.Lang_id,
                        SchlStudentStrength = c.SchlStudentStrength,
                        TimeZone = c.TimeZone,
                        Display_Invoice = c.Display_Invoice,
                        Invoice_FreeText = c.Invoice_FreeText,
                        CreateDate = c.CreatedOn,
                        Currency = c.Currency,
                        AccountNumber = c.BANKACCOUNT,
                        Flow_Id = c.Flow_Id                
                    }).FirstOrDefault();
                    if (ShopInfo.Display_Invoice.Value == 2)
                        return this.GeneralInvoice(null, ReservationIdList, InvoiceId, Url, ShopInfo);
                    if (InvoiceId == null)
                    {
                        Invoice = AddInvoice(ReservationIdList, ShopInfo);
                        if (!string.IsNullOrEmpty(Invoice.url))
                        {
                            Session["Message"] = "Forinvoicing_CI";
                            return Redirect(Invoice.url);
                        }

                    }
                    var Date = fu.ZonalDate(ShopInfo.TimeZone);
                    var InvoiceDetails = new List<Models.ManualInvoice>();
                    if (InvoiceId != null)
                        InvoiceDetails = fu.PrintMailManualInvoiveList(Convert.ToString(InvoiceId.Value));
                    else
                        InvoiceDetails = fu.ManualInvoiveList(ReservationIdList, null);
                    if (InvoiceId != null)
                    {
                        List<long> ReservationList = new List<long>();
                        ReservationList.Add(InvoiceId.Value);
                        Invoice.InvoiceDate = InvoiceDetails.Count > 0 ? InvoiceDetails.Select(c => c.InvoiceDate).FirstOrDefault() : Date;
                        Invoice.ReservationList = ReservationList;
                    }
                    if (InvoiceDetails != null)
                        ManualInvoiceList = InvoiceDetails;
                    var CatgNameList = "Invoice_Law,reimbursement,Reason for treatment,Invoice_Diagnose,Invoice_Therapie,Invoice_Rate,New_Gender";
                    ViewBag.DropDownList = fu.getInvoiceCatgDetails(CatgNameList, ShopInfo.Lang_id.Value);
                    ViewBag.ReservationIdList = ReservationIdList;
                    ViewBag.AddOnProductList = fu.AddOnProductList();
                    ViewBag.settlementtextList = SPA.Database.SqlQuery<Models.settlementTxt>(new Sql().getSettlement(schlId, true, ShopInfo.Lang_id.Value,"1")).ToList();
                    ViewBag.Invoice = Invoice;
                    ViewBag.Url = Url;
                    ViewBag.LngLocal = fu.SmallCalLangTranslate(ShopInfo.Lang_id);
                    bool ServiceTabAccess = true;
                    if (ShopInfo.Flow_Id > 1)
                        ServiceTabAccess = fu.CheckSubTabAccess("Product_Catalog", ShopInfo.Flow_Id.Value);
                    ViewBag.ServiceTabAccess = ServiceTabAccess;
                    bool ReservationTabAccess = true;
                    if (ShopInfo.Flow_Id > 1)
                        ReservationTabAccess = fu.CheckTabAccess("Reservation", ShopInfo.Flow_Id.Value);
                    ViewBag.ReservationTabAccess = ReservationTabAccess;
                    ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Invoice_Page" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                    {
                        Order_id = c.Order_id,
                        Lang_id = c.Lang_id,
                        English_Label = c.English_Label,
                        Value = c.Value,
                        Page_Name = c.Page_Name
                    }).ToList();
                    ViewBag.ShopInfo = ShopInfo;
                    var TreatDate = "";
                    var TreatToDate = "";
                    var ServicesList = new List<Models.InvoiceServicesList>();
                    if (ManualInvoiceList.Count > 0)
                    {
                        TreatDate = InvoiceDetails.Select(c => c.BookingDate).FirstOrDefault() != null ? DateTime.Parse(InvoiceDetails.OrderBy(c => DateTime.Parse(c.BookingDate, enGB)).Select(c => c.BookingDate).FirstOrDefault(), enGB).ToString("dd.MM.yyyy") : "";
                        TreatToDate = InvoiceDetails.Select(c => c.BookingDate).FirstOrDefault() != null ? DateTime.Parse(InvoiceDetails.OrderBy(c => DateTime.Parse(c.BookingDate, enGB)).Select(c => c.BookingDate).LastOrDefault(), enGB).ToString("dd.MM.yyyy") : "";
                        ServicesList = fu.ServicesList(InvoiceDetails.Select(c => c.EmpId).FirstOrDefault().Value, schlId);
                    }
                    ViewBag.TreatDate = TreatDate;
                    ViewBag.TreatToDate = TreatToDate;
                    ViewBag.ServicesList = ServicesList;
                    return View(ManualInvoiceList);
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
        public string ManualInvoice(FormCollection InvoiceIdDetails)
        {
            try
            {
                List<long> ReservationList = new List<long>();
                var Data = "Error";
                if (InvoiceIdDetails.AllKeys.Where(c => c.Contains("ReservationList")).Count() > 0)
                    ReservationList = InvoiceIdDetails["ReservationList"].Split(',').Select(c => Convert.ToInt64(c)).ToList();
                if (ReservationList.Count > 0)
                    Data = string.Join(",", ReservationList);
                return Data;
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return "Error";
            }
        }
        [ValidateInput(false)]
        public void AddManualInvoice(Models.ManualInvoice Invoice, string Status)
        {
            try
            {
                if (Invoice != null)
                {
                    //Add InvoiceDetails
                    DateTime CurrentDate = fu.ZonalDate(null);
                    if (Invoice.InvoiceId != null)
                    {
                        List<long> InvoiceIdList = new List<long>();
                        InvoiceIdList.Add(Invoice.InvoiceId.Value);
                        fu.UpdateInvoicestatus("CI", InvoiceIdList, CurrentDate, true);
                        var DeleteQuery = "update SPA_INVOICE_Detail set ActiveStatus='D' where ActiveStatus='A' and Invoice_Id=" + Invoice.InvoiceId;
                        var DeletePreDetails = DeleteQuery + "  " + "update Invoice_CategoryDetails set ActiveStatus='D' where  ActiveStatus='A' and InvoiceId=" + Invoice.InvoiceId;
                        SPA.Database.ExecuteSqlCommand(DeletePreDetails);
                    }
                    fu.ManualInvoiceAdd(Invoice, CurrentDate);
                    Session["Message"] = Invoice.UrlStatus + "_" + Status;

                }
                // return Redirect("/Invoice/Invoicing#Forinvoicing");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                // return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult MailInvoice(string InvoiceId, string status, string url)
        {
            try
            {
                fu.UpdateInvoicestatus(status, InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).ToList(), null, true);
                fu.MailInvoice(InvoiceId, status, false, "NO");
                if (!string.IsNullOrEmpty(url))
                {
                    if (url == "openlistview" || url == "calendarmonth" || url == "Appclosed")
                        return Redirect("/Reservation/Reservation#" + url);
                    else
                        return Redirect("/Invoice/Invoicing#" + url);
                }
                else
                    return RedirectToAction("Invoicing", "Invoice");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult PrintInvoice(string InvoiceId, string status, string url)
        {
            try
            {
                fu.UpdateInvoicestatus(status, InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).ToList(), null, true);
                return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = InvoiceId, Status = status, IsPrint = true, Url = url, schlId = schlId, RStatus = "NO" });
                //return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = InvoiceId, Status = status });
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult MailPrintInvoice(string InvoiceId, string status, string url)
        {
            try
            {
                fu.MailInvoice(InvoiceId, status, false, "NO");
                fu.UpdateInvoicestatus(status, InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).ToList(), null, true);
                return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = InvoiceId, Status = status, IsPrint = true, Url = url, schlId = schlId, RStatus = "NO" });
                //return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = InvoiceId, Status = status });
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult MailPrintMultiple(string InvoiceId, string url)
        {
            try
            {
                if (InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).Count() > 0)
                {
                    var getInvoiceDetail = SPA.Database.SqlQuery<Models.InvoiceCustomerDetails>(sql.InvoiceCustomerDetails(InvoiceId)).ToList();
                    List<long?> InvoiceList = new List<long?>();
                    if (getInvoiceDetail.Count > 0)
                    {
                        fu.UpdateInvoiceStatusAuto(InvoiceId, fu.ZonalDate(getInvoiceDetail.Select(c => c.TimeZone).FirstOrDefault()), true);
                        InvoiceList = getInvoiceDetail.Where(c => (c.status == "CIS" || c.status == "CIPS")).Select(c => c.Invoice_Id).ToList();
                        if (InvoiceList.Count > 0)
                            fu.MailInvoice(string.Join(",", InvoiceList), "CIS", false, "NO");
                        InvoiceList = getInvoiceDetail.Where(c => (c.status == "CIP" || c.status == "CIPS")).Select(c => c.Invoice_Id).ToList();
                        if (InvoiceList.Count > 0)
                            return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = InvoiceId, Status = "CIP", IsPrint = true, Url = url, schlId = schlId, RStatus = "NO" });
                        //return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = string.Join(",", InvoiceList), Status = "CIP" });
                        else
                            return Redirect("/Invoice/Invoicing#" + url);
                    }
                    else
                        return Redirect("/Invoice/Invoicing#" + url);
                }
                else
                    return Redirect("/Invoice/Invoicing#" + url);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult InvoiceAddOnProduct(List<Models.AddOnProductList> AddOnProductList, string Currency, List<Models.LanguageLabelDetails> Language, string IStatus, DateTime InvoiceDate)
        {
            ViewBag.Currency = Currency;
            ViewBag.Language = Language;
            ViewBag.IStatus = IStatus;
            ViewBag.InvoiceDate = InvoiceDate;
            return View(AddOnProductList);
        }
        public ActionResult InvoiceServicesList(List<Models.InvoiceServicesList> ServicesList, string Currency, List<Models.LanguageLabelDetails> Language, long CustomerId, string IStatus, DateTime InvoiceDate)
        {
            ViewBag.Currency = Currency;
            ViewBag.Language = Language;
            ViewBag.CustomerId = CustomerId;
            ViewBag.IStatus = IStatus;
            ViewBag.InvoiceDate = InvoiceDate;
            return View(ServicesList);
        }
        public ActionResult InvoiceSettlementTextList(List<Models.settlementTxt> SettlementTextList, string Currency, List<Models.LanguageLabelDetails> Language, long CustomerId, string IStatus, DateTime InvoiceDate)
        {
            ViewBag.Currency = Currency;
            ViewBag.Language = Language;
            ViewBag.CustomerId = CustomerId;
            ViewBag.IStatus = IStatus;
            ViewBag.InvoiceDate = InvoiceDate;
            return View(SettlementTextList);
        }
        [HttpPost]
        public ActionResult InvoiceAddOnProductAdd(Models.AddOnProductInvoiceAddList IAddOnProductList, string Status, string Istatus)
        {
            try
            {
                List<Models.DisplayAddOnProductInvoiceAddList> DisplayAddOnList = new List<Models.DisplayAddOnProductInvoiceAddList>();
                Models.DisplayAddOnProductInvoiceAddList DisplayAddOn = new Models.DisplayAddOnProductInvoiceAddList();
                ViewBag.Istatus = Istatus;
                if (IAddOnProductList.IAddOnProductId != null)
                {

                    foreach (var Id in IAddOnProductList.IAddOnProductId.Distinct().ToList())
                    {
                        if (Id > 0)
                        {
                            DisplayAddOn = new Models.DisplayAddOnProductInvoiceAddList();
                            var Index = IAddOnProductList.IAddOnProductId.IndexOf(Id);
                            if (Status == "Reservation" || Status == "AddServices")
                                DisplayAddOn.PReservationid = IAddOnProductList.IAddOnProductId[Index];
                            else if (Status == "AddOnSettlementText")
                                DisplayAddOn.AddOnSettlementTextId = IAddOnProductList.IAddOnProductId[Index];
                            else
                                DisplayAddOn.AddOnProductId = IAddOnProductList.IAddOnProductId[Index];
                            DisplayAddOn.ProductName = IAddOnProductList.ProductName[Index];
                            DisplayAddOn.Add_OnQuantity = IAddOnProductList.Add_OnQuantity[Index];
                            DisplayAddOn.Rate = IAddOnProductList.Rate[Index];
                            DisplayAddOn.Add_OnDate = IAddOnProductList.Add_OnDate[Index].ToString("yyyy-MM-dd") != "0001-01-01" ? IAddOnProductList.Add_OnDate[Index] : IAddOnProductList.InvoiceDate[Index];
                            if (IAddOnProductList.Add_OnTime != null)
                                DisplayAddOn.Add_OnTime = IAddOnProductList.Add_OnTime.ElementAtOrDefault(Index);
                            if (IAddOnProductList.Duration != null)
                                DisplayAddOn.Duration = IAddOnProductList.Duration.ElementAtOrDefault(Index);
                            if (IAddOnProductList.Tarif_Number != null)
                                DisplayAddOn.Tarif_Number = IAddOnProductList.Tarif_Number.ElementAtOrDefault(Index);
                            if (IAddOnProductList.Settlement_text != null)
                                DisplayAddOn.Settlement_text = IAddOnProductList.Settlement_text.ElementAtOrDefault(Index);
                            if (IAddOnProductList.Settlement_Number != null && IAddOnProductList.Settlement_Number.Count > 0)
                                DisplayAddOn.Settlement_Number = IAddOnProductList.Settlement_Number.ElementAtOrDefault(Index);
                            if (IAddOnProductList.vat != null)
                                DisplayAddOn.vat = IAddOnProductList.vat.ElementAtOrDefault(Index);
                            if (IAddOnProductList.EmpId != null)
                                DisplayAddOn.EmpId = IAddOnProductList.EmpId.ElementAtOrDefault(Index);
                            if (IAddOnProductList.CustomerId != null)
                                DisplayAddOn.CustomerId = IAddOnProductList.CustomerId.ElementAtOrDefault(Index);
                            DisplayAddOnList.Add(DisplayAddOn);

                        }
                    }
                    if (Status == "AddServices" && DisplayAddOnList.Count > 0)
                    {
                        var ReservationList = DisplayAddOnList.Select(c => new SPA_EmployeeScheduler
                        {
                            Product_Id = c.PReservationid,
                            CLIENT_UserId = c.CustomerId,
                            EMP_UserId = c.EmpId,
                            SchId = schlId,
                            ActiveStatus = "I",
                            BookedStatus = "I",
                            BookingDate = c.Add_OnDate.ToString("yyyy-MM-dd") == "0001-01-01" ? DateTime.Now.ToString("dd MMMM, yyyy") : c.Add_OnDate.ToString("dd MMMM, yyyy"),
                            CreatedOn = DateTime.Now,
                            UpdatedOn = DateTime.Now,
                        }).ToList();
                        SPA.SPA_EmployeeScheduler.AddRange(ReservationList);
                        SPA.SaveChanges();
                        DisplayAddOnList = DisplayAddOnList.Select(c => new Models.DisplayAddOnProductInvoiceAddList
                        {
                            PReservationid = ReservationList.Where(d => d.Product_Id == c.PReservationid).Select(d => d.EmpSchDetailsId).FirstOrDefault(),
                            Add_OnDate = c.Add_OnDate,
                            Add_OnTime = c.Add_OnTime,
                            ProductName = c.ProductName,
                            Add_OnQuantity = c.Add_OnQuantity,
                            Settlement_Number = c.Settlement_Number,
                            Settlement_text = c.Settlement_text,
                            vat = c.vat,
                            Duration = c.Duration,
                            Tarif_Number = c.Tarif_Number,
                            Rate = c.Rate
                        }).ToList();
                    }

                }
                return View(DisplayAddOnList);
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public JsonResult CompleteInvoice(Models.AddpartialPayment info)
        {
            Models.BasicInfo_PartialPopup Basicinfo = new Models.BasicInfo_PartialPopup();
            Basicinfo.FullPayment = 0;
            Basicinfo.Msg = "Error";
            try
            {
                var jobj = new JObject();
                if (info != null)
                {
                    var Currentdate = fu.ZonalDate(null);
                    DateTime dt = Currentdate;
                    if (!string.IsNullOrEmpty(info.Pay_Date))
                        dt = DateTime.Parse(info.Pay_Date, enGB);
                    info.Discount = info.Discount != null ? info.Discount : 0;
                    info.Pay_Amount = info.Pay_Amount != null ? info.Pay_Amount : 0;
                    SPA_INVOICE_PAYMENTDETAILS PaymentDetails = new SPA_INVOICE_PAYMENTDETAILS();
                    Basicinfo.Invoice_Id = info.Invoice_Id;
                    Basicinfo.Invoice_Service = info.Invoice_Service;
                    Basicinfo.UrlStatus = info.UrlStatus;
                    PaymentDetails.Invoice_Id = info.Invoice_Id.Value;
                    PaymentDetails.Pay_Date = dt;
                    PaymentDetails.ActiveStatus = "A";
                    PaymentDetails.CreatedDate = DateTime.Now;
                    PaymentDetails.UpdatedDate = DateTime.Now;
                    PaymentDetails.SchlId = schlId;
                    PaymentDetails.TotalPrice = info.Pay_Amount + info.Discount;
                    PaymentDetails.Discount_Amount = info.Discount;
                    PaymentDetails.Pay_Amount = info.Pay_Amount;
                    SPA.SPA_INVOICE_PAYMENTDETAILS.Add(PaymentDetails);
                    SPA.SaveChanges();
                    if (PaymentDetails.TotalPrice > 0)
                    {
                        var Amount = SPA.SPA_INVOICE_PAYMENTDETAILS.Where(c => c.Invoice_Id == info.Invoice_Id && c.TotalPrice != null && c.ActiveStatus == "A").Select(c => c.TotalPrice).Sum();
                        decimal PayAmount = Convert.ToDecimal(Amount);
                        decimal TotalAmount = Convert.ToDecimal(info.Total_Amount);
                        if (PayAmount >= TotalAmount)
                        {
                            fu.UpdateInvoicestatus("CMP", new List<long> { info.Invoice_Id.Value }, Currentdate, false);
                            Basicinfo.FullPayment = 1;
                        }
                        var Invoicemaster = SPA.SPA_INVOICE_MASTER.Where(c => c.Invoice_Id == info.Invoice_Id.Value).FirstOrDefault();
                        if (PayAmount >= TotalAmount)
                            Invoicemaster.Invoice_Status = "CMP";
                        Invoicemaster.UpdatedDate = Currentdate;
                        Invoicemaster.Paid_Date = dt;
                        Invoicemaster.Remaining_Amount = Convert.ToDouble(TotalAmount - PayAmount);

                        SPA.SaveChanges();
                        Basicinfo.Msg = "Success";
                    }
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
            return Json(Basicinfo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InvoiceReservationList(string ReservationList, long EmpId, long CustomerId, long InvoiceId, int LangId, string Istatus, DateTime InvoiceDate)
        {
            try
            {
                ViewBag.Istatus = Istatus;
                ViewBag.InvoiceDate = InvoiceDate;
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Invoice_Page" && c.Lang_id == LangId).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                return View(SPA.Database.SqlQuery<Models.InvoiceReservationList>(sql.InvoiceReservationList(ReservationList, EmpId, CustomerId, InvoiceId)).ToList());
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult RemPrintInvoice(string InvoiceId, string status, string url, string RStatus)
        {
            try
            {
                fu.UpdateReminderInvoicestatus(status, InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).ToList(), null, true);
                return RedirectToAction("InvoicePrint", "PDF", new { InvoiceId = InvoiceId, Status = status, IsPrint = true, Url = url, schlId = schlId, RStatus = RStatus });
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult RemEmailInvoice(string InvoiceId, string status, string url, string RStatus)
        {
            try
            {
                fu.UpdateReminderInvoicestatus(status, InvoiceId.Split(',').Select(c => Convert.ToInt64(c)).ToList(), null, true);
                fu.MailInvoice(InvoiceId, status, false, RStatus);
                if (!string.IsNullOrEmpty(url))
                    return Redirect("/Invoice/Invoicing#" + url);
                else
                    return Redirect("/Invoice/Invoicing");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult CreateManualInvoice(string UrlStatus)
        {
            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.ShopMasterDetail
            {
                Lang_id = c.Lang_id,
                SchlStudentStrength = c.SchlStudentStrength,
                TimeZone = c.TimeZone
            }).FirstOrDefault();
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "For_Invoicing" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
            {
                Order_id = c.Order_id,
                Lang_id = c.Lang_id,
                English_Label = c.English_Label,
                Value = c.Value,
                Page_Name = c.Page_Name
            }).ToList();
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            ViewBag.UrlStatus = UrlStatus;
            return View(fu.EmpAndCustomerList(schlId, UserId));
        }
        public ActionResult AddCreateManualInvoice(Models.SelectedCustomerEmployeeId Data, string UrlStatus)
        {
            try
            {
                string Url = "Forinvoicing";
                if(!string.IsNullOrEmpty(UrlStatus))
                    Url = UrlStatus;
                long InvoiceId = AddCustEmpInfoInInvoiceMaster(Data);
                if (InvoiceId > 0)
                    return RedirectToAction("Invoice1", "Invoice", new { InvoiceId = InvoiceId, Url = Url });
                else
                    return Redirect("/Invoice/Invoicing#" + Url);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public long AddCustEmpInfoInInvoiceMaster(Models.SelectedCustomerEmployeeId Data)
        {
            long InvoiceId = 0;
            if (Data != null)
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var Info = fu.EmpAndCustomerList(schlId, UserId);
                var CustomerInfo = Info.Where(c => c.UserId == Data.SelectedSearchMICustomer && c.RoleId == 4).FirstOrDefault();
                var EmpInfo = Info.Where(c => c.UserId == Data.SelectedSearchMIEmployee && c.RoleId != 4).FirstOrDefault();
                var Date = fu.ZonalDate(null);
                SPA.SPA_INVOICE_MASTER Invoice = new SPA_INVOICE_MASTER()
                {
                    ActiveStatus = "A",
                    SchlId = schlId,
                    CreatedDate = Date,
                    UpdatedDate = Date,
                    CustomerId = CustomerInfo.UserId.Value,
                    EmployeeId = EmpInfo.UserId.Value,
                    CustomerFirstName = CustomerInfo.FirstName,
                    CustomerLastName = CustomerInfo.LastName,
                    EmployeeFirstName = EmpInfo.FirstName,
                    EmployeeLastName = EmpInfo.LastName,
                    Invoice_Status = "CI"
                };
                SPA.SPA_INVOICE_MASTER.Add(Invoice);
                SPA.SaveChanges();
                List<long> InvoiceList = new List<long>();
                InvoiceList.Add(Invoice.Invoice_Id);
                fu.UpdateInvoicestatus("CI", InvoiceList, Date, false);
                InvoiceId = Invoice.Invoice_Id;
            }
            return InvoiceId;
        }
        public ActionResult Reports(int? Status)
        {
            var Language = (from g in SPA.CCTSP_SchoolMaster
                            join h in SPA.Language_Label_Detail on g.Lang_id equals h.Lang_id
                            where g.SchlId == schlId && h.Page_Name == "Report_Dashboard" && h.ActiveStatus == "A"
                            select h).ToList();
            ViewBag.Language = Language;
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            ViewBag.status = Status!= null?Status:1;
            Models.dropdownfilter Filter = new Models.dropdownfilter();
            List<Models.dropdownfilter> FilterList = new List<Models.dropdownfilter>();
            Filter = new Models.dropdownfilter();
            Filter.id = 1;
            Filter.filterName = Language.Where(c => c.Order_id == 3).Select(c => c.Value).FirstOrDefault();
            FilterList.Add(Filter);
            Filter = new Models.dropdownfilter();
            Filter.id = 2;
            Filter.filterName = Language.Where(c => c.Order_id == 4).Select(c => c.Value).FirstOrDefault();
            FilterList.Add(Filter);
            ViewBag.FilterList = FilterList;
            return View(SPA.Database.SqlQuery<Models.AbstractInvoiceReport>(sql.AbstractReport(schlId, UserId, Status)).ToList());
        }
        public ActionResult Monthly_Report(string Month, int? Year,int? Status)
        {
            //Get Language
            ViewBag.Language = (from g in SPA.CCTSP_SchoolMaster
                                join h in SPA.Language_Label_Detail on g.Lang_id equals h.Lang_id
                                where g.SchlId == schlId && h.ActiveStatus == "A" && h.Page_Name == "Invoice_Report"
                                select new Models.LanguageLabelDetails
                                {
                                    Order_id = h.Order_id,
                                    Lang_id = h.Lang_id,
                                    English_Label = h.English_Label,
                                    Value = h.Value,
                                    Page_Name = h.Page_Name,
                                    languageCulture = h.Language_Master.Currency_ShortName
                                }).ToList();
            //Qry to get Monthly Report
            long UserId = Convert.ToInt32(Session["UserId"].ToString());
            var qry = sql.InvoiceReport(Month, Year, schlId, UserId, Status);
            var InvoiceReportDetails = new List<Models.InvoiceReport>();
            if (!string.IsNullOrEmpty(qry))
                InvoiceReportDetails = SPA.Database.SqlQuery<Models.InvoiceReport>(qry).ToList();
            InvoiceReportDetails.All(c => { c.year = Year; c.MonthName = Month; c.Months = DateTime.Parse("1 " + Month + ", " + Year, enGB).Month; return true; });
            InvoiceReportDetails.All(c => { c.ShopImage = fu.LogoImg(c.ShopImage); return true; });
            return View(InvoiceReportDetails);
        }
        public ActionResult Yearly_Report(int? Year,int? Status)
        {
            try
            {
                //Get Language
                ViewBag.Language = (from g in SPA.CCTSP_SchoolMaster
                                    join h in SPA.Language_Label_Detail on g.Lang_id.Value equals h.Lang_id
                                    where g.SchlId == schlId && h.ActiveStatus == "A" && h.Page_Name.Trim().ToLower() == "invoice_report"
                                    select new Models.LanguageLabelDetails
                                    {
                                        Order_id = h.Order_id,
                                        Lang_id = h.Lang_id,
                                        English_Label = h.English_Label,
                                        Value = h.Value,
                                        Page_Name = h.Page_Name,
                                        languageCulture = h.Language_Master.Currency_ShortName
                                    }).ToList();
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                var qry = sql.InvoiceReport(null, Year, schlId, UserId, Status);
                var InvoiceReportDetails = new List<Models.InvoiceReport>();
                if (!string.IsNullOrEmpty(qry))
                    InvoiceReportDetails = SPA.Database.SqlQuery<Models.InvoiceReport>(qry).ToList();
                InvoiceReportDetails.All(c => { c.ShopImage = fu.LogoImg(c.ShopImage); c.year = Year; return true; });
                return View(InvoiceReportDetails);
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult GeneralInvoice(int? IsView, string ReservationIdList, long? InvoiceId, string Url, Models.ShopMasterDetail ShopInfo)
        {
            if (!string.IsNullOrEmpty(ReservationIdList) || InvoiceId != null)
            {
                ViewBag.isView = IsView;
                List<Models.GeneralInvoice> genInvoice = new List<Models.GeneralInvoice>();
                var Invoice = new Models.InvoiceStatus();
                if (!string.IsNullOrEmpty(ReservationIdList))
                    genInvoice = SPA.Database.SqlQuery<Models.GeneralInvoice>(sql.GetGeneralInvoice(ReservationIdList, schlId)).ToList();
                DateTime InvoiceDate = DateTime.Now;
                if (InvoiceId != null)
                {
                    genInvoice = SPA.Database.SqlQuery<Models.GeneralInvoice>(sql.GetGeneralInvoice(InvoiceId.Value, schlId)).ToList();
                    InvoiceDate = genInvoice.Count > 0 ? genInvoice.Select(c => c.InvoiceDate).FirstOrDefault().Value : DateTime.Now;
                }
                if (InvoiceId == null)
                {
                    Invoice = AddInvoice(ReservationIdList, ShopInfo);
                    if (!string.IsNullOrEmpty(Invoice.url))
                    {
                        Session["Message"] = "Forinvoicing";
                        return Redirect(Invoice.url);
                    }
                    InvoiceId = Invoice.ReservationList.FirstOrDefault();
                    InvoiceDate = Invoice.InvoiceDate.Value;
                }
                if (genInvoice.Select(c => c.DOB) != null)
                    genInvoice.All(c => { c.InvoiceId = InvoiceId; c.URL = Url; c.InvoiceDate = InvoiceDate; return true; });
                if (Url == "Forinvoicing")
                    genInvoice.All(c => { c.txtAreaGInvoice = ShopInfo.Invoice_FreeText; return true; });
                ViewBag.AddOnProductList = fu.AddOnProductList();
                var ServicesList = new List<Models.InvoiceServicesList>();
                if (genInvoice.Count > 0)
                    ServicesList = fu.ServicesList(genInvoice.Select(c => c.EmpId).FirstOrDefault(), schlId);
                ViewBag.ServicesList = ServicesList;
                bool ServiceTabAccess = true;
                if (ShopInfo.Flow_Id > 1)
                    ServiceTabAccess = fu.CheckSubTabAccess("Product_Catalog", ShopInfo.Flow_Id.Value);
                ViewBag.ServiceTabAccess = ServiceTabAccess;
                bool ReservationTabAccess = true;
                if (ShopInfo.Flow_Id > 1)
                    ReservationTabAccess = fu.CheckTabAccess("Reservation", ShopInfo.Flow_Id.Value);
                ViewBag.ReservationTabAccess = ReservationTabAccess;
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && (c.Page_Name == "Invoice_Page" || c.Page_Name == "General_Invoice") && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                ViewBag.ShopInfo = ShopInfo;
                ViewBag.LngLocal = fu.SmallCalLangTranslate(ShopInfo.Lang_id);
                return View("GeneralInvoice", genInvoice);
            }
            else
                return RedirectToAction("Invoicing", "Invoice");
        }
        public void DeleteInvoice(long? InvoiceId)
        {
            try
            {
                if (InvoiceId > 0)
                {
                    var DeleteQuery = "update SPA_INVOICE_MASTER set ActiveStatus='D' where ActiveStatus='A' and Invoice_Id=" + InvoiceId;
                    var DeleteInvoiceStatus = DeleteQuery + " " + "update SPA_INVOICE_STATUS set ActiveStatus='T' where ActiveStatus='A' and Invoice_Id=" + InvoiceId;
                    var DeleteInvoiceDetails = DeleteInvoiceStatus + " " + "update SPA_INVOICE_Detail set ActiveStatus='D' where ActiveStatus='A' and Invoice_Id=" + InvoiceId;
                    SPA.Database.ExecuteSqlCommand(DeleteInvoiceDetails);
                    SPA_INVOICE_STATUS InvoiceStatus = new SPA_INVOICE_STATUS();
                    InvoiceStatus.ActiveStatus = "A";
                    InvoiceStatus.CreatedDate = DateTime.Now;
                    InvoiceStatus.UpdatedDate = DateTime.Now;
                    InvoiceStatus.Invoice_Id = InvoiceId.Value;
                    InvoiceStatus.Invoice_Status = "D";
                    SPA.SPA_INVOICE_STATUS.Add(InvoiceStatus);
                    fu.setActivityLogAsync("DeleteInvoice", "Invoice", schlId, "Delete Invoice", DeleteInvoiceDetails);
                    SPA.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
            }
        }
        public ActionResult PayAndClosedInvoice(long InvoiceId, string Url, string Invoice_Service, string Msg, int? FullPayment)
        {
            try
            {
                var Status = "CIPS";
                if (Invoice_Service == "1")
                    Status = "CIS";
                if (Invoice_Service == "3")
                    Status = "CIP";
                if (FullPayment == 1)
                {
                    Status = "DCMP";
                    Msg = Url + "_DCMP";
                }
                Session["Message"] = Msg;
                if (Invoice_Service == "1")
                    return this.MailInvoice(Convert.ToString(InvoiceId), Status, Url);
                else if (Invoice_Service == "3")
                    return this.PrintInvoice(Convert.ToString(InvoiceId), Status, Url);
                else
                    return this.MailPrintInvoice(Convert.ToString(InvoiceId), Status, Url);
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult PartialPaymentPopup(long InvoiceId, int LangId, string LngLocal, DateTime ShopDate, string UrlStatus, string Invoice_Service)
        {
            try
            {
                ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Open Invoices" && c.Lang_id == LangId && (c.Order_id > 18 || c.Order_id == 5 || c.Order_id == 7)).Select(c => new Models.LanguageLabelDetails
                {
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name
                }).ToList();
                Models.BasicInfo_PartialPopup BasicInfo = new Models.BasicInfo_PartialPopup();
                BasicInfo.Invoice_Service = Invoice_Service;
                BasicInfo.LngLocal = LngLocal;
                BasicInfo.UrlStatus = UrlStatus;
                BasicInfo.ShopDate = ShopDate;
                ViewBag.BasicInfo = BasicInfo;
                return View(SPA.Database.SqlQuery<Models.PartialPaymentList>(sql.PartialPaymentQuery(InvoiceId)).ToList());
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public decimal RemovePartialPayment(long PartialPId)
        {
            try
            {
                var DeletePartialPDetails = SPA.SPA_INVOICE_PAYMENTDETAILS.Where(c => c.Invoice_Payment_Id == PartialPId).FirstOrDefault();
                DeletePartialPDetails.ActiveStatus = "D";
                DeletePartialPDetails.UpdatedDate = DateTime.Now;
                SPA.SaveChanges();
                var InvoiceMaster = SPA.SPA_INVOICE_MASTER.Where(c => c.Invoice_Id == DeletePartialPDetails.Invoice_Id).FirstOrDefault();
                InvoiceMaster.Remaining_Amount = InvoiceMaster.Remaining_Amount + DeletePartialPDetails.TotalPrice;
                UpdateModel(InvoiceMaster);
                SPA.SaveChanges();
                fu.setActivityLogAsync("RemovePartialPayment", "Invoice", schlId, "Delete Partial Payment", PartialPId);
                return Convert.ToDecimal(InvoiceMaster.Remaining_Amount.Value);

            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return 0;
            }

        }
        public ActionResult PaymentHistory(List<Models.PartialPaymentList> PartialPaymentList, List<Models.LanguageLabelDetails> LangList)
        {
            ViewBag.LanguageInfo = LangList;
            return View(PartialPaymentList);
        }
        public List<Models.PaymentNtry> Readxml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(HostingEnvironment.MapPath("~/Upload/Camt_053_Beispiel_1.XML"));
            List<Models.PaymentNtry> NList = new List<Models.PaymentNtry>();
            Models.PaymentNtry NData = new Models.PaymentNtry();
            Models.PaymentNtryDetails Ndetails = new Models.PaymentNtryDetails();
            List<Models.PaymentNtryDetails> NdetailList = new List<Models.PaymentNtryDetails>();
            XmlNodeList List = xmlDoc.GetElementsByTagName("Ntry");
            foreach (XmlNode node in List)
            {
                XmlNodeList MNode = node.ChildNodes;
                NData = new Models.PaymentNtry();
                foreach (XmlNode XmlNode in MNode)
                {
                    if (XmlNode.Name == "Amt")
                        NData.Amt = XmlNode.InnerText;
                    if (XmlNode.Name == "BookgDt")
                        NData.BookgDt = XmlNode.FirstChild.InnerText;
                    if (XmlNode.Name == "Sts")
                        NData.Sts = XmlNode.InnerText;
                    if (XmlNode.Name == "NtryDtls")
                    {
                        XmlNodeList NtryDetailsNode = XmlNode.ChildNodes;
                        NdetailList = new List<Models.PaymentNtryDetails>();
                        foreach (XmlNode Item in NtryDetailsNode)
                        {
                            Ndetails = new Models.PaymentNtryDetails();
                            if (Item.ChildNodes[0].Name == "Refs")
                                Ndetails.EndToEndId = Item.ChildNodes[0].InnerText;
                            if (Item.ChildNodes[1].Name == "AmtDtls")
                                Ndetails.Amt = Item.ChildNodes[1].InnerText;
                            if (Item.ChildNodes[2].Name == "RmtInf")
                                Ndetails.Ref = Item.ChildNodes[2].InnerText;
                            NdetailList.Add(Ndetails);
                        }
                        NData.NtryDetails = NdetailList;
                    }
                }
                NList.Add(NData);
            }
            return NList;
        }
        public ActionResult SaveAndPrintInvoice(string Invoice_Service, long InvoiceId, string Url)
        {
            Session["Message"] = Url + "_CIP";
            if (Invoice_Service != "1")
                return this.PrintInvoice(Convert.ToString(InvoiceId), "CIP", Url);
            else
            {
                var Link = "/Invoice/Invoicing#" + Url;
                if (Url == "openlistview" || Url == "calendarmonth" || Url == "Appclosed")
                    Link = "/Reservation/Reservation#" + Url;
                return Redirect(Link);
            }
        }
    }
}
