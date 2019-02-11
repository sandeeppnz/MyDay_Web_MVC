using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.Controllers;
using SANSurveyWebAPI.DTOs;
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
    public class BaselineSurveyController : BaseController
    {
        // View the progress of the baseline/Registration 
        // Send the invitation 
        // ProfileId, Reg Email Id, Registration Status, Reg Start Date, Reg End Date, Uid, Comments, Link to page stat

        private AdminService adminService;
        private ProfileService profileSvc;

        private JobService jobService;

        
        public BaselineSurveyController()
        {
            this.adminService = new AdminService();
            this.profileSvc = new ProfileService();
            this.jobService = new JobService();
        }

        protected override void Dispose(bool disposing)
        {
            adminService.Dispose();
            profileSvc.Dispose();
            jobService.Dispose();
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Index()
        {
            var profiles = adminService.GetAllProfiles();
            return View(profiles);

        }

        public async Task<ActionResult> Details(int? id)
        {
            //profile id
            if (id.HasValue)
            {
                var profiles = adminService.GetPageStatsByProfileId(id.Value);
                return View(profiles);
            }
            return null;

        }


        #region Single Email
        public async Task<ActionResult> RegistrationSurveyInvite(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Trigger via Hangfire
            jobService.CreateJobAsync(JobName.RegisterBaselineSurveyEmail.ToString(),
                JobType.Email.ToString(), id.Value, JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);



            //var k = adminService.GetAllProfiles()
            //    .Where(p => p.Id == id.Value)
            //    .SingleOrDefault();

            //Guid uid = Guid.NewGuid();
            //RegistrationInvitationEmailDto e = new RegistrationInvitationEmailDto();
            //e.ToEmail = k.LoginEmail;
            //e.RecipientName = k.Name;
            //e.Link = GetBaseURL() + "Register/Index" + "/" + uid;


            //if (k != null)
            //{
            //    await profileSvc.ResetProfile(k.Id, uid, emailJobId: null, smsJobId: null);

            //    //No Hangfire
            //    await Task.Run(() =>
            //    {
            //        PostalEmail.RegisterAndBaseline(e);
            //    });
            //}
            //else
            //{
            //    return HttpNotFound();
            //}


            return RedirectToAction("Index");
        }


        public async Task<ActionResult> ExitSurveyInvite(int? id)
        {
            if (!id.HasValue)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Trigger via Hangfire
            jobService.CreateJobAsync(JobName.ExitSurveyEmail.ToString(),
                JobType.Email.ToString(), id.Value, JobMethod.Auto.ToString(), GetBaseURL(), string.Empty);



            //var k = adminService.GetAllProfiles()
            //    .Where(p => p.Id == id.Value)
            //    .SingleOrDefault();

            //Guid uid = Guid.NewGuid();
            //RegistrationInvitationEmailDto e = new RegistrationInvitationEmailDto();
            //e.ToEmail = k.LoginEmail;
            //e.RecipientName = k.Name;
            //e.Link = GetBaseURL() + "Register/Index" + "/" + uid;


            //if (k != null)
            //{
            //    await profileSvc.ResetProfile(k.Id, uid, emailJobId: null, smsJobId: null);

            //    //No Hangfire
            //    await Task.Run(() =>
            //    {
            //        PostalEmail.RegisterAndBaseline(e);
            //    });
            //}
            //else
            //{
            //    return HttpNotFound();
            //}


            return RedirectToAction("Index");
        }

        #endregion

    }
}