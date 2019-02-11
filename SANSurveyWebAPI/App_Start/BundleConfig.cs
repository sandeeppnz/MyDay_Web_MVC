using System.Web;
using System.Web.Optimization;

namespace SANSurveyWebAPI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            #region Registration
            bundles.Add(new StyleBundle("~/Content/siteRegistration").Include(
                  "~/Content/bootstrap.css",
                  "~/Content/stepwizard.css", // no min
                  "~/Content/remodal/remodal-default-theme.css", //no min
                  "~/Content/remodal/remodal.css", //no min
                  "~/Content/bootstrap-slider.min.css", //****** no normal
                  "~/Content/Calendar2/responsive-calendar.css", //****** no normal
                  //"~/Content/bootstrap-datetimepicker.min.css", //****** no normal
                  //"~/Content/funkyradio.css", //no min
                  //"~/Content/Slick/slick.css", //no min
                  //"~/Content/Slick/slick-theme.css", //no min
                  "~/Content/tooltipster/tooltipster.bundle.css",
                  "~/Content/kendo/2016.3.1118/kendo.common.min.css", // no normal ****
                  "~/Content/kendo/2016.3.1118/Custom/kendo.custom3.css", // no min
                  //"~/Content/kendo/2016.3.1118/kendo.common-bootstrap.min.css", //??
                  //"~/Content/kendo/2016.3.1118/kendo.common.css", //??
                  //"~/Content/kendo/2016.3.1118/kendo.default.css", //??
                  //"~/Content/kendo/2016.3.1118/kendo.default.mobile.css", //??
                  //"~/Content/iCheck/skins/flat/green.css", //??
                  "~/Content/formValidation/formValidation.css",
                  "~/Content/UserRegistration.css" //no min
                  //"~/Content/timepicker/bootstrap-timepicker.css",
                  //"~/Content/timepicker/pygments.css", //no min
                  ));


            bundles.Add(new ScriptBundle("~/bundles/siteRegistration").Include(
                   "~/Scripts/modernizr-*",
                   "~/Scripts/jquery-1.10.2.js",
                   //"~/Scripts/jquery-3.1.1.js",
                   "~/Scripts/bootstrap.js",
                   "~/Scripts/respond.js",
                   "~/Scripts/stepwizard.js",
                   "~/Scripts/moment.js",
                   "~/Scripts/moment-with-locales.js",
                   "~/Scripts/moment-timezone.js",
                   "~/Scripts/jquery.unobtrusive-ajax.js",
                   "~/Scripts/cultures/kendo.culture.en-GB.min.js", //no normal
                   "~/Scripts/MySurvey.js"
                    ));
            #endregion

            #region User

            bundles.Add(new StyleBundle("~/Content/siteUser").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.min.css",
                "~/Content/remodal/remodal-default-theme.css", //no min
                "~/Content/remodal/remodal.css", //no min
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/kendo/2016.3.1118/kendo.common.min.css", // no normal ****
                "~/Content/kendo/2016.3.1118/Custom/kendo.custom3.css", // no min
                "~/Content/remodal/remodal-default-theme.css", //no min
                "~/Content/remodal/remodal.css", //no min
                "~/Content/formValidation/formValidation.css",
                "~/Content/Calendar2/responsive-calendar.css",
                "~/Content/UserModule.css",
                "~/Content/NavBarUserMobile.css"
            ));


            bundles.Add(new ScriptBundle("~/bundles/siteUser").Include(
               "~/Scripts/modernizr-*",
               "~/Scripts/jquery-1.10.2.js",
               "~/Scripts/jquery-3.1.1.js",
               "~/Scripts/bootstrap.js",
               "~/Scripts/respond.js",
               "~/Scripts/jquery.blockUI.js", // no min
               "~/Scripts/jquery.big-slide.js", //no min
               "~/Scripts/moment.js",
               "~/Scripts/moment-with-locales.js",
               "~/Scripts/moment-timezone.js",
               "~/Scripts/remodal/remodal.js",
               "~/Scripts/bootstrap-datetimepicker.js",
               "~/Scripts/BootBox/bootbox.min.js", //no normal
               "~/Scripts/formValidation/formValidation.js",
               "~/Scripts/formValidation/framework/bootstrap.js",
               "~/Scripts/Calendar2/responsive-calendar.js",
               "~/Scripts/jquery.unobtrusive-ajax.js"
            ));

            #endregion

            #region Survey
            bundles.Add(new StyleBundle("~/Content/siteSurvey").Include(
              "~/Content/bootstrap.css",
              //"~/Content/font-awesome.css",
              "~/Content/remodal/remodal-default-theme.css", //no min
              "~/Content/remodal/remodal.css", //no min
              "~/Content/tooltipster/tooltipster.bundle.css",
              "~/Content/funkyradioSurvey.css", //no min
              //"~/Content/Slick/slick.css", //no min
              //"~/Content/Slick/slick-theme.css", //no min
              "~/Content/timepicker/bootstrap-timepicker.css",
              "~/Content/timepicker/pygments.css", //no min
              "~/Content/jquery.bootstrap-touchspin.css", //no min
              "~/Content/formValidation/formValidation.css",
              "~/Content/bootstrap-slider.min.css", //****** no normal
              "~/Content/kendo/2016.3.1118/kendo.common.min.css", // no normal ****
              "~/Content/kendo/2016.3.1118/Custom/kendo.custom3.css", // no min
              "~/Content/RotatingCard/rotating-card.css", // no min
              "~/Content/SurveyModule.css" // no min
              ));


            bundles.Add(new ScriptBundle("~/bundles/siteSurvey").Include(
                  "~/Scripts/modernizr-*",
                  "~/Scripts/bootstrap.js",
                  "~/Scripts/respond.js",
                  //"~/Scripts/jquery.blockUI.js", // no min
                  "~/Scripts/jquery.unobtrusive-ajax.js",
                  //"~/Scripts/remodal/remodal.js",
                  "~/Scripts/bootstrap-timepicker.js",
                  //"~/Scripts/jquery.blockUI.js",
                  //"~/Scripts/Slick/slick.js",
                  //"~/Scripts/tooltipster/tooltipster.core.js",
                  //"~/Scripts/tooltipster/tooltipster.bundle.js",
                  //"~/Scripts/kendo/2016.3.1118/Custom/kendo.custom.min.js", // no normal
                  //"~/Scripts/kendo/2016.3.1118/kendo.all.min.js", // no normal
                  "~/Scripts/cultures/kendo.culture.en-GB.min.js", //no normal
                  //"~/Scripts/jquery.bootstrap-touchspin.js", // no min
                  //"~/Scripts/formValidation/formValidation.js",
                  "~/Scripts/ormValidation/framework/bootstrap.js"
                  //"~/Scripts/bootstrap-slider.min.js" //no normal
            ));
            #endregion

            #region Admin
            bundles.Add(new StyleBundle("~/Content/siteAdmin").Include(
               //"~/Content/tooltipster/tooltipster.bundle.min.css",
               "~/Content/font-awesome.min.css",
               //"~/Content/jquery.bootstrap-touchspin.css",
               //"~/Content/bootstrap-slider.min.css",
               //"~/Content/funkyradio.css",
               //"~/Content/Slick/slick.css",
               //"~/Content/Slick/slick-theme.css",
               "~/Content/bootstrap.css",
               "~/Content/kendo/2016.3.1118/kendo.common-bootstrap.min.css",
              "~/Content/kendo/2016.3.1118/Custom/kendo.Custom.css",
               "~/Content/bootstrap-datetimepicker.css",
               "~/Content/timepicker/bootstrap-timepicker.min.css",
               "~/Content/remodal/remodal-default-theme.css",
               "~/Content/remodal/remodal.css",
               "~/Content/AdminModule.css",
               "~/Content/formValidation/formValidation.min.css",
               //"~/Content/NavBarUser.css",
               "~/Content/NavBarUserMobile.css"
            ));
            #endregion





            bundles.Add(new ScriptBundle("~/bundles/bootbox").Include(
              "~/Scripts/BootBox/bootbox.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-slider").Include(
              "~/Scripts/bootstrap-slider.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/telerik").Include(
              "~/Scripts/kendo/2016.3.1118/kendo.all.min.js",
              "~/Scripts/kendo/2016.3.1118/kendo.aspnetmvc.min.js"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/telerikCustom").Include(
              "~/Scripts/kendo/2016.3.1118/Custom/kendo.custom.min.js",
              "~/Scripts/kendo/2016.3.1118/Custom/kendo.customCalendar.min.js"
              //"~/Scripts/kendo/2016.3.1118/kendo.aspnetmvc.min.js"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/telerikTimeZone").Include(
              "~/Scripts/kendo/2016.3.1118/kendo.timezones.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"
                 ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/tooltipster").Include(
                "~/Scripts/tooltipster/tooltipster.bundle.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/blockUI").Include(
                "~/Scripts/jquery.blockUI.js"
                ));
       
            bundles.Add(new ScriptBundle("~/bundles/slick").Include(
              "~/Scripts/Slick/slick.js"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/TouchspinNumericTextBox").Include(
              "~/Scripts/jquery.bootstrap-touchspin.js"
                  ));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap-timepicker").Include(
                "~/Scripts/bootstrap-timepicker.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datetimepicker").Include(
          "~/Scripts/bootstrap-datetimepicker.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/DateTimePickerV3").Include(
             "~/Scripts/DateTimePickerV3/bootstrap-datetimepicker.js"
                          ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                "~/Scripts/moment.js",
                "~/Scripts/moment-with-locales.js"
                               ));

            bundles.Add(new ScriptBundle("~/bundles/momentTimeZone").Include(
                "~/Scripts/moment-timezone.js"
                   //"~/Scripts/moment-timezone-utils.js",
                   //"~/Scripts/moment-timezone-with-data-2010-2020.min.js",
                   //"~/Scripts/moment-timezone-with-data.min.js"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/remodal").Include(
                "~/Scripts/remodal/remodal.js"
                 ));


            bundles.Add(new ScriptBundle("~/bundles/iCheck").Include(
         "~/Scripts/iCheck/icheck.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/jquery.unobtrusive-ajax").Include(
              "~/Scripts/jquery.unobtrusive-ajax.js"
                             ));

            bundles.Add(new ScriptBundle("~/bundles/formValidation").Include(
             "~/Scripts/formValidation/formValidation.js",
             "~/Scripts/formValidation/framework/bootstrap.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/jquery.validate*"));


        }
    }
}
