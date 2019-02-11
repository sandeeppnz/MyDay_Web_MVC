using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SANSurveyWebAPI.Models;
using System.Data.Entity;
using System.Web.Security;
using System.Web.Hosting;
using System.IO;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using SANSurveyWebAPI;
using SANSurveyWebAPI.BLL;
using System.Net;
using SANSurveyWebAPI.Controllers;

namespace SANSurveyWebAPI.Areas.ClientAdmin.Controllers
{
    [Authorize(Roles = "ClientAdmin")]
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