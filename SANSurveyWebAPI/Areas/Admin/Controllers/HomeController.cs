using SANSurveyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : BaseController
    {

        public HomeController()
        {

        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    //db.Dispose();
            //}
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Admin Home Page";
            return View();
        }
    }
}