using SANSurveyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.DataScientist.Controllers
{
    public class HomeController : BaseController
    {
        // GET: DataScientist/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}