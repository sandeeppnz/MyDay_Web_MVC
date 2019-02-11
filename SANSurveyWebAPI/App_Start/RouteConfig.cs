using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SANSurveyWebAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //routes.MapRoute(
            //    name: "AreaRoute",
            //    url: "{area:exists}/{controller=Home}/{action=Index}"
            //);


           

            routes.MapRoute(
                  name: "Root",
                  url: "",
                  defaults: new { controller = "Root", action = "Index"},
                  namespaces: new[] { "SANSurveyWebAPI.Controllers" }
              );

            routes.MapRoute(
                  name: "Home",
                  url: "Home/",
                  defaults: new { controller = "Root", action = "Index" },
                  namespaces: new[] { "SANSurveyWebAPI.Controllers" }
              );


            routes.MapRoute(
              name: "SurveyRoute",
              url: "App/Survey/{id}",
              defaults: new { controller = "Survey3", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "SANSurveyWebAPI.Controllers" }

          );

            routes.MapRoute(
                 name: "Default",
                 url: "App/{controller}/{action}/{id}",
                 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                 namespaces: new[] { "SANSurveyWebAPI.Controllers" }
             );
        }
    }
}
