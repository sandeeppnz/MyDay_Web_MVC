using Hangfire;
using Microsoft.AspNet.Identity;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static SANSurveyWebAPI.BLL.SurveyService;

namespace SANSurveyWebAPI.BLL
{
    public class UserHomeService
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static bool UpdateDatabase = true;


        //private SchedulerService schedulerService;


        public UserHomeService()
        {
            //this.schedulerService = new SchedulerService();
        }


        public void Dispose()
        {
            //schedulerService.Dispose();
            _unitOfWork.Dispose();
        }

        public ProfileDto GetProfileById(int id)
        {
            Profile profile = _unitOfWork.ProfileRespository.GetByID(id);
        
            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
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

        public async virtual Task<List<HomeMySurveyVM>> GetHomeMySurveysList(string baseUrl, int profileId)
        {
            List<HomeMySurveyVM> list = new List<HomeMySurveyVM>();

            #region Expiring Soon
            List<HomeMySurvey> expiringSoonSurveys = await GetHomeMySurveysExpiringSoon(profileId);
            foreach (var s in expiringSoonSurveys)
            {
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Expiring soon";
                v.ActionStr = baseUrl + "Survey3/Index?id=" + s.Uid;
                list.Add(v);
            }
            #endregion

            #region Expired
            List<HomeMySurvey> expiredSurveys = await GetHomeMySurveysExpired(profileId);
            foreach (var s in expiredSurveys)
            {
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Expired";
                v.ActionStr = baseUrl + "Survey3/View?id=" + s.Uid;
                list.Add(v);
            }
            #endregion

            #region Completed Surveys
            List<HomeMySurvey> completedSurveys = await GetHomeMySurveysCompleted(profileId);
            foreach (var s in completedSurveys)
            {
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Completed";
                v.ActionStr = baseUrl + "Survey3/View?id=" + s.Uid;
                list.Add(v);
            }
            #endregion

            #region Invited
            List<HomeMySurvey> invitedSurveys = await GetHomeMySurveysInvited(profileId);
            foreach (var s in invitedSurveys)
            {
                DateTime currentdatetime = DateTime.Now;
                DateTime? expirydate = s.SurveyExpiryDateTime;
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Invited";
                v.ActionStr = baseUrl + "Survey3/Index?id=" + s.Uid;
                list.Add(v);
            }
            #endregion
            return list;
        }

        //For WAM User
        public async virtual Task<List<HomeMySurveyVM>> GetWAMHomeMySurveysList(string baseUrl, int profileId)
        {
            List<HomeMySurveyVM> list = new List<HomeMySurveyVM>();

            #region Expiring Soon
            List<HomeMySurvey> expiringSoonSurveys = await GetHomeMySurveysExpiringSoon(profileId);
            foreach (var s in expiringSoonSurveys)
            {
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Expiring soon";
                v.ActionStr = baseUrl + "Survey3/Index?id=" + s.Uid;
                list.Add(v);
            }
            #endregion

            #region Expired
            List<HomeMySurvey> expiredSurveys = await GetHomeMySurveysExpired(profileId);
            foreach (var s in expiredSurveys)
            {
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Expired";
                v.ActionStr = baseUrl + "Survey3/WAMView?id=" + s.Uid;
                list.Add(v);
            }
            #endregion

            #region Completed Surveys
            List<HomeMySurvey> completedSurveys = await GetHomeMySurveysCompleted(profileId);
            foreach (var s in completedSurveys)
            {
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Completed";
                v.ActionStr = baseUrl + "Survey3/WAMView?id=" + s.Uid;
                list.Add(v);
            }
            #endregion

            #region Invited
            List<HomeMySurvey> invitedSurveys = await GetHomeMySurveysInvited(profileId);
            foreach (var s in invitedSurveys)
            {
                DateTime currentdatetime = DateTime.Now;
                DateTime? expirydate = s.SurveyExpiryDateTime;
                HomeMySurveyVM v = new HomeMySurveyVM();
                v.DateStr = string.Format("{0:dd/MM}", s.SurveyStartDateTime.Value);
                v.NameStr = s.SurveyDescription;
                v.StatusStr = "Invited";
                v.ActionStr = baseUrl + "Survey3/Index?id=" + s.Uid;
                list.Add(v);
            }
            #endregion
            return list;
        }

        public async Task<List<NotificationVM>> GetNotificationList(string baseUrl, int profileId)
        {
            List<NotificationVM> list = new List<NotificationVM>();

            #region Roster Empty?
            if (await CheckUpdateRoster(profileId))
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "RED";
                v.Message = Constants.Notification_UpdateRoster;
                v.Link = baseUrl + "Calendar/List/" + DateTime.UtcNow.ToString("yyyy-MM");
                list.Add(v);
            }
            #endregion

            #region Expiring Surveys
            List<string> expiringSurveys = await GetExpiringSurvey(profileId);
            foreach (var s in expiringSurveys)
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "RED";
                v.Message = Constants.Notification_ExpiringSurvey;
                v.Link = baseUrl + "Survey3/Index?id=" + s;
                list.Add(v);
            }
            #endregion

