using SANSurveyWebAPI.BLL;
using SANSurveyWebAPI.ViewModels.Web;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SANSurveyWebAPI.Controllers
{
    public class NotificationsController : BaseController
    {
        private NotificationService notificationSvc;


        public NotificationsController()
        {
            this.notificationSvc = new NotificationService();
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}
            base.Dispose(disposing);
        }


        public async Task<ActionResult> Index()
        {
            NotificationListVM v = new NotificationListVM();

            
            v.notifications = await notificationSvc.GetNotificationList(GetBaseURL());


            if (Request.IsAjaxRequest())
            {
                return PartialView(v);

            }
            return View(v);

        }


      



    }
}