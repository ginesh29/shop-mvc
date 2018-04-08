using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

namespace SPA.Common
{
    public class PuchSMS
    {
        cctspDatabaseEntities22 spa = new cctspDatabaseEntities22();
        int schId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string link = System.Web.HttpContext.Current.Request.Url.Host;      
        public PuchSMS()
        {
            schId = Convert.ToInt32(GetShopId(link));
        }
        public long GetShopId(string Link)
        {
            long? Shopid = 0;
            if (Link != "localhost" && Link != "devtestspa001.azurewebsites.net" && Link != "click-and-go.co.in" && Link != "click-and-go.ch")
                Shopid = spa.CCTSP_LendingPageMaster.Where(c => c.Azure_Website == Link || c.Original_Website == Link).Select(c => c.Schid).FirstOrDefault();

            if (Shopid == 0 || Shopid == null)
                Shopid = schId;
            return Shopid.Value;
        }
        internal string send(string message, string no)
        {
            try
            {
                HttpWebRequest myReq =
               (HttpWebRequest)WebRequest.Create("http://144.76.180.44/WebSMS/SMSAPI.jsp?username=maarkss&password=Maarkss&sendername=" + ConfigurationManager.AppSettings["SenderID"].ToString() + "&mobileno=" + no + "&message=" + message.Trim());

                HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                string responseString = respStreamReader.ReadToEnd();
                respStreamReader.Close();
                myResp.Close();
                return responseString;
            }
            catch
            {
                return "Can't Send SMS ,Please Contact System Admin";
            }
        }
        public void SendMarketingSms(string Content , List<string> PhoneNumber,string Url, string Country)
        {
            try
            {
                int Count = 0;
                foreach (var item in PhoneNumber)
                {
                    string Number = item;
                    if (Country != "INDIA")
                    {
                        if (Number.Count() > 3)
                        {
                            if (Number.ToString().ElementAtOrDefault(0) == '0' && Number.ToString().ElementAtOrDefault(1) == '0')
                            {
                                Number = "0041" + Number.Substring(2); ;
                            }
                            else if (Number.ToString().ElementAtOrDefault(0) == '0')
                            {
                                Number = "0041" + Number.Substring(1);
                            }
                            else
                                Number = "0041" + Number;
                        }
                    }
                    string DatabaseBody = Url;
                    if (DatabaseBody.IndexOf("@Text", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "@Text", HttpUtility.UrlEncode(Content), RegexOptions.IgnoreCase);
                    if (DatabaseBody.IndexOf("@PhoneNumber", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "@PhoneNumber", Number, RegexOptions.IgnoreCase);
                    HttpWebRequest myReq =
                            (HttpWebRequest)WebRequest.Create(DatabaseBody);
                    HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                    System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                    string responseString = respStreamReader.ReadToEnd();
                    if (responseString.ToLower().Contains("success"))
                        Count = Count + 1;
                    respStreamReader.Close();
                    myResp.Close();
                   
                }
                if(Count>0)
                {
                    var query = "update CCTSP_SchoolMaster set total_sms=total_sms+" + Count + " where SchlId=" + schId;
                    spa.Database.ExecuteSqlCommand(query);
                }                
            }
            catch(Exception Ex)
            {

            }
            
        }
        public bool Loginmail(string name, string teamName, string number)
        {
            try
            {
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CatgTypeId == 11169 && c.Activestatus == "A" && c.Group==null && c.SchId == schId).FirstOrDefault();
                if (data != null)
                {
                    DatabaseBody = data.SectionDesc.Replace("\r\n", "");

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name, RegexOptions.IgnoreCase);

                    int length = DatabaseBody.Length;
                    int messageSize = 150;
                    List<string> Message = new List<string>();
                    for (int i = 0; i < length; i += messageSize)
                    {
                        if (messageSize + i > length)
                            messageSize = length - i;
                        Message.Add(DatabaseBody.Substring(i, messageSize));
                    }
                    foreach (var content in Message)
                    {
                        send(content, number);
                    }
                }               
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UserRegistration(string name, string teamName, string number)
        {
            try
            {
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CatgTypeId == 11163 && c.Activestatus == "A" && c.Group==null && c.SchId == schId).FirstOrDefault();
                if (data != null)
                {
                    DatabaseBody = data.SectionDesc.Replace("\r\n", "");
                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name, RegexOptions.IgnoreCase);

                    int length = DatabaseBody.Length;
                    int messageSize = 150;
                    List<string> Message = new List<string>();


                    for (int i = 0; i < length; i += messageSize)
                    {
                        if (messageSize + i > length)
                            messageSize = length - i;

                        Message.Add(DatabaseBody.Substring(i, messageSize));

                    }
                    foreach (var content in Message)
                    {
                        send(content, number);
                    }
                }
              

                return true;
            }
            catch
            {
                return false;
            }


        }
        public bool Approvebooking(string name, string date, string number)
        {
            try
            {
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CatgTypeId == 11165 && c.Activestatus == "A" && c.Group==null && c.SchId == schId).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schId && c.RoleId == 1).FirstOrDefault();

