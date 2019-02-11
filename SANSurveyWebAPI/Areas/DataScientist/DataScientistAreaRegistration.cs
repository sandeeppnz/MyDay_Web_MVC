using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.DataScientist
{
    public class DataScientistAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataScientist";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DataScientist_default",
                "DataScientist/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}