using Microsoft.AspNet.Identity;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.BLL
{
    public class CalendarService
    {

        /*
         * Mix UnitOfWork and Db access
        */


        private static bool UpdateDatabase = true;
        private ApplicationDbContext db;
        private HangfireScheduler schedulerService;

        private JobService _jobService;


        private ProfileService profileService;
        readonly UnitOfWork _unitOfWork = new UnitOfWork();


        public CalendarService(ApplicationDbContext context)
        {
            db = context;
            this.schedulerService = new HangfireScheduler();
            this.profileService = new ProfileService();
            this._jobService = new JobService();
        }

        public CalendarService()
            : this(new ApplicationDbContext())
        {
        }

        public ProfileDto GetCurrentLoggedInProfile()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();

            if (user != null)
            {
                var profile = GetProfileByLoginEmail(user);
                return profile;
            }

            return null;
        }

        public ProfileDto GetProfileByLoginEmail(string loginEmail)
        {
            loginEmail = StringCipher.EncryptRfc2898(loginEmail);

            var profile = _unitOfWork.ProfileRespository
                .GetUsingNoTracking(m => m.LoginEmail == loginEmail)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }

            return null;
        }


        public virtual Dictionary<string, CalendarVM> GetAll(int? profileId)
        {

            Dictionary<string, CalendarVM> result = new Dictionary<string, CalendarVM>();

            try
            {
                int intProf = 0;
                if (profileId.HasValue)
                {
                    string user = HttpContext.Current.User.Identity.GetUserName();
                    user = StringCipher.EncryptRfc2898(user);

                    intProf = db.Profiles
                                .Where(x => x.LoginEmail == user)
                                .Select(m => m.Id)
                                .SingleOrDefault();
                }
                else
                {
                    intProf = profileId.Value;
                }


                //var fullList = db.RosterItems
                //            .Where(x => x.ProfileId == intProf)
                //            .Select(x => new { x.Start, x.End, x.Name, x.Id })
                //            .OrderBy(x => x.Start)
                //            .ToList();

                var fullList = db.ProfileRosters
                           .Where(x => x.ProfileId == intProf)
                           .Select(x => new { x.Start, x.End, x.Name, x.Id })
                           .OrderBy(x => x.Start)
                           .ToList();

                var listByDate = fullList.GroupBy(x => x.Start.Date).Distinct();

                var listOfSurveyforUser = db.Surveys
                                        .Where(u => u.ProfileId == intProf)
                                        .Select(u => new { u.Id, u.RosterItemId })
                                        .ToList();


                foreach (var c in listByDate)
                {
                    string key = c.Key.ToString("yyyy-MM-dd");
                    CalendarVM v = new CalendarVM();
                    v.number = " ";
                    v.badgeClass = "badge-warning";

                    v.dayEvents = new List<DayEvents>();

                    foreach (var e in c)
                    {
                        int hasSurvey = listOfSurveyforUser
                            .Where(i => i.RosterItemId == e.Id)
                            .Count();

                        DayEvents d = new DayEvents()
                        {
                            Name = e.Name,
                            RosterItemId = e.Id.ToString(),
                            StartDateTime = e.Start.ToString("yyyy-MM-dd HH:mm"),
                            EndDateTime = e.End.ToString("yyyy-MM-dd HH:mm"),
                            StartTime = e.Start.ToString("HH:mm"),
                            EndTime = e.End.ToString("HH:mm"),

                            //HasSurvey = (hasSurvey > 0) ? "disabled" : ""
                            HasSurvey = (hasSurvey > 0 && e.Start < DateTime.Now) ? "disabled" : ""


                        };

                        v.dayEvents.Add(d);
                    }

                    result.Add(key, v);
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return result;
        }


        public virtual Dictionary<string, CalendarVM> GetAll()
        {

            Dictionary<string, CalendarVM> result = new Dictionary<string, CalendarVM>();

            try
            {
                string user = HttpContext.Current.User.Identity.GetUserName();

                int profileId = db.Profiles
                    .Where(x => x.LoginEmail == user)
                    .Select(m => m.Id)
                    .SingleOrDefault();

                //var fullList = db.RosterItems
                //            .Where(x => x.ProfileId == profileId)
                //            .Select(x => new { x.Start, x.End, x.Name, x.Id })
                //            .OrderBy(x => x.Start)
                //            .ToList();

                var fullList = db.ProfileRosters
                          .Where(x => x.ProfileId == profileId)
                          .Select(x => new { x.Start, x.End, x.Name, x.Id })
                          .OrderBy(x => x.Start)
                          .ToList();

                var listByDate = fullList.GroupBy(x => x.Start.Date).Distinct();

                var listOfSurveyforUser = db.Surveys
                                        .Where(u => u.ProfileId == profileId)
                                        .Select(u => new { u.Id, u.RosterItemId })
                                        .ToList();


                foreach (var c in listByDate)
                {
                    string key = c.Key.ToString("yyyy-MM-dd");
                    CalendarVM v = new CalendarVM();
                    v.number = " ";
                    v.badgeClass = "badge-warning";

                    v.dayEvents = new List<DayEvents>();

                    foreach (var e in c)
                    {
                        int hasSurvey = listOfSurveyforUser
                            .Where(i => i.RosterItemId == e.Id)
                            .Count();

                        DayEvents d = new DayEvents()
                        {
                            Name = e.Name,
                            RosterItemId = e.Id.ToString(),
                            StartDateTime = e.Start.ToString("yyyy-MM-dd HH:mm"),
                            EndDateTime = e.End.ToString("yyyy-MM-dd HH:mm"),
                            StartTime = e.Start.ToString("HH:mm"),
                            EndTime = e.End.ToString("HH:mm"),
                            //HasSurvey = (hasSurvey > 0) ? "disabled" : ""
                            HasSurvey = (hasSurvey > 0 && e.Start < DateTime.Now) ? "disabled" : ""

                        };

                        v.dayEvents.Add(d);
                    }

                    result.Add(key, v);
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return result;
        }


        public async virtual Task<bool> IsNeedRosterUpdate()
        {
            try
            {

                int profileId = await profileService.GetCurrentProfileIdAsync();

                //TODO: MOD
                DateTime fromDate = DateTime.UtcNow.Date;
                DateTime toDate = fromDate.AddDays(Constants.ROSTER_EMPTY_PERIOD_TILL_DAYS + 1);

                var list = db.ProfileRosters
                        .Where(x => x.ProfileId == profileId)
                        .Where(x => x.Start >= DateTime.UtcNow)
                        .Where(x => x.Start <= toDate)
                        .Select(x => x.Id)
                        .Count();

                if (list > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }


        public virtual CalendarEditVM Get(string rosterItemId)
        {
            try
            {
                int id = int.Parse(rosterItemId);

                //var rosterItem = db.RosterItems
                //    .Where(x => x.Id == id)
                //    .Select(x => new { x.Id, x.Start, x.End })
                //    .FirstOrDefault();

                var rosterItem = db.ProfileRosters
                    .Where(x => x.Id == id)
                    .Select(x => new { x.Id, x.Start, x.End })
                    .FirstOrDefault();

                CalendarEditVM v = new CalendarEditVM()
                {
                    RosterItemId = rosterItem.Id.ToString(),
                    StartDateTime = rosterItem.Start.ToString("yyyy-MM-dd HH:mm"),
                    EndDateTime = rosterItem.End.ToString("yyyy-MM-dd HH:mm")
                };

                return v;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private int GetNumOfShiftsInLast24Hrs(int profileId, DateTime shiftStartDateTime)
        {
            //Check if there is a shift in previous day
            DateTime lower = shiftStartDateTime.Date.AddDays(-1);
            DateTime upper = shiftStartDateTime.Date;

            //var rcs = db.RosterItems
            //    .Where(x => x.ProfileId == profileId)
            //    .Where(x => x.Start >= lower)
            //    .Where(x => x.Start <= upper)
            //    .Where(x => x.ShiftReminderEmailJobId != null)
            //    .Where(x => x.CreateSurveyJobId != null);

            var rcs = db.ProfileRosters
              .Where(x => x.ProfileId == profileId)
              .Where(x => x.Start >= lower)
              .Where(x => x.Start <= upper);
            //.Where(x => x.ShiftReminderEmailJobId != null)
            //.Where(x => x.CreateSurveyJobId != null);

            int count = rcs
                .Count();

            return count;
        }

        //CreateRoster
        public virtual async Task<int> SaveNewAsyncParam(string startDateTime, string endDateTime, string baseUrl, string profileId, 
            string profileEmail, string profileOffset, string profileName, string clientInitials)
        {
            try
            {
                string Url = baseUrl + "Calendar/List";
                int? shiftReminderEmailJobId = null;
                int? createSurveyJobId = null;

                CurrentProfile currProfile = GetProfile(profileId, profileEmail, profileOffset, profileName, clientInitials);

                DateTime st = Convert.ToDateTime(startDateTime);
                DateTime et = Convert.ToDateTime(endDateTime);
                DateTime utcSt = st.AddHours(currProfile.OffsetFromUTC);
                DateTime utcEt = et.AddHours(currProfile.OffsetFromUTC);


                ProfileRosterDto rosterDto = new ProfileRosterDto();
                rosterDto.Name = "On Shift";
                rosterDto.Start = st;
                rosterDto.End = et;
                rosterDto.ProfileId = currProfile.ProfileId;
                rosterDto.Description = st.ToString("dd MMM yyyy hh:mm tt") + " -  " + et.ToString("dd MMM yyyy hh:mm tt");
                rosterDto.StartUtc = utcSt;
                rosterDto.EndUtc = utcEt;
                var e = ObjectMapper.GetProfileRosterEntity(rosterDto);
                db.ProfileRosters.Add(e);
                int saveResult = await db.SaveChangesAsync(); // SAVE Roster.
                rosterDto.Id = e.Id;


                //Schedule Shift Reminder Email
                shiftReminderEmailJobId = await _jobService.CreateJobAsync(JobName.ShiftReminderEmail.ToString(),
                   JobType.Email.ToString(), int.Parse(profileId),
                   JobMethod.Auto.ToString(),
                   string.Empty, Url, currProfile, rosterDto
               );

                //Schedule Create Survey Job
                createSurveyJobId = await _jobService.CreateJobAsync(JobName.CreateSurveyJob.ToString(),
                  JobType.Method.ToString(), int.Parse(profileId),
                  JobMethod.Auto.ToString(), baseUrl, string.Empty, currProfile, rosterDto
              );

              return 0; //changed due to null when additional fields were added.
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private CurrentProfile GetProfile(string profileId, string profileEmail, string profileOffset, string profileName, string clientInitials)
        {
            bool retFromDB = false;
            if (string.IsNullOrEmpty(profileId) || string.IsNullOrEmpty(profileEmail) 
                || string.IsNullOrEmpty(profileOffset) || string.IsNullOrEmpty(profileName)
                || string.IsNullOrEmpty(clientInitials))
            {
                retFromDB = true;
            }

            CurrentProfile currProfile = new CurrentProfile();

            if (retFromDB)
            {
                currProfile = profileService.GetCurrentProfile();
            }
            else
            {
                currProfile.ProfileId = int.Parse(profileId);
                currProfile.ProfileName = profileName;
                currProfile.ProfileEmailAddress = profileEmail;
                currProfile.OffsetFromUTC = int.Parse(profileOffset);
                currProfile.ClientInitials = clientInitials;
            }

            return currProfile;
        }

        private static void GetScheduleTime(DateTime st, NewSurvey s, out double serverLocationFromUtc, out double scheduleTimespan)
        {
            serverLocationFromUtc = ((DateTime.Now - DateTime.UtcNow).TotalHours) * 60;
            DateTime adjShiftStartUtc = st.AddHours(-1 * s.OffsetUtc);
            TimeSpan timeToShiftFromNowUtc = adjShiftStartUtc - DateTime.UtcNow;
            double shiftHoursFromNow = timeToShiftFromNowUtc.TotalHours;
            DateTime userShiftDStartDateTime = DateTime.UtcNow.AddHours(shiftHoursFromNow); //Servertime
            TimeSpan totalMin = userShiftDStartDateTime - DateTime.UtcNow;
            scheduleTimespan = totalMin.TotalMinutes;
            //with repsect to server location
        }

        public virtual async Task<int> SaveEditAsyncParam(string rosterItemId, string startDateTime, string endDateTime, string baseUrl, 
            string profileId, string profileEmail, string profileOffset, string profileName, string clientInitials)
        {
            try
            {
                string Url = baseUrl + "Calendar/List";
                int? shiftReminderEmailJobId = null;
                int? createSurveyJobId = null;


                CurrentProfile currProfile = GetProfile(profileId, profileEmail, profileOffset, profileName, clientInitials);


                int id = int.Parse(rosterItemId);

                var shift = db.ProfileRosters
                    .Where(x => x.Id == id)
                    .SingleOrDefault();

                // Delete current Jobs related to this shift in Hangfire
                //await RemoveShiftReminderJobs(id);
                //await RemoveCreateSurveyJobs(id); 
                //await RemoveStartSurveyReminders(id);
                //await RemoveUpdateSurveyJobs(id);
                //await RemoveCompleteSurveyReminders(id);
                //await RemoveExpiringSoonNotStartedReminder(id);
                //await RemoveExpiringSoonNotCompletedReminders(id);

                await _jobService.RemoveShiftReminderJobs(id);
                await _jobService.RemoveCreateSurveyJobs(id);
                await _jobService.RemoveStartSurveyReminders(id);
                await _jobService.RemoveUpdateSurveyJob(id);
                await _jobService.RemoveCompleteSurveyReminders(id);
                await _jobService.RemoveExpiringSoonNotStartedReminder(id);
                await _jobService.RemoveExpiringSoonNotCompletedReminders(id);


                var survey = db.Surveys
                      .Where(x => x.RosterItemId == shift.Id)
                      .ToList();
                db.Surveys.RemoveRange(survey); //Remove all surveys
                await db.SaveChangesAsync();

                #region derive ProfileRosterDto
                DateTime st = Convert.ToDateTime(startDateTime);
                DateTime et = Convert.ToDateTime(endDateTime);
                DateTime utcSt = st.AddHours(currProfile.OffsetFromUTC);
                DateTime utcEt = et.AddHours(currProfile.OffsetFromUTC);

                ProfileRosterDto rosterDto = new ProfileRosterDto();
                rosterDto.Name = "On Shift";
                rosterDto.Start = st;
                rosterDto.End = et;
                rosterDto.ProfileId = currProfile.ProfileId;
                rosterDto.Description = st.ToString("dd MMM yyyy hh:mm tt") + " -  " + et.ToString("dd MMM yyyy hh:mm tt");
                rosterDto.StartUtc = utcSt;
                rosterDto.EndUtc = utcEt;
                
                shift.Start = st;
                shift.End = et;
                shift.Description = st.ToString("dd MMM yyyy hh:mm tt") + " -  " + et.ToString("dd MMM yyyy hh:mm tt");
                shift.StartUtc = utcSt;
                shift.EndUtc = utcEt;
                rosterDto.Id = id;

                int rosterId = await db.SaveChangesAsync(); //changed due to null when additional fields were added.
               



                #endregion

                //Schedule Shift Reminder Email
                shiftReminderEmailJobId = await _jobService.CreateJobAsync(JobName.ShiftReminderEmail.ToString(),
                   JobType.Email.ToString(), int.Parse(profileId),
                   JobMethod.Auto.ToString(),
                   string.Empty, Url, currProfile, rosterDto
                );

                //Schedule Create Survey Job
                createSurveyJobId = await _jobService.CreateJobAsync(JobName.CreateSurveyJob.ToString(),
                  JobType.Method.ToString(), int.Parse(profileId),
                  JobMethod.Auto.ToString(), baseUrl, string.Empty, currProfile, rosterDto
                );

                return 0; //changed due to null when additional fields were added.

            }
            catch (Exception ex)
            {
                throw;
            }
        }




        //public virtual async Task<string> Delete(string rosterItemId)
        //{
        //    /*
        //     Errors:
        //     ErrorDelete: General errors while operation
        //     ErrorSurveyCreated: A sruvey is created already
             
        //     */
        //    try
        //    {

        //        int id = int.Parse(rosterItemId);

        //        var survey = db.Surveys
        //              .Where(x => x.RosterItemId == id)
        //              .ToList();


        //        if (survey.Count > 0)
        //        {
        //            Survey s = survey[0];
        //            if(s.SurveyWindowStartDateTime < DateTime.Now)
        //                return "ErrorSurveyCreated";
        //        }


        //        //int? ShiftReminderEmailJobId = null;
        //        //int? ShiftReminderSmsJobId = null;
        //        //int? CreateSurveyJobId = null;

        //        //Efficient method
        //        //RosterItem r = new RosterItem { Id = id };
        //        //db.RosterItems.Attach(r);
        //        //db.RosterItems.Remove(r);
        //        //db.SaveChanges();

        //        var r = db.ProfileRosters
        //            .Where(x => x.Id == id)
        //            .SingleOrDefault();
        //        db.ProfileRosters.Remove(r);

        //        await RemoveShiftReminderJobs(id);
        //        await RemoveCreateSurveyJobs(id);
        //        await RemoveStartSurveyReminders(id);
        //        await RemoveUpdateSurveyJobs(id);
        //        await RemoveCompleteSurveyReminders(id);
        //        await RemoveExpiringSoonNotStartedReminder(id);
        //        await RemoveExpiringSoonNotCompletedReminders(id);
                
        //        db.Surveys.RemoveRange(survey); //Remove all surveys
        //        await db.SaveChangesAsync();
        //        return r.Start.ToString("yyyy-MM");
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw;
        //        return "ErrorDelete";
        //    }
        //}


        //Revised with Job Service
        public virtual async Task<string> Delete(string rosterItemId)
        {
            /*
             Errors:
             ErrorDelete: General errors while operation
             ErrorSurveyCreated: A sruvey is created already
             */
            try
            {

                //TODO: Need to optimize, using required fields
                int id = int.Parse(rosterItemId);

                var survey = db.Surveys
                      .Where(x => x.RosterItemId == id)
                      .ToList();




                if (survey.Count > 0)
                {
                    Survey s = survey[0];

                    //Allow only the future surveys be deleted
                    if (s.SurveyWindowStartDateTime < DateTime.Now)
                        return "ErrorSurveyCreated";
                }

                var r = db.ProfileRosters
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
                db.ProfileRosters.Remove(r);

                await _jobService.RemoveShiftReminderJobs(id);
                await _jobService.RemoveCreateSurveyJobs(id);
                await _jobService.RemoveStartSurveyReminders(id);
                await _jobService.RemoveUpdateSurveyJob(id);
                await _jobService.RemoveCompleteSurveyReminders(id);
                await _jobService.RemoveExpiringSoonNotStartedReminder(id);
                await _jobService.RemoveExpiringSoonNotCompletedReminders(id);

                //await RemoveShiftReminderJobs(id);
                //await RemoveCreateSurveyJobs(id);
                //await RemoveStartSurveyReminders(id);
                //await RemoveUpdateSurveyJobs(id);
                //await RemoveCompleteSurveyReminders(id);
                //await RemoveExpiringSoonNotStartedReminder(id);
                //await RemoveExpiringSoonNotCompletedReminders(id);

                db.Surveys.RemoveRange(survey); //Remove all surveys


                await db.SaveChangesAsync();
                return r.Start.ToString("yyyy-MM");
            }
            catch (Exception ex)
            {
                //throw;
                return "ErrorDelete";
            }
        }

        //private async Task RemoveExpiringSoonNotCompletedReminders(int id)
        //{
        //    var expiringSoonNotCompletedReminders = db.JobLogExpiringSoonSurveyNotCompletedReminderEmails
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in expiringSoonNotCompletedReminders)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.RemoveRange(db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.Where(x => x.ProfileRosterId == id));
        //}

        //private async Task RemoveExpiringSoonNotStartedReminder(int id)
        //{
        //    var expiringSoonNotStartedReminders = db.JobLogExpiringSoonSurveyNotStartedReminderEmails
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in expiringSoonNotStartedReminders)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogExpiringSoonSurveyNotStartedReminderEmails.RemoveRange(db.JobLogExpiringSoonSurveyNotStartedReminderEmails.Where(x => x.ProfileRosterId == id));
        //}

        //private async Task RemoveCompleteSurveyReminders(int id)
        //{
        //    var completeSurveyReminders = db.JobLogCompleteSurveyReminderEmails
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in completeSurveyReminders)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogCompleteSurveyReminderEmails.RemoveRange(db.JobLogCompleteSurveyReminderEmails.Where(x => x.ProfileRosterId == id));
        //}

        //private async Task RemoveStartSurveyReminders(int id)
        //{
        //    var startSurveyReminders = db.JobLogStartSurveyReminderEmails
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in startSurveyReminders)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogStartSurveyReminderEmails.RemoveRange(db.JobLogStartSurveyReminderEmails.Where(x => x.ProfileRosterId == id));
        //}

        //private async Task RemoveCreateSurveyJobs(int id)
        //{
        //    var createSurveyJobs = db.JobLogCreateSurveys
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in createSurveyJobs)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogCreateSurveys.RemoveRange(db.JobLogCreateSurveys.Where(x => x.ProfileRosterId == id));
        //}


        //private async Task RemoveUpdateSurveyJobs(int id)
        //{
        //    var updateSurveyJobs = db.JobLogUpdateSurveys
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in updateSurveyJobs)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogUpdateSurveys.RemoveRange(db.JobLogUpdateSurveys.Where(x => x.ProfileRosterId == id));
        //}


        //private async Task RemoveShiftReminderJobs(int id)
        //{
        //    var shiftReminderJobs = db.JobLogShiftReminderEmails
        //        .Where(x => x.ProfileRosterId == id);
        //    foreach (var x in shiftReminderJobs)
        //    {
        //        if (x.HangfireJobId.HasValue)
        //        {
        //            await schedulerService.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
        //        }
        //    }
        //    db.JobLogShiftReminderEmails.RemoveRange(db.JobLogShiftReminderEmails.Where(x => x.ProfileRosterId == id));
        //}


        public void Dispose()
        {
            schedulerService.Dispose();
            profileService.Dispose();
            _jobService.Dispose();
            db.Dispose();
        }



    }
}