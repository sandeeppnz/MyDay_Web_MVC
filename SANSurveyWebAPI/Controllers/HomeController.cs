using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.ViewModels.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Controllers
{

    /*
     
        User Home page
         
         
         */



    [Authorize]
    //[AllowAnonymous]

    public class HomeController : BaseController
    {
        private ProfileService profileService;
        private UserHomeService userHomeService;
        private PageStatService pageStatSvc;

        private async Task LogSessionError(Constants.PageName page)
        {
            await pageStatSvc.Insert(null, null, null, false, page, Constants.PageAction.Post, Constants.PageType.SESSION_ERROR);
        }

        public HomeController()
        {
            this.userHomeService = new UserHomeService();
            this.profileService = new ProfileService();
            this.pageStatSvc = new PageStatService();
        }

        protected override void Dispose(bool disposing)
        {
            userHomeService.Dispose();
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }



        public async Task<ActionResult> Index()
        {
            //Redirect to Admin Roles
            if (Request.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            if (Request.IsAuthenticated && User.IsInRole("DataScientist"))
            {
                return RedirectToAction("Index", "Home", new { area = "DataScientist" });
            }

            if (Request.IsAuthenticated && User.IsInRole("CallStaff"))
            {
                return RedirectToAction("Index", "Home", new { area = "CallStaff" });
            }

            if (Request.IsAuthenticated && User.IsInRole("ClientAdmin"))
            {
                return RedirectToAction("Index", "Home", new { area = "ClientAdmin" });
            }


            //Redirect to Registration if not completed
            ProfileDto loggedInProfile = new ProfileDto();
            if (Session["ProfileId"] == null)
            {
                loggedInProfile = userHomeService.GetCurrentLoggedInProfile();
                if (loggedInProfile == null)
                {
                    await LogSessionError(Constants.PageName.UserHome_Index);
                    return RedirectToAction("SessionError");
                }
                Session["ProfileId"] = loggedInProfile.Id;
            }

            int profileId = (int) Session["ProfileId"];

            if (loggedInProfile == null || loggedInProfile.Id <= 0)
            {
                loggedInProfile = userHomeService.GetProfileById(profileId);               
            }

            if (loggedInProfile != null)
            {
                if (loggedInProfile.RegistrationProgressNext.ToString() != "Completed")
                {
                    if (loggedInProfile.RegistrationProgressNext.ToString() != "Signup")
                    {
                        return RedirectToAction("Signup", "Account");
                    }
                    return RedirectToAction(loggedInProfile.RegistrationProgressNext.ToString(), "Register");
                }
            }


            //If User Role
            HomeMySurveyListVM v = new HomeMySurveyListVM();
            v.notifications = new List<NotificationVM>();
            v.surveys = new List<HomeMySurveyVM>();

            //int profileId = 0;
            //if (Session["ProfileId"] == null)
            //{
            //    Session["ProfileId"] = profileService.GetCurrentProfileIdNonAsync();
            //}
            //else
            //{
            //    profileId = (int) Session["ProfileId"];
            //}

            
            v.notifications = await userHomeService.GetNotificationList(GetBaseURL(), profileId);

            if (loggedInProfile.ClientInitials.ToLower().ToString() == "wam")
            { v.surveys = await userHomeService.GetWAMHomeMySurveysList(GetBaseURL(), profileId); }
            else
            { v.surveys = await userHomeService.GetHomeMySurveysList(GetBaseURL(), profileId); }
            

            ViewBag.Title = "Home Page";

            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }

            return View(v);
        }

        public async Task<ActionResult> Home()
        {

            int profileId = 0;
            if (Session["ProfileId"] == null)
            {
                Session["ProfileId"] = profileService.GetCurrentProfileIdNonAsync();
            }
            else
            {
                profileId = (int) Session["ProfileId"];
            }

            HomeMySurveyListVM v = new HomeMySurveyListVM();
            v.notifications = new List<NotificationVM>();
            v.surveys = new List<HomeMySurveyVM>();


            
            v.notifications = await userHomeService.GetNotificationList(GetBaseURL(), profileId);
            v.surveys = await userHomeService.GetHomeMySurveysList(GetBaseURL(), profileId);

            ViewBag.Title = "Home Page";

            return PartialView("Index");
        }



        [AllowAnonymous]
        public ActionResult SessionError()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();

            }
            return View();
        }
     
    }
}
