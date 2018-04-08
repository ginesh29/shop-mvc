
using SPA.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SPA.Controllers
{
    [checkShop]
    public class FreeCustomerController : Controller
    {
        cctspDatabaseEntities22 SPA = null;
        JavaScriptSerializer js = null;
        int schlId = Convert.ToInt16(ConfigurationManager.AppSettings["ApplicationVariable"]);
        string StPay = ConfigurationManager.AppSettings["StPayment"];
        string ErrorClick = ConfigurationManager.AppSettings["ErrorClick"];
        string link = null;
        Function fu = null;
        public FreeCustomerController()
        {
            SPA = new cctspDatabaseEntities22();
            js = new JavaScriptSerializer();
            fu = new Function();
            link = System.Web.HttpContext.Current.Request.Url.Host;
            schlId = Convert.ToInt16(fu.GetShopId(link).ToString());
        }
        [CustomAutohrize(null)]
        public ActionResult AddMultipleEmployee(Models.EmployeeJsonModel Jsonresult, string Status)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                    //Specialization  
                    int lang = 1;
                    var CommonInfo = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.Lang_Id == lang && (c.CCTSP_CategoryMaster.CatgName == "Specialization" || c.CatgId == 110)).ToList();
                    var Category = CommonInfo.Where(c => c.CCTSP_CategoryMaster.CatgName == "Specialization" && c.ActiveStatus == "A" && c.Lang_Id == lang).OrderBy(c => c.CatgDesc)
                                   .Select(c => new Models.MainCategoryDetails
                                   { MainCategory = c.CatgDesc, MainCatgId = c.CatgTypeId, WorkFlowId = c.Work_Flow_Id }).OrderBy(c => c.MainCategory).ToList();
                    ViewBag.Category = Category;
                    var Default = Category.Where(c => c.WorkFlowId == 1).FirstOrDefault();
                    var SelectedCatg = new List<Models.MainCategoryDetails>();
                    ViewBag.selected = Default;
                    //GenderInfo
                    ViewBag.Client = new SelectList(CommonInfo.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == lang).OrderBy(c => c.CatgType), "CatgType", "CatgType");
                    ViewBag.Status = Status;
                    return View(Jsonresult);
                //}
                //else
                //    return RedirectToAction("SignIn", "Login");
            }
            catch(Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [HttpPost]
        public ActionResult AddMultipleEmployee(FormCollection EmpDetails)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                Models.FreeCustomer FreeCustomerInfo;
                FreeCustomerInfo = (Models.FreeCustomer)Session["Registration"];
                FreeCustomerInfo.EmpJson = EmpDetails["EmpJsonModel"];
                if (FreeCustomerInfo != null)
                {
                    #region ShopInfo
                    var currency = SPA.spa_currency.Where(c => c.name_country == FreeCustomerInfo.shopcountry && c.ActiveStatus == "A").Select(c => c.currency).FirstOrDefault();
                    CCTSP_SchoolMaster ShopInfo = new CCTSP_SchoolMaster();
                    ShopInfo.SchlName = FreeCustomerInfo.shopname;
                    ShopInfo.SchlCity = FreeCustomerInfo.shopcity;
                    ShopInfo.SchlState = FreeCustomerInfo.shopState;
                    ShopInfo.Schcountry = FreeCustomerInfo.shopcountry;
                    ShopInfo.SchPin = FreeCustomerInfo.shopzipcode;
                    ShopInfo.SchlAddress = FreeCustomerInfo.shopaddress;
                    ShopInfo.latitude = FreeCustomerInfo.latitude;
                    ShopInfo.longitude = FreeCustomerInfo.longitude;
                    ShopInfo.location = FreeCustomerInfo.Location;
                    ShopInfo.Color_Id = 2;
                    ShopInfo.SchlEmail = FreeCustomerInfo.emailid;
                    ShopInfo.SHOPTYPE = FreeCustomerInfo.ShopType;
                    ShopInfo.MainCategory = FreeCustomerInfo.MainCatgId;
                    ShopInfo.ActiveStatus = "F";
                    if (FreeCustomerInfo.DoctorStatus == "2")
                        ShopInfo.SchlStudentStrength = 3;
                    ShopInfo.Currency = currency;
                    ShopInfo.Lang_id = 1;
                    ShopInfo.Is_FreeCustomer = 1;
                    ShopInfo.CreatedOn = DateTime.Now;
                    ShopInfo.SchlEmail = FreeCustomerInfo.emailid;
                    SPA.CCTSP_SchoolMaster.Add(ShopInfo);
                    SPA.SaveChanges();
                    CCTSP_LendingPageMaster LendingInfo = new CCTSP_LendingPageMaster();
                    LendingInfo.Schid = ShopInfo.SchlId;
                    LendingInfo.ShopDesc = FreeCustomerInfo.ShopDesc;
                    SPA.CCTSP_LendingPageMaster.Add(LendingInfo);
                    SPA.SaveChanges();
                    #region ShopOwnerInfo
                    CCTSP_User Personalinfo = new CCTSP_User();
                    Personalinfo.FirstName = FreeCustomerInfo.FirstName;
                    Personalinfo.LastName = FreeCustomerInfo.FamilyName;
                    Personalinfo.Gender = FreeCustomerInfo.Title;
                    Personalinfo.PhoneNo = FreeCustomerInfo.phoneno;
                    Personalinfo.LoginName = FreeCustomerInfo.phoneno != null ? FreeCustomerInfo.phoneno : "0123456789";
                    Personalinfo.EmailId = FreeCustomerInfo.emailid;
                    Personalinfo.Password = FreeCustomerInfo.phoneno != null ? FreeCustomerInfo.phoneno : "Admin@12345";
                    Personalinfo.Country = FreeCustomerInfo.Country;
                    Personalinfo.State = FreeCustomerInfo.State;
                    Personalinfo.City = FreeCustomerInfo.City;
                    Personalinfo.Pincode = FreeCustomerInfo.Zipcode;
                    Personalinfo.Address1 = FreeCustomerInfo.Address;
                    Personalinfo.ActiveStatus = "F";
                    Personalinfo.RoleId = 1;
                    Personalinfo.SchId = ShopInfo.SchlId;
                    Personalinfo.CreatedOn = DateTime.Now;
                    Personalinfo.Updated_Date = DateTime.Now;
                    SPA.CCTSP_User.Add(Personalinfo);
                    SPA.SaveChanges();
                    #endregion
                    fu.AddFreecustomerInfo(ShopInfo.SchlId, FreeCustomerInfo);
                    #endregion
                    return RedirectToAction("ThankYou", "FreeCustomer");
                }
                else
                    return RedirectToAction("FreeCustomerList", "FreeCustomer");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult MultipleEmpList(Models.EmployeeJsonModel Jsonresult)
        {
            return View(Jsonresult);
        }
        [CustomAutohrize(null)]
        public ActionResult OwnerInfo(string Status)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                //if (Convert.ToInt32(Session["RoleId"].ToString()) !=4)
                //{
                    int lang = 1;
                    ViewBag.Status = Status;
                    var CommonInfo = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.Lang_Id == lang && c.CatgId == 110).ToList();
                    //GenderInfo
                    ViewBag.Client = new SelectList(CommonInfo.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == lang).OrderBy(c => c.CatgType), "CatgType", "CatgType");
                    //CountryList
                    ViewBag.countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A").ToList();
                    //MainCategory And ShopType
                    string MainList = "select a.CatgTypeId as MainCatgId,a.CatgDesc as MainCategory ,b.SectionDesc as ShopTypeName,b.SolutionId as ShopTypeId from cctsp_categoryDetails a join ctsp_solutionMaster b on a.CatgTypeId = b.CatgTypeId and b.ActiveStatus = 'A' where a.catgId = 127 and a.ActiveStatus = 'A' and a.Lang_Id = " + lang;
                    var MainCategoryList = SPA.Database.SqlQuery<Models.ShopTypeDetails>(MainList).ToList();
                    ViewBag.MainCategory = MainCategoryList.Select(c => new Models.MainCategoryDetails
                    { MainCategory = c.MainCategory, MainCatgId = c.MainCatgId }).FirstOrDefault();
                    //MainCategoryList
                    ViewBag.ShopDetails = js.Serialize(MainCategoryList);
                    //ShopTypeList
                    ViewBag.ShopTypeList = new SelectList(MainCategoryList, "ShopTypeId", "ShopTypeName");

                    //WeekScheduleList
                    string WeekDay = " select a.CatgTypeId,b.Value as Day from cctsp_CategoryDetails a " +
                              " join Language_Label_Detail b on a.Catgdesc = b.English_Label " +
                              " Where a.CatgId = 30 and a.ActiveStatus = 'A' and a.DomainType = 236 " +
                              " and b.Lang_Id = " + lang + " and b.ActiveStatus = 'A' and b.Page_Name = 'Create_Employee'";
                    ViewBag.WeekDetails = SPA.Database.SqlQuery<Models.LendingWeekScheduleDetails>(WeekDay).ToList();
                    ViewBag.SerialzeLendingList = js.Serialize(ViewBag.WeekDetails);
                    ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Lending_Page" && c.ActiveStatus == "A" && c.Lang_id == lang)
                        .Select(c => new Models.LanguageNewShop
                        { Lang_id = c.Lang_id, English_Label = c.English_Label, Page_Name = c.Page_Name, Order_id = c.Order_id, Value = c.Value }).ToList();
                    if (Session["Registration"] != "" && Session["Registration"] != null)
                    {
                        Models.FreeCustomer model;
                        model = (Models.FreeCustomer)Session["Registration"];
                        ViewBag.WeekDetails = js.Deserialize<List<Models.LendingWeekScheduleDetails>>(model.LendingObject);
                        ViewBag.SerialzeLendingList = js.Serialize(ViewBag.WeekDetails);
                        return View(model);
                    }
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
        public ActionResult AddOwnerInfo(Models.FreeCustomer FreeCustomerInfo)
        {
            try
            {
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();
                if (FreeCustomerInfo != null)
                {
                    Session["Registration"] = FreeCustomerInfo;
                    return RedirectToAction("AddMultipleEmployee", "FreeCustomer", new { Status = FreeCustomerInfo.DoctorStatus });
                }
                else
                    return RedirectToAction("FreeCustomerList", "FreeCustomer");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [CustomAutohrize(null)]
        public ActionResult FreeCustomerList()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4 )
                //{
                    Session["Registration"] = "";
                    var List = "select a.Schlid as ShopId,a.SchlName as ShopName,a.SchlCity as ShopCity,b.FirstName as OwnerName,b.LastName as OwnerLName,c.Catgdesc as MainCategory,d.SectionName as ShopType from cctsp_SchoolMaster a " +
                          "join cctsp_User b on a.Schlid = b.Schid " +
                          "join cctsp_CategoryDetails c on c.CatgtypeId = a.MainCategory " +
                          "join ctsp_SolutionMaster d on d.Solutionid = convert(bigint, a.ShopType) " +
                          "where a.Is_FreeCustomer=1  and  (a.ActiveStatus='A' or a.ActiveStatus='F') and (b.ActiveStatus='A' or b.ActiveStatus='F') and b.RoleId = 1";
                    var Listinfo = SPA.Database.SqlQuery<Models.FreeCustomerList>(List).ToList();
                    return View(Listinfo);
                //}
                //else
                //    return RedirectToAction("SignIn", "Login");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult ThankYou()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Session["Registration"] = "";
            //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
            //{
                return View();
            //}
            //else
            //    return RedirectToAction("SignIn", "Login");
        }
        public ActionResult EditFreeCustomer(long? SchlId)
        {
            ViewBag.BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            var Details = "select a.SchlName as shopname,a.SchlStudentStrength as DoctorFlowStatus,a.Schlid as ShopId,a.Lang_Id as Langid,a.Schcountry as shopcountry,a.SchlState as shopState,a.SchlCity as shopcity, " +
                                    "a.Schpin as shopzipcode,a.SchlAddress as shopaddress,a.latitude as latitude,a.longitude as longitude, " +
                                    "a.Location,f.ShopDesc,b.FirstName,b.LastName as FamilyName,b.phoneno,b.password,b.emailid,b.Country,b.State,b.City, " +
                                    "b.Pincode as Zipcode,b.Address1 as Address,b.Gender as OwnerGender,g.Amount as EmpCharge, " +
                                    "c.CatgTypeid as MainCatgId,c.CatgDesc as maincategory,d.SolutionId as ShopTypeId,d.SectionName as ShopTypeName " +
                                    "from cctsp_Schoolmaster a " +
                                    "join cctsp_User b on a.Schlid = b.Schid " +
                                    "join cctsp_CategoryDetails c on c.CatgTypeId = a.MainCategory " +
                                    "join ctsp_SolutionMaster d on d.SolutionId = Convert(bigint, a.ShopType) " +
                                    "join CCTSP_LendingPageMaster f on f.Schid = a.Schlid " +
                                    "join ctsp_SolutionMaster g on g.Schid = a.Schlid " +
                                    "where b.RoleId = 1  and c.CatgId = 127 and a.Schlid = " + SchlId + "" +
                                    "and g.SectionName = 'ChoosProduct'";
            var Shopinfo = SPA.Database.SqlQuery<Models.EditFreeCustomer>(Details).FirstOrDefault();
            //var EmpInfo = SPA.CCTSP_User.Where(c => c.ActiveStatus == "A" && c.SchId == SchlId && c.RoleId == 3).Select(c => new Models.FreeCustomerEmployeeInfo
            //{
            //    EmpUserId = c.UserId,
            //    EmpFName = c.FirstName,
            //    EmpLName = c.LastName,
            //    EmpEmail = c.EmailId,
            //    EmpNumber = c.PhoneNo,
            //    EmpDegree = c.Degree,
            //    EmpGender = c.Gender,
            //    EmpExpMonth = c.ExperienceMonth,
            //    EmpExpYear = c.ExperienceYear,
            //    EmpImage = c.Answer2,
            //    specialization = c.specialization,
            //    EmpSkypeId = c.skypeId
            //}).OrderBy(c => c.EmpUserId).ToList();
            var EmpList = " select a.UserId as EmpUserId,a.FirstName as EmpFName, " +
                         "a.LastName as EmpLName, a.EmailId as EmpEmail,a.PhoneNo as EmpNumber, " +
                         "a.Degree as EmpDegree,a.Gender as EmpGender ,a.ExperienceMonth as EmpExpMonth , " +
                         " a.ExperienceYear as EmpExpYear,a.Answer2 as EmpImage ,a.specialization as specialization, " +
                         "a.skypeId as EmpSkypeId,b.Amount as EmpCharge " +
                         " from cctsp_User a " +
                         " left join ctsp_SolutionMaster b on a.UserId = b.UserId and b.ActiveStatus = 'A' " +
                         " Where a.RoleId = 3 and a.activestatus = 'A' and b.CatgtypeId =11145 and a.Schid =" + SchlId;
            var EmpInfo = SPA.Database.SqlQuery<Models.FreeCustomerEmployeeInfo>(EmpList).OrderBy(c => c.EmpUserId).Distinct().ToList();
            ViewBag.EmpInfo = EmpInfo;
            var CommonInfo = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.Lang_Id == Shopinfo.Langid && (c.CatgId == 110 || c.CCTSP_CategoryMaster.CatgName == "Specialization")).ToList();
            //GenderInfo
            ViewBag.Client = new SelectList(CommonInfo.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == Shopinfo.Langid).OrderBy(c => c.CatgType), "CatgType", "CatgType");
            ViewBag.GenderList = CommonInfo.Where(c => c.CatgId == 110 && c.ActiveStatus == "A" && c.Lang_Id == Shopinfo.Langid).OrderBy(c => c.CatgType).ToList();
            //CountryList
            ViewBag.countrylist = SPA.spatime_zone.Where(c => c.ActiveStatus == "A").ToList();
            //MainCategory And ShopType
            string MainList = "select a.CatgTypeId as MainCatgId,a.CatgDesc as MainCategory ,b.SectionDesc as ShopTypeName,b.SolutionId as ShopTypeId from cctsp_categoryDetails a join ctsp_solutionMaster b on a.CatgTypeId = b.CatgTypeId and b.ActiveStatus = 'A' where a.catgId = 127 and a.ActiveStatus = 'A' and a.Lang_Id = " + Shopinfo.Langid;
            var MainCategoryList = SPA.Database.SqlQuery<Models.FreecustomerShoptypeDetails>(MainList).ToList();
            ViewBag.MainCategory = MainCategoryList.Select(c => new Models.MainCategoryDetails
            { MainCategory = c.MainCategory, MainCatgId = c.MainCatgId }).FirstOrDefault();
            //MainCategoryList
            ViewBag.ShopDetails = js.Serialize(MainCategoryList);
            //ShopTypeList
            ViewBag.ShopTypeList = MainCategoryList;
            //Specialization                              
            var Category = CommonInfo.Where(c => c.CCTSP_CategoryMaster.CatgName == "Specialization" && c.ActiveStatus == "A" && c.Lang_Id == 1).OrderBy(c => c.CatgDesc)
                            .Select(c => new Models.MainCategoryDetails
                            { MainCategory = c.CatgDesc, MainCatgId = c.CatgTypeId, WorkFlowId = c.Work_Flow_Id }).OrderBy(c => c.MainCategory).ToList();
            ViewBag.Category = Category;
            List<Models.EmployeeSpecializationList> SpecializationList = new List<Models.EmployeeSpecializationList>();
            Models.EmployeeSpecializationList Specialization = new Models.EmployeeSpecializationList();
            var Default = Category.Where(c => c.WorkFlowId == 1).FirstOrDefault();
            foreach (var Item in EmpInfo)
            {
                Specialization = new Models.EmployeeSpecializationList();
                Specialization.EmpId = Item.EmpUserId;
                if (Item.specialization != null)
                {
                    var EmpidList = Item.specialization.Split(',').ToList();
                    Specialization.SpecializationList = Category.Where(c => EmpidList.Contains(c.MainCatgId.ToString())).ToList();
                }
                var Count = 4 - Specialization.SpecializationList.Count;
                if (Count != 0)
                {
                    for (int i = 0; i <= Count - 1; i++)
                        Specialization.SpecializationList.Add(Default);
                }
                SpecializationList.Add(Specialization);
            }
            ViewBag.SpecializationList = SpecializationList;
            //WeekScheduleList
            string WeekDay = " select a.CatgTypeId,b.Value as Day,c.CatgDesc,c.Gender from cctsp_CategoryDetails a " +
                              " join Language_Label_Detail b on a.Catgdesc = b.English_Label " +
                              " left join cctsp_CategoryDetails c on c.value = a.CatgTypeId and c.DomainType = " + SchlId + " and c.ActiveStatus = 'A' and c.CatgId = 148 " +
                              " Where a.CatgId = 30 and a.ActiveStatus = 'A' and a.DomainType = 236 " +
                              " and b.Lang_Id = " + Shopinfo.Langid + " and b.ActiveStatus = 'A' and b.Page_Name = 'Create_Employee'";
            ViewBag.WeekDetails = SPA.Database.SqlQuery<Models.LendingWeekScheduleDetails>(WeekDay).ToList();
            ViewBag.SerialzeLendingList = js.Serialize(ViewBag.WeekDetails);
            var Key = "AIzaSyAs0JFbu9JqD2dmkLbHiSNBmwtQ92UKYkY";
            int Number = fu.CheckRandom();
            if (Number == 1)
                Key = "AIzaSyCYjxraxYd9NwCLWbVZNqGARiSgRNfBhiY";
            if (Number == 2)
                Key = "AIzaSyAs0JFbu9JqD2dmkLbHiSNBmwtQ92UKYkY";
            if (Number == 3)
                Key = "AIzaSyAs0JFbu9JqD2dmkLbHiSNBmwtQ92UKYkY";
            ViewBag.Key = Key;
            return View(Shopinfo);
        }
        [HttpPost]
        public ActionResult UpdateFreeCustomer(Models.EditFreeCustomer FreeCustomerInfo)
        {
            //Update ShopInfo
            //var SerializedObject = js.Serialize(FreeCustomerInfo);
            var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == FreeCustomerInfo.ShopId).FirstOrDefault();
            ShopInfo.SchlName = FreeCustomerInfo.shopname;
            ShopInfo.SchlAddress = FreeCustomerInfo.shopaddress;
            ShopInfo.Schcountry = FreeCustomerInfo.shopcountry;
            ShopInfo.SchlState = FreeCustomerInfo.shopState;
            ShopInfo.SchlCity = FreeCustomerInfo.City;
            ShopInfo.SchPin = FreeCustomerInfo.shopzipcode;
            ShopInfo.latitude = FreeCustomerInfo.latitude;
            ShopInfo.longitude = FreeCustomerInfo.longitude;
            ShopInfo.location = FreeCustomerInfo.Location;
            ShopInfo.SHOPTYPE = FreeCustomerInfo.ShopType;
            ShopInfo.MainCategory = FreeCustomerInfo.MainCatgId;
            SPA.SaveChanges();

            //update LendingInfo
            var LendingInfo = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == FreeCustomerInfo.ShopId).FirstOrDefault();
            LendingInfo.ShopDesc = FreeCustomerInfo.ShopDesc;
            SPA.SaveChanges();

            //Update ShopOwnerInfo
            var OwnerInfo = SPA.CCTSP_User.Where(c => c.SchId == FreeCustomerInfo.ShopId && c.RoleId == 1).FirstOrDefault();
            OwnerInfo.FirstName = FreeCustomerInfo.FirstName;
            OwnerInfo.LastName = FreeCustomerInfo.FamilyName;
            OwnerInfo.PhoneNo = FreeCustomerInfo.phoneno;
            OwnerInfo.EmailId = FreeCustomerInfo.emailid;
            OwnerInfo.Country = FreeCustomerInfo.Country;
            OwnerInfo.State = FreeCustomerInfo.State;
            OwnerInfo.City = FreeCustomerInfo.City;
            OwnerInfo.Pincode = FreeCustomerInfo.Zipcode;
            OwnerInfo.Gender = FreeCustomerInfo.OwnerGender;
            OwnerInfo.Address1 = FreeCustomerInfo.Address;
            SPA.SaveChanges();

            //Update Further Information in Async call
            fu.EditFreecustomerInfo(FreeCustomerInfo.ShopId.Value, FreeCustomerInfo).ConfigureAwait(false);
            return Redirect("/FreeCustomer/FreeCustomerList");
        }
        public void DeleteNewShop(int? Schlid)
        {
            try
            {
                var queryU = "update CCTSP_SchoolMaster set  ActiveStatus= 'D' where SchlId=" + Schlid + "";
                SPA.Database.ExecuteSqlCommand(queryU);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
    }
}