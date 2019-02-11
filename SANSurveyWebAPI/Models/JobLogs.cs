using System;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class JobLog
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        [StringLength(10)]
        public string JobMethod { get; set; } //Manual or Auto

        public int? HangfireJobId { get; set; }
        public double? RunAfterMin { get; set; }

        public DateTime CreatedDateTimeServer { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }

    }

    public class JobLogMethod : JobLog
    { }

    public class JobLogEmail : JobLog
    {
        //public byte[] Email { get; set; }
    }

    public class JobLogCreateSurvey : JobLogMethod
    {
        public int? ProfileRosterId { get; set; }
        public int? SurveyId { get; set; }
    }

    public class JobLogUpdateSurvey : JobLogMethod
    {
        public int? ProfileRosterId { get; set; }
        public int? SurveyId { get; set; }
    }

    public class JobLogExitSurveyEmail : JobLogEmail
    { }

    public class JobLogBaselineSurveyEmail : JobLogEmail
    { }

    public class JobLogRegistrationCompletedEmail : JobLogEmail
    { }

    public class JobLogShiftReminderEmail : JobLogEmail
    {
        public int ProfileRosterId { get; set; }
    }

    public class JobLogStartSurveyReminderEmail : JobLogEmail
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

    public class JobLogCompleteSurveyReminderEmail : JobLogEmail
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

    public class JobLogExpiringSoonSurveyNotStartedReminderEmail : JobLogEmail
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

    public class JobLogExpiringSoonSurveyNotCompletedReminderEmail : JobLogEmail
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

}