using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SPA.Common
{
    public class CustomAutohrize : AuthorizeAttribute
    {
        private readonly string RoleId;
        public CustomAutohrize(string role)
        {
            this.RoleId = role;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = true;
            if (!((RoleId != null && HttpContext.Current.Session["RoleId"].ToString() == RoleId) || (RoleId == null && HttpContext.Current.Session["RoleId"].ToString() != null && HttpContext.Current.Session["RoleId"].ToString() != "4" && HttpContext.Current.Session["RoleId"].ToString() != "0")))
            {
                authorize = false;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest()) filterContext.HttpContext.Items["AjaxPermissionDenied"] = true;
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "SignIn"
                }));
            }
        }
    }
}