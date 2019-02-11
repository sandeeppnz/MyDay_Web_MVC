using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Controllers
{

    /*
     
        Required by the Calendar/Roster 
         
         
         
         */

    public class CalendarController : BaseController
    {

        private CalendarService calendarSvc;
        private NotificationService notificationSvc;
        private SurveyService mydaysurveysSvc;
        
        public CalendarController()
        {
            this.calendarSvc = new CalendarService();
            this.notificationSvc = new NotificationService();
            this.mydaysurveysSvc = new SurveyService();
        }

        protected override void Dispose(bool disposing)
        {
            calendarSvc.Dispose();
            notificationSvc.Dispose();
            mydaysurveysSvc.Dispose();
            base.Dispose(disposing);
        }


        //To read all Calendar events
        public virtual JsonResult Read(string profileId)
        {
            if (!string.IsNullOrEmpty(profileId))
            {
                return Json(calendarSvc.GetAll(int.Parse(profileId)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(calendarSvc.GetAll(null), JsonRequestBehavior.AllowGet);
            }
        }


        //To read a Calendar event
        [HttpGet]
        public virtual JsonResult Get(string id)
        {
            return Json(calendarSvc.Get(id), JsonRequestBehavior.AllowGet);
        }





        //To delete a Calendar event
        public virtual async Task<ActionResult> Delete(string id)
        {

            string result = await calendarSvc.Delete(id);

            //if (result > 0 )
            //{
            //return RedirectToAction("Index","Calendar");

            if (result == "ErrorDelete" || result == "ErrorSurveyCreated")
            {
                return Json(new { Success = false, Result = result }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Success = true, Result = result }, JsonRequestBehavior.AllowGet);
            //}


            //return Json(, JsonRequestBehavior.AllowGet);
        }


        #region deprecated
        //[HttpPost]
        //public virtual async Task<ActionResult> SaveEdit(string id, string startStr, string endStr)
        //{


        //    try
        //    {
        //        DateTime st = Convert.ToDateTime(startStr);

        //        var result = await calendarSvc.SaveEditAsync(id, startStr, endStr, GetBaseURL());

        //        var str = st.ToString("yyyy-MM");

        //        return Json(new { Success = true, Result = str }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }

        //}
        #endregion


        //To create a new Calendar event
        [HttpPost]
        public virtual async Task<ActionResult> SaveEditParam(string id, string startStr, string endStr, string profileId, string profileEmail, 
                            string profileOffset, string profileName, string clientInitials)
        {
            

            try
            {
                DateTime st = Convert.ToDateTime(startStr);

                var result = await calendarSvc.SaveEditAsyncParam(id, startStr, endStr, GetBaseURL(), 
                                                    profileId, profileEmail, profileOffset, profileName, clientInitials);

                var str = st.ToString("yyyy-MM");

                return Json(new { Success = true, Result = str }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public virtual async Task<ActionResult> SaveNewParam(string startStr, string endStr, string profileId, string profileEmail, 
            string profileOffset, string profileName, string clientInitials)
        {
            

            try
            {

                DateTime st = Convert.ToDateTime(startStr);

                var result = await calendarSvc.SaveNewAsyncParam(startStr, endStr, GetBaseURL(), profileId, profileEmail, 
                                                        profileOffset, profileName, clientInitials);

                var str = st.ToString("yyyy-MM");

                return Json(new { Success = true, Result = str }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
            }

        }

        #region deprecated
        //[HttpPost]
        //public virtual async Task<ActionResult> SaveNew(string startStr, string endStr)
        //{


        //    try
        //    {

        //        DateTime st = Convert.ToDateTime(startStr);

        //        var result = await calendarSvc.SaveNewAsync(startStr, endStr, GetBaseURL());

        //        var str = st.ToString("yyyy-MM");

        //        return Json(new { Success = true, Result = str }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Success = false }, JsonRequestBehavior.AllowGet);
        //    }

        //}
        #endregion


        //public virtual JsonResult Update()
        //{
        //    return Json(svc.GetAll(), JsonRequestBehavior.AllowGet);
        //}

        //public virtual JsonResult Create()
        //{
        //    return Json(svc.GetAll(), JsonRequestBehavior.AllowGet);
        //}






        /*
         User Home page views
             
             */

        // GET: Calendar
        //List current month
        public ActionResult Index()
        {
            return RedirectToAction("List");
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView();

            //}
            //return View();
        }

    

        //Used in User Home
        [HttpGet]
        public async Task<ActionResult> List(string id)
        {
            CalendarListViewModel v = new CalendarListViewModel();
            v.notifications = new List<NotificationVM>();

            
            v.notifications = await notificationSvc.GetNotificationList(GetBaseURL());

            v.YearMonth = id;

            if (Session["ProfileIdSession"] == null)
            {
                var loggedInProfile = calendarSvc.GetCurrentLoggedInProfile();

                if (loggedInProfile == null)
                {
                    //await LogSessionError(Constants.PageName.Registration_Wellbeing);
                    //return RedirectToAction("SessionError","Home");
                    return RedirectToAction("Login", "Account");
                }
                //Session["ProfileId"] = loggedInProfile.Id;
                Session["ProfileIdSession"] = (ProfileDto) loggedInProfile;
             
            }

            ProfileDto pDto = (ProfileDto) Session["ProfileIdSession"];
            v.ProfileId = pDto.Id;
            v.ProfileName = pDto.Name;
            v.ProfileEmail = pDto.LoginEmail;
            v.ProfileOffset = pDto.OffSetFromUTC;


            //v.ProfileId = (int) Session["ProfileId"];

            v.totalMyDaysurveys = await mydaysurveysSvc.GetMyDayCompletedSurveys(v.ProfileId);

            if (v.totalMyDaysurveys.Count >= 3)
            { v.freezeRoster = true; }
            else { v.freezeRoster = false; }

            if (Request.IsAjaxRequest())
            {
                return PartialView(v);
            }
            return View(v);
        }

    }
}