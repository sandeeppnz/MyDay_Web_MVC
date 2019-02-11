using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;

namespace SANSurveyWebAPI.Controllers
{
    [Authorize]
    public class CaseWorkersRegisterController : BaseController
    {

        private CaseWorkersService registrationService;
        private ApplicationDbContext db = new ApplicationDbContext();

        public CaseWorkersRegisterController()
        {
            this.registrationService = new CaseWorkersService();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            { }
            base.Dispose(disposing);
        }

        // GET: SocialWorkersRegister
        //public ActionResult Index()
        //{
        //    return View();
        //}

        #region "RANDOM USER GENERATION"

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Welcome(string id)
        { return View(); }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Welcome()
        {
            if (ModelState.IsValid)
            {
                //Creating Random User Id and password for demo purposes
                string caseworkers = "caseworkers";
                #region "Random User"

                string UserEmailGenerator = string.Empty;
                Random randomGenerator = new Random();
                int randomNumber = randomGenerator.Next(6999);
                UserEmailGenerator = caseworkers.ToString() + randomNumber + "@aut.ac.nz";
                Session["CaseWorkersEmail"] = UserEmailGenerator;
                Session["CaseWorkersPwd"] = "******";

                Profile newProfile = new Profile();
                //Generating Unique Indentifier: UID
                Guid uId = Guid.NewGuid();

                //Which Client
                
                newProfile.ClientInitials = "CW";
                newProfile.ClientName = "Case Workers";
               

                //Encryption of Email
                newProfile.LoginEmail = StringCipher.EncryptRfc2898(UserEmailGenerator);

                newProfile.CreatedDateTimeUtc = DateTime.UtcNow;
                newProfile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.New.ToString();
                newProfile.UserId = db.Users.Where(x => x.Email == Constants.AdminEmail.ToString()).SingleOrDefault().Id;
                newProfile.Uid = uId.ToString();
                newProfile.Name = caseworkers.ToString() + randomNumber.ToString();
                newProfile.MaxStep = 0;
                newProfile.OffSetFromUTC = 13;
                newProfile.Incentive = 0;
                newProfile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                newProfile.RegisteredDateTimeUtc = DateTime.UtcNow;
                db.Profiles.Add(newProfile);
                await db.SaveChangesAsync();

                //To get the created profile id upon above code execution
                db.Entry(newProfile).GetDatabaseValues();
                Session["ProfileId"] = newProfile.Id;
                Session["UiD"] = newProfile.Uid;
                Session["CaseWorkersUserId"] = newProfile.UserId;
                ViewBag.UserId = new SelectList(db.Users, "Id", "Email", newProfile.UserId);
                //end of Profile creation (will generate Profile ID)

                #endregion 
                
            }

            return RedirectToAction("CWSignUp", "Account", new { uid = Session["UiD"].ToString() });
        }
        #endregion

        #region Subjective Wellbeing

        [HttpGet]
        public async Task<ActionResult> WellBeing()
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
                CaseWellBeingVM v = new CaseWellBeingVM();
                int profileId = (int)Session["ProfileId"];

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                if (profile.MaxStep > 1)
                {
                    if (Session["CaseWellBeing"] == null)
                    { Session["CaseWellBeing"] = registrationService.GetSubjectiveWellBeingByProfileId(profileId); }

                    SWSubjectiveWellBeingDto w = (SWSubjectiveWellBeingDto)Session["CaseWellBeing"];

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
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WellBeing(CaseWellBeingVM v)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Session["ProfileId"] != null)
                    {
                        var d = new SWSubjectiveWellBeingDto();

                        d.ProfileId = v.ProfileId;
                        d.SwbLife = Constants.GetText5ScaleRating(v.Q1Ans);
                        d.SwbHome = Constants.GetText5ScaleRating(v.Q2Ans);
                        d.SwbJob = Constants.GetText5ScaleRating(v.Q3Ans);

                        await registrationService.SaveSWSubjectiveWellbeing(d);
                        Session["CaseWellBeing"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.RoleAtCurrentWorkPlace.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStep < 2)
                        { profile.MaxStep = 2; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        int profileId = (int)Session["ProfileId"];
                        return RedirectToAction("CurrentWorkplace");
                    }
                    return RedirectToAction("SessionError");                   
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }

