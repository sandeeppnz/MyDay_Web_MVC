using SANSurveyWebAPI.DTOs;
using SANSurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{

    public class AdminSurveyDetailsVM
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public int? ProfileId { get; set; }
        public int? RosterItemId { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime? SurveyUserStartDateTimeUtc { get; set; }
        public DateTime? SurveyUserCompletedDateTimeUtc { get; set; }
        public string SurveyProgressNext { get; set; }
        public string SurveyDescription { get; set; }
        public DateTime? SurveyExpiryDateTime { get; set; }
        public DateTime? SurveyExpiryDateTimeUtc { get; set; }

        //
        public List<HangfireStateDto> ShiftReminders { get; set; }
        public List<HangfireStateDto> StartSurveyReminders { get; set; }
        public List<HangfireStateDto> CompleteSurveyReminders { get; set; }
        public List<HangfireStateDto> ExpiringSoonNotStartedReminders { get; set; }
        public List<HangfireStateDto> ExpiringSoonNotCompletedReminders { get; set; }

        public JobLogShiftReminderEmailDto JobLogShiftReminderEmail { get; set; }
        public List<JobLogShiftReminderEmailDto> JobLogShiftReminderEmailList { get; set; }


        public JobLogStartSurveyReminderEmailDto JobLogStartSurveyReminderEmail { get; set; }
        public List<JobLogStartSurveyReminderEmailDto> JobLogStartSurveyReminderEmailList { get; set; }

        public JobLogCompleteSurveyReminderEmailDto JobLogCompleteSurveyReminderEmail { get; set; }
        public List<JobLogCompleteSurveyReminderEmailDto> JobLogCompleteSurveyReminderEmailList { get; set; }

        public JobLogExpiringSoonSurveyNotStartedReminderEmailDto JobLogExpiringSoonSurveyNotStartedReminderEmail { get; set; }
        public List<JobLogExpiringSoonSurveyNotStartedReminderEmailDto> JobLogExpiringSoonSurveyNotStartedReminderEmailList { get; set; }

        public JobLogExpiringSoonSurveyNotCompletedReminderEmailDto JobLogExpiringSoonSurveyNotCompletedReminderEmail { get; set; }
        public List<JobLogExpiringSoonSurveyNotCompletedReminderEmailDto> JobLogExpiringSoonSurveyNotCompletedReminderEmailList { get; set; }


    }

}