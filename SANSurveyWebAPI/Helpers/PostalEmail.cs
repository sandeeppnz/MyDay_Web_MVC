using System.Web.Mvc;
using SANSurveyWebAPI.Models;
using System.IO;
using System.Web.Hosting;
using Postal;
using System.ComponentModel;
using SANSurveyWebAPI.DTOs;



namespace SANSurveyWebAPI.BLL
{
    /*
     * Using MVC.Postal
     * 
     */


    public class PostalEmail
    {
        #region deprecated
        //public EmailService()
        //{
        //    var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
        //    var engines = new ViewEngineCollection();
        //    engines.Add(new FileSystemRazorViewEngine(viewsPath));
        //    var emailService = new Postal.EmailService(engines);
        //}
        #endregion
        public static void SendEmailStayAlive(string url)
        {
            var email = new StayAliveNotification
            {
                ToEmail = Constants.AdminEmail,
                ServerLink = url,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }
        public static void SendError(EmailDto e)
        {
            //Added try-catch loop to exclude the exception thrown by application on 2/08/2017 --by Bharati
            try
            {
                var email = new ErrorRegistrationEmail
                {
                    ToEmail = Constants.AdminEmail,
                    AppName = SANSurveyWebAPI.Constants.AppName,
                    Name = e.RecipientName,
                    Message = e.Link
                };
                email.SendAsync();
            }
            catch (System.Exception ex) { }
        }
        public static void WebsiteContactUs(WebsiteContactUsEmailDto e)
        {
            var email = new WebsiteContactUsEmail
            {
                To = e.ToEmail,
                AppName = SANSurveyWebAPI.Constants.AppName,
                Name = e.RecipientName,
                Message = e.Message,
                ContactName = e.Name,
                PreferredContact = e.PreferredContact,
                PreferredTime = e.PreferredTime,
                ReturnPhone = e.ReturnPhone,
                ReturnEmail = e.ReturnEmail
            };
            email.SendAsync();
        }
        public static void SignupCompleted(SignupCompletedEmailDto e)
        {
            var email = new SignupCompletedEmail
            {
                To = e.ToEmail,
                AppName = SANSurveyWebAPI.Constants.AppName,
                Name = e.RecipientName,
                Message = e.Link
            };
            email.SendAsync();
        }

        public static void WAMSignupCompleted(WarrenMahonySignupCompletedEmailDto e)
        {
            var email = new WarrenMahonySignupCompletedEmail
            {
                To = e.ToEmail,
                AppName = SANSurveyWebAPI.Constants.AppName,
                Name = e.RecipientName,
                Message = e.Link
            };
            email.SendAsync();
        }



        //For Junior Doctors
        public static void RegisterAndBaseline(RegistrationInvitationEmailDto e)
        {
            var email = new SigupInvitationEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                AppName = Constants.AppName,
                Incentive = e.Incentive
            };

            email.SendAsync();
        }
        //For Warren and Mahony
        public static void WarrenMahonyBaseline(RegistrationInvitationEmailDto e)
        {
            var email = new WarrenMahonySignUpEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                AppName = Constants.AppName,
                Incentive = e.Incentive
            };

