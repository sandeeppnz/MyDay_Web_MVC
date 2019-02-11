using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.ClientAdmin
{
    public class ClientAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ClientAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {


            context.MapRoute(
                "ClientAdmin_default",
                "ClientAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}