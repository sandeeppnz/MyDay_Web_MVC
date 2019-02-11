using System;
using System.Collections.Generic;
using SANSurveyWebAPI.Models.Api;
using SANSurveyWebAPI.ViewModels.Web;
using System.Threading.Tasks;

namespace SANSurveyWebAPI.BLL
{
    public class NotificationService
    {
        private ApplicationDbContext db;

        private CalendarService calendarSvc;
        private SurveyService surveySvc;



        public NotificationService(ApplicationDbContext context)
        {
            db = context;
            this.calendarSvc = new CalendarService();
            this.surveySvc = new SurveyService();
        }


        public void Dispose()
        {
            calendarSvc.Dispose();
            surveySvc.Dispose();
            db.Dispose();
        }

        public NotificationService()
            : this(new ApplicationDbContext())
        {
        }

        private async Task<bool> CheckUpdateRoster()
        {
            //if the roster is empty for next two weeks
            return await calendarSvc.IsNeedRosterUpdate();
        }

        private async Task<List<string>> GetExpiringSurvey()
        {
            return await surveySvc.GetExpiringSoon();
        }


        private async Task<List<string>> CheckIncompleteSurvey()
        {
            return await surveySvc.GetIncompleteSurvey();

        }

        public async Task<List<string>> CheckNewSurvey()
        {
            return await surveySvc.GetNewSurvey();
        }


        public async Task<List<NotificationVM>> GetNotificationList(string baseUrl)
        {
            List<NotificationVM> list = new List<NotificationVM>();


            if (await CheckUpdateRoster())
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "RED";
                v.Message = Constants.Notification_UpdateRoster;
                v.Link = baseUrl + "Calendar/List/"+ DateTime.UtcNow.ToString("yyyy-MM");
                list.Add(v);
            }

            List<string> expiringSurveys = await GetExpiringSurvey();
            foreach (var s in expiringSurveys)
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "RED";
                v.Message = Constants.Notification_ExpiringSurvey;
                v.Link = baseUrl + "Survey3/Index?id=" + s;
                list.Add(v);
            }

            List<string> incompleteSurveys = await CheckIncompleteSurvey();
            foreach (var s in incompleteSurveys)
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "ORANGE";
                v.Message = Constants.Notification_NewSurvey;
                v.Link = baseUrl + "Survey3/Index?id=" + s;
                list.Add(v);
            }

            List<string> newSurveys = await CheckNewSurvey();
            foreach (var s in newSurveys)
            {
                NotificationVM v = new NotificationVM();
                v.FlagColor = "";
                v.Message = Constants.Notification_NewSurvey;
                v.Link = baseUrl + "Survey3/Index?id=" + s;
                list.Add(v);
            }

            return list;
        }
       
    }
}