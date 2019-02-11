using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SANSurveyWebAPI.DTOs;
using System.Data.Entity;

namespace SANSurveyWebAPI.Controllers
{
    public class ExitV2Controller : Controller
    {
        private Survey3Service surveyService;
        private UserHomeService userHomeService;
        private ProfileService profileService;
        private ExitV2Service exitService;

        private ApplicationDbContext db = new ApplicationDbContext();

        public ExitV2Controller()
        {
            this.surveyService = new Survey3Service();
            this.userHomeService = new UserHomeService();
            this.profileService = new ProfileService();
            this.exitService = new ExitV2Service();
        }
        // GET: ExitV2
        public ActionResult Index()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            { }
            base.Dispose(disposing);
        }
        [HttpGet]
        public ActionResult ExitSurvey(string Id)
        {            
            ExitV2VM k = new ExitV2VM();
            k.CurrentDate = String.Format(format: "{0:dddd, d MMMM, yyyy}", arg0: DateTime.Today);
            
            return View(k);
        }
        //Here were generating the temporary user id and creating a kids survey for them for demo purposes.
        //Upon creation it will take user to New Kids Survey layout
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ExitSurvey()
        {
            try
            {
                string currentSurveyID = string.Empty;
                string demoForUser = "exit";

                if (ModelState.IsValid)
                {
                    #region I.Create Random Kids Profile
                    //Generate random email
                    string exitEmail = string.Empty;
                    Random randomGenerator = new Random();
                    int randomNumber = randomGenerator.Next(6999);
                    exitEmail = demoForUser + randomNumber + "@aut.ac.nz";
                    Session["ExitEmail"] = exitEmail;
                    Session["ExitPassword"] = "******";

                    Profile newProfile = new Profile();
                    //Genrate UID
                    Guid uid = Guid.NewGuid();

                    //Which Client
                    if (demoForUser == "exit")
                    {
                        newProfile.ClientInitials = "exitv2";
                        newProfile.ClientName = "Exit Survey Version 2";
                    }

                    //Encryption
                    newProfile.LoginEmail = StringCipher.EncryptRfc2898(exitEmail);

                    newProfile.CreatedDateTimeUtc = DateTime.UtcNow;
                    newProfile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Completed.ToString();
                    newProfile.UserId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;
                    newProfile.Uid = uid.ToString();
                    newProfile.Name = demoForUser + randomNumber;
                    newProfile.MaxStep = 10;
                    newProfile.OffSetFromUTC = 13;
                    newProfile.Incentive = 0;
                    newProfile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                    newProfile.RegisteredDateTimeUtc = DateTime.UtcNow;
                    db.Profiles.Add(newProfile);
                    await db.SaveChangesAsync();
                    db.Entry(newProfile).GetDatabaseValues(); //Get PROFILE

                    Session["ProfileId"] = newProfile.Id;
                    Session["UID"] = newProfile.Uid;
                    ViewBag.UserId = new SelectList(db.Users, "Id", "Email", newProfile.UserId);
                    //----End of Create Profile (will generate Profile ID)

                    #endregion

                    #region II. Generate Survey
                    //---------------------------------------------
                    //Previous Day Work Shift from 9 to 5
                    DateTime currentDate = DateTime.Now;
                    DateTime previousDate = currentDate.Date.AddDays(-1);
                    previousDate = previousDate.AddHours(9);
                    DateTime endDate = currentDate.Date.AddDays(-1);
                    endDate = endDate.Date.AddHours(18);

                    DateTime previousDateUTC = previousDate.AddHours(-13);
                    DateTime EndDateUTC = endDate.AddHours(-13);

                    ExitV2Survey e = new ExitV2Survey();
                    e.ProfileId = newProfile.Id;
                    e.Uid = newProfile.Uid;
                    e.SurveyProgress = Constants.ExitV2Status.NewSurvey.ToString();
                    e.SurveyDate = previousDate;
                    Session["ExitSurveyDate"] = previousDate;
                    e.EntryStartCurrent = DateTime.Now;
                    e.EntryStartUTC = DateTime.UtcNow;
                    db.ExitV2Surveys.Add(e);
                  
                    await db.SaveChangesAsync();

                    //db.Entry(e).GetDatabaseValues();
                    //Session["ExitV2SurveyId"] = e.Id;

                    #endregion                    
                }
                return RedirectToAction("ExitSignUp", "Account", new { uid = Session["UID"].ToString() });
            }
            catch (Exception ex)
            {
                string EMsg = "ExitSignUp:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        #region "NEW KIDS SURVEY SCREEN"

        [HttpGet]
        public async Task<ActionResult> NewSurvey(string uid)
        {
            try
            {
                int profileId = (int) Session["ProfileId"];                

                GetProfileSession();
                if (!string.IsNullOrEmpty(uid))
                {
                    var z = db.ExitV2Surveys.Where(u => u.Uid == uid
                                                    && u.ProfileId == profileId)
                                           .Select(u => u).FirstOrDefault();

                    Session["ExitV2SurveyId"] = z.Id;
                }
                else { Session["ExitV2SurveyId"] = ""; }
                return View();
            }
            catch (Exception ex)
            {
                string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> NewSurvey()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int profileId = (int) Session["ProfileId"];
                    int exitSurveyId = (int) Session["ExitV2SurveyId"];

                    InitializeWizard(Constants.ExitV2Status.WellBeing.ToString(), 1);                    

                    return RedirectToAction("WellBeing");
                }
                catch (Exception ex)
                {
                    string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        #endregion

        #region "WellBeing"
        //Page one
        [HttpGet]
        public async Task<ActionResult> WellBeing()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();                    
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                int exitSurveyId = (int) Session["ExitV2SurveyId"];

                WellbeingVM v = new WellbeingVM();
                v.QnsList = exitService.GetQuestionsListWellBeing();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId);  }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                var currList = exitService.GetExitSurveyAnsWellbeing(profile.Id, exitSurveyId);

                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

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
                string EMsg = "WellBeing GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WellBeing(WellbeingVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        int exitSurveyId = (int) Session["ExitV2SurveyId"];

                        await exitService.SaveWellBeing(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.FirstJob.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 2)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.FirstJob; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                       
                        return RedirectToAction("FirstJob");
                    }                   
                }
                catch (Exception ex)
                {
                    string EMsg = "WellBeing POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "FirstJob"

        [HttpGet]
        public async Task<ActionResult> FirstJob()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();                                       
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                int exitSurveyId = (int) Session["ExitV2SurveyId"];

                FirstJobVM v = new FirstJobVM();
                v.QnsList = exitService.GetQuestionsFirstJob();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId);}
                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetAnsFirstJob(profile.Id, exitSurveyId);

                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;                

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
                string EMsg = "FirstJob GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> FirstJob(FirstJobVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        int exitSurveyId = (int) Session["ExitV2SurveyId"];
                        await exitService.SaveFirstJob(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.FirstJob.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 3)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.SecondJob; }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);                       
                        return RedirectToAction("SecondJob");
                    }                   
                }
                catch (Exception ex)
                {
                    string EMsg = "FirstJob POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Second Job"

        [HttpGet]
        public async Task<ActionResult> SecondJob()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();                   
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                int exitSurveyId = (int) Session["ExitV2SurveyId"];

                SecondJobVM v = new SecondJobVM();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

                if (profile.MaxExitV2Step > 3)
                {
                    if (Session["ExitSurveySecondJob"] == null)
                    { Session["ExitSurveySecondJob"] = exitService.GetSecondJobById(profileId, exitSurveyId);  }

                    SecondJobDto c = (SecondJobDto) Session["ExitSurveySecondJob"];
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
                string EMsg = "SecondJob GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> SecondJob(SecondJobVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        int exitSurveyId = (int) Session["ExitV2SurveyId"];

                        var d = new SecondJobDto();
                        d.ProfileId = v.ProfileId;
                        d.Q1 = v.Q1;
                        d.Q2 = v.Q2;
                        d.ExitSurveyId = exitSurveyId;

                        await exitService.SaveSecondJob(d);
                        Session["ExitSurveySecondJob"] = d;
                        
                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.FirstWorkEnvironment.ToString();
                        profile.MaxExitV2Step = v.MaxExitV2Step;

                        if (profile.MaxExitV2Step < 4)
                        {   profile.MaxExitV2Step = (int) Constants.ExitV2Status.FirstWorkEnvironment; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        //return RedirectToAction("ThirdJob");
                        return RedirectToAction("FirstWorkEnvironment");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "SecondJob POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Third Job"

        //[HttpGet]
        //public async Task<ActionResult> ThirdJob()
        //{
        //    try
        //    {
        //        //TODO: populate from table
        //        if (Session["ProfileId"] == null)
        //        {
        //            var loggedInProfile = exitService.GetCurrentLoggedInProfile();                    
        //            Session["ProfileId"] = loggedInProfile.Id;
        //        }
        //        int profileId = (int) Session["ProfileId"];
        //        int exitSurveyId = (int) Session["ExitV2SurveyId"];

        //        ThirdJobVM v = new ThirdJobVM();
        //        v.QnsList = exitService.GetQuestionsThirdJob();

        //        if (Session["ProfileSession"] == null)
        //        {  Session["ProfileSession"] = exitService.GetProfileById(profileId); }

        //        ProfileDto profile = (ProfileDto) Session["ProfileSession"];

        //        var currList = exitService.GetAnsThirdJob(profile.Id, exitSurveyId);

        //        v.ProfileId = profile.Id;
        //        v.MaxExitV2Step = profile.MaxExitV2Step;

        //        if (currList != null && currList.Count() > 0)
        //        {
        //            foreach (var d in currList)
        //            {
        //                foreach (var c in v.QnsList)
        //                {
        //                    if (d.Name == c.Name)
        //                    {
        //                        c.Ans = d.Ans;
        //                    }
        //                }
        //            }
        //        }
        //        return View(v);
        //    }
        //    catch (Exception ex)
        //    {
        //        string EMsg = "ThirdJob GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
        //        await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
        //        return RedirectToAction("SurveyError");
        //    }
        //}
        //[HttpPost]
        //public async Task<ActionResult> ThirdJob(ThirdJobVM v)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (Session["ProfileId"] != null)
        //            {
        //                int profileId = (int) Session["ProfileId"];
        //                int exitSurveyId = (int) Session["ExitV2SurveyId"];
        //                await exitService.SaveThirdJob(profileId, exitSurveyId, v.QnsList);

        //                if (Session["ProfileSession"] == null)
        //                { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

        //                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
        //                profile.ExitSurveyProgressNext = Constants.ExitV2Status.FirstWorkEnvironment.ToString();
        //                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

        //                if (profile.MaxExitV2Step < 5)
        //                {
        //                    profile.MaxExitV2Step = (int)Constants.ExitV2Status.FirstWorkEnvironment;
        //                }
        //                Session["ProfileSession"] = profile;
        //                exitService.UpdateProfile(profile);                      
        //                return RedirectToAction("FirstWorkEnvironment");
        //            }                    
        //        }
        //        catch (Exception ex)
        //        {
        //            string EMsg = "ThirdJob POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
        //            await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
        //            return RedirectToAction("SurveyError");
        //        }
        //    }
        //    return View(v);
        //}

        #endregion

        #region "First Work Environment"

        [HttpGet]
        public async Task<ActionResult> FirstWorkEnvironment()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();                    
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int) Session["ProfileId"];
                int exitSurveyId = (int) Session["ExitV2SurveyId"];

                FirstWEVM v = new FirstWEVM();
                v.QnsList = exitService.GetQuestionsFirstWE();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];

                var currList = exitService.GetAnsFirstWE(profile.Id, exitSurveyId);

                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

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
                string EMsg = "FirstWorkEnvironment GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FirstWorkEnvironment(FirstWEVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int) Session["ProfileId"];
                        int exitSurveyId = (int) Session["ExitV2SurveyId"];
                        await exitService.SaveFirstWE(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.SecondWorkEnvironment.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 5)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.SecondWorkEnvironment; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                      
                        return RedirectToAction("SecondWorkEnvironment");
                    }          
                }
                catch (Exception ex)
                {
                    string EMsg = "FirstWorkEnvironment POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Second Work Environment"

        [HttpGet]
        public async Task<ActionResult> SecondWorkEnvironment()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    { return RedirectToAction("SessionError"); }

                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                SecondWEVM v = new SecondWEVM();
                v.QnsList = exitService.GetSecondWorkEnvironmentQns();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                var currList = exitService.GetSecondWorkEnvironmentAns(profile.Id);
                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

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
                string EMsg = "SecondWorkEnvironment GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SecondWorkEnvironment(SecondWEVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        await exitService.SaveSecondWE(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.ThirdWorkEnvironment.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.MaxExitV2Step = v.MaxExitV2Step;

                        if (profile.MaxExitV2Step < 6)
                        { profile.MaxExitV2Step = (int) Constants.ExitV2Status.ThirdWorkEnvironment; }

                            Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                      
                        return RedirectToAction("ThirdWorkEnvironment");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "SecondWorkEnvironment POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Third Work Environment"

        [HttpGet]
        public async Task<ActionResult> ThirdWorkEnvironment()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                ThirdWEVM v = new ThirdWEVM();
                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                }
                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

                if (profile.MaxStepExitSurvey > 6)
                {
                    if (Session["ExitSurveyThirdWE"] == null)
                    { Session["ExitSurveyThirdWE"] = exitService.GetThirdWEbyId(profileId, exitSurveyId); }

                    ThirdWorkEnvironmentDto c = (ThirdWorkEnvironmentDto)Session["ExitSurveyThirdWE"];
                    if (c != null)
                    {
                        v.Q1 = c.Q1;
                        v.Q1Other = c.Q1Other;
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "ThirdWorkEnvironment GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ThirdWorkEnvironment(ThirdWEVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        var d = new ThirdWorkEnvironmentDto();
                        d.ProfileId = v.ProfileId;
                        d.Q1 = v.Q1;
                        d.ExitSurveyId = exitSurveyId;

                        if (v.Q1 == "Other")
                        { d.Q1Other = v.Q1Other; }
                        else
                        { d.Q1Other = null; }

                        await exitService.SaveThirdWE(d);
                        Session["ExitSurveyThirdWE"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.FourthWorkEnvironment.ToString();
                        profile.MaxExitV2Step = v.MaxExitV2Step;

                        if (profile.MaxExitV2Step < 7)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.FourthWorkEnvironment; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        return RedirectToAction("FourthWorkEnvironment");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "ThirdWorkEnvironment POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Fourth Work Environment"

        [HttpGet]
        public async Task<ActionResult> FourthWorkEnvironment()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                FourthWEVM v = new FourthWEVM();
                v.QnsList = exitService.GetQuestionsFourthWE();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                var currList = exitService.GetAnsFourthWE(profile.Id, exitSurveyId);

                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

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
                string EMsg = "FourthWorkEnvironment GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> FourthWorkEnvironment(FourthWEVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        await exitService.SaveFourthWE(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.FifthWorkEnvironment.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 8)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.FifthWorkEnvironment; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        return RedirectToAction("FifthWorkEnvironment");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "FourthWorkEnvironment POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Fifth Work Environment"

        [HttpGet]
        public async Task<ActionResult> FifthWorkEnvironment()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    { return RedirectToAction("SessionError"); }

                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                FifthWEVM v = new FifthWEVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

                if (profile.MaxExitV2Step > 8)
                {
                    if (Session["ExitSurveyFifthWE"] == null)
                    {
                        Session["ExitSurveyFifthWE"] = exitService.GetFifthWEById(profileId, exitSurveyId); ;
                    }
                    FifthWorkEnvironmentDto c = (FifthWorkEnvironmentDto)Session["ExitSurveyFifthWE"];
                    if (c != null)
                    {
                        v.Q1 = c.Q1;
                        v.Q2 = c.Q2;

                        if (!string.IsNullOrEmpty(c.Q3))
                        { v.Q3 = int.Parse(c.Q3); }
                        else
                        {  v.Q3 = 0; }

                        v.Q4 = c.Q4;

                        if (!string.IsNullOrEmpty(c.Q5))
                        {  v.Q5 = int.Parse(c.Q5); }
                        else
                        {  v.Q5 = 0;  }

                        if (!string.IsNullOrEmpty(c.Q6))
                        { v.Q6 = int.Parse(c.Q6);  }
                        else
                        {  v.Q6 = 0; }

                        if (!string.IsNullOrEmpty(c.Q7))
                        {  v.Q7 = int.Parse(c.Q7); }
                        else
                        {  v.Q7 = 0; }

                        if (!string.IsNullOrEmpty(c.Q8))
                        {  v.Q8 = int.Parse(c.Q8);  }
                        else
                        { v.Q8 = 0;  }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "FifthWorkEnvironment GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> FifthWorkEnvironment(FifthWEVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        var d = new FifthWorkEnvironmentDto();

                        d.ProfileId = v.ProfileId;
                        d.ExitSurveyId = exitSurveyId;
                        d.Q1 = v.Q1;
                        d.Q2 = v.Q2;
                        d.Q4 = v.Q4; //Yes or No (knowing senior)
                        d.Q3 = v.Q3.ToString(); //How long if Yes
                        d.Q5 = v.Q5.ToString();
                        d.Q6 = v.Q6.ToString();
                        d.Q7 = v.Q7.ToString();
                        d.Q8 = v.Q8.ToString();


                        await exitService.SaveFifthWE(d);
                        Session["ExitSurveyFifthWE"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }
                        
                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.FirstTraining.ToString();
                        profile.MaxExitV2Step = v.MaxExitV2Step;

                        if (profile.MaxExitV2Step < 9)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.FirstTraining;  }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        return RedirectToAction("FirstTraining");
                    }                    
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "FifthWorkEnvironment POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "First Training"

        [HttpGet]
        public async Task<ActionResult> FirstTraining()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                FirstTraining2VM v = new FirstTraining2VM();
                v.QnsYourTraining = exitService.GetQnsFirstTrainingYourTraining();
                v.QnsFeelingValued = exitService.GetQnsFirstTrainingFeelingValued();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                var ytList = exitService.GetFirstTrainingYourTrainingAns(profileId, exitSurveyId);
                var fvList = exitService.GetFirstTrainingFeelingValued(profileId, exitSurveyId);

                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

                if (ytList != null && ytList.Count() > 0)
                {
                    foreach (var d in ytList)
                    {
                        foreach (var c in v.QnsYourTraining)
                        {
                            if (d.Name == c.Name)
                            {
                                c.Ans = d.Ans;
                            }
                        }
                    }
                }

                if (fvList != null && fvList.Count() > 0)
                {
                    foreach (var d in fvList)
                    {
                        foreach (var c in v.QnsFeelingValued)
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
                string EMsg = "FirstTraining GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> FirstTraining(FirstTraining2VM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        List<QnAn> combinedList = v.QnsYourTraining.Concat(v.QnsFeelingValued).ToList();

                        await exitService.SaveFirstTraining(profileId, exitSurveyId, combinedList);

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.SecondTraining.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 10)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.SecondTraining; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        return RedirectToAction("SecondTraining");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "FirstTraining POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }



        //[HttpGet]
        //public async Task<ActionResult> FirstTraining_old()
        //{
        //    try
        //    {
        //        if (Session["ProfileId"] == null)
        //        {
        //            var loggedInProfile = exitService.GetCurrentLoggedInProfile();
        //            if (loggedInProfile == null)
        //            { return RedirectToAction("SessionError");}

        //            Session["ProfileId"] = loggedInProfile.Id;
        //        }
        //        int profileId = (int)Session["ProfileId"];
        //        int exitSurveyId = (int)Session["ExitV2SurveyId"];

        //        FirstTrainingVM v = new FirstTrainingVM();
        //        v.QptionsList = exitService.GetOptionsFirstTraining();

        //        if (Session["ProfileSession"] == null)
        //        {  Session["ProfileSession"] = exitService.GetProfileById(profileId); }

        //        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
        //        var currList = exitService.GetFirstTrainingOptions(profile.Id, exitSurveyId);

        //        v.ProfileId = profile.Id;
        //        v.MaxExitV2Step = profile.MaxExitV2Step;

        //        if (currList != null && currList.Count() > 0)
        //        {
        //            foreach (var d in currList)
        //            {
        //                foreach (var c in v.QptionsList)
        //                {
        //                    if (d.ID == c.ID)
        //                    {
        //                        if (c.ID <= 7)
        //                        { v.HiddenTrainingOptionIds = c.ID + "," + v.HiddenTrainingOptionIds; }
        //                        else
        //                        {  v.HiddenValuedOptionIds = c.ID + "," + v.HiddenValuedOptionIds;  }

        //                        if (c.ID == 7) { v.TrainingOther = d.Ans; }
        //                        else if (c.ID == 14) { v.ValuedOther = d.Ans; }
        //                    }
        //                }
        //            }
        //        }
        //        return View(v);
        //    }
        //    catch (Exception ex)
        //    {
        //        string EMsg = "FirstTraining GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
        //        await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
        //        return RedirectToAction("SurveyError");
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult> FirstTraining_old(FirstTrainingVM v)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (Session["ProfileId"] != null)
        //            {
        //                int profileId = (int)Session["ProfileId"];
        //                int exitSurveyId = (int)Session["ExitV2SurveyId"];

        //                IList<string> TrainingIds = v.HiddenTrainingOptionIds.Split(',').Reverse().ToList<string>();

        //                IList<string> ValuedIds = v.HiddenValuedOptionIds.Split(',').Reverse().ToList<string>();

        //                IList<string> allIds = TrainingIds.Concat(ValuedIds).ToList();

        //                FirstTrainingVM AllOList = new FirstTrainingVM();
        //                AllOList.QptionsList = exitService.GetOptionsFirstTraining();

        //                List<QuestionAnswer> selectedList = new List<QuestionAnswer>();

        //                foreach (var id in allIds)
        //                {
        //                    int i = Convert.ToInt32(id);

        //                    selectedList.Add(AllOList.QptionsList[i]);
        //                    if (i == 7)
        //                    {  selectedList[selectedList.Count() - 1].LongName = v.TrainingOther; }
        //                    else if (i == 14)
        //                    { selectedList[selectedList.Count() - 1].LongName = v.ValuedOther; }
        //                }

        //                await exitService.SaveFirstTraining(profileId, exitSurveyId, selectedList);

        //                if (Session["ProfileSession"] == null)
        //                {  Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

        //                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
        //                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
        //                profile.ExitSurveyProgressNext = Constants.ExitV2Status.SecondTraining.ToString();
        //                profile.MaxExitV2Step = v.MaxExitV2Step;

        //                if (profile.MaxExitV2Step < 13)
        //                { profile.MaxExitV2Step = (int)Constants.ExitV2Status.SecondTraining; }

        //                Session["ProfileSession"] = profile;
        //                exitService.UpdateProfile(profile);
                        
        //                return RedirectToAction("SecondTraining");
        //            }
        //            return RedirectToAction("SessionError");
        //        }
        //        catch (Exception ex)
        //        {
        //            string EMsg = "FirstTraining POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
        //            await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
        //            return RedirectToAction("SurveyError");
        //        }
        //    }
        //    return View(v);
        //}

        #endregion

        #region "Second Training"

        [HttpGet]
        public async Task<ActionResult> SecondTraining()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    { return RedirectToAction("SessionError"); }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                SecondTrainingVM v = new SecondTrainingVM();
                v.QnsList = exitService.GetSecondTrainingQuestions();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                var currList = exitService.GetSecondTrainingAns(profile.Id, exitSurveyId);

                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

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
                string EMsg = "SecondTraining GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> SecondTraining(SecondTrainingVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {

                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        await exitService.SaveSecondTraining(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        {  Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.ThirdTraining.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 11)
                        {  profile.MaxExitV2Step = (int)Constants.ExitV2Status.ThirdTraining; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);

                        return RedirectToAction("ThirdTraining");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "SecondTraining POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "Third Training"

        [HttpGet]
        public async Task<ActionResult> ThirdTraining()
        {
            try
            {
                //TODO: populate from table
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {  return RedirectToAction("SessionError"); }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                ThirdTrainingVM v = new ThirdTrainingVM();
                v.QnsList = exitService.GetThirdTrainingQuestions();

                if (Session["ProfileSession"] == null)
                {  Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                var currList = exitService.GetThirdTrainingAns(profile.Id, exitSurveyId);
                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

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
                string EMsg = "ThirdTraining GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ThirdTraining(ThirdTrainingVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        await exitService.SaveThirdTraining(profileId, exitSurveyId, v.QnsList);

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.AboutYou.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxExitV2Step < 12)
                        {  profile.MaxExitV2Step = (int)Constants.ExitV2Status.AboutYou; }

                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile); 

                        return RedirectToAction("AboutYou");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "ThirdTraining POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }

        #endregion

        #region "About You"

        [HttpGet]
        public async Task<ActionResult> AboutYou()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = exitService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    { return RedirectToAction("SessionError"); }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
                int exitSurveyId = (int)Session["ExitV2SurveyId"];

                AboutYouESVM v = new AboutYouESVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxExitV2Step = profile.MaxExitV2Step;

                if (profile.MaxExitV2Step >= 13)
                {
                    if (Session["ExitSurveyAboutYou"] == null)
                    { Session["ExitSurveyAboutYou"] = exitService.GetAboutYouById(profileId, exitSurveyId); }

                    AboutYouESDto c = (AboutYouESDto)Session["ExitSurveyAboutYou"];
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
                string EMsg = "AboutYou GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> AboutYou(AboutYouESVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        int exitSurveyId = (int)Session["ExitV2SurveyId"];

                        var d = new AboutYouESDto();
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
                        await exitService.SaveAboutYouES(d);
                        Session["ExitSurveyAboutYou"] = d;


                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = exitService.GetProfileById(v.ProfileId);
                        }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ExitSurveyProgressNext = Constants.ExitV2Status.AboutYou.ToString();
                        profile.MaxExitV2Step = v.MaxExitV2Step;

                        if (profile.MaxExitV2Step < 16)
                        { profile.MaxExitV2Step = (int)Constants.ExitV2Status.AboutYou; }
                        Session["ProfileSession"] = profile;
                        exitService.UpdateProfile(profile);
                        ClearFields();
                        return RedirectToAction("ExitSurvey");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "AboutYou POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);
        }
        #endregion

        #region "FeedBack"


        #endregion

        #region "Method Calls"

        public void GetProfileSession()
        {
            try
            {
                int profileId = (int) Session["ProfileId"];
                ExitV2ProfileUpdatesVM p = new ExitV2ProfileUpdatesVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = exitService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto) Session["ProfileSession"];
                profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                p.ProfileId = profileId;
                p.MaxExitV2Step = profile.MaxExitV2Step;
            }
            catch (Exception ex)
            {
                string EMsg = "GetProfileSession:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
            }
        }
        public async void InitializeWizard(string progress, int maxStep)
        {
            try
            {
                int profileId = (int) Session["ProfileId"];
                int exitSurveyId = (int) Session["ExitV2SurveyId"];

                //for setting Wizard values and status update

                var exitProfile = exitService.GetExitProfile(profileId);
                exitProfile.MaxExitV2Step = maxStep;
                exitProfile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                exitProfile.ExitSurveyProgressNext = progress;
                exitService.UpdateExitProfile(exitProfile);

                var exitSurveyUpdate = exitService.GetExitSurvey(exitSurveyId);
                exitSurveyUpdate.SurveyProgress = progress;
                exitService.UpdateExitSurvey(exitSurveyUpdate);
                //end of Wizard update
            }
            catch (Exception ex)
            {
                string EMsg = "InitializeWizard:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
            }
        }

        public void ClearFields()
        {
            Session["ExitEmail"] = null;
            Session["ExitPassword"] = null;
            Session["ProfileId"] = null;
            Session["UID"] = null;
            Session["ExitSurveyDate"] = null;
            Session["ExitV2SurveyId"] = null;
            Session["ProfileSession"] = null;
            Session["ExitSurveySecondJob"] = null;
            Session["ExitSurveyThirdWE"] = null;
            Session["ExitSurveyFifthWE"] = null;
            Session["ExitSurveyAboutYou"] = null;
        }

        #endregion
        

        #region "LOG ERRORS TO DB"

        private async Task LogMyDayError(string uid, string errorMsg, string action)
        {
            var mydayVM = new MyDayErrorLogViewModel();

            var mydayDto = new MyDayErrorLogs_Dto();
            if (Session["ProfileId"].ToString() != string.Empty || Session["ProfileId"].ToString() != null)
            { mydayDto.ProfileId = Convert.ToInt32(Session["ProfileId"].ToString()); }

            mydayDto.SurveyUID = uid.ToString();
            mydayDto.AccessedDateTime = DateTime.Now;
            mydayDto.ErrorMessage = errorMsg;
            mydayDto.HtmlContent = action;

            if (mydayDto != null)
            {
                mydayVM.SurveyUID = mydayDto.SurveyUID;
                mydayVM.AccessedDateTime = mydayDto.AccessedDateTime;
                mydayVM.ErrorMessage = mydayDto.ErrorMessage;
                mydayVM.HtmlContent = mydayDto.HtmlContent;
            }
            await surveyService.Save_MyDayErrorLogs(mydayDto);
        }
        [HttpGet]
        public ActionResult SurveyError()
        {
            if (Request.IsAjaxRequest())
            { return PartialView(); }
            return View();
        }
        [HttpPost]
        public ActionResult SurveyError(SurveyError v)
        {
            if (Request.IsAjaxRequest())
            { return RedirectToAction("Index", "Home"); }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}