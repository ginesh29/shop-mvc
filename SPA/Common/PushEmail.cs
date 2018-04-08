using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using System.Xml;
using SPA.Common;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Web.Script.Serialization;

namespace SPA.Common
{
    public class PushEmail
    {
        cctspDatabaseEntities22 spa = new cctspDatabaseEntities22();
        Alert AlertSPA = new Alert();
        CultureInfo enGB = new CultureInfo("en-GB");
        JavaScriptSerializer js = new JavaScriptSerializer();
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //int LangId = Convert.ToInt32(ConfigurationManager.AppSettings["Language"]);
        string EmailUrl = ConfigurationManager.AppSettings["EmailUrl"];
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        string FromIns = "no-reply@click-and-go";
        string ShopUrl = "http://";
        public PushEmail()
        {
            ShopUrl = ShopUrl + link;
            schlId = Convert.ToInt32(GetShopId(link));
        }
        public long GetShopId(string Link)
        {
            long? Shopid = 0;
            if (Link != "localhost" && Link != "devtestspa001.azurewebsites.net" && Link != "click-and-go.co.in" && Link != "click-and-go.ch")
                Shopid = spa.CCTSP_LendingPageMaster.Where(c => c.Azure_Website == Link || c.Original_Website == Link).Select(c => c.Schid).FirstOrDefault();

            if (Shopid == 0 || Shopid == null)
                Shopid = schlId;
            return Shopid.Value;
        }
        public bool CheckSubTabAccess(string SubTabName, long FlowId)
        {
            bool Status = false;
            int Count = spa.Database.SqlQuery<int>(checkSubTabAccess(SubTabName, FlowId)).FirstOrDefault();
            if (Count > 0)
                Status = true;
            return Status;
        }
        public string checkSubTabAccess(string SubTabName, long FlowId)
        {
            return "select Count(d.solutionId) as Count " +
                   "from cctsp_CategoryDetails a " +
                   "join ctsp_SolutionMaster d on d.CatgTypeId = a.CatgTypeId " +
                   "join Spa_Flow b on b.flow_id = " + FlowId + " " +
                   "join Spa_FlowDetails c on c.flow_id = b.Flow_id and a.CatgTypeid = c.MainTab_Id and d.solutionId = c.SubTab_Id " +
                   "Where CatgId = 147 and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' " +
                   "and d.sectionDesc = '" + SubTabName + "' ";
        }
        public void ErrorSend(RouteData route, Exception e)
        {
            try
            {
                SPA_Error ErrorAdd = new SPA_Error();
                ErrorAdd.Action_Name = route.Values["Action"].ToString();
                ErrorAdd.Controller_name = route.Values["Controller"].ToString();
                ErrorAdd.ErrorMsg = e.Message;
                ErrorAdd.ReqColumn1 = schlId.ToString();
                ErrorAdd.ReqColumn2 = e.InnerException.Message;
                TimeZoneInfo IndiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime NowDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaZone);
                ErrorAdd.Created_Date = NowDate;
                spa.SPA_Error.Add(ErrorAdd);
                spa.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public bool SendPushEmail(string EmailTo, string EmailFrom, string subject, string Content)
        {
            SmtpClient Client = new SmtpClient();
            MailMessage message;
            try
            {
                Client.Host = ConfigurationManager.AppSettings["SmtpHost"];
                Client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpAuthentification"]) &&
                    Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpAuthentification"]) == true)
                    Client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpLogin"], ConfigurationManager.AppSettings["SmtpPassword"]);
                else
                {
                    Client.Credentials = new NetworkCredential();
                    Client.UseDefaultCredentials = true;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SmtpSSL"]) &&
                    Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSSL"]) == true)
                    Client.EnableSsl = true;
                else
                    Client.EnableSsl = false;
                message = new MailMessage(ConfigurationManager.AppSettings["MailSender"].ToString(), EmailTo, subject, Content);
                message.IsBodyHtml = true;
                Client.Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task MailForLogin(string Name, string EmailId)
        {
            await Task.Run(() => Loginmail(Name, EmailId));
        }
        public void Loginmail(string Name, string EmailId)
        {
            // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                string mailTo = EmailId;
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Login" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                    var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 1 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    string text;
                    if (data.ItenName == "Default")
                    {
                        text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        text = data.SectionDesc;
                    }
                    DatabaseBody = text;

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Herr X / Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Frau Y\"", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms. X / Mdm Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms. X / Mdm Y\"", Name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
                    CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                }
            }
            catch
            {
            }
        }
        public async Task MailForUserRegistration(long UserId, string login_email_id)
        {
            await Task.Run(() => UserRegistration(UserId, login_email_id));
        }
        public void UserRegistration(long UserId, string login_email_id)
        {
            // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                var UserInformation = spa.CCTSP_User.Where(c => c.UserId == UserId).FirstOrDefault();
                string name = UserInformation.Gender + " " + UserInformation.FirstName + " " + UserInformation.LastName;
                string mailTo = login_email_id;
                string DatabaseBody = "";

                var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Reg" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                    var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 2 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    // var Title = spa.Language_Label_Detail.Where(c => c.Page_Name == "Layout_Client" && c.Order_id == 9 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    string text;
                    if (data.ItenName == "Default")
                    {
                        text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        text = data.SectionDesc;
                    }
                    DatabaseBody = text;

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Herr X / Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms. X / Mdm Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms. X / Mdm Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);

                    CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                }

                #region CodeForReminder
                DateTime DTTEMP = DateTime.Parse("0001-01-01");
                if (UserInformation.DOB != null && UserInformation.DOB != DTTEMP)
                {
                    var BirthdayWish = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Bir" && c.Activestatus == "A" && c.SchId == schlId && (c.CCTSP_CategoryDetails.CatgId == 120 || c.CCTSP_CategoryDetails.CatgId == 121));
                    Models.AlertAPIModel Alert = new Models.AlertAPIModel();
                    Alert.username = "mohammad786.rajpur@gmail.com";
                    Alert.Password = "qq";
                    Alert.ToWhom = name;
                    Alert.EmailId = login_email_id;
                    Alert.PhoneNo = UserInformation.PhoneNo;
                    Alert.DateTime = UserInformation.DOB.ToString("yyyy/MM/dd HH:mm:ss");
                    Alert.Birthday_wish =
                    Alert.DateTime = null;
                    Alert.type_id = 1;
                    Alert.greetingMessage = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault().CCTSP_CategoryDetails.CatgDesc;
                    Alert.display_name = true;
                    Alert.bodyMessage = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault().SectionDesc;
                    Alert.endMessage = "Thanks";
                    Alert.endName = UserInformation.CCTSP_SchoolMaster.SchlName;
                    Alert.Birthday_wish = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 121).FirstOrDefault().SectionDesc;
                    AlertSPA.AlertAPI(Alert);
                }
                #endregion
            }
            catch
            {

            }
        }
        public async Task Approvebooking(string name, string date, string login_email_id, string Time, int? EmpSchId)
        {
            await Task.Run(() => MailForApproveBooking(name, date, login_email_id, Time, EmpSchId));
        }
        public void MailForApproveBooking(string name, string date, string login_email_id, string Time, int? EmpSchId)
        {
            //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                string mailTo = login_email_id;
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "App" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                int LangId = data.CCTSP_SchoolMaster.Lang_id.Value;
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                    var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 5 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
                    var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
                    var LangDate = date.Replace(Monthname, monthlanguagechange);
                    string text;
                    if (data.ItenName == "Default")
                    {
                        text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        text = data.SectionDesc;
                    }
                    DatabaseBody = text;

                    if (DatabaseBody.IndexOf("«date & time & product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "«date & time & product name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"date & time & product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time & product name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«date & time & product name»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "«date & time & product name»", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«date & time»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«date & time»", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«date & time\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"XappointtimeX\"", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XappointtimeX»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX»", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX\"", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", LangDate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br>Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);

                    var EmailData = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email" && (c.Order_id == 1 || c.Order_id == 2) && c.Lang_id == LangId).ToList();
                    string CancelPath = link + "/Reservation/CancelBookingAlert?BookingId=" + EmpSchId;
                    string UrlFormat = "<br><br>" + EmailData.Where(c => c.Order_id == 1).Select(c => c.Value).FirstOrDefault() + " <a href=http://" + CancelPath + ">" + EmailData.Where(c => c.Order_id == 2).Select(c => c.Value).FirstOrDefault() + "</a>";
                    DatabaseBody = DatabaseBody + UrlFormat;
                    DateTime DateTimeBook = new DateTime();
                    DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
                    // New SendGrid Email Code
                    CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                    #region CodeForReminder
                    Models.AlertAPIModel Alert = new Models.AlertAPIModel();
                    string UserNameShop = ConfigurationManager.AppSettings["EmailUserName"];
                    string PasswordShop = ConfigurationManager.AppSettings["EmailPassword"];
                    var ReminderCheck = spa.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 120 && c.CCTSP_CategoryDetails.CatgType == "Rem").FirstOrDefault();
                    string Reminder;
                    if (ReminderCheck.ItenName == "Default")
                    {
                        Reminder = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == ReminderCheck.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        Reminder = ReminderCheck.SectionDesc;
                    }
                    if (Reminder != null)
                    {

                        var BookingInformation = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        var ClientInformation = spa.CCTSP_User.Where(c => c.UserId == BookingInformation.CLIENT_UserId).FirstOrDefault();

                        if (Reminder.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "\"date & time & product & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "“date & time & product name & shopname”", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "\"Product, date & time & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "shop name", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                        if (Reminder.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            Reminder = Regex.Replace(Reminder, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (Reminder.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            Reminder = Regex.Replace(Reminder, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (Reminder.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            Reminder = Regex.Replace(Reminder, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (Reminder.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "«Date»", LangDate, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "«Date\"", LangDate, RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (Reminder.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            Reminder = Regex.Replace(Reminder, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        Reminder = HttpUtility.UrlEncode(Reminder);

                        Alert.username = UserNameShop;
                        Alert.Password = PasswordShop;
                        Alert.ToWhom = name;
                        Alert.EmailId = ClientInformation.EmailId;

                        #region Ankit
                        string phoneno;
                        if (ClientInformation.PhoneNo.Count() > 3)
                        {

                            if (ClientInformation.PhoneNo.ToString().ElementAtOrDefault(0) == '0' && ClientInformation.PhoneNo.ToString().ElementAtOrDefault(1) == '0')
                            {
                                phoneno = ClientInformation.PhoneNo;
                                Alert.PhoneNo = "0041" + phoneno.Substring(2); ;
                            }
                            else if (ClientInformation.PhoneNo.ToString().ElementAtOrDefault(0) == '0')
                            {
                                phoneno = ClientInformation.PhoneNo;
                                Alert.PhoneNo = "0041" + phoneno.Substring(1); ;
                            }
                            else
                            {
                                Alert.PhoneNo = "0041" + ClientInformation.PhoneNo;
                            }
                        }
                        #endregion

                        else
                        {
                            Alert.PhoneNo = ClientInformation.PhoneNo;
                        }
                        Alert.DateTime = DateTimeBook.AddHours(-12).ToString("yyyy/MM/dd HH:mm:ss");
                        Alert.type_id = 2;
                        Alert.greetingMessage = "Reminder";
                        Alert.display_name = true;
                        Alert.bodyMessage = Reminder;
                        var SMSMessageCheck = spa.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 121 && c.CCTSP_CategoryDetails.CatgType == "Rem").FirstOrDefault();
                        string SMSMessage;
                        if (SMSMessageCheck.ItenName == "Default")
                        {
                            SMSMessage = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == SMSMessageCheck.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                        }
                        else
                        {
                            SMSMessage = SMSMessageCheck.SectionDesc;
                        }

                        if (SMSMessage.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "\"date & time & product & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "“date & time & product name & shopname”", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "\"Product, date & time & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "shop name", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                        if (SMSMessage.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            SMSMessage = Regex.Replace(SMSMessage, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (SMSMessage.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            SMSMessage = Regex.Replace(SMSMessage, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (SMSMessage.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            SMSMessage = Regex.Replace(SMSMessage, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (SMSMessage.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "«Date»", LangDate, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "«Date\"", LangDate, RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (SMSMessage.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            SMSMessage = Regex.Replace(SMSMessage, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);
                        SMSMessage = HttpUtility.UrlEncode(SMSMessage);

                        Alert.Birthday_wish = SMSMessage;
                        Alert.endMessage = "Thanks";
                        Alert.endName = ownername.CCTSP_SchoolMaster.SchlName;
                        AlertSPA.AlertAPI(Alert);
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {

            }
        }

        public async Task MultipleEmail(string IdList)
        {
            await Task.Run(() => SendMultipleEmail(IdList));
        }
        public void SendMultipleEmail(string IdList)
        {
            var BookingIdList = IdList.Split(' ').Select(i => int.Parse(i)).ToList();
            var BookingInfoList = (from d in spa.SPA_EmployeeScheduler where BookingIdList.Contains(d.EmpSchDetailsId) && d.SchId == schlId select d);
            var ClientId = BookingInfoList.Select(c => c.CLIENT_UserId).ToList();
            var ClientInfoList = (from e in spa.CCTSP_User where ClientId.Contains(e.UserId) select e).ToList();
            string DatabaseBody = "";
            var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "App" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
            int LangId = data.CCTSP_SchoolMaster.Lang_id.Value;
            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 5 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
            var LangMonthList = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander").ToList();
            string text;
            if (data.ItenName == "Default")
            {
                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
            }
            else
            {
                text = data.SectionDesc;
            }
            if (data != null)
            {
                foreach (var item in BookingInfoList)
                {
                    var ClientInformation = ClientInfoList.Where(d => d.UserId == item.CLIENT_UserId).FirstOrDefault();
                    string date = item.BookingDate;
                    int EmpSchId = item.EmpSchDetailsId;
                    string Time = item.FromSlotMasterId;
                    string mailTo = ClientInformation.EmailId;
                    if (ClientInformation.Email_Service == 3 || ClientInformation.Email_Service == 2)
                    {
                        var name = ClientInformation.Gender + " " + ClientInformation.FirstName + " " + ClientInformation.LastName;
                        var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
                        var monthlanguagechange = LangMonthList.Where(C => C.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
                        var LangDate = date.Replace(Monthname, monthlanguagechange);
                        DatabaseBody = text;
                        if (DatabaseBody.IndexOf("«date & time & product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            DatabaseBody = Regex.Replace(DatabaseBody, "«date & time & product name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"date & time & product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time & product name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«date & time & product name»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            DatabaseBody = Regex.Replace(DatabaseBody, "«date & time & product name»", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«date & time»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«date & time»", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«date & time\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"XappointtimeX\"", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«XappointtimeX»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX»", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX\"", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                                DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                        }

                        if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", LangDate, RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                        if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                        if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br>Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                        if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                        if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                            DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);

                        var EmailData = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email" && (c.Order_id == 1 || c.Order_id == 2) && c.Lang_id == LangId).ToList();
                        string CancelPath = link + "/Reservation/CancelBookingAlert?BookingId=" + EmpSchId;
                        string UrlFormat = "<br><br>" + EmailData.Where(c => c.Order_id == 1).Select(c => c.Value).FirstOrDefault() + " <a href=http://" + CancelPath + ">" + EmailData.Where(c => c.Order_id == 2).Select(c => c.Value).FirstOrDefault() + "</a>";
                        DatabaseBody = DatabaseBody + UrlFormat;
                        DateTime DateTimeBook = new DateTime();
                        DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
                        // New SendGrid Email Code
                        CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                        #region CodeForReminder
                        Models.AlertAPIModel Alert = new Models.AlertAPIModel();
                        string UserNameShop = ConfigurationManager.AppSettings["EmailUserName"];
                        string PasswordShop = ConfigurationManager.AppSettings["EmailPassword"];
                        var ReminderCheck = spa.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 120 && c.CCTSP_CategoryDetails.CatgType == "Rem").FirstOrDefault();
                        string Reminder;
                        if (ReminderCheck.ItenName == "Default")
                        {
                            Reminder = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == ReminderCheck.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                        }
                        else
                        {
                            Reminder = ReminderCheck.SectionDesc;
                        }
                        if (Reminder != null)
                        {

                            var BookingInformation = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                            // var ClientInformation = spa.CCTSP_User.Where(c => c.UserId == BookingInformation.CLIENT_UserId).FirstOrDefault();

                            if (Reminder.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "\"date & time & product & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "“date & time & product name & shopname”", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "\"Product, date & time & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "shop name", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                            if (Reminder.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                                Reminder = Regex.Replace(Reminder, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                            }

                            if (Reminder.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                                Reminder = Regex.Replace(Reminder, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                            }

                            if (Reminder.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                                Reminder = Regex.Replace(Reminder, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                            }

                            if (Reminder.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "«Date»", LangDate, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "«Date\"", LangDate, RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                            if (Reminder.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                Reminder = Regex.Replace(Reminder, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                            Reminder = HttpUtility.UrlEncode(Reminder);

                            Alert.username = UserNameShop;
                            Alert.Password = PasswordShop;
                            Alert.ToWhom = name;
                            Alert.EmailId = ClientInformation.EmailId;

                            #region Ankit
                            string phoneno;
                            if (ClientInformation.PhoneNo.Count() > 3)
                            {

                                if (ClientInformation.PhoneNo.ToString().ElementAtOrDefault(0) == '0' && ClientInformation.PhoneNo.ToString().ElementAtOrDefault(1) == '0')
                                {
                                    phoneno = ClientInformation.PhoneNo;
                                    Alert.PhoneNo = "0041" + phoneno.Substring(2); ;
                                }
                                else if (ClientInformation.PhoneNo.ToString().ElementAtOrDefault(0) == '0')
                                {
                                    phoneno = ClientInformation.PhoneNo;
                                    Alert.PhoneNo = "0041" + phoneno.Substring(1); ;
                                }
                                else
                                {
                                    Alert.PhoneNo = "0041" + ClientInformation.PhoneNo;
                                }
                            }
                            #endregion

                            else
                            {
                                Alert.PhoneNo = ClientInformation.PhoneNo;
                            }
                            Alert.DateTime = DateTimeBook.AddHours(-12).ToString("yyyy/MM/dd HH:mm:ss");
                            Alert.type_id = 2;
                            Alert.greetingMessage = "Reminder";
                            Alert.display_name = true;
                            Alert.bodyMessage = Reminder;
                            var SMSMessageCheck = spa.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 121 && c.CCTSP_CategoryDetails.CatgType == "Rem").FirstOrDefault();
                            string SMSMessage;
                            if (SMSMessageCheck.ItenName == "Default")
                            {
                                SMSMessage = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == SMSMessageCheck.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                            }
                            else
                            {
                                SMSMessage = SMSMessageCheck.SectionDesc;
                            }

                            if (SMSMessage.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "\"date & time & product & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "“date & time & product name & shopname”", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "\"Product, date & time & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "shop name", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                            if (SMSMessage.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                                SMSMessage = Regex.Replace(SMSMessage, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                            }

                            if (SMSMessage.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                                SMSMessage = Regex.Replace(SMSMessage, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                            }

                            if (SMSMessage.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                            {
                                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                                SMSMessage = Regex.Replace(SMSMessage, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                            }

                            if (SMSMessage.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "«Date»", LangDate, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "«Date\"", LangDate, RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                            if (SMSMessage.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                                SMSMessage = Regex.Replace(SMSMessage, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);
                            SMSMessage = HttpUtility.UrlEncode(SMSMessage);

                            Alert.Birthday_wish = SMSMessage;
                            Alert.endMessage = "Thanks";
                            Alert.endName = ownername.CCTSP_SchoolMaster.SchlName;
                            AlertSPA.AlertAPI(Alert);
                            //EmailSend.Approvebooking(name, item.BookingDate, ClientInformation.EmailId, item.FromSlotMasterId, item.EmpSchDetailsId);
                        }
                    }

                    #endregion
                }

            }
        }
        public async Task TemporaryApprove(string name, string date, string login_email_id, int EmpSchDetail, string Time)
        {
            await Task.Run(() => MailForTemporaryApprove(name, date, login_email_id, EmpSchDetail, Time));
        }
        public void MailForTemporaryApprove(string name, string date, string login_email_id, int EmpSchDetail, string Time)
        {
            //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                string mailTo = login_email_id;
                string DatabaseBody = "";

                var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Book" && c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();

                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                    var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 3 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
                    var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
                    var Langtdate = date.Replace(Monthname, monthlanguagechange);
                    string text;
                    if (data.ItenName == "Default")
                    {
                        text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        text = data.SectionDesc;
                    }
                    DatabaseBody = text;
                    if (DatabaseBody.IndexOf("«date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«date & time\"", Langtdate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time\"", Langtdate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«date & time»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«date & time»", Langtdate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.Y /Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y /Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.X /Mr.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X /Mr.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Herr X / Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms. X / Mdm Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms. X / Mdm Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", Langtdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"XappointtimeX\"", Langtdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XappointtimeX»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX»", Langtdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX\"", Langtdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchDetail).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchDetail).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchDetail).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
                    // New SendGrid Email Code
                    CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                }
            }
            catch
            {

            }
        }
        public async Task Cancelbooking(string name, string login_email_id, string Time, string date, int? EmpSchId)
        {
            await Task.Run(() => MailForCancelbooking(name, login_email_id, Time, date, EmpSchId));
        }
        public void MailForCancelbooking(string name, string login_email_id, string Time, string date, int? EmpSchId)
        {
            //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                string mailTo = login_email_id;
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Can" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                    var ownerDetails = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                    var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 4 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
                    var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
                    var Langdate = date.Replace(Monthname, monthlanguagechange);
                    string text;
                    if (data.ItenName == "Default")
                    {
                        text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        text = data.SectionDesc;
                    }
                    DatabaseBody = text;

                    if (DatabaseBody.IndexOf("\"phone number\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"phone number\"", "<br>" + ownerDetails.SchlMobile1 + "<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("\"email address\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"email address\"", ownerDetails.SchlEmail + "<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("\"title\" \"first name\" \"family name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"title\" \"first name\" \"family name\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)

                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.X / chère Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chère Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }



                    if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("“shop name”", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "“shop name”", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", Langdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", Langdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", Langdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
                    DateTime DateTimeBook = new DateTime();
                    DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
                    // New SendGrid Email Code
                    CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                }
            }
            catch
            {

            }
        }
        public async Task CancelbookingCustomer(string name, string login_email_id, string Time, string date, int EmpschId)
        {
            await Task.Run(() => MailForCancelbookingCustomer(name, login_email_id, Time, date, EmpschId));
        }
        public void MailForCancelbookingCustomer(string name, string login_email_id, string Time, string date, int EmpschId)
        {
            //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                string mailTo = login_email_id;
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "CanCus" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                    var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 4 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                    var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
                    var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
                    var Langdate = date.Replace(Monthname, monthlanguagechange);
                    string text;
                    if (data.ItenName == "Default")
                    {
                        text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                    }
                    else
                    {
                        text = data.SectionDesc;
                    }
                    DatabaseBody = text;

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.X / chère Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chère Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                            DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpschId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }

                    if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpschId).FirstOrDefault();
                        DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
                    }



                    if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", Langdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", Langdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", Langdate, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
                    // DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);
                    DateTime DateTimeBook = new DateTime();
                    DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
                    // New SendGrid Email Code
                    CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
                }
            }
            catch { }
        }
        public async Task ForgotPassword(string EmailId, long? UserId)
        {
            await Task.Run(() => MailForForgotPassword(EmailId, UserId));
        }
        public void MailForForgotPassword(string EmailId, long? UserId)
        {
            // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
                var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 8 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
                string mailTo = EmailId;
                string DatabaseBody = "";
                var EmailContent = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "For" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                string text;
                if (EmailContent.ItenName == "Default")
                {
                    text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == EmailContent.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
                }
                else
                {
                    text = EmailContent.SectionDesc;
                }
                string UrlForget = "http://" + link + "/Login/ChangePassword?User=" + UserId;
                DatabaseBody = text + "<br><br><a href=" + UrlForget + ">Click Here</a>";
                if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }

                if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
                        DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
                }
                // New SendGrid Email Code
                CommonXml(mailTo, DatabaseBody, TitleEmail.Value, ownername.CCTSP_SchoolMaster.MainCategory, ownername.CCTSP_SchoolMaster.Lang_id, ownername.CCTSP_SchoolMaster.SchlStudentStrength, null);
            }
            catch
            {
            }
        }
        public async Task EmailForLiveDemo(spa_DemoShop Details, int? Lang)
        {
            await Task.Run(() => LiveDemoEmail(Details, Lang));
        }
        public void LiveDemoEmail(spa_DemoShop Details, int? Lang)
        {
            try
            {
                using (spa = new cctspDatabaseEntities22())
                {
                    var LanguageInfo = spa.Language_NewShop.Where(c => c.Page_Name == "Contact_Email" && c.Lang_id == Lang && c.col2 == "A").Select(c => new Models.LanguageNewShop
                    {
                        Lang_id = c.Lang_id,
                        English_Label = c.English_Label,
                        Page_Name = c.Page_Name,
                        Value = c.Value,
                        Order_id = c.Order_id
                    }).ToList();
                    string subject = LanguageInfo.Where(c => c.Order_id == 1).Select(c => c.Value).FirstOrDefault();/*"Live Demo As Per"*/
                    string Content = LanguageInfo.Where(c => c.Order_id == 2).Select(c => c.Value).FirstOrDefault();/*"Some One Contact You. These are the following details."*/
                    var shopName = "CLICK-AND-GO";
                    var ShopOwnerName = "Mr Christoph Glatthard";

                    string body = "<table>" +
                                       "<tbody>" +
                                            "<tr>" +
                                    "<table>" +
                                       "<tbody>" +
                                            "<tr>" +
                                                "<td style = 'font-weight:bold' colspan = '3'>" +
                                                  "<p>" + LanguageInfo.Where(c => c.Order_id == 3).Select(c => c.Value).FirstOrDefault() + ",<br>" + ShopOwnerName + "</p>" +
                                                 "</td>" +
                                                  "</tr>" +
                                                 "<tr>" +
                                                 "<td colspan = '3'>" +
                                                  " <p> " + Content + "</p>" +
                                                " </td>" +
                                                 "</tr>" +
                                                 "<tr>" +
                                                 " <td>" + LanguageInfo.Where(c => c.Order_id == 4).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                                 " <td>:</td>" +
                                                 " <td> " + Details.first_name + " " + Details.LastName + " </td>" +
                                                "  </tr> " +
                                                "<tr>" +
                                                 " <td>" + LanguageInfo.Where(c => c.Order_id == 5).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                                 " <td>:</td>" +
                                                 " <td> " + Details.email + " </td>" +
                                                "  </tr> " +
                                                 "<tr>" +
                                                 " <td>" + LanguageInfo.Where(c => c.Order_id == 6).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                                 " <td>:</td>" +
                                                 " <td> " + Details.mobilenumber + " </td>" +
                                                "  </tr> " +
                                                   "<tr>" +
                                                 " <td>" + LanguageInfo.Where(c => c.Order_id == 7).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                                 " <td>:</td>" +
                                                 " <td> " + Details.ShopType + " </td>" +
                                                "  </tr> " +
                                                  "<tr>" +
                                                 " <td>" + LanguageInfo.Where(c => c.Order_id == 8).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                                 " <td>:</td>" +
                                                 " <td> " + Details.Extra1 + " </td>" +
                                                "  </tr> " +
                                                  "<tr>" +
                                                 " <td>" + LanguageInfo.Where(c => c.Order_id == 9).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                                 " <td>:</td>" +
                                                 " <td> " + Details.Extra2 + " </td>" +
                                                "  </tr> " +
                                                 "</ tbody >" +
                                                 "</ table > " +
                                                 " </tr> " +
                                                 "<tr>" +
                                                 " <td>" +
                                                 " <p style = 'font-weight:bold;line-height:8px' > " + LanguageInfo.Where(c => c.Order_id == 10).Select(c => c.Value).FirstOrDefault() + ",<br><br>" + shopName + "</p>" +
                                                 "</td>" +
                                                   "</ tbody >" +
                                                 "</ table > ";


                    // New SendGrid Email Code
                    //"christoph.glatthard@gmail.com"           
                    var EmailId = "info@click-and-go.ch;bcsbcs109@gmail.com";
                    CommonXml(EmailId, body, subject, null, 1, null, null);
                }

            }
            catch (Exception Ex)
            {
                FunError("PushEmail", "LiveDemoContactEmail", Ex);
            }

        }
        public void FunError(string Controller, string ActionNAame, Exception e)
        {
            SPA_Error ErrorAdd = new SPA_Error();
            ErrorAdd.Action_Name = ActionNAame;
            ErrorAdd.Controller_name = Controller;
            ErrorAdd.ErrorMsg = e.Message;
            ErrorAdd.ReqColumn1 = schlId.ToString();
            ErrorAdd.ReqColumn2 = e.InnerException.Message;
            TimeZoneInfo IndiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime NowDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IndiaZone);
            ErrorAdd.Created_Date = NowDate;
            spa.SPA_Error.Add(ErrorAdd);
            spa.SaveChanges();
        }
        public async Task EmailForContact(spa_contact contact)
        {
            await Task.Run(() => ContactEmail(contact));
        }
        public void ContactEmail(spa_contact contact)
        {
            var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.shopMaster { LangId = c.Lang_id }).FirstOrDefault();
            var LanguageInfo = spa.Language_Label_Detail.Where(c => c.Page_Name == "Contact_Email" && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.LangId).Select(c => new Models.LanguageLabelDetails
            {
                English_Label = c.English_Label,
                Page_Name = c.Page_Name,
                Lang_id = c.Lang_id,
                Value = c.Value,
                Order_id = c.Order_id
            }).ToList();
            string subject = LanguageInfo.Where(c => c.Order_id == 1).Select(c => c.Value).FirstOrDefault(); /*"Contact As Per"*/
            string Content = LanguageInfo.Where(c => c.Order_id == 2).Select(c => c.Value).FirstOrDefault();
            var shopInfo = spa.CCTSP_User.Where(c => c.SchId == contact.schid && c.ActiveStatus == "A" && c.RoleId == 1).FirstOrDefault();
            var ShopOwnerName = shopInfo.Gender + " " + shopInfo.FirstName + " " + shopInfo.LastName;
            string body = "<table>" +
                               "<tbody>" +
                                    "<tr>" +
                            "<table>" +
                               "<tbody>" +
                                    "<tr>" +
                                        "<td style = 'font-weight:bold'>" +
                                          "<p>" + LanguageInfo.Where(c => c.Order_id == 3).Select(c => c.Value).FirstOrDefault() + "," + ShopOwnerName + "</p>" +
                                         "</td>" +
                                         "<td></td>" +
                                         "<td></td>" +
                                          "</tr>" +
                                         "<tr>" +
                                         "<td colspan = '3'>" +
                                          " <p> " + Content + "</p>" +
                                        " </td>" +
                                         "</tr>" +
                                         "<tr>" +
                                         " <td>" + LanguageInfo.Where(c => c.Order_id == 4).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + contact.FirstName + " </td>" +
                                        "  </tr> " +
                                        "<tr>" +
                                         " <td>" + LanguageInfo.Where(c => c.Order_id == 5).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + contact.Email + " </td>" +
                                        "  </tr> " +
                                         "<tr>" +
                                         " <td>" + LanguageInfo.Where(c => c.Order_id == 6).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + contact.communication + " </td>" +
                                        "  </tr> " +
                                         "</ tbody >" +
                                         "</ table > " +
                                         " </tr> " +
                                         "<tr>" +
                                         " <td>" +
                                         " <p style = 'font-weight:bold;line-height:8px' > " + LanguageInfo.Where(c => c.Order_id == 7).Select(c => c.Value).FirstOrDefault() + ",<br><br>" + shopInfo.CCTSP_SchoolMaster.SchlName + "</p>" +
                                         "</td>" +
                                           "</ tbody >" +
                                         "</ table > ";


            // New SendGrid Email Code
            CommonXml(shopInfo.CCTSP_SchoolMaster.SchlEmail, body, subject, shopInfo.CCTSP_SchoolMaster.MainCategory, shopInfo.CCTSP_SchoolMaster.Lang_id, shopInfo.CCTSP_SchoolMaster.SchlStudentStrength, null);

        }
        public async Task EmailToShopOwner(Models.ConfirmModel BookingDetails)
        {
            await Task.Run(() => ShopOwnerEmail(BookingDetails));
        }
        public async Task SendEmail(Models.Registration RegistrationDetails, int ShopId, int LangId)
        {
            await Task.Run(() => CommonEmail(RegistrationDetails, ShopId, LangId));
        }
        public void CommonEmail(Models.Registration RegistrationDetails, int ShopId, int LangId)
        {

            string EmailId = RegistrationDetails.emailid;
            int language = RegistrationDetails.Langid;
            var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == ShopId).FirstOrDefault();
            string schlid = schlId.ToString();
            var LanguageInfo = spa.Language_NewShop.Where(c => c.Page_Name == "Registration_Page" && c.col2 == "A" && c.Schid == schlid && c.col1 == "Registration_Email" && c.Lang_id == LangId).ToList();
            var Databasebody = LanguageInfo.Where(c => c.Order_id == 41).Select(c => c.Value).FirstOrDefault();
            string subject = LanguageInfo.Where(c => c.Order_id == 47).Select(c => c.Value).FirstOrDefault();
            if (Databasebody.IndexOf("\"FirstName\"", StringComparison.OrdinalIgnoreCase) > 0)
                Databasebody = Regex.Replace(Databasebody, "\"FirstName\"", RegistrationDetails.FirstName, RegexOptions.IgnoreCase);
            if (Databasebody.IndexOf("\"LastName\",", StringComparison.OrdinalIgnoreCase) > 0)
                Databasebody = Regex.Replace(Databasebody, "\"LastName\",", RegistrationDetails.FamilyName + "<br><br>", RegexOptions.IgnoreCase);
            if (Databasebody.IndexOf("\"LastName\" ", StringComparison.OrdinalIgnoreCase) > 0)
                Databasebody = Regex.Replace(Databasebody, "\"LastName\" ", RegistrationDetails.FamilyName + "<br><br>", RegexOptions.IgnoreCase);
            // New SendGrid Email Code
            CommonXml(EmailId, Databasebody, subject, ShopInfo.MainCategory, LangId, ShopInfo.SchlStudentStrength, null);
        }
        public void ShopOwnerEmail(Models.ConfirmModel BookingDetails)
        {
            try
            {

                var ShopOwnerInfo = spa.CCTSP_User.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.RoleId == 1).FirstOrDefault();
                var languageList = spa.Language_Label_Detail.Where(c => (c.Page_Name == "Reservation_update" || c.Page_Name == "Shop_Owner_Text") && c.ActiveStatus == "A" && c.Lang_id == ShopOwnerInfo.CCTSP_SchoolMaster.Lang_id).ToList();
                var EmailTextLang = languageList.Where(c => c.Page_Name.ToLower() == "shop_owner_text").ToList();
                var TextFieldLang = languageList.Where(c => c.Page_Name.ToLower() == "reservation_update").ToList();
                string subject = EmailTextLang.Where(c => c.Order_id == 33).Select(c => c.Value).FirstOrDefault();
                // string ProjectName = "CLICK-AND-GO.com";
                string Content = EmailTextLang.Where(c => c.Order_id == 32).Select(c => c.Value).FirstOrDefault();
                var Status = EmailTextLang.Where(c => c.Order_id == 11).Select(c => c.Value).FirstOrDefault();
                string UrlFormat = "";
                if (ShopOwnerInfo.CCTSP_SchoolMaster.BookingApproval == "YES")
                {
                    Status = EmailTextLang.Where(c => c.Order_id == 16).Select(c => c.Value).FirstOrDefault();
                    string CancelPath = HttpContext.Current.Request.Url.Host + "/Reservation/AcceptBookingAlert?BookingId=" + BookingDetails.EmpSchDetailsId;
                    UrlFormat = "<br><br>" + EmailTextLang.Where(c => c.Order_id == 36).Select(c => c.Value).FirstOrDefault() + " <a href=http://" + CancelPath + ">" + EmailTextLang.Where(c => c.Order_id == 37).Select(c => c.Value).FirstOrDefault() + "</a>";
                }
                if (Content.IndexOf("@status", StringComparison.OrdinalIgnoreCase) > 0)
                    Content = Regex.Replace(Content, "@status", Status, RegexOptions.IgnoreCase);
                string body = "<table>" +
                               "<tbody>" +
                                    "<tr>" +
                            "<table>" +
                               "<tbody>" +
                                    "<tr>" +
                                        "<td style = 'font-weight:bold'>" +
                                          "<p>  " + EmailTextLang.Where(c => c.Order_id == 34).Select(c => c.Value).FirstOrDefault() + " " + ShopOwnerInfo.FirstName + " " + ShopOwnerInfo.LastName + "</p>" +
                                         "</td>" +
                                         "<td></td>" +
                                         "<td></td>" +
                                          "</tr>" +
                                         "<tr>" +
                                         "<td colspan = '3'>" +
                                          " <p> " + Content + "</p>" +
                                        " </td>" +
                                         "</tr>" +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 5).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + BookingDetails.FirstName + " " + BookingDetails.LastName + " </td>" +
                                        "  </tr> " +
                                        "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 13).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + BookingDetails.ClientName + " " + BookingDetails.ClientLastName + " </td>" +
                                        "  </tr> " +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 14).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + BookingDetails.ClientPhoneNo + " </td>" +
                                        "  </tr> " +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 15).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td>" + BookingDetails.ClientEmail + "</td>" +
                                        "  </tr> " +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 4).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td>" + BookingDetails.BookingDate + "</td>" +
                                        "  </tr> " +
                                        "<tr>" +
                                         " <td> " + TextFieldLang.Where(c => c.Order_id == 3).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + BookingDetails.ProductName + "</td>" +
                                        "  </tr> " +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 18).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + BookingDetails.Product_price + "</td>" +
                                        "  </tr> " +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 6).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td> " + BookingDetails.FromSlotMasterId + "</td>" +
                                        " </tr> " +
                                         "<tr>" +
                                         " <td>" + TextFieldLang.Where(c => c.Order_id == 7).Select(c => c.Value).FirstOrDefault() + "</td>" +
                                         " <td>:</td>" +
                                         " <td>" + BookingDetails.ToSlotMasterId + "</td>" +
                                        " </tr> " +
                                         "</ tbody >" +
                                         "</ table > " +
                                         " </tr> " +
                                         "<tr>" +
                                         " <td>" +
                                         " <p style = 'font-weight:bold;line-height:8px' > " + EmailTextLang.Where(c => c.Order_id == 35).Select(c => c.Value).FirstOrDefault() + "<br><br>" + ShopOwnerInfo.CCTSP_SchoolMaster.SchlName + "</p>" +
                                         "</td>" +
                                          " </tr> " +
                                           "<tr>" +
                                         " <td>" + UrlFormat + "</td>" +
                                          " </tr> " +
                                           "</ tbody >" +
                                         "</ table > ";

                // New SendGrid Email Code
                CommonXml(ShopOwnerInfo.CCTSP_SchoolMaster.SchlEmail, body, subject, ShopOwnerInfo.CCTSP_SchoolMaster.MainCategory, ShopOwnerInfo.CCTSP_SchoolMaster.Lang_id, ShopOwnerInfo.CCTSP_SchoolMaster.SchlStudentStrength, null);
            }
            catch
            {


            }
        }
        public async Task EmailWithNewsletter(List<string> EmailId, int SchlId, string Content, List<string> Attachlinks, List<string> FileNames, string Subject)
        {
            await Task.Run(() => SendNewsletter(EmailId, SchlId, Content, Attachlinks, FileNames, Subject));
        }
        public void SendNewsletter(List<string> EmailId, int schlId, string Content, List<string> Attachlinks, List<string> FileNames, string Subject)
        {
            try
            {
                var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                List<Models.Email> EmailList = new List<Models.Email>();
                Models.Email Email = new Models.Email();
                var Count = 0;
                foreach (var item in Attachlinks)
                {
                    Email = new Models.Email();
                    Email.pdfLink = item;
                    Email.PDFName = FileNames[Count];
                    EmailList.Add(Email);
                    Count = Count + 1;
                }
                foreach (var item in EmailId)
                {
                    CommonXml(item, Content, Subject, ShopInfo.MainCategory, ShopInfo.Lang_id, ShopInfo.SchlStudentStrength, EmailList);
                }
                //var XmlName = "COMMON_XML";
                //string body = "";
                //XmlDocument xmldocument = null;
                //XmlNode subjectNode = null;
                //XmlNode bodyNode = null;
                //string subject = Subject;
                //var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                //var MainCatgImg = spa.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == ShopInfo.MainCategory).Select(c => c.Email_Image).FirstOrDefault();
                //var LogoText = spa.Language_NewShop.Where(c => c.Page_Name == "Header" && c.Order_id == 14 && c.Lang_id == ShopInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
                //var DefaultImage = "image/spa_email.png";
                //if (ShopInfo.SchlStudentStrength != null && ShopInfo.SchlStudentStrength == 3)
                //    DefaultImage = "/image/doctor.png";
                //var MainImage = MainCatgImg != null ? MainCatgImg : DefaultImage;
                //if (System.IO.File.Exists(HostingEnvironment.MapPath("~/MailMessages/Message2.xml")))
                //{
                //    xmldocument = new XmlDocument();
                //    xmldocument.Load(HostingEnvironment.MapPath("~/MailMessages/Message2.xml"));
                //    subjectNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='" + XmlName + "']/PASSWORD/SUBJECT");
                //    bodyNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='" + XmlName + "']/PASSWORD/BODY");
                //    body = bodyNode.InnerText;

                //    string subjectTemp = subjectNode.InnerText;
                //    if (subjectTemp.Contains("@" + XmlName))
                //        subject = Regex.Replace(subjectTemp, "@" + XmlName, subject + " - CLICK-AND-GO", RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@EmailBody", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@EmailBody", Content, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@Image", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@Image", BaseUrl + MainImage, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@MainUrl", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@MainUrl", ShopUrl, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@LogoText", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@LogoText", LogoText, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@HostUrl", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@HostUrl", "<a href=" + ShopUrl + ">" + ShopUrl + "</a>", RegexOptions.IgnoreCase);

                //    var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/MarketingMail");
                //    // var request = (HttpWebRequest)WebRequest.Create("http://localhost:8635/Test/MarketingMail");
                //    var postData = "EmailTo=" + js.Serialize(EmailId);
                //    postData = postData + "&subject=" + HttpUtility.UrlEncode(Subject);
                //    postData = postData + "&emailCC=" + "";
                //    postData = postData + "&projectName=" + "click-and-go.com";
                //    postData = postData + "&Content=" + HttpUtility.UrlEncode(body);
                //    postData = postData + "&AttachLink=" + js.Serialize(Attachlinks);
                //    postData = postData + "&FileNames=" + js.Serialize(FileNames);
                //    //postData = postData + "&schId=" + schlId;
                //    var data = Encoding.ASCII.GetBytes(postData);
                //    request.Method = "POST";
                //    request.ContentType = "application/x-www-form-urlencoded";
                //    request.ContentLength = data.Length;
                //    using (var stream = request.GetRequestStream())
                //    {
                //        stream.Write(data, 0, data.Length);
                //    }
                //    var LocalResponseEmail = (HttpWebResponse)request.GetResponse();
                //    var ResultEmail = new StreamReader(LocalResponseEmail.GetResponseStream()).ReadToEnd();
                //}
            }
            catch (Exception ex)
            {
            }
        }
        //public async Task EmailInvoice(string EmailId, string Links, string Name, int LangId)
        //{
        //    await Task.Run(() => MailInvoice(EmailId, Links, Name, LangId));
        //}
        //public void MailInvoice(string EmailId, string Links, string Name, int LangId)
        //{
        //    var LangTrans = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && ((c.Page_Name == "Shop_Owner_Text" && c.Order_id == 29) || (c.Page_Name == "Email_Tag" && c.Order_id == 10)) && c.Lang_id == LangId).ToList();
        //    try
        //    {
        //        var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
        //        List<Models.Email> EmailList = new List<Models.Email>();
        //        Models.Email Email = new Models.Email();
        //        Email.pdfLink = Links;
        //        Email.PDFName = Name;
        //        EmailList.Add(Email);
        //        CommonXml(EmailId, LangTrans.Where(c => c.Order_id == 29).Select(c => c.Value).FirstOrDefault(), LangTrans.Where(c => c.Order_id == 10).Select(c => c.Value).FirstOrDefault(), ShopInfo.MainCategory, ShopInfo.Lang_id, ShopInfo.SchlStudentStrength, EmailList);
        //    }
        //    catch { }
        //}
        public async Task EmailInvoice(List<Models.InvoiceCustomerDetails> Info, DateTime CurrentDate, string LinkName, bool isprint, string status, string RStatus)
        {
            await Task.Run(() => MailInvoice(Info, CurrentDate, LinkName, isprint, status, RStatus));
        }
        public void MailInvoice(List<Models.InvoiceCustomerDetails> Info, DateTime CurrentDate, string LinkName, bool isprint, string status, string RStatus)
        {
            try
            {
                var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                var LangTrans = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && ((c.Page_Name == "Shop_Owner_Text" && c.Order_id == 29) || (c.Page_Name == "Email_Tag" && c.Order_id == 10)) && c.Lang_id == ShopInfo.Lang_id).ToList();
                foreach (var Item in Info)
                {
                    var Link = "/PDF/InvoicePrint?InvoiceId=" + Item.Invoice_Id + "&IsPrint=" + isprint + "&Status=" + status + "&schlId=" + schlId + "&RStatus=" + RStatus;
                    Link = LinkName + Link;
                    List<Models.Email> EmailList = new List<Models.Email>();
                    Models.Email Email = new Models.Email();
                    Email.pdfLink = Link;
                    //LangTrans.Where(c => c.Order_id == 10).Select(c => c.Value).FirstOrDefault()
                    if (LangTrans.Where(c => c.Order_id == 10).Count() > 0)
                        Email.PDFName = LangTrans.Where(c => c.Order_id == 10).Select(c => c.Value).FirstOrDefault() + ".pdf";
                    else
                        Email.PDFName = "Invoice.pdf";
                    EmailList.Add(Email);
                    var Databasebody = LangTrans.Where(c => c.Order_id == 29).Select(c => c.Value).FirstOrDefault();
                    bool TxtTabAccess = true;
                    if (ShopInfo.Flow_Id > 1)
                        TxtTabAccess = CheckSubTabAccess("Texts", ShopInfo.Flow_Id.Value);
                    if (TxtTabAccess == false)
                    {
                        if (!string.IsNullOrEmpty(ShopInfo.Invoice_Email_Txt))
                            Databasebody = ShopInfo.Invoice_Email_Txt;
                    }
                    else
                    {
                        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Invoice" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
                        if (data != null)
                        {
                            if (data.ItenName != "Default")
                                Databasebody = data.SectionDesc;
                        }
                    }


                    if (Databasebody.IndexOf("@FirstName", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "@FirstName", Item.FirstName, RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("@LastName", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "@LastName", Item.LastName, RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("@Email", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "@Email", Item.EmailId, RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("@PhoneNumber", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "@PhoneNumber", Item.PhoneNo, RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "votre team", "<br><br> Cordiali saluti,<br>", RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("@shop name", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "@shop name", ShopInfo.SchlName, RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("@shopName", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "@shopName", ShopInfo.SchlName, RegexOptions.IgnoreCase);
                    if (Databasebody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                        Databasebody = Regex.Replace(Databasebody, "\"shop name\"", ShopInfo.SchlName, RegexOptions.IgnoreCase);
                    CommonXml(Item.EmailId, Databasebody, LangTrans.Where(c => c.Order_id == 10).Select(c => c.Value).FirstOrDefault(), ShopInfo.MainCategory, ShopInfo.Lang_id, ShopInfo.SchlStudentStrength, EmailList);
                }

            }
            catch { }
        }
        public async Task EmailWithPdf(string EmailId, int SchlId, string Links, string name, string Initial, int LangId, long? ReseravtionId)
        {
            await Task.Run(() => NewShopSendEmail(EmailId, SchlId, Links, name, Initial, LangId, ReseravtionId));
        }
        public void NewShopSendEmail(string EmailId, int schlId, string Links, string name, string Initial, int LangId, long? ReseravtionId)
        {
            var LangTrans = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && ((c.Page_Name == "Shop_Owner_Text" && c.Order_id == 42) || (c.Page_Name == "Email_Tag" && c.Order_id == 9)) && c.Lang_id == LangId).ToList();
            try
            {
                var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                List<Models.Email> EmailList = new List<Models.Email>();
                Models.Email Email = new Models.Email();
                Email.pdfLink = Links;
                if (LangTrans.Where(c => c.Order_id == 9).Count() > 0)
                    Email.PDFName = LangTrans.Where(c => c.Order_id == 9).Select(c => c.Value).FirstOrDefault() + ".pdf";
                else
                    Email.PDFName = "Prescription.pdf";
                EmailList.Add(Email);
                var Info = new Models.InvoiceCustomerDetails();
                if (ReseravtionId > 0)
                {
                    var InfoQuery = "select a.Firstname,a.LastName,a.EmailId,a.PhoneNo from cctsp_User a " +
                                    "join Spa_EmployeeScheduler b on a.UserId = b.Client_UserId " +
                                    "Where b.EmpSchDetailsId = " + ReseravtionId.Value;
                    Info = spa.Database.SqlQuery<Models.InvoiceCustomerDetails>(InfoQuery).FirstOrDefault();
                }
                var Databasebody = LangTrans.Where(c => c.Order_id == 42).Select(c => c.Value).FirstOrDefault();
                if (Databasebody.IndexOf("@FirstName", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "@FirstName", Info.FirstName, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("@LastName", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "@LastName", Info.LastName, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("@Email", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "@Email", Info.EmailId, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("@PhoneNumber", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "@PhoneNumber", Info.PhoneNo, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("@shop name", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "@shop name", ShopInfo.SchlName, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("@shopName", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "@shopName", ShopInfo.SchlName, RegexOptions.IgnoreCase);
                if (Databasebody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    Databasebody = Regex.Replace(Databasebody, "\"shop name\"", ShopInfo.SchlName, RegexOptions.IgnoreCase);
                CommonXml(EmailId, Databasebody, LangTrans.Where(c => c.Order_id == 9).Select(c => c.Value).FirstOrDefault(), ShopInfo.MainCategory, ShopInfo.Lang_id, ShopInfo.SchlStudentStrength, EmailList);
                //var XmlName = "COMMON_XML";
                //string body = "";
                //XmlDocument xmldocument = null;
                //XmlNode subjectNode = null;
                //XmlNode bodyNode = null;
                //string subject = LangTrans.Where(c => c.Order_id == 9).Select(c => c.Value).FirstOrDefault();
                //var ShopInfo = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
                //var MainCatgImg = spa.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == ShopInfo.MainCategory).Select(c => c.Email_Image).FirstOrDefault();
                //var LogoText = spa.Language_NewShop.Where(c => c.Page_Name == "Header" && c.Order_id == 14 && c.Lang_id == LangId).Select(c => c.Value).FirstOrDefault();
                //var DefaultImage = "image/spa_email.png";
                //if (ShopInfo.SchlStudentStrength != null && ShopInfo.SchlStudentStrength == 3)
                //    DefaultImage = "/image/doctor.png";
                //var MainImage = MainCatgImg != null ? MainCatgImg : DefaultImage;
                //if (System.IO.File.Exists(HostingEnvironment.MapPath("~/MailMessages/Message2.xml")))
                //{
                //    xmldocument = new XmlDocument();
                //    xmldocument.Load(HostingEnvironment.MapPath("~/MailMessages/Message2.xml"));
                //    subjectNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='" + XmlName + "']/PASSWORD/SUBJECT");
                //    bodyNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='" + XmlName + "']/PASSWORD/BODY");
                //    body = bodyNode.InnerText;

                //    string subjectTemp = subjectNode.InnerText;
                //    if (subjectTemp.Contains("@" + XmlName))
                //        subject = Regex.Replace(subjectTemp, "@" + XmlName, subject + " - CLICK-AND-GO", RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@EmailBody", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@EmailBody", LangTrans.Where(c => c.Order_id == 29).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@Image", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@Image", BaseUrl + MainImage, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@MainUrl", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@MainUrl", ShopUrl, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@LogoText", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@LogoText", LogoText, RegexOptions.IgnoreCase);
                //    if (body.IndexOf("@HostUrl", StringComparison.OrdinalIgnoreCase) > 0)
                //        body = Regex.Replace(body, "@HostUrl", "<a href=" + ShopUrl + ">" + ShopUrl + "</a>", RegexOptions.IgnoreCase);
                //    //var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/AttachMail");
                //    //var request = (HttpWebRequest)WebRequest.Create("http://ad.maarkss.com/Home/AttachMail1");
                //    // var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/PDFEmail");
                //    var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/PDFEmail");
                //    var postData = "EmailTo=" + EmailId;
                //    postData = postData + "&subject=" + HttpUtility.UrlEncode(subject);
                //    postData = postData + "&projectName=" + "no-reply@click-and-go.com";
                //    postData = postData + "&Content=" + HttpUtility.UrlEncode(body);
                //    postData = postData + "&Links=" + Links;
                //    postData = postData + "&name=" + name;
                //    postData = postData + "&Initial=" + Initial;
                //    //postData = postData + "&schId=" + schlId;
                //    var data = Encoding.ASCII.GetBytes(postData);
                //    request.Method = "POST";
                //    request.ContentType = "application/x-www-form-urlencoded";
                //    request.ContentLength = data.Length;
                //    using (var stream = request.GetRequestStream())
                //    {
                //        stream.Write(data, 0, data.Length);
                //    }
                //    var LocalResponseEmail = (HttpWebResponse)request.GetResponse();
                //    var ResultEmail = new StreamReader(LocalResponseEmail.GetResponseStream()).ReadToEnd();
                //}

            }
            catch { }
        }
        public void CommonXml(string mailTo, string DatabaseBody, string EmailSubject, long? MainCategory, int? LangId, int? DoctorFlow, List<Models.Email> FileList)
        {
            if (LangId == null || LangId == 0)
                LangId = 1;
            var LogoText = spa.Language_NewShop.Where(c => c.Page_Name == "Header" && c.Order_id == 14 && c.col2 == "A" && c.Lang_id == LangId && c.Schid == "388").Select(c => c.Value).FirstOrDefault();
            var XmlName = "COMMON_XML";
            string body = "";
            XmlDocument xmldocument = null;
            XmlNode subjectNode = null;
            XmlNode bodyNode = null;
            string subject = EmailSubject;
            var MainCatgImg = spa.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == MainCategory.Value).Select(c => c.Email_Image).FirstOrDefault();
            var DefaultImage = "image/spa_email.png";
            if (DoctorFlow != null && DoctorFlow == 3)
                DefaultImage = "/image/doctor.png";
            var MainImage = MainCatgImg != null ? MainCatgImg : DefaultImage;
            if (System.IO.File.Exists(HostingEnvironment.MapPath("~/MailMessages/Message2.xml")))
            {
                xmldocument = new XmlDocument();
                xmldocument.Load(HostingEnvironment.MapPath("~/MailMessages/Message2.xml"));
                subjectNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='" + XmlName + "']/PASSWORD/SUBJECT");
                bodyNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='" + XmlName + "']/PASSWORD/BODY");
                body = bodyNode.InnerText;

                string subjectTemp = subjectNode.InnerText;
                if (subjectTemp.Contains("@" + XmlName))
                    subject = Regex.Replace(subjectTemp, "@" + XmlName, subject + " - CLICK-AND-GO", RegexOptions.IgnoreCase);
                if (body.IndexOf("@EmailBody", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@EmailBody", DatabaseBody, RegexOptions.IgnoreCase);
                if (body.IndexOf("@Image", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@Image", BaseUrl + MainImage, RegexOptions.IgnoreCase);
                if (body.IndexOf("@MainUrl", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@MainUrl", ShopUrl, RegexOptions.IgnoreCase);
                if (body.IndexOf("@LogoText", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@LogoText", LogoText, RegexOptions.IgnoreCase);
                if (body.IndexOf("@HostUrl", StringComparison.OrdinalIgnoreCase) > 0)
                    body = Regex.Replace(body, "@HostUrl", "<a href=" + ShopUrl + ">" + ShopUrl + "</a>", RegexOptions.IgnoreCase);
                SendMail(mailTo, null, null, subject, HttpUtility.UrlEncode(body), FromIns, FileList);
            }
        }
        public bool SendMail(string sendTo, string sendEmailCc, string sendEmailBcc, string subject, string content, string From, List<Models.Email> FileList)
        {
            try
            {
                var ActionName = "Send-Email";
                if (FileList != null)
                    ActionName = "Send-PDF-Email";
                EmailUrl = EmailUrl != null ? EmailUrl : "http://api.click-and-go.in/api/User/";
                var request = (HttpWebRequest)WebRequest.Create(EmailUrl + ActionName);
                var postData = "sendTo=" + sendTo;
                postData = postData + "&sendEmailCc=" + sendEmailCc;
                postData = postData + "&sendEmailBcc=" + sendEmailBcc;
                postData = postData + "&subject=" + HttpUtility.UrlEncode(subject);
                postData = postData + "&content=" + content;
                postData = postData + "&From=" + From;
                if (FileList != null)
                    postData = postData + "&Links=" + js.Serialize(FileList);
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
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }

        //public void Loginmail(string name, string teamName, string login_email_id)
        //{
        //    // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        string mailTo = login_email_id;
        //        string DatabaseBody = "";
        //        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Login" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();

        //        if (data != null)
        //        {
        //            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 1 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            string text;
        //            if (data.ItenName == "Default")
        //            {
        //                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                text = data.SectionDesc;
        //            }
        //            DatabaseBody = text;
        //            if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Herr X / Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Frau Y\"", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms. X / Mdm Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms. X / Mdm Y\"", name, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
        //            DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);

        //            WebRequestToEmailApi(mailTo, "Loginmail", "Maarkss.com", DatabaseBody, data.CCTSP_SchoolMaster.SchlName.ToString(), TitleEmail.Value);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}
        //public void UserRegistration(long UserId, string teamName, string login_email_id)
        //{
        //   // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        var UserInformation = spa.CCTSP_User.Where(c => c.UserId == UserId).FirstOrDefault();
        //        string name = UserInformation.Gender + " " + UserInformation.FirstName + " " + UserInformation.LastName;
        //        string mailTo = login_email_id;
        //        string DatabaseBody = "";

        //        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Reg" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
        //        if (data != null)
        //        {
        //            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 2 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            // var Title = spa.Language_Label_Detail.Where(c => c.Page_Name == "Layout_Client" && c.Order_id == 9 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            string text;
        //            if (data.ItenName == "Default")
        //            {
        //                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                text = data.SectionDesc;
        //            }
        //            DatabaseBody = text;

        //            if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Herr X / Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms. X / Mdm Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms. X / Mdm Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
        //            DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);

        //            WebRequestToEmailApi(mailTo, "UserRegistration", "Maarkss.com", DatabaseBody, data.CCTSP_SchoolMaster.SchlName.ToString(), TitleEmail.Value);
        //        }

        //        #region CodeForReminder
        //        DateTime DTTEMP = DateTime.Parse("0001-01-01");
        //        if (UserInformation.DOB != null && UserInformation.DOB != DTTEMP)
        //        {
        //            var BirthdayWish = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Bir" && c.Activestatus == "A" && c.SchId == schlId && (c.CCTSP_CategoryDetails.CatgId == 120 || c.CCTSP_CategoryDetails.CatgId == 121));
        //            Models.AlertAPIModel Alert = new Models.AlertAPIModel();
        //            Alert.username = "mohammad786.rajpur@gmail.com";
        //            Alert.Password = "qq";
        //            Alert.ToWhom = name;
        //            Alert.EmailId = login_email_id;
        //            Alert.PhoneNo = UserInformation.PhoneNo;
        //            Alert.DateTime = UserInformation.DOB.ToString("yyyy/MM/dd HH:mm:ss");
        //            Alert.Birthday_wish =
        //            Alert.DateTime = null;
        //            Alert.type_id = 1;
        //            Alert.greetingMessage = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault().CCTSP_CategoryDetails.CatgDesc;
        //            Alert.display_name = true;
        //            Alert.bodyMessage = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault().SectionDesc;
        //            Alert.endMessage = "Thanks";
        //            Alert.endName = UserInformation.CCTSP_SchoolMaster.SchlName;
        //            Alert.Birthday_wish = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 121).FirstOrDefault().SectionDesc;
        //            AlertSPA.AlertAPI(Alert);
        //        }
        //        #endregion
        //    }
        //    catch
        //    {

        //    }
        //}
        //public void TemporaryApprove(string name, string date, string login_email_id, int EmpSchDetail, string Time)
        //{
        //    //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        string mailTo = login_email_id;
        //        string DatabaseBody = "";

        //        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Book" && c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();

        //        if (data != null)
        //        {
        //            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 3 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
        //            var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
        //            var Langtdate = date.Replace(Monthname, monthlanguagechange);
        //            string text;
        //            if (data.ItenName == "Default")
        //            {
        //                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                text = data.SectionDesc;
        //            }
        //            DatabaseBody = text;
        //            if (DatabaseBody.IndexOf("«date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«date & time\"", Langtdate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time\"", Langtdate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«date & time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«date & time»", Langtdate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.Y /Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y /Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.X /Mr.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X /Mr.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Herr X / Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms. X / Mdm Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms. X / Mdm Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", Langtdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"XappointtimeX\"", Langtdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XappointtimeX»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX»", Langtdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX\"", Langtdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchDetail).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchDetail).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchDetail).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
        //            DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);

        //            WebRequestToEmailApi(mailTo, "TemporaryApprove", "Maarkss.com", DatabaseBody, data.CCTSP_SchoolMaster.SchlName.ToString(), TitleEmail.Value);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        //public void ApproveBooking(string name, string date, string login_email_id, string Time, int? EmpSchId)
        //{
        //    //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        string mailTo = login_email_id;
        //        string DatabaseBody = "";
        //        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "App" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
        //        int LangId = data.CCTSP_SchoolMaster.Lang_id.Value;
        //        if (data != null)
        //        {
        //            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 5 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
        //            var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
        //            var LangDate = date.Replace(Monthname, monthlanguagechange);
        //            string text;
        //            if (data.ItenName == "Default")
        //            {
        //                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                text = data.SectionDesc;
        //            }
        //            DatabaseBody = text;

        //            if (DatabaseBody.IndexOf("«date & time & product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«date & time & product name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"date & time & product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time & product name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«date & time & product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«date & time & product name»", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"date & time\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«date & time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«date & time»", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«date & time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«date & time\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"XappointtimeX\"", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XappointtimeX»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX»", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XappointtimeX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XappointtimeX\"", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", LangDate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br>Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);

        //            var EmailData = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email" && (c.Order_id == 1 || c.Order_id == 2) && c.Lang_id == LangId).ToList();
        //            string CancelPath = HttpContext.Current.Request.Url.Host + "/Reservation/CancelBookingAlert?BookingId=" + EmpSchId;
        //            string UrlFormat = "<br><br>" + EmailData.Where(c => c.Order_id == 1).Select(c => c.Value).FirstOrDefault() + " <a href=http://" + CancelPath + ">" + EmailData.Where(c => c.Order_id == 2).Select(c => c.Value).FirstOrDefault() + "</a>";
        //            DatabaseBody = DatabaseBody + UrlFormat;
        //            DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);
        //            DateTime DateTimeBook = new DateTime();
        //            DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
        //            WebRequestToEmailApi(mailTo, "Approvebooking", "Maarkss.com", DatabaseBody, data.CCTSP_SchoolMaster.SchlName.ToString(), TitleEmail.Value);
        //            #region CodeForReminder
        //            Models.AlertAPIModel Alert = new Models.AlertAPIModel();
        //            string UserNameShop = ConfigurationManager.AppSettings["EmailUserName"];
        //            string PasswordShop = ConfigurationManager.AppSettings["EmailPassword"];
        //            var ReminderCheck = spa.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 120 && c.CCTSP_CategoryDetails.CatgType == "Rem").FirstOrDefault();
        //            string Reminder;
        //            if (ReminderCheck.ItenName == "Default")
        //            {
        //                Reminder = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == ReminderCheck.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                Reminder = ReminderCheck.SectionDesc;
        //            }
        //            if (Reminder != null)
        //            {

        //                var BookingInformation = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                var ClientInformation = spa.CCTSP_User.Where(c => c.UserId == BookingInformation.CLIENT_UserId).FirstOrDefault();

        //                if (Reminder.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "\"date & time & product & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "“date & time & product name & shopname”", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "\"Product, date & time & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "shop name", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
        //                if (Reminder.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                {
        //                    var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                    Reminder = Regex.Replace(Reminder, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //                }

        //                if (Reminder.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                {
        //                    var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                    Reminder = Regex.Replace(Reminder, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //                }

        //                if (Reminder.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                {
        //                    var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                    Reminder = Regex.Replace(Reminder, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //                }

        //                if (Reminder.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "«Date»", LangDate, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "«Date\"", LangDate, RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //                if (Reminder.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    Reminder = Regex.Replace(Reminder, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //                Reminder = HttpUtility.UrlEncode(Reminder);

        //                Alert.username = UserNameShop;
        //                Alert.Password = PasswordShop;
        //                Alert.ToWhom = name;
        //                Alert.EmailId = ClientInformation.EmailId;

        //                #region Ankit
        //                string phoneno;
        //                if (ClientInformation.PhoneNo.Count() > 3)
        //                {

        //                    if (ClientInformation.PhoneNo.ToString().ElementAtOrDefault(0) == '0' && ClientInformation.PhoneNo.ToString().ElementAtOrDefault(1) == '0')
        //                    {
        //                        phoneno = ClientInformation.PhoneNo;
        //                        Alert.PhoneNo = "0041" + phoneno.Substring(2); ;
        //                    }
        //                    else if (ClientInformation.PhoneNo.ToString().ElementAtOrDefault(0) == '0')
        //                    {
        //                        phoneno = ClientInformation.PhoneNo;
        //                        Alert.PhoneNo = "0041" + phoneno.Substring(1); ;
        //                    }
        //                    else
        //                    {
        //                        Alert.PhoneNo = "0041" + ClientInformation.PhoneNo;
        //                    }
        //                }
        //                #endregion

        //                else
        //                {
        //                    Alert.PhoneNo = ClientInformation.PhoneNo;
        //                }
        //                Alert.DateTime = DateTimeBook.AddHours(-12).ToString("yyyy/MM/dd HH:mm:ss");
        //                Alert.type_id = 2;
        //                Alert.greetingMessage = "Reminder";
        //                Alert.display_name = true;
        //                Alert.bodyMessage = Reminder;
        //                var SMSMessageCheck = spa.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.Group == null && c.CCTSP_CategoryDetails.CatgId == 121 && c.CCTSP_CategoryDetails.CatgType == "Rem").FirstOrDefault();
        //                string SMSMessage;
        //                if (SMSMessageCheck.ItenName == "Default")
        //                {
        //                    SMSMessage = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == SMSMessageCheck.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //                }
        //                else
        //                {
        //                    SMSMessage = SMSMessageCheck.SectionDesc;
        //                }

        //                if (SMSMessage.IndexOf("\"date & time & product & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "\"date & time & product & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("“date & time & product name & shopname”", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "“date & time & product name & shopname”", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("\"Product, date & time & shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "\"Product, date & time & shop name\"", LangDate + " & " + Time.Remove(Time.Length - 3, 3) + " & " + BookingInformation.CCTSP_CategoryDetails.CatgDesc + ownername.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "shop name", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
        //                if (SMSMessage.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                {
        //                    var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                    SMSMessage = Regex.Replace(SMSMessage, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //                }

        //                if (SMSMessage.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                {
        //                    var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                    SMSMessage = Regex.Replace(SMSMessage, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //                }

        //                if (SMSMessage.IndexOf("«product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                {
        //                    var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                    SMSMessage = Regex.Replace(SMSMessage, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //                }

        //                if (SMSMessage.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "\"Date\"", LangDate, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "«Date»", LangDate, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "«Date\"", LangDate, RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //                if (SMSMessage.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                    SMSMessage = Regex.Replace(SMSMessage, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);
        //                SMSMessage = HttpUtility.UrlEncode(SMSMessage);

        //                Alert.Birthday_wish = SMSMessage;
        //                Alert.endMessage = "Thanks";
        //                Alert.endName = ownername.CCTSP_SchoolMaster.SchlName;
        //                AlertSPA.AlertAPI(Alert);
        //            }

        //            #endregion
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}
        //public void Cancelbooking(string name, string login_email_id, string Time, string date, int? EmpSchId)
        //{
        //    //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        string mailTo = login_email_id;
        //        string DatabaseBody = "";
        //        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "Can" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
        //        if (data != null)
        //        {
        //            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //            var ownerDetails = spa.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId).FirstOrDefault();
        //            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 4 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
        //            var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
        //            var Langdate = date.Replace(Monthname, monthlanguagechange);
        //            string text;
        //            if (data.ItenName == "Default")
        //            {
        //                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                text = data.SectionDesc;
        //            }
        //            DatabaseBody = text;

        //            if (DatabaseBody.IndexOf("\"phone number\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"phone number\"", "<br>" + ownerDetails.SchlMobile1 + "<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("\"email address\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"email address\"", ownerDetails.SchlEmail + "<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("\"title\" \"first name\" \"family name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"title\" \"first name\" \"family name\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)

        //                DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.X / chère Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chère Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpSchId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }



        //            if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("“shop name”", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "“shop name”", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", Langdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", Langdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", Langdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);



        //            DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);
        //            DateTime DateTimeBook = new DateTime();
        //            DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
        //            WebRequestToEmailApi(mailTo, "Cancelbooking", "Maarkss.com", DatabaseBody, data.CCTSP_SchoolMaster.SchlName.ToString(), TitleEmail.Value);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}
        //public void CancelbookingCustomer(string name, string login_email_id, string Time, string date, int EmpschId)
        //{
        //    //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        string mailTo = login_email_id;
        //        string DatabaseBody = "";
        //        var data = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "CanCus" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
        //        if (data != null)
        //        {
        //            var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //            var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 4 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //            var Monthname = DateTime.Parse(date).ToString("MMMM", enGB);
        //            var monthlanguagechange = spa.Language_Label_Detail.Where(c => c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Order_id >= 8 && c.Page_Name == "Small_calander" && c.English_Label == Monthname).Select(c => c.Value).FirstOrDefault();
        //            var Langdate = date.Replace(Monthname, monthlanguagechange);
        //            string text;
        //            if (data.ItenName == "Default")
        //            {
        //                text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == data.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //            }
        //            else
        //            {
        //                text = data.SectionDesc;
        //            }
        //            DatabaseBody = text;

        //            if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.Y / Mr.X\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.Y / Mr.X\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Mr.X / Ms. Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Mr.X / Ms. Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Herr X / Liebe Frau Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Herr X / Liebe Frau Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.X / chère Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chère Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Ms.X / chere Md.Y\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Ms.X / chere Md.Y\"", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«customer X»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«customer X»", name + "," + "<br><br>", RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«XshopownerX»", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"XshopownerX\"", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«XshopownerX\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                    DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("\"product name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpschId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"product name\"", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }

        //            if (DatabaseBody.IndexOf("«product name»", StringComparison.OrdinalIgnoreCase) > 0)
        //            {
        //                var ProductName = spa.SPA_EmployeeScheduler.Where(c => c.EmpSchDetailsId == EmpschId).FirstOrDefault();
        //                DatabaseBody = Regex.Replace(DatabaseBody, "product name", ProductName.CCTSP_CategoryDetails.CatgDesc, RegexOptions.IgnoreCase);
        //            }



        //            if (DatabaseBody.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name»", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop name\"", data.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Date\"", Langdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Date»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Date»", Langdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Date\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Date\"", Langdate, RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("\"Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Time»", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Time»", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("«Time\"", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«Time\"", Time.Remove(Time.Length - 3, 3), RegexOptions.IgnoreCase);

        //            if (DatabaseBody.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Kind regards,", "<br><br>Kind regards,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Freundliche Grüsse,", " <br><br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "Meilleures salutations,", "<br><br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);
        //            if (DatabaseBody.IndexOf("votre team", StringComparison.OrdinalIgnoreCase) > 0)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "votre team", "<br><br>votre team<br>", RegexOptions.IgnoreCase);
        //            DatabaseBody = HttpUtility.UrlEncode(DatabaseBody);
        //            DateTime DateTimeBook = new DateTime();
        //            DateTimeBook = DateTime.Parse(date) + TimeSpan.Parse(Time);
        //            WebRequestToEmailApi(mailTo, "Cancelbooking", "Maarkss.com", DatabaseBody, data.CCTSP_SchoolMaster.SchlName.ToString(), TitleEmail.Value);
        //        }
        //    }
        //    catch { }
        //}
        //public void ForgotPassword(string EmailId, long? UserId)
        //{
        //    // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //    try
        //    {
        //        var ownername = spa.CCTSP_User.Where(c => c.SchId == schlId && c.RoleId == 1).FirstOrDefault();
        //        var TitleEmail = spa.Language_Label_Detail.Where(c => c.Page_Name == "Email_Tag" && c.Order_id == 8 && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id).FirstOrDefault();
        //        string mailTo = EmailId;
        //        string DatabaseBody = "";
        //        var EmailContent = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "For" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault();
        //        string text;
        //        if (EmailContent.ItenName == "Default")
        //        {
        //            text = spa.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Lang_id == ownername.CCTSP_SchoolMaster.Lang_id && c.Label_Name == EmailContent.CatgTypeId.ToString() && c.Page_Name == "Shop_Owner_Text").Select(c => c.Value).FirstOrDefault();
        //        }
        //        else
        //        {
        //            text = EmailContent.SectionDesc;
        //        }
        //        string UrlForget = "http://" + HttpContext.Current.Request.Url.Host + "/Login/ChangePassword?User=" + UserId;
        //        DatabaseBody = text + "<a href=" + UrlForget + ">" + UrlForget + "</a>";
        //        if (DatabaseBody.IndexOf("x@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "x@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("\"x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("«x@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("«x@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«x@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("\"shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("shop email@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "shop email@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("«shop email@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("«shop email@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shop email@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("\"shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "\"shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("shopemail@hotmail.com", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "shopemail@hotmail.com", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("«shopemail@hotmail.com»", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com»", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }

        //        if (DatabaseBody.IndexOf("«shopemail@hotmail.com\"", StringComparison.OrdinalIgnoreCase) > 0)
        //        {
        //            if (ownername.CCTSP_SchoolMaster.SchlEmail != null && ownername.CCTSP_SchoolMaster.SchlEmail != string.Empty)
        //                DatabaseBody = Regex.Replace(DatabaseBody, "«shopemail@hotmail.com\"", ownername.CCTSP_SchoolMaster.SchlEmail, RegexOptions.IgnoreCase);
        //        }
        //        WebRequestToEmailApi(mailTo, "ForgotPassword", "Maarkss.com", DatabaseBody, EmailContent.CCTSP_SchoolMaster.SchlName, TitleEmail.Value);
        //    }
        //    catch
        //    {
        //    }
        //}
        //public void ShopOwnerEmail(Models.ConfirmModel BookingDetails)
        //{
        //    try
        //    {
        //        #region EmailData           
        //        string body = "";
        //        XmlDocument xmldocument = null;
        //        XmlNode subjectNode = null;
        //        XmlNode bodyNode = null;
        //        //int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //        var ShopOwnerInfo = spa.CCTSP_User.Where(c => c.SchId == schlId && c.ActiveStatus == "A" && c.RoleId == 1).FirstOrDefault();
        //        var languageList = spa.Language_Label_Detail.Where(c => (c.Page_Name == "Reservation_update" || c.Page_Name == "Shop_Owner_Text") && c.ActiveStatus == "A" && c.Lang_id == ShopOwnerInfo.CCTSP_SchoolMaster.Lang_id).ToList();
        //        var EmailTextLang = languageList.Where(c => c.Page_Name.ToLower() == "shop_owner_text").ToList();
        //        var TextFieldLang = languageList.Where(c => c.Page_Name.ToLower() == "reservation_update").ToList();
        //        string subject = EmailTextLang.Where(c => c.Order_id == 33).Select(c => c.Value).FirstOrDefault();
        //        string ProjectName = "CLICK-AND-GO.com";
        //        string Content = EmailTextLang.Where(c => c.Order_id == 32).Select(c => c.Value).FirstOrDefault();
        //        var Status = EmailTextLang.Where(c => c.Order_id == 11).Select(c => c.Value).FirstOrDefault();
        //        string UrlFormat = "";
        //        if (ShopOwnerInfo.CCTSP_SchoolMaster.BookingApproval == "YES")
        //        {
        //            Status = EmailTextLang.Where(c => c.Order_id == 16).Select(c => c.Value).FirstOrDefault();
        //            string CancelPath = HttpContext.Current.Request.Url.Host + "/Reservation/AcceptBookingAlert?BookingId=" + BookingDetails.EmpSchDetailsId;
        //            UrlFormat = "<br><br>" + EmailTextLang.Where(c => c.Order_id == 36).Select(c => c.Value).FirstOrDefault() + " <a href=http://" + CancelPath + ">" + EmailTextLang.Where(c => c.Order_id == 37).Select(c => c.Value).FirstOrDefault() + "</a>";
        //        }
        //        var LogoImage = "http://tshope.azurewebsites.net/images/Maarkss.png";
        //        if (System.IO.File.Exists(HostingEnvironment.MapPath("~/MailMessages/messages.xml")))
        //        {
        //            xmldocument = new XmlDocument();
        //            xmldocument.Load(HostingEnvironment.MapPath("~/MailMessages/messages.xml"));
        //            subjectNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='REGISTRATIONDETAILS']/PASSWORD/SUBJECT");
        //            bodyNode = xmldocument.SelectSingleNode("//DATAS/DATA[@NAME='REGISTRATIONDETAILS']/PASSWORD/BODY");
        //            body = bodyNode.InnerText;
        //            string subjectTemp = subjectNode.InnerText;
        //            if (subjectTemp.Contains("@REGISTRATIONDETAILS"))
        //                subject = Regex.Replace(subjectTemp, "@REGISTRATIONDETAILS", subject + " - CLICK-AND-GO", RegexOptions.IgnoreCase);
        //            if (Content.IndexOf("@status", StringComparison.OrdinalIgnoreCase) > 0)
        //                Content = Regex.Replace(Content, "@status", Status, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@ShopOwnerName", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@ShopOwnerName", ShopOwnerInfo.FirstName + " " + ShopOwnerInfo.LastName, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@EmployeeName", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@EmployeeName", BookingDetails.FirstName + " " + BookingDetails.LastName, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@CustomerName", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@CustomerName", BookingDetails.ClientName + " " + BookingDetails.ClientLastName, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@MobileNumber", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@MobileNumber", BookingDetails.ClientPhoneNo, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@EmailId", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@EmailId", BookingDetails.ClientEmail, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BookingDate", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BookingDate", BookingDetails.BookingDate, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BookedProduct", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BookedProduct", BookingDetails.ProductName, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@ProductPrice", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@ProductPrice", BookingDetails.Product_price, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BookingStartTime", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BookingStartTime", BookingDetails.FromSlotMasterId, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BookingEndTime", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BookingEndTime", BookingDetails.ToSlotMasterId, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@Content", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@Content", Content, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@ShopName", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@ShopName", ShopOwnerInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@Image", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@Image", LogoImage, RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@EName_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@EName_Title", TextFieldLang.Where(c => c.Order_id == 5).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@CustName_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@CustName_Title", TextFieldLang.Where(c => c.Order_id == 13).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@MNumber_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, " @MNumber_Title", TextFieldLang.Where(c => c.Order_id == 14).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@EId_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@EId_Title", TextFieldLang.Where(c => c.Order_id == 15).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BDate_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BDate_Title", TextFieldLang.Where(c => c.Order_id == 4).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BProduct_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BProduct_Title", TextFieldLang.Where(c => c.Order_id == 3).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@Price_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@Price_Title", TextFieldLang.Where(c => c.Order_id == 18).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BStart_TimeTitle", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BStart_TimeTitle", TextFieldLang.Where(c => c.Order_id == 6).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@BEndTime_Title", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@BEndTime_Title", TextFieldLang.Where(c => c.Order_id == 7).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@Hello", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@Hello", EmailTextLang.Where(c => c.Order_id == 34).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);
        //            if (body.IndexOf("@Regards", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@Regards", EmailTextLang.Where(c => c.Order_id == 35).Select(c => c.Value).FirstOrDefault(), RegexOptions.IgnoreCase);


        //            if (body.IndexOf("@LinkDetails", StringComparison.OrdinalIgnoreCase) > 0)
        //                body = Regex.Replace(body, "@LinkDetails", UrlFormat, RegexOptions.IgnoreCase);

        //            var request = (HttpWebRequest)WebRequest.Create("http://a.maarkss.ch/Home/ContactMail");
        //            // var request = (HttpWebRequest)WebRequest.Create("http://localhost:8635/Test/ContactMail");
        //            var postData = "Contactdetails=" + HttpUtility.UrlEncode(body);
        //            postData = postData + "&emailSend=" + ShopOwnerInfo.CCTSP_SchoolMaster.SchlEmail;
        //            postData = postData + "&subject=" + subject;
        //            postData = postData + "&ProjectName=" + ProjectName;
        //            var data = Encoding.ASCII.GetBytes(postData);
        //            request.Method = "POST";
        //            request.ContentType = "application/x-www-form-urlencoded";
        //            request.ContentLength = data.Length;

        //            using (var stream = request.GetRequestStream())
        //            {
        //                stream.Write(data, 0, data.Length);
        //            }
        //            var response = (HttpWebResponse)request.GetResponse();
        //            var responsestring = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //        }
        //        #endregion
        //    }
        //    catch { }
        //}
        public void WebRequestToEmailApi(string emailid, string header, string ProjectName, string DatabaseBody, string ShopName, string TitleHead)
        {
            // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                var Logotext = "CLICK-AND-GO";
                var Image = HttpContext.Current.Request.Url.Host + "/images/Maarkss.png";
                var LogoImage = "";
                if (!Image.Contains("localhost") && Image != "" && Image != null)
                {
                    LogoImage = "http://" + HttpContext.Current.Request.Url.Host + "/images/Maarkss.png";
                }
                else
                {
                    LogoImage = "http://tshope.azurewebsites.net/images/Maarkss.png";
                }
                StringBuilder sb = null;
                sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"emailid\":\"" + emailid + "\",");
                sb.Append("\"header\":\"" + HttpUtility.UrlEncode(header) + "\",");
                sb.Append("\"ProjectName\":\"" + "CLICK-AND-GO.com" + "\",");
                sb.Append("\"DatabaseBody\":\"" + DatabaseBody + "\",");
                sb.Append("\"ShopName\":\"" + HttpUtility.UrlEncode(ShopName) + "\",");
                sb.Append("\"TitleHead\":\"" + TitleHead + "\",");
                sb.Append("\"LogoText\":\"" + Logotext + "\",");
                sb.Append("\"LogoImage\":\"" + LogoImage + "\",");
                sb.Append("}");
                // string url = "http://a.maarkss.ch/Home/PushEmail?emailid=" + emailid + "&header=" + header + "&ProjectName=" + "smartshop.com" + "&DatabaseBody=" + DatabaseBody + "&ShopName=" + ShopName + "&TitleHead=" + TitleHead;
                string url = "http://a.maarkss.ch/Home/PushEmail1?EmailData=" + sb.ToString();
                var task = MakeAsyncRequest(url, "text/html");
                var query = "update CCTSP_SchoolMaster set total_email=total_email+1 where SchlId=" + schlId;
                spa.Database.ExecuteSqlCommand(query);
                // HttpWebRequest myReq = 
                //(HttpWebRequest)WebRequest.Create(url);
                // HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                // System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                // string responseString = respStreamReader.ReadToEnd();
                // respStreamReader.Close();
                // myResp.Close();
            }
            catch { }

        }
        // Define other methods and classes here
        public static Task<string> MakeAsyncRequest(string url, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = contentType;
            request.Method = WebRequestMethods.Http.Get;
            request.Timeout = 20000;
            request.Proxy = null;
            Task<WebResponse> task = Task.Factory.FromAsync(
                            request.BeginGetResponse,
                            asyncResult => request.EndGetResponse(asyncResult),
                            (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }
        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }
        public void AddBirthdayReminder(long UserId)
        {
            // int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
            try
            {
                var UserInfo = spa.CCTSP_User.Where(c => c.UserId == UserId && c.ActiveStatus == "A" && c.SchId == schlId).FirstOrDefault();
                var BirthdayWish = spa.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CatgType == "bir" && c.Activestatus == "A" && c.Group == null && c.SchId == schlId && (c.CCTSP_CategoryDetails.CatgId == 120 || c.CCTSP_CategoryDetails.CatgId == 121));
                Models.AlertAPIModel Alert = new Models.AlertAPIModel();
                string UserNameShop = ConfigurationManager.AppSettings["EmailUserName"].ToString();
                string PasswordShop = ConfigurationManager.AppSettings["EmailPassword"].ToString();
                Alert.username = UserNameShop;
                Alert.Password = PasswordShop;
                Alert.ToWhom = UserInfo.Gender + " " + UserInfo.FirstName + " " + UserInfo.LastName;
                Alert.EmailId = UserInfo.EmailId;
                Alert.PhoneNo = UserInfo.PhoneNo;
                Alert.DateTime = UserInfo.DOB.ToString("yyyy/MM/dd HH:mm:ss");
                Alert.type_id = 1;
                Alert.greetingMessage = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault().CCTSP_CategoryDetails.CatgDesc;
                Alert.display_name = true;
                string BirthdayWishMessage = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 120).FirstOrDefault().SectionDesc;
                if (BirthdayWishMessage.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage = Regex.Replace(BirthdayWishMessage, "\"shop name\"", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage = Regex.Replace(BirthdayWishMessage, "«shop name»", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage = Regex.Replace(BirthdayWishMessage, "shop name", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage = Regex.Replace(BirthdayWishMessage, "«shop name\"", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);
                Alert.bodyMessage = BirthdayWishMessage;
                Alert.endMessage = "Thanks";
                Alert.endName = UserInfo.CCTSP_SchoolMaster.SchlName;
                string BirthdayWishMessage1 = BirthdayWish.Where(c => c.CCTSP_CategoryDetails.CatgId == 121).FirstOrDefault().SectionDesc;
                if (BirthdayWishMessage1.IndexOf("\"shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "\"shop name\"", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage1.IndexOf("«shop name»", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "«shop name»", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage1.IndexOf("shop name", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "shop name", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage1.IndexOf("«shop name\"", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "«shop name\"", UserInfo.CCTSP_SchoolMaster.SchlName, RegexOptions.IgnoreCase);

                if (BirthdayWishMessage1.IndexOf("Kind regards,", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "Kind regards,", "<br>Kind regards,<br>", RegexOptions.IgnoreCase);
                if (BirthdayWishMessage1.IndexOf("Freundliche Grüsse,", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "Freundliche Grüsse,", " <br> Freundliche Grüsse,<br>", RegexOptions.IgnoreCase);
                if (BirthdayWishMessage1.IndexOf("Meilleures salutations,", StringComparison.OrdinalIgnoreCase) > 0)
                    BirthdayWishMessage1 = Regex.Replace(BirthdayWishMessage1, "Meilleures salutations,", "<br>Meilleures salutations,<br>", RegexOptions.IgnoreCase);

                Alert.Birthday_wish = BirthdayWishMessage1;
                AlertSPA.AlertAPI(Alert);
            }
            catch { }
        }
        public void DynamicEmail(object obj, long? CatgTypeId)
        {
            dynamicClass MCB = new dynamicClass("DynamicEmail");
            var myclass = MCB.CreateObject(new List<Models.DynamicClass>() { new Models.DynamicClass() { FieldName = "FirstName", Datatype = "System.String" }, new Models.DynamicClass() { FieldName = "LastName", Datatype = "System.String" } });
            Type TP = myclass.GetType();
            
        }
    }
}