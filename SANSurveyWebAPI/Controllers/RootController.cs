using Microsoft.AspNet.Identity.Owin;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Controllers
{
    /*
     
        Public facing website controller
         
         
         */


    public class RootController : BaseController
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RegistrationService registrationService;
        private HangfireScheduler schedulerSvc;
        private RootService rootService;



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            registrationService.Dispose();
            schedulerSvc.Dispose();
            rootService.Dispose();

            base.Dispose(disposing);
        }

        public RootController()
        {
            this.registrationService = new RegistrationService();
            this.schedulerSvc = new HangfireScheduler();
            this.rootService = new RootService();
        }

        public RootController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (returnUrl == "/")
                return RedirectToAction("Index", "Home");

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        // GET: Root
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SendMessage(string name, string preferredContact, string preferredTime, string email, string phoneNumber, string message)
        {
            //string msg = String.Format(
            //format: " Name: {0} <br/> Preferred Method: {1} <br/> Preferred Time: {2} <br/> Email: {3} <br/>  Phone: {4} <br/> Email: {5} <br/>", 
            //args: new object[] { name, preferredContact, preferredTime, email, phoneNumber, message });

            var e = new WebsiteContactUsEmailDto
            {
                //ToEmail = Constants.AdminEmail,
                ToEmail = Constants.WebsiteEmail,
                RecipientName = Constants.AdminName,
                Link = string.Empty,
                Name = name,
                PreferredContact = preferredContact,
                PreferredTime = preferredTime,
                ReturnEmail = email,
                ReturnPhone = phoneNumber,
                Message = message
            };
            await schedulerSvc.SendWebsiteContactUs(e);

            EmailFeedbackViewModel v = new EmailFeedbackViewModel();
            v.EmailAddress = email;
            v.Message = message;
            v.PhoneNumber = phoneNumber;
            v.PreferedContact = preferredContact;
            v.PreferedTime = preferredTime;

            await rootService.CreateFeedback(v);


            return Json(new { Success = true, Result = "OK" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel v, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(v);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(v.Email, v.Password, v.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:

                    int profileId;

                    var role = User.IsInRole("Registered User");

                    //if (User.IsInRole("Registered User"))
                    //{
                    var redirect = registrationService.CheckRegistrationAtLogin(v.Email, out profileId);

                    if (redirect != null)
                    {
                        Session["UserName"] = v.Email;
                        Session["ProfileId"] = profileId;
                        if (redirect.Controller == "Register" && !string.IsNullOrEmpty(redirect.Action))
                        {
                            return RedirectToAction(redirect.Action, redirect.Controller);
                        }

                        if (redirect.Controller == "Account" && redirect.Action == "Signup")
                        {
                            redirect.Action = "WellBeing";
                            redirect.Controller = "Register";
                            return RedirectToAction(redirect.Action, redirect.Controller);
                        }

                    }


                    //}
                    return RedirectToLocal(v.returnUrl);



                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = v.returnUrl, RememberMe = v.RememberMe });
                case SignInStatus.Failure:
                    ModelState.AddModelError("ErroLogin", Constants.ErrorLoginMsgMainSite);
                    return View("Index");
                    //return RedirectToAction("Login","Account");
                default:
                    ModelState.AddModelError("ErroLogin", Constants.ErrorLoginMsgMainSite);
                    return View(v);
            }
        }

        public ActionResult FAQs()
        { return View(); }

        public ActionResult About()
        { return View(); }

        public ActionResult Contact()
        { return View(); }
    }
}