using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SPA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               "Login",
               "Login",
               new { controller = "Login", action = "Login" }
               );
            routes.MapRoute(
               "SignIn",
               "SignIn",
               new { controller = "Login", action = "SignIn" }
               );

            routes.MapRoute(
               "ChooseEmployee",
               "ChooseEmployee",
               new { controller = "Home", action = "ChooseEmployee" }
               );
            routes.MapRoute(
              "ChooseYourProduct",
              "ChooseYourProduct",
              new { controller = "Home", action = "ChooseYourProduct" }
              );

            

            routes.MapRoute(
             "PREISE",
             "PREISE",
             new { controller = "Clickandgo", action = "PREISE" }
             );
            routes.MapRoute(
            "VORTEILE",
            "VORTEILE",
            new { controller = "Clickandgo", action = "VORTEILE" }
            );
            routes.MapRoute(
            "KONTAKTREGISTRIERUNG",
            "KONTAKTREGISTRIERUNG",
            new { controller = "Clickandgo", action = "KONTAKTREGISTRIERUNG" }
            );
            routes.MapRoute(
            "FUNKTIONEN",
            "FUNKTIONEN",
            new { controller = "Clickandgo", action = "FUNKTIONEN" }
            );
            routes.MapRoute(
            "About",
            "About",
            new { controller = "Clickandgo", action = "About" }
            );
            routes.MapRoute(
            "SMARTWAREPLUS",
            "SMARTWAREPLUS",
            new { controller = "Clickandgo", action = "SMARTWAREPLUS" }
            );
            routes.MapRoute(
            "clickandgo",
            "clickandgo",
            new { controller = "ClickGo", action = "CLICKANDGO" }
            );
            routes.MapRoute(
            "Aboutus",
            "Aboutus",
            new { controller = "ClickGo", action = "Aboutus" }
            );
            routes.MapRoute(
            "Index",
            "Index",
            new { controller = "Clickandgo", action = "Index" }
            );
            routes.MapRoute(
            name: "Default",
           url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
        );
        }

    }
}
