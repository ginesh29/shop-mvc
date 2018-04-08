using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace SPA.Controllers
{
    public class APIController : Controller
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        string Logotext = "CLICK-AND-GO";
        string LogoImage = "http://tshope.azurewebsites.net/images/Maarkss.png";
        TimeZoneInfo IND_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        Common.Function fu = new Common.Function();
        JObject send = null;
        string jsondata = "";
        Common.PuchSMS SMS = new Common.PuchSMS();
        // GET: API
        public void SMSRem()
        {
            #region NewCode
            try
            {

                string CurrentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE).ToString("yyyy-MM-dd HH:mm:ss");
                //var Mainquery = "select a.BookingDate,a.EmpSchDetailsId,b.schCountry as Country,a.FromSlotMasterId,a.ToSlotMasterId,e.CatgDesc,b.SchlName,c.EmailId,c.PhoneNo,c.FirstName as ClientName,c.LastName as ClientLastName,convert(nvarchar,c.Email_service) as Email_Reg,convert(nvarchar,c.Sms_service) as Sms_reg,c.gender as ClientGender, d.FirstName as EmployeeName,d.LastName as EmployeeLastName,a.SchId from spa_employeescheduler a join cctsp_schoolMaster b on a.schId = b.SchlId join CCTSP_User c on c.UserId = a.Client_UserId join CCTSP_User d on d.UserId = a.Emp_UserId join CCTSP_CategoryDetails e on e.CatgTypeId = a.Product_Id join spatime_zone f on f.timezone_id = b.timezone_id where b.ActiveStatus = 'A' and b.Send_Sms = 1 and a.activestatus = 'A' and a.Bookedstatus = 'B' and a.Rem_Sms is null and CONVERT(datetime, dateadd(hour,(-1 * b.AlertRemainder), CONVERT(datetime, (a.bookingdate + ' ' + a.fromslotmasterid))),108)>= CONVERT(datetime, dateadd(minute, -20, (DBO.GetGeographicalTime('(GMT +05:30) Kolkata', f.name_timezone, '" + CurrentDate + "'))), 108) and CONVERT(datetime, dateadd(hour,(-1 * b.AlertRemainder), CONVERT(datetime, (a.bookingdate + ' ' + a.fromslotmasterid))),108)<= CONVERT(datetime, dateadd(minute, 20, (DBO.GetGeographicalTime('(GMT +05:30) Kolkata', f.name_timezone, '" + CurrentDate + "'))), 108)";
                var Mainquery = "select a.BookingDate,a.EmpSchDetailsId,b.schCountry as Country,a.FromSlotMasterId,a.ToSlotMasterId,e.CatgDesc,b.SchlName,c.EmailId,c.PhoneNo,c.FirstName as ClientName,c.LastName as ClientLastName,convert(nvarchar, c.Email_service) as Email_Reg,convert(nvarchar, c.Sms_service) as Sms_reg,c.gender as ClientGender, d.FirstName as EmployeeName,d.LastName as EmployeeLastName,a.SchId,b.TimeZone,convert(datetime,a.BookingDate+' '+a.fromslotmasterid,108) as BookedDate from spa_employeescheduler a join cctsp_schoolMaster b on a.schId = b.SchlId join CCTSP_User c on c.UserId = a.Client_UserId join CCTSP_User d on d.UserId = a.Emp_UserId join CCTSP_CategoryDetails e on e.CatgTypeId = a.Product_Id join spatime_zone f on f.timezone_id = b.timezone_id where b.ActiveStatus = 'A'and b.Schlid in(614) and a.activestatus = 'A' and a.Bookedstatus = 'B'  and a.Rem_Sms is null and CONVERT(datetime, dateadd(hour,(-1 * b.AlertRemainder), CONVERT(datetime, (a.bookingdate + ' ' + a.fromslotmasterid))),108)< CONVERT(datetime, DBO.GetGeographicalTime('(GMT +05:30) Kolkata', f.name_timezone, '" + CurrentDate + "'), 108)";
                var MainResult = SPA.Database.SqlQuery<Models.ReminderCountry>(Mainquery).ToList();
                //120 for email and 121 for SMS
               List<Models.ReminderText> ReminderTextList = new List<Models.ReminderText>();
                 ReminderTextList = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.Group == null && (c.CCTSP_CategoryDetails.CatgId == 120 || c.CCTSP_CategoryDetails.CatgId == 121) && c.CCTSP_CategoryDetails.CatgType == "Rem").Select(c=> new Models.ReminderText {
                    SectionDesc=c.SectionDesc,
                    Schid=c.SchId,
                    ItenName=c.ItenName,
                    CatgId=c.CCTSP_CategoryDetails.CatgId,
                    LangId=c.CCTSP_SchoolMaster.Lang_id,
                    MainCategory=c.CCTSP_SchoolMaster.MainCategory
                }).ToList();
                //Email and Sms Text
                var LanguageText = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && (c.Label_Name == "11319" || c.Label_Name == "11320" && c.Page_Name == "Shop_Owner_Text")).ToList();
                //Language Tag
                var LanguageTag = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.ActiveStatus == "A" && c.Order_id == 6).ToList();
                foreach (var Result in MainResult)
                {
                    try
                    {
                        var CurrentDateTime = fu.ZonalDate(Result.TimeZone);
                        Models.EmailSend AlertEmail = new Models.EmailSend();
                        var ReminderEmailText = new Models.ReminderText();
                        var ReminderSMSText = new Models.ReminderText();
                        //Record
                        //RecordData(Result.EmpSchDetailsId.ToString());
                        if (CurrentDateTime >= Result.BookedDate)
                        {
                            //Intialization Globle for Email and Sms both
                            #region Init
                            var EmailText = "";
                            var SmsText = "";
                            
                            ReminderEmailText = ReminderTextList.Where(c => c.Schid == Result.SchId && c.CatgId == 120).FirstOrDefault();
                            ReminderSMSText = ReminderTextList.Where(c => c.Schid == Result.SchId && c.CatgId == 121).FirstOrDefault();
                            #endregion
                            //Email
                            #region Emailsection
                            if (ReminderEmailText != null)
                            {
                                if (ReminderEmailText.ItenName == "Default")
                                    EmailText = LanguageText.Where(c => c.Label_Name == "11319" && c.Lang_id == ReminderEmailText.LangId).Select(c => c.Value).FirstOrDefault();
                                else
                                    EmailText = ReminderEmailText.SectionDesc;
                                AlertEmail.LogoText = SPA.Language_NewShop.Where(c => c.Page_Name == "Header" && c.Order_id == 14 && c.col2 == "A" && c.Lang_id == ReminderEmailText.LangId).Select(c => c.Value).FirstOrDefault();
                                //Replacement for email
                                #region EmailReplacement

                                if (EmailText.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "“date & time & product name & shopname”", Result.BookingDate + " & " + Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3) + " & " + Result.CatgDesc + Result.SchlName, RegexOptions.IgnoreCase);

                                if (EmailText.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "\"Product, date & time & shop name\"", Result.BookingDate + " & " + Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3) + " & " + Result.CatgDesc + Result.SchlName, RegexOptions.IgnoreCase);

                                if (EmailText.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = EmailText.Replace("\"product name\"", Result.CatgDesc);
                                if (EmailText.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = EmailText.Replace("«product name»", Result.CatgDesc);
                                if (EmailText.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = EmailText.Replace("«product name\"", Result.CatgDesc);
                                if (EmailText.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "\"Date\"", Result.BookingDate, RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "«Date»", Result.BookingDate, RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "«Date\"", Result.BookingDate, RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "\"Time\"", Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3), RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "«Time»", Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3), RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "«Time\"", Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3), RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "\"shop name\"", Result.SchlName, RegexOptions.IgnoreCase);

                                if (EmailText.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "«shop name»", Result.SchlName, RegexOptions.IgnoreCase);

                                if (EmailText.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "shop name", Result.SchlName, RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "«shop name\"", Result.SchlName, RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "Freundliche Grüsse,", "<br><br>Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                                if (EmailText.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                                    EmailText = Regex.Replace(EmailText, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                                #endregion
                                //Insertion to Model for Email
                                #region EmailInsertion
                                EmailText = HttpUtility.UrlEncode(EmailText);
                                //AlertEmail.ToWhom = Result.ClientGender + " " + Result.ClientName + " " + Result.ClientLastName;
                                AlertEmail.EmailId = Result.EmailId;
                                AlertEmail.DatabaseBody = EmailText;
                                AlertEmail.ShopName = Result.SchlName;
                                AlertEmail.schid = Result.SchId;
                                AlertEmail.TitleHead = LanguageTag.Where(c => c.Lang_id == ReminderEmailText.LangId).Select(c => c.Value).FirstOrDefault();
                                AlertEmail.MainCategory = ReminderEmailText.MainCategory;
                                #endregion
                            }
                            #endregion
                            //SMS
                            #region SMSSection
                            if (ReminderSMSText != null)
                            {
                                if (ReminderSMSText.ItenName == "Default")
                                    SmsText = LanguageText.Where(c => c.Label_Name == "11320" && c.Lang_id == ReminderEmailText.LangId).Select(c => c.Value).FirstOrDefault();
                                else
                                    SmsText = ReminderSMSText.SectionDesc;

                                //SMS Replacment
                                #region SmsReplace

                                if (SmsText.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "\"date & time & product & shop name\"", Result.BookingDate + " & " + Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3) + " & " + Result.CatgDesc + Result.SchlName, RegexOptions.IgnoreCase);

                                if (SmsText.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "“date & time & product name & shopname”", Result.BookingDate + " & " + Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3) + " & " + Result.CatgDesc + Result.SchlName, RegexOptions.IgnoreCase);

                                if (SmsText.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "\"Product, date & time & shop name\"", Result.BookingDate + " & " + Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3) + " & " + Result.CatgDesc + Result.SchlName, RegexOptions.IgnoreCase);

                                if (SmsText.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = SmsText.Replace("\"product name\"", Result.CatgDesc);
                                if (SmsText.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = SmsText.Replace("«product name»", Result.CatgDesc);
                                if (SmsText.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = SmsText.Replace("«product name\"", Result.CatgDesc);
                                if (SmsText.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "\"Date\"", Result.BookingDate, RegexOptions.IgnoreCase);
                                if (SmsText.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "«Date»", Result.BookingDate, RegexOptions.IgnoreCase);
                                if (SmsText.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "«Date\"", Result.BookingDate, RegexOptions.IgnoreCase);
                                if (SmsText.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "\"Time\"", Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3), RegexOptions.IgnoreCase);
                                if (SmsText.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "«Time»", Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3), RegexOptions.IgnoreCase);
                                if (SmsText.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "«Time\"", Result.FromSlotMasterId.ToString().Remove(Result.FromSlotMasterId.ToString().Length - 3, 3), RegexOptions.IgnoreCase);
                                if (SmsText.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "\"shop name\"", Result.SchlName, RegexOptions.IgnoreCase);

                                if (SmsText.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "«shop name»", Result.SchlName, RegexOptions.IgnoreCase);

                                if (SmsText.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                    SmsText = Regex.Replace(SmsText, "«shop name\"", Result.SchlName, RegexOptions.IgnoreCase);

                                #endregion
                                //Insertion to Model for SMS
                                #region SMSInsertion
                                AlertEmail.SMstext = SmsText;
                                AlertEmail.PhoneNumber = Result.PhoneNo;
                                #endregion
                            }
                            #endregion
                            //Request sent
                            #region StartEmailandSmsreq
                            AlertEmail.Country = Result.Country;
                            if (AlertEmail != null)
                            {
                                if (ReminderEmailText != null)
                                    if (Result.Email_Reg == "2" || Result.Email_Reg == "1") { SendEmailpls(AlertEmail); }
                                if (ReminderSMSText != null && Result.send_sms != null)
                                    if (Result.Sms_reg == "2" || Result.Sms_reg == "1") { SendSms(AlertEmail); }

                                //Update of Status of Reminder Text
                                var queryUpdteTxt = "update SPA_EmployeeScheduler set Rem_Sms=1 where EmpSchDetailsId=" + Result.EmpSchDetailsId;
                                SPA.Database.ExecuteSqlCommand(queryUpdteTxt);
                            }
                            #endregion
                        }
                    }
                    catch (Exception e)
                    {
                        fu.ErrorSend(RouteData, e);
                        //SendErrorLog(e);
                    }
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                SendErrorLog(e);
            }
            #endregion
        }
        public void SendEmailpls(Models.EmailSend Email)
        {
            try
            {
                //Models.MainEmailSend EmSend = new Models.MainEmailSend()
                //{
                //    emailid = Email.EmailId,
                //    header = Email.TitleHead,
                //    ProjectName = "CLICK-AND-GO",
                //    DatabaseBody = Email.DatabaseBody,
                //    ShopName = Email.ShopName,
                //    TitleHead = Email.TitleHead,
                //    LogoText = Logotext,
                //    LogoImage = LogoImage
                //};
                //jsondata = js.Serialize(EmSend);
                //send = JObject.Parse(jsondata);
                //string url = "http://a.maarkss.ch/Home/PushEmail1?EmailData=" + jsondata;
                //HttpWebRequest myReq =
                //(HttpWebRequest)WebRequest.Create(url);
                //HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                //System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                //string responseString = respStreamReader.ReadToEnd();
                //respStreamReader.Close();
                //myResp.Close();
                //var request = (HttpWebRequest)WebRequest.Create("http://localhost:64887/api/User/Reminder-Email");
                var request = (HttpWebRequest)WebRequest.Create("http://API.click-and-go.in/api/User/Reminder-Email");
                var postData = "EmailId=" + Email.EmailId;
                postData = postData + "&TitleHead=" + HttpUtility.UrlEncode(Email.TitleHead);
                postData = postData + "&DatabaseBody=" + HttpUtility.UrlEncode(Email.DatabaseBody);
                postData = postData + "&ShopName=" + Email.ShopName;
                postData = postData + "&schid=" + Email.schid;
                postData = postData + "&LogoText=" + Email.LogoText;
                postData = postData + "&MainCategory=" + Email.MainCategory;
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
                var query = "update CCTSP_SchoolMaster set total_email=total_email+1 where SchlId=" + Email.schid;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                //SendErrorLog(e);
            }

        }
        public void SendSms(Models.EmailSend SMSTXT)
        {
            try
            {
                var SmsCountry = "EuropeSms";
                if (SMSTXT.Country.Trim().ToLower() == "india")
                    SmsCountry = "IndiaSms";
                //SMS URL To Use
                var SmsUrl = SPA.CTSP_SolutionMaster.Where(c => c.Activestatus == "A" && c.CCTSP_CategoryDetails.CatgId == 145 && c.CCTSP_CategoryDetails.CatgDesc == SmsCountry).Select(c => c.SectionDesc).FirstOrDefault();
                List<string> phno = new List<string>();
                phno.Add(SMSTXT.PhoneNumber);
                SMS.SendMarketingSms(SMSTXT.SMstext, phno, SmsCountry, SMSTXT.Country.Trim().ToUpper());
                var query = "update CCTSP_SchoolMaster set total_sms=total_sms+" + phno.Count + " where SchlId=" + SMSTXT.schid;
                SPA.Database.ExecuteSqlCommand(query);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                //SendErrorLog(e);
            }
        }
        public void SendErrorLog(Exception e)
        {
            try
            {
                
                string directoryPath = "C:\\SpaReminderLogFiles";
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    DateTime now = DateTime.Now;
                    string format = "MM-dd-yyyy";
                    string path = directoryPath + "\\" + "SendEmail" + now.ToString(format) + ".txt";
                    StreamWriter swErrorLog = null;
                    DirectoryInfo dtDirectory = null;
                    if (!Directory.Exists(directoryPath))
                    {
                        dtDirectory = Directory.CreateDirectory(directoryPath);
                        dtDirectory = null;
                    }

                    if (System.IO.File.Exists(path))
                        swErrorLog = new StreamWriter(path, true); //append the error message
                    else
                        swErrorLog = System.IO.File.CreateText(path);

                    swErrorLog.WriteLine("Date and Time of Exception: " + DateTime.Now);
                    swErrorLog.WriteLine("Source of Exception: " + e.Source);
                    swErrorLog.WriteLine(" ");
                    swErrorLog.WriteLine("Error Message: " + e.Message);
                    swErrorLog.WriteLine("Error Location:" + e.TargetSite);
                    swErrorLog.WriteLine("stackTrace :" + e.StackTrace);
                    swErrorLog.WriteLine("InnerException" + e.InnerException);
                    swErrorLog.WriteLine("------------------------------------------- ");
                    swErrorLog.WriteLine(" ");
                    swErrorLog.Close();
                    swErrorLog = null;
                }
            }
            catch (NullReferenceException)
            {

            }
        }
        public void RecordData(string ResId)
        {
            try
            {
                string directoryPath = "C:\\RecordSPAData";
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    DateTime now = DateTime.Now;
                    string format = "MM-dd-yyyy";
                    string path = directoryPath + "\\" + "RecordSPA" + now.ToString(format) + ".txt";
                    StreamWriter swErrorLog = null;
                    DirectoryInfo dtDirectory = null;
                    if (!Directory.Exists(directoryPath))
                    {
                        dtDirectory = Directory.CreateDirectory(directoryPath);
                        dtDirectory = null;
                    }

                    if (System.IO.File.Exists(path))
                        swErrorLog = new StreamWriter(path, true); //append the error message
                    else
                        swErrorLog = System.IO.File.CreateText(path);

                    swErrorLog.WriteLine("Record Date: " + DateTime.Now);
                    swErrorLog.WriteLine("Reservation Id: " + ResId);
                    swErrorLog.WriteLine(" ");
                    swErrorLog.WriteLine("------------------------------------------- ");
                    swErrorLog.WriteLine(" ");
                    swErrorLog.Close();
                    swErrorLog = null;
                }
            }
            catch (NullReferenceException)
            {

            }
        }
        public void SendBirthday()
        {
            DateTime CurrentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IND_ZONE);
            var date = SPA.CCTSP_User.Where(c => c.DOB.Day == CurrentDate.Day && c.DOB.Month == CurrentDate.Month && c.RoleId == 4 && c.ActiveStatus == "A").ToList();
            var LangBirth = SPA.Language_Label_Detail.Where(c => ((c.Page_Name == "shop_owner_text" && c.Order_id == 27) || (c.Page_Name == "Email_Tag" && c.Order_id == 7)) && c.ActiveStatus == "A").ToList();
           var LogoList = SPA.Language_NewShop.Where(c => c.Page_Name == "Header" && c.Order_id == 14 && c.col2 == "A").ToList();
            #region MailProgram
            foreach (var item in date)
            {
                try
                {
                    var Lang_Id = item.CCTSP_SchoolMaster.Lang_id;
                    if (item.Email_Service == 2 || item.Email_Service == 1)
                    {
                        // var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/PushEmail");
                        //var postData = "emailid=" + item.EmailId;
                        //postData = postData + "&header=" + LangBirth.Where(c => c.Lang_id == Lang_Id && c.Page_Name == "Email_Tag").Select(c => c.Value).FirstOrDefault();
                        //postData = postData + "&ProjectName=" + "CLICK-AND-GO";
                        //postData = postData + "&DatabaseBody=" + LangBirth.Where(c => c.Lang_id == Lang_Id && c.Page_Name == "shop_owner_text").Select(c => c.Value).FirstOrDefault();
                        var EmailText = LangBirth.Where(c => c.Lang_id == Lang_Id && c.Order_id == 27).Select(c => c.Value).FirstOrDefault();
                        if (EmailText.IndexOf("\"First name & Family name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            EmailText = Regex.Replace(EmailText, "\"First name & Family name\"", item.FirstName+ " "+item.LastName, RegexOptions.IgnoreCase);
                        if (EmailText.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            EmailText = Regex.Replace(EmailText, "\"shop name\"", item.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                        if (EmailText.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            EmailText = Regex.Replace(EmailText, "\"shop name\"", item.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                        if (EmailText.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                            EmailText = Regex.Replace(EmailText, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                        if (EmailText.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                            EmailText = Regex.Replace(EmailText, "Freundliche Grüsse,", "<br><br>Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                        if (EmailText.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                            EmailText = Regex.Replace(EmailText, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);

                        //var request = (HttpWebRequest)WebRequest.Create("http://localhost:64887/api/User/Reminder-Email");
                        var request = (HttpWebRequest)WebRequest.Create("http://API.click-and-go.in/api/User/Reminder-Email");                       
                        var postData = "EmailId=" + item.EmailId;
                        postData = postData + "&TitleHead=" + HttpUtility.UrlEncode(LangBirth.Where(c => c.Lang_id == Lang_Id && c.Page_Name == "Email_Tag").Select(c => c.Value).FirstOrDefault());
                        postData = postData + "&DatabaseBody=" + HttpUtility.UrlEncode(EmailText);
                        postData = postData + "&ShopName=" + item.CCTSP_SchoolMaster.SchlName;
                        postData = postData + "&schid=" + item.CCTSP_SchoolMaster.SchlId;
                        postData = postData + "&LogoText=" + LogoList.Where(c=>c.Lang_id==item.CCTSP_SchoolMaster.Lang_id).Select(c=>c.Value).FirstOrDefault();
                        postData = postData + "&MainCategory=" + item.CCTSP_SchoolMaster.MainCategory;



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
                        var query = "update CCTSP_SchoolMaster set total_email=total_email+1 where SchlId=" + item.SchId;
                        SPA.Database.ExecuteSqlCommand(query);
                    }
                }
                catch (Exception ex)
                {
                    fu.ErrorSend(RouteData, ex);
                }
            }
            #endregion
        }
    }
}