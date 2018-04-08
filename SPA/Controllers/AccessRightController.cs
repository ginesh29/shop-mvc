using SPA.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA.Controllers
{

    [checkShop]
    public class AccessRightController : Controller
    {
        int schId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        Function fu = new Function();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        public AccessRightController()
        {
            schId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: AccessRight
        public ActionResult Roles()
        {
            try
            {
                List<Models.TabInfo> TabList = new List<Models.TabInfo>();
                var ShopInfo = SPA.CCTSP_LendingPageMaster.Where(c => c.Schid == schId).Select(c => new Models.ShopMasterDetail
                {
                    Lang_id = c.CCTSP_SchoolMaster.Lang_id,
                    Display_FreeCustomer = c.CCTSP_SchoolMaster.Display_FreeCustomer,
                    Display_Invoice = c.CCTSP_SchoolMaster.Display_Invoice,
                    DisplayMarketing = c.Display_Marketing,
                    BookingApproval=c.CCTSP_SchoolMaster.BookingApproval,
                    SchlWebsite=c.CCTSP_SchoolMaster.SchlWebsite
                }).FirstOrDefault();
                var LanguageInfo = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Role" && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.Lang_id).Select(c => new Models.LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id
                }).ToList();
                ViewBag.Language = LanguageInfo;
                var Tabinfo = fu.Tabinfo(ShopInfo.Lang_id.Value);
                var ConditionList = fu.LayoutCondition(ShopInfo);
               // ViewBag.ConditionList = ConditionList;
                Tabinfo = Tabinfo.Select(c => new Models.TabInfo
                {
                    RoleId = c.RoleId,
                    RoleName = c.Display_Status == 1 ? LanguageInfo.Where(d => (d.English_Label).Trim().ToLower() == (c.RoleName).Trim().ToLower()).Select(d => d.Value).FirstOrDefault() : c.RoleName,
                    Display_Status = c.Display_Status,
                    MainTab = c.MainTab,
                    MainTabId = c.MainTabId,
                    Condition = c.Condition,
                    ConditionValue = c.ConditionValue,
                    Schlid = c.Schlid,
                    OwnAnyStatus = c.OwnAnyStatus,
                    SubTabId = c.SubTabId,
                    SubTab = c.SubTab,
                    AccessToData = c.AccessToData,
                    insertAccess = c.insertAccess,
                    DeleteAccess = c.DeleteAccess,
                    UpdateAccess = c.UpdateAccess,
                    AccesstoMenu = c.AccesstoMenu,
                    AccessToSub = c.AccessToSub,
                    CheckCondition =c.Condition!=null?ConditionList.Where(d => d.Condition == c.Condition && d.ConditionValue == c.ConditionValue).FirstOrDefault() != null ? "" : "1":"",
                    CheckSubCondition = c.SubCondition != null?ConditionList.Where(d => d.Condition == c.SubCondition && d.ConditionValue == c.SubConditionValue).FirstOrDefault() != null ? "" : "1":"",
                }).ToList();
                List<Models.AccessType> AccessTypeList = new List<Models.AccessType>();
                Models.AccessType AccessType = new Models.AccessType();
                AccessType.AccessName = LanguageInfo.Where(c => c.Order_id == 16).Select(c => c.English_Label).FirstOrDefault(); ;
                AccessType.AccessValue1 = LanguageInfo.Where(c => c.Order_id == 17).Select(c => c.Value).FirstOrDefault();
                AccessType.AccessValue2 = LanguageInfo.Where(c => c.Order_id == 18).Select(c => c.Value).FirstOrDefault();
                AccessTypeList.Add(AccessType);
                AccessType = new Models.AccessType();
                AccessType.AccessName = LanguageInfo.Where(c => c.Order_id == 10).Select(c => c.English_Label).FirstOrDefault(); ;
                AccessType.AccessValue1 = LanguageInfo.Where(c => c.Order_id == 11).Select(c => c.Value).FirstOrDefault();
                AccessType.AccessValue2 = LanguageInfo.Where(c => c.Order_id == 12).Select(c => c.Value).FirstOrDefault();
                AccessTypeList.Add(AccessType);
                ViewBag.AccessTypeList = AccessTypeList;
                return View("../AccessRight/Roles", Tabinfo);
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        public ActionResult AddRole(FormCollection RoleName)
        {
            try
            {
                if (RoleName != null)
                {
                    CCTSP_Role Role = new CCTSP_Role();
                    List<cctsp_RoleDetails> AddRoleDetails = new List<cctsp_RoleDetails>();
                    var Name = RoleName["Role"].Trim().ToLower();
                    //var RoleExist = SPA.cctsp_RoleDetails.Where(c =>((c.ActiveStatus == "A" && c.SchId == schId && c.CCTSP_Role.RoleName.ToLower() == Name.ToLower())||(c.ActiveStatus=="A" &&c.SchId==236 && c.CCTSP_Role.RoleName.ToLower()==Name.ToLower()))).Count();
                    var RoleExist = SPA.CCTSP_Role.Where(c => ((c.ActiveStatus == "A" && c.Schlid == schId && c.RoleName.Trim().ToLower() == Name) || (c.ActiveStatus == "A" && c.Schlid == 236 && c.RoleName.Trim().ToLower() == Name))).Count();
                    if (RoleExist == 0)
                    {
                        Role = new CCTSP_Role();
                        Role.ActiveStatus = "A";
                        Role.RoleName = RoleName["Role"];
                        Role.CreatedOn = DateTime.Now;
                        Role.Role_Status = 1;
                        Role.Schlid = schId;
                        SPA.CCTSP_Role.Add(Role);
                        SPA.SaveChanges();

                        var RoleDetails = SPA.cctsp_RoleDetails.Where(c => c.RoleId == 3 && c.SchId == 236 && c.ActiveStatus == "A").ToList();
                        AddRoleDetails = RoleDetails.Select(c => new cctsp_RoleDetails
                        {
                            RoleId = Role.RoleId,
                            SchId = schId,
                            AccessToMenu = c.AccessToMenu,
                            AccessToSub = c.AccessToSub,
                            AccessToData = c.AccessToData,
                            ModifyAccess = c.ModifyAccess,
                            insertAccess = c.insertAccess,
                            DeleteAccess = c.DeleteAccess,
                            UpdateAccess = c.UpdateAccess,
                            ActiveStatus = "A"
                        }).ToList();
                        SPA.cctsp_RoleDetails.AddRange(AddRoleDetails);
                        SPA.SaveChanges();
                    }
                    else
                    {
                        Session["Message"] = "AlreadyExist";
                    }

                }
                //return RedirectToAction("AccessRight", "AccessRight");
                return Redirect("/Shop/shop#Roles");
            }
            catch (Exception Ex)
            {
                fu.ErrorSend(RouteData, Ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult AssignRoles(FormCollection RoleDetails)
        {
            try
            {
                List<Models.TabInfo> AddTabList = new List<Models.TabInfo>();
                List<Models.TabInfo> AccessList = new List<Models.TabInfo>();
                Models.TabInfo AddTab = new Models.TabInfo();
                var RoleList = SPA.CCTSP_Role.Where(c => c.ActiveStatus == "A" && c.Schlid == schId).ToList();
                foreach (var Role in RoleList)
                {
                    foreach (var item in RoleDetails.AllKeys.Where(c => c.Contains(Role.RoleId + "#")).ToList())
                    {
                        AddTab = new Models.TabInfo();
                        AddTab.RoleId = Role.RoleId;
                        AddTab.MainTabId = Convert.ToInt64(item.Replace(Role.RoleId + "#", "").Split('_')[0]);
                        AddTab.SubTabId = Convert.ToInt64(item.Replace(Role.RoleId + "#", "").Split('_')[1]);
                        AddTabList.Add(AddTab);
                    }
                    foreach (var item in RoleDetails.AllKeys.Where(c => c.Contains(Role.RoleId + "_insert_") || c.Contains(Role.RoleId + "_Edit_") || c.Contains(Role.RoleId + "_Delete_") || c.Contains(Role.RoleId + "_Display_") || c.Contains(Role.RoleId + "_Updation_")).ToList())
                    {
                        AddTab = new Models.TabInfo();
                        var Data = RoleDetails[item];
                        if (Data.Split('_')[0] == "Yes")
                        {
                            AddTab.OwnAnyStatus = Data.Split('_')[0];
                            AddTab.RoleId = Convert.ToInt64(Data.Split('_')[1]);
                            AddTab.SubTabId = Convert.ToInt64(Data.Split('_')[3]);
                            AddTab.RoleName = Data.Split('_')[2].Replace("_", "");
                            AccessList.Add(AddTab);
                        }
                    }
                }
                var Query = " update cctsp_RoleDetails set ActiveStatus='D' where Schid=" + schId;
                SPA.Database.ExecuteSqlCommand(Query);
                List<cctsp_RoleDetails> RoleDetailsList = new List<cctsp_RoleDetails>();
                cctsp_RoleDetails RoleDetail = new cctsp_RoleDetails();
                foreach (var Item in AddTabList)
                {
                    var Access = AccessList.Where(c => c.SubTabId == Item.SubTabId && c.RoleId == Item.RoleId).ToList();
                    RoleDetail = new cctsp_RoleDetails();
                    RoleDetail.AccessToMenu = Convert.ToString(Item.MainTabId);
                    RoleDetail.AccessToSub = Convert.ToString(Item.SubTabId);
                    RoleDetail.RoleId = Item.RoleId;
                    RoleDetail.ActiveStatus = "A";
                    RoleDetail.SchId = schId;
                    RoleDetail.AccessToData = "Any";
                    RoleDetail.ModifyAccess = "N";
                    RoleDetail.UpdateAccess = "N";
                    RoleDetail.insertAccess = "N";
                    RoleDetail.DeleteAccess = "N";
                    foreach (var Details in Access)
                    {
                        if (Details.RoleName == "Edit")
                            RoleDetail.UpdateAccess = "Y";
                        if (Details.RoleName == "insert")
                            RoleDetail.insertAccess = "Y";
                        if (Details.RoleName == "Delete")
                            RoleDetail.DeleteAccess = "Y";
                        if (Details.RoleName == "Display")
                            RoleDetail.AccessToData = "Own";
                        if (Details.RoleName == "Updation")
                        {
                            RoleDetail.UpdateAccess = "Y";
                            RoleDetail.insertAccess = "Y";
                            RoleDetail.DeleteAccess = "Y";
                        }
                    }
                    RoleDetailsList.Add(RoleDetail);
                }
                SPA.cctsp_RoleDetails.AddRange(RoleDetailsList);
                SPA.SaveChanges();
                //return RedirectToAction("AccessRight", "AccessRight");
                return Redirect("/Shop/shop#Roles");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult Permissions()
        {
            try
            {
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schId && c.ActiveStatus == "A").Select(c => new Models.ShopDetails
                {
                    LangId = c.Lang_id
                }).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => (c.Page_Name == "Permission" || (c.Page_Name=="Role" && c.Order_id==14)) && c.ActiveStatus == "A" && c.Lang_id == ShopInfo.LangId).Select(c => new Models.LanguageLabelDetails
                {
                    English_Label = c.English_Label,
                    Value = c.Value,
                    Page_Name = c.Page_Name,
                    Order_id = c.Order_id,
                    Lang_id = c.Lang_id
                }).ToList();
                ViewBag.RoleDetails = SPA.CCTSP_Role.Where(c => c.ActiveStatus == "A" && ((c.Schlid == 236 && c.RoleId != 1 && c.RoleId != 4) || c.Schlid == schId) && c.Role_Status == 1).Select(c => new Models.MainCategoryDetails
                {
                    MainCategory = c.RoleName,
                    MainCatgId = c.RoleId
                }).Distinct().ToList();
                ViewBag.User = SPA.CCTSP_User.Where(c => c.ActiveStatus == "A" && c.RoleId != 4 && c.RoleId != 1 && c.SchId == schId).Select(c => new Models.UserRoleDetails
                {
                    RoleId = c.RoleId,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    UserId = c.UserId
                }).ToList();
                return View("../AccessRight/Permissions");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult AddPermissions(Models.AddPermission Permission)
        {
            List<CCTSP_User> UserList = new List<CCTSP_User>();
            CCTSP_User User = new CCTSP_User();
            try
            {
                foreach (var UserId in Permission.UserId)
                {
                    var Index = Permission.UserId.IndexOf(UserId);
                    User = SPA.CCTSP_User.Where(c => c.UserId == UserId && c.SchId == schId && c.ActiveStatus == "A").FirstOrDefault();
                    User.RoleId = Permission.SelectedRoleId[Index];
                    UserList.Add(User);
                }
                SPA.SaveChanges();
                return Redirect("/Shop/shop#Permissions");
                //return Redirect("/AccessRight/AccessRight#Permissions");
            }
            catch (Exception ex)
            {
                fu.ErrorSend(RouteData, ex);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        public ActionResult AccessRight()
        {
            try
            {
                //if (Convert.ToInt32(Session["RoleId"].ToString()) != 4)
                //{
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schId)
                               .Select(c => new Models.ShopMasterDetail
                               {
                                   Currency = c.Currency,
                                   Lang_id = c.Lang_id.Value,
                                   TimeZone = c.TimeZone,
                                   Flow_Id = c.Flow_Id
                               }).FirstOrDefault();
                ViewBag.SubMenu = fu.AccessSubMenu(SchoolInfo.Lang_id.Value, Convert.ToInt32(Session["RoleId"].ToString()), "AccessRight_Tab",SchoolInfo.Flow_Id);
                return View();
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
    }
}