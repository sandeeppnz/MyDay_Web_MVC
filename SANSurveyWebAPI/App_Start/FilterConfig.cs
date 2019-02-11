using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DAL;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AuthorizeAttribute());



        }
    }
}
