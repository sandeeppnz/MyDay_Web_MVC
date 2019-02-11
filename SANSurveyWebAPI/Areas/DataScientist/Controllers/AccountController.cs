using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.DataScientist.Controllers
{
    [Authorize(Roles = "DataScientist")]
    public class AccountController : BaseController
    {
        private AuthenticationService authService;

        public AccountController()
        {
            this.authService = new AuthenticationService();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            var ctx = Request.GetOwinContext();
            var status = await authService.Logout(ctx);
            Session.RemoveAll();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}