using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.Controllers;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static SANSurveyWebAPI.Constants;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{
    public class RecurrentSurveyController : BaseController
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();


        private AdminService adminService;
        private ProfileService profileService;
        private JobService jobService = new JobService();


        public RecurrentSurveyController()
        {
            this.adminService = new AdminService();
            this.profileService = new ProfileService();
        }

        protected override void Dispose(bool disposing)
        {
            adminService.Dispose();
            profileService.Dispose();
            base.Dispose(disposing);
        }

        // GET: Admin/RecurrentSurvey
        public ActionResult Index()
        {
            Session["SelectedProfileId"] = null;
            var profiles = adminService.GetRecurrentSurveys();
            return View(profiles);
        }


        public async Task<ActionResult> RosterDetails(int? id)
        {
            // id = profileId
            if (id.HasValue)
            {

                Session["SelectedProfileId"] = id.Value;

                //ViewBag.ProfileId = id.Value;

                var rosters = adminService.GetRecurrentSurveyDetails(id.Value);
                return View(rosters);
            }
            return null;

        }



        #region ProfileRoster Create, Delete
        public async Task<ActionResult> CreateProfileRoster(int? id)
        {
            ViewBag.ProfileId = id.Value;
            if (id.HasValue)
            {
                RecurrentSurveyCreateProfileRosterVM v = new RecurrentSurveyCreateProfileRosterVM();
                var profile = adminService.GetProfileById(id.Value);

                if (profile != null)
                {
                    Session["SelectedProfileId"] = id.Value;


                    v.ProfileId = id.Value;
                    v.OffSetFromUTC = profile.OffSetFromUTC;
                    v.Start = DateTime.Now;
                    v.End = DateTime.Now;
                }
                return View(v);
            }
            return RedirectToAction("RosterDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProfileRoster(RecurrentSurveyCreateProfileRosterVM v)
        {
            if (ModelState.IsValid)
            {
                //Using a double VM 
                RosterItemViewModelRevised p = new RosterItemViewModelRevised();
                p.ProfileId = v.ProfileId;
                p.Start = v.Start;
                p.End = v.End;
                adminService.CreateProfileRoster(p);
                return RedirectToAction("RosterDetails", new { id = v.ProfileId });
            }
            return View(v);
        }


        public async Task<ActionResult> DeleteProfileRoster(ProfileRosterDto p)
        {
            if (p.Id == null || p.Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfileRosterDto profileDto = null;
            profileDto = adminService.GetProfileRosterByRosterId(p.Id);

            //ViewBag.ProfileId = profileDto.ProfileId;

            if (profileDto == null)
            {
                return HttpNotFound();
            }
            return View(profileDto);
        }

        [HttpPost, ActionName("DeleteProfileRoster")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(ProfileRosterDto p)
        {
            //int profileId = ViewBag.ProfileId;
            adminService.DeleteProfileRoster(p.Id);
            return RedirectToAction("RosterDetails", new { id = @Session["SelectedProfileId"] });
        }
        #endregion



        #region Survey Create, Delete, View 
        public async Task<ActionResult> CreateSurvey(int? id)
        {
            if (id.HasValue)
            {
                RecurrentSurveyCreateSurveyVM v = new RecurrentSurveyCreateSurveyVM();
                var roster = adminService.GetProfileRosterByRosterId(id.Value);

                v.ProfileId = roster.ProfileId;
                v.ProfileRosterId = roster.Id;

                return View(v);
            }
            return null;

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSurvey(RecurrentSurveyCreateSurveyVM v)
        {
            if (ModelState.IsValid)
            {
                int profileId = v.ProfileId;
                int profileRosterId = v.ProfileRosterId;
                string baseUrl = GetBaseURL();
                string Url = baseUrl + "Calendar/List";

                var profileRoster = _unitOfWork.ProfileRosterRespository.GetByID(profileRosterId);


                CurrentProfile currProfile = GetProfile(profileId.ToString(), string.Empty, string.Empty, string.Empty);

                var profileRosterDto = ObjectMapper.GetProfileRosterDto(profileRoster);



                //Schedule Shift Reminder Email
                int? shiftReminderEmailJobId = await jobService.CreateJobAsync(JobName.ShiftReminderEmail.ToString(),
                   JobType.Email.ToString(), profileId,
                   JobMethod.Auto.ToString(),
                   string.Empty, Url, currProfile, profileRosterDto
               );

                //Schedule Create Survey Job
                int? createSurveyJobId = await jobService.CreateJobAsync(JobName.CreateSurveyJob.ToString(),
                  JobType.Method.ToString(), profileId,
                  JobMethod.Auto.ToString(), baseUrl, string.Empty, currProfile, profileRosterDto
              );
            }
            return RedirectToAction("RosterDetails", new { id = @Session["SelectedProfileId"] });
        }


        //Delete Survey
        public async Task<ActionResult> DeleteSurvey(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            SurveyDto surveyDto = null;
            surveyDto = adminService.GetSurveyById(id.Value);

            //ViewBag.ProfileId = surveyDto.ProfileId;

            if (surveyDto == null)
            {
                return HttpNotFound();
            }

            return View(surveyDto);
        }



        [HttpPost, ActionName("DeleteSurvey")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSurveyConfirmed(ProfileRosterDto p)
        {
            //int profileId = ViewBag.ProfileId;
            adminService.DeleteSurvey(p.Id);
            return RedirectToAction("RosterDetails", new { id = @Session["SelectedProfileId"] });
        }




        //View
        public async Task<ActionResult> ViewSurvey(int? id)
        {
            if (!id.HasValue) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            //Get the VM for the ViewSurvey
            SurveyDto surveyDto = adminService.GetSurveyById(id.Value);
            if (surveyDto == null) { return HttpNotFound(); }

            List<HangfireStateDto> shiftReminders = adminService.GetShiftReminderJobDetails(surveyDto);

            List<HangfireStateDto> startSurveyReminders = adminService.GetStartSurveyReminderJobDetails(surveyDto);

            List<HangfireStateDto> completeSurveyReminders = adminService.GetCompleteSurveyReminderJobDetails(surveyDto);

            List<HangfireStateDto> expiringSoonNotStartedReminders = adminService.GetExpiringSoonNotStartedSurveyReminderJobDetails(surveyDto);

            List<HangfireStateDto> expiringSoonNotCompletedReminders = adminService.GetExpiringSoonNotCompletedSurveyReminderJobDetails(surveyDto);


            AdminSurveyDetailsVM vm = new AdminSurveyDetailsVM();
            vm.Id = surveyDto.Id;
            vm.Uid = surveyDto.Uid;
            vm.ProfileId = surveyDto.ProfileId;
            vm.RosterItemId = surveyDto.RosterItemId;
            vm.CreatedDateTimeUtc = surveyDto.CreatedDateTimeUtc;
            vm.SurveyUserStartDateTimeUtc = surveyDto.SurveyUserStartDateTimeUtc;
            vm.SurveyUserCompletedDateTimeUtc = surveyDto.SurveyUserCompletedDateTimeUtc;
            vm.SurveyProgressNext = surveyDto.SurveyProgressNext;
            vm.SurveyDescription = surveyDto.SurveyDescription;
            vm.SurveyExpiryDateTime = surveyDto.SurveyExpiryDateTime;
            vm.SurveyExpiryDateTimeUtc = surveyDto.SurveyExpiryDateTimeUtc;

            vm.ShiftReminders = shiftReminders;
            vm.StartSurveyReminders = startSurveyReminders;
            vm.CompleteSurveyReminders = completeSurveyReminders;
            vm.ExpiringSoonNotStartedReminders = expiringSoonNotStartedReminders;
            vm.ExpiringSoonNotCompletedReminders = expiringSoonNotCompletedReminders;

            vm.JobLogShiftReminderEmail = adminService.GetShiftReminderJobLogDetails(surveyDto);
            vm.JobLogShiftReminderEmailList = adminService.GetShiftReminderJobLogDetailsList(surveyDto);



            vm.JobLogStartSurveyReminderEmail = adminService.GetStartSurveyReminderJobLogDetails(surveyDto);
            vm.JobLogStartSurveyReminderEmailList = adminService.GetStartSurveyReminderJobLogDetailsList(surveyDto);



            vm.JobLogCompleteSurveyReminderEmail = adminService.GetCompleteSurveyReminderJobLogDetails(surveyDto);
            vm.JobLogCompleteSurveyReminderEmailList = adminService.GetCompleteSurveyReminderJobDetailsList(surveyDto);


            vm.JobLogExpiringSoonSurveyNotStartedReminderEmail = adminService.GetExpiringSoonNotStartedSurveyReminderJobLogDetails(surveyDto);
            vm.JobLogExpiringSoonSurveyNotStartedReminderEmailList = adminService.GetExpiringSoonNotStartedSurveyReminderJobLogDetailsList(surveyDto);


            vm.JobLogExpiringSoonSurveyNotCompletedReminderEmail = adminService.GetExpiringSoonNotCompletedSurveyReminderJobLogDetails(surveyDto);
            vm.JobLogExpiringSoonSurveyNotCompletedReminderEmailList = adminService.GetExpiringSoonNotCompletedSurveyReminderJobLogDetailsList(surveyDto);


            return View(vm);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ViewSurvey(ProfileRosterDto p)
        {
            //int profileId = ViewBag.ProfileId;
            //adminService.DeleteSurvey(p.Id);
            return RedirectToAction("RosterDetails", new { id = @Session["SelectedProfileId"] });
        }




        #endregion

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
                var profileDto = profileService.GetProfileById(int.Parse(profileId));
                currProfile.ProfileId = int.Parse(profileId);
                currProfile.ProfileName = profileDto.Name;
                currProfile.ProfileEmailAddress = profileDto.LoginEmail;
                currProfile.OffsetFromUTC = profileDto.OffSetFromUTC;
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



        #region Trigger Jobs

        public async Task<ActionResult> ShiftReminder(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id.Value, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.ShiftReminderEmail.ToString(), JobType.Email.ToString(),
                 survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                 );


            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> StartSurvey(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id.Value, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.StartRecurrentSurveyEmail.ToString(), JobType.Email.ToString(),
                        survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                        );

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> CompleteSurvey(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id.Value, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.CompleteRecurrentSurveyEmail.ToString(), JobType.Email.ToString(),
               survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
               );

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ExpiringSoon1(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id.Value, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.ExpiringSoonRecurrentSurveyNotStartedEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ExpiringSoon2(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Survey survey;
            ProfileRosterDto rosterDto;
            CurrentProfile profile;
            GetObjectsForJob(id.Value, out survey, out rosterDto, out profile);

            jobService.CreateJobAsync(JobName.ExpiringSoonRecurrentSurveyNotCompletedEmail.ToString(), JobType.Email.ToString(),
                survey.ProfileId.Value, JobMethod.Manual.ToString(), GetBaseURL(), string.Empty, profile, rosterDto, survey
                );

            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }

        private void GetObjectsForJob(int? id, out Survey survey, out ProfileRosterDto rosterDto, out CurrentProfile profile)
        {
            int surveyId = id.Value;
            survey = _unitOfWork.SurveyRespository.GetByID(surveyId);
            var roster = _unitOfWork.ProfileRosterRespository.GetByID(survey.RosterItemId);
            rosterDto = ObjectMapper.GetProfileRosterDto(roster);
            profile = GetProfile(survey.ProfileId.ToString(), string.Empty, string.Empty, string.Empty);
        }
        #endregion






    }

}