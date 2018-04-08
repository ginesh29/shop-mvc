using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SPA.Common;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
namespace SPA.Controllers
{
    [checkShop]
    public class ProductController : Controller
    {
        //Entities
        cctspDatabaseEntities22 SPA = new cctspDatabaseEntities22();
        //SchoolId
        int schlId = Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationVariable"]);
        //Common Function
        Function fu = new Function();
        JavaScriptSerializer js = new JavaScriptSerializer();
        string link = System.Web.HttpContext.Current.Request.Url.Host;
        public ProductController()
        {
            schlId = Convert.ToInt32(fu.GetShopId(link));
        }
        // GET: Product
        public void AddGroupText(FormCollection GroupText)
        {
            try
            {
                DateTime EuropeDate = fu.ZonalDate(null);
                CCTSP_CategoryDetails GroupTextDetails = new CCTSP_CategoryDetails();
                var GroupTextName = GroupText["GroupName"];
                if (GroupText["EditGroupNameCatgTypeId"] == "")
                {
                    GroupTextDetails.CatgDesc = GroupTextName;
                    GroupTextDetails.CatgId = 126;
                    GroupTextDetails.CatgType = GroupTextName.Substring(0, 3);
                    GroupTextDetails.ActiveStatus = "A";
                    GroupTextDetails.DomainType = schlId;
                    GroupTextDetails.CreatedOn = EuropeDate;
                    long GroupOrderCal = 0;
                    var GroupOrderData = SPA.CCTSP_CategoryDetails.Where(c => c.DomainType == schlId && c.ActiveStatus == "A" && c.CatgId == 126);
                    if (GroupOrderData.Count() != 0)
                    {
                        if (GroupOrderData.Where(c => c.Group_orderdata != null).Count() != 0)
                            GroupOrderCal = GroupOrderData.Where(c => c.Group_orderdata != null).Select(c => c.Group_orderdata).Max().Value + 1;
                        else
                            GroupOrderCal = 1;
                    }
                    else
                        GroupOrderCal = 1;
                    GroupTextDetails.Group_orderdata = GroupOrderCal;
                    SPA.CCTSP_CategoryDetails.Add(GroupTextDetails);
                    SPA.SaveChanges();
                }
                else
                {
                    long CatgTypeId = Convert.ToInt64(GroupText["EditGroupNameCatgTypeId"]);
                    var GroupDetails = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == CatgTypeId).FirstOrDefault();
                    GroupDetails.CatgDesc = GroupTextName;
                    UpdateModel(GroupDetails);
                    SPA.SaveChanges();
                    //var EditGroupName = "update CCTSP_CategoryDetails set CatgDesc='" + GroupTextName + "' where CatgTypeId=" + CatgTypeId;
                    //SPA.Database.ExecuteSqlCommand(EditGroupName);
                }

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }

        }
        [CustomAutohrize(null)]
        //public ActionResult Product_Catalog()
        //{
        //    try
        //    {
        //        ViewBag.Exist = "No";
        //        /*Shop Information like Currency and Lang_Id*/
        //        var Shopinfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
        //                        .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, Currency = c.Currency })
        //                        .FirstOrDefault();
        //        ViewBag.currency = fu.currency(Shopinfo.Currency);
        //        /*Language Information*/
        //        var LanguageDetails = SPA.Language_Label_Detail
        //                              .Where(c => c.Lang_id == Shopinfo.Lang_id && ((c.Page_Name == "Product_Catalog") || (c.Page_Name == "Title" && c.Order_id == 9)))
        //                              .ToList();
        //        ViewBag.Language = LanguageDetails.Where(c => c.Page_Name == "Product_Catalog").ToList();
        //        ViewBag.Title = LanguageDetails.Where(c => c.Page_Name == "Title" && c.Order_id == 9).Select(c => c.Value).FirstOrDefault();
        //        var ProductGroupList = SPA.CCTSP_CategoryDetails
        //                               .Where(c => c.ActiveStatus == "A" && (
        //                               (c.CatgId == 126 && c.DomainType == schlId) ||
        //                               (c.CatgId == 124 && c.Lang_Id == Shopinfo.Lang_id && c.Group_orderdata == 1)) ||
        //                               (c.CatgId == 174)
        //                               && c.CatgDesc != null && c.CatgDesc != "").ToList();
        //        ViewBag.ProductGroupList = ProductGroupList.Where(c => c.CatgId == 126).ToList();
        //        //var DefaultGroupList = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgId == 124 && c.Lang_Id==Shopinfo.Lang_id && c.CatgDesc != null).ToList();
        //        ViewBag.christophGroupList = ProductGroupList.Where(c => c.CatgId == 124 && c.Group_orderdata == 1).Select(c => c.CatgTypeId).FirstOrDefault();
        //        ViewBag.Vat = ProductGroupList.Where(c => c.CatgId == 174).ToList();
        //        ViewBag.InsuranceList = SPA.Health_Insurance.Where(c => c.ActiveStatus == "A" && c.LangId == Shopinfo.Lang_id).Select(c => new Models.InsuranceList
        //        {
        //            Insurance = c.Settlement_text,
        //            InsuranceId = c.Insurance_Id,
        //            Settlement_Number = c.Settlement_Number,
        //            Tarif_Number = c.Tarif_Number
        //        }).ToList();
        //        return View();
        //    }
        //    catch (Exception e)
        //    {
        //        fu.ErrorSend(RouteData, e);
        //        return RedirectToAction("Error_List", "Error");
        //    }

