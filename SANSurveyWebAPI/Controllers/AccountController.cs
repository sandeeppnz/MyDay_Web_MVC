using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SANSurveyWebAPI.Models;
using System.Web.Hosting;
using System.IO;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        #region Logger
        private async Task LogSignupIn(string uid)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Signup, Constants.PageAction.Get, Constants.PageType.Enter, 0, uid);
        }

        private async Task LogSignupOut(string uid, string status)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Signup, Constants.PageAction.Post, Constants.PageType.Exit, 0, uid + " " + status);
        }

        private async Task LogLoginIn(string url)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Login, Constants.PageAction.Get, Constants.PageType.Enter, 0, url);
        }

        private async Task LogLoginOut(string url)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Login, Constants.PageAction.Post, Constants.PageType.Exit, 0, url);
        }



        #endregion

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private RegistrationService registrationService;
        private HangfireScheduler hangfireService;

        private JobService jobService;

        private PageStatService pageStatSvc;


        public AccountController()
        {
            this.registrationService = new RegistrationService();
            this.hangfireService = new HangfireScheduler();
            this.pageStatSvc = new PageStatService();
            this.jobService = new JobService();

        }

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
            hangfireService.Dispose();
            jobService.Dispose();
            pageStatSvc.Dispose();

            base.Dispose(disposing);
        }



        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {

            await LogLoginIn(returnUrl);

            LoginViewModel v = new LoginViewModel();
            v.returnUrl = returnUrl;
            return View(v);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ValidateUser(string userid, string password)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

            string returnUrl = "/";

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(userid, password, true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    Session["UserName"] = userid;
                    return RedirectToLocal_JSON(returnUrl);

                //case SignInStatus.LockedOut:
                //    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                            await LogLoginOut(redirect.Action + " " + redirect.Controller);
                            return RedirectToAction(redirect.Action, redirect.Controller);
                        }

                        if (redirect.Controller == "Account" && redirect.Action == "Signup")
                        {
                            redirect.Action = "WellBeing";
                            redirect.Controller = "Register";
                            await LogLoginOut(redirect.Action + " " + redirect.Controller);
                            return RedirectToAction(redirect.Action, redirect.Controller);
                        }

                    }


                    //}
                    await LogLoginOut(v.returnUrl);
                    return RedirectToLocal(v.returnUrl);



                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = v.returnUrl, RememberMe = v.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("ErroLogin", Constants.ErrorLoginMsg);
                    return View(v);
            }
        }




        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);


                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                await Task.Run(() =>
                {
                    ForgotPasswordSendEmail(user.Email, callbackUrl);
                });


                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public async void ForgotPasswordSendEmail(string emailAddress, string linkMsg)
        {

            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            engines.Add(new Postal.FileSystemRazorViewEngine(viewsPath));

            var emailService = new Postal.EmailService(engines);

            var email = new RetrievePasswordEmail
            {
                To = emailAddress,
                AppName = SANSurveyWebAPI.Constants.AppName,
                Name = "User",
                Message = linkMsg
            };

            await email.SendAsync();
        }






        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {

                string domain = base.GetBaseWebsiteURL();

                await Task.Run(() =>
                {
                    PasswordResetSendEmail(user.Email, domain);
                });

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }


        public async void PasswordResetSendEmail(string emailAddress, string linkMsg)
        {

            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            engines.Add(new Postal.FileSystemRazorViewEngine(viewsPath));

            var emailService = new Postal.EmailService(engines);

            var email = new PasswordResetEmail
            {
                To = emailAddress,
                AppName = SANSurveyWebAPI.Constants.AppName,
                Name = "User",
                Message = linkMsg + "/#contact"
            };

            await email.SendAsync();
        }




        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {


            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.RemoveAll();
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1)); Response.Cache.SetNoStore();
            //return View();
            return View();
            //return RedirectToAction("Login");
            //return Content("Logout Sucessfull");
            //return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOffAjax()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.RemoveAll();
            //return View();
            //return View();
            return Json(new[] { true });
            //return Content("Logout Sucessfull");
            //return RedirectToAction("Index", "Home");
        }
        #region "Kids Survey"

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> KidsSignUp(string uid)
        {
            await LogSignupIn(uid);
            if (string.IsNullOrEmpty(uid))
            {return RedirectToAction("Login", "Account");}

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;
            v.Email = Session["kidsEmail"].ToString();
            v.Password = Session["kidsPassword"].ToString();
            v.ConfirmPassword = Session["kidsPassword"].ToString();
            Session["KidsUid"] = uid;
            return View(v);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> KidsSignUp(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if (registrationService.IsRegisteredProfile(v.Email))
                    {emailLcase = v.Email.ToString();}
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    {string userId = string.Empty;
                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);
                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };
                                var result = UserManager.Create(user, v.Password);
                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;
                                }
                                else
                                { AddErrors(result); }
                            }
                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);
                            
                            if (profile != null)
                            {
                                await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(),
                                    JobType.Email.ToString(), profile.Id,
                                    JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;

                                await LogSignupOut(uid, profile.RegistrationProgressNext);
                                return RedirectToAction("NewSurvey", "Kids", new { uid = Session["KidsUid"].ToString() });
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            { ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg); }
            // If we got this far, something failed, redisplay form
            return View(v);
        }
        #endregion

        #region Loop Survey
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SignupForDemo(string uid, string surveyID)
        {

            await LogSignupIn(uid);

            if (string.IsNullOrEmpty(uid))
            {
                return RedirectToAction("Login", "Account");
            }

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;
            v.Email = Session["UserEmailGenerated"].ToString();
            v.Password = Session["UserPasswordGenerated"].ToString();
            v.ConfirmPassword = Session["UserPasswordGenerated"].ToString();
            Session["surveyUID"] = surveyID;
            return View(v);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignupForDemo(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if (registrationService.IsRegisteredProfile(v.Email))
                    {
                        emailLcase = v.Email.ToString();
                    }
                    if (registrationService.IsRegisteredProfile(emailLcase))// || registrationService.IsRegisteredProfile(v.Email))
                    {
                        string userId = string.Empty;

                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))// || registrationService.IsRegistrationCompleted(v.Email, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);

                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };

                                var result = UserManager.Create(user, v.Password);

                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;                                    
                                }
                                else
                                {
                                    AddErrors(result);
                                }

                            }

                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);

                            /*
                             Dummy comment
                            */

                            if (profile != null)
                            {
                                await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(),
                                    JobType.Email.ToString(), profile.Id,
                                    JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;

                                await LogSignupOut(uid, profile.RegistrationProgressNext);
                                //return RedirectToAction(profile.RegistrationProgressNext, "Register");
                                
                                return RedirectToAction("NewSurvey", "Survey3", new { uid = Session["surveyUID"].ToString() });
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            {
                ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg);
            }
            // If we got this far, something failed, redisplay form
            return View(v);
        }
        #endregion

        #region CaseWorkers Demo Survey
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SignupForCaseWorkers(string uid, string surveyID)
        {
            await LogSignupIn(uid);

            if (string.IsNullOrEmpty(uid))
            {
                return RedirectToAction("Login", "Account");
            }

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;
            v.Email = Session["UserEmailGenerated"].ToString();
            v.Password = Session["UserPasswordGenerated"].ToString();
            v.ConfirmPassword = Session["UserPasswordGenerated"].ToString();
            Session["surveyUID"] = surveyID;
            return View(v);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignupForCaseWorkers(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if (registrationService.IsRegisteredProfile(v.Email))
                    {
                        emailLcase = v.Email.ToString();
                    }
                    if (registrationService.IsRegisteredProfile(emailLcase))// || registrationService.IsRegisteredProfile(v.Email))
                    {
                        string userId = string.Empty;

                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))// || registrationService.IsRegistrationCompleted(v.Email, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);

                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };

                                var result = UserManager.Create(user, v.Password);

                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;
                                }
                                else
                                {
                                    AddErrors(result);
                                }

                            }

                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);

                            /*
                             Dummy comment
                            */

                            if (profile != null)
                            {
                                await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(),
                                    JobType.Email.ToString(), profile.Id,
                                    JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;

                                await LogSignupOut(uid, profile.RegistrationProgressNext);
                                //return RedirectToAction(profile.RegistrationProgressNext, "Register");

                                return RedirectToAction("NewSurvey", "CaseWorker", new { uid = Session["surveyUID"].ToString() });
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            {
                ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg);
            }
            // If we got this far, something failed, redisplay form
            return View(v);
        }
        #endregion

        #region WAMDemo

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SignupForWAM(string uid, string wamUserId)
        {

            await LogSignupIn(uid);

            if (string.IsNullOrEmpty(uid))
            {
                return RedirectToAction("Login", "Account");
            }

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;
            v.Email = Session["WAMUserEmailGenerated"].ToString();
            v.Password = Session["WAMUserPasswordGenerated"].ToString();
            v.ConfirmPassword = Session["WAMUserPasswordGenerated"].ToString();
            Session["WAMUserId"] = wamUserId;
            return View(v);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignupForWAM(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if (registrationService.IsRegisteredProfile(v.Email))
                    {
                        emailLcase = v.Email.ToString();
                    }
                    if (registrationService.IsRegisteredProfile(emailLcase))// || registrationService.IsRegisteredProfile(v.Email))
                    {
                        string userId = string.Empty;

                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))// || registrationService.IsRegistrationCompleted(v.Email, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);

                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };

                                var result = UserManager.Create(user, v.Password);

                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;
                                }
                                else
                                {
                                    AddErrors(result);
                                }

                            }

                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);

                            /*
                             Dummy comment
                            */

                            if (profile != null)
                            {
                                //await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(),
                                await jobService.CreateJobAsync(JobName.WarrenMahonySignupCompletedEmail.ToString(),
                                    JobType.Email.ToString(), profile.Id,
                                    JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;

                                await LogSignupOut(uid, profile.RegistrationProgressNext);
                                //return RedirectToAction(profile.RegistrationProgressNext, "Register");

                                //return RedirectToAction("NewSurvey", "Survey3", new { uid = Session["surveyUID"].ToString() });
                                return RedirectToAction("WAMWellBeing", "Register", new { id = Session["UiD"].ToString() });
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            {
                ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg);
            }
            // If we got this far, something failed, redisplay form
            return View(v);
        }

        #endregion

        #region "Exit Version 2 Survey"

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ExitSignUp(string uid)
        {
            await LogSignupIn(uid);
            if (string.IsNullOrEmpty(uid))
            { return RedirectToAction("Login", "Account"); }

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;
            v.Email = Session["ExitEmail"].ToString();
            v.Password = Session["ExitPassword"].ToString();
            v.ConfirmPassword = Session["ExitPassword"].ToString();
            Session["ExitV2Uid"] = uid;
            return View(v);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExitSignUp(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if (registrationService.IsRegisteredProfile(v.Email))
                    { emailLcase = v.Email.ToString(); }
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    {
                        string userId = string.Empty;
                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);
                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };
                                var result = UserManager.Create(user, v.Password);
                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;
                                }
                                else
                                { AddErrors(result); }
                            }
                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);

                            if (profile != null)
                            {
                                await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(),
                                    JobType.Email.ToString(), profile.Id,
                                    JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;

                                await LogSignupOut(uid, profile.RegistrationProgressNext);
                                return RedirectToAction("NewSurvey", "ExitV2", new { uid = Session["ExitV2Uid"].ToString() });
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            { ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg); }
            // If we got this far, something failed, redisplay form
            return View(v);
        }

        #endregion


        #region "SOCIAL WORKERS - BASELINE"

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CWSignUp(string uid)
        {
            await LogSignupIn(uid);

            if (string.IsNullOrEmpty(uid))
            { return RedirectToAction("Login", "Account"); }

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;
            v.Email = Session["CaseWorkersEmail"].ToString();
            v.Password = Session["CaseWorkersPwd"].ToString();
            v.ConfirmPassword = Session["CaseWorkersPwd"].ToString();
            Session["CaseWorkersUid"] = uid;

            return View(v);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CWSignUp(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if (registrationService.IsRegisteredProfile(v.Email))
                    { emailLcase = v.Email.ToString(); }
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    {
                        string userId = string.Empty;
                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);
                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };
                                var result = UserManager.Create(user, v.Password);
                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;
                                }
                                else
                                { AddErrors(result); }
                            }
                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);

                            if (profile != null)
                            {
                                await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(),
                                    JobType.Email.ToString(), profile.Id,
                                    JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;

                                await LogSignupOut(uid, Constants.CaseWorkersRegistrationStatus.SubjectiveWellBeing.ToString());
                                return RedirectToAction("WellBeing", "CaseWorkersRegister", new { uid = Session["UiD"].ToString() });
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            { ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg); }
            // If we got this far, something failed, redisplay form
            return View(v);
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Signup(string uid)
        {

            await LogSignupIn(uid);

            if (string.IsNullOrEmpty(uid))
            {
                return RedirectToAction("Login", "Account");
            }

            RegisterViewModel v = new RegisterViewModel();
            v.Uid = uid;

            return View(v);
        }

        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup(RegisterViewModel v, string uid)
        {
            if (ModelState.IsValid)
            {
                string emailLcase = v.Email.ToLower();
                if (registrationService.IsEmailValid(emailLcase) || registrationService.IsEmailValid(v.Email)) //if two emails are same, flag 
                {
                    if (registrationService.IsRegisteredProfile(emailLcase))
                    { emailLcase = emailLcase.ToString(); }
                    else if(registrationService.IsRegisteredProfile(v.Email))
                    {
                        emailLcase = v.Email.ToString();
                    }
                    if (registrationService.IsRegisteredProfile(emailLcase))// || registrationService.IsRegisteredProfile(v.Email))
                    {
                        string userId = string.Empty;

                        if (registrationService.IsRegistrationCompleted(emailLcase, v.Uid))// || registrationService.IsRegistrationCompleted(v.Email, v.Uid))
                        {
                            ModelState.AddModelError("RegistrationIsAlreadyCompleted", Constants.ErrorRegistrationAlreadyCompletedMsg);
                            return View(v);
                        }
                        else
                        {
                            //Get the aspnet user
                            var userInDb = UserManager.FindByEmail(emailLcase);
                          
                            if (userInDb != null)
                            {
                                userId = userInDb.Id;
                                await SignInManager.SignInAsync(userInDb, isPersistent: false, rememberBrowser: false);
                            }
                            else
                            {
                                //Create ASPNET account
                                var user = new ApplicationUser
                                {
                                    UserName = emailLcase,
                                    Email = emailLcase
                                };

                                var result = UserManager.Create(user, v.Password);

                                if (result.Succeeded)
                                {
                                    //Add role as registered user
                                    var resultRole = UserManager.AddToRole(
                                        user.Id, SANSurveyWebAPI.Constants.ApplicationRole.Registered_User.ToString().Replace("_", " "));
                                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                                    userId = user.Id;
                                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                                    // Send an email with this link
                                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                                }
                                else
                                {
                                    AddErrors(result);
                                }

                            }

                            ProfileDto profile = registrationService.SignupOnExit(emailLcase, v.Uid, userId);
                            
                            /*
                             Dummy comment
                            */

                            if (profile != null)
                            {
                                await jobService.CreateJobAsync(JobName.RegistrationCompletedEmail.ToString(), 
                                    JobType.Email.ToString(), profile.Id, 
                                    JobMethod.Auto.ToString(),GetBaseURL(), string.Empty);

                                Session["UserName"] = profile.LoginEmail;
                                Session["ProfileId"] = profile.Id;                              

                                await LogSignupOut(uid, profile.RegistrationProgressNext);
                                return RedirectToAction(profile.RegistrationProgressNext, "Register"); 
                            }
                            else
                            {
                                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                                return View(v);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
                        return View(v);
                    }
                }
                ModelState.AddModelError("EmailNotFound", Constants.ErrorSignupMsg);
            }
            else
            {
                ModelState.AddModelError("EmailAlreadyExists", Constants.ErrorSignupEmailExistsMsg);
            }
            // If we got this far, something failed, redisplay form
            return View(v);
        }



        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }



        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
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


        private JsonResult RedirectToLocal_JSON(string returnUrl)
        {
            if (returnUrl == "/")
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

            if (Url.IsLocalUrl(returnUrl))
            {
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);


        }





        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}