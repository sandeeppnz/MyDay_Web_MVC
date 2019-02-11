using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static SANSurveyWebAPI.BLL.RandomTaskGenerationService;

namespace SANSurveyWebAPI.Controllers.User
{
    [Authorize]
    public class CaseWorkerController : BaseController
    {
        private CaseWorkersService surveyService;
        private HangfireScheduler schedulerService;
        private UserHomeService userHomeService;
        private ProfileService profileService;

        private ApplicationDbContext db = new ApplicationDbContext();

        public CaseWorkerController()
        {
            this.surveyService = new CaseWorkersService();
            this.schedulerService = new HangfireScheduler();
            this.userHomeService = new UserHomeService();
            this.profileService = new ProfileService();
        }
        //TODO: add dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            { }
            base.Dispose(disposing);
        }
        // GET: CaseWorker
        public ActionResult Index()
        {
            return View();
        }

        #region Methods

        public void ResetLoopSessionVariables()
        {
            Session["ForClient"] = null;
            Session["DemoForUser"] = null;
            Session["TaskType"] = null;
            Session["LoopDemo"] = null;
            Session["CaseWorkers"] = null;            
            Session["ProgressValueValue"] = 0;
            Session["TaskLimit"] = null;           
        }

        private async Task LoadSurveySessionsBySurveyUid(string uid)
        {
            SurveyDto s = surveyService.GetSurveyByUid(uid);
            if (s != null)
            {
                ProfileRosterDto r = null;
                if (s.RosterItemId.HasValue)
                {
                    r = surveyService.GetRosterByRosterId(s.RosterItemId.Value);
                }

                //Load Tasklist and match it to the tasks
                var taskList = surveyService.GetAllTask();
                await SaveSurveySessions(s, taskList);
                await SaveRosterSessions(r);
            }
        }
        private async Task LoadSurveySessionsBySurveyId(int surveyId)
        {
            SurveyDto s = surveyService.GetSurveyById(surveyId);
            ProfileRosterDto r = null;

            if (s.RosterItemId.HasValue)
            { r = surveyService.GetRosterByRosterId(s.RosterItemId.Value); }

            var taskList = surveyService.GetAllTask();
            await SaveSurveySessions(s, taskList);
            await SaveRosterSessions(r);
        }
        private async Task SaveSurveySessions(SurveyDto s, IList<TaskVM> taskList)
        {
            if (s != null)
            {
                s = surveyService.ResolveSurvey(s);

                DateTime startTime = s.SurveyWindowStartDateTime.Value;
                DateTime endTime = s.SurveyWindowEndDateTime.Value;
                TimeSpan span = endTime.Subtract(startTime);
                int surveyDuration = (int)Math.Round(span.TotalMinutes);

                Session["SurveyId"] = s.Id;
                Session["SurveyUID"] = s.Uid;
                Session["ProfileId"] = s.ProfileId;
                Session["CurrRound"] = s.MaxStep;
                Session["RemainingDuration"] = s.RemainingDuration;
                Session["CurrTaskEndTime"] = s.CurrTaskEndTime;
                Session["CurrTaskStartTime"] = s.CurrTaskStartTime;
                Session["CurrTask"] = s.CurrTask;
                Session["AddTaskId"] = s.AddTaskId;

                if (s.CurrTask != null && s.CurrTask != 0)
                {
                    string taskName = taskList.FirstOrDefault(x => x.ID == s.CurrTask).Name;
                    Session["CurrTaskName"] = taskName;
                }
                if (s.AddTaskId != null)
                {
                    string taskName = taskList.FirstOrDefault(x => x.ID == s.AddTaskId).Name;
                    Session["AddCurrTaskName"] = taskName;
                }
                Session["NextTaskStartTime"] = s.NextTaskStartTime;
                Session["FirstQuestion"] = s.FirstQuestion;
                Session["SurveyStartTime"] = s.SurveyWindowStartDateTime.Value;
                Session["SurveyEndTime"] = s.SurveyWindowEndDateTime.Value;
                Session["SurveyDuration"] = surveyDuration;
                Session["SurveySpan"] = s.SurveyDescription;

                //TODO: Modifcation from Non-UTC to UTC
                Session["SurveyExpiryDate"] = s.SurveyExpiryDateTimeUtc;
                Session["SurveyProgressNext"] = s.SurveyProgressNext;               
            }
        }

