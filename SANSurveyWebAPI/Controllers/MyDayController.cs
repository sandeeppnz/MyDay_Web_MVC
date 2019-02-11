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
    public class MyDayController : BaseController
    {
        private PageStatService pageStatSvc;
        private Survey3Service surveyService;
        private HangfireScheduler schedulerService;
        private UserHomeService userHomeService;
        private ProfileService profileService;            

        private ApplicationDbContext db = new ApplicationDbContext();
        public MyDayController()
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
        //public async Task<ActionResult> View(string id)
        //{
        //    try
        //    {
        //        int profileId = 0;
        //        if (Session["ProfileId"] == null)
        //        {Session["ProfileId"] = profileService.GetCurrentProfileIdNonAsync(); }
        //        else
        //        {profileId = (int) Session["ProfileId"];}

        //        var survey = surveyService.GetSurveyByUid(id);
        //        int surveyId = 0;
        //        if (survey != null)
        //        { surveyId = survey.Id; }

        //        SurveyNonEditViewModel v = new SurveyNonEditViewModel();
        //        v.notifications = new List<NotificationVM>();
        //        v.surveySpan = survey.SurveyDescription;
        //        v.notifications = await userHomeService.GetNotificationList(GetBaseURL(), profileId);
        //        var responsesGeneric = surveyService.GetGenericResponses(surveyId, profileId);
        //        var responsesAdditional = surveyService.GetAdditionalResponses(surveyId, profileId);
        //        var responsesWR = surveyService.GetWRResponses(surveyId, profileId);
        //        var tasklist = surveyService.GetAllTask();

        //        SurveyTaskTime1ViewModel ref1 = new SurveyTaskTime1ViewModel();
        //        SurveyTaskRating1ViewModel ref2 = new SurveyTaskRating1ViewModel();
        //        SurveyTaskRating2ViewModel ref3 = new SurveyTaskRating2ViewModel();
        //        Dictionary<string, string> refenceTable = new Dictionary<string, string>();

        //        refenceTable.Add(ref1.QDB, ref1.QDisplayShort);
        //        refenceTable.Add(ref2.Q1DB, ref2.Q1DisplayShort);
        //        refenceTable.Add(ref2.Q2DB, ref2.Q2DisplayShort);
        //        refenceTable.Add(ref2.Q3DB, ref2.Q3DisplayShort);
        //        refenceTable.Add(ref2.Q4DB, ref2.Q4DisplayShort);
        //        refenceTable.Add(ref2.Q5DB, ref2.Q5DisplayShort);
        //        refenceTable.Add(ref3.Q6DB, ref3.Q6DisplayShort);
        //        refenceTable.Add(ref3.Q7DB, ref3.Q7DisplayShort);
        //        refenceTable.Add(ref3.Q8DB, ref3.Q8DisplayShort);
        //        refenceTable.Add(ref3.Q9DB, ref3.Q9DisplayShort);
        //        refenceTable.Add(ref3.Q10DB, ref3.Q10DisplayShort);
                                
        //        List<Tuple<string, SurveyResponseViewModel>> genericList = new List<Tuple<string, SurveyResponseViewModel>>();
        //        List<Tuple<string, SurveyResponseViewModel>> additionalList = new List<Tuple<string, SurveyResponseViewModel>>();
        //        List<Tuple<string, SurveyResponseWRViewModel>> wrList = new List<Tuple<string, SurveyResponseWRViewModel>>();
        //        DateTime? taskStartDate = null;
        //        int totalMins = 0;

        //        foreach (var k in responsesGeneric)
        //        {
        //            SurveyResponseViewModel r = new SurveyResponseViewModel();
        //            var taskDet = tasklist
        //                                .Where(m => m.ID == k.TaskId)
        //                                .Select(m => new { m.LongName, m.Name })
        //                                .Single();
        //            r.TaskId = k.TaskId;
        //            r.TaskStartDateTime = k.TaskStartDateTime.Value;
        //            r.TaskStartDate = k.TaskStartDateTime.Value.ToLongDateString();
        //            r.TaskStartTime = k.TaskStartDateTime.Value.ToLongTimeString();

        //            if (k.Question == Constants.QDB)
        //            { totalMins = int.Parse(k.Answer.ToString()); }

        //            r.Question = refenceTable[k.Question].Trim(); //get display qns
        //            r.Answer = k.Answer;
        //            r.RatingString = surveyService.GetRatingString(k.Answer);
        //            r.TaskName = taskDet.Name;
        //            if (k.TaskOther != null)
        //            { r.TaskName = k.TaskOther; }
        //            r.TaskDescription = taskDet.LongName;
        //            r.TaskDuration = totalMins.ToString() + "min";
        //            r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                   
        //            genericList.Add(Tuple.Create(r.TaskName + " STARTDATESAN " + k.TaskStartDateTime.Value, r));
        //        }

        //        v.FullResponseList = (Lookup<string, SurveyResponseViewModel>) genericList.ToLookup(t => t.Item1, t => t.Item2);

        //        //Additional
        //        foreach (var k in responsesAdditional)
        //        {
        //            SurveyResponseViewModel r = new SurveyResponseViewModel();

        //            var taskDet = tasklist
        //                                .Where(m => m.ID == k.TaskId)
        //                                .Select(m => new { m.LongName, m.Name })
        //                                .Single();
        //            r.TaskId = k.TaskId;

        //            if (k.Question == Constants.QDB)
        //            { totalMins = int.Parse(k.Answer.ToString()); }

        //            r.Question = refenceTable[k.Question].Trim(); //get display qns
        //            r.Answer = k.Answer;
        //            r.RatingString = surveyService.GetRatingString(k.Answer);
        //            r.TaskName = taskDet.Name;
        //            if (k.TaskOther != null)
        //            {  r.TaskName = k.TaskOther; }

        //            r.TaskDescription = taskDet.LongName;
        //            r.TaskDuration = totalMins.ToString() + "min";
        //            r.TaskTimeSpan = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt") + "- " + k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");
        //            additionalList.Add(Tuple.Create(r.TaskName + " STARTDATESANADD", r));
        //        }
        //        v.FullResponseAdditionalList = (Lookup<string, SurveyResponseViewModel>) additionalList.ToLookup(t => t.Item1, t => t.Item2);
                
        //        //Additional
        //        foreach (var k in responsesWR)
        //        {
        //            SurveyResponseWRViewModel r = new SurveyResponseWRViewModel();
        //            var taskDet = tasklist
        //                                .Where(m => m.ID == k.TaskId)
        //                                .Select(m => new { m.LongName, m.Name })
        //                                .Single();
        //            r.TaskId = k.TaskId;
        //            if (k.Question == Constants.QDB)
        //            {totalMins = int.Parse(k.Answer.ToString()); }
        //            r.Question = refenceTable[k.Question].Trim(); //get display qns
        //            r.Answer = k.Answer;
        //            r.RatingString = surveyService.GetRatingString(k.Answer);
        //            r.TaskName = taskDet.Name;
        //            if (k.TaskOther != null)
        //            { r.TaskName = k.TaskOther; }
        //            r.TaskDescription = taskDet.LongName;
        //            r.WRStartDateTime = k.WardRoundWindowStartDateTime;
        //            if (k.WardRoundWindowStartDateTime.HasValue)
        //            { r.WRStartTime = k.WardRoundWindowStartDateTime.Value.ToString("hh:mm tt"); }
        //            r.WREndDateTime = k.WardRoundWindowEndDateTime;
        //            if (k.WardRoundWindowEndDateTime.HasValue)
        //            { r.WREndTime = k.WardRoundWindowEndDateTime.Value.ToString("hh:mm tt");}
        //            r.TaskDuration = totalMins.ToString() + "min";
        //            r.TaskTimeSpan = k.TaskStartDateTime.Value.ToString("hh:mm tt") + "- " + k.TaskEndDateTime.Value.ToString("hh:mm tt");
                    
        //            wrList.Add(Tuple.Create(r.TaskName + " STARTDATESANWRR " + k.TaskStartDateTime.Value, r));
        //        }
        //        v.FullResponseWRList = (Lookup<string, SurveyResponseWRViewModel>) wrList.ToLookup(t => t.Item1, t => t.Item2);
                
        //        if (Request.IsAjaxRequest())
        //        {return PartialView(v); }
        //        return View(v);
        //    }
        //    catch (Exception ex)
        //    {
        //        string EMsg = "View:: Exception Message: " + ex.Message + " InnerException: " +ex.InnerException;
        //        await LogMyDayError(id, EMsg, "SurveyError");
        //        LogError(ex, Constants.PageName.Survey_Original_ViewResponse, Constants.PageAction.Get);
        //        return RedirectToAction("SurveyError");
        //    }
        //}

        #region MinorViews
        private void LogError(Exception ex, Constants.PageName pageName, Constants.PageAction pageAction)
        {
            SurveyError er = new ViewModels.Web.SurveyError();
            er.Message = ex.Message;
            er.StackTrace = ex.StackTrace;
            er.CodeFile = this.GetType().Name;

            pageStatSvc.Insert(null, null, null, false, pageName, pageAction, Constants.PageType.ERROR, null, er.DisplayError());
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


            //Session["CurrRound"] = null;
            //Session["RemainingDuration"] = null;
            //Session["CurrTaskEndTime"] = null;
            //Session["NextTaskStartTime"] = null;
            //Session["FirstQuestion"] = null;
            //Session["CurrProgressValue"] = null;
            //Session["ShiftStartTime"] = null;
            //Session["ShiftEndTime"] = null;
            //Session["SurveyStartTime"] = null;
            //Session["SurveyEndTime"] = null;
            //Session["SurveyDuration"] = null;
            //Session["SurveyExpiryDate"] = null;
            //Session["SurveyProgressNext"] = null;
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
            return RedirectToAction("SignupForDemo","Account", new { uid = Session["UID"].ToString(), surveyID = Session["SurveyUID"].ToString()});            
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
                    await LoadSurveySessionsBySurveyUid(id);
                    Session["ProgressValueValue"] = 0;
                    SurveyDto s = surveyService.GetSurveyByUid(id);
                    if (s != null)
                    {
                        await pageStatSvc.Insert(s.Id, null, null, false, Constants.PageName.Survey_Original_Start, Constants.PageAction.Get, Constants.PageType.Enter, s.ProfileId, id);

                        #region check survey table if completed
                        switch (s.SurveyProgressNext)
                        {
                            case "Invited":
                                return RedirectToAction("NewSurvey", new { uid = id });
                            case "Completed":
                                return RedirectToAction("CompletedSurvey", new { id = id });
                            case "ShiftTime":
                            case "MultiTask":
                            case "TaskView":
                            case "AffectStage1":
                            case "AffectStage2":
                                return RedirectToAction("ResumeSurvey", new { uid = id });
                            default:
                                break;
                        }
                        #endregion
                    
                        //TODO: Check when this occurs e.g. New
                        return RedirectToAction("ShiftTime");
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
                //string loopDemo = string.Empty;
                //string mydayv2 = string.Empty;

                //if (Session["LoopDemo"] != null && Session["MyDayV2"] == null)
                //{
                //    loopDemo = Session["LoopDemo"].ToString();
                //}
                //else if (Session["MyDayV2"] != null && Session["LoopDemo"] == null)
                //{ mydayv2 = Session["MyDayV2"].ToString(); }
                //else { }

                //if (uid != null && loopDemo == "Yes")
                //{
                //    await LoadSurveySessionsBySurveyUid(uid);
                //    Session["ProgressValueValue"] = 0;
                //    SurveyDto s = surveyService.GetSurveyByUid(uid);
                //}
                //else if (uid != null && mydayv2 == "Yes")
                //{
                    await LoadSurveySessionsBySurveyUid(uid);
                    Session["ProgressValueValue"] = 0;
                    SurveyDto s = surveyService.GetSurveyByUid(uid);
                //}
                //else { Session["DemoForUser"] = "MyDay"; Session["TaskType"] = "doctors"; } ///please check for normal myday

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

                    return RedirectToAction("ShiftTime");   
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

               // Session["DemoForUser"] = "MyDay";
               // Session["TaskType"] = "doctors";

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

                    if (button == "Restart")
                    {
                        await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Resume, Constants.PageAction.Post, Constants.PageType.Exit, profileId, "Clicked Restart");
                        return RedirectToAction("ShiftTime");
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
                    //string demoForUser = Session["DemoForUser"].ToString().ToLower();

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

                    //if (demoForUser == "myday" || demoForUser == "wam")
                    //{ Session["ProgressValueValue"] = 5; }
                    //else if (demoForUser == "mydayv2")
                    //{ Session["ProgressValueValue"] = 10; }
                    //else { Session["ProgressValueValue"] = 5; }

                    await surveyService.RemovePreviousResponses(surveyId);
                    
                    await surveyService.SetShitTimeSettingsAsyncGET(surveyId, profileId, GetBaseURL());
                    
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
                    
                    await surveyService.SetShitTimeSettings(surveyId, v.IsOnCall, v.WasWorking);
                    await pageStatSvc.Insert(surveyId, null, null, true, Constants.PageName.Survey_Original_Shift, Constants.PageAction.Post, Constants.PageType.Exit);

                    if (v.WasWorking != "No")
                    {
                       // if (Session["LoopDemo"] == null && Session["MyDayV2"] != null)
                        { return RedirectToAction("Multitask"); }
                       // else { return RedirectToAction("Tasks"); }
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
                    //string demoForUser = Session["DemoForUser"].ToString().ToLower();
                    //string taskType = Session["TaskType"].ToString();

                    int currProgressValueValue = (int) Session["ProgressValueValue"];

                    //if (demoForUser == "myday" || demoForUser == "wam")
                    //{
                    //    if (currProgressValueValue <= 15)
                    //    { Session["ProgressValueValue"] = 15; }
                    //}
                    //else if (demoForUser == "mydayv2")
                    //{ Session["ProgressValueValue"] = 20; }
                    //else {
                    //    if (currProgressValueValue <= 15)
                    //    { Session["ProgressValueValue"] = 15; }
                    //}
                    

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
                    //string demoForUser = Session["DemoForUser"].ToString().ToLower();

                    //if (demoForUser == "mydayv2")
                    //{ Session["ProgressValueValue"] = 30; }
                                        
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
                       
                        currentaffect = (int) Session["CurrentAffectStage"];
                        currentaffect++;
                        Session["CurrentAffectStage"] = currentaffect;                                                 
                        
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
                    else {
                        return RedirectToAction("Results"); }
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
                        
                        r.Question = refenceTable[k.Question].Trim(); //get display qns
                        r.Answer = k.Answer;
                        r.RatingString = surveyService.GetRatingString(k.Answer);
                        r.TaskName = taskDet.Name;

                        if (k.TaskOther != null)
                        {
                            r.TaskName = k.TaskOther;
                        }                      

                        r.TaskDescription = taskDet.LongName;
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

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EmailFeedback(EmailFeedbackViewModel v)
        {
            try
            {
                int surveyId = (int) Session["SurveyId"];
                int profileId = (int) Session["ProfileId"];

                surveyService.CreateUserFeedback(surveyId, profileId, string.Empty, v);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            { return Json(new { Success = false }, JsonRequestBehavior.AllowGet); }
        }        

        #region Miscellaneous Views

        [HttpGet]
        public ActionResult InvalidError()
        {
            if (Request.IsAjaxRequest())
            { return PartialView(); }
            return View();
        }
        [HttpPost]
        public ActionResult InvalidError(InvalidError v)
        {
            if (Request.IsAjaxRequest())
            {  return RedirectToAction("Index", "Home"); }
            return RedirectToAction("Index", "Home");
        }
       
        [HttpGet]
        public ActionResult SurveyError()
        {
            if (Request.IsAjaxRequest())
            {  return PartialView(); }
            return View();
        }
        [HttpPost]
        public ActionResult SurveyError(SurveyError v)
        {
            if (Request.IsAjaxRequest())
            {  return RedirectToAction("Index", "Home"); }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConnectionError()
        {
            if (Request.IsAjaxRequest())
            {  return PartialView(); }
            return View();
        }
        [HttpPost]
        public ActionResult ConnectionError(SurveyError v)
        {
            if (Request.IsAjaxRequest())
            { return RedirectToAction("Index", "Home"); }
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
                    { return PartialView(v); }
                    else
                    { return View(v);  }
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
                    {  return PartialView(v);  }
                    else
                    { return View(v); }
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
            return RedirectToAction("Index", "Home");
        }

        #endregion     
        
    }
}