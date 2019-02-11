using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System.Data.Entity;
using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using System.Data.Entity.Infrastructure;


using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using static SANSurveyWebAPI.Constants;
using System.ComponentModel;
using Hangfire;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.DAL;

namespace SANSurveyWebAPI.BLL
{
    public class NewSurvey
    {
        public int ProfileId { get; set; }
        public int OffsetUtc { get; set; }
        public int RosterItemId { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public string ScheduledDateTime { get; set; }
        public int SurveyId { get; set; }

        public string BaseUrl { get; set; }
        public string EmailAddress { get; set; }
        public string RecipientName { get; set; }

        public DateTime SurveyStartDateTime { get; set; }

    }

    public class SurveyService
    {
        private static bool UpdateDatabase = true;
        private ApplicationDbContext db;

        private ProfileService profileSvc;

        readonly UnitOfWork _unitOfWork = new UnitOfWork();



        public SurveyService(ApplicationDbContext context)
        {
            db = context;
        }

        public SurveyService()
            : this(new ApplicationDbContext())
        {
            this.profileSvc = new ProfileService();
        }


        public void Dispose()
        {
            profileSvc.Dispose();
            _unitOfWork.Dispose();
            db.Dispose();
        }


        public class HomeMySurvey
        {
            public int SurveyId { get; set; }
            public string SurveyDescription { get; set; }
            public DateTime? SurveyStartDateTime { get; set; }
            public DateTime? SurveyExpiryDateTime { get; set; }
            public string Status { get; set; }
            public string Uid { get; set; }
        }

        
       
        [DisplayName("UpdateSurveyStatus;{1}")]
        public static async Task<int?> UpdateSurveyStatus(NewSurvey e, string msg)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var survey = db.Surveys
                 .Where(x => x.Id == e.SurveyId)
                 .SingleOrDefault();

            survey.SurveyProgressNext = StatusSurveyProgress.Invited.ToString();
            db.Surveys.Attach(survey);
            var manager = ((IObjectContextAdapter) db).ObjectContext.ObjectStateManager;
            manager.ChangeObjectState(survey, EntityState.Modified);
            //Update the entity in the database
            int createSurveyJobId = await db.SaveChangesAsync();
            e.SurveyId = survey.Id;
            return createSurveyJobId;
        }




        public async virtual Task<List<string>> GetNewSurvey()
        {
            int profileId = await profileSvc.GetCurrentProfileIdAsync();

            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            var list = await db.Surveys
                .Where(x => x.ProfileId == profileId)
                .Where(x => x.SurveyProgressNext == Constants.StatusSurveyProgress.Invited.ToString())
                .Where(x => x.SurveyExpiryDateTimeUtc > toDate)
                .OrderBy(x => x.SurveyExpiryDateTimeUtc)
                .Select(x => x.Uid)
                .ToListAsync();

            return list;
        }

        public async virtual Task<List<string>> GetIncompleteSurvey()
        {
            int profileId = await profileSvc.GetCurrentProfileIdAsync();
            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            var list = await db.Surveys
                .Where(x => x.ProfileId == profileId)
                .Where(x => x.SurveyProgressNext != Constants.StatusSurveyProgress.Invited.ToString())
                .Where(x => x.SurveyProgressNext != Constants.StatusSurveyProgress.New.ToString())
                .Where(x => x.SurveyUserCompletedDateTimeUtc == null) //not completed
                .Where(x => x.SurveyExpiryDateTimeUtc > toDate)
                .OrderBy(x => x.SurveyExpiryDateTimeUtc)
                .Select(x => x.Uid)
                .ToListAsync();

            return list;
        }

        public async virtual Task<List<string>> GetExpiringSoon()
        {
            int profileId = await profileSvc.GetCurrentProfileIdAsync();

            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            var list = await db.Surveys
                .Where(x => x.ProfileId == profileId)
                .Where(x => x.SurveyProgressNext != Constants.StatusSurveyProgress.New.ToString())
                .Where(x => x.SurveyUserCompletedDateTimeUtc == null)
                .Where(x => x.SurveyExpiryDateTimeUtc <= toDate)
                .Where(x => x.SurveyExpiryDateTimeUtc > DateTime.UtcNow)
                .OrderBy(x => x.SurveyExpiryDateTime)
                .Select(x => x.Uid)
                .ToListAsync();

            return list;
        }
        public async virtual Task<List<int>> GetMyDayCompletedSurveys(int ProfileID)
        {
             var list = await db.Surveys
                .Where(x => x.ProfileId == ProfileID)
                .Where(x => x.SurveyProgressNext == Constants.StatusSurveyProgress.Completed.ToString())                
                .Select(x => x.Id)
                .ToListAsync();

            return list;
        }
        public virtual IList<Response> GetAll(int taskId, int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user).Id;

