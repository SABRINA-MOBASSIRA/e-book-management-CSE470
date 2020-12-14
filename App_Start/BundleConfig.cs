using System.Web;
using System.Web.Optimization;

namespace EBM
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/Themejquery").Include(
                        "~/Theme/vendors/jquery/dist/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/ThemeJS").Include(
                      "~/Theme/vendors/bootstrap/dist/js/bootstrap.min.js",
                      "~/Theme/vendors/fastclick/lib/fastclick.js",
                      "~/Theme/vendors/nprogress/nprogress.js",
                      "~/Theme/vendors/jQuery-Smart-Wizard/js/jquery.smartWizard.js",
                      "~/Theme/vendors/Chart.js/dist/Chart.min.js",
                      "~/Theme/vendors/gauge.js/dist/gauge.min.js",
                      "~/Theme/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js",
                      "~/Theme/vendors/iCheck/icheck.min.js",
                      "~/Theme/vendors/skycons/skycons.js",
                      "~/Theme/vendors/Flot/jquery.flot.js",
                      "~/Theme/vendors/Flot/jquery.flot.pie.js",
                      "~/Theme/vendors/Flot/jquery.flot.time.js",
                      "~/Theme/vendors/Flot/jquery.flot.stack.js",
                      "~/Theme/vendors/Flot/jquery.flot.resize.js",
                      "~/Theme/vendors/flot.orderbars/js/jquery.flot.orderBars.js",
                      "~/Theme/vendors/flot-spline/js/jquery.flot.spline.min.js",
                      "~/Theme/vendors/flot.curvedlines/curvedLines.js",
                      "~/Theme/vendors/DateJS/build/date.js",
                      "~/Theme/vendors/jqvmap/dist/jquery.vmap.js",
                      "~/Theme/vendors/jqvmap/dist/maps/jquery.vmap.world.js",
                      "~/Theme/vendors/jqvmap/examples/js/jquery.vmap.sampledata.js",
                      "~/Theme/vendors/moment/min/moment.min.js",
                      "~/Theme/vendors/bootstrap-daterangepicker/daterangepicker.js",
                      "~/Theme/vendors/datatables.net/js/jquery.dataTables.min.js",
                      "~/Theme/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js",
                      "~/Theme/vendors/datatables.net-buttons/js/dataTables.buttons.min.js",
                      "~/Theme/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
                      "~/Theme/vendors/datatables.net-buttons/js/buttons.flash.min.js",
                      "~/Theme/vendors/datatables.net-buttons/js/buttons.html5.min.js",
                      "~/Theme/vendors/datatables.net-buttons/js/buttons.print.min.js",
                      "~/Theme/vendors/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                      "~/Theme/vendors/datatables.net-keytable/js/dataTables.keyTable.min.js",
                      "~/Theme/vendors/datatables.net-responsive/js/dataTables.responsive.min.js",
                      "~/Theme/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js",
                      "~/Theme/vendors/datatables.net-scroller/js/dataTables.scroller.min.js",
                      "~/Theme/vendors/jszip/dist/jszip.min.js",
                      "~/Theme/vendors/pdfmake/build/pdfmake.min.js",
                      "~/Theme/vendors/pdfmake/build/vfs_fonts.js",
                      "~/Theme/vendors/bootstrap-wysiwyg/js/bootstrap-wysiwyg.min.js",
                      "~/Theme/vendors/jquery.hotkeys/jquery.hotkeys.js",
                      "~/Theme/vendors/google-code-prettify/src/prettify.js",
                      "~/Theme/vendors/jquery.tagsinput/src/jquery.tagsinput.js",
                      "~/Theme/vendors/switchery/dist/switchery.min.js",
                      "~/Theme/vendors/select2/dist/js/select2.full.min.js",
                      "~/Theme/vendors/parsleyjs/dist/parsley.min.js",
                      "~/Theme/vendors/autosize/dist/autosize.min.js",
                      //"~/Theme/vendors/devbridge-autocomplete/dist/jquery.autocomplete.min.js",
                      //"~/Theme/vendors/starrr/dist/starrr.js",
                      "~/Theme/vendors/pnotify/dist/pnotify.js",
                      "~/Theme/vendors/pnotify/dist/pnotify.buttons.js",
                      "~/Theme/vendors/pnotify/dist/pnotify.nonblock.js",
                      "~/Theme/build/js/SiteScript.js",
                      "~/Theme/vendors/validator/validator.js",
                      "~/Theme/vendors/ion.rangeSlider/js/ion.rangeSlider.min.js",
                      "~/Theme/vendors/mjolnic-bootstrap-colorpicker/dist/js/bootstrap-colorpicker.min.js",
                      "~/Theme/vendors/jquery.inputmask/dist/min/jquery.inputmask.bundle.min.js",
                      "~/Theme/vendors/jquery-knob/dist/jquery.knob.min.js",
                      "~/Theme/vendors/cropper/dist/cropper.min.js",
                      //"~/Theme/Upload/jasny-bootstrap/js/jasny-bootstrap.min.js",
                      //"~/Theme/vendors/dropzone/dist/min/dropzone.min.js",
                      //"~/Scripts/jquery-ui-1.12.1.custom/jquery-ui.min.js",
                      "~/Theme/build/js/custom.min.js"));

            bundles.Add(new StyleBundle("~/Content/ThemeCss").Include(
                      "~/Theme/vendors/bootstrap/dist/css/bootstrap.min.css",
                      "~/Theme/vendors/font-awesome/css/font-awesome.min.css",
                      "~/Theme/vendors/nprogress/nprogress.css",
                      "~/Theme/vendors/iCheck/skins/flat/green.css",
                      "~/Theme/vendors/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css",
                      "~/Theme/vendors/jqvmap/dist/jqvmap.min.css",
                      "~/Theme/vendors/bootstrap-daterangepicker/daterangepicker.css",
                      "~/Theme/build/css/custom.min.css",
                      "~/Theme/vendors/datatables.net-bs/css/dataTables.bootstrap.min.css",
                      "~/Theme/vendors/datatables.net-buttons-bs/css/buttons.bootstrap.min.css",
                      "~/Theme/vendors/datatables.net-fixedheader-bs/css/fixedHeader.bootstrap.min.css",
                      "~/Theme/vendors/datatables.net-responsive-bs/css/responsive.bootstrap.min.css",
                      "~/Theme/vendors/datatables.net-scroller-bs/css/scroller.bootstrap.min.css",
                      "~/Theme/vendors/google-code-prettify/bin/prettify.min.css",
                      "~/Theme/vendors/select2/dist/css/select2.min.css",
                      "~/Theme/vendors/switchery/dist/switchery.min.css",
                      "~/Theme/vendors/starrr/dist/starrr.css",
                      "~/Theme/vendors/pnotify/dist/pnotify.css",
                      "~/Theme/vendors/pnotify/dist/pnotify.buttons.css",
                      "~/Theme/vendors/pnotify/dist/pnotify.nonblock.css",
                      "~/Theme/vendors/normalize-css/normalize.css",
                      "~/Theme/vendors/ion.rangeSlider/css/ion.rangeSlider.css",
                      "~/Theme/vendors/ion.rangeSlider/css/ion.rangeSlider.skinFlat.css",
                      "~/Theme/vendors/dropzone/dist/min/dropzone.min.css",
                      "~/Theme/vendors/mjolnic-bootstrap-colorpicker/dist/css/bootstrap-colorpicker.min.css",
                      "~/Theme/vendors/cropper/dist/cropper.min.css"
                      //"~/Scripts/jquery-ui-1.12.1.custom/jquery-ui.min.css"
                      //"~/Theme/Upload/jasny-bootstrap/css/jasny-bootstrap.min.css",
                      //"~/Theme/Upload/jasny-bootstrap/css/jasny-bootstrap-responsive.min.css"
                      ));
        }
    }
}
