using Postal;

namespace SANSurveyWebAPI.Models
{

    /*
     
        MVC Postal Email models


         */

    public class SurveyInvitationEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public string AppName { get; set; }
    }

    public class StayAliveNotification : Email
    {
        public string ToEmail { get; set; }
        public string ServerLink { get; set; }
        public string AppName { get; set; }
    }


    public class ShiftStartReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //login to update roster
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string ShiftStartDate { get; set; }
        public string ShiftEndDate { get; set; }
        public string AppName { get; set; }
    }


    public class CompleteSurveyReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }

        public string AppName { get; set; }
    }


    public class ExpiringSoonNotStartedSurveyReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }


        public string AppName { get; set; }
    }


    public class ExpiringSoonStartButNotCompletedSurveyReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }

        public string AppName { get; set; }
    }


    //Registration InvitationEmail
    public class SigupInvitationEmail: Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; }
        public string AppName { get; set; }
        public int Incentive { get; set; }
    }

    // Emails models should be same
    public class WarrenMahonySignupCompletedEmail : Email
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
    }


    public class WarrenMahonySignUpEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; }
        public string AppName { get; set; }

        public int Incentive { get; set; }
    }
    public class WarrenMahonyReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; }
        public string AppName { get; set; }

        public int Incentive { get; set; }
    }
    public class ReminderInvitationEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; }
        public string AppName { get; set; }

        public int Incentive { get; set; }
    }
    public class ExitSurveyEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; }
        public string AppName { get; set; }
    }


    public class ErrorRegistrationEmail : Email
    {
        public string ToEmail { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
    }

    public class WebsiteContactUsEmail : Email
    {

        public string To { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }

        //Email Body
        public string ContactName { get; set; }
        public string PreferredContact { get; set; }
        public string PreferredTime { get; set; }
        public string ReturnEmail { get; set; }
        public string ReturnPhone { get; set; }
    }

    public class SignupCompletedEmail : Email
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
    }

    public class RetrievePasswordEmail : Email
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
    }

    public class PasswordResetEmail : Email
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public string AppName { get; set; }
    }
    //For Warren and Mahony
    public class ShiftReminderWarrenMahonyEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //login to update roster
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string ShiftStartDate { get; set; }
        public string ShiftEndDate { get; set; }
        public string AppName { get; set; }
    }
    public class StartWarrenSurveyInvitationEmail : Email
    {        
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public string AppName { get; set; }
    }

    public class CompleteWarrenSurveyReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } 
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public string AppName { get; set; }
    }


    public class ExpiringSoonNotStartedWarrenSurveyReminderEmail : Email
    {        
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }

        public string AppName { get; set; }
    }
    public class ExpirinSoonNotCompletedWarrenSurveyReminderEmail : Email
    {
        public string ToEmail { get; set; }
        public string RecipientName { get; set; }
        public string Link { get; set; } //survey link
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }


        public string AppName { get; set; }
    }
}