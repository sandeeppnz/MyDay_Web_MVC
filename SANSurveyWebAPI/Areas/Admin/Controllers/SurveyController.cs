using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Twilio;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using static SANSurveyWebAPI.Constants;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Controllers;

using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using SANSurveyWebAPI.DAL;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SurveyController : BaseController
    {
        private AdminService adminService;
        private SurveyService surveyService;

        private JobService jobService;
        private ProfileService profileService;
        private UnitOfWork _unitOfWork = new UnitOfWork();


        private ApplicationDbContext db = new ApplicationDbContext();
        private HangfireScheduler schedulerSvc;

        public class ProfileCascade
        {
            public int Id { get; set; }
            public string EmailAddress { get; set; }

            public string ProfileName { get; set; }
        }


        public SurveyController()
        {
            this.adminService = new AdminService();
            this.surveyService = new SurveyService();
            this.jobService = new JobService();
            this.profileService = new ProfileService();

        }


        protected override void Dispose(bool disposing)
        {
            adminService.Dispose();
            surveyService.Dispose(); // Reset survey function
            jobService.Dispose();
            profileService.Dispose();

            base.Dispose(disposing);
        }

        #region Normal Email
        

        private CurrentProfile GetProfile(string profileId, string profileEmail, string profileOffset, string profileName)
        {
            bool retFromDB = false;
            if (string.IsNullOrEmpty(profileId) || string.IsNullOrEmpty(profileEmail) || string.IsNullOrEmpty(profileOffset) || string.IsNullOrEmpty(profileName))
            {
                retFromDB = true;
            }

            CurrentProfile currProfile = new CurrentProfile();
            ProfileDto dt = new ProfileDto();

            if (retFromDB)
            {
                dt = profileService.GetProfileById(int.Parse(profileId));
                currProfile.ProfileId = dt.Id;
                currProfile.ProfileName = dt.Name;
                currProfile.ProfileEmailAddress = dt.LoginEmail;
                currProfile.OffsetFromUTC = dt.OffSetFromUTC;
                currProfile.ClientInitials = dt.ClientInitials;
            }
         

            return currProfile;
        }



        public async Task<ActionResult> ExpiringSoonNotCompletedEmail(int? id)
        {            

            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.ExpiringSoonRecurrentSurveyNotCompletedEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> ExpiringSoonNotStartedEmail(int? id)
        {

            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync( JobName.ExpiringSoonRecurrentSurveyNotStartedEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> CompleteReminderEmail(int? id)
        {

            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.CompleteRecurrentSurveyEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> ShiftReminderEmail(int? id)
        {

            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
          
            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.ShiftReminderEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return RedirectToAction("Index");
        }




        public async Task<ActionResult> StartReminderEmail(int? id)
        {

            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile p = new Profile();
            p = (Profile)Session["ClientByProfile"];

            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.StartRecurrentSurveyEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return RedirectToAction("Index");
        }





        private void GetObjectsForJob(int? id, out Survey survey, out ProfileRosterDto rosterDto, out CurrentProfile profile)
        {
            int surveyId = id.Value;
            survey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            var roster = _unitOfWork.ProfileRosterRespository.GetByID(survey.RosterItemId);
            rosterDto = ObjectMapper.GetProfileRosterDto(roster);
            profile = GetProfile(survey.ProfileId.ToString(), string.Empty, string.Empty, string.Empty);
        }



        ////[AutomaticRetry(Attempts = 20)]
        //public async static void SendEmailNormal(Profile profile, string body, string GetBaseURL())
        //{
        //    //var heading = GetBaseURL() + "Images/logo-mini.png";
        //    //var heading = "Text Header";
        //    //string heading = string.Format("<img src='{0}Images/logo.png' height='42' width='42'></img>", GetBaseURL());

        //    // Prepare Postal classes to work outside of ASP.NET request
        //    var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
        //    var engines = new ViewEngineCollection();
        //    engines.Add(new FileSystemRazorViewEngine(viewsPath));

        //    var emailService = new Postal.EmailService(engines);

        //    var email = new SurveyInvitationEmail
        //    {
        //        To = profile.EmailAddress,
        //        AppName = SANSurveyWebAPI.Constants.AppName,
        //        Name = profile.Name,
        //        Message = body
        //    };

        //    await email.SendAsync();
        //}

        #endregion





        [AllowAnonymous]
        public JsonResult GetCascadeProfiles()
        {

            var profs = db.Profiles.Select(c => new ProfileCascade { Id = c.Id, EmailAddress = c.LoginEmail, ProfileName = c.Name}).ToList();


            foreach (var prof in profs)
            {
                prof.EmailAddress = StringCipher.DecryptRfc2898(prof.EmailAddress);
            }
            return Json(profs, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetCascadeShifts(int? Profile)
        {
            //var shifts = db.RosterItems.AsQueryable();
            var shifts = db.ProfileRosters.AsQueryable();

            if (Profile != null)
            {
                shifts = shifts.Where(p => p.ProfileId == Profile);
            }

            return Json(shifts.Select(p => new { Id = p.Id, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }




        // GET: Surveys
        public async Task<ActionResult> Index()
        {
            var surveys = db.Surveys.Include(p => p.Profile).Include(r => r.RosterItem);

            
            return View(await surveys.ToListAsync());
        }





        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var survey = await db.Surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }
            return View(survey);
        }


        public ActionResult DeleteAll()
        {
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Surveys]");
            return RedirectToAction("Index");
        }

        #region SMS feature is not implemented
        //public ActionResult InviteSMS(int? id)
        //{
        //    if (!id.HasValue)
        //    {
        //        return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Profile profile = db.Profiles.Where(p => p.Id == id.Value).SingleOrDefault();

        //    Guid uid = Guid.NewGuid();
        //    profile.Uid = uid.ToString();
        //    profile.RegistrationProgressNext = Constants.StatusRegistrationProgress.Invited.ToString();

        //    db.Entry(profile).State = EntityState.Modified;
        //    db.SaveChanges();

        //    if (profile == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    var accountSid = "ACe16948352dbb7be01f8e2cacb2c5089b"; // Your Account SID from www.twilio.com/console
        //    var authToken = "f084b6461e8278d8ebfa51f71526ea7a";  // Your Auth Token from www.twilio.com/console

        //    var twilio = new TwilioRestClient(accountSid, authToken);
        //    var message = twilio.SendMessage(
        //        "+18559765791", // From (Replace with your Twilio number)
        //        "+64223249110", // To (Replace with your phone number)
        //        "Dear <Insert Name> You are invited to participate in a survey being conducted by the Auckland University of Technology and Health Education England about the work day experience of junior doctors.Please go to www.aut.ac.nz/Signup?uid=a419eddb-9f74-40d2-a270-46efb30c6e2a to participate.Thank you."
        //        );
        //    return RedirectToAction("Index");
        //}
        #endregion



        // GET: Profiles/Create
        public ActionResult Create()
        {
            ProfileAdminCreateVM v = new ProfileAdminCreateVM();
            //v.IsScheduled = false;
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,Uid,ProfileId,RosterItemId,CreatedDate,SurveyUserStartDateTime,SurveyUserCompletedDateTime,SurveyExpiryDateTime,Status")] Survey survey)
        public async Task<ActionResult> Create(ProfileAdminCreateVM v)
        {
            //bool surveyExists = false;
            //bool IsHangFireJobScheduled = false;
            //if (v.Shift != 0)
            //{
            //    surveyExists = surveyService.CheckRosterIdHasScheduledEmailsOrNot(v.Shift);

            //    if (surveyExists == true)
            //    {
            //        IsHangFireJobScheduled = surveyService.CheckScheduledHangFireJobs(v.Shift);
            //    }
            //}

            //if (IsHangFireJobScheduled == false)
            {
                int offSetUtc = db.Profiles.Where(x => x.Id == v.Profile).SingleOrDefault().OffSetFromUTC;
                offSetUtc = -offSetUtc;


                Survey survey = new Survey();

                if (ModelState.IsValid)
                {
                    survey.ProfileId = v.Profile;
                    survey.RosterItemId = v.Shift;
                    survey.CreatedDateTimeUtc = DateTime.UtcNow;
                    survey.Status = StatusSurvey.Active.ToString();
                    survey.SurveyProgressNext = Constants.StatusSurveyProgress.New.ToString();

                    //added on 23rd september 2018
                    if (survey.Uid == string.Empty || survey.Uid == null)
                    {
                        Guid uid = Guid.NewGuid();                       
                        survey.Uid = uid.ToString();
                    }

                    //added 2017-01-11

                    //var roster = db.RosterItems.Where(x => x.Id == v.Shift).SingleOrDefault();
                    var roster = db.ProfileRosters.Where(x => x.Id == v.Shift).SingleOrDefault();

                    TimeSpan span = roster.End.Subtract(roster.Start);
                    int shiftDuration = (int)Math.Round(span.TotalMinutes);

                    if (shiftDuration <= (Constants.SURVEY_WINDOW_IN_HRS * 60))
                    {
                        survey.SysGenRandomNumber = null; //generate 0 to 4
                        survey.SurveyWindowStartDateTime = roster.Start;
                        survey.SurveyWindowEndDateTime = roster.End;
                        survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                        survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);
                        survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");
                    }
                    else if (shiftDuration > (Constants.SURVEY_WINDOW_IN_HRS * 60) && shiftDuration <= (Constants.SURVEY_WINDOW_IN_HRS * 2 * 60))
                    {
                        int shiftDurationInHrs = (int)shiftDuration / 60;
                        Random rnd = new Random();
                        int rnNum = rnd.Next(shiftDurationInHrs - Constants.SURVEY_WINDOW_IN_HRS);
                        survey.SysGenRandomNumber = rnNum; //generate 0 to 4
                        survey.SurveyWindowStartDateTime = roster.Start.AddHours(rnNum);
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
                        survey.SurveyWindowStartDateTime = roster.Start.AddHours(rnNum);
                        survey.SurveyWindowEndDateTime = survey.SurveyWindowStartDateTime.Value.AddHours(Constants.SURVEY_WINDOW_IN_HRS);
                        survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                        survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);
                        survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");

                    }

                    survey.SurveyExpiryDateTime = survey.SurveyWindowEndDateTime.Value.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);
                    survey.SurveyExpiryDateTimeUtc = survey.SurveyExpiryDateTime.Value.AddHours(offSetUtc);

                    db.Surveys.Add(survey);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            //else
            //{
            //    v.IsScheduled = true;
            //}
            return View(v);
        }




        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = await db.Surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            //string name = survey.Name;
            //survey.Name = name;

            //ViewBag.RosterId = new SelectList(db.RosterItems, "Id", "Start", survey.RosterItemId);
            ViewBag.RosterId = new SelectList(db.ProfileRosters, "Id", "Start", survey.RosterItemId);

            ViewBag.UserId = new SelectList(db.Profiles, "Id", "EmailAddress", survey.ProfileId);
            return View(survey);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Uid,ProfileId,RosterItemId,CreatedDate,SurveyUserStartDateTime,SurveyUserCompletedDateTime,SurveyExpiryDateTime,Status")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                //string name = profile.Name;
                //profile.Name = name;

                db.Entry(survey).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.RosterId = new SelectList(db.RosterItems, "Id", "Start", survey.RosterItemId);
            ViewBag.RosterId = new SelectList(db.ProfileRosters, "Id", "Start", survey.RosterItemId);

            ViewBag.UserId = new SelectList(db.Profiles, "Id", "EmailAddress", survey.ProfileId);
            return View(survey);
        }


        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Survey survey = await db.Surveys.FindAsync(id);
            if (survey == null)
            {
                return HttpNotFound();
            }

            //string name = profile.Name;
            //profile.Name = name;

            return View(survey);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            db.Responses.RemoveRange(db.Responses.Where(x => x.SurveyId == id));
            await db.SaveChangesAsync();

            Survey surveys = await db.Surveys.FindAsync(id);
            db.Surveys.Remove(surveys);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }










        #region Hangfire Mass Email


        private async Task<int> CreateSurveyAuto(int profileId, int rosterItemId, DateTime start, DateTime end)
        {

            int offSetUtc = db.Profiles.Where(x => x.Id == profileId).SingleOrDefault().OffSetFromUTC;
            offSetUtc = -offSetUtc;


            Survey survey = new Survey();


            survey.ProfileId = profileId;
            survey.RosterItemId = rosterItemId;
            survey.CreatedDateTimeUtc = DateTime.UtcNow;
            survey.Status = StatusSurvey.Active.ToString();
            survey.SurveyProgressNext = Constants.StatusSurveyProgress.New.ToString();

            //added 2017-01-11
            //var roster = db.RosterItems.Where(x => x.Id == rosterItemId).SingleOrDefault();

            TimeSpan span = end.Subtract(start);
            int shiftDuration = (int) Math.Round(span.TotalMinutes);

            if (shiftDuration <= (Constants.SURVEY_WINDOW_IN_HRS * 60))
            {
                survey.SysGenRandomNumber = null; //generate 0 to 4
                survey.SurveyWindowStartDateTime = start;
                survey.SurveyWindowEndDateTime = end;

                survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);



                survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");
            }
            else if (shiftDuration > (Constants.SURVEY_WINDOW_IN_HRS * 60) && shiftDuration <= (Constants.SURVEY_WINDOW_IN_HRS * 2 * 60))
            {
                int shiftDurationInHrs = (int) shiftDuration / 60;
                Random rnd = new Random();
                int rnNum = rnd.Next(shiftDurationInHrs - Constants.SURVEY_WINDOW_IN_HRS);
                survey.SysGenRandomNumber = rnNum; //generate 0 to 4
                survey.SurveyWindowStartDateTime = start.AddHours(rnNum);
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
                survey.SurveyWindowStartDateTime = start.AddHours(rnNum);
                survey.SurveyWindowEndDateTime = survey.SurveyWindowStartDateTime.Value.AddHours(Constants.SURVEY_WINDOW_IN_HRS);

                survey.SurveyWindowStartDateTimeUtc = survey.SurveyWindowStartDateTime.Value.AddHours(offSetUtc);
                survey.SurveyWindowEndDateTimeUtc = survey.SurveyWindowEndDateTime.Value.AddHours(offSetUtc);


                survey.SurveyDescription = survey.SurveyWindowStartDateTime.Value.ToString("dd MMM yyyy hh:mm tt") + " -  " + survey.SurveyWindowEndDateTime.Value.ToString("dd MMM yyyy hh:mm tt");

            }


            survey.SurveyExpiryDateTime = survey.SurveyWindowEndDateTime.Value.AddHours(Constants.SURVEY_EXPIRY_AFTER_HRS);
            survey.SurveyExpiryDateTimeUtc = survey.SurveyExpiryDateTime.Value.AddHours(offSetUtc);



            db.Surveys.Add(survey);
            return await db.SaveChangesAsync();
        }


        public async Task<ActionResult> BtnMassCreate()
        {
            //var currentSurveys = db.Surveys.Select(x=>new { SurveyId = x.Id, ProfileId = x.ProfileId, RosteItemId = x.RosterItemId  });
            var currSurveyRosterIds = await db.Surveys.ToListAsync();

            //pick rosters 
            //var allRosterIds = await db.RosterItems.ToListAsync();
            var allRosterIds = await db.ProfileRosters.ToListAsync();

            var notInListRosterIds = allRosterIds.Where(p => !currSurveyRosterIds.Any(p2 => p2.RosterItemId == p.Id));


            foreach (var r in notInListRosterIds)
            {
                await CreateSurveyAuto(r.ProfileId, r.Id, r.Start, r.End);
            }

            return RedirectToAction("Index");
        }


        public async Task<ActionResult> BtnMassEmail()
        {
            //Commented since Mass Email modifications require ample of changes
            //var surveys = db.Surveys
            //    .Where(x => x.SurveyProgressNext == Constants.StatusSurveyProgress.New.ToString())
            //    .Select(x => new {
            //        x.Id,
            //        x.ProfileId,
            //        x.SurveyWindowStartDateTime,
            //        x.SurveyWindowEndDateTime,
            //        x.Profile.Name,
            //        x.Profile.LoginEmail
            //    })
            //    .ToList();

            
            ////if (!surveyId.HasValue)
            ////{
            ////    return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ////}
            ////if (profile == null)
            ////{
            ////    return HttpNotFound();
            ////}

            //foreach (var k in surveys)
            //{
            //    Guid uid = Guid.NewGuid();

            //    SurveyInvitationEmailDto e = new SurveyInvitationEmailDto();
            //    e.ToEmail = StringCipher.DecryptRfc2898(k.LoginEmail);
            //    e.RecipientName = k.Name;

            //    e.Link = GetBaseURL() + "Survey3/Index?id=" + uid.ToString();


            //    e.SurveyWindowStartDate = k.SurveyWindowStartDateTime.Value.ToString("dd-MMM-yyyy");
            //    e.SurveyWindowEndDate = k.SurveyWindowEndDateTime.Value.ToString("dd-MMM-yyyy");
            //    e.SurveyWindowStartTime = k.SurveyWindowStartDateTime.Value.ToString("hh:mm tt");
            //    e.SurveyWindowEndTime = k.SurveyWindowEndDateTime.Value.ToString("hh:mm tt");



            //    int? jobId = await schedulerSvc.StartRecurrentSurveyEmail(e, 0.0);

            //    if (jobId != 0)
            //    {
            //        await surveyService.ResetSurvey(k.Id, uid, jobId, sendSurveySmsJobId: null);
            //    }
            //    else
            //    {
            //        throw new Exception("Survey Id:" + k.Id + ", Hangfire jobid is zero error!");
            //    }
            //}
            return RedirectToAction("Index");
        }
        #endregion


    }
}