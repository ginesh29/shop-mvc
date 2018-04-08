using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Models;
using System.Xml;
using System.Configuration;
using SPA.Common;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;

namespace SPA.Controllers
{
    public class TranslationController : Controller
    {
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        int schlId = Convert.ToInt16(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        Function fu = new Function();
        // GET: Translation
        public TranslationController()
        {
            schlId = Convert.ToInt16(fu.GetShopId(link).ToString());
        }
        #region Click-and-go
        public ActionResult Index()
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            if (clickShop.status)
            {
                var TranslationData = SPA.Language_NewShop.Where(c => c.col2 == "A" && c.Schid == clickShop.schId).OrderBy(c => c.Created_date).Distinct().ToList();
                ViewBag.TranslationData = TranslationData;
                return View();
            }
            else
                return RedirectToAction("Error_List", "Error");
        }
        public ActionResult GetSubHeading(string Pagename)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            if (clickShop.status)
            {
                var TranslationData = SPA.Language_NewShop.Where(c => c.col2 == "A" && c.Page_Name == Pagename && c.Schid == clickShop.schId).OrderBy(c => c.Created_date).Distinct().ToList();
                ViewBag.TranslationData = TranslationData;
                return View();
            }
            else
                return RedirectToAction("Error_List", "Error");
        }
        public ActionResult GetDataofSubHeading(string SubHeading, string Page)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            if (clickShop.status)
            {
                var TranslationData = SPA.Language_NewShop.Where(c => c.col2 == "A" && c.Page_Name == Page && c.col1 == SubHeading && c.Schid == clickShop.schId).OrderBy(c => c.Created_date).Distinct().ToList();
                ViewBag.TranslationData = TranslationData;
                ViewBag.TranslationSub = SubHeading;
                ViewBag.TranslationPage = Page;
                return View();
            }
            else
                return RedirectToAction("Error_List", "Error");
        }
        public JObject EditText(long Id)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            JObject send = null;
            StringBuilder sb = null;
            string TitleHead = "";
            var GetData = SPA.Language_NewShop.Where(c => c.Lang_Newshop_Id == Id && c.Schid == clickShop.schId).FirstOrDefault();
            var GetAllLangData = SPA.Language_NewShop.Where(c => c.Page_Name == GetData.Page_Name && c.col1 == GetData.col1 && c.Order_id == GetData.Order_id && c.Schid == clickShop.schId && c.Catg_Sep == GetData.Catg_Sep).ToList();
            sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"EngText\":\"" + GetAllLangData.Where(c => c.Lang_id == 1).Select(c => c.Value).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            sb.Append("\"FrText\":\"" + GetAllLangData.Where(c => c.Lang_id == 3).Select(c => c.Value).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            sb.Append("\"deText\":\"" + GetAllLangData.Where(c => c.Lang_id == 2).Select(c => c.Value).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            if (GetAllLangData.Select(c => c.Catg_Sep).FirstOrDefault() != null)
            {
                sb.Append("\"TitleAdd\":\"" + GetAllLangData.Select(c => c.Catg_Sep).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            }
            else
            {
                sb.Append("\"TitleAdd\":\"" + TitleHead + "\",");
            }

            sb.Append("\"Id\":\"" + Id + "\"");
            sb.Append("}");
            send = JObject.Parse(sb.ToString());
            return send;
        }

        public JObject EditImage(long Id)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            JObject send = null;
            StringBuilder sb = null;
            var GetData = SPA.Language_NewShop.Where(c => c.Lang_Newshop_Id == Id && c.Schid == clickShop.schId).FirstOrDefault();
            var GetAllLangData = SPA.Language_NewShop.Where(c => c.Page_Name == GetData.Page_Name && c.Order_id == GetData.Order_id && c.col1 == GetData.col1 && c.Schid == clickShop.schId && c.Catg_Sep == GetData.Catg_Sep).ToList();
            sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"EngImg\":\"" + GetAllLangData.Where(c => c.Lang_id == 1).Select(c => c.Image_Url).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            sb.Append("\"FrImg\":\"" + GetAllLangData.Where(c => c.Lang_id == 3).Select(c => c.Image_Url).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            sb.Append("\"deImg\":\"" + GetAllLangData.Where(c => c.Lang_id == 2).Select(c => c.Image_Url).FirstOrDefault().ToString().Replace("\"", "'") + "\",");
            sb.Append("\"Id\":\"" + Id + "\"");
            sb.Append("}");
            send = JObject.Parse(sb.ToString());
            return send;
        }

        [HttpPost]
        [ValidateInput(false)]
        public void AddText(FormCollection user)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            List<Language_NewShop> shopLang = new List<Language_NewShop>();
            long Id = Convert.ToInt64(user["IdLang"]);
            var TextData = SPA.Language_NewShop.Where(c => c.Lang_Newshop_Id == Id && c.Schid == clickShop.schId).FirstOrDefault();
            var GetAllLangData = SPA.Language_NewShop.Where(c => c.Page_Name == TextData.Page_Name && c.col1 == TextData.col1 && c.Order_id == TextData.Order_id && c.Schid == clickShop.schId && c.Catg_Sep == TextData.Catg_Sep).ToList();
            foreach (var item in GetAllLangData)
            {
                if (item.Lang_id == 1)
                {
                    item.Value = user["EngTxt"];
                    item.Catg_Sep = user["TransTitle"].ToString();
                }

                if (item.Lang_id == 2)
                {
                    item.Value = user["deTxt"];
                    item.Catg_Sep = user["TransTitle"].ToString();
                }

                if (item.Lang_id == 3)
                {
                    item.Value = user["frTxt"];
                    item.Catg_Sep = user["TransTitle"].ToString();
                }

                shopLang.Add(item);
            }
            SPA.SaveChanges();
        }
        [ValidateInput(false)]
        public void Addimage(FormCollection user)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            string ImgePathProfile = "";
            List<Language_NewShop> shopLang = new List<Language_NewShop>();
            long DataUserId = Convert.ToInt32(user["IdImage"]);
            var TextData = SPA.Language_NewShop.Where(c => c.Lang_Newshop_Id == DataUserId && c.Schid == clickShop.schId).FirstOrDefault();
            var GetAllLangData = SPA.Language_NewShop.Where(c => c.Page_Name == TextData.Page_Name && c.col1 == TextData.col1 && c.Order_id == TextData.Order_id && c.Schid == clickShop.schId && c.Catg_Sep == TextData.Catg_Sep).ToList();
            HttpPostedFileBase file = Request.Files["ImageUrlFile"];

            if (file.FileName != "")
            {
                var postedFile = file;
                string subPath = "/Images/";
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (postedFile.ContentLength > 16780000)
                {
                    ViewBag.message = "File Size Not Valid";
                }
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    ModelState.AddModelError("photo", "Invalid type. Only the following types (jpg, jpeg, png) are supported.");
                }
                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                    string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                    postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(file.FileName + "." + fileExt));
                    ImgePathProfile = subPath + file.FileName + "." + fileExt;
                }
                else
                {
                    string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                    int numFiles = files1.Length;
                    postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(file.FileName + numFiles + "." + fileExt));
                    ImgePathProfile = subPath + file.FileName + numFiles + "." + fileExt;
                }
                foreach (var item in GetAllLangData)
                {
                    item.Image_Url = ImgePathProfile;
                    shopLang.Add(item);
                }
                SPA.SaveChanges();
                //var query = "update Language_NewShop set Image_Url='" + ImgePathProfile + "' where Lang_Newshop_Id=" + DataUserId;
                //shop.Database.ExecuteSqlCommand(query);
            }
        }
        [ValidateInput(false)]
        public void AddingText(FormCollection Data)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            //pageName
            var PageName = Data["PageData"].ToString();
            //Order Find
            int Order_IdPage = Convert.ToInt16(SPA.Language_NewShop.Where(c => c.Page_Name == PageName && c.Schid == clickShop.schId).Select(c => c.Order_id).ToList().Max()) + 1;
            //SubHeading
            var SubHead = Data["SubHeadingData"].ToString();
            List<Language_NewShop> NewShopData = new List<Language_NewShop>();
            Language_NewShop ShopAdd = new Language_NewShop();
            //EnglishAdd
            ShopAdd.Order_id = Order_IdPage;
            ShopAdd.col1 = SubHead;
            ShopAdd.Page_Name = PageName;
            ShopAdd.English_Label = Convert.ToString(Data["EngAddTxt"]);
            ShopAdd.Value = Convert.ToString(Data["EngAddTxt"]);
            ShopAdd.Lang_id = 1;
            ShopAdd.col2 = "A";
            ShopAdd.col3 = "Dyn";
            ShopAdd.Schid = clickShop.schId;
            ShopAdd.Created_date = DateTime.Now;
            ShopAdd.UpdatedDate = DateTime.Now;
            NewShopData.Add(ShopAdd);
            //FrenchAdd
            ShopAdd = new Language_NewShop();
            ShopAdd.Order_id = Order_IdPage;
            ShopAdd.col1 = SubHead;
            ShopAdd.Page_Name = PageName;
            ShopAdd.English_Label = Convert.ToString(Data["EngAddTxt"]);
            ShopAdd.Value = Convert.ToString(Data["frAddTxt"]);
            ShopAdd.Lang_id = 3;
            ShopAdd.Schid = clickShop.schId;
            ShopAdd.col2 = "A";
            ShopAdd.col3 = "Dyn";
            ShopAdd.Created_date = DateTime.Now;
            ShopAdd.UpdatedDate = DateTime.Now;
            NewShopData.Add(ShopAdd);
            //GermanyAdd
            ShopAdd = new Language_NewShop();
            ShopAdd.Order_id = Order_IdPage;
            ShopAdd.col1 = SubHead;
            ShopAdd.Page_Name = PageName;
            ShopAdd.English_Label = Convert.ToString(Data["EngAddTxt"]);
            ShopAdd.Value = Convert.ToString(Data["deAddTxt"]);
            ShopAdd.Lang_id = 2;
            ShopAdd.col2 = "A";
            ShopAdd.Schid = clickShop.schId;
            ShopAdd.col3 = "Dyn";
            ShopAdd.Created_date = DateTime.Now;
            ShopAdd.UpdatedDate = DateTime.Now;
            NewShopData.Add(ShopAdd);
            SPA.Language_NewShop.AddRange(NewShopData);
            SPA.SaveChanges();
        }
        [ValidateInput(false)]
        public void AddingQuestionAnswerText(FormCollection data)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            var PageName = data["PageDataQuestionAnswer"].ToString();
            int Order_IdPage = Convert.ToInt16(SPA.Language_NewShop.Where(c => c.Page_Name == PageName && c.Schid == clickShop.schId).Select(c => c.Order_id).ToList().Max()) + 1;
            var SubHead = data["SubHeadingDataQuestionAnswer"].ToString();

            List<Language_NewShop> QuestionAnswerText = new List<Language_NewShop>();
            //for adding question in English
            Language_NewShop QA = new Language_NewShop();
            QA.English_Label = data["EngAddQTxt"].ToString();
            QA.Value = data["EngAddQTxt"].ToString();
            QA.col1 = SubHead;
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 1;
            QA.col2 = "A";
            QA.Schid =clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "Q";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);


            //for adding question in German
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddQTxt"].ToString();
            QA.Value = data["deAddQTxt"].ToString();
            QA.col1 = SubHead;
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 2;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "Q";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);

            //for adding question in French
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddQTxt"].ToString();
            QA.Value = data["frAddQTxt"].ToString();
            QA.col1 = SubHead;
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 3;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "Q";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);

            //for adding Question in Italian
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddQTxt"].ToString();
            QA.Value = data["itAddQTxt"].ToString();
            QA.col1 = SubHead;
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 4;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "Q";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);

            //for Adding Answer  in English
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddAnsTxt"].ToString();
            QA.Value = data["EngAddAnsTxt"].ToString();
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 1;
            QA.col1 = SubHead;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "A";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);

            //for Adding Answer  in German
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddAnsTxt"].ToString();
            QA.Value = data["deAddAnsTxt"].ToString();
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 2;
            QA.col1 = SubHead;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "A";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);


            //for Adding Answer  in French
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddAnsTxt"].ToString();
            QA.Value = data["frAddAnsTxt"].ToString();
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 3;
            QA.col1 = SubHead;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "A";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);

            //for Adding Answer in Italian

            //for Adding Answer  in French
            QA = new Language_NewShop();
            QA.English_Label = data["EngAddAnsTxt"].ToString();
            QA.Value = data["itAddAnsTxt"].ToString();
            QA.Order_id = Order_IdPage;
            QA.Page_Name = PageName;
            QA.Lang_id = 4;
            QA.col1 = SubHead;
            QA.col2 = "A";
            QA.Schid = clickShop.schId;
            QA.col3 = "Dyn";
            QA.Catg_Sep = "A";
            QA.Created_date = DateTime.Now;
            QA.UpdatedDate = DateTime.Now;
            QuestionAnswerText.Add(QA);
            SPA.Language_NewShop.AddRange(QuestionAnswerText);
            SPA.SaveChanges();

        }
        [ValidateInput(false)]
        public void Addingimage(FormCollection user)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            string ImgePathProfile = "";
            string PageName = user["PageDataImg"].ToString();
            string SubHeading = user["SubHeadingImg"].ToString();
            int Order_IdPage = Convert.ToInt16(SPA.Language_NewShop.Where(c => c.Page_Name == PageName && c.Schid == clickShop.schId).Select(c => c.Order_id).ToList().Max()) + 1;

            HttpPostedFileBase file = Request.Files["ImageFileSubAdd"];
            if (file.FileName != "")
            {
                var postedFile = file;
                string subPath = "/Images/";
                bool IsExists = System.IO.Directory.Exists(Server.MapPath(subPath));
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (postedFile.ContentLength > 16780000)
                {
                    ViewBag.message = "File Size Not Valid";
                }
                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError("photo", "Invalid type. Only the following types (jpg, jpeg, png) are supported.");
                }
                if (!IsExists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                    string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                    postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(file.FileName + "." + fileExt));
                    ImgePathProfile = subPath + file.FileName + "." + fileExt;
                }
                else
                {
                    string[] files1 = Directory.GetFiles(Server.MapPath(subPath));
                    int numFiles = files1.Length;
                    postedFile.SaveAs(Server.MapPath(subPath) + Path.GetFileName(file.FileName + numFiles + "." + fileExt));
                    ImgePathProfile = subPath + file.FileName + numFiles + "." + fileExt;
                }
                List<Language_NewShop> NewShopData = new List<Language_NewShop>();
                Language_NewShop ShopAdd = new Language_NewShop();
                //EnglishAdd
                ShopAdd.Order_id = Order_IdPage;
                ShopAdd.col1 = SubHeading;
                ShopAdd.Page_Name = PageName;
                ShopAdd.Image_Url = ImgePathProfile;
                ShopAdd.Lang_id = 1;
                ShopAdd.col2 = "A";
                ShopAdd.col3 = "Dyn";
                ShopAdd.Schid = clickShop.schId;
                ShopAdd.Created_date = DateTime.Now;
                ShopAdd.UpdatedDate = DateTime.Now;
                NewShopData.Add(ShopAdd);
                //FrenchAdd
                ShopAdd = new Language_NewShop();
                ShopAdd.Order_id = Order_IdPage;
                ShopAdd.col1 = SubHeading;
                ShopAdd.Page_Name = PageName;
                ShopAdd.Image_Url = ImgePathProfile;
                ShopAdd.Lang_id = 3;
                ShopAdd.col2 = "A";
                ShopAdd.Schid = clickShop.schId;
                ShopAdd.col3 = "Dyn";
                ShopAdd.Created_date = DateTime.Now;
                ShopAdd.UpdatedDate = DateTime.Now;
                NewShopData.Add(ShopAdd);
                //GermanyAdd
                ShopAdd = new Language_NewShop();
                ShopAdd.Order_id = Order_IdPage;
                ShopAdd.col1 = SubHeading;
                ShopAdd.Page_Name = PageName;
                ShopAdd.Image_Url = ImgePathProfile;
                ShopAdd.Lang_id = 2;
                ShopAdd.col2 = "A";
                ShopAdd.Schid = clickShop.schId;
                ShopAdd.col3 = "Dyn";
                ShopAdd.Created_date = DateTime.Now;
                ShopAdd.UpdatedDate = DateTime.Now;
                NewShopData.Add(ShopAdd);
                SPA.Language_NewShop.AddRange(NewShopData);
                //Italian Add
                ShopAdd = new Language_NewShop();
                ShopAdd.Order_id = Order_IdPage;
                ShopAdd.col1 = SubHeading;
                ShopAdd.Page_Name = PageName;
                ShopAdd.Image_Url = ImgePathProfile;
                ShopAdd.Lang_id = 4;
                ShopAdd.col2 = "A";
                ShopAdd.Schid = clickShop.schId;
                ShopAdd.col3 = "Dyn";
                ShopAdd.Created_date = DateTime.Now;
                ShopAdd.UpdatedDate = DateTime.Now;
                NewShopData.Add(ShopAdd);
                SPA.Language_NewShop.AddRange(NewShopData);

                SPA.SaveChanges();
                //var query = "update Language_NewShop set Image_Url='" + ImgePathProfile + "' where Lang_Newshop_Id=" + DataUserId;
                //shop.Database.ExecuteSqlCommand(query);
            }
        }
        public void DeleteData(string orderId, string PageName)
        {
            int order_id = Convert.ToInt16(orderId);
            var BookingUpdate = "update Language_NewShop set col2 ='D' where order_id=" + order_id + " and Page_Name='" + PageName + "' and Schid=" + schlId;
            SPA.Database.ExecuteSqlCommand(BookingUpdate);
        }
        public string AlertMsg(long? id, int? LangId)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            var Lang = SPA.Language_NewShop.Where(c => c.Order_id == id && c.col2 == "A" && c.Lang_id == LangId && c.Page_Name == "AlertMsg" && c.Schid == clickShop.schId).FirstOrDefault().Value;
            return Lang;
        }

        public ActionResult SubEdit(long Id)
        {
            Models.ShopClickandgo clickShop = fu.CheckClickandgo(link, null, schlId);
            var GetData = SPA.Language_NewShop.Where(c => c.Lang_Newshop_Id == Id && c.Schid == clickShop.schId).FirstOrDefault();
            ViewBag.GetAllLangData = SPA.Language_NewShop.Where(c => c.Page_Name == GetData.Page_Name && c.col1 == GetData.col1 && c.Order_id == GetData.Order_id && c.Schid == clickShop.schId && c.Catg_Sep == GetData.Catg_Sep).ToList();
            ViewBag.Id = Id;
            return View();
        }
        #endregion
        public ActionResult Translate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Translate(translateModel Detail)
        {
            for (int i = 0; i <= 2; i++)
            {

                Language_Label_Detail Lang = new Language_Label_Detail();
                Lang.Lang_id = i + 1;
                if (i == 0)
                {
                    Lang.Value = Detail.Value;
                }
                if (i == 1)
                {
                    Lang.Value = Detail.GermanValue;
                }
                if (i == 2)
                {
                    Lang.Value = Detail.FrenchValue;
                }
                Lang.English_Label = Detail.Value.Trim();
                Lang.Page_Name = Detail.Page_Name.Trim();
                Lang.Label_Name = Detail.Label_Name;
                Lang.Order_id = Detail.Order_id;
                Lang.Lan_Source = Detail.Lan_Source;
                Lang.CreateDate = DateTime.Now;
                Lang.UpdatedDate = DateTime.Now;
                Lang.newdata = "1";
                Lang.ActiveStatus = "A";
                var Languageorder = SPA.Language_Label_Detail.Where(c => c.Page_Name == Lang.Page_Name).ToList();
                if (Languageorder != null)
                {
                    if (Languageorder.Where(c => c.English_Label == Lang.English_Label).FirstOrDefault() != null)
                    {
                        Lang.Order_id = Languageorder.Where(c => c.English_Label == Lang.English_Label).Select(c => c.Order_id).FirstOrDefault();
                    }
                    else
                    {
                        Lang.Order_id = Languageorder.Select(c => c.Order_id).ToList().Max() + 1;
                    }
                }
                SPA.Language_Label_Detail.Add(Lang);
                SPA.SaveChanges();
            }
            return View();
        }

        public void CreateNewShop()
        {
            // Max Count of SchId
            var MaxCount = SPA.CCTSP_SchoolMaster.Select(c => c.SchlId).Max() + 1;
            #region GetCountryandGetTimeZone
            string location = "vadodara";
            string Country = "";
            string latitude = "";
            string longitude = "";
            string TimeZone = "";
            string Url = "http://api.geonames.org/search?q=" + location + "&maxRows=1&username=deeppatel387";
            try
            {
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(Url);

                if (objXmlDocument.InnerText.ToString() != "0")
                {
                    XmlDocument mainaddress = new XmlDocument();
                    mainaddress.LoadXml(((objXmlDocument).SelectSingleNode("/geonames/geoname").ParentNode.OuterXml));
                    Country = ((mainaddress).SelectSingleNode("/geonames/geoname/countryName")).InnerText;
                    latitude = ((mainaddress).SelectSingleNode("/geonames/geoname/lat")).InnerText;
                    longitude = ((mainaddress).SelectSingleNode("/geonames/geoname/lng")).InnerText;
                }
            }
            catch
            {
                // Process an error action here if needed 
            }
            try
            {
                string newurl = "https://maps.googleapis.com/maps/api/timezone/xml?location=" + latitude + "," + longitude + "&timestamp=1331161200&key=AIzaSyDIC_E9wYRwrYkj58j_QaBnffFFoK2K51k";
                XmlDocument objXmlDocument = new XmlDocument();
                objXmlDocument.Load(newurl);
                if (objXmlDocument.InnerText.ToString() != "0")
                {
                    XmlDocument mainaddress = new XmlDocument();
                    mainaddress.LoadXml(((objXmlDocument).SelectSingleNode("/TimeZoneResponse").ParentNode.OuterXml));
                    TimeZone = ((mainaddress).SelectSingleNode("/TimeZoneResponse/time_zone_name")).InnerText;
                }
            }
            catch
            {

            }
            #endregion
            //Making New Shop
            var queryU = "insert into CCTSP_SchoolMaster (schlname,schlgroupname,createdon,activestatus,Lang_Id)values('BCSTEST'," + MaxCount + ",getdate(),'A',2)";
            SPA.Database.ExecuteSqlCommand(queryU);
            //Making new Shop Owner
            queryU = "insert into CCTSP_User(SchId,RoleId,LoginName,FirstName,LastName,Password,DOB,phoneno) values (" + MaxCount + ",1,'Admin','BCS','TEST','admin@12345',getdate(),123456789)";
            SPA.Database.ExecuteSqlCommand(queryU);
            //Make all the Category
            queryU = "insert into CCTSP_CategoryDetails ([CatgId],[CatgType],[CatgDesc],[Value],[DomainType],[CreatedOn],[ActiveStatus]) select [CatgId],[CatgType],[CatgDesc],[Value]," + MaxCount + ",[CreatedOn],[ActiveStatus] from CCTSP_CategoryDetails where CatgId=112 and DomainType=248";
            SPA.Database.ExecuteSqlCommand(queryU);
            //Give Schedule to Shop
            queryU = "insert into CCTSP_SchedulerMaster ([SchId],[WeekDay],[StartTime],[Duration],[EndTime],[PeriodNumber],[PeriodType],[GroupId],[CreatedOn],[ActiveStatus],[AcdYearId]) select " + MaxCount + ",[WeekDay],[StartTime],[Duration],[EndTime],[PeriodNumber],[PeriodType],[GroupId],[CreatedOn],[ActiveStatus],[AcdYearId] from CCTSP_SchedulerMaster where SchId = 248 and ActiveStatus = 'A'";
            SPA.Database.ExecuteSqlCommand(queryU);
            //Add Text from other Shop
            //queryU = "insert into CTSP_SolutionMaster ([CatgTypeId],[SectionName],[SectionDesc],[CraetedOn],[Activestatus],[SchId]) select [CatgTypeId],[SectionName],[SectionDesc],[CraetedOn],[Activestatus],"+ MaxCount + " from CTSP_SolutionMaster where sectionname like '%email%' and activestatus='A' and schid=252 and catgtypeid != 0";
            queryU = "insert into CTSP_SolutionMaster ([CatgTypeId],[SectionName],[SectionDesc],[CraetedOn],[Activestatus],[SchId],[itenname]) select [CatgTypeId],[SectionName],[SectionDesc],[CraetedOn],[Activestatus]," + MaxCount + ",[itenname] from CTSP_SolutionMaster where sectionname like '%email%' and activestatus='A' and schid=252 and catgtypeid != 0";
            SPA.Database.ExecuteSqlCommand(queryU);
        }

        public ActionResult ClickgoLanguage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ClickgoLanguage(FormCollection Language)
        {
            try
            {
                Language_NewShop Lang = new Language_NewShop();
                List<Language_NewShop> LangList = new List<Language_NewShop>();
                for (int i = 0; i <= 2; i++)
                {
                    Lang = new Language_NewShop();
                    Lang.Lang_id = i + 1;
                    if (i == 0)
                        Lang.Value = Language["EnglishValue"];
                    if (i == 1)
                        Lang.Value = Language["GermanValue"];
                    if (i == 2)
                        Lang.Value = Language["FrenchValue"];
                    Lang.English_Label = Language["EnglishValue"];
                    Lang.Page_Name = Language["PageName"];
                    Lang.Created_date = DateTime.Now;
                    Lang.UpdatedDate = DateTime.Now;
                    Lang.Schid = "3";
                    Lang.col2 = "A";
                    Lang.col1 = Language["SubText"];
                    Lang.Field1 = "NE";
                    Lang.newdata = "1";
                    var Languageorder = SPA.Language_NewShop.Where(c => c.Page_Name == Lang.Page_Name && c.Schid == "3").ToList();
                    if (Languageorder.Count > 0)
                    {
                        if (Languageorder.Where(c => c.English_Label == Lang.English_Label).FirstOrDefault() != null)
                            Lang.Order_id = Languageorder.Where(c => c.English_Label == Lang.English_Label).Select(c => c.Order_id).FirstOrDefault();
                        else
                            Lang.Order_id = Languageorder.Select(c => c.Order_id).ToList().Max() + 1;
                    }
                    else
                        Lang.Order_id = 1;
                    LangList.Add(Lang);
                }
                SPA.Language_NewShop.AddRange(LangList);
                SPA.SaveChanges();
                return View();
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public void AddAnother()
        {
            Language_Label_Detail Detail = new Language_Label_Detail();
            var data = SPA.Language_Label_Detail.Where(c => c.Value == c.English_Label).ToList();
            foreach (var DataItem in data)
            {
                Detail = new Language_Label_Detail();
                Detail = DataItem;
                Detail.Value = Detail.English_Label;
                SPA.SaveChanges();
            }
        }
        public void AddOrdertoProduct()
        {
            var Data = SPA.CTSP_SolutionMaster.Where(c => c.SchId == 252 && c.Activestatus == "A" && c.SectionName == "ChoosProduct").ToList();
            int index = 1;
            if (Data.Count() != 0)
            {
                foreach (var item in Data)
                {
                    item.OrderData = index;
                    SPA.SaveChanges();
                    index = index + 1;
                }
            }
        }
        public void AddOrderData(long? solution, int order)
        {
            CTSP_SolutionMaster Sol = new CTSP_SolutionMaster();
            Sol = SPA.CTSP_SolutionMaster.Where(c => c.SolutionId == solution).FirstOrDefault();
            Sol.AcdYearId = order;
            SPA.SaveChanges();
        }

    }
}