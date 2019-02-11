using SANSurveyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.CallStaff.Controllers
{
    [Authorize(Roles = "CallStaff")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}