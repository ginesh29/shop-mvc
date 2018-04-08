using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SPA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            if (Context.Items["AjaxPermissionDenied"] is bool)
            {
                Context.Response.StatusCode = 401;
                Context.Response.End();
            }
        }
        protected void Session_Start()
        {
            Session["ProductChoosed"] = "";
            Session["heading"] = "";
            Session["LanguageId"] = 1;
            Session["Message"] = "";
            Session["Error"] = false;
            //De
            Session["UserId"] = "";
            Session["SchoolId"] = 0;
            Session["RoleId"] = 0;
            Session["UserEmailId"] = "";
            Session["UserFirstName"] = "";
            Session["UserLastName"] = "";
            Session["UserGender"] = "";
            Session["UserName"] = "";
            Session["PhoneNumber"] = "";
            Session["Result"] = "";
            Session["FirstNameValidation"] = "";
            Session["AddCustomerMsg"] = "";
            Session["CustomerValidationMsg"] = "";
            Session["Remember"] = "";
            Session["Expire"] = "";
            Session["MultipleLogin"] = "";

            //For Taking Redirection to right direction in Calander
            Session["View"] = "";
            Session["ViewUserId"] = "";
            Session["ViewDate"] = "";
            Session["ResId"] = "";
            Session["AllView"]= "";
            Session["Medicments"] = new List<Models.SpecialInsertForDoctor>();
            Session["Advice"] = new List<Models.SpecialInsertForDoctor>();
            Session["UploadImagePath"] = "";
            Session["CountSpecial"] = 0;
            Session["CountAdvice"] = 0;
            Session.Timeout = 52000;

            /*Click-and-go*/
            Session["Registration"] = new Models.Registration();
            Session["RegistrationDetails"] = "";
            Session["Various"] = "";
        }
        //protected void Application_EndRequest(object sender, EventArgs e)
        //{
        //    if (Context.Items["AjaxPermissionDenied"] is bool)
        //    {
        //        Context.Response.StatusCode = 401;
        //        Context.Response.End();
        //    }
        //}



    }
}
