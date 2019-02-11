using System;
using System.Linq;
using System.Web;
using System.Data;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System.Data.Entity;
using SANSurveyWebAPI.Models;


using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using static SANSurveyWebAPI.Constants;
using SANSurveyWebAPI.DTOs;

namespace SANSurveyWebAPI.BLL
{

    public class CurrentProfile
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmailAddress { get; set; }
        public int OffsetFromUTC { get; set; }
        public string ClientInitials { get; set; }
    }

    public class ProfileDetailsByClient
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ClientInitials { get; set; }
        public string ClientName { get; set; }
    }
    public class EditProfile
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmailAddress { get; set; }
        public string ProfileMobile { get; set; }
        public bool IsMobileNotificationOff { get; set; }

        //public int OffsetFromUTC { get; set; }
    }


    
    public class ProfileService
    {
        #region
       
        public void Dispose()
        {
            db.Dispose();
        }
        #endregion


        private static bool UpdateDatabase = true;
        private ApplicationDbContext db;

        private HangfireScheduler schedulerSvc;


        public ProfileService(ApplicationDbContext context)
        {
            db = context;
            this.schedulerSvc = new HangfireScheduler();
        }

        public ProfileService()
            : this(new ApplicationDbContext())
        {
        }

        //-----------------

        public ProfileDto GetProfileById(int profileId)
        {
            Profile p = db.Profiles.Where(x => x.Id == profileId).SingleOrDefault();
            var dto = ObjectMapper.GetProfileDto(p);

            return dto;
        }


        public async Task<int> GetCurrentProfileIdAsync()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();

            return await db.Profiles
                .Where(x => x.LoginEmail == user)
                .Select(m => m.Id)
                .SingleOrDefaultAsync();
        }

        public int GetCurrentProfileIdNonAsync()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();

            return db.Profiles
                .Where(x => x.LoginEmail == user)
                .Select(m => m.Id)
                .FirstOrDefault();
        }


        public CurrentProfile GetCurrentProfile()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();

            user = StringCipher.EncryptRfc2898(user);

            var profile = db.Profiles
                .Where(x => x.LoginEmail == user)
                .Select(x => new CurrentProfile
                {
                    ProfileId = x.Id,
                    ProfileEmailAddress = x.LoginEmail,
                    ProfileName = x.Name,
                    OffsetFromUTC = (x.OffSetFromUTC * -1),
                    ClientInitials = x.ClientInitials
                }).SingleOrDefault();

            profile.ProfileEmailAddress = StringCipher.DecryptRfc2898(profile.ProfileEmailAddress);

            return profile;
        }

        public ProfileDetailsByClient GetClientDetailsByProfileId(int profileId)
        {
            var clientDetails = db.Profiles
                .Where(x => x.Id == profileId)
                .Select(x => new ProfileDetailsByClient
                {
                    ProfileId = x.Id,
                    ProfileName = x.Name,
                    ClientInitials = x.ClientInitials,
                    ClientName = x.ClientName
                }).SingleOrDefault();           

            return clientDetails;
        }

        public async Task<CurrentProfile> GetCurrentProfileAsync()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();
            user = StringCipher.EncryptRfc2898(user);


            var profile = await db.Profiles
                .Where(x => x.LoginEmail == user)
                .Select(x => new CurrentProfile
                {
                    ProfileId = x.Id,
                    ProfileEmailAddress = StringCipher.DecryptRfc2898(x.LoginEmail),
                    ProfileName = x.Name,
                    OffsetFromUTC = (x.OffSetFromUTC * -1)
                }).SingleOrDefaultAsync();
            return profile;
        }


        /* public EditProfileViewModel GetEditProfile()
         {
             string user = HttpContext.Current.User.Identity.GetUserName();
             user = StringCipher.EncryptRfc2898(user);
            //string encryptedUserId = StringCipher.EncryptRfc2898(user);

             var profile = db.Profiles
                .Where(x => x.LoginEmail == user)
                 //.Where(x => x.LoginEmail == encryptedUserId)
                 .Select(x => new EditProfileViewModel
                 {
                     Id = x.Id,
                     Email = StringCipher.DecryptRfc2898(x.LoginEmail),
                   //Email = x.LoginEmail,
                     Name = x.Name,
                     MobileNumber = x.MobileNumber,
                     //MOBILEON
                     //MobileNotificationOn = x.MobileNotificationOn
                 }).SingleOrDefault();
            // profile.Email = StringCipher.DecryptRfc2898(profile.Email);
             return profile;
         }*/

            /* Modification to user edit code on 26th July 2017 -- by Bharati*/
        public EditProfileViewModel GetEditProfile()
        {
            string user = HttpContext.Current.User.Identity.GetUserName();
            string encryptedUserId = StringCipher.EncryptRfc2898(user);

            var profile = db.Profiles
               .Where(x => x.LoginEmail == encryptedUserId)
                .Select(x => new EditProfileViewModel
                {
                    Id = x.Id,
                    Email = x.LoginEmail,
                     Name = x.Name,
                    MobileNumber = x.MobileNumber,
                }).SingleOrDefault();
            profile.Email = StringCipher.DecryptRfc2898(profile.Email);
            return profile;
        }
        /* End of Modification of user edit code*/

        #region
        /*public async Task<int> SaveProfile(EditProfileViewModel v)
        {
            var profile = new Profile() { Id = v.Id };
            profile.Name = v.Name;
            profile.MobileNumber = v.MobileNumber;
            profile.LoginEmail = v.Email;

            //MOBILEON
            //profile.MobileNotificationOn = v.MobileNotificationOn;

            db.Profiles.Attach(profile);

            db.Entry(profile).Property(x => x.Name).IsModified = true;
            db.Entry(profile).Property(x => x.LoginEmail).IsModified = true;
            db.Entry(profile).Property(x => x.MobileNumber).IsModified = true;

            //MOBILEON
            //db.Entry(profile).Property(x => x.MobileNotificationOn).IsModified = true;

            return await db.SaveChangesAsync();
        }*/

            /* Modification done to the user edit profile save changes to database on 26th July 2017 -- by Bharati*/
        public async Task<int> SaveProfile(EditProfileViewModel v)
        {
            var profile = new Profile() { Id = v.Id };
            profile.Name = v.Name;
            profile.MobileNumber = v.MobileNumber;
            //profile.LoginEmail = v.Email;

            profile.LoginEmail = StringCipher.EncryptRfc2898(v.Email);

            db.Profiles.Attach(profile);

            db.Entry(profile).Property(x => x.Name).IsModified = true;
            db.Entry(profile).Property(x => x.LoginEmail).IsModified = true;
            db.Entry(profile).Property(x => x.MobileNumber).IsModified = true;

            return await db.SaveChangesAsync();
        }
        /* End of modification to the save code*/
        public async Task<int> ResetProfileAsync(int profileId, Guid uid, int? emailJobId, int? smsJobId)
        {
            try
            {
                Profile profile = db.Profiles.Where(x => x.Id == profileId).SingleOrDefault();
                string currEmailJobId = "0";
                string currSMSJobId = "0";

                if (profile.RegistrationEmailJobId.HasValue)
                {
                    currEmailJobId = profile.RegistrationEmailJobId.Value.ToString();
                }
                if (profile.RegistrationSmsJobId.HasValue)
                {
                    currSMSJobId = profile.RegistrationSmsJobId.Value.ToString();
                }



                profile.Uid = uid.ToString();
                profile.CurrentLevelOfTraining = null;


                //db.ProfileEthinicitys.RemoveRange(db.ProfileEthinicitys.Where(x => x.ProfileId == profile.Id));

                db.ProfileWellbeings.RemoveRange(db.ProfileWellbeings.Where(x => x.ProfileId == profile.Id));
                db.ProfileTaskTimes.RemoveRange(db.ProfileTaskTimes.Where(x => x.ProfileId == profile.Id));
                db.ProfilePlacements.RemoveRange(db.ProfilePlacements.Where(x => x.ProfileId == profile.Id));
                db.ProfileContracts.RemoveRange(db.ProfileContracts.Where(x => x.ProfileId == profile.Id));
                db.ProfileTrainings.RemoveRange(db.ProfileTrainings.Where(x => x.ProfileId == profile.Id));
                db.ProfileDemographics.RemoveRange(db.ProfileDemographics.Where(x => x.ProfileId == profile.Id));
                db.ProfileComments.RemoveRange(db.ProfileComments.Where(x => x.ProfileId == profile.Id));

                //profile.SwbLife = null;
                //profile.SwbHome = null;
                //profile.SwbJob = null;
                //profile.SpecialityId = null;

                profile.ProfileTaskType = null;

                profile.MaxStep = 0;
                profile.IsCurrentPlacement = null;

                profile.RegisteredDateTimeUtc = null;

                profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Invited.ToString();

                if (emailJobId.HasValue)
                    profile.RegistrationEmailJobId = emailJobId.Value;
                else
                    profile.RegistrationEmailJobId = null;

                if (smsJobId.HasValue)
                    profile.RegistrationSmsJobId = smsJobId.Value;
                else
                    profile.RegistrationSmsJobId = null;


                db.Entry(profile).State = EntityState.Modified;

                db.ProfileEthinicitys.RemoveRange(db.ProfileEthinicitys.Where(x => x.ProfileId == profile.Id));
                db.ProfileSpecialtys.RemoveRange(db.ProfileSpecialtys.Where(x => x.ProfileId == profile.Id));

                //var rosters = db.RosterItems
                //    .Where(x => x.ProfileId == profile.Id);

                //var rosters = db.ProfileRosters
                //  .Where(x => x.ProfileId == profile.Id);

                //foreach (var r in rosters)
                //{
                //    //Delete current Jobs related to this shift in Hangfire
                //    if (r.ShiftReminderEmailJobId.HasValue)
                //        await schedulerSvc.DeleteScheduledJob(r.ShiftReminderEmailJobId.Value.ToString());
                //    if (r.ShiftReminderSmsJobId.HasValue)
                //        await schedulerSvc.DeleteScheduledJob(r.ShiftReminderSmsJobId.Value.ToString());
                //    if (r.CreateSurveyJobId.HasValue)
                //        await schedulerSvc.DeleteScheduledJob(r.CreateSurveyJobId.Value.ToString());
                //}

                //Clear Current Jobs of Surveys
                await ResetShiftRemindesForProfileAsync(profile);
                await ResetCreateSurveyForProfileAsync(profile);
                await ClearStartSurveyRemindersForProfileAsync(profile);
                await ClearCompleteSurveyRemindersForProfileAsync(profile);
                await ClearExpiringSoonNotStartedRemindersForProfileAync(profile);
                await ClearExpiringSoonNotCompletedRemindersForProfileAysnc(profile);



                db.ProfileRosters.RemoveRange(db.ProfileRosters.Where(x => x.ProfileId == profile.Id));
                db.ProfileTasks.RemoveRange(db.ProfileTasks.Where(x => x.ProfileId == profile.Id));
                var surveys = db.Surveys.RemoveRange(db.Surveys.Where(x => x.ProfileId == profile.Id));
                db.Responses.RemoveRange(db.Responses.Where(x => x.ProfileId == profile.Id));

                //TODO: Clear Current Jobs of Profile 
                schedulerSvc.DeleteScheduledJob(currEmailJobId);
                schedulerSvc.DeleteScheduledJob(currSMSJobId);

                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private async Task ClearExpiringSoonNotCompletedRemindersForProfileAysnc(Profile profile)
        {
            var expiringSoonNotCompletedReminders = db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in expiringSoonNotCompletedReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.RemoveRange(db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private async Task ClearExpiringSoonNotStartedRemindersForProfileAync(Profile profile)
        {
            var expiringSoonNotStartedReminders = db.JobLogExpiringSoonSurveyNotStartedReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in expiringSoonNotStartedReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogExpiringSoonSurveyNotStartedReminderEmails.RemoveRange(db.JobLogExpiringSoonSurveyNotStartedReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private async Task ClearCompleteSurveyRemindersForProfileAsync(Profile profile)
        {
            var completeSurveyReminders = db.JobLogCompleteSurveyReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in completeSurveyReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogCompleteSurveyReminderEmails.RemoveRange(db.JobLogCompleteSurveyReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private async Task ClearStartSurveyRemindersForProfileAsync(Profile profile)
        {
            var startSurveyReminders = db.JobLogStartSurveyReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in startSurveyReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogStartSurveyReminderEmails.RemoveRange(db.JobLogStartSurveyReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private async Task ResetCreateSurveyForProfileAsync(Profile profile)
        {
            var createSurveyJobs = db.JobLogCreateSurveys.Where(x => x.ProfileId == profile.Id);
            foreach (var x in createSurveyJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogCreateSurveys.RemoveRange(db.JobLogCreateSurveys.Where(x => x.ProfileId == profile.Id));
        }

        private async Task ResetShiftRemindesForProfileAsync(Profile profile)
        {
            var shiftReminderJobs = db.JobLogShiftReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in shiftReminderJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    await schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogShiftReminderEmails.RemoveRange(db.JobLogShiftReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        public int ResetProfile(int profileId, Guid uid, int? emailJobId, int? smsJobId)
        {
            try
            {
                Profile profile = db.Profiles.Where(x => x.Id == profileId).SingleOrDefault();
                string currEmailJobId = "0";
                string currSMSJobId = "0";

                if (profile.RegistrationEmailJobId.HasValue)
                {
                    currEmailJobId = profile.RegistrationEmailJobId.Value.ToString();
                }
                if (profile.RegistrationSmsJobId.HasValue)
                {
                    currSMSJobId = profile.RegistrationSmsJobId.Value.ToString();
                }



                profile.Uid = uid.ToString();
                profile.CurrentLevelOfTraining = null;


                //db.ProfileEthinicitys.RemoveRange(db.ProfileEthinicitys.Where(x => x.ProfileId == profile.Id));

                db.ProfileWellbeings.RemoveRange(db.ProfileWellbeings.Where(x => x.ProfileId == profile.Id));
                db.ProfileTaskTimes.RemoveRange(db.ProfileTaskTimes.Where(x => x.ProfileId == profile.Id));
                db.ProfilePlacements.RemoveRange(db.ProfilePlacements.Where(x => x.ProfileId == profile.Id));
                db.ProfileContracts.RemoveRange(db.ProfileContracts.Where(x => x.ProfileId == profile.Id));
                db.ProfileTrainings.RemoveRange(db.ProfileTrainings.Where(x => x.ProfileId == profile.Id));
                db.ProfileDemographics.RemoveRange(db.ProfileDemographics.Where(x => x.ProfileId == profile.Id));
                db.ProfileComments.RemoveRange(db.ProfileComments.Where(x => x.ProfileId == profile.Id));

                //profile.SwbLife = null;
                //profile.SwbHome = null;
                //profile.SwbJob = null;
                //profile.SpecialityId = null;

                profile.ProfileTaskType = null;

                profile.MaxStep = 0;
                profile.IsCurrentPlacement = null;

                profile.RegisteredDateTimeUtc = null;

                profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Invited.ToString();

                if (emailJobId.HasValue)
                    profile.RegistrationEmailJobId = emailJobId.Value;
                else
                    profile.RegistrationEmailJobId = null;

                if (smsJobId.HasValue)
                    profile.RegistrationSmsJobId = smsJobId.Value;
                else
                    profile.RegistrationSmsJobId = null;


                db.Entry(profile).State = EntityState.Modified;

                db.ProfileEthinicitys.RemoveRange(db.ProfileEthinicitys.Where(x => x.ProfileId == profile.Id));
                db.ProfileSpecialtys.RemoveRange(db.ProfileSpecialtys.Where(x => x.ProfileId == profile.Id));


                //var rosters = db.ProfileRosters
                //  .Where(x => x.ProfileId == profile.Id);


                // Clear Current Jobs of Surveys
                ResetShiftRemindersForProfile(profile);
                ResetCreateSurveyJobsForProfile(profile);
                ClearStartSurveyRemindersForProfile(profile);
                ClearCompleteSurveyRemindersForProfile(profile);
                ClearExpiringSoonNotStartedRemindersForProfile(profile);
                ClearExpiringSoonNotCompletedRemindersForProfile(profile);
                //-----

                db.ProfileRosters.RemoveRange(db.ProfileRosters.Where(x => x.ProfileId == profile.Id));
                db.ProfileTasks.RemoveRange(db.ProfileTasks.Where(x => x.ProfileId == profile.Id));
                var surveys = db.Surveys.RemoveRange(db.Surveys.Where(x => x.ProfileId == profile.Id));
                db.Responses.RemoveRange(db.Responses.Where(x => x.ProfileId == profile.Id));

                //Clear Current Jobs of Profile
                //TODO: Remove these fields from Profile table
                schedulerSvc.DeleteScheduledJob(currEmailJobId);
                schedulerSvc.DeleteScheduledJob(currSMSJobId);


                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private void ResetCreateSurveyJobsForProfile(Profile profile)
        {
            var createSurveyJobs = db.JobLogCreateSurveys.Where(x => x.ProfileId == profile.Id);
            foreach (var x in createSurveyJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogCreateSurveys.RemoveRange(db.JobLogCreateSurveys.Where(x => x.ProfileId == profile.Id));
        }

        private void ResetShiftRemindersForProfile(Profile profile)
        {
            var shiftReminderJobs = db.JobLogShiftReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in shiftReminderJobs)
            {
                if (x.HangfireJobId.HasValue)
                {
                    schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogShiftReminderEmails.RemoveRange(db.JobLogShiftReminderEmails.Where(x => x.ProfileId == profile.Id));
        }



        private void ClearExpiringSoonNotCompletedRemindersForProfile(Profile profile)
        {
            var expiringSoonNotCompletedReminders = db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in expiringSoonNotCompletedReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.RemoveRange(db.JobLogExpiringSoonSurveyNotCompletedReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private void ClearExpiringSoonNotStartedRemindersForProfile(Profile profile)
        {
            var expiringSoonNotStartedReminders = db.JobLogExpiringSoonSurveyNotStartedReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in expiringSoonNotStartedReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogExpiringSoonSurveyNotStartedReminderEmails.RemoveRange(db.JobLogExpiringSoonSurveyNotStartedReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private void ClearCompleteSurveyRemindersForProfile(Profile profile)
        {
            var completeSurveyReminders = db.JobLogCompleteSurveyReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in completeSurveyReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogCompleteSurveyReminderEmails.RemoveRange(db.JobLogCompleteSurveyReminderEmails.Where(x => x.ProfileId == profile.Id));
        }

        private void ClearStartSurveyRemindersForProfile(Profile profile)
        {
            var startSurveyReminders = db.JobLogStartSurveyReminderEmails.Where(x => x.ProfileId == profile.Id);
            foreach (var x in startSurveyReminders)
            {
                if (x.HangfireJobId.HasValue)
                {
                    schedulerSvc.DeleteScheduledJob(x.HangfireJobId.Value.ToString());
                }
            }
            db.JobLogStartSurveyReminderEmails.RemoveRange(db.JobLogStartSurveyReminderEmails.Where(x => x.ProfileId == profile.Id));
        }


        #endregion
    }
}