            #region Incomplete Surveys
            List<string> incompleteSurveys = await CheckIncompleteSurvey(profileId);
            foreach (var s in incompleteSurveys)
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "ORANGE";
                v.Message = Constants.Notification_IncompleteSurvey;
                v.Link = baseUrl + "Survey3/Index?id=" + s;
                list.Add(v);
            }
            #endregion

            #region NewSurveys?
            List<string> newSurveys = await CheckNewSurvey(profileId);
            foreach (var s in newSurveys)
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "";
                v.Message = Constants.Notification_NewSurvey;
                v.Link = baseUrl + "Survey3/Index?id=" + s;
                list.Add(v);
            }
            #endregion


            return list;
        }


        #region Surveys
        public async virtual Task<List<HomeMySurvey>> GetHomeMySurveysExpiringSoon(int profileId)
        {
            //int profileId = await profileSvc.GetCurrentProfileId();
            //TODO: MOD
            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            //Expiring Soon
            var list = _unitOfWork.SurveyRespository
                        .GetUsingNoTracking(x => (x.ProfileId == profileId)
                        && (x.SurveyProgressNext != Constants.StatusSurveyProgress.New.ToString())
                        && (x.SurveyUserCompletedDateTimeUtc == null)
                        && (x.SurveyWindowStartDateTime != null)
                        && (x.SurveyExpiryDateTimeUtc <= toDate)
                        && (x.SurveyExpiryDateTimeUtc > DateTime.UtcNow)
                        )
                        .OrderByDescending(x => x.SurveyWindowStartDateTime)
                        .Select(x => new HomeMySurvey()
                        {
                            SurveyId = x.Id,
                            SurveyDescription = x.SurveyDescription,
                            SurveyStartDateTime = x.SurveyWindowStartDateTime,
                            SurveyExpiryDateTime = x.SurveyExpiryDateTime,
                            Uid = x.Uid,
                            Status = x.Status
                        })
                         .ToList()
                        ;
            return list;
        }

        public async virtual Task<List<HomeMySurvey>> GetHomeMySurveysExpired(int profileId)
        {
            var list = _unitOfWork.SurveyRespository
                    .GetUsingNoTracking(x => (x.ProfileId == profileId)
                    && (x.SurveyProgressNext != Constants.StatusSurveyProgress.Completed.ToString())
                    && (x.SurveyProgressNext != Constants.StatusSurveyProgress.New.ToString())
                    && (x.SurveyWindowStartDateTime != null)
                    && (x.SurveyUserCompletedDateTimeUtc == null)
                    && (x.SurveyExpiryDateTimeUtc < DateTime.UtcNow)
                    )
                    .OrderByDescending(x => x.SurveyWindowStartDateTime)
                    .Select(x => new HomeMySurvey()
                    {
                        SurveyId = x.Id,
                        SurveyDescription = x.SurveyDescription,
                        SurveyStartDateTime = x.SurveyWindowStartDateTime,
                        SurveyExpiryDateTime = x.SurveyExpiryDateTime,
                        Uid = x.Uid,
                        Status = x.Status
                    })
                     .ToList()
                    ;


            return list;
        }

        public async virtual Task<List<HomeMySurvey>> GetHomeMySurveysCompleted(int profileId)
        {

            var list = _unitOfWork.SurveyRespository
              .GetUsingNoTracking(x => (x.ProfileId == profileId)
              && (x.SurveyProgressNext == Constants.StatusSurveyProgress.Completed.ToString())
              //&& (x.SurveyUserCompletedDateTimeUtc != null)
              && (x.SurveyWindowStartDateTime != null)
              )
              .OrderByDescending(x => x.SurveyWindowStartDateTime)
              .Select(x => new HomeMySurvey()
              {
                  SurveyId = x.Id,
                  SurveyDescription = x.SurveyDescription,
                  SurveyStartDateTime = x.SurveyWindowStartDateTime,
                  SurveyExpiryDateTime = x.SurveyExpiryDateTime,
                  Uid = x.Uid,
                  Status = x.Status
              })
               .ToList()
              ;

            return list;
        }
        public async virtual Task<List<HomeMySurvey>> GetHomeMySurveysInvited(int profileId)
        {

            var list = _unitOfWork.SurveyRespository
              .GetUsingNoTracking(x => (x.ProfileId == profileId)
              && (x.SurveyProgressNext == Constants.StatusSurveyProgress.Invited.ToString())              
              && (x.SurveyWindowStartDateTime != null)
              && (DateTime.Now >= x.SurveyWindowEndDateTime)
              && (DateTime.Now < x.SurveyExpiryDateTime)
              )
              .OrderByDescending(x => x.SurveyWindowStartDateTime)
              .Select(x => new HomeMySurvey()
              {
                  SurveyId = x.Id,
                  SurveyDescription = x.SurveyDescription,
                  SurveyStartDateTime = x.SurveyWindowStartDateTime,
                  SurveyExpiryDateTime = x.SurveyExpiryDateTime,
                  Uid = x.Uid,
                  Status = x.Status
              })
               .ToList()
              ;

            return list;
        }
        #endregion

        #region Notifications
        //private async Task<bool> CheckUpdateRoster(int profileId)
        //{
        //    //if the roster is empty for next two weeks
        //    return await IsNeedRosterUpdate(profileId);
        //}

        //private async Task<List<string>> GetExpiringSurvey(int profileId)
        //{
        //    return await GetExpiringSoon(profileId);
        //}


        //private async Task<List<string>> CheckIncompleteSurvey(int profileId)
        //{
        //    return await GetIncompleteSurvey(profileId);

        //}

        //public async Task<List<string>> CheckNewSurvey(int profileId)
        //{
        //    return await GetNewSurvey(profileId);
        //}

        private async Task<bool> CheckUpdateRoster(int profileId)
        {
            //if the roster is empty for next two weeks
            return await IsNeedRosterUpdate(profileId);
        }

        private async Task<List<string>> GetExpiringSurvey(int profileId)
        {
            return await GetExpiringSoon(profileId);
        }


        private async Task<List<string>> CheckIncompleteSurvey(int profileId)
        {
            return await GetIncompleteSurvey(profileId);

        }

        public async Task<List<string>> CheckNewSurvey(int profileId)
        {
            return await GetNewSurvey(profileId);
        }

        public async virtual Task<bool> IsNeedRosterUpdate(int profileId)
        {
            try
            {
                //TODO: MOD
                DateTime fromDate = DateTime.UtcNow.Date;
                DateTime toDate = fromDate.AddDays(Constants.ROSTER_EMPTY_PERIOD_TILL_DAYS + 1);

                //TODO: MOD
                //var list = _unitOfWork.RosterItemRespository
                //            .GetUsingNoTracking(x => (x.ProfileId == profileId)
                //                && (x.Start >= DateTime.UtcNow)
                //                && (x.Start <= toDate)
                //            )
                //            .Select(x => x.Id)
                //            .Count();

                var list = _unitOfWork.ProfileRosterRespository
                          .GetUsingNoTracking(x => (x.ProfileId == profileId)
                              && (x.Start >= DateTime.UtcNow)
                              && (x.Start <= toDate)
                          )
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


        public async virtual Task<List<string>> GetExpiringSoon(int profileId)
        {
            //TODO: MOD
            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            var list = _unitOfWork.SurveyRespository
                        .GetUsingNoTracking(x => (x.ProfileId == profileId)
                              && (x.SurveyProgressNext != Constants.StatusSurveyProgress.New.ToString())
                              && (x.SurveyUserCompletedDateTimeUtc == null)
                              && (x.SurveyExpiryDateTimeUtc <= toDate)
                              && (x.SurveyExpiryDateTimeUtc > DateTime.UtcNow)
                        )
                       .OrderBy(x => x.SurveyExpiryDateTime)
                       .Select(x => x.Uid)
                       .ToList();

            return list;
        }


        public async virtual Task<List<string>> GetIncompleteSurvey(int profileId)
        {
            //int profileId = await profileSvc.GetCurrentProfileId();

            //TODO: MOD
            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            var list = _unitOfWork.SurveyRespository
                        .GetUsingNoTracking(x => (x.ProfileId == profileId)
                            && (x.SurveyProgressNext != Constants.StatusSurveyProgress.Invited.ToString())
                            && (x.SurveyProgressNext != Constants.StatusSurveyProgress.New.ToString())
                             && (x.SurveyProgressNext != Constants.StatusSurveyProgress.Completed.ToString())
                            //&& (x.SurveyUserCompletedDateTimeUtc == null)
                            && (x.SurveyExpiryDateTimeUtc > toDate)
            )
              .OrderBy(x => x.SurveyExpiryDateTimeUtc)
             .Select(x => x.Uid)
             .ToList();

            return list;
        }


        public async virtual Task<List<string>> GetNewSurvey(int profileId)
        {
            //int profileId = await profileSvc.GetCurrentProfileId();

            //TODO: MOD
            DateTime fromDate = DateTime.UtcNow;
            DateTime toDate = fromDate.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);

            var list = _unitOfWork.SurveyRespository
                         .GetUsingNoTracking(x => (x.ProfileId == profileId)
                         && (x.SurveyProgressNext == Constants.StatusSurveyProgress.Invited.ToString())
                         && (x.SurveyExpiryDateTimeUtc > toDate)
                         )
                         .OrderBy(x => x.SurveyExpiryDateTimeUtc)
                         .Select(x => x.Uid)
                         .ToList();

            return list;
        }

        #endregion


    }
}