using System;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.DTOs
{
    public class JobLogDto
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

    public class JobLogMethodDto : JobLogDto
    { }

    public class JobLogEmailDto : JobLogDto
    {
        public byte[] Email { get; set; }
    }

    public class JobLogCreateSurveyDto : JobLogMethodDto
    {
        public int? ProfileRosterId { get; set; }
        public int? SurveyId { get; set; }
    }

    public class JobLogUpdateSurveyDto : JobLogMethodDto
    {
        public int? ProfileRosterId { get; set; }
        public int? SurveyId { get; set; }
    }

    public class JobLogBaselineSurveyEmailDto : JobLogEmailDto
    { }

    public class JobLogRegistrationCompletedEmailDto : JobLogEmailDto
    { }

    public class JobLogShiftReminderEmailDto : JobLogEmailDto
    {
        public int ProfileRosterId { get; set; }
    }

    public class JobLogStartSurveyReminderEmailDto : JobLogEmailDto
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

    public class JobLogCompleteSurveyReminderEmailDto : JobLogEmailDto
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

    public class JobLogExpiringSoonSurveyNotStartedReminderEmailDto : JobLogEmailDto
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

    public class JobLogExpiringSoonSurveyNotCompletedReminderEmailDto : JobLogEmailDto
    {
        public int SurveyId { get; set; }
        public int ProfileRosterId { get; set; }

    }

}