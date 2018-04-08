using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SPA.Common
{
    public class Alert
    {
        public void AlertAPI(Models.AlertAPIModel AlertCheck)
        {
            try
            {
                string url = "http://sendmail.maarkss.ch/ConsolidatedMIS/MessageReminder?userName=" + AlertCheck.username + "&password=" + AlertCheck.Password + "&toWhom=" + AlertCheck.ToWhom + "&emailid=" + AlertCheck.EmailId + "&phoneNo=" + AlertCheck.PhoneNo + "&date_time=" + AlertCheck.DateTime + "&type_id=" + AlertCheck.type_id + "&greetingMessage=" + AlertCheck.greetingMessage + "&display_name=" + AlertCheck.display_name + "&bodyMessage=" + AlertCheck.bodyMessage + "&endMessage=" + AlertCheck.endMessage + "&endName=" + AlertCheck.endName + "&birthday_msg="+AlertCheck.Birthday_wish;
                HttpWebRequest myReq =
               (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                string responseString = respStreamReader.ReadToEnd();
                respStreamReader.Close();
                myResp.Close();
            }
            catch
            {
               
            }




            
        }
        



    }
}