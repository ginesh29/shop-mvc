using System.Web;
using System.Web.Optimization;

namespace SPA
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       // "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/jquery-2.1.1.min.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/pointer_events_polyfill.js",
                        "~/Scripts/moment-with-locales.js",
                        "~/Scripts/bootstrap-tabcollapse.js",
                        "~/Scripts/owl.carousel.js",
                        "~/Scripts/spa_script.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/Common.js",
                        "~/Scripts/Plugins.js",
                        "~/Scripts/kalendae.js",
                        "~/Scripts/bootstrap-datetimepicker.js",
                        "~/Scripts/tableHeadFixer.js",
                        "~/Scripts/dropzone.js",
                        "~/Scripts/jquery.slimscroll.min.js",
                        "~/Scripts/sweetalert2.min.js"
                        
                       ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/bootstrap.min.css",
                     "~/Content/font-awesome.min.css",
                     "~/Content/main.css",
                     "~/Content/owl.carousel.css",
                     "~/Content/spa_style.css",
                     "~/Content/spa_responsive.css",
                     "~/Content/site.css",
                     "~/Content/kalendae.css",
                     "~/Content/metal.css",
                     "~/Content/bootstrap-datetimepicker.css",
                     "~/Content/dropzone.css",
                     "~/Content/print.css",
                     "~/Content/animate.css",
                     "~/Content/sweetalert2.css"
                    ));

            bundles.Add(new StyleBundle("~/Contents/css").Include(
                      "~/clickcontent/bootstrap.css",
                      "~/clickcontent/font-min.css",
                      "~/clickcontent/font-awesome.min.css",
                      "~/clickcontent/style.css",
                      "~/clickcontent/responsive.css",
                      "~/clickcontent/aos.css",
                      "~/clickcontent/ClickGo.css",
                        "~/clickcontent/ClickGo_Responsive.css"));

            bundles.Add(new ScriptBundle("~/bundles/clickscript").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/commonclick.js",
                      "~/Scripts/jquery.validate.min.js",
                       "~/Scripts/aos.js"
                      ));
            BundleTable.EnableOptimizations = true;
        }
    }
}
