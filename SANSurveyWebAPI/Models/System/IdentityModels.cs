
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.Models.Auth;

namespace SANSurveyWebAPI.Models.Api
{

    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }

        // Add any custom Role properties/code here
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>, IQueryableRoleStore<ApplicationRole, string>, IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();

            // Add any custom User properties/code here
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined 
            // in CookieAuthenticationOptions.AuthenticationType
            var userIdentity =
                await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

    }

    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, string>, IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        //******************
        //Set of DB objects
        //******************

        public DbSet<ProximiVisitor> ProximiVisitors { get; set; }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Ethinicity> Ethinicitys { get; set; }
        public DbSet<Specialty> Specialitys { get; set; }
        public DbSet<BirthYear> BirthYears { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        public DbSet<MedicalUniversity> MedicalUniversitys { get; set; }
        public DbSet<ProfileComment> ProfileComments { get; set; }
        public DbSet<ProfileWellbeing> ProfileWellbeings { get; set; }
        public DbSet<ProfileTaskTime> ProfileTaskTimes { get; set; }
        public DbSet<ProfilePlacement> ProfilePlacements { get; set; }
        public DbSet<ProfileContract> ProfileContracts { get; set; }
        public DbSet<ProfileTraining> ProfileTrainings { get; set; }
        public DbSet<ProfileDemographic> ProfileDemographics { get; set; }
        public DbSet<ProfileRoster> ProfileRosters { get; set; }
        public DbSet<ProfileTask> ProfileTasks { get; set; }
        public DbSet<ProfileEthinicity> ProfileEthinicitys { get; set; }
        public DbSet<ProfileSpecialty> ProfileSpecialtys { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<PageStat> PageStats { get; set; }
        public DbSet<Response> Responses { get; set; }

        #region Admin

        public DbSet<MasterData> MasterDataS { get; set; }
        public DbSet<TaskCategory> TaskCategory { get; set; }

        #endregion

        #region "New Multiple Task List selection"

        public DbSet<MyDayTaskList> MyDayTasks { get; set; }
        public DbSet<ResponseAffects> ResponseAffects { get; set; }

        #endregion

        #region "Kids Survey"

        public DbSet<KidsSurvey> KidsSurveys { get; set; }
        public DbSet<KidsTasks> KidsTaskS { get; set; }
        public DbSet<KidsResponses> KidsResponseS { get; set; }
        public DbSet<KidsFeedback> KidsFeedbackk { get; set; }
        public DbSet<KidsLocation> KidsLocations { get; set; }
        public DbSet<KidsTravel> KidsTravels { get; set; }
        public DbSet<KidsTasksOnLocation> KidsTasksLocation { get; set; }
        public DbSet<KidsReaction> KidsReactions { get; set; }
        public DbSet<KidsEmoStageTracked> KidsEmoTracked { get; set; }

        #endregion

        #region Exit Survey Version 2

        public DbSet<ExitV2Survey> ExitV2Surveys { get; set; }
        public DbSet<WellBeing> WellBeings { get; set; }
        public DbSet<FirstJob> FirstJobs { get; set; }
        public DbSet<SecondJob> SecondJobs { get; set; }
        public DbSet<ThirdJob> ThirdJobs { get; set; }
        public DbSet<FirstWorkEnvironment> FirstWorkEnvironments { get; set; }
        public DbSet<SecondWorkEnvironment> SecondWorkEnvironments { get; set; }
        public DbSet<ThirdWorkEnvironment> ThirdWorkEnvironment { get; set; }
        public DbSet<FourthWorkEnvironment> FourthWorkEnvironments { get; set; }
        public DbSet<FifthWorkEnvironment> FifthWorkEnvironments { get; set; }
        public DbSet<FirstTraining> FirstTrainings { get; set; }
        public DbSet<SecondTraining> SecondTrainings { get; set; }
        public DbSet<ThirdTraining> ThirdTrainings { get; set; }
        public DbSet<AboutYouES> AboutYouESs { get; set; }

        #endregion

        #region Case Workers

        public DbSet<SWSubjectiveWellBeing> SWSubjectiveWellBeings { get; set; }
        public DbSet<CurrentWorkplace> CWCurrentWorkplace { get; set; }
        public DbSet<CaseLoad> CWCaseLoad { get; set; }
        public DbSet<TimeAllocation> CWTimeAllocation { get; set; }
        public DbSet<Demographics> CWDemographics { get; set; }
        public DbSet<EducationBackground> CWEducationBackground { get; set; }
        public DbSet<JobIntentions> CWJobIntentions { get; set; }
        public DbSet<CaseWorkersFeedback> CWFeedback { get; set; }
        public DbSet<CurrentWorkplaceContd> CWCurrentWorkPlaceContd { get; set; }

        #endregion

        public DbSet<JobLogBaselineSurveyEmail> JobLogBaselineSurveyEmails { get; set; }
        public DbSet<JobLogExitSurveyEmail> JobLogExitSurveyEmails { get; set; }


        public DbSet<JobLogRegistrationCompletedEmail> JobLogRegistrationCompletedEmails { get; set; }
        public DbSet<JobLogShiftReminderEmail> JobLogShiftReminderEmails { get; set; }

        public DbSet<JobLogCreateSurvey> JobLogCreateSurveys { get; set; }
        public DbSet<JobLogStartSurveyReminderEmail> JobLogStartSurveyReminderEmails { get; set; }

        public DbSet<JobLogCompleteSurveyReminderEmail> JobLogCompleteSurveyReminderEmails { get; set; }
        public DbSet<JobLogExpiringSoonSurveyNotStartedReminderEmail> JobLogExpiringSoonSurveyNotStartedReminderEmails { get; set; }
        public DbSet<JobLogExpiringSoonSurveyNotCompletedReminderEmail> JobLogExpiringSoonSurveyNotCompletedReminderEmails { get; set; }

        public DbSet<JobLogUpdateSurvey> JobLogUpdateSurveys { get; set; }


        #region exit survey
        public DbSet<ExitSurvey_Page1> ExitSurvey_Page1s { get; set; }
        public DbSet<ExitSurvey_Page2> ExitSurvey_Page2s { get; set; }
        public DbSet<ExitSurvey_Page3> ExitSurvey_Page3s { get; set; }


        public DbSet<ExitSurvey_Page4> ExitSurvey_Page4s { get; set; }
        public DbSet<ExitSurvey_Page5> ExitSurvey_Page5s { get; set; }
        public DbSet<ExitSurvey_PageContinued5> ExitSurvey_PageContinued5s { get; set; }
        public DbSet<ExitSurvey_Page6> ExitSurvey_Page6s { get; set; }

        public DbSet<ExitSurvey_Page7> ExitSurvey_Page7s { get; set; }
        public DbSet<ExitSurvey_Page8> ExitSurvey_Page8s { get; set; }

        public DbSet<ExitSurvey_Page9> ExitSurvey_Page9s { get; set; }
        public DbSet<ExitSurvey_Page10> ExitSurvey_Page10s { get; set; }
        public DbSet<ExitSurvey_Page11> ExitSurvey_Page11s { get; set; }
        public DbSet<ExitSurvey_Page12> ExitSurvey_Page12s { get; set; }

        public DbSet<ExitSurvey_PageContinued12> ExitSurvey_PageContinued12s { get; set; }
        public DbSet<ExitSurvey_Page13> ExitSurvey_Page13s { get; set; }
        public DbSet<ExitSurvey_Page14> ExitSurvey_Page14s { get; set; }
        public DbSet<ExitSurvey_Feedback> ExitSurvey_Feedback { get; set; }

        #endregion

        public DbSet<MyDayErrorLogs> MyDayErrorLogsS { get; set; }

        #region WAMDemo

        public DbSet<WAMWellBeing> WAMWellBeingS { get; set; }
        public DbSet<WAMProfileRole> WAMProfileRoleS { get; set; }
        public DbSet<WAMTasks> WAMTasksS { get; set; }
        public DbSet<WAMDemographics> WAMDemographicsS { get; set; }
        public DbSet<WAMIntentions> WAMIntentionsS { get; set; }
        public DbSet<WAMFeedback> WAMFeedbackS { get; set; }
        public DbSet<WAMResponse> WAMResponses { get; set; }
        public DbSet<WAMWithWhomTaskTime> WAMWithWHOM { get; set; }
        public DbSet<DisciplineOrRole> DisciplineOrRoles { get; set; }

        #endregion

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    


}