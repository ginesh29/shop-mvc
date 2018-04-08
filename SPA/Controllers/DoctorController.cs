using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.IO;
using SPA.Common;

namespace SPA.Controllers
{
    [checkShop]
    public class DoctorController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        Function fu = new Function();
        PushEmail Email = new PushEmail();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        JavaScriptSerializer js = new JavaScriptSerializer();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public DoctorController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        [CustomAutohrize(null)]
        public ActionResult DocPre(long Reservationid, int diff, string Url)
        {
            try
            {
                if (diff > 3)                                  
                    return RedirectToAction("Notes", new { Reservationid = Reservationid, Url = Url, diff= diff });
                int Langid = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.doctorPageLang = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Doctor" && c.ActiveStatus == "A" && c.Lang_id == Langid).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id
                }).ToList();
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 13 && c.Lang_id == Langid).Select(c => c.Value).FirstOrDefault();
                int CheckCount = SPA.SPA_EmployeeScheduler.Where(c => ((c.ActiveStatus.Trim() == "A" && c.BookedStatus.Trim() == "B")||(c.ActiveStatus.Trim()=="C" && c.BookedStatus.Trim()=="C")) && c.SchId == schlId && c.EmpSchDetailsId == Reservationid).Count();
                if (CheckCount > 0)
                {
                    var CatgIdList = "130,131,132,133,134,135,136,137,139";
                    ViewBag.PrintPagesetUp = fu.PrintSetup();
                    Session["Medicments"] = new List<Models.SpecialInsertForDoctor>();
                    Session["CountSpecial"] = 0;
                    int Count = SPA.Prescription_Detail.Where(c => c.BookingId == Reservationid && c.ActiveStatus == "A" && c.Prescription_Master.Diff == diff).Count();
                    ViewBag.Count = Count;
                    if (Count > 0)
                    {
                        var MedicationDisplay = fu.CommonAdvice(Reservationid, diff, "130");
                        Session["Medicments"] = MedicationDisplay;
                        Session["CountSpecial"] = MedicationDisplay.Count;
                    }
                    var PatientDetails = fu.CommonPatientDetails(Reservationid);
                    ViewBag.PatientData = PatientDetails;
                    //Gender
                    ViewBag.Gender = fu.ChangeLangGender(Langid, PatientDetails.Gender);
                    ViewBag.MedicationList = fu.getCatgDetails(CatgIdList);
                    ViewBag.Logo = fu.ImageLogoChange(PatientDetails.ShopImg);
                    ViewBag.Date = fu.ZonalDate(null);
                    ViewBag.Url = Url;
                    return View("EditDoctorPrescriptions");
                }
                else
                    return Redirect("/Reservation/Reservation#" + Url);
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
        public ActionResult DoctorInvoiceDisplay(long ReservationId, string Url)
        {
            //string DoctorInvoice = "select a.EmpSchDetailsId,a.Product_Price , a.comment,b.UserId,b.FirstName,b.LastName,b.DOB,b.EmailId,d.Lang_id,b.Gender,c.FirstName as EmpFirstName,C.LastName as EmpLastName ,d.SchlAddress as Address,d.ImageLogo as ShopImg ,CASE WHEN  datediff(year, b.DOB, getdate()) < 180 THEN datediff(year, b.DOB, getdate()) ELSE 0 END as Age ,e.catgDesc as Productname from SPA_EmployeeScheduler a join cctsp_user b on b.UserId = a.Client_UserId join cctsp_user c on c.UserId = a.Emp_UserId  join cctsp_schoolmaster d on d.schlid = a.schid join cctsp_CategoryDetails e on e.CatgTypeId = a.Product_Id  where a.schid = " + schlId+" and b.schid = "+schlId+" and c.schid = "+schlId+" and a.EmpSchDetailsId = "+ ReservationId + " and e.DomainType = "+schlId+" and d.ActiveStatus = 'A' and e.ActiveStatus = 'A'";
            string DoctorInvoice = "select a.EmpSchDetailsId,a.Product_Price , a.comment,b.UserId,b.FirstName,b.LastName,b.DOB,b.EmailId,d.Lang_id,b.Gender,c.FirstName as EmpFirstName,C.LastName as EmpLastName ,d.SchlAddress as Address,d.ImageLogo as ShopImg ,CASE WHEN  datediff(year, b.DOB, getdate()) < 180 THEN datediff(year, b.DOB, getdate()) ELSE 0 END as Age ,e.catgDesc as Productname from SPA_EmployeeScheduler a join cctsp_user b on b.UserId = a.Client_UserId join cctsp_user c on c.UserId = a.Emp_UserId  join cctsp_schoolmaster d on d.schlid = a.schid join cctsp_CategoryDetails e on e.CatgTypeId = a.Product_Id  where a.schid = " + schlId + "  and c.schid = " + schlId + " and a.EmpSchDetailsId = " + ReservationId + " and e.DomainType = " + schlId + " and d.ActiveStatus = 'A' and e.ActiveStatus = 'A'";
            var DoctorInvoiceDisplay = SPA.Database.SqlQuery<Models.DoctorPagesDisplay>(DoctorInvoice).FirstOrDefault();
            ViewBag.DoctorInvoiceDisplay = DoctorInvoiceDisplay;
            DoctorInvoiceDisplay.Gender = fu.ChangeLangGender(1, DoctorInvoiceDisplay.Gender);
            DoctorInvoiceDisplay.ShopImg = fu.LogoImg(DoctorInvoiceDisplay.ShopImg);
            DoctorInvoiceDisplay.created_on = fu.ZonalDate(null);
            ViewBag.language = fu.getLanguageData("InvoiceDisplay_page", 0, DoctorInvoiceDisplay.Lang_id.Value);
            ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 17 && c.Lang_id == DoctorInvoiceDisplay.Lang_id.Value).Select(c => c.Value).FirstOrDefault();
            ViewBag.Url = Url;
            return View(DoctorInvoiceDisplay);
        }

        public ActionResult EmailPrescriptions(long Reservationid)
        {
            try
            {
                //Real Data Generation
                var EmailPrescriptionsDisplay = fu.CommonPreDocDisplay(Reservationid, 3, 0);
                //Logo Path Generation
                ViewBag.Logo = fu.ImageLogoChange(EmailPrescriptionsDisplay.Select(c => c.ShopImg).FirstOrDefault());
                //School Info
                var Lang = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault();
                //Language
                ViewBag.Language = fu.getLanguageData("Doctor", 0, Lang.Value);
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 14 && c.Lang_id == Lang.Value).Select(c => c.Value).FirstOrDefault();
                //Gender                
                ViewBag.Gender = fu.ChangeLangGender(Lang.Value, EmailPrescriptionsDisplay.Select(c => c.Gender).FirstOrDefault());
                ViewBag.Date = fu.ZonalDate(null);
                return View(EmailPrescriptionsDisplay);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult EmailDoctorPrescriptions(long Reservationid)
        {
            try
            {
                var MergeDocPresc = fu.CommonPreDocDisplay(Reservationid, 3, 0);
                ViewBag.Logo = fu.ImageLogoChange(MergeDocPresc.Select(c => c.ShopImg).FirstOrDefault());
                int LangId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Language = fu.getLanguageData("Doctor", 0, LangId);
                //Gender
                ViewBag.Gender = fu.ChangeLangGender(LangId, MergeDocPresc.Select(c => c.Gender).FirstOrDefault());
                return View(MergeDocPresc);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult DoctorPrescriptionsDisplay(long Reservationid, int diff)
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
               
                if (diff > 3)
                {
                    var Link = "/PDF/NotesDisplay";
                    var DynamicLink = "";
                    DynamicLink = SPA.CCTSP_CategoryDetails.Where(c => c.Group_orderdata == diff && c.ActiveStatus == "A" && c.CatgId == 188).Select(c => c.Gender).FirstOrDefault();
                    DynamicLink = !string.IsNullOrEmpty(DynamicLink) ? DynamicLink : Link;
                    return Redirect(DynamicLink + "?Reservationid=" + Reservationid + "&IsPrint=false&Display=D&schlId=" + schlId + "&Url=" + Url + "&UserId=" + UserId + "");
                }                   
                //if (diff > 3)
                //    return RedirectToAction("NotesDisplay", "PDF", new { Reservationid = Reservationid, IsPrint = false, Display = "D", schlId = schlId, UserId = UserId });
                ViewBag.PrintPageSetUp = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 144 && c.ActiveStatus == "A" && c.DomainType == schlId).Select(c => c.CatgDesc).ToList();
                var MergeDocPresc = fu.CommonPreDocDisplay(Reservationid, diff, 1);
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                              .Select(c => new Models.ShopMasterDetail { TimeZone = c.TimeZone, Lang_id = c.Lang_id }).FirstOrDefault();
                int Langid = ShopInfo.Lang_id.Value;
                ViewBag.doctorPageLang = fu.getLanguageData("Doctor", null, Langid);
                //Gender
                ViewBag.Gender = fu.ChangeLangGender(Langid, MergeDocPresc.Select(c => c.Gender).FirstOrDefault());
                ViewBag.Date = fu.ZonalDate(ShopInfo.TimeZone);
                ViewBag.Logo = fu.ImageLogoChange(MergeDocPresc.Select(c => c.ShopImg).FirstOrDefault());
                return View(MergeDocPresc);

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult DoctorInvoice()
        {
            return View();
        }
        [ValidateInput(false)]
        //Used for Prescription Add and Prescription Edit
        public void AddDoctorNotes(FormCollection frmDoctor)
        {
            Models.DoctorNotes Doctor = new Models.DoctorNotes();
            List<long> DList = new List<long>();
            List<string> DSLIST = new List<string>();
            List<long> PreDiagnosisLIST = new List<long>();
            try
            {
                //All SelectList in view
                foreach (var item in frmDoctor.AllKeys.Where(c => c.Contains("patientFind_") || c.Contains("DiagnosisFind_") || c.Contains("InvestigationFind_") || c.Contains("AllergiesFind_")).ToList())
                {
                    DList.Add(long.Parse(item.Replace("patientFind_", "").Replace("DiagnosisFind_", "").Replace("InvestigationFind_", "").Replace("AllergiesFind_", "")));
                }
                var PresDiagnosis = frmDoctor.AllKeys.Where(c => c.Contains("Prescription_")).ToList();
                foreach (var Diagnosisid in PresDiagnosis)
                {
                    PreDiagnosisLIST.Add(long.Parse(Diagnosisid.Replace("Prescription_", "")));
                }
                //Get all CatgTypeId Details
                var CatgIdDetails = (from a in SPA.CCTSP_CategoryDetails where a.ActiveStatus == "A" && DList.Contains(a.CatgTypeId) select a).ToList();
                //All Free Text from Prescription and DoctorNotes
                #region FreeText
                Doctor.specialMedication = new List<Models.SpecialInsertForDoctor>();
                Doctor.AdviceList = new List<Models.SpecialInsertForDoctor>();
                if (!string.IsNullOrEmpty(frmDoctor["textareaPatient"]))
                    Doctor.FreePatient = frmDoctor["textareaPatient"];
                if (!string.IsNullOrEmpty(frmDoctor["textareaDiagnosis"]))
                    Doctor.DiagFree = frmDoctor["textareaDiagnosis"];
                Doctor.diff = Convert.ToInt32(frmDoctor["Diff"]);
                if (Convert.ToInt32(Session["CountSpecial"]) > 0 && (Doctor.diff == 1 || Doctor.diff == 3))
                {
                    Doctor.specialMedication = (List<Models.SpecialInsertForDoctor>)Session["Medicments"];
                    Session["Medicments"] = new List<Models.SpecialInsertForDoctor>();
                    Session["CountSpecial"] = 0;
                }
                if (Convert.ToInt32(Session["CountAdvice"]) > 0 && Doctor.diff == 2 || Doctor.diff == 3)
                {
                    Doctor.AdviceList = (List<Models.SpecialInsertForDoctor>)Session["Advice"];
                    Session["Advice"] = new List<Models.SpecialInsertForDoctor>();
                    Session["CountAdvice"] = 0;
                }
                if (!string.IsNullOrEmpty(frmDoctor["textareaMedicamentDetails"]))
                    Doctor.FreeMedication = frmDoctor["textareaMedicamentDetails"];
                if (!string.IsNullOrEmpty(Session["UploadImagePath"].ToString()))
                    Doctor.UploadImageList = (List<string>)Session["UploadImagePath"];
                if (!string.IsNullOrEmpty(frmDoctor["textareaAllergies"]))
                    Doctor.FreeAllergies = frmDoctor["textareaAllergies"];
                if (!string.IsNullOrEmpty(frmDoctor["textareaPreMedicine"]))
                    Doctor.FreepreDiag = frmDoctor["textareaPreMedicine"];

                if (!string.IsNullOrEmpty(frmDoctor["textareaAdvise"]))
                    Doctor.FreeAdvise = frmDoctor["textareaAdvise"];
                if (!string.IsNullOrEmpty(frmDoctor["textareaFollowUp"]))
                    Doctor.FreeFollowUp = frmDoctor["textareaFollowUp"];
                if (!string.IsNullOrEmpty(frmDoctor["Tempreature"]))
                    Doctor.Temp = frmDoctor["Tempreature"];
                if (!string.IsNullOrEmpty(frmDoctor["Pulse"]))
                    Doctor.pulse = frmDoctor["Pulse"];
                if (!string.IsNullOrEmpty(frmDoctor["BloodPressure"]))
                    Doctor.BP = frmDoctor["BloodPressure"];
                if (!string.IsNullOrEmpty(frmDoctor["RespiratoryRate"]))
                    Doctor.RepositoryRate = frmDoctor["RespiratoryRate"];
                #endregion
                Doctor.UserId = Convert.ToInt64(frmDoctor["Userid"]);
                Doctor.BookingId = Convert.ToInt32(frmDoctor["BookingId"]);

                if (Doctor != null)
                {
                    //Initial                
                    var RegionalDate = fu.ZonalDate(null);
                    Prescription_Detail DetailPre = null;
                    List<Prescription_Detail> DetailPreList = new List<Prescription_Detail>();
                    //Check Whether Prescription Id Exist
                    var checkUserDetails = SPA.Prescription_Master.Where(c => c.UserId == Doctor.UserId && c.ActiveStatus == "A" && c.CCTSP_User.SchId == schlId && c.Diff == Doctor.diff && c.BookingId == Doctor.BookingId).FirstOrDefault();
                    //Add PrescriptionMaster only if before it is not added
                    #region AddMasterPrescription
                    if (checkUserDetails == null)
                    {
                        checkUserDetails = new Prescription_Master();
                        checkUserDetails.ActiveStatus = "A";
                        checkUserDetails.UserId = Doctor.UserId;
                        checkUserDetails.created_on = RegionalDate;
                        checkUserDetails.BookingId = Doctor.BookingId;
                        checkUserDetails.Diff = Doctor.diff;
                        checkUserDetails.SchId = schlId;
                        SPA.Prescription_Master.Add(checkUserDetails);
                        SPA.SaveChanges();
                        //checkUserDetails = PreMaster;
                    }
                    var DeleteQuery = "update Prescription_Detail set ActiveStatus='D' where ActiveStatus='A' and Prescription_Id=" + checkUserDetails.Prescription_Id;
                    var DeletePreDetails = DeleteQuery + "  " + "update Medicine_Master set ActiveStatus='D' where  ActiveStatus='A' and Prescription_Id=" + checkUserDetails.Prescription_Id;
                    SPA.Database.ExecuteSqlCommand(DeletePreDetails);
                    #endregion
                    //Add Details of Prescription
                    #region AddDetailsofPrescription
                    if (checkUserDetails != null)
                    {
                        //Add Automatic Details
                        #region Add Automatic Details
                        //Add All Automatic Details for Patient Medication and Investigation
                        foreach (var item in CatgIdDetails)
                        {
                            DetailPre = new Prescription_Detail();
                            DetailPre.ActiveStatus = "A";
                            long CatgId = item.CatgId;
                            DetailPre.created_on = RegionalDate;
                            DetailPre.UserId = checkUserDetails.UserId;
                            DetailPre.CatgId = CatgId;
                            DetailPre.CatgTypeId = item.CatgTypeId;
                            DetailPre.Prescription_Id = checkUserDetails.Prescription_Id;
                            DetailPre.BookingId = checkUserDetails.BookingId;
                            DetailPre.ActiveStatus = "A";
                            DetailPre.SchId = schlId;
                            DetailPreList.Add(DetailPre);
                        }
                        foreach (var item in PreDiagnosisLIST)
                        {
                            DetailPre = new Prescription_Detail();
                            DetailPre.ActiveStatus = "A";
                            long CatgId = 137;
                            DetailPre.created_on = RegionalDate;
                            DetailPre.UserId = checkUserDetails.UserId;
                            DetailPre.CatgId = CatgId;
                            DetailPre.CatgTypeId = item;
                            DetailPre.Prescription_Id = checkUserDetails.Prescription_Id;
                            DetailPre.BookingId = checkUserDetails.BookingId;
                            DetailPre.ActiveStatus = "A";
                            DetailPre.SchId = schlId;
                            DetailPreList.Add(DetailPre);
                        }
                        List<Medicine_Master> MediList = new List<Medicine_Master>();
                        foreach (var item in Doctor.specialMedication)
                        {
                            Medicine_Master spMedication = new Medicine_Master();
                            spMedication.Prescription_Id = checkUserDetails.Prescription_Id;
                            spMedication.UserId = checkUserDetails.UserId;
                            spMedication.created_on = RegionalDate;
                            spMedication.BookingId = checkUserDetails.BookingId;
                            spMedication.field1 = item.Medicine;
                            spMedication.NtimesDay = item.HowManyTimes;
                            spMedication.Times = item.When;
                            spMedication.field3 = "130";
                            spMedication.Remarks = item.FreeText;
                            spMedication.SchId = schlId;
                            spMedication.ActiveStatus = "A";
                            SPA.Medicine_Master.Add(spMedication);
                        }
                        foreach (var item in Doctor.AdviceList)
                        {
                            Medicine_Master spMedication = new Medicine_Master();
                            spMedication.Prescription_Id = checkUserDetails.Prescription_Id;
                            spMedication.UserId = checkUserDetails.UserId;
                            spMedication.created_on = RegionalDate;
                            spMedication.BookingId = checkUserDetails.BookingId;
                            spMedication.field1 = item.Medicine;
                            spMedication.NtimesDay = item.HowManyTimes;
                            spMedication.Times = item.When;
                            spMedication.field3 = "138";
                            spMedication.Remarks = item.FreeText;
                            spMedication.SchId = schlId;
                            spMedication.ActiveStatus = "A";
                            SPA.Medicine_Master.Add(spMedication);
                        }
                        #endregion
                        //Add Free Details
                        #region Add Free Text
                        //Add Patient
                        if (!string.IsNullOrEmpty(Doctor.FreePatient))
                        {
                            DetailPre = fu.AddFreePrescription(131, RegionalDate, checkUserDetails, Doctor.FreePatient);
                            DetailPreList.Add(DetailPre);
                        }
                        if (!string.IsNullOrEmpty(Doctor.FreeAllergies))
                        {
                            DetailPre = fu.AddFreePrescription(136, RegionalDate, checkUserDetails, Doctor.FreeAllergies);
                            DetailPreList.Add(DetailPre);
                        }
                        //Add Medication
                        if (!string.IsNullOrEmpty(Doctor.FreeMedication))
                        {
                            DetailPre = fu.AddFreePrescription(130, RegionalDate, checkUserDetails, Doctor.FreeMedication);
                            DetailPreList.Add(DetailPre);
                        }
                        if (!string.IsNullOrEmpty(Doctor.FreeAdvise))
                        {
                            DetailPre = fu.AddFreePrescription(138, RegionalDate, checkUserDetails, Doctor.FreeAdvise);
                            DetailPreList.Add(DetailPre);
                        }
                        //Add Diagnosis
                        if (!string.IsNullOrEmpty(Doctor.DiagFree))
                        {
                            DetailPre = fu.AddFreePrescription(132, RegionalDate, checkUserDetails, Doctor.DiagFree);
                            DetailPreList.Add(DetailPre);
                        }

                        if (!string.IsNullOrEmpty(Doctor.FreepreDiag))
                        {
                            DetailPre = fu.AddFreePrescription(137, RegionalDate, checkUserDetails, Doctor.FreepreDiag);
                            DetailPreList.Add(DetailPre);
                        }

                        if (!string.IsNullOrEmpty(Doctor.FreeFollowUp))
                        {
                            DetailPre = fu.AddFreePrescription(139, RegionalDate, checkUserDetails, Doctor.FreeFollowUp);
                            DetailPreList.Add(DetailPre);
                        }
                        if (!string.IsNullOrEmpty(Doctor.Temp))
                        {
                            DetailPre = fu.AddFreePrescription(140, RegionalDate, checkUserDetails, Doctor.Temp);
                            DetailPreList.Add(DetailPre);
                        }
                        if (!string.IsNullOrEmpty(Doctor.pulse))
                        {
                            DetailPre = fu.AddFreePrescription(141, RegionalDate, checkUserDetails, Doctor.pulse);
                            DetailPreList.Add(DetailPre);
                        }
                        if (!string.IsNullOrEmpty(Doctor.BP))
                        {
                            DetailPre = fu.AddFreePrescription(142, RegionalDate, checkUserDetails, Doctor.BP);
                            DetailPreList.Add(DetailPre);
                        }
                        if (!string.IsNullOrEmpty(Doctor.RepositoryRate))
                        {
                            DetailPre = fu.AddFreePrescription(143, RegionalDate, checkUserDetails, Doctor.RepositoryRate);
                            DetailPreList.Add(DetailPre);
                        }
                        #endregion
                        SPA.Prescription_Detail.AddRange(DetailPreList);
                        SPA.SaveChanges();
                    }
                    #endregion                 
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void AddPrintPageSetup(string getallUncheck)
        {
            try
            {
                var DeleteQuery = "update cctsp_categoryDetails set ActiveStatus='D' where catgid=144  and DomainType=" + schlId;
                SPA.Database.ExecuteSqlCommand(DeleteQuery);
                if (getallUncheck != "")
                {
                    List<long> CatgList = getallUncheck.Split(',').Select(c => long.Parse(c)).ToList();
                    foreach (var item in CatgList)
                    {
                        fu.AddCategoriesDetail("144", item.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void MailPrescriptionCard(long? Reservationid, string EmailID, int Diff)
        {
            try
            {
                int LangId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
                var LinkName = System.Web.HttpContext.Current.Request.Url.Authority;
                var Link = "/Doctor/EmailPrescriptions?Reservationid=" + Reservationid;
                if (Diff == 3)
                    Link = "/Doctor/EmailPrescriptions?Reservationid=" + Reservationid;
                if (!LinkName.Contains("localhost") && LinkName != "" && LinkName != null)
                    Link = "http://" + LinkName + Link;
                else
                    Link = "http://tshope.azurewebsites.net" + Link;
                var Intial = "Reservationid=" + Reservationid;
                Email.EmailWithPdf(EmailID, schlId, Link, "Prescription.pdf", Intial, LangId, Reservationid);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }

        public JObject AddCategories(string catgName, string catgId)
        {
            return JObject.Parse(js.Serialize(fu.AddCategoriesDetail(catgId, catgName)));
        }
        public ActionResult ImageUpload(HttpPostedFileBase file, string filename)
        {
            try
            {
                if (file.FileName != "")
                {
                    var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                    var ImgPath = fu.CommonImageUpload(file, supportedTypes);
                    if (ImgPath != "Only image" && ImgPath != "File Size Not Valid")
                    {
                        List<string> ImagList = new List<string>();
                        if (!string.IsNullOrEmpty(Session["UploadImagePath"].ToString()))
                        {
                            ImagList = (List<string>)Session["UploadImagePath"];
                            ImagList.Add(ImgPath);
                            Session["UploadImagePath"] = ImagList;
                        }
                        else
                        {
                            ImagList.Add(ImgPath);
                            Session["UploadImagePath"] = ImagList;
                        }
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
        public void RemoveImage(string file)
        {
            try
            {
                List<string> ImagList = new List<string>();
                if (!string.IsNullOrEmpty(Session["UploadImagePath"].ToString()))
                {
                    ImagList = (List<string>)Session["UploadImagePath"];
                    ImagList.Remove("/Upload/" + file);
                    Session["UploadImagePath"] = ImagList;
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public int TempMedicamentsdata(Models.SpecialInsertForDoctor MedicationDetail)
        {
            try
            {
                List<Models.SpecialInsertForDoctor> MediList = new List<Models.SpecialInsertForDoctor>();
                var Count = Convert.ToInt16(Session[MedicationDetail.CountSpecial]) + 1;
                MedicationDetail.SpId = Count;
                if (Convert.ToInt16(Session[MedicationDetail.CountSpecial]) == 0)
                {
                    MediList.Add(MedicationDetail);
                    Session[MedicationDetail.AddPreDetails] = MediList;
                    Session[MedicationDetail.CountSpecial] = Count;
                }
                else
                {
                    MediList = (List<Models.SpecialInsertForDoctor>)Session[MedicationDetail.AddPreDetails];
                    if (MediList.Where(c => c.Medicine == MedicationDetail.Medicine).Count() == 0)
                    {
                        MediList.Add(MedicationDetail);
                        Session[MedicationDetail.AddPreDetails] = MediList;
                        Session[MedicationDetail.CountSpecial] = Count;
                    }
                    else
                        MedicationDetail.SpId = 0;
                }
                return MedicationDetail.SpId;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return 0;
            }

        }
        public void DeleteMedication(int? id, string SessionName, string CountName)
        {
            try
            {
                List<Models.SpecialInsertForDoctor> spList = new List<Models.SpecialInsertForDoctor>();
                if (Convert.ToInt16(Session[CountName]) > 0)
                {
                    spList = (List<Models.SpecialInsertForDoctor>)Session[SessionName];
                    spList.RemoveAll(c => c.SpId == id);
                    Session[SessionName] = spList;
                    if (spList.Count == 0)
                        Session[CountName] = 0;
                    else
                        Session[CountName] = spList.Select(c => c.SpId).Max();
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        [CustomAutohrize(null)]
        public ActionResult PrescriptionCard(long Reservationid, string Url)
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var PrescriptionDisplay = fu.CommonPreDocDisplay(Reservationid, 3, 0);
                int Langid = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.Logo = fu.ImageLogoChange(PrescriptionDisplay.Select(c => c.ShopImg).FirstOrDefault());
                ViewBag.doctorPageLang = fu.getLanguageData("Doctor", null, Langid);
                ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 14 && c.Lang_id == Langid).Select(c => c.Value).FirstOrDefault();
                //Gender                 
                ViewBag.Gender = fu.ChangeLangGender(Langid, PrescriptionDisplay.Select(c => c.Gender).FirstOrDefault());
                ViewBag.Date = fu.ZonalDate(null);
                ViewBag.Medicaments = fu.CommonAdvice(Reservationid, 3, "130");
                ViewBag.ReservationId = Reservationid;
                bool PopupStatus = true;
                if (PrescriptionDisplay.Where(c => c.ActiveStatus != null).Select(c => c.ActiveStatus.Trim()).FirstOrDefault() == "C")
                    PopupStatus = false;
                ViewBag.CheckPopupStatus = PopupStatus;
                ViewBag.Url = Url;
                return View(PrescriptionDisplay);
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
        public ActionResult DoctorNotesDisplay(long Reservationid)
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var DoctorDisplay = fu.CommonPreDocDisplay(Reservationid, 1, 1);
                int Langid = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault().Value;
                ViewBag.doctorPageLang = fu.getLanguageData("Doctor", null, Langid);
                //Gender
                ViewBag.Gender = fu.ChangeLangGender(Langid, DoctorDisplay.Select(c => c.Gender).FirstOrDefault());
                return View(DoctorDisplay);
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
        public void DeleteSelectedItem(long? id)
        {
            try
            {
                var Delete = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == id).FirstOrDefault();
                Delete.ActiveStatus = "D";
                SPA.SaveChanges();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public JObject EditDoctorNotes(long ReservationId, int Diff)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                string EditDoctorNote = "select a.CatgTypeId,b.CatgDesc,a.CatgId,c.FirstName,c.LastName,c.Gender,c.DOB  ,f.FirstName as EmpFirstName,f.LastName from Prescription_Detail a, cctsp_categoryDetails b,cctsp_User c ,Prescription_Master d ,SPA_EmployeeScheduler e , cctsp_User f  where b.CatgTypeId = a.CatgTypeId and a.Prescription_id=d.Prescription_id and a.BookingId = " + ReservationId + " and a.ActiveStatus = 'A' and b.ActiveStatus = 'A' and c.UserId = a.UserId and a.schid=" + schlId + " and b.DomainType=" + schlId + " and d.ActiveStatus='A' and d.schid=" + schlId + " and d.Diff=" + Diff + " and c.UserId=a.UserId and c.schid=" + schlId + " and e.Schid=" + schlId + " and f.schid=" + schlId + " and e.EmpSchDetailsId = " + ReservationId + " and e.Emp_UserId = f.UserId ";
                var EditDoctorDisplay = SPA.Database.SqlQuery<Models.DoctorPagesDisplay>(EditDoctorNote).ToList();
                var Remarks = (from c in SPA.Prescription_Detail where c.ActiveStatus == "A" && c.BookingId == ReservationId && (c.Type == Diff) && c.Prescription_Master.Diff == Diff && c.SchId == schlId select new Models.DoctorPagesDisplay { CatgId = c.CatgId.Value, CatgDesc = c.Remarks }).ToList();
                string CatgTypeId = string.Join(",#", EditDoctorDisplay.Where(c => c.CatgId != 137).Select(c => c.CatgTypeId).ToList());
                string PrescTypeId = "#PreDiagnosis_" + string.Join(",#PreDiagnosis_", EditDoctorDisplay.Where(c => c.CatgId == 137).Select(c => c.CatgTypeId).ToList());
                Models.DoctorNotes Doctor = new Models.DoctorNotes()
                {
                    CatgIDPatient = CatgTypeId,
                    PreDiagnosisId = PrescTypeId,
                    FreePatient = Remarks.Where(c => c.CatgId == 131).Select(c => c.CatgDesc).FirstOrDefault(),
                    DiagFree = Remarks.Where(c => c.CatgId == 132).Select(c => c.CatgDesc).FirstOrDefault(),
                    FreeMedication = Remarks.Where(c => c.CatgId == 130).Select(c => c.CatgDesc).FirstOrDefault(),
                    Temp = Remarks.Where(c => c.CatgId == 140).Select(c => c.CatgDesc).FirstOrDefault(),
                    pulse = Remarks.Where(c => c.CatgId == 141).Select(c => c.CatgDesc).FirstOrDefault(),
                    BP = Remarks.Where(c => c.CatgId == 142).Select(c => c.CatgDesc).FirstOrDefault(),
                    RepositoryRate = Remarks.Where(c => c.CatgId == 143).Select(c => c.CatgDesc).FirstOrDefault(),
                    diff = Diff,
                    FreeAllergies = Remarks.Where(c => c.CatgId == 136).Select(c => c.CatgDesc).FirstOrDefault(),
                    FreepreDiag = Remarks.Where(c => c.CatgId == 137).Select(c => c.CatgDesc).FirstOrDefault(),
                    FreeAdvise = Remarks.Where(c => c.CatgId == 138).Select(c => c.CatgDesc).FirstOrDefault(),
                    FreeFollowUp = Remarks.Where(c => c.CatgId == 139).Select(c => c.CatgDesc).FirstOrDefault()

                };
                jsondata = js.Serialize(Doctor);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch
            {
                JObject send = null;
                return send;
            }
        }
        public int DeleteMedicationList(string Catgtypeid)
        {
            List<Models.SpecialInsertForDoctor> MediList = new List<Models.SpecialInsertForDoctor>();
            MediList = (List<Models.SpecialInsertForDoctor>)Session["Medicments"];
            var TempList = MediList.Where(c => (c.Medicine == Catgtypeid || c.When == Catgtypeid || c.HowManyTimes == Catgtypeid)).ToList();
            int Result = 0;
            if (TempList.Count() > 0)
            {
                Result = TempList.Select(c => c.SpId).FirstOrDefault();
                MediList.RemoveAll(c => (c.Medicine == Catgtypeid || c.When == Catgtypeid || c.HowManyTimes == Catgtypeid));
                Session["Medicments"] = MediList;
                if (MediList.Count > 0)
                    Session["CountSpecial"] = MediList.Select(c => c.SpId).Max();
                else
                    Session["CountSpecial"] = 0;
            }

            return Result;
        }
        [CustomAutohrize(null)]
        public ActionResult Notes(long Reservationid, string Url,int?diff)
        {
            try
            {
                var History = new List<Models.getHistoryNotes>();
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.shopMaster
                {
                    SchlStudentStrength = c.SchlStudentStrength,
                    LangId = c.Lang_id,
                    ImageLogo = c.ImageLogo
                }).FirstOrDefault();
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                //ViewBag.PatientData = fu.CommonPatientDetails(Reservationid.Value);
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Therapist_Notes" || c.Page_Name == "Therapist_Notes_Default") && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.LangId).Select(c => new Models.LanguageLabelDetails
                {
                    Lang_id = c.Lang_id,
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Label_Name = c.Label_Name
                }).ToList();
                var NoteSectionList = fu.getNotesSectionWithData(Reservationid);
                if (NoteSectionList.Count > 0)
                {
                    var Historydata = fu.getAllHistoryNotes(NoteSectionList.FirstOrDefault().EmpSchDetailsId, NoteSectionList.FirstOrDefault().UserId, ShopInfo.SchlStudentStrength);
                    if (Historydata == null)
                        Historydata = new List<Models.getHistoryNotes>();
                    var Idlist = Historydata.Where(c => c.EmpSchDetailsId != null).Select(c => c.EmpSchDetailsId).ToList();
                    Historydata.Add(new Models.getHistoryNotes() { BookingDate = NoteSectionList.FirstOrDefault().BookingDate, EmpSchDetailsId = NoteSectionList.FirstOrDefault().EmpSchDetailsId, schId = schlId, Bookings = NoteSectionList.FirstOrDefault().Bookings });
                    ViewBag.History = Historydata;                
                    List<Models.GetNoteHistoryDetails> HistoryList = new List<Models.GetNoteHistoryDetails>();
                    if (Idlist.Count>0)
                        HistoryList = fu.GetNoteHistoryList(string.Join(",", Idlist), NoteSectionList.FirstOrDefault().UserId.Value,diff.Value);
                    ViewBag.HistoryList = HistoryList;
                }
                ViewBag.PatientData = NoteSectionList;
                ViewBag.Catgdetails = fu.getNoteCatgDetails(NoteSectionList.Select(c => c.SectionCatgId.Value).Distinct().ToList());
                ViewBag.logo = fu.LogoImg(ShopInfo.ImageLogo);
                ViewBag.ShopInfo = ShopInfo;
                ViewBag.Url = Url;
                ViewBag.CheckAccessRight = fu.CheckAccessofPage("For Invoicing", UserId);
                var Link = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 188 && c.ActiveStatus == "A" && c.Group_orderdata == diff).Select(c => c.Email_Image).FirstOrDefault();
                Link = !string.IsNullOrEmpty(Link) ? Link : "Notes";               
                return View(Link);
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddNotes(FormCollection Note, int? ReservationId, int? UserId, int? status, string Url, int? diff)
        {
            try
            {
                if (ReservationId != 0 && UserId != 0 && ReservationId != null && UserId != null)
                {
                    CommonAddDoctorNote(Note, ReservationId, UserId);
                    if (status > 0)
                    {
                        long LoginUserId = Convert.ToInt32(Session["UserId"].ToString());
                        if (status == 2)
                            MailNotes(ReservationId, Note["EmailId"], LoginUserId);
                        var Link = "/PDF/NotesDisplay";
                        var DynamicLink = "";
                        if (diff > 3)
                            DynamicLink = SPA.CCTSP_CategoryDetails.Where(c => c.Group_orderdata == diff && c.ActiveStatus == "A" && c.CatgId == 188).Select(c => c.Gender).FirstOrDefault();
                        DynamicLink = !string.IsNullOrEmpty(DynamicLink) ? DynamicLink : Link;
                        return Redirect(DynamicLink + "?Reservationid=" + ReservationId + "&IsPrint=true&schlId=" + schlId + "&Url=" + Url + "&UserId=" + LoginUserId + "");
                        // return RedirectToAction("NotesDisplay", "PDF", new { Reservationid = ReservationId, IsPrint = true, schlId = schlId, Url= Url,UserId= LoginUserId });
                    }
                }
            }
            catch (Exception e)
            {

            }
            return Redirect("/Reservation/Reservation#" + Url);
        }
        public void CommonAddDoctorNote(FormCollection Note, int? ReservationId, int? UserId)
        {
            /*Variable*/
            #region Variable
            List<long> CatgTypeIdList = new List<long>();
            List<string> FreetextList = new List<string>();
            Prescription_Detail DetailPre = new Prescription_Detail();
            List<Prescription_Detail> getDetailPre = new List<Prescription_Detail>();
            #endregion
            /*get Shop information and Zonal Dates*/
            #region ShopInformation
            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                            .Select(c => new Models.ShopMasterDetail
                            {
                                SchlStudentStrength = c.SchlStudentStrength,
                                TimeZone = c.TimeZone
                            }).FirstOrDefault();
            var ZonalDate = fu.ZonalDate(ShopInfo.TimeZone);
            #endregion
            /*Add Prescription Master or get Prescription*/
            #region PrescriptionMaster
            Prescription_Master PM = fu.AddPrescreptionMaster(ReservationId.Value, ShopInfo.SchlStudentStrength.Value, UserId.Value, ZonalDate);
            #endregion
            if (PM != null)
            {
                /*get Category*/
                #region CategoryForAddingData
                var getCatg = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 179 && c.ActiveStatus == "A"
                && c.Group_orderdata == ShopInfo.SchlStudentStrength)
                .Select(c => new Models.CategoryDetails
                {
                    CatgDesc = c.CatgDesc,
                    CatgName = c.CatgType
                }).ToList();
                #endregion
                /*Add Notes Main Data*/
                #region NotesAdd
                foreach (var CatgData in getCatg)
                {
                    foreach (var item in Note.AllKeys.Where(c => c.Contains(CatgData.CatgName)))
                    {
                        if (!string.IsNullOrEmpty(Note[item]))
                        {
                            DetailPre = new Prescription_Detail();
                            DetailPre.ActiveStatus = "A";
                            DetailPre.created_on = ZonalDate;
                            DetailPre.UserId = UserId.Value;
                            DetailPre.CatgId = Convert.ToInt64(CatgData.CatgDesc);
                            DetailPre.Prescription_Id = PM.Prescription_Id;
                            DetailPre.BookingId = ReservationId.Value;
                            DetailPre.SchId = schlId;
                            if (item.Contains("FreeText_"))
                            {
                                DetailPre.Remarks = Note[item];
                                DetailPre.Type = ShopInfo.SchlStudentStrength;
                            }
                            if (!item.Contains("FreeText_") && Note[item] != "0")
                                DetailPre.CatgTypeId = Convert.ToInt64(Note[item]);
                            getDetailPre.Add(DetailPre);
                        }
                    }
                }
                if (getDetailPre.Count > 0)
                {
                    SPA.Prescription_Detail.AddRange(getDetailPre);
                    SPA.SaveChanges();
                }
                #endregion
            }
        }
        public void MailNotes(long? Reservationid, string EmailID, long UserId)
        {
            try
            {
                bool IsPrint = false;
                var Shopinfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.shopMaster { LangId = c.Lang_id,SchlStudentStrength=c.SchlStudentStrength}).FirstOrDefault();

                int LangId = Shopinfo.LangId.Value;
                var LinkName = fu.GetCurrenturl();
                var LinkD = "/PDF/NotesDisplay";
                var DynamicLink = "";
                if (Shopinfo.SchlStudentStrength > 3)
                    DynamicLink = SPA.CCTSP_CategoryDetails.Where(c => c.Group_orderdata == Shopinfo.SchlStudentStrength && c.ActiveStatus == "A" && c.CatgId == 188).Select(c => c.Gender).FirstOrDefault();
                DynamicLink = !string.IsNullOrEmpty(DynamicLink) ? DynamicLink : LinkD;
                var Link = DynamicLink + "?Reservationid=" + Reservationid + "&IsPrint=" + IsPrint + "&schlId=" + schlId + "&UserId=" + UserId + "";
                Link = LinkName + Link;
                Email.EmailWithPdf(EmailID, schlId, Link, "Prescription.pdf", "", LangId, Reservationid);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        public ActionResult DunamicNoteSection()
        {
            return View();
        }
        public ActionResult AddDynamicNoteSection(FormCollection Section)
        {
            //Add Catg in Master
            var List = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Therapist_Notes").ToList();
            int OrderId = List.Count > 0 ? List.Select(c => c.Order_id).Max().Value + 1 : 0;
            CCTSP_CategoryMaster Catg = new CCTSP_CategoryMaster();
            Catg.CatgName = Section["KeyName"];
            Catg.CatgDesc = Section["CategoryName"];
            Catg.ActiveStatus = "A";
            Catg.CreatedOn = DateTime.Now;
            SPA.CCTSP_CategoryMaster.Add(Catg);
            SPA.SaveChanges();
            //Assign Catg In Details
            CCTSP_CategoryDetails CatgDetails = new CCTSP_CategoryDetails();
            CatgDetails.CatgId = 179;
            CatgDetails.CatgType = Section["KeyName"];
            CatgDetails.CatgDesc = Convert.ToString(Catg.CatgId);
            CatgDetails.ActiveStatus = "A";
            CatgDetails.CreatedOn = DateTime.Now;
            SPA.CCTSP_CategoryDetails.Add(CatgDetails);
            SPA.SaveChanges();
            List<Language_Label_Detail> LangList = new List<Language_Label_Detail>();
            List<Language_Label_Detail> AddLangList = new List<Language_Label_Detail>();
            //Place order Free Text
            Language_Label_Detail Lang = new Language_Label_Detail();
            Lang.ActiveStatus = "A";
            Lang.CreateDate = DateTime.Now;
            Lang.UpdatedDate = DateTime.Now;
            Lang.newdata = "1";
            Lang.Value = Section["CategoryName"] + " Free Text";
            Lang.English_Label = "Place_Order_FreeText";
            Lang.Label_Name = Convert.ToString(Catg.CatgId);
            Lang.Page_Name = "Therapist_Notes";
            LangList.Add(Lang);
            //Place Order Dropdown Search
            Lang = new Language_Label_Detail();
            Lang.ActiveStatus = "A";
            Lang.CreateDate = DateTime.Now;
            Lang.UpdatedDate = DateTime.Now;
            Lang.newdata = "1";
            Lang.Value = Section["CategoryName"] + " Search";
            Lang.English_Label = "Place_Order_Drop_Search";
            Lang.Label_Name = Convert.ToString(Catg.CatgId);
            Lang.Page_Name = "Therapist_Notes";
            LangList.Add(Lang);
            //Select Catg Label
            Lang = new Language_Label_Detail();
            Lang.ActiveStatus = "A";
            Lang.CreateDate = DateTime.Now;
            Lang.UpdatedDate = DateTime.Now;
            Lang.newdata = "1";
            Lang.Value = "Select " + Section["CategoryName"];
            Lang.English_Label = "Select_Catg_Label";
            Lang.Label_Name = Convert.ToString(Catg.CatgId);
            Lang.Page_Name = "Therapist_Notes";
            LangList.Add(Lang);
            //Catg Label
            Lang = new Language_Label_Detail();
            Lang.ActiveStatus = "A";
            Lang.CreateDate = DateTime.Now;
            Lang.UpdatedDate = DateTime.Now;
            Lang.newdata = "1";
            Lang.Value = Section["CategoryName"];
            Lang.English_Label = "Catg_Label";
            Lang.Label_Name = Convert.ToString(Catg.CatgId);
            Lang.Page_Name = "Therapist_Notes";
            LangList.Add(Lang);
            var LangNameList = fu.LanguageNameList();
            foreach (var LangId in LangNameList)
            {
                int POrderId = OrderId;
                foreach (var Item in LangList)
                {
                    Lang = new Language_Label_Detail();
                    Lang.ActiveStatus = Item.ActiveStatus;
                    Lang.CreateDate = Item.CreateDate;
                    Lang.UpdatedDate = Item.UpdatedDate;
                    Lang.newdata = Item.newdata;
                    Lang.Value = Item.Value;
                    Lang.Order_id = POrderId;
                    Lang.English_Label = Item.English_Label;
                    Lang.Label_Name = Item.Label_Name;
                    Lang.Page_Name = Item.Page_Name;
                    Lang.Lang_id = LangId.Lang_id;
                    AddLangList.Add(Lang);
                    POrderId = POrderId + 1;
                }

            }
            SPA.Language_Label_Detail.AddRange(AddLangList);
            SPA.SaveChanges();
            return RedirectToAction("DunamicNoteSection", "Doctor");
        }
        public bool AddFlow(string Text)
        {
            long? catgId = SPA.CCTSP_CategoryMaster.Where(c => c.CatgName == "FlowList" && c.ActiveStatus == "A").Select(c => c.CatgId).FirstOrDefault();
            long? MaxGrouporder = SPA.CCTSP_CategoryDetails.Where(c => c.CCTSP_CategoryMaster.CatgName == "FlowForDoctor" && c.ActiveStatus == "A").Select(c => c.Group_orderdata).Max() + 1;
            if (catgId != null)
            {
                CCTSP_CategoryDetails catg = new CCTSP_CategoryDetails();
                catg.CreatedOn = DateTime.Now;
                catg.ActiveStatus = "A";
                catg.CatgType = "USER";
                catg.CatgDesc = Text;
                catg.CatgId = catgId.Value;
                catg.Group_orderdata = MaxGrouporder == 0 || MaxGrouporder == null ? 1 : MaxGrouporder;
                SPA.CCTSP_CategoryDetails.Add(catg);
                SPA.SaveChanges();
                return true;
            }
            return false;

        }
        public ActionResult DisplayNotes(long Reservationid)
        {
            var Details = fu.getNotesSectionWithData(Reservationid);
            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A")
                               .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, Logo = c.ImageLogo })
                               .FirstOrDefault();
            ViewBag.Logo = fu.LogoImg(ShopInfo.Logo);
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Therapist_Notes" || c.Page_Name == "Therapist_Notes_Default") && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
            {
                Lang_id = c.Lang_id,
                English_Label = c.English_Label,
                Value = c.Value,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id,
                Label_Name = c.Label_Name
            }).ToList();
            return View(Details);
        }
        public ActionResult SaveDoctorNote(FormCollection Note, int? ReservationId, int? UserId, string Status, string Url)
        {
            try
            {
                if (ReservationId != 0 && UserId != 0 && ReservationId != null && UserId != null)
                {
                    CommonAddDoctorNote(Note, ReservationId, UserId);
                    if (Status == "NR")
                        return Redirect("/BookOrder/OrderList?id=" + UserId + "&Status=DO&Url=" + Url);
                    else
                        return Redirect("/Reservation/AppClosedAndRedirectToInvoice?ReservationId=" + ReservationId + "&Url=" + Url);
                }
                else
                    return Redirect("/Reservation/Reservation#" + Url);
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Therapistsnotes()
        {
            return View();
        }
        public ActionResult TCMnote()
        {
            return View();
        }
    }
}