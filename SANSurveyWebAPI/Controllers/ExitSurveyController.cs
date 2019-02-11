using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using static SANSurveyWebAPI.Constants;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.App_Start.Auth;
using Microsoft.Owin;
using System.Web.UI;

namespace SANSurveyWebAPI.Controllers
{

    [Authorize]
    public class ExitSurveyController : BaseController
    {
        #region Logger
        private void EmailError(string msg)
        {
            HangfireScheduler schedulerService = new HangfireScheduler();
            var e = new EmailDto
            {
                ToEmail = Constants.AdminEmail,
                RecipientName = "Admin",
                Link = msg
            };
            schedulerService.SendError(e);
        }

        private void LogError(Exception ex, Constants.PageName pageName, Constants.PageAction pageAction, int? profileId = null)
        {
            SurveyError er = new ViewModels.Web.SurveyError();
            er.Message = ex.Message;
            er.StackTrace = ex.StackTrace;
            er.CodeFile = this.GetType().Name;
            pageStatSvc.Insert(null, null, null, false, pageName, pageAction, Constants.PageType.ERROR, profileId, er.DisplayError());

            EmailError(ex.Message);
        }

       
        private async Task LogSessionError(Constants.PageName page)
        {
            await pageStatSvc.Insert(null, null, null, false, page, Constants.PageAction.Post, Constants.PageType.SESSION_ERROR);
            EmailError("Session Error");
        }

        //TODO: rename the PageNames
        private async Task LogPage1In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage1Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }


