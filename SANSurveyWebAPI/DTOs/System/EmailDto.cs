namespace SANSurveyWebAPI.DTOs
{
    public class EmailDto
    {
        public int Id { get; set; } //profile id
        public string ToEmail { get; set; }
        public string CCToEmail { get; set; }
        public string FromEmail { get; set; }
        public string Link { get; set; }
        public string RecipientName { get; set; }
        public string ScheduledDateTime { get; set; }
        public int Incentive { get; set; }
    }

    public class ExitSurveyInvitationEmailDto : EmailDto
    {

    }

    public class WebsiteContactUsEmailDto : EmailDto
    {
        public string Name { get; set; }
        public string PreferredContact { get; set; }
        public string PreferredTime { get; set; }
        public string ReturnEmail { get; set; }
        public string ReturnPhone { get; set; }
        public string Message { get; set; }
    }



    public class SignupCompletedEmailDto : EmailDto
    {
    }

    public class WarrenMahonySignupCompletedEmailDto : SignupCompletedEmailDto
    {
    }



    public class SurveyInvitationEmailDto : EmailDto
    {
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }

    }

    public class RegistrationInvitationEmailDto : EmailDto
    {
    }


   

    public class ShiftStartReminderEmailDto : EmailDto
    {
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string ShiftStartDate { get; set; }
        public string ShiftEndDate { get; set; }
        public int RosterItemId { get; set; }
    }

    public class CompleteSurveyReminderEmailDto : EmailDto
    {
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }
    }

    public class ExpiringSoonNotStartedSurveyReminderEmailDto : EmailDto
    {
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }


        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }
    }

    public class ExpirinSoonNotCompletedSurveyReminderEmailDto : EmailDto
    {
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }

        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }       

    }

    //For Warren and Mahony ShiftReminderWarrenMahonyEmail
    public class ShiftReminderWarrenMahonyEmailDto : EmailDto
    {
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string ShiftStartDate { get; set; }
        public string ShiftEndDate { get; set; }
        public int RosterItemId { get; set; }
    }
    public class StartWarrenSurveyInvitationEmailDto : EmailDto
    {
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }

    }



    public class CompleteWarrenSurveyReminderEmailDto : EmailDto
    {
        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }
        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }
    }
    public class ExpiringSoonNotStartedWarrenSurveyReminderEmailDto : EmailDto
    {
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }

        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }
    }
    public class ExpirinSoonNotCompletedWarrenSurveyReminderEmailDto : EmailDto
    {
        public string SurveyWindowStartDateTime { get; set; }
        public string SurveyWindowEndDateTime { get; set; }

        public string SurveyWindowStartTime { get; set; }
        public string SurveyWindowEndTime { get; set; }
        public string SurveyWindowStartDate { get; set; }
        public string SurveyWindowEndDate { get; set; }

        public int SurveyId { get; set; }
        public int RosterItemId { get; set; }

    }
}