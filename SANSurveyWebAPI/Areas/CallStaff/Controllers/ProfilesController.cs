using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SANSurveyWebAPI.Areas.CallStaff.BLL;
using SANSurveyWebAPI.Areas.CallStaff.ViewModels;
using SANSurveyWebAPI.Controllers;
using SANSurveyWebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Areas.CallStaff.Controllers
{
    [Authorize(Roles = "CallStaff")]
    public class ProfilesController : BaseController
    {
        private readonly ProfileService profileService;

        public ProfilesController()
        {
            profileService = new ProfileService();
        }

        public ActionResult Index()
        {
            //Get the Profiles into a grid
            return View();
        }


        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProfileDto dto = profileService.GetProfileById(id.Value);

            ProfileViewVM v = new ProfileViewVM {
                Id = dto.Id,
                Name = dto.Name,
                LoginEmail = dto.LoginEmail,
                Consent = false,
                RegistrationStatus = dto.RegistrationProgressNext,
                RegisteredDateTimeUtc = dto.RegisteredDateTimeUtc,
            };






            v.Surveys = new List<SurveyProfileVM>();


            if(v == null)
                return HttpNotFound();

            //ViewBag.RosterId = new SelectList(db.RosterItems, "Id", "Start", survey.RosterItemId);
            //ViewBag.UserId = new SelectList(db.Profiles, "Id", "EmailAddress", survey.ProfileId);
            return View(v);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> View(ProfileDto dto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //string name = profile.Name;
        //        //profile.Name = name;

        //        db.Entry(survey).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.RosterId = new SelectList(db.RosterItems, "Id", "Start", survey.RosterItemId);
        //    ViewBag.UserId = new SelectList(db.Profiles, "Id", "EmailAddress", survey.ProfileId);
        //    return View(survey);
        //}





        public ActionResult Profiles_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(profileService.GetProfiles().ToDataSourceResult(request));
        }


        protected override void Dispose(bool disposing)
        {
            profileService.Dispose();
            base.Dispose(disposing);
        }


    }
}