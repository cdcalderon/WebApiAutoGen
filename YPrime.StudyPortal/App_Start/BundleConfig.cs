using System.Web.Optimization;

namespace YPrime.StudyPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/Overloads/jquery.unobtrusive-ajax.overload.js",
                "~/Scripts/jquery.cookie-1.4.1.min.js",
                "~/Scripts/jQuery.print.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"
            ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"
            ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/EasyDropdown/jquery.easydropdown.min.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-dialog.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-toggle.js",
                "~/Scripts/spin.min.js",
                "~/Scripts/bootstrap-multiselect.js",
                "~/Scripts/bootstrap-datetimepicker.min.js"
            ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-dialog.css",
                "~/Content/font-awesome.min.css",
                "~/Content/bootstrap-toggle.css",
                "~/Content/themes/base/jquery-ui.theme.css",
                "~/Content/themes/base/datepicker.css",
                "~/Content/bootstrap-multiselect.css",
                "~/Content/bootstrap-datetimepicker.min.css",
                "~/Content/YPrimeStudyPortal.css",
                "~/Content/VerticalMenu.css",
                "~/Content/YPrimeDrugReturnKit.css",
                "~/Content/YPrime2017.css",
                "~/Content/CEI2017.css",
                "~/Content/CEI2017Overload.css",
                "~/Content/easydropdown.flat.css",
                "~/Content/Datatables/datatable.css",
                "~/Content/Select2/select2.css"
            ));
            bundles.Add(new StyleBundle("~/Content/gridmvc").Include(
                "~/Content/gridmvc.datepicker.min.css"
            ));
            bundles.Add(new ScriptBundle("~/bundles/helpers").Include(
                "~/Scripts/Helpers/ajaxHelper.js",
                "~/Scripts/Helpers/uiHelper.js",
                "~/Scripts/Helpers/electronicSignatureHelper.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/d3").Include(
                "~/Scripts/d3/d3.min.js",
                "~/Scripts/ReportEngine/chartEngine.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                "~/Scripts/Datatables/datatables.min.js",
                "~/Scripts/Datatables/datetime-moment.js",
                "~/Scripts/Datatables/datatablesInit.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                "~/Scripts/Select2/select2.min.js",
                "~/Scripts/Select2/select2.full.min.js"
            ));
            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
                "~/Scripts/jquery.inputmask/inputmask.js",
                "~/Scripts/jquery.inputmask/jquery.inputmask.js",
                "~/Scripts/jquery.inputmask/inputmask.extensions.js",
                "~/Scripts/jquery.inputmask/inputmask.date.extensions.js",
                "~/Scripts/jquery.inputmask/inputmask.regex.extensions.js",
                //and other extensions you want to include
                "~/Scripts/jquery.inputmask/inputmask.numeric.extensions.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                "~/Scripts/moment.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sessiontimeout").Include(
                "~/Scripts/SessionTimeoutVm.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/timeout-worker").Include(
                "~/Scripts/timeout-worker.js"
            ));
        }
    }
}