                    DatabaseBody = data.SectionDesc.Replace("\r\n", "");

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", date, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);

                    int length = DatabaseBody.Length;
                    int messageSize = 150;

                    List<string> Message = new List<string>();


                    for (int i = 0; i < length; i += messageSize)
                    {
                        if (messageSize + i > length)
                            messageSize = length - i;

                        Message.Add(DatabaseBody.Substring(i, messageSize));

                    }

                    foreach (var content in Message)
                    {
                        send(content, number);
                    }
                }
      

                return true;


            }
            catch
            {
                return false;
            }


        }
        public bool Cancelbooking(string name, string date, string number)
        {
            try
            {
                string DatabaseBody = "";

                var data = spa.CTSP_SolutionMaster.Where(c => c.CatgTypeId == 11166 && c.Activestatus == "A" && c.Group==null && c.SchId == schId).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schId && c.RoleId == 1).FirstOrDefault();
                    DatabaseBody = data.SectionDesc.Replace("\r\n", "");

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);
                    int length = DatabaseBody.Length;
                    int messageSize = 150;
                    List<string> Message = new List<string>();
                    for (int i = 0; i < length; i += messageSize)
                    {
                        if (messageSize + i > length)
                            messageSize = length - i;

                        Message.Add(DatabaseBody.Substring(i, messageSize));

                    }

                    foreach (var content in Message)
                    {
                        send(content, number);
                    }

                }

                return true;

            }
            catch
            {
                return false;
            }


        }
        public bool TemporaryApprove(string name, string date, string number)
        {
            try
            {
                List<string> MessageBody = new List<string>();
                string DatabaseBody = "";
                var data = spa.CTSP_SolutionMaster.Where(c => c.CatgTypeId == 11164 && c.Activestatus == "A" && c.Group==null && c.SchId == schId).FirstOrDefault();
                if (data != null)
                {
                    var ownername = spa.CCTSP_User.Where(c => c.SchId == schId && c.RoleId == 1).FirstOrDefault();
                    DatabaseBody = data.SectionDesc.Replace("\r\n", "");

                    if (DatabaseBody.IndexOf("Mr.XXX/Ms.YYY", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "Mr.XXX/Ms.YYY", name, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XappointtimeX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XappointtimeX", date, RegexOptions.IgnoreCase);

                    if (DatabaseBody.IndexOf("XshopownerX", StringComparison.OrdinalIgnoreCase) > 0)
                        DatabaseBody = Regex.Replace(DatabaseBody, "XshopownerX", ownername.FirstName + " " + ownername.LastName, RegexOptions.IgnoreCase);


                    int length = DatabaseBody.Length;
                    int messageSize = 150;

                    List<string> Message = new List<string>();


                    for (int i = 0; i < length; i += messageSize)
                    {
                        if (messageSize + i > length)
                            messageSize = length - i;

                        Message.Add(DatabaseBody.Substring(i, messageSize));

                    }

                    foreach (var content in Message)
                    {
                        send(content, number);
                    }

                }



                return true;



            }
            catch
            {
                return false;
            }


        }
    }
}