using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SPA.Common
{
    public class Sql
    {
        CultureInfo enGB = new CultureInfo("en-GB");
        #region PaymentHistory
        public string getPayHistory(long? schId)
        {
            var payHistory = " select TBL.ShopId,TBL.Monthd,convert(int,TBL.Months) as Months,convert(int,TBL.years) as years,b.Currency,TBL.MonthNames," +
                                " (" +
                                " select sum(Credit) from spa_BillingDetails" +
                                " where FORMAT(Updated_On, 'yyyy')<=TBL.years and FORMAT(Updated_On, 'MM')<=TBL.Months" +
                                " and ShopId=" + schId +
                                " ) " +
                                " as CreditRemaining" +
                             " from" +
                                " (" +
                                " select ShopId, DateName(Month, Updated_On) + ',' + Format(Updated_On, 'yyyy')" +
                                " as Monthd, Format(Updated_On, 'yyyy') as Years," +
                                " Format(Updated_On, 'MM') as Months,DateName(Month, Updated_On) as MonthNames" +
                                " from spa_BillingDetails where ShopId = " + schId + " and ActiveStatus = 'A'" +
                                " and Credit is not null" +
                                " ) as TBL" +
                             " JOIN cctsp_schoolMaster b ON b.SchlId=TBL.ShopId" +
                             " Group by TBL.Monthd,TBL.ShopId,TBL.Years,TBL.Months,b.Currency,TBL.MonthNames" +
                             " Order by TBL.Years,TBL.Months";
            return payHistory;
        }
        #endregion
        #region Invoice
        public string InvoiceGenerated(string status, long SchlId, string FromDate, string Todate, long UserId)
        {
            var Data = " SELECT a.Invoice_Id,a.CustomerFirstName as CustomerFName,a.CustomerLastName as CustomerLName," +
                    " c.EmailId as CustomerEmail,a.CustomerId,a.UpdatedDate as CreatedDate,d.Currency," +
                    " c.Invoice_service,Amount = sum(ISNULL(b.TotalPricewithvat,(b.TotalPrice+((b.TotalPrice*ISNULL(b.VAT,0))/100))))," +
                    " CountReservation = sum(case when b.ReservationId is null then 0 else 1 end), " +
                    " j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName as FlowStatus" +
                    " from SPA_INVOICE_MASTER a" +
                    " JOIN SPA_INVOICE_Detail b ON b.Invoice_Id = a.Invoice_Id" +
                    " JOIN CCTSP_USER c on c.UserId = a.CustomerId" +
                    " JOIN CCTSP_SCHOOLMASTER d on d.schlId = a.schlId and d.ActiveStatus = 'A'" +
                    " JOIN ctsp_SolutionMaster g ON g.SectionName = 'Invoice Generated' and g.Activestatus='A' " +
                    " JOIN cctsp_user h on h.UserId = " + UserId + " " +
                    " JOIN cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + SchlId + " or i.schlid = 236) " +
                    " AND i.ActiveStatus = 'A' " +
                    " JOIN cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                    " AND j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub) " +
                    " AND j.SchId = i.Schlid " +
                    " AND((j.AccessToData = 'Own' and a.EmployeeId = h.UserId) or(j.AccessToData != 'Own')) " +
                    " WHERE a.schlId = " + SchlId + " and a.ActiveStatus = 'A' and b.ActiveStatus = 'A' " +
                    " and a.Invoice_Status = '" + status + "'";
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(Todate))
                Data += " and CONVERT(DATE,a.UpdatedDate) >= '" + DateTime.Parse(FromDate, enGB).ToString("yyyy/MM/dd") + "' and CONVERT(DATE,a.UpdatedDate) <= '" + DateTime.Parse(Todate, enGB).ToString("yyyy/MM/dd") + "'";
            Data += " GROUP BY a.Invoice_Id,a.CustomerFirstName,a.CustomerLastName,c.EmailId,a.CustomerId," +
                    " a.UpdatedDate,d.Currency,c.Invoice_service, j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName";
            return Data;
        }
        public string OpenInvoice(string status, long SchlId, string FromDate, string Todate, long UserId)
        {
            var Data = " select ROW_NUMBER() OVER(ORDER BY a.Invoice_Id ASC) AS [Index],a.Invoice_Id," +
                    " a.CustomerFirstName as CustomerFName,a.CustomerLastName as CustomerLName," +
                    " a.CreatedDate,a.CustomerId,Amount = sum(ISNULL(b.TotalPricewithvat,(b.TotalPrice+((b.TotalPrice*ISNULL(b.VAT,0))/100))))," +
                    " d.Currency,c.Invoice_service,c.EmailId as CustomerEmail,e.UpdatedDate as INVOICINGDATE," +
                    " DueDate = dateadd(day, IsNull(d.OverDue, 30), e.UpdatedDate)," +
                    " overdue =case when dateadd(day, IsNull(d.OverDue, 30),e.UpdatedDate)< getdate()" +
                    " then dateadd(day, IsNull(d.OverDue, 30)+1,e.UpdatedDate) else null end," +
                    " a.Reminderdate1,a.Reminderdate2,a.Reminderdate3 ,a.Remaining_Amount ," +
                    " j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName as FlowStatus " +
                    " from SPA_INVOICE_MASTER a" +
                    " JOIN SPA_INVOICE_Detail b ON b.Invoice_Id = a.Invoice_Id" +
                    " JOIN CCTSP_USER c on c.UserId = a.CustomerId" +
                    " JOIN CCTSP_SCHOOLMASTER d on d.schlId = a.schlId and d.ActiveStatus = 'A'" +
                    " JOIN SPA_INVOICE_STATUS e on e.Invoice_Status in ('CIP','CIPS','CIS') and e.ActiveStatus in ('A','T') and e.Invoice_Id=a.Invoice_Id" +
                    " JOIN ctsp_SolutionMaster g ON g.SectionName = 'Open Invoices' and g.Activestatus='A' " +
                    " JOIN cctsp_user h on h.UserId = " + UserId + " " +
                    " JOIN cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + SchlId + " or i.schlid = 236) " +
                    " AND i.ActiveStatus = 'A' " +
                    " JOIN cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                    " AND j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub) " +
                    " AND j.SchId = i.Schlid " +
                    " AND((j.AccessToData = 'Own' and a.EmployeeId = h.UserId) or(j.AccessToData != 'Own')) " +
                    " LEFT JOIN SPA_INVOICE_PAYMENTDETAILS f ON f.Invoice_Id=a.Invoice_Id and f.ActiveStatus = 'A' and f.schlid = a.schlid " +
                    " WHERE a.schlId = " + SchlId + " and a.ActiveStatus = 'A' and b.ActiveStatus = 'A'" +
                    " and a.Invoice_Status in (" + status + ")";
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(Todate))
                Data += " and CONVERT(DATE,e.UpdatedDate) >= '" + DateTime.Parse(FromDate, enGB).ToString("yyyy/MM/dd") + "' and CONVERT(DATE,e.UpdatedDate) <= '" + DateTime.Parse(Todate, enGB).ToString("yyyy/MM/dd") + "'";
            Data += " GROUP BY a.Invoice_Id,a.CustomerFirstName,a.CustomerLastName,c.EmailId,a.CustomerId," +
                    " a.UpdatedDate,d.Currency,c.Invoice_service,a.CreatedDate,d.OverDue,e.UpdatedDate," +
                    " a.Reminderdate1,a.Reminderdate2,a.Reminderdate3,a.Remaining_Amount,  j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName";
            return Data;
        }
        public string ManualInvoiceQuery(string ReservationIdList, long? Invoiceid)
        {
            var Invoice = "Select b.UserId as PatientId,a.EmpSchDetailsId as ReservationId,b.FirstName as Cust_FirstName,b.LastName as Cust_LastName,b.Street as Cust_Street,b.StreetNumber as Cust_StreetNumber,b.Pincode as Cust_Pincode,b.City as Cust_City , " +
                          "b.DOB,b.Title as Cust_Title,b.Gender as Cust_Gender,b.AHV_Number as Cust_AHV_Number,b.VEKA_Number as Cust_VEKA_Number,b.Invoice_Service , " +
                          "b.InsuranceNumber as Cust_InsuranceNumber,b.State as Cust_State,b.PhoneNo as Cust_PhoneNo,b.GLN_No as Cust_GLN_No , " +
                          "b.ZSR_No as Cust_ZSR_No, " +
                          "b.EmailId as CustEmail,b.Country as Cust_Country,b.Address1 as CustAddress,c.ZSR_No as Own_ZSR_No , " +
                          "c.GLN_No as Own_GLN_No,c.FirstName as Own_Fname,c.LastName as Own_Lname , " +
                          "c.Address1 as OwnAddress,c.EmailId as OwnEmail,c.Phoneno as OwnPhoneno, " +
                          "d.SchlName,d.Currency,i.CatgDesc as ProductName,a.BookingDate ,a.Product_price as ProductPrice , " +
                          "e.FirstName as EmpFName,e.UserId as EmpId,e.LastName as EmpLName,e.GLN_No as Emp_GLN_No ,e.ZSR_No as Emp_ZSR_No,e.PhoneNo as EmpPhoneNo,e.EmailId as EmpEmail,e.Address1 as EmpAddress " +
                          " ,i.CatgDesc as ProductName, cast(j.Duration as int) as Duration,Cast(Cast(j.Duration as float)/5 as float ) as Quantity, " +
                          "Isnull(cast(i.Vat as decimal(38,1)),0.0) as Vat," +
                          "Cast((cast(a.Product_price as float) * 5) / cast(j.Duration as int) as float) as Rate, " +
                          "Cast(((Cast(j.Duration as float) / 5) * ((cast(a.Product_price as float) * 5) / cast(j.Duration as int))) as float) as RealAmount " +
                          ",cast(a.empschdetailsId as bigint) as Invoice_Detail_Id,k.Settlement_Number ,case when k.Settlement_text is null then i.Email_Image else k.Settlement_text end as Settlement_text  ,k.Tarif_Number " +
                          ",d.street as Shopstreet, d.StreetNumber as ShopStreetNumber, d.SchPin as ShopZipcode , d.SchlCity as ShopCity " +
                          ",e.Street as Emp_Street, e.StreetNumber as Emp_StreetNumber, e.Pincode as Emp_Pincode , e.City as Emp_City " +
                          "from SPA_EmployeeScheduler a " +
                                    "join cctsp_User b on a.Client_UserId = b.UserId " +
                                    "join cctsp_User c on c.Schid = a.Schid and c.RoleId = 1 " +
                                    "join cctsp_SchoolMaster d on d.Schlid = a.Schid " +
                                    "join cctsp_User e on e.UserId=a.EMP_UserId " +
                                    "join cctsp_CategoryDetails i on i.CatgTypeId=a.Product_Id and i.catgId=111 " +
                                    "join ctsp_SolutionMaster j on j.CatgTypeId = i.catgTypeId " +
                                    "left join Health_Insurance k on k.Insurance_Id=i.Insurance_Id " +
                                    "Where a.EmpSchDetailsId in (" + ReservationIdList + ")";
            return Invoice;
        }
        public string PrintEmailManualInvoice(string InvoiceId)
        {
            var Invoice = "Select b.UserId as PatientId,a.EmpSchDetailsId as ReservationId,b.FirstName as Cust_FirstName, " +
                          "b.LastName as Cust_LastName,b.Street as Cust_Street,b.StreetNumber as Cust_StreetNumber,b.Pincode as Cust_Pincode,b.City as Cust_City ,b.Display_Invoice as Cust_Display_Invoice,b.Invoice_Service , " +
                          "b.DOB,b.Title as Cust_Title,o.Catgtype as Short_Cust_Title,b.Gender as Cust_Gender ,b.AHV_Number as Cust_AHV_Number,b.VEKA_Number as Cust_VEKA_Number, " +
                          "b.InsuranceNumber as Cust_InsuranceNumber,b.Country as Cust_Country,b.State as Cust_State,b.PhoneNo as Cust_PhoneNo,b.GLN_No as Cust_GLN_No ," +
                          "b.ZSR_No as Cust_ZSR_No, b.EmailId as CustEmail,b.Address1 as CustAddress,c.ZSR_No as Own_ZSR_No , " +
                          "c.GLN_No as Own_GLN_No,c.FirstName as Own_Fname,c.LastName as Own_Lname , " +
                          "c.Address1 as OwnAddress,c.EmailId as OwnEmail,c.Phoneno as OwnPhoneno, " +
                          "d.SchlName,d.Currency,d.Lang_id,BookingDate=case when a.BookingDate is null then format(cast(k.Date_Of_Buying as date),'yyyy-MM-dd') else format(cast(a.bookingDate as date),'yyyy-MM-dd') end," +
                          "d.SchlAddress AS ShopAddress,d.SchlCity AS ShopCity,d.StreetNumber as ShopStreetNumber , " +
                          "d.street as Shopstreet,d.SchPin AS ShopZipcode,d.schCountry AS ShopCountry,d.TimeZone,d.SchlState AS ShopState," +
                          "ShopImage = CASE WHEN d.ImageLogo IS NOT NULL AND d.ImageLogo != '' THEN d.ImageLogo ELSE z.Logo END," +
                          "Original_Website=CASE WHEN d.SchlWebsite IS NOT NULL AND d.schlWebsite != '' THEN d.schlWebsite ELSE z.Original_Website END," +
                          "a.Product_price as ProductPrice ,e.FirstName as EmpFName,e.UserId as EmpId,e.LastName as EmpLName,e.GLN_No as Emp_GLN_No ,e.ZSR_No as Emp_ZSR_No, " +
                          "e.PhoneNo as EmpPhoneNo,e.EmailId as EmpEmail,e.Address1 as EmpAddress ,  " +
                          "g.Value,g.Field1 as CatgDetail,f.CatgDesc as CategoryName " +
                          ",h.Invoice_Id as InvoiceNo,h.CreatedDate as InvoiceDate,h.Image_barcode " +
                          ",k.Quantity as Quantity,k.ProductName as ProductName,k.FiveMinuteChargeorPercharge as Rate, " +
                          "ISNull(cast((case when k.Add_On_Pid is null then l.vat else m.vat end) as decimal(38,1)),0.0) as Vat," +
                          "k.TotalPrice as RealAmount,k.Duration as Duration,k.Add_On_Pid as AddOnPId, " +
                          "cast(k.Date_Of_Buying as Date) as EAddOnDate,cast(k.Date_Of_Buying as Time) as EAddOnTime,k.Invoice_Detail_Id ,n.Settlement_Number , " +
                          "case when n.Settlement_text is null and l.Insurance_Id is not null " +
                          "then l.Email_Image " +
                          "when n.Settlement_text is null and m.Insurance_Id is not null  " +
                          "then m.Field3 " +
                          "else n.Settlement_text " +
                          "end as Settlement_text , " +
                          "n.Tarif_Number,h.Reminderdate1 as ReminderDate,k.Manufacturer,k.Dozes,h.field3 as LongNumber, " +
                          "TotalPricewithvat=ISNULL(k.TotalPricewithvat,CONVERT(DECIMAL(38, 2), ((k.TotalPrice * k.VAT) / 100) + k.TotalPrice)), " +
                          "ISNULL(cast(k.vat as decimal(38,1)),0.0) AS Print_Vat, " +
                          "h.field4 as txtAreaGInvoice , e.Gender as Emp_Title " +
                          ",e.Street as Emp_Street, e.StreetNumber as Emp_StreetNumber, e.Pincode as Emp_Pincode , e.City as Emp_City " +
                          ",k.Health_Assign_id as Assign_HealthInsurance_Id,k.Settlement_text as Invoice_Settlement_text, " +
                          "k.Settlement_Number as Invoice_Settlement_Number,k.Tarif_Number as Invoice_Tarif_Number " +
                          "from SPA_INVOICE_MASTER h " +
                          "join cctsp_User b on h.CustomerId = b.UserId " +
                          "join cctsp_User c on c.Schid = h.Schlid and c.RoleId = 1 " +
                          "join cctsp_SchoolMaster d on d.Schlid = h.Schlid " +
                          "join cctsp_User e on e.UserId = h.EmployeeId " +
                          "left join SPA_INVOICE_Detail K on k.Invoice_Id = h.Invoice_Id and k.ActiveStatus='A' " +
                          "left join Spa_EmployeeScheduler a on a.EmpSchDetailsId = K.ReservationId " +
                          "join cctsp_CategoryDetails f on f.Catgid = 161 and f.ActiveStatus = 'A' " +
                          "left join Invoice_CategoryDetails g on g.Schlid = h.Schlid and g.InvoiceId = h.Invoice_Id " +
                          "and f.CatgTypeId = g.CatgTypeId and g.ActiveStatus = 'A' " +
                          "left join cctsp_categoryDetails l on l.CatgtypeId=a.Product_Id " +
                          "left join Add_on_Product_Master m on m.Add_On_Pid = k.Add_On_Pid " +
                          "left join Health_Insurance n on n.Insurance_Id = l.Insurance_Id  or n.Insurance_Id = m.Insurance_Id  " +
                          "JOIN CCTSP_LendingPageMaster z ON z.schId = d.SchlId " +
                          "left join cctsp_CategoryDetails o on o.CatgDesc=b.Title " +
                          "Where h.Invoice_Id in (" + InvoiceId + ")";
            return Invoice;
        }
        public string ClosedInvoice(string status, long schlId, int? month, int? year, string FromDate, string Todate, long UserId)
        {
            var Data = " select ROW_NUMBER() OVER(ORDER BY a.Invoice_Id ASC) AS[Index],a.Invoice_Id," +
                       " a.CustomerFirstName as CustomerFName,a.CustomerLastName as CustomerLName,a.CreatedDate," +
                       " a.CustomerId,Amount = sum(ISNULL(b.TotalPricewithvat,(b.TotalPrice+((b.TotalPrice*ISNULL(b.VAT,0))/100)))),d.Currency,c.Invoice_service,c.EmailId as CustomerEmail," +
                       " e.UpdatedDate as INVOICINGDATE,a.Paid_Date," +
                       " DueDate = dateadd(day, IsNull(d.OverDue, 30), e.UpdatedDate),a.Reminderdate1,a.Reminderdate2,a.Reminderdate3," +
                       " CountReservation = sum(case when b.ReservationId is null then 0 else 1 end) ," +
                       " j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName as FlowStatus " +
                       " from SPA_INVOICE_MASTER a" +
                       " JOIN SPA_INVOICE_Detail b ON b.Invoice_Id = a.Invoice_Id" +
                       " JOIN CCTSP_USER c on c.UserId = a.CustomerId" +
                       " JOIN CCTSP_SCHOOLMASTER d on d.schlId = a.schlId and d.ActiveStatus = 'A'" +
                       " JOIN SPA_INVOICE_STATUS e on e.Invoice_Status in ('CIP','CIPS','CIS','DCMP') and e.ActiveStatus in ('A','T') and e.Invoice_Id=a.Invoice_Id" +
                       " JOIN ctsp_SolutionMaster g ON g.SectionName = 'Closed Invoices' and g.Activestatus='A' " +
                       " JOIN cctsp_user h on h.UserId = " + UserId + " " +
                       " JOIN cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236) " +
                       " AND i.ActiveStatus = 'A' " +
                       " JOIN cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                       " AND j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub) " +
                       " AND j.SchId = i.Schlid " +
                       " AND((j.AccessToData = 'Own' and a.EmployeeId = h.UserId) or(j.AccessToData != 'Own')) " +
                       " WHERE a.schlId = " + schlId + " and a.ActiveStatus = 'A' and b.ActiveStatus = 'A' " +
                       " and a.Invoice_Status in (" + status + ") ";
            if (month > 0)
                Data += " and DATEPART(mm, a.Paid_Date)=" + month;
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(Todate))
                Data += " and CONVERT(DATE,e.UpdatedDate) >= '" + DateTime.Parse(FromDate, enGB).ToString("yyyy/MM/dd") + "' and CONVERT(DATE,e.UpdatedDate) <= '" + DateTime.Parse(Todate, enGB).ToString("yyyy/MM/dd") + "'";
            Data += " and DATEPART(yyyy, a.Paid_Date)=" + year +
                           " GROUP BY a.Invoice_Id,a.CustomerFirstName,a.CustomerLastName,c.EmailId,a.CustomerId," +
                           " a.UpdatedDate,d.Currency,c.Invoice_service,a.CreatedDate,d.OverDue,e.UpdatedDate," +
                           " a.Paid_Date,a.Reminderdate1,a.Reminderdate2,a.Reminderdate3, j.AccessToData ,j.insertAccess,j.DeleteAccess,j.UpdateAccess,g.ItenName";
            return Data;
        }
        public string InvoiceCustomerDetails(string InvoiceIdList)
        {
            return "SELECT b.EmailId,b.FirstName,b.LastName,b.PhoneNo,c.TimeZone,c.Lang_Id,a.Invoice_Id,b.UserId," +
                    "status=CASE WHEN b.Invoice_Service IS NULL OR b.Invoice_Service=2 THEN 'CIPS'" +
                    "WHEN b.Invoice_Service = 1 THEN 'CIS'" +
                    "ELSE 'CIP' END " +
                    "FROM SPA_INVOICE_MASTER a " +
                    "JOIN cctsp_User b ON a.CustomerId = b.UserId " +
                    "JOIN cctsp_SchoolMaster c ON c.Schlid = a.Schlid " +
                    "WHERE a.ActiveStatus = 'A' AND a.Invoice_id IN (" + InvoiceIdList + ")";
        }
        public string AutoupdatestatusofInvoice(string InvoiceId, string date, bool IsCurrent)
        {
            var qry = "";
            qry += "update SPA_INVOICE_STATUS set ActiveStatus = 'T' where Invoice_Id in (" + InvoiceId + ") ";
            if (IsCurrent)
                qry += " UPDATE a SET Invoice_Status = " +
                        " case when b.Invoice_Service is null or b.Invoice_Service = 2 then 'CIPS'" +
                        " when b.Invoice_Service = 1 then 'CIS'" +
                        " else 'CIP' end" +
                        " FROM SPA_INVOICE_MASTER a" +
                        " JOIN CCTSP_USER b on a.CustomerId = b.UserId" +
                        " WHERE a.ActiveStatus = 'A' and a.Invoice_Id in (" + InvoiceId + ")";
            qry += " INSERT INTO SPA_INVOICE_STATUS(Invoice_Id, Invoice_Status, ActiveStatus, CreatedDate, UpdatedDate)" +
                    " SELECT a.Invoice_Id," +
                    " case when b.Invoice_Service is null or b.Invoice_Service = 2 then 'CIPS'" +
                    " when b.Invoice_Service = 1 then 'CIS'" +
                    " else 'CIP' end,'A','" + date + "','" + date + "'" +
                    " FROM SPA_INVOICE_MASTER a" +
                    " JOIN CCTSP_USER b on a.CustomerId = b.UserId" +
                    " WHERE a.ActiveStatus = 'A' and a.Invoice_Id in (" + InvoiceId + ")";
            return qry;
        }
        public string InvoiceReservationList(string ReservationList, long EmpId, long CustomerId, long InvoiceId)
        {

            var Data = "select a.BookingDate,a.Emp_UserId ,a.CLIENT_UserId,a.EmpSchdetailsId as ReservtionId,cast(a.Product_price as float) as Amount, " +
                  "b.CatgDesc as ProductName,cast(c.Duration as int) as Duration " +
                  ",Cast(Cast(c.Duration as int)/5 as float ) as Quantity, " +
                  "Cast((cast(a.Product_price as float) * 5) / cast(c.Duration as int) as float) as Rate,d.Currency " +
                  ",e.Tarif_Number ,e.Settlement_Number,e.Settlement_text,ISNull(cast(b.vat as decimal(38,1)),0.0) as Vat " +
                  "from spa_EmployeeScheduler a " +
                  "join cctsp_CategoryDetails b on b.CatgTypeId = a.Product_Id and b.CatgId = 111 " +
                  "join ctsp_SolutionMaster c on c.CatgtypeId = b.CatgtypeId " +
                  "join cctsp_SchoolMaster d on d.Schlid = a.Schid " +
                  "left join Health_Insurance e on e.Insurance_id=b.Insurance_id " +
                  "where a.CLIENT_UserId = " + CustomerId + " and a.EMP_UserId = " + EmpId + " and " +
                  "a.ActiveStatus = 'C' and a.BookedStatus = 'C' " +
                  "and(Select Count(Invoice_Id) from SPA_INVOICE_Detail Where ReservationId = a.EmpSchdetailsId " +
                  "and ActiveStatus = 'A' and Invoice_id not in (" + InvoiceId + ")) = 0 ";
            /* "and(Select Count(Invoice_Id) from SPA_INVOICE_Detail Where ReservationId = a.EmpSchdetailsId and ActiveStatus = 'A') = 0 ";*/
            if (!string.IsNullOrEmpty(ReservationList))
                Data = Data + "and a.EmpSchdetailsId not in (" + ReservationList + ")";
            return Data;
        }
        public string addDefaultDropDownValue(string InvoiceId, int? LangId)
        {
            return " INSERT INTO Invoice_CategoryDetails " +
                   " (CatgTypeId, InvoiceId, CreatedDate, ActiveStatus, Value, schlId)" +
                   " SELECT b.CatgTypeId, c.Invoice_Id, getdate(), c.ActiveStatus, b.CatgDesc, c.SchlId " +
                   " FROM cctsp_CategoryMaster a" +
                   " JOIN cctsp_CategoryDetails b ON a.CatgId = b.CatgId AND b.Activestatus = 'A'" +
                   " JOIN SPA_INVOICE_MASTER c ON c.Invoice_Id IN (" + InvoiceId + ") AND c.ActiveStatus = 'A'" +
                   " WHERE a.CatgName IN('Invoice_Law','reimbursement','Reason for treatment'," +
                   " 'Invoice_Therapie') AND a.ActiveStatus = 'A' " +
                   " AND b.Lang_Id = " + LangId + " AND b.Group_Orderdata = 1 ORDER BY b.CatgId";
        }
        public string InvoiceServicesList(long EmpId, long Schlid)
        {
            return "select d.CatgTypeId as ProductId,a.SectionDesc as ProductName,a.UserId as EmpId, " +
                   "d.SectionDesc,cast(a.Duration as int) as Duration, " +
                   "CAST(CASE WHEN ISNUMERIC(a.Amount) = 1 THEN a.Amount ELSE NULL END AS decimal(38, 2)) as Amount " +
                   ",e.Settlement_Number ,case when e.Settlement_text is null then b.Email_Image else e.Settlement_text end as Settlement_text " +
                   ",e.Tarif_Number ,Cast(Cast(a.Duration as float) / 5 as float) as Quantity,Isnull(cast(b.Vat as decimal(38, 1)),0.0) as Vat, " +
                   "Cast((cast(a.Amount as float) * 5) / cast(a.Duration as int) as float) as Rate, " +
                   "Cast(((Cast(a.Duration as float) / 5) * ((cast(a.Amount as float) * 5) / cast(a.Duration as int))) as float) as RealAmount  " +
                   "from CTSP_SolutionMaster a " +
                   "join CCTSP_CategoryDetails b on a.sectionDesc = b.CatgDesc " +
                   "join cctsp_user c on a.UserId = c.UserId " +
                   "join CTSP_SolutionMaster d on d.CatgTypeId = b.CatgTypeId " +
                   "left join Health_Insurance e on e.Insurance_Id = b.Insurance_Id " +
                   "where a.activestatus = 'A' and d.activestatus = 'A' " +
                   "and c.activestatus = 'A'and b.activestatus = 'A' and a.SchId = " + Schlid + "  " +
                   "and d.SchId = a.SchId and c.SchId = a.SchId and a.UserId=" + EmpId + " " +
                   "and a.SectionName = 'EmployeeProduct' and b.catgId = 111 and b.catgDesc = a.SectionDesc and d.Duration = a.Duration and d.amount = a.amount  " +
                   "group by d.CatgTypeId,a.SectionDesc,a.UserId, d.SectionDesc,a.Duration,a.Amount,d.orderdata,e.Settlement_Number,e.Settlement_text, " +
                   "e.Tarif_Number ,b.Email_Image,b.Vat " +
                   "order by d.orderdata";
        }
        public string EmployeeAndCustomerList(long Schlid, long UserId)
        {
            return "SELECT ab.UserId,ab.RoleId,ab.FirstName,ab.LastName,C.AccessToData FROM (" +
                    getCustWithOutAccessRit(Schlid) +
                    "Union " +
                    "Select UserId,RoleId,FirstName,LastName,null as Emp_UserId  " +
                    "from cctsp_User " +
                    "Where Schid = " + Schlid + " and RoleId not in (1,4) and ActiveStatus = 'A' " +
                    "group by UserId,RoleId,FirstName,LastName " +
                    ") ab " +
                    "JOIN CTSP_SOLUTIONMASTER A ON A.SectionName = 'For Invoicing' AND A.ActiveStatus = 'A' " +
                    "JOIN CCTSP_USER B ON B.UserId = " + UserId + " AND b.ActiveStatus = 'A' " +
                    "JOIN cctsp_RoleDetails C ON C.RoleId = B.RoleId AND C.ActiveStatus = 'A' AND A.SolutionId = CONVERT(BIGINT, C.AccesstoSub) " +
                    "AND((ab.RoleId = 4 and C.AccessToData = 'Own' and ab.Emp_UserId = B.UserId) " +
                    "or(ab.RoleId = 4 and C.AccessToData != 'Own') " +
                    "or(ab.RoleId != 4 and C.AccessToData = 'Own' and ab.UserId = B.UserId) " +
                    "or(ab.RoleId != 4 and C.AccessToData != 'Own')) " +
                    "Group by ab.UserId,ab.RoleId,ab.FirstName,ab.LastName,C.AccessToData ";
        }
        public string getCustWithOutAccessRit(long schlid)
        {
            return "select a.UserId,a.RoleId,a.FirstName,a.LastName,b.Emp_UserId  " +
                    "from cctsp_user a " +
                    "LEFT JOIN SPA_employeeScheduler b ON b.SchId = " + schlid + " " +
                    "and((b.activeStatus = 'A' and b.BookedStatus = 'B')  " +
                    "or(b.ActiveStatus = 'DA' and b.BookedStatus = 'B') " +
                    "or(b.activeStatus = 'C' and b.BookedStatus = 'C')) and a.UserId = b.Client_UserId " +
                    "where a.ActiveStatus = 'A' and a.RoleId = 4 and " +
                    "(a.Schid = " + schlid + " or a.schId != " + schlid + " and b.EmpSchDetailsId is not null) " +
                    "group by a.UserId,a.RoleId,a.FirstName,a.LastName,b.Emp_UserId ";
        }
        public string GetGeneralInvoice(string ReservationIdList, long Schlid)
        {
            return " SELECT a.SchlName AS ShopName,a.SchlAddress AS ShopAddress,a.SchlCity AS ShopCity,a.StreetNumber,a.street," +
                    " a.SchPin AS ShopZipcode,a.Currency,a.schCountry AS ShopCountry,a.TimeZone,a.SchlState AS ShopState," +
                    " ShopImage = CASE WHEN a.ImageLogo IS NOT NULL AND a.ImageLogo != '' THEN a.ImageLogo ELSE h.Logo END," +
                    " Original_Website=CASE WHEN a.SchlWebsite IS NOT NULL AND a.schlWebsite !='' THEN a.schlWebsite ELSE h.Original_Website END," +
                    " convert(date,b.BookingDate) as BookingDate,CONVERT(DECIMAL(38,2),ISNULL(b.Product_price,'0')) AS TotalPrice,b.product_id," +
                    " Cast((cast(b.Product_price as float) * 5) / cast(f.Duration as int) as float) as Rate, " +
                    " c.FirstName AS EmpFName,c.UserId AS EmpId,c.LastName AS EmpLName,c.Salutation," +
                    " d.UserId AS PatientId,d.DOB,d.Title AS Cust_Title,d.Gender AS Cust_Gender,d.Invoice_Service, " +
                    " d.FirstName AS Cust_FirstName,d.LastName AS Cust_LastName,d.Street AS Cust_Street," +
                    " d.StreetNumber AS Cust_StreetNumber,d.Pincode AS Cust_Pincode,d.City AS Cust_City,d.State AS Cust_State," +
                    " e.CatgDesc as ProductName,CONVERT(INT,ISNULL(f.Duration,'0')) as Duration,ISNULL(e.VAT,0.00) as VAT," +
                    " TotalPricewithvat=CONVERT(DECIMAL(38,2),ISNULL(CONVERT(DECIMAL(38,2),ISNULL(b.Product_price,'0'))*e.VAT/100,0)+CONVERT(DECIMAL(38,2),ISNULL(b.Product_price,'0')))," +
                    " CAST(CAST(f.Duration AS FLOAT)/5 AS FLOAT ) AS Quantity,b.EmpSchDetailsId as ReservationId ,CAST(b.empschdetailsId as bigint) as Invoice_Detail_Id , " +
                    " ISNULL(a.OverDue,30) as DueDays,c.Gender as Emp_Title,a.IBANNO as ShopIban_Number " +
                    " FROM cctsp_SchoolMaster a" +
                    " JOIN SPA_EMPLOYEESCHEDULER b ON b.ActiveStatus = 'C' and b.BookedStatus = 'C'" +
                    " JOIN cctsp_User c ON c.UserId = b.EMP_UserId" +
                    " JOIN cctsp_User d ON d.UserId = b.Client_UserId" +
                    " JOIN cctsp_CategoryDetails e ON e.CatgTypeId = b.Product_Id AND e.catgId = 111" +
                    " JOIN ctsp_SolutionMaster f ON f.CatgTypeId = e.catgTypeId" +
                    " JOIN CCTSP_LendingPageMaster h ON h.schId = a.SchlId" +
                    " WHERE a.SchlId = " + Schlid + " AND b.EmpSchDetailsId IN (" + ReservationIdList + ")";
        }
        public string GetGeneralInvoice(long InvoiceId, long Schlid)
        {
            return " SELECT a.SchlName AS ShopName,a.SchlAddress AS ShopAddress,a.SchlCity AS ShopCity," +
                    " a.SchPin AS ShopZipcode,a.Currency,a.schCountry AS ShopCountry,a.TimeZone," +
                    " a.SchlState AS ShopState,a.StreetNumber,a.street," +
                    " ShopImage = CASE WHEN a.ImageLogo IS NOT NULL AND a.ImageLogo != '' THEN a.ImageLogo ELSE b.Logo END," +
                    " Original_Website=CASE WHEN a.SchlWebsite IS NOT NULL AND a.schlWebsite !='' THEN a.schlWebsite ELSE b.Original_Website END," +
                    " c.Date_Of_Buying AS BookingDate,CONVERT(DECIMAL(38,2),c.TotalPrice) as TotalPrice,c.Duration,c.ProductName," +
                    " ISNULL(c.Product_Id, c.Add_On_Pid) as product_id," +
                    " e.UserId AS PatientId,e.DOB,e.Title AS Cust_Title,e.Gender AS Cust_Gender," +
                    " e.FirstName AS Cust_FirstName,e.LastName AS Cust_LastName,e.Street AS Cust_Street,e.Invoice_Service , " +
                    " e.StreetNumber AS Cust_StreetNumber,e.Pincode AS Cust_Pincode,e.City AS Cust_City," +
                    " e.State AS Cust_State,f.FirstName AS EmpFName,f.UserId AS EmpId,f.LastName AS EmpLName,f.Salutation, ISNULL(c.VAT,0.00) AS VAT, " +
                    " TotalPricewithvat=CONVERT(DECIMAL(38,2),ISNULL(CONVERT(DECIMAL(38,2),ISNULL(c.TotalPrice,'0'))*c.VAT/100,0)+CONVERT(DECIMAL(38,2),ISNULL(c.TotalPrice,'0'))), " +
                    " c.FiveMinuteChargeorPercharge as Rate," +
                    " c.Quantity,c.ReservationId,c.Add_On_Pid, d.Field4 as txtAreaGInvoice ,d.CreatedDate as InvoiceDate ,  " +
                    " ISNULL(a.OverDue,30) AS DueDays,f.Gender AS Emp_Title,a.IBANNO AS ShopIban_Number ,c.Invoice_Detail_Id " +
                    " FROM cctsp_SchoolMaster a" +
                    " JOIN CCTSP_LendingPageMaster b ON a.schlId = b.SchId" +
                    " JOIN SPA_INVOICE_MASTER d ON d.SchlId = a.schlId  AND d.ActiveStatus = 'A' " +
                    " LEFT JOIN SPA_INVOICE_Detail c ON c.Invoice_Id =d.Invoice_Id AND c.SchlId = a.schlId AND c.ActiveStatus = 'A' " +
                    " JOIN CCTSP_USER e ON e.UserId = d.CustomerId" +
                    " JOIN CCTSP_USER f ON f.UserId = d.EmployeeId" +
                    " WHERE a.SchlId = " + Schlid + " AND d.Invoice_Id =" + InvoiceId;
        }
        #endregion
        #region Note
        public string SectionList(int LangId, long SchlOrder)
        {
            return "SELECT a.CatgType ,c.Value as Label_Name,c.English_Label ,a.Catgdesc as LangCatgId ,b.CatgId  " +
                   "FROM cctsp_CategoryDetails a " +
                   "JOIN cctsp_categoryMaster b ON b.CatgId = Convert(BigInt, a.CatgDesc) " +
                   "JOIN Language_Label_Detail c ON c.Label_Name = a.CatgDesc " +
                   "WHERE a.CatgId = 179 and a.Group_orderdata = " + SchlOrder + " and c.Lang_id =" + LangId + "  " +
                   "and a.ActiveStatus = 'A' and c.ActiveStatus = 'A'";
        }
        public string getAllNotesData(long ReservationId, long schlId)
        {
            return " SELECT a.EmpSchDetailsId ,a.ActiveStatus,a.comment,a.CLIENT_UserId as ClientUserId,b.UserId,b.PhoneNo,b.FirstName,b.LastName,b.Title,b.DOB,b.EmailId,b.Gender," +
                    " c.FirstName as EmpFirstName,C.LastName as EmpLastName ,d.SchlAddress as Address,d.TimeZone, " +
                    " d.ImageLogo as ShopImg ,CASE WHEN  datediff(year, b.DOB, getdate()) < 180" +
                    " THEN datediff(year, b.DOB, getdate()) ELSE 0 END as Age ,b.DOB ," +
                    " f.CatgTypeId,f.CatgId,CatgDesc = case when f.CatgTypeId is null then f.Remarks else g.CatgDesc end," +
                    " SectionCatgId = convert(bigint,h.CatgDesc), " +
                    " SectionType = h.CatgType," +
                    " FORMAT(convert(date,a.BookingDate),'dd.MM.yyyy') as BookingDate,h.value as flg,convert(date,a.BookingDate) as Bookings" +
                    " from SPA_EmployeeScheduler a" +
                    " JOIN cctsp_user b on b.UserId = a.Client_UserId" +
                    " JOIN cctsp_user c on c.UserId = a.Emp_UserId" +
                    " JOIN cctsp_schoolmaster d on  d.schlid = a.schid" +
                    " LEFT JOIN Prescription_Master e on e.Diff = d.schlstudentstrength and e.ActiveStatus = 'A' and e.SchId = a.schId and e.BookingId = a.empschdetailsId" +
                    " LEFT JOIN CCTSP_categoryDetails h on h.CatgId = 179 and h.Group_orderdata = d.schlstudentstrength and h.ActiveStatus = 'A'" +
                    " LEFT JOIN Prescription_Detail f on f.Prescription_Id = e.Prescription_Id and f.ActiveStatus = 'A' and f.CatgId=convert(bigint,h.CatgDesc)" +
                    " LEFT JOIN CCTSP_categoryDetails g on g.CatgTypeId = f.CatgTypeId" +
                    " WHERE a.schid =" + schlId +
                    " and c.schid = a.schid and a.EmpSchDetailsId = " + ReservationId +
                    " and d.ActiveStatus = 'A'";
        }
        public string getAllHistoryNotes(long? ReservationId, long? schlId, long? UserId, int? Flow)
        {
            return " SELECT FORMAT(CONVERT(date,a.BookingDate),'dd.MM.yyyy') as BookingDate,a.EmpSchDetailsId," +
                    " a.schId,b.Prescription_Id,CONVERT(date,a.BookingDate) as Bookings" +
                    " FROM SPA_EmployeeScheduler a" +
                    " JOIN Prescription_Detail b on b.BookingId = a.EmpSchDetailsId" +
                    " JOIN prescription_Master c on c.Prescription_Id=b.prescription_Id and c.diff=" + Flow +
                    " WHERE a.Client_UserId =" + UserId +
                    " and a.EmpSchDetailsId != " + ReservationId + " and a.Schid =" + schlId +
                    " GROUP BY a.BookingDate,a.EmpSchDetailsId,a.schId,b.Prescription_Id";
        }
        public string GetAllNoteHistory(string BookingIdList, long UserId, int diff)
        {
            var IdList = "";
            if (!string.IsNullOrEmpty(BookingIdList))
                IdList = "and b.BookingId in (" + BookingIdList + ")";
            return " select b.Catgid,b.Remarks as CatgDesc ,FORMAT(CONVERT(date,a.BookingDate),'dd.MM.yyyy') as BookingDate , " +
                   " CAST(c.catgDesc AS bigint) as SectionCatgId ,a.EmpSchDetailsId  as ReservationId,CONVERT(date,a.BookingDate) as Bookings " +
                   " from SPA_EmployeeScheduler a " +
                   " join Prescription_Detail b on a.EmpSchDetailsId = b.BookingId " +
                   " left join cctsp_CategoryDetails c on c.Group_orderdata = b.Type and c.CatgId = 179 and c.ActiveStatus = 'A' " +
                   " where b.ActiveStatus = 'A' and b.Type = " + diff + "" +
                   " " + IdList + " and b.UserId = " + UserId + " " +
                   " group by b.Catgid,b.Remarks,a.BookingDate,c.catgDesc,a.EmpSchDetailsId ";
        }
        #endregion
        #region Reservation
        public string openListView(long LoggedInUser, long schlId)
        {
            return " SELECT a.CLIENT_UserId,a.BookedStatus,a.ActiveStatus,a.comment,a.reg_status," +
                    " e.firstName as EmpfirstName,e.lastName as EmpLastName, a.EmpSchDetailsId,b.CatgDesc as CatgDesc," +
                    " cast(c.Duration as int) as Duration,  a.BookingDate," +
                    " cast(a.FromSlotMasterId as time) as FromSlotMasterId," +
                    " cast(a.ToSlotMasterId as time) as ToSlotMasterId," +
                    " cast((datediff(minute, cast(a.FromSlotMasterId as time)," +
                    " cast(a.ToSlotMasterId as time))) as int) as diff," +
                        " CAST(CASE WHEN ISNUMERIC(a.Product_price) = 1" +
                        " THEN a.Product_price ELSE NULL END AS decimal(38, 2)) as Amount," +
                    " a.CreatedOn as CreatedOn ,d.firstname as CustomerFirstName,d.lastName as CustomerLastName," +
                    " d.phoneNo as CustomerPhone,  g.AccessToData ,g.insertAccess,g.DeleteAccess,g.UpdateAccess," +
                    " f.ItenName as FlowStatus," +
                    " stMerge=ISNULL((select top 1 case when y.prescription_Id is not null or " +
                        " z.prescription_Id is not null then 1 else 0 end" +
                        " from Prescription_Master x" +
                        " LEFT JOIN Prescription_Detail y on y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A'" +
                        " and y.Prescription_Id = x.Prescription_Id" +
                        " LEFT JOIN Medicine_Master z on z.BookingId = a.EmpSchDetailsId and z.ActiveStatus = 'A'" +
                        " and z.Prescription_Id = x.Prescription_Id" +
                        " where x.BookingId = a.EmpSchDetailsId and x.ActiveStatus = 'A' and x.diff = j.SchlStudentStrength),0) " +
                    " from spa_employeeScheduler a" +
                    " JOIN cctsp_categoryDetails b on a.product_id = b.catgtypeId" +
                    " JOIN ctsp_solutionMaster c on b.catgtypeId = c.catgtypeId" +
                    " JOIN cctsp_user d on a.client_userid = d.UserId" +
                    " JOIN cctsp_user e on e.UserId = a.Emp_UserId" +
                    " JOIN ctsp_SolutionMaster f on f.SectionName = 'Open List View' and f.ActiveStatus = 'A'" +
                    " JOIN cctsp_user h on h.UserId =" + LoggedInUser +
                    " JOIN cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236) and i.ActiveStatus = 'A'" +
                    " JOIN cctsp_RoleDetails g on g.RoleId = i.RoleId and g.ActiveStatus = 'A'" +
                    " and f.SolutionId = convert(bigint, g.AccesstoSub)  and g.SchId = i.Schlid" +
                    " and f.SolutionId = convert(bigint, g.AccesstoSub)" +
                    " and((g.AccessToData = 'Own' and a.Emp_UserId = h.UserId) or(g.AccessToData != 'Own'))" +
                    " join cctsp_SchoolMaster j on j.SchlId = a.SchId and j.ActiveStatus = 'A'" +
                    " where(a.BookedStatus = 'B' or a.BookedStatus = 'Z') and(a.ActiveStatus = 'A' or a.ActiveStatus = 'Z')" +
                    " and a.SchId = " + schlId + " and d.RoleId = 4 and e.RoleId not in(1,4) " +
                    " GROUP BY a.Client_UserId,a.BookedStatus,a.ActiveStatus,a.Comment,a.reg_Status,e.firstName," +
                    " e.lastName,a.EmpSchDetailsId,b.CatgDesc,c.Duration,a.BookingDate,a.FromSlotMasterId," +
                    " a.ToSlotMasterId,a.Product_price,a.CreatedOn,d.firstname,d.lastName,d.phoneNo,g.AccessToData," +
                    " g.insertAccess,g.DeleteAccess,g.UpdateAccess,f.ItenName" +
                    " order by cast(a.BookingDate as date), cast(a.FromSlotMasterId as time)";
        }
        public string BlockDay(long? UserId, long? LoginUserId, long schlId, string WeekDay)
        {
            return
                "SELECT " +
                "a.UserId,a.weekDay,a.StartTime AS MinTime,a.EndTime AS MaxTime, b.SlotDue, j.AccessToData,g.ItenName AS FlowStatus,j.insertAccess, " +
                "j.DeleteAccess,j.UpdateAccess " +
                "FROM SPA_EmployeeSchedulers a " +
                "JOIN (SELECT TOP 1z.StartTime AS MinTime, z.SchedulerId, z.SlotDue, z.UserId " +
                "FROM SPA_SchedulerSlotMASter z " +
                "JOIN cctsp_schedulerMASter c ON c.WeekDay = '" + WeekDay + "' AND c.ActiveStatus = 'A' " +
                "WHERE z.UserId = " + UserId + " AND z.ActiveStatus = 'A' AND z.SlotDue is not null) b ON b.UserId = a.UserId " +
                "JOIN cctsp_user d ON d.UserId = a.UserId and d.schId=a.SchId " +
                "JOIN ctsp_SolutiONMASter g ON g.SectiONName = 'Calendar MONth' AND g.Activestatus = 'A' " +
                "JOIN cctsp_user h ON h.UserId = " + LoginUserId + " " +
                "JOIN cctsp_Role i ON i.Roleid = h.Roleid AND(i.schlid = a.SchId or i.schlid = 236)  AND i.ActiveStatus = 'A' " +
                "JOIN cctsp_RoleDetails j ON j.RoleId = i.RoleId AND j.ActiveStatus = 'A' AND g.SolutiONId = cONvert(bigint, j.AccesstoSub) " +
                "AND j.SchId = i.Schlid AND((j.AccessToData = 'Own' AND a.UserId = h.UserId) or(j.AccessToData != 'Own')) " +
                "WHERE a.SchId = " + schlId + " AND a.WeekDay = '" + WeekDay + "' AND a.UserId = " + UserId + " " +
                "AND d.activeStatus = 'A'  AND a.activestatus = 'A' " +
                "ORDER BY a.StartTime,a.EndTime ";
        }
        public string BlockDay(long? UserId, long schlId, long? LoginUserId)
        {
            return "select a.UserId,a.weekDay,min(a.StartTime) as MinTime,max(a.EndTime) as MaxTime,b.SlotDue, " +
                    "j.AccessToData,g.ItenName as FlowStatus,j.insertAccess,j.DeleteAccess,j.UpdateAccess " +
                    "from SPA_EmployeeSchedulers a " +
                    "join(select top 1 min(z.StartTime) as MinTime, z.SchedulerId, z.SlotDue, z.UserId from SPA_SchedulerSlotMaster z " +
                    "where z.UserId = " + UserId + " and z.ActiveStatus = 'A' and z.SlotDue is not null group by StartTime, SchedulerId, " +
                    "SlotDue, UserId) b on b.UserId = a.UserId " +
                    "join cctsp_schedulerMaster c on c.SchedulerId=b.SchedulerId " +
                    "join cctsp_user d on d.UserId=a.UserId " +
                    "join ctsp_SolutionMaster g on g.SectionName = 'Calendar Month' and g.Activestatus='A' " +
                    "join cctsp_user h on h.UserId = " + LoginUserId + " " +
                    "join cctsp_Role i on i.Roleid = h.Roleid and(i.schlid = " + schlId + " or i.schlid = 236)  " +
                    "and i.ActiveStatus = 'A' " +
                    "join cctsp_RoleDetails j on j.RoleId = i.RoleId " +
                    "and j.ActiveStatus = 'A' and g.SolutionId = convert(bigint, j.AccesstoSub)  " +
                    "and j.SchId = i.Schlid " +
                    "and((j.AccessToData = 'Own' and a.UserId = h.UserId) or(j.AccessToData != 'Own')) " +
                    "where a.SchId=" + schlId + " and d.SchId=" + schlId + " and a.WeekDay=c.weekday " +
                    "and a.UserId=" + UserId + " and d.activeStatus='A' and a.activestatus='A' " +
                    "and c.activeStatus = 'A' group by a.WeekDay,a.UserId,b.slotDue,j.AccessToData,g.ItenName ,j.insertAccess,j.DeleteAccess,j.UpdateAccess";
        }
        public string GetTimeForEmployee(long? UserId, string startDateTime, string startDate, string CurrentDate, bool? AllowPast, long? ReservationId)
        {
            if (UserId.HasValue)
            {
                return "SELECT " +
                    "DISTINCT(FORMAT(CONVERT(DATETIME, b.StartTime), 'HH:mm')) AS T1, " +
                    "b.StartTime AS T2,d.Scheduler_Time_Id,b.SlotDue, " +
                    "CASE WHEN b.StartTime >= (FORMAT(CONVERT(DATETIME, '" + startDateTime + "'), 'HH:mm')) THEN 1 ELSE 0 END AS Status " +
                    "FROM " +
                    "SPA_SchedulerSlotMaster b " +
                    "JOIN cctsp_schedulerMaster c ON c.WeekDay = DATENAME(dw, CONVERT(DATE, '" + startDate + "')) " +
                    "AND c.SchId = b.SchId  AND c.ActiveStatus = 'A' AND c.SchedulerId = b.SchedulerId " +
                    "JOIN spa_employeeSchedulers d ON d.WeekDay = DATENAME(dw, CONVERT(Date, '" + startDate + "')) " +
                    "AND d.ActiveStatus = 'A' AND d.UserId = " + UserId + " " +
                    "AND b.Starttime BETWEEN d.starttime AND d.endtime " +
                    "WHERE b.UserId = " + UserId + " AND b.ActiveStatus = 'A' " +
                    (AllowPast != true ? "AND CAST(CONCAT('" + DateTime.Parse(startDate).ToString("yyyy-MM-dd") + "', ' ', CAST(b.StartTime AS NVARCHAR(8))) AS DATETIME) > CONVERT(DATETIME, '" + CurrentDate + "') " : "") +
                    "AND b.StartTime NOT IN " +
                                    "( " +
                                        "SELECT b.Starttime " +
                                        "FROM spa_employeeScheduler a " +
                                        "WHERE CONVERT(DATE, a.BookingDate) = CONVERT(DATE, '" + startDate + "') " +
                                        GetAllConditionForReservation() +
                                        ((ReservationId > 0 || ReservationId != null) ? "and a.EmpSchDetailsId!=" + ReservationId + " " : "") +
                                        "AND b.StartTime >= CONVERT(TIME, a.FromSlotMasterId " +
                                    ") " +
                    "AND b.StartTime < CONVERT(TIME, a.ToSlotMasterId) AND a.Emp_UserId = " + UserId + " ) " +
                    "ORDER BY b.StartTime";
            }
            return "";

        }
        public string GetAllConditionForReservation()
        {
            return "AND ( " +
                             "   (a.ActiveStatus = 'A' AND a.BookedStatus = 'B') " +
                             "OR (a.ActiveStatus = 'C' AND a.BookedStatus = 'C') " +
                             "OR (a.ActiveStatus = 'DA' AND a.BookedStatus = 'B') " +
                         ") ";
        }
        //public string BlockAllEmployeeDayDetail()
        //{
        //    return 
        //}
        #endregion
        #region CustomerHistory
        public string CustomerHistory(long schlId, int UserId)
        {
            return " SELECT c.currency,ISNULL(c.SchlStudentStrength, 0) as SchlStudentStrength ,a.empschdetailsid," +
                    " a.bookingdate,cast(a.fromslotmasterid as time) as fromslotmasterid,b.firstname as empfirstname," +
                    " a.reg_status,d.catgdesc," +
                    " cast(case when isnumeric(e.amount) = 1 then e.amount else null end as decimal(38, 2)) as amount," +
                    " cast(e.duration as int) as duration,e.sectiondesc as comment," +
                    " stMerge =ISNULL((select top 1 case when y.prescription_Id is not null or " +
                        " z.prescription_Id is not null then 1 else 0 end" +
                        " FROM Prescription_Master x" +
                        " LEFT JOIN Prescription_Detail y on y.BookingId = a.EmpSchDetailsId and y.ActiveStatus = 'A'" +
                        " and y.Prescription_Id = x.Prescription_Id" +
                        " LEFT JOIN Medicine_Master z on z.BookingId = a.EmpSchDetailsId and z.ActiveStatus = 'A'" +
                        " and z.Prescription_Id = x.Prescription_Id" +
                        " WHERE x.BookingId = a.EmpSchDetailsId and x.ActiveStatus = 'A'" +
                        " and x.diff = c.SchlStudentStrength),0)" +
                    " FROM spa_employeescheduler a" +
                    " JOIN cctsp_user b on b.userid = a.emp_userid" +
                    " JOIN cctsp_schoolmaster c on c.schlid = a.schid" +
                    " JOIN cctsp_categorydetails d on d.catgtypeid = a.product_id" +
                    " JOIN ctsp_solutionmaster e on e.catgtypeid = d.catgtypeid" +
                    " WHERE a.schid = " + schlId + " and a.client_userid = " + UserId +
                    " and a.activestatus = 'c' and a.bookedstatus = 'c'" +
                    " and c.activestatus = 'A'";
        }
        #endregion
        #region InvoiceReport
        //public string InvoiceReport(int? Month, int? Year, long Schlid)
        //{
        //    var Report = "SELECT " +
        //                 "a.SchlName AS ShopName,a.SchlWebsite AS ShopWebsite,a.SchlAddress AS ShopAddress,a.SchlCity As ShopCity, " +
        //                 "a.SchlState AS ShopState,a.SchPin AS ShopZipcode,a.Currency,a.schCountry AS ShopCountry,a.TimeZone,  " +
        //                 "Convert(Date, c.BookingDate) AS BookingDate, c.Product_Id AS ProductId,Convert(float,c.Product_price ) AS Amount, " +
        //                 "b.Logo AS ShopImage,d.CatgDesc AS ServiceName,e.FirstName AS Owner_FName,e.LastName AS Owner_LName,b.Original_Website as ShopUrl,  " +
        //                 "e.Gender AS Title, datename(Month, convert(date, c.BookingDate)) as Month_Name, c.EmpSchDetailsId AS ReservationId ,Addon = 0 " +
        //                 "FROM " +
        //                 "cctsp_schoolMaster a " +
        //                 "JOIN CCTSP_LendingPageMaster b ON a.schlId = b.SchId " +
        //                 "LEFT JOIN SPA_EmployeeScheduler c ON c.SchId = a.SchlId AND c.ActiveStatus = 'C' and c.BookedStatus = 'C' ";
        //    if (Month != null)
        //        Report += "AND datepart(Month, convert(date, c.BookingDate))= '" + Month + "' ";
        //    Report += "AND datepart(YEAR, convert(date, c.BookingDate))= '" + Year + "' " +
        //                 "LEFT JOIN CCTSP_CategoryDetails d ON d.CatgTypeId = c.Product_Id " +
        //                 "JOIN CCTSP_USER e ON e.SchId = a.SchlId AND e.RoleId = 1 " +
        //                 "WHERE a.SchlId = " + Schlid + " AND a.ActiveStatus = 'A' AND e.ActiveStatus = 'A' " +
        //                 "Union " +
        //                 "SELECT a.SchlName AS ShopName,a.SchlWebsite AS ShopWebsite,a.SchlAddress AS ShopAddress, " +
        //                 " a.SchlCity As ShopCity, a.SchlState AS ShopState,a.SchPin AS ShopZipcode, " +
        //                 " a.Currency,a.schCountry AS ShopCountry,a.TimeZone,  " +
        //                 "f.Date_Of_Buying AS BookingDate, f.Add_On_Pid AS ProductId, " +
        //                 "f.TotalPrice AS Amount, b.Logo AS ShopImage," +
        //                 "f.ProductName AS ServiceName,e.FirstName AS Owner_FName," +
        //                 "e.LastName AS Owner_LName,b.Original_Website as ShopUrl, e.Gender AS Title, " +
        //                 " datename(Month, f.Date_Of_Buying) as Month_Name, " +
        //                 "f.Invoice_Detail_Id AS ReservationId ,Addon = 1 " +
        //                 "FROM cctsp_schoolMaster a " +
        //                 "JOIN CCTSP_LendingPageMaster b ON a.schlId = b.SchId " +
        //                "LEFT JOIN SPA_INVOICE_Detail f on f.Schlid = a.Schlid and f.ActiveStatus = 'A' and f.Add_On_Pid is not null ";
        //    if (Month != null)
        //        Report += "AND datepart(Month, f.Date_Of_Buying)= '" + Month + "' ";
        //    Report +=   "AND datepart(YEAR, f.Date_Of_Buying)= '" + Year + "' " +
        //                "JOIN CCTSP_USER e ON e.SchId = a.SchlId AND e.RoleId = 1 " +
        //                "WHERE a.SchlId = " + Schlid + " AND a.ActiveStatus = 'A' " +
        //                "AND e.ActiveStatus = 'A' ";
        //    return Report;
        //}
        public string InvoiceReport(string Month, int? Year, long Schlid, long UserId, int? Filter)
        {
            string FilterQuery = " c.Date_Of_Buying ";
            if (Filter == 2)
                FilterQuery = " d.Paid_Date ";
            string Months = Month != null ? " AND DATENAME(MONTH," + FilterQuery + ")='" + Month + " '" : " ";
            if (Year != null && Schlid != 0)
            {
                return "DECLARE @dtDate DATE=convert(Date,DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE()),0))) " +
                       "SELECT a.SchlName AS ShopName,a.SchlAddress AS ShopAddress,a.SchlCity AS ShopCity, " +
                       "a.SchPin AS ShopZipcode,a.Currency,a.schCountry AS ShopCountry,a.TimeZone,a.SchlState AS ShopState, " +
                       "CASE WHEN a.SchlWebsite IS NOT NULL AND a.SchlWebsite != '' THEN a.SchlWebsite ELSE Original_Website END " +
                       "AS ShopWebsite, ShopImage = CASE WHEN a.ImageLogo IS NOT NULL AND a.ImageLogo != '' THEN a.ImageLogo ELSE b.Logo END, " +
                       "c.Date_Of_Buying AS BookingDate, CASE WHEN c.Product_Id IS NOT NULL THEN c.Product_Id " +
                       "ELSE c.Add_On_Pid END AS ProductId, CONVERT(DECIMAL(38, 2), c.TotalPrice) AS Amount, " +
                       "qty = c.Quantity, c.ProductName AS ServiceName,e.FirstName AS Owner_FName,e.LastName AS Owner_LName, " +
                       "e.Gender AS Title,DATENAME(MONTH, c.Date_Of_Buying) as Month_Name, " +
                       "Addon = CASE WHEN c.Add_On_Pid IS NOT NULL THEN 1 WHEN C.ReservationId IS NOT NULL THEN 0 WHEN c.Health_Assign_id IS NOT NULL " +
                       "THEN 3 ELSE 2 END, " +
                       "VAT = CONVERT(DECIMAL(38, 2), " +
                       "CASE WHEN c.TotalPricewithvat is not null THEN " +
                       "(c.TotalPricewithvat - c.TotalPrice) ELSE 0 END),  " +
                       "TotalWithVAT = CONVERT(DECIMAL(38, 2), " +
                       "CASE WHEN c.TotalPricewithvat is not null THEN c.TotalPricewithvat ELSE c.TotalPrice END), " +
                       "c.Invoice_Detail_id " +
                       ",c.Settlement_text as Invoice_SettlementText,c.Settlement_Number as Invoice_SettleMentNumber,c.Tarif_Number as Invoice_TarifNumber " +
                       "FROM " +
                       "cctsp_SchoolMaster a " +
                       "JOIN CCTSP_LendingPageMaster b ON a.schlId = b.SchId " +
                       "LEFT JOIN SPA_INVOICE_MASTER d ON d.SchlId = a.schlId AND d.ActiveStatus = 'A' " +
                       "AND d.Invoice_Status in ('CMPEP','CMPE','CMPP','DCMP','CMP')  " +
                       "LEFT JOIN SPA_INVOICE_Detail c ON c.Invoice_Id = d.Invoice_Id AND c.ActiveStatus = 'A' " +
                       "AND " + FilterQuery + " IS NOT NULL AND CONVERT(INT, DATENAME(YEAR, " + FilterQuery + "))= " + Year + Months +
                       //"AND c.Date_Of_Buying <= (CASE WHEN CONVERT(DATE, a.CreatedOn) <= @dtDate THEN @dtDate " +
                       //"ELSE CONVERT(DATE, a.CreatedOn) END) " +
                       "JOIN CCTSP_USER e ON e.ActiveStatus = 'A' AND e.RoleId = 1 AND e.schId = a.schlId " +
                       "WHERE " +
                       "a.SchlId = " + Schlid + " AND a.ActiveStatus = 'A' " +
                       "GROUP BY a.SchlName,a.SchlAddress,a.SchlCity, " +
                       "a.SchPin,a.Currency,a.schCountry,a.TimeZone,a.SchlState,a.SchlWebsite,  " +
                       "a.ImageLogo,b.Logo,c.Product_Id, " +
                       "c.Quantity,c.ProductName,e.FirstName,e.LastName, " +
                       "e.Gender,c.Date_Of_Buying, " +
                       "c.Add_On_Pid,C.ReservationId,c.TotalPricewithvat,c.TotalPrice, " +
                       "c.Invoice_Detail_id,b.Original_Website ,c.Settlement_text,c.Settlement_Number,c.Tarif_Number,c.Health_Assign_id ";
            }
            return null;
        }
        public string AbstractReport(int Schlid, long UserId, int? Status)
        {
            //string FQuery = " a.Date_Of_Buying ";
            //if (Status == 2)
            //    FQuery = " b.Paid_Date ";
            //return "DECLARE @dtDate DATE=convert(Date,DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE()),0))) " +
            //       "SELECT " +
            //       "CONVERT(DATE, CONCAT(DATENAME(YEAR, " + FQuery + "), '/', DATEPART(MONTH,  " + FQuery + "), '/01')) AS dates, DATENAME(MONTH,  " + FQuery + ") AS Month " +
            //       " , CONVERT(INT, DATENAME(YEAR,  " + FQuery + ")) AS Year, " +
            //       "SUM(CONVERT(DECIMAL(10, 2), CASE WHEN TotalPricewithvat IS NOT NULL THEN TotalPricewithvat ELSE TotalPrice END)) AS Price, d.Currency_ShortName AS Culture, " +
            //       "MINDATE= CASE WHEN CONVERT(DATE, MIN( " + FQuery + ")) <= CONVERT(DATE, c.CreatedOn)  " +
            //       "THEN CONVERT(DATE, MIN( " + FQuery + ")) ELSE CONVERT(DATE, c.CreatedOn) END, " +
            //       "MAXDATE = @dtDate , c.currency " +
            //       "FROM SPA_INVOICE_Detail a " +
            //       "JOIN SPA_INVOICE_MASTER b ON a.Invoice_Id = b.Invoice_Id " +
            //       "JOIN cctsp_schoolMaster c ON c.schlId = a.SchlId " +
            //       "JOIN Language_Master d ON d.Lang_id = c.Lang_Id " +
            //       "WHERE a.schlId = " + Schlid + " " +
            //       "AND a.ActiveStatus = 'A' AND b.ActiveStatus = 'A'  AND  " + FQuery + " IS NOT NULL " +
            //       "AND b.Invoice_Status IN('CMPEP', 'CMPE', 'CMPP', 'DCMP','CMP') " +
            //       " AND  " + FQuery + " <= @dtDate " +
            //       "GROUP BY " +
            //       "DATEPART(MONTH,  " + FQuery + "),DATENAME(YEAR,  " + FQuery + "),  " +
            //       "DATENAME(MONTH,  " + FQuery + "),d.Currency_ShortName,c.CreatedOn,c.currency " +
            //        "ORDER BY " +
            //        "DATENAME(YEAR,  " + FQuery + "),DATEPART(MONTH,  " + FQuery + ") ";

            string FQuery = " a.Date_Of_Buying ";
            if (Status == 2)
                FQuery = " b.Paid_Date ";
            return "DECLARE @dtDate DATE=convert(Date,DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,GETDATE()),0))) " +
                   "SELECT CASE WHEN DATEPART(MONTH, " + FQuery + ") IS NOT NULL " +
                   "THEN "+
                   "CONVERT(DATE, CONCAT(DATENAME(YEAR, "+FQuery+ "), '/', DATEPART(MONTH, " + FQuery + "), '/01')) " +
                   "ELSE "+
                   "NULL END "+
                   "AS dates, "+
                   "DATENAME(MONTH, " + FQuery + ") AS Month, " +
                   "CONVERT(INT, DATENAME(YEAR, " + FQuery + ")) AS Year, " +
                   "SUM(CONVERT(DECIMAL(10, 2), "+
                   "CASE WHEN TotalPricewithvat IS NOT NULL THEN TotalPricewithvat ELSE TotalPrice END)) AS Price, "+
                   "d.Currency_ShortName AS Culture, "+
                   "MINDATE = CASE WHEN "+
                   "CONVERT(DATE, MIN(" + FQuery + ")) <= CONVERT(DATE, c.CreatedOn) " +
                   "THEN CONVERT(DATE, MIN(" + FQuery + ")) " +
                   "ELSE CONVERT(DATE, c.CreatedOn) END, "+
                   "MAXDATE = CASE "+
                   "WHEN CONVERT(DATE, MAX(" + FQuery + ")) is NULL and CONVERT(DATE, c.CreatedOn) <= @dtDate " +
                   "THEN @dtDate "+
                   "WHEN CONVERT(DATE, MAX(" + FQuery + ")) IS NOT NULL and CONVERT(DATE, MAX(" + FQuery + ")) >= @dtDate " +
                   "THEN CONVERT(DATE, MAX(" + FQuery + ")) " +
                   "WHEN CONVERT(DATE, MAX(" + FQuery + ")) IS NOT NULL and  CONVERT(DATE, MAX(" + FQuery + ")) <= @dtDate " +
                   "THEN @dtDate "+
                   "ELSE CONVERT(DATE, c.CreatedOn) "+
                   "END, "+
                   "c.currency "+
                   "FROM cctsp_schoolMaster c "+
                   "LEFT JOIN SPA_INVOICE_MASTER b ON   b.ActiveStatus = 'A' AND b.SchlId = c.schlId AND b.Invoice_Status IN('CMPEP', 'CMPE', 'CMPP', 'DCMP', 'CMP') "+
                   "LEFT JOIN SPA_INVOICE_Detail a ON a.ActiveStatus = 'A' AND " + FQuery + "  IS NOT NULL AND a.Invoice_Id = b.Invoice_Id " +
                   "LEFT JOIN Language_Master d ON d.Lang_id = c.Lang_Id "+
                   "WHERE c.schlId = "+Schlid+" and c.ActiveStatus = 'A' "+
                   "GROUP BY DATEPART(MONTH, " + FQuery + "), " +
                   "DATENAME(YEAR, " + FQuery + "), DATENAME(MONTH, " + FQuery + "), " +
                   "d.Currency_ShortName, c.CreatedOn, c.currency "+
                   "ORDER BY DATENAME(YEAR, " + FQuery + "), DATEPART(MONTH, " + FQuery + ") ";
        }

        #endregion
        #region Employee
        public string checkinsertAccess(long? UserId, string SectionName)
        {
            return " SELECT COUNT(a.UserId) FROM cctsp_user a" +
                    " JOIN cctsp_Role b ON b.RoleId = a.RoleId AND (b.Schlid = a.schId OR b.Schlid = 236) AND b.ActiveStatus = 'A'" +
                    " JOIN ctsp_SolutionMaster d ON d.SectionName = '" + SectionName + "'" +
                    " JOIN cctsp_RoleDetails c ON c.ActiveStatus = 'A' AND c.RoleId = b.RoleId AND c.Schid = b.Schlid" +
                    " AND convert(bigint, c.AccesstoSub) = d.SolutionId WHERE a.UserId = " + UserId +
                    " AND ((c.insertAccess != 'N' And (d.ItenName ='2' or d.itenName ='3')) or d.itenName ='1')";
        }
        #endregion
        #region ImportExcel
        public string updateUrlAccordingtoShop(long schlId, string url)
        {
            return "INSERT INTO cctsp_categoryDetails " +
                    "(CatgId, CatgType, CatgDesc, domainType, ActiveStatus, CreatedOn) " +
                    "SELECT CatgId, 'IME URL', '" + url + "', " + schlId + ", 'A', getdate() " +
                    "FROM cctsp_categoryMaster " +
                    "WHERE catgDesc = 'Import Excel Url'";
        }
        public string GetColumns(long SchlId)
        {
            return "SELECT [Identity]=a.CatgTypeId,ColumnName=a.Gender,AliasName=a.Banner_Image, " +
                    "CustColName = c.SectionDesc,Datatype = a.Search_Image,[ISNULL]=a.Email_Image " +
                    "FROM cctsp_categoryDetails a " +
                    "JOIN cctsp_categoryMaster b ON b.CatgName='Import Excel' AND b.ActiveStatus='A' " +
                    "LEFT JOIN CTSP_SolutionMaster c ON c.CatgTypeId= a.CatgTypeId AND c.ActiveStatus= 'A' and c.SchId= " + SchlId + " " +
                    "JOIN CCTSP_SchoolMaster d on d.schlId=" + SchlId + " " +
                    "WHERE a.CatgId= b.CatgId AND a.ActiveStatus= 'A' and a.Lang_id=d.Lang_id";
        }
        #endregion
        #region PartialPayment
        public string PartialPaymentQuery(long Invoiceid)
        {
            return "SELECT a.Invoice_Id,ISNULL(a.Total_Amount, (select sum(ISNULL(TotalPricewithvat,(TotalPrice+((TotalPrice*ISNULL(VAT,0))/100)))) " +
                     "from SPA_INVOICE_Detail " +
                     "Where ActiveStatus = 'A' and Invoice_Id = a.invoice_Id) ) AS Total_Amount, " +
                     "a.Remaining_Amount, b.TotalPrice , b.Invoice_Payment_id,b.Pay_Date,b.CreatedDate,b.Payment_Catg, b.Discount_Amount,b.Pay_Amount " +
                     "FROM SPA_INVOICE_MASTER a " +
                     "LEFT JOIN SPA_INVOICE_PAYMENTDETAILS b on " +
                     "a.Invoice_Id = b.Invoice_Id AND b.ActiveStatus='A' Where a.Invoice_Id =" + Invoiceid;
        }
        #endregion
        #region Access
        public string getAccess(string PageName, long schlId, long UserId)
        {
            return "SELECT f.SectionName as subTabName,f.itenName as FlowStatus,insertAccess as [Insert],updateAccess as [update], " +
            "DeleteAccess as [Delete],AccessToData as [Display] " +
            "FROM ctsp_SolutionMaster f " +
            "JOIN cctsp_user d ON d.UserId =" + UserId + " " +
            "JOIN cctsp_Role e ON e.RoleId = d.RoleId AND(e.Schlid = " + schlId + " OR e.Schlid = 236) AND e.ActiveStatus = 'A' " +
            "JOIN cctsp_RoleDetails c ON c.ActiveStatus = 'A' AND e.RoleId = c.RoleId AND c.Schid = e.Schlid " +
            "AND f.SolutionId = CONVERT(BIGINT, c.AccesstoSub) " +
            "WHERE f.SectionName = '" + PageName + "'";
        }
        #endregion
        #region Check Invoice
        public string CheckInvoiceStaus(long ReservationId)
        {
            return " select a.Invoice_Id as InvoiceId,b.Invoice_Status as InvoiceStatus " +
                   " from SPA_INVOICE_Detail " +
                   " a join SPA_INVOICE_STATUS b on a.Invoice_Id = b.Invoice_Id " +
                   " Where a.ActiveStatus = 'A' and b.ActiveStatus = 'A' and a.ReservationId = " + ReservationId;
        }
        #endregion
        #region Product
        public string getSettlement(long SchlId, bool ischecked, int Lang_id, string CheckStatus)
        {
            var Query = "";
            if (CheckStatus == "1")
                Query = "and b.Duration != '' and b.Amount != '' ";
            return "SELECT " +
                    "a.Insurance_Id,a.Tarif_Number,a.Settlement_Number,a.Settlement_text,b.Health_Assign_id,a.[order]," +
                    "CAST(CASE WHEN b.Health_Assign_id IS NULL THEN 0 ELSE 1 END AS bit) AS checkbox, " +
                    "b.Duration,b.Amount,b.Field1 as flg, " +
                    "Cast(Cast(b.Duration as float)/5 as float ) as Quantity, " +
                    "Cast((cast(b.Amount as float) * 5) / cast(b.Duration as int) as float) as Rate " +
                    "FROM health_insurance a " +
                    "LEFT JOIN Assigned_Health_Insurance b ON b.[order] = a.[order] AND b.ActiveStatus = 'A' and b.SchlId = " + SchlId + " " +
                    "WHERE a.ActiveStatus = 'A' and a.Langid=" + Lang_id +
                    (ischecked == true ? " and b.Health_Assign_id IS NOT NULL " : " ") + Query +
                    "ORDER BY a.Tarif_Number,a.Settlement_Number";
        }
        #endregion
        #region Dynamic Field according Flow
        public string DynammicFormFieldAccordingFlow(int LangId, long FlowId, string PageName)
        {
            return "select Lang_id,Value,Page_Name,Order_id,English_Label,Label_Name, " +
                   "case When(select count(a.FieldDetail_id) from Spa_Assign_FieldDetails a " +
                   "join Spa_Table_FieldName b on a.Field_Id = b.Field_Id " +
                   "Where a.Flow_Id = " + FlowId + " and a.ActiveStatus = 'A' and b.ActiveStatus = 'A'and b.field_Name = English_Label) > 0 " +
                   "then 1 else 0 end as checkId " +
                   "from Language_Label_Detail " +
                   "where Page_Name = '" + PageName + "' and Lang_Id = " + LangId;
        }
        public string checkTabAccess(string TabName, long FlowId)
        {
            return "select Count(c.MainTab_id) as Count from cctsp_CategoryDetails a " +
                   "join Spa_Flow b on b.flow_id = " + FlowId + " " +
                   "join Spa_FlowDetails c on c.flow_id = b.Flow_id and a.CatgTypeid = c.MainTab_Id " +
                   "Where CatgId = 147 and b.ActiveStatus = 'A' and c.ActiveStatus = 'A' " +
                   "and a.CatgDesc = '" + TabName + "'";
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
        public string CheckSubTabAccessFlowVise(long FlowId, string MainTab, string SubTab, int LangId, string PageName)
        {
            var Query = "";
            if (FlowId > 1)
                Query = "join Spa_Flow c on c.Flow_Id = " + FlowId + " and c.ActiveStatus = 'A' " +
                        "join Spa_FlowDetails d on d.Flow_Id = c.Flow_Id and d.ActiveStatus = 'A' " +
                        "and d.MainTab_Id = a.CatgTypeId and d.SubTab_Id = b.SolutionId ";
            return "select b.SolutionId as SubId,f.value as SubTabName, " +
                   "b.Url as TabLink,b.image as Condition,b.[Group] as ConditionValue ,b.SectionDesc as Link " +
                   "from cctsp_CategoryDetails a  " +
                   "join ctsp_SolutionMaster b on a.CatgTypeId = b.CatgtypeId " + Query +
                   "join Language_Label_Detail f on f.English_Label = b.SectionName " +
                   "where a.CatgId = 147 and a.CatgDesc = '" + MainTab + "' and a.ActiveStatus = 'A' and " +
                   "b.ActiveStatus = 'A' and b.SectionName != '" + SubTab + "' " +
                   "and  f.Page_Name = '" + PageName + "' and f.Lang_Id =" + LangId + " " +
                   "group by b.SolutionId,f.value,b.Url,b.image,b.[Group],b.SectionDesc";
        }
        #endregion

        #region RegistrationDetails
        public string RegistrationDetails(long schlId,int ReservationId)
        {
           return "select b.CatgDesc as ProductName,c.sectiondesc as Productdesc, "+
                  "h.FirstName as ShopOwnerName,h.LastName as ShopOwnerLastName,h.EmailId as ShopOwnerEmail, "+
                  "g.original_Website as ShopUrl,f.SchlAddress as ShopAddress,f.SchlCity as ShopCity,f.SchlState as ShopState ,f.Schpin as ShopPincode, "+
                  "a.comment,Convert(TIME, a.FromSlotMasterId) as EndTime , "+
                  "Convert(TIME, a.ToSlotMasterId) as StartTime "+
                  ",CONVERT(DATE, a.BookingDate) as BookedDate, "+
                  "e.FirstName as ClientName,  e.LastName as ClientLastName ,e.EmailId as ClientEmail, "+
	              "d.FirstName as EmpFName, d.LastName as EmpLName "+
                  "from SPA_EmployeeScheduler a "+
                  "join CCTSP_CategoryDetails b on b.catgtypeid = a.product_id "+
                  "join CTSP_SolutionMaster c on b.CatgtypeId = c.CatgtypeId "+
                  "join CCTSP_User d on d.UserId = a.Emp_UserId "+
                  "join CCTSP_User e on e.UserId = a.CLIENT_UserId "+
                  "join cctsp_SchoolMaster f on f.Schlid = a.Schid "+
                  "join CCTSP_User h on h.Schid = a.Schid and h.RoleId = 1 "+
                 "join cctsp_LendingPageMaster g on g.Schid = a.Schid "+
                 "where a.schid = "+schlId+"  and b.catgid = 111 and c.SchId = a.schid "+
                 "and d.schid = a.schid "+
                 "and a.EmpSchDetailsId = "+ ReservationId;
        }
        #endregion

    }
}