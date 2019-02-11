using System;
using System.Linq;
using System.Data;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.Models;
using System.Threading.Tasks;
using Hangfire;
using System.Net;
using SANSurveyWebAPI.DTOs;

namespace SANSurveyWebAPI.BLL
{


    public class HangfireScheduler
    {
        private static bool UpdateDatabase = true;
        private ApplicationDbContext db;
        private PostalEmail emailSvc;

        public HangfireScheduler()
            : this(new ApplicationDbContext())
        {
        }
        public HangfireScheduler(ApplicationDbContext context)
        {
            db = context;
            this.emailSvc = new PostalEmail();
        }
        public void Dispose()
        {
            db.Dispose();
        }

        public async Task<bool> DeleteScheduledJob(string jobId)
        {
            try
            {
                return BackgroundJob.Delete(jobId);

            }
            catch (Exception ex)
            {

            }
            return false;
        }
        
        public int SendError(EmailDto e)
        {
            try
            {
                var result = BackgroundJob.Enqueue(() => PostalEmail.SendError(e));
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }



        public async Task<int> SendWebsiteContactUs(WebsiteContactUsEmailDto e)
        {
            try
            {
                var result = BackgroundJob.Enqueue(() => PostalEmail.WebsiteContactUs(e));
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        #region Registration and Baseline
        public async Task<int?> RegisterBaselineSurveyEmail(RegistrationInvitationEmailDto e, double timespan)
        {
            try
            {
                var result = BackgroundJob.Schedule(() => PostalEmail.RegisterAndBaseline(e),
                     TimeSpan.FromMinutes(timespan));
                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<int?> ExitSurveyEmail(ExitSurveyInvitationEmailDto e, double timespan)
        {
            try
            {
                var result = BackgroundJob.Schedule(() => PostalEmail.ExitSurvey(e),
                     TimeSpan.FromMinutes(timespan));
                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int?> RegistrationCompletedEmail(SignupCompletedEmailDto e, double timespan)
        {
            try
            {
                var result = BackgroundJob.Schedule(() => PostalEmail.SignupCompleted(e),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<int?> WAMRegistrationCompletedEmail(WarrenMahonySignupCompletedEmailDto e, double timespan)
        {
            try
            {

                var result = BackgroundJob.Schedule(() => PostalEmail.WAMSignupCompleted(e),
                       TimeSpan.FromMinutes(timespan));


                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion



        public async Task<int?> ShiftReminderEmail(ShiftStartReminderEmailDto e, double timeSpanInMin)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", ShiftStart: " + e.ShiftStartDate + " " + e.ShiftStartTime + "];Server[" + DateTime.Now + "," + timeSpanInMin +", Trigger: "+ DateTime.Now.AddMinutes(timeSpanInMin) + ", Actual: " + DateTime.Now.AddMinutes(timeSpanInMin + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.ShiftStartReminder(e, emailMsg),
                     TimeSpan.FromMinutes(timeSpanInMin));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int?> ShiftReminderWarrenEmail(ShiftReminderWarrenMahonyEmailDto e, double timeSpanInMin)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", ShiftStart: " + e.ShiftStartDate + " " + e.ShiftStartTime + "];Server[" + DateTime.Now + "," + timeSpanInMin + ", Trigger: " + DateTime.Now.AddMinutes(timeSpanInMin) + ", Actual: " + DateTime.Now.AddMinutes(timeSpanInMin + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.ShiftReminderWarrenMahony(e, emailMsg),
                     TimeSpan.FromMinutes(timeSpanInMin));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int?> UpdateSurveyStatus(NewSurvey e, double timeSpanInMin)
        {
            //Run Method to create a new survey
            try
            {
                string createSurveyMsg = "Record[ProfileId: " + e.ProfileId + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", ShiftStart: " + e.ShiftStart + ", SurveyStart: " + e.SurveyStartDateTime + "];Server[" + DateTime.Now + "," + timeSpanInMin + ", Trigger: " + DateTime.Now.AddMinutes(timeSpanInMin) + ", Actual: " + DateTime.Now.AddMinutes(timeSpanInMin + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";

                var createSurveyResult = BackgroundJob.Schedule(() => SurveyService.UpdateSurveyStatus(e, createSurveyMsg),
                         TimeSpan.FromMinutes(timeSpanInMin));

                int hangfireJobId;
                bool isJobCreate = int.TryParse(createSurveyResult, out hangfireJobId);
                if (isJobCreate)
                {
                    return hangfireJobId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int?> CreateSurveyJob(NewSurvey e, double timeSpanInMin)
        {
            try
            {
                string createSurveyMsg = "Record[ProfileId: " + e.ProfileId + ", RosterId: " + e.RosterItemId + ", ShiftStart: " + e.ShiftStart + ", SurveyStart: " + e.SurveyStartDateTime + "];Server[" + DateTime.Now + "," + timeSpanInMin + ", Trigger: "+ DateTime.Now.AddMinutes(timeSpanInMin)  + ", Actual: " + DateTime.Now.AddMinutes(timeSpanInMin + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";

                var createSurveyResult = BackgroundJob.Schedule(() => SurveyService.CreateSurveyScheduled(e, createSurveyMsg),
                         TimeSpan.FromMinutes(timeSpanInMin));

                int hangfireJobId;
                bool isJobCreate = int.TryParse(createSurveyResult, out hangfireJobId);
                if (isJobCreate)
                {
                    return hangfireJobId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<int?> StartRecurrentSurveyEmail(SurveyInvitationEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDate + " "+ e.SurveyWindowStartTime  +"];Server[" + DateTime.Now + "," + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.StartRecurrentSurvey(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));
                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int?> StartWarrenSurveyInvitationEmail(StartWarrenSurveyInvitationEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDate + " " + e.SurveyWindowStartTime + "];Server[" + DateTime.Now + "," + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.StartWarrenSurveyInvitation(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));
                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //public async Task<int?> UpdateSurveyStatus(Survey s, double timespan)
        //{
        //    try
        //    {
        //        //string emailMsg = string.Empty;
        //        //string emailMsg = e.ScheduledDateTime + " (ProfId: " + e.Id + ", RosterId: " + e. + ", SurveyId: " + survey.Id + ", Survey Window End: " + survey.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy hh:mm: tt") + ")";
        //        //string emailMsg = e.ScheduledDateTime;

        //        var result = BackgroundJob.Schedule(() => SurveyService.StartRecurrentSurvey(e, emailMsg),
        //             TimeSpan.FromMinutes(timespan));

        //        int jobId;
        //        bool isJobCreate = int.TryParse(result, out jobId);
        //        if (isJobCreate)
        //        {
        //            return jobId;
        //        }
        //        return 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}


        public async Task<int?> CompelteRecurrentSurveyEmail(CompleteSurveyReminderEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDate + " " + e.SurveyWindowStartTime + "];Server[" + DateTime.Now + "," + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.CompleteRecurrentSurvey(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int?> CompleteWarrenRecurrentSurveyEmail(CompleteWarrenSurveyReminderEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDate + " " + e.SurveyWindowStartTime + "];Server[" + DateTime.Now + "," + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.CompleteWarrenSurveyReminderSurvey(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }




        public async Task<int?> ExpiringSoonNotCompletedSurveyEmail(ExpirinSoonNotCompletedSurveyReminderEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " 
                    + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDateTime + "];Server[" + DateTime.Now + "," 
                    + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " 
                    + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.ExpiringSoonStartNotCompletedSurvey(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int?> ExpirinSoonNotCompletedWarrenSurveyReminderEmail(ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: "
                    + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDateTime + "];Server[" + DateTime.Now + ","
                    + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: "
                    + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.ExpirinSoonNotCompletedWarrenSurveyReminder(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int?> ExpiringSoonNotStarteSurveyEmail(ExpiringSoonNotStartedSurveyReminderEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDateTime + "];Server[" + DateTime.Now + "," + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.ExpiringSoonNotStartedSurvey(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int?> ExpiringSoonNotStartedWarrenSurveyReminderEmail(ExpiringSoonNotStartedWarrenSurveyReminderEmailDto e, double timespan)
        {
            try
            {
                string emailMsg = "Record[ProfileId: " + e.Id + ", RosterId: " + e.RosterItemId + ", SurveyId: " + e.SurveyId + ", SurveyStart: " + e.SurveyWindowStartDateTime + "];Server[" + DateTime.Now + "," + timespan + ", Trigger: " + DateTime.Now.AddMinutes(timespan) + ", Actual: " + DateTime.Now.AddMinutes(timespan + (DateTime.UtcNow - DateTime.Now).TotalMinutes) + "];";
                var result = BackgroundJob.Schedule(() => PostalEmail.ExpiringSoonNotStartedWarrenSurveyReminder(e, emailMsg),
                     TimeSpan.FromMinutes(timespan));

                int jobId;
                bool isJobCreate = int.TryParse(result, out jobId);
                if (isJobCreate)
                {
                    return jobId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public static async Task<int> StopKeepAliveJob()
        {
            try
            {
                HangfireScheduler.Ping();
                RecurringJob.RemoveIfExists("KeepAlive");


                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static bool Ping()
        {
            try
            {

                //return true;

                //string url = "http://localhost:50685/";
                string url = "http://surveyapp.laksakura.com/";
                //string url = "http://surveymyday.azurewebsites.net/";

                var request = (HttpWebRequest) WebRequest.Create(url);

                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";

                using (request.GetResponse())
                {
                    PostalEmail.SendEmailStayAlive(url);
                    return true;
                }

            }
            catch
            {
                return false;
            }
        }




    }
}