using Kendo.Mvc.UI;
using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.ViewModels.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SANSurveyWebAPI.Controllers;

namespace SANSurveyWebAPI.Areas.Admin.Controllers
{

    /* NOTE: Used in conjunction with telerik Scheduler implemented in Admin */

    #region deprecated

    [Authorize]
    public class RosterController : BaseController
    {
        private RosterItemService rosterSvc;
        private CalendarService calendarSvc;
        private NotificationService notificationSvc;

        public RosterController()
        {
            this.rosterSvc = new RosterItemService();
            this.calendarSvc = new CalendarService();
            this.notificationSvc = new NotificationService();
        }

        protected override void Dispose(bool disposing)
        {
            rosterSvc.Dispose();
            notificationSvc.Dispose();
            calendarSvc.Dispose();
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Index()
        {
            RosterItemViewModel v = new RosterItemViewModel();

            if (Request.IsAjaxRequest())
                return PartialView(v);
            return View(v);
        }

        public virtual JsonResult Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(rosterSvc.GetAll());
        }

        public virtual JsonResult ReadByProfileId([DataSourceRequest]DataSourceRequest request, int profileId)
        {
            return Json(rosterSvc.GetAllByProfileId(profileId));
        }

        public virtual JsonResult Create([DataSourceRequest]DataSourceRequest request, RosterItemViewModel rosterVM)
        {
            if (ModelState.IsValid)
            {
                rosterSvc.Insert(rosterVM, ModelState);
            }
            return Json(new[] { rosterVM });
        }


        public virtual JsonResult Update([DataSourceRequest]DataSourceRequest request, RosterItemViewModel rosterVM)
        {
            if (ModelState.IsValid)
            {
                rosterSvc.Update(rosterVM, ModelState);
            }

            return Json(new[] { rosterVM });
        }

        public virtual JsonResult Destroy([DataSourceRequest]DataSourceRequest request, RosterItemViewModel rosterVM)
        {
            if (ModelState.IsValid)
            {
                rosterSvc.Delete(rosterVM, ModelState);
            }
            return Json(new[] { rosterVM });
        }

    }

    #endregion

}