        #endregion

        #region Current Workplace

        public async Task<ActionResult> CurrentWorkplace()
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
                int profileId = (int)Session["ProfileId"];

                CurrentWorkplaceVM v = new CurrentWorkplaceVM
                {
                    QptionsList = registrationService.GetOptionsforCurrentWorkplace(),                    
                };

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                var currList = registrationService.GetCurrentWorkplace(profile.Id);
                v.ProfileId = profile.Id;
                v.TaskType = profile.ProfileTaskType;
                v.MaxStep = profile.MaxStep;

                if (currList != null && currList.Count() > 0)
                {
                    foreach (var d in currList)
                    {
                        foreach (var c in v.QptionsList)
                        {
                            if (d.ID == c.ID)
                            {
                               
                                v.HiddenWorkHoursOptionIds = c.ID + "," + v.HiddenWorkHoursOptionIds;                                                                     
                            }
                        }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> CurrentWorkplace(CurrentWorkplaceVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];

                        //await registrationService.SaveCurrentWorkplace(v.ProfileId, v.QptionsList);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }
                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.CaseLoad.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        
                        if (profile.MaxStep < 3)
                        { profile.MaxStep = 3; }

                        Session["ProfileSession"] = profile;

                        #region WithWHom

                        IList<string> OptionIds = v.HiddenWorkHoursOptionIds.Split(',').Reverse().ToList<string>();
                        CurrentWorkplaceVM getAllOptions = new CurrentWorkplaceVM();
                        getAllOptions.QptionsList = registrationService.GetOptionsforCurrentWorkplace();
                        List<CaseTasksQA> selectedList = new List<CaseTasksQA>();
                        foreach (var id in OptionIds)
                        {
                            int i = Convert.ToInt32(id);
                            selectedList.Add(getAllOptions.QptionsList[i]);                            
                        }
                       await registrationService.SaveCurrentWorkplace(profileId, selectedList);

                        #endregion


                        registrationService.UpdateProfile(profile);

                        return RedirectToAction("CurrentWorkplaceContd");
                    }                  
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region Current Workplace Contd

        public async Task<ActionResult> CurrentWorkplaceContd()
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
                int profileId = (int)Session["ProfileId"];

                CurrentWorkplaceContdVM v = new CurrentWorkplaceContdVM();
                

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                var currList = registrationService.GetCurrentWorkplaceContd(profile.Id);
                v.ProfileId = profile.Id;
                if (currList != null)
                {
                    v.WorkStatus = currList.WorkStatus;
                    v.WorkPosition = currList.WorkPosition;
                    v.WorkCountry = currList.WorkCountry;
                    v.OtherWorkCountry = currList.OtherWorkCountry;
                }
                v.MaxStep = profile.MaxStep;
                
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> CurrentWorkplaceContd(CurrentWorkplaceContdVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }
                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.CaseWorkersTask.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStep < 4)
                        { profile.MaxStep = 4; }

                        Session["ProfileSession"] = profile;

                        CurrentWorkPlaceContdDto p = new CurrentWorkPlaceContdDto();
                        p.ProfileId = profileId;
                        p.WorkStatus = v.WorkStatus;
                        p.WorkPosition = v.WorkPosition;
                        p.WorkCountry = v.WorkCountry;
                        p.OtherWorkCountry = v.OtherWorkCountry;

                        await registrationService.SaveCurrentWorkPlaceContd(p);

                        registrationService.UpdateProfile(profile);

                        return RedirectToAction("Task");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        public async Task<ActionResult> Task()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();

