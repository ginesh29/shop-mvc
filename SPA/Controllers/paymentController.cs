using System;
using SPA.Common;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace SPA.Controllers
{
    public class paymentController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        Function fu = new Function();
        string StPay = ConfigurationManager.AppSettings["StPayment"];
        string PaymentKey = ConfigurationManager.AppSettings["PaymentKey"];
        int schlId = Convert.ToInt16(ConfigurationManager.AppSettings["ApplicationVariable"]);
        public void GoToPayu(Models.Pay Payu)
        {
            RemotePost myremotepost = new RemotePost();
            string key = "rjQUPktU";
            string salt = "e5iIg1jwi8";
            myremotepost.Url = "https://test.payu.in/_payment";
            if (PaymentKey == "1")
            {
                /*live*/
                key = "hcGk0F53";
                salt = "Uf1awi9Ijx";
                myremotepost.Url = "https://secure.payu.in/_payment";
            }
            myremotepost.Add("key", key);
            string txnid = myremotepost.Generatetxnid();
            myremotepost.Add("txnid", txnid);
            myremotepost.Add("amount", Payu.amount);
            myremotepost.Add("productinfo", Payu.productinfo);
            myremotepost.Add("firstname", Payu.firstname);
            if (Payu.lastname != null)
                myremotepost.Add("lastname", Payu.lastname);
            myremotepost.Add("phone", Payu.phone);
            myremotepost.Add("email", Payu.email);
            myremotepost.Add("surl", Payu.surl);
            myremotepost.Add("furl", Payu.furl);
            string HashForUDF = "";
            for (int o = 0; o <= 9; o++)
            {
                if (Payu.udf1.Count > o)
                {
                    var k = o + 1;
                    myremotepost.Add("udf" + k, Payu.udf1[o]);
                    HashForUDF = HashForUDF + Payu.udf1[o] + "|";
                }
                else
                {
                    HashForUDF = HashForUDF + "|";
                }
            }
            myremotepost.Add("service_provider", "payu_paisa");
            string hashString = key + "|" + txnid + "|" + Payu.amount + "|" + Payu.productinfo + "|" + Payu.firstname + "|" + Payu.email + "|";
            if (HashForUDF != "" && HashForUDF != null)
                hashString = hashString + HashForUDF + salt;
            string hash = myremotepost.Generatehash512(hashString);
            myremotepost.Add("hash", hash);
            myremotepost.Post();
        }
        /*Payment Gateway*/
        public ActionResult getPay()
        {
            if (Session["Registration"] != null && Session["Registration"] != "")
            {
                Models.Registration RegistrationDetails;
                RegistrationDetails = (Models.Registration)Session["Registration"];
                if (RegistrationDetails != null)
                {
                    //Uri uri = HttpContext.Request.Url;
                    //var GetHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                    //Get All Id of one whose Input is not taken
                    var AllCatgDetailId = RegistrationDetails.PaymentDetails_id;
                    //Get All information for one whose UserInput is not taken
                    var Result = fu.getClickPaymentDetails(AllCatgDetailId, RegistrationDetails.Timezone);
                    var TotalAmount = Convert.ToString(Result.Select(c => c.Amount).Sum());
                    DateTime CurrentDate = fu.ZonalDate(null);
                    List<string> UDEFINED = new List<string>();
                    spa_Payment_Gateway PayGateway = new spa_Payment_Gateway()
                    {
                        ActiveStatus = "A",
                        amount_ = TotalAmount,
                        email_ = RegistrationDetails.emailid,
                        firstname_ = RegistrationDetails.FirstName,
                        lastname_ = RegistrationDetails.FamilyName,
                        furl = "/Error/Error_List",
                        surl = "/payment/ResultPay",
                        phone = RegistrationDetails.phoneno,
                        productinfo = "Shop Setup Fees",
                        Created_Date = CurrentDate,
                        Updated_Date = CurrentDate
                    };
                    SPA.spa_Payment_Gateway.Add(PayGateway);
                    SPA.SaveChanges();
                    if (PayGateway.PaymentGatewayId != 0)
                    {
                        UDEFINED.Add(Convert.ToString(PayGateway.PaymentGatewayId));
                        UDEFINED.Add(Convert.ToString(RegistrationDetails.Langid));
                        UDEFINED.Add("/Registration/ThanksPopup?Lang=" + RegistrationDetails.Langid);
                        Models.Pay payu = new Models.Pay()
                        {
                            amount = TotalAmount,
                            firstname = RegistrationDetails.FirstName,
                            email = RegistrationDetails.emailid,
                            furl = PayGateway.furl,
                            surl = PayGateway.surl,
                            productinfo = PayGateway.productinfo,
                            lastname = RegistrationDetails.FamilyName,
                            phone = RegistrationDetails.phoneno,
                            udf1 = UDEFINED,
                            payId = PayGateway.PaymentGatewayId,
                            Lang_id = RegistrationDetails.Langid
                        };
                        GoToPayu(payu);
                    }
                }
            }
            return Redirect("/Home/ThankYou?Lang=1");
        }
        public ActionResult PaymentError()
        {
            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => new Models.ShopMasterDetail
            {
                Lang_id = c.Lang_id,                
            }).FirstOrDefault();
            ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Payment_Error" && c.Lang_id == ShopInfo.Lang_id && c.ActiveStatus == "A").Select(c => new Models.LanguageLabelDetails
            {
                Lang_id = c.Lang_id,
                English_Label = c.English_Label,
                Page_Name = c.Page_Name,
                Order_id = c.Order_id,
                Value = c.Value
            }).ToList();
            return View();
        }

        public ActionResult ResultPay(FormCollection form)
        {
            try
            {
                Uri uri = HttpContext.Request.Url;
                var GetHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                string merc_hash_string = string.Empty;
                string merc_hash = string.Empty;
                string order_id = string.Empty;
                RemotePost myremotepost = new RemotePost();
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                spa_Payment_Gateway paygateway = new spa_Payment_Gateway();
                if (Convert.ToInt64(form["udf1"]) != 0)
                {
                    var PayId = Convert.ToInt64(form["udf1"]);
                    paygateway = SPA.spa_Payment_Gateway.Where(c => c.ActiveStatus == "A" && c.PaymentGatewayId == PayId).FirstOrDefault();
                }
                if (form["status"].ToString() == "success")
                {
                    TempData["status"] = true;
                    paygateway.field4 = "success";
                    int LangId = 1;
                    if (Convert.ToInt32(form["udf2"]) != 0)
                        LangId = Convert.ToInt32(form["udf2"]);
                    SPA.SaveChanges();
                    if (form["udf3"] == null)
                        return Redirect(GetHost);
                    else
                        return Redirect(Convert.ToString(form["udf3"]));
                }
                else
                {
                    TempData["status"] = false;
                    paygateway.field4 = "Failure";
                    SPA.SaveChanges();
                    return RedirectToAction("PaymentError");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error_List", "Error");
            }
        }       
        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();


            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";

            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }

            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();

                System.Web.HttpContext.Current.Response.Write("<html><head>");

                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");

                System.Web.HttpContext.Current.Response.End();
            }
            public string Generatehash512(string text)
            {

                byte[] message = Encoding.UTF8.GetBytes(text);

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] hashValue;
                SHA512Managed hashString = new SHA512Managed();
                string hex = "";
                hashValue = hashString.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }
                return hex;

            }


            public string Generatetxnid()
            {

                Random rnd = new Random();
                string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
                string txnid1 = strHash.ToString().Substring(0, 20);

                return txnid1;
            }
        }
    }
}