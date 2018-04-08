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
using System.Web.Routing;
using System.Web.Hosting;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml;

namespace SPA.Common
{
    public class Function
    {
        PushEmail EmailSend = new PushEmail();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string StPay = ConfigurationManager.AppSettings["StPayment"];
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();

        JavaScriptSerializer js = new JavaScriptSerializer();
        Sql sql = new Sql();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        Sql sq = new Sql();
        CultureInfo enGB = new CultureInfo("en-GB");
        public Function()
        {
            schlId = Convert.ToInt32(GetShopId(link));
        }
        public void ScheduleSlotAdd(List<SPA_SchedulerSlotMaster> schedule)
        {
            SPA.SPA_SchedulerSlotMaster.AddRange(schedule);
            SPA.SaveChanges();
        }
        public long GetShopId(string Link)
        {
            long? Shopid = 0;
            if (Link != "localhost" && Link != "devtestspa001.azurewebsites.net" && Link != "click-and-go.co.in" && Link != "click-and-go.ch")
                Shopid = SPA.CCTSP_LendingPageMaster.Where(c => c.Azure_Website == Link || c.Original_Website == Link).Select(c => c.Schid).FirstOrDefault();

            if (Shopid == 0 || Shopid == null)
                Shopid = schlId;
            return Shopid.Value;
        }
        public Models.ShopClickandgo CheckClickandgo(string WebUrl, int? lang, int schId)
        {
            Models.ShopClickandgo ClickResult = new Models.ShopClickandgo();
            ClickResult.schId = schId.ToString();
            ClickResult.status = false;
            //if (WebUrl == "regclick.azurewebsites.net" || WebUrl == "click-and-go.org" || WebUrl == "www.click-and-go.org" || WebUrl == "www.click-and-go.ch" || WebUrl == "click-and-go.ch")
            //    schId = schlId;
            if (WebUrl == "localhost" || WebUrl == "insurancetesting.azurewebsites.net" || WebUrl == "shopf-rel1.azurewebsites.net")
            {
                schId = schlId;
                WebUrl = "click-and-go.ch";
            }
            if (schId < 251)
            {
                List<string> WebList = new List<string>();
                if (WebUrl == "regclick.azurewebsites.net" || WebUrl == "click-and-go.org" || WebUrl == "www.click-and-go.org" || WebUrl == "www.click-and-go.ch" || WebUrl == "click-and-go.ch" || WebUrl == "clickapitest.azurewebsites.net" || WebUrl == "abcd.click-and-go.ch")
                    WebUrl = "click-and-go.ch";
                if (WebUrl == "click-and-go.co.in" || WebUrl == "www.click-and-go.co.in")
                    WebUrl = "click-and-go.co.in";

                WebList.Add("click-and-go.co.in");
                WebList.Add("click-and-go.ch");
                if (WebList.Where(c => c == WebUrl).Count() > 0)
                {
                    string WorkFlowStatus = ConfigurationManager.AppSettings["WorkFlow"];
                    ClickResult.status = true;
                    ClickResult.WorkFlowStatus = WorkFlowStatus;
                    if (WebUrl == "click-and-go.co.in")
                    {
                        ClickResult.LangId = 1; ClickResult.stat = 1;
                    }
                    if (WebUrl == "click-and-go.ch")
                    {
                        if (lang != null && lang != 0)
                            ClickResult.LangId = lang.Value;
                        else
                            ClickResult.LangId = 2;
                        ClickResult.stat = 2;
                    }
                    ClickResult.Website = WebUrl;
                }
            }

            return ClickResult;
        }
        public List<Models.LanguageLabelDetails> LanguageNameList()
        {
            return SPA.Language_Master.Select(c => new Models.LanguageLabelDetails { Lang_id = c.Lang_id, Value = c.Lang_Name, English_Label = c.Currency_Symbol }).ToList();
        }
        public async Task CommonManualInvoice(Models.ManualInvoice Invoice, DateTime CurrentDate)
        {
            await Task.Run(() => ManualInvoiceAdd(Invoice, CurrentDate));
        }
        public async Task AddFreecustomerInfo(long Schlid, Models.FreeCustomer FreeCustomerInfo)
        {
            await Task.Run(() => customerInfo(Schlid, FreeCustomerInfo));
        }
        public async Task EditFreecustomerInfo(long Schlid, Models.EditFreeCustomer FreeCustomerInfo)
        {
            await Task.Run(() => EditCustomerInfo(Schlid, FreeCustomerInfo));
        }
        public void ManualInvoiceAdd(Models.ManualInvoice Invoice, DateTime CurrentDate)
        {
            var IdList = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 161 && c.ActiveStatus == "A").ToList();
            List<Invoice_CategoryDetails> CatgDetailsList = new List<Invoice_CategoryDetails>();
            Invoice_CategoryDetails CatgDetails = new Invoice_CategoryDetails();
            //Add Dropdown Selected Value
            if (!string.IsNullOrEmpty(Invoice.Law))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Law, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Law").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Kind_of_reimbursement))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Kind_of_reimbursement, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "reimbursement").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Treatment))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Treatment, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Reason for treatment").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Therapie))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Therapie, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Therapie").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Invoice_Diagnosis))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Invoice_Diagnosis, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Diagnose").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Invoice_Gln))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Invoice_Gln, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Gln").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Invoice_Zsr))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Invoice_Zsr, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Zsr").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Invoice_Referer))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Invoice_Referer, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Referer").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.Invoice_Remarks))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.Invoice_Remarks, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_Remarks").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (Invoice.TreatFromdate != null && Invoice.TreatTodate != null)
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.TreatFromdate.Value.ToString("dd-MM-yyyy"), Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "InvoiceFromToDate").Select(c => c.CatgTypeId).FirstOrDefault(), Invoice.TreatTodate.Value.ToString("dd-MM-yyyy"));
                CatgDetailsList.Add(CatgDetails);
            }
            if (!string.IsNullOrEmpty(Invoice.ContractNo))
            {
                CatgDetails = InvoiceDetails(CurrentDate, Invoice.ContractNo, Invoice.InvoiceId.Value, IdList.Where(c => c.CatgDesc == "Invoice_ContractNo").Select(c => c.CatgTypeId).FirstOrDefault(), null);
                CatgDetailsList.Add(CatgDetails);
            }
            if (CatgDetailsList.Count > 0)
            {
                SPA.Invoice_CategoryDetails.AddRange(CatgDetailsList);
                SPA.SaveChanges();
            }
            //Add On Product and Reservation Assign to Invoice
            Models.ValueList Product = new Models.ValueList();
            List<Models.ValueList> ProductList = new List<Models.ValueList>();
            int Index = 0;
            if (Invoice.PReservationid != null)
            {
                foreach (var Id in Invoice.PReservationid)
                {
                    Product = new Models.ValueList();
                    if (Id != 0)
                    {
                        Product.Id = Id;
                        Product.value = Invoice.PDuaration[Index];
                        Product.isReservation = true;
                    }
                    else
                    {
                        Product.Id = Invoice.AddOnProductId[Index];
                        Product.value = Invoice.AddOnQuantity[Index];
                        Product.Date = Invoice.AddOnDate[Index];
                        //Product.Time = Invoice.AddOnTime[Index];
                    }
                    ProductList.Add(Product);
                    Index = Index + 1;
                }
            }
            Models.ValueList SettlementText = new Models.ValueList();
            List<Models.ValueList> SettlementTextList = new List<Models.ValueList>();
            int TextIndex = 0;
            if (Invoice.AddOnSettlementTextId != null)
            {
                foreach (var Id in Invoice.AddOnSettlementTextId)
                {
                    SettlementText = new Models.ValueList();
                    if (Id != 0)
                    {
                        SettlementText.Id = Id;
                        SettlementText.value = Invoice.SettlementTextDuaration[TextIndex];
                        SettlementText.isReservation = false;
                        SettlementText.SettlementText = Invoice.ISettlementText[TextIndex];
                        SettlementText.Date = Invoice.SettlementTextDate[TextIndex];
                    }
                    SettlementTextList.Add(SettlementText);
                    TextIndex = TextIndex + 1;
                }
            }
            List<SPA_INVOICE_Detail> InvoiceList = new List<SPA_INVOICE_Detail>();
            List<SPA_INVOICE_Detail> InvoiceList1 = new List<SPA_INVOICE_Detail>();
            List<SPA_INVOICE_Detail> InvoiceList2 = new List<SPA_INVOICE_Detail>();
            List<SPA_INVOICE_Detail> InvoiceList3 = new List<SPA_INVOICE_Detail>();
            if (ProductList.Where(c => c.isReservation == true).Count() > 0)
                InvoiceList1 = AddINVDETRESERVATION(ProductList.Where(c => c.isReservation == true).ToList(), Invoice.InvoiceId.Value, CurrentDate);
            if (ProductList.Where(c => c.isReservation == false).Count() > 0)
                InvoiceList2 = AddINVDETADDON(ProductList.Where(c => c.isReservation == false).ToList(), Invoice.InvoiceId.Value, CurrentDate);
            if (SettlementTextList.Count > 0)
                InvoiceList3 = AddSettlementTextToInvoice(SettlementTextList, Invoice.InvoiceId.Value, CurrentDate);
            if (InvoiceList1.Count > 0)
                InvoiceList.AddRange(InvoiceList1);
            if (InvoiceList2.Count > 0)
                InvoiceList.AddRange(InvoiceList2);
            if (InvoiceList3.Count > 0)
                InvoiceList.AddRange(InvoiceList3);
            if (InvoiceList.Count > 0)
            {
                var MinBookingDate = InvoiceList.Select(c => c.Date_Of_Buying).FirstOrDefault();
                Decimal TotalAmount = Convert.ToDecimal(InvoiceList.Select(c => c.TotalPricewithvat).Sum().Value);
                SPA.SPA_INVOICE_Detail.AddRange(InvoiceList);
                var GenerateMatrixCode = "";
                if (Invoice.InvoiceDate != null && Invoice.InvoiceStatus == null)
                    GenerateMatrixCode = Generate2dMatrix(Invoice.InvoiceDate.Value, Invoice.Emp_ZSR_No, Invoice.DOB.Value, Invoice.Cust_Pincode.Value, TotalAmount, MinBookingDate.Value);
                var MasterInvoiceInfo = SPA.SPA_INVOICE_MASTER.Where(c => c.Invoice_Id == Invoice.InvoiceId.Value).FirstOrDefault();
                if (MasterInvoiceInfo != null)
                {
                    MasterInvoiceInfo.Image_barcode = GenerateMatrixCode;
                    MasterInvoiceInfo.field4 = Invoice.txtAreaGInvoice;
                    MasterInvoiceInfo.field3 = MakeESRCODE(MasterInvoiceInfo.CCTSP_SchoolMaster.Currency, TotalAmount, MasterInvoiceInfo.CCTSP_SchoolMaster.BANKACCOUNT);
                }
                SPA.SaveChanges();
            }
            //Update UserInfo
            if (Invoice.PatientId != null && Invoice.InvoiceStatus == null)
            {
                var UserInfo = SPA.CCTSP_User.Where(c => c.UserId == Invoice.PatientId).FirstOrDefault();
                UserInfo.AHV_Number = Invoice.Cust_AHV_Number;
                UserInfo.VEKA_Number = Invoice.Cust_VEKA_Number;
                UserInfo.State = Invoice.Cust_State;
                UserInfo.Street = Invoice.Cust_Street;
                UserInfo.InsuranceNumber = Invoice.Cust_InsuranceNumber;
                UserInfo.City = Invoice.Cust_City;
                UserInfo.Pincode = Invoice.Cust_Pincode;
                UserInfo.Title = Invoice.Cust_Title;
                UserInfo.FirstName = Invoice.Cust_FirstName;
                UserInfo.LastName = Invoice.Cust_LastName;
                if (Invoice.DOB != null)
                    UserInfo.DOB = Invoice.DOB.Value;
                else
                    UserInfo.DOB = DateTime.Parse("0001-01-01", enGB);
                var Empinfo = SPA.CCTSP_User.FirstOrDefault(c => c.UserId == Invoice.EmpId);
                if (Empinfo != null)
                {
                    Empinfo.ZSR_No = Invoice.Emp_ZSR_No;
                    Empinfo.City = Invoice.Emp_City;
                    Empinfo.Pincode = Invoice.Emp_Pincode;
                    Empinfo.Street = Invoice.Emp_Street;
                    Empinfo.StreetNumber = Invoice.Emp_StreetNumber;
                }
                var ShopDetails = SPA.CCTSP_SchoolMaster.FirstOrDefault(c => c.SchlId == schlId);
                if (ShopDetails != null)
                {
                    ShopDetails.SchlCity = Invoice.ShopCity;
                    ShopDetails.SchPin = Invoice.ShopZipcode;
                    ShopDetails.street = Invoice.Shopstreet;
                    ShopDetails.StreetNumber = Invoice.ShopStreetNumber;
                }
                var OwnerInfo = SPA.CCTSP_User.FirstOrDefault(C => C.SchId == schlId && C.RoleId == 1 && C.ActiveStatus == "A");
                if (OwnerInfo != null)
                    OwnerInfo.ZSR_No = Invoice.Own_ZSR_No;
                SPA.SaveChanges();
            }
        }
        public void EditCustomerInfo(long Schlid, Models.EditFreeCustomer FreeCustomerInfo)
        {
            try
            {
                #region EmpInfo
                // Update EmployeeInfo
                List<Models.FreeCustomerEmployeeInfo> Emplist = new List<Models.FreeCustomerEmployeeInfo>();
                CCTSP_User EmpDetails = new CCTSP_User();
                List<long> EmpIdlist = new List<long>();
                List<Models.FreecustomerImage> ImageList = new List<Models.FreecustomerImage>();
                var Image = new Models.FreecustomerImage();

                if (FreeCustomerInfo.EmpUserId != null)
                {
                    EmpIdlist = FreeCustomerInfo.EmpUserId;
                    var query = "update cctsp_User set ActiveStatus='D' Where Schid=" + Schlid + " and RoleId=3 and Activestatus='A' and UserId not in(" + string.Join(",", EmpIdlist) + ")";
                    SPA.Database.ExecuteSqlCommand(query);
                    foreach (var Id in FreeCustomerInfo.EmpUserId)
                    {
                        var Index = FreeCustomerInfo.EmpUserId.IndexOf(Id);
                        EmpDetails = SPA.CCTSP_User.Where(c => c.SchId == Schlid && c.ActiveStatus == "A" && c.UserId == Id).FirstOrDefault();
                        EmpDetails.FirstName = FreeCustomerInfo.EmpFName[Index];
                        EmpDetails.LastName = FreeCustomerInfo.EmpLName[Index];
                        EmpDetails.Gender = FreeCustomerInfo.EmpGender[Index];
                        EmpDetails.PhoneNo = FreeCustomerInfo.EmpNumber[Index];
                        EmpDetails.EmailId = FreeCustomerInfo.EmpEmail[Index];
                        EmpDetails.Degree = FreeCustomerInfo.EmpDegree[Index];
                        EmpDetails.skypeId = FreeCustomerInfo.EmpSkypeId[Index];
                        List<long> CatgTypeIdlist = new List<long>();
                        if (FreeCustomerInfo.HSpecialization_1[Index] > 0)
                            CatgTypeIdlist.Add(FreeCustomerInfo.HSpecialization_1[Index]);
                        if (FreeCustomerInfo.HSpecialization_2[Index] > 0)
                            CatgTypeIdlist.Add(FreeCustomerInfo.HSpecialization_2[Index]);
                        if (FreeCustomerInfo.HSpecialization_3[Index] > 0)
                            CatgTypeIdlist.Add(FreeCustomerInfo.HSpecialization_3[Index]);
                        if (FreeCustomerInfo.HSpecialization_4[Index] > 0)
                            CatgTypeIdlist.Add(FreeCustomerInfo.HSpecialization_4[Index]);
                        if (CatgTypeIdlist.Count > 0)
                            EmpDetails.specialization = string.Join(",", CatgTypeIdlist.Distinct());
                        else
                            EmpDetails.specialization = null;
                        EmpDetails.ExperienceMonth = FreeCustomerInfo.EmpExpMonth[Index] != null ? FreeCustomerInfo.EmpExpMonth[Index] : 0;
                        EmpDetails.ExperienceYear = FreeCustomerInfo.EmpExpYear[Index] != null ? FreeCustomerInfo.EmpExpYear[Index] : 0;
                        EmpDetails.Updated_Date = DateTime.Now;
                        if (FreeCustomerInfo.EmpImage_Name[Index] != null)
                        {
                            Image = new Models.FreecustomerImage();
                            Image.EmpId = Id;
                            Image.EmpImage = convertPostBasefiletoBase64(FreeCustomerInfo.EmpImage_Name[Index]);
                            if (!string.IsNullOrEmpty(Image.EmpImage))
                                ImageList.Add(Image);
                        }
                        //  EmpDetails.Answer2=
                        //EmpDetails.Answer2 = EditCommonImageUpload(FreeCustomerInfo.EmpImage_Name[Index], FreeCustomerInfo.EmpUserId[Index], "Upload");

                        SPA.SaveChanges();
                    }
                    if (ImageList.Count > 0)
                    {
                        CommonImageUpload(js.Serialize(ImageList));

                    }
                }
                else
                {
                    EmpIdlist = SPA.CCTSP_User.Where(c => c.SchId == Schlid && c.ActiveStatus == "A" && c.RoleId == 3).Select(c => c.UserId).ToList();
                    if (EmpIdlist.Count > 0)
                    {
                        var query = "update cctsp_User set ActiveStatus='D' Where Schid=" + Schlid + " and RoleId=3 and Activestatus='A' and UserId  in(" + string.Join(",", EmpIdlist) + ")";
                        SPA.Database.ExecuteSqlCommand(query);
                    }
                }
                #endregion
                var WeekSch = new List<Models.LendingWeekScheduleDetails>();
                #region LandingInfo
                if (FreeCustomerInfo.LendingObject != null)
                {
                    var query = "update cctsp_categoryDetails set ActiveStatus='D' where CatgId=148 and Domaintype=" + Schlid;
                    SPA.Database.ExecuteSqlCommand(query);
                    WeekSch = js.Deserialize<List<Models.LendingWeekScheduleDetails>>(FreeCustomerInfo.LendingObject);
                    List<CCTSP_CategoryDetails> List = new List<CCTSP_CategoryDetails>();
                    CCTSP_CategoryDetails Detail = new CCTSP_CategoryDetails();
                    //int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId ==Schlid).Select(c => c.Lang_id).FirstOrDefault().Value;
                    var Closed = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.ActiveStatus == "A" && c.Order_id == 41 && c.Lang_id == 1).FirstOrDefault();
                    var StatusClosed = Closed.Value;
                    var EnLabelStatusClosed = Closed.English_Label;
                    foreach (var item in WeekSch)
                    {
                        Detail = new CCTSP_CategoryDetails();
                        Detail.CatgId = 148;
                        Detail.CatgType = "LendingDay";
                        Detail.ActiveStatus = "A";
                        Detail.DomainType = Convert.ToInt32(Schlid);
                        Detail.CatgDesc = item.Catgdesc != null && item.Catgdesc != EnLabelStatusClosed ? item.Catgdesc : EnLabelStatusClosed;
                        Detail.Gender = item.Gender != null && item.Gender != EnLabelStatusClosed ? item.Gender : EnLabelStatusClosed;
                        Detail.Value = item.CatgTypeId;
                        List.Add(Detail);
                    }
                    SPA.CCTSP_CategoryDetails.AddRange(List);
                    SPA.SaveChanges();
                }
                #endregion
                if (FreeCustomerInfo.EmpUserId != null)
                {
                    #region WeekSchedule Info
                    List<string> DaysList = new List<string>();
                    CTSP_SolutionMaster Days = new CTSP_SolutionMaster();
                    List<CTSP_SolutionMaster> SolutionList = new List<CTSP_SolutionMaster>();
                    List<SPA_EmployeeSchedulers> BulkAddUserDays = new List<SPA_EmployeeSchedulers>();
                    var query1 = "update CTSP_SolutionMaster set ActiveStatus='D' where SectionName='EmployeeDays' and SchId='" + Schlid + "' and ActiveStatus='A' and UserId in ( " + string.Join(",", EmpIdlist) + ")";
                    SPA.Database.ExecuteSqlCommand(query1);
                    foreach (var EmpId in EmpIdlist)
                    {
                        foreach (var item in WeekSch.Where(c => (c.Catgdesc != "Closed" || c.Gender != "Closed") && c.Catgdesc != null && c.Gender != null).ToList())
                        {
                            Days = new CTSP_SolutionMaster();
                            Days.SchId = Schlid;
                            Days.CatgTypeId = 11143;
                            Days.UserId = EmpId;
                            Days.CraetedOn = DateTime.Now;
                            Days.SectionName = "EmployeeDays";
                            Days.Activestatus = "A";
                            Days.SectionDesc = item.Day.Trim();
                            SolutionList.Add(Days);
                            DaysList.Add(item.Day.Trim());
                        }
                    }
                    SPA_EmployeeSchedulers EmployeeSchedule = new SPA_EmployeeSchedulers();
                    var updateAllTimeSlotExisting = "update SPA_EmployeeSchedulers set ActiveStatus='D' where UserId in (" + EmpIdlist + ") and ActiveStatus='A' and schid=" + Schlid + "";
                    SPA.Database.ExecuteSqlCommand(updateAllTimeSlotExisting);
                    foreach (var EmpId in EmpIdlist)
                    {
                        foreach (var Daysadded in WeekSch.Where(c => (c.Catgdesc != "Closed" || c.Gender != "Closed") && c.Catgdesc != null && c.Gender != null).ToList())
                        {
                            #region Section1
                            EmployeeSchedule = new SPA_EmployeeSchedulers();
                            EmployeeSchedule.ActiveStatus = "A";
                            EmployeeSchedule.SchId = Schlid;
                            EmployeeSchedule.UserId = EmpId;
                            EmployeeSchedule.WeekDay = Daysadded.Day.Trim();
                            if (Daysadded.Catgdesc != "Closed" && Daysadded.Catgdesc != null)
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[0]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
                            }
                            else
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
                            }
                            EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
                            EmployeeSchedule.CreatedOn = DateTime.Now;
                            BulkAddUserDays.Add(EmployeeSchedule);


                            #endregion
                            #region Section2

                            EmployeeSchedule = new SPA_EmployeeSchedulers();
                            EmployeeSchedule.ActiveStatus = "A";
                            EmployeeSchedule.SchId = Schlid;
                            EmployeeSchedule.UserId = EmpId;
                            EmployeeSchedule.WeekDay = Daysadded.Day.Trim();
                            if (Daysadded.Gender != "Closed")
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Gender.Split('-')[1]).TimeOfDay;
                            }
                            else
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
                            }
                            EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
                            EmployeeSchedule.CreatedOn = DateTime.Now;
                            BulkAddUserDays.Add(EmployeeSchedule);

                            #endregion
                        }
                    }
                    if (BulkAddUserDays.Count > 0)
                    {
                        SPA.SPA_EmployeeSchedulers.AddRange(BulkAddUserDays);
                        SPA.SaveChanges();
                    }
                    SPA.CTSP_SolutionMaster.AddRange(SolutionList);
                    SPA.SaveChanges();
                    #endregion
                    #region TimeSlotInfo
                    DateTime EuropeDate = DateTime.Now;
                    List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
                    List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMasterList = new List<SPA_SchedulerSlotMaster>();
                    string time = "";
                    TimeSpan OriginalMinutes;
                    SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
                    var updateTimeSlotExisting = "update SPA_SchedulerSlotMaster set ActiveStatus='D' where UserId in(" + EmpIdlist + ") and ActiveStatus='A' and SchId=" + Schlid;
                    SPA.Database.ExecuteSqlCommand(updateTimeSlotExisting);
                    int CatgTypeId = 11194;
                    var data = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatgTypeId && c.DomainType == 251).FirstOrDefault();
                    var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == Schlid && c.ActiveStatus == "A").ToList();
                    #region MinuteCode
                    if (data.CatgDesc.Contains("Minute"))
                    {
                        time = data.CatgDesc.Replace("Minute", "").Replace("s", "");
                        OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(time.Trim()));
                        foreach (var UserId in EmpIdlist)
                        {
                            var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == Schlid && c.UserId == UserId).ToList();
                            #region GetDays of Employee
                            foreach (var Term in UserDays)
                            {
                                var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == Schlid && c.UserId == UserId).ToList();
                                #region SlotEntry
                                if (SlotTimeNew.Count() > 0)
                                {
                                    foreach (var item in SlotTimeNew)
                                    {
                                        TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
                                        while (TempTime <= item.EndTime)
                                        {
                                            var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
                                            if (BulkSchedulerSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
                                            {
                                                schedule = new SPA_SchedulerSlotMaster();
                                                schedule.SchedulerId = WeekDayForSelected;
                                                schedule.StartTime = TempTime;
                                                schedule.UserId = UserId;
                                                schedule.CreatedOn = EuropeDate;
                                                schedule.ActiveStatus = "A";
                                                schedule.SlotDue = data.CatgDesc;
                                                schedule.SchId = Schlid;
                                                BulkSchedulerSlotMaster.Add(schedule);
                                            }
                                            TempTime = TempTime + OriginalMinutes;
                                        }
                                    }
                                }
                                #endregion
                            }
                            BulkSchedulerSlotMasterList.AddRange(BulkSchedulerSlotMaster);
                            BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
                        }
                        if (BulkSchedulerSlotMasterList.Count > 0)
                        {
                            SPA.SPA_SchedulerSlotMaster.AddRange(BulkSchedulerSlotMasterList);
                            SPA.SaveChanges();
                        }

                        #endregion

                    }
                    #endregion
                    #endregion
                }
            }
            catch (Exception Ex)
            {

            }
        }
        public string convertPostBasefiletoBase64(HttpPostedFileBase Image)
        {
            var ReturnImage = "";
            byte[] data;
            try
            {
                using (Stream inputStream = Image.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                    ReturnImage = Convert.ToBase64String(data);
                }
            }
            catch (Exception e)
            {

            }

            return ReturnImage;
        }
        //public void customerInfo(long Schlid, Models.FreeCustomer FreeCustomerInfo)
        //{
        //    try
        //    {

        //        /*All The Information Related to Employee*/
        //        #region EmpInfo
        //        CCTSP_User Empinfo = new CCTSP_User();
        //        List<CCTSP_User> EmployeeList = new List<CCTSP_User>();
        //        List<Models.EmployeeJsonModel> Emplist = new List<Models.EmployeeJsonModel>();
        //        Models.FreecustomerImage Image = new Models.FreecustomerImage();
        //        List<Models.FreecustomerImage> ImageList = new List<Models.FreecustomerImage>();
        //        if (FreeCustomerInfo.EmpJson != null)
        //        {
        //            Emplist = js.Deserialize<List<Models.EmployeeJsonModel>>(FreeCustomerInfo.EmpJson);

        //            foreach (var item in Emplist)
        //            {
        //                Empinfo = new CCTSP_User();
        //                Empinfo.FirstName = item.EmpFName;
        //                Empinfo.LastName = item.EmpLName;
        //                Empinfo.Gender = item.EmpGender;
        //                Empinfo.PhoneNo = item.EmpNumber;
        //                Empinfo.LoginName = item.EmpNumber != null ? item.EmpNumber : "0123456789";
        //                Empinfo.Password = "Emp@1234";
        //                Empinfo.EmailId = item.EmpEmail;
        //                Empinfo.Degree = item.EmpDegree;
        //                Empinfo.skypeId = item.EmpSkypeId;
        //                Empinfo.RoleId = 3;
        //                Empinfo.SchId = Schlid;
        //                Empinfo.ActiveStatus = "A";
        //                List<long> CatgTypeIdlist = new List<long>();
        //                if (item.HSpecialization_1 > 0)
        //                    CatgTypeIdlist.Add(item.HSpecialization_1.Value);
        //                if (item.HSpecialization_2 > 0)
        //                    CatgTypeIdlist.Add(item.HSpecialization_2.Value);
        //                if (item.HSpecialization_3 > 0)
        //                    CatgTypeIdlist.Add(item.HSpecialization_3.Value);
        //                if (item.HSpecialization_4 > 0)
        //                    CatgTypeIdlist.Add(item.HSpecialization_4.Value);
        //                if (CatgTypeIdlist.Count > 0)
        //                    Empinfo.specialization = string.Join(",", CatgTypeIdlist.Distinct());
        //                else
        //                    Empinfo.specialization = null;
        //                Empinfo.ExperienceMonth = item.EmpExpMonth != null ? item.EmpExpMonth : 0;
        //                Empinfo.ExperienceYear = item.EmpExpYear != null ? item.EmpExpYear : 0;
        //                Empinfo.CreatedOn = DateTime.Now;
        //                Empinfo.Updated_Date = DateTime.Now;
        //                SPA.CCTSP_User.Add(Empinfo);
        //                SPA.SaveChanges();
        //                item.EmpId = Empinfo.UserId;
        //                EmployeeList.Add(Empinfo);
        //            }

        //        }
        //        var EmpIdlist = EmployeeList.Select(c => c.UserId).Distinct().ToList();
        //        foreach (var EmpId in EmployeeList)
        //        {
        //            Image = new Models.FreecustomerImage();
        //            Image.EmpId = EmpId.UserId;
        //            Image.EmpImage = Emplist.Where(c => c.EmpId == EmpId.UserId).Select(c => c.Hidden_EmpImage).FirstOrDefault();
        //            ImageList.Add(Image);
        //        }
        //        var JsonImageList = js.Serialize(ImageList);
        //        CommonImageUpload(JsonImageList);
        //        #endregion
        //        /*Shop Week Schedule and Employee Week Schedule*/
        //        #region WeekSchedule Info

        //        if (FreeCustomerInfo.LendingObject != null)
        //        {
        //            var WeekSch = js.Deserialize<List<Models.LendingWeekScheduleDetails>>(FreeCustomerInfo.LendingObject);
        //            List<CCTSP_CategoryDetails> List = new List<CCTSP_CategoryDetails>();
        //            CCTSP_CategoryDetails Detail = new CCTSP_CategoryDetails();
        //            //int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId ==Schlid).Select(c => c.Lang_id).FirstOrDefault().Value;
        //            var Closed = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.ActiveStatus == "A" && c.Order_id == 41 && c.Lang_id == 1).FirstOrDefault();
        //            var StatusClosed = Closed.Value;
        //            var EnLabelStatusClosed = Closed.English_Label;
        //            foreach (var item in WeekSch)
        //            {
        //                Detail = new CCTSP_CategoryDetails();
        //                Detail.CatgId = 148;
        //                Detail.CatgType = "LendingDay";
        //                Detail.ActiveStatus = "A";
        //                Detail.DomainType = Convert.ToInt32(Schlid);
        //                Detail.CatgDesc = item.Catgdesc != null && item.Catgdesc != EnLabelStatusClosed ? item.Catgdesc : EnLabelStatusClosed;
        //                Detail.Gender = item.Gender != null && item.Gender != EnLabelStatusClosed ? item.Gender : EnLabelStatusClosed;
        //                Detail.Value = item.CatgTypeId;
        //                List.Add(Detail);
        //            }
        //            SPA.CCTSP_CategoryDetails.AddRange(List);
        //            SPA.SaveChanges();
        //            List<string> DaysList = new List<string>();
        //            CTSP_SolutionMaster Days = new CTSP_SolutionMaster();
        //            List<CTSP_SolutionMaster> SolutionList = new List<CTSP_SolutionMaster>();
        //            List<SPA_EmployeeSchedulers> BulkAddUserDays = new List<SPA_EmployeeSchedulers>();
        //            foreach (var EmpId in EmpIdlist)
        //            {
        //                foreach (var item in WeekSch.Where(c => (c.Catgdesc != "Closed" || c.Gender != "Closed") && c.Catgdesc != null && c.Gender != null).ToList())
        //                {
        //                    Days = new CTSP_SolutionMaster();
        //                    Days.SchId = Schlid;
        //                    Days.CatgTypeId = 11143;
        //                    Days.UserId = EmpId;
        //                    Days.CraetedOn = DateTime.Now;
        //                    Days.SectionName = "EmployeeDays";
        //                    Days.Activestatus = "A";
        //                    Days.SectionDesc = item.Day.Trim();
        //                    SolutionList.Add(Days);
        //                    DaysList.Add(item.Day.Trim());
        //                }
        //            }

        //            SPA_EmployeeSchedulers EmployeeSchedule = new SPA_EmployeeSchedulers();
        //            foreach (var EmpId in EmpIdlist)
        //            {
        //                foreach (var Daysadded in WeekSch.Where(c => (c.Catgdesc != "Closed" || c.Gender != "Closed") && c.Catgdesc != null && c.Gender != null).ToList())
        //                {
        //                    #region Section1
        //                    EmployeeSchedule = new SPA_EmployeeSchedulers();
        //                    EmployeeSchedule.ActiveStatus = "A";
        //                    EmployeeSchedule.SchId = Schlid;
        //                    EmployeeSchedule.UserId = EmpId;
        //                    EmployeeSchedule.WeekDay = Daysadded.Day.Trim();
        //                    if (Daysadded.Catgdesc != "Closed" && Daysadded.Catgdesc != null)
        //                    {
        //                        EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[0]).TimeOfDay;
        //                        EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
        //                    }
        //                    else
        //                    {
        //                        EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
        //                        EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
        //                    }
        //                    EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
        //                    EmployeeSchedule.CreatedOn = DateTime.Now;
        //                    BulkAddUserDays.Add(EmployeeSchedule);


        //                    #endregion
        //                    #region Section2

        //                    EmployeeSchedule = new SPA_EmployeeSchedulers();
        //                    EmployeeSchedule.ActiveStatus = "A";
        //                    EmployeeSchedule.SchId = Schlid;
        //                    EmployeeSchedule.UserId = EmpId;
        //                    EmployeeSchedule.WeekDay = Daysadded.Day.Trim();
        //                    if (Daysadded.Gender != "Closed")
        //                    {
        //                        EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
        //                        EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Gender.Split('-')[1]).TimeOfDay;
        //                    }
        //                    else
        //                    {
        //                        EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
        //                        EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
        //                    }
        //                    EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
        //                    EmployeeSchedule.CreatedOn = DateTime.Now;
        //                    BulkAddUserDays.Add(EmployeeSchedule);

        //                    #endregion
        //                }
        //            }
        //            if (BulkAddUserDays.Count > 0)
        //            {
        //                SPA.SPA_EmployeeSchedulers.AddRange(BulkAddUserDays);
        //                SPA.SaveChanges();
        //            }
        //            SPA.CTSP_SolutionMaster.AddRange(SolutionList);
        //            SPA.SaveChanges();
        //        }

        //        #endregion
        //        /*Time Slot for Employee*/
        //        #region TimeSlotInfo
        //        DateTime EuropeDate = DateTime.Now;
        //        var queryU = "insert into CCTSP_SchedulerMaster ([SchId],[WeekDay],[StartTime],[Duration],[EndTime],[PeriodNumber],[PeriodType],[GroupId],[CreatedOn],[ActiveStatus],[AcdYearId]) select " + Schlid + ",[WeekDay],[StartTime],[Duration],[EndTime],[PeriodNumber],[PeriodType],[GroupId],getdate(),[ActiveStatus],[AcdYearId] from CCTSP_SchedulerMaster where SchId = 248 and ActiveStatus = 'A'";
        //        SPA.Database.ExecuteSqlCommand(queryU);

        //        List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
        //        List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMasterList = new List<SPA_SchedulerSlotMaster>();
        //        string time = "";
        //        TimeSpan OriginalMinutes;
        //        SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
        //        // long UserId = Empinfo.UserId;
        //        int CatgTypeId = 11194;
        //        var data = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatgTypeId && c.DomainType == 251).FirstOrDefault();
        //        var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == Schlid && c.ActiveStatus == "A").ToList();
        //        #region MinuteCode
        //        if (data.CatgDesc.Contains("Minute"))
        //        {
        //            time = data.CatgDesc.Replace("Minute", "").Replace("s", "");
        //            OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(time.Trim()));
        //            foreach (var UserId in EmpIdlist)
        //            {
        //                var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == Schlid && c.UserId == UserId).ToList();
        //                #region GetDays of Employee
        //                foreach (var Term in UserDays)
        //                {
        //                    var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == Schlid && c.UserId == UserId).ToList();
        //                    #region SlotEntry
        //                    if (SlotTimeNew.Count() > 0)
        //                    {
        //                        foreach (var item in SlotTimeNew)
        //                        {
        //                            TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
        //                            while (TempTime <= item.EndTime)
        //                            {
        //                                var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
        //                                if (BulkSchedulerSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
        //                                {
        //                                    schedule = new SPA_SchedulerSlotMaster();
        //                                    schedule.SchedulerId = WeekDayForSelected;
        //                                    schedule.StartTime = TempTime;
        //                                    schedule.UserId = UserId;
        //                                    schedule.CreatedOn = EuropeDate;
        //                                    schedule.ActiveStatus = "A";
        //                                    schedule.SlotDue = data.CatgDesc;
        //                                    schedule.SchId = Schlid;
        //                                    BulkSchedulerSlotMaster.Add(schedule);
        //                                }
        //                                TempTime = TempTime + OriginalMinutes;
        //                            }
        //                        }
        //                    }
        //                    #endregion
        //                }


        //                #endregion
        //                BulkSchedulerSlotMasterList.AddRange(BulkSchedulerSlotMaster);
        //                BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
        //            }
        //            if (BulkSchedulerSlotMasterList.Count > 0)
        //            {
        //                SPA.SPA_SchedulerSlotMaster.AddRange(BulkSchedulerSlotMasterList);
        //                SPA.SaveChanges();
        //            }

        //        }
        //        #endregion

        //        #endregion
        //        /*Product Are Added To Shop, Groups are assigned and Product are also assigned to Employee*/
        //        #region ProductInfo

        //        CCTSP_CategoryDetails GroupInfo = new CCTSP_CategoryDetails();
        //        GroupInfo.CatgDesc = "Default";
        //        GroupInfo.CatgType = "Def";
        //        GroupInfo.CatgId = 126;
        //        GroupInfo.DomainType = Convert.ToInt32(Schlid);
        //        GroupInfo.CreatedOn = EuropeDate;
        //        GroupInfo.Update_Date = EuropeDate;
        //        GroupInfo.ActiveStatus = "A";
        //        GroupInfo.Lang_Id = 1;
        //        GroupInfo.Group_orderdata = 1;
        //        SPA.CCTSP_CategoryDetails.Add(GroupInfo);
        //        SPA.SaveChanges();

        //        CCTSP_CategoryDetails ProductInfo = new CCTSP_CategoryDetails();
        //        ProductInfo.CatgDesc = "Consultancy";
        //        ProductInfo.CatgId = 111;
        //        ProductInfo.Gender = "Both";
        //        ProductInfo.DomainType = Convert.ToInt32(Schlid);
        //        ProductInfo.CatgType = "Con";
        //        ProductInfo.CreatedOn = EuropeDate;
        //        ProductInfo.Update_Date = EuropeDate;
        //        ProductInfo.ActiveStatus = "A";
        //        SPA.CCTSP_CategoryDetails.Add(ProductInfo);
        //        SPA.SaveChanges();

        //        CTSP_SolutionMaster ProductDetails = new CTSP_SolutionMaster();
        //        ProductDetails.CatgTypeId = ProductInfo.CatgTypeId;
        //        ProductDetails.Duration = "15";
        //        ProductDetails.Amount = FreeCustomerInfo.EmpCharge;
        //        ProductDetails.SectionName = "ChoosProduct";
        //        ProductDetails.SectionDesc = "Consultancy";
        //        ProductDetails.Activestatus = "A";
        //        ProductDetails.CraetedOn = EuropeDate;
        //        ProductDetails.Update_Date = EuropeDate;
        //        ProductDetails.Activestatus = "A";
        //        ProductDetails.OrderData = 1;
        //        ProductDetails.SchId = Schlid;
        //        SPA.CTSP_SolutionMaster.Add(ProductDetails);
        //        SPA.SaveChanges();

        //        CCTSP_GroupProductDetails Groupdetails = new CCTSP_GroupProductDetails();
        //        Groupdetails.GroupIdDefault = 11693;
        //        Groupdetails.GroupIdByShop = GroupInfo.CatgTypeId;
        //        Groupdetails.ProductId = ProductInfo.CatgTypeId;
        //        Groupdetails.schlId = Schlid;
        //        Groupdetails.Attributeorder = 1;
        //        Groupdetails.ActiveStatus = "A";
        //        Groupdetails.CreatedOn = EuropeDate;
        //        SPA.CCTSP_GroupProductDetails.Add(Groupdetails);
        //        SPA.SaveChanges();

        //        CTSP_SolutionMaster AssignProdut = new CTSP_SolutionMaster();
        //        List<CTSP_SolutionMaster> AssignProductList = new List<CTSP_SolutionMaster>();
        //        foreach (var EmpId in EmpIdlist)
        //        {
        //            AssignProdut = new CTSP_SolutionMaster();
        //            AssignProdut.Activestatus = "A";
        //            AssignProdut.UserId = EmpId;
        //            AssignProdut.SectionName = "EmployeeProduct";
        //            AssignProdut.CatgTypeId = 11145;
        //            AssignProdut.SchId = Schlid;
        //            AssignProdut.CraetedOn = EuropeDate;
        //            int catgTypeId = Convert.ToInt32(ProductInfo.CatgTypeId);
        //            var ProductData = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.SchId == Schlid && c.SectionName == "ChoosProduct" && c.CatgTypeId == catgTypeId).FirstOrDefault();
        //            if (ProductData != null)
        //            {
        //                AssignProdut.SectionDesc = ProductData.CCTSP_CategoryDetails.CatgDesc;
        //                AssignProdut.Image = ProductData.SectionDesc;
        //                AssignProdut.Amount = ProductData.Amount;
        //                AssignProdut.Duration = ProductData.Duration;
        //                AssignProdut.ItenName = ProductData.CCTSP_CategoryDetails.Gender;
        //            }
        //            AssignProductList.Add(AssignProdut);
        //        }
        //        SPA.CTSP_SolutionMaster.AddRange(AssignProductList);
        //        SPA.SaveChanges();
        //        #endregion
        //    }
        //    catch (Exception Ex)
        //    {

        //    }
        //}
        public void customerInfo(long Schlid, Models.FreeCustomer FreeCustomerInfo)
        {
            try
            {

                /*All The Information Related to Employee*/
                #region EmpInfo
                CCTSP_User Empinfo = new CCTSP_User();
                DateTime EuropeDate = DateTime.Now;
                List<CCTSP_User> EmployeeList = new List<CCTSP_User>();
                List<Models.EmployeeJsonModel> Emplist = new List<Models.EmployeeJsonModel>();
                Models.FreecustomerImage Image = new Models.FreecustomerImage();
                List<Models.FreecustomerImage> ImageList = new List<Models.FreecustomerImage>();
                if (FreeCustomerInfo.EmpJson != null)
                {
                    Emplist = js.Deserialize<List<Models.EmployeeJsonModel>>(FreeCustomerInfo.EmpJson);

                    foreach (var item in Emplist)
                    {
                        Empinfo = new CCTSP_User();
                        Empinfo.FirstName = item.EmpFName;
                        Empinfo.LastName = item.EmpLName;
                        Empinfo.Gender = item.EmpGender;
                        Empinfo.PhoneNo = item.EmpNumber;
                        Empinfo.LoginName = item.EmpNumber != null ? item.EmpNumber : "0123456789";
                        Empinfo.Password = "Emp@1234";
                        Empinfo.EmailId = item.EmpEmail;
                        Empinfo.Degree = item.EmpDegree;
                        Empinfo.skypeId = item.EmpSkypeId;
                        Empinfo.RoleId = 3;
                        Empinfo.SchId = Schlid;
                        Empinfo.ActiveStatus = "A";
                        List<long> CatgTypeIdlist = new List<long>();
                        if (item.HSpecialization_1 > 0)
                            CatgTypeIdlist.Add(item.HSpecialization_1.Value);
                        if (item.HSpecialization_2 > 0)
                            CatgTypeIdlist.Add(item.HSpecialization_2.Value);
                        if (item.HSpecialization_3 > 0)
                            CatgTypeIdlist.Add(item.HSpecialization_3.Value);
                        if (item.HSpecialization_4 > 0)
                            CatgTypeIdlist.Add(item.HSpecialization_4.Value);
                        if (CatgTypeIdlist.Count > 0)
                            Empinfo.specialization = string.Join(",", CatgTypeIdlist.Distinct());
                        else
                            Empinfo.specialization = null;
                        Empinfo.ExperienceMonth = item.EmpExpMonth != null ? item.EmpExpMonth : 0;
                        Empinfo.ExperienceYear = item.EmpExpYear != null ? item.EmpExpYear : 0;
                        Empinfo.CreatedOn = DateTime.Now;
                        Empinfo.Updated_Date = DateTime.Now;
                        SPA.CCTSP_User.Add(Empinfo);
                        SPA.SaveChanges();
                        item.EmpId = Empinfo.UserId;
                        EmployeeList.Add(Empinfo);
                    }
                    CCTSP_CategoryDetails GroupInfo = new CCTSP_CategoryDetails();
                    GroupInfo.CatgDesc = "Default";
                    GroupInfo.CatgType = "Def";
                    GroupInfo.CatgId = 126;
                    GroupInfo.DomainType = Convert.ToInt32(Schlid);
                    GroupInfo.CreatedOn = EuropeDate;
                    GroupInfo.Update_Date = EuropeDate;
                    GroupInfo.ActiveStatus = "A";
                    GroupInfo.Lang_Id = 1;
                    GroupInfo.Group_orderdata = 1;
                    SPA.CCTSP_CategoryDetails.Add(GroupInfo);
                    SPA.SaveChanges();

                    foreach (var item in Emplist)
                    {
                        #region ProductInfo
                        CCTSP_CategoryDetails ProductInfo = new CCTSP_CategoryDetails();
                        ProductInfo.CatgDesc = "Consultancy";
                        ProductInfo.CatgId = 111;
                        ProductInfo.Gender = "Both";
                        ProductInfo.DomainType = Convert.ToInt32(Schlid);
                        ProductInfo.CatgType = "Con";
                        ProductInfo.CreatedOn = EuropeDate;
                        ProductInfo.Update_Date = EuropeDate;
                        ProductInfo.ActiveStatus = "A";
                        SPA.CCTSP_CategoryDetails.Add(ProductInfo);
                        SPA.SaveChanges();

                        CTSP_SolutionMaster ProductDetails = new CTSP_SolutionMaster();
                        ProductDetails.CatgTypeId = ProductInfo.CatgTypeId;
                        ProductDetails.Duration = "15";
                        ProductDetails.Amount = item.EmpCharge;
                        ProductDetails.SectionName = "ChoosProduct";
                        ProductDetails.SectionDesc = "Consultancy";
                        ProductDetails.Activestatus = "A";
                        ProductDetails.CraetedOn = EuropeDate;
                        ProductDetails.Update_Date = EuropeDate;
                        ProductDetails.Activestatus = "A";
                        ProductDetails.OrderData = 1;
                        ProductDetails.SchId = Schlid;
                        SPA.CTSP_SolutionMaster.Add(ProductDetails);
                        SPA.SaveChanges();

                        CCTSP_GroupProductDetails Groupdetails = new CCTSP_GroupProductDetails();
                        Groupdetails.GroupIdDefault = 11693;
                        Groupdetails.GroupIdByShop = GroupInfo.CatgTypeId;
                        Groupdetails.ProductId = ProductInfo.CatgTypeId;
                        Groupdetails.schlId = Schlid;
                        Groupdetails.Attributeorder = 1;
                        Groupdetails.ActiveStatus = "A";
                        Groupdetails.CreatedOn = EuropeDate;
                        SPA.CCTSP_GroupProductDetails.Add(Groupdetails);
                        SPA.SaveChanges();

                        CTSP_SolutionMaster AssignProdut = new CTSP_SolutionMaster();
                        AssignProdut = new CTSP_SolutionMaster();
                        AssignProdut.Activestatus = "A";
                        AssignProdut.UserId = item.EmpId;
                        AssignProdut.SectionName = "EmployeeProduct";
                        AssignProdut.CatgTypeId = 11145;
                        AssignProdut.SchId = Schlid;
                        AssignProdut.CraetedOn = EuropeDate;
                        int catgTypeId = Convert.ToInt32(ProductInfo.CatgTypeId);
                        var ProductData = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.SchId == Schlid && c.SectionName == "ChoosProduct" && c.CatgTypeId == catgTypeId).FirstOrDefault();
                        if (ProductData != null)
                        {
                            AssignProdut.SectionDesc = ProductData.CCTSP_CategoryDetails.CatgDesc;
                            AssignProdut.Image = ProductData.SectionDesc;
                            AssignProdut.Amount = ProductData.Amount;
                            AssignProdut.Duration = ProductData.Duration;
                            AssignProdut.ItenName = ProductData.CCTSP_CategoryDetails.Gender;
                        }
                        SPA.CTSP_SolutionMaster.Add(AssignProdut);
                        SPA.SaveChanges();
                        #endregion
                    }

                }
                var EmpIdlist = EmployeeList.Select(c => c.UserId).Distinct().ToList();
                foreach (var EmpId in EmployeeList)
                {
                    Image = new Models.FreecustomerImage();
                    Image.EmpId = EmpId.UserId;
                    Image.EmpImage = Emplist.Where(c => c.EmpId == EmpId.UserId).Select(c => c.Hidden_EmpImage).FirstOrDefault();
                    ImageList.Add(Image);
                }
                var JsonImageList = js.Serialize(ImageList);
                CommonImageUpload(JsonImageList);
                /*Product Are Added To Shop, Groups are assigned and Product are also assigned to Employee*/



                #endregion
                /*Shop Week Schedule and Employee Week Schedule*/
                #region WeekSchedule Info

                if (FreeCustomerInfo.LendingObject != null)
                {
                    var WeekSch = js.Deserialize<List<Models.LendingWeekScheduleDetails>>(FreeCustomerInfo.LendingObject);
                    List<CCTSP_CategoryDetails> List = new List<CCTSP_CategoryDetails>();
                    CCTSP_CategoryDetails Detail = new CCTSP_CategoryDetails();
                    //int LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId ==Schlid).Select(c => c.Lang_id).FirstOrDefault().Value;
                    var Closed = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.ActiveStatus == "A" && c.Order_id == 41 && c.Lang_id == 1).FirstOrDefault();
                    var StatusClosed = Closed.Value;
                    var EnLabelStatusClosed = Closed.English_Label;
                    foreach (var item in WeekSch)
                    {
                        Detail = new CCTSP_CategoryDetails();
                        Detail.CatgId = 148;
                        Detail.CatgType = "LendingDay";
                        Detail.ActiveStatus = "A";
                        Detail.DomainType = Convert.ToInt32(Schlid);
                        Detail.CatgDesc = item.Catgdesc != null && item.Catgdesc != EnLabelStatusClosed ? item.Catgdesc : EnLabelStatusClosed;
                        Detail.Gender = item.Gender != null && item.Gender != EnLabelStatusClosed ? item.Gender : EnLabelStatusClosed;
                        Detail.Value = item.CatgTypeId;
                        List.Add(Detail);
                    }
                    SPA.CCTSP_CategoryDetails.AddRange(List);
                    SPA.SaveChanges();
                    List<string> DaysList = new List<string>();
                    CTSP_SolutionMaster Days = new CTSP_SolutionMaster();
                    List<CTSP_SolutionMaster> SolutionList = new List<CTSP_SolutionMaster>();
                    List<SPA_EmployeeSchedulers> BulkAddUserDays = new List<SPA_EmployeeSchedulers>();
                    foreach (var EmpId in EmpIdlist)
                    {
                        foreach (var item in WeekSch.Where(c => (c.Catgdesc != "Closed" || c.Gender != "Closed") && c.Catgdesc != null && c.Gender != null).ToList())
                        {
                            Days = new CTSP_SolutionMaster();
                            Days.SchId = Schlid;
                            Days.CatgTypeId = 11143;
                            Days.UserId = EmpId;
                            Days.CraetedOn = DateTime.Now;
                            Days.SectionName = "EmployeeDays";
                            Days.Activestatus = "A";
                            Days.SectionDesc = item.Day.Trim();
                            SolutionList.Add(Days);
                            DaysList.Add(item.Day.Trim());
                        }
                    }

                    SPA_EmployeeSchedulers EmployeeSchedule = new SPA_EmployeeSchedulers();
                    foreach (var EmpId in EmpIdlist)
                    {
                        foreach (var Daysadded in WeekSch.Where(c => (c.Catgdesc != "Closed" || c.Gender != "Closed") && c.Catgdesc != null && c.Gender != null).ToList())
                        {
                            #region Section1
                            EmployeeSchedule = new SPA_EmployeeSchedulers();
                            EmployeeSchedule.ActiveStatus = "A";
                            EmployeeSchedule.SchId = Schlid;
                            EmployeeSchedule.UserId = EmpId;
                            EmployeeSchedule.WeekDay = Daysadded.Day.Trim();
                            if (Daysadded.Catgdesc != "Closed" && Daysadded.Catgdesc != null)
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[0]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
                            }
                            else
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
                            }
                            EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
                            EmployeeSchedule.CreatedOn = DateTime.Now;
                            BulkAddUserDays.Add(EmployeeSchedule);


                            #endregion
                            #region Section2

                            EmployeeSchedule = new SPA_EmployeeSchedulers();
                            EmployeeSchedule.ActiveStatus = "A";
                            EmployeeSchedule.SchId = Schlid;
                            EmployeeSchedule.UserId = EmpId;
                            EmployeeSchedule.WeekDay = Daysadded.Day.Trim();
                            if (Daysadded.Gender != "Closed")
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Gender.Split('-')[0]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Gender.Split('-')[1]).TimeOfDay;
                            }
                            else
                            {
                                EmployeeSchedule.StartTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
                                EmployeeSchedule.EndTime = DateTime.Parse(Daysadded.Catgdesc.Split('-')[1]).TimeOfDay;
                            }
                            EmployeeSchedule.Duration = EmployeeSchedule.EndTime - EmployeeSchedule.StartTime;
                            EmployeeSchedule.CreatedOn = DateTime.Now;
                            BulkAddUserDays.Add(EmployeeSchedule);

                            #endregion
                        }
                    }
                    if (BulkAddUserDays.Count > 0)
                    {
                        SPA.SPA_EmployeeSchedulers.AddRange(BulkAddUserDays);
                        SPA.SaveChanges();
                    }
                    SPA.CTSP_SolutionMaster.AddRange(SolutionList);
                    SPA.SaveChanges();
                }

                #endregion
                /*Time Slot for Employee*/
                #region TimeSlotInfo

                var queryU = "insert into CCTSP_SchedulerMaster ([SchId],[WeekDay],[StartTime],[Duration],[EndTime],[PeriodNumber],[PeriodType],[GroupId],[CreatedOn],[ActiveStatus],[AcdYearId]) select " + Schlid + ",[WeekDay],[StartTime],[Duration],[EndTime],[PeriodNumber],[PeriodType],[GroupId],getdate(),[ActiveStatus],[AcdYearId] from CCTSP_SchedulerMaster where SchId = 248 and ActiveStatus = 'A'";
                SPA.Database.ExecuteSqlCommand(queryU);

                List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
                List<SPA_SchedulerSlotMaster> BulkSchedulerSlotMasterList = new List<SPA_SchedulerSlotMaster>();
                string time = "";
                TimeSpan OriginalMinutes;
                SPA_SchedulerSlotMaster schedule = new SPA_SchedulerSlotMaster();
                // long UserId = Empinfo.UserId;
                int CatgTypeId = 11194;
                var data = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatgTypeId && c.DomainType == 251).FirstOrDefault();
                var WeekDaysSchool = SPA.CCTSP_SchedulerMaster.Where(c => c.SchId == Schlid && c.ActiveStatus == "A").ToList();
                #region MinuteCode
                if (data.CatgDesc.Contains("Minute"))
                {
                    time = data.CatgDesc.Replace("Minute", "").Replace("s", "");
                    OriginalMinutes = TimeSpan.FromMinutes(Convert.ToInt32(time.Trim()));
                    foreach (var UserId in EmpIdlist)
                    {
                        var UserDays = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CatgTypeId == 11143 && c.SectionName == "EmployeeDays" && c.SchId == Schlid && c.UserId == UserId).ToList();
                        #region GetDays of Employee
                        foreach (var Term in UserDays)
                        {
                            var SlotTimeNew = SPA.SPA_EmployeeSchedulers.Where(c => c.WeekDay == Term.SectionDesc && c.ActiveStatus == "A" && c.SchId == Schlid && c.UserId == UserId).ToList();
                            #region SlotEntry
                            if (SlotTimeNew.Count() > 0)
                            {
                                foreach (var item in SlotTimeNew)
                                {
                                    TimeSpan TempTime = DateTime.Parse(item.StartTime.ToString()).TimeOfDay;
                                    while (TempTime <= item.EndTime)
                                    {
                                        var WeekDayForSelected = WeekDaysSchool.Where(c => c.WeekDay == Term.SectionDesc).Select(c => c.SchedulerId).FirstOrDefault();
                                        if (BulkSchedulerSlotMaster.Where(c => c.StartTime == TempTime && c.SchedulerId == WeekDayForSelected).Count() == 0)
                                        {
                                            schedule = new SPA_SchedulerSlotMaster();
                                            schedule.SchedulerId = WeekDayForSelected;
                                            schedule.StartTime = TempTime;
                                            schedule.UserId = UserId;
                                            schedule.CreatedOn = EuropeDate;
                                            schedule.ActiveStatus = "A";
                                            schedule.SlotDue = data.CatgDesc;
                                            schedule.SchId = Schlid;
                                            BulkSchedulerSlotMaster.Add(schedule);
                                        }
                                        TempTime = TempTime + OriginalMinutes;
                                    }
                                }
                            }
                            #endregion
                        }


                        #endregion
                        BulkSchedulerSlotMasterList.AddRange(BulkSchedulerSlotMaster);
                        BulkSchedulerSlotMaster = new List<SPA_SchedulerSlotMaster>();
                    }
                    if (BulkSchedulerSlotMasterList.Count > 0)
                    {
                        SPA.SPA_SchedulerSlotMaster.AddRange(BulkSchedulerSlotMasterList);
                        SPA.SaveChanges();
                    }

                }
                #endregion

                #endregion

            }
            catch (Exception Ex)
            {

            }
        }
        public string CommonImageUpload(string ImageList)
        {
            var BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            var request = (HttpWebRequest)WebRequest.Create(BaseUrl + "/Customer/FreeCustomerImageUpload");
            var postData = "ImageList=" + HttpUtility.UrlEncode(ImageList);
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            var responsestring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responsestring;
        }

        public void CommonAddRegisterationDetails(int? LangId, long? PayId)
        {
            CCTSP_SchoolMaster ShopDetails = new CCTSP_SchoolMaster();
            CCTSP_User ShopOwnerDetails = new CCTSP_User();
            CCTSP_User EmployeeDetails = new CCTSP_User();
            CCTSP_CategoryDetails ShopMaincategory = new CCTSP_CategoryDetails();
            CCTSP_LendingPageMaster LandingData = new CCTSP_LendingPageMaster();
            spa_BillingMaster BillMaster = new spa_BillingMaster();
            List<spa_BillingDetails> BillDetails = new List<spa_BillingDetails>();
            var CurrentDate = ZonalDate(null);
            TimeZoneInfo EuropeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime EuropeDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EuropeZone);
            if (HttpContext.Current.Session["Registration"] != null && HttpContext.Current.Session["Registration"] != "")
            {
                Models.Registration RegistrationDetails;
                RegistrationDetails = (Models.Registration)HttpContext.Current.Session["Registration"];
                //add code for shop details
                if (RegistrationDetails != null)
                {
                    var currency = SPA.spa_currency.Where(c => c.name_country == RegistrationDetails.shopcountry_hidden && c.ActiveStatus == "A").Select(c => c.currency).FirstOrDefault();
                    //Shop Info
                    #region ShopDetails
                    ShopDetails.SchlName = RegistrationDetails.shopname;
                    ShopDetails.SchPin = RegistrationDetails.shopzipcode;
                    ShopDetails.Schcountry = RegistrationDetails.shopcountry_hidden;
                    ShopDetails.SchlAddress = RegistrationDetails.shopaddress;
                    ShopDetails.SchlWebsite = RegistrationDetails.Linktomywebsite;
                    ShopDetails.SchlCity = RegistrationDetails.shopcity_hidden;
                    ShopDetails.SchlState = RegistrationDetails.shopState_hidden;
                    ShopDetails.SHOPTYPE = RegistrationDetails.ShopTypeName;
                    ShopDetails.SchlEmployeeStrength = Convert.ToInt32(RegistrationDetails.Numberofemployees);
                    ShopDetails.HostingProvider = RegistrationDetails.HostingProvider;
                    ShopDetails.Hosting_UserName = RegistrationDetails.HostingUserName;
                    ShopDetails.Hosting_Password = RegistrationDetails.HostingPassword;
                    ShopDetails.ActiveStatus = "R";
                    ShopDetails.TimeSlot = RegistrationDetails.TimeSlot;
                    // ShopDetails.Terms_conditions = Status;
                    ShopDetails.timezone_id = RegistrationDetails.Timezone;
                    ShopDetails.Currency = currency;
                    ShopDetails.SchlLandline2 = RegistrationDetails.phoneno;
                    ShopDetails.SchlMobile1 = RegistrationDetails.phoneno;
                    ShopDetails.SchlEmail = RegistrationDetails.emailid;
                    ShopDetails.CreatedOn = EuropeDate;
                    ShopDetails.PaymentType = RegistrationDetails.PaymentType;
                    ShopDetails.Lang_id = RegistrationDetails.ShopLangId;
                    ShopDetails.AllOW_PastDate = true;
                    //if (RegistrationDetails.ShopLanguage == "LangEnglish")
                    //{
                    //    ShopDetails.Lang_id = 1;
                    //}
                    //if (RegistrationDetails.ShopLanguage == "LangGerman")
                    //{
                    //    ShopDetails.Lang_id = 2;
                    //}
                    //if (RegistrationDetails.ShopLanguage == "LangFrench")
                    //{
                    //    ShopDetails.Lang_id = 3;
                    //}
                    ShopDetails.MainCategory = RegistrationDetails.MainCatgId;
                    SPA.CCTSP_SchoolMaster.Add(ShopDetails);
                    SPA.SaveChanges();
                    #endregion
                    //Add LandingData
                    #region
                    string ShopStatus = ConfigurationManager.AppSettings["WebsiteName"];
                    LandingData.Schid = ShopDetails.SchlId;
                    LandingData.Original_Website = RegistrationDetails.ShopUrl + "." + ShopStatus;
                    LandingData.Azure_Website = RegistrationDetails.ShopUrl + "." + ShopStatus;
                    SPA.CCTSP_LendingPageMaster.Add(LandingData);
                    SPA.SaveChanges();
                    #endregion
                    //Add Payment data 
                    #region Payment Section
                    if (StPay == "1")
                    {
                        //Get All Id of one whose Input is not taken
                        var AllCatgDetailId = RegistrationDetails.PaymentDetails_id;
                        //Get All information for one whose UserInput is not taken
                        var Result = getClickPaymentDetails(AllCatgDetailId, RegistrationDetails.Timezone);
                        //List<Models.DebitPayment> UserSpecific = new List<Models.DebitPayment>();
                        //For One whose input is taken
                        //if (!string.IsNullOrEmpty(frm["PayDetail"]))
                        //UserSpecific = js.Deserialize<List<Models.DebitPayment>>(Convert.ToString(frm["PayDetail"]));
                        var TotAmount = Result.Where(c => (c.CatPayment == "2" || c.CatPayment == "3") && c.PaymentType != null).Select(c => c.Amount).Sum() /*+ UserSpecific.Where(c => (c.CatPayment == "2" || c.CatPayment == "3")).Select(c => c.Amount).Sum()*/;
                        var TotalCredit = Result.Where(c => (c.CatPayment == "1" || c.CatPayment == "2") && c.PaymentType != null).Select(c => c.Amount).Sum() /*+ UserSpecific.Where(c => (c.CatPayment == "1" || c.CatPayment == "2")).Select(c => c.credit).Sum()*/;
                        //Billing Master information
                        #region BillMaster

                        BillMaster.ShopId = ShopDetails.SchlId;
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
                        SPA.spa_BillingMaster.Add(BillMaster);
                        SPA.SaveChanges();
                        #endregion
                        //Categories of Payment
                        #region BillingDetails

                        foreach (var ReBillDetail in Result)
                        {
                            string PayDetailId = Convert.ToString(ReBillDetail.PaymentDetail_Id);
                            spa_BillingDetails bDetails = new spa_BillingDetails();
                            bDetails.ReceiptId = BillMaster.Receipt_Id;
                            bDetails.created_on = EuropeDate;
                            bDetails.Updated_on = EuropeDate;
                            bDetails.ShopId = ShopDetails.SchlId;
                            bDetails.ActiveStatus = "A";
                            bDetails.RechargeId = ReBillDetail.PaymentCatgId;
                            if ((ReBillDetail.CatPayment == "2" || ReBillDetail.CatPayment == "3") && ReBillDetail.PaymentType != null)
                                bDetails.Amount = ReBillDetail.Amount;
                            if ((ReBillDetail.CatPayment == "1" || ReBillDetail.CatPayment == "2") && ReBillDetail.PaymentType != null)
                                bDetails.Credit = ReBillDetail.Amount;
                            //if ((ReBillDetail.CatPayment == "2" || ReBillDetail.CatPayment == "3") && ReBillDetail.PaymentType == null)
                            //    bDetails.Amount = UserSpecific.Where(c => c.CatgId == PayDetailId).Select(c => c.Amount).FirstOrDefault();
                            //if ((ReBillDetail.CatPayment == "1" || ReBillDetail.CatPayment == "2") && ReBillDetail.PaymentType == null)
                            //    bDetails.Credit = UserSpecific.Where(c => c.CatgId == PayDetailId).Select(c => c.credit).FirstOrDefault();
                            bDetails.receivedAmount = bDetails.Amount;
                            if (ReBillDetail.Duration != 0)
                                bDetails.Duration = Convert.ToString(ReBillDetail.Duration);
                            BillDetails.Add(bDetails);
                        }
                        SPA.spa_BillingDetails.AddRange(BillDetails);
                        SPA.SaveChanges();
                        #endregion
                    }
                    #endregion
                    //add code for shopowner details                    
                    #region ShopownerDetail
                    ShopOwnerDetails.FirstName = RegistrationDetails.FirstName;
                    ShopOwnerDetails.LastName = RegistrationDetails.FamilyName;
                    ShopOwnerDetails.PhoneNo = RegistrationDetails.phoneno;
                    ShopOwnerDetails.EmailId = RegistrationDetails.emailid;
                    ShopOwnerDetails.Gender = RegistrationDetails.Title;
                    ShopOwnerDetails.Pincode = Convert.ToInt32(RegistrationDetails.Zipcode.ToString());
                    ShopOwnerDetails.RoleId = 1;
                    ShopOwnerDetails.ActiveStatus = "R";
                    ShopOwnerDetails.Address1 = RegistrationDetails.Address;
                    if (RegistrationDetails.password != null)
                    {
                        ShopOwnerDetails.Password = RegistrationDetails.password;
                    }
                    else
                    {
                        ShopOwnerDetails.Password = "Admin@12345";
                    }
                    ShopOwnerDetails.LoginName = RegistrationDetails.phoneno;
                    ShopOwnerDetails.CreatedOn = EuropeDate;
                    ShopOwnerDetails.Updated_Date = EuropeDate;
                    ShopOwnerDetails.Country = RegistrationDetails.Country_hidden;
                    ShopOwnerDetails.City = RegistrationDetails.City_hidden;
                    ShopOwnerDetails.State = RegistrationDetails.State_hidden;
                    ShopOwnerDetails.SchId = ShopDetails.SchlId;
                    ShopOwnerDetails.ZSR_No = RegistrationDetails.ZsrNo;
                    SPA.CCTSP_User.Add(ShopOwnerDetails);
                    SPA.SaveChanges();
                    #endregion
                    #region EmployeeDetail
                    EmployeeDetails.FirstName = RegistrationDetails.FirstName;
                    EmployeeDetails.LastName = RegistrationDetails.FamilyName;
                    EmployeeDetails.PhoneNo = RegistrationDetails.phoneno;
                    EmployeeDetails.EmailId = RegistrationDetails.emailid;
                    EmployeeDetails.Gender = RegistrationDetails.Title;
                    EmployeeDetails.Pincode = Convert.ToInt32(RegistrationDetails.Zipcode.ToString());
                    EmployeeDetails.RoleId = 3;
                    EmployeeDetails.ActiveStatus = "A";
                    EmployeeDetails.Address1 = RegistrationDetails.Address;
                    if (RegistrationDetails.password != null)
                    {
                        EmployeeDetails.Password = RegistrationDetails.password;
                    }
                    else
                    {
                        EmployeeDetails.Password = "Emp@12345";
                    }
                    EmployeeDetails.LoginName = RegistrationDetails.phoneno;
                    EmployeeDetails.CreatedOn = EuropeDate;
                    EmployeeDetails.Updated_Date = EuropeDate;
                    EmployeeDetails.Country = RegistrationDetails.Country_hidden;
                    EmployeeDetails.City = RegistrationDetails.City_hidden;
                    EmployeeDetails.State = RegistrationDetails.State_hidden;
                    EmployeeDetails.SchId = ShopDetails.SchlId;
                    EmployeeDetails.ZSR_No = RegistrationDetails.ZsrNo;
                    SPA.CCTSP_User.Add(EmployeeDetails);
                    SPA.SaveChanges();
                    #endregion
                    #region Add paymentGateway
                    // Add PaymentGateway
                    spa_Payment_Gateway paygateway = new spa_Payment_Gateway();
                    if (PayId > 0)
                    {
                        paygateway = SPA.spa_Payment_Gateway.Where(c => c.ActiveStatus == "A" && c.PaymentGatewayId == PayId).FirstOrDefault();
                        paygateway.Schlid = ShopDetails.SchlId;
                        paygateway.UserId = ShopOwnerDetails.UserId;
                        paygateway.TableName = "spa_BillingMaster";
                        paygateway.productinfo = Convert.ToString(BillMaster.Receipt_Id);
                        paygateway.PriceList = string.Join(",", BillDetails.Select(c => c.receivedAmount).ToList());
                        paygateway.PaymentReason = "Shop Setup Fees";
                        paygateway.Updated_Date = CurrentDate;
                        SPA.SaveChanges();
                    }
                    #endregion
                    //Email
                    PushEmail Email = new PushEmail();
                    Email.SendEmail(RegistrationDetails, Convert.ToInt32(ShopDetails.SchlId), LangId.Value);
                }
            }
        }
        public Models.RemainingShopCredit RemainingCredit(long shId, DateTime CurrentDate)
        {
            string status = "select RemainingDays = " +
                             " case when y.DurationStatus = 'N' then - 1 * (Round(y.RemainingCredit / (sum(x.PerDayCredit) / count(*)), 0)) " +
                              "else " +
                              "case when y.RemainingCredit != 0 " +
                              "then - 1 * (Round(y.RemainingCredit / (sum(x.PerDayCredit) / count(*)), 0)) + Datediff(day, '" + CurrentDate.ToString("yyyy-MM-dd HH:mm") + "', DATEADD(month, convert(int, y.Duration), y.startdate)) " +
                              "else Datediff(day, '" + CurrentDate.ToString("yyyy-MM-dd HH:mm") + "', DATEADD(month, convert(int, y.Duration), y.startdate)) " +
                               "end " +
                               "end, " +
                               "TotalReservation = " +
                               "case when y.DurationStatus = 'N' " +
                               "then Round(CAST(sum(x.ResCount) AS float) / CAST(count(*) AS float) * -1 * (Round(y.RemainingCredit / (sum(x.PerDayCredit) / count(*)), 0)), 0) " +
                               "else" +
                               " case when y.RemainingCredit != 0 " +
                               " then Round(CAST(sum(x.ResCount) AS float) / CAST(count(*) AS float) * -1 * (Round(y.RemainingCredit / (sum(x.PerDayCredit) / count(*)), 0)), 0) +Round(cast(Datediff(day, '" + CurrentDate.ToString("yyyy-MM-dd HH:mm") + "', DATEADD(month, convert(int, y.Duration), y.startdate)) as float) * (CAST(sum(x.ResCount) AS float) / CAST(count(*) AS float)), 0) " +
                               "else " +
                               " Round(cast(Datediff(day, '" + CurrentDate.ToString("yyyy-MM-dd HH:mm") + "', DATEADD(month, convert(int, y.Duration), y.startdate)) as float) * (CAST(sum(x.ResCount) AS float) / CAST(count(*) AS float)), 0) " +
                               " end " +
                               " end " +
                               " from " +
                                "(select convert(date, created_on) as dates, sum(Credit) as PerDayCredit, count(ReservationId) as ResCount from spa_BillingDetails where ActiveStatus = 'A' and ReservationId is not null and ShopId = " + shId + " group by convert(date, created_on)) as x,spa_billingMaster y where y.ActiveStatus = 'A' and y.ShopId = " + shId + " group by y.RemainingCredit, y.DurationStatus,y.Duration,y.startdate";
            return SPA.Database.SqlQuery<Models.RemainingShopCredit>(status).FirstOrDefault();
        }
        public List<Models.PaySection> getPaymentDetails(string getall, long shopId)
        {
            var payquery = "select a.PaymentDetail_Id,a.PaymentCatgId,b.PaymentCatgName,d.SchCountry,a.quantity_or_price as Amount,a.Percharge as specificshop,a.discount as Duration,a.Calculation as PaymentType,a.country_id,b.Field3 as CatPayment,d.Currency,d.schlId" +
                " from spa_Payment_plan a " +
                "join spa_paymentCategory b on a.PaymentCatgId = b.PaymentCatgId and Field2 = '1' " +
                "join spatime_zone c on c.timezone_id = a.country_id " +
                "join cctsp_schoolMaster d on d.SchlId = " + shopId + " and d.timezone_id = c.timezone_id " +
                "where a.ActiveStatus = 'A' and b.ActiveStatus = 'A' and c.Activestatus = 'A' and d.ActiveStatus = 'A' and b.Where_To_display in (2,3) ";
            if (getall != null)
                payquery = payquery + " and a.paymentDetail_Id in (" + getall + ")";
            var getpaylist = SPA.Database.SqlQuery<Models.PaySection>(payquery).ToList();
            return getpaylist;
        }
        public List<Models.PaySection> getClickPaymentDetails(string getall, int countryid)
        {
            var payquery = "select a.PaymentDetail_Id,a.PaymentCatgId,b.PaymentCatgName,a.quantity_or_price as Amount,a.Percharge as specificshop,a.discount as Duration,a.Calculation as PaymentType,a.country_id,b.Field3 as CatPayment,c.Currency" +
                " from spa_Payment_plan a " +
                "join spa_paymentCategory b on a.PaymentCatgId = b.PaymentCatgId and Field2 = '1' " +
                "join spatime_zone c on c.timezone_id = a.country_id " +
                //"join cctsp_schoolMaster d on d.SchlId = " + shopId + " and d.timezone_id = c.timezone_id " +
                "where a.ActiveStatus = 'A' and b.ActiveStatus = 'A' and c.Activestatus = 'A' and a.Country_id=" + countryid + " and b.Where_To_display in (1,3) ";
            if (getall != null)
                payquery = payquery + " and a.paymentDetail_Id in (" + getall + ")";
            var getpaylist = SPA.Database.SqlQuery<Models.PaySection>(payquery).ToList();
            return getpaylist;
        }
        public async Task setActivityLogAsync(string ActionName, string ControllerName, long schId, string Message, object obj)
        {
            Models.ActivityLog ALog = new Models.ActivityLog();
            ALog.Action = ActionName;
            ALog.controller = ControllerName;
            ALog.schId = schId;
            ALog.Message = Message;
            if (obj != null)
                ALog.Activity = obj;
            await Task.Run(() => setActivityLog(ALog));
        }
        public List<CCTSP_CategoryDetails> getCatgDetails(string CatgId)
        {
            List<long> CatgList = CatgId.Split(',').Select(c => long.Parse(c)).ToList();
            return (from c in SPA.CCTSP_CategoryDetails where c.ActiveStatus == "A" && CatgList.Contains(c.CatgId) && c.DomainType == schlId select c).ToList();
        }
        public List<Models.CategoryDetails> getNoteCatgDetails(List<long> CatgList)
        {
            return (from c in SPA.CCTSP_CategoryDetails where c.ActiveStatus == "A" && CatgList.Contains(c.CatgId) && c.DomainType == schlId select c).Select(c => new Models.CategoryDetails
            {
                CatgId = c.CatgId,
                CatgDesc = c.CatgDesc,
                CatgTypeId = c.CatgTypeId,
                Group_orderdata = c.Group_orderdata,
                CatgName = c.CCTSP_CategoryMaster.CatgName,
                Lang_Id = c.Lang_Id
            }).ToList();
        }
        public CCTSP_CategoryDetails AddCategoriesDetail(string catgId, string catgName)
        {
            CCTSP_CategoryDetails CatgDetails = new CCTSP_CategoryDetails();
            try
            {
                CatgDetails = new CCTSP_CategoryDetails();
                CatgDetails.CatgId = long.Parse(catgId);
                CatgDetails.CatgType = catgId;
                CatgDetails.CatgDesc = catgName;
                CatgDetails.ActiveStatus = "A";
                CatgDetails.DomainType = schlId;
                CatgDetails.CreatedOn = ZonalDate(null);
                SPA.CCTSP_CategoryDetails.Add(CatgDetails);
                SPA.SaveChanges();
                return CatgDetails;
            }
            catch (Exception e)
            {
                //ErrorSend(RouteData, e);
                return CatgDetails;
            }
        }
        public List<Models.PaySection> getPaymentDetail(string getall, long shopId)
        {
            var payquery = "select a.PaymentDetail_Id,a.PaymentCatgId,b.PaymentCatgName,d.SchCountry,a.quantity_or_price as Amount,a.Percharge as specificshop,a.discount as Duration,a.Calculation as PaymentType,a.country_id,b.Field3 as CatPayment,d.Currency,d.schlId" +
                " from spa_Payment_plan a " +
                "join spa_paymentCategory b on a.PaymentCatgId = b.PaymentCatgId and Field2 = '1' " +
                "join spatime_zone c on c.timezone_id = a.country_id " +
                "join cctsp_schoolMaster d on d.SchlId = " + shopId + " and d.timezone_id = c.timezone_id " +
                "where a.ActiveStatus = 'A' and b.ActiveStatus = 'A' and c.Activestatus = 'A' and d.ActiveStatus = 'A' ";
            if (getall != null)
                payquery = payquery + " and a.paymentDetail_Id in (" + getall + ")";
            var getpaylist = SPA.Database.SqlQuery<Models.PaySection>(payquery).ToList();
            return getpaylist;
        }
        public List<Language_Label_Detail> getLanguageData(string pageName, int? order_id, long LangId)
        {
            if (order_id == null) order_id = 0;
            var language = SPA.Language_Label_Detail.Where(c => c.Page_Name == pageName && c.ActiveStatus == "A" && c.Lang_id == LangId && c.Order_id >= order_id).ToList();
            if (language.Count == 0)
                language = new List<Language_Label_Detail>();
            return language;
        }
        public List<Language_Label_Detail> getstartandEndLangList(string pageName, int? strtorder_id, int? endorder, int LangId)
        {
            var language = new List<Language_Label_Detail>();
            var languageTemp = SPA.Language_Label_Detail.Where(c => c.Page_Name == pageName && c.ActiveStatus == "A" && c.Lang_id == LangId);
            if (strtorder_id != null)
                languageTemp = languageTemp.Where(c => c.Order_id >= strtorder_id);
            if (endorder != null)
                languageTemp = languageTemp.Where(c => c.Order_id <= endorder);
            language = languageTemp.ToList();
            if (language.Count == 0)
                language = new List<Language_Label_Detail>();
            return language;
        }
        public List<Models.LanguageLabelDetails> DynamicFieldsAccordingFlow(int LangId, long FlowId, string PageName)
        {
            return SPA.Database.SqlQuery<Models.LanguageLabelDetails>(sql.DynammicFormFieldAccordingFlow(LangId, FlowId, PageName)).ToList();
        }
        public List<Models.DoctorPagesDisplay> CommonPreDocDisplay(long? Reservationid, int diff, int Flag)
        {
            var Remarks = new List<Models.DoctorPagesDisplay>();
            string PrescriptionNote = "select ISNULL(g.CatgTypeId, 0 ) as CatgTypeId ,d.ImageLogo as ShopImg,g.CatgDesc, ISNULL(e.CatgId, 0 )as CatgId,b.FirstName,b.LastName,b.Gender,b.DOB,c.FirstName as EmpFirstName,c.LastName as EmpLastName ,f.created_on ,d.SchlAddress as Address, a.comment, CASE WHEN  datediff(year, b.DOB, getdate()) < 180 THEN datediff(year, b.DOB, getdate()) ELSE 0 END as Age,a.ActiveStatus from spa_employeeScheduler a join cctsp_user b on b.UserId = a.Client_UserId join cctsp_User c on c.UserId = a.Emp_UserId join cctsp_SchoolMaster d on d.SchlId = a.SchId left join Prescription_Detail e on (e.BookingId = a.EmpSchDetailsId and e.ActiveStatus = 'A' and e.UserId = b.UserId and e.schId = a.SchId and e.CatgTypeid is not null) left join Prescription_Master f on(f.Prescription_Id = e.Prescription_Id and f.ActiveStatus = 'A' and f.SchId = a.SchId) left join cctsp_categoryDetails g on(g.CatgTypeId = e.CatgTypeId and g.DomainType = a.SchId) where a.SchId = " + schlId + " and a.EmpSchDetailsId = " + Reservationid + " and((a.ActiveStatus = 'A' and a.BookedStatus = 'B') or(a.ActiveStatus = 'C' and a.BookedStatus = 'C')) and b.RoleId = 4 ";
            var PrescriptionDisplay = SPA.Database.SqlQuery<Models.DoctorPagesDisplay>(PrescriptionNote).ToList();
            string QueryAuto = "select b.CatgDesc as Medicine,c.CatgDesc as HowManyTimes,d.CatgDesc as [When],a.Remarks as catgDesc,'A' as status,convert(bigint,field3) as CatgId" +
                               " from Medicine_Master a join cctsp_categoryDetails b on b.CatgTypeId = a.field1 join cctsp_categoryDetails c on c.CatgTypeId = a.NtimesDay join cctsp_categoryDetails d on d.CatgTypeId = a.Times join Prescription_Master e on e.Prescription_Id = a.Prescription_Id " +
                               " where a.Activestatus = 'A' and a.BookingId = " + Reservationid + " and e.diff = 3";
            PrescriptionDisplay.AddRange(SPA.Database.SqlQuery<Models.DoctorPagesDisplay>(QueryAuto).ToList());
            //Free Text
            if (Flag == 1)
                Remarks = (from c in SPA.Prescription_Detail where c.ActiveStatus == "A" && c.BookingId == Reservationid && (c.Type == diff) && c.Prescription_Master.Diff == diff && c.SchId == schlId select new Models.DoctorPagesDisplay { CatgId = c.CatgId.Value, CatgDesc = c.Remarks, Type = c.Type.Value, status = "F" }).ToList();
            else
                Remarks = (from c in SPA.Prescription_Detail where c.ActiveStatus == "A" && c.BookingId == Reservationid && (c.Type == diff) && c.Prescription_Master.Diff == diff && (c.CatgId == 139 || c.CatgId == 137) && c.SchId == schlId select new Models.DoctorPagesDisplay { CatgId = c.CatgId.Value, CatgDesc = c.Remarks, status = "F" }).ToList();

            PrescriptionDisplay.AddRange(Remarks);
            return PrescriptionDisplay;
        }
        public string ImageLogoChange(string LogoPath)
        {
            var Link = "http://" + HttpContext.Current.Request.Url.Host;
            if (string.IsNullOrEmpty(LogoPath))
                LogoPath = (Link + "/images/prescription-img.png").Replace("localhost", "tshope.azurewebsites.net");
            else
                LogoPath = (Link + LogoPath).Replace("localhost", "tshope.azurewebsites.net");
            return LogoPath;
        }
        public List<Models.SpecialInsertForDoctor> CommonAdvice(long Reservationid, int Diff, string catgid)
        {
            string CommonAdviceList = "select convert(int,ROW_NUMBER() OVER (ORDER BY c.catgTypeId)) AS SpId ,b.CatgDesc as MedicinecatgDesc,c.CatgDesc as HowManyTimescatgDesc,d.CatgDesc as WhencatgDesc ,a.Remarks as [FreeText], convert(nvarchar, b.CatgTypeId) as Medicine ,convert(nvarchar, c.CatgTypeId) as HowManyTimes,convert(nvarchar, d.CatgTypeId) as [When] from Medicine_Master a join cctsp_categoryDetails b on b.CatgTypeId = a.field1 join cctsp_categoryDetails c on c.CatgTypeId = a.NtimesDay join cctsp_categoryDetails d on d.CatgTypeId = a.Times join Prescription_Master e on e.Prescription_Id = a.Prescription_Id where a.Activestatus = 'A' and a.BookingId = " + Reservationid + "  and e.diff = " + Diff + " and a.field3 =" + catgid + " and b.Activestatus='A' and c.activestatus='A' and d.activestatus='A'";
            var CommonAdviceDisplay = SPA.Database.SqlQuery<Models.SpecialInsertForDoctor>(CommonAdviceList).ToList();
            return CommonAdviceDisplay;
        }
        public Models.DoctorPagesDisplay CommonPatientDetails(long Reservationid)
        {
            //string PatientDetails = "select a.EmpSchDetailsId ,a.comment,b.UserId,b.FirstName,b.LastName,b.DOB,b.EmailId,b.Gender,c.FirstName as EmpFirstName,C.LastName as EmpLastName ,d.SchlAddress as Address,d.ImageLogo as ShopImg ,CASE WHEN  datediff(year,b.DOB,getdate())< 180 THEN datediff(year,b.DOB,getdate()) ELSE 0 END as Age from SPA_EmployeeScheduler a join cctsp_user b on b.UserId = a.Client_UserId join cctsp_user c on c.UserId = a.Emp_UserId join cctsp_schoolmaster d on  d.schlid=a.schid where a.schid = " + schlId + " and a.ActiveStatus = 'A' and b.schid = " + schlId + " and c.schid = " + schlId + " and a.EmpSchDetailsId = " + Reservationid + " and d.ActiveStatus='A'";
            string PatientDetails = "select a.EmpSchDetailsId ,a.comment,b.UserId,b.FirstName,b.LastName,b.Title,b.DOB,b.EmailId,b.Gender,c.FirstName as EmpFirstName,C.LastName as EmpLastName ,d.SchlAddress as Address,d.SchlStudentStrength,d.ImageLogo as ShopImg ,CASE WHEN  datediff(year,b.DOB,getdate())< 180 THEN datediff(year,b.DOB,getdate()) ELSE 0 END as Age from SPA_EmployeeScheduler a join cctsp_user b on b.UserId = a.Client_UserId join cctsp_user c on c.UserId = a.Emp_UserId join cctsp_schoolmaster d on  d.schlid=a.schid where a.schid = " + schlId + "  and c.schid = " + schlId + " and a.EmpSchDetailsId = " + Reservationid + " and d.ActiveStatus='A'";
            var PatientDetailsDisplay = SPA.Database.SqlQuery<Models.DoctorPagesDisplay>(PatientDetails).FirstOrDefault();
            return PatientDetailsDisplay;
        }
        public List<Models.DoctorPagesDisplay> getNotesSectionWithData(long ReservationId)
        {
            return SPA.Database.SqlQuery<Models.DoctorPagesDisplay>(sql.getAllNotesData(ReservationId, schlId)).ToList();
        }
        public string currency(string SchCurrency)
        {
            string currency = "CHF";
            if (string.IsNullOrEmpty(SchCurrency))
                SchCurrency = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Currency).FirstOrDefault();
            if (!string.IsNullOrEmpty(SchCurrency))
                currency = SchCurrency;
            return currency;
        }
        public string CommonImageUpload(HttpPostedFileBase file, string[] supportedTypes)
        {
            string PathDetails = "";
            var postedFile = file;

            //string subPath = "Upload/";
            string subPath = "LandingUpload/" + schlId + "/";
            bool IsExists = Directory.Exists(Path.Combine(HttpRuntime.AppDomainAppPath, subPath, file.FileName));
            //bool IsExists = Directory.Exists(Server.MapPath(subPath));
            //var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
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
            if (!supportedTypes.Contains(fileExt))
            {
                PathDetails = "Only image";
            }
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
                    postedFile.SaveAs(Path.Combine(HttpRuntime.AppDomainAppPath, subPath, DateTime.Now.ToString("ddMMyyyyHHmmss") + file.FileName));
                    PathDetails = Path.Combine("/" + subPath + DateTime.Now.ToString("ddMMMyyyyHHmmss") + file.FileName);
                }
            }
            return PathDetails;
        }
        public Prescription_Detail AddFreePrescription(long CatgId, DateTime RegionalDate, Prescription_Master checkUserDetails, string Text)
        {
            Prescription_Detail DetailPre = new Prescription_Detail();
            DetailPre.ActiveStatus = "A";
            DetailPre.CatgId = CatgId;
            DetailPre.created_on = RegionalDate;
            DetailPre.UserId = checkUserDetails.UserId;
            DetailPre.Prescription_Id = checkUserDetails.Prescription_Id;
            DetailPre.BookingId = checkUserDetails.BookingId;
            DetailPre.Remarks = Text;
            DetailPre.Type = checkUserDetails.Diff;
            DetailPre.ActiveStatus = "A";
            DetailPre.SchId = schlId;
            return DetailPre;
        }
        public DateTime ZonalDate(string TimeZone)
        {
            if (string.IsNullOrEmpty(TimeZone))
            {
                TimeZone = "Central Europe Standard Time";
                var timezone = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.TimeZone).FirstOrDefault();
                if (timezone != null && timezone != "")
                    TimeZone = timezone;
            }
            TimeZoneInfo EuropeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            DateTime ZonalDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EuropeZone);
            return ZonalDate;
        }
        public string Colorclass(long? Color_Id)
        {
            string Colorclass = "bg-img";
            if (Color_Id != null)
                Colorclass = SPA.CCTSP_ColorMaster.Where(c => c.Color_Id == Color_Id).Select(c => c.Color_Class).FirstOrDefault();
            return Colorclass;
        }
        public string NullToString(string Value)
        {
            if (Value == null)
                return "";
            else
                return Value.ToString();
        }
        public void CheckEmployeeTimeExist(int userId)
        {
            var data = SPA.SPA_SchedulerSlotMaster.Where(c => c.UserId == userId && c.ActiveStatus == "A");
            if (data.Count() != 0)
            {
                var query = "update SPA_SchedulerSlotMaster set ActiveStatus='D' where ActiveStatus='A' and UserId=" + userId;
                SPA.Database.ExecuteSqlCommand(query);
            }
        }
        public string check(int? typeId, int? userId)
        {
            string Msg = "";
            var dtg = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == typeId && c.Activestatus == "A" && c.SchId == schlId).FirstOrDefault();
            var data = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == 11145 && c.UserId == userId && c.SchId == schlId && c.Activestatus == "A" && c.SectionDesc == dtg.CCTSP_CategoryDetails.CatgDesc && c.Amount == dtg.Amount && c.Duration == dtg.Duration).FirstOrDefault();
            if (data == null)
                Msg = "No";
            else
                Msg = "Yes";

            return Msg;
        }
        public string updateTextRange(List<long> TxtsolutionList)
        {
            string Txt = "";
            int cnt = 0;
            foreach (var item in TxtsolutionList)
            {
                if (cnt == 0)
                {
                    Txt = Txt + item;
                    cnt = cnt + 1;
                }
                else
                    Txt = Txt + "," + item;
            }
            var query = "update CTSP_SolutionMaster set [Group] = 'D' where SolutionId in(" + Txt + ")";
            SPA.Database.ExecuteSqlCommand(query);
            return Txt;

        }
        public void BookedDataUpdate(string FromSlot, string ToSlot, long? schedulerId, long? Emp_UserId, long? Client_UserId, long? CatgTypeId, string BookedDate)
        {
            var query = "update SPA_EmployeeScheduler set BookedStatus='BT',BookingDate='" + BookedDate + "',FromSlotMasterId='" + FromSlot + "', ToSlotMasterId='" + ToSlot + "' , SchedulerId='" + schedulerId + "' , EMP_UserId='" + Emp_UserId + "' where CLIENT_UserId=" + Client_UserId + " and Product_Id=" + CatgTypeId + " and SchId=" + schlId + " and BookedStatus='T'";
            SPA.Database.ExecuteSqlCommand(query);
        }
        //public void SendMultipleEmail(string IdList)
        //{
        //    var BookingIdList = IdList.Split(' ').Select(i => int.Parse(i)).ToList();
        //    var BookingInfoList = (from d in SPA.SPA_EmployeeScheduler where BookingIdList.Contains(d.EmpSchDetailsId) && d.SchId == schlId select d);
        //    var ClientId = BookingInfoList.Select(c => c.CLIENT_UserId).ToList();
        //    var ClientInfoList = (from e in SPA.CCTSP_User where ClientId.Contains(e.UserId) select e).ToList();
        //    foreach (var item in BookingInfoList)
        //    {
        //        var ClientInformation = ClientInfoList.Where(d => d.UserId == item.CLIENT_UserId).FirstOrDefault();
        //        if (ClientInformation.Email_Service == 3 || ClientInformation.Email_Service == 2)
        //        {
        //            var name = ClientInformation.Gender + " " + ClientInformation.FirstName + " " + ClientInformation.LastName;
        //            EmailSend.Approvebooking(name, item.BookingDate, ClientInformation.EmailId, item.FromSlotMasterId, item.EmpSchDetailsId);
        //        }
        //    }
        //}
        public void SendMultipleEmail(string IdList)
        {
            EmailSend.MultipleEmail(IdList);
        }


        //public void SendNewsletter(List<string> EmailId, int schlId, string Content, List<string> Attachlinks, List<string> FileNames, string Subject)
        //{
        //    try
        //    {
        //        XmlDocument xmldocument = null;
        //        XmlNode subjectNode = null;
        //        XmlNode bodyNode = null;
        //        string body = "";
        //        string subjectTemp = "";
        //        var LogoImage = "http://tshope.azurewebsites.net/images/Maarkss.png";
        //        xmldocument = new XmlDocument();
        //        xmldocument.Load(HostingEnvironment.MapPath("~/MailMessages/messages.xml"));
        //        subjectNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='REGISTRATION_PDF_EMAIL']/PASSWORD/SUBJECT");
        //        bodyNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='REGISTRATION_PDF_EMAIL']/PASSWORD/BODY");
        //        body = bodyNode.InnerText;
        //        subjectTemp = subjectNode.InnerText;

        //        if (body.IndexOf("@DATA", StringComparison.OrdinalIgnoreCase) > 0)
        //            body = Regex.Replace(body, "@DATA", Content, RegexOptions.IgnoreCase);

        //        if (body.IndexOf("@Image", StringComparison.OrdinalIgnoreCase) > 0)
        //            body = Regex.Replace(body, "@Image", LogoImage, RegexOptions.IgnoreCase);
        //        var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/MarketingMail");
        //        // var request = (HttpWebRequest)WebRequest.Create("http://localhost:8635/Test/MarketingMail");
        //        var postData = "EmailTo=" + js.Serialize(EmailId);
        //        postData = postData + "&subject=" + HttpUtility.UrlEncode(Subject);
        //        postData = postData + "&emailCC=" + "";
        //        postData = postData + "&projectName=" + "click-and-go.com";
        //        postData = postData + "&Content=" + HttpUtility.UrlEncode(body);
        //        postData = postData + "&AttachLink=" + js.Serialize(Attachlinks);
        //        postData = postData + "&FileNames=" + js.Serialize(FileNames);
        //        //postData = postData + "&schId=" + schlId;
        //        var data = Encoding.ASCII.GetBytes(postData);
        //        request.Method = "POST";
        //        request.ContentType = "application/x-www-form-urlencoded";
        //        request.ContentLength = data.Length;
        //        using (var stream = request.GetRequestStream())
        //        {
        //            stream.Write(data, 0, data.Length);
        //        }
        //        var LocalResponseEmail = (HttpWebResponse)request.GetResponse();
        //        var ResultEmail = new StreamReader(LocalResponseEmail.GetResponseStream()).ReadToEnd();
        //    }
        //    catch
        //    {
        //    }
        //}
        public List<CCTSP_CategoryDetails> PrintSetup()
        {
            var SetupDeatil = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 144 && c.ActiveStatus == "A" && c.DomainType == schlId).ToList();
            return new List<CCTSP_CategoryDetails>(SetupDeatil);
        }
        public string ChangeLangGender(int Lang, string Gen)
        {
            if (Gen == "Mr" || Gen == "Herr" || Gen == "Monsieur")
                return "Male";
            else
                return "female";
        }
        public string CheckReservationExistOrNot(string startDate, string EndDate, long Emp_UserId)
        {
            //var qryChk = "select count(EmpSchDetailsId) from spa_employeeScheduler where schId = " + schlId + "  and((cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) < '" + startDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + startDate + "') or(cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) < '" + EndDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + EndDate + "')) and((ActiveStatus = 'DA' and BookedStatus = 'B') or(ActiveStatus = 'A' and BookedStatus = 'B') or(ActiveStatus = 'C' and BookedStatus = 'C')) and Emp_UserId = " + Emp_UserId;
            var qryChk = " select count(EmpSchDetailsId) from SPA_EmployeeScheduler where cast(BookingDate as date) = cast('" + startDate + "' as date) and ((cast(concat(BookingDate, ' ', FromSlotMasterId) as datetime) >= cast('" + startDate + "' as datetime) and  cast(concat(BookingDate, ' ', FromSlotMasterId) as datetime) < cast('" + EndDate + "' as datetime)) or(cast(concat(BookingDate, ' ', ToSlotMasterId) as datetime) > cast('" + startDate + "' as datetime) and  cast(concat(BookingDate, ' ', ToSlotMasterId) as datetime) <= cast('" + EndDate + "' as datetime))) and((ActiveStatus = 'DA' and BookedStatus = 'B') or(ActiveStatus = 'A' and BookedStatus = 'B') or(ActiveStatus = 'C' and BookedStatus = 'C')) and EMP_UserId = " + Emp_UserId;
            int checkbooking = SPA.Database.SqlQuery<int>(qryChk).FirstOrDefault();
            if (checkbooking == 0)
                return "No";
            else
                return "Yes";

        }
        public string CheckUpdateReservationExistOrNot(string startDate, string EndDate, long Emp_UserId, long ReservationId)
        {
            //var qryChk = "select count(EmpSchDetailsId) from spa_employeeScheduler where schId = " + schlId + "  and((cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) < '" + startDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + startDate + "') or(cast(concat(BookingDate, ' ', fromslotmasterid) as datetime) < '" + EndDate + "' and cast(concat(BookingDate, ' ', toslotmasterId) as datetime) > '" + EndDate + "')) and((ActiveStatus = 'DA' and BookedStatus = 'B') or(ActiveStatus = 'A' and BookedStatus = 'B') or(ActiveStatus = 'C' and BookedStatus = 'C')) and Emp_UserId = " + Emp_UserId;
            var qryChk = " select count(EmpSchDetailsId) from SPA_EmployeeScheduler where cast(BookingDate as date) = cast('" + startDate + "' as date) and ((cast(concat(BookingDate, ' ', FromSlotMasterId) as datetime) >= cast('" + startDate + "' as datetime) and  cast(concat(BookingDate, ' ', FromSlotMasterId) as datetime) < cast('" + EndDate + "' as datetime)) or(cast(concat(BookingDate, ' ', ToSlotMasterId) as datetime) > cast('" + startDate + "' as datetime) and  cast(concat(BookingDate, ' ', ToSlotMasterId) as datetime) <= cast('" + EndDate + "' as datetime))) and((ActiveStatus = 'DA' and BookedStatus = 'B') or(ActiveStatus = 'A' and BookedStatus = 'B') or(ActiveStatus = 'C' and BookedStatus = 'C')) and EMP_UserId = " + Emp_UserId + " and EmpSchDetailsId not in (" + ReservationId + ")";
            int checkbooking = SPA.Database.SqlQuery<int>(qryChk).FirstOrDefault();
            if (checkbooking == 0)
                return "No";
            else
                return "Yes";

        }
        public void AddMasterData(Models.ConfirmModel ReservationInfo, string ActiveStatus, string BookedStatus, string RegStatus)
        {
            if (ReservationInfo != null)
            {
                DateTime CurrentDate = ZonalDate(null);
                spa_Master_Reservation Master = new spa_Master_Reservation();
                Master.BookingDate = ReservationInfo.BookingDate;
                Master.ActiveStatus = ActiveStatus;
                Master.BookedStatus = BookedStatus;
                Master.CustomerId = ReservationInfo.CLIENT_UserId;
                Master.EmpId = ReservationInfo.EMP_UserId;
                Master.StartTime = ReservationInfo.FromSlotMasterId;
                Master.EndTime = ReservationInfo.ToSlotMasterId;
                Master.TotalAmount = Convert.ToDouble(ReservationInfo.Product_price);
                Master.created_date = CurrentDate;
                Master.Updated_date = CurrentDate;
                Master.Schlid = schlId;
                Master.ProductId_list = Convert.ToString(ReservationInfo.ProductId);
                Master.TotalTime = Convert.ToDouble(ReservationInfo.Duration);
                Master.Reg_Status = RegStatus;
                SPA.spa_Master_Reservation.Add(Master);
                SPA.SaveChanges();
                var query = "update SPA_EmployeeScheduler set MasterResId=" + Master.MasterResId + " where EmpSchDetailsId=" + ReservationInfo.EmpSchDetailsId + " and SchId=" + schlId;
                SPA.Database.ExecuteSqlCommand(query);
            }
        }
        public void PaymentDeduction(string price, long ReservationId, CCTSP_SchoolMaster ShopInfo, string ActiveStatus)
        {
            //Payment Deduction
            #region PaymentDeduction
            double Amount = 0;
            // var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
            if (!string.IsNullOrEmpty(price))
                Amount = Convert.ToDouble(price);
            DateTime CurrentDate = ZonalDate(ShopInfo.TimeZone);
            var CreditCheck = SPA.spa_BillingMaster.Where(c => c.ActiveStatus == "A" && c.ShopId == schlId).FirstOrDefault();
            var PaymentDetails = SPA.spa_PaymentDeduction.Where(c => c.ActiveStatus == "A" && c.spatime_zone.name_country == ShopInfo.Schcountry && c.spa_paymentCategory.Field1 == "5").FirstOrDefault();
            double Percentage = 0;
            if (PaymentDetails != null)
                Percentage = Convert.ToDouble(PaymentDetails.DeductionAmount);
            double DeductionAmount = 0;
            var DeductAmount = "YES";
            var DeductStatus = "NO";
            if (CreditCheck != null)
            {
                if (CreditCheck.DurationStatus == "Y" && CurrentDate <= CreditCheck.StartDate.Value.AddMonths(Convert.ToInt32(CreditCheck.Duration)))
                {
                    DeductAmount = "NO";
                    DeductStatus = "YES";
                }
                else
                {
                    if (PaymentDetails.Type == "2")
                        DeductionAmount = (Amount * Convert.ToDouble(Percentage)) / 100;
                    else
                        DeductionAmount = Percentage;
                    if (DeductionAmount <= CreditCheck.RemainingCredit)
                        DeductStatus = "YES";
                }
            }
            if (DeductStatus == "YES" && PaymentDetails != null && ActiveStatus == "A")
            {
                List<spa_BillingDetails> BillingList = new List<spa_BillingDetails>();
                //foreach (var item in AddReservationList)
                //{
                spa_BillingDetails Billing = new spa_BillingDetails();
                Billing.ActiveStatus = "A";
                if (Percentage != 0 && DeductAmount == "YES")
                    Billing.Credit = -(DeductionAmount);
                else
                    Billing.Credit = 0;
                Billing.RechargeId = PaymentDetails.PaymentCatgId.Value;
                Billing.ReceiptId = CreditCheck.Receipt_Id;
                Billing.ShopId = schlId;
                Billing.ReservationId = Convert.ToInt32(ReservationId);
                Billing.Amount = 0;
                Billing.created_on = CurrentDate;
                Billing.Updated_on = CurrentDate;
                BillingList.Add(Billing);
                //}
                SPA.spa_BillingDetails.AddRange(BillingList);
                if (DeductAmount == "YES")
                    CreditCheck.RemainingCredit = CreditCheck.RemainingCredit - DeductionAmount;
                SPA.SaveChanges();
            }
            #endregion
        }
        public void MultiplePaymentDeduction(List<SPA.SPA_EmployeeScheduler> AddReservationList, CCTSP_SchoolMaster ShopInfo, string ActiveStatus)
        {

            //Payment Deduction
            #region PaymentDeduction
            double Amount = 0;
            // var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
            if (AddReservationList.Count > 0)
                Amount = AddReservationList.Select(c => Convert.ToDouble(c.Product_price)).ToList().Sum();
            DateTime CurrentDate = ZonalDate(ShopInfo.TimeZone);
            var CreditCheck = SPA.spa_BillingMaster.Where(c => c.ActiveStatus == "A" && c.ShopId == schlId).FirstOrDefault();
            var PaymentDetails = SPA.spa_PaymentDeduction.Where(c => c.ActiveStatus == "A" && c.spatime_zone.name_country == ShopInfo.Schcountry && c.spa_paymentCategory.Field1 == "5").FirstOrDefault();
            double Percentage = 0;
            if (PaymentDetails != null)
                Percentage = Convert.ToDouble(PaymentDetails.DeductionAmount);
            double DeductionAmount = 0;
            var DeductAmount = "YES";
            var DeductStatus = "NO";
            if (CreditCheck != null)
            {
                if (CreditCheck.DurationStatus == "Y" && CurrentDate <= CreditCheck.StartDate.Value.AddMonths(Convert.ToInt32(CreditCheck.Duration)))
                {
                    DeductAmount = "NO";
                    DeductStatus = "YES";
                }
                else
                {
                    if (PaymentDetails.Type == "2")
                        DeductionAmount = (Amount * Convert.ToDouble(Percentage)) / 100;
                    else
                        DeductionAmount = Percentage * AddReservationList.Count;
                    if (DeductionAmount <= CreditCheck.RemainingCredit)
                        DeductStatus = "YES";
                }
            }
            if (DeductStatus == "YES" && PaymentDetails != null && ActiveStatus == "A")
            {
                List<spa_BillingDetails> BillingList = new List<spa_BillingDetails>();
                foreach (var item in AddReservationList)
                {
                    spa_BillingDetails Billing = new spa_BillingDetails();
                    Billing.ActiveStatus = "A";
                    if (Percentage != 0 && DeductAmount == "YES")
                        if (PaymentDetails.Type == "2")
                            Billing.Credit = -((Convert.ToDouble(item.Product_price) * Convert.ToDouble(Percentage)) / 100);
                        else
                            Billing.Credit = -(Percentage);
                    else
                        Billing.Credit = 0;
                    Billing.RechargeId = PaymentDetails.PaymentCatgId.Value;
                    Billing.ReceiptId = CreditCheck.Receipt_Id;
                    Billing.ShopId = schlId;
                    Billing.ReservationId = item.EmpSchDetailsId;
                    Billing.Amount = 0;
                    Billing.created_on = CurrentDate;
                    Billing.Updated_on = CurrentDate;
                    BillingList.Add(Billing);
                }
                SPA.spa_BillingDetails.AddRange(BillingList);
                if (DeductAmount == "YES")
                    CreditCheck.RemainingCredit = CreditCheck.RemainingCredit - DeductionAmount;
                SPA.SaveChanges();
            }
            #endregion


        }
        public void RefundPayment(int ReservationId)
        {
            var info = SPA.spa_BillingDetails.Where(c => c.ReservationId == ReservationId && c.ShopId == schlId).FirstOrDefault();
            if (info != null)
            {
                DateTime CurrentDate = ZonalDate(null);
                SPA.spa_BillingDetails AddDeduction = new spa_BillingDetails();
                AddDeduction.ReceiptId = info.ReceiptId;
                AddDeduction.RechargeId = info.RechargeId;
                AddDeduction.ShopId = schlId;
                AddDeduction.Amount = 0;
                AddDeduction.ReservationId = ReservationId;
                AddDeduction.ActiveStatus = "A";
                AddDeduction.Credit = -(info.Credit);
                AddDeduction.created_on = CurrentDate;
                AddDeduction.Updated_on = CurrentDate;
                SPA.spa_BillingDetails.Add(AddDeduction);
                var CreditCheck = SPA.spa_BillingMaster.Where(c => c.ActiveStatus == "A" && c.ShopId == schlId).FirstOrDefault();
                if (CreditCheck.DurationStatus != "Y")
                    CreditCheck.RemainingCredit = CreditCheck.RemainingCredit + (-(info.Credit.Value));
                SPA.SaveChanges();
            }
        }
        public List<Models.BillingHistory> BookingHistory(int? Years, int? Months, bool status)
        {
            var bookingList = " Select a.Credit,a.Amount,c.CatgDesc as ProductName , a.ReservationId,d.FirstName,d.LastName,a.created_on,e.PaymentCatgName,0 as [status] " +
                                " from spa_BillingDetails a " +
                                " left join SPA_EmployeeScheduler b on b.EmpSchDetailsId = a.ReservationId " +
                                " left join CCTSP_CategoryDetails c on c.CatgTypeId = b.Product_Id " +
                                " left join cctsp_User d on d.UserId = b.Client_UserId " +
                                " join spa_paymentCategory e on e.PaymentCatgId = a.RechargeId " +
                                " where a.ShopId = " + schlId;
            if (Years != null && Years != 0 && Months != 0 && Months != null)
            {
                bookingList += " and Format(Updated_On, 'MM')=" + Months + " and Format(Updated_On, 'yyyy')=" + Years;
                if (status)
                {
                    bookingList += " union" +
                                " SELECT" +
                                " sum(a.Credit) as credit, sum(a.Amount) as Amount, null as ProductName," +
                                " null as ReservationId, null as FirstName, null as LastName,convert(date,'" + Years + "-" + Months + "-1') as created_on," +
                                " 'Past Record' as PaymentCatgName,1 as [status]" +
                                " FROM spa_BillingDetails a" +
                                " LEFT JOIN SPA_EmployeeScheduler b on b.EmpSchDetailsId = a.ReservationId" +
                                " LEFT JOIN CCTSP_CategoryDetails c on c.CatgTypeId = b.Product_Id" +
                                " LEFT JOIN cctsp_User d on d.UserId = b.Client_UserId" +
                                " JOIN spa_paymentCategory e on e.PaymentCatgId = a.RechargeId" +
                                " WHERE a.ShopId = " + schlId + " and convert(bigint, Format(Updated_On, 'MM')) <" + Months +
                                " and convert(bigint, Format(Updated_On, 'yyyy')) <= " + Years;
                }
            }


            return SPA.Database.SqlQuery<Models.BillingHistory>(bookingList).OrderBy(c => c.created_on).ToList();
        }
        public Models.quickBlockSlot QuickBlockQuery(string WeekDay, long UserId, long Schlid, string StartDate)
        {
            TimeZoneInfo EuropeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime EuropeDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EuropeZone);
            var BlockQuery = "select top 1min(a.starttime) as StartTime,max(a.startTime) as EndTime,a.SlotDue " +
                             "from SPA_SchedulerSlotMaster a " +
                             "join CCTSP_schedulerMaster b on b.SchedulerId = a.SchedulerId and b.WeekDay = '" + WeekDay + "' " +
                             "join cctsp_schoolMaster c on c.SchlId = a.SchId and c.ActiveStatus = 'A' " +
                             "join spatime_zone d on d.timeZone_Id = c.timeZone_Id and d.ActiveStatus = 'A' " +
                             "where a.UserId = " + UserId + " and a.ActiveStatus = 'A' and a.Schid =" + schlId + " " +
                             "and convert(datetime, concat('" + StartDate + "', ' ', CONVERT(VARCHAR(5), a.startTime, 108)))> " +
                             "CONVERT(datetime, DBO.GetGeographicalTime('(GMT +05:30) Kolkata', d.name_timezone, '" + EuropeDate.ToString("yyyy/MM/dd HH:mm") + "'), 108) " +
                             "and a.StartTime is not null group by a.SlotDue ";
            return SPA.Database.SqlQuery<Models.quickBlockSlot>(BlockQuery).FirstOrDefault();
        }
        public JObject Timeblocking(long? UserId, string Date, int ReservationId)
        {
            JObject send = null;
            string Blockdate = "";
            string Jsondata = "";
            List<string> MsgList = new List<string>();
            DateTime CurrentDate = ZonalDate(null);
            TimeSpan CurrentStart = DateTime.Parse(CurrentDate.ToString()).TimeOfDay;
            if (ReservationId > 0)
                Blockdate = "Select distinct(format(convert(datetime,b.StartTime),'HH:mm')) from SPA_EmployeeScheduler a join SPA_SchedulerSlotMaster b on a.Emp_UserId = b.UserId join CCTSP_SchedulerMaster c on c.SchedulerId = b.SchedulerId where a.Emp_UserId = " + UserId + " and a.EmpSchDetailsId !=" + ReservationId + " and convert(date, a.BookingDate)= convert(date, '" + DateTime.Parse(Date).ToString("yyyy/MM/dd") + "') and a.SchId = " + schlId + " and((a.ActiveStatus = 'DA' and a.BookedStatus = 'B') or(a.ActiveStatus = 'A' and a.BookedStatus = 'B') or(a.ActiveStatus = 'C' and a.BookedStatus = 'C'))  and c.ActiveStatus = 'A' and c.SchId = " + schlId + " and b.SchId = " + schlId + " and b.ActiveStatus = 'A' and LOWER(c.weekDay) = LOWER(datename(dw, convert(date, a.BookingDate))) and ((convert(time, b.StartTime) >= convert(time, a.FromSlotMasterId) and convert(time, b.StartTime) < convert(time, a.ToSlotMasterId)) or convert(datetime, a.bookingDate) + convert(datetime, b.StartTime) <= convert(datetime, '" + CurrentDate.ToString("yyyy/MM/dd HH:mm") + "'))";
            else
                Blockdate = "Select distinct(format(convert(datetime,b.StartTime),'HH:mm')) from SPA_EmployeeScheduler a join SPA_SchedulerSlotMaster b on a.Emp_UserId = b.UserId join CCTSP_SchedulerMaster c on c.SchedulerId = b.SchedulerId where a.Emp_UserId = " + UserId + " and convert(date, a.BookingDate)= convert(date, '" + DateTime.Parse(Date).ToString("yyyy/MM/dd") + "') and a.SchId = " + schlId + " and((a.ActiveStatus = 'DA' and a.BookedStatus = 'B') or(a.ActiveStatus = 'A' and a.BookedStatus = 'B') or(a.ActiveStatus = 'C' and a.BookedStatus = 'C'))  and c.ActiveStatus = 'A' and c.SchId = " + schlId + " and b.SchId = " + schlId + " and b.ActiveStatus = 'A' and LOWER(c.weekDay) = LOWER(datename(dw, convert(date, a.BookingDate))) and ((convert(time, b.StartTime) >= convert(time, a.FromSlotMasterId) and convert(time, b.StartTime) < convert(time, a.ToSlotMasterId)) or convert(datetime, a.bookingDate) + convert(datetime, b.StartTime) <= convert(datetime, '" + CurrentDate.ToString("yyyy/MM/dd HH:mm") + "'))";

            string Currentdatablock = "select distinct(format(convert(datetime,b.StartTime),'HH:mm')) from  SPA_SchedulerSlotMaster b join CCTSP_SchedulerMaster c on c.SchedulerId = b.SchedulerId where c.ActiveStatus = 'A' and b.userid = " + UserId + " and c.SchId = " + schlId + " and b.SchId = " + schlId + " and b.ActiveStatus = 'A' and convert(datetime, '" + DateTime.Parse(Date).ToString("yyyy/MM/dd") + "') + convert(datetime, b.StartTime) <= convert(datetime, '" + CurrentDate.ToString("yyyy/MM/dd HH:mm") + "')";
            var Currentdata = SPA.Database.SqlQuery<string>(Currentdatablock).ToList();
            var MSGData = SPA.Database.SqlQuery<string>(Blockdate).ToList();
            MSGData.AddRange(Currentdata);
            Models.DateAndTimeArray BlockDetails = new Models.DateAndTimeArray()
            {
                BlockData = MSGData.ToArray(),
            };
            Jsondata = js.Serialize(BlockDetails);
            send = JObject.Parse(Jsondata);
            return send;
        }
        public List<Models.PaymentGateway> getPaymentDetail(string getall, int countryid)
        {
            var payquery = "select a.PaymentCatgName,b.PaymentDetail_Id,b.PaymentCatgId,b.Calculation,c.timezone_id as country_id,b.Percharge, LegalAmount = case when b.Calculation != 'Percentage' then round((b.quantity_or_price * b.Percharge), 2) else round(b.quantity_or_price, 2) end, b.quantity_or_price,b.discount,c.currency, TotalAmount = case when b.Calculation != 'Percentage' then round(((b.quantity_or_price * b.percharge) - ((b.quantity_or_price * b.percharge) * (b.discount / 100))), 2) else round((b.quantity_or_price - (b.quantity_or_price * (b.discount / 100))), 2) end from spa_paymentCategory a join spa_Payment_plan b on a.paymentCatgId = b.paymentCatgId join spatime_zone c on c.timezone_id = b.country_id where a.ActiveStatus = 'A' and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' and b.country_id = " + countryid + "  and(a.Where_to_display = 3 or a.Where_to_display = 1)";
            if (getall != null)
                payquery = payquery + "and b.paymentDetail_Id in (" + getall + ")";
            var getpaylist = SPA.Database.SqlQuery<Models.PaymentGateway>(payquery).ToList();
            return getpaylist;
        }
        public void CommonEmail(Models.Registration RegistrationDetails, int schlId, int LangId)
        {
            string subject = "";
            string body = "";
            string EmailId = RegistrationDetails.emailid;
            string subjectTemp = "";
            string ProjectName = "";
            XmlDocument xmldocument = null;
            XmlNode subjectNode = null;
            XmlNode bodyNode = null;
            subject = "Registration as per";
            ProjectName = "CLICK-AND-GO.com";
            int language = RegistrationDetails.Langid;
            string schlid = schlId.ToString();
            var LogoImage = "http://tshope.azurewebsites.net/images/Maarkss.png";
            var Databasebody = SPA.Language_NewShop.Where(c => c.Page_Name == "Registration_Page" && c.col2 == "A" && c.Schid == schlid && c.col1 == "Registration_Email" && c.Order_id == 41 && c.Lang_id == LangId).Select(c => c.Value).FirstOrDefault();
            if (System.IO.File.Exists(HostingEnvironment.MapPath("~/XMLPAGE/messages1.xml")))
            {
                xmldocument = new XmlDocument();
                xmldocument.Load(HostingEnvironment.MapPath("~/XMLPAGE/messages1.xml"));
                subjectNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='REGISTRATION_EMAIL']/PASSWORD/SUBJECT");
                bodyNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='REGISTRATION_EMAIL']/PASSWORD/BODY");
                body = bodyNode.InnerText;
                subjectTemp = subjectNode.InnerText;
                if (subjectTemp.Contains("@REGISTRATION_EMAIL"))
                    subject = Regex.Replace(subjectTemp, "@REGISTRATION_EMAIL", subject + " - CLICK-AND-GO.ch", RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("\"FirstName\"", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "\"FirstName\"", RegistrationDetails.FirstName, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("\"LastName\",", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "\"LastName\",", RegistrationDetails.FamilyName + "<br><br>", RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("\"LastName\" ", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "\"LastName\" ", RegistrationDetails.FamilyName + "<br><br>", RegexOptions.IgnoreCase);
                if (body.IndexOf("@DATA", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@DATA", Databasebody, RegexOptions.IgnoreCase);
                if (body.IndexOf("@Image", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@Image", LogoImage, RegexOptions.IgnoreCase);
                if (body.IndexOf("@ShopName", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@ShopName", "CLICK-AND-GO.ch", RegexOptions.IgnoreCase);

                var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/ContactMail");
                var postData = "Contactdetails=" + HttpUtility.UrlEncode(body);
                postData = postData + "&emailSend=" + EmailId;
                postData = postData + "&subject=" + subject;
                postData = postData + "&ProjectName=" + ProjectName;
                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responsestring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
        }
        public async Task SendEmail(Models.Registration RegistrationDetails, int schlId, int LangId)
        {
            await Task.Run(() => CommonEmail(RegistrationDetails, schlId, LangId));
        }
        public Models.CustomerDisplay GetRegistrationDetail(CCTSP_User user)
        {
            Models.CustomerDisplay customerDisplay = new Models.CustomerDisplay();
            customerDisplay.FirstName = user.FirstName;
            customerDisplay.FamilyName = user.LastName;
            customerDisplay.PhoneNumber = user.PhoneNo;
            customerDisplay.Email = user.EmailId;
            return customerDisplay;
        }
        #region RelatedTocookies
        //public void SetCookie(string key, string value, TimeSpan expires)
        //{
        //    HttpCookie encodedCookie = new HttpCookie(key, value);
        //    if (HttpContext.Current.Request.Cookies[key] != null)
        //    {
        //        var cookieOld = HttpContext.Current.Request.Cookies[key];
        //        cookieOld.Expires = DateTime.Now.Add(expires);
        //        cookieOld.Value = encodedCookie.Value;
        //        HttpContext.Current.Response.Cookies.Add(cookieOld);
        //    }
        //    else
        //    {
        //        encodedCookie.Expires = DateTime.Now.Add(expires);
        //        HttpContext.Current.Response.Cookies.Add(encodedCookie);
        //    }
        //}
        //public string GetCookie(string key)
        //{
        //    string value = string.Empty;
        //    HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
        //    if (cookie != null)
        //    {
        //        HttpCookie decodedCookie = cookie;
        //        value = decodedCookie.Value;
        //    }
        //    return value;
        //}
        //public void RemoveCookie(string Key)
        //{
        //    if (HttpContext.Current.Request.Cookies[Key] != null)
        //    {
        //        HttpCookie myCookie = new HttpCookie(Key);
        //        myCookie.Expires = DateTime.Now.AddDays(-1d);
        //        HttpContext.Current.Response.Cookies.Add(myCookie);
        //    }
        //}
        //public void CheckandSetCookies()
        //{
        //    var CModel = js.Deserialize<Models.CookieModel>(GetCookie("Auth"));
        //    HttpContext.Current.Session["UserId"] = CModel.UserId;
        //    HttpContext.Current.Session["SchoolId"] = schlId;
        //    HttpContext.Current.Session["RoleId"] = CModel.RoleId;
        //    HttpContext.Current.Session["UserEmailId"] = CModel.UserEmailId;
        //    HttpContext.Current.Session["UserFirstName"] = CModel.UserFirstName;
        //    HttpContext.Current.Session["UserLastName"] = CModel.UserLastName;
        //    HttpContext.Current.Session["UserGender"] = CModel.UserGender;
        //    HttpContext.Current.Session["UserName"] = CModel.UserName;
        //    HttpContext.Current.Session["PhoneNumber"] = CModel.PhoneNumber;
        //}
        #endregion
        #region ErrorandActivity
        public void ErrorSend(RouteData route, Exception e)
        {
            ErrorSend(route.Values["Action"].ToString(), route.Values["Controller"].ToString(), e);
        }
        public void ErrorSend(string Action, string controller, Exception e)
        {
            try
            {
                SPA_Error ErrorAdd = new SPA_Error();
                ErrorAdd.Action_Name = Action;
                ErrorAdd.Controller_name = controller;
                ErrorAdd.ErrorMsg = e?.Message;
                ErrorAdd.ReqColumn1 = schlId.ToString();
                ErrorAdd.ReqColumn2 = e?.InnerException?.Message;
                TimeZoneInfo IndiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime NowDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaZone);
                ErrorAdd.Created_Date = NowDate;
                SPA.SPA_Error.Add(ErrorAdd);
                SPA.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public void setActivityLog(Models.ActivityLog ActivityLog)
        {
            try
            {
                using (SPA = new cctspDatabaseEntities22())
                {
                    SPA_ActivityLog Activity = new SPA_ActivityLog();
                    Activity.Action_Name = ActivityLog.Action;
                    Activity.Controller_name = ActivityLog.controller;
                    Activity.schlId = ActivityLog.schId;
                    Activity.CreatedDate = DateTime.Now;
                    Activity.Activity = ActivityLog.Message;
                    if (ActivityLog.Activity != null)
                        Activity.ReqColumn1 = js.Serialize(ActivityLog.Activity);
                    SPA.SPA_ActivityLog.Add(Activity);
                    SPA.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion

        public List<Models.AccessMenu> AccessMenu(int LangId, int RoleId, string PageName, long? FlowId)
        {
            var AccessDetails = " select d.Value as TabName,c.CatgTypeId as MainTabId ,c.Gender as Link, c.Banner_Image as TabId ,c.Search_Image as Condition ,c.Email_image as ConditionValue from CCTSP_Role a " +
                                " join cctsp_RoleDetails b on  a.RoleId = b.RoleId  and  b.ActiveStatus='A' and b.Schid=a.Schlid " +
                                " join cctsp_CategoryDetails c on c.CatgTypeId=b.AccessToMenu and c.ActiveStatus='A' " +
                                " join Language_Label_Detail d on d.English_Label = c.CatgDesc ";
            if (FlowId > 1)
            {
                AccessDetails = AccessDetails + " join spa_flow f on f.Flow_id = " + FlowId + " and f.ActiveStatus = 'A' " +
                                " join spa_FlowDetails g on g.Flow_id = f.Flow_id and g.ActiveStatus = 'A' " +
                                " and b.AccessTomenu = g.Maintab_id and b.AccessToSub = g.subTab_Id";
            }
            AccessDetails = AccessDetails + " where a.RoleId = " + RoleId + " and d.Page_Name = '" + PageName + "' and d.Lang_Id = " + LangId + " and c.Gender is not null and (a.Schlid=" + schlId + " or a.Schlid=236) and a.ActiveStatus='A' " +
            " group by d.Value,c.Gender ,c.Banner_Image,c.Search_Image ,c.Email_image ,c.CatgTypeId";
            return SPA.Database.SqlQuery<Models.AccessMenu>(AccessDetails).ToList();
        }
        public List<Models.AccessSubMenu> AccessSubMenu(int LangId, int RoleId, string PageName, long? FlowId)
        {
            var Flow = "";
            if (FlowId > 1)
            {
                Flow = " join spa_flow h on h.Flow_id=" + FlowId + " and h.ActiveStatus='A' " +
                       "join spa_FlowDetails g on g.Flow_id = h.Flow_id and g.ActiveStatus = 'A' " +
                       "and b.AccessTomenu = g.Maintab_id and b.AccessToSub = g.subTab_Id ";
            }
            var AccessDetails = " select f.value as SubTabName,e.SolutionId as SubId,e.Amount as SubLink,e.Duration as SubTabId,e.Url as TabLink ,e.image as Condition,e.[Group] as ConditionValue ,e.SectionDesc as Link " +
                                " from CCTSP_Role a " +
                                " join cctsp_RoleDetails b on a.RoleId = b.RoleId and b.ActiveStatus='A' and  b.Schid=a.Schlid " +
                                " join cctsp_CategoryDetails c on c.CatgTypeId=b.AccessToMenu and c.ActiveStatus='A' " +
                                " join ctsp_SolutionMaster e on e.CatgtypeId = c.CatgtypeId and  e.SolutionId=b.AccessToSub and e.ActiveStatus='A' " +
                                " join Language_Label_Detail f on f.English_Label = e.SectionName " + Flow +
                                " where a.RoleId = " + RoleId + " and f.Page_Name = '" + PageName + "' and f.Lang_Id =" + LangId + " and (a.Schlid=" + schlId + " or a.Schlid=236) and a.ActiveStatus='A' ";
            return SPA.Database.SqlQuery<Models.AccessSubMenu>(AccessDetails).ToList();
        }
        public List<Models.ConditionList> LayoutCondition(Models.ShopMasterDetail Shop)
        {
            List<Models.ConditionList> ConditionList = new List<Models.ConditionList>();
            Models.ConditionList Condition = new Models.ConditionList();
            Condition.Condition = "AccessRight";
            Condition.ConditionValue = "YES";
            ConditionList.Add(Condition);
            Condition = new Models.ConditionList();
            Condition.Condition = "FreeCustomer";
            Condition.ConditionValue = Shop.Display_FreeCustomer == 1 ? "YES" : "NO";
            ConditionList.Add(Condition);
            Condition = new Models.ConditionList();
            Condition.Condition = "Invoicing";
            Condition.ConditionValue = Shop.Display_Invoice >= 1 ? "YES" : "NO";
            ConditionList.Add(Condition);
            Condition = new Models.ConditionList();
            Condition.Condition = "Marketing";
            Condition.ConditionValue = Shop.DisplayMarketing == "1" ? "YES" : "NO";
            ConditionList.Add(Condition);
            Condition = new Models.ConditionList();
            Condition.Condition = "Language";
            Condition.ConditionValue = schlId == 251 ? "YES" : "NO";
            ConditionList.Add(Condition);
            Condition = new Models.ConditionList();
            Condition.Condition = "BookingApproval";
            Condition.ConditionValue = Shop.BookingApproval == "NO" ? "NO" : "YES";
            ConditionList.Add(Condition);
            Condition = new Models.ConditionList();
            Condition.Condition = "Landing";
            Condition.ConditionValue = !string.IsNullOrEmpty(Shop.SchlWebsite) ? "NO" : "YES";
            ConditionList.Add(Condition);
            return ConditionList;
        }
        //public List<Models.TabInfo> Tabinfo()
        //{
        //    var TabDetails = "select a.RoleId,a.RoleName,a.Display_Status,c.CatgDesc as MainTab,c.CatgTypeId as MainTabId,c.Search_image as Condition,c.Email_image as ConditionValue,b.schid as Schlid," +
        //                     "d.ItenName as OwnAnyStatus,d.SolutionId as SubTabId,d.SectionName as SubTab,b.AccessToData,b.insertAccess,b.DeleteAccess,b.UpdateAccess," +
        //                     " convert(bigint,IsNull(b.AccesstoMenu,0)) as AccesstoMenu ,convert(bigint,IsNull(b.AccessToSub,0)) as AccessToSub " +
        //                     "from cctsp_Role a " +
        //                     "join cctsp_CategoryDetails c on c.CatgId = 147 and c.Activestatus = 'A' " +
        //                     "join ctsp_solutionMaster d on d.CatgTypeId = c.CatgTypeId and d.ActiveStatus = 'A'" +
        //                     "left join cctsp_RoleDetails b on a.RoleId = b.RoleId and b.ActiveStatus = 'A' and  b.schid = a.schlid " +
        //                     " and c.CatgTypeId = b.AccessToMenu and d.SolutionId = b.AccessToSub " +
        //                     " where a.ActiveStatus = 'A' and a.Role_Status = 1 and a.RoleName != 'Customer' and (a.schlid = " + schlId + " or a.schlid = 236) ";
        //    return SPA.Database.SqlQuery<Models.TabInfo>(TabDetails).ToList();
        //}
        public List<Models.TabInfo> Tabinfo(int LangId)
        {
            var TabDetails = "select a.RoleId,a.RoleName,a.Display_Status,g.Value as MainTab, " +
                             "c.CatgTypeId as MainTabId,c.Search_image as Condition,c.Email_image as ConditionValue, " +
                             "b.schid as Schlid,d.ItenName as OwnAnyStatus,d.SolutionId as SubTabId,d.[Image] as SubCondition,d.[Group] as SubConditionValue, " +
                             " b.AccessToData,b.insertAccess,b.DeleteAccess,b.UpdateAccess, " +
                             "convert(bigint, IsNull(b.AccesstoMenu, 0)) as AccesstoMenu , " +
                             "convert(bigint, IsNull(b.AccessToSub, 0)) as AccessToSub  " +
                             ",e.Value as SubTab " +
                             "from cctsp_Role a " +
                             "join cctsp_CategoryDetails c on c.CatgId = 147 and c.Activestatus = 'A' " +
                             "join ctsp_solutionMaster d on d.CatgTypeId = c.CatgTypeId and d.ActiveStatus = 'A' " +
                             "left join cctsp_RoleDetails b on a.RoleId = b.RoleId and b.ActiveStatus = 'A' " +
                             "and b.schid = a.schlid  and c.CatgTypeId = b.AccessToMenu and d.SolutionId = b.AccessToSub " +
                             "left join Language_Label_Detail e on e.Lang_Detail_Id = (select top 1 Lang_Detail_Id from  Language_Label_Detail where English_Label = d.SectionName and Lang_Id = " + LangId + " and ActiveStatus = 'A') " +
                             "left join Language_Label_Detail g on g.Lang_Detail_Id= (select top 1 Lang_Detail_Id from  Language_Label_Detail where English_Label=c.CatgDesc and Lang_Id=" + LangId + " and ActiveStatus='A') " +
                             "where a.ActiveStatus = 'A' and a.Role_Status = 1 and a.RoleName != 'Customer' " +
                             "and(a.schlid = " + schlId + " or a.schlid = 236) " +
                             "order by d.SolutionId ,a.RoleId";
            return SPA.Database.SqlQuery<Models.TabInfo>(TabDetails).ToList();
        }
        public Models.TabInfo AccessDetails(long RoleId, string SubTab)
        {
            Models.TabInfo Access = new Models.TabInfo();
            var TabDetails = "select a.RoleId,a.RoleName,c.CatgDesc as MainTab,c.CatgTypeId as MainTabId," +
                             "d.ItenName as OwnAnyStatus,d.SolutionId as SubTabId,d.SectionName as SubTab,b.AccessToData," +
                             " convert(bigint,IsNull(b.AccesstoMenu,0)) as AccesstoMenu ,convert(bigint,IsNull(b.AccessToSub,0)) as AccessToSub " +
                             "from cctsp_Role a " +
                             "join cctsp_CategoryDetails c on c.CatgId = 147 and c.Activestatus = 'A' " +
                             "join ctsp_solutionMaster d on d.CatgTypeId = c.CatgTypeId and d.ActiveStatus = 'A' " +
                             "left join cctsp_RoleDetails b on a.RoleId = b.RoleId and b.ActiveStatus = 'A' and b.schid = " + schlId + " " +
                             " and c.CatgTypeId = b.AccessToMenu and d.SolutionId = b.AccessToSub " +
                             " where a.ActiveStatus = 'A' and a.Role_Status = 1 and a.RoleName != 'Customer' and d.SectionName='" + SubTab + "' and a.RoleId=" + RoleId;
            Access = SPA.Database.SqlQuery<Models.TabInfo>(TabDetails).FirstOrDefault();
            return Access != null ? Access : new Models.TabInfo();
        }
        public string AccessRightRedirectLink(long RoleId, long? FlowId)
        {
            var Data = "select c.CatgDesc as AccessTab ,c.Gender as AccessLink  from cctsp_Role a " +
                       "join cctsp_RoleDetails b on a.RoleId = b.RoleId and b.Schid = a.Schlid " +
                       "join cctsp_CategoryDetails  c on c.CatgTypeId = convert(bigint, b.AccesstoMenu) ";
            if (FlowId > 1)
            {
                Data = Data + "join spa_flow f on f.Flow_id =" + FlowId + "  and f.ActiveStatus = 'A' " +
                       "join spa_FlowDetails g on g.Flow_id = f.Flow_id and g.ActiveStatus = 'A' " +
                       "and b.AccessTomenu = g.Maintab_id ";
            }
            Data = Data + "Where c.catgid = 147 and c.activeStatus = 'A' and b.ActiveStatus = 'A' " +
                       "and(a.Schlid = 236 or a.Schlid = " + schlId + ") and a.RoleId = " + RoleId + "  " +
                       "group by c.CatgDesc,c.Gender";
            var Details = SPA.Database.SqlQuery<Models.Redirection>(Data).ToList();
            var Link = Details.Where(c => c.AccessTab == "Reservation").FirstOrDefault() != null ? Details.Where(c => c.AccessTab == "Reservation").Select(c => c.AccessLink).FirstOrDefault() : Details.Select(c => c.AccessLink).FirstOrDefault();
            return Link;
        }
        public List<Models.Invoice> InvoiceList(string Status, long UserId)
        {
            var Data = " SELECT a.EmpSchDetailsId AS ReservationId,b.FirstName AS CustomerFName,b.LastName AS CustomerLName, c.CatgDesc AS ProductName, " +
                       " CAST(a.FromSlotMasterId AS TIME) AS StartTime, CAST(a.ToSlotMasterId  AS TIME) AS EndTime, a.BookingDate, a.CLIENT_UserId as CustomerId , " +
                       " CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1 THEN a.Product_price ELSE NULL END AS DECIMAL(38, 2)) AS ProductPrice, " +
                       " CAST(d.Duration AS INT) AS Duration ,e.Currency,b.Invoice_Service,a.Invoice_Id," +
                       " j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName as FlowStatus, " +
                       " a.EMP_UserId as EmployeeId" +
                       " FROM SPA_EmployeeScheduler a " +
                       " JOIN cctsp_User b ON b.UserId = a.CLIENT_UserId AND b.RoleId = 4" +
                       " JOIN cctsp_CategoryDetails c ON c.CatgTypeId = a.Product_Id " +
                       " JOIN ctsp_SolutionMaster d ON d.CatgtypeId = a.Product_Id " +
                       " JOIN cctsp_SchoolMaster e ON e.Schlid = a.Schid " +
                       " JOIN ctsp_SolutionMaster g ON g.SectionName = 'For Invoicing' and g.Activestatus = 'A' " +
                       " JOIN cctsp_user h on h.UserId = " + UserId + " " +
                       " JOIN cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236) " +
                       " AND i.ActiveStatus = 'A' " +
                       " JOIN cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                       " AND j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub) " +
                       " AND j.SchId = i.Schlid " +
                       " AND((j.AccessToData = 'Own' and a.Emp_UserId = h.UserId) or(j.AccessToData != 'Own')) " +
                       " LEFT JOIN SPA_INVOICE_Detail f ON f.ReservationId=a.EmpSchdetailsId and f.ActiveStatus='A'" +
                       " WHERE (Select Count(Invoice_Id) from SPA_INVOICE_Detail Where ReservationId=a.EmpSchdetailsId and ActiveStatus='A')=0" +
                       " AND a.Schid = " + schlId + " AND a.ActiveStatus = 'C' AND a.BookedStatus = 'C' AND f.Invoice_Id is null " +
                       " ORDER BY a.CLIENT_UserId";
            return SPA.Database.SqlQuery<Models.Invoice>(Data).ToList();
        }
        public List<Models.ManualInvoice> ManualInvoiveList(string ReseravtionIdList, long? InvoiceId)
        {
            return SPA.Database.SqlQuery<Models.ManualInvoice>(sql.ManualInvoiceQuery(ReseravtionIdList, InvoiceId)).ToList();
        }
        public List<Models.ManualInvoice> PrintMailManualInvoiveList(string InvoiceId)
        {
            return SPA.Database.SqlQuery<Models.ManualInvoice>(sql.PrintEmailManualInvoice(InvoiceId)).ToList();
        }
        public List<Models.InvoiceServicesList> ServicesList(long EmpId, long Schlid)
        {
            return SPA.Database.SqlQuery<Models.InvoiceServicesList>(sql.InvoiceServicesList(EmpId, Schlid)).ToList();
        }
        public List<Models.UserDetails> EmpAndCustomerList(long Schlid, long UserId)
        {
            return SPA.Database.SqlQuery<Models.UserDetails>(sql.EmployeeAndCustomerList(Schlid, UserId)).ToList();
        }
        public List<Models.NoteSectionList> NoteSectionList(int LangId, long SchlOrder)
        {
            return SPA.Database.SqlQuery<Models.NoteSectionList>(sql.SectionList(LangId, SchlOrder)).ToList();
        }
        public int CheckRandom()
        {
            Random random = new Random();
            int Number = random.Next(1, 4);
            return Number;
        }

        public string GetAnyLinkHtml(string Link)
        {
            var request = (HttpWebRequest)WebRequest.Create(Link);
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            var responsestring = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responsestring;
        }
        public List<Models.CatgDropDownList> getInvoiceCatgDetails(string CatgId, int LangId)
        {
            List<string> CatgList = CatgId.Split(',').Select(c => c).ToList();
            return (from c in SPA.CCTSP_CategoryDetails
                    where c.ActiveStatus == "A" && CatgList.Contains(c.CCTSP_CategoryMaster.CatgName) && c.Lang_Id == LangId
                    select new Models.CatgDropDownList
                    {
                        CatgDesc = c.CatgDesc,
                        CatgId = c.CatgId,
                        CatgType = c.CatgType,
                        CatgTypeId = c.CatgTypeId,
                        CatgName = c.CCTSP_CategoryMaster.CatgName,
                        Group_OrderData = c.Group_orderdata
                    }).ToList();
        }
        public void UpdateInvoicestatus(string Status, List<long> InvoiceIdList, DateTime? Date, bool IsCurrent)
        {
            if (InvoiceIdList.Count > 0)
            {
                SPA = new cctspDatabaseEntities22();
                if (Date == null)
                    Date = ZonalDate(null);
                var DeleteQuery = "update SPA_INVOICE_STATUS set ActiveStatus='T' where ActiveStatus='A' and Invoice_Id in (" + string.Join(",", InvoiceIdList) + ")";
                SPA.Database.ExecuteSqlCommand(DeleteQuery);
                List<SPA_INVOICE_STATUS> SPASTATUS = InvoiceIdList.Select(c => new SPA_INVOICE_STATUS
                {
                    Invoice_Status = Status,
                    ActiveStatus = "A",
                    CreatedDate = Date.Value,
                    UpdatedDate = Date.Value,
                    Invoice_Id = c
                }).ToList();
                SPA.SPA_INVOICE_STATUS.AddRange(SPASTATUS);
                if (IsCurrent && InvoiceIdList.Count > 0)
                {
                    SPA.SPA_INVOICE_MASTER.Where(c => InvoiceIdList.Contains(c.Invoice_Id)).ToList().ForEach(c => { c.Invoice_Status = Status; c.UpdatedDate = Date.Value; });
                }
                SPA.SaveChanges();
            }
        }
        public void UpdateReminderInvoicestatus(string Status, List<long> InvoiceIdList, DateTime? Date, bool IsCurrent)
        {
            if (Date == null)
                Date = ZonalDate(null);
            UpdateInvoicestatus(Status, InvoiceIdList, Date, false);
            if (IsCurrent)
            {
                var Invoicemaster = SPA.SPA_INVOICE_MASTER.Where(c => InvoiceIdList.Contains(c.Invoice_Id)).ToList();
                foreach (var item in Invoicemaster)
                {
                    item.Invoice_Status = Status;
                    item.UpdatedDate = Date.Value;
                    if (item.Reminderdate1 == null)
                        item.Reminderdate1 = Date;
                    else if (item.Reminderdate2 == null)
                        item.Reminderdate2 = Date;
                    else if (item.Reminderdate3 == null)
                        item.Reminderdate3 = Date;
                    else
                        item.Reminderdate4 = Date;
                }
                SPA.SaveChanges();
            }

        }
        public bool UpdateInvoiceStatusAuto(string InvoiceId, DateTime? Date, bool IsCurrent)
        {
            try
            {
                if (Date == null)
                    Date = DateTime.Now;
                var CurrentDate = Date.Value.ToString("yyyy-MM-dd HH:mm:ss");
                SPA.Database.ExecuteSqlCommand(sql.AutoupdatestatusofInvoice(InvoiceId, CurrentDate, IsCurrent));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public Invoice_CategoryDetails InvoiceDetails(DateTime CurrenDate, string SelectedValue, long Invoiceid, long CatgTypeId, string Name)
        {
            Invoice_CategoryDetails Details = new Invoice_CategoryDetails();
            Details.ActiveStatus = "A";
            Details.CreatedDate = CurrenDate;
            Details.Value = SelectedValue;
            Details.InvoiceId = Invoiceid;
            Details.schlId = schlId;
            Details.CatgTypeId = CatgTypeId;
            Details.field1 = Name;
            return Details;
        }
        public List<SPA_INVOICE_Detail> AddINVDETRESERVATION(List<Models.ValueList> value, long Invoice_Id, DateTime date)
        {
            try
            {
                List<SPA_INVOICE_Detail> InvoiceDetailsList = new List<SPA_INVOICE_Detail>();
                if (value.Count > 0 && value.Select(c => c.Id != 0).Select(c => c).Count() > 0)
                {
                    var qry = " SELECT a.EmpSchDetailsId as ReservationId,a.Product_Id,b.CatgDesc as ProductName,a.BookingDate, " +
                                " convert(float,c.Amount) as Amount,convert(int,c.Duration) as Duration" +
                                " ,ISNULL(b.VAT,0.0) as VAT" +
                                " ,b.Email_Image as DefaultText,d.Tarif_Number,d.Settlement_Number,d.Settlement_text,d.Field1 as Flag " +
                                " FROM SPA_EmployeeScheduler a" +
                                " JOIN cctsp_CategoryDetails b on b.CatgTypeId = a.Product_Id" +
                                " JOIN ctsp_solutionMaster c on c.CatgTypeId = b.CatgTypeId" +
                                " LEFT JOIN Health_Insurance d on d.insurance_id=b.Insurance_Id " +
                                " WHERE ((a.ActiveStatus = 'C' AND a.BookedStatus = 'C') OR (a.ActiveStatus = 'I' AND a.BookedStatus = 'I') ) AND a.schId = " + schlId +
                                " AND EmpSchDetailsId IN (" + string.Join(",", value.Select(c => c.Id).ToList()) + ")";
                    var Returnvalue = SPA.Database.SqlQuery<Models.getDataforProduct>(qry).ToList();
                    var ModelsData = Returnvalue
                                        .Select(c => new SPA_INVOICE_Detail
                                        {
                                            ActiveStatus = "A",
                                            ReservationId = c.ReservationId,
                                            CreatedDate = date,
                                            UpdatedDate = date,
                                            SchlId = schlId,
                                            ProductName = c.ProductName,
                                            Settlement_Number = c.Settlement_Number,
                                            Settlement_text = c.Flag == "1" ? c.DefaultText : c.Settlement_text,
                                            Tarif_Number = c.Tarif_Number,
                                            Product_Id = c.Product_Id,
                                            Duration = Convert.ToInt32(value.Where(e => e.Id == c.ReservationId).Select(e => e.value).FirstOrDefault()),
                                            Quantity = value.Where(e => e.Id == c.ReservationId).Select(e => e.value).FirstOrDefault() / 5,
                                            FiveMinuteChargeorPercharge = c.Amount * 5 / c.Duration,
                                            TotalPrice = (value.Where(e => e.Id == c.ReservationId).Select(e => e.value).FirstOrDefault() / 5) * (c.Amount * 5 / c.Duration),
                                            Invoice_Id = Invoice_Id,
                                            Date_Of_Buying = DateTime.Parse(c.BookingDate, enGB).ToString("yyyy-MM-dd") == "0001-01-01" ? DateTime.Now : DateTime.Parse(c.BookingDate, enGB),
                                            VAT = c.VAT,
                                            TotalPricewithvat = (value.Where(e => e.Id == c.ReservationId).Select(e => e.value).FirstOrDefault() / 5) * (c.Amount * 5 / c.Duration) + (value.Where(e => e.Id == c.ReservationId).Select(e => e.value).FirstOrDefault() / 5) * (c.Amount * 5 / c.Duration) * c.VAT / 100
                                        }).ToList();
                    InvoiceDetailsList.AddRange(ModelsData);
                    //SPA.SaveChanges();
                }
                return InvoiceDetailsList;
            }
            catch (Exception Ex)
            {
                return new List<SPA_INVOICE_Detail>();
            }

        }
        public List<SPA_INVOICE_Detail> AddINVDETADDON(List<Models.ValueList> value, long Invoice_Id, DateTime date)
        {
            try
            {
                List<SPA_INVOICE_Detail> InvoiceDetailsList = new List<SPA_INVOICE_Detail>();
                if (value.Count > 0 && value.Select(c => c.Id != 0).Select(c => c).Count() > 0)
                {
                    var qry = " SELECT a.Add_On_Pid,a.ProductName,a.Manufacturer,a.Dozes,b.Selling_Price as Amount,a.VAT" +
                              " ,c.Tarif_Number,c.Settlement_Number,c.Settlement_text,c.Field1 as Flag,b.field3 as DefaultText " +
                              " FROM Add_on_Product_Master a" +
                              " JOIN Add_On_Product_Detail b on b.Add_On_Pid = a.Add_On_Pid" +
                              " LEFT JOIN Health_Insurance c on c.insurance_id=a.Insurance_Id " +
                              " WHERE a.schlId = " + schlId + " and a.ActiveStatus = 'A' and b.ActiveStatus = 'A'" +
                              " and a.Add_On_Pid in (" + string.Join(",", value.Select(c => c.Id).Distinct().ToList()) + ")";
                    var Returnvalue = SPA.Database.SqlQuery<Models.getDataforProduct>(qry).ToList();
                    var ModelData = value.Select(c => new SPA_INVOICE_Detail
                    {
                        ActiveStatus = "A",
                        Add_On_Pid = c.Id,
                        CreatedDate = date,
                        UpdatedDate = date,
                        SchlId = schlId,
                        Settlement_Number = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Settlement_Number).FirstOrDefault(),
                        Settlement_text = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Flag).FirstOrDefault() == "1" ? Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.DefaultText).FirstOrDefault() : Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Settlement_text).FirstOrDefault(),
                        Tarif_Number = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Tarif_Number).FirstOrDefault(),
                        ProductName = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.ProductName).FirstOrDefault(),
                        Invoice_Id = Invoice_Id,
                        Manufacturer = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Manufacturer).FirstOrDefault(),
                        Dozes = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Dozes).FirstOrDefault(),
                        Quantity = c.value,
                        FiveMinuteChargeorPercharge = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Amount).FirstOrDefault(),
                        TotalPrice = c.value * Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Amount).FirstOrDefault(),
                        Date_Of_Buying = c.Date.ToString("yyyy-MM-dd") == "0001-01-01" ? DateTime.Now : (c.Date + c.Time),
                        VAT = Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.VAT).FirstOrDefault(),
                        TotalPricewithvat = c.value * Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Amount).FirstOrDefault() + (c.value * Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.Amount).FirstOrDefault() * Returnvalue.Where(d => d.Add_On_Pid == c.Id).Select(d => d.VAT).FirstOrDefault() / 100)
                    });
                    //SPA.SPA_INVOICE_Detail.AddRange(ModelData);
                    //SPA.SaveChanges();
                    InvoiceDetailsList.AddRange(ModelData);
                }
                return InvoiceDetailsList;
            }
            catch (Exception Ex)
            {
                return new List<SPA_INVOICE_Detail>();
            }

        }

        public List<SPA.SPA_INVOICE_Detail> AddSettlementTextToInvoice(List<Models.ValueList> value, long Invoice_Id, DateTime date)
        {
            try
            {
                List<SPA_INVOICE_Detail> InvoiceDetailsList = new List<SPA_INVOICE_Detail>();
                if (value.Count > 0 && value.Select(c => c.Id != 0).Select(c => c).Count() > 0)
                {
                    var qry = " select insurance_id, SettlementNumber as Settlement_Number,Amount,Duration,Health_Assign_Id,Settlement_text,Tarif_Number, " +
                              " Cast(Cast(Duration as float) / 5 as float) as Quantity, " +
                              " Cast((cast(Amount as float) * 5) / cast(Duration as int) as float) as Rate " +
                              " from Assigned_Health_Insurance " +
                              " where Health_Assign_Id in (" + string.Join(",", value.Select(c => c.Id).Distinct().ToList()) + ")";
                    var Returnvalue = SPA.Database.SqlQuery<Models.settlementTxt>(qry).ToList();
                    var ModelData = value.Select(c => new SPA_INVOICE_Detail
                    {
                        ActiveStatus = "A",
                        Health_Assign_id = c.Id,
                        Settlement_Number = Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Settlement_Number).FirstOrDefault(),
                        Settlement_text = c.SettlementText != null ? c.SettlementText : Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Settlement_text).FirstOrDefault(),
                        Tarif_Number = Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Tarif_Number).FirstOrDefault(),
                        Duration = Convert.ToInt16(c.value),
                        ProductName = c.SettlementText != null ? c.SettlementText : Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Settlement_text).FirstOrDefault(),
                        CreatedDate = date,
                        UpdatedDate = date,
                        SchlId = schlId,
                        Invoice_Id = Invoice_Id,
                        Quantity = (c.value / 5),
                        FiveMinuteChargeorPercharge = Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Rate).FirstOrDefault(),
                        TotalPrice = (c.value / 5) * Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Rate).FirstOrDefault(),
                        Date_Of_Buying = c.Date.ToString("yyyy-MM-dd") == "0001-01-01" ? DateTime.Now : (c.Date + c.Time),
                        TotalPricewithvat = (c.value / 5) * Returnvalue.Where(d => d.Health_Assign_id == c.Id).Select(d => d.Rate).FirstOrDefault(),
                    });
                    InvoiceDetailsList.AddRange(ModelData);
                }
                return InvoiceDetailsList;
            }
            catch (Exception Ex)
            {
                return new List<SPA_INVOICE_Detail>();
            }
        }
        //public async Task MailAsyncInvoice(string InvoiceId, string status, bool isprint)
        //{
        //    var LinkName = GetCurrenturl();
        //    await Task.Run(() => MailInvoice(InvoiceId, status, isprint));
        //}
        //public void MailInvoice(string InvoiceId, string status, bool isprint)
        //{
        //    string LinkName = GetCurrenturl();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(InvoiceId) && !string.IsNullOrEmpty(status))
        //        {
        //            var Info = SPA.Database.SqlQuery<Models.InvoiceCustomerDetails>(sql.InvoiceCustomerDetails(InvoiceId)).ToList();
        //            DateTime CurrentDate = ZonalDate(Info.Select(c => c.TimeZone).FirstOrDefault());
        //            foreach (var item in Info)
        //            {
        //                var Link = "/PDF/InvoicePrint?InvoiceId=" + item.Invoice_Id + "&IsPrint=" + isprint + "&Status=" + status + "&schlId=" + schlId;
        //                Link = LinkName + Link;
        //                if (!string.IsNullOrEmpty(item.EmailId))
        //                {
        //                    EmailSend.MailInvoice(item.EmailId, Link, "Invoice.pdf", item.Lang_Id.Value);
        //                    HttpContext.Current.Session["Message"] = "MailSentSuccessFully";
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        public void MailInvoice(string InvoiceId, string status, bool isprint, string RStatus)
        {
            string LinkName = GetCurrenturl();
            try
            {
                if (!string.IsNullOrEmpty(InvoiceId) && !string.IsNullOrEmpty(status))
                {

                    var Info = SPA.Database.SqlQuery<Models.InvoiceCustomerDetails>(sql.InvoiceCustomerDetails(InvoiceId)).ToList();
                    DateTime CurrentDate = ZonalDate(Info.Select(c => c.TimeZone).FirstOrDefault());
                    EmailSend.EmailInvoice(Info, CurrentDate, LinkName, isprint, status, RStatus);
                    HttpContext.Current.Session["AddCustomerMsg"] = "MailSentSuccessFully";
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task AsyncGenerate(List<Models.twodMatrix> twod)
        {
            await Task.Run(() => GenerateGroup2dMatrix(twod));
        }
        public void GenerateGroup2dMatrix(List<Models.twodMatrix> twod)
        {
            var qry = "";
            foreach (var item in twod)
                qry += "UPDATE SPA_INVOICE_MASTER set Image_barcode='" + Generate2dMatrix(item.invoiceDate, item.ZSRNO, item.CustBdate.Value, item.custZip.Value, item.Amount.Value, item.FirstTreatment.Value) + "',field3='" + item.ESRCODE + "' where Invoice_Id=" + item.InvoiceId + " ";
            SPA.Database.ExecuteSqlCommand(qry);
        }
        public string Generate2dMatrix(DateTime invoiceDate, string ZSRNO, DateTime CustBdate, long custZip, decimal Amount, DateTime FirstTreatment)
        {
            string Text = null;
            string name = null;
            var path = "/Upload/";
            if (string.IsNullOrEmpty(name))
                name = DateTime.Now.ToString("ddMMyyyyHHss") + CheckRandom() + ".jpg";
            else
                name = name + ".jpg";
            Text = Generate2dcode(invoiceDate, ZSRNO, CustBdate, custZip, Amount, FirstTreatment);
            if (!string.IsNullOrEmpty(Text))
            {
                DataMatrix.net.DmtxImageEncoder encoder = new DataMatrix.net.DmtxImageEncoder();
                Bitmap TwoDCode = encoder.EncodeImage(Text);
                bool isexist = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(path));
                if (!isexist)
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                }
                var PathName = Path.Combine(HttpContext.Current.Server.MapPath(path), name);
                TwoDCode.Save(PathName, System.Drawing.Imaging.ImageFormat.Jpeg);
                //DataMatrix.net.DmtxImageDecoder decoder = new DataMatrix.net.DmtxImageDecoder();
                //var abc = decoder.DecodeImage(TwoDCode).FirstOrDefault();
                return path + name;
            }
            return "";
        }
        public string Generate2dcode(DateTime invoiceDate, string ZSRNO, DateTime CustBdate, long custZip, decimal Amount, DateTime FirstTreatment)
        {
            long n;
            DateTime dt;
            long Total = 0;
            Random ra = new Random();
            int Number = ra.Next(10, 99);
            if (!string.IsNullOrEmpty(ZSRNO) && ZSRNO.Length >= 7 && long.TryParse(ZSRNO.Substring(ZSRNO.Length - 6, 6), out n))
                Total += Convert.ToInt64(ZSRNO.Substring(ZSRNO.Length - 6, 6));
            else
                return "";
            dt = new DateTime(1900, 01, 01);
            Total += Convert.ToInt64((CustBdate - dt).TotalDays);
            if (custZip > 0)
                Total += custZip;
            if (Amount > 0)
                Total += Convert.ToInt64(Math.Round(Amount, 1));
            dt = new DateTime(1900, 01, 01);
            Total += Convert.ToInt64((FirstTreatment - dt).TotalDays);
            return "/-/" + "#" + Number + "#" + invoiceDate.ToString("ddMMyyyyHHmmss") + "#" + Total;
        }
        public string SmallCalLangTranslate(int? LanguageId)
        {
            var LngLocal = "en";
            if (LanguageId == 2)
                LngLocal = "de";
            if (LanguageId == 3)
                LngLocal = "fr-ca";
            if (LanguageId == 4)
                LngLocal = "it";
            return LngLocal;
        }
        public string GetCurrenturl()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
        }
        public string LogoImg(string LogoPath)
        {
            var Link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "");
            if (string.IsNullOrEmpty(LogoPath))
                LogoPath = (Link + "/images/prescription-img.png").Replace("localhost", "tshope.click-and-go.in");
            else
                LogoPath = (Link + LogoPath);
            return LogoPath;
        }
        public bool ShowReminderButton(Models.Invoice Invoice, DateTime CurrentDate, int? ExtendDuration)
        {
            ExtendDuration = ExtendDuration != null ? ExtendDuration : 10;
            if (Invoice.Reminderdate3 != null)
            {
                if (Invoice.Reminderdate3.Value.AddDays(ExtendDuration.Value) > CurrentDate)
                    return true;
                else
                    return false;
            }
            else if (Invoice.Reminderdate2 != null)
            {
                if (Invoice.Reminderdate2.Value.AddDays(ExtendDuration.Value) > CurrentDate)
                    return true;
                else
                    return false;
            }
            else if (Invoice.Reminderdate1 != null)
            {
                if (Invoice.Reminderdate1.Value.AddDays(ExtendDuration.Value) > CurrentDate)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        public void AddDefaultDropValues(int? Lang_Id, string Invoice_Id)
        {
            SPA.Database.ExecuteSqlCommand(sql.addDefaultDropDownValue(Invoice_Id, Lang_Id));
        }
        public Prescription_Master AddPrescreptionMaster(int ReservationId, int diff, int UserId, DateTime ZonalDate)
        {
            Prescription_Master PM = null;
            using (SPA = new cctspDatabaseEntities22())
            {
                PM = SPA.Prescription_Master.Where(c => c.BookingId == ReservationId && c.ActiveStatus == "A" && c.SchId == schlId && c.Diff == diff).FirstOrDefault();
                if (PM == null)
                {
                    PM = new Prescription_Master();
                    PM.SchId = schlId;
                    PM.ActiveStatus = "A";
                    PM.BookingId = ReservationId;
                    PM.Diff = diff;
                    PM.UserId = UserId;
                    PM.created_on = ZonalDate;
                    SPA.Prescription_Master.Add(PM);
                    SPA.SaveChanges();
                }
                var DeleteQuery = "update Prescription_Detail set ActiveStatus='D' where ActiveStatus='A' and Prescription_Id=" + PM.Prescription_Id;
                SPA.Database.ExecuteSqlCommand(DeleteQuery);
            }
            return PM;
        }
        //Get History of Any Customer
        public List<Models.PendingApproval> CustomerHistory(int CustomerId)
        {
            return SPA.Database.SqlQuery<Models.PendingApproval>(sql.CustomerHistory(schlId, CustomerId)).ToList();
        }
        //Note Base on Reservation Booked Status
        public List<Models.getHistoryNotes> getAllHistoryNotes(long? ReservationId, long? UserId, int? Flow)
        {
            if (ReservationId == null && UserId == null && Flow == null)
                return new List<Models.getHistoryNotes>();
            return SPA.Database.SqlQuery<Models.getHistoryNotes>(sql.getAllHistoryNotes(ReservationId, schlId, UserId, Flow)).ToList();
        }
        //Note Base on Reservation Booked Status
        public List<Models.GetNoteHistoryDetails> GetNoteHistoryList(string BookingIdList, long UserId, int diff)
        {
            return SPA.Database.SqlQuery<Models.GetNoteHistoryDetails>(sql.GetAllNoteHistory(BookingIdList, UserId, diff)).ToList();
        }
        public string LongRandom(int min, int max, int digits)
        {
            Random ran;
            string result = "";
            int cnt = 0;
            if (digits > 8)
            {
                var GetReminder = Math.Floor(decimal.Parse((digits / 7).ToString()));
                ran = new Random();
                for (int i = 1; i <= GetReminder; i++)
                {
                    result += (ran.Next(min, max)).ToString();
                }
            }
            else
            {
                ran = new Random();
                ran.Next(min, max);
            }
            if (digits != result.Count() && digits > result.Count())
            {
                cnt = (digits - result.Count());
                for (int i = 1; i <= cnt; i++)
                {
                    ran = new Random();
                    result += ran.Next(0, 9);
                }
            }
            return result;
        }
        public static string Mid(string s, int a, int b)

        {

            string temp = s.Substring(a - 1, b);

            return temp;

        }
        public long getCheckDigit(string number)
        {
            var CheckSum = new int[] { 0, 9, 4, 6, 8, 2, 7, 1, 3, 5 };
            var Carry = 0;
            var i = 1;
            for (i = 1; i <= number.Length; i++)
                Carry = CheckSum[((Carry + Convert.ToInt32(Mid(number, i, 1))) % 10)];
            return (10 - Carry) % 10;
        }
        public string MakeESRCODE(string Currency, Decimal Amount, string Accountnumber)
        {
            string ESRNUMBER = "";
            long n;
            string Temp = "";
            //Currency= Will be Equal to CHF,EUR and lots more as this functionality is for Europe only so two currencies are considered
            if (Currency == "EUR")
                ESRNUMBER += "03";
            else
                ESRNUMBER += "01";
            /*Amount*/
            if (Amount > 0)
                Temp = Convert.ToInt64(String.Format("{0:0.00}", Amount).Replace(".", "")).ToString("0000000000");
            else
                Temp = "00000000000";
            /*Amount Check Sum*/
            ESRNUMBER += Temp + getCheckDigit(Temp);
            Temp = "";
            ESRNUMBER += ">";
            /*Reference Number*/
            Temp = LongRandom(10000000, 99999999, 26);
            /*Check Sum for Reference Number*/
            ESRNUMBER += Temp + getCheckDigit(Temp) + "+ ";
            Temp = "";
            /*Account Number*/
            Regex reg = new Regex(@"\b\d{1,2}-\d{1,6}-\d{1}");
            if (!string.IsNullOrEmpty(Accountnumber) && Accountnumber.Contains("-") && Accountnumber.Count(c => c == '-') >= 2 && reg.IsMatch(Accountnumber))
                ESRNUMBER +=
                        /*Two Digit-Six Digit-One Digit*/
                        (long.TryParse(Accountnumber.Split('-').FirstOrDefault(), out n)
                        ? Convert.ToInt64(Accountnumber.Split('-').FirstOrDefault()).ToString("00")
                        : "00")
                        + (
                        long.TryParse(Accountnumber.Split('-').Skip(1).FirstOrDefault(), out n)
                        ? long.Parse(Accountnumber.Split('-').Skip(1).FirstOrDefault()).ToString("000000")
                        : "000000"
                        ) +
                        (
                        long.TryParse(Accountnumber.Split('-').Skip(2).FirstOrDefault(), out n)
                        ? long.Parse(Accountnumber.Split('-').Skip(2).FirstOrDefault()).ToString("0")
                        : "0"
                        );
            else
                ESRNUMBER += "000000000";
            ESRNUMBER += ">";
            return ESRNUMBER;
        }
        public bool CheckInvoice(int? statusInvoice)
        {
            return statusInvoice != 1 && statusInvoice != 3;
        }
        public List<Models.AddOnProductList> AddOnProductList()
        {
            return SPA.Add_On_Product_Detail.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.AddOnProductList
            {
                ProductName = c.Add_on_Product_Master.ProductName,
                AddOnProductId = c.Add_On_Pid,
                Selling_Price = c.Selling_Price.Value,
                Settlement_Number = c.Add_on_Product_Master.Health_Insurance.Settlement_Number,
                Settlement_text = c.Add_on_Product_Master.Health_Insurance.Settlement_text != null ? c.Add_on_Product_Master.Health_Insurance.Settlement_text : c.Add_on_Product_Master.Field3,
                Tarif_Number = c.Add_on_Product_Master.Health_Insurance.Tarif_Number,
                Manufacturer = c.Add_on_Product_Master.Manufacturer,
                Dozes = c.Add_on_Product_Master.Dozes,
                vat = c.Add_on_Product_Master.VAT
            }).ToList();
        }
        public string CommonImageUpload(string ImgFile, long UserId)
        {
            string PathDetails = "";
            var FileName = "";
            if (ImgFile != null)
            {
                try
                {
                    PathDetails = "/UserImage/" + UserId + "/";
                    bool IsExists = Directory.Exists(HttpRuntime.AppDomainAppPath + PathDetails);
                    if (!IsExists)
                    {
                        System.IO.Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + PathDetails);
                    }
                    if (ImgFile.Contains("base64") && ImgFile.Contains("data:"))
                        ImgFile = ImgFile.Replace(ImgFile.Split(',')[0] + ",", "");
                    byte[] imageBytes = Convert.FromBase64String(ImgFile);
                    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    Image image = Image.FromStream(ms, true);
                    FileName = DateTime.Now.ToString("ddMMMyyyyHHmm") + ".jpg";
                    var PathName = Path.Combine(HttpContext.Current.Server.MapPath("~" + PathDetails), FileName);
                    image.Save(PathName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    PathDetails = PathDetails + FileName;
                }
                catch (Exception e)
                {
                }
            }
            return PathDetails;

        }
        public string CommonImageUpload(string file, string Resize)
        {
            try
            {
                if (!string.IsNullOrEmpty(Resize))
                {
                    var SplitPara = Resize.Split(',');
                    if (SplitPara.Count() > 3)
                    {
                        var width = Convert.ToInt32(SplitPara[0].Replace("-", "").Replace("px", "").Trim());
                        var height = Convert.ToInt32(SplitPara[1].Replace("-", "").Replace("px", "").Trim());
                        var left = Convert.ToInt32(SplitPara[2].Replace("-", "").Replace("px", "").Trim());
                        var top = Convert.ToInt32(SplitPara[3].Replace("-", "").Replace("px", "").Trim());
                        var ImagePath = HttpContext.Current.Server.MapPath(file);

                        var image = new System.Web.Helpers.WebImage(ImagePath);
                        image.Resize(width, height);
                        image.Crop(top, left, image.Height - top - 153, image.Width - left - 316);
                        System.IO.File.Delete(ImagePath);
                        image.Save(ImagePath);
                    }
                }
            }
            catch (Exception e)
            {

            }
            return file;
        }
        public bool Validate<T>(T obj, out ICollection<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        }
        public List<Models.combineSucImport> AddDataTableToDatabase(DataTable dt, List<Models.ImportCustomer> ImportCust)
        {
            //Initialize the return value
            List<Models.combineSucImport> sImportCust = new List<Models.combineSucImport>();
            //Rows Loop
            foreach (DataRow Rows in dt.Rows)
            {
                //Initialize the user table
                Models.userTable user = new Models.userTable();
                //Column Loop
                foreach (DataColumn Columns in dt.Columns)
                {
                    //List of Column in which data need to be imported
                    var GetColumnInfo = ImportCust.Where(c => (c.ColumnName == Columns.ColumnName || c.CustColName == Columns.ColumnName || c.AliasName == Columns.ColumnName)).FirstOrDefault();
                    if (GetColumnInfo != null)
                    {
                        //Getting name of column from particulat class dynamically using reflection
                        PropertyInfo prop = user.GetType().GetProperty(GetColumnInfo.ColumnName, BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite)
                        {
                            Type t = prop.PropertyType;
                            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(Rows[Columns.ColumnName])))
                                    prop.SetValue(user, null, null);
                                else
                                    prop.SetValue(user, Convert.ChangeType(Rows[Columns.ColumnName], t.GetGenericArguments()[0]), null);
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(Rows[Columns.ColumnName])))
                                    prop.SetValue(user, Convert.ChangeType(null, prop.PropertyType), null);
                                else
                                    prop.SetValue(user, Convert.ChangeType(Rows[Columns.ColumnName], prop.PropertyType), null);
                            }
                            //setting value as per the format of object
                        }
                    }
                }
                if (user != null)
                {
                    ICollection<ValidationResult> results = null;
                    var isValid = Validate(user, out results);
                    sImportCust.Add(new Models.combineSucImport { UserData = user, FirstName = user.FirstName, LastName = user.LastName, IsValid = isValid, RowNumber = dt.Rows.IndexOf(Rows) + 1, IsOther = false, IsRepeated = false });
                }
            }
            /*start: check Phone number list in Database*/
            var getCustomerList = sImportCust.Select(c => c.UserData.PhoneNo).ToList();
            var checkCustomer = SPA.CCTSP_User.Where(c => getCustomerList.Contains(c.PhoneNo) && (c.ActiveStatus == "A" || c.ActiveStatus == "L") && c.RoleId == 4).Select(c => c.PhoneNo).ToList();
            /*End*/
            if (checkCustomer.Count != 0)
            {
                //Updating status of Already Existing user
                sImportCust.Where(c => checkCustomer.Contains(c.UserData.PhoneNo)).All(c => { c.IsRepeated = true; return true; });
            }
            //Adding of user to official Table
            var userList = new List<CCTSP_User>();
            userList = sImportCust
                                .Where(c => c.IsValid == true && c.IsRepeated == false)
                                .Select(c => new CCTSP_User
                                {
                                    SchId = schlId,
                                    ActiveStatus = "A",
                                    RoleId = 4,
                                    LoginName = c.UserData.PhoneNo == null ? "" : c.UserData.PhoneNo,
                                    PhoneNo = c.UserData.PhoneNo,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    EmailId = c.UserData.EmailId,
                                    PasswordQuerry2 = c.UserData.PasswordQuerry2,
                                    Password = "Password",
                                    DOB = Convert.ToDateTime(c.UserData.DOB),
                                    Pincode = Convert.ToInt32(c.UserData.Pincode),
                                    VEKA_Number = c.UserData.VEKA_Number,
                                    GLN_No = c.UserData.GLN_No,
                                    ZSR_No = c.UserData.ZSR_No,
                                    AHV_Number = c.UserData.AHV_Number,
                                    InsuranceNumber = c.UserData.InsuranceNumber,
                                    CreatedOn = DateTime.Now,
                                    Updated_Date = DateTime.Now,
                                    SMS_Service = 1,
                                    Email_Service = 3
                                }).ToList();
            if (userList.Count > 0)
            {
                SPA.CCTSP_User.AddRange(userList);
                SPA.SaveChanges();
            }
            if (sImportCust.Count > 0)
            {
                //Download Excel
                var ExlUrl = DownloadErrorExcel(dt, sImportCust);
                sImportCust.All(c => { c.DownloadExcel = ExlUrl; return true; });
            }
            return sImportCust;
        }
        public List<string> GetAllHeaders(string url)
        {
            FileInfo info = new FileInfo(url);
            ExcelPackage package = new ExcelPackage(info);
            ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            List<string> AllColumn = new List<string>();
            List<int> IndexColumns = new List<int>();
            if (workSheet != null)
            {
                int columns = workSheet.Dimension.End.Column;
                int rows = workSheet.Dimension.End.Row;
                if (rows > 0)
                {
                    for (int j = 1; j <= columns; j++)
                    {
                        #region Header
                        if (!string.IsNullOrEmpty(Convert.ToString(workSheet.Cells[1, j].Text).Trim()))
                            AllColumn.Add(Convert.ToString(workSheet.Cells[1, j].Text).Trim());
                        #endregion
                    }
                }
            }
            return AllColumn;
        }
        public string AddUrlofExcel(string url)
        {
            SPA.Database.ExecuteSqlCommand(sql.updateUrlAccordingtoShop(schlId, url));
            return url;
        }
        public List<Models.ImportCustomer> getAllColInfo()
        {
            return SPA.Database.SqlQuery<Models.ImportCustomer>(sql.GetColumns(schlId)).ToList();
        }
        public string DownloadErrorExcel(DataTable dt, List<Models.combineSucImport> sImportCust)
        {
            List<DataRow> dtRows = new List<DataRow>();
            foreach (var item in sImportCust.Where(c => (c.IsValid == true && c.IsRepeated == false)).ToList())
                dtRows.Add(dt.Rows[item.RowNumber - 1]);
            foreach (DataRow dr in dtRows)
                dt.Rows.Remove(dr);
            var fileName = DateTime.Now.ToString("yyyy-MM-dd--hh-mm-ss") + ".xlsx";
            var outputDir = HttpContext.Current.Server.MapPath("/Upload/");
            // Create the file using the FileInfo object
            var file = new FileInfo(outputDir + fileName);
            using (ExcelPackage objExcelPackage = new ExcelPackage(file))
            {
                int i = 1;
                ExcelWorksheet objWorksheet = objExcelPackage.Workbook.Worksheets.Add(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                objWorksheet.Cells.LoadFromDataTable(dt, true);
                objExcelPackage.Save();
            }
            return "/Upload/" + fileName;
        }
        public int getLanguageId(int Lang_id, int? user_Lang)
        {
            return user_Lang != null ? user_Lang.Value : Lang_id;
        }
        public List<Models.CustomerTabInfo> getCustomerList(string OrderBy, int? Sorting, long UserId, string PageName)
        {
            var Desc = "";
            if (string.IsNullOrEmpty(OrderBy))
                OrderBy = "LastVisited";
            if (Sorting == 1)
                Desc = "DESC";
            var CustomerList = "select * from ( " +
                                   "SELECT Distinct(a.UserId),a.UserId as CustomerId, a.FirstName, a.LastName, a.PasswordQuerry2 as LandLineNumber, " +
                                   "a.Emailid, a.Gender, a.Phoneno, a.SchId ,b.ActiveStatus " +
                                   ", c.FirstName as EMPFIRSTNAME, b.updatedOn as LastVisited,g.AccessToData,g.insertAccess,g.DeleteAccess,g.UpdateAccess,d.ItenName as FlowStatus, c.UserId as EMPUserID, c.SchId as EmpSchId " +
                                   "FROM " +
                                   "cctsp_user a " +
                                   "JOIN ctsp_SolutionMaster d ON d.SectionName = '" + PageName + "' and d.ActiveStatus = 'A' " +
                                   /*get Logged in User Detail*/
                                   "JOIN cctsp_User h ON h.UserId = " + UserId + " and h.ActiveStatus = 'A' and h.schId = " + schlId + " " +
                                   "JOIN cctsp_RoleDetails g ON g.RoleId = h.RoleId AND g.ActiveStatus = 'A' " +
                                   "AND d.SolutionId = CONVERT(BIGINT, g.AccesstoSub) " +
                                   "AND((g.AccessToData = 'Own' AND h.UserId = " + UserId + ") OR(g.AccessToData != 'Own')) " +
                                   "LEFT JOIN spa_employeeScheduler b ON b.CLIENT_UserId = a.UserId and b.schId = " + schlId + " " +
                                   "AND b.EmpSchDetailsId = " +
                                   " ( select tblInner.EmpSchDetailsId from " +
                                   " ( " +
                                   "SELECT top 1MAX(updatedOn) as updatedOn, EmpSchDetailsId " +
                                   "FROM spa_employeeScheduler " +
                                   "WHERE CLIENT_UserId = a.UserId AND  ((activeStatus='A' and BookedStatus='B') " +
                                   " or (activeStatus = 'DA' and BookedStatus = 'B') " +
                                   " or (activeStatus = 'C' and BookedStatus = 'C')) and schId = " + schlId + " " +
                                   " and ((g.AccessToData = 'Own' AND EMP_UserId = " + UserId + ") OR (g.AccessToData != 'Own')) " +
                                   "group by updatedOn,EmpSchDetailsId,BookedStatus" +
                                   " order by BookedStatus Desc, updatedon desc " +
                                   ") tblInner " +
                                   " ) " +
                                   "LEFT JOIN cctsp_user c ON c.UserId = b.EMP_UserId " +
                                   "WHERE a.Activestatus = 'A' AND a.RoleId = 4 " +
                                   ") TBL " +
                                   "where( " +
                                   "( " +
                                   "TBL.AccessToData = 'own' and TBL.EMPUserID is not null and TBL.EMPSchId = " + schlId + " " +
                                   " ) or " +
                                   "( " +
                                   "TBL.AccessToData != 'own' and(TBL.EMPSchId = " + schlId + " or TBL.SchId = " + schlId + ")  " +
                                   " ) " +
                                   " ) " +
                                   " Order by " + OrderBy + " " + Desc;
            return SPA.Database.SqlQuery<Models.CustomerTabInfo>(CustomerList).ToList();
        }
        public void setRedirectionofCalendar(string View, string AllView, string Date, long? EmpUserId)
        {
            if (AllView == "All" && AllView != null && View == "Day")
            {
                HttpContext.Current.Session["AllView"] = "All";
            }
            if (View != null && View != "")
            {
                HttpContext.Current.Session["View"] = View;
                HttpContext.Current.Session["ViewUserId"] = Convert.ToString(EmpUserId);
            }
            if (Date != null && Date != "")
            {
                var BDate = Convert.ToDateTime(Date, enGB);
                HttpContext.Current.Session["ViewDate"] = BDate.Day + "-" + BDate.AddMonths(-1).ToString("MM") + "-" + BDate.Year;
            }
        }
        public Models.getAccess CheckAccessofPage(string PageName, long UserId)
        {
            var Result = SPA.Database.SqlQuery<Models.getAccess>(sql.getAccess(PageName, schlId, UserId)).FirstOrDefault();
            if (Result == null)
                Result = new Models.getAccess();
            return Result;
        }
        public Models.CheckInvoiceStatus CheckInvoiceStatus(long ReservationId)
        {
            var Result = SPA.Database.SqlQuery<Models.CheckInvoiceStatus>(sql.CheckInvoiceStaus(ReservationId)).FirstOrDefault();
            if (Result == null)
                Result = new Models.CheckInvoiceStatus();
            return Result;
        }
        public bool CheckTabAccess(string TabName, long FlowId)
        {
            bool Status = false;
            int Count = SPA.Database.SqlQuery<int>(sql.checkTabAccess(TabName, FlowId)).FirstOrDefault();
            if (Count > 0)
                Status = true;
            return Status;
        }
        public bool CheckSubTabAccess(string SubTabName, long FlowId)
        {
            bool Status = false;
            int Count = SPA.Database.SqlQuery<int>(sql.checkSubTabAccess(SubTabName, FlowId)).FirstOrDefault();
            if (Count > 0)
                Status = true;
            return Status;
        }
        //Assign Product Dynamically
        public bool AssignProductToEmp(List<long> UserId, List<long> catgTypeId, DateTime createdDate)
        {
            try
            {
                //get Product Info
                var data = SPA.CTSP_SolutionMaster
                           .Where(c => c.Activestatus == "A"
                           && c.SchId == schlId && c.SectionName == "ChoosProduct"
                           && catgTypeId.Contains(c.CatgTypeId))
                           .Select(c => new Models.productDetail
                           {
                               Duration = c.Duration,
                               Amount = c.Amount,
                               SectionDesc = c.CCTSP_CategoryDetails.CatgDesc,
                               Image = c.SectionDesc,
                               ItenName = c.CCTSP_CategoryDetails.Gender
                           }).ToList();
                //Assign Product to Employee
                foreach (var item in UserId)
                {
                    var AssignedProduct = data.Select(c => new CTSP_SolutionMaster
                    {
                        Activestatus = "A",
                        UserId = item,
                        SectionName = "EmployeeProduct",
                        CatgTypeId = 11145,
                        SchId = schlId,
                        CraetedOn = createdDate,
                        SectionDesc = c.SectionDesc,
                        Image = c.Image,
                        Amount = c.Amount,
                        Duration = c.Duration,
                        ItenName = c.ItenName
                    }).ToList();
                    SPA.CTSP_SolutionMaster.AddRange(AssignedProduct);
                }
                SPA.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorSend("AssignProductToEmp", "Function", ex);
                return false;
            }
            return true;
        }
        public bool AssignProductToEmp(long UserId, DateTime createdDate)
        {
            try
            {
                List<long> catgType = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.DomainType == schlId && c.CCTSP_CategoryMaster.CatgName == "SPA Products").Select(c => c.CatgTypeId).ToList();
                return AssignProductToEmp(new List<long> { UserId }, catgType, createdDate);
            }
            catch (Exception ex)
            {
                ErrorSend("AssignProductToEmp", "Function", ex);
                return false;
            }
        }
        public bool AssignProductToEmp(List<long> catgType, DateTime createdDate)
        {
            try
            {
                var UserInfo = SPA.CCTSP_User.Where(c => c.RoleId != 1 && c.RoleId != 4 && c.ActiveStatus == "A" && c.SchId == schlId).Select(c => c.UserId).ToList();
                return AssignProductToEmp(UserInfo, catgType, createdDate);
            }
            catch (Exception ex)
            {
                ErrorSend("AssignProductToEmp", "Function", ex);
                return false;
            }
        }
        public int? getLangId()
        {
            return SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault();
        }
        public Models.shopMaster ShopInfo()
        {
            return SPA.CCTSP_SchoolMaster.Where(C => C.SchlId == schlId).Select(c => new Models.shopMaster { LangId = c.Lang_id, SchlStudentStrength = c.SchlStudentStrength, Display_Invoice = c.Display_Invoice, Flow_Id = c.Flow_Id, Allow_PastDate = c.AllOW_PastDate,TimeZone=c.TimeZone }).FirstOrDefault();
        }
        public bool CheckPastDate(string TimeZone, DateTime BookingDate)
        {
            bool CheckStatus = false;
            DateTime CurrentDate = ZonalDate(TimeZone);
            if (BookingDate <= CurrentDate)
                CheckStatus = true;
            return CheckStatus;
        }
        public Models.ReservationInfo ReservationInfo(long Schlid,int ReservationId)
        {            
             return SPA.Database.SqlQuery<Models.ReservationInfo>(sql.RegistrationDetails(Schlid,ReservationId)).FirstOrDefault();
        }

    }
}