                    if (loggedInProfile == null)
                    {
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                int profileId = (int)Session["ProfileId"];
                
                CaseWorkersTaskVM v = new CaseWorkersTaskVM
                {
                    FullTaskList = registrationService.GetTasksItemsTableListByType("CaseWorkers")
                };

                if (Session["ProfileSession"] == null)
                {
                    Session["ProfileSession"] = registrationService.GetProfileById(profileId);
                }
                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                var currTaskProfileList = registrationService.GetProfileTaskItemTableListByProfileId(profile.Id);
                v.ProfileId = profile.Id;
                v.TaskType = profile.ProfileTaskType;
                v.MaxStep = profile.MaxStep;

                if (currTaskProfileList != null && currTaskProfileList.Count() > 0)
                {
                    foreach (var d in currTaskProfileList)
                    {
                        foreach (var c in v.FullTaskList)
                        {
                            if (d.ID == c.ID)
                            {
                                c.Frequency = d.Frequency;
                            }
                        }
                    }
                }
                return View(v);

            }
            catch (Exception ex)
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Task(CaseWorkersTaskVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];

                        var currDbDefaultTasks = registrationService.GetProfileTaskItemsByProfileId(v.ProfileId);

                        if (Session["ProfileSession"] == null)
                        {
                            Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId);
                        }
                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.CaseLoad.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.ProfileTaskType = "Yes"; //v.TaskType;

