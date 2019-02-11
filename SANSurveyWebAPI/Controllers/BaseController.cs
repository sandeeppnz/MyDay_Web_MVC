using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Controllers
{
    /*
     */
    public class BaseController : Controller
    {
        public String GetBaseURL()
        {
            return Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/App/";
        }

        public String GetBaseWebsiteURL()
        {
            return Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
        }
    }
}