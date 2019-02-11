using Hangfire;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.BLL
{
    public class Survey3Service
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static bool UpdateDatabase = true;


        private HangfireScheduler schedulerService;
        private JobService jobService;
        private ProfileService profileService;



        public Survey3Service()
        {
            this.schedulerService = new HangfireScheduler();
            this.jobService = new JobService();
            this.profileService = new ProfileService();

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
        public SurveyDto GetSurveySession(int surveyId)
        {
            SurveyDto s = this.GetSurveyById(surveyId);

            if (s != null) return s;

            return null;
        }
        public ProfileRosterDto GetRosterSession(int rosterId)
        {
            ProfileRosterDto r = this.GetRosterByRosterId(rosterId);

            if (r != null) return r;

            return null;
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

                    int sDur = (int) Math.Round(sp.TotalMinutes);
                    s.RemainingDuration = sDur;
                    s.FirstQuestion = true;
                }

                return s;

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


        public async Task SetShitTimeSettingsAsyncGET(
            int surveyId,
            int profileId,
            string baseUrl)
        {

            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            CurrentProfile currProfile = GetProfile(profileId.ToString(), null, null, null);

            //await RemoveCompleteSurveyJob(surveyId);
            //await RemoveExpiringSoonNotStartedJob(surveyId);

            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);


            var e = _unitOfWork.ProfileRespository.GetByID(profileId);
            int? createSurveyJobId = null;

            ProfileRosterDto rosterDto = new ProfileRosterDto();
            rosterDto.Id = tempSurvey.RosterItemId.HasValue?tempSurvey.RosterItemId.Value:0;


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

            if (currProfile.ClientInitials.ToLower().ToString() == "jd") //For Junior doctors
            {
                tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.ShiftTime.ToString(); //this is to ensure that the status is updated due to a refresh
            }
            else if (currProfile.ClientInitials.ToLower().ToString() == "wam") //For Warren and Mahony
            {
                tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WAMTasks.ToString();
            }

                _unitOfWork.SaveChanges();
        }
        public async Task SetShitTimeSettings(int surveyId, string isOnCall, string wasWorking)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            if (wasWorking == "Yes")
            {tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.Tasks.ToString();}
            else { tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.Completed.ToString(); }
            tempSurvey.IsOnCall = isOnCall;
            tempSurvey.WasWorking = wasWorking;

            _unitOfWork.SaveChanges();
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
       
        public async Task SetShitTimeSettingsAsyncPOST(int surveyId, string isOnCall, string clientInitials)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            if (clientInitials.ToLower().ToString() == "jd")
            {
                tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.Tasks.ToString();
            }
            else if (clientInitials.ToLower().ToString() == "wam")
            {
                tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WAMTasks.ToString();
            }
            tempSurvey.IsOnCall = isOnCall;

            _unitOfWork.SaveChanges();
        }

        public async Task SetSummaryAsyncPOST(int surveyId)
        {
            //This section is never called currently due to ajax redirecting to Home
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.Tasks.ToString();
            tempSurvey.SurveyUserCompletedDateTimeUtc = DateTime.UtcNow;
            _unitOfWork.SaveChanges();
        }



        public async Task SetWRSettingsAsyncPOST(string nextStatus, int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion,
            int? WRCurrTaskId,
            string WRCurrTasksIds,
            int? WRRemainingDuration,
            DateTime? WRCurrTaskStartTime,
            DateTime? WRCurrTaskEndTime,
            DateTime? WRNextTaskStartTime
            )
        {

            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = nextStatus;
            tempSurvey.WRCurrTaskId = WRCurrTaskId;
            tempSurvey.WRCurrTasksId = WRCurrTasksIds;
            tempSurvey.WRRemainingDuration = WRRemainingDuration;
            tempSurvey.WRCurrTaskStartTime = WRCurrTaskStartTime;
            tempSurvey.WRCurrTaskEndTime = WRCurrTaskEndTime;
            tempSurvey.WRNextTaskStartTime = WRNextTaskStartTime;

            _unitOfWork.SaveChanges();
        }


        public async Task SetWRSettingsAsyncPOST(string nextStatus, int surveyId,
          int currRound, int remainingDuration,
          DateTime? currTaskEndTime, DateTime? currTaskStartTime,
          int currTask, DateTime? nextTaskStartTime,
          bool firstQuestion,
          int? WRCurrTaskId,
          string WRCurrTasksIds,
          int? WRRemainingDuration,
          DateTime? WRCurrTaskStartTime,
          DateTime? WRCurrTaskEndTime,
          DateTime? WRNextTaskStartTime,
          DateTime? WRWindowStart,
          DateTime? WRWindowEnd
          )
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            tempSurvey.SurveyProgressNext = nextStatus;
            tempSurvey.WRCurrTaskId = WRCurrTaskId;
            tempSurvey.WRCurrTasksId = WRCurrTasksIds;
            tempSurvey.WRRemainingDuration = WRRemainingDuration;
            tempSurvey.WRCurrTaskStartTime = WRCurrTaskStartTime;
            tempSurvey.WRCurrTaskEndTime = WRCurrTaskEndTime;
            tempSurvey.WRNextTaskStartTime = WRNextTaskStartTime;
            tempSurvey.WRCurrWindowStartTime = WRWindowStart;
            tempSurvey.WRCurrWindowEndTime = WRWindowEnd;

            _unitOfWork.SaveChanges();
        }


        public async Task SetTaskTimeAsyncPOST(int surveyId,
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

            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.TaskRating1.ToString();

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

            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WAMTaskRating1.ToString();

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
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WAMTasks.ToString();

            _unitOfWork.SaveChanges();
        }
        public async Task SetRating2AsyncPOSTNext(int surveyId,
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
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.Tasks.ToString();

            _unitOfWork.SaveChanges();
        }

        public async Task SetWAMRating2AsyncPOSTResult(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            //Remove Reminders
            //await RemoveCompleteSurveyJob(surveyId);
            //await RemoveExpiringSoonNotStartedJob(surveyId);
            //await RemoveExpiringSoonNotCompletedJob(surveyId);

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
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.AddShiftTime.ToString();
            _unitOfWork.SaveChanges();
        }
        //Goto Additional Quiz
        public async Task SetRating2AsyncPOSTResult(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion)
        {

            //var tempSurvey = db.Surveys.Where(x => x.Id == surveyId)
            //    .SingleOrDefault();

            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            //Remove Reminders
            //await RemoveCompleteSurveyJob(surveyId);
            //await RemoveExpiringSoonNotStartedJob(surveyId);
            //await RemoveExpiringSoonNotCompletedJob(surveyId);

            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);
            RemoveExpiringSoonNotCompletedJob(surveyId);

            //var survey = db.Surveys.Where(x => x.Id == surveyId).Select(x => new { MaxStep = x.MaxStep }).SingleOrDefault();
            //var tempSurvey = new Survey() { Id = surveyId 
            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;
            //tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.Completed.ToString();
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.AddShiftTime.ToString();
            //tempSurvey.SurveyUserCompletedDateTimeUtc = DateTime.UtcNow;
            _unitOfWork.SaveChanges();

        }
        public async Task SetWAMAdditionalAsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion, string nextStatus)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            //Remove Reminders

            //Task.Run(() => RemoveCompleteSurveyJob(surveyId)).Wait();
            //await RemoveCompleteSurveyJob(surveyId);
            //await RemoveExpiringSoonNotStartedJob(surveyId);
            //await RemoveExpiringSoonNotCompletedJob(surveyId);

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




        public async Task SetAdditionalAsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion, string nextStatus)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            //Remove Reminders
            //await RemoveCompleteSurveyJob(surveyId);
            //await RemoveExpiringSoonNotStartedJob(surveyId);
            //await RemoveExpiringSoonNotCompletedJob(surveyId);

            RemoveCompleteSurveyJob(surveyId);
            RemoveExpiringSoonNotStartedJob(surveyId);
            RemoveExpiringSoonNotCompletedJob(surveyId);

            //var survey = db.Surveys.Where(x => x.Id == surveyId).Select(x => new { MaxStep = x.MaxStep }).SingleOrDefault();
            //var tempSurvey = new Survey() { Id = surveyId 

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
                     
            //var survey = db.Surveys.Where(x => x.Id == surveyId).Select(x => new { MaxStep = x.MaxStep }).SingleOrDefault();
            //var tempSurvey = new Survey() { Id = surveyId 

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



        public async Task SetFeedbackAsyncPOST(int surveyId,
        string nextStatus, string comment)
        {
            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            tempSurvey.Comment = comment;
            tempSurvey.SurveyProgressNext = nextStatus;
            _unitOfWork.SaveChanges();

        }

        


        public async Task SetTasksAsyncPOST(int surveyId,
            int currRound, int remainingDuration,
            DateTime? currTaskEndTime, DateTime? currTaskStartTime,
            int currTask, DateTime? nextTaskStartTime,
            bool firstQuestion, bool IsWardRound)
        {

            var tempSurvey = _unitOfWork.SurveyRespository.GetByID(surveyId);

            tempSurvey.MaxStep = currRound;
            tempSurvey.RemainingDuration = remainingDuration;
            tempSurvey.CurrTaskEndTime = currTaskEndTime;
            tempSurvey.CurrTaskStartTime = currTaskStartTime;
            tempSurvey.CurrTask = currTask;
            tempSurvey.NextTaskStartTime = nextTaskStartTime;
            tempSurvey.FirstQuestion = firstQuestion;

            if (IsWardRound)
                tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WRTaskTime.ToString();
            else
                tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.TaskTime.ToString();

            _unitOfWork.SaveChanges();
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
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WAMTaskTime.ToString();

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
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.WAMTaskRating2.ToString();
            _unitOfWork.SaveChanges();
        }
        public async Task SetRating1AsyncPOST(int surveyId,
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
            tempSurvey.SurveyProgressNext = Constants.StatusSurveyProgress.TaskRating2.ToString();
            _unitOfWork.SaveChanges();
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
                   //IsWardRoundIndicator = x.WardRoundIndicator
               }).ToList();

            return list;
        }
        public IList<TaskVM> GetAllTaskSpecific(int[] selectedTasks)
        {
            var list = _unitOfWork.TaskItemRespository
               .GetUsingNoTracking(x => selectedTasks.Contains(x.Id))
               .Select(x => new TaskVM()
               {
                   ID = x.Id,
                   Name = x.ShortName,
                   LongName = x.LongName,
                   Frequency = null
               }).ToList();

            return list;
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
        //public MyDayTaskList DeleteTaskById(int taskId, int profileId)
        //{
        //    var addedtaskId = _unitOfWork.MyDayTasksRespository.GetUsingNoTracking(x => x.Id == taskId && x.ProfileId == profileId);            
        //    _unitOfWork.MyDayTasksRespository.Delete(addedtaskId);
        //    _unitOfWork.SaveChanges();
        //    return null;
        //}
        //public MyDayTaskListDto GetMyDayTaskById(int id)
        //{
        //    MyDayTaskList s = _unitOfWork.MyDayTasksRespository.GetByID(id);
        //    if (s != null)
        //    { return ObjectMapper.GetMyDayTaskItemDto(s); }
        //    return null;
        //}
        public TaskItemDto GetTaskByTaskName(string name)
        {
            TaskItem s = _unitOfWork.TaskItemRespository
                .GetUsingNoTracking(t => t.ShortName == name)
                .FirstOrDefault();
            if (s != null)
            {
                return ObjectMapper.GetTaskItemDto(s);
            }
            return null;
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
        //public IList<TaskItem> GetTasksNotinSelectedMyDayTasks(string type, int profileId)
        //{
        //    var mydaytasklist = _unitOfWork.MyDayTasksRespository
        //        .Get(x => x.ProfileId == profileId)
        //        .Select(x => new MyDayTaskList()
        //        {
        //            Id = x.Id,
        //            TaskName = x.TaskName
        //        }).ToList();

        //    foreach (var i in mydaytasklist) {
        //        var list = _unitOfWork.TaskItemRespository
        //            .Get(y => y.Id != mydaytasklist[i].Id)
        //            .Select(y => new TaskItem()
        //            {
        //                Id = y.Id,
        //                ShortName = y.ShortName
        //            });
        //        }

        //    return list; }
        public List<TaskItem> GetAvailableTasks(string type, int profileId)
        {
            var list = _unitOfWork.TaskItemRespository
               .Get(x => x.Type == type)               
               .Select(x => new TaskItem()
               {
                   Id = x.Id,
                   ShortName = x.ShortName,
                   LongName = x.LongName
               }).ToList();

            return list;
        }
        public IList<MyDayTaskList> GetAllTaskByProfileID(int ProfileId)
        {
            var list = _unitOfWork.MultiTaskListRepository
               .Get(x => x.ProfileId == ProfileId)
               .Select(x => new MyDayTaskList()
               {
                   Id = x.Id,
                   ProfileId = x.ProfileId,
                   SurveyId = x.SurveyId,
                   TaskId = x.TaskId,
                   TaskStartDateCurrentTime = x.TaskStartDateCurrentTime,
                   TaskEndDateCurrentTime = x.TaskEndDateCurrentTime,
                   TaskStartDateTimeUtc = x.TaskStartDateTimeUtc,
                   TaskEndDateTimeUtc = x.TaskEndDateTimeUtc,
                   TaskName = x.TaskName,
                   TaskCategoryId = x.TaskCategoryId,
                   TaskDuration = x.TaskDuration,
                   IsRandomlySelected = x.IsRandomlySelected,
                   IsAffectStageCompleted = x.IsAffectStageCompleted
               }).ToList();

            return list;
        }

        public List<MyDayTaskList> GetRandomlySelectedTasksByProfileID(int ProfileId, int surveyId)
        {
            var list = _unitOfWork.MultiTaskListRepository
               .Get(x => x.ProfileId == ProfileId && x.SurveyId == surveyId)
               .Where(x => x.IsRandomlySelected == true && x.IsAffectStageCompleted == false)
               .Select(x => new MyDayTaskList()
               {
                   Id = x.Id,
                   ProfileId = x.ProfileId,
                   SurveyId = x.SurveyId,
                   TaskId = x.TaskId,
                   TaskStartDateCurrentTime = x.TaskStartDateCurrentTime,
                   TaskEndDateCurrentTime = x.TaskEndDateCurrentTime,
                   TaskStartDateTimeUtc = x.TaskStartDateTimeUtc,
                   TaskEndDateTimeUtc = x.TaskEndDateTimeUtc,
                   TaskName = x.TaskName,
                   TaskCategoryId = x.TaskCategoryId,
                   TaskDuration = x.TaskDuration,
                   IsRandomlySelected = x.IsRandomlySelected,
                   IsAffectStageCompleted = x.IsAffectStageCompleted
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
        public IList<TaskItemDto> GetWardRoundTasks()
        {
            var list = _unitOfWork.TaskItemRespository
                       .GetUsingNoTracking(t => t.IsWardRoundTask == true)
                       .Select(t => new TaskItemDto()
                       {
                           Id = t.Id,
                           ShortName = t.ShortName,
                           LongName = t.LongName,
                           Sequence = t.Sequence,
                           Type = t.Type,
                           OtherTaskIndicator = t.OtherTaskIndicator,
                           WardRoundIndicator = t.WardRoundIndicator,
                           IsWardRoundTask = t.IsWardRoundTask
                       }).ToList();
            return list;
        }
        public virtual IList<ResponseAffects> GetGenericResponseAffects(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<ResponseAffects> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<ResponseAffects>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.ResponseAffectRespository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.SurveyId == surveyId
                        && i.IsOtherTask == false)
                        ).ToList();
                }
                catch (Exception) { throw; }
                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
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
        public virtual IList<WAMResponse> GetWAMGenericResponses(int surveyId, int profileId)
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
                    result = _unitOfWork.WAMResponsesRespository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId && i.SurveyId == surveyId
                        && i.IsOtherTask == false)
                        ).ToList();
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

        
        public virtual IList<Response> GetGenericResponses(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.SurveyId == surveyId
                        && i.IsOtherTask == false
                        && i.WardRoundTaskId == null
                        && (i.IsWardRoundTask != true || i.IsWardRoundTask == null))
                        ).ToList();
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
        public virtual IList<Response> GetAdditionalResponses(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId && i.SurveyId == surveyId && i.IsOtherTask == true)
                        ).ToList();
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
        public virtual IList<Response> GetWRResponses(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                        i => (i.ProfileId == profileId
                        && i.SurveyId == surveyId
                        && i.IsOtherTask == false
                        && i.WardRoundTaskId != null)
                        ).ToList();
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
        public async virtual Task<IList<Response>> GetAllResponseAsync(int surveyId, int profileId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                        i => (
                        i.ProfileId == profileId
                        && i.SurveyId == surveyId
                        )
                        ).ToList();
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
        public virtual IList<Response> GetAllWRResponseAsync(int taskId, int profileId, int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {

                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.SurveyId == surveyId
                       && i.TaskId == taskId
                       && i.WardRoundTaskId == taskId
                       )
                       ).ToList();
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
        public virtual IList<ResponseAffects> GetAllResponseAffectAsync(int taskId, int profileId, int surveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<ResponseAffects> result = null;

            if (!IsWebApiRequest)
            { result = HttpContext.Current.Session["SurveyResponses"] as IList<ResponseAffects>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.ResponseAffectRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.SurveyId == surveyId
                       && i.TaskId == taskId                      
                       ) ).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                {HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
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
        public virtual IList<Response> GetAllResponseAsync(int taskId, int profileId, int surveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {

                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.SurveyId == surveyId
                       && i.TaskId == taskId
                       && i.TaskStartDateTime == taskStartDate
                       )
                       ).ToList();
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
        public virtual IList<Response> GetAllWRResponseAsync(int taskId, int profileId, int surveyId, DateTime taskStartDate)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {

                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.SurveyId == surveyId
                       && i.TaskId == taskId
                       && i.WardRoundTaskId == taskId
                       )
                       ).ToList();
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
        public virtual IList<WAMResponse> GetWAMAdditionalResponseAsync(int taskId, int profileId, int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<WAMResponse> result = null;

            if (!IsWebApiRequest)
            {  result = HttpContext.Current.Session["SurveyResponses"] as IList<WAMResponse>; }

            if (result == null || UpdateDatabase)
            {
                try
                {
                    result = _unitOfWork.WAMResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                            && i.SurveyId == surveyId
                            && i.TaskId == taskId
                            && i.IsOtherTask == true ) ).ToList();
                }
                catch (Exception)
                { throw; }

                if (!IsWebApiRequest)
                { HttpContext.Current.Session["SurveyResponses"] = result; }
            }
            return result;
        }
        public virtual IList<Response> GetAdditionalResponseAsync(int taskId,
            int profileId,
            int surveyId)
        {
            bool IsWebApiRequest = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
            IList<Response> result = null;

            if (!IsWebApiRequest)
            {
                result = HttpContext.Current.Session["SurveyResponses"] as IList<Response>;
            }

            if (result == null || UpdateDatabase)
            {
                try
                {

                    result = _unitOfWork.ResponsesRespository.GetUsingNoTracking(
                       i => (i.ProfileId == profileId
                       && i.SurveyId == surveyId
                       && i.TaskId == taskId
                       && i.IsOtherTask == true
                       )
                       ).ToList();
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
        public void UpdateWAMResponse(WAMResponse r)
        {
            _unitOfWork.WAMResponsesRespository.Update(r);
        }
        public void UpdateResponse(Response r)
        {
            _unitOfWork.ResponsesRespository.Update(r);
        }
        public void UpdateResponseAffect(ResponseAffects r)
        {
            _unitOfWork.ResponseAffectRespository.Update(r);
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
        public void AddWAMResponse(WAMResponseDto r)
        {
            var e = ObjectMapper.GetWAMResponseEntity(r);
            _unitOfWork.WAMResponsesRespository.Insert(e);
        }
        public async Task AddWAMResponseAsync(WAMResponseDto r)
        {
            //
            var e = ObjectMapper.GetWAMResponseEntity(r);
            _unitOfWork.WAMResponsesRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public async Task AddResponseAsync(ResponseDto r)
        {
            //
            var e = ObjectMapper.GetResponseEntity(r);
            _unitOfWork.ResponsesRespository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public void AddResponse(ResponseDto r)
        {
            var e = ObjectMapper.GetResponseEntity(r);
            _unitOfWork.ResponsesRespository.Insert(e);
        }
        public void SaveResponse()
        {
            _unitOfWork.SaveChanges();
        }
        public void AddResponseAffect(ResponseAffectDto r)
        {
            var e = ObjectMapper.GetResponseAffectEntity(r);
            _unitOfWork.ResponseAffectRespository.Insert(e);
        }
        public void SaveResponseAffect()
        {
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
        public int CalProgressValTaskTime(int prev, bool firstRound)
        {
            if (firstRound)
            {
                prev = 25;
                return prev;
            }

            return 0;
        }
        public int CalProgressValRating1(int prev, decimal allocationSpanPercentage, int numRounds, bool resetGoto, decimal remainingLength)
        {
            if (!resetGoto)
            {

                decimal length = remainingLength * (allocationSpanPercentage / numRounds); //num of rounds added to 
                int lengthInt = (int) length;
                prev += lengthInt;
            }
            return prev;
        }
        public int CalProgressValRating2(int prev, decimal allocationSpanPercentage, int numRounds, bool resetGoto, decimal remainingLength)
        {
            if (!resetGoto)
            {
                decimal length = remainingLength * (allocationSpanPercentage / numRounds);

                int lengthInt = (int) length;
                prev += lengthInt;
            }
            return prev;
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

        //Wardround
        public int CalProgressWRSubpages(int prev, decimal subpageRatio, bool resetGoto)
        {
            if (!resetGoto)
            {
                prev += (int) subpageRatio;
            }
            return prev;
        }


        public async Task CreateUserFeedback(int surveyId, int profileId, string baseUrl, EmailFeedbackViewModel v)
        {
            //add a record to db
            Feedback e = new Feedback();
            e.Channel = "Survey";
            e.CreatedDateTimeUtc = DateTime.UtcNow;
            e.Email = v.EmailAddress;
            e.ContactNumber = v.PhoneNumber;
            e.PreferedContact = v.PreferedContact;
            e.PreferedTime = v.PreferedTime;
            e.Message = v.Message;
            e.ProfileId = profileId;
            e.SurveyId = surveyId;
            _unitOfWork.FeedbackRespository.Insert(e);
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


        public void Dispose()
        {
            schedulerService.Dispose();
            jobService.Dispose();
            profileService.Dispose();
            _unitOfWork.Dispose();
        }

        public async Task Save_MyDayErrorLogs(MyDayErrorLogs_Dto myday)
        {
            var list = _unitOfWork.Mydayerrorlogs_Respository.Get(p => p.ProfileId == myday.ProfileId);
           // _unitOfWork.Mydayerrorlogs_Respository.RemoveRange(list);
            var e = ObjectMapper.GetMyDayErroLogs_Entity(myday);
            _unitOfWork.Mydayerrorlogs_Respository.Insert(e);
            _unitOfWork.SaveChanges();
        }
        public async Task DeleteMyDayTasksByProfileId(int profileId)
        {
            var list = _unitOfWork.MyDayTasksRespository.Get(p => p.ProfileId == profileId);
            _unitOfWork.MyDayTasksRespository.RemoveRange(list);
            _unitOfWork.SaveChanges();
        }

        ///For Warren and Mahony
        public ProfileDetailsByClient GetClientByProfileID(int profileId)
        {
            ProfileDetailsByClient currProfile = new ProfileDetailsByClient();
            currProfile = profileService.GetClientDetailsByProfileId(profileId);
            return currProfile;
        }
    }
}