        //}
        public ActionResult Product()
        {
            try
            {
                ViewBag.Exist = "No";
                /*Shop Information like Currency and Lang_Id*/
                var Shopinfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                                .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, Currency = c.Currency, Display_Invoice = c.Display_Invoice })
                                .FirstOrDefault();
                ViewBag.currency = fu.currency(Shopinfo.Currency);
                /*Check Invoice Flow*/
                ViewBag.CheckInvoice = fu.CheckInvoice(Shopinfo.Display_Invoice);
                /*Language Information*/
                var LanguageDetails = SPA.Language_Label_Detail
                                      .Where(c => c.Lang_id == Shopinfo.Lang_id && ((c.Page_Name == "Product_Catalog") || (c.Page_Name == "Title" && c.Order_id == 9)))
                                      .ToList();
                ViewBag.Language = LanguageDetails.Where(c => c.Page_Name == "Product_Catalog").ToList();
                ViewBag.Title = LanguageDetails.Where(c => c.Page_Name == "Title" && c.Order_id == 9).Select(c => c.Value).FirstOrDefault();
                var ProductGroupList = SPA.CCTSP_CategoryDetails
                                       .Where(c => c.ActiveStatus == "A" && (
                                       (c.CatgId == 126 && c.DomainType == schlId) ||
                                       (c.CatgId == 124 && c.Lang_Id == Shopinfo.Lang_id && c.Group_orderdata == 1)) ||
                                       (c.CatgId == 174)
                                       && c.CatgDesc != null && c.CatgDesc != "").ToList();
                ViewBag.ProductGroupList = ProductGroupList.Where(c => c.CatgId == 126).ToList();
                //var DefaultGroupList = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgId == 124 && c.Lang_Id==Shopinfo.Lang_id && c.CatgDesc != null).ToList();
                ViewBag.christophGroupList = ProductGroupList.Where(c => c.CatgId == 124 && c.Group_orderdata == 1).Select(c => c.CatgTypeId).FirstOrDefault();
                ViewBag.Vat = ProductGroupList.Where(c => c.CatgId == 174).OrderBy(c => c.CatgDesc).ToList();
                ViewBag.InsuranceList = SPA.Health_Insurance.Where(c => c.ActiveStatus == "A" && c.LangId == Shopinfo.Lang_id).Select(c => new Models.InsuranceList
                {
                    Insurance = c.Settlement_text,
                    InsuranceId = c.Insurance_Id,
                    Settlement_Number = c.Settlement_Number,
                    Tarif_Number = c.Tarif_Number,
                    DefaultStatus = c.Field1
                }).ToList();
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }

        }
        [CustomAutohrize(null)]
        //public ActionResult Product()
        //{
        //    int LangId = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault().Value;
        //    ViewBag.SubMenu = fu.AccessSubMenu(LangId, Convert.ToInt32(Session["RoleId"].ToString()), "Product_Tab");
        //    return View();
        //}
        public ActionResult Product_Catalog()
        {
            var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                              .Select(c => new Models.ShopMasterDetail
                              {
                                  Currency = c.Currency,
                                  Lang_id = c.Lang_id.Value,
                                  TimeZone = c.TimeZone,
                                  Flow_Id = c.Flow_Id,
                                  Display_Invoice=c.Display_Invoice
                              }).FirstOrDefault();
            ViewBag.SubMenu = fu.AccessSubMenu(SchoolInfo.Lang_id.Value, Convert.ToInt32(Session["RoleId"].ToString()), "Product_Tab", SchoolInfo.Flow_Id);
            List<Models.ConditionList> ConditionList = new List<Models.ConditionList>();
            Models.ConditionList Condition = new Models.ConditionList();
            Condition = new Models.ConditionList();
            Condition.Condition = "SettlementText";
            Condition.ConditionValue = SchoolInfo.Display_Invoice==2 || SchoolInfo.Display_Invoice== null ? "NO" : "YES";
            ConditionList.Add(Condition);
            ViewBag.ConditionList = ConditionList;
            ViewBag.Title = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Title" && c.Order_id == 9 && c.Lang_id == SchoolInfo.Lang_id).Select(c => c.Value).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public string Product_Catalog(FormCollection Product)
        {
            try
            {
                var SchoolInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                                .Select(c => new Models.ShopMasterDetail
                                {
                                    Currency = c.Currency,
                                    Lang_id = c.Lang_id.Value,
                                    TimeZone = c.TimeZone
                                }).FirstOrDefault();
                DateTime EuropeDate = fu.ZonalDate(SchoolInfo.TimeZone);
                string Result = "";
                ViewBag.currency = fu.currency(SchoolInfo.Currency);
                ViewBag.Language = fu.getLanguageData("Product_Catalog", 0, SchoolInfo.Lang_id.Value);
                CCTSP_CategoryDetails prodDetail = new CCTSP_CategoryDetails();
                CTSP_SolutionMaster productAdvanceDetail = new CTSP_SolutionMaster();
                CCTSP_GroupProductDetails Groupdetails = new CCTSP_GroupProductDetails();
                var ProductName = Product["productName"];
                var ProductDuration = Product["ProductDuration"];
                var ProductPrice = Product["ProductPrice"];
                var ProductDescription = Product["ProductDescription"];
                int Groupid = Convert.ToInt32(Product["SelectedGroup"]);
                int DefultGroupid = Convert.ToInt32(Product["SelectedPlateformGroup"]);
                float Vat = float.Parse(Product["SelectedVat"]);
                long InsuranceId = 0;
                if (!string.IsNullOrEmpty(Product["SelectedInsurance"]))
                    InsuranceId = Convert.ToInt64(Product["SelectedInsurance"]);
                var Gender = "";
                var CustomText = Product["C_Settlement_Text"];
                if (Convert.ToString(Product["Female"]) != null || Convert.ToString(Product["male"]) != null)
                {

                    if (Convert.ToString(Product["Female"]) != null && Convert.ToString(Product["male"]) != null)
                    {
                        Gender = "Both";
                    }
                    else if (Convert.ToString(Product["Female"]) != null)
                    {
                        Gender = "Female";
                    }
                    else if (Convert.ToString(Product["male"]) != null)
                    {
                        Gender = "male";
                    }
                }
                #region AddProduct
                if (Product["EditCatgTypeId"] == "")
                {
                    var ChkExistNot = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgDesc == ProductName && c.Amount == ProductPrice && c.Duration == ProductDuration && c.Activestatus == "A" && c.CCTSP_CategoryDetails.ActiveStatus == "A").FirstOrDefault();
                    if (ChkExistNot == null)
                    {
                        #region AddInitialProductDetail
                        prodDetail.CatgDesc = ProductName;
                        if (ProductName.Length >= 3)
                            prodDetail.CatgType = ProductName.Substring(0, 3);
                        else
                            prodDetail.CatgType = ProductName;
                        prodDetail.CatgId = 111;
                        prodDetail.DomainType = schlId;
                        prodDetail.CreatedOn = EuropeDate;
                        prodDetail.ActiveStatus = "A";
                        prodDetail.Gender = Gender;
                        prodDetail.Email_Image = !string.IsNullOrEmpty(CustomText) ? CustomText : null;
                        if (InsuranceId > 0)
                            prodDetail.Insurance_Id = InsuranceId;
                        prodDetail.VAT = Vat;
                        SPA.CCTSP_CategoryDetails.Add(prodDetail);
                        SPA.SaveChanges();
                        #endregion
                        #region ProductDetailInformation
                        productAdvanceDetail.CatgTypeId = prodDetail.CatgTypeId;
                        productAdvanceDetail.Duration = ProductDuration;
                        productAdvanceDetail.Amount = Math.Round(double.Parse(ProductPrice, System.Globalization.NumberStyles.Any), 2).ToString();
                        productAdvanceDetail.SectionName = "ChoosProduct";
                        productAdvanceDetail.SectionDesc = ProductDescription;
                        productAdvanceDetail.Activestatus = "A";
                        productAdvanceDetail.CraetedOn = EuropeDate;
                        long OrderCal = 0;
                        var OrderData = SPA.CTSP_SolutionMaster.Where(c => c.SchId == schlId && c.Activestatus == "A" && c.SectionName == "ChoosProduct");
                        if (OrderData.Count() != 0)
                        {
                            if (OrderData.Where(c => c.OrderData != null).Count() != 0)
                                OrderCal = OrderData.Where(c => c.OrderData != null).Select(c => c.OrderData).Max().Value + 1;
                            else
                                OrderCal = 1;
                        }
                        else
                            OrderCal = 1;

                        productAdvanceDetail.OrderData = OrderCal;
                        productAdvanceDetail.SchId = schlId;
                        SPA.CTSP_SolutionMaster.Add(productAdvanceDetail);
                        SPA.SaveChanges();
                        //Group Details add
                        Groupdetails.GroupIdDefault = DefultGroupid;
                        Groupdetails.GroupIdByShop = Groupid;
                        Groupdetails.ProductId = prodDetail.CatgTypeId;
                        Groupdetails.schlId = schlId;
                        Groupdetails.Attributeorder = 1;
                        //New Code is changed to update Group order for search Attribute
                        Groupdetails.Attributeorder = Convert.ToInt16(SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == DefultGroupid && c.ActiveStatus == "A" && c.CatgId == 124).Select(c => c.Group_orderdata).FirstOrDefault().Value);
                        Groupdetails.ActiveStatus = "A";
                        Groupdetails.CreatedOn = EuropeDate;
                        SPA.CCTSP_GroupProductDetails.Add(Groupdetails);
                        SPA.SaveChanges();
                        #endregion
                        #region AssignProducttoEmployee
                        fu.AssignProductToEmp(new List<long> { prodDetail.CatgTypeId }, EuropeDate);
                        #endregion
                        Result = "No";
                    }
                    else
                    {
                        Result = "yes";
                    }

                }
                #endregion
                #region EditProduct
                else
                {
                    long CatgTypeId = Convert.ToInt64(Product["EditCatgTypeId"]);
                    var ChkExistNot = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.SchId == schlId && c.CCTSP_CategoryDetails.CatgDesc == ProductName && c.Amount == ProductPrice && c.Duration == ProductDuration && c.Activestatus == "A" && c.CCTSP_CategoryDetails.ActiveStatus == "A" && c.CatgTypeId != CatgTypeId).FirstOrDefault();
                    if (ChkExistNot == null)
                    {
                        productAdvanceDetail = new CTSP_SolutionMaster();
                        productAdvanceDetail = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == CatgTypeId).FirstOrDefault();
                        //var ChangeEmployeeDataAlso = "update CTSP_SolutionMaster set Amount='" + ProductPrice + "',Duration='" + ProductDuration + "', 
                        //SectionDesc ='" + ProductName + "',image='" + ProductDescription + "',ItenName='" + Gender + "'
                        //where SectionName='EmployeeProduct' and SectionDesc='" + productAdvanceDetail.CCTSP_CategoryDetails.CatgDesc + "' and image='" + productAdvanceDetail.SectionDesc + "' and Amount='" + productAdvanceDetail.Amount + "'and Duration='" + productAdvanceDetail.Duration + "'  and ActiveStatus='A'";
                        //SPA.Database.ExecuteSqlCommand(ChangeEmployeeDataAlso);
                        var ChangeEmployeeDataAlso = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "EmployeeProduct" && c.SectionDesc == productAdvanceDetail.CCTSP_CategoryDetails.CatgDesc && c.Image == productAdvanceDetail.SectionDesc && c.Amount == productAdvanceDetail.Amount && c.Duration == productAdvanceDetail.Duration && c.Activestatus == "A").FirstOrDefault();
                        if (ChangeEmployeeDataAlso != null)
                        {
                            ChangeEmployeeDataAlso.Amount = ProductPrice;
                            ChangeEmployeeDataAlso.Duration = ProductDuration;
                            ChangeEmployeeDataAlso.SectionDesc = ProductName;
                            ChangeEmployeeDataAlso.Image = ProductDescription;
                            ChangeEmployeeDataAlso.ItenName = Gender;
                            UpdateModel(ChangeEmployeeDataAlso);
                            SPA.SaveChanges();
                        }
                        productAdvanceDetail.CCTSP_CategoryDetails.CatgDesc = ProductName;
                        productAdvanceDetail.CCTSP_CategoryDetails.VAT = Vat;
                        if (InsuranceId > 0)
                            productAdvanceDetail.CCTSP_CategoryDetails.Insurance_Id = InsuranceId;
                        productAdvanceDetail.CCTSP_CategoryDetails.Gender = Gender;
                        productAdvanceDetail.CCTSP_CategoryDetails.Email_Image = !string.IsNullOrEmpty(CustomText) ? CustomText : null;
                        productAdvanceDetail.Duration = ProductDuration;
                        productAdvanceDetail.Amount = Math.Round(double.Parse(ProductPrice, System.Globalization.NumberStyles.Any), 2).ToString();
                        productAdvanceDetail.SectionDesc = ProductDescription;
                        UpdateModel(productAdvanceDetail);
                        SPA.SaveChanges();
                        //New Code is changed to update Group order for search Attribute
                        var EditGroup = "update CCTSP_GroupProductDetails set GroupIdByShop=" + Groupid + ",GroupIdDefault=" + DefultGroupid + ",Attributeorder=" + Convert.ToInt16(SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == DefultGroupid && c.ActiveStatus == "A" && c.CatgId == 124).Select(c => c.Group_orderdata).FirstOrDefault().Value) + " where ProductId=" + CatgTypeId + " and ActiveStatus='A'";
                        SPA.Database.ExecuteSqlCommand(EditGroup);

                        Result = "No";
                    }
                    else
                    {
                        Result = "yes";
                    }
                }
                #endregion
                return Result;

            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }
        }
        public ActionResult ProductDataDisplay()
        {
            try
            {
                long UserId = Convert.ToInt32(Session["UserId"].ToString());
                string ProductList = " select c.Work_Flow_Id,a.catgDesc as ProductName,a.CatgTypeId as ProductId,a.Gender," +
                "b.SectionDesc,CAST(CASE WHEN ISNUMERIC(b.Amount) = 1 THEN b.Amount ELSE NULL END AS decimal(38, 2)) as Amount,cast(b.Duration as int) as Duration,b.OrderData, b.SolutionId," +
                "c.CatgTypeId as GroupIdByShop /*,c.GroupIdDefault */,c.Group_orderdata,c.CatgDesc as GroupName, " +
                "g.AccessToData, g.insertAccess,g.DeleteAccess,g.UpdateAccess,f.ItenName as FlowStatus, " +
                " Insurance=case when h.Settlement_text is null then a.Email_Image else h.Settlement_text end " +
                /*,d.Group_orderdata,d.CatgDesc as GroupName
                ,e.CatgDesc as DefaultGroupName */
                "from CCTSP_CategoryDetails a join ctsp_solutionMaster b on a.catgTypeId = b.catgTypeId " +
                "join cctsp_categoryDetails c on c.catgId = 126 " +
                "join ctsp_SolutionMaster f on f.SectionName = 'Services' " +
                "join cctsp_user d on d.UserId = " + UserId + " " +
                "join cctsp_Role e on e.RoleId = d.RoleId and(e.Schlid = " + schlId + " or e.Schlid = 236) and e.ActiveStatus = 'A' " +
                "join cctsp_RoleDetails g on g.ActiveStatus = 'A' and e.RoleId = g.RoleId " +
                "and g.Schid = e.Schlid    and f.SolutionId = convert(bigint, g.AccesstoSub) " +
                "left join Health_Insurance h on h.Insurance_Id=a.Insurance_Id " +
                "where a.domaintype = " + schlId + " and a.catgId = 111 and a.ActiveStatus = 'A' " +
                "and b.schId = a.domaintype and b.ActiveStatus = 'A' " +
                "and c.ActiveStatus = 'A' and c.DomainType = a.domaintype and " +
                "c.CatgTypeId in (select GroupIdbyShop from CCTSP_GroupProductDetails  " +
                "where schlId = a.domaintype and ProductId = a.CatgTypeId and ActiveStatus = 'A') " +
                "order by c.Group_orderdata,b.orderdata";
                var ProductDetail = SPA.Database.SqlQuery<Models.GroupProductList>(ProductList).ToList();
                ViewBag.ProductDetail = ProductDetail;
                /*Shop Information like Currency and Lang_Id*/
                var ShopInfo = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId)
                                .Select(c => new Models.ShopMasterDetail { Lang_id = c.Lang_id, Currency = c.Currency, Display_Invoice = c.Display_Invoice })
                                .FirstOrDefault();
                /*Check Invoice Flow*/
                ViewBag.CheckInvoice = fu.CheckInvoice(ShopInfo.Display_Invoice);
                ViewBag.currency = fu.currency(ShopInfo.Currency);
                ViewBag.Language = fu.getLanguageData("Product_Catalog", 0, ShopInfo.Lang_id.Value);
                ViewBag.ProductDetailCount = ProductDetail.Count;
                ViewBag.GroupListCount = ProductDetail.Select(c => c.GroupIdByShop).Distinct().Count();
                ViewBag.InsuranceList = SPA.Health_Insurance.Where(c => c.ActiveStatus == "A" && c.LangId == ShopInfo.Lang_id).Select(c => new Models.InsuranceList
                {
                    Insurance = c.Settlement_text,
                    InsuranceId = c.Insurance_Id
                }).ToList().Count;
                return View();
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public JObject EditGroupNameDetail(long Catgtypeid)
        {
            try
            {
                JObject send = null;
                string jasondata = "";
                var DefaultGroupName = SPA.CCTSP_CategoryDetails.Where(c => c.ActiveStatus == "A" && c.CatgTypeId == Catgtypeid).Select(c => c.CatgDesc).FirstOrDefault();
                Models.EditShopDetails GroupDetails = new Models.EditShopDetails()
                { Catgtypeid = Catgtypeid, CatgDesc = DefaultGroupName };
                jasondata = js.Serialize(GroupDetails);
                send = JObject.Parse(jasondata);
                return send;
            }
            catch (Exception e)
            {
                JObject Send = null;
                fu.ErrorSend(RouteData, e);
                return Send;
            }

        }
        public void DeleteGroupNameDetail(long? Catgtypeid)
        {
            string ProductList = "select a.catgDesc as ProductName, CAST(CASE WHEN ISNUMERIC(b.Amount) = 1 THEN b.Amount ELSE NULL END AS decimal(38, 2)) as Amount,cast(b.Duration as int) as Duration,b.SectionDesc ,d.CatgDesc as GroupName , a.CatgTypeId as ProductId  ,a.Gender ,b.OrderData, b.SolutionId  ,c.GroupIdByShop ,e.CatgDesc as DefaultGroupName ,d.Group_orderdata from CCTSP_CategoryDetails a join ctsp_solutionMaster b on a.catgTypeId = b.catgTypeId join CCTSP_GroupProductDetails c on c.productId = a.CatgTypeId join cctsp_categoryDetails d on d.CatgTypeId = c.GroupIdbyShop  join cctsp_categoryDetails e on e.CatgTypeId = c.GroupIdDefault where  a.Activestatus = 'A' and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' and d.ActiveStatus = 'A' and e.ActiveStatus = 'A' and a.domaintype = " + schlId + " and b.schId = " + schlId + " and d.DomainType = " + schlId + " and c.schlid = " + schlId + " and a.catgId = 111  and d.CatgId = 126  and e.CatgId = 124 and  d.CatgTypeId =" + Catgtypeid + " order by OrderData desc";
            var ProductDetail = SPA.Database.SqlQuery<Models.GroupProductList>(ProductList).ToList();
            string EmployeeProductlist = "select d.SolutionId from CCTSP_GroupProductDetails a join CCTSP_CategoryDetails b on a.ProductId = b.CatgTypeId join Ctsp_SolutionMaster c on c.CatgTypeId = b.CatgTypeId join Ctsp_solutionMaster d on d.SectionDesc = b.CatgDesc where a.Activestatus = 'A' and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' and d.ActiveStatus = 'A' and a.schlId =" + schlId + " and b.DomainType = " + schlId + " and d.schId = " + schlId + " and c.schid = " + schlId + " and b.catgId = 111 and a.GroupIdByShop = " + Catgtypeid + " and d.CatgTypeId = 11145 and c.Amount = d.Amount and c.Duration = d.Duration and c.SectionDesc = d.[Image]";
            var EmployeeProductDetails = SPA.Database.SqlQuery<Models.GroupProductList>(EmployeeProductlist).ToList();
            //For Delete Employee Product 
            if (EmployeeProductDetails != null && EmployeeProductDetails.Count > 0)
            {
                var EmployeeProductIdlist = string.Join(",", EmployeeProductDetails.Select(c => c.SolutionId).ToArray());
                var EmployeeProduct = "update CTSP_SolutionMaster set ActiveStatus='D' where SolutionId in (" + EmployeeProductIdlist + ")";
                SPA.Database.ExecuteSqlCommand(EmployeeProduct);
            }
            //For Delete Groupname
            var DeleteGroupname = "update CCTSP_CategoryDetails set ActiveStatus='D' where CatgTypeId=" + Catgtypeid;
            SPA.Database.ExecuteSqlCommand(DeleteGroupname);
            //For Delete Groupid from list
            var DeleteGroupid = "update CCTSP_GroupProductDetails set ActiveStatus='D' where GroupIdByShop=" + Catgtypeid;
            SPA.Database.ExecuteSqlCommand(DeleteGroupid);
            //For Delete Product Details
            if (ProductDetail.Count > 0 && ProductDetail != null)
            {
                var ProductIdlist = string.Join(",", ProductDetail.Select(c => c.ProductId).ToArray());
                var ProductName = "update CCTSP_CategoryDetails set ActiveStatus='D' where CatgTypeId in (" + ProductIdlist + ")";
                SPA.Database.ExecuteSqlCommand(ProductName);
                var ProductDetails = "update CTSP_SolutionMaster set ActiveStatus='D' where CatgTypeId in (" + ProductIdlist + ")";
                SPA.Database.ExecuteSqlCommand(ProductDetails);
            }
        }
        public void DeleteProduct(long? CatgTypeId)
        {
            try
            {
                //Deleting Data from Shop
                var query = "update CTSP_SolutionMaster set ActiveStatus='D' where CatgTypeId=" + CatgTypeId;
                SPA.Database.ExecuteSqlCommand(query);
                var query1 = "update CCTSP_CategoryDetails set ActiveStatus='D' where CatgTypeId=" + CatgTypeId;
                SPA.Database.ExecuteSqlCommand(query1);
                //Getting Product Which Are To Be Removed
                var ProductName = SPA.CTSP_SolutionMaster.Where(c => c.CatgTypeId == CatgTypeId).FirstOrDefault();
                //Removing Same Product From Employee
                var query2 = "update CTSP_SolutionMaster set ActiveStatus='D' where SchId=" + schlId + " and SectionName='EmployeeProduct' and ActiveStatus='A' and SectionDesc='" + ProductName.CCTSP_CategoryDetails.CatgDesc + "' and Amount='" + ProductName.Amount + "'and Duration='" + ProductName.Duration + "'";
                SPA.Database.ExecuteSqlCommand(query2);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public JObject EditProductDetail(long CatgTypeId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                var Data = SPA.CTSP_SolutionMaster.Where(c => c.CCTSP_CategoryDetails.CCTSP_CategoryMaster.CatgName == "SPA Products" && c.CCTSP_CategoryDetails.ActiveStatus == "A" && c.SectionName == "ChoosProduct" && c.Activestatus == "A" && c.CatgTypeId == CatgTypeId).FirstOrDefault();
                var groupdata = SPA.CCTSP_GroupProductDetails.Where(c => c.ProductId == CatgTypeId && c.ActiveStatus == "A").FirstOrDefault();
                var groupname = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == groupdata.GroupIdByShop && c.ActiveStatus == "A" && c.DomainType == schlId).Select(c => c.CatgDesc).FirstOrDefault();
                var DefaultGroupname = SPA.CCTSP_CategoryDetails.Where(c => c.CatgTypeId == groupdata.GroupIdDefault && c.ActiveStatus == "A").Select(c => c.CatgDesc).FirstOrDefault();
                var InsuranceInfo = new Models.InsuranceList();
                if (Data.CCTSP_CategoryDetails.Insurance_Id != null)
                    InsuranceInfo = SPA.Health_Insurance.Where(c => c.Insurance_Id == Data.CCTSP_CategoryDetails.Insurance_Id).Select(c => new Models.InsuranceList
                    {
                        Insurance = c.Settlement_text,
                        InsuranceId = c.Insurance_Id,
                        Settlement_Number = c.Settlement_Number,
                        Tarif_Number = c.Tarif_Number,
                        DefaultStatus = c.Field1
                    }).FirstOrDefault();
                if (Data.SectionDesc == null)
                    Data.SectionDesc = "";
                Models.GroupProductList EditProduct = new Models.GroupProductList()
                {
                    ProductId = CatgTypeId,
                    ProductName = Data.CCTSP_CategoryDetails.CatgDesc,
                    Duration = Convert.ToInt32(Data.Duration),
                    Amount = Convert.ToDecimal(Data.Amount),
                    SectionDesc = Data.SectionDesc,
                    Gender = Data.CCTSP_CategoryDetails.Gender,
                    GroupName = groupname,
                    GroupIdByDefault = groupdata.GroupIdDefault.Value,
                    GroupIdByShop = groupdata.GroupIdByShop.Value,
                    DefaultGroupName = DefaultGroupname,
                    Insurance = InsuranceInfo != null && InsuranceInfo.Insurance != null ? InsuranceInfo.Insurance : "",
                    InsuranceId = InsuranceInfo != null && InsuranceInfo.InsuranceId > 0 ? InsuranceInfo.InsuranceId : 0,
                    Settlement_Number = InsuranceInfo != null && InsuranceInfo.Settlement_Number != null ? InsuranceInfo.Settlement_Number.ToString() : "",
                    Tarif_Number = InsuranceInfo.Tarif_Number,
                    CustomText = Data.CCTSP_CategoryDetails.Email_Image != null ? Data.CCTSP_CategoryDetails.Email_Image : "",
                    DefaultStatus = InsuranceInfo != null && InsuranceInfo.DefaultStatus != null ? InsuranceInfo.DefaultStatus : "",
                    vat = Data.CCTSP_CategoryDetails.VAT != null ? string.Format("{0:0.0}", Data.CCTSP_CategoryDetails.VAT) : "0.0"
                };
                jsondata = js.Serialize(EditProduct);
                send = JObject.Parse(jsondata);
                return send;

                //FinalData = Data.CatgTypeId + "~" + Data.CCTSP_CategoryDetails.CatgDesc + "~" + Data.Duration + "~" + Data.Amount + "~" + Data.SectionDesc + "~" + Data.CCTSP_CategoryDetails.Gender + "~" + groupname + "~" + groupdata.GroupIdDefault + "~" + groupdata.GroupIdByShop + "~" + DefaultGroupname;
                //return FinalData;
            }
            catch (Exception e)
            {
                JObject FinalData = null;
                fu.ErrorSend(RouteData, e);
                return FinalData;
            }

        }
        public string GetDataOfLanguage(string Text, string id)
        {
            try
            {
                var LanguageId = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => c.Lang_id).FirstOrDefault();
                Language_Label_Detail LanguageDetail = new Language_Label_Detail();
                if (LanguageId == null && LanguageId == 0)
                    LanguageId = 1;

                int orderId = Convert.ToInt32(id);
                LanguageDetail = SPA.Language_Label_Detail.Where(c => c.Order_id == orderId && c.Lang_id == LanguageId && c.Page_Name == "AlertMsg").FirstOrDefault();
                if (LanguageDetail != null)
                    return LanguageDetail.Value;
                else
                    return "";
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }

        }
        public void GoUp(int index, long Id, long otherId, string Status)
        {
            try
            {
                if (Status == "ProductIndexing")
                {
                    List<CTSP_SolutionMaster> dataChange = new List<CTSP_SolutionMaster>();
                    var Product1 = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.Activestatus == "A" && c.SchId == schlId && c.OrderData == index && c.SolutionId == Id).FirstOrDefault();
                    var Product2 = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.Activestatus == "A" && c.SchId == schlId && c.SolutionId == otherId).FirstOrDefault();
                    long orderpro1 = Product1.OrderData.Value;
                    long orderpro2 = Product2.OrderData.Value;
                    var query = "update CTSP_SolutionMaster set OrderData=" + orderpro2 + " where SolutionId=" + Id;
                    SPA.Database.ExecuteSqlCommand(query);
                    var query1 = "update CTSP_SolutionMaster set OrderData=" + orderpro1 + " where SolutionId=" + Product2.SolutionId;
                    SPA.Database.ExecuteSqlCommand(query1);
                }
                if (Status == "GroupIndexing")
                {
                    var Product1 = SPA.CCTSP_CategoryDetails.Where(c => c.DomainType == schlId && c.ActiveStatus == "A" && c.Group_orderdata == index && c.CatgTypeId == Id && c.CatgId == 126).FirstOrDefault();
                    var Product2 = SPA.CCTSP_CategoryDetails.Where(c => c.DomainType == schlId && c.ActiveStatus == "A" && c.CatgTypeId == otherId).FirstOrDefault();
                    long orderpro1 = Product1.Group_orderdata.Value;
                    long orderpro2 = Product2.Group_orderdata.Value;
                    var query = "update CCTSP_CategoryDetails set Group_orderdata=" + orderpro2 + "where CatgTypeId = " + Id;
                    SPA.Database.ExecuteSqlCommand(query);
                    var query1 = "update CCTSP_CategoryDetails set Group_orderdata=" + orderpro1 + " where CatgTypeId=" + Product2.CatgTypeId;
                    SPA.Database.ExecuteSqlCommand(query1);

                }

                #region OldCode

                //CTSP_SolutionMaster solution = new CTSP_SolutionMaster();
                //CTSP_SolutionMaster solutionChange = new CTSP_SolutionMaster();
                //foreach (var item in ProductList)
                //{
                //    if (item.CatgTypeId == Id)
                //    {
                //        solutionChange = ProductList.Where(c => c.CatgTypeId == Done).FirstOrDefault();
                //        solution = item;
                //        long oldOrder = Convert.ToInt64(solution.OrderData);
                //        long newOrder = Convert.ToInt64(solutionChange.OrderData);
                //        fu.ChangeData(solution, newOrder);
                //        fu.ChangeData(solutionChange, oldOrder);
                //        break;
                //    }
                //    Done = item.CatgTypeId;
                //}
                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public void GoDown(int index, long Id, long otherId, string Status)
        {
            try
            {
                if (Status == "ProductIndexing")
                {
                    List<CTSP_SolutionMaster> dataChange = new List<CTSP_SolutionMaster>();
                    var Product1 = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.Activestatus == "A" && c.SchId == schlId && c.OrderData == index && c.SolutionId == Id).FirstOrDefault();
                    var Product2 = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.Activestatus == "A" && c.SchId == schlId && c.SolutionId == otherId).FirstOrDefault();
                    long orderpro1 = Product1.OrderData.Value;
                    long orderpro2 = Product2.OrderData.Value;
                    var query = "update CTSP_SolutionMaster set OrderData=" + orderpro2 + " where SolutionId=" + Id;
                    SPA.Database.ExecuteSqlCommand(query);
                    var query1 = "update CTSP_SolutionMaster set OrderData=" + orderpro1 + " where SolutionId=" + Product2.SolutionId;
                    SPA.Database.ExecuteSqlCommand(query1);
                }
                if (Status == "GroupIndexing")
                {
                    var Product1 = SPA.CCTSP_CategoryDetails.Where(c => c.DomainType == schlId && c.ActiveStatus == "A" && c.Group_orderdata == index && c.CatgTypeId == Id && c.CatgId == 126).FirstOrDefault();
                    var Product2 = SPA.CCTSP_CategoryDetails.Where(c => c.DomainType == schlId && c.ActiveStatus == "A" && c.CatgTypeId == otherId).FirstOrDefault();
                    long orderpro1 = Product1.Group_orderdata.Value;
                    long orderpro2 = Product2.Group_orderdata.Value;
                    var query = "update CCTSP_CategoryDetails set Group_orderdata=" + orderpro2 + " where CatgTypeId=" + Id;
                    SPA.Database.ExecuteSqlCommand(query);
                    var query1 = "update CCTSP_CategoryDetails set Group_orderdata=" + orderpro1 + " where CatgTypeId=" + Product2.CatgTypeId;
                    SPA.Database.ExecuteSqlCommand(query1);
                }
                #region OldCode
                //int Done = 0;
                //var ProductList = SPA.CTSP_SolutionMaster.Where(c => c.SectionName == "ChoosProduct" && c.Activestatus == "A" && c.SchId == schlId).OrderByDescending(c => c.OrderData);
                //CTSP_SolutionMaster solution = new CTSP_SolutionMaster();
                //CTSP_SolutionMaster solutionChange = new CTSP_SolutionMaster();
                //foreach (var item in ProductList)
                //{
                //    if (Done == 1)
                //    {
                //        solutionChange = item;
                //        long oldOrder = Convert.ToInt64(solution.OrderData);
                //        long newOrder = Convert.ToInt64(solutionChange.OrderData);
                //        fu.ChangeData(solutionChange, oldOrder);
                //        fu.ChangeData(solution, newOrder);
                //        break;
                //    }
                //    if (item.CatgTypeId == Id)
                //    {
                //        solution = item;
                //        Done = 1;
                //    }
                //}
                #endregion
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public string GetCommonLangauge(string PageName, int? orderid)
        {
            try
            {
                var SchlLanguage = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").FirstOrDefault();
                var LangaugeContent = SPA.Language_Label_Detail.Where(c => c.Order_id == orderid && c.Page_Name == PageName && c.Lang_id == SchlLanguage.Lang_id).FirstOrDefault();
                return LangaugeContent.Value;
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return "";
            }
        }
        [CustomAutohrize(null)]
        public ActionResult AddOnProduct()
        {
            var Shopinfo = SPA.CCTSP_SchoolMaster.Where(c => c.SchlId == schlId && c.ActiveStatus == "A").Select(c => new Models.shopMaster
            {
                LangId = c.Lang_id,
                Currency = c.Currency,
                Display_Invoice = c.Display_Invoice
            }).FirstOrDefault();
            ViewBag.Currency = Shopinfo.Currency;
            ViewBag.Vat = SPA.CCTSP_CategoryDetails.Where(c => c.CatgId == 174).Select(c => new Models.CatgDetails { CatgDesc = c.CatgDesc, CatgId = c.CatgId }).OrderBy(c => c.CatgDesc).ToList();
            var ProductList = SPA.Add_On_Product_Detail.Where(c => c.ActiveStatus == "A" && c.Add_on_Product_Master.ActiveStatus == "A" && c.Add_on_Product_Master.schlId == schlId).Select(c => new Models.AddonProduct
            {
                ProductDesc = c.Add_on_Product_Master.Field1,
                ProductName = c.Add_on_Product_Master.ProductName,
                Selling_Price = c.Selling_Price,
                Buying_Price = c.Buying_Price,
                Quantity = c.Quantity,
                AddOnProductId = c.Add_On_Pid,
                Manufacturer = c.Add_on_Product_Master.Manufacturer,
                Dozes = c.Add_on_Product_Master.Dozes,
                insurance = c.Add_on_Product_Master.Health_Insurance.Settlement_text != null ? c.Add_on_Product_Master.Health_Insurance.Settlement_text : c.Add_on_Product_Master.Field3,
            }).ToList();
            ViewBag.InsuranceList = SPA.Health_Insurance.Where(c => c.ActiveStatus == "A" && c.LangId == Shopinfo.LangId).Select(c => new Models.InsuranceList
            {
                Insurance = c.Settlement_text,
                InsuranceId = c.Insurance_Id,
                Settlement_Number = c.Settlement_Number,
                Tarif_Number = c.Tarif_Number,
                DefaultStatus = c.Field1
            }).ToList();
            /*Check Invoice Flow*/
            ViewBag.CheckInvoice = fu.CheckInvoice(Shopinfo.Display_Invoice);
            ViewBag.LanguageInfo = SPA.Language_Label_Detail.Where(c => c.ActiveStatus == "A" && c.Page_Name == "Add_On_Product" && c.Lang_id == Shopinfo.LangId).Select(c => new Models.LanguageLabelDetails
            {
                Order_id = c.Order_id,
                Lang_id = c.Lang_id,
                English_Label = c.English_Label,
                Value = c.Value,
                Page_Name = c.Page_Name
            }).ToList();
            return View(ProductList);
        }
        [HttpPost]
        public ActionResult AddOnProduct(Models.AddonProduct Product)
        {
            Add_on_Product_Master Masterproduct = new Add_on_Product_Master();
            Add_On_Product_Detail ProductDetails = new Add_On_Product_Detail();
            if (Product.AddOnProductId == null)
            {
                //var CheckExist = SPA.Add_On_Product_Detail.Where(c => c.Add_on_Product_Master.ActiveStatus == "A" && c.ActiveStatus == "A" && c.Buying_Price == Product.Buying_Price && c.Selling_Price == Product.Selling_Price && c.Add_on_Product_Master.ProductName == Product.ProductName).FirstOrDefault();
                //if (CheckExist == null)
                //{
                Masterproduct.ProductName = Product.ProductName;
                Masterproduct.schlId = schlId;
                Masterproduct.Field1 = Product.ProductDesc;
                Masterproduct.Creation_Date = DateTime.Now;
                Masterproduct.Updation_Date = DateTime.Now;
                Masterproduct.ActiveStatus = "A";
                Masterproduct.Manufacturer = Product.Manufacturer;
                Masterproduct.Dozes = Product.Dozes;
                Masterproduct.VAT = double.Parse(Product.SelectedVat != null ? Product.SelectedVat : "0.0");
                Masterproduct.Field3 = !string.IsNullOrEmpty(Product.AddOn_C_Settlement_Text) ? Product.AddOn_C_Settlement_Text : null;
                if (Product.SelectedAddOnPInsurance > 0)
                    Masterproduct.Insurance_Id = Product.SelectedAddOnPInsurance;
                SPA.Add_on_Product_Master.Add(Masterproduct);
                SPA.SaveChanges();
                ProductDetails.Add_On_Pid = Masterproduct.Add_On_Pid;
                ProductDetails.ActiveStatus = "A";
                ProductDetails.Created_Date = DateTime.Now;
                ProductDetails.Buying_Price = Product.Buying_Price;
                ProductDetails.Selling_Price = Product.Selling_Price;
                //ProductDetails.Quantity = Product.Quantity;
                ProductDetails.updated_Date = DateTime.Now;
                ProductDetails.SchlId = schlId;

                SPA.Add_On_Product_Detail.Add(ProductDetails);
                SPA.SaveChanges();
                //}
                //else
                //    Session["Message"] = "AlreadyExistProduct";
            }
            else
            {
                //var CheckExist = SPA.Add_On_Product_Detail.Where(c => c.Add_on_Product_Master.ActiveStatus == "A" && c.ActiveStatus == "A" && c.Buying_Price == Product.Buying_Price && c.Selling_Price == Product.Selling_Price && c.Add_on_Product_Master.ProductName == Product.ProductName && c.Add_On_Pid != Product.AddOnProductId).FirstOrDefault();
                //if (CheckExist == null)
                //{
                ProductDetails = SPA.Add_On_Product_Detail.Where(c => c.Add_On_Pid == Product.AddOnProductId).FirstOrDefault();
                ProductDetails.Add_on_Product_Master.ProductName = Product.ProductName;
                ProductDetails.Add_on_Product_Master.VAT = double.Parse(Product.SelectedVat != null ? Product.SelectedVat : "0.0");
                ProductDetails.Add_on_Product_Master.Field1 = Product.ProductDesc;
                ProductDetails.Add_on_Product_Master.Manufacturer = Product.Manufacturer;
                ProductDetails.Add_on_Product_Master.Field3 = !string.IsNullOrEmpty(Product.AddOn_C_Settlement_Text) ? Product.AddOn_C_Settlement_Text : null;
                if (Product.SelectedAddOnPInsurance > 0)
                    ProductDetails.Add_on_Product_Master.Insurance_Id = Product.SelectedAddOnPInsurance;
                ProductDetails.Add_on_Product_Master.Dozes = Product.Dozes;
                //ProductDetails.Quantity = Product.Quantity;
                ProductDetails.Buying_Price = Product.Buying_Price;
                ProductDetails.Selling_Price = Product.Selling_Price;
                ProductDetails.updated_Date = DateTime.Now;
                ProductDetails.Add_on_Product_Master.Updation_Date = DateTime.Now;
                UpdateModel(ProductDetails);
                SPA.SaveChanges();
                //}
                //else
                //    Session["Message"] = "AlreadyExistProduct";
            }
            return Redirect("/Product/Product_Catalog#AddOnProduct");
        }
        public JObject EditAddOnProductDetail(long AddOnProductId)
        {
            try
            {
                JObject send = null;
                string jsondata = "";
                var Data = SPA.Add_On_Product_Detail.Where(c => c.ActiveStatus == "A" && c.Add_on_Product_Master.ActiveStatus == "A" && c.Add_on_Product_Master.Add_On_Pid == AddOnProductId).FirstOrDefault();
                var InsuranceInfo = new Models.InsuranceList();
                if (Data.Add_on_Product_Master.Insurance_Id != null)
                {
                    InsuranceInfo = SPA.Health_Insurance.Where(c => c.Insurance_Id == Data.Add_on_Product_Master.Insurance_Id).Select(c => new Models.InsuranceList
                    {
                        Insurance = c.Settlement_text,
                        InsuranceId = c.Insurance_Id,
                        Tarif_Number = c.Tarif_Number,
                        Settlement_Number = c.Settlement_Number,
                        DefaultStatus = c.Field1

                    }).FirstOrDefault();
                }
                Models.AddonProduct Product = new Models.AddonProduct()
                {
                    AddOnProductId = AddOnProductId,
                    ProductName = Data.Add_on_Product_Master.ProductName,
                    Buying_Price = Data.Buying_Price,
                    Selling_Price = Data.Selling_Price,
                    ProductDesc = Data.Add_on_Product_Master.Field1,
                    Quantity = Data.Quantity,
                    Manufacturer = Data.Add_on_Product_Master.Manufacturer,
                    Dozes = Data.Add_on_Product_Master.Dozes,
                    SelectedAddOnPInsurance = InsuranceInfo.InsuranceId > 0 ? InsuranceInfo.InsuranceId : 0,
                    insurance = InsuranceInfo.Insurance != null ? InsuranceInfo.Insurance : "",
                    Settlement_Number = InsuranceInfo.Settlement_Number != null ? InsuranceInfo.Settlement_Number.ToString() : "",
                    Tarif_Number = InsuranceInfo.Tarif_Number,
                    SelectedVat = Data.Add_on_Product_Master.VAT != null ? string.Format("{0:0.0}", Data.Add_on_Product_Master.VAT) : "0.0",
                    AddOn_C_Settlement_Text = Data.Add_on_Product_Master.Field3 != null ? Data.Add_on_Product_Master.Field3 : "",
                    DefaultStatus = InsuranceInfo.DefaultStatus != null ? InsuranceInfo.DefaultStatus : ""
                };
                jsondata = js.Serialize(Product);
                send = JObject.Parse(jsondata);
                return send;
            }
            catch (Exception e)
            {
                JObject FinalData = null;
                fu.ErrorSend(RouteData, e);
                return FinalData;
            }

        }
        public void DeleteAddOnProduct(long AddOnProductId)
        {
            try
            {
                var ProductDetails = SPA.Add_On_Product_Detail.Where(c => c.Add_on_Product_Master.Add_On_Pid == AddOnProductId).FirstOrDefault();
                if (ProductDetails != null)
                {
                    ProductDetails.Add_on_Product_Master.ActiveStatus = "D";
                    ProductDetails.ActiveStatus = "D";
                    UpdateModel(ProductDetails);
                    SPA.SaveChanges();
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
        }
        public ActionResult SettlementText()
        {
            try
            {
                var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Favourite_Service" && c.ActiveStatus == "A")
                    .Select(c=>new Models.LanguageLabelDetails { Value=c.Value,English_Label=c.English_Label,Lang_id=c.Lang_id,Order_id=c.Order_id})
                    .ToList();
                List<Models.settlementTxt> settleTxt = SPA.Database.SqlQuery<Models.settlementTxt>(new Sql().getSettlement(schlId, false, Lang_id.Value,null)).ToList();
                return View(settleTxt);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public ActionResult getAssignedSettlementText()
        {
            try
            {
                var Lang_id = SPA.CCTSP_SchoolMaster.Where(c => c.ActiveStatus == "A" && c.SchlId == schlId).Select(c => c.Lang_id).FirstOrDefault();
                ViewBag.Language = SPA.Language_Label_Detail.Where(c => c.Page_Name == "Favourite_Service" && c.ActiveStatus == "A")
                    .Select(c => new Models.LanguageLabelDetails { Value = c.Value, English_Label = c.English_Label, Lang_id = c.Lang_id, Order_id = c.Order_id })
                    .ToList();
                List<Models.settlementTxt> settleTxt = SPA.Database.SqlQuery<Models.settlementTxt>(new Sql().getSettlement(schlId, true, Lang_id.Value,null)).ToList();
                return View(settleTxt);
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        [HttpPost]
        public ActionResult AssignSettlementText(FormCollection settlement)
        {
            try
            {
                if (settlement.Count > 0)
                {
                    long setText = 0;
                    //All the Selected Settlement from UI
                    var SelectedInsuranceId = Convert.ToString(settlement["Settlement"]).Split(',').Select(c => long.TryParse(c, out setText) == true ? Convert.ToInt64(c) : 0).Where(c => c != 0).ToList();
                    if (SelectedInsuranceId.Count > 0)
                    {
                        //Check From all selected which are already there in database
                        var getselectedFromDb = SPA.Assigned_Health_Insurance
                                                .Where(d => SelectedInsuranceId.Contains(d.Insurance_Id.Value)
                                                && d.ActiveStatus == "A"
                                                && d.SchlId == schlId)
                                                .Select(c => c.Insurance_Id)
                                                .ToList();
                        //All InsuranceId Detail except which are already added into the database
                        var getAllInsuranceDetail = SPA.Health_Insurance
                                                    .Where(d => !getselectedFromDb.Contains(d.Insurance_Id) && SelectedInsuranceId.Contains(d.Insurance_Id)
                                                    && d.ActiveStatus == "A")
                                                    .ToList();
                        var EuropeDate = fu.ZonalDate(null);
                        //Add All the Remaining Settlement which are not added into database
                        List<Assigned_Health_Insurance> AsHealthInsurance = getAllInsuranceDetail.Select(c => new Assigned_Health_Insurance
                        {
                            ActiveStatus = "A",
                            Created_Date = EuropeDate,
                            Updated_Date = EuropeDate,
                            SchlId = schlId,
                            Insurance_Id = c.Insurance_Id,
                            Tarif_Number = c.Tarif_Number,
                            SettlementNumber = c.Settlement_Number,
                            Settlement_text = c.Settlement_text,
                            order = c.order,
                            Field1=c.Field1
                        }).ToList();
                        if (AsHealthInsurance.Count > 0)
                        {
                            SPA.Assigned_Health_Insurance.AddRange(AsHealthInsurance);
                            SPA.SaveChanges();
                        }
                    }
                }
                return Redirect("/Product/Product_Catalog#SettlementText");
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
                return RedirectToAction("Error_List", "Error");
            }
        }
        public bool AddMissingInformation(List<Models.settlementTxt> settleTxt)
        {
            if (settleTxt.Count > 0)
            {
                var ListHealthAssignId = settleTxt.Where(c => c.Health_Assign_id != null).Select(c => c.Health_Assign_id).ToList();
                if (ListHealthAssignId.Count > 0)
                {
                    var getSettlement = SPA.Assigned_Health_Insurance.Where(c => ListHealthAssignId.Contains(c.Health_Assign_id) && c.ActiveStatus == "A" && c.SchlId == schlId).ToList();
                    foreach (var gettsettle in getSettlement)
                    {
                        var SingleSet = settleTxt.Where(c => c.Health_Assign_id == gettsettle.Health_Assign_id).FirstOrDefault();
                        gettsettle.Duration = SingleSet.Duration;
                        gettsettle.Amount = SingleSet.Amount;
                    }
                    SPA.SaveChanges();
                }
            }
            return true;
        }
        public bool DeleteAssignedSettlement(long Id)
        {
            try
            {
                var getAssigned = SPA.Assigned_Health_Insurance.Where(c => c.Health_Assign_id == Id).FirstOrDefault();
                if (getAssigned != null)
                {
                    getAssigned.ActiveStatus = "D";
                    SPA.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                fu.ErrorSend(RouteData, e);
            }
            return false;
        }
    }
}