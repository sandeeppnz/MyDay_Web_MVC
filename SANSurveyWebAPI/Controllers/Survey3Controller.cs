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
    public class Survey3Controller : BaseController
    {
        private PageStatService pageStatSvc;
        private Survey3Service surveyService;
        private HangfireScheduler schedulerService;
        private UserHomeService userHomeService;
        private ProfileService profileService;            

        private ApplicationDbContext db = new ApplicationDbContext();
        public Survey3Controller()
        {
            this.pageStatSvc = new PageStatService();
            this.surveyService = new Survey3Service();
            this.schedulerService = new HangfireScheduler();
            this.userHomeService = new UserHomeService();
            this.profileService = new ProfileService();
        }

        //TODO: add dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {            }
            base.Dispose(disposing);
        }
        public async Task<ActionResult> View(string id)
        {
            try
            {
                int profileId = 0;
                //ProfileDetailsByClient pc = new ProfileDetailsByClient();

                if (Session["ProfileId"] == null)
                {Session["ProfileId"] = profileService.GetCurrentProfileIdNonAsync(); }
                else
                {profileId = (int) Session["ProfileId"];}

                var survey = surveyService.GetSurveyByUid(id);
                int surveyId = 0;
                if (survey != null)
                { surveyId = survey.Id; }

                //pc = profileService.GetClientDetailsByProfileId(profileId);

                SurveyNonEditViewModel v = new SurveyNonEditViewModel();
                v.notifications = new List<NotificationVM>();
                v.surveySpan = survey.SurveyDescription;

                v.notifications = await userHomeService.GetNotificationList(GetBaseURL(), profileId);
                var responsesGeneric = surveyService.GetGenericResponses(surveyId, profileId);
                var responsesAdditional = surveyService.GetAdditionalResponses(surveyId, profileId);
                var responsesWR = surveyService.GetWRResponses(surveyId, profileId);
                var tasklist = surveyService.GetAllTask();

                SurveyTaskTime1ViewModel ref1 = new SurveyTaskTime1ViewModel();
                SurveyTaskRating1ViewModel ref2 = new SurveyTaskRating1ViewModel();
                SurveyTaskRating2ViewModel ref3 = new SurveyTaskRating2ViewModel();
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

                //List<Tuple<string, SurveyResponseViewModel>> list = new List<Tuple<string, SurveyResponseViewModel>>();
                List<Tuple<string, SurveyResponseViewModel>> genericList = new List<Tuple<string, SurveyResponseViewModel>>();
                List<Tuple<string, SurveyResponseViewModel>> additionalList = new List<Tuple<string, SurveyResponseViewModel>>();
                List<Tuple<string, SurveyResponseWRViewModel>> wrList = new List<Tuple<string, SurveyResponseWRViewModel>>();
                DateTime? taskStartDate = null;
                int totalMins = 0;

                foreach (var k in responsesGeneric)
                {
                    SurveyResponseViewModel r = new SurveyResponseViewModel();
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

                v.FullResponseList = (Lookup<string, SurveyResponseViewModel>) genericList.ToLookup(t => t.Item1, t => t.Item2);

                //Additional
                foreach (var k in responsesAdditional)
                {
                    SurveyResponseViewModel r = new SurveyResponseViewModel();

                    var taskDet = tasklist
                                        .Where(m => m.ID == k.TaskId)
                                        .Select(m => new { m.LongName, m.Name })
                                        .Single();
                    r.TaskId = k.TaskId;

                    if (k.Question == Constants.QDB)
                    { totalMins = int.Parse(k.Answer.ToString()); }

                    r.Question = refenceTable[k.Question].Trim(); //get display qns
                    r.Answer = k.Answer;
                    r.RatingString = surveyService.GetRatingString(k.Answer);
                    r.TaskName = taskDet.Name;
                    if (k.TaskOther != null)
                    {  r.TaskName = k.TaskOther; }

                    r.TaskDescription = taskDet.LongName;
                    r.TaskDuration = totalMins.ToString() + "min";
                    r.TaskTimeSpan = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt") + "- " + k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                    additionalList.Add(Tuple.Create(r.TaskName + " STARTDATESANADD", r));
                }
                v.FullResponseAdditionalList = (Lookup<string, SurveyResponseViewModel>) additionalList.ToLookup(t => t.Item1, t => t.Item2);
                
                //Additional
                foreach (var k in responsesWR)
                {
                    SurveyResponseWRViewModel r = new SurveyResponseWRViewModel();
                    var taskDet = tasklist
                                        .Where(m => m.ID == k.TaskId)
                                        .Select(m => new { m.LongName, m.Name })
                                        .Single();
                    r.TaskId = k.TaskId;
                    if (k.Question == Constants.QDB)
                    {totalMins = int.Parse(k.Answer.ToString()); }
                    r.Question = refenceTable[k.Question].Trim(); //get display qns
                    r.Answer = k.Answer;
                    r.RatingString = surveyService.GetRatingString(k.Answer);
                    r.TaskName = taskDet.Name;
                    if (k.TaskOther != null)
                    { r.TaskName = k.TaskOther; }
                    r.TaskDescription = taskDet.LongName;
                    r.WRStartDateTime = k.WardRoundWindowStartDateTime;
                    if (k.WardRoundWindowStartDateTime.HasValue)
                    { r.WRStartTime = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt"); }
                    r.WREndDateTime = k.WardRoundWindowEndDateTime;
                    if (k.WardRoundWindowEndDateTime.HasValue)
                    { r.WREndTime = k.WardRoundWindowEndDateTime.Value.ToString("hh:mm tt");}
                    r.TaskDuration = totalMins.ToString() + "min";
                    r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                    //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                    wrList.Add(Tuple.Create(r.TaskName + " STARTDATESANWRR " + k.TaskStartDateTime.Value, r));
                }
                v.FullResponseWRList = (Lookup<string, SurveyResponseWRViewModel>) wrList.ToLookup(t => t.Item1, t => t.Item2);
                
                if (Request.IsAjaxRequest())
                {return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "View:: Exception Message: " + ex.Message + " InnerException: " +ex.InnerException;
                await LogMyDayError(id, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_ViewResponse, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }


        public async Task<ActionResult> WAMView(string id)
        {
            try
            {
                int profileId = 0;

                if (Session["ProfileId"] == null)
                { Session["ProfileId"] = profileService.GetCurrentProfileIdNonAsync(); }
                else
                { profileId = (int)Session["ProfileId"]; }

                var survey = surveyService.GetSurveyByUid(id);
                int surveyId = 0;
                if (survey != null)
                { surveyId = survey.Id; }

                SurveyWAMNonEditViewModel v = new SurveyWAMNonEditViewModel();
                v.notifications = new List<NotificationVM>();
                v.surveySpan = survey.SurveyDescription;

                v.notifications = await userHomeService.GetNotificationList(GetBaseURL(), profileId);

                var responsesGeneric = surveyService.GetWAMGenericResponses(surveyId, profileId);
                var responsesAdditional = surveyService.GetAdditionalWAMResponses(surveyId, profileId);
                
                var tasklist = surveyService.GetAllTask();

                SurveyTaskTime1ViewModel ref1 = new SurveyTaskTime1ViewModel();
                SurveyTaskRating1ViewModel ref2 = new SurveyTaskRating1ViewModel();
                SurveyTaskRating2ViewModel ref3 = new SurveyTaskRating2ViewModel();
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

                //List<Tuple<string, SurveyResponseViewModel>> list = new List<Tuple<string, SurveyResponseViewModel>>();
                List<Tuple<string, SurveyWAMResponseVM>> genericList = new List<Tuple<string, SurveyWAMResponseVM>>();
                List<Tuple<string, SurveyWAMResponseVM>> additionalList = new List<Tuple<string, SurveyWAMResponseVM>>();
                
                DateTime? taskStartDate = null;
                int totalMins = 0;

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

                v.FullResponseList = (Lookup<string, SurveyWAMResponseVM>)genericList.ToLookup(t => t.Item1, t => t.Item2);

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
                    { totalMins = int.Parse(k.Answer.ToString()); }

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
                v.FullResponseAdditionalList = (Lookup<string, SurveyWAMResponseVM>)additionalList.ToLookup(t => t.Item1, t => t.Item2);               

                if (Request.IsAjaxRequest())
                { return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "View:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(id, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_ViewResponse, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        #region MinorViews
        private void LogError(Exception ex, Constants.PageName pageName, Constants.PageAction pageAction)
        {
            SurveyError er = new ViewModels.Web.SurveyError();
            er.Message = ex.Message;
            er.StackTrace = ex.StackTrace;
            er.CodeFile = this.GetType().Name;

            pageStatSvc.Insert(null, null, null, false, pageName, pageAction, Constants.PageType.ERROR, null, er.DisplayError());
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

        private async Task SaveSurveySessions(SurveyDto s, IList<TaskVM> taskList)
        {
            if (s != null)
            {
                s = surveyService.ResolveSurvey(s);

                DateTime startTime = s.SurveyWindowStartDateTime.Value;
                DateTime endTime = s.SurveyWindowEndDateTime.Value;
                TimeSpan span = endTime.Subtract(startTime);
                int surveyDuration = (int) Math.Round(span.TotalMinutes);

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

                //WR sessions

                if (s.WRCurrTaskStartTime.HasValue)
                    Session["CurrTaskStartTimeWR"] = s.WRCurrTaskStartTime.Value;

                if (s.WRCurrTaskEndTime.HasValue)
                    Session["CurrTaskEndTimeWR"] = s.WRCurrTaskEndTime.Value;

                if (s.WRNextTaskStartTime.HasValue)
                    Session["NextTaskStartTimeWR"] = s.WRNextTaskStartTime.Value;

                if (s.WRRemainingDuration.HasValue)
                    Session["RemainingDurationWR"] = s.WRRemainingDuration.Value;
                Session["WRCurrTasksId"] = s.WRCurrTasksId;
                List<SessionSurveyTask> wrTaskList = new List<SessionSurveyTask>();
                if (!string.IsNullOrEmpty(s.WRCurrTasksId))
                {
                    List<int> Ids = s.WRCurrTasksId.Split(',').Select(int.Parse).ToList();
                    foreach (var t in Ids)
                    {
                        string taskName = taskList.FirstOrDefault(x => x.ID == t).Name;
                        wrTaskList.Add(
                            new SessionSurveyTask
                            {
                                TaskId = t,
                                TaskName = taskName,
                                IsCompleted = false,
                            }
                        );
                    }
                }

                Session["SelectedWRTaskList"] = wrTaskList;


                if (s.WRCurrTaskId.HasValue)
                    Session["CurrTaskWR"] = s.WRCurrTaskId.Value;

                if (s.WRCurrWindowStartTime.HasValue)
                    Session["WardRoundWindowStartDateTime"] = s.WRCurrWindowStartTime.Value;
                if (s.WRCurrWindowEndTime.HasValue)
                    Session["WardRoundWindowEndDateTime"] = s.WRCurrWindowEndTime.Value;

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
            { r = surveyService.GetRosterByRosterId(s.RosterItemId.Value);}

            var taskList = surveyService.GetAllTask();
            await SaveSurveySessions(s, taskList);
            await SaveRosterSessions(r);
        }

        #endregion

        #region Loop survey

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LoopSurvey(string id)
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
                //taskType = Request.QueryString[splitSecondKey.ToString()].ToString();
                taskType = Request.QueryString[secondKey].ToString();
            }
            else {
                demoForUser = Request.QueryString["demoFor"].ToString();
                taskType = Request.QueryString["taskType"].ToString();
            }
            if (taskType == "WAM")
            { Session["ForClient"] = "The Warren and Mahoney"; } //\u0028 WAM \u0029"; }
            else if (taskType == "doctors" || taskType == "Patient")
            { Session["ForClient"] = "MyDay"; }
            else { Session["ForClient"] = "MyDay"; }

            Session["DemoForUser"] = demoForUser;              
            Session["TaskType"] = taskType;
            
            return View();
        }
        public void ResetLoopSessionVariables()
        {
            Session["ForClient"] = null;
            Session["DemoForUser"] = null;
            Session["TaskType"] = null;
            Session["LoopDemo"] = null;
            Session["MyDayV2"] = null;
            Session["IsItWam"] = null;
            Session["ProgressValueValue"] = 0;
            Session["TaskLimit"] = null;
            Session["CurrentAffectStage"] = null;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoopSurvey()
        {
            string currentSurveyID = string.Empty;
            string demoForUser = Session["DemoForUser"].ToString().ToLower();
            string taskType = Session["TaskType"].ToString();

            if (demoForUser == "myday" || demoForUser == "wam")
            {
                Session["LoopDemo"] = "Yes";               
                Session["MyDayV2"] = null;
            }
            else
            {
                Session["LoopDemo"] = null;
                Session["MyDayV2"] = "Yes";
            }
            if (demoForUser == "wam")
            { Session["IsItWam"] = "Yes"; }
            else { Session["IsItWam"] = "No"; }

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
                if (demoForUser == "myday" || demoForUser == "mydayv2")
                {
                    newProfile.ClientInitials = "JD";
                    newProfile.ClientName = "Junior Doctors";
                }
                else if (demoForUser == "wam")
                {
                    newProfile.ClientInitials = "WAM";
                    newProfile.ClientName = "Warren and Mahoney";
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
            //return RedirectToAction("NewSurvey", Session["currentSurveyID"].ToString());
            return RedirectToAction("SignupForDemo","Account", new { uid = Session["UID"].ToString(), surveyID = Session["SurveyUID"].ToString()});
            //return RedirectToAction("GetEmployees", new{DepId = dep.DepId,CatId = cat.CatId,RoleId = role.RoleId});
            //Response.Redirect("localhost:50685/App/Survey3/Index?id=" + Session["currentSurveyID"].ToString());
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
            await surveyService.Save_MyDayErrorLogs(mydayDto);
        }

        [HttpGet]
        public async Task<ActionResult> Index(string id)
        {
            try
            { 
                    Session["LoopDemo"] = "No";
                    Session["MyDayV2"] = "No";
                    ProfileDetailsByClient pc = new ProfileDetailsByClient();
                    await LoadSurveySessionsBySurveyUid(id);
                    Session["ProgressValueValue"] = 0;
                    SurveyDto s = surveyService.GetSurveyByUid(id);

                    
                    if (s != null)
                    {
                        await pageStatSvc.Insert(s.Id, null, null, false, Constants.PageName.Survey_Original_Start, Constants.PageAction.Get, Constants.PageType.Enter, s.ProfileId, id);                                              

                        pc = surveyService.GetClientByProfileID(s.ProfileId.GetValueOrDefault());
                        Session["ClientByProfile"] = pc;

                        if (pc.ClientInitials.ToLower().ToString() == "jd")
                        {
                            #region check survey table if completed

                            switch (s.SurveyProgressNext)
                            {
                                case "Invited":
                                    return RedirectToAction("NewSurvey", new { uid = id });
                                case "Completed":
                                    return RedirectToAction("CompletedSurvey", new { id = id });
                                case "ShiftTime":
                                case "Tasks":
                                case "TaskTime":
                                case "TaskRating1":
                                case "TaskRating2":
                                case "AddShiftTime":
                                case "AddTaskTime":
                                case "AddTasks":
                                case "AddTaskRating1":
                                case "AddTaskRating2":

                                case "WRTaskTime":
                                case "WRTasks":
                                case "WRTaskRating1":
                                case "WRTaskRating2":
                                case "WRTaskTimeInd":
                                    return RedirectToAction("ResumeSurvey", new { uid = id });
                                default:
                                    break;
                            }
                            #endregion

                            //TODO: Check when this occurs e.g. New
                            return RedirectToAction("ShiftTime");
                        }
                        else if (pc.ClientInitials.ToLower().ToString() == "wam")
                        {
                            switch (s.SurveyProgressNext)
                            {
                                case "Invited":
                                    return RedirectToAction("NewSurvey", new { uid = id });
                                case "Completed":
                                    return RedirectToAction("CompletedSurvey", new { id = id });
                                case "WAMTasks":
                                case "WAMTaskTime":
                                case "WAMTaskRating1":
                                case "WAMTaskRating2":
                               
                                case "WAMAddTaskTime":
                                case "WAMAddTasks":
                                case "WAMAddTaskRating1":
                                case "WAMAddTaskRating2":
                                    return RedirectToAction("ResumeSurvey", new { uid = id });
                                default:
                                    break;                                    
                            }
                            return RedirectToAction("");    
                        }
                    return RedirectToAction("");
                }
                    else {
                        await LogMyDayError(id, "Index: Survey UID wasn't found!", "InvalidError");
                        await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Start, Constants.PageAction.Get, Constants.PageType.ERROR, null, "Invalid Uid = " + id);
                        return RedirectToAction("InvalidError");
                }
            }
            catch (Exception ex)
            {
                string EMsg = "Index:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(id, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Start, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }

        [HttpGet]
        public async Task<ActionResult> NewSurvey(string uid)
        {
            try
            {
                string loopDemo = string.Empty;
                string mydayv2 = string.Empty;
                ProfileDetailsByClient pc = new ProfileDetailsByClient();
                pc = (ProfileDetailsByClient)Session["ClientByProfile"];

                if (Session["LoopDemo"] != null && Session["MyDayV2"] == null)
                {
                    loopDemo = Session["LoopDemo"].ToString();
                }
                else if (Session["MyDayV2"] != null && Session["LoopDemo"] == null)
                { mydayv2 = Session["MyDayV2"].ToString(); }
                else if (pc.ClientInitials.ToLower().ToString() == "wam")
                {
                    Session["LoopDemo"] = null;
                    Session["MyDayV2"] = null;
                }
                else { }

                if (uid != null && loopDemo == "Yes") //For Warren and Mahony Demo
                {
                    await LoadSurveySessionsBySurveyUid(uid);
                    Session["ProgressValueValue"] = 0;
                    SurveyDto s = surveyService.GetSurveyByUid(uid);
                }
                else if (uid != null && mydayv2 == "Yes") //For MyDay Version 2 demo
                {
                    await LoadSurveySessionsBySurveyUid(uid);
                    Session["ProgressValueValue"] = 0;
                    SurveyDto s = surveyService.GetSurveyByUid(uid);
                }
                else if (pc.ClientInitials.ToLower().ToString() == "wam") //For Warren and Mahony
                {
                    Session["DemoForUser"] = "Warren and Mahony";
                    Session["TaskType"] = "WAM";
                }
                else { Session["DemoForUser"] = "MyDay"; Session["TaskType"] = "doctors"; } ///please check for normal myday

                ResumeSurveyMV v = new ResumeSurveyMV();
                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                DateTime? surveyExpiryDate = (DateTime?) Session["SurveyExpiryDate"];

                v.SurveyProgressNext = (string) Session["SurveyProgressNext"];
                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                Session["ProgressValueValue"] = 0;


                await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_New, Constants.PageAction.Get, Constants.PageType.Enter, profileId, "Uid = " + uid);

                if (surveyExpiryDate.HasValue)
                {
                    //TODO: MOD
                    if (surveyExpiryDate.Value <= DateTime.UtcNow)
                    {
                        //pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_New, Constants.PageAction.Get, Constants.PageType.Exit, profileId, "Expired, uid=" + uid);
                        return RedirectToAction("ExpiredSurvey", new { id = uid });
                    }
                }

                if (!string.IsNullOrEmpty(v.SurveyProgressNext))
                {
                    if (!string.IsNullOrEmpty(uid))
                    {
                        v.Uid = uid;

                        if (Request.IsAjaxRequest())
                        {
                            return PartialView(v);
                        }
                        else
                        {
                            return View(v);
                        }
                    }
                    else
                    {
                        await LogMyDayError(uid, "NewSurvey Task 1: Survey UID not found!", "InvalidError");
                        await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_New, Constants.PageAction.Get, Constants.PageType.ERROR, profileId, "Invalid Uid = " + uid);
                        return RedirectToAction("InvalidError");
                    }
                }

                await LogMyDayError(uid, "NewSurvey Task 2: Survey UID not found!", "NEW SURVEY");
                await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_New, Constants.PageAction.Get, Constants.PageType.ERROR, profileId, "Error due to nextprogress being null");

                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "NewSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_New, Constants.PageAction.Get);
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
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_New, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked New Survey");
                    if (Session["DemoForUser"].ToString() == "WAM" 
                        || Session["DemoForUser"].ToString() == "Warren and Mahony")
                    { return RedirectToAction("WAMTasks"); }
                    else
                    {
                        return RedirectToAction("ShiftTime");
                    }

                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_New, Constants.PageAction.Post);
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
        public async Task<ActionResult> ResumeSurvey(string uid)
        {
            try
            {
                ResumeSurveyMV v = new ResumeSurveyMV();

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                DateTime? surveyExpiryDate = (DateTime?) Session["SurveyExpiryDate"];

                v.SurveyProgressNext = (string) Session["SurveyProgressNext"];
                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                Session["ProgressValueValue"] = 2;

                ProfileDetailsByClient pc = new ProfileDetailsByClient();
                pc = (ProfileDetailsByClient)Session["ClientByProfile"];

                if (pc.ClientInitials.ToLower().ToString() == "wam")
                {
                    Session["DemoForUser"] = "Warren and Mahony";
                    Session["TaskType"] = "WAM";
                }
                else if (pc.ClientInitials.ToLower().ToString() == "jd")
                {
                    Session["DemoForUser"] = "MyDay";
                    Session["TaskType"] = "doctors";
                }

                if (!string.IsNullOrEmpty(v.SurveyProgressNext))
                {
                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get, Constants.PageType.Enter, profileId, "Uid = " + uid);

                    if (surveyExpiryDate.HasValue)
                    {
                        //TODO: MOD
                        if (surveyExpiryDate.Value < DateTime.UtcNow)
                        {
                            return RedirectToAction("ExpiredSurvey", new { id = uid });
                        }
                    }
                    if (!string.IsNullOrEmpty(uid))
                    {
                        v.Uid = uid;

                        if (Request.IsAjaxRequest())
                        {
                            return PartialView(v);
                        }
                        else
                        {
                            return View(v);
                        }
                    }
                    else
                    {
                        await LogMyDayError(uid, "ResumeSurvey Task 1: Survey UID not found!", "InvalidError");
                        await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get, Constants.PageType.ERROR, profileId, "Invalid Uid = " + uid);
                        return RedirectToAction("InvalidError");
                    }
                }
                await LogMyDayError(uid, "ResumeSurvey Task 2: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get, Constants.PageType.ERROR, profileId, "Error due to nextprogress being null");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "ResumeSurvey:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ResumeSurvey(ResumeSurveyMV v, string button)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    ProfileDetailsByClient pc = new ProfileDetailsByClient();
                    pc = (ProfileDetailsByClient)Session["ClientByProfile"];

                    //await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post, Constants.PageType.Enter, profileId);
                    if (button == "Restart")
                    {
                        await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked Restart");

                        if (pc.ClientInitials.ToLower().ToString() == "jd")
                        {
                            return RedirectToAction("ShiftTime");
                        }
                        else if (pc.ClientInitials.ToLower().ToString() == "wam")
                        {
                            return RedirectToAction("WAMTasks");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(v.Uid))
                        {
                            await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked Continue");

                            if (v.SurveyProgressNext == "AddShiftTime")
                            {
                                return RedirectToAction(v.SurveyProgressNext, new { v.Uid });
                            }

                            return RedirectToAction(v.SurveyProgressNext);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "ResumeSurvey POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");

                    LogError(ex, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        /* Continue or restart Page upon hit Refresh   */
        [HttpGet]
        public async Task<ActionResult> SID(string id)
        {
            try
            {
                SidMV v = new SidMV();
                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.Enter, profileId, "Uid = " + id);
                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                if (!string.IsNullOrEmpty(id))
                {
                    v.Uid = id;
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.Exit, profileId, "Uid = " + v.Uid);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    else
                    {
                        return View(v);
                    }
                }
                else
                {
                    await LogMyDayError(id, "SID Task: Survey UID not found!", "InvalidError");
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.ERROR, profileId, "Invalid Uid = " + v.Uid);
                    return RedirectToAction("InvalidError");
                }
            }
            catch (Exception ex)
            {
                string EMsg = "SID:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(id, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get);
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

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];


                    await LoadSurveySessionsBySurveyId(surveyId);

                    //await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Post, Constants.PageType.Enter);
                    if (button == "Restart")
                    {
                        Session["GoToReset"] = false;


                        await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked Restart");
                        return RedirectToAction("ShiftTime");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(v.Uid))
                        {
                            Session["GoToReset"] = true;


                            v.SurveyProgressNext = (string) Session["SurveyProgressNext"];
                            //string progress = await LoadIncompleteSurveySettings(surveyId);
                            await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked Continue");
                            return RedirectToAction(v.SurveyProgressNext, new { v.Uid });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "SID POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Post);
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
        public async Task<ActionResult> ShiftTime()
        {
            string suid = string.Empty;
            try
            {                
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();

                    await LoadSurveySessionsBySurveyId(surveyId);

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    Survey2ShiftTime1ViewModel v = new Survey2ShiftTime1ViewModel();

                    v.ShiftStartTime = (DateTime) Session["ShiftStartTime"];
                    v.ShiftEndTime = (DateTime) Session["ShiftEndTime"];
                    v.ShiftSpan = (string) Session["ShiftSpan"];
                    v.SurveyStartTime = (DateTime) Session["SurveyStartTime"];
                    v.SurveyEndTime = (DateTime) Session["SurveyEndTime"];
                    v.SurveySpan = (string) Session["SurveySpan"];
                    v.Uid = (string) Session["SurveyUID"];
                    suid = v.Uid;
                    //TODO: remove not needed
                    Session["CurrProgressValue"] = Constants.PAGE_ONE_PROGRESS_ORIGINAL;

                    if (demoForUser == "myday" || demoForUser == "wam")
                    { Session["ProgressValueValue"] = 5; }
                    else if (demoForUser == "mydayv2")
                    { Session["ProgressValueValue"] = 10; }
                    else { Session["ProgressValueValue"] = 5; }

                    await surveyService.RemovePreviousResponses(surveyId);
                    
                    await surveyService.SetShitTimeSettingsAsyncGET(surveyId, profileId, GetBaseURL());
                    //await ResetSurvey(surveyId);
                    //await SetShitTimeSettingsAsyncGET(surveyId);
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Get, Constants.PageType.Exit, profileId);

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);

                    }
                    return View(v);
                }
                await LogMyDayError(suid, "ShiftTime GET: Survey UID not found!", "SHIFT TIME");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "ShiftTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Get);
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
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    //2107-01-13
                    Session["CurrRound"] = null;
                    Session["NumberOfRounds"] = null;
                    Session["GoToReset"] = null;

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

                    DateTime startTime = v.SurveyStartTime;
                    DateTime endTime = v.SurveyEndTime;
                    string surveySpan = startTime.ToString("dd MMM yyyy hh:mm tt") + " -  " + endTime.ToString("dd MMM yyyy hh:mm tt");

                    if (v.SurveySpan == surveySpan)
                    { Session["SurveySpan"] = v.SurveySpan; }
                    else
                    { Session["SurveySpan"] = surveySpan; }

                    TimeSpan span = endTime.Subtract(startTime);
                    int surveyDuration = (int) Math.Round(span.TotalMinutes);

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

                    //await surveyService.SetShitTimeSettingsAsyncPOST(surveyId, v.IsOnCall);
                    await surveyService.SetShitTimeSettings(surveyId, v.IsOnCall, v.WasWorking);
                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Post, Constants.PageType.Exit);

                    if (v.WasWorking != "No")
                    {
                        if (Session["LoopDemo"] == null && Session["MyDayV2"] != null)
                        { return RedirectToAction("Multitask"); }
                        else { return RedirectToAction("Tasks"); }
                    }
                    else { return RedirectToAction("List","Calendar"); }
                }
                catch (Exception ex)
                {
                    string EMsg = "ShiftTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }

        #region "WAM - MyDay"

        [HttpGet]
        public async Task<ActionResult> WAMTasks()
        {
            string suid = string.Empty;
            int surveyId = (int) Session["SurveyId"];
            int profileId = (int) Session["ProfileId"];
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
                        
                        u.ShiftStartTime = (DateTime) Session["ShiftStartTime"];
                        u.ShiftEndTime = (DateTime) Session["ShiftEndTime"];
                        u.ShiftSpan = (string) Session["ShiftSpan"];
                        u.SurveyStartTime = (DateTime) Session["SurveyStartTime"];
                        u.SurveyEndTime = (DateTime) Session["SurveyEndTime"];
                        u.SurveySpan = (string) Session["SurveySpan"];
                        u.Uid = (string) Session["SurveyUID"];
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
                        int surveyDuration = (int) Math.Round(span.TotalMinutes);

                        //Survey
                        Session["SurveyStartTime"] = u.SurveyStartTime;
                        Session["SurveyEndTime"] = u.SurveyEndTime;
                        Session["CurrTaskStartTime"] = u.SurveyStartTime; //set the starting time
                        Session["SurveyDuration"] = surveyDuration;
                        if (Convert.ToInt32(Session["RemainingDuration"].ToString()) != 0 
                            && Convert.ToInt32(Session["RemainingDuration"].ToString()) != 240)
                        {
                            //string dtRem = string.Empty; int remDura = 0;                            
                            //remDura = Convert.ToInt32(u.SurveyEndTime.TimeOfDay.TotalHours) - 
                            //        (Convert.ToInt32(Session["RemainingDuration"].ToString()) / 60);
                            //dtRem = u.SurveyStartTime.Date.ToString() + TimeSpan.FromHours(remDura);
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

                    //int surveyId = (int) Session["SurveyId"];
                    //int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    //string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;

                    int currProgressValueValue = (int) Session["ProgressValueValue"];

                    if (currProgressValueValue <= 15)
                    { Session["ProgressValueValue"] = 15; }

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();
                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();
                    string taskType = Session["TaskType"].ToString();

                    //GetAllTaskByType
                    IList<TaskVM> result;

                    if (taskType == "WAM")
                    { result = surveyService.GetAllTaskByType(taskType); }
                    else if (taskType == "doctors")
                    { result = surveyService.GetAllTaskByType("Patient"); }
                    else { result = surveyService.GetAllTaskByType("Patient"); }

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

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }

                    return View(v);
                }
                await LogMyDayError(suid, "Tasks GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "Tasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
            #endregion
        }
        [HttpPost]
        public async Task<ActionResult> WAMTasks(SurveyTasks1ViewModel v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    if (Session["CurrRound"] == null) { Session["CurrRound"] = 1; }

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.Enter, profileId);
                    int taskId = 0;
                    bool isWRIndicatorTask = false;

                    TaskItemDto task = new TaskItemDto();
                    if (!string.IsNullOrEmpty(v.OtherTaskName))
                    {
                        task = surveyService.GetOtherTask();
                        Session["CurrTaskNameOther"] = v.OtherTaskName;
                    }
                    else
                    {
                        taskId = (int) selectedTasks[0];
                        Session["CurrTaskNameOther"] = null;
                        task = surveyService.GetTaskByTaskId(taskId);

                        if (selectedTasks == null || selectedTasks.Length <= 0)
                        {
                            await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.ERROR, null, "TaskNotFound");
                            return RedirectToAction("TaskNotSelectedError");
                        }
                        if (task.IsWardRoundTask.HasValue)
                            isWRIndicatorTask = true;
                    }
                    Session["CurrTask"] = task.Id;
                    Session["CurrTaskName"] = task.ShortName;

                    int currRound = (int) Session["CurrRound"];

                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];

                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];
                    int remainingDuration = (int) Session["RemainingDuration"];

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
                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
                                        
                    return RedirectToAction("WAMTaskTime");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
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
        public async Task<ActionResult> WAMTaskTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskTimeVM v = new SurveyWAMTaskTimeVM();

                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                    int taskId = (int) Session["CurrTask"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    int remainingDuration = (int) Session["RemainingDuration"];                    
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    { taskName = (string) Session["CurrTaskNameOther"]; }

                    int pv = (int) Session["ProgressValueValue"];
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
                await LogMyDayError(suid, "WAMTaskTime: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "WAMTaskTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMTaskTime(SurveyWAMTaskTimeVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    string taskOther = (string) Session["CurrTaskNameOther"];
                    
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

                    int remainingDuration = (int) Session["RemainingDuration"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

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

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

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

                    return RedirectToAction("WAMTaskRating1");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> WAMTaskRating1()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskRating1VM v = new SurveyWAMTaskRating1VM();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    { taskName = (string) Session["CurrTaskNameOther"]; }

                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    
                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;
                    v.TaskName = taskName;

                    int pv = (int) Session["ProgressValueValue"];
                    int surveyDuration = (int) Session["SurveyDuration"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    if (Session["GoToReset"] == null)
                    { Session["GoToReset"] = false; }

                    bool resetGoto = (bool) Session["GoToReset"];
                    if (Session["NumberOfRounds"] == null)
                    { Session["NumberOfRounds"] = 1;}
                    else
                    {
                        if (resetGoto == false)
                        {
                            int x = (int) Session["NumberOfRounds"];
                            x++;
                            Session["NumberOfRounds"] = x;
                        }
                    }
                    int numRounds = (int) Session["NumberOfRounds"];
                    
                    decimal proportion = (decimal) ((decimal) remainingDuration / (decimal) surveyDuration);
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
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMTaskRating1(SurveyWAMTaskRating1VM v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];

                    Session["GoToReset"] = false;

                    string taskOther = (string) Session["CurrTaskNameOther"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

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

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

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
                    return RedirectToAction("WAMTaskRating2");
                }
                catch (Exception ex)
                {
                    string EMsg = "TaskRating1 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> WAMTaskRating2()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskRating2VM v = new SurveyWAMTaskRating2VM();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    { taskName = (string) Session["CurrTaskNameOther"]; }
                    
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

                    int pv = (int) Session["ProgressValueValue"];
                    int surveyDuration = (int) Session["SurveyDuration"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    int numRounds = (int) Session["NumberOfRounds"];
                    bool resetGoto = (bool) Session["GoToReset"];
                    decimal proportion = (decimal) ((decimal) remainingDuration / (decimal) surveyDuration);
                    decimal allocationSpanPercentage = 1 - proportion;
                    decimal remainingLength = 75 - pv;
                    Session["ProgressValueValue"] = surveyService.CalProgressValRating2(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);
                    
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "WAMTaskRating2 GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "WAMTaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMTaskRating2(SurveyWAMTaskRating2VM v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    string taskOther = (string) Session["CurrTaskNameOther"];

                    Session["GoToReset"] = false;


                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTime"];
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];
                    
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
                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

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
                        return RedirectToAction("WAMAddShiftTime", new { v.Uid });
                    }
                    else
                    {
                        Session["CurrTaskNameOther"] = null;
                        currRound++;
                        Session["CurrRound"] = currRound; //this is to say that it is the next round
                        Session["CurrTaskStartTime"] = Session["NextTaskStartTime"];
                        currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];

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
                        return RedirectToAction("WAMTasks");
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "WAMTaskRating2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            { return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> WAMAddShiftTime(string uid)
        {
            try
            {
                ResumeSurveyMV v = new ResumeSurveyMV();

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                DateTime? surveyExpiryDate = (DateTime?) Session["SurveyExpiryDate"];

                //v.SurveyProgressNext = (string) Session["SurveyProgressNext"];
                v.SurveyProgressNext = "WAMAddTasks";
                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                Session["ProgressValueValue"] = 80;

                if (!string.IsNullOrEmpty(uid))
                {
                    v.Uid = uid;
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    else
                    { return View(v);}
                }
                await LogMyDayError(uid, "WAMAddShiftTime GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "WAMAddShiftTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WAMAddShiftTime(ResumeSurveyMV v, string button)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];
                    int remainingDuration = (int) Session["RemainingDuration"];
                    if (button == "Yes")
                    {
                       surveyService.SetWAMAdditionalAsyncPOST(surveyId, currRound, remainingDuration, currTaskEndTime,
                                              currTaskStartTime, currTask, nextTaskStartTime, firstQuestion,
                                              Constants.StatusSurveyProgress.WAMAddTasks.ToString() );
                        return RedirectToAction("WAMAddTasks");
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
                    string EMsg = "WAMAddShiftTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
        /* Continue or restart Page upon hit Refresh   */
        [HttpGet]
        public async Task<ActionResult> WAMSID(string id)
        {
            try
            {
                SidMV v = new SidMV();
                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.Enter, profileId, "Uid = " + id);
                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                if (!string.IsNullOrEmpty(id))
                {
                    v.Uid = id;
                    if (Request.IsAjaxRequest())
                    { return PartialView(v);}
                    else
                    {  return View(v);}
                }
                else
                {
                    await LogMyDayError(id, "SID Task: Survey UID not found!", "InvalidError");
                    return RedirectToAction("InvalidError");
                }
            }
            catch (Exception ex)
            {
                string EMsg = "WAMSID:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(id, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMSID(SidMV v, string button)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    await LoadSurveySessionsBySurveyId(surveyId);
                    if (button == "Restart")
                    {
                        Session["GoToReset"] = false;
                        return RedirectToAction("WAMTasks");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(v.Uid))
                        {
                            Session["GoToReset"] = true;
                            v.SurveyProgressNext = (string) Session["SurveyProgressNext"];
                            return RedirectToAction(v.SurveyProgressNext, new { v.Uid });
                        }
                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "WAMSID POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {return PartialView();}
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> WAMAddTasks()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTasks1ViewModel v = new SurveyTasks1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();
                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);
                    //var result = surveyService.GetAllTask();
                    var result = surveyService.GetAllTaskByType("WAM");
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
                    int pv = (int) Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddTasks(pv, true);

                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                await LogMyDayError(suid, "WAMAddTasks GET: Survey UID not found!", "InvalidError");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "WAMAddTasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMAddTasks(SurveyTasks1ViewModel v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
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
                        taskId = (int) selectedTasks[0];
                        Session["CurrTaskNameOther"] = null;
                        task = surveyService.GetTaskByTaskId(taskId);
                        if (selectedTasks == null || selectedTasks.Length <= 0)
                        {   return RedirectToAction("TaskNotSelectedError"); }
                    }

                    Session["AddTaskId"] = task.Id;
                    Session["CurrTaskName"] = null;
                    Session["AddCurrTaskName"] = task.ShortName;                    

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    surveyService.SetAdditionalTasksAsyncPOST(surveyId, currRound, remainingDuration, currTaskEndTime,
                            currTaskStartTime, currTask, nextTaskStartTime, firstQuestion,
                            Constants.StatusSurveyProgress.WAMAddTaskTime.ToString(), task.Id);
                    
                    return RedirectToAction("WAMAddTaskTime");
                }
                catch (Exception ex)
                {
                    string EMsg = "WAMAddTasks POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {  return PartialView(v); }
            return View(v);
        }
        [HttpGet]
        public async Task<ActionResult> WAMAddTaskTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyWAMTaskTimeVM v = new SurveyWAMTaskTimeVM();

                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];

                    //int taskId = (int) Session["CurrTask"];
                    int taskId = (int) Session["AddTaskId"];

                    string uid = (string) Session["SurveyUID"];
                    //int remainingDuration = (int) Session["RemainingDuration"];
                    suid = uid;
                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Enter);
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    string taskName = (string) Session["AddCurrTaskName"];


                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }


                    v.ShiftSpan = shiftSpan;
                    v.Uid = uid;
                    v.SurveySpan = surveySpan;
                    
                    v.TaskId = taskId;
                    v.TaskName = taskName;

                    DateTime surveyStart = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyEnd = (DateTime) Session["SurveyEndTime"];
                    TimeSpan difference = surveyEnd - surveyStart;
                    double remainingDuration = difference.TotalMinutes;

                    v.TimeHours = (int) (remainingDuration / 60);
                    v.TimeMinutes = (int) (remainingDuration % 60);

                    v.remainingTimeHours = (int) (remainingDuration / 60);
                    v.remainingTimeMinutes = (int) (remainingDuration % 60);

                    v.OptionsList = surveyService.GetWAMTaskTimeOptions();

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);
                }
                await LogMyDayError(suid, "TaskRating2 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMAddTaskTime(SurveyWAMTaskTimeVM v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                    int profileId = (int) Session["ProfileId"];
                    int addTaskId = (int) Session["AddTaskId"];
                    string taskOther = (string) Session["CurrTaskNameOther"];
                    await pageStatSvc.Insert(surveyId, addTaskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Enter);

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

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

                    int pv = (int) Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddTaskTime(pv, true);

                    #region save status to survey

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

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

                    return RedirectToAction("WAMAddTaskRating1");
                }
                catch (Exception ex)
                {
                    string EMsg = "WAMAddTaskTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post);
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
        public async Task<ActionResult> WAMAddTaskRating1()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating1ViewModel v = new SurveyTaskRating1ViewModel();

                    int surveyId = (int) Session["SurveyId"];

                    int profileId = (int) Session["ProfileId"];

                    int addTaskId = (int) Session["AddTaskId"];

                    int taskId = (int) Session["CurrTask"];


                    //string taskName = (string) Session["CurrTaskName"];
                    string taskName = (string) Session["AddCurrTaskName"];


                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }



                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

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

                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Exit, profileId);

                    int pv = (int) Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddRating1(pv, 1);
                    
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);
                }
                await LogMyDayError(suid, "WAMAddTaskRating1 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "WAMAddTaskRating1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMAddTaskRating1(SurveyTaskRating1ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int addTaskId = (int) Session["AddTaskId"];

                    string taskOther = (string) Session["CurrTaskNameOther"];
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

                    await pageStatSvc.Insert(surveyId, addTaskId, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

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

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];



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

                    await pageStatSvc.Insert(surveyId, addTaskId, null, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("WAMAddTaskRating2");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskRating POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post);
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
        public async Task<ActionResult> WAMAddTaskRating2()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating2ViewModel v = new SurveyTaskRating2ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int addTaskId = (int) Session["AddTaskId"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];                    
                    string taskName = (string) Session["AddCurrTaskName"];

                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }

                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Enter);
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
                    int pv = (int) Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddRating2(pv, 1);

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);
                }
                await LogMyDayError(suid, "AddTaskRating2 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "AddTaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMAddTaskRating2(SurveyTaskRating2ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int addTaskId = (int) Session["AddTaskId"];
                    string taskOther = (string) Session["CurrTaskNameOther"];
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

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
                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

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
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }

        public async Task<ActionResult> WAMEditResponse(int taskId, DateTime taskStartDate)
        {
            string suid = string.Empty;
            try
            {
                SurveyEditResponseViewModel v = new SurveyEditResponseViewModel();

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];

                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;

                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>) Session["TaskList"];
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
                {  v.TaskName = response.TaskOther; }

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
                { v.Q1Ans = Constants.NA_7Rating;}

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
                {  v.Q5Ans = Constants.NA_7Rating; }

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
                {  v.Q9Ans = Constants.NA_7Rating; }

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
                var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMEditResponse(SurveyEditResponseViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

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

                    return RedirectToAction("WAMSurveyResults");


                }
                catch (Exception ex)
                {
                    string EMsg = "EditResponse POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        public async Task<ActionResult> WAMAddEditResponse(int taskId)
        {
            try
            {
                SurveyEditResponseViewModel v = new SurveyEditResponseViewModel();

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];

                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;

                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>) Session["TaskList"];
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
                {  v.Q8Ans = Constants.NA_7Rating; }

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
                var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMAddEditResponse(SurveyEditResponseViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

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

                    return RedirectToAction("WAMSurveyResults");
                }
                catch (Exception ex)
                {
                    string EMsg = "WAMAddEditResponse POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {  return PartialView(v); }
            return View(v);
        }
        #endregion

        #region New MyDay Tasks Representation
        [HttpGet]
        public async Task<ActionResult> MultiTask()
        {
            string suid = string.Empty;

            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyMultiTaskVM v = new SurveyMultiTaskVM();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();
                    string taskType = Session["TaskType"].ToString();

                    int currProgressValueValue = (int) Session["ProgressValueValue"];

                    if (demoForUser == "myday" || demoForUser == "wam")
                    {
                        if (currProgressValueValue <= 15)
                        { Session["ProgressValueValue"] = 15; }
                    }
                    else if (demoForUser == "mydayv2")
                    { Session["ProgressValueValue"] = 20; }
                    else {
                        if (currProgressValueValue <= 15)
                        { Session["ProgressValueValue"] = 15; }
                    }
                    

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var z = db.Surveys
                                .Where(u => u.Id == surveyId && u.ProfileId == profileId)
                                .Select(u => u).First();
                    Session["StartDate"] = Convert.ToDateTime(z.SurveyWindowStartDateTime).ToLongDateString();

                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);
                    

                    var mstData = db.MasterDataS.Select(u => u).FirstOrDefault();
                    v.TotalTaskSelectionLimit = mstData.RecurrentSurveyTaskSelectionLimit;

                    var MyDayTasksExists = db.MyDayTasks
                            .Where(c => c.ProfileId == profileId)
                            .Select(c => c.TaskId).ToList();

                    if (MyDayTasksExists.Count == 0)
                    {
                        v.AllTaskItemsObj = db.TaskItems.Where(c => c.Type == "Patient")
                                                        .Select(m => new AlltheTasks()
                                                        {
                                                            Id = m.Id,
                                                            ShortName = m.ShortName,
                                                        }).ToList();
                    }
                    else
                    {
                        v.AllTaskItemsObj = db.TaskItems.Where(c => !MyDayTasksExists.Contains(c.Id))
                                                .Where(c => c.Type == "Patient")
                                                .Select(m => new AlltheTasks()
                                                {
                                                    Id = m.Id,
                                                    ShortName = m.ShortName,
                                                }).ToList();
                    }
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }

                    return View(v);
                }
                await LogMyDayError(suid, "Tasks GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "Tasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        //To fill up the dropdownlist with the non-selected tasks -- added by Bharati
        public JsonResult selectedListBox()
        {
            int profileId = (int) Session["ProfileId"];           

            var profileTasks = db.MyDayTasks
                .Where(c => c.ProfileId == profileId)
                .Select(c => c.TaskId).ToList();

            var SelectedTasks = db.TaskItems.Where(c => profileTasks.Contains(c.Id))
                                    .Where(c => c.Type == "Patient")
                                    .Select(m => new TaskCascade()
                                    {
                                        Id = m.Id,
                                        ShortName = m.ShortName,
                                    }).ToList();

            return Json(SelectedTasks, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> MultiTask(SurveyMultiTaskVM v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    IList<string> selectedTaskIds = v.HiddenSelectedTasksIds.Split(',').Reverse().ToList<string>();

                    MyDayTaskList newSurveyTasks = new MyDayTaskList();
                    var profileTasks = db.MyDayTasks
                                         .Where(c => c.ProfileId == profileId)
                                         .Select(c => c.TaskId).ToList();

                    if (profileTasks.Count > 0)
                    {
                        await surveyService.DeleteMyDayTasksByProfileId(profileId);
                    }

                    foreach (var i in selectedTaskIds)
                    {
                        newSurveyTasks.ProfileId = profileId;
                        newSurveyTasks.SurveyId = surveyId;
                        newSurveyTasks.TaskId = Convert.ToInt32(i.ToString());
                        newSurveyTasks.TaskStartDateCurrentTime = DateTime.UtcNow;
                        newSurveyTasks.TaskStartDateTimeUtc = DateTime.Now;
                        newSurveyTasks.TaskName = surveyService.GetTaskByTaskId(Convert.ToInt32(i.ToString())).ShortName.ToString();
                        //newSurveyTasks.TaskCategoryId = 
                        db.MyDayTasks.Add(newSurveyTasks);
                        await db.SaveChangesAsync();
                    } 
                    return RedirectToAction("TaskView");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
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
        public async Task<ActionResult> MultiTaskCopy()
        {
            string suid = string.Empty;

            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyMultiTaskVM v = new SurveyMultiTaskVM();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;

                    int currProgressValueValue = (int) Session["ProgressValueValue"];

                    if (currProgressValueValue <= 15)
                    { Session["ProgressValueValue"] = 15; }

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var z = db.Surveys
                                .Where(u => u.Id == surveyId && u.ProfileId == profileId)
                                .Select(u => u).First();
                    Session["StartDate"] = Convert.ToDateTime(z.SurveyWindowStartDateTime).ToLongDateString();

                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();
                    string taskType = Session["TaskType"].ToString();

                    //GetAllTaskByType
                    IList<TaskVM> result;

                    if (taskType == "WAM")
                    { result = surveyService.GetAllTaskByType(taskType); }
                    else if (taskType == "doctors")
                    { result = surveyService.GetAllTaskByType("Patient"); }
                    else { result = surveyService.GetAllTaskByType("Patient"); }
                   
                    //Generating JSON Data File
                    var tasklist = result.ToList();
                    // Pass the "personlist" object for conversion object to JSON string  
                    string jsondata = new JavaScriptSerializer().Serialize(tasklist);
                    string path = Server.MapPath("~/data/");
                    // Write that JSON to txt file, 
                    if (taskType == "WAM")
                    { System.IO.File.WriteAllText(path + "WAM.json", jsondata); }
                    else if (taskType == "doctors")
                    { System.IO.File.WriteAllText(path + "doctors.json", jsondata); }
                    else { System.IO.File.WriteAllText(path + "doctors.json", jsondata); }

                    //System.IO.File.WriteAllText(path + "output.json", jsondata);                    
                    
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }

                    return View(v);
                }
                await LogMyDayError(suid, "Tasks GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "Tasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> MultiTaskCopy(SurveyMultiTaskVM v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    IList<string> selectedTaskIds = v.HiddenSelectedTasksIds.Split(',').Reverse().ToList<string>();

                    MyDayTaskList newSurveyTasks = new MyDayTaskList();
                    foreach (var i in selectedTaskIds)
                    {
                        newSurveyTasks.ProfileId = profileId;
                        newSurveyTasks.SurveyId = surveyId;
                        newSurveyTasks.TaskId = Convert.ToInt32(i.ToString());
                        newSurveyTasks.TaskStartDateCurrentTime = DateTime.UtcNow;
                        newSurveyTasks.TaskStartDateTimeUtc = DateTime.Now;
                        newSurveyTasks.TaskName = surveyService.GetTaskByTaskId(Convert.ToInt32(i.ToString())).ShortName.ToString();
                        //newSurveyTasks.TaskCategoryId = 
                        db.MyDayTasks.Add(newSurveyTasks);
                        await db.SaveChangesAsync();
                    }
                    return RedirectToAction("TaskView");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
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
        public async Task<ActionResult> TaskView()
        {
            if (ModelState.IsValid)
            {
               
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    string demoForUser = Session["DemoForUser"].ToString().ToLower();

                    if (demoForUser == "mydayv2")
                    { Session["ProgressValueValue"] = 30; }

                    //int pv = (int) Session["ProgressValueValue"];

                    //if (pv <= 15)
                    //{
                    //    Session["ProgressValueValue"] = surveyService.CalProgressValTaskTime(pv, true);
                    //}
                    var mstData = db.MasterDataS.Select(u => u).FirstOrDefault();
                    
                    SurveyMyTaskViewVM objTaskViewVM = new SurveyMyTaskViewVM();
                    objTaskViewVM.TaskListsObj = new List<MyDayTaskList>();
                    objTaskViewVM.TaskListsObj = surveyService.GetAllTaskByProfileID(profileId).ToList<MyDayTaskList>();
                    objTaskViewVM.totalRowCount = objTaskViewVM.TaskListsObj.Count();

                    objTaskViewVM.SurveySpan = surveySpan;
                    objTaskViewVM.TotalTaskHoursAlloted = (mstData.RecurrentSurveyTimeSlot * 60);

                    return View(objTaskViewVM);
                }
                catch (Exception ex) {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }     
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> TaskView(string TableData)
        {
            Session["AS"] = 2;
            return RedirectToAction("AffectStage1");
            //return View();
        }
        //For selection of three random tasks -- added by Bharati 
        public async Task<ActionResult> RandomTaskSelection()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int profileId = (int) Session["ProfileId"];
                    IList<MyDayTaskList> result;
                    result = surveyService.GetAllTaskByProfileID(profileId);

                    var totalTaskSelectionlimit = db.MasterDataS.Select(u => u).FirstOrDefault();

                    Session["TaskLimit"] = totalTaskSelectionlimit.RecurrentSurveyTaskSelectionLimit;

                    IList<MyDayTaskList> GetThree = RandomTaskGenerationService
                                                    .GetRandom(result, totalTaskSelectionlimit.RecurrentSurveyTaskSelectionLimit)
                                                    .ToList();
                    foreach (var i in GetThree)
                    {
                        var RandM = db.MyDayTasks.Where(u => u.ProfileId == profileId && u.Id == i.Id).First();
                        RandM.IsRandomlySelected = true;
                        db.Entry(RandM).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    Session["AS"] = 1;
                    return RedirectToAction("AffectStage1");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }        
        [HttpGet]
        public async Task<ActionResult> AffectStage1()
        {
            if (ModelState.IsValid)
            {
                string suid = string.Empty;
                int taskId;
                string taskName = string.Empty;

                try
                {
                    if (Session["SurveyId"] != null)
                    {
                        SurveyTaskRating1ViewModel v = new SurveyTaskRating1ViewModel();
                        
                        int surveyId = (int) Session["SurveyId"];
                        int profileId = (int) Session["ProfileId"];
                        
                        List<MyDayTaskList> result;
                        result = surveyService.GetRandomlySelectedTasksByProfileID(profileId, surveyId);

                        var responseAffectTask = result.First();
                        taskId = responseAffectTask.TaskId;
                        Session["CurrTask"] = taskId;
                        taskName = responseAffectTask.TaskName;
                        Session["CurrTaskName"] = taskName;

                        #region "For displaying Popup content"

                        v.NextTaskName = taskName;
                        if (result.Count() == 3)
                        {
                            v.DisplayPara1 = "Thank you for telling us about your workday. Next we would like to ask you about three of your tasks in more detail.";
                            v.DisplayPara2 = "First, we will ask you about ";
                            v.TaskRound = 1; }
                        else if (result.Count() == 1)
                        {
                            v.DisplayPara1 = "";
                            v.DisplayPara2 = "Lastly, we will ask you about ";
                            v.TaskRound++;
                        }
                        else {
                            v.DisplayPara1 = "";
                            v.DisplayPara2 = "Next, we will ask you about ";
                            v.TaskRound++; }

                        #endregion 

                        DateTime startTime = (DateTime) responseAffectTask.TaskStartDateTimeUtc;
                        string uid = (string) Session["SurveyUID"];
                        suid = uid;
                        string shiftSpan = (string) Session["ShiftSpan"];
                        string surveySpan = (string) Session["SurveySpan"];

                        v.Uid = uid;
                        v.ShiftSpan = shiftSpan;
                        v.SurveySpan = surveySpan;
                        v.PageStartDateTimeUtc = DateTime.UtcNow;
                        v.TaskId = taskId;
                        v.TaskName = taskName;

                        var z = db.MyDayTasks
                                  .Where(u => u.TaskId == v.TaskId && u.ProfileId == profileId)
                                  .Select(u => u).First();
                        Session["CurrentTaskDuration"] = z.TaskDuration.ToString();

                        #region "ProgressBar Calculations"

                        if (Session["AS"].ToString() == "2")
                        {
                            int tasklimit = (int) Session["TaskLimit"];
                            //currentaffect used to store the current round of affect stage
                            int currentaffect = 0;
                            if (Session["CurrentAffectStage"] != null && Session["CurrentAffectStage"].ToString() != string.Empty)
                            { currentaffect = (int) Session["CurrentAffectStage"]; }
                            //if (string.IsNullOrEmpty(Session["CurrentAffectStage"] as string))
                            //{ currentaffect = (int) Session["CurrentAffectStage"]; }
                            else { Session["CurrentAffectStage"] = 1; currentaffect = (int) Session["CurrentAffectStage"]; } //initializing it to first round on first attempt

                            int pv = (int) Session["ProgressValueValue"];

                            var c = db.MyDayTasks.Where(u => u.ProfileId == profileId)
                                                 .Select(u => u).ToList();

                            if (c.Count() == 1)
                            {                                
                               Session["ProgressValueValue"] = 75;                              
                            }
                            else if (c.Count() == 2)
                            {
                                if (currentaffect == 1)
                                { Session["ProgressValueValue"] = 60; }
                                else if (currentaffect == 2)
                                { Session["ProgressValueValue"] = pv + 30; }
                            }
                            else
                            {
                                if (tasklimit == 3)
                                {
                                    if (currentaffect == 1)
                                    { Session["ProgressValueValue"] = 40; }
                                    else if (currentaffect == 2 || currentaffect == 3)
                                    { Session["ProgressValueValue"] = pv + 20; }
                                }
                                else if (tasklimit == 4 || tasklimit == 5)
                                {
                                    if (currentaffect == 1)
                                    { Session["ProgressValueValue"] = 40; }
                                    else if (currentaffect >= 2 || currentaffect <= 5)
                                    { Session["ProgressValueValue"] = pv + 10; }
                                }
                                else
                                {
                                    #region Earlier Calc

                                    var masterData = db.MasterDataS.Select(w => w).First();
                                    int totalDurationofSurvey = (masterData.RecurrentSurveyTimeSlot * 60); //converted to mins

                                    var calcRemDuration = db.MyDayTasks
                                                            .Where(a => a.ProfileId == profileId && a.IsRandomlySelected == true && a.IsAffectStageCompleted == false)
                                                            .Select(a => a).ToList();
                                    foreach (var b in calcRemDuration)
                                    {
                                        int j;
                                        if (Session["RemDura"] == null)
                                        { j = 0; }
                                        else { j = Convert.ToInt32(Session["RemDura"].ToString()); }
                                        j = b.TaskDuration + j;
                                        Session["RemDura"] = j;
                                    }
                                    int remainingDuration = (int) Session["RemDura"];
                                    if (Session["GoToReset"] == null)
                                    { Session["GoToReset"] = false; }
                                    bool resetGoto = (bool) Session["GoToReset"];
                                    if (Session["NumberOfRounds"] == null)
                                    { Session["NumberOfRounds"] = 1; }
                                    else
                                    {
                                        if (resetGoto == false)
                                        {
                                            int x = (int) Session["NumberOfRounds"];
                                            x++;
                                            Session["NumberOfRounds"] = x;
                                        }
                                    }
                                    int numRounds = (int) Session["NumberOfRounds"];
                                    decimal proportion = (decimal) ((decimal) remainingDuration / (decimal) totalDurationofSurvey);
                                    decimal allocationSpanPercentage = 1 - proportion;
                                    decimal remainingLength = 75 - pv;
                                    Session["ProgressValueValue"] = surveyService.CalProgressValRating1(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);

                                    #endregion
                                }
                            }
                        }
                            #endregion

                            v.Q1Ans = Constants.NA_7Rating;
                        v.Q2Ans = Constants.NA_7Rating;
                        v.Q3Ans = Constants.NA_7Rating;
                        v.Q4Ans = Constants.NA_7Rating;
                        v.Q5Ans = Constants.NA_7Rating;

                        if (Request.IsAjaxRequest())
                        {
                            return PartialView(v);
                        }
                        return View(v);
                }
                    await LogMyDayError(suid, "AffectStage1 GET: Survey UID not found!", "InvalidError");
                    await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                    return RedirectToAction("InvalidError");
                }
                catch (Exception ex)
                {
                    string EMsg = "AffectStage1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(suid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get);
                    return RedirectToAction("SurveyError");
                }
            }
             return View();
        }
        [HttpPost]
        public async Task<ActionResult> AffectStage1(SurveyTaskRating1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    
                    Session["GoToReset"] = false;
                    
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    #region Save  Responses
                    //Q2
                    ResponseAffectDto r1 = new ResponseAffectDto();
                    r1.SurveyId = surveyId;
                    r1.TaskId = taskId;
                    r1.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r1);

                    ResponseAffectDto r2 = new ResponseAffectDto();
                    r2.SurveyId = surveyId;
                    r2.TaskId = taskId;
                    r2.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r2);

                    ResponseAffectDto r3 = new ResponseAffectDto();
                    r3.SurveyId = surveyId;
                    r3.TaskId = taskId;
                    r3.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r3);

                    ResponseAffectDto r4 = new ResponseAffectDto();
                    r4.SurveyId = surveyId;
                    r4.TaskId = taskId;
                    r4.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r4);

                    ResponseAffectDto r5 = new ResponseAffectDto();
                    r5.SurveyId = surveyId;
                    r5.TaskId = taskId;
                    r5.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r5);

                    #endregion

                    surveyService.SaveResponseAffect();

                    #region save status to survey
                    
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];
                    
                    #endregion
                                        
                    return RedirectToAction("AffectStage2");
                }
                catch (Exception ex)
                {
                    string EMsg = "AffectStage1 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post);
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
        public async Task<ActionResult> AffectStage2()
        {
            if (ModelState.IsValid)
            {
                string suid = string.Empty;
                try
                {
                    if (Session["SurveyId"] != null)
                    {
                        SurveyTaskRating2ViewModel v = new SurveyTaskRating2ViewModel();

                        int surveyId = (int) Session["SurveyId"];
                        int profileId = (int) Session["ProfileId"];
                        int taskId = (int) Session["CurrTask"];
                        DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                        string uid = (string) Session["SurveyUID"];
                        suid = uid;
                        string shiftSpan = (string) Session["ShiftSpan"];
                        string surveySpan = (string) Session["SurveySpan"];
                        string taskName = (string) Session["CurrTaskName"];
                        
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

                        #region ProgressBar Calculations

                        int currentaffect;
                        //int tasklimit = (int) Session["TaskLimit"];

                        //if (string.IsNullOrEmpty(Session["CurrentAffectStage"] as string))
                        {
                            currentaffect = (int) Session["CurrentAffectStage"];
                            currentaffect++;
                            Session["CurrentAffectStage"] = currentaffect;

                            //if (tasklimit <= 3)
                            //{
                            //    if (currentaffect == 0 || currentaffect == 1)
                            //    {
                            //        Session["ProgressValueValue"] = 40;
                            //    }
                            //    else if (currentaffect == 2 || currentaffect == 3)
                            //    {
                            //        Session["ProgressValueValue"] = pv + 20;
                            //    }
                            //}
                            //else if (tasklimit == 5)
                            //{
                            //    if (currentaffect == 1)
                            //    {
                            //        Session["ProgressValueValue"] = 40;
                            //    }
                            //    else if (currentaffect >= 2 || currentaffect <= 5)
                            //    {
                            //        Session["ProgressValueValue"] = pv + 10;
                            //    }
                            //}
                            //else
                            //{
                            //    #region Earlier Calc

                            //    int pv = (int) Session["ProgressValueValue"];
                            //    int surveyDuration = (int) Session["SurveyDuration"];
                            //    int remainingDuration = (int) Session["RemainingDuration"];
                            //    int numRounds = (int) Session["NumberOfRounds"];
                            //    bool resetGoto = (bool) Session["GoToReset"];

                            //    decimal proportion = (decimal) ((decimal) remainingDuration / (decimal) surveyDuration);
                            //    decimal allocationSpanPercentage = 1 - proportion;
                            //    decimal remainingLength = 75 - pv;
                            //    Session["ProgressValueValue"] = surveyService.CalProgressValRating2(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);

                            //    #endregion
                            //}
                        }
                        
                        #endregion

                        if (Request.IsAjaxRequest())
                        {
                            return PartialView(v);
                        }
                        return View(v);
                    }
                    await LogMyDayError(suid, "AffectStage2 GET: Survey UID not found!", "InvalidError");
                    await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                    return RedirectToAction("InvalidError");
                }
                catch (Exception ex)
                {
                    string EMsg = "AffectStage2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    await LogMyDayError(suid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get);
                    return RedirectToAction("SurveyError");
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AffectStage2(SurveyTaskRating2ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    Session["GoToReset"] = false;
                    Session["RemDura"] = null;
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTime"];
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    #region "To save data to ResponseAffects"

                    ResponseAffectDto r6 = new ResponseAffectDto();
                    r6.SurveyId = surveyId;
                    r6.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r6);

                    ResponseAffectDto r7 = new ResponseAffectDto();
                    r7.SurveyId = surveyId;
                    r7.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r7);

                    ResponseAffectDto r8 = new ResponseAffectDto();
                    r8.SurveyId = surveyId;
                    r8.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r8);

                    ResponseAffectDto r9 = new ResponseAffectDto();
                    r9.SurveyId = surveyId;
                    r9.ProfileId = profileId;
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
                    surveyService.AddResponseAffect(r9);

                    ResponseAffectDto r10 = new ResponseAffectDto();
                    r10.SurveyId = surveyId;
                    r10.ProfileId = profileId;
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

                    #endregion

                    surveyService.AddResponseAffect(r10);
                    surveyService.SaveResponseAffect();                   
                                        
                    int currTask = (int) Session["CurrTask"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

                    //To update the Emotional Affect stage completed to true
                    var myDayTL = db.MyDayTasks
                                    .Where(w => w.TaskId == taskId 
                                                && w.ProfileId == profileId 
                                                && w.SurveyId == surveyId)
                                    .First();
                    myDayTL.IsAffectStageCompleted = true;
                    db.Entry(myDayTL).State = EntityState.Modified;                    
                    await db.SaveChangesAsync();
                   
                    //To get the remaining emotional affect stages pending to be filled by the user
                    var totalAffectStageCompleted = db.MyDayTasks
                                                      .Where(u => u.ProfileId == profileId
                                                                    && u.SurveyId == surveyId
                                                                    && u.IsAffectStageCompleted == false
                                                                    && u.IsRandomlySelected == true)
                                                      .Select( u => u).ToList();
                    //To get the task selection limit set in master page
                    var mstData = db.MasterDataS.Select(b => b).FirstOrDefault();
                    
                    //To check if the emotional affect stage is not completed for particular task
                    //then redirect user to the emotional affect page or else redirect to feedback page upon completion
                    if (totalAffectStageCompleted.Count() < mstData.RecurrentSurveyTaskSelectionLimit 
                                && totalAffectStageCompleted.Count != 0)
                    {
                        return RedirectToAction("AffectStage1");
                    }
                    else { return RedirectToAction("Feedback"); }                    
                }
                catch (Exception ex)
                {
                    string EMsg = "AffectStage2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        
        //To fill up the dropdownlist with the non-selected tasks -- added by Bharati
        public JsonResult remainingTasksList()
        {
            int profileId = (int) Session["ProfileId"];
            var remtasks = db.TaskItems.Select(c => new TaskCascade { Id = c.Id, ShortName = c.ShortName}).ToList();
                        
            var AvailableTaskList = new List<Object>();
            foreach (var taskId in remtasks)
            {
                AvailableTaskList.Add(new {Id = taskId.Id, ShortName = taskId.ShortName });
            }

            var profileTasks = db.MyDayTasks
                .Where(c=> c.ProfileId == profileId)
                .Select(c => c.TaskId).ToList();

            var NotSelectedTasks = db.TaskItems.Where(c => !profileTasks.Contains(c.Id))
                                    .Where(c=> c.Type == "Patient")
                                    .Select(m => new TaskCascade() {
                                        Id = m.Id,
                                        ShortName = m.ShortName,
                                    }).ToList();

            return Json(NotSelectedTasks, JsonRequestBehavior.AllowGet);
        }
        //for selection of the task --added by Bharati
        public async Task<ActionResult> newTaskaddition(string taskId)
        {
            MyDayTaskList newSurveyTasks = new MyDayTaskList();
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    
                    newSurveyTasks.ProfileId = profileId;
                    newSurveyTasks.SurveyId = surveyId;
                    newSurveyTasks.TaskId = Convert.ToInt32(taskId);
                    newSurveyTasks.TaskStartDateCurrentTime = DateTime.UtcNow;
                    newSurveyTasks.TaskName = surveyService.GetTaskByTaskId(Convert.ToInt32(taskId)).ShortName.ToString();
                    newSurveyTasks.TaskStartDateTimeUtc = DateTime.Now;
                    //newSurveyTasks.TaskCategoryId = 
                    db.MyDayTasks.Add(newSurveyTasks);
                    await db.SaveChangesAsync();
                   
                    //return RedirectToAction("TaskView");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View("TaskView", newSurveyTasks);
        }
        //To delete the selected task --added by Bharati
        public async Task<ActionResult> deleteMeTask(string taskId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskIds = Convert.ToInt32(taskId);                   

                    var mt = db.MyDayTasks.Where(u => u.Id.Equals(taskIds)).FirstOrDefault();
                    db.MyDayTasks.Remove(mt);
                    db.SaveChanges();                                
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
        //To save the time spent in mins to the DB -- added by Bharati
        public async Task<ActionResult> addDuration(string TableData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int TaskDuration = 0;
                    int TaskId = 0;
                    IList<string> allTasks = TableData.Trim().Split(',').Reverse().ToList<string>(); 

                    for (int i = 0; i < allTasks.Count(); i++)
                    {
                        string TI = getBetween(allTasks[i], "T", "I");
                        TaskId = Convert.ToInt32(TI.Trim().ToString());

                        string TD = getBetween(allTasks[i], "D", "M");
                        TaskDuration = Convert.ToInt32(TD.Trim().ToString());                        

                        var TidDura = db.MyDayTasks.Where(u => u.Id  == TaskId).First();
                        TidDura.TaskDuration = TaskDuration;
                        db.Entry(TidDura).State = EntityState.Modified;
                        await db.SaveChangesAsync();                        
                    }
                    return RedirectToAction("RandomTaskSelection");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            return View();
        }
        //To get Elements present in between the string -- added by Bharati
        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
            //Example to test:
            //string text = "This is an example string and my data is here";
            //string data = getBetween(text, "my", "is");

        }
        public class TaskCascade
        {
            public int Id { get; set; }
            public string ShortName { get; set; }
            public string IsSelected { get; set; }
        }
        public async Task<ActionResult> EditResponseAffect(int taskId, DateTime taskStartDate)
        {
            string suid = string.Empty;
            try
            {
                SurveyEditResponseAffectVM v = new SurveyEditResponseAffectVM();

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];

                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;

                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>) Session["TaskList"];
                    task = taskList.Where(m => m.ID == taskId)
                                            .Select(m => new TaskItemDto()
                                            {
                                                Id = m.ID,
                                                LongName = m.LongName,
                                                ShortName = m.Name
                                            })
                                            .Single();
                }
                else { task = surveyService.GetTaskByTaskId(taskId); }
                v.TaskName = task.ShortName;

                Session["CurrTask"] = task.Id;
                Session["CurrTaskStartTime"] = taskStartDate;
                bool isAny = false;
                var listOfResponses = surveyService.GetAllResponseAffectAsync(taskId, profileId, surveyId, taskStartDate);
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
                else { v.Q1Ans = Constants.NA_7Rating; }
                response = listOfResponses.Where(t => t.Question == v.Q2DB).SingleOrDefault();

                if (response != null)
                { v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer); isAny = true; }
                else { v.Q2Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q3Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q4Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q5Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q6Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q7Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q8Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q9Ans = Constants.NA_7Rating; }

                response = listOfResponses.Where(t => t.Question == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else { v.Q10Ans = Constants.NA_7Rating; }

                if (isAny)
                { v.IsExist = true; }

                if (Request.IsAjaxRequest())
                { return PartialView(v); }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "EditResponse GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditResponseAffect(SurveyEditResponseViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

                    // Save to db
                    #region update Question  
                    var listOfResponses = surveyService.GetAllResponseAffectAsync(taskId, profileId, surveyId, startTime);
                    var response = listOfResponses.Where(r => r.Question == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        surveyService.UpdateResponseAffect(response);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        surveyService.UpdateResponseAffect(response);
                    }
                    surveyService.SaveResponse();
                    #endregion                    
                    return RedirectToAction("TaskSummary");
                }
                catch (Exception ex)
                {
                    string EMsg = "EditResponseAffect POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post);
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


        #region "MyDay - Main Recurrent Survey"

        [HttpGet]
        public async Task<ActionResult> Tasks()
        {
            string suid = string.Empty;

            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTasks1ViewModel v = new SurveyTasks1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;

                    int currProgressValueValue = (int) Session["ProgressValueValue"];

                    if (currProgressValueValue <= 15)
                    { Session["ProgressValueValue"] = 15; }

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();

                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);

                    string demoForUser = Session["DemoForUser"].ToString().ToLower();
                    string taskType = Session["TaskType"].ToString();

                    //GetAllTaskByType
                    IList<TaskVM> result;

                    if (taskType == "WAM")
                    {
                        result = surveyService.GetAllTaskByType(taskType);
                    }
                    else if (taskType == "doctors")
                    {
                        result = surveyService.GetAllTaskByType("Patient");
                    }
                    else { result = surveyService.GetAllTaskByType("Patient"); }

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
                                //IsWardRoundIndicator = k.IsWardRoundIndicator
                            });
                        }
                    }
                    v.FullTaskList = checkBoxListItems;

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }

                    return View(v);
                }
                await LogMyDayError(suid, "Tasks GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "Tasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> Tasks(SurveyTasks1ViewModel v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    if (Session["CurrRound"] == null)
                    { Session["CurrRound"] = 1; }

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

                    int taskId = 0;
                    bool isWRIndicatorTask = false;

                    TaskItemDto task = new TaskItemDto();

                    if (!string.IsNullOrEmpty(v.OtherTaskName))
                    {
                        task = surveyService.GetOtherTask();
                        Session["CurrTaskNameOther"] = v.OtherTaskName;
                    }
                    else
                    {
                        taskId = (int) selectedTasks[0];
                        Session["CurrTaskNameOther"] = null;
                        task = surveyService.GetTaskByTaskId(taskId);

                        if (selectedTasks == null || selectedTasks.Length <= 0)
                        {
                            await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.ERROR, null, "TaskNotFound");
                            return RedirectToAction("TaskNotSelectedError");
                        }
                        if (task.IsWardRoundTask.HasValue)
                            isWRIndicatorTask = true;
                    }

                    Session["CurrTask"] = task.Id;
                    Session["CurrTaskName"] = task.ShortName;

                    int currRound = (int) Session["CurrRound"];

                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];

                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await surveyService.SetTasksAsyncPOST(
                         surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        isWRIndicatorTask
                        );

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.Exit, profileId);

                    if (task.WardRoundIndicator == true)
                    {
                        return RedirectToAction("WRTaskTime");
                    }
                    return RedirectToAction("TaskTime");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
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
        public async Task<ActionResult> TaskTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskTime1ViewModel v = new SurveyTaskTime1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                    int taskId = (int) Session["CurrTask"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Enter);
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }

                    int pv = (int) Session["ProgressValueValue"];

                    if (pv <= 15)
                    {
                        Session["ProgressValueValue"] = surveyService.CalProgressValTaskTime(pv, true);
                    }



                    v.ShiftSpan = shiftSpan;
                    v.Uid = uid;
                    v.SurveySpan = surveySpan;

                    //TaskItemDto t = surveyService.GetTaskByTaskId(taskId);
                    //string taskName = await db.TaskItems
                    //                    .Where(t => t.Id == taskId)
                    //                    .Select(t => t.ShortName)
                    //                    .SingleOrDefaultAsync();

                    //v.TaskId = t.Id;
                    v.TaskId = taskId;


                    //v.TaskName = t.ShortName;
                    v.TaskName = taskName;

                    v.TimeHours = remainingDuration / 60;
                    v.TimeMinutes = remainingDuration % 60;

                    v.remainingTimeHours = remainingDuration / 60;
                    v.remainingTimeMinutes = remainingDuration % 60;

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);
                }
                await LogMyDayError(suid, "TaskTime: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "TaskTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> TaskTime(SurveyTaskTime1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    string taskOther = (string) Session["CurrTaskNameOther"];

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Enter);

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

                    int remainingDuration = (int) Session["RemainingDuration"];

                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];


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




                    ResponseDto r = new ResponseDto();
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
                    r.IsWardRoundTask = false;
                    r.WardRoundTaskId = null;
                    r.WardRoundWindowStartDateTime = null;
                    r.WardRoundWindowEndDateTime = null;

                    await surveyService.AddResponseAsync(r);

                    //await svc.InsertAsync(r, ModelState);


                    #region save status to survey
                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

                    await surveyService.SetTaskTimeAsyncPOST(
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

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("TaskRating1");

                    //Session["CurrProgressValue"] = shiftDuration - remainingDuration;

                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post);
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
        public async Task<ActionResult> TaskRating1()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating1ViewModel v = new SurveyTaskRating1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }

                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;
                    v.TaskName = taskName;


                    int pv = (int) Session["ProgressValueValue"];
                    int surveyDuration = (int) Session["SurveyDuration"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    if (Session["GoToReset"] == null)
                    {
                        Session["GoToReset"] = false;
                    }

                    bool resetGoto = (bool) Session["GoToReset"];


                    if (Session["NumberOfRounds"] == null)
                    {
                        Session["NumberOfRounds"] = 1;
                    }
                    else
                    {

                        if (resetGoto == false)
                        {
                            int x = (int) Session["NumberOfRounds"];
                            x++;
                            Session["NumberOfRounds"] = x;

                        }

                    }


                    int numRounds = (int) Session["NumberOfRounds"];

                    //if (remainingDuration == 0)
                    //{
                    //    Session["ProgressValueValue"] = surveyService.CalProgressValRating1(pv, 1, numRounds);
                    //}
                    //else
                    //{
                    decimal proportion = (decimal) ((decimal) remainingDuration / (decimal) surveyDuration);
                    decimal allocationSpanPercentage = 1 - proportion;
                    decimal remainingLength = 75 - pv;
                    Session["ProgressValueValue"] = surveyService.CalProgressValRating1(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);

                    //}


                    //((decimal) ((int) Session["SurveyDuration"] - (int) @Session["RemainingDuration"]) / (int) Session["SurveyDuration"]) * 100 )







                    //string taskName = await db.TaskItems
                    //                    .Where(t => t.Id == taskId)
                    //                    .Select(t => t.ShortName)
                    //                    .SingleOrDefaultAsync();



                    v.Q1Ans = Constants.NA_7Rating;
                    v.Q2Ans = Constants.NA_7Rating;
                    v.Q3Ans = Constants.NA_7Rating;
                    v.Q4Ans = Constants.NA_7Rating;
                    v.Q5Ans = Constants.NA_7Rating;

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Exit, profileId);

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);

                }
                await LogMyDayError(suid, "TaskRating1 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating1 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> TaskRating1(SurveyTaskRating1ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];

                    Session["GoToReset"] = false;

                    string taskOther = (string) Session["CurrTaskNameOther"];



                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

                    //Q2
                    ResponseDto r1 = new ResponseDto();
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

                    //svc.Insert(r1, ModelState);
                    //await svc.InsertAsync(r1, ModelState);
                    //await surveyService.AddResponseAsync(r1);
                    surveyService.AddResponse(r1);






                    ResponseDto r2 = new ResponseDto();
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

                    //await svc.InsertAsync(r2, ModelState);
                    //await surveyService.AddResponseAsync(r2);
                    surveyService.AddResponse(r2);


                    ResponseDto r3 = new ResponseDto();
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

                    //await svc.InsertAsync(r3, ModelState);
                    //await surveyService.AddResponseAsync(r3);
                    surveyService.AddResponse(r3);



                    ResponseDto r4 = new ResponseDto();
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

                    //await svc.InsertAsync(r4, ModelState);
                    //await surveyService.AddResponseAsync(r4);
                    surveyService.AddResponse(r4);


                    ResponseDto r5 = new ResponseDto();
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

                    //await svc.InsertAsync(r5, ModelState);
                    //await surveyService.AddResponseAsync(r5);
                    surveyService.AddResponse(r5);

                    surveyService.SaveResponse();



                    #region save status to survey

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

                    await surveyService.SetRating1AsyncPOST(
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion
                        );


                    //await SetRating1AsyncPOST(surveyId);



                    #endregion

                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("TaskRating2");
                }
                catch (Exception ex)
                {
                    string EMsg = "TaskRating1 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post);
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
        public async Task<ActionResult> TaskRating2()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating2ViewModel v = new SurveyTaskRating2ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }


                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Enter);
                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;



                    //string taskName = await db.TaskItems
                    //    .Where(t => t.Id == taskId)
                    //    .Select(t => t.ShortName)
                    //    .SingleOrDefaultAsync();



                    v.TaskName = taskName;

                    v.Q6Ans = Constants.NA_7Rating;
                    v.Q7Ans = Constants.NA_7Rating;
                    v.Q8Ans = Constants.NA_7Rating;
                    v.Q9Ans = Constants.NA_7Rating;
                    v.Q10Ans = Constants.NA_7Rating;

                    int pv = (int) Session["ProgressValueValue"];
                    //int remainingDuration = (int) Session["RemainingDuration"];

                    int surveyDuration = (int) Session["SurveyDuration"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    int numRounds = (int) Session["NumberOfRounds"];
                    bool resetGoto = (bool) Session["GoToReset"];

                    //if (remainingDuration == 0)
                    //{
                    //    Session["ProgressValueValue"] = surveyService.CalProgressValRating2(pv, 1, numRounds);

                    //}
                    //else
                    //{
                    decimal proportion = (decimal) ((decimal) remainingDuration / (decimal) surveyDuration);
                    decimal allocationSpanPercentage = 1 - proportion;
                    decimal remainingLength = 75 - pv;
                    Session["ProgressValueValue"] = surveyService.CalProgressValRating2(pv, allocationSpanPercentage, numRounds, resetGoto, remainingLength);

                    //}


                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);

                }
                await LogMyDayError(suid, "TaskRating2 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> TaskRating2(SurveyTaskRating2ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    string taskOther = (string) Session["CurrTaskNameOther"];

                    Session["GoToReset"] = false;


                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTime"];
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Enter);


                    //var listOfResponses = svc.GetAll(taskId, surveyId, startTime);
                    //var response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{
                    ResponseDto r6 = new ResponseDto();
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

                    //await svc.InsertAsync(r6, ModelState);
                    //await surveyService.AddResponseAsync(r6);
                    surveyService.AddResponse(r6);


                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{
                    ResponseDto r7 = new ResponseDto();
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

                    //await svc.InsertAsync(r7, ModelState);
                    //await surveyService.AddResponseAsync(r7);
                    surveyService.AddResponse(r7);

                    //}



                    //response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r8 = new ResponseDto();
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

                    //await svc.InsertAsync(r8, ModelState);
                    //await surveyService.AddResponseAsync(r8);
                    surveyService.AddResponse(r8);

                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r9 = new ResponseDto();
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

                    //await svc.InsertAsync(r9, ModelState);
                    //await surveyService.AddResponseAsync(r9);
                    surveyService.AddResponse(r9);

                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r10 = new ResponseDto();
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

                    //await svc.InsertAsync(r10, ModelState);
                    //await surveyService.AddResponseAsync(r10);
                    surveyService.AddResponse(r10);

                    surveyService.SaveResponse();
                    //}


                    //#region save status to survey
                    //var survey = db.Surveys.Where(x => x.Id == surveyId).Select(x => new { MaxStep = x.MaxStep }).SingleOrDefault();
                    //var tempSurvey = new Survey() { Id = surveyId };
                    //if (survey.MaxStep < 5)
                    //{
                    //    tempSurvey.MaxStep = 5;
                    //}
                    //tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.TaskRating2.ToString();
                    //db.Surveys.Attach(tempSurvey);
                    //db.Entry(tempSurvey).Property(x => x.SurveyProgressNext).IsModified = true;
                    //db.Entry(tempSurvey).Property(x => x.MaxStep).IsModified = true;
                    //db.SaveChangesAsync();
                    //#endregion

                    //------------

                    //stopping criteria
                    //update the duration sessions variables


                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];


                    if (remainingDuration <= 0)
                    {

                        surveyService.SetRating2AsyncPOSTResult(
                            surveyId,
                            currRound,
                            remainingDuration,
                            currTaskEndTime,
                            currTaskStartTime,
                            currTask,
                            nextTaskStartTime,
                            firstQuestion
                            );

                        //                    await surveyService.SetRating2AsyncPOSTResult(
                        //surveyId,
                        //currRound,
                        //remainingDuration,
                        //currTaskEndTime,
                        //currTaskStartTime,
                        //currTask,
                        //nextTaskStartTime,
                        //firstQuestion
                        //);


                        //await SetRating2AsyncPOSTResult(surveyId);

                        //SetReminderAsyncPOSTResult(surveyId);

                        await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Exit);
                        //return RedirectToAction("Results");
                        return RedirectToAction("AddShiftTime", new { v.Uid });



                    }
                    else
                    {
                        //await SetRating2AsyncPOSTNext(surveyId);

                        Session["CurrTaskNameOther"] = null;
                        currRound++;
                        Session["CurrRound"] = currRound; //this is to say that it is the next round
                        Session["CurrTaskStartTime"] = Session["NextTaskStartTime"];
                        currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];

                        await surveyService.SetRating2AsyncPOSTNext(
                         surveyId,
                         currRound,
                         remainingDuration,
                         currTaskEndTime,
                         currTaskStartTime,
                         currTask,
                         nextTaskStartTime,
                         firstQuestion
                         );

                        await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Exit);
                        return RedirectToAction("Tasks");
                    }
                    //Session["CurrProgressValue"] = ((int) Session["ShiftDuration"] - (int) Session["RemainingDuration"] ) / (int) Session["ShiftDuration"] ;
                    //Session["currProgressValue"] = (int) v.CurrProgressValue;
                }
                catch (Exception ex)
                {
                    string EMsg = "TaskRating2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post);
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

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];


                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;


                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>) Session["TaskList"];
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
                {
                    task = surveyService.GetTaskByTaskId(taskId);
                }

                //v.TaskName = db.TaskItems.Where(x => x.Id == taskId).SingleOrDefault().ShortName;
                v.TaskName = task.ShortName;


                Session["CurrTask"] = task.Id;
                Session["CurrTaskStartTime"] = taskStartDate;

                await pageStatSvc.Insert(surveyId, taskId, taskStartDate, true, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get, Constants.PageType.Enter);

                bool isAny = false;

                //v.TaskId = taskId;
                //v.TaskName = taskName;
                //v.TaskStartDateTime = taskStartDate;

                //var listOfResponses = await svc.GetAllAsync(taskId, surveyId, taskStartDate);
                var listOfResponses = surveyService.GetAllResponseAsync(taskId, profileId, surveyId, taskStartDate);
                var response = listOfResponses.Where(t => t.Question == v.Q1DB).SingleOrDefault();

                if (response.TaskOther != null)
                {
                    v.TaskName = response.TaskOther;
                }

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
                {
                    v.Q1Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q2DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q2Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q3Ans = Constants.NA_7Rating;
                }


                response = listOfResponses.Where(t => t.Question == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q4Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q5Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q6Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q7Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q8Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q9Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q10Ans = Constants.NA_7Rating;
                }


                if (isAny)
                {
                    v.IsExist = true;
                }

                await pageStatSvc.Insert(surveyId, taskId, taskStartDate, false, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get, Constants.PageType.Exit);

                if (Request.IsAjaxRequest())
                {
                    return PartialView(v);
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "EditResponse GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get);
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
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post, Constants.PageType.Enter);


                    //DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    //currentTaskList.Where(w => w.TaskId == v.TaskId).ToList().ForEach(s => s.IsCompleted = true);


                    // Save to db
                    #region update Question  
                    //var listOfResponses = await svc.GetAllAsync(taskId, surveyId, startTime);
                    var listOfResponses = surveyService.GetAllResponseAsync(taskId, profileId, surveyId, startTime);


                    var response = listOfResponses.Where(r => r.Question == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        surveyService.UpdateResponse(response);
                        //await svc.UpdateAsync(response, ModelState);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        surveyService.UpdateResponse(response);
                        //await svc.UpdateAsync(response, ModelState);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }


                    response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }


                    response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }

                    surveyService.SaveResponse();


                    #endregion

                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post, Constants.PageType.Exit);
                                        
                    if (Session["LoopDemo"].ToString() == "Yes")
                    { return RedirectToAction("LoopSurveyResults");}
                    else { return RedirectToAction("Results"); }
                }
                catch (Exception ex)
                {
                    string EMsg = "EditResponse POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int) Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }


        #region Additional Questions MyDay Junior Doctors Version

        [HttpGet]
        public async Task<ActionResult> AddShiftTime(string uid)
        {
            try
            {
                ResumeSurveyMV v = new ResumeSurveyMV();

                int surveyId = (int)Session["SurveyId"];
                int profileId = (int)Session["ProfileId"];
                DateTime? surveyExpiryDate = (DateTime?)Session["SurveyExpiryDate"];

                v.SurveyProgressNext = (string)Session["SurveyProgressNext"];
                v.ShiftSpan = (string)Session["ShiftSpan"];
                v.SurveySpan = (string)Session["SurveySpan"];

                Session["ProgressValueValue"] = 80;


                if (!string.IsNullOrEmpty(uid))
                {
                    v.Uid = uid;

                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    else
                    {
                        return View(v);
                    }
                }
                await LogMyDayError(uid, "AddShiftTime GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get, Constants.PageType.ERROR, profileId, "Error due to nextprogress being null");
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddShiftTime GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Get);
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




                    //await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post, Constants.PageType.Enter, profileId);
                    if (button == "Yes")
                    {


                        surveyService.SetAdditionalAsyncPOST(
                                              surveyId,
                                              currRound,
                                              remainingDuration,
                                              currTaskEndTime,
                                              currTaskStartTime,
                                              currTask,
                                              nextTaskStartTime,
                                              firstQuestion,
                                              Constants.StatusSurveyProgress.AddTasks.ToString()
                                              );


                        await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked Restart");
                        return RedirectToAction("AddTasks");
                    }
                    else
                    {
                        surveyService.SetAdditionalAsyncPOST(
                                             surveyId,
                                             currRound,
                                             remainingDuration,
                                             currTaskEndTime,
                                             currTaskStartTime,
                                             currTask,
                                             nextTaskStartTime,
                                             firstQuestion,
                                             //Change to Completed
                                             Constants.StatusSurveyProgress.Completed.ToString()
                                             //Constants.StatusSurveyProgress.AddTasks.ToString()
                                             );

                        ResetSessionVariables();

                        return RedirectToAction("Feedback");
                        //return RedirectToAction("Results");

                    }
                }
                catch (Exception ex)
                {
                    string EMsg = "AddShiftTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post);
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

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();

                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);

                    //var source = surveyService.GetAllTask();
                    //var rnd = new Random();
                    //var result = source.OrderBy(item => rnd.Next());
                    var result = surveyService.GetAllTask();



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


                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Exit);


                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddTasks(pv, true);




                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);

                }
                await LogMyDayError(suid, "AddTasks GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddTasks GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
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

                    //string userName = (string) Session["UserName"];
                    if (Session["CurrRound"] == null)
                    {
                        Session["CurrRound"] = 1;
                    }

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.Enter, profileId);


                    int taskId = 0;
                    TaskItemDto task = new TaskItemDto();

                    if (!string.IsNullOrEmpty(v.OtherTaskName))
                    {
                        //task = surveyService.GetTaskByTaskName("Other");
                        task = surveyService.GetOtherTask();

                        Session["CurrTaskNameOther"] = v.OtherTaskName;
                    }
                    else
                    {
                        taskId = (int)selectedTasks[0];

                        Session["CurrTaskNameOther"] = null;

                        task = surveyService.GetTaskByTaskId(taskId);
                        if (selectedTasks == null || selectedTasks.Length <= 0)
                        {
                            await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.ERROR, null, "TaskNotFound");
                            return RedirectToAction("TaskNotSelectedError");
                        }

                    }


                    Session["AddTaskId"] = task.Id;
                    Session["CurrTaskName"] = null;
                    Session["AddCurrTaskName"] = task.ShortName;


                    //Due to only one task should be selected from the UI
                    //var tasks = db.TaskItems.Where(x => selectedTasks.Contains(x.Id)).ToList();
                    //List<SessionSurveyTask> surveyTasks = new List<SessionSurveyTask>();
                    //foreach (var t in tasks)
                    //{
                    //    surveyTasks.Add(
                    //        new SessionSurveyTask
                    //        {
                    //            TaskId = t.Id,
                    //            TaskName = t.ShortName,
                    //            IsCompleted = false
                    //        }
                    //    );
                    //}
                    //Session["SelectedTaskList"] = surveyTasks;
                    //await SetTasksAsyncPOST(surveyId);

                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];
                    int remainingDuration = (int)Session["RemainingDuration"];


                    surveyService.SetAdditionalTasksAsyncPOST(
                         surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        Constants.StatusSurveyProgress.AddTaskTime.ToString(),
                        task.Id
                        );

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.Exit, profileId);

                    return RedirectToAction("AddTaskTime");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTasks POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
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
        public async Task<ActionResult> AddTaskTime()
        {
            string suid = string.Empty;
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskTime1ViewModel v = new SurveyTaskTime1ViewModel();

                    int surveyId = (int)Session["SurveyId"];
                    DateTime taskStartTime = (DateTime)Session["CurrTaskStartTime"];

                    //int taskId = (int) Session["CurrTask"];
                    int taskId = (int)Session["AddTaskId"];

                    string uid = (string)Session["SurveyUID"];
                    //int remainingDuration = (int) Session["RemainingDuration"];
                    suid = uid;
                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Enter);
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

                    //TaskItemDto t = surveyService.GetTaskByTaskId(taskId);
                    //string taskName = await db.TaskItems
                    //                    .Where(t => t.Id == taskId)
                    //                    .Select(t => t.ShortName)
                    //                    .SingleOrDefaultAsync();

                    //v.TaskId = t.Id;
                    v.TaskId = taskId;

                    //v.TaskName = t.ShortName;
                    v.TaskName = taskName;

                    DateTime surveyStart = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyEnd = (DateTime)Session["SurveyEndTime"];
                    TimeSpan difference = surveyEnd - surveyStart;
                    double remainingDuration = difference.TotalMinutes;

                    v.TimeHours = (int)(remainingDuration / 60);
                    v.TimeMinutes = (int)(remainingDuration % 60);

                    v.remainingTimeHours = (int)(remainingDuration / 60);
                    v.remainingTimeMinutes = (int)(remainingDuration % 60);

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);
                }
                await LogMyDayError(suid, "TaskRating2 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "TaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> AddTaskTime(SurveyTaskTime1ViewModel v)
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

                    await pageStatSvc.Insert(surveyId, addTaskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Enter);

                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];

                    #region Calculate the next task time
                    int totalTimeInMin = (v.TimeHours * 60) + v.TimeMinutes;


                    #endregion

                    //insert




                    ResponseDto r = new ResponseDto();
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
                    r.IsWardRoundTask = false;
                    r.WardRoundTaskId = null;
                    r.WardRoundWindowStartDateTime = null;
                    r.WardRoundWindowEndDateTime = null;


                    await surveyService.AddResponseAsync(r);

                    //await svc.InsertAsync(r, ModelState);


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
                        Constants.StatusSurveyProgress.AddTaskRating1.ToString(),
                        addTaskId);
                    #endregion




                    //await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Exit, profileId);
                    return RedirectToAction("AddTaskRating1");

                    //Session["CurrProgressValue"] = shiftDuration - remainingDuration;

                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskTime POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post);
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


                    //string taskName = (string) Session["CurrTaskName"];
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

                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = addTaskId;
                    v.TaskName = taskName;


                    //string taskName = await db.TaskItems
                    //                    .Where(t => t.Id == taskId)
                    //                    .Select(t => t.ShortName)
                    //                    .SingleOrDefaultAsync();



                    v.Q1Ans = Constants.NA_7Rating;
                    v.Q2Ans = Constants.NA_7Rating;
                    v.Q3Ans = Constants.NA_7Rating;
                    v.Q4Ans = Constants.NA_7Rating;
                    v.Q5Ans = Constants.NA_7Rating;

                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Exit, profileId);

                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddRating1(pv, 1);



                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }


                    return View(v);

                }
                await LogMyDayError(suid, "AddTaskRating GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                string EMsg = "AddTaskRating GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get);
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



                    //DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    //DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];

                    await pageStatSvc.Insert(surveyId, addTaskId, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

                    //Q2
                    ResponseDto r1 = new ResponseDto();
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

                    //svc.Insert(r1, ModelState);
                    //await svc.InsertAsync(r1, ModelState);
                    //await surveyService.AddResponseAsync(r1);
                    surveyService.AddResponse(r1);






                    ResponseDto r2 = new ResponseDto();
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

                    //await svc.InsertAsync(r2, ModelState);
                    //await surveyService.AddResponseAsync(r2);
                    surveyService.AddResponse(r2);


                    ResponseDto r3 = new ResponseDto();
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

                    //await svc.InsertAsync(r3, ModelState);
                    //await surveyService.AddResponseAsync(r3);
                    surveyService.AddResponse(r3);



                    ResponseDto r4 = new ResponseDto();
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

                    //await svc.InsertAsync(r4, ModelState);
                    //await surveyService.AddResponseAsync(r4);
                    surveyService.AddResponse(r4);


                    ResponseDto r5 = new ResponseDto();
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

                    //await svc.InsertAsync(r5, ModelState);
                    //await surveyService.AddResponseAsync(r5);
                    surveyService.AddResponse(r5);

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
                        Constants.StatusSurveyProgress.AddTaskRating2.ToString(),
                        addTaskId
                        );


                    //await surveyService.SetRating1AsyncPOST(
                    //    surveyId,
                    //    currRound,
                    //    0,
                    //    currTaskEndTime,
                    //    currTaskStartTime,
                    //    currTask,
                    //    nextTaskStartTime,
                    //    firstQuestion
                    //    );


                    //await SetRating1AsyncPOST(surveyId);



                    #endregion

                    await pageStatSvc.Insert(surveyId, addTaskId, null, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("AddTaskRating2");
                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskRating POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post);
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


                    //string taskName = (string) Session["CurrTaskName"];
                    string taskName = (string)Session["AddCurrTaskName"];


                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string)Session["CurrTaskNameOther"];
                    }


                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Enter);
                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = addTaskId;



                    //string taskName = await db.TaskItems
                    //    .Where(t => t.Id == taskId)
                    //    .Select(t => t.ShortName)
                    //    .SingleOrDefaultAsync();



                    v.TaskName = taskName;

                    v.Q6Ans = Constants.NA_7Rating;
                    v.Q7Ans = Constants.NA_7Rating;
                    v.Q8Ans = Constants.NA_7Rating;
                    v.Q9Ans = Constants.NA_7Rating;
                    v.Q10Ans = Constants.NA_7Rating;


                    await pageStatSvc.Insert(surveyId, addTaskId, startTime, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Exit);


                    int pv = (int)Session["ProgressValueValue"];
                    Session["ProgressValueValue"] = surveyService.CalProgressValAddRating2(pv, 1);


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);

                }
                await LogMyDayError(suid, "AddTaskRating2 GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                string EMsg = "AddTaskRating2 GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                await LogMyDayError(suid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get);
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
                    //int taskId = (int) Session["CurrTask"];
                    int addTaskId = (int)Session["AddTaskId"];


                    string taskOther = (string)Session["CurrTaskNameOther"];


                    //DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    //DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    DateTime shiftStartTime = (DateTime)Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime)Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime)Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime)Session["SurveyEndTime"];
                    //int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, addTaskId, null, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Enter);


                    //var listOfResponses = svc.GetAll(taskId, surveyId, startTime);
                    //var response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{
                    ResponseDto r6 = new ResponseDto();
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

                    //await svc.InsertAsync(r6, ModelState);
                    //await surveyService.AddResponseAsync(r6);
                    surveyService.AddResponse(r6);


                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{
                    ResponseDto r7 = new ResponseDto();
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

                    //await svc.InsertAsync(r7, ModelState);
                    //await surveyService.AddResponseAsync(r7);
                    surveyService.AddResponse(r7);

                    //}



                    //response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r8 = new ResponseDto();
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

                    //await svc.InsertAsync(r8, ModelState);
                    //await surveyService.AddResponseAsync(r8);
                    surveyService.AddResponse(r8);

                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r9 = new ResponseDto();
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

                    //await svc.InsertAsync(r9, ModelState);
                    //await surveyService.AddResponseAsync(r9);
                    surveyService.AddResponse(r9);

                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r10 = new ResponseDto();
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

                    //await svc.InsertAsync(r10, ModelState);
                    //await surveyService.AddResponseAsync(r10);
                    surveyService.AddResponse(r10);

                    surveyService.SaveResponse();
                    //}


                    //#region save status to survey
                    //var survey = db.Surveys.Where(x => x.Id == surveyId).Select(x => new { MaxStep = x.MaxStep }).SingleOrDefault();
                    //var tempSurvey = new Survey() { Id = surveyId };
                    //if (survey.MaxStep < 5)
                    //{
                    //    tempSurvey.MaxStep = 5;
                    //}
                    //tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.TaskRating2.ToString();
                    //db.Surveys.Attach(tempSurvey);
                    //db.Entry(tempSurvey).Property(x => x.SurveyProgressNext).IsModified = true;
                    //db.Entry(tempSurvey).Property(x => x.MaxStep).IsModified = true;
                    //db.SaveChangesAsync();
                    //#endregion

                    //------------

                    //stopping criteria
                    //update the duration sessions variables


                    int currRound = (int)Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?)Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?)Session["CurrTaskStartTime"];
                    int currTask = (int)Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?)Session["NextTaskStartTime"];
                    bool firstQuestion = (bool)Session["FirstQuestion"];


                    //await surveyService.SetAdditionalTasksAsyncPOST(
                    //    surveyId,
                    //    currRound,
                    //    0,
                    //    currTaskEndTime,
                    //    currTaskStartTime,
                    //    currTask,
                    //    nextTaskStartTime,
                    //    firstQuestion,
                    //    //Constants.StatusSurveyProgress.Completed.ToString()
                    //    Constants.StatusSurveyProgress.AddTaskRating2.ToString(),
                    //    addTaskId
                    //    );

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



                    //await surveyService.SetRating2AsyncPOSTResult(
                    //    surveyId,
                    //    currRound,
                    //    0,
                    //    currTaskEndTime,
                    //    currTaskStartTime,
                    //    currTask,
                    //    nextTaskStartTime,
                    //    firstQuestion
                    //    );

                    //await SetRating2AsyncPOSTResult(surveyId);

                    //SetReminderAsyncPOSTResult(surveyId);

                    await pageStatSvc.Insert(surveyId, addTaskId, null, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Exit);




                    return RedirectToAction("Feedback");

                    //return RedirectToAction("Results");



                    //Session["CurrProgressValue"] = ((int) Session["ShiftDuration"] - (int) Session["RemainingDuration"] ) / (int) Session["ShiftDuration"] ;
                    //Session["currProgressValue"] = (int) v.CurrProgressValue;


                }
                catch (Exception ex)
                {
                    string EMsg = "AddTaskRating2 POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post);
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
                {
                    task = surveyService.GetTaskByTaskId(taskId);
                }

                //v.TaskName = db.TaskItems.Where(x => x.Id == taskId).SingleOrDefault().ShortName;
                v.TaskName = task.ShortName;


                Session["CurrTask"] = task.Id;
                //Session["CurrTaskStartTime"] = taskStartDate;

                await pageStatSvc.Insert(surveyId, taskId, null, true, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get, Constants.PageType.Enter);

                bool isAny = false;

                //v.TaskId = taskId;
                //v.TaskName = taskName;
                //v.TaskStartDateTime = taskStartDate;

                //var listOfResponses = await svc.GetAllAsync(taskId, surveyId, taskStartDate);
                var listOfResponses = surveyService.GetAdditionalResponseAsync(taskId, profileId,
                    surveyId);

                var response = listOfResponses.Where(t => t.Question == v.Q1DB).SingleOrDefault();

                if (response.TaskOther != null)
                {
                    v.TaskName = response.TaskOther;
                }

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
                {
                    v.Q1Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q2DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q2Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q3Ans = Constants.NA_7Rating;
                }


                response = listOfResponses.Where(t => t.Question == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q4Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q5Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q6Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q7Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q8Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q9Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q10Ans = Constants.NA_7Rating;
                }


                if (isAny)
                {
                    v.IsExist = true;
                }

                await pageStatSvc.Insert(surveyId, taskId, null, false, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get, Constants.PageType.Exit);

                if (Request.IsAjaxRequest())
                {
                    return PartialView(v);
                }
                return View(v);
            }
            catch (Exception ex)
            {
                string EMsg = "AddEditResponse GET:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                await LogMyDayError(s.Uid, EMsg, "SurveyError");
                LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get);
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

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post, Constants.PageType.Enter);


                    //DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    //currentTaskList.Where(w => w.TaskId == v.TaskId).ToList().ForEach(s => s.IsCompleted = true);


                    // Save to db
                    #region update Question  
                    //var listOfResponses = await svc.GetAllAsync(taskId, surveyId, startTime);
                    var listOfResponses = surveyService.GetAdditionalResponseAsync(taskId, profileId, surveyId);


                    var response = listOfResponses.Where(r => r.Question == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        surveyService.UpdateResponse(response);
                        //await svc.UpdateAsync(response, ModelState);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        surveyService.UpdateResponse(response);
                        //await svc.UpdateAsync(response, ModelState);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }


                    response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }


                    response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }

                    surveyService.SaveResponse();


                    #endregion

                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post, Constants.PageType.Exit);
                    if (Session["LoopDemo"].ToString() == "Yes")
                    { return RedirectToAction("LoopSurveyResults"); }
                    else { return RedirectToAction("Results"); }


                }
                catch (Exception ex)
                {
                    string EMsg = "AddEditResponse POST:: Exception Message: " + ex.Message + " InnerException: " + ex.InnerException;
                    var s = surveyService.GetSurveyById((int)Session["SurveyId"]);
                    await LogMyDayError(s.Uid, EMsg, "SurveyError");
                    LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post);
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

        #endregion



        #region Ward Round


        //After Main task list selection screen
        [HttpGet]
        public async Task<ActionResult> WRTaskTime()
        {
            try
            {
                string suid = string.Empty;
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskTime1ViewModel v = new SurveyTaskTime1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                    int taskId = (int) Session["CurrTask"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Enter);
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }

                    v.ShiftSpan = shiftSpan;
                    v.Uid = uid;
                    v.SurveySpan = surveySpan;

                    //TaskItemDto t = surveyService.GetTaskByTaskId(taskId);
                    //string taskName = await db.TaskItems
                    //                    .Where(t => t.Id == taskId)
                    //                    .Select(t => t.ShortName)
                    //                    .SingleOrDefaultAsync();

                    v.TaskId = taskId;
                    //v.TaskName = t.ShortName;
                    v.TaskName = taskName;

                    v.TimeHours = remainingDuration / 60;
                    v.TimeMinutes = remainingDuration % 60;

                    v.remainingTimeHours = remainingDuration / 60;
                    v.remainingTimeMinutes = remainingDuration % 60;

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Exit);

                    int pv = (int) Session["ProgressValueValue"];

                    if (pv <= 15)
                    {
                        Session["ProgressValueValue"] = surveyService.CalProgressValTaskTime(pv, true);
                    }


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);
                }
                await LogMyDayError(suid, "WRTaskTime GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        
        [HttpPost]
        public async Task<ActionResult> WRTaskTime(SurveyTaskTime1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

                    string taskOther = (string) Session["CurrTaskNameOther"];
                    Session["GoToReset"] = false;


                    int remainingDuration = (int) Session["RemainingDuration"];

                    DateTime currTaskStartDateTime = (DateTime) Session["CurrTaskStartTime"];

                    await pageStatSvc.Insert(surveyId, taskId, currTaskStartDateTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Enter);



                    #region Calculate the next task time
                    int totalTimeInMin = (v.TimeHours * 60) + v.TimeMinutes;
                    remainingDuration = remainingDuration - totalTimeInMin;
                    Session["RemainingDuration"] = remainingDuration;


                    DateTime endTime = currTaskStartDateTime.AddMinutes(totalTimeInMin);
                    Session["CurrTaskEndTime"] = endTime;
                    Session["NextTaskStartTime"] = endTime;
                    Session["FirstQuestion"] = false;

                    //Work Round Duration
                    Session["CurrTaskStartTimeWR"] = currTaskStartDateTime;
                    Session["CurrTaskEndTimeWR"] = null;
                    Session["NextTaskStartTimeWR"] = null;
                    Session["RemainingDurationWR"] = totalTimeInMin;
                    Session["CurrTaskWR"] = (int) Session["CurrTask"]; //set the current WR task id, will be needed when the tasks are inserted to DB

                    //Progress: WRTaskTime
                    int pv = (int) Session["ProgressValueValue"];
                    //int remainingDuration = (int) Session["RemainingDuration"];
                    decimal? percentageToRoll = null;
                    int surveyDuration = (int) Session["SurveyDuration"];
                    int remainingSurveyDuration = (int) Session["RemainingDuration"];
                    int remainingDurationWR = (int) Session["RemainingDurationWR"];

                    //Allocate proporation of the 50% based on remaining durations
                    //If remaining survey duration is 0, all 50%
                    decimal WRProportion = 1 - ( (decimal)remainingSurveyDuration / (decimal) surveyDuration);
                    decimal remainingLength = 75 - pv;

                    Session["WREndProgress"] = pv + (remainingLength * WRProportion);
                    Session["WRSpanProgress"] = remainingLength * WRProportion;

                    //decimal WREndProgress = (decimal) Session["WsREndProgress"];
                    //add 5% to the current pv.
                    pv += 5;
                    Session["ProgressValueValue"] = pv;


                    Session["WardRoundWindowStartDateTime"] = currTaskStartDateTime;
                    Session["WardRoundWindowEndDateTime"] = endTime;



                    #endregion

                    ResponseDto r = new ResponseDto();
                    r.SurveyId = surveyId;
                    r.ProfileId = profileId;
                    r.TaskId = taskId; //added to the model by Get Method using Session
                    r.TaskOther = taskOther;
                    r.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r.EndResponseDateTimeUtc = DateTime.UtcNow;
                    r.TaskStartDateTime = currTaskStartDateTime;
                    r.TaskEndDateTime = endTime;
                    r.ShiftStartDateTime = shiftStartTime;
                    r.ShiftEndDateTime = shiftEndTime;
                    r.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r.SurveyWindowEndDateTime = surveyWindowEndDateTime;
                    r.Question = v.QDB;
                    r.Answer = totalTimeInMin.ToString();
                    r.IsOtherTask = false;
                    r.IsWardRoundTask = true; //mark as the wardround task
                    r.WardRoundTaskId = null;
                    r.WardRoundWindowStartDateTime = null;
                    r.WardRoundWindowEndDateTime = null;



                    await surveyService.AddResponseAsync(r);

                    #region save status to survey
                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];


                    int WRCurrTaskWR = (int) Session["CurrTaskWR"];
                    int WRremainingDuration = (int) Session["RemainingDurationWR"];
                    DateTime? WRCurrTaskEndTime = (DateTime?) Session["CurrTaskEndTimeWR"];
                    DateTime? WRCurrTaskStartTime = (DateTime?) Session["CurrTaskStartTimeWR"];
                    DateTime? WRNextTaskStartTime = (DateTime?) Session["NextTaskStartTimeWR"];

                    DateTime? wStart = (DateTime?) Session["WardRoundWindowStartDateTime"];
                    DateTime? wEnd = (DateTime?) Session["WardRoundWindowEndDateTime"];


                    await surveyService.SetWRSettingsAsyncPOST(
                        Constants.StatusSurveyProgress.WRTasks.ToString(),
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        WRCurrTaskWR,
                        null,
                        WRremainingDuration,
                        WRCurrTaskStartTime,
                        WRCurrTaskEndTime,
                        WRNextTaskStartTime,
                        wStart,
                        wEnd
                        );
                    #endregion

                    await pageStatSvc.Insert(surveyId, taskId, currTaskStartDateTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("WRTasks");

                    //Session["CurrProgressValue"] = shiftDuration - remainingDuration;

                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");


                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
              

        //wardround task selection
        [HttpGet]
        public async Task<ActionResult> WRTasks()
        {
            try
            {
                string suid = string.Empty;
                if (Session["SurveyId"] != null)
                {
                    SurveyTasks1ViewModel v = new SurveyTasks1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string uid = (string) Session["SurveyUID"];
                    suid = uid;
                    Session["GoToReset"] = false;


                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    var checkBoxListItems = new List<CheckBoxListItem>();

                    var profileTasks = surveyService.GetProfileTaksByTaskId(profileId);

                    //var source = surveyService.GetAllTask();
                    //var rnd = new Random();
                    //var result = source.OrderBy(item => rnd.Next());
                    var result = surveyService.GetWardRoundTasks();



                    foreach (var k in result)
                    {
                        checkBoxListItems.Add(new CheckBoxListItem()
                        {
                            ID = k.Id,
                            Display = k.ShortName,
                            Description = k.LongName,
                            IsChecked = false
                        });
                    }
                    v.FullTaskList = checkBoxListItems;

                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.Exit);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);

                }
                await LogMyDayError(suid, "WRTasks GET: Survey UID not found!", "InvalidError");
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {

                LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }

        [HttpPost]
        public async Task<ActionResult> WRTasks(SurveyTasks1ViewModel v, params int[] selectedTasks)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    //string userName = (string) Session["UserName"];
                    if (Session["CurrRound"] == null)
                    {
                        Session["CurrRound"] = 1;
                    }

                    if (selectedTasks == null || selectedTasks.Length <= 0)
                    {
                        await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post, Constants.PageType.ERROR, null, "TaskNotFound");
                        return RedirectToAction("TaskNotSelectedError");
                    }


                    var tasks = surveyService.GetAllTaskSpecific(selectedTasks);
                    Session["GoToReset"] = false;


                    //var tasks = db.TaskItems.Where(x => selectedTasks.Contains(x.Id)).ToList();

                    List<SessionSurveyTask> surveyTasks = new List<SessionSurveyTask>();

                    int count = 1;
                    int tot = tasks.Count * Constants.PAGES_PER_TASK_VARIANT; //Two pages for each task

                    foreach (var t in tasks)
                    {
                        surveyTasks.Add(
                            new SessionSurveyTask
                            {
                                TaskId = t.ID,
                                TaskName = t.Name,
                                StepValue = Math.Round(Constants.TASK_COMPLETION_PERCENTAGE_VARIANT / tot),
                                IsCompleted = false,

                            }
                        );
                        count++;
                    }

                    Session["SelectedWRTaskList"] = surveyTasks;
                    Session["WRCurrTasksId"] = (string) string.Join(",", selectedTasks);




                    Session["CurrTaskWR"] = (int) selectedTasks[0];

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];


                    int WRCurrTaskId = (int) Session["CurrTaskWR"];
                    string WRCurrTasksId = (string) Session["WRCurrTasksId"];
                    int remainingDuration = (int) Session["RemainingDuration"];
                    int WRremainingDuration = (int) Session["RemainingDurationWR"];
                    DateTime? WRCurrTaskEndTime = (DateTime?) Session["CurrTaskEndTimeWR"];
                    DateTime? WRCurrTaskStartTime = (DateTime?) Session["CurrTaskStartTimeWR"];
                    DateTime? WRNextTaskStartTime = (DateTime?) Session["NextTaskStartTimeWR"];


                    await surveyService.SetWRSettingsAsyncPOST(
                        Constants.StatusSurveyProgress.WRTaskTimeInd.ToString(),
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        WRCurrTaskId,
                        WRCurrTasksId,
                        WRremainingDuration,
                        WRCurrTaskStartTime,
                        WRCurrTaskEndTime,
                        WRNextTaskStartTime
                        );

                    //Progress:
                    decimal wrSpanProgress = (decimal) Session["WRSpanProgress"];
                    decimal wrSubpageProportion = 0;
                    if (surveyTasks.Count > 0)
                    {
                        wrSubpageProportion = (wrSpanProgress / (surveyTasks.Count)) / 3; //three pages 
                    }
                    Session["WRSubpageProportion"] = wrSubpageProportion;


                    return RedirectToAction("WRTaskTimeInd");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Task, Constants.PageAction.Post);
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
        public ActionResult WRTaskTimeInd()
        {
            SurveyTaskTime1ViewModel v = new SurveyTaskTime1ViewModel();

            try
            {

                int surveyId = (int) Session["SurveyId"];
                //DateTime taskStartTime = (DateTime) Session["CurrTaskStartTime"];
                //int taskId = (int) Session["CurrTask"];
                //int taskWRId = (int) Session["CurrTaskWR"];

                string uid = (string) Session["SurveyUID"];

                //int remainingDuration = (int) Session["RemainingDuration"];

                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];

                v.ShiftSpan = shiftSpan;
                v.Uid = uid;
                v.SurveySpan = surveySpan;

                //await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.Enter);
                //string taskName = (string) Session["CurrTaskName"];
                //if (Session["CurrTaskNameOther"] != null)
                //{
                //    taskName = (string) Session["CurrTaskNameOther"];
                //}

                //Load task details
                var wrSelTaskList = (List<SessionSurveyTask>) Session["SelectedWRTaskList"];


                #region WR
                //DateTime taskStartTimeWR = (DateTime) Session["CurrTaskStartTimeWR"];
                int remainingDurationWR = (int) Session["RemainingDurationWR"];
                #endregion



                var task = wrSelTaskList
                            .Where(t => t.IsCompleted == false)
                            .FirstOrDefault();
                if (task != null)
                {
                    v.TaskId = task.TaskId;
                    v.TaskName = task.TaskName;


                    Session["CurrTaskName"] = task.TaskName;
                    Session["CurrTaskNameOther"] = task.TaskName;
                    Session["CurrTask"] = task.TaskId; //used in Rating page, maybe use a viewbag
                    //Session["CurrTaskWR"] = task.TaskId;

                    v.TimeHours = remainingDurationWR / 60;
                    v.TimeMinutes = remainingDurationWR % 60;

                    v.remainingTimeHours = remainingDurationWR / 60;
                    v.remainingTimeMinutes = remainingDurationWR % 60;

                    //Progress: 
                    int pv = (int) Session["ProgressValueValue"];
                    decimal subpageRatio = (decimal) Session["WRSubpageProportion"];
                    if (Session["GoToReset"] == null)
                    {
                        Session["GoToReset"] = false;
                    }

                    bool resetGoto = (bool) Session["GoToReset"];

                    Session["ProgressValueValue"] = surveyService.CalProgressWRSubpages(pv, subpageRatio, resetGoto);




                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }

                    return View(v);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            
            //await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Get, Constants.PageType.ERROR, null);
            return RedirectToAction("InvalidError");
        }

        [HttpPost]
        public async Task<ActionResult> WRTaskTimeInd(SurveyTaskTime1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    int surveyId = (int) Session["SurveyId"];
                    DateTime taskStartTime = (DateTime) Session["CurrTaskStartTimeWR"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];



                    #region WR
                    int taskWRId = (int) Session["CurrTaskWR"];
                    DateTime? wardRoundWindowStartDateTime = (DateTime) Session["WardRoundWindowStartDateTime"];
                    DateTime? wardRoundWindowEndDateTime = (DateTime) Session["WardRoundWindowEndDateTime"];
                    #endregion

                    string taskOther = null;// (string) Session["CurrTaskNameOther"];
                    Session["GoToReset"] = false;


                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, false, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Enter);

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];

                    //int remainingDuration = (int) Session["RemainingDuration"];
                    int remainingDurationWR = (int) Session["RemainingDurationWR"];
                    //DateTime startTime = (DateTime) Session["CurrTaskStartTime"];



                    #region Calculate the next task time

                    int totalTimeInMin = (v.TimeHours * 60) + v.TimeMinutes;
                    //remainingDuration = remainingDuration - totalTimeInMin;
                    remainingDurationWR = remainingDurationWR - totalTimeInMin;


                    //Session["RemainingDuration"] = remainingDuration;
                    Session["RemainingDurationWR"] = remainingDurationWR;
                    DateTime endTime = taskStartTime.AddMinutes(totalTimeInMin);
                    //Session["CurrTaskEndTime"] = endTime;
                    Session["CurrTaskEndTimeWR"] = endTime;

                    //DateTime nextStartTime = startTime.AddMinutes(totalTimeInMin);
                    //Session["NextTaskStartTime"] = nextStartTime;
                    Session["NextTaskStartTimeWR"] = endTime;

                    Session["FirstQuestion"] = false;

                    #endregion


                    //insert

                    ResponseDto r = new ResponseDto();
                    r.SurveyId = surveyId;
                    r.ProfileId = profileId;
                    r.TaskId = taskId; //added to the model by Get Method using Session
                    r.TaskOther = taskOther;
                    //Page Stats
                    r.StartResponseDateTimeUtc = v.PageStartDateTimeUtc;
                    r.EndResponseDateTimeUtc = DateTime.UtcNow;

                    r.TaskStartDateTime = taskStartTime;
                    r.TaskEndDateTime = endTime;

                    r.ShiftStartDateTime = shiftStartTime;
                    r.ShiftEndDateTime = shiftEndTime;
                    r.SurveyWindowStartDateTime = surveyWindowStartDateTime;
                    r.SurveyWindowEndDateTime = surveyWindowEndDateTime;

                    r.Question = v.QDB;
                    r.Answer = totalTimeInMin.ToString();
                    r.IsOtherTask = false;
                    r.IsWardRoundTask = false;
                    r.WardRoundTaskId = taskWRId;
                    r.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;


                    await surveyService.AddResponseAsync(r);

                    //await svc.InsertAsync(r, ModelState);


                    #region save status to survey
                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

                    int WRCurrTaskId = (int) Session["CurrTaskWR"];
                    string WRCurrTasksId = (string) Session["WRCurrTasksId"];
                    int remainingDuration = (int) Session["RemainingDuration"];
                    int WRremainingDuration = (int) Session["RemainingDurationWR"];
                    DateTime? WRCurrTaskEndTime = (DateTime?) Session["CurrTaskEndTimeWR"];
                    DateTime? WRCurrTaskStartTime = (DateTime?) Session["CurrTaskStartTimeWR"];
                    DateTime? WRNextTaskStartTime = (DateTime?) Session["NextTaskStartTimeWR"];


                    await surveyService.SetWRSettingsAsyncPOST(
                        Constants.StatusSurveyProgress.WRTaskRating1.ToString(),
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        WRCurrTaskId,
                        WRCurrTasksId,
                        WRremainingDuration,
                        WRCurrTaskStartTime,
                        WRCurrTaskEndTime,
                        WRNextTaskStartTime
                        );



                    //await surveyService.SetTaskTimeAsyncPOST(
                    //    surveyId,
                    //    currRound,
                    //    //remainingDuration,
                    //    remainingDurationWR,
                    //    currTaskEndTime,
                    //    currTaskStartTime,
                    //    currTask,
                    //    nextTaskStartTime,
                    //    firstQuestion
                    //    );
                    #endregion

                    await pageStatSvc.Insert(surveyId, taskId, taskStartTime, true, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("WRTaskRating1");

                    //Session["CurrProgressValue"] = shiftDuration - remainingDuration;

                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Time, Constants.PageAction.Post);
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
        public async Task<ActionResult> WRTaskRating1()
        {
            try
            {

                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating1ViewModel v = new SurveyTaskRating1ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    int taskWRId = (int) Session["CurrTaskWR"];

                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }



                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    string uid = (string) Session["SurveyUID"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Enter, profileId);

                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;
                    v.TaskName = taskName;


                    //string taskName = await db.TaskItems
                    //                    .Where(t => t.Id == taskId)
                    //                    .Select(t => t.ShortName)
                    //                    .SingleOrDefaultAsync();



                    v.Q1Ans = Constants.NA_7Rating;
                    v.Q2Ans = Constants.NA_7Rating;
                    v.Q3Ans = Constants.NA_7Rating;
                    v.Q4Ans = Constants.NA_7Rating;
                    v.Q5Ans = Constants.NA_7Rating;

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.Exit, profileId);



                    //Progress: 

                    if (Session["GoToReset"] == null)
                    {
                        Session["GoToReset"] = false;
                    }

                    bool resetGoto = (bool) Session["GoToReset"];

                    int pv = (int) Session["ProgressValueValue"];
                    decimal subpageRatio = (decimal) Session["WRSubpageProportion"];

                    Session["ProgressValueValue"] = surveyService.CalProgressWRSubpages(pv, subpageRatio, resetGoto);






                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);

                }
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }

        [HttpPost]
        public async Task<ActionResult> WRTaskRating1(SurveyTaskRating1ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];
                    int taskWRId = (int) Session["CurrTaskWR"];
                    DateTime? wardRoundWindowStartDateTime = (DateTime) Session["WardRoundWindowStartDateTime"];
                    DateTime? wardRoundWindowEndDateTime = (DateTime) Session["WardRoundWindowEndDateTime"];

                    string taskOther = null;// (string) Session["CurrTaskNameOther"];
                    Session["GoToReset"] = false;


                    DateTime startTime = (DateTime) Session["CurrTaskStartTimeWR"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTimeWR"];

                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];

                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Enter, profileId);

                    //Q2
                    ResponseDto r1 = new ResponseDto();
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
                    r1.IsWardRoundTask = false;
                    r1.WardRoundTaskId = taskWRId;
                    r1.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r1.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //svc.Insert(r1, ModelState);
                    //await svc.InsertAsync(r1, ModelState);
                    //await surveyService.AddResponseAsync(r1);
                    surveyService.AddResponse(r1);






                    ResponseDto r2 = new ResponseDto();
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
                    r2.IsWardRoundTask = false;
                    r2.WardRoundTaskId = taskWRId;
                    r2.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r2.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //await svc.InsertAsync(r2, ModelState);
                    //await surveyService.AddResponseAsync(r2);
                    surveyService.AddResponse(r2);


                    ResponseDto r3 = new ResponseDto();
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
                    r3.IsWardRoundTask = false;
                    r3.WardRoundTaskId = taskWRId;
                    r3.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r3.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //await svc.InsertAsync(r3, ModelState);
                    //await surveyService.AddResponseAsync(r3);
                    surveyService.AddResponse(r3);



                    ResponseDto r4 = new ResponseDto();
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
                    r4.IsWardRoundTask = false;
                    r4.WardRoundTaskId = taskWRId;
                    r4.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r4.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;

                    //await svc.InsertAsync(r4, ModelState);
                    //await surveyService.AddResponseAsync(r4);
                    surveyService.AddResponse(r4);


                    ResponseDto r5 = new ResponseDto();
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
                    r5.IsWardRoundTask = false;
                    r5.WardRoundTaskId = taskWRId;
                    r5.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r5.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;


                    //await svc.InsertAsync(r5, ModelState);
                    //await surveyService.AddResponseAsync(r5);
                    surveyService.AddResponse(r5);
                    surveyService.SaveResponse();



                    #region save status to survey

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

                    //await surveyService.SetRating1AsyncPOST(
                    //    surveyId,
                    //    currRound,
                    //    remainingDuration,
                    //    currTaskEndTime,
                    //    currTaskStartTime,
                    //    currTask,
                    //    nextTaskStartTime,
                    //    firstQuestion
                    //    );

                    int WRCurrTaskId = (int) Session["CurrTaskWR"];
                    string WRCurrTasksId = (string) Session["WRCurrTasksId"];
                    int WRremainingDuration = (int) Session["RemainingDurationWR"];
                    DateTime? WRCurrTaskEndTime = (DateTime?) Session["CurrTaskEndTimeWR"];
                    DateTime? WRCurrTaskStartTime = (DateTime?) Session["CurrTaskStartTimeWR"];
                    DateTime? WRNextTaskStartTime = (DateTime?) Session["NextTaskStartTimeWR"];


                    await surveyService.SetWRSettingsAsyncPOST(
                        Constants.StatusSurveyProgress.WRTaskRating2.ToString(),
                        surveyId,
                        currRound,
                        remainingDuration,
                        currTaskEndTime,
                        currTaskStartTime,
                        currTask,
                        nextTaskStartTime,
                        firstQuestion,
                        WRCurrTaskId,
                        WRCurrTasksId,
                        WRremainingDuration,
                        WRCurrTaskStartTime,
                        WRCurrTaskEndTime,
                        WRNextTaskStartTime
                        );


                    //await SetRating1AsyncPOST(surveyId);



                    #endregion

                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("WRTaskRating2");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Rating1, Constants.PageAction.Post);
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
        public async Task<ActionResult> WRTaskRating2()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    SurveyTaskRating2ViewModel v = new SurveyTaskRating2ViewModel();

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    //int taskId = (int) Session["CurrTask"];
                    int taskId = (int) Session["CurrTaskWR"];
                    //DateTime startTime = (DateTime) Session["CurrTaskStartTime"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTimeWR"];

                    string uid = (string) Session["SurveyUID"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    string taskName = (string) Session["CurrTaskName"];
                    if (Session["CurrTaskNameOther"] != null)
                    {
                        taskName = (string) Session["CurrTaskNameOther"];
                    }


                    await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Enter);
                    v.Uid = uid;
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.PageStartDateTimeUtc = DateTime.UtcNow;
                    v.TaskId = taskId;



                    //string taskName = await db.TaskItems
                    //    .Where(t => t.Id == taskId)
                    //    .Select(t => t.ShortName)
                    //    .SingleOrDefaultAsync();



                    v.TaskName = taskName;

                    v.Q6Ans = Constants.NA_7Rating;
                    v.Q7Ans = Constants.NA_7Rating;
                    v.Q8Ans = Constants.NA_7Rating;
                    v.Q9Ans = Constants.NA_7Rating;
                    v.Q10Ans = Constants.NA_7Rating;


                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.Exit);

                    //Progress: 
                    int pv = (int) Session["ProgressValueValue"];
                    decimal subpageRatio = (decimal) Session["WRSubpageProportion"];
                    if (Session["GoToReset"] == null)
                    {
                        Session["GoToReset"] = false;
                    }
                    bool resetGoto = (bool) Session["GoToReset"];
                    Session["ProgressValueValue"] = surveyService.CalProgressWRSubpages(pv, subpageRatio, resetGoto);


                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    return View(v);

                }
                await pageStatSvc.Insert(null, null, null, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get, Constants.PageType.ERROR, null);
                return RedirectToAction("InvalidError");

            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }

        [HttpPost]
        public async Task<ActionResult> WRTaskRating2(SurveyTaskRating2ViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    int taskId = (int) Session["CurrTask"];

                    Session["GoToReset"] = false;





                    int taskWRId = (int) Session["CurrTaskWR"];

                    var currentTaskList = (List<SessionSurveyTask>) Session["SelectedWRTaskList"];
                    currentTaskList.Where(w => w.TaskId == taskId).ToList().ForEach(s => s.IsCompleted = true);
                    Session["SelectedWRTaskList"] = currentTaskList;


                    DateTime? wardRoundWindowStartDateTime = (DateTime) Session["WardRoundWindowStartDateTime"];
                    DateTime? wardRoundWindowEndDateTime = (DateTime) Session["WardRoundWindowEndDateTime"];


                    string taskOther = null;// (string) Session["CurrTaskNameOther"];


                    DateTime startTime = (DateTime) Session["CurrTaskStartTimeWR"];
                    DateTime endTime = (DateTime) Session["CurrTaskEndTimeWR"];





                    DateTime shiftStartTime = (DateTime) Session["ShiftStartTime"];
                    DateTime shiftEndTime = (DateTime) Session["ShiftEndTime"];
                    DateTime surveyWindowStartDateTime = (DateTime) Session["SurveyStartTime"];
                    DateTime surveyWindowEndDateTime = (DateTime) Session["SurveyEndTime"];
                    int remainingDuration = (int) Session["RemainingDuration"];
                    int remainingDurationWR = (int) Session["RemainingDurationWR"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Enter);


                    //var listOfResponses = svc.GetAll(taskId, surveyId, startTime);
                    //var response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{
                    ResponseDto r6 = new ResponseDto();
                    r6.SurveyId = surveyId;
                    r6.ProfileId = profileId;
                    r6.TaskOther = taskOther;
                    r6.TaskId = taskId;
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
                    r6.IsWardRoundTask = false;

                    r6.WardRoundTaskId = taskWRId;
                    r6.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r6.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;

                    //await svc.InsertAsync(r6, ModelState);
                    //await surveyService.AddResponseAsync(r6);
                    surveyService.AddResponse(r6);


                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{
                    ResponseDto r7 = new ResponseDto();
                    r7.SurveyId = surveyId;
                    r7.ProfileId = profileId;

                    r7.TaskOther = taskOther;


                    r7.TaskId = taskId;
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
                    r7.IsWardRoundTask = false;

                    r7.WardRoundTaskId = taskWRId;
                    r7.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r7.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //await svc.InsertAsync(r7, ModelState);
                    //await surveyService.AddResponseAsync(r7);
                    surveyService.AddResponse(r7);

                    //}



                    //response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r8 = new ResponseDto();
                    r8.SurveyId = surveyId;
                    r8.ProfileId = profileId;

                    r8.TaskOther = taskOther;


                    r8.TaskId = taskId;
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
                    r8.IsWardRoundTask = false;

                    r8.WardRoundTaskId = taskWRId;
                    r8.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r8.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //await svc.InsertAsync(r8, ModelState);
                    //await surveyService.AddResponseAsync(r8);
                    surveyService.AddResponse(r8);

                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r9 = new ResponseDto();
                    r9.SurveyId = surveyId;
                    r9.ProfileId = profileId;

                    r9.TaskOther = taskOther;


                    r9.TaskId = taskId;
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
                    r9.IsWardRoundTask = false;

                    r9.WardRoundTaskId = taskWRId;
                    r9.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r9.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //await svc.InsertAsync(r9, ModelState);
                    //await surveyService.AddResponseAsync(r9);
                    surveyService.AddResponse(r9);

                    //}

                    //response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    //if (response != null)
                    //{
                    //    response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                    //    svc.Update(response, ModelState);
                    //}
                    //else
                    //{

                    ResponseDto r10 = new ResponseDto();
                    r10.SurveyId = surveyId;
                    r10.ProfileId = profileId;

                    r10.TaskOther = taskOther;


                    r10.TaskId = taskId;
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
                    r10.IsWardRoundTask = false;

                    r10.WardRoundTaskId = taskWRId;
                    r10.WardRoundWindowStartDateTime = wardRoundWindowStartDateTime;
                    r10.WardRoundWindowEndDateTime = wardRoundWindowEndDateTime;
                    //await svc.InsertAsync(r10, ModelState);
                    //await surveyService.AddResponseAsync(r10);
                    surveyService.AddResponse(r10);

                    surveyService.SaveResponse();
                    //}


                    //#region save status to survey
                    //var survey = db.Surveys.Where(x => x.Id == surveyId).Select(x => new { MaxStep = x.MaxStep }).SingleOrDefault();
                    //var tempSurvey = new Survey() { Id = surveyId };
                    //if (survey.MaxStep < 5)
                    //{
                    //    tempSurvey.MaxStep = 5;
                    //}
                    //tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.TaskRating2.ToString();
                    //db.Surveys.Attach(tempSurvey);
                    //db.Entry(tempSurvey).Property(x => x.SurveyProgressNext).IsModified = true;
                    //db.Entry(tempSurvey).Property(x => x.MaxStep).IsModified = true;
                    //db.SaveChangesAsync();
                    //#endregion

                    //------------

                    //stopping criteria
                    //update the duration sessions variables

                    //
                    Session["CurrTaskStartTime"] = Session["CurrTaskEndTime"];

                    int currRound = (int) Session["CurrRound"];
                    DateTime? currTaskEndTime = (DateTime?) Session["CurrTaskEndTime"];
                    DateTime? currTaskStartTime = (DateTime?) Session["CurrTaskStartTime"];
                    int currTask = (int) Session["CurrTask"];
                    DateTime? nextTaskStartTime = (DateTime?) Session["NextTaskStartTime"];
                    bool firstQuestion = (bool) Session["FirstQuestion"];

                    //Check the total remaining duration in the survey
                    //if remaining duration is zero or less, proceed to exit
                    //TODO: set database exit settings

                    Session["CurrTaskStartTimeWR"] = Session["NextTaskStartTimeWR"];
                    string WRCurrTasksIdTemp = (string) Session["WRCurrTasksId"];
                    List<int> IdsTemp = WRCurrTasksIdTemp.Split(',').Select(int.Parse).ToList();


                    if (remainingDurationWR <= 0 || (IdsTemp.Count <= 1))
                    {
                        //Exit WR

                        Session["CurrTaskStartTimeWR"] = null;
                        Session["NextTaskStartTimeWR"] = null;
                        Session["WRCurrTasksId"] = null;
                        Session["CurrTaskWR"] = null;
                        Session["RemainingDurationWR"] = null;
                        Session["CurrTaskEndTimeWR"] = null;
                        Session["CurrTaskStartTimeWR"] = null;
                        Session["NextTaskStartTimeWR"] = null;

                        if (remainingDuration <= 0)
                        {
                            //Exit survey
                            //Update Status

                            await surveyService.SetWRSettingsAsyncPOST(
                                  Constants.StatusSurveyProgress.AddShiftTime.ToString(),
                                  surveyId,
                                  currRound,
                                  remainingDuration,
                                  currTaskEndTime,
                                  currTaskStartTime,
                                  currTask,
                                  nextTaskStartTime,
                                  firstQuestion,
                                  null,
                                  null,
                                  0,
                                  null,
                                  null,
                                  null,
                                  null,
                                  null
                          );

                            //await SetRating2AsyncPOSTResult(surveyId);
                            //SetReminderAsyncPOSTResult(surveyId);
                            await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post, Constants.PageType.Exit);
                            //return RedirectToAction("Results");
                            return RedirectToAction("AddShiftTime", new { v.Uid });
                        }
                        else
                        {

                            await surveyService.SetWRSettingsAsyncPOST(
                                  Constants.StatusSurveyProgress.Tasks.ToString(),
                                  surveyId,
                                  currRound,
                                  remainingDuration,
                                  currTaskEndTime,
                                  currTaskStartTime,
                                  currTask,
                                  nextTaskStartTime,
                                  firstQuestion,
                                  null,
                                  null,
                                  0,
                                  null,
                                  null,
                                  null,
                                  null,
                                  null
                          );


                            Session["CurrTaskStartTime"] = Session["NextTaskStartTime"];
                            return RedirectToAction("Tasks");
                        }

                    }
                    else
                    {
                        Session["CurrTaskStartTimeWR"] = Session["NextTaskStartTimeWR"];

                        string WRCurrTasksId = (string) Session["WRCurrTasksId"];
                        List<int> Ids = WRCurrTasksId.Split(',').Select(int.Parse).ToList();
                        Ids.RemoveAt(0);

                        int WRCurrTaskId = Ids[0];
                        Session["CurrTaskWR"] = WRCurrTaskId;

                        WRCurrTasksId = string.Join(",", Ids.Select(n => n.ToString()).ToArray());
                        Session["WRCurrTasksId"] = WRCurrTasksId;

                        int WRremainingDuration = (int) Session["RemainingDurationWR"];
                        DateTime? WRCurrTaskEndTime = (DateTime?) Session["CurrTaskEndTimeWR"];
                        DateTime? WRCurrTaskStartTime = (DateTime?) Session["CurrTaskStartTimeWR"];
                        DateTime? WRNextTaskStartTime = (DateTime?) Session["NextTaskStartTimeWR"];


                        await surveyService.SetWRSettingsAsyncPOST(
                            Constants.StatusSurveyProgress.WRTaskTimeInd.ToString(),
                            surveyId,
                            currRound,
                            remainingDuration,
                            currTaskEndTime,
                            currTaskStartTime,
                            currTask,
                            nextTaskStartTime,
                            firstQuestion,
                            WRCurrTaskId,
                            WRCurrTasksId,
                            WRremainingDuration,
                            WRCurrTaskStartTime,
                            WRCurrTaskEndTime,
                            WRNextTaskStartTime
                            );

                        return RedirectToAction("WRTaskTimeInd");

                    }


                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Rating2, Constants.PageAction.Post);
                    return RedirectToAction("SurveyError");

                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }

        public async Task<ActionResult> WREditResponse(int taskId)
        {

            try
            {
                SurveyEditResponseViewModel v = new SurveyEditResponseViewModel();

                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];
                string shiftSpan = (string) Session["ShiftSpan"];
                string surveySpan = (string) Session["SurveySpan"];


                //TODO: save to a session task list for quick retrieval
                //or get it from Results()
                TaskItemDto task = null;


                if (Session["TaskList"] != null)
                {
                    var taskList = (IList<TaskVM>) Session["TaskList"];
                    task = taskList.Where(m => (m.ID == taskId))
                                            .Select(m => new TaskItemDto()
                                            {
                                                Id = m.ID,
                                                LongName = m.LongName,
                                                ShortName = m.Name
                                            })
                                            .Single();
                }
                else
                {
                    task = surveyService.GetTaskByTaskId(taskId);
                }

                //v.TaskName = db.TaskItems.Where(x => x.Id == taskId).SingleOrDefault().ShortName;
                v.TaskName = task.ShortName;


                Session["CurrTask"] = task.Id;
                //-Session["CurrTaskStartTime"] = taskStartDate;

                //-await pageStatSvc.Insert(surveyId, taskId, taskStartDate, true, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get, Constants.PageType.Enter);

                bool isAny = false;

                //v.TaskId = taskId;
                //v.TaskName = taskName;
                //v.TaskStartDateTime = taskStartDate;

                //var listOfResponses = await svc.GetAllAsync(taskId, surveyId, taskStartDate);
                var listOfResponses = surveyService.GetAllWRResponseAsync(taskId, profileId, surveyId);
                var response = listOfResponses.Where(t => t.Question == v.Q1DB).SingleOrDefault();

                if (response.TaskOther != null)
                {
                    v.TaskName = response.TaskOther;
                }

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
                {
                    v.Q1Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q2DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q2Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q2Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q3DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q3Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q3Ans = Constants.NA_7Rating;
                }


                response = listOfResponses.Where(t => t.Question == v.Q4DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q4Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q4Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q5DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q5Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q5Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q6DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q6Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q6Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q7DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q7Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q7Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q8DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q8Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q8Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q9DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q9Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q9Ans = Constants.NA_7Rating;
                }

                response = listOfResponses.Where(t => t.Question == v.Q10DB).SingleOrDefault();
                if (response != null)
                {
                    v.Q10Ans = Constants.GetInt7ScaleRating(response.Answer);
                    isAny = true;
                }
                else
                {
                    v.Q10Ans = Constants.NA_7Rating;
                }


                if (isAny)
                {
                    v.IsExist = true;
                }

                //await pageStatSvc.Insert(surveyId, taskId, taskStartDate, false, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get, Constants.PageType.Exit);

                if (Request.IsAjaxRequest())
                {
                    return PartialView(v);
                }
                return View(v);
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }

        [HttpPost]
        public async Task<ActionResult> WREditResponse(SurveyEditResponseViewModel v)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    int taskId = (int) Session["CurrTask"];
                    DateTime startTime = (DateTime) Session["CurrTaskStartTime"];

                    await pageStatSvc.Insert(surveyId, taskId, startTime, false, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post, Constants.PageType.Enter);


                    //DateTime endTime = (DateTime) Session["CurrTaskEndTime"];

                    //currentTaskList.Where(w => w.TaskId == v.TaskId).ToList().ForEach(s => s.IsCompleted = true);


                    // Save to db
                    #region update Question  
                    //var listOfResponses = await svc.GetAllAsync(taskId, surveyId, startTime);
                    var listOfResponses = surveyService.GetAllWRResponseAsync(taskId, profileId, surveyId, startTime);


                    var response = listOfResponses.Where(r => r.Question == v.Q1DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q1Ans);
                        surveyService.UpdateResponse(response);
                        //await svc.UpdateAsync(response, ModelState);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q2DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q2Ans);
                        surveyService.UpdateResponse(response);
                        //await svc.UpdateAsync(response, ModelState);
                    }

                    response = listOfResponses.Where(r => r.Question == v.Q3DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q3Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q4DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q4Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q5DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q5Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);

                    }


                    response = listOfResponses.Where(r => r.Question == v.Q6DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q6Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }


                    response = listOfResponses.Where(r => r.Question == v.Q7DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q7Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }


                    response = listOfResponses.Where(r => r.Question == v.Q8DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q8Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q9DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q9Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }
                    response = listOfResponses.Where(r => r.Question == v.Q10DB).SingleOrDefault();
                    if (response != null)
                    {
                        response.Answer = Constants.GetText7ScaleRating(v.Q10Ans);
                        //await svc.UpdateAsync(response, ModelState);
                        surveyService.UpdateResponse(response);
                    }

                    surveyService.SaveResponse();


                    #endregion

                    //-await pageStatSvc.Insert(surveyId, taskId, startTime, true, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post, Constants.PageType.Exit);
                    return RedirectToAction("Results");


                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_EditResponse, Constants.PageAction.Post);
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



        public async Task<ActionResult> Feedback()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];
                    Session["ProgressValueValue"] = 100;
                    string uid = (string) Session["SurveyUID"];


                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
                    SurveyFeedbackViewModel v = new SurveyFeedbackViewModel();
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;
                    v.Uid = uid;

                    
                    //await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get, Constants.PageType.Exit, profileId);
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
                LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
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
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];

                    ProfileDetailsByClient pc = new ProfileDetailsByClient();
                    pc = (ProfileDetailsByClient)Session["ClientByProfile"];

                    surveyService.SetFeedbackAsyncPOST(
                           surveyId, Constants.StatusSurveyProgress.Completed.ToString(),v.Comment
                           );

                    if (Session["LoopDemo"] != null && Session["MyDayV2"] == null)
                    {
                        if (Session["IsItWam"] == "Yes")
                        { return RedirectToAction("WAMSurveyResults"); }
                        else { return RedirectToAction("LoopSurveyResults"); }
                    }
                    else if (Session["MyDayV2"] != null && Session["LoopDemo"] == null)
                    { return RedirectToAction("TaskSummary"); }
                    else if (pc.ClientInitials.ToLower().ToString() == "wam")
                    { return RedirectToAction("WAMSurveyResults"); }
                    else
                    {
                        return RedirectToAction("Results");
                    }
                }

                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
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
        public async Task<ActionResult> TaskSummary()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    Session["ProgressValueValue"] = 100;
                                        
                    SurveyResults1ViewModel v = new SurveyResults1ViewModel();

                    var survey = surveyService.GetSurveyById(surveyId);
                    v.Comment = survey.Comment;

                    SurveyTaskDurationVM tdresponse = new SurveyTaskDurationVM();
                    tdresponse.MyDayTaskListsObj = new List<MyDayTaskList>();
                    tdresponse.MyDayTaskListsObj = surveyService.GetAllTaskByProfileID(profileId).ToList<MyDayTaskList>();

                    var responsesGeneric = surveyService.GetGenericResponseAffects(surveyId, profileId);                    
                    var tasklist = surveyService.GetAllTask();
                    Session["TaskList"] = tasklist; //to be used on Edit Response
                    
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;

                    SurveyTaskTime1ViewModel ref1 = new SurveyTaskTime1ViewModel();
                    SurveyTaskRating1ViewModel ref2 = new SurveyTaskRating1ViewModel();
                    SurveyTaskRating2ViewModel ref3 = new SurveyTaskRating2ViewModel();
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

                    List<Tuple<string, SurveyResponseAffectVM>> genericList = 
                                                            new List<Tuple<string, SurveyResponseAffectVM>>();                   