                        if (profile.MaxStep < 3)
                        {
                            profile.MaxStep = 3;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        await registrationService.SaveDefaultTaskAsync(v.ProfileId, v.FullTaskList);                      

                        return RedirectToAction("CaseLoad");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #region Case Load

        [HttpGet]
        public async Task<ActionResult> CaseLoad()
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
                int profileId = (int)Session["ProfileId"];
               
                CaseLoadVM v = new CaseLoadVM();
               
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (Session["CaseLoad"] == null)
                { Session["CaseLoad"] = registrationService.GetCaseLoadOptionsByProfileId(profile.Id); }

                CaseLoadDto c = (CaseLoadDto)Session["CaseLoad"];
                if (c != null)
                {
                    v.Q1 = c.Option;
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }


        [HttpPost]
        public async Task<ActionResult> CaseLoad(CaseLoadVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];

                        var d = new CaseLoadDto();
                        d.ProfileId = v.ProfileId;
                        d.Option = v.Q1;

                        await registrationService.SaveCaseLoad(d);
                        Session["CaseLoad"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.TimeAllocation.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 5)
                        { profile.MaxStep = (int)Constants.CaseWorkersRegistrationStatus.TimeAllocation; }

                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                    }
                    return RedirectToAction("TimeAllocation");                
                }
                catch (Exception ex)
                {
                    string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region Time Allocation

        public async Task<ActionResult> TimeAllocation()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    if (loggedInProfile == null)
                    {  return RedirectToAction("SessionError"); }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];

                TimeAllocationVM v = new TimeAllocationVM();
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (v.MaxStep > 5)
                {
                    if (Session["TimeAllocation"] == null)
                    { Session["TimeAllocation"] = registrationService.GetTimeAllocatedByProfileId(profileId); }

                    TimeAllocationDto t = (TimeAllocationDto)Session["TimeAllocation"];

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
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> TimeAllocation(TimeAllocationVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        var d = new TimeAllocationDto();
                        d.ProfileId = v.ProfileId;
                        d.ClinicalActualTime = registrationService.ConvertToDecimal(v.ClinicalActualTime);
                        d.ClinicalDesiredTime = registrationService.ConvertToDecimal(v.ClinicalDesiredTime);

                        d.ResearchActualTime = registrationService.ConvertToDecimal(v.ResearchActualTime);
                        d.ResearchDesiredTime = registrationService.ConvertToDecimal(v.ResearchDesiredTime);

                        d.TeachingLearningActualTime = registrationService.ConvertToDecimal(v.TeachingLearningActualTime);
                        d.TeachingLearningDesiredTime = registrationService.ConvertToDecimal(v.TeachingLearningDesiredTime);

                        d.AdminActualTime = registrationService.ConvertToDecimal(v.AdminActualTime);
                        d.AdminDesiredTime = registrationService.ConvertToDecimal(v.AdminDesiredTime);

                        registrationService.SaveTimeAllocation(d);
                        Session["TimeAllocation"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.DemoGraphics.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 6) { profile.MaxStep = 6; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        return RedirectToAction("Demographics");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
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
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];
               
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                DemographicsVM v = new DemographicsVM
                {
                    BirthYearList = registrationService.GetAllBirthYears(),
                };

                if (profile != null)
                {
                    v.MaxStep = profile.MaxStep;
                    v.ProfileId = profile.Id;
                    if (v.MaxStep > 6)
                    {
                        if (Session["Demographics"] == null)
                        { Session["Demographics"] = registrationService.GetCWDemographicsByProfileId(profileId); }
                        DemographicsDto d = (DemographicsDto)Session["Demographics"];
                        if (d != null)
                        {
                            v.Gender = d.Gender;
                            v.BirthYear = d.BirthYear;
                            v.MaritalStatus = d.MaritalStatus;
                            v.IsCaregiverChild = d.IsCaregiverChild;
                            v.IsCaregiverAdult = d.IsCaregiverAdult;
                            v.EthnicityOrRace = d.EthnicityOrRace;
                        }
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Demographics(DemographicsVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];

                        DemographicsDto d = new DemographicsDto();
                        d.ProfileId = v.ProfileId;
                        d.BirthYear = v.BirthYear;
                        d.Gender = v.Gender;
                        d.MaritalStatus = v.MaritalStatus;
                        d.IsCaregiverChild = v.IsCaregiverChild;
                        d.IsCaregiverAdult = v.IsCaregiverAdult;
                        d.EthnicityOrRace = v.EthnicityOrRace;

                        registrationService.SaveCWDemographics(d);
                        Session["Demographics"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.EducationBackground.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 7) { profile.MaxStep = 7; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);
                        
                        return RedirectToAction("EducationBackground");
                    }
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "CaseWorkers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }

        #endregion

        #region Education Background

        [HttpGet]
        public async Task<ActionResult> EducationBackground()
        {
            try
            {
                if (Session["ProfileId"] == null)
                {
                    var loggedInProfile = registrationService.GetCurrentLoggedInProfile();
                    Session["ProfileId"] = loggedInProfile.Id;
                }
                int profileId = (int)Session["ProfileId"];

                EducationBackgroundVM v = new EducationBackgroundVM();

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;

                if (profile.MaxStep > 7)
                {
                    if (Session["EducationBackground"] == null)
                    { Session["EducationBackground"] = registrationService.GetEducationBackgroundById(profileId); }

                    EducationBackgroundDto c = (EducationBackgroundDto)Session["EducationBackground"];

                    if (c != null)
                    {
                        v.BachelorsDegree = c.BachelorsDegree;
                        v.MastersDegree = c.MasterDegree;
                        v.PreServiceTraining = c.PreServiceTraining;
                    }
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "EducationBackground GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EducationBackground(EducationBackgroundVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];

                        var d = new EducationBackgroundDto();
                        d.ProfileId = v.ProfileId;
                        d.BachelorsDegree = v.BachelorsDegree;
                        d.MasterDegree = v.MastersDegree;
                        d.PreServiceTraining = v.PreServiceTraining;

                        await registrationService.SaveEducationBackground(d);
                        Session["EducationBackground"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.JobIntentions.ToString();
                        profile.MaxStep = v.MaxStep;

                        if (profile.MaxStep < 8)
                        { profile.MaxStep = (int)Constants.CaseWorkersRegistrationStatus.JobIntentions; }

                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        return RedirectToAction("JobIntentions");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "EducationBackground POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            return View(v);            
        }

        #endregion

        #region Job Intentions
        [HttpGet]
        public async Task<ActionResult> JobIntentions()
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
                JobIntentionsVM v = new JobIntentionsVM();
                int profileId = (int)Session["ProfileId"];

                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];

                if (profile.MaxStep > 8)
                {
                    if (Session["JobIntentions"] == null)
                    { Session["JobIntentions"] = registrationService.GetJobIntentionsByProfileId(profileId); }

                    JobIntentionsDto w = (JobIntentionsDto)Session["JobIntentions"];

                    if (!string.IsNullOrEmpty(w.CurrentWorkplace)) v.Q1Ans = Constants.GetInt5LikelyScaleRating(w.CurrentWorkplace);
                    else v.Q1Ans = Constants.NA_5Rating;

                    if (!string.IsNullOrEmpty(w.CurrentIndustry)) v.Q2Ans = Constants.GetInt5LikelyScaleRating(w.CurrentIndustry);
                    else v.Q2Ans = Constants.NA_5Rating;                  
                }
                else
                {
                    v.Q1Ans = Constants.NA_5Rating;
                    v.Q2Ans = Constants.NA_5Rating;
                }

                v.ProfileId = profile.Id;
                v.MaxStep = profile.MaxStep;
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "JobIntentions Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> JobIntentions(JobIntentionsVM v)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Session["ProfileId"] != null)
                    {
                        var d = new JobIntentionsDto();

                        d.ProfileId = v.ProfileId;
                        d.CurrentWorkplace = Constants.GetText5LikelyScaleRating(v.Q1Ans);
                        d.CurrentIndustry = Constants.GetText5LikelyScaleRating(v.Q2Ans);

                        await registrationService.SaveJobIntentions(d);
                        Session["JobIntentions"] = d;

                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.SocialWorkersRoster.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;

                        if (profile.MaxStep < 9)
                        { profile.MaxStep = (int)Constants.CaseWorkersRegistrationStatus.SocialWorkersRoster; }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);

                        int profileId = (int)Session["ProfileId"];
                        return RedirectToAction("Roster");
                    }
                    return RedirectToAction("SessionError");
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "JobIntentions Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
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
                        return RedirectToAction("SessionError");
                    }
                    Session["ProfileId"] = loggedInProfile.Id;
                }

                RosterVM v = new RosterVM();
                int profileId = (int)Session["ProfileId"];

                
                if (Session["ProfileSession"] == null)
                { Session["ProfileSession"] = registrationService.GetProfileById(profileId); }

                ProfileDto profile = (ProfileDto)Session["ProfileSession"];
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
                string EMsg = "Roster Case Workers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                return RedirectToAction("RegistrationError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Roster(RosterVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Session["ProfileId"] != null)
                    {
                        int profileId = (int)Session["ProfileId"];
                        if (Session["ProfileSession"] == null)
                        { Session["ProfileSession"] = registrationService.GetProfileById(v.ProfileId); }

                        ProfileDto profile = (ProfileDto)Session["ProfileSession"];
                        profile.RegistrationProgressNext = Constants.CaseWorkersRegistrationStatus.SocialWorkersFeedback.ToString();
                        profile.LastUpdatedDateTimeUtc = DateTime.UtcNow;
                        profile.RegisteredDateTimeUtc = DateTime.UtcNow;
                        profile.MaxStep = v.MaxStep;
                        if (profile.MaxStep < 10)
                        {
                            profile.MaxStep = 10;
                        }
                        Session["ProfileSession"] = profile;
                        registrationService.UpdateProfile(profile);
                      
                        return RedirectToAction("Feedback");                       
                    }                    
                    return RedirectToAction("SessionError");
                }
                catch (Exception ex)
                {
                    string EMsg = "Roster Case Workers Registration:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "CaseWorkersRegistrationError");
                    return RedirectToAction("RegistrationError");
                }
            }
            return View(v);
        }
        public async Task<JsonResult> RosterValidate()
        {
            if (Session["ProfileId"] != null)
            {
                int profileId = (int)Session["ProfileId"];

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

        #region FeedBack

        public async Task<ActionResult> Feedback()
        {
            try
            {
                if (Session["ProfileId"] != null)
                {                    
                    int profileId = (int)Session["ProfileId"];
                   
                    FeedbackVM v = new FeedbackVM();

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Case Workers Feedback GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Feedback(FeedbackVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int profileId = (int)Session["ProfileId"];

                    CaseWorkersFeedbackDto f = new CaseWorkersFeedbackDto();
                    f.ProfileId = profileId;                   
                    f.Comments = v.Comment;
                    registrationService.AddCaseWorkersFeedback(f);
                    registrationService.SaveCaseWorkersFeedback();

                    return RedirectToAction("Welcome");
                }

                catch (Exception ex)
                {
                    string EMsg = "Case Workers Feedback POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["UID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }

        #endregion

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
            await registrationService.Save_CaseWorkersErrorLogs(mydayDto);
        }


    }
}