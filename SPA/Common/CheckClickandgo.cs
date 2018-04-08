using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SPA.Common
{

    public class CheckClickandgo : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Function fu = new Function();

            if (!fu.CheckClickandgo(System.Web.HttpContext.Current.Request.Url.Host, null, 0).status)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Home"
                }));
            }
        }
    }
   
    //public class Exceptionfilters : HandleErrorAttribute
    //{
    //    public override void OnException(ExceptionContext filterContext)
    //    {
    //        if (filterContext.HttpContext.IsCustomErrorEnabled)
    //        {
    //            filterContext.ExceptionHandled = true;
    //            filterContext.Result = new RedirectResult("/Error/Error_List");
    //            new Function().ErrorSend(filterContext.RouteData, filterContext.Exception);
    //        }
    //    }
    //}
    public class checkShop : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Function fu = new Function();
            //if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Current.Session["RoleId"])))
            //{
            //    /*get Authorize Access*/
            //    fu.CheckandSetCookies();
            //}

            var Details = fu.CheckClickandgo(System.Web.HttpContext.Current.Request.Url.Host, null, 0);
            if (Details.status)
            {
                if (Details.WorkFlowStatus == "2")
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "ClickGo",
                        action = "Home"
                    }));
                }
                if (Details.WorkFlowStatus == "1")
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Clickandgo",
                        action = "Index"
                    }));
                }


            }

        }
    }
    public class Compress : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;
            if (filterContext.IsChildAction) return;
            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = filterContext.HttpContext.Response;
            if (encodingsAccepted.Contains("gzip"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
        }

    }
}