//                    DateTime? taskStartDate = null;
                    int totalMins = 0;

                    //Generic
                    foreach (var k in responsesGeneric)
                    {
                        SurveyResponseAffectVM r = new SurveyResponseAffectVM();
                        
                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();
                        r.TaskId = k.TaskId;
                        r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();

                        //if (k.Question == Constants.QDB)
                        //{ totalMins = int.Parse(k.Answer.ToString()); }
                        
                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;
                        r.RatingString = surveyService.GetRatingString(k.Answer);
                        r.TaskName = taskDet.Name;

                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }

                        //string s = @"([<>\?\*\\\""/\|\(\)])+";
                        //Regex rg = new Regex(s);
                        //char[] splitParams = new char[] { '(', ')','/','\\','[',']' };
                        ////string[] splitResult = toSplit.Split(splitParams);

                        //if (rg.IsMatch(r.TaskName) == true)
                        //{
                        //    r.TaskName.Split(splitParams);
                        //    r.TaskName = r.TaskName[0].ToString();
                        //}

                        r.TaskDescription = taskDet.LongName;
                        //r.TaskDuration = totalMins.ToString() + "min";
                        foreach (var z in tdresponse.MyDayTaskListsObj)
                        {
                            if (z.TaskId == k.TaskId)
                            {
                                r.TaskDuration = z.TaskDuration.ToString();                                
                            }
                        }
                        r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");                        
                        genericList.Add(Tuple.Create(r.TaskName + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                    }
                    v.FullResponseAffectList = (Lookup<string, SurveyResponseAffectVM>) genericList.ToLookup(t => t.Item1, t => t.Item2);
                    
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        public async Task<ActionResult> LoopSurveyResults()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    Session["ProgressValueValue"] = 100;

                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
                    SurveyResults1ViewModel v = new SurveyResults1ViewModel();

                    var survey = surveyService.GetSurveyById(surveyId);
                    v.Comment = survey.Comment;

                    var responsesGeneric = surveyService.GetGenericResponses(surveyId, profileId);
                    var responsesAdditional = surveyService.GetAdditionalResponses(surveyId, profileId);
                    var responsesWR = surveyService.GetWRResponses(surveyId, profileId);
                    var tasklist = surveyService.GetAllTask();
                    Session["TaskList"] = tasklist; //to be used on Edit Response

                    //var tasklist = await db.TaskItems.ToListAsync();
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;


                    SurveyTaskTime1ViewModel ref1 = new SurveyTaskTime1ViewModel();
                    SurveyTaskRating1ViewModel ref2 = new SurveyTaskRating1ViewModel();
                    SurveyTaskRating2ViewModel ref3 = new SurveyTaskRating2ViewModel();
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

                    List<Tuple<string, SurveyResponseViewModel>> genericList = new List<Tuple<string, SurveyResponseViewModel>>();
                    List<Tuple<string, SurveyResponseViewModel>> additionalList = new List<Tuple<string, SurveyResponseViewModel>>();
                    List<Tuple<string, SurveyResponseWRViewModel>> wrList = new List<Tuple<string, SurveyResponseWRViewModel>>();



                    DateTime? taskStartDate = null;

                    int totalMins = 0;

                    //Generic
                    foreach (var k in responsesGeneric)
                    {
                        SurveyResponseViewModel r = new SurveyResponseViewModel();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();
                        r.TaskId = k.TaskId;
                        r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();



                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }


                        r.TaskDescription = taskDet.LongName;
                        //if (k.TaskOther != null)
                        //{
                        //    r.TaskDescription = k.TaskOther;
                        //}


                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                        //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                        genericList.Add(Tuple.Create(r.TaskName + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                    }

                    v.FullResponseList = (Lookup<string, SurveyResponseViewModel>) genericList.ToLookup(t => t.Item1, t => t.Item2);


                    //Additional
                    foreach (var k in responsesAdditional)
                    {
                        SurveyResponseViewModel r = new SurveyResponseViewModel();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();


                        r.TaskId = k.TaskId;
                        //r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        //r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        //r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();



                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }


                        r.TaskDescription = taskDet.LongName;
                        //if (k.TaskOther != null)
                        //{
                        //    r.TaskDescription = k.TaskOther;
                        //}


                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt") + "- " + k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                        //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                        //additionalList.Add(Tuple.Create(r.TaskName, r));
                        additionalList.Add(Tuple.Create(r.TaskName + " STARTDATESANADD", r));

                    }


                    v.FullResponseAdditionalList = (Lookup<string, SurveyResponseViewModel>) additionalList.ToLookup(t => t.Item1, t => t.Item2);



                    //Additional
                    foreach (var k in responsesWR)
                    {
                        SurveyResponseWRViewModel r = new SurveyResponseWRViewModel();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();


                        r.TaskId = k.TaskId;
                        //r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        //r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        //r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();



                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }


                        r.TaskDescription = taskDet.LongName;
                        //if (k.TaskOther != null)
                        //{
                        //    r.TaskDescription = k.TaskOther;
                        //}

                        r.WRStartDateTime = k.WardRoundWindowStartDateTime;

                        if (k.WardRoundWindowStartDateTime.HasValue)
                        {
                            //r.WRStartDate = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt");
                            r.WRStartTime = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt");
                        }



                        r.WREndDateTime = k.WardRoundWindowEndDateTime;
                        if (k.WardRoundWindowEndDateTime.HasValue)
                        {
                            //r.WRStartDate = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt");
                            r.WREndTime = k.WardRoundWindowEndDateTime.Value.ToString("hh:mm tt");
                        }

                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                        //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                        wrList.Add(Tuple.Create(r.TaskName + " STARTDATESANWRR " + k.TaskStartDateTime.Value, r));

                    }


                    v.FullResponseWRList = (Lookup<string, SurveyResponseWRViewModel>) wrList.ToLookup(t => t.Item1, t => t.Item2);



                    //await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get, Constants.PageType.Exit, profileId);


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
                LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        public async Task<ActionResult> WAMSurveyResults()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

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
                    v.FullWAMResponseList = (Lookup<string, SurveyWAMResponseVM>) genericList.ToLookup(t => t.Item1, t => t.Item2);
                    
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
                        {  r.TaskName = k.TaskOther;}

                        r.TaskDescription = taskDet.LongName;

                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt") + "- " + k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                        additionalList.Add(Tuple.Create(r.TaskName + " STARTDATESANADD", r));
                    }
                    v.FullWAMResponseAdditionalList = (Lookup<string, SurveyWAMResponseVM>) additionalList.ToLookup(t => t.Item1, t => t.Item2);
                    
                    if (Request.IsAjaxRequest())
                    { return PartialView(v); }
                    return View(v);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");
            }
        }
        [HttpPost]
        public async Task<ActionResult> WAMSurveyResults(SurveyResults1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }
        public async Task<ActionResult> Results()
        {
            try
            {
                if (Session["SurveyId"] != null)
                {
                    int surveyId = (int) Session["SurveyId"];
                    int profileId = (int) Session["ProfileId"];
                    string shiftSpan = (string) Session["ShiftSpan"];
                    string surveySpan = (string) Session["SurveySpan"];

                    Session["ProgressValueValue"] = 100;

                


                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get, Constants.PageType.Enter, profileId);
                    SurveyResults1ViewModel v = new SurveyResults1ViewModel();

                    var survey = surveyService.GetSurveyById(surveyId);
                    v.Comment = survey.Comment;

                    var responsesGeneric = surveyService.GetGenericResponses(surveyId, profileId);
                    var responsesAdditional = surveyService.GetAdditionalResponses(surveyId, profileId);
                    var responsesWR = surveyService.GetWRResponses(surveyId, profileId);


                    

                    var tasklist = surveyService.GetAllTask();

                    Session["TaskList"] = tasklist; //to be used on Edit Response

                    //var tasklist = await db.TaskItems.ToListAsync();
                    v.ShiftSpan = shiftSpan;
                    v.SurveySpan = surveySpan;


                    SurveyTaskTime1ViewModel ref1 = new SurveyTaskTime1ViewModel();
                    SurveyTaskRating1ViewModel ref2 = new SurveyTaskRating1ViewModel();
                    SurveyTaskRating2ViewModel ref3 = new SurveyTaskRating2ViewModel();
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

                    List<Tuple<string, SurveyResponseViewModel>> genericList = new List<Tuple<string, SurveyResponseViewModel>>();
                    List<Tuple<string, SurveyResponseViewModel>> additionalList = new List<Tuple<string, SurveyResponseViewModel>>();
                    List<Tuple<string, SurveyResponseWRViewModel>> wrList = new List<Tuple<string, SurveyResponseWRViewModel>>();



                    DateTime? taskStartDate = null;

                    int totalMins = 0;

                    //Generic
                    foreach (var k in responsesGeneric)
                    {
                        SurveyResponseViewModel r = new SurveyResponseViewModel();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();
                        r.TaskId = k.TaskId;
                        r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();



                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }


                        r.TaskDescription = taskDet.LongName;
                        //if (k.TaskOther != null)
                        //{
                        //    r.TaskDescription = k.TaskOther;
                        //}


                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                        //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                        genericList.Add(Tuple.Create(r.TaskName + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                    }

                    v.FullResponseList = (Lookup<string, SurveyResponseViewModel>) genericList.ToLookup(t => t.Item1, t => t.Item2);


                    //Additional
                    foreach (var k in responsesAdditional)
                    {
                        SurveyResponseViewModel r = new SurveyResponseViewModel();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();


                        r.TaskId = k.TaskId;
                        //r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        //r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        //r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();



                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }


                        r.TaskDescription = taskDet.LongName;
                        //if (k.TaskOther != null)
                        //{
                        //    r.TaskDescription = k.TaskOther;
                        //}


                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt") + "- " + k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
                        //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                        //additionalList.Add(Tuple.Create(r.TaskName, r));
                        additionalList.Add(Tuple.Create(r.TaskName + " STARTDATESANADD", r));

                    }


                    v.FullResponseAdditionalList = (Lookup<string, SurveyResponseViewModel>) additionalList.ToLookup(t => t.Item1, t => t.Item2);



                    //Additional
                    foreach (var k in responsesWR)
                    {
                        SurveyResponseWRViewModel r = new SurveyResponseWRViewModel();

                        var taskDet = tasklist
                                            .Where(m => m.ID == k.TaskId)
                                            .Select(m => new { m.LongName, m.Name })
                                            .Single();


                        r.TaskId = k.TaskId;
                        //r.TaskStartDateTime = k.TaskStartDateTime.Value;
                        //r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
                        //r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();



                        if (k.Question == Constants.QDB)
                        {
                            totalMins = int.Parse(k.Answer.ToString());
                        }

                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;

                        r.RatingString = surveyService.GetRatingString(k.Answer);

                        r.TaskName = taskDet.Name;
                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }


                        r.TaskDescription = taskDet.LongName;
                        //if (k.TaskOther != null)
                        //{
                        //    r.TaskDescription = k.TaskOther;
                        //}

                        r.WRStartDateTime = k.WardRoundWindowStartDateTime;

                        if (k.WardRoundWindowStartDateTime.HasValue)
                        {
                            //r.WRStartDate = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt");
                            r.WRStartTime = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt");
                        }



                        r.WREndDateTime = k.WardRoundWindowEndDateTime;
                        if (k.WardRoundWindowEndDateTime.HasValue)
                        {
                            //r.WRStartDate = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt");
                            r.WREndTime = k.WardRoundWindowEndDateTime.Value.ToString("hh:mm tt");
                        }

                        r.TaskDuration = totalMins.ToString() + "min";
                        r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                        //list.Add(Tuple.Create(taskDet.Name + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
                        wrList.Add(Tuple.Create(r.TaskName + " STARTDATESANWRR " + k.TaskStartDateTime.Value, r));

                    }


                    v.FullResponseWRList = (Lookup<string, SurveyResponseWRViewModel>) wrList.ToLookup(t => t.Item1, t => t.Item2);



                    //await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get, Constants.PageType.Exit, profileId);


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
                LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }

        [HttpPost]
        public async Task<ActionResult> Results(SurveyResults1ViewModel v)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    //int surveyId = (int) Session["SurveyId"];
                    ////pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Post, Constants.PageType.Enter);

                    ////pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Post, Constants.PageType.Exit);

                    ////await SetSummaryAsyncPOST(surveyId);
                    //await surveyService.SetSummaryAsyncPOST(
                    //    surveyId
                    //    );

                    
                        return RedirectToAction("Index", "Home");
                   
                }

                catch (Exception ex)
                {
                    LogError(ex, Constants.PageName.Survey_Original_Summary, Constants.PageAction.Get);
                    return RedirectToAction("SurveyError");
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult EmailFeedback(EmailFeedbackViewModel v)
        {
            try
            {
                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];

                surveyService.CreateUserFeedback(surveyId, profileId, string.Empty, v);

                //if (Session["ProfileId"] != null)
                //{
                //int profileId = (int) Session["ProfileId"];



                //await LogSessionError(Constants.PageName.Registration_Screening);
                //return RedirectToAction("SessionError");
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //LogError(ex, Constants.PageName.Registration_Screening, Constants.PageAction.Post);
                //return RedirectToAction("RegistrationError");
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);

            }
        }







        #region Miscellaneous Views
        [HttpGet]
        public ActionResult InvalidError()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }
        [HttpPost]
        public ActionResult InvalidError(InvalidError v)
        {
            if (Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("Index", "Home");
        }
        //public ActionResult TaskLoadingError()
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView();

        //    }
        //    return View();
        //}
        //public ActionResult TaskNotSelectedError()
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView();

        //    }
        //    return View();
        //}
        [HttpGet]
        public ActionResult SurveyError()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }
        [HttpPost]
        public ActionResult SurveyError(SurveyError v)
        {
            if (Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConnectionError()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }
        [HttpPost]
        public ActionResult ConnectionError(SurveyError v)
        {
            if (Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<ActionResult> CompletedSurvey(string id)
        {
            try
            {
                SidMV v = new SidMV();
                int surveyId = (int) Session["SurveyId"];
                await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Completed_Already, Constants.PageAction.Get, Constants.PageType.Enter, null, "Uid = " + id);
                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                if (!string.IsNullOrEmpty(id))
                {
                    v.Uid = id;
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.Exit, null, "Uid = " + v.Uid);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    else
                    {
                        return View(v);
                    }
                }
                else
                {
                    await LogMyDayError(id, "CompletedSurvey GET: Survey UID not found!", "InvalidError");
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.ERROR, null, "Invalid Uid = " + v.Uid);
                    return RedirectToAction("InvalidError");
                }
            }
            catch (Exception ex)
            {
                LogError(ex, Constants.PageName.Survey_Original_Completed_Already, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }
        [HttpPost]
        public async Task<ActionResult> CompletedSurvey(SidMV v)
        {
            //if (Request.IsAjaxRequest())
            //{
            //    return RedirectToAction("Index", "Home");

            //}
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public async Task<ActionResult> ExpiredSurvey(string id)
        {
            try
            {
                SidMV v = new SidMV();

                int surveyId = (int) Session["SurveyId"];

                await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Expired, Constants.PageAction.Get, Constants.PageType.Enter, null, "Uid = " + id);

                v.ShiftSpan = (string) Session["ShiftSpan"];
                v.SurveySpan = (string) Session["SurveySpan"];

                if (!string.IsNullOrEmpty(id))
                {
                    v.Uid = id;
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.Exit, null, "Uid = " + v.Uid);
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView(v);
                    }
                    else
                    {
                        return View(v);
                    }
                }
                else
                {
                    await LogMyDayError(id, "ExpiredSurvey GET: Survey UID not found!", "InvalidError");
                    await pageStatSvc.Insert(surveyId, null, null, false, Constants.PageName.Survey_Original_Refresh, Constants.PageAction.Get, Constants.PageType.ERROR, null, "Invalid Uid = " + v.Uid);
                    return RedirectToAction("InvalidError");
                }
            }
            catch (Exception ex)
            {

                LogError(ex, Constants.PageName.Survey_Original_Expired, Constants.PageAction.Get);
                return RedirectToAction("SurveyError");

            }
        }

        [HttpPost]
        public async Task<ActionResult> ExpiredSurvey(SidMV v)
        {
            //if (Request.IsAjaxRequest())
            //{
            //    return RedirectToAction("Index", "Home");

            //}
            return RedirectToAction("Index", "Home");
        }
        #endregion

        


    }
}