                    result = db.Responses
                        .Where(i => i.ProfileId == profile)
                        .Where(i => i.SurveyId == surveyId)
                        .Where(i => i.TaskId == taskId)
                        .ToList();
                }
                catch (Exception)
                {

                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SurveyResponses"] = result;
                }
            }

            return result;
        }

        //Used in EditResponse
        public virtual IList<Response> GetAll(int taskId, int surveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user).Id;

                    result = db.Responses
                        .Where(i => i.ProfileId == profile)
                        .Where(i => i.SurveyId == surveyId)
                        .Where(i => i.TaskId == taskId)
                        .Where(i => i.TaskStartDateTime == taskStartDate)
                        .ToList();
                }
                catch (Exception)
                {

                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SurveyResponses"] = result;
                }
            }

            return result;
        }

        //Used in EditResponse
        public async virtual Task<IList<Response>> GetAllAsync(int taskId, int surveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = await profileSvc.GetCurrentProfileIdAsync();

                    result = await db.Responses
                        .Where(i => i.ProfileId == profile)
                        .Where(i => i.SurveyId == surveyId)
                        .Where(i => i.TaskId == taskId)
                        .Where(i => i.TaskStartDateTime == taskStartDate)
                        .ToListAsync();
                }
                catch (Exception)
                {

                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SurveyResponses"] = result;
                }
            }

            return result;
        }

        //Used in results
        public virtual IList<Response> GetAll(int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user).Id;

                    result = db.Responses
                        .Where(i => i.ProfileId == profile)
                        .Where(i => i.SurveyId == surveyId)
                        .ToList();
                }
                catch (Exception)
                {

                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SurveyResponses"] = result;
                }
            }

            return result;
        }

        public async virtual Task<IList<Response>> GetAllAsync(int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = await profileSvc.GetCurrentProfileIdAsync();

                    result = await db.Responses
                        .Where(i => i.ProfileId == profile)
                        .Where(i => i.SurveyId == surveyId)
                        .ToListAsync();
                }
                catch (Exception)
                {

                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SurveyResponses"] = result;
                }
            }

            return result;
        }

        public virtual IList<Response> GetAll()
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                try
                {
                    var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user).Id;

                    result = db.Responses
                        .Where(i => i.ProfileId == profile)
                        //.Where(i => i.ProfileId == profile.Id)
                        //.Where(i => i.ProfileId == profile.Id)
                        //.Where(i => i.ProfileId == profile.Id)
                        .ToList();
                }
                catch (Exception)
                {
                    throw;
                }

                if (!IsWebApiRequest)
                {
                    HttpContext.Current.Session["SurveyResponses"] = result;
                }
            }

            return result;
        }

        public async Task<int> InsertAsync(Response response, ModelStateDictionary modelState)
        {
            if (ValidateModel(response, modelState))
            {
                if (response.ProfileId == null || response.ProfileId == 0)
                {
                    string user = HttpContext.Current.User.Identity.GetUserName();

                    try
                    {
                        var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user).Id;
                        response.ProfileId = profile;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                //Add the entity.
                db.Responses.Add(response);
                //Insert the entity in the database.
                return await db.SaveChangesAsync();
                //Get the TaskID generated by the database.
                //response.Id = entity.Id;

            }
            return 0;
        }


        public virtual void Insert(Response response, ModelStateDictionary modelState)
        {
            if (ValidateModel(response, modelState))
            {
                if (response.ProfileId == null || response.ProfileId == 0)
                {
                    string user = HttpContext.Current.User.Identity.GetUserName();

                    try
                    {
                        var profile = db.Profiles.SingleOrDefault(m => m.LoginEmail == user).Id;
                        response.ProfileId = profile;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                //Create a new Task entity and set its properties from the posted TaskViewModel.
                //var entity = new 
                //{
                //    Id = rosterVM.TaskID,
                //    Name = rosterVM.Title,
                //    Start = rosterVM.Start,
                //    End = rosterVM.End,
                //    Description = rosterVM.Description,
                //    RecurrenceRule = rosterVM.RecurrenceRule,
                //    RecurrenceException = rosterVM.RecurrenceException,
                //    RecurrenceID = rosterVM.RecurrenceID,
                //    IsAllDay = rosterVM.IsAllDay,
                //    ProfileId = rosterVM.OwnerID.Value 
                //};

                //Add the entity.
                db.Responses.Add(response);
                //Insert the entity in the database.
                db.SaveChanges();
                //Get the TaskID generated by the database.
                //response.Id = entity.Id;

            }
        }

        public virtual void Update(Response response, ModelStateDictionary modelState)
        {
            if (ValidateModel(response, modelState))
            {

                ////Create a new Task entity and set its properties from the posted TaskViewModel.
                //var entity = new RosterItem
                //{
                //    Id = rosterVM.TaskID,
                //    Name = rosterVM.Title,
                //    Start = rosterVM.Start,
                //    End = rosterVM.End,
                //    Description = rosterVM.Description,
                //    RecurrenceRule = rosterVM.RecurrenceRule,
                //    RecurrenceException = rosterVM.RecurrenceException,
                //    RecurrenceID = rosterVM.RecurrenceID,
                //    IsAllDay = rosterVM.IsAllDay,
                //    ProfileId = rosterVM.OwnerID.Value
                //};
                //Attach the entity.
                db.Responses.Attach(response);
                //Change its state to Modified so the Entity Framework can update the existing task instead of creating a new one.
                //sampleDB.Entry(entity).State = EntityState.Modified;
                //Or use ObjectStateManager if using a previous version of Entity Framework.

                var manager = ((IObjectContextAdapter) db).ObjectContext.ObjectStateManager;
                manager.ChangeObjectState(response, EntityState.Modified);
                //Update the entity in the database
                db.SaveChanges();

            }
        }

        public async Task<int> UpdateAsync(Response response, ModelStateDictionary modelState)
        {
            if (ValidateModel(response, modelState))
            {

                ////Create a new Task entity and set its properties from the posted TaskViewModel.
                //var entity = new RosterItem
                //{
                //    Id = rosterVM.TaskID,
                //    Name = rosterVM.Title,
                //    Start = rosterVM.Start,
                //    End = rosterVM.End,
                //    Description = rosterVM.Description,
                //    RecurrenceRule = rosterVM.RecurrenceRule,
                //    RecurrenceException = rosterVM.RecurrenceException,
                //    RecurrenceID = rosterVM.RecurrenceID,
                //    IsAllDay = rosterVM.IsAllDay,
                //    ProfileId = rosterVM.OwnerID.Value
                //};
                //Attach the entity.
                db.Responses.Attach(response);
                //Change its state to Modified so the Entity Framework can update the existing task instead of creating a new one.
                //sampleDB.Entry(entity).State = EntityState.Modified;
                //Or use ObjectStateManager if using a previous version of Entity Framework.

                var manager = ((IObjectContextAdapter) db).ObjectContext.ObjectStateManager;
                manager.ChangeObjectState(response, EntityState.Modified);
                //Update the entity in the database
                await db.SaveChangesAsync();

            }
            return 0;
        }


        public virtual void Delete(Response response, ModelStateDictionary modelState)
        {

            ////Create a new Task entity and set its properties from the posted TaskViewModel.
            //var entity = new RosterItem
            //{
            //    Id = rosterVM.TaskID,
            //    Name = rosterVM.Title,
            //    Start = rosterVM.Start,
            //    End = rosterVM.End,
            //    Description = rosterVM.Description,
            //    RecurrenceRule = rosterVM.RecurrenceRule,
            //    RecurrenceException = rosterVM.RecurrenceException,
            //    RecurrenceID = rosterVM.RecurrenceID,
            //    IsAllDay = rosterVM.IsAllDay,
            //    ProfileId = rosterVM.OwnerID.Value
            //};
            //Attach the entity.
            db.Responses.Attach(response);
            //Delete the entity.
            //sampleDB.Tasks.Remove(entity);
            //Or use DeleteObject if using a previous versoin of Entity Framework.
            db.Responses.Remove(response);
            //Delete the entity in the database.
            db.SaveChanges();

        }

        private bool ValidateModel(Response response, ModelStateDictionary modelState)
        {
            //if (rosterVM.Start > rosterVM.End)
            //{
            //    modelState.AddModelError("errors", "End date must be greater or equal to Start date.");
            //    return false;
            //}

            return true;
        }

        public Response One(Func<Response, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public async Task<int> ResetSurvey(int surveyId, Guid uid, int? sendSurveyEmailJobId, int? sendSurveySmsJobId)
        {
            var s = new Survey() { Id = surveyId };
            s.Uid = uid.ToString();

            //if (sendSurveyEmailJobId.HasValue)
            //    s.SendSurveyEmailJobId = sendSurveyEmailJobId.Value;

            //if (sendSurveySmsJobId.HasValue)
            //    s.SendSurveySmsJobId = sendSurveySmsJobId.Value;

            s.SurveyProgressNext = Constants.StatusSurveyProgress.Invited.ToString();

            db.Surveys.Attach(s);

            db.Entry(s).Property(x => x.SurveyProgressNext).IsModified = true;

            //db.Entry(s).Property(x => x.SendSurveyEmailJobId).IsModified = true;

            //db.Entry(s).Property(x => x.SendSurveySmsJobId).IsModified = true;


            db.Entry(s).Property(x => x.Uid).IsModified = true;
            return await db.SaveChangesAsync();
        }


        [DisplayName("CreateSurveyJob;{1}")]
        public static async Task<int?> CreateSurveyScheduled(NewSurvey e, string msg)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            Survey survey = CreateSurveyFromNewSurvey(e);
            db.Surveys.Add(survey); //SaveSurvey
            int createSurveyJobId = await db.SaveChangesAsync();
            //await db.SaveChangesAsync();
            e.SurveyId = survey.Id;
            return createSurveyJobId; //as it is needed by the 
        }

     

        public static Survey CreateSurveyFromNewSurvey(NewSurvey e)
        {
            int offSetUtc = e.OffsetUtc;
            Survey survey = new Survey();
            survey.ProfileId = e.ProfileId;
            survey.RosterItemId = e.RosterItemId;
            survey.CreatedDateTimeUtc = DateTime.UtcNow;
            survey.Status = StatusSurvey.Active.ToString();
            survey.SurveyProgressNext = Constants.StatusSurveyProgress.New.ToString();
            //survey.SurveyProgressNext = Constants.StatusSurveyProgress.Invited.ToString();
            TimeSpan span = e.ShiftEnd.Subtract(e.ShiftStart);
            int shiftDuration = (int) Math.Round(span.TotalMinutes);


            //Randomly Select a Window of 4 hours
            AssignSurveyWindowWithinShift(e, offSetUtc, survey, shiftDuration);
            if (survey.Uid == string.Empty || survey.Uid == null)
            {
                Guid uid = Guid.NewGuid();
                //if (uid == null || uid.ToString() == "")
                //{
                //    Guid newuid = Guid.NewGuid();
                //    uid = newuid;
                //}
                survey.Uid = uid.ToString();               
            }
            
            survey.SurveyExpiryDateTime = survey.SurveyWindowEndDateTime.Value.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);
            survey.SurveyExpiryDateTimeUtc = survey.SurveyExpiryDateTime.Value.AddHours(offSetUtc);
            return survey;
        }


        #region OLD IMPLE
        //[DisplayName("CreateSurveyByScheduler;{1}")]
        //public static async Task<int?> CreateSurveyByScheduler(NewSurvey e, Survey survey, string msg)
        //{
        //    //Create a New Survey
        //    //Schedule StartSurveyEmail
        //    //Schedule CompleteSurveyEmail
        //    //Schedule ExpiringSoonSurveyNotStarted


        //    ApplicationDbContext db = new ApplicationDbContext();
        //    int offSetUtc = e.OffsetUtc;
        //    //Survey survey = new Survey();
        //    survey.ProfileId = e.ProfileId;
        //    survey.RosterItemId = e.RosterItemId;
        //    survey.CreatedDateTimeUtc = DateTime.UtcNow;
        //    survey.Status = StatusSurvey.Active.ToString();
        //    survey.SurveyProgressNext = Constants.StatusSurveyProgress.New.ToString();
        //    Guid uid = Guid.NewGuid();
        //    //added 2017-01-11
        //    //var roster = db.RosterItems.Where(x => x.Id == rosterItemId).SingleOrDefault();
        //    TimeSpan span = e.ShiftEnd.Subtract(e.ShiftStart);
        //    int shiftDuration = (int) Math.Round(span.TotalMinutes);
        //    AssignSurveyWindowWithinShift(e, offSetUtc, survey, shiftDuration);

        //    survey.Uid = uid.ToString();

        //    survey.SurveyExpiryDateTime = survey.SurveyWindowEndDateTime.Value.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);
        //    survey.SurveyExpiryDateTimeUtc = survey.SurveyExpiryDateTime.Value.AddHours(offSetUtc);

        //    survey.SurveyProgressNext = Constants.StatusSurveyProgress.Invited.ToString();


        //    db.Surveys.Add(survey);

        //    int createSurveyJobId = await db.SaveChangesAsync();



        //    #region set SendSurvey job
        //    //-------------------------
        //    var si = new SurveyInvitationEmailDto();
        //    si.ToEmail = e.EmailAddress;
        //    si.Id = e.ProfileId;
        //    si.SurveyId = survey.Id;
        //    si.RecipientName = e.RecipientName;
        //    si.Link = e.BaseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
        //    si.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
        //    si.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
        //    si.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
        //    si.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");


        //    //Commented out LOGIC for scheduler
        //    //TimeSpan diff = survey.SurveyWindowEndDateTime.Value - DateTime.UtcNow;  //create survey at the start of shift
        //    //double totalMin = diff.TotalMinutes;

        //    //si.ScheduledDateTime = DateTime.UtcNow.AddMinutes(totalMin).ToString("dd-MMM-yyyy hh:mm: tt");

        //    TimeSpan diff = survey.SurveyWindowEndDateTimeUtc.Value - DateTime.Now;  //create survey at the start of shift
        //    TimeSpan server_locaiton_diff = DateTime.Now - DateTime.UtcNow;
        //    double totalMin = diff.TotalMinutes + server_locaiton_diff.TotalMinutes;
        //    si.ScheduledDateTime = DateTime.Now.AddMinutes(totalMin).ToString("dd-MMM-yyyy hh:mm: tt");
        //    ////Normal Email
        //    ////await ResetSurvey(newSurveyId, uid, null, null);

        //    string emailMsg = si.ScheduledDateTime + " (ProfId: " + si.Id + ", RosterId: " + survey.RosterItemId + ", SurveyId: " + survey.Id + ", Survey Window End: " + survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm: tt") + ")";
        //    var sendSurveyJobIdResult = BackgroundJob.Schedule(() => PostalEmail.StartRecurrentSurvey(si, emailMsg),
        //        TimeSpan.FromMinutes(totalMin));


        //    Survey newSurvey = await db.Surveys.Where(x => x.Id == survey.Id).SingleAsync();
        //    int sendSurveyJobId;
        //    bool isJobSent = int.TryParse(sendSurveyJobIdResult, out sendSurveyJobId);
        //    if (isJobSent)
        //    {
        //        //newSurvey.SendSurveyEmailJobId = sendSurveyJobId;
        //        //await db.SaveChangesAsync();
        //    }


        //    #endregion




        //    #region set CompleteSurvey reminder job
        //    CompleteSurveyReminderEmailDto cs = new CompleteSurveyReminderEmailDto();
        //    cs.ToEmail = e.EmailAddress;
        //    cs.Id = e.ProfileId;
        //    cs.RecipientName = e.RecipientName;
        //    cs.Link = e.BaseUrl + "Survey3/Index?id=" + survey.Uid.ToString();
        //    cs.SurveyWindowStartDate = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
        //    cs.SurveyWindowEndDate = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
        //    cs.SurveyWindowStartTime = survey.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
        //    cs.SurveyWindowEndTime = survey.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");


        //    //Commented
        //    //DateTime shiftEnd = db.RosterItems
        //    //    .Where(x => x.Id == e.RosterItemId)
        //    //    .SingleOrDefault().EndUtc;

        //    //DateTime d = shiftEnd.AddHours(Constants.REMINDER_AFTER_SHIFT_END_HRS);
        //    //diff = d - DateTime.UtcNow;  //create survey at the start of shift
        //    //totalMin = diff.TotalMinutes;
        //    //cs.ScheduledDateTime = d.ToString("dd-MMM-yyyy hh:mm: tt");

        //    //DateTime shiftEndUtc = db.RosterItems
        //    //    .Where(x => x.Id == e.RosterItemId)
        //    //    .SingleOrDefault().EndUtc;

        //    DateTime shiftEndUtc = db.ProfileRosters
        //    .Where(x => x.Id == e.RosterItemId)
        //    .SingleOrDefault().EndUtc;

        //    DateTime d = shiftEndUtc.AddHours(Constants.REMINDER_AFTER_SHIFT_END_HRS);
        //    diff = d - DateTime.Now;  //create survey at the start of shift
        //    totalMin = diff.TotalMinutes + server_locaiton_diff.TotalMinutes;
        //    //cs.ScheduledDateTime = d.ToString("dd-MMM-yyyy hh:mm: tt");
        //    cs.ScheduledDateTime = DateTime.Now.AddMinutes(totalMin).ToString("dd-MMM-yyyy hh:mm: tt");



        //    ////Normal Email
        //    ////await ResetSurvey(newSurveyId, uid, null, null);

        //    emailMsg = cs.ScheduledDateTime + " (ProfId: " + cs.Id + ", RosterId: " + survey.RosterItemId + ", SurveyId: " + survey.Id + ", Survey Window End: " + survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm: tt") + ")";

        //    var completeSurveyReminderEmailJobIdResult = BackgroundJob.Schedule(() => PostalEmail.CompleteRecurrentSurvey(cs, emailMsg),
        //        TimeSpan.FromMinutes(totalMin));


        //    //Survey completeSurvey = await db.Surveys.Where(x => x.Id == survey.Id).SingleAsync();

        //    int completeSurveyReminderEmailJobId;
        //    bool isJobComplete = int.TryParse(completeSurveyReminderEmailJobIdResult, out completeSurveyReminderEmailJobId);
        //    if (isJobComplete)
        //    {
        //        newSurvey.CompleteSurveyReminderEmailJobId = completeSurveyReminderEmailJobId;
        //    }
        //    #endregion








        //    #region set ExpiringSoonNotStartedSurvey reminder job
        //    ExpiringSoonNotStartedSurveyReminderEmailDto esNS = new ExpiringSoonNotStartedSurveyReminderEmailDto();
        //    esNS.ToEmail = e.EmailAddress;
        //    esNS.Id = e.ProfileId;
        //    esNS.RecipientName = e.RecipientName;
        //    esNS.Link = e.BaseUrl + "Survey3/Index?id=" + survey.Uid.ToString();

        //    esNS.SurveyWindowStartDateTime = survey.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy hh:mm tt");
        //    esNS.SurveyWindowEndDateTime = survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm tt");

        //    //Commented out
        //    ////5 hrs before expiring time
        //    //DateTime d2 = survey.SurveyExpiryDateTime.Value.AddHours(-1 * Constants.REMINDER_BEFORE_EXPIRY_HRS);
        //    //diff = d2 - DateTime.UtcNow;  //create survey at the start of shift
        //    //totalMin = diff.TotalMinutes;
        //    //esNS.ScheduledDateTime = d2.ToString("dd-MMM-yyyy hh:mm: tt");

        //    DateTime expiryUtc = survey.SurveyExpiryDateTimeUtc.Value.AddHours(-1 * Constants.REMINDER_BEFORE_EXPIRY_HRS);
        //    diff = expiryUtc - DateTime.Now;  //create survey at the start of shift
        //    totalMin = diff.TotalMinutes + server_locaiton_diff.TotalMinutes;
        //    //esNS.ScheduledDateTime = d2.ToString("dd-MMM-yyyy hh:mm: tt");
        //    esNS.ScheduledDateTime = DateTime.Now.AddMinutes(totalMin).ToString("dd-MMM-yyyy hh:mm: tt");




        //    ////Normal Email
        //    ////await ResetSurvey(newSurveyId, uid, null, null);

        //    emailMsg = esNS.ScheduledDateTime + " (ProfId: " + esNS.Id + ", RosterId: " + survey.RosterItemId + ", SurveyId: " + survey.Id + ", Survey Window End: " + survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm: tt") + ")";

        //    var esNSEmailJobIdResult = BackgroundJob.Schedule(() => PostalEmail.ExpiringSoonNotStartedSurvey(esNS, emailMsg),
        //        TimeSpan.FromMinutes(totalMin));


        //    //Survey completeSurvey = await db.Surveys.Where(x => x.Id == survey.Id).SingleAsync();

        //    int esNSEmailJobId;
        //    bool isJobExpiringNS = int.TryParse(esNSEmailJobIdResult, out esNSEmailJobId);
        //    if (isJobExpiringNS)
        //    {
        //        newSurvey.ExpiringSoonNotStartedSurveyReminderEmailJobId = esNSEmailJobId;
        //    }
        //    #endregion


        //    await db.SaveChangesAsync();
        //    return createSurveyJobId; //as it is needed by the 
        //}
        #endregion


        private static void AssignSurveyWindowWithinShift(NewSurvey e, int offSetUtc, Survey survey, int shiftDuration)
        {

            //if shift <= 4,hrs surveywindow == shiftwindow
            if (shiftDuration <= (Constants.SURVEY_WINDOW_IN_HRS * 60))
            {
                survey.SysGenRandomNumber = null; //generate 0 to 4
                survey.SurveyWindowStartDateTime = e.ShiftStart;
                survey.SurveyWindowEndDateTime = e.ShiftEnd;
                survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);
                survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");
            }
            else if (shiftDuration > (Constants.SURVEY_WINDOW_IN_HRS * 60)
                && shiftDuration <= (Constants.SURVEY_WINDOW_IN_HRS * 2 * 60))
            {
                int shiftDurationInHrs = (int) shiftDuration / 60;
                Random rnd = new Random();
                int rnNum = rnd.Next(shiftDurationInHrs - Constants.SURVEY_WINDOW_IN_HRS);
                survey.SysGenRandomNumber = rnNum; //generate 0 to 4
                survey.SurveyWindowStartDateTime = e.ShiftStart.AddHours(rnNum);
                survey.SurveyWindowEndDateTime = survey.SurveyWindowStartDateTime.Value.AddHours(Constants.SURVEY_WINDOW_IN_HRS);
                survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);
                survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");
            }
            else
            {
                Random rnd = new Random();
                int rnNum = rnd.Next(Constants.SURVEY_WINDOW_IN_HRS);
                survey.SysGenRandomNumber = rnNum; //generate 0 to 4
                survey.SurveyWindowStartDateTime = e.ShiftStart.AddHours(rnNum);
                survey.SurveyWindowEndDateTime = survey.SurveyWindowStartDateTime.Value.AddHours(Constants.SURVEY_WINDOW_IN_HRS);
                survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);
                survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");

            }
        }

        public bool CheckRosterIdHasScheduledEmailsOrNot(int RosterId)
        {
            bool surveyExists = false;
            Survey s = db.Surveys.Where(x => x.RosterItemId == RosterId).FirstOrDefault();

            if (s != null)
            {
                surveyExists = true;
            }
            return surveyExists;
        }
        public bool CheckScheduledHangFireJobs(int RosterItemId)
        {
            bool hasScheduledJobs = false;
            System.Text.StringBuilder likeString = new System.Text.StringBuilder();
            likeString.Append("'%"); // %'
            likeString.Append("\"RosterItemId");
            likeString.Append("\\");
            likeString.Append("\"");
            likeString.Append(":");
            likeString.Append(RosterItemId);
            likeString.Append("%'");

            string likeToPass = likeString.ToString();

            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(cnnString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "CheckScheduledHangFireJobs";
            //add any parameters the stored procedure might require
            cmd.Parameters.AddWithValue("@RosterItemId", likeToPass);

            System.Data.SqlClient.SqlParameter sqlParam = new System.Data.SqlClient.SqlParameter("@Result", DbType.Boolean);
            sqlParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(sqlParam);
            cnn.Open();

            //object o = cmd.ExecuteScalar();
            cmd.ExecuteNonQuery();
            cnn.Close();
            object r = cmd.Parameters["@Result"].Value;
            hasScheduledJobs = Convert.ToInt32(r.ToString()) == 1 ? true:false;

            return hasScheduledJobs;
        }
    }
}