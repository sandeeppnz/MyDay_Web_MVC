using Hangfire;
using SANSurveyWebAPI.DAL;
using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using SANSurveyWebAPI.ViewModels.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SANSurveyWebAPI.BLL
{
    public class RootService
    {
        readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static bool UpdateDatabase = true;


        private HangfireScheduler schedulerService;


        public RootService()
        {
            this.schedulerService = new HangfireScheduler();

        }



        public async Task CreateFeedback(
          EmailFeedbackViewModel v)
        {
            //add a record to db

            Feedback e = new Feedback();
            e.Channel = "Website";
            e.CreatedDateTimeUtc = DateTime.UtcNow;

            e.Email = v.EmailAddress;
            e.ContactNumber = v.PhoneNumber;
            e.PreferedContact = v.PreferedContact;
            e.PreferedTime = v.PreferedTime;
            e.Message = v.Message;
         

            _unitOfWork.FeedbackRespository.Insert(e);
            _unitOfWork.SaveChanges();

        }

        public void Dispose()
        {
            schedulerService.Dispose();
            _unitOfWork.Dispose();
        }
    }
}