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

namespace SANSurveyWebAPI.Controllers
{

    [Authorize]
    public class RegisterController : BaseController
    {
        /*
         - register controller handles all the view related to the registration (baseline) process
         - Consist of all the pages except the Signup page
             

         Notes:   
         The session objects are used to save temporarily on the page, so that data does not have to be 
         accesssed whenever the user changes to different pages in the registration process

        The comments submitted from the Registration process is saved in ProfileComments table


        This controller depends on the RegistrationServices

             */



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

        private async Task LogConsentIn(string uid)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Consent, Constants.PageAction.Get, Constants.PageType.Enter, 0, uid);
        }

        private async Task LogConsentOut(string uid, string response)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Consent, Constants.PageAction.Post, Constants.PageType.Exit, 0, uid + " " + response);
        }

        private async Task LogScreeningIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Screening, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogScreeningOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Screening, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogScreenedOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Screening, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "ScreenedOut");
        }

        private async Task LogSessionError(Constants.PageName page)
        {
            await pageStatSvc.Insert(null, null, null, false, page, Constants.PageAction.Post, Constants.PageType.SESSION_ERROR);
            EmailError("Session Error");
        }

        private async Task LogWellbeingIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogWellbeingOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogProfileTasksIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogProfileTasksOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Task, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogProfileTaskTimeIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_TaskTime, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogProfileTaskTimeOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_TaskTime, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogPlacementIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Placement, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogPlacementOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Placement, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogContractIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Contract, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogContractOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Contract, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogSpecialtyIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Specialty, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogSpecialtyOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Specialty, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogTrainingIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Training, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogTrainingOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Training, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogDemographicsIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Demographics, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogDemographicsOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Demographics, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        private async Task LogRosterIn(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Roster, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
        }

        private async Task LogRosterOut(int profileId)
        {
            await pageStatSvc.Insert(null, null, null, true, Constants.PageName.Registration_Roster, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
        }

        #endregion

        private PageStatService pageStatSvc;
        private RegistrationService registrationService;


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
            //Session.Abandon();
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

        [AllowAnonymous]
        public ActionResult NeedToRegister()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult DNP()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }
      
        #endregion


        public RegisterController()
        {
            this.pageStatSvc = new PageStatService();
            this.registrationService = new RegistrationService();
        }

        //TODO: add dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #region Index (For Junior Doctors)
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
            return RedirectToAction("Consent", "Register", new { v.Uid });
        }

        #endregion

        #region Guide (For Warren and Mahony)

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Guide(string id)
        {
            IntroductionViewModel v = new IntroductionViewModel();
            v.Uid = id;
            return View(v);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Guide(IntroductionViewModel v)
        {
            return RedirectToAction("Assent", "Register", new { v.Uid });
        }

        #endregion

        #region Consent (For Junior Doctors)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Consent(string uid)
        {
            try
            {
                await LogConsentIn(uid);

                var redirect = registrationService.ValidateConsentOnEntry(uid);

                if (redirect != null)
                {
                    return RedirectToAction(redirect.Action, redirect.Controller);
                }

                ConsentViewModel v = new ConsentViewModel();
                v.Uid = uid;
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Consent, Constants.PageAction.Get, 0); //zero profile ID as its consent
                return RedirectToAction("RegistrationError");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Consent(ConsentViewModel model, string uid)
        {
            try
            {
                var redirect = registrationService.ValidateConsentOnExit(uid);
                if (redirect != null)
                {
                    //pre-check
                    string action = redirect.Action;
                    string controller = redirect.Controller;

                    if (action == "DNP")
                    {
                        return Json(new { Success = true, State = "DNP" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (action == "Login")
                    {
                        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                    }


                    if (ModelState.IsValid)
                    {
                        if (model.IsAgree == "True")
                        {
                            await LogConsentOut(uid, "YES");

                            if (registrationService.RedirectConsentOnAgree(uid))
                            {
                                return Json(new { Success = true, State = "Go" }, JsonRequestBehavior.AllowGet);
                            }
                            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                        }
                        else if (model.IsAgree == "False") //This part of logic in not reached.
                        {
                            await LogConsentOut(uid, "NO");
                            if (registrationService.RedirectConsentOnDisagree(uid))
                            {
                                return Json(new { Success = true, State = "Stop" }, JsonRequestBehavior.AllowGet);
                            }
                            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                // If we got this far, something failed, redisplay form
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Consent, Constants.PageAction.Post, 0); //zero profile ID as its concent
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Assent (For Warren and Mahony)

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Assent(string uid)
        {
            try
            {
                await LogConsentIn(uid);

                var redirect = registrationService.ValidateConsentOnEntry(uid);

                if (redirect != null)
                {
                    return RedirectToAction(redirect.Action, redirect.Controller);
                }

                ConsentViewModel v = new ConsentViewModel();
                v.Uid = uid;
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Consent, Constants.PageAction.Get, 0); //zero profile ID as its consent
                return RedirectToAction("RegistrationError");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> Assent(ConsentViewModel model, string uid)
        {
            try
            {
                var redirect = registrationService.ValidateConsentOnExit(uid);
                if (redirect != null)
                {
                    //pre-check
                    string action = redirect.Action;
                    string controller = redirect.Controller;

                    if (action == "Login")
                    {
                        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                    }

                    if (ModelState.IsValid)
                    {
                        if (model.IsAgree == "True")
                        {
                            await LogConsentOut(uid, "YES");

                            if (registrationService.RedirectConsentOnAgree(uid))
                            {
                                return Json(new { Success = true, State = "Go" }, JsonRequestBehavior.AllowGet);
                            }
                            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                        }
                        else if (model.IsAgree == "False") //This part of logic in not reached.
                        {
                            await LogConsentOut(uid, "NO");
                            if (registrationService.RedirectConsentOnDisagree(uid))
                            {
                                return Json(new { Success = true, State = "Stop" }, JsonRequestBehavior.AllowGet);
                            }
                            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                // If we got this far, something failed, redisplay form
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Consent, Constants.PageAction.Post, 0); //zero profile ID as its concent
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region ScreeningExt
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ScreeningExt(string uid)
        {
            try
            {
              
                var v = new RegisterScreeningViewModel();
                ProfileDto profile = registrationService.GetProfileByUid(uid);
                await LogScreeningOut(profile.Id);

                v.CurrentLevelOfTraining = !string.IsNullOrEmpty(profile.CurrentLevelOfTraining) ? profile.CurrentLevelOfTraining : string.Empty;
                v.IsCurrentPlacement = !string.IsNullOrEmpty(profile.IsCurrentPlacement) ? profile.IsCurrentPlacement : string.Empty;

                int newMaxStep = profile.MaxStep;
                if (profile.MaxStep < 1)
                {
                    newMaxStep = 1;
                }
                v.MaxStep = newMaxStep;

                v.Uid = profile.Uid;
                v.ProfileId = profile.Id;

                return View(v);

                await LogSessionError(Constants.PageName.Registration_Screening);
                return RedirectToAction("SessionError");
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Screening, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ScreeningExt(RegisterScreeningViewModel v)
        {
            try
            {
                ProfileDto profile = registrationService.GetProfileByUid(v.Uid);
                int profileId = profile.Id;
                v.ProfileId = profileId;

                await registrationService.SaveScreening(v);

                if (v.CurrentLevelOfTraining == "Not in training" || v.IsCurrentPlacement == "No")
                {
                    await LogScreenedOut(profileId);
                    return RedirectToAction("ScreenedOut");
                }
                else
                {
                    await LogScreeningOut(profileId);
                    return RedirectToAction("Signup", "Account", new { v.Uid });
                }
               
                await LogSessionError(Constants.PageName.Registration_Screening);
                return RedirectToAction("SessionError");
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Screening, Constants.PageAction.Post);
                return RedirectToAction("RegistrationError");
            }
        }
        #endregion

        #region Screening deprecated
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<ActionResult> Screening()
        //{

        //    try
        //    {
        //        if (Session["ProfileId"] != null)
        //        {
        //            int profileId = (int) Session["ProfileId"];

        //            await LogWorkIn(profileId);

        //            var v = new RegisterScreeningViewModel();

        //            ProfileDto profile = registrationService.GetProfileById(profileId);

        //            v.CurrentLevelOfTraining = !string.IsNullOrEmpty(profile.CurrentLevelOfTraining) ? profile.CurrentLevelOfTraining : string.Empty;
        //            v.IsCurrentPlacement = !string.IsNullOrEmpty(profile.IsCurrentPlacement) ? profile.IsCurrentPlacement : string.Empty;

        //            int newMaxStep = profile.MaxStep;
        //            if (profile.MaxStep < 1)
        //            {
        //                newMaxStep = 1;
        //            }

        //            v.ProfileId = profile.Id;
        //            v.MaxStep = newMaxStep;

        //            return View(v);

        //        }

        //        await LogSessionError(Constants.PageName.Registration_Screening);
        //        return RedirectToAction("SessionError");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex, Constants.PageName.Registration_Screening, Constants.PageAction.Get);
        //        return RedirectToAction("RegistrationError");
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<ActionResult> Screening(RegisterScreeningViewModel v)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //    try
        //    {
        //        if (Session["ProfileId"] != null)
        //        {
        //            int profileId = (int) Session["ProfileId"];


        //            bool goNext = registrationService.SaveScreening(v);

        //            if (goNext)
        //            {
        //                await LogWorkOut(profileId);
        //                return RedirectToAction("WellBeing");



        //            }
        //            else
        //            {
        //                await LogScreenedOut(profileId);
        //                return RedirectToAction("ScreenedOut");


        //            }



        //        }

        //        await LogSessionError(Constants.PageName.Registration_Screening);
        //        return RedirectToAction("SessionError");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogError(ex, Constants.PageName.Registration_Screening, Constants.PageAction.Post);
        //        return RedirectToAction("RegistrationError");
        //    }

        //    //}
        //    //return View(v);
        //}
        #endregion

        #region Wellbeing
        public async Task<ActionResult> WellBeing()
        {

            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Wellbeing);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


                RegisterWellBeing1ViewModel v = new RegisterWellBeing1ViewModel();

                int profileId = (int) Session["ProfileId"];
                await LogWellbeingIn(profileId);


                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                
                if (profile.MaxStep > 1)
                {
                    if (Session["ProfileWellBeingSession"] == null)
                    {
                        Session["ProfileWellBeingSession"] = registrationService.GetProfileWellBeingByProfileId(profileId); ;
                    }

                    ProfileWellbeingDto w = (ProfileWellbeingDto) Session["ProfileWellBeingSession"];

                

                    if (!string.IsNullOrEmpty(w.SwbHome))
                        v.Q1Ans = Constants.GetInt5ScaleRating(w.SwbLife);
                    else
                        v.Q1Ans = Constants.NA_5Rating;

                    if (!string.IsNullOrEmpty(w.SwbHome))
                        v.Q2Ans = Constants.GetInt5ScaleRating(w.SwbHome);
                    else
                        v.Q2Ans = Constants.NA_5Rating;

                    if (!string.IsNullOrEmpty(w.SwbJob))
                        v.Q3Ans = Constants.GetInt5ScaleRating(w.SwbJob);
                    else
                        v.Q3Ans = Constants.NA_5Rating;

                }
                else
                {
                    v.Q1Ans = Constants.NA_5Rating;
                    v.Q2Ans = Constants.NA_5Rating;
                    v.Q3Ans = Constants.NA_5Rating;

                }

                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                return View(v);

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WellBeing(RegisterWellBeing1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                  
                    if (Session["ProfileId"] != null)
                    {
                        var d = new ProfileWellbeingDto();

                        d.ProfileId = v.ProfileId;
                        d.SwbLife = Constants.GetText5ScaleRating(v.Q1Ans);
                        d.SwbHome = Constants.GetText5ScaleRating(v.Q2Ans);
                        d.SwbJob = Constants.GetText5ScaleRating(v.Q3Ans);

                        await registrationService.SaveProfileWellbeing(d);
                        Session["ProfileWellBeingSession"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Task.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStep < 2)
                        {
                            profile.MaxStep = 2;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);



                        int profileId = (int) Session["ProfileId"];


                        await LogWellbeingOut(profileId);
                        return RedirectToAction("Task");
                    }

                    await LogSessionError(Constants.PageName.Registration_Wellbeing);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Registration_Wellbeing, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }

            }
            return View(v);
        }
        #endregion

        #region Default Tasks
        public async Task<ActionResult> Task()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Task);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


        
                int profileId = (int) Session["ProfileId"];

                await LogProfileTasksIn(profileId);

                RegisterTask2ViewModel v = new RegisterTask2ViewModel
                {
                    FullTaskListPatient = registrationService.GetTasksItemsTableListByType("Patient"),
                    //FullTaskListNonPatient = registrationService.GetTasksItemsTableListByType("Non-Patient")
                };

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                var currTaskProfileList = registrationService.GetProfileTaskItemTableListByProfileId(profile.Id);
                v.ProfileId = profile.Id;
                v.TaskType = profile.ProfileTaskType;
                v.MaxStep = profile.MaxStep;

                if (currTaskProfileList != null && currTaskProfileList.Count() > 0)
                {
                    foreach (var d in currTaskProfileList)
                    {
                        foreach (var c in v.FullTaskListPatient)
                        {
                            if (d.ID == c.ID)
                            {
                                c.Frequency = d.Frequency;
                            }
                        }

                        foreach (var e in v.FullTaskListNonPatient)
                        {
                            if (d.ID == e.ID)
                            {
                                e.Frequency = d.Frequency;
                            }
                        }

                    }
                }
                return View(v);
           
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Task, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Task(RegisterTask2ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var currDbDefaultTasks = registrationService.GetProfileTaskItemsByProfileId(v.ProfileId);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }
                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                        profile.RegistrationProgressNext = StatusRegistrationProgress.TaskTime.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ProfileTaskType = "Yes"; //v.TaskType;

                        if (profile.MaxStep < 3)
                        {
                            profile.MaxStep = 3;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        await registrationService.SaveDefaultTaskAsync(v.ProfileId, v.FullTaskListPatient);
                        //if (v.TaskType == "Yes")
                        //{
                        //    await registrationService.SaveDefaultTaskAsync(v.ProfileId, v.FullTaskListPatient);
                        //}
                        //else
                        //{
                        //    await registrationService.SaveDefaultTaskAsync(v.ProfileId, v.FullTaskListNonPatient);

                        //}



                        await LogProfileTasksOut(profileId);
                        return RedirectToAction("TaskTime");
                    }

                    await LogSessionError(Constants.PageName.Registration_Task);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.Registration_Task, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }


        #region deprecated
        //[HttpPost]
        //public async Task<ActionResult> Task(RegisterTask2ViewModel v, params int[] selectedProfileDefaultTasksList)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (Session["ProfileId"] != null)
        //            {
        //                int profileId = (int) Session["ProfileId"];

        //                var currDbDefaultTasks = registrationService.GetProfileTaskItemsByProfileId(v.ProfileId);

        //                selectedProfileDefaultTasksList = selectedProfileDefaultTasksList ?? new int[] { };

        //                //await profileSvc.SaveRegistrationDefaultTasks(v);
        //                registrationService.SaveDefaultTasks(v);

        //                await registrationService.AddDeaultTaskAsync(v.ProfileId, selectedProfileDefaultTasksList, currDbDefaultTasks);
        //                await registrationService.RemoveDefaultTaskAsync(v.ProfileId, selectedProfileDefaultTasksList, currDbDefaultTasks);

        //                await LogProfileTasksOut(profileId);
        //                return RedirectToAction("TaskTime");
        //            }

        //            await LogSessionError(Constants.PageName.Registration_Task);
        //            return RedirectToAction("SessionError");
        //        }
        //        catch (Exception ex)
        //        {

        //            LogError(ex, Constants.PageName.Registration_Task, Constants.PageAction.Post);
        //            return RedirectToAction("RegistrationError");
        //        }
        //    }
        //    return View(v);
        //}
        #endregion

        #endregion


        #region TaskTime
        public async Task<ActionResult> TaskTime()
        {

            try
            {

                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_TaskTime);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];

                await LogProfileTaskTimeIn(profileId);

                RegisterTaskTimeViewModel v = new RegisterTaskTimeViewModel();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }


                ProfileDto profile = (ProfileDto) Session["ProfileSession"];


                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (v.MaxStep > 3)
                {

                    if (Session["ProfileTaskTimeSession"] == null)
                    {
                        Session["ProfileTaskTimeSession"] = registrationService.GetProfileTaskTimeByProfileId(profileId);
                    }

                    ProfileTaskTimeDto t = (ProfileTaskTimeDto) Session["ProfileTaskTimeSession"];

                    if (t != null)
                    {
                        v.ClinicalActualTime = registrationService.ConvertToNumber(t.ClinicalActualTime);
                        v.ClinicalDesiredTime = registrationService.ConvertToNumber(t.ClinicalDesiredTime);
                        v.ResearchActualTime = registrationService.ConvertToNumber(t.ResearchActualTime);
                        v.ResearchDesiredTime = registrationService.ConvertToNumber(t.ResearchDesiredTime);
                        v.TeachingLearningActualTime = registrationService.ConvertToNumber(t.TeachingLearningActualTime);
                        v.TeachingLearningDesiredTime = registrationService.ConvertToNumber(t.TeachingLearningDesiredTime);
                        v.AdminActualTime = registrationService.ConvertToNumber(t.AdminActualTime);
                        v.AdminDesiredTime = registrationService.ConvertToNumber(t.AdminDesiredTime);

                    }


                }


                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                return View(v);
                
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_TaskTime, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> TaskTime(RegisterTaskTimeViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                  

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ProfileTaskTimeDto();

                        d.ProfileId = v.ProfileId;

                        d.ClinicalActualTime = registrationService.ConvertToDecimal(v.ClinicalActualTime);
                        d.ClinicalDesiredTime = registrationService.ConvertToDecimal(v.ClinicalDesiredTime);

                        d.ResearchActualTime = registrationService.ConvertToDecimal(v.ResearchActualTime);
                        d.ResearchDesiredTime = registrationService.ConvertToDecimal(v.ResearchDesiredTime);

                        d.TeachingLearningActualTime = registrationService.ConvertToDecimal(v.TeachingLearningActualTime);
                        d.TeachingLearningDesiredTime = registrationService.ConvertToDecimal(v.TeachingLearningDesiredTime);

                        d.AdminActualTime = registrationService.ConvertToDecimal(v.AdminActualTime);
                        d.AdminDesiredTime = registrationService.ConvertToDecimal(v.AdminDesiredTime);

                        registrationService.SaveTaskTimes(d);


                        Session["ProfileTaskTimeSession"] = d;



                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Placement.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 4)
                        {
                            profile.MaxStep = 4;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);


                        await LogProfileTasksOut(profileId);
                        return RedirectToAction("Placement");
                    }

                    await LogSessionError(Constants.PageName.Registration_TaskTime);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.Registration_TaskTime, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }
        #endregion


        #region placement
        public async Task<ActionResult> Placement()
        {

            try
            {

                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Placement);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


         
                int profileId = (int) Session["ProfileId"];

                await LogPlacementIn(profileId);

                RegisterPlacementViewModel v = new RegisterPlacementViewModel();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                           if (v.MaxStep > 4)
                {
                    if (Session["ProfilePlacementSession"] == null)
                    {
                        Session["ProfilePlacementSession"] = registrationService.GetProfilePlacementByProfileId(profileId);
                    }

                    var placement = (ProfilePlacementDto) Session["ProfilePlacementSession"];

                    if (placement != null)
                    {
                        v.PlacementStartMonth = placement.PlacementStartMonth;
                        v.PlacementStartYear = placement.PlacementStartYear;
                        v.PlacementIsInHospital = placement.PlacementIsInHospital;
                        v.PlacementHospitalName = placement.PlacementHospitalName;
                        v.PlacementHospitalStartMonth = placement.PlacementHospitalStartMonth;
                        v.PlacementHospitalStartYear = placement.PlacementHospitalStartYear;
                        v.PlacementHospitalNameOther = placement.PlacementHospitalNameOther;

                    }


                }

                return View(v);
            
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Placement, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Placement(RegisterPlacementViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                  

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ProfilePlacementDto();
                        d.ProfileId = v.ProfileId;
                        d.PlacementStartMonth = v.PlacementStartMonth;
                        d.PlacementStartYear = v.PlacementStartYear;
                        d.PlacementIsInHospital = v.PlacementIsInHospital;

                        if (v.PlacementIsInHospital == "Yes")
                        {
                            d.PlacementHospitalName = v.PlacementHospitalName;
                            d.PlacementHospitalStartMonth = v.PlacementHospitalStartMonth;
                            d.PlacementHospitalStartYear = v.PlacementHospitalStartYear;

                            if (v.PlacementHospitalName != null)
                            {
                                if (v.PlacementHospitalName.Trim() == "Other")
                                {
                                    d.PlacementHospitalNameOther = v.PlacementHospitalNameOther;
                                }
                                else
                                {
                                    d.PlacementHospitalNameOther = null;
                                }
                            }
                            else
                            {
                                d.PlacementHospitalNameOther = null;

                            }
                        }
                        else
                        {
                            d.PlacementHospitalName = null;
                            d.PlacementHospitalStartMonth = null;
                            d.PlacementHospitalStartYear = null;
                            d.PlacementHospitalNameOther = null;

                        }

                        registrationService.SavePlacement(d);
                        Session["ProfilePlacementSession"] = d;



                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Contract.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 5)
                        {
                            profile.MaxStep = 5;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);



                        await LogPlacementIn(profileId);
                        return RedirectToAction("Contract");
                    }

                    await LogSessionError(Constants.PageName.Registration_Placement);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.Registration_Placement, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region contract
        public async Task<ActionResult> Contract()
        {

            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Contract);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


          
                int profileId = (int) Session["ProfileId"];

                await LogContractIn(profileId);

                RegisterContractViewModel v = new RegisterContractViewModel();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];


                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (profile.MaxStep > 5)
                {
                    if (Session["ProfileContractSession"] == null)
                    {
                        Session["ProfileContractSession"] = registrationService.GetProfileContractByProfileId(profileId); ;
                    }

                    ProfileContractDto c = (ProfileContractDto) Session["ProfileContractSession"];

                    if (c != null)
                    {
                        v.ContractType = c.ContractType;
                        v.WorkingStatus = c.WorkingType;
                        v.HoursWorkedLastMonth = c.HoursWorkedLastMonth;
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
        public async Task<ActionResult> Contract(RegisterContractViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   


                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ProfileContractDto();
                        d.ProfileId = v.ProfileId;
                        d.ContractType = v.ContractType;
                        d.WorkingType = v.WorkingStatus;
                        d.HoursWorkedLastMonth = v.HoursWorkedLastMonth;

                        registrationService.SaveContract(d);
                        Session["ProfileContractSession"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Specialty.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 6)
                        {
                            profile.MaxStep = 6;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        await LogContractOut(profileId);
                        return RedirectToAction("Specialty");
                    }

                    await LogSessionError(Constants.PageName.Registration_Contract);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.Registration_Contract, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region Specialty
        public async Task<ActionResult> Specialty()
        {

            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Specialty);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int) Session["ProfileId"];

                await LogSpecialtyIn(profileId);

                RegisterSpecialtyViewModel v = new RegisterSpecialtyViewModel();



                IList<SelectedSpecialityVM> s = registrationService.GetProfileSpecialitiesSpecialityIdByProfileId(profileId);

                if (s.Count > 0)
                {
                    var first = s[0];

                    v.SpecialityId = first.ID;
                    v.OtherText = first.OtherText;
                }
                else
                {
                    v.SpecialityId = null;
                    v.OtherText = null;
                }




                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];



                v.SpecialityTypeList = registrationService.GetAllSpecialities();


                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                return View(v);
              
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Specialty, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Specialty(RegisterSpecialtyViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                  


                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Training.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 7)
                        {
                            profile.MaxStep = 7;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);


                        if (v.SpecialityId.HasValue)
                        {
                            var arrayInt = new int[] { v.SpecialityId.Value };

                            if (string.IsNullOrEmpty(v.OtherText))
                            {
                                await registrationService.SaveProfileSpecialtyAsync(v.ProfileId, arrayInt, null);
                            }
                            else
                            {
                                await registrationService.SaveProfileSpecialtyAsync(v.ProfileId, arrayInt, v.OtherText);
                            }
                        }

                        await LogSpecialtyOut(profileId);
                        return RedirectToAction("Training");
                    }

                    await LogSessionError(Constants.PageName.Registration_Specialty);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.Registration_Specialty, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region Training
        public async Task<ActionResult> Training()
        {

            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Training);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


               
                int profileId = (int) Session["ProfileId"];

                await LogTrainingIn(profileId);

                RegisterTrainingViewModel v = new RegisterTrainingViewModel();

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (v.MaxStep > 7)
                {

                    if (Session["ProfileTrainingSession"] == null)
                    {
                        Session["ProfileTrainingSession"] = registrationService.GetProfileTrainingByProfileId(profileId); ;
                    }

                    ProfileTrainingDto t = (ProfileTrainingDto) Session["ProfileTrainingSession"];


                    if (t != null)
                    {
                        //ProfileTrainingDto t = registrationService.GetProfileTrainingByProfileId(v.ProfileId);
                        v.TrainingStartYear = t.TrainingStartYear;
                        v.IsTrainingBreak = t.IsTrainingBreak;
                        v.TrainingBreakLengthMonths = t.TrainingBreakLengthMonths;

                    }


                }

                return View(v);
                
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Training, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Training(RegisterTrainingViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //if (Session["ProfileId"] == null)
                    //{
                    //    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    //    if (loggedInProfile == null)
                    //    {
                    //        await LogSessionError(Constants.PageName.Registration_Training);
                    //        return RedirectToAction("SessionError");
                    //    }
                    //    Session["ProfileId"] = loggedInProfile.Id;
                    //}


                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var t = new ProfileTrainingDto();
                        t.ProfileId = v.ProfileId;

                        t.TrainingStartYear = v.TrainingStartYear;
                        t.IsTrainingBreak = v.IsTrainingBreak;

                        if (v.IsTrainingBreak == "Yes")
                        {
                            t.TrainingBreakLengthMonths = v.TrainingBreakLengthMonths;
                        }
                        else
                        {
                            t.TrainingBreakLengthMonths = null;
                        }

                        registrationService.SaveTraining(t);
                        Session["ProfileTrainingSession"] = t;



                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Demographics.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 8)
                        {
                            profile.MaxStep = 8;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);





                        await LogTrainingOut(profileId);
                        return RedirectToAction("Demographics");
                    }

                    await LogSessionError(Constants.PageName.Registration_Training);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {

                    LogError(ex, Constants.PageName.Registration_Training, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }
        #endregion

        #region Demographics
        public async Task<ActionResult> Demographics()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Demographics);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


              
                int profileId = (int) Session["ProfileId"];

                await LogDemographicsIn(profileId);

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                RegisterDemographicsViewModel v = new RegisterDemographicsViewModel
                {
                    BirthYearList = registrationService.GetAllBirthYears(),
                    ////EthinicityTypeList = registrationService.GetAllEthinicitiesCheckBoxList()
                    //EthinicityTypeListDropDown = registrationService.GetAllEthinicitiesMultiSelectList(),
                    //SelectedEthinicities = registrationService.GetProfileEthiniticitiesEthnicityIdByProfileId(profileId).ToArray()
                    MedicalUniversityTypeListDropDown = registrationService.GetAllMedicalUniversities(),
                };

                if (profile != null)
                {
                    v.MaxStep = profile.MaxStep;
                    v.ProfileId = profile.Id;
                    v.Email = profile.LoginEmail;
                    v.Name = profile.Name;
                    v.MobileNumber = profile.MobileNumber;
                    if (v.MaxStep > 8)
                    {
                        if (Session["ProfileDemographicSession"] == null)
                        {
                            Session["ProfileDemographicSession"] = registrationService.GetProfileDemographicByProfileId(profileId);
                        }
                        ProfileDemographicDto d = (ProfileDemographicDto) Session["ProfileDemographicSession"];
                        if (d != null)
                        {
                            v.Gender = d.Gender;
                            v.MaritialStatus = d.MaritialStatus;
                            v.BirthYear = d.BirthYear;
                            v.IsCaregiverAdult = d.IsCaregiverAdult;
                            v.IsCaregiverChild = d.IsCaregiverChild;
                            v.IsUniversityBritish = d.IsUniversityBritish;
                            v.UniversityAttended = d.UniversityAttended;
                            v.UniversityAttendedOtherText = d.UniversityAttendedOtherText;
                            v.IsLeadership = d.IsLeadership;
                        }
                    }
                }
                return View(v);
             

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Demographics, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Demographics(RegisterDemographicsViewModel v, int[] SelectedEthinicities)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        ProfileDemographicDto d = new ProfileDemographicDto();
                        d.ProfileId = v.ProfileId;
                        d.BirthYear = v.BirthYear;
                        d.Gender = v.Gender;

                        d.MaritialStatus = v.MaritialStatus;
                        d.BirthYear = v.BirthYear;
                        d.IsCaregiverAdult = v.IsCaregiverAdult;
                        d.IsCaregiverChild = v.IsCaregiverChild;
                        d.IsUniversityBritish = v.IsUniversityBritish;
                        d.UniversityAttended = v.UniversityAttended;
                        d.UniversityAttendedOtherText = v.UniversityAttendedOtherText;
                        d.IsLeadership = v.IsLeadership;
                        registrationService.SaveDemographics(d);
                        Session["ProfileDemographicSession"] = d;

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LoginEmail = v.Email;
                        profile.Name = v.Name;
                        profile.MobileNumber = v.MobileNumber;
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Roster.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 9)
                        {
                            profile.MaxStep = 9;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        //ProfileDto profile = registrationService.GetProfileById(profileId);
                        //var selectedProfileEthinicities = v.EthinicityTypeList
                        //    .Where(x => x.IsChecked)
                        //    .Select(x => x.Display).ToList();

                        ////var currDbEthinicities = registrationService.GetProfileEthiniticitiesByProfileId(profileId);
                        //await registrationService.SaveProfileEthinicityAsync(v.ProfileId, v.SelectedEthinicities);
                        ////await registrationService.AddProfileEthnicityAsync(profileId, SelectedEthinicities, currDbEthinicities);
                        ////await registrationService.RemoveProfileEthinictyAsync(profileId, SelectedEthinicities, currDbEthinicities);
                        await LogDemographicsOut(profileId);
                        return RedirectToAction("Roster");

                    }

                    await LogSessionError(PageName.Registration_Demographics);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Registration_Demographics, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }

            return View(v);
        }
        #endregion


        #region Roster
        /*
          The roster calendar uses Ajax to create, edit and Delete the events.
          If you goto the Roster View, the ajax calls are made 
          using the CalendarController's Get, SaveNewParam, SaveEditParam, Delete
             */


        public async Task<ActionResult> Roster(string id)
        {
            try
            {
                bool containsNumbers = false; //URL checking

                if (!string.IsNullOrEmpty(id))
                {

                    containsNumbers = id.Any(char.IsDigit);
                }

                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Roster);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }


             
                RegisterRoster1ViewModel v = new RegisterRoster1ViewModel();
                int profileId = (int) Session["ProfileId"];

                await LogRosterIn(profileId);

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                v.ProfileName = profile.Name;
                v.ProfileEmail = profile.LoginEmail;
                v.ProfileOffset = profile.OffSetFromUTC;



                v.YearMonth = containsNumbers ? id : string.Empty;

                return View(v);
              
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Roster(RegisterRoster1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Completed.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegisteredDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;
                        if (profile.MaxStep < 10)
                        {
                            profile.MaxStep = 10;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        await LogRosterOut(profileId);
                        return RedirectToAction("Feedback");
                        //return null;
                    }
                    await LogSessionError(Constants.PageName.Registration_Roster);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }


        //TODO: Modify the logic to check 14 days
        public async Task<JsonResult> RosterValidate()
        {
            if (Session["ProfileId"] != null)
            {
                int profileId = (int) Session["ProfileId"];

                int numberOfSlots = registrationService.ValidateRoster(profileId);

                if (numberOfSlots < 1)
                {
                    return Json(new { Success = true, State = "Empty" }, JsonRequestBehavior.AllowGet);
                }
                else if (numberOfSlots < 3)
                {
                    return Json(new { Success = true, State = "Less" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = true, State = "OK" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Feedback

        public async Task<ActionResult> Feedback()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Roster);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

         
                RegisterFeedbackViewModel v = new RegisterFeedbackViewModel();
                int profileId = (int) Session["ProfileId"];

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                v.ProfileName = profile.Name;
                v.ProfileEmail = profile.LoginEmail;
                v.ProfileOffset = profile.OffSetFromUTC;

                return View(v);
          
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Feedback(RegisterFeedbackViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new ProfileCommentDto();
                        d.ProfileId = v.ProfileId;
                        d.Comment = v.Comment;

                        registrationService.SaveComment(d);
                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Completed.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegisteredDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;
                        if (profile.MaxStep < 10)
                        {
                            profile.MaxStep = 10;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);                                               

                        return RedirectToAction("Completed");
                        //return View(v);


                    }
                    await LogSessionError(Constants.PageName.Registration_Roster);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion


        #region ScreenedOut
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ScreenedOut()
        {
            ResetRegistrationSessions();
            return View();
        }

        #endregion

        #region Completed
        public async Task<ActionResult> Completed()
        {
            if (Session["ProfileId"] == null)
            {
                var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

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
            return RedirectToAction("Screening");
        }

        [HttpPost]
        public async Task<ActionResult> Completed(RegisterCompletedViewModel step6)
        {
            return RedirectToAction("Index", "Home");
        }

        #endregion


        [AllowAnonymous]
        public async Task<ActionResult> End()
        {
            return View();
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EmailFeedback(EmailFeedbackViewModel v)
        {
            try
            {
                int profileId = (int) Session["ProfileId"];
                registrationService.CreateUserFeedback(profileId, string.Empty, v);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }
        }


        private async Task ResetRegistrationSessions()
        {
            await Logout();
            Session["ProfileId"] = null;
        }

        #region WAM
        private ApplicationDbContext db = new ApplicationDbContext();

        #region WAM Random User Generation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult WAMDemo(string id)
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> WAMDemo()
        {
            string currentSurveyID = string.Empty;
            Session["WAMDemo"] = "Yes";
            if (ModelState.IsValid)
            {
                #region I.Create Random Profile
                //Generate random email
                string UserEmailGenerated = string.Empty;
                Random randomGenerator = new Random();
                int randomNumber = randomGenerator.Next(6999);
                UserEmailGenerated = "wam" + randomNumber + "@aut.ac.nz";
                Session["WAMUserEmailGenerated"] = UserEmailGenerated;
                Session["WAMUserPasswordGenerated"] = "******";

                Profile newProfile = new Profile();
                //Genrate UID
                Guid uid = Guid.NewGuid();

                //Which Client
                if (Session["WAMDemo"].ToString() == "Yes")
                {
                    newProfile.ClientInitials = "WAM";
                    newProfile.ClientName = "Warren and Mahoney";
                }

                //Encryption
                newProfile.LoginEmail = StringCipher.EncryptRfc2898(UserEmailGenerated);

                newProfile.CreatedDateTimeUtc = DateTime.UtcNow;
                newProfile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Invited.ToString();
                newProfile.UserId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;
                newProfile.Uid = uid.ToString();
                newProfile.Name = "WAM" + randomNumber;
                newProfile.MaxStep = 0;
                newProfile.OffSetFromUTC = 13;
                newProfile.Incentive = 0;
                newProfile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                newProfile.RegisteredDateTimeUtc = DateTime.UtcNow;
                db.Profiles.Add(newProfile);
                await db.SaveChangesAsync();
                db.Entry(newProfile).GetDatabaseValues(); //Get PROFILE

                Session["ProfileId"] = newProfile.Id;
                Session["UiD"] = newProfile.Uid;
                Session["WAMUserId"] = newProfile.UserId;
                ViewBag.UserId = new SelectList(db.Users, "Id", "Email", newProfile.UserId);
                //----End of Create Profile (will generate Profile ID)

                #endregion               
            }
            return RedirectToAction("SignupForWAM", "Account", new { uid = Session["UiD"].ToString(), wamUserId = Session["WAMUserId"].ToString() });
        }

        #endregion

        #region I. WAM Wellbeing      
        [HttpGet] 
        public async Task<ActionResult> WAMWellBeing()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    { return RedirectToAction("SessionError"); }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                RegisterWAMWellbeingVM v = new RegisterWAMWellbeingVM();
                int profileId = (int) Session["ProfileId"];

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                if (profile.MaxStep > 18)
                {
                    if (Session["WAMWellBeingSession"] == null)
                    { Session["WAMWellBeingSession"] = registrationService.GetWAMWellbeingByProfileId(profileId); }

                    WAMWellBeingDto w = (WAMWellBeingDto) Session["WAMWellBeingSession"];
                                        
                    if (!string.IsNullOrEmpty(w.SwbHome)) v.Q1Ans = Constants.GetInt5ScaleRating(w.SwbLife);
                    else v.Q1Ans = Constants.NA_5Rating;

                    if (!string.IsNullOrEmpty(w.SwbHome)) v.Q2Ans = Constants.GetInt5ScaleRating(w.SwbHome);
                    else v.Q2Ans = Constants.NA_5Rating;

                    if (!string.IsNullOrEmpty(w.SwbJob)) v.Q3Ans = Constants.GetInt5ScaleRating(w.SwbJob);
                    else v.Q3Ans = Constants.NA_5Rating;
                }
                else
                { v.Q1Ans = Constants.NA_5Rating; v.Q2Ans = Constants.NA_5Rating; v.Q3Ans = Constants.NA_5Rating; }

                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                return View(v);
            }
            catch (Exception ex)
            { return RedirectToAction("RegistrationError"); }
        }
        public void ClearSession()
        {           
            Session["ProfileId"] = null;
            Session["WAMWellBeingSession"] = null;
            Session["ProfileSession"] = null;
            Session["WAMProfileRole"] = null;
            Session["WAMTasksSession"] = null;
            Session["WAMDemographicsSession"] = null;
            Session["WAMIntentionsSession"] = null;
        }
        [HttpPost]
        public async Task<ActionResult> WAMWellBeing(RegisterWAMWellbeingVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        var d = new WAMWellBeingDto();

                        d.ProfileId = v.ProfileId;
                        d.SwbLife = Constants.GetText5ScaleRating(v.Q1Ans);
                        d.SwbHome = Constants.GetText5ScaleRating(v.Q2Ans);
                        d.SwbJob = Constants.GetText5ScaleRating(v.Q3Ans);

                        await registrationService.SaveWAMWellbeing(d);
                        Session["WAMWellBeingSession"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.WAMProfileRole.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStep < 19)
                        { profile.MaxStep = 19; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        int profileId = (int) Session["ProfileId"];
                        //await LogWellbeingOut(profileId);
                        return RedirectToAction("WAMProfileRole");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                { return RedirectToAction("RegistrationError"); }
            }
            return View(v);
        }

        #endregion

        #region II.WAM Profile Role

        [HttpGet]
        public async Task<ActionResult> WAMProfileRole()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        //await LogSessionError(Constants.PageName.Registration_ProfileRole);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                RegisterWAMProfileRoleVM v = new RegisterWAMProfileRoleVM();
                int profileId = (int) Session["ProfileId"];
                //await LogWellbeingIn(profileId);

                var disciplineList = db.DisciplineOrRoles.Select(u => u).ToList();
                v.disciplineRoleObjList = disciplineList;

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                if (profile.MaxStep > 19)
                {
                    if (Session["WAMProfileRole"] == null)
                    { Session["WAMProfileRole"] = registrationService.GetWAMProfileRoleByProfileId(profileId); }

                    WAMProfileRoleDto profileRoledto = (WAMProfileRoleDto) Session["WAMProfileRole"];
                    if (profileRoledto != null)
                    {
                        v.roleStartYear = profileRoledto.StartYear;
                        v.myProfileRole = profileRoledto.MyProfileRole;
                    }
                }
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                return View(v);
            }
            catch (Exception ex)
            {
               // LogError(ex, Constants.PageName.Registration_ProfileRole, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WAMProfileRole(RegisterWAMProfileRoleVM profileRoleVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var profileRoledto = new WAMProfileRoleDto();
                        profileRoledto.ProfileId = profileRoleVM.ProfileId;
                        profileRoledto.MyProfileRole = profileRoleVM.myProfileRole;
                        profileRoledto.StartYear = profileRoleVM.roleStartYear;

                        registrationService.SaveWAMProfileRole(profileRoledto);
                        Session["WAMProfileRole"] = profileRoledto;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(profileRoleVM.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.WAMTasks.ToString();
                        profile.MaxStep = profileRoleVM.MaxStep;

                        if (profile.MaxStep < 20)
                        { profile.MaxStep = 20; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        //await LogProfileRoleOut(profileId);
                        return RedirectToAction("WAMTasks");
                    }
                    //await LogSessionError(Constants.PageName.Registration_ProfileRole);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                   // LogError(ex, Constants.PageName.Registration_ProfileRole, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(profileRoleVM);
        }

        #endregion

        #region III. WAM Tasks

        public async Task<ActionResult> WAMTasks()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        //await LogSessionError(Constants.PageName.Registration_TaskTime);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                //await LogProfileTaskTimeIn(profileId);
                RegisterWAMTasksVM v = new RegisterWAMTasksVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (v.MaxStep > 20)
                {
                    if (Session["WAMTasksSession"] == null)
                    { Session["WAMTasksSession"] = registrationService.GetWAMTasksByProfileId(profileId); }

                    WAMTasksDto t = (WAMTasksDto) Session["WAMTasksSession"];

                    if (t != null)
                    {
                        v.ClinicalActualTime = registrationService.ConvertToNumber(t.ClinicalActualTime);
                        v.ClinicalDesiredTime = registrationService.ConvertToNumber(t.ClinicalDesiredTime);
                        v.ResearchActualTime = registrationService.ConvertToNumber(t.ResearchActualTime);
                        v.ResearchDesiredTime = registrationService.ConvertToNumber(t.ResearchDesiredTime);
                        v.TeachingLearningActualTime = registrationService.ConvertToNumber(t.TeachingLearningActualTime);
                        v.TeachingLearningDesiredTime = registrationService.ConvertToNumber(t.TeachingLearningDesiredTime);
                        v.AdminActualTime = registrationService.ConvertToNumber(t.AdminActualTime);
                        v.AdminDesiredTime = registrationService.ConvertToNumber(t.AdminDesiredTime);
                    }
                }
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                return View(v);
            }
            catch (Exception ex)
            {
                //LogError(ex, Constants.PageName.Registration_TaskTime, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WAMTasks(RegisterWAMTasksVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        var d = new WAMTasksDto();
                        d.ProfileId = v.ProfileId;
                        d.ClinicalActualTime = registrationService.ConvertToDecimal(v.ClinicalActualTime);
                        d.ClinicalDesiredTime = registrationService.ConvertToDecimal(v.ClinicalDesiredTime);

                        d.ResearchActualTime = registrationService.ConvertToDecimal(v.ResearchActualTime);
                        d.ResearchDesiredTime = registrationService.ConvertToDecimal(v.ResearchDesiredTime);

                        d.TeachingLearningActualTime = registrationService.ConvertToDecimal(v.TeachingLearningActualTime);
                        d.TeachingLearningDesiredTime = registrationService.ConvertToDecimal(v.TeachingLearningDesiredTime);

                        d.AdminActualTime = registrationService.ConvertToDecimal(v.AdminActualTime);
                        d.AdminDesiredTime = registrationService.ConvertToDecimal(v.AdminDesiredTime);

                        registrationService.SaveWAMTasks(d);
                        Session["WAMTasksSession"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.WAMDemographics.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 21) { profile.MaxStep = 21; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        //await LogProfileTaskTimeOut(profileId);
                        return RedirectToAction("WAMIntentions");
                    }
                    //await LogSessionError(Constants.PageName.Registration_TaskTime);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    //LogError(ex, Constants.PageName.Registration_TaskTime, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region IV. WAM Demographics

        public async Task<ActionResult> WAMDemographics()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        //await LogSessionError(Constants.PageName.Registration_Demographics);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                //await LogDemographicsIn(profileId);

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                RegisterWAMDemographicsVM v = new RegisterWAMDemographicsVM {
                    BirthYearList = registrationService.GetAllBirthYears(),
                };

                if (profile != null)
                {
                    v.MaxStep = profile.MaxStep;
                    v.ProfileId = profile.Id;                    
                    if (v.MaxStep > 22)
                    {
                        if (Session["WAMDemographicsSession"] == null)
                        { Session["WAMDemographicsSession"] = registrationService.GetWAMDemographicsByProfileId(profileId); }
                        WAMDemographicsDto d = (WAMDemographicsDto) Session["WAMDemographicsSession"];
                        if (d != null)
                        {
                            v.Gender = d.Gender;                            
                            v.BirthYear = d.BirthYear;
                            v.IsCaregiverChild = d.IsCaregiverChild;
                        }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                //LogError(ex, Constants.PageName.Registration_Demographics, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }

        }

        [HttpPost]
        public async Task<ActionResult> WAMDemographics(RegisterWAMDemographicsVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        WAMDemographicsDto d = new WAMDemographicsDto();
                        d.ProfileId = v.ProfileId;
                        d.BirthYear = v.BirthYear;
                        d.Gender = v.Gender;       
                        d.IsCaregiverChild = v.IsCaregiverChild;

                        registrationService.SaveWAMDemographics(d);
                        Session["WAMDemographicsSession"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];                     
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = StatusRegistrationProgress.WAMIntentions.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 23) { profile.MaxStep = 23; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);                        
                        //await LogDemographicsOut(profileId);
                        return RedirectToAction("WAMRoster");
                    }
                    //await LogSessionError(PageName.Registration_Demographics);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    //LogError(ex, Constants.PageName.Registration_Demographics, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region V. WAM Intentions

        public async Task<ActionResult> WAMIntentions()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        //await LogSessionError(Constants.PageName.Registration_Intentions);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                //await LogProfileIntentionsIn(profileId);
                RegisterWAMIntentionsVM v = new RegisterWAMIntentionsVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (v.MaxStep > 21)
                {
                    if (Session["WAMIntentionsSession"] == null)
                    { Session["WAMIntentionsSession"] = registrationService.GetWAMIntentionsByProfileId(profileId); }

                    WAMIntentionsDto t = (WAMIntentionsDto) Session["WAMIntentionsSession"];
                    if (t != null)
                    {
                        v.SameEmployer = registrationService.ConvertToNumber(t.SameEmployer);
                        v.SameIndustry = registrationService.ConvertToNumber(t.SameIndustry);
                    }
                }
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                return View(v);
            }
            catch (Exception ex)
            {
                //LogError(ex, Constants.PageName.Registration_Intentions, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WAMIntentions(RegisterWAMIntentionsVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        var d = new WAMIntentionsDto();
                        d.ProfileId = profileId;
                        d.SameEmployer = registrationService.ConvertToDecimal(v.SameEmployer);
                        d.SameIndustry = registrationService.ConvertToDecimal(v.SameIndustry);

                        registrationService.SaveWAMIntentions(d);
                        Session["WAMIntentionsSession"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.WAMRoster.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 22) { profile.MaxStep = 22; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        //await LogProfileIntentionsOut(profileId);
                        return RedirectToAction("WAMDemographics");
                    }
                    //await LogSessionError(Constants.PageName.Registration_Intentions);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    //LogError(ex, Constants.PageName.Registration_Intentions, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region VI. WAM Roster
        /*
          The roster calendar uses Ajax to create, edit and Delete the events.
          If you goto the Roster View, the ajax calls are made 
          using the CalendarController's Get, SaveNewParam, SaveEditParam, Delete
             */
        public async Task<ActionResult> WAMRoster(string id)
        {
            try
            {
                bool containsNumbers = false; //URL checking

                if (!string.IsNullOrEmpty(id)) {
                    containsNumbers = id.Any(char.IsDigit); }

                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        await LogSessionError(Constants.PageName.Registration_Roster);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                RegisterRoster1ViewModel v = new RegisterRoster1ViewModel();
                int profileId = (int) Session["ProfileId"];

                await LogRosterIn(profileId);
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                v.ProfileName = profile.Name;
                v.ProfileEmail = profile.LoginEmail;
                v.ProfileOffset = profile.OffSetFromUTC;
                v.YearMonth = containsNumbers ? id : string.Empty;
                v.ClientInitials = profile.ClientInitials;
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WAMRoster(RegisterRoster1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Completed.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegisteredDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;
                        if (profile.MaxStep < 24)
                        {
                            profile.MaxStep = 24;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        await LogRosterOut(profileId);
                        return RedirectToAction("WAMFeedback");
                        //return null;
                    }
                    await LogSessionError(Constants.PageName.Registration_Roster);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }
        #endregion

        #region VII. WAM Feedback

        public async Task<ActionResult> WAMFeedback()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {
                        //await LogSessionError(Constants.PageName.Registration_Roster);
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                RegisterWAMFeedbackVM v = new RegisterWAMFeedbackVM();
                int profileId = (int) Session["ProfileId"];

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                v.ProfileName = profile.Name;
                v.ProfileEmail = profile.LoginEmail;
                v.ProfileOffset = profile.OffSetFromUTC;

                return View(v);
            }
            catch (Exception ex)
            {
                //LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Get);
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WAMFeedback(RegisterWAMFeedbackVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];

                        var d = new WAMFeedbackDto();
                        d.ProfileId = v.ProfileId;
                        d.Feedback = v.Feedback;

                        registrationService.SaveWAMFeedback(d);
                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.RegistrationProgressNext = StatusRegistrationProgress.Completed.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegisteredDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;
                        if (profile.MaxStep < 25)
                        { profile.MaxStep = 25; }

                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);
                        ClearSession();
                        return RedirectToAction("Completed");
                    }
                    //await LogSessionError(Constants.PageName.Registration_Roster);
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    //LogError(ex, Constants.PageName.Registration_Roster, Constants.PageAction.Post);
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }
        //[HttpGet]
        //public ActionResult WAMCompleted()
        //{           
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult WAMCompleted(string id)
        //{
        //    return RedirectToAction("WAMDemo");
        //}
        #endregion
        #endregion

    }
}