        private async Task LogPage2In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage2Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }


        private async Task LogPage3In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage3Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }


        private async Task LogPage4In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage4Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogPage5In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }
        
        private async Task LogPage5Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }
        private async Task LogPageContinued5In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.ExitSurvey_PageContinued5, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }
        private async Task LogPageContinued5Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.ExitSurvey_PageContinued5, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }
        private async Task LogPage6In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage6Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }



        private async Task LogPage7In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage7Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogPage8In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage8Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogPage9In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage9Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }



        private async Task LogPage10In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage10Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }


        private async Task LogPage11In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage11Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }


        private async Task LogPage12In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage12Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }
        private async Task LogPageContinued12In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.ExitSurvey_PageContinued12, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }
        private async Task LogPageContinued12Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.ExitSurvey_PageContinued12, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }
        private async Task LogPage13In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage13Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogPage14In(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPage14Out(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }
        private async Task LogFeedbackIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Feedback, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogFeedbackOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Feedback, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        #endregion

        private PageStatService pageStatSvc;
        private ExitService exitService;

        #region Settings

        private ApplicationUserManager _userManager;
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

        private ApplicationSignInManager _signInManager;
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

        [AllowAnonymous]
        public ActionResult SessionError()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegistrationError()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }

        #endregion

        public ExitSurveyController()
        {
            this.pageStatSvc = new PageStatService();
            this.exitService = new ExitService();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }



        #region Index
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string id)
        {
            IntroductionViewModel v = new IntroductionViewModel();
            v.Uid = id;
            return View(v);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(IntroductionViewModel v)
        {
            return RedirectToAction("Page1", "ExitSurvey", new { v.Uid });
            //return RedirectToAction("Consent", "Register", new { v.Uid });
        }


        [HttpGet]
        public async Task<ActionResult> Page1()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page1);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage1In(profileId);

                Page1_ExitSurveyVM v = new Page1_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage1();   

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                var currList = exitService.GetExitSurveyPage1Ans(profile.Id);
                
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page1, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }



        [HttpPost]
        public async Task<ActionResult> Page1(Page1_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page1(profileId, v.QnsList);                       

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page2.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 2)
                        {
                            profile.MaxStepExitSurvey = 2;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);                        

                        await LogPage1Out(profileId);
                        return RedirectToAction("Page2");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page2);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page2, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }
        #endregion




        [HttpGet]
        public async Task<ActionResult> Page2()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page2);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage2In(profileId);

                Page2_ExitSurveyVM v = new Page2_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage2();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage2Ans(profile.Id);

                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                //v.TaskType = profile.ProfileTaskType;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page2, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page2(Page2_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page2(profileId, v.QnsList);
                        
                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page3.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 3)
                        {
                            profile.MaxStepExitSurvey = 3;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage2Out(profileId);
                        return RedirectToAction("Page3");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page2);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page2, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }




        [HttpGet]
        public async Task<ActionResult> Page3()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page3);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage3In(profileId);
                Page3_ExitSurveyVM v = new Page3_ExitSurveyVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyPage3ById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (profile.MaxStep > 3)
                {
                    if (Session["ExitSurveyPage3Session"] == null)
                    {
                        Session["ExitSurveyPage3Session"] = exitService.GetExitSurveyPage3ById(profileId); ;
                    }
                    ExitSurveyPage3_Dto c = (ExitSurveyPage3_Dto) Session["ExitSurveyPage3Session"];
                    if (c != null)
                    {
                        v.Q1 = c.Q1;
                        v.Q2 = c.Q2;
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Contract, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page3(Page3_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ExitSurveyPage3_Dto();
                        d.ProfileId = v.ProfileId;
                        d.Q1 = v.Q1;
                        d.Q2 = v.Q2;

                        await exitService.Save_Page3(d);
                        Session["ExitSurveyPage3Session"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page4.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStep < 4)
                        {
                            profile.MaxStep = 4;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogPage3Out(profileId);
                        return RedirectToAction("Page4");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page3);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.ExitSurvey_Page3, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }


        [HttpGet]
        public async Task<ActionResult> Page4()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page4);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage4In(profileId);

                Page4_ExitSurveyVM v = new Page4_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage4();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage4Ans(profile.Id);

                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page4, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page4(Page4_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page4(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page5.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 5)
                        {
                            profile.MaxStepExitSurvey = 5;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage4Out(profileId);
                        return RedirectToAction("Page5");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page4);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page4, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }




        [HttpGet]
        public async Task<ActionResult> Page5()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page5);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage5In(profileId);

                Page5_ExitSurveyVM v = new Page5_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage5();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage5Ans(profile.Id);


                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;


                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }


                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page5, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page5(Page5_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page5(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        //profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page6.ToString();
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.PageContinued5.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 6)
                        {
                            profile.MaxStepExitSurvey = 6;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage5Out(profileId);
                        //return RedirectToAction("Page6");
                        return RedirectToAction("PageContinued5");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page5);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page5, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }

        [HttpGet]
        public async Task<ActionResult> PageContinued5()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_PageContinued5);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPageContinued5In(profileId);

                PageContinued5_ExitSurveyVM v = new PageContinued5_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPageContinued5();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPageContinued5Ans(profile.Id);

                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;


                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }


                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_PageContinued5, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> PageContinued5(PageContinued5_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_PageContinued5(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page6.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 7)
                        {
                            profile.MaxStepExitSurvey = 7;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPageContinued5Out(profileId);
                        return RedirectToAction("Page6");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_PageContinued5);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_PageContinued5, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        [HttpGet]
        public async Task<ActionResult> Page6()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page6);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage6In(profileId);
                Page6_ExitSurveyVM v = new Page6_ExitSurveyVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyPage6ById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (profile.MaxStepExitSurvey > 6)
                {
                    if (Session["ExitSurveyPage6Session"] == null)
                    {
                        Session["ExitSurveyPage6Session"] = exitService.GetExitSurveyPage6ById(profileId); ;
                    }
                    ExitSurveyPage6_Dto c = (ExitSurveyPage6_Dto) Session["ExitSurveyPage6Session"];
                    if (c != null)
                    {
                        v.Q1 = c.Q1;
                        v.Q2 = c.Q2;
                        v.Q1Other = c.Q1Other;
                        v.Q2Other = c.Q2Other;

                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page6, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page6(Page6_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ExitSurveyPage6_Dto();
                        d.ProfileId = v.ProfileId;
                        d.Q1 = v.Q1;
                        d.Q2 = v.Q2;

                        if (v.Q1 == "Other")
                        {
                            d.Q1Other = v.Q1Other;
                        }
                        else
                        {
                            d.Q1Other = null;
                        }

                        if (v.Q2 == "Other")
                        {
                            d.Q2Other = v.Q2Other;
                        }
                        else
                        {
                            d.Q2Other = null;
                        }



                        await exitService.Save_Page6(d);
                        Session["ExitSurveyPage6Session"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page7.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStepExitSurvey < 8)
                        {
                            profile.MaxStepExitSurvey = 8;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogPage6Out(profileId);
                        return RedirectToAction("Page7");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page6);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.ExitSurvey_Page6, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }


        [HttpGet]
        public async Task<ActionResult> Page7()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page7);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage7In(profileId);

                Page7_ExitSurveyVM v = new Page7_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage7();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage7Ans(profile.Id);


                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page7, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page7(Page7_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page7(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page8.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 9)
                        {
                            profile.MaxStepExitSurvey = 9;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage7Out(profileId);
                        return RedirectToAction("Page8");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page7);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page7, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }


        [HttpGet]
        public async Task<ActionResult> Page8()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page8);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage8In(profileId);
                Page8_ExitSurveyVM v = new Page8_ExitSurveyVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyPage8ById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (profile.MaxStepExitSurvey > 8)
                {
                    if (Session["ExitSurveyPage8Session"] == null)
                    {
                        Session["ExitSurveyPage8Session"] = exitService.GetExitSurveyPage8ById(profileId); ;
                    }
                    ExitSurveyPage8_Dto c = (ExitSurveyPage8_Dto) Session["ExitSurveyPage8Session"];
                    if (c != null)
                    {
                        v.Q1 = c.Q1;
                        v.Q2 = c.Q2;

                        if (!string.IsNullOrEmpty(c.Q3))
                        {
                            v.Q3 = int.Parse(c.Q3);
                        }
                        else
                        {
                            v.Q3 = 0;
                        }
                        
                        v.Q4 = c.Q4;

                        if (!string.IsNullOrEmpty(c.Q5))
                        {
                            v.Q5 = int.Parse(c.Q5);
                        }
                        else
                        {
                            v.Q5 = 0;
                        }

                        if (!string.IsNullOrEmpty(c.Q6))
                        {
                            v.Q6 = int.Parse(c.Q6);
                        }
                        else
                        {
                            v.Q6 = 0;
                        }


                        if (!string.IsNullOrEmpty(c.Q7))
                        {
                            v.Q7 = int.Parse(c.Q7);
                        }
                        else
                        {
                            v.Q7 = 0;
                        }

                        if (!string.IsNullOrEmpty(c.Q8))
                        {
                            v.Q8 = int.Parse(c.Q8);
                        }
                        else
                        {
                            v.Q8 = 0;
                        }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page8, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page8(Page8_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ExitSurveyPage8_Dto();
                       
                        d.ProfileId = v.ProfileId;
                        d.Q1 = v.Q1;
                        d.Q2 = v.Q2;                        
                        d.Q4 = v.Q4; //Yes or No (knowing senior)
                        d.Q3 = v.Q3.ToString(); //How long if Yes
                        d.Q5 = v.Q5.ToString();
                        d.Q6 = v.Q6.ToString();
                        d.Q7 = v.Q7.ToString();
                        d.Q8 = v.Q8.ToString();


                        await exitService.Save_Page8(d);
                        Session["ExitSurveyPage8Session"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page9.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStepExitSurvey < 10)
                        {
                            profile.MaxStepExitSurvey = 10;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogPage8Out(profileId);
                        return RedirectToAction("Page9");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page8);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.ExitSurvey_Page8, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }


        [HttpGet]
        public async Task<ActionResult> Page9()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page9);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage9In(profileId);

                Page9_ExitSurveyVM v = new Page9_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage9();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage9Ans(profile.Id);


                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page9, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page9(Page9_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page9(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page10.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 11)
                        {
                            profile.MaxStepExitSurvey = 11;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage9Out(profileId);
                        return RedirectToAction("Page10");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page9);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page9, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }



        [HttpGet]
        public async Task<ActionResult> Page10()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page10);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage10In(profileId);

                Page10_ExitSurveyVM v = new Page10_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage10();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage10Ans(profile.Id);


                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page10, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page10(Page10_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page10(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page11.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 12)
                        {
                            profile.MaxStepExitSurvey = 12;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage10Out(profileId);
                        return RedirectToAction("Page11");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page10);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page10, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }


        [HttpGet]
        public async Task<ActionResult> Page11()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page11);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage11In(profileId);

                Page11_ExitSurveyVM v = new Page11_ExitSurveyVM();
                v.QptionsList = exitService.GetOptionsListPage11();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyPage11ById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                var currList = exitService.GetExitSurveyPage11Options(profile.Id);

                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;                                

                if (currList != null && currList.Count() > 0)
                {                    
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QptionsList)
                        {
                            if (d.ID == c.ID)
                            {
                                //c.Ans = d.Ans;
                                if (c.ID <= 7) {
                                    v.HiddenTrainingOptionIds = c.ID + "," + v.HiddenTrainingOptionIds;
                                }
                                else{
                                    v.HiddenValuedOptionIds =  c.ID + "," + v.HiddenValuedOptionIds;
                                }                                
                                    if (c.ID == 7) { v.TrainingOther = d.Ans; }
                                    else if (c.ID == 14) { v.ValuedOther = d.Ans; }
                                
                            }
                        }
                    }
                    //Page page = new Page();
                    //ScriptManager.RegisterStartupScript(page, this.GetType(), "script", "alert('HIII');", true);
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page11, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page11(Page11_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {                                       
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        IList<string> TrainingIds = v.HiddenTrainingOptionIds.Split(',').Reverse().ToList<string>();

                        IList<string> ValuedIds = v.HiddenValuedOptionIds.Split(',').Reverse().ToList<string>();

                        IList<string> allIds = TrainingIds.Concat(ValuedIds).ToList();                        

                        Page11_ExitSurveyVM AllOList = new Page11_ExitSurveyVM();
                        AllOList.QptionsList = exitService.GetOptionsListPage11();

                        List<Page11Qptions> selectedList = new List<Page11Qptions>();
                        
                        foreach (var id in allIds)
                        {
                           int i = Convert.ToInt32(id);
                            
                            selectedList.Add(AllOList.QptionsList[i]);
                            if (i == 7)
                            {                                
                                selectedList[selectedList.Count()-1].LongName = v.TrainingOther;
                            }
                            else if (i == 14)
                            {
                                selectedList[selectedList.Count()-1].LongName = v.ValuedOther;
                            }
                        }

                        await exitService.Save_Page11(profileId, selectedList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }
                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page12.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStepExitSurvey < 13)
                        {
                            profile.MaxStepExitSurvey = 13;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogPage11Out(profileId);
                        return RedirectToAction("Page12");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page11);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.ExitSurvey_Page11, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        [HttpGet]
        public async Task<ActionResult> Page12()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page12);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPage12In(profileId);

                Page12_ExitSurveyVM v = new Page12_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPage12();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPage12Ans(profile.Id);


                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page12, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page12(Page12_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_Page12(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page13.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 14)
                        {
                            profile.MaxStepExitSurvey = 14;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPage12Out(profileId);
                        return RedirectToAction("PageContinued12");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_Page12);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page12, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }

        [HttpGet]
        public async Task<ActionResult> PageContinued12()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_PageContinued12);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];
                await LogPageContinued12In(profileId);

                PageContinued12_ExitSurveyVM v = new PageContinued12_ExitSurveyVM();
                v.QnsList = exitService.GetQuestionsListPageContinued12();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetExitSurveyPageContinued12Ans(profile.Id);


                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QnsList)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_PageContinued12, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PageContinued12(PageContinued12_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int) Session["ProfileId"];
                        await exitService.Save_PageContinued12(profileId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.PageContinued12.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStepExitSurvey < 15)
                        {
                            profile.MaxStepExitSurvey = 15;
                        }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        await LogPageContinued12Out(profileId);
                        return RedirectToAction("Page13");
                    }
                    await LogSessionError(Constants.PageName.ExitSurvey_PageContinued12);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_PageContinued12, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }

        [HttpGet]
        public async Task<ActionResult> Page13()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page13);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage13In(profileId);
                Page13_ExitSurveyVM v = new Page13_ExitSurveyVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyPage13ById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (profile.MaxStepExitSurvey >= 16)
                {
                    if (Session["ExitSurveyPage13Session"] == null)
                    {
                        Session["ExitSurveyPage13Session"] = exitService.GetExitSurveyPage13ById(profileId);
                    }
                    ExitSurveyPage13_Dto c = (ExitSurveyPage13_Dto) Session["ExitSurveyPage13Session"];
                    if (c != null)
                    {
                        v.Q1_Applicable = c.Q1_Applicable;
                        if (c.Q1_Applicable == "Applicable")
                            v.Q1_Year = c.Q1_Year;
                        else v.Q1_Year = 0;

                        v.Q2_PTWork = c.Q2_PTWork;
                        if (c.Q2_PTWork == "Other")
                            v.Q2_Other = c.Q2_Other;
                        else { v.Q2_Other = null; }

                        v.Q3_NoOfPeople = c.Q3_NoOfPeople;
                        v.Q4_Martial = c.Q4_Martial;
                        if (c.Q4_Martial == "Partnership" || c.Q4_Martial == "Married")
                            v.Q5_PartnershipMarried = c.Q5_PartnershipMarried;
                        else { v.Q5_PartnershipMarried = null; }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page13, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page13(Page13_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ExitSurveyPage13_Dto();
                        d.ProfileId = v.ProfileId;
                        d.Q1_Applicable = v.Q1_Applicable;
                        if (v.Q1_Applicable == "Applicable")
                            d.Q1_Year = v.Q1_Year;
                        else d.Q1_Year = 0;
                        d.Q2_PTWork = v.Q2_PTWork;
                        if (v.Q2_PTWork == "Other")
                            d.Q2_Other = v.Q2_Other;
                        else d.Q2_Other = null;
                        d.Q3_NoOfPeople = v.Q3_NoOfPeople;
                        d.Q4_Martial = v.Q4_Martial;
                        if (v.Q4_Martial == "Partnership" || v.Q4_Martial == "Married")
                            d.Q5_PartnershipMarried = v.Q5_PartnershipMarried;
                        else d.Q5_PartnershipMarried = null;
                        await exitService.Save_Page13(d);
                        Session["ExitSurveyPage13Session"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page14.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStepExitSurvey < 16)
                        {
                            profile.MaxStepExitSurvey = 16;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogPage13Out(profileId);
                        return RedirectToAction("Page14");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page13);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page13, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }


        [HttpGet]
        public async Task<ActionResult> Page14()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.ExitSurvey_Page14);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogPage14In(profileId);
                Page14_ExitSurveyVM v = new Page14_ExitSurveyVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyPage14ById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (profile.MaxStepExitSurvey >= 17)
                {
                    if (Session["ExitSurveyPage14Session"] == null)
                    {
                        Session["ExitSurveyPage14Session"] = exitService.GetExitSurveyPage14ById(profileId);
                    }
                    ExitSurveyPage14_Dto c = (ExitSurveyPage14_Dto) Session["ExitSurveyPage14Session"];
                    if (c != null)
                    {
                        v.Q1 = c.Q1;
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.ExitSurvey_Page14, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Page14(Page14_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ExitSurveyPage14_Dto();
                        d.ProfileId = v.ProfileId;
                        d.Q1 = v.Q1;

                        await exitService.Save_Page14(d);
                        Session["ExitSurveyPage14Session"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Page14.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStepExitSurvey < 17)
                        {
                            profile.MaxStepExitSurvey = 17;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogPage14Out(profileId);
                        return RedirectToAction("Feedback");
                    }

                    await LogSessionError(Constants.PageName.ExitSurvey_Page14);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.ExitSurvey_Page14, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        [HttpGet]
        public async Task<ActionResult> Feedback()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Feedback);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                await LogFeedbackIn(profileId);
                FeedbackFor_ExitSurveyVM v = new FeedbackFor_ExitSurveyVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetExitSurveyFeedbackId(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStepExitSurvey = profile.MaxStepExitSurvey;

                if (profile.MaxStepExitSurvey >= 18)
                {
                    if (Session["ExitSurveyFeedbackSession"] == null)
                    {
                        Session["ExitSurveyFeedbackSession"] = exitService.GetExitSurveyFeedbackId(profileId);
                    }
                    ExitSurveyFeedback_Dto c = (ExitSurveyFeedback_Dto) Session["ExitSurveyFeedbackSession"];
                    if (c != null)
                    {
                        v.feedbackComments = c.feedbackComments;
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Feedback, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Feedback(FeedbackFor_ExitSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ExitSurveyFeedback_Dto();
                        d.ProfileId = v.ProfileId;
                        d.feedbackComments = v.feedbackComments;

                        await exitService.Save_Feedback(d);
                        Session["ExitSurveyFeedbackSession"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = StatusExitSurveyProgress.Completed.ToString();
                        profile.MaxStepExitSurvey = v.MaxStepExitSurvey;

                        if (profile.MaxStepExitSurvey < 18)
                        {
                            profile.MaxStepExitSurvey = 18;
                        }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        await LogFeedbackOut(profileId);
                        return RedirectToAction("Completed");
                    }

                    await LogSessionError(Constants.PageName.Feedback);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Feedback, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }
        public async Task<ActionResult> Completed()
        {
            if (Session["ProfileId"] == null)
            {
                var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                if (loggedInProfile == null)
                {
                    await LogSessionError(Constants.PageName.Registration_Roster);
                    return RedirectToAction("SessionError");
                }
                Session["ProfileId"] = loggedInProfile.Id;
            }

            if (Session["ProfileId"] != null)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Completed(CompletedViewModel_ExitSurvey step6)
        {
            return RedirectToAction("Index", "Home");
        }



        [AllowAnonymous]
        public async Task<ActionResult> Expired()
        {
            return View();
        }


        public async Task<bool> Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            return true;
        }

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult EmailFeedback(EmailFeedbackViewModel v)
        //{
        //    try
        //    {
        //        int profileId = (int) Session["ProfileId"];
        //        exitService.CreateUserFeedback(profileId, string.Empty, v);
        //        return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        private async Task ResetRegistrationSessions()
        {
            await Logout();
            Session["ProfileId"] = null;
        }

    }

}