            email.SendAsync();
        }
        public static void WarenMahonyReminderBaseline(RegistrationInvitationEmailDto e)
        {
            var email = new WarrenMahonyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                AppName = Constants.AppName,
                Incentive = e.Incentive
            };
            email.SendAsync();
        }
        public static void ReminderBaseline(RegistrationInvitationEmailDto e)
        {
            var email = new ReminderInvitationEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                AppName = Constants.AppName,
                Incentive = e.Incentive
            };
            email.SendAsync();
        }
        public static void ExitSurvey(ExitSurveyInvitationEmailDto e)
        {
            var email = new ExitSurveyEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }


        [DisplayName("ShiftStartReminderEmail;{1}")]
        public static void ShiftStartReminder(ShiftStartReminderEmailDto e, string msg)
        {
            var email = new ShiftStartReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                ShiftStartDate = e.ShiftStartDate,
                ShiftEndDate = e.ShiftEndDate,
                ShiftStartTime = e.ShiftStartTime,
                ShiftEndTime = e.ShiftEndTime,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }

        [DisplayName("StartRecurrentSurveyEmail;{1}")]
        public static void StartRecurrentSurvey(SurveyInvitationEmailDto e, string msg)
        {
            var email = new SurveyInvitationEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();


        }

        
        [DisplayName("CompleteRecurrentSurveyEmail;{1}")]
        public static void CompleteRecurrentSurvey(CompleteSurveyReminderEmailDto e, string msg)
        {
            var email = new CompleteSurveyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }

        [DisplayName("ExpiringSoonNotStartedSurveyEmail;{1}")]
        public static void ExpiringSoonNotStartedSurvey(ExpiringSoonNotStartedSurveyReminderEmailDto e, string msg)
        {
            var email = new ExpiringSoonNotStartedSurveyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDateTime = e.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = e.SurveyWindowEndDateTime,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }

        [DisplayName("ExpiringSoonStartButNotCompletedSurvey;{1}")]
        public static void ExpiringSoonStartNotCompletedSurvey(ExpirinSoonNotCompletedSurveyReminderEmailDto e, string msg)
        {
            var email = new ExpiringSoonStartButNotCompletedSurveyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDateTime = e.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = e.SurveyWindowEndDateTime,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }
        //For Warren and Mahony
        [DisplayName("ShiftReminderWarrenMahony;{1}")]
        public static void ShiftReminderWarrenMahony(ShiftReminderWarrenMahonyEmailDto e, string msg)
        {
            var email = new ShiftReminderWarrenMahonyEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                Link = e.Link,
                ShiftStartDate = e.ShiftStartDate,
                ShiftEndDate = e.ShiftEndDate,
                ShiftStartTime = e.ShiftStartTime,
                ShiftEndTime = e.ShiftEndTime,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }
        [DisplayName("StartWarrenSurveyInvitation;{1}")]
        public static void StartWarrenSurveyInvitation(StartWarrenSurveyInvitationEmailDto e, string msg)
        {
            var email = new StartWarrenSurveyInvitationEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }

        [DisplayName("CompleteWarrenSurveyReminderEmailDto;{1}")]
        public static void CompleteWarrenSurveyReminderSurvey(CompleteWarrenSurveyReminderEmailDto e, string msg)
        {
            var email = new CompleteWarrenSurveyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }


        [DisplayName("ExpiringSoonNotStartedWarrenSurveyReminder;{1}")]
        public static void ExpiringSoonNotStartedWarrenSurveyReminder(ExpiringSoonNotStartedWarrenSurveyReminderEmailDto e, string msg)
        {
            var email = new ExpiringSoonNotStartedWarrenSurveyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDateTime = e.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = e.SurveyWindowEndDateTime,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }
        [DisplayName("ExpirinSoonNotCompletedWarrenSurveyReminder;{1}")]
        public static void ExpirinSoonNotCompletedWarrenSurveyReminder(ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto e, string msg)
        {
            var email = new ExpirinSoonNotCompletedWarrenSurveyReminderEmail
            {
                ToEmail = e.ToEmail,
                RecipientName = e.RecipientName,
                SurveyWindowStartDateTime = e.SurveyWindowStartDateTime,
                SurveyWindowEndDateTime = e.SurveyWindowEndDateTime,
                SurveyWindowStartDate = e.SurveyWindowStartDate,
                SurveyWindowEndDate = e.SurveyWindowEndDate,
                SurveyWindowStartTime = e.SurveyWindowStartTime,
                SurveyWindowEndTime = e.SurveyWindowEndTime,
                Link = e.Link,
                AppName = Constants.AppName
            };

            email.SendAsync();
        }
    }
}