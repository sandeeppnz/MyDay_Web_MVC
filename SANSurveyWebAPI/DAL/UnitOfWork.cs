using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SANSurveyWebAPI.DAL
{
    public class UnitOfWork : IDisposable
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        #region Custom DB Access
        public virtual IEnumerable<HangfireStateCustomDto> GetHanfireJobStates()
        {
            var jobs = db.Database.SqlQuery<HangfireStateCustomDto>(
                      "SELECT s.Id AS StateId,s.JobId,s.Name AS StateName,s.Reason, s.Data ,  j.Arguments AS JobArguments,j.CreatedAt,j.ExpireAt FROM [HangFire].[State] s LEFT JOIN[HangFire].[Job] j ON s.Id = j.StateId").ToList();


            return jobs;
        }
        public virtual List<HangfireStateDto> GetHangfireJobStates(int hangfireJobId)
        {
            //http://adicodes.com/entity-framework-raw-sql-queries-and-stored-procedure-execution/
            var param = new SqlParameter("@jobId", hangfireJobId);
            var jobs = db.Database.SqlQuery<HangfireStateDto>(
                      "SELECT * FROM [HangFire].[State] WHERE JobId = @jobId", param).ToList();

            return jobs;
        }

        public virtual void GetHangfireJobStates(int hangfireJobId, ref List<HangfireStateDto> states)
        {
            //http://adicodes.com/entity-framework-raw-sql-queries-and-stored-procedure-execution/
            var param = new SqlParameter("@jobId", hangfireJobId);
            var jobs = db.Database.SqlQuery<HangfireStateDto>(
                      "SELECT * FROM [HangFire].[State] WHERE JobId = @jobId", param).ToList();

            foreach (var j in jobs)
            {
                states.Add(j);
            }

        }

        #endregion



        private GenericRepository<JobLogExpiringSoonSurveyNotCompletedReminderEmail> jobLogExpiringSoonSurveyNotCompletedReminderEmail;
        public GenericRepository<JobLogExpiringSoonSurveyNotCompletedReminderEmail> JobLogExpiringSoonSurveyNotCompletedReminderEmailRespository
        {
            get
            {
                if (this.jobLogExpiringSoonSurveyNotCompletedReminderEmail == null)
                {
                    this.jobLogExpiringSoonSurveyNotCompletedReminderEmail = new GenericRepository<JobLogExpiringSoonSurveyNotCompletedReminderEmail>(db);
                }
                return jobLogExpiringSoonSurveyNotCompletedReminderEmail;
            }
        }

        private GenericRepository<JobLogExpiringSoonSurveyNotStartedReminderEmail> jobLogExpiringSoonSurveyNotStartedReminderEmail;
        public GenericRepository<JobLogExpiringSoonSurveyNotStartedReminderEmail> JobLogExpiringSoonSurveyNotStartedReminderEmailRespository
        {
            get
            {
                if (this.jobLogExpiringSoonSurveyNotStartedReminderEmail == null)
                {
                    this.jobLogExpiringSoonSurveyNotStartedReminderEmail = new GenericRepository<JobLogExpiringSoonSurveyNotStartedReminderEmail>(db);
                }
                return jobLogExpiringSoonSurveyNotStartedReminderEmail;
            }
        }




        private GenericRepository<JobLogCompleteSurveyReminderEmail> jobLogCompleteSurveyReminderEmails;
        public GenericRepository<JobLogCompleteSurveyReminderEmail> JobLogCompleteSurveyReminderEmailRespository
        {
            get
            {
                if (this.jobLogCompleteSurveyReminderEmails == null)
                {
                    this.jobLogCompleteSurveyReminderEmails = new GenericRepository<JobLogCompleteSurveyReminderEmail>(db);
                }
                return jobLogCompleteSurveyReminderEmails;
            }
        }



        private GenericRepository<JobLogStartSurveyReminderEmail> jobLogStartSurveyReminderEmail;
        public GenericRepository<JobLogStartSurveyReminderEmail> JobLogStartSurveyReminderEmailRespository
        {
            get
            {
                if (this.jobLogStartSurveyReminderEmail == null)
                {
                    this.jobLogStartSurveyReminderEmail = new GenericRepository<JobLogStartSurveyReminderEmail>(db);
                }
                return jobLogStartSurveyReminderEmail;
            }
        }

        private GenericRepository<JobLogUpdateSurvey> jobLogUpdateSurvey;
        public GenericRepository<JobLogUpdateSurvey> JobLogUpdateSurveyRespository
        {
            get
            {
                if (this.jobLogUpdateSurvey == null)
                {
                    this.jobLogUpdateSurvey = new GenericRepository<JobLogUpdateSurvey>(db);
                }
                return jobLogUpdateSurvey;
            }
        }

        private GenericRepository<JobLogCreateSurvey> jobLogCreateSurvey;
        public GenericRepository<JobLogCreateSurvey> JobLogCreateSurveyRespository
        {
            get
            {
                if (this.jobLogCreateSurvey == null)
                {
                    this.jobLogCreateSurvey = new GenericRepository<JobLogCreateSurvey>(db);
                }
                return jobLogCreateSurvey;
            }
        }


        private GenericRepository<JobLogShiftReminderEmail> jobLogShiftReminderEmail;
        public GenericRepository<JobLogShiftReminderEmail> JobLogShiftReminderEmailRespository
        {
            get
            {
                if (this.jobLogShiftReminderEmail == null)
                {
                    this.jobLogShiftReminderEmail = new GenericRepository<JobLogShiftReminderEmail>(db);
                }
                return jobLogShiftReminderEmail;
            }
        }


        private GenericRepository<JobLogRegistrationCompletedEmail> jobLogRegistrationCompletedEmail;
        public GenericRepository<JobLogRegistrationCompletedEmail> JobLogRegistrationCompletedEmailRespository
        {
            get
            {
                if (this.jobLogRegistrationCompletedEmail == null)
                {
                    this.jobLogRegistrationCompletedEmail = new GenericRepository<JobLogRegistrationCompletedEmail>(db);
                }
                return jobLogRegistrationCompletedEmail;
            }
        }

        private GenericRepository<JobLogBaselineSurveyEmail> jobLogBaselineSurveyEmail;
        public GenericRepository<JobLogBaselineSurveyEmail> JobLogBaselineSurveyEmailRespository
        {
            get
            {
                if (this.jobLogBaselineSurveyEmail == null)
                {
                    this.jobLogBaselineSurveyEmail = new GenericRepository<JobLogBaselineSurveyEmail>(db);
                }
                return jobLogBaselineSurveyEmail;
            }
        }

        private GenericRepository<JobLogExitSurveyEmail> jobLogExitSurveyEmail;
        public GenericRepository<JobLogExitSurveyEmail> JobLogExitSurveyEmailRespository
        {
            get
            {
                if (this.jobLogExitSurveyEmail == null)
                {
                    this.jobLogExitSurveyEmail = new GenericRepository<JobLogExitSurveyEmail>(db);
                }
                return jobLogExitSurveyEmail;
            }
        }



        private GenericRepository<BirthYear> birthYears;
        public GenericRepository<BirthYear> BirthYearRespository
        {
            get
            {
                if (this.birthYears == null)
                {
                    this.birthYears = new GenericRepository<BirthYear>(db);
                }
                return birthYears;
            }
        }

        private GenericRepository<Ethinicity> ethinicities;
        public GenericRepository<Ethinicity> EthinicityRespository
        {
            get
            {
                if (this.ethinicities == null)
                {
                    this.ethinicities = new GenericRepository<Ethinicity>(db);
                }
                return ethinicities;
            }
        }

        private GenericRepository<MedicalUniversity> medicalUniversities;
        public GenericRepository<MedicalUniversity> MedicalUniversitiesRespository
        {
            get
            {
                if (this.medicalUniversities == null)
                {
                    this.medicalUniversities = new GenericRepository<MedicalUniversity>(db);
                }
                return medicalUniversities;
            }
        }



        private GenericRepository<PageStat> pageStats;
        public GenericRepository<PageStat> PageStatsRespository
        {
            get
            {
                if (this.pageStats == null)
                {
                    this.pageStats = new GenericRepository<PageStat>(db);
                }
                return pageStats;
            }
        }

        private GenericRepository<ProfileSpecialty> profileSpecialties;
        public GenericRepository<ProfileSpecialty> ProfileSpecialtiesRespository
        {
            get
            {
                if (this.profileSpecialties == null)
                {
                    this.profileSpecialties = new GenericRepository<ProfileSpecialty>(db);
                }
                return profileSpecialties;
            }
        }

        private GenericRepository<ProfileEthinicity> profileEthinicities;
        public GenericRepository<ProfileEthinicity> ProfileEthinicitiesRespository
        {
            get
            {
                if (this.profileEthinicities == null)
                {
                    this.profileEthinicities = new GenericRepository<ProfileEthinicity>(db);
                }
                return profileEthinicities;
            }
        }

        private GenericRepository<Profile> profile;
        public GenericRepository<Profile> ProfileRespository
        {
            get
            {
                if (this.profile == null)
                {
                    this.profile = new GenericRepository<Profile>(db);
                }
                return profile;
            }
        }

        private GenericRepository<ProfileComment> profileComment;
        public GenericRepository<ProfileComment> ProfileCommentRespository
        {
            get
            {
                if (this.profileComment == null)
                {
                    this.profileComment = new GenericRepository<ProfileComment>(db);
                }
                return profileComment;
            }
        }




        private GenericRepository<Feedback> feedback;
        public GenericRepository<Feedback> FeedbackRespository
        {
            get
            {
                if (this.feedback == null)
                {
                    this.feedback = new GenericRepository<Feedback>(db);
                }
                return feedback;
            }
        }


        private GenericRepository<ProfileWellbeing> profileWellbeing;
        public GenericRepository<ProfileWellbeing> ProfileWellbeingRespository
        {
            get
            {
                if (this.profileWellbeing == null)
                {
                    this.profileWellbeing = new GenericRepository<ProfileWellbeing>(db);
                }
                return profileWellbeing;
            }
        }

        private GenericRepository<ProfileTaskTime> profileTaskTime;
        public GenericRepository<ProfileTaskTime> ProfileTaskTimeRespository
        {
            get
            {
                if (this.profileTaskTime == null)
                {
                    this.profileTaskTime = new GenericRepository<ProfileTaskTime>(db);
                }
                return profileTaskTime;
            }
        }

        private GenericRepository<ProfilePlacement> profilePlacement;
        public GenericRepository<ProfilePlacement> ProfilePlacementRespository
        {
            get
            {
                if (this.profilePlacement == null)
                {
                    this.profilePlacement = new GenericRepository<ProfilePlacement>(db);
                }
                return profilePlacement;
            }
        }


        private GenericRepository<ProfileContract> profileContract;
        public GenericRepository<ProfileContract> ProfileContractRespository
        {
            get
            {
                if (this.profileContract == null)
                {
                    this.profileContract = new GenericRepository<ProfileContract>(db);
                }
                return profileContract;
            }
        }

        private GenericRepository<ProfileTraining> profileTraining;
        public GenericRepository<ProfileTraining> ProfileTrainingRespository
        {
            get
            {
                if (this.profileTraining == null)
                {
                    this.profileTraining = new GenericRepository<ProfileTraining>(db);
                }
                return profileTraining;
            }
        }

        private GenericRepository<ProfileDemographic> profileDemographic;
        public GenericRepository<ProfileDemographic> ProfileDemographicRespository
        {
            get
            {
                if (this.profileDemographic == null)
                {
                    this.profileDemographic = new GenericRepository<ProfileDemographic>(db);
                }
                return profileDemographic;
            }
        }

        private GenericRepository<ProfileTask> profileTasks;
        public GenericRepository<ProfileTask> ProfileTasksRespository
        {
            get
            {
                if (this.profileTasks == null)
                {
                    this.profileTasks = new GenericRepository<ProfileTask>(db);
                }
                return profileTasks;
            }
        }
        private GenericRepository<ResponseAffects> respAffect;
        public GenericRepository<ResponseAffects> ResponseAffectRespository
        {
            get
            {
                if (this.respAffect == null)
                {
                    this.respAffect = new GenericRepository<ResponseAffects>(db);
                }
                return respAffect;
            }
        }
        private GenericRepository<WAMResponse> wamresponses;
        public GenericRepository<WAMResponse> WAMResponsesRespository
        {
            get
            {
                if (this.wamresponses == null)
                {
                    this.wamresponses = new GenericRepository<WAMResponse>(db);
                }
                return wamresponses;
            }
        }
        private GenericRepository<WAMWithWhomTaskTime> wamwithwhom;
        public GenericRepository<WAMWithWhomTaskTime> WAMWithWhomRepository
        {
            get
            {
                if (this.wamwithwhom == null)
                {
                    this.wamwithwhom = new GenericRepository<WAMWithWhomTaskTime>(db);
                }
                return wamwithwhom;
            }
        }
        private GenericRepository<Response> responses;
        public GenericRepository<Response> ResponsesRespository
        {
            get
            {
                if (this.responses == null)
                {
                    this.responses = new GenericRepository<Response>(db);
                }
                return responses;
            }
        }

        private GenericRepository<ProfileRoster> profileRoster;
        public GenericRepository<ProfileRoster> ProfileRosterRespository
        {
            get
            {
                if (this.profileRoster == null)
                {
                    this.profileRoster = new GenericRepository<ProfileRoster>(db);
                }
                return profileRoster;
            }
        }


        private GenericRepository<Specialty> specialities;
        public GenericRepository<Specialty> SpecialitiesRespository
        {
            get
            {
                if (this.specialities == null)
                {
                    this.specialities = new GenericRepository<Specialty>(db);
                }
                return specialities;
            }
        }

        private GenericRepository<Survey> surveys;
        public GenericRepository<Survey> SurveyRespository
        {
            get
            {
                if (this.surveys == null)
                {
                    this.surveys = new GenericRepository<Survey>(db);
                }
                return surveys;
            }
        }
        private GenericRepository<MyDayTaskList> multiTasklist;
        public GenericRepository<MyDayTaskList> MultiTaskListRepository
        {
            get
            {
                if (this.multiTasklist == null)
                {
                    this.multiTasklist = new GenericRepository<MyDayTaskList>(db);
                }
                return multiTasklist;
            }
        }
        private GenericRepository<TaskItem> taskItem;
        public GenericRepository<TaskItem> TaskItemRespository
        {
            get
            {
                if (this.taskItem == null)
                {
                    this.taskItem = new GenericRepository<TaskItem>(db);
                }
                return taskItem;
            }
        }
        private GenericRepository<MyDayTaskList> mydaytasks;
        public GenericRepository<MyDayTaskList> MyDayTasksRespository
        {
            get
            {
                if (this.mydaytasks == null)
                {
                    this.mydaytasks = new GenericRepository<MyDayTaskList>(db);
                }
                return mydaytasks;
            }
        }
        private GenericRepository<MyDayErrorLogs> mydayerrorlogs;
        public GenericRepository<MyDayErrorLogs> Mydayerrorlogs_Respository
        {
            get
            {
                if (this.mydayerrorlogs == null)
                {
                    this.mydayerrorlogs = new GenericRepository<MyDayErrorLogs>(db);
                }
                return mydayerrorlogs;
            }
        }


        #region Exit Survey
        private GenericRepository<ExitSurvey_Page1> exitSurvey_Page1;
        public GenericRepository<ExitSurvey_Page1> ExitSurvey_Page1_Respository
        {
            get
            {
                if (this.exitSurvey_Page1 == null)
                {
                    this.exitSurvey_Page1 = new GenericRepository<ExitSurvey_Page1>(db);
                }
                return exitSurvey_Page1;
            }
        }

        private GenericRepository<ExitSurvey_Page2> exitSurvey_Page2;
        public GenericRepository<ExitSurvey_Page2> ExitSurvey_Page2_Respository
        {
            get
            {
                if (this.exitSurvey_Page2 == null)
                {
                    this.exitSurvey_Page2 = new GenericRepository<ExitSurvey_Page2>(db);
                }
                return exitSurvey_Page2;
            }
        }

        private GenericRepository<ExitSurvey_Page3> exitSurvey_Page3;
        public GenericRepository<ExitSurvey_Page3> ExitSurvey_Page3_Respository
        {
            get
            {
                if (this.exitSurvey_Page3 == null)
                {
                    this.exitSurvey_Page3 = new GenericRepository<ExitSurvey_Page3>(db);
                }
                return exitSurvey_Page3;
            }
        }


        private GenericRepository<ExitSurvey_Page4> exitSurvey_Page4;
        public GenericRepository<ExitSurvey_Page4> ExitSurvey_Page4_Respository
        {
            get
            {
                if (this.exitSurvey_Page4 == null)
                {
                    this.exitSurvey_Page4 = new GenericRepository<ExitSurvey_Page4>(db);
                }
                return exitSurvey_Page4;
            }
        }


        private GenericRepository<ExitSurvey_Page5> exitSurvey_Page5;
        public GenericRepository<ExitSurvey_Page5> ExitSurvey_Page5_Respository
        {
            get
            {
                if (this.exitSurvey_Page5 == null)
                {
                    this.exitSurvey_Page5 = new GenericRepository<ExitSurvey_Page5>(db);
                }
                return exitSurvey_Page5;
            }
        }
        private GenericRepository<ExitSurvey_PageContinued5> exitSurvey_PageContinued5;
        public GenericRepository<ExitSurvey_PageContinued5> ExitSurvey_PageContinued5_Respository
        {
            get
            {
                if (this.exitSurvey_PageContinued5 == null)
                {
                    this.exitSurvey_PageContinued5 = new GenericRepository<ExitSurvey_PageContinued5>(db);
                }
                return exitSurvey_PageContinued5;
            }
        }

        private GenericRepository<ExitSurvey_Page6> exitSurvey_Page6;
        public GenericRepository<ExitSurvey_Page6> ExitSurvey_Page6_Respository
        {
            get
            {
                if (this.exitSurvey_Page6 == null)
                {
                    this.exitSurvey_Page6 = new GenericRepository<ExitSurvey_Page6>(db);
                }
                return exitSurvey_Page6;
            }
        }


        private GenericRepository<ExitSurvey_Page7> exitSurvey_Page7;
        public GenericRepository<ExitSurvey_Page7> ExitSurvey_Page7_Respository
        {
            get
            {
                if (this.exitSurvey_Page7 == null)
                {
                    this.exitSurvey_Page7 = new GenericRepository<ExitSurvey_Page7>(db);
                }
                return exitSurvey_Page7;
            }
        }

        private GenericRepository<ExitSurvey_Page8> exitSurvey_Page8;
        public GenericRepository<ExitSurvey_Page8> ExitSurvey_Page8_Respository
        {
            get
            {
                if (this.exitSurvey_Page8 == null)
                {
                    this.exitSurvey_Page8 = new GenericRepository<ExitSurvey_Page8>(db);
                }
                return exitSurvey_Page8;
            }
        }


        private GenericRepository<ExitSurvey_Page9> exitSurvey_Page9;
        public GenericRepository<ExitSurvey_Page9> ExitSurvey_Page9_Respository
        {
            get
            {
                if (this.exitSurvey_Page9 == null)
                {
                    this.exitSurvey_Page9 = new GenericRepository<ExitSurvey_Page9>(db);
                }
                return exitSurvey_Page9;
            }
        }


        private GenericRepository<ExitSurvey_Page10> exitSurvey_Page10;
        public GenericRepository<ExitSurvey_Page10> ExitSurvey_Page10_Respository
        {
            get
            {
                if (this.exitSurvey_Page10 == null)
                {
                    this.exitSurvey_Page10 = new GenericRepository<ExitSurvey_Page10>(db);
                }
                return exitSurvey_Page10;
            }
        }



        private GenericRepository<ExitSurvey_Page11> exitSurvey_Page11;
        public GenericRepository<ExitSurvey_Page11> ExitSurvey_Page11_Respository
        {
            get
            {
                if (this.exitSurvey_Page11 == null)
                {
                    this.exitSurvey_Page11 = new GenericRepository<ExitSurvey_Page11>(db);
                }
                return exitSurvey_Page11;
            }
        }


        private GenericRepository<ExitSurvey_Page12> exitSurvey_Page12;
        public GenericRepository<ExitSurvey_Page12> ExitSurvey_Page12_Respository
        {
            get
            {
                if (this.exitSurvey_Page12 == null)
                {
                    this.exitSurvey_Page12 = new GenericRepository<ExitSurvey_Page12>(db);
                }
                return exitSurvey_Page12;
            }
        }

        private GenericRepository<ExitSurvey_PageContinued12> exitSurvey_PageContinued12;
        public GenericRepository<ExitSurvey_PageContinued12> ExitSurvey_PageContinued12_Respository
        {
            get
            {
                if (this.exitSurvey_PageContinued12 == null)
                {
                    this.exitSurvey_PageContinued12 = new GenericRepository<ExitSurvey_PageContinued12>(db);
                }
                return exitSurvey_PageContinued12;
            }
        }


        private GenericRepository<ExitSurvey_Page13> exitSurvey_Page13;
        public GenericRepository<ExitSurvey_Page13> ExitSurvey_Page13_Respository
        {
            get
            {
                if (this.exitSurvey_Page13 == null)
                {
                    this.exitSurvey_Page13 = new GenericRepository<ExitSurvey_Page13>(db);
                }
                return exitSurvey_Page13;
            }
        }



        private GenericRepository<ExitSurvey_Page14> exitSurvey_Page14;
        public GenericRepository<ExitSurvey_Page14> ExitSurvey_Page14_Respository
        {
            get
            {
                if (this.exitSurvey_Page14 == null)
                {
                    this.exitSurvey_Page14 = new GenericRepository<ExitSurvey_Page14>(db);
                }
                return exitSurvey_Page14;
            }
        }
        private GenericRepository<ExitSurvey_Feedback> exitSurvey_Feedback;
        public GenericRepository<ExitSurvey_Feedback> ExitSurvey_Feedback_Respository
        {
            get
            {
                if (this.exitSurvey_Feedback == null)
                {
                    this.exitSurvey_Feedback = new GenericRepository<ExitSurvey_Feedback>(db);
                }
                return exitSurvey_Feedback;
            }
        }
        #endregion

        #region methods
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public int SaveChangesAndGetId()
        {
            return db.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await db.SaveChangesAsync();
        }
        #endregion

        #region WAMDemo

        private GenericRepository<WAMWellBeing> wamWellbeing;
        public GenericRepository<WAMWellBeing> WAMWellbeingRespository
        {
            get
            {
                if (this.wamWellbeing == null)
                {
                    this.wamWellbeing = new GenericRepository<WAMWellBeing>(db);
                }
                return wamWellbeing;
            }
        }
        private GenericRepository<WAMProfileRole> wamProfileRole;
        public GenericRepository<WAMProfileRole> WAMProfileRoleRepository
        {
            get
            {
                if (this.wamProfileRole == null)
                { this.wamProfileRole = new GenericRepository<WAMProfileRole>(db); }
                return wamProfileRole;
            }
        }
        private GenericRepository<WAMTasks> wamTasks;
        public GenericRepository<WAMTasks> WAMTasksRespository
        {
            get
            {
                if (this.wamTasks == null)
                {
                    this.wamTasks = new GenericRepository<WAMTasks>(db);
                }
                return wamTasks;
            }
        }
        private GenericRepository<WAMDemographics> wamDemographics;
        public GenericRepository<WAMDemographics> WAMDemographicsRespository
        {
            get
            {
                if (this.wamDemographics == null)
                {
                    this.wamDemographics = new GenericRepository<WAMDemographics>(db);
                }
                return wamDemographics;
            }
        }
        private GenericRepository<WAMIntentions> wamIntentions;
        public GenericRepository<WAMIntentions> WAMIntentionsRepository
        {
            get
            {
                if (this.wamIntentions == null)
                {
                    this.wamIntentions = new GenericRepository<WAMIntentions>(db);
                }
                return wamIntentions;
            }
        }
        private GenericRepository<WAMFeedback> wamFeedback;
        public GenericRepository<WAMFeedback> WAMFeedbackRespository
        {
            get
            {
                if (this.wamFeedback == null)
                {
                    this.wamFeedback = new GenericRepository<WAMFeedback>(db);
                }
                return wamFeedback;
            }
        }
        #endregion

        #region KIDS

        private GenericRepository<KidsTasks> kidsTaskList;
        public GenericRepository<KidsTasks> KidsTaskRepository
        {
            get
            {
                if (this.kidsTaskList == null)
                {
                    this.kidsTaskList = new GenericRepository<KidsTasks>(db);
                }
                return kidsTaskList;
            }
        }
        private GenericRepository<KidsResponses> kidsResponses;
        public GenericRepository<KidsResponses> KidsResponseRepository
        {
            get
            {
                if (this.kidsResponses == null)
                {
                    this.kidsResponses = new GenericRepository<KidsResponses>(db);
                }
                return kidsResponses;
            }
        }
        private GenericRepository<KidsFeedback> kidsFeedback;
        public GenericRepository<KidsFeedback> KidsFeedbackRepository
        {
            get
            {
                if (this.kidsFeedback == null)
                {
                    this.kidsFeedback = new GenericRepository<KidsFeedback>(db);
                }
                return kidsFeedback;
            }
        }
        private GenericRepository<KidsSurvey> kidsSurvey;
        public GenericRepository<KidsSurvey> KidsSurveyRepository
        {
            get
            {
                if (this.kidsSurvey == null)
                {
                    this.kidsSurvey = new GenericRepository<KidsSurvey>(db);
                }
                return kidsSurvey;
            }
        }
        private GenericRepository<KidsLocation> kidsLocation;
        public GenericRepository<KidsLocation> KidsLocationRepository
        {
            get
            {
                if (this.kidsLocation == null)
                {
                    this.kidsLocation = new GenericRepository<KidsLocation>(db);
                }
                return kidsLocation;
            }
        }
        private GenericRepository<KidsTravel> kidsTravel;
        public GenericRepository<KidsTravel> KidsTravelRepository
        {
            get
            {
                if (this.kidsTravel == null)
                {
                    this.kidsTravel = new GenericRepository<KidsTravel>(db);
                }
                return kidsTravel;
            }
        }
        private GenericRepository<KidsTasksOnLocation> kidsTaskOnLocation;
        public GenericRepository<KidsTasksOnLocation> KidsTasksOnLocationRepository
        {
            get
            {
                if (this.kidsTaskOnLocation == null)
                {
                    this.kidsTaskOnLocation = new GenericRepository<KidsTasksOnLocation>(db);
                }
                return kidsTaskOnLocation;
            }
        }
        private GenericRepository<KidsEmoStageTracked> kidsEmo;
        public GenericRepository<KidsEmoStageTracked> KidsEMoTrackRepository
        {
            get
            {
                if (this.kidsEmo == null)
                {
                    this.kidsEmo = new GenericRepository<KidsEmoStageTracked>(db);
                }
                return kidsEmo;
            }
        }
        private GenericRepository<KidsReaction> kidsReact;
        public GenericRepository<KidsReaction> KidsReactionRepository
        {
            get
            {
                if (this.kidsReact == null)
                {
                    this.kidsReact = new GenericRepository<KidsReaction>(db);
                }
                return kidsReact;
            }
        }
        #endregion

        #region "EXIT Version 2"

        private GenericRepository<ExitV2Survey> exitv2Survey;
        public GenericRepository<ExitV2Survey> ExitV2SurveyRepository
        {
            get
            {
                if (this.exitv2Survey == null)
                {
                    this.exitv2Survey = new GenericRepository<ExitV2Survey>(db);
                }
                return exitv2Survey;
            }
        }
        private GenericRepository<WellBeing> wellBeing;
        public GenericRepository<WellBeing> WellBeingRepository
        {
            get
            {
                if (this.wellBeing == null)
                {
                    this.wellBeing = new GenericRepository<WellBeing>(db);
                }
                return wellBeing;
            }
        }
        private GenericRepository<FirstJob> firstJob;
        public GenericRepository<FirstJob> FirstJobRepository
        {
            get
            {
                if (this.firstJob == null)
                {
                    this.firstJob = new GenericRepository<FirstJob>(db);
                }
                return firstJob;
            }
        }
        private GenericRepository<SecondJob> secondJob;
        public GenericRepository<SecondJob> SecondJobRepository
        {
            get
            {
                if (this.secondJob == null)
                {
                    this.secondJob = new GenericRepository<SecondJob>(db);
                }
                return secondJob;
            }
        }
        private GenericRepository<ThirdJob> thirdJob;
        public GenericRepository<ThirdJob> ThirdJobRepository
        {
            get
            {
                if (this.thirdJob == null)
                {
                    this.thirdJob = new GenericRepository<ThirdJob>(db);
                }
                return thirdJob;
            }
        }
        private GenericRepository<FirstWorkEnvironment> firstWE;
        public GenericRepository<FirstWorkEnvironment> FirstWERepository
        {
            get
            {
                if (this.firstWE == null)
                {
                    this.firstWE = new GenericRepository<FirstWorkEnvironment>(db);
                }
                return firstWE;
            }
        }
        private GenericRepository<SecondWorkEnvironment> secondWE;
        public GenericRepository<SecondWorkEnvironment> SecondWERepository
        {
            get
            {
                if (this.secondWE == null)
                {
                    this.secondWE = new GenericRepository<SecondWorkEnvironment>(db);
                }
                return secondWE;
            }
        }
        private GenericRepository<ThirdWorkEnvironment> thirdWE;
        public GenericRepository<ThirdWorkEnvironment> ThirdWERepository
        {
            get
            {
                if (this.thirdWE == null)
                {
                    this.thirdWE = new GenericRepository<ThirdWorkEnvironment>(db);
                }
                return thirdWE;
            }
        }
        private GenericRepository<FourthWorkEnvironment> fourthWE;
        public GenericRepository<FourthWorkEnvironment> FourthWERepository
        {
            get
            {
                if (this.fourthWE == null)
                {
                    this.fourthWE = new GenericRepository<FourthWorkEnvironment>(db);
                }
                return fourthWE;
            }
        }
        private GenericRepository<FifthWorkEnvironment> fifthWE;
        public GenericRepository<FifthWorkEnvironment> FifthWERepository
        {
            get
            {
                if (this.fifthWE == null)
                {
                    this.fifthWE = new GenericRepository<FifthWorkEnvironment>(db);
                }
                return fifthWE;
            }
        }
        private GenericRepository<FirstTraining> firstTraining;
        public GenericRepository<FirstTraining> FirstTrainingRepository
        {
            get
            {
                if (this.firstTraining == null)
                {
                    this.firstTraining = new GenericRepository<FirstTraining>(db);
                }
                return firstTraining;
            }
        }
        private GenericRepository<SecondTraining> secondTraining;
        public GenericRepository<SecondTraining> SecondTrainingRepository
        {
            get
            {
                if (this.secondTraining == null)
                {
                    this.secondTraining = new GenericRepository<SecondTraining>(db);
                }
                return secondTraining;
            }
        }
        private GenericRepository<ThirdTraining> thirdTraining;
        public GenericRepository<ThirdTraining> ThirdTrainingRepository
        {
            get
            {
                if (this.thirdTraining == null)
                {
                    this.thirdTraining = new GenericRepository<ThirdTraining>(db);
                }
                return thirdTraining;
            }
        }
        private GenericRepository<AboutYouES> aboutYouES;
        public GenericRepository<AboutYouES> AboutYouESRepository
        {
            get
            {
                if (this.aboutYouES == null)
                {
                    this.aboutYouES = new GenericRepository<AboutYouES>(db);
                }
                return aboutYouES;
            }
        }
        #endregion

        #region Case Workers

        private GenericRepository<SWSubjectiveWellBeing> swSubjectiveWellbeing;
        public GenericRepository<SWSubjectiveWellBeing> SWSubjectiveWellbeingRespository
        {
            get
            {
                if (this.swSubjectiveWellbeing == null)
                {
                    this.swSubjectiveWellbeing = new GenericRepository<SWSubjectiveWellBeing>(db);
                }
                return swSubjectiveWellbeing;
            }
        }
        private GenericRepository<CurrentWorkplace> currentWorkplace;
        public GenericRepository<CurrentWorkplace> CurrentWorkplaceRepository
        {
            get
            {
                if (this.currentWorkplace == null)
                {
                    this.currentWorkplace = new GenericRepository<CurrentWorkplace>(db);
                }
                return currentWorkplace;
            }
        }
        private GenericRepository<CurrentWorkplaceContd> currentWorkplaceContd;
        public GenericRepository<CurrentWorkplaceContd> CurrentWorkplaceContdRepository
        {
            get
            {
                if (this.currentWorkplaceContd == null)
                {
                    this.currentWorkplaceContd = new GenericRepository<CurrentWorkplaceContd>(db);
                }
                return currentWorkplaceContd;
            }
        }
        private GenericRepository<CaseLoad> caseload;
        public GenericRepository<CaseLoad> CaseLoadRepository
        {
            get
            {
                if (this.caseload == null)
                {
                    this.caseload = new GenericRepository<CaseLoad>(db);
                }
                return caseload;
            }
        }
        private GenericRepository<TimeAllocation> timeAllocated;
        public GenericRepository<TimeAllocation> TimeAllocatedRepository
        {
            get
            {
                if (this.timeAllocated == null)
                {
                    this.timeAllocated = new GenericRepository<TimeAllocation>(db);
                }
                return timeAllocated;
            }
        }
        private GenericRepository<Demographics> cwdemographics;
        public GenericRepository<Demographics> CaseDemographicsRespository
        {
            get
            {
                if (this.cwdemographics == null)
                {
                    this.cwdemographics = new GenericRepository<Demographics>(db);
                }
                return cwdemographics;
            }
        }
        private GenericRepository<EducationBackground> educationbackground;
        public GenericRepository<EducationBackground> EducationBackgroundRepository
        {
            get
            {
                if (this.educationbackground == null)
                {
                    this.educationbackground = new GenericRepository<EducationBackground>(db);
                }
                return educationbackground;
            }
        }
        private GenericRepository<JobIntentions> jobIntentions;
        public GenericRepository<JobIntentions> JobIntentionsRepository
        {
            get
            {
                if (this.jobIntentions == null)
                {
                    this.jobIntentions = new GenericRepository<JobIntentions>(db);
                }
                return jobIntentions;
            }
        }
        private GenericRepository<CaseWorkersFeedback> caseWorkersFeedback;
        public GenericRepository<CaseWorkersFeedback> CaseWorkersFeedBackRepository
        {
            get
            {
                if (this.caseWorkersFeedback == null)
                {
                    this.caseWorkersFeedback = new GenericRepository<CaseWorkersFeedback>(db);
                }
                return caseWorkersFeedback;
            }
        }
        #endregion

    }
}