using System.Web;
using System.Web.Optimization;

namespace RaceAlly.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/js").Include(
                "~/Scripts/excanvas.js",
                "~/Scripts/jquery.js",
                "~/Scripts/jquery.ui.custom.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.flot.js",
                "~/Scripts/jquery.flot.resize.js",
                "~/Scripts/jquery.peity.js",
                "~/Scripts/fullcalendar.js",
                "~/Scripts/unicorn.js",
                "~/Scripts/unicorn.dashboard.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/fullcalendar.css",
                "~/Content/unicorn.main.css",
                "~/Content/unicorn.grey.css"));
        }
    }
}