using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SPA.Controllers
{
    public class NutzenController : Controller
    {
        // GET: Nutzen
        public ActionResult Nutzen()
        {
            return View();
        }
        public ActionResult Nutzen_Content()
        {
            return View();
        }
    }
}