        private async Task SaveRosterSessions(ProfileRosterDto r)
        {
            if (r != null)
            {
                Session["ShiftStartTime"] = r.Start;
                Session["ShiftEndTime"] = r.End;
                Session["ShiftSpan"] = r.Description;
            }
        }

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
            await surveyService.Save_CaseWorkersErrorLogs(mydayDto);
        }

        #endregion

        #region CaseWorkers - MyDay

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MyDaySurvey(string id)
        {
            string secondKey = string.Empty;
            string splitSecondKey = string.Empty;
            string demoForUser = string.Empty;
            string taskType = string.Empty;
            ResetLoopSessionVariables();

            if (Request.QueryString.AllKeys[1].ToString() != "taskType")
            {
                secondKey = Request.QueryString.AllKeys[1].ToString();
                splitSecondKey = secondKey.Split(';')[1];

                demoForUser = Request.QueryString["demoFor"].ToString();               
                taskType = Request.QueryString[secondKey].ToString();
            }
            else
            {
                demoForUser = Request.QueryString["demoFor"].ToString();
                taskType = Request.QueryString["taskType"].ToString();
            }

            if (taskType == "CaseWorkers")
            { Session["ForClient"] = "CaseWorkers"; } //\u0028 WAM \u0029"; }

            Session["DemoForUser"] = demoForUser;
            Session["TaskType"] = taskType;

            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> MyDaySurvey()
        {
            string currentSurveyID = string.Empty;
            string demoForUser = Session["DemoForUser"].ToString().ToLower();
            string taskType = Session["TaskType"].ToString();

            if (demoForUser == "caseworkers")
            {  Session["CaseWorkers"] = "Yes";   }

            if (ModelState.IsValid)
            {
                #region I.Create Random Profile
                //Generate random email
                string UserEmailGenerated = string.Empty;
                Random randomGenerator = new Random();
                int randomNumber = randomGenerator.Next(6999);
                UserEmailGenerated = demoForUser + randomNumber + "@aut.ac.nz";
                Session["UserEmailGenerated"] = UserEmailGenerated;
                Session["UserPasswordGenerated"] = "******";

                Profile newProfile = new Profile();
                //Genrate UID
                Guid uid = Guid.NewGuid();

                //Which Client
                if (demoForUser == "caseworkers")
                {
                    newProfile.ClientInitials = "CW";
                    newProfile.ClientName = "Case Workers";
                }

                //Encryption
                newProfile.LoginEmail = StringCipher.EncryptRfc2898(UserEmailGenerated);

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

                #region II. Generate roster 

                DateTime currentDate = DateTime.Now;

                //Previous Day Work Shift from 9 to 5
                DateTime previousDate = currentDate.Date.AddDays(-1);
                previousDate = previousDate.AddHours(9);
                DateTime endDate = currentDate.Date.AddDays(-1);
                endDate = endDate.Date.AddHours(18);

                DateTime previousDateUTC = previousDate.AddHours(-13);
                DateTime EndDateUTC = endDate.AddHours(-13);

                ProfileRosterDto rosterDto = new ProfileRosterDto();
                rosterDto.Name = "On Shift";
                rosterDto.Start = previousDate;
                rosterDto.End = endDate;
                rosterDto.ProfileId = newProfile.Id;
                rosterDto.Description = previousDate.ToString("dd MMM yyyy hh:mm tt") + " -  " + endDate.ToString("dd MMM yyyy hh:mm tt");
                rosterDto.StartUtc = previousDateUTC;
                rosterDto.EndUtc = EndDateUTC;
                var e = ObjectMapper.GetProfileRosterEntity(rosterDto);
                db.ProfileRosters.Add(e);
                int saveResult = await db.SaveChangesAsync(); // SAVE Roster.
                rosterDto.Id = e.Id;

                #endregion

                #region III. Generate Survey for the Roster
                //---------------------------------------------

                Survey myDaySurvey = new Survey();
                Guid surveyId = Guid.NewGuid();
                Random systemNum = new Random();
                int sysRandom = systemNum.Next(4);
                DateTime surveyStartWindow = previousDate.AddHours(1);
                DateTime surveyEndWindow = surveyStartWindow.AddHours(4);
                myDaySurvey.ProfileId = newProfile.Id;
                myDaySurvey.Uid = surveyId.ToString();
                myDaySurvey.RosterItemId = e.Id;
                myDaySurvey.CreatedDateTimeUtc = currentDate.AddHours(-13); //(nowtime)
                myDaySurvey.SurveyProgressNext = "Invited";
                myDaySurvey.MaxStep = 0;
                myDaySurvey.Status = "Active";
                myDaySurvey.SurveyWindowStartDateTime = surveyStartWindow; //So that 10am
                myDaySurvey.SurveyWindowStartDateTimeUtc = surveyStartWindow.AddHours(-13);
                myDaySurvey.SurveyWindowEndDateTime = surveyEndWindow; //So its 2pm
                myDaySurvey.SurveyWindowEndDateTimeUtc = surveyEndWindow.AddHours(-13);
                myDaySurvey.SurveyExpiryDateTime = surveyStartWindow.AddHours(24);//(after 24hours of enddatetime)
                myDaySurvey.SurveyExpiryDateTimeUtc = (surveyStartWindow.AddHours(24)).AddHours(13);
                myDaySurvey.SysGenRandomNumber = sysRandom;
                myDaySurvey.SurveyDescription = surveyStartWindow.ToString("dd MMM yyyy hh:mm tt") + " -  " + surveyEndWindow.ToString("dd MMM yyyy hh:mm tt");
                myDaySurvey.CurrTaskStartTime = surveyStartWindow;//(fix it to 10)
                myDaySurvey.CurrTaskEndTime = surveyEndWindow;//(fix it to 14)
                myDaySurvey.NextTaskStartTime = surveyEndWindow; // = CurrentTaskEndTIme
                db.Surveys.Add(myDaySurvey);
                await db.SaveChangesAsync();
                db.Entry(newProfile).GetDatabaseValues();
                currentSurveyID = myDaySurvey.Uid;
                Session["SurveyId"] = myDaySurvey.Id;
                Session["SurveyUID"] = myDaySurvey.Uid;
                #endregion              
            }
            return RedirectToAction("SignupForCaseWorkers", "Account", new { uid = Session["UID"].ToString(), surveyID = Session["SurveyUID"].ToString() });            
        }
        #endregion

        #region CaseWorkers - New Survey

        [HttpGet]
        public async Task<ActionResult> NewSurvey(string uid)
        {
            try
            {
                string loopDemo = string.Empty;
                string mydayv2 = string.Empty;              
                                
                await LoadSurveySessionsBySurveyUid(uid);
                Session["ProgressValueValue"] = 0;
                SurveyDto s = surveyService.GetSurveyByUid(uid);                
                
                ResumeSurveyMV v = new ResumeSurveyMV();
                int surveyId = (int)Session["SurveyId"];
                int profileId = (int)Session["ProfileId"];
                DateTime? surveyExpiryDate = (DateTime?)Session["SurveyExpiryDate"];

                v.SurveyProgressNext = (string)Session["SurveyProgressNext"];
                v.ShiftSpan = (string)Session["ShiftSpan"];
                v.SurveySpan = (string)Session["SurveySpan"];

                Session["ProgressValueValue"] = 0;

                if (surveyExpiryDate.HasValue)
                {
                    //TODO: MOD
                    if (surveyExpiryDate.Value <= DateTime.UtcNow)
                    { return RedirectToAction("ExpiredSurvey", new { id = uid });    }
                }

                if (!string.IsNullOrEmpty(v.SurveyProgressNext))
                {
                    if (!string.IsNullOrEmpty(uid))
                    {
                        v.Uid = uid;

                        if (Request.IsAjaxRequest())
                        {  return PartialView(v); }
                        else
                        { return View(v);  }
                    }
                    else
                    {
                        await LogMyDayError(uid, "NewSurvey Task 1: Survey UID not found!", "InvalidError");                       
                        return RedirectToAction("InvalidError");
                    }
                }
                await LogMyDayError(uid, "NewSurvey Task 2: Survey UID not found!", "NEW SURVEY");               
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(uid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> NewSurvey(ResumeSurveyMV v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];

                    return RedirectToAction("ShiftTime");
                }
                catch (Exception ex)
                {
                    string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["SurveyUID"].ToString(), EMsg, "SurveyError");
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

        #region Shift Time

        [HttpGet]
        public async Task<ActionResult> ShiftTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();

                    await LoadSurveySessionsBySurveyId(surveyId);

                    Survey2ShiftTime1ViewModel v = new Survey2ShiftTime1ViewModel();

                    v.ShiftStartTime = (DateTime)Session["ShiftStartTime"];
                    v.ShiftEndTime = (DateTime)Session["ShiftEndTime"];
                    v.ShiftSpan = (string)Session["ShiftSpan"];
                    v.SurveyStartTime = (DateTime)Session["SurveyStartTime"];
                    v.SurveyEndTime = (DateTime)Session["SurveyEndTime"];
                    v.SurveySpan = (string)Session["SurveySpan"];
                    v.Uid = (string)Session["SurveyUID"];
                    suid = v.Uid;
                    //TODO: remove not needed
                    Session["CurrProgressValue"] = Constants.PAGE_ONE_PROGRESS_ORIGINAL;

                    Session["ProgressValueValue"] = 5;                   

                    await surveyService.RemovePreviousResponses(surveyId);

                    await surveyService.SetShitTimeSettingsAsyncGET(surveyId, profileId, GetBaseURL());                    
                   
                    if (Request.IsAjaxRequest())
                    {  return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "ShiftTime GET: Survey UID not found!", "SHIFT TIME");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "ShiftTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ShiftTime(Survey2ShiftTime1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];

                    //2107-01-13
                    Session["CurrRound"] = null;
                    Session["NumberOfRounds"] = null;
                    Session["GoToReset"] = null;

                    DateTime startTime = v.SurveyStartTime;
                    DateTime endTime = v.SurveyEndTime;
                    string surveySpan = startTime.ToString("dd MMM yyyy hh:mm tt") + " -  " + endTime.ToString("dd MMM yyyy hh:mm tt");

                    if (v.SurveySpan == surveySpan)
                    { Session["SurveySpan"] = v.SurveySpan; }
                    else
                    { Session["SurveySpan"] = surveySpan; }

                    TimeSpan span = endTime.Subtract(startTime);
                    int surveyDuration = (int)Math.Round(span.TotalMinutes);

                    //Survey
                    Session["SurveyStartTime"] = v.SurveyStartTime;
                    Session["SurveyEndTime"] = v.SurveyEndTime;
                    Session["CurrTaskStartTime"] = v.SurveyStartTime; //set the starting time
                    Session["SurveyDuration"] = surveyDuration;
                    Session["RemainingDuration"] = surveyDuration;
                    Session["FirstQuestion"] = true;
                    Session["SurveyProgressNext"] = Constants.StatusSurveyProgress.Tasks.ToString();
                    if (v.WasWorking == "No")
                    { v.IsOnCall = "No"; }

                    await surveyService.SetShitTimeSettings(surveyId, v.IsOnCall, v.WasWorking);
                    
                    if (v.WasWorking != "No")
                    {
                        return RedirectToAction("Tasks"); 
                    }
                    else { return RedirectToAction("List", "Calendar"); }
                }
                catch (Exception ex)
                {
                    string EMsg = "ShiftTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
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

        #region CaseWorkers - Tasks

        [HttpGet]
        public async Task<ActionResult> Tasks()
        {
            string suid = string.Empty;
            int surveyId = (int)Session["SurveyId"];
            int profileId = (int)Session["ProfileId"];
            Survey2ShiftTime1ViewModel u = new Survey2ShiftTime1ViewModel();
            ProfileDetailsByClient pc = new ProfileDetailsByClient();

            if (Session["ClientByProfile"] == null)
            {
                pc = profileService.GetClientDetailsByProfileId(profileId);
            }
            else
            {
                pc = (ProfileDetailsByClient)Session["ClientByProfile"];
            }

            try
            {
                #region Shift Time "GET" processing

                if (Session["SurveyId"] != null)
                {
                    await LoadSurveySessionsBySurveyId(surveyId);

                    u.ShiftStartTime = (DateTime)Session["ShiftStartTime"];
                    u.ShiftEndTime = (DateTime)Session["ShiftEndTime"];
                    u.ShiftSpan = (string)Session["ShiftSpan"];
                    u.SurveyStartTime = (DateTime)Session["SurveyStartTime"];
                    u.SurveyEndTime = (DateTime)Session["SurveyEndTime"];
                    u.SurveySpan = (string)Session["SurveySpan"];
                    u.Uid = (string)Session["SurveyUID"];
                    suid = u.Uid;

                    //TODO: remove not needed
                    Session["CurrProgressValue"] = Constants.PAGE_ONE_PROGRESS_ORIGINAL;
                    Session["ProgressValueValue"] = 5;
                    await surveyService.RemovePreviousResponses(surveyId);
                    await surveyService.SetShitTimeSettingsAsyncGET(surveyId, profileId, GetBaseURL());
                }

                #endregion

                #region Shift Time "POST" processing

                //2107-01-13
                Session["CurrRound"] = null;
                Session["NumberOfRounds"] = null;
                Session["GoToReset"] = null;
                DateTime startTime = u.SurveyStartTime;
                DateTime endTime = u.SurveyEndTime;
                string surveySpan = startTime.ToString("dd MMM yyyy hh:mm tt") + " -  " + endTime.ToString("dd MMM yyyy hh:mm tt");

                if (u.SurveySpan == surveySpan)
                { Session["SurveySpan"] = u.SurveySpan; }
                else
                { Session["SurveySpan"] = surveySpan; }
                TimeSpan span = endTime.Subtract(startTime);
                int surveyDuration = (int)Math.Round(span.TotalMinutes);

                //Survey
                Session["SurveyStartTime"] = u.SurveyStartTime;
                Session["SurveyEndTime"] = u.SurveyEndTime;
                Session["CurrTaskStartTime"] = u.SurveyStartTime; //set the starting time
                Session["SurveyDuration"] = surveyDuration;
                if (Convert.ToInt32(Session["RemainingDuration"].ToString()) != 0
                    && Convert.ToInt32(Session["RemainingDuration"].ToString()) != 240)
                {                   
                    Session["CurrTaskStartTime"] = Session["CurrTaskEndTime"];
                }
                else { Session["RemainingDuration"] = surveyDuration; }

                Session["FirstQuestion"] = true;
                Session["SurveyProgressNext"] = Constants.StatusSurveyProgress.WAMTasks.ToString();
                await surveyService.SetShitTimeSettingsAsyncPOST(surveyId, u.IsOnCall, pc.ClientInitials.ToLower().ToString());

                #endregion

                #region "Tasks"

                if (Session["SurveyId"] != null)
                {
                    SurveyTasks1ViewModel v = new SurveyTasks1ViewModel();
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string uid = (string)Session["SurveyUID"];
                    suid = uid;

                    int currProgressValueValue = (int)Session["ProgressValueValue"];

                    if (currProgressValueValue <= 15)
                    { Session["ProgressValueValue"] = 15; }

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();
                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();
                    string taskType = Session["TaskType"].ToString();

                    //GetAllTaskByType
                    IList<TaskVM> result;                    
                    result = surveyService.GetAllTaskByType(taskType); 
                                      
                    foreach (var k in result)
                    {
                        if (k.ID != 70)
                        {
                            checkBoxListItems.Add(new CheckBoxListItem()
                            {
                                ID = k.ID,
                                Display = k.Name,
                                Description = k.LongName,
                                IsChecked = false
                            });
                        }
                    }
                    v.FullTaskList = checkBoxListItems;

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }

                    return View(v);
                }
                await LogMyDayError(suid, "Tasks GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "Tasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
            #endregion
        }
        [HttpPost]
        public async Task<ActionResult> Tasks(SurveyTasks1ViewModel v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];

                    if (Session["CurrRound"] == null) { Session["CurrRound"] = 1; }
                    int taskId = 0;

                    TaskItemDto task = new TaskItemDto();
                    if (!string.IsNullOrEmpty(v.OtherTaskName))
                    {
                        task = surveyService.GetOtherTask();
                        Session["CurrTaskNameOther"] = v.OtherTaskName;
                    }
                    else
                    {
                        taskId = (int)selectedTasks[0];
                        Session["CurrTaskNameOther"] = null;
                        task = surveyService.GetTaskByTaskId(taskId);

                        if (selectedTasks == null || selectedTasks.Length <= 0)
                        { return RedirectToAction("TaskNotSelectedError");   }

                    }
                    Session["CurrTask"] = task.Id;
                    Session["CurrTaskName"] = task.ShortName;

                    int currRound = (int)Session["CurrRound"];

                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];

                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];
                    int remainingDuration = (int)Session["RemainingDuration"];

                    await surveyService.SetWAMTasksAsyncPOST(
                         surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion
                        );
                   
                    return RedirectToAction("TaskTime");
                }
                catch (Exception ex)
                {
                    string EMsg = "Tasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["SurveyUID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }

        #endregion

        #region CaseWorkers - TaskTime

        [HttpGet]
        public async Task<ActionResult> TaskTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskTimeVM v = new SurveyWAMTaskTimeVM();

                    int surveyId = (int)Session["SurveyId"];
                    DateTime taskStartTime = (DateTime)Session["CurrTaskStartTime"];
                    int taskId = (int)Session["CurrTask"];
                    string uid = (string)Session["SurveyUID"];
                    suid = uid;
                    int remainingDuration = (int)Session["RemainingDuration"];
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];

                    string taskName = (string)Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    { taskName = (string)Session["CurrTaskNameOther"]; }

                    int pv = (int)Session["ProgressValueValue"];
                    if (pv <= 15)
                    { Session["ProgressValueValue"] = surveyService.CalProgressValTaskTime(pv, true); }

                    v.ShiftSpan = shiftSpan;
                    v.Uid = uid;
                    v.SurveySpan = surveySpan;
                    v.TaskId = taskId;
                    v.TaskName = taskName;

                    v.TimeHours = remainingDuration / 60;
                    v.TimeMinutes = remainingDuration % 60;

                    v.remainingTimeHours = remainingDuration / 60;
                    v.remainingTimeMinutes = remainingDuration % 60;

                    v.OptionsList = surveyService.GetWAMTaskTimeOptions();

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "TaskTime: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "TaskTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TaskTime(SurveyWAMTaskTimeVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    DateTime taskStartTime = (DateTime)Session["CurrTaskStartTime"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];
                    string taskOther = (string)Session["CurrTaskNameOther"];

                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];

                    int remainingDuration = (int)Session["RemainingDuration"];
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];

                    #region Calculate the next task time

                    int totalTimeInMin = (v.TimeHours * 60) + v.TimeMinutes;
                    remainingDuration = remainingDuration - totalTimeInMin;
                    Session["RemainingDuration"] = remainingDuration;
                    DateTime endTime = startTime.AddMinutes(totalTimeInMin);
                    Session["CurrTaskEndTime"] = endTime;
                    DateTime nextStartTime = startTime.AddMinutes(totalTimeInMin);
                    Session["NextTaskStartTime"] = nextStartTime;
                    Session["FirstQuestion"] = false;

                    #endregion

                    //insert
                    WAMResponseDto r = new WAMResponseDto();
                    r.SurveyId = surveyId;
                    r.ProfileId = profileId;
                    r.TaskId = taskId; //added to the model by Get Method using Session
                    r.TaskOther = taskOther;
                    //Page Stats
                    r.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r.EndResponseDateTimeUtc = DateTime.UtcNow;

                    r.TaskStartDateTime = startTime;
                    r.TaskEndDateTime = endTime;

                    r.ShiftStartDateTime = shiftStartTime;
                    r.ShiftEndDateTime = shiftEndTime;
                    r.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r.SurveyWindowEndDateTime = surveyWindowEndDateTime;

                    r.Question = v.QDB;
                    r.Answer = totalTimeInMin.ToString();
                    r.IsOtherTask = false;

                    await surveyService.AddWAMResponseAsync(r);

                    #region save status to survey

                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];

                    await surveyService.SetWAMTaskTimeAsyncPOST(
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion
                        );

                    #endregion

                    #region WithWHom

                    IList<string> OptionIds = v.HiddenOptionIds.Split(',').Reverse().ToList<string>();
                    SurveyWAMTaskTimeVM getAllOptions = new SurveyWAMTaskTimeVM();
                    getAllOptions.OptionsList = surveyService.GetWAMTaskTimeOptions();
                    List<WAMTaskTimeOptions> selectedList = new List<WAMTaskTimeOptions>();
                    foreach (var id in OptionIds)
                    {
                        int i = Convert.ToInt32(id);
                        selectedList.Add(getAllOptions.OptionsList[i]);
                        if (i == 5)
                        { selectedList[selectedList.Count() - 1].LongName = v.WithWHomOther; }
                    }
                    bool IsSaved = surveyService.SaveWAMWithWhom(profileId, surveyId, taskId, selectedList,
                                                v.NearestLocation, v.NearestOtherLocation);

                    #endregion                   

                    return RedirectToAction("TaskRating1");
                }
                catch (Exception ex)
                {
                    string EMsg = "TaskTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(Session["SurveyUID"].ToString(), EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }

        #endregion

        #region CaseWorkers - TaskRating

        [HttpGet]
        public async Task<ActionResult> TaskRating1()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskRating1VM v = new SurveyWAMTaskRating1VM();

                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];
                    string taskName = (string)Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    { taskName = (string)Session["CurrTaskNameOther"]; }

                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];
                    string uid = (string)Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];

                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;
                    v.TaskName = taskName;

                    int pv = (int)Session["ProgressValueValue"];
                    int surveyDuration = (int)Session["SurveyDuration"];
                    int remainingDuration = (int)Session["RemainingDuration"];

                    if (Session["GoToReset"] == null)
                    { Session["GoToReset"] = false; }

                    bool resetGoto = (bool)Session["GoToReset"];
                    if (Session["NumberOfRounds"] == null)
                    { Session["NumberOfRounds"] = 1; }
                    else
                    {
                        if (resetGoto == false)
                        {
                            int x = (int)Session["NumberOfRounds"];
                            x++;
                            Session["NumberOfRounds"] = x;
                        }
                    }
                    int numRounds = (int)Session["NumberOfRounds"];

                    decimal proportion = (decimal)((decimal)remainingDuration / (decimal)surveyDuration);
                    decimal allocationSpanPercentage = 1 - proportion;
                    decimal remainingLength = 75 - pv;
                    Session["ProgressValueValue"] = surveyService.CalProgressValRating1(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);

                    v.Q1Ans = Constants.NA_7Rating;
                    v.Q2Ans = Constants.NA_7Rating;
                    v.Q3Ans = Constants.NA_7Rating;
                    v.Q4Ans = Constants.NA_7Rating;
                    v.Q5Ans = Constants.NA_7Rating;

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "TaskRating1 GET: Survey UID not found!", "InvalidError");              
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");                
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TaskRating1(SurveyWAMTaskRating1VM v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];

                    Session["GoToReset"] = false;

                    string taskOther = (string)Session["CurrTaskNameOther"];
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime)Session["CurrTaskEndTime"];

                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];
                    int remainingDuration = (int)Session["RemainingDuration"];

                    //Q2
                    WAMResponseDto r1 = new WAMResponseDto();
                    r1.SurveyId = surveyId;
                    r1.TaskId = taskId;
                    r1.ProfileId = profileId;
                    r1.TaskOther = taskOther;
                    r1.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r1.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r1.TaskStartDateTime = startTime;
                    r1.TaskEndDateTime = endTime;
                    r1.ShiftStartDateTime = shiftStartTime;
                    r1.ShiftEndDateTime = shiftEndTime;
                    r1.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r1.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r1.Question = v.Q1DB;
                    r1.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                    r1.IsOtherTask = false;
                    surveyService.AddWAMResponse(r1);

                    WAMResponseDto r2 = new WAMResponseDto();
                    r2.SurveyId = surveyId;
                    r2.TaskId = taskId;
                    r2.ProfileId = profileId;
                    r2.TaskOther = taskOther;
                    r2.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r2.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r2.TaskStartDateTime = startTime;
                    r2.TaskEndDateTime = endTime;
                    r2.ShiftStartDateTime = shiftStartTime;
                    r2.ShiftEndDateTime = shiftEndTime;
                    r2.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r2.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r2.Question = v.Q2DB;
                    r2.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                    r2.IsOtherTask = false;
                    surveyService.AddWAMResponse(r2);

                    WAMResponseDto r3 = new WAMResponseDto();
                    r3.SurveyId = surveyId;
                    r3.TaskId = taskId;
                    r3.ProfileId = profileId;
                    r3.TaskOther = taskOther;
                    r3.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r3.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r3.TaskStartDateTime = startTime;
                    r3.TaskEndDateTime = endTime;
                    r3.ShiftStartDateTime = shiftStartTime;
                    r3.ShiftEndDateTime = shiftEndTime;
                    r3.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r3.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r3.Question = v.Q3DB;
                    r3.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                    r3.IsOtherTask = false;
                    surveyService.AddWAMResponse(r3);

                    WAMResponseDto r4 = new WAMResponseDto();
                    r4.SurveyId = surveyId;
                    r4.TaskId = taskId;
                    r4.ProfileId = profileId;
                    r4.TaskOther = taskOther;
                    r4.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r4.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r4.TaskStartDateTime = startTime;
                    r4.TaskEndDateTime = endTime;
                    r4.ShiftStartDateTime = shiftStartTime;
                    r4.ShiftEndDateTime = shiftEndTime;
                    r4.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r4.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r4.Question = v.Q4DB;
                    r4.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                    r4.IsOtherTask = false;
                    surveyService.AddWAMResponse(r4);

                    WAMResponseDto r5 = new WAMResponseDto();
                    r5.SurveyId = surveyId;
                    r5.TaskId = taskId;
                    r5.ProfileId = profileId;
                    r5.TaskOther = taskOther;
                    r5.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r5.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r5.TaskStartDateTime = startTime;
                    r5.TaskEndDateTime = endTime;
                    r5.ShiftStartDateTime = shiftStartTime;
                    r5.ShiftEndDateTime = shiftEndTime;
                    r5.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r5.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r5.Question = v.Q5DB;
                    r5.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                    r5.IsOtherTask = false;
                    surveyService.AddWAMResponse(r5);

                    surveyService.SaveResponse();

                    #region save status to survey

                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];

                    await surveyService.SetWAMRating1AsyncPOST(
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion
                        );

                    #endregion                    
                    return RedirectToAction("TaskRating2");
                }
                catch (Exception ex)
                {
                    string EMsg = "TaskRating1 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> TaskRating2()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskRating2VM v = new SurveyWAMTaskRating2VM();

                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];
                    string uid = (string)Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];
                    string taskName = (string)Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    { taskName = (string)Session["CurrTaskNameOther"]; }

                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;
                    v.TaskName = taskName;

                    v.Q6Ans = Constants.NA_7Rating;
                    v.Q7Ans = Constants.NA_7Rating;
                    v.Q8Ans = Constants.NA_7Rating;
                    v.Q9Ans = Constants.NA_7Rating;
                    v.Q10Ans = Constants.NA_7Rating;

                    int pv = (int)Session["ProgressValueValue"];
                    int surveyDuration = (int)Session["SurveyDuration"];
                    int remainingDuration = (int)Session["RemainingDuration"];

                    int numRounds = (int)Session["NumberOfRounds"];
                    bool resetGoto = (bool)Session["GoToReset"];
                    decimal proportion = (decimal)((decimal)remainingDuration / (decimal)surveyDuration);
                    decimal allocationSpanPercentage = 1 - proportion;
                    decimal remainingLength = 75 - pv;
                    Session["ProgressValueValue"] = surveyService.CalProgressValRating2(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "TaskRating2 GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TaskRating2(SurveyWAMTaskRating2VM v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];
                    string taskOther = (string)Session["CurrTaskNameOther"];

                    Session["GoToReset"] = false;


                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime)Session["CurrTaskEndTime"];
                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];
                    int remainingDuration = (int)Session["RemainingDuration"];

                    WAMResponseDto r6 = new WAMResponseDto();
                    r6.SurveyId = surveyId;
                    r6.ProfileId = profileId;
                    r6.TaskOther = taskOther;
                    r6.TaskId = v.TaskId;
                    r6.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r6.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r6.TaskStartDateTime = startTime;
                    r6.TaskEndDateTime = endTime;
                    r6.ShiftStartDateTime = shiftStartTime;
                    r6.ShiftEndDateTime = shiftEndTime;
                    r6.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r6.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r6.Question = v.Q6DB;
                    r6.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    r6.IsOtherTask = false;
                    surveyService.AddWAMResponse(r6);

                    WAMResponseDto r7 = new WAMResponseDto();
                    r7.SurveyId = surveyId;
                    r7.ProfileId = profileId;
                    r7.TaskOther = taskOther;
                    r7.TaskId = v.TaskId;
                    r7.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r7.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r7.TaskStartDateTime = startTime;
                    r7.TaskEndDateTime = endTime;
                    r7.ShiftStartDateTime = shiftStartTime;
                    r7.ShiftEndDateTime = shiftEndTime;
                    r7.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r7.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r7.Question = v.Q7DB;
                    r7.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    r7.IsOtherTask = false;
                    surveyService.AddWAMResponse(r7);

                    WAMResponseDto r8 = new WAMResponseDto();
                    r8.SurveyId = surveyId;
                    r8.ProfileId = profileId;
                    r8.TaskOther = taskOther;
                    r8.TaskId = v.TaskId;
                    r8.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r8.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r8.TaskStartDateTime = startTime;
                    r8.TaskEndDateTime = endTime;
                    r8.ShiftStartDateTime = shiftStartTime;
                    r8.ShiftEndDateTime = shiftEndTime;
                    r8.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r8.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r8.Question = v.Q8DB;
                    r8.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    r8.IsOtherTask = false;
                    surveyService.AddWAMResponse(r8);

                    WAMResponseDto r9 = new WAMResponseDto();
                    r9.SurveyId = surveyId;
                    r9.ProfileId = profileId;
                    r9.TaskOther = taskOther;
                    r9.TaskId = v.TaskId;
                    r9.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r9.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r9.TaskStartDateTime = startTime;
                    r9.TaskEndDateTime = endTime;
                    r9.ShiftStartDateTime = shiftStartTime;
                    r9.ShiftEndDateTime = shiftEndTime;
                    r9.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r9.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r9.Question = v.Q9DB;
                    r9.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    r9.IsOtherTask = false;
                    surveyService.AddWAMResponse(r9);

                    WAMResponseDto r10 = new WAMResponseDto();
                    r10.SurveyId = surveyId;
                    r10.ProfileId = profileId;
                    r10.TaskOther = taskOther;
                    r10.TaskId = v.TaskId;
                    r10.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r10.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r10.TaskStartDateTime = startTime;
                    r10.TaskEndDateTime = endTime;
                    r10.ShiftStartDateTime = shiftStartTime;
                    r10.ShiftEndDateTime = shiftEndTime;
                    r10.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r10.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r10.Question = v.Q10DB;
                    r10.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    r10.IsOtherTask = false;
                    surveyService.AddWAMResponse(r10);

                    surveyService.SaveResponse();

                    //stopping criteria
                    //update the duration sessions variables
                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];

                    if (remainingDuration <= 0)
                    {
                        surveyService.SetWAMRating2AsyncPOSTResult(
                            surveyId,
                            currRound,
                            remainingDuration,
                            currTaskEndTime,
                            currTaskStartTime,
                            currTask,
                            nextTaskStartTime,
                            firstQuestion
                            );
                        return RedirectToAction("AddShiftTime", new { v.Uid });
                    }
                    else
                    {
                        Session["CurrTaskNameOther"] = null;
                        currRound++;
                        Session["CurrRound"] = currRound; //this is to say that it is the next round
                        Session["CurrTaskStartTime"] = Session["NextTaskStartTime"];
                        currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];

                        await surveyService.SetWAMRating2AsyncPOSTNext(
                         surveyId,
                         currRound,
                         remainingDuration,
                         currTaskEndTime,
                         currTaskStartTime,
                         currTask,
                         nextTaskStartTime,
                         firstQuestion
                         );
                        return RedirectToAction("Tasks");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "TaskRating2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }

        #endregion


        #region CaseWorkers - SID

        [HttpGet]
        public async Task<ActionResult> SID(string id)
        {
            try
            {
                SidMV v = new SidMV();
                int surveyId = (int)Session["SurveyId"];
                int profileId = (int)Session["ProfileId"];                
                v.ShiftSpan = (string)Session["ShiftSpan"];
                v.SurveySpan = (string)Session["SurveySpan"];

                if (!string.IsNullOrEmpty(id))
                {
                    v.Uid = id;
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    else
                    { return View(v); }
                }
                else
                {
                    await LogMyDayError(id, "SID Task: Survey UID not found!", "InvalidError");
                    return RedirectToAction("InvalidError");
                }
            }
            catch (Exception ex)
            {
                string EMsg = "SID:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(id, EMsg, "SurveyError");                
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> SID(SidMV v, string button)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];

                    await LoadSurveySessionsBySurveyId(surveyId);
                    if (button == "Restart")
                    {
                        Session["GoToReset"] = false;
                        return RedirectToAction("Tasks");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(v.Uid))
                        {
                            Session["GoToReset"] = true;
                            v.SurveyProgressNext = (string)Session["SurveyProgressNext"];
                            return RedirectToAction(v.SurveyProgressNext, new { v.Uid });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "SID POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");                    
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(); }
            return View();
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> AddShiftTime(string uid)
        {
            try
            {
                ResumeSurveyMV v = new ResumeSurveyMV();

                int surveyId = (int)Session["SurveyId"];
                int profileId = (int)Session["ProfileId"];
                DateTime? surveyExpiryDate = (DateTime?)Session["SurveyExpiryDate"];

                //v.SurveyProgressNext = (string) Session["SurveyProgressNext"];
                v.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.AddTasks.ToString();
                v.ShiftSpan = (string)Session["ShiftSpan"];
                v.SurveySpan = (string)Session["SurveySpan"];

                Session["ProgressValueValue"] = 80;

                if (!string.IsNullOrEmpty(uid))
                {
                    v.Uid = uid;
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    else
                    { return View(v); }
                }
                await LogMyDayError(uid, "AddShiftTime GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddShiftTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(uid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddShiftTime(ResumeSurveyMV v, string button)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];
                    int remainingDuration = (int)Session["RemainingDuration"];
                    if (button == "Yes")
                    {
                        surveyService.SetWAMAdditionalAsyncPOST(surveyId, currRound, remainingDuration, currTaskEndTime,
                                               currTaskStartTime, currTask, nextTaskStartTime, firstQuestion,
                                               Constants.StatusSurveyProgress.WAMAddTasks.ToString());
                        return RedirectToAction("AddTasks");
                    }
                    else
                    {
                        surveyService.SetWAMAdditionalAsyncPOST(surveyId, currRound, remainingDuration, currTaskEndTime,
                                              currTaskStartTime, currTask, nextTaskStartTime, firstQuestion,
                                              Constants.StatusSurveyProgress.Completed.ToString());

                        ResetSessionVariables();
                        return RedirectToAction("Feedback");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "AddShiftTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> AddTasks()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTasks1ViewModel v = new SurveyTasks1ViewModel();

                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];
                    string uid = (string)Session["SurveyUID"];

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();
                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);
                    //var result = surveyService.GetAllTask();
                    var result = surveyService.GetAllTaskByType("CaseWorkers");
                    foreach (var k in result)
                    {
                        checkBoxListItems.Add(new CheckBoxListItem()
                        {
                            ID = k.ID,
                            Display = k.Name,
                            Description = k.LongName,
                            IsChecked = false
                        });
                    }
                    v.FullTaskList = checkBoxListItems;
                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddTasks(pv, true);

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "AddTasks GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddTasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> AddTasks(SurveyTasks1ViewModel v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    if (Session["CurrRound"] == null)
                    { Session["CurrRound"] = 1; }

                    int taskId = 0;
                    TaskItemDto task = new TaskItemDto();

                    if (!string.IsNullOrEmpty(v.OtherTaskName))
                    {
                        task = surveyService.GetOtherTask();
                        Session["CurrTaskNameOther"] = v.OtherTaskName;
                    }
                    else
                    {
                        taskId = (int)selectedTasks[0];
                        Session["CurrTaskNameOther"] = null;
                        task = surveyService.GetTaskByTaskId(taskId);
                        if (selectedTasks == null || selectedTasks.Length <= 0)
                        { return RedirectToAction("TaskNotSelectedError"); }
                    }

                    Session["AddTaskId"] = task.Id;
                    Session["CurrTaskName"] = null;
                    Session["AddCurrTaskName"] = task.ShortName;

                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];
                    int remainingDuration = (int)Session["RemainingDuration"];

                    surveyService.SetAdditionalTasksAsyncPOST(surveyId, currRound, remainingDuration, currTaskEndTime,
                            currTaskStartTime, currTask, nextTaskStartTime, firstQuestion,
                            Constants.StatusSurveyProgress.WAMAddTaskTime.ToString(), task.Id);

                    return RedirectToAction("AddTaskTime");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTasks POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> AddTaskTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskTimeVM v = new SurveyWAMTaskTimeVM();

                    int surveyId = (int)Session["SurveyId"];
                    DateTime taskStartTime = (DateTime)Session["CurrTaskStartTime"];

                    //int taskId = (int) Session["CurrTask"];
                    int taskId = (int)Session["AddTaskId"];

                    string uid = (string)Session["SurveyUID"];
                    //int remainingDuration = (int) Session["RemainingDuration"];
                    suid = uid;                    
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];

                    string taskName = (string)Session["AddCurrTaskName"];

                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string)Session["CurrTaskNameOther"];
                    }

                    v.ShiftSpan = shiftSpan;
                    v.Uid = uid;
                    v.SurveySpan = surveySpan;

                    v.TaskId = taskId;
                    v.TaskName = taskName;

                    DateTime surveyStart = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyEnd = (DateTime)Session["SurveyEndTime"];
                    TimeSpan difference = surveyEnd - surveyStart;
                    double remainingDuration = difference.TotalMinutes;

                    v.TimeHours = (int)(remainingDuration / 60);
                    v.TimeMinutes = (int)(remainingDuration % 60);

                    v.remainingTimeHours = (int)(remainingDuration / 60);
                    v.remainingTimeMinutes = (int)(remainingDuration % 60);

                    v.OptionsList = surveyService.GetWAMTaskTimeOptions();
                                        
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);
                }
                await LogMyDayError(suid, "TaskRating2 GET: Survey UID not found!", "InvalidError");               
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddTaskTime(SurveyWAMTaskTimeVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    DateTime taskStartTime = (DateTime)Session["CurrTaskStartTime"];
                    int profileId = (int)Session["ProfileId"];
                    int addTaskId = (int)Session["AddTaskId"];
                    string taskOther = (string)Session["CurrTaskNameOther"];
                    
                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];

                    #region Calculate the next task time

                    int totalTimeInMin = (v.TimeHours * 60) + v.TimeMinutes;

                    #endregion

                    //insert

                    WAMResponseDto r = new WAMResponseDto();
                    r.SurveyId = surveyId;
                    r.ProfileId = profileId;
                    r.TaskId = addTaskId; //added to the model by Get Method using Session
                    r.TaskOther = taskOther;
                    //Page Stats
                    r.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r.TaskStartDateTime = null;
                    r.TaskEndDateTime = null;
                    r.ShiftStartDateTime = shiftStartTime;
                    r.ShiftEndDateTime = shiftEndTime;
                    r.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r.SurveyWindowEndDateTime = surveyWindowEndDateTime;

                    r.Question = v.QDB;
                    r.Answer = totalTimeInMin.ToString();
                    r.IsOtherTask = true;

                    await surveyService.AddWAMResponseAsync(r);

                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddTaskTime(pv, true);

                    #region save status to survey

                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];

                    surveyService.SetAdditionalTasksAsyncPOST(
                        surveyId,
                        currRound,
                        0,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        Constants.StatusSurveyProgress.WAMAddTaskRating1.ToString(),
                        addTaskId);

                    #endregion

                    #region WithWHom

                    IList<string> OptionIds = v.HiddenOptionIds.Split(',').Reverse().ToList<string>();
                    SurveyWAMTaskTimeVM getAllOptions = new SurveyWAMTaskTimeVM();
                    getAllOptions.OptionsList = surveyService.GetWAMTaskTimeOptions();
                    List<WAMTaskTimeOptions> selectedList = new List<WAMTaskTimeOptions>();
                    foreach (var id in OptionIds)
                    {
                        int i = Convert.ToInt32(id);
                        selectedList.Add(getAllOptions.OptionsList[i]);
                        if (i == 5)
                        { selectedList[selectedList.Count() - 1].LongName = v.WithWHomOther; }
                    }
                    bool IsSaved = surveyService.SaveWAMWithWhom(profileId, surveyId, addTaskId, selectedList,
                                    v.NearestLocation, v.NearestOtherLocation);

                    #endregion

                    return RedirectToAction("AddTaskRating1");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> AddTaskRating1()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating1ViewModel v = new SurveyTaskRating1ViewModel();

                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int addTaskId = (int)Session["AddTaskId"];
                    int taskId = (int)Session["CurrTask"];

                    string taskName = (string)Session["AddCurrTaskName"];

                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string)Session["CurrTaskNameOther"];
                    }
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];
                    string uid = (string)Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];

                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = addTaskId;
                    v.TaskName = taskName;

                    v.Q1Ans = Constants.NA_7Rating;
                    v.Q2Ans = Constants.NA_7Rating;
                    v.Q3Ans = Constants.NA_7Rating;
                    v.Q4Ans = Constants.NA_7Rating;
                    v.Q5Ans = Constants.NA_7Rating;

                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddRating1(pv, 1);

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);
                }
                await LogMyDayError(suid, "AddTaskRating1 GET: Survey UID not found!", "InvalidError");               
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddTaskRating1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");                
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddTaskRating1(SurveyTaskRating1ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int addTaskId = (int)Session["AddTaskId"];

                    string taskOther = (string)Session["CurrTaskNameOther"];
                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];

                    //Q2
                    WAMResponseDto r1 = new WAMResponseDto();
                    r1.SurveyId = surveyId;
                    r1.TaskId = addTaskId;
                    r1.ProfileId = profileId;
                    r1.TaskOther = taskOther;
                    r1.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r1.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r1.TaskStartDateTime = null;
                    r1.TaskEndDateTime = null;
                    r1.ShiftStartDateTime = shiftStartTime;
                    r1.ShiftEndDateTime = shiftEndTime;
                    r1.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r1.SurveyWindowEndDateTime = surveyWindowEndDateTime;

                    r1.Question = v.Q1DB;
                    r1.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                    r1.IsOtherTask = true;
                    surveyService.AddWAMResponse(r1);

                    WAMResponseDto r2 = new WAMResponseDto();
                    r2.SurveyId = surveyId;
                    r2.TaskId = addTaskId;
                    r2.ProfileId = profileId;
                    r2.TaskOther = taskOther;
                    r2.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r2.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r2.TaskStartDateTime = null;
                    r2.TaskEndDateTime = null;
                    r2.ShiftStartDateTime = shiftStartTime;
                    r2.ShiftEndDateTime = shiftEndTime;
                    r2.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r2.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r2.Question = v.Q2DB;
                    r2.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                    r2.IsOtherTask = true;
                    surveyService.AddWAMResponse(r2);

                    WAMResponseDto r3 = new WAMResponseDto();
                    r3.SurveyId = surveyId;
                    r3.TaskId = addTaskId;
                    r3.ProfileId = profileId;
                    r3.TaskOther = taskOther;
                    r3.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r3.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r3.TaskStartDateTime = null;
                    r3.TaskEndDateTime = null;
                    r3.ShiftStartDateTime = shiftStartTime;
                    r3.ShiftEndDateTime = shiftEndTime;
                    r3.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r3.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r3.Question = v.Q3DB;
                    r3.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                    r3.IsOtherTask = true;
                    surveyService.AddWAMResponse(r3);

                    WAMResponseDto r4 = new WAMResponseDto();
                    r4.SurveyId = surveyId;
                    r4.TaskId = addTaskId;
                    r4.ProfileId = profileId;
                    r4.TaskOther = taskOther;
                    r4.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r4.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r4.TaskStartDateTime = null;
                    r4.TaskEndDateTime = null;
                    r4.ShiftStartDateTime = shiftStartTime;
                    r4.ShiftEndDateTime = shiftEndTime;
                    r4.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r4.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r4.Question = v.Q4DB;
                    r4.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                    r4.IsOtherTask = true;
                    surveyService.AddWAMResponse(r4);

                    WAMResponseDto r5 = new WAMResponseDto();
                    r5.SurveyId = surveyId;
                    r5.TaskId = addTaskId;
                    r5.ProfileId = profileId;
                    r5.TaskOther = taskOther;
                    r5.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r5.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r5.TaskStartDateTime = null;
                    r5.TaskEndDateTime = null;
                    r5.ShiftStartDateTime = shiftStartTime;
                    r5.ShiftEndDateTime = shiftEndTime;
                    r5.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r5.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r5.Question = v.Q5DB;
                    r5.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                    r5.IsOtherTask = true;

                    surveyService.AddWAMResponse(r5);

                    surveyService.SaveResponse();


                    #region save status to survey

                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];



                    surveyService.SetAdditionalTasksAsyncPOST(
                        surveyId,
                        currRound,
                        0,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        Constants.StatusSurveyProgress.WAMAddTaskRating2.ToString(),
                        addTaskId
                        );

                    #endregion

                    return RedirectToAction("AddTaskRating2");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskRating POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> AddTaskRating2()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating2ViewModel v = new SurveyTaskRating2ViewModel();

                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int addTaskId = (int)Session["AddTaskId"];
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];
                    string uid = (string)Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];
                    string taskName = (string)Session["AddCurrTaskName"];

                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string)Session["CurrTaskNameOther"];
                    }
                    
                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = addTaskId;
                    v.TaskName = taskName;

                    v.Q6Ans = Constants.NA_7Rating;
                    v.Q7Ans = Constants.NA_7Rating;
                    v.Q8Ans = Constants.NA_7Rating;
                    v.Q9Ans = Constants.NA_7Rating;
                    v.Q10Ans = Constants.NA_7Rating;
                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddRating2(pv, 1);

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);
                }
                await LogMyDayError(suid, "AddTaskRating2 GET: Survey UID not found!", "InvalidError");               
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddTaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddTaskRating2(SurveyTaskRating2ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int addTaskId = (int)Session["AddTaskId"];
                    string taskOther = (string)Session["CurrTaskNameOther"];
                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];

                    WAMResponseDto r6 = new WAMResponseDto();
                    r6.SurveyId = surveyId;
                    r6.ProfileId = profileId;
                    r6.TaskOther = taskOther;
                    r6.TaskId = addTaskId;
                    r6.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r6.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r6.TaskStartDateTime = null;
                    r6.TaskEndDateTime = null;
                    r6.ShiftStartDateTime = shiftStartTime;
                    r6.ShiftEndDateTime = shiftEndTime;
                    r6.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r6.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r6.Question = v.Q6DB;
                    r6.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    r6.IsOtherTask = true;
                    surveyService.AddWAMResponse(r6);

                    WAMResponseDto r7 = new WAMResponseDto();
                    r7.SurveyId = surveyId;
                    r7.ProfileId = profileId;
                    r7.TaskOther = taskOther;
                    r7.TaskId = addTaskId;
                    r7.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r7.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r7.TaskStartDateTime = null;
                    r7.TaskEndDateTime = null;
                    r7.ShiftStartDateTime = shiftStartTime;
                    r7.ShiftEndDateTime = shiftEndTime;
                    r7.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r7.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r7.Question = v.Q7DB;
                    r7.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    r7.IsOtherTask = true;
                    surveyService.AddWAMResponse(r7);

                    WAMResponseDto r8 = new WAMResponseDto();
                    r8.SurveyId = surveyId;
                    r8.ProfileId = profileId;
                    r8.TaskOther = taskOther;
                    r8.TaskId = addTaskId;
                    r8.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r8.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r8.TaskStartDateTime = null;
                    r8.TaskEndDateTime = null;
                    r8.ShiftStartDateTime = shiftStartTime;
                    r8.ShiftEndDateTime = shiftEndTime;
                    r8.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r8.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r8.Question = v.Q8DB;
                    r8.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    r8.IsOtherTask = true;
                    surveyService.AddWAMResponse(r8);

                    WAMResponseDto r9 = new WAMResponseDto();
                    r9.SurveyId = surveyId;
                    r9.ProfileId = profileId;
                    r9.TaskOther = taskOther;
                    r9.TaskId = addTaskId;
                    r9.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r9.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r9.TaskStartDateTime = null;
                    r9.TaskEndDateTime = null;
                    r9.ShiftStartDateTime = shiftStartTime;
                    r9.ShiftEndDateTime = shiftEndTime;
                    r9.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r9.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r9.Question = v.Q9DB;
                    r9.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    r9.IsOtherTask = true;
                    surveyService.AddWAMResponse(r9);

                    WAMResponseDto r10 = new WAMResponseDto();
                    r10.SurveyId = surveyId;
                    r10.ProfileId = profileId;
                    r10.TaskOther = taskOther;
                    r10.TaskId = addTaskId;
                    r10.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r10.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r10.TaskStartDateTime = null;
                    r10.TaskEndDateTime = null;
                    r10.ShiftStartDateTime = shiftStartTime;
                    r10.ShiftEndDateTime = shiftEndTime;
                    r10.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r10.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r10.Question = v.Q10DB;
                    r10.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    r10.IsOtherTask = true;
                    surveyService.AddWAMResponse(r10);

                    surveyService.SaveResponse();

                    //stopping criteria
                    //update the duration sessions variables
                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];

                    surveyService.SetAdditionalAsyncPOST(
                                            surveyId,
                                            currRound,
                                            0,
                                            currTaskEndTime,
                                            currTaskStartTime,
                                            currTask,
                                            nextTaskStartTime,
                                            firstQuestion,
                                            //Change to Completed
                                            Constants.StatusSurveyProgress.Completed.ToString()
                                            );

                    ResetSessionVariables();
                    return RedirectToAction("Feedback");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskRating2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        public void ResetSessionVariables()
        {
            Session["CurrRound"] = null;
            Session["RemainingDuration"] = null;
            Session["CurrTaskEndTime"] = null;
            Session["NextTaskStartTime"] = null;
            Session["FirstQuestion"] = null;
            Session["CurrProgressValue"] = null;
            Session["ShiftStartTime"] = null;
            Session["ShiftEndTime"] = null;
            Session["SurveyStartTime"] = null;
            Session["SurveyEndTime"] = null;
            Session["SurveyDuration"] = null;
            Session["SurveyExpiryDate"] = null;
            Session["SurveyProgressNext"] = null;
        }

        public async Task<ActionResult> Feedback()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];
                    Session["ProgressValueValue"] = 100;
                    string uid = (string)Session["SurveyUID"];

                    SurveyFeedbackViewModel v = new SurveyFeedbackViewModel();
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Feedback GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Feedback(SurveyFeedbackViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];

                    ProfileDetailsByClient pc = new ProfileDetailsByClient();
                    pc = (ProfileDetailsByClient)Session["ClientByProfile"];

                    surveyService.SetFeedbackAsyncPOST(
                           surveyId, Constants.StatusSurveyProgress.Completed.ToString(), v.Comment
                           );
                   
                    return RedirectToAction("SurveyResults");
                    
                }
                catch (Exception ex)
                {
                    string EMsg = "Feedback POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        public async Task<ActionResult> SurveyResults()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    string shiftSpan = (string)Session["ShiftSpan"];
                    string surveySpan = (string)Session["SurveySpan"];

                    Session["ProgressValueValue"] = 100;
                    SurveyResults1ViewModel v = new SurveyResults1ViewModel();

                    var survey = surveyService.GetSurveyById(surveyId);
                    v.Comment = survey.Comment;

                    var responsesGeneric = surveyService.GetGenericWAMResponses(surveyId, profileId);
                    var responsesAdditional = surveyService.GetAdditionalWAMResponses(surveyId, profileId);
                    var tasklist = surveyService.GetAllTask();
                    Session["TaskList"] = tasklist; //to be used on Edit Response

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;

                    SurveyWAMTaskTimeVM ref1 = new SurveyWAMTaskTimeVM();
                    SurveyWAMTaskRating1VM ref2 = new SurveyWAMTaskRating1VM();
                    SurveyWAMTaskRating2VM ref3 = new SurveyWAMTaskRating2VM();
                    Dictionary<string, string> refenceTable = new Dictionary<string, string>();

                    refenceTable.Add(ref1.QDB, ref1.QDisplayShort);
                    refenceTable.Add(ref2.Q1DB, ref2.Q1DisplayShort);
                    refenceTable.Add(ref2.Q2DB, ref2.Q2DisplayShort);
                    refenceTable.Add(ref2.Q3DB, ref2.Q3DisplayShort);
                    refenceTable.Add(ref2.Q4DB, ref2.Q4DisplayShort);
                    refenceTable.Add(ref2.Q5DB, ref2.Q5DisplayShort);
                    refenceTable.Add(ref3.Q6DB, ref3.Q6DisplayShort);
                    refenceTable.Add(ref3.Q7DB, ref3.Q7DisplayShort);
                    refenceTable.Add(ref3.Q8DB, ref3.Q8DisplayShort);
                    refenceTable.Add(ref3.Q9DB, ref3.Q9DisplayShort);
                    refenceTable.Add(ref3.Q10DB, ref3.Q10DisplayShort);

                    List<Tuple<string, SurveyWAMResponseVM>> genericList = new List<Tuple<string, SurveyWAMResponseVM>>();
                    List<Tuple<string, SurveyWAMResponseVM>> additionalList = new List<Tuple<string, SurveyWAMResponseVM>>();

                    DateTime? taskStartDate = null;

                    int totalMins = 0;

                    //Generic
                    foreach (var k in responsesGeneric)
                    {
                        SurveyWAMResponseVM r = new SurveyWAMResponseVM();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();
                        r.TaskId = k.TaskId;
                        r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();

                        if (k.Question == Constants.QDB)
                        { totalMins = int.Parse(k.Answer.ToString()); }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;
                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        { r.TaskName = k.TaskOther; }

                        r.TaskDescription = taskDet.LongName;
                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                        genericList.Add(Tuple.Create(r.TaskName + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                    }
                    v.FullWAMResponseList = (Lookup<string, SurveyWAMResponseVM>)genericList.ToLookup(t => t.Item1, t => t.Item2);

                    //Additional
                    foreach (var k in responsesAdditional)
                    {
                        SurveyWAMResponseVM r = new SurveyWAMResponseVM();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();


                        r.TaskId = k.TaskId;

                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        { r.TaskName = k.TaskOther; }

                        r.TaskDescription = taskDet.LongName;

                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt") + "- " + k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                        additionalList.Add(Tuple.Create(r.TaskName + " STARTDATESANADD", r));
                    }
                    v.FullWAMResponseAdditionalList = (Lookup<string, SurveyWAMResponseVM>)additionalList.ToLookup(t => t.Item1, t => t.Item2);

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                string EMsg = "Feedback POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> SurveyResults(SurveyResults1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    string EMsg = "Feedback POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        public async Task<ActionResult> EditResponse(int taskId, DateTime taskStartDate)
        {
            string suid = string.Empty;
            try
            {
                SurveyEditResponseViewModel v = new SurveyEditResponseViewModel();

                int surveyId = (int)Session["SurveyId"];
                int profileId = (int)Session["ProfileId"];
                string shiftSpan = (string)Session["ShiftSpan"];
                string surveySpan = (string)Session["SurveySpan"];

                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;

                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>)Session["TaskList"];
                    task = taskList.Where(m => m.ID == taskId)
                                            .Select(m => new TaskItemDto()
                                            {
                                                Id = m.ID,
                                                LongName = m.LongName,
                                                ShortName = m.Name
                                            })
                                            .Single();
                }
                else
                { task = surveyService.GetTaskByTaskId(taskId); }
                v.TaskName = task.ShortName;
                Session["CurrTask"] = task.Id;
                Session["CurrTaskStartTime"] = taskStartDate;

                bool isAny = false;
                var listOfResponses = surveyService.GetAllWAMResponseAsync(taskId, profileId, surveyId, taskStartDate);
                var response = listOfResponses.Where(t => t.Question == v.Q1DB).SingleOrDefault();

                if (response.TaskOther != null)
                { v.TaskName = response.TaskOther; }

                Session["editCurrTaskStartTime"] = response.TaskStartDateTime;
                Session["editCurrTaskEndTime"] = response.TaskEndDateTime;
                v.ShiftSpan = shiftSpan;
                v.SurveySpan = surveySpan;

                if (response != null)
                {
                    v.Q1Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q1Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q2DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q2Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q3Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q4Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q5Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q6Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q7Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q8Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q9Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q10Ans = Constants.NA_7Rating; }

                if (isAny)
                { v.IsExist = true; }

                if (Request.IsAjaxRequest())
                { return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "EditResponse GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditResponse(SurveyEditResponseViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];

                    // Save to db
                    #region update Question  

                    var listOfResponses = surveyService.GetAllWAMResponseAsync(taskId, profileId, surveyId, startTime);
                    var response = listOfResponses.Where(r => r.Question == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        surveyService.UpdateWAMResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        surveyService.UpdateWAMResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        surveyService.UpdateWAMResponse(response);
                    }
                    surveyService.SaveResponse();

                    #endregion

                    return RedirectToAction("SurveyResults");


                }
                catch (Exception ex)
                {
                    string EMsg = "EditResponse POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        public async Task<ActionResult> AddEditResponse(int taskId)
        {
            try
            {
                SurveyEditResponseViewModel v = new SurveyEditResponseViewModel();

                int surveyId = (int)Session["SurveyId"];
                int profileId = (int)Session["ProfileId"];
                string shiftSpan = (string)Session["ShiftSpan"];
                string surveySpan = (string)Session["SurveySpan"];

                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;

                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>)Session["TaskList"];
                    task = taskList.Where(m => m.ID == taskId)
                                            .Select(m => new TaskItemDto()
                                            {
                                                Id = m.ID,
                                                LongName = m.LongName,
                                                ShortName = m.Name
                                            })
                                            .Single();
                }
                else
                { task = surveyService.GetTaskByTaskId(taskId); }
                v.TaskName = task.ShortName;

                Session["CurrTask"] = task.Id;
                bool isAny = false;

                var listOfResponses = surveyService.GetWAMAdditionalResponseAsync(taskId, profileId,
                    surveyId);
                var response = listOfResponses.Where(t => t.Question == v.Q1DB).SingleOrDefault();

                if (response.TaskOther != null)
                { v.TaskName = response.TaskOther; }

                Session["editCurrTaskStartTime"] = response.TaskStartDateTime;
                Session["editCurrTaskEndTime"] = response.TaskEndDateTime;
                v.ShiftSpan = shiftSpan;
                v.SurveySpan = surveySpan;

                if (response != null)
                {
                    v.Q1Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q1Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q2DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q2Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q3Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q4Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q5Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q6Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q7Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q8Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q9Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                { v.Q10Ans = Constants.NA_7Rating; }


                if (isAny)
                { v.IsExist = true; }
                if (Request.IsAjaxRequest())
                { return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "AddEditResponse GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddEditResponse(SurveyEditResponseViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int)Session["SurveyId"];
                    int profileId = (int)Session["ProfileId"];
                    int taskId = (int)Session["CurrTask"];
                    DateTime startTime = (DateTime)Session["CurrTaskStartTime"];

                    // Save to db
                    #region update Question  

                    var listOfResponses = surveyService.GetWAMAdditionalResponseAsync(taskId, profileId, surveyId);
                    var response = listOfResponses.Where(r => r.Question == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        surveyService.UpdateWAMResponse(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        surveyService.UpdateWAMResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        surveyService.UpdateWAMResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        surveyService.UpdateWAMResponse(response);
                    }
                    surveyService.SaveResponse();


                    #endregion

                    return RedirectToAction("SurveyResults");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddEditResponse POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
    }
}