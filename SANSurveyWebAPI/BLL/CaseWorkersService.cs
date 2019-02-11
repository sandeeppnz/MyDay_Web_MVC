using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using Hangfire;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.BLL
{
    public class CaseWorkersService : IDisposable
    {
        //For Social Workers
        readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private HangfireScheduler schedulerService;
        private JobService jobService;
        private ProfileService profileService;

        private static bool UpdateDatabase = true;

        public CaseWorkersService()
        {
            this.schedulerService = new HangfireScheduler();
            this.jobService = new JobService();
            this.profileService = new ProfileService();
        }

        //Implementation of IDisposable interface
        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
        #region Profile Info

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
        public ProfileDto GetProfileById(int id)
        {
            Profile profile = _unitOfWork.ProfileRespository.GetByID(id);
            if (profile != null)
            {
                return ObjectMapper.GetProfileDto(profile);
            }
            return null;
        }
        public int ValidateRoster(int profileId)
        {
            /*             
             Logic checked in Controller:
                    1) If the total number of slots is less than 1, the calendar is empty
                    2) If the total number of slots is less than 3, the calendar is incomplete
                    3) Else the calendar is valid
             */

            return
                        _unitOfWork.ProfileRosterRespository
                                                .GetUsingNoTracking(m => (m.ProfileId == profileId))
                                                .Count();
        }
        public void UpdateProfile(ProfileDto p)
        {
            Profile e = ObjectMapper.GetProfileEntity(p);
            _unitOfWork.ProfileRespository.Update(e);
            _unitOfWork.SaveChanges();
        }

        public int? ConvertToNumber(decimal? val)
        {
            if (val.HasValue)
            {

                decimal x = val.Value * 100;

                return (int)x;
            }
            return null;
        }
        public decimal? ConvertToDecimal(decimal? s)
        {
            if (s.HasValue)
                return s / 100;
            else
                return null;
        }
        public IEnumerable<System.Web.Mvc.SelectListItem> GetAllBirthYears()
        {
            var list = _unitOfWork.BirthYearRespository.Get()
                .OrderBy(x => x.Sequence)
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name.ToString()
                });

            return list;
        }
        public void SaveComment(ProfileCommentDto d)
        {
            var list = _unitOfWork.ProfileCommentRespository.Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.ProfileCommentRespository.RemoveRange(list);

            var e = ObjectMapper.GetProfileCommentEntity(d);
            _unitOfWork.ProfileCommentRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        #endregion

        #region Social Workers - Baseline or Registration

        public SWSubjectiveWellBeingDto GetSubjectiveWellBeingByProfileId(int profileId)
        {
            SWSubjectiveWellBeing profile = _unitOfWork.SWSubjectiveWellbeingRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetSWSubjectiveWellbeingDto(profile);
            }
            return null;

        }
        public async Task SaveSWSubjectiveWellbeing(SWSubjectiveWellBeingDto d)
        {
            var list = _unitOfWork.SWSubjectiveWellbeingRespository.Get(p => p.ProfileId == d.ProfileId);
                       _unitOfWork.SWSubjectiveWellbeingRespository.RemoveRange(list);

            var e = ObjectMapper.GetSWSubjectiveMWellbeingEntity(d);
                    _unitOfWork.SWSubjectiveWellbeingRespository.Insert(e);
                    _unitOfWork.SaveChanges();
        }

        public IList<CaseTasksQA> GetOptionsforCurrentWorkplace()
        {
            string q1 = "Screening";
            string q2 = "Investigations";
            string q3 = "Ongoing/in-home services";
            string q4 = "Placement";
            string q5 = "Adoption or Guardianship";

            string tooltip = "";

            CaseTasksQA q1o = new CaseTasksQA { ID = 0, Name = q1, LongName = null, Frequency = null, Ans = false };
            CaseTasksQA q2o = new CaseTasksQA { ID = 1, Name = q2, LongName = null, Frequency = null, Ans = false };
            CaseTasksQA q3o = new CaseTasksQA { ID = 2, Name = q3, LongName = null, Frequency = null, Ans = false };
            CaseTasksQA q4o = new CaseTasksQA { ID = 3, Name = q4, LongName = null, Frequency = null, Ans = false };
            CaseTasksQA q5o = new CaseTasksQA { ID = 4, Name = q5, LongName = null, Frequency = null, Ans = false };

            List<CaseTasksQA> list = new List<CaseTasksQA>();
            list.Add(q1o);
            list.Add(q2o);
            list.Add(q3o);
            list.Add(q4o);
            list.Add(q5o);

            return list;
        }

        public IList<CaseTasksQA> GetCurrentWorkplace(int profileId)
        {
            var list = _unitOfWork.CurrentWorkplaceRepository
                                  .GetUsingNoTracking(x => x.ProfileId == profileId)
                                  .Select(m => new CaseTasksQA()
                                  {
                                      ID = m.OptionId,
                                      Name = m.OptionValue,  
                                      Ans = m.Ans,
                                  }).ToList();

            return list;
        }
        public CurrentWorkPlaceContdDto GetCurrentWorkplaceContd(int profileId)
        {
            CurrentWorkplaceContd profile = _unitOfWork.CurrentWorkplaceContdRepository
                                            .GetUsingNoTracking(x => x.ProfileId == profileId)
                                            .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetCurrentWorkPlaceContdDto(profile);
            }
            return null;
        }
        public async Task SaveCurrentWorkplace(int profileId, IList<CaseTasksQA> selectedQns)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.CurrentWorkplaceRepository.Get(p => p.ProfileId == profileId );
                           _unitOfWork.CurrentWorkplaceRepository.RemoveRange(list);
                           _unitOfWork.SaveChanges();

                foreach (var r in selectedQns)
                {
                    if (!string.IsNullOrEmpty(r.Name))
                    {
                        var newDefaultTask = new CurrentWorkplace
                        {
                            ProfileId = profileId,
                            OptionId = r.ID,
                            OptionValue = r.Name,
                            Ans = r.Ans
                        };
                        _unitOfWork.CurrentWorkplaceRepository.Insert(newDefaultTask);
                    }
                }
                        _unitOfWork.SaveChanges();
            });
        }

        #endregion
        public IList<SelectedTaskVM> GetProfileTaskItemTableListByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileTasksRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => new SelectedTaskVM()
                {
                    ID = m.TaskItemId,
                    Frequency = m.Frequency
                }).ToList();

            return list;

        }
        public IList<TaskVM> GetTasksItemsTableListByType(string type)
        {
            var list = _unitOfWork.TaskItemRespository.Get(x => x.Type == type)
                .Select(x => new TaskVM()
                {
                    ID = x.Id,
                    Name = x.ShortName,
                    LongName = x.LongName,
                    Frequency = null
                }).ToList();

            return list;
        }
        public IList<int> GetProfileTaskItemsByProfileId(int profileId)
        {
            var list = _unitOfWork.ProfileTasksRespository
                .GetUsingNoTracking(x => x.ProfileId == profileId)
                .Select(m => m.TaskItemId)
                .ToList();

            return list;

        }
        public async Task SaveDefaultTaskAsync(int profileId, IList<TaskVM> selectedTasks)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ProfileTasksRespository.Get(p => p.ProfileId == profileId);

                _unitOfWork.ProfileTasksRespository.RemoveRange(list);
                _unitOfWork.SaveChanges();

                foreach (var r in selectedTasks)
                {
                    if (!string.IsNullOrEmpty(r.Frequency))
                    {
                        var newDefaultTask = new ProfileTask
                        {
                            ProfileId = profileId,
                            TaskItemId = r.ID,
                            Frequency = r.Frequency,
                            CreatedDateTimeUtc = DateTime.UtcNow
                        };
                        _unitOfWork.ProfileTasksRespository.Insert(newDefaultTask);
                    }
                }
                _unitOfWork.SaveChanges();
            });
        }
        #region Case Load

        public CaseLoadDto GetCaseLoadByProfileId(int profileId)
        {
            CaseLoad profile = _unitOfWork.CaseLoadRepository.GetUsingNoTracking(x => x.ProfileId == profileId)
                                                             .FirstOrDefault();

                        if (profile != null)
                        {
                            return ObjectMapper.GetCaseLoadDto(profile);
                        }
                        return null;
        }
     
        public CaseLoadDto GetCaseLoadOptionsByProfileId(int profileId)
        {
            var profile = _unitOfWork.CaseLoadRepository
                                           .GetUsingNoTracking(x => x.ProfileId == profileId)
                                           .FirstOrDefault();
            if (profile != null)
            { return ObjectMapper.GetCaseLoadDto(profile); }

            return null;
        }
       
        public async Task SaveCaseLoad(CaseLoadDto d)
        {
            var list = _unitOfWork.CaseLoadRepository
                                  .Get(p => p.ProfileId == d.ProfileId );
                       _unitOfWork.CaseLoadRepository.RemoveRange(list);

            var e = ObjectMapper.GetCaseLoadEntity(d);
                    _unitOfWork.CaseLoadRepository.Insert(e);
                    _unitOfWork.SaveChanges();
        }
        #endregion

        public async Task SaveCurrentWorkPlaceContd(CurrentWorkPlaceContdDto d)
        {
            var list = _unitOfWork.CurrentWorkplaceContdRepository
                                  .Get(p => p.ProfileId == d.ProfileId);
            _unitOfWork.CurrentWorkplaceContdRepository.RemoveRange(list);

            var e = ObjectMapper.GetCurrentWorkPlaceContdEntity(d);
            _unitOfWork.CurrentWorkplaceContdRepository.Insert(e);
            _unitOfWork.SaveChanges();
        }

        #region Time Allocation

        public TimeAllocationDto GetTimeAllocatedByProfileId(int profileId)
        {
            TimeAllocation profile = _unitOfWork.TimeAllocatedRepository.GetUsingNoTracking(x => x.ProfileId == profileId)
                                                                        .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetTimeAllocatedDto(profile);
            }
            return null;
        }
        public void SaveTimeAllocation(TimeAllocationDto d)
        {
            var list = _unitOfWork.TimeAllocatedRepository.Get(p => p.ProfileId == d.ProfileId);
                       _unitOfWork.TimeAllocatedRepository.RemoveRange(list);

            var e = ObjectMapper.GetTimeAllocatedEntity(d);
                   _unitOfWork.TimeAllocatedRepository.Insert(e);
                   _unitOfWork.SaveChanges();
        }

        #endregion

        #region Demographics

        public DemographicsDto GetCWDemographicsByProfileId(int profileId)
        {
            Demographics profile = _unitOfWork.CaseDemographicsRespository.GetUsingNoTracking(x => x.ProfileId == profileId)
                                                                        .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetCWDemographics(profile);
            }
            return null;
        }
        public void SaveCWDemographics(DemographicsDto d)
        {
            var list = _unitOfWork.CaseDemographicsRespository.Get(p => p.ProfileId == d.ProfileId);
                       _unitOfWork.CaseDemographicsRespository.RemoveRange(list);

            var e = ObjectMapper.GetCWDemographicsEntity(d);
                      _unitOfWork.CaseDemographicsRespository.Insert(e);
                      _unitOfWork.SaveChanges();

        }
        #endregion

        #region Education Background

        public EducationBackgroundDto GetEducationBackgroundById(int profileId)
        {
            EducationBackground profile = _unitOfWork.EducationBackgroundRepository
                                           .GetUsingNoTracking(x => x.ProfileId == profileId)
                                           .FirstOrDefault();
            if (profile != null)
            { return ObjectMapper.GetEducationBackgroundDto(profile); }

            return null;
        }
        public async Task SaveEducationBackground(EducationBackgroundDto d)
        {
            var list = _unitOfWork.EducationBackgroundRepository
                                  .Get(p => p.ProfileId == d.ProfileId);
                        _unitOfWork.EducationBackgroundRepository.RemoveRange(list);

            var e = ObjectMapper.GetEducationBackgroundEntity(d);
                _unitOfWork.EducationBackgroundRepository.Insert(e);
                _unitOfWork.SaveChanges();
        }
        #endregion

        #region Job Intentions

        public JobIntentionsDto GetJobIntentionsByProfileId(int profileId)
        {
            JobIntentions profile = _unitOfWork.JobIntentionsRepository
                                            .GetUsingNoTracking(x => x.ProfileId == profileId)
                                            .FirstOrDefault();

            if (profile != null)
            {
                return ObjectMapper.GetJobIntentionsDto(profile);
            }
            return null;

        }
        public async Task SaveJobIntentions(JobIntentionsDto d)
        {
            var list = _unitOfWork.JobIntentionsRepository.Get(p => p.ProfileId == d.ProfileId);
                    _unitOfWork.JobIntentionsRepository.RemoveRange(list);

            var e = ObjectMapper.GetJobIntentionsEntity(d);
                        _unitOfWork.JobIntentionsRepository.Insert(e);
                        _unitOfWork.SaveChanges();
        }
        #endregion

        #region Feedback

        public void AddCaseWorkersFeedback(CaseWorkersFeedbackDto r)
        {
            var e = ObjectMapper.GetCaseWorkersFeedback(r);
                _unitOfWork.CaseWorkersFeedBackRepository.Insert(e);
        }
        public void SaveCaseWorkersFeedback()
        { _unitOfWork.SaveChanges(); }

        #endregion


        public async Task Save_CaseWorkersErrorLogs(MyDayErrorLogs_Dto myday)
        {
            var list = _unitOfWork.Mydayerrorlogs_Respository.Get(p => p.ProfileId == myday.ProfileId);
        
            var e = ObjectMapper.GetMyDayErroLogs_Entity(myday);
                    _unitOfWork.Mydayerrorlogs_Respository.Insert(e);
                    _unitOfWork.SaveChanges();
        }

        public SurveyDto GetSurveyByUid(string uid)
        {
            var s = _unitOfWork.SurveyRespository
                .GetUsingNoTracking(m => m.Uid == uid)
                .FirstOrDefault();

            if (s != null)
            {
                return ObjectMapper.GetSurveyDto(s);
            }
            return null;
        }

        public ProfileRosterDto GetRosterSession(int rosterId)
        {
            ProfileRosterDto r = this.GetRosterByRosterId(rosterId);

            if (r != null) return r;

            return null;
        }
        public ProfileRosterDto GetRosterByRosterId(int rosterId)
        {
            var r = _unitOfWork.ProfileRosterRespository.GetByID(rosterId);

            if (r != null)
            {
                return ObjectMapper.GetProfileRosterDto(r);
            }
            return null;
        }
        public IList<TaskVM> GetAllTask()
        {
            var list = _unitOfWork.TaskItemRespository
               .Get()
               .Select(x => new TaskVM()
               {
                   ID = x.Id,
                   Name = x.ShortName,
                   LongName = x.LongName,
                   Frequency = null
               }).ToList();

            return list;
        }
        public SurveyDto ResolveSurvey(SurveyDto s)
        {
            if (s != null)
            {
                if (!s.CurrTaskStartTime.HasValue)
                {
                    s.CurrTaskStartTime = s.SurveyWindowStartDateTime;

                    DateTime st = s.SurveyWindowStartDateTime.Value;
                    DateTime et = s.SurveyWindowEndDateTime.Value;
                    TimeSpan sp = et.Subtract(st);

                    int sDur = (int)Math.Round(sp.TotalMinutes);
                    s.RemainingDuration = sDur;
                    s.FirstQuestion = true;
                }

                return s;

            }

            return null;
        }
        public SurveyDto GetSurveySession(int surveyId)
        {
            SurveyDto s = this.GetSurveyById(surveyId);

            if (s != null) return s;

            return null;
        }
        public SurveyDto GetSurveyById(int id)
        {
            Survey s = _unitOfWork.SurveyRespository.GetByID(id);
            if (s != null)
            {
                return ObjectMapper.GetSurveyDto(s);
            }
            return null;
        }
        public async Task RemovePreviousResponses(int surveyId)
        {
            await Task.Run(() =>
            {
                var list = _unitOfWork.ResponsesRespository.Get(p => p.SurveyId == surveyId);
                _unitOfWork.ResponsesRespository.RemoveRange(list);
                _unitOfWork.SaveChanges();
            });
        }
        public async Task SetShitTimeSettingsAsyncGET(
            int surveyId,
            int profileId,
            string baseUrl)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            CurrentProfile currProfile = GetProfile(profileId.ToString(), null, null, null);
            
            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);

            var e = _unitOfWork.ProfileRespository.GetByID(profileId);
            int? createSurveyJobId = null;

            ProfileRosterDto rosterDto = new ProfileRosterDto();
            rosterDto.Id = tempSurvey.RosterItemId.HasValue ? tempSurvey.RosterItemId.Value : 0;

            //ExpiringSoonNotCompleted Email Reminder
            var expiringSoonNotCompleted = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository
                .GetUsingNoTracking(x => x.ProfileRosterId == tempSurvey.RosterItemId.Value);

            if (expiringSoonNotCompleted.Count() <= 0)
            {

                createSurveyJobId = await jobService.CreateJobAsync(JobName.ExpiringSoonRecurrentSurveyNotCompletedEmail.ToString(),
                    JobType.Method.ToString(), profileId,
                    JobMethod.Auto.ToString(), baseUrl, string.Empty, currProfile, rosterDto, tempSurvey
                  );

            }

            //var tempSurvey = new Survey() { Id = surveyId };
            tempSurvey.MaxStep = 1; //RoundNumber
            tempSurvey.SurveyUserStartDateTimeUtc = DateTime.UtcNow; //RoundNumber
            tempSurvey.SurveyUserCompletedDateTimeUtc = null; //RoundNumber
            tempSurvey.CurrTaskStartTime = null; //RoundNumber
            tempSurvey.CurrTaskEndTime = null; //RoundNumber
            tempSurvey.NextTaskStartTime = null; //RoundNumber
            tempSurvey.RemainingDuration = 0; //RoundNumber
            tempSurvey.FirstQuestion = true; //RoundNumber
            tempSurvey.CurrTask = 0; //RoundNumber
            tempSurvey.AddTaskId = null;
            tempSurvey.IsOnCall = null;

            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.Tasks.ToString();
            

            _unitOfWork.SaveChanges();
        }
        private CurrentProfile GetProfile(string profileId, string profileEmail, string profileOffset, string profileName)
        {
            bool retFromDB = false;
            if (string.IsNullOrEmpty(profileId) || string.IsNullOrEmpty(profileEmail) || string.IsNullOrEmpty(profileOffset) || string.IsNullOrEmpty(profileName))
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
            }

            return currProfile;
        }
        public async Task SetShitTimeSettings(int surveyId, string isOnCall, string wasWorking)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            if (wasWorking == "Yes")
            { tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.Tasks.ToString(); }
            else { tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.Completed.ToString(); }
            tempSurvey.IsOnCall = isOnCall;
            tempSurvey.WasWorking = wasWorking;

            _unitOfWork.SaveChanges();
        }
        #region Private Methods for Scheduler Jobs

        private async Task RemoveCompleteSurveyJob(int surveyId)
        {
            var startSurveyReminders = _unitOfWork.JobLogCompleteSurveyReminderEmailRespository
                .GetUsingNoTracking(x => x.SurveyId == surveyId);
            foreach (var k in startSurveyReminders)
            {
                if (k.HangfireJobId.HasValue)
                {
                    await schedulerService.DeleteScheduledJob(k.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogCompleteSurveyReminderEmailRespository.RemoveRange(startSurveyReminders);

        }
        private async Task RemoveExpiringSoonNotStartedJob(int surveyId)
        {
            var expiringSoonNotStarted = _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository
              .GetUsingNoTracking(x => x.SurveyId == surveyId);
            foreach (var k in expiringSoonNotStarted)
            {
                if (k.HangfireJobId.HasValue)
                {
                    await schedulerService.DeleteScheduledJob(k.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogExpiringSoonSurveyNotStartedReminderEmailRespository.RemoveRange(expiringSoonNotStarted);
        }
        private async Task RemoveExpiringSoonNotCompletedJob(int surveyId)
        {
            var expiringSoonNotCompleted = _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository
              .GetUsingNoTracking(x => x.SurveyId == surveyId);
            foreach (var k in expiringSoonNotCompleted)
            {
                if (k.HangfireJobId.HasValue)
                {
                    await schedulerService.DeleteScheduledJob(k.HangfireJobId.Value.ToString());
                }
            }
            _unitOfWork.JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository.RemoveRange(expiringSoonNotCompleted);
        }

        #endregion

        public async Task SetShitTimeSettingsAsyncPOST(int surveyId, string isOnCall, string clientInitials)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);           
            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.Tasks.ToString();            
            tempSurvey.IsOnCall = isOnCall;

            _unitOfWork.SaveChanges();
        }
        public IList<ProfileTaskItemDto> GetProfileTaksByTaskId(int profileId)
        {
            var list = _unitOfWork.ProfileTasksRespository
                        .Get(t => t.ProfileId == profileId)
                        .Select(t => new ProfileTaskItemDto()
                        {
                            Id = t.Id,
                            ProfileId = t.ProfileId,
                            TaskItemId = t.TaskItemId,
                            Frequency = t.Frequency
                        }).ToList();
            return list;
        }
        public IList<TaskVM> GetAllTaskByType(string type)
        {
            var list = _unitOfWork.TaskItemRespository
               .Get(x => x.Type == type)
               .Select(x => new TaskVM()
               {
                   ID = x.Id,
                   Name = x.ShortName,
                   LongName = x.LongName,
                   Frequency = null
               }).ToList();

            return list;
        }
        public TaskItemDto GetOtherTask()
        {
            TaskItem s = _unitOfWork.TaskItemRespository
                .GetUsingNoTracking(t => t.OtherTaskIndicator == true)
                .FirstOrDefault();
            if (s != null)
            {
                return ObjectMapper.GetTaskItemDto(s);
            }
            return null;
        }
        public TaskItemDto GetTaskByTaskId(int id)
        {
            TaskItem s = _unitOfWork.TaskItemRespository.GetByID(id);
            if (s != null)
            {
                return ObjectMapper.GetTaskItemDto(s);
            }
            return null;
        }
        public async Task SetWAMTasksAsyncPOST(int surveyId,
           int currRound, int remainingDuration,
           DateTime? currTaskEndTime, DateTime? currTaskStartTime,
           int currTask, DateTime? nextTaskStartTime,
           bool firstQuestion)
        {

            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.TaskTime.ToString();

            _unitOfWork.SaveChanges();
        }

        public int CalProgressValTaskTime(int prev, bool firstRound)
        {
            if (firstRound)
            {
                prev = 25;
                return prev;
            }

            return 0;
        }

        public IList<WAMTaskTimeOptions> GetWAMTaskTimeOptions()
        {
            string option1 = "I was alone";
            string option2 = "Colleague from my team";
            string option3 = "Colleague from another team";
            string option4 = "Supervisor";
            string option5 = "Client";
            string option6 = "Other";

            string tooltip = "";

            WAMTaskTimeOptions option1o = new WAMTaskTimeOptions { ID = 0, Name = option1, LongName = null, Ans = null };
            WAMTaskTimeOptions option2o = new WAMTaskTimeOptions { ID = 1, Name = option2, LongName = null, Ans = null };
            WAMTaskTimeOptions option3o = new WAMTaskTimeOptions { ID = 2, Name = option3, LongName = null, Ans = null };
            WAMTaskTimeOptions option4o = new WAMTaskTimeOptions { ID = 3, Name = option4, LongName = null, Ans = null };
            WAMTaskTimeOptions option5o = new WAMTaskTimeOptions { ID = 4, Name = option5, LongName = null, Ans = null };
            WAMTaskTimeOptions option6o = new WAMTaskTimeOptions { ID = 5, Name = option6, LongName = null, Ans = null };

            List<WAMTaskTimeOptions> list = new List<WAMTaskTimeOptions>();
            list.Add(option1o);
            list.Add(option2o);
            list.Add(option3o);
            list.Add(option4o);
            list.Add(option5o);
            list.Add(option6o);

            return list;
        }
        public async Task AddWAMResponseAsync(WAMResponseDto r)
        {
            //
            var e = ObjectMapper.GetWAMResponseEntity(r);
            _unitOfWork.WAMResponsesRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public async Task SetWAMTaskTimeAsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;

            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.TaskRating1.ToString();

            _unitOfWork.SaveChanges();
        }
        public bool SaveWAMWithWhom(int profileID, int surveyId, int taskId, IList<WAMTaskTimeOptions> selectedOptions,
                        string nearestLocation, string nearestOtherLocation)
        {
            var list = _unitOfWork.WAMWithWhomRepository.Get(p => p.ProfileId == profileID
                                                                && p.SurveyId == surveyId
                                                                && p.TaskId == taskId);
            if (list.Count() == 0)
            {
                foreach (var r in selectedOptions)
                {
                    if (!string.IsNullOrEmpty(r.ID.ToString()))
                    {
                        var withWHom = new WAMWithWhomTaskTime
                        {
                            ProfileId = profileID,
                            SurveyId = surveyId,
                            TaskId = taskId,
                            QuestionId = r.ID,
                            WithWhomOptions = r.Name,
                            WithWhomOther = r.LongName,
                            NearestLocation = nearestLocation,
                            NearestOtherLocation = nearestOtherLocation
                        };
                        _unitOfWork.WAMWithWhomRepository.Insert(withWHom);
                        _unitOfWork.SaveChanges();
                    }
                }
                return true;
            }
            else { return false; }
        }
        public int CalProgressValRating1(int prev, decimal allocationSpanPercentage, int numRounds, bool resetGoto, decimal remainingLength)
        {
            if (!resetGoto)
            {

                decimal length = remainingLength * (allocationSpanPercentage / numRounds); //num of rounds added to 
                int lengthInt = (int)length;
                prev += lengthInt;
            }
            return prev;
        }
        public void AddWAMResponse(WAMResponseDto r)
        {
            var e = ObjectMapper.GetWAMResponseEntity(r);
            _unitOfWork.WAMResponsesRespository.Insert(e);
        }
        public void SaveResponse()
        {
            _unitOfWork.SaveChanges();
        }
        public async Task SetWAMRating1AsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.TaskRating2.ToString();
            _unitOfWork.SaveChanges();
        }
        public int CalProgressValRating2(int prev, decimal allocationSpanPercentage, int numRounds, bool resetGoto, decimal remainingLength)
        {
            if (!resetGoto)
            {
                decimal length = remainingLength * (allocationSpanPercentage / numRounds);

                int lengthInt = (int)length;
                prev += lengthInt;
            }
            return prev;
        }
        public async Task SetWAMRating2AsyncPOSTResult(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);
            RemoveExpiringSoonNotCompletedJob(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.AddShiftTime.ToString();
            _unitOfWork.SaveChanges();
        }
        public async Task SetWAMRating2AsyncPOSTNext(int surveyId,
              int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = Constants.CaseWorkersSurveyProgress.Tasks.ToString();

            _unitOfWork.SaveChanges();
        }
        //Additional
        public int CalProgressValAddTasks(int prev, bool firstRound)
        {
            if (firstRound)
            {
                prev = 83;
                return prev;
            }

            return 0;
        }
        public async Task SetAdditionalTasksAsyncPOST(int surveyId,
         int currRound, int remainingDuration,
         DateTime? currTaskEndTime, DateTime? currTaskStartTime,
         int currTask, DateTime? nextTaskStartTime,
         bool firstQuestion, string nextStatus, int addTaskId)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            //Remove Reminders
            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);
            RemoveExpiringSoonNotCompletedJob(surveyId);
            
            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.AddTaskId = addTaskId;

            tempSurvey.SurveyProgressNext = nextStatus;

            _unitOfWork.SaveChanges();

        }
        public int CalProgressValAddTaskTime(int prev, bool firstRound)
        {
            if (firstRound)
            {
                prev = 85;
                return prev;
            }

            return 0;
        }
        public int CalProgressValAddRating1(int prev, decimal percentageToRoll)
        {

            if (percentageToRoll == 1)
            {
                prev = 90;
                return prev;
            }
            return 0;
        }
        public int CalProgressValAddRating2(int prev, decimal percentageToRoll)
        {
            if (percentageToRoll == 1)
            {
                prev = 95;
                return prev;
            }
            return 0;
        }
        public async Task SetAdditionalAsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion, string nextStatus)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            
            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);
            RemoveExpiringSoonNotCompletedJob(surveyId);
            
            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = nextStatus;

            if (nextStatus == Constants.CaseWorkersSurveyProgress.Completed.ToString())
            {
                tempSurvey.SurveyUserCompletedDateTimeUtc = DateTime.UtcNow;
            }
            _unitOfWork.SaveChanges();
        }
        public async Task SetFeedbackAsyncPOST(int surveyId,
        string nextStatus, string comment)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            tempSurvey.Comment = comment;
            tempSurvey.SurveyProgressNext = nextStatus;
            _unitOfWork.SaveChanges();

        }
        public virtual IList<WAMResponse> GetGenericWAMResponses(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<WAMResponse> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<WAMResponse>;
            }
            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.WAMResponsesRespository.GetUsingNoTracking(i => (i.ProfileId == profileId
                        && i.SurveyId == surveyId
                        && i.IsOtherTask == false)).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public virtual IList<WAMResponse> GetAdditionalWAMResponses(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<WAMResponse> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<WAMResponse>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.WAMResponsesRespository.GetUsingNoTracking(i => (i.ProfileId == profileId
                                                                        && i.SurveyId == surveyId
                                                                        && i.IsOtherTask == true)).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public string GetRatingString(string answer)
        {

            switch (answer)
            {
                case "Not at all":
                    return "fa fa-thermometer-empty fa-2x";
                case "Slightly":
                    return "fa fa-thermometer-quarter fa-2x";
                case "Somewhat":
                    return "fa fa-thermometer-quarter fa-2x";
                case "Moderately":
                    return "fa fa-thermometer-half fa-2x";
                case "Strongly":
                    return "fa fa-thermometer-three-quarters fa-2x";
                case "Very Strongly":
                    return "fa fa-thermometer-three-quarters fa-2x";
                case "Extremely":
                    return "fa fa-thermometer-full fa-2x";
                case "N/A":
                    return "fa fa-ban fa-2x";
                default:
                    return "fa fa-ban fa-2x";
            }
        }
        public virtual IList<WAMResponse> GetAllWAMResponseAsync(int taskId, int profileId, int surveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<WAMResponse> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<WAMResponse>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.WAMResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                            && i.SurveyId == surveyId
                            && i.TaskId == taskId
                            && i.TaskStartDateTime == taskStartDate)).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public void UpdateWAMResponse(WAMResponse r)
        {
            _unitOfWork.WAMResponsesRespository.Update(r);
        }
        public virtual IList<WAMResponse> GetWAMAdditionalResponseAsync(int taskId, int profileId, int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<WAMResponse> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<WAMResponse>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.WAMResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                            && i.SurveyId == surveyId
                            && i.TaskId == taskId
                            && i.IsOtherTask == true)).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public async Task SetWAMAdditionalAsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion, string nextStatus)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            //Remove Reminders
            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);
            RemoveExpiringSoonNotCompletedJob(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = nextStatus;

            if (nextStatus == Constants.StatusSurveyProgress.Completed.ToString())
            {
                tempSurvey.SurveyUserCompletedDateTimeUtc = DateTime.UtcNow;
            }
            _unitOfWork.SaveChanges();
        }
        
    }
}