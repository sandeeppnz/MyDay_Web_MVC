using System;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public int? ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int? RosterItemId { get; set; }
        //public virtual RosterItem RosterItem { get; set; }
        public virtual ProfileRoster RosterItem { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }        
        public DateTime? SurveyUserStartDateTimeUtc { get; set; }
        public DateTime? SurveyUserCompletedDateTimeUtc { get; set; }
        [MaxLength(20)]
        public string SurveyProgressNext { get; set; }
        public int MaxStep { get; set; }
        public string Status { get; set; }
        //added 2017-01-11
        public DateTime? SurveyWindowStartDateTime { get; set; } //span of survey, extracted from the roster
        public DateTime? SurveyWindowStartDateTimeUtc { get; set; } //span of survey, extracted from the roster
        public DateTime? SurveyWindowEndDateTime { get; set; } //span of survey, extracted from the roster
        public DateTime? SurveyWindowEndDateTimeUtc { get; set; } //span of survey, extracted from the roster
        public DateTime? SurveyExpiryDateTime { get; set; }
        public DateTime? SurveyExpiryDateTimeUtc { get; set; }
        public int? SysGenRandomNumber { get; set; }
        public string SurveyDescription { get; set; }
        //to help load abondoned surveys
        public DateTime? CurrTaskStartTime { get; set; }
        public DateTime? CurrTaskEndTime { get; set; }
        public DateTime? NextTaskStartTime { get; set; }
        public int RemainingDuration { get; set; }
        public bool FirstQuestion { get; set; }
        public int CurrTask { get; set; }
        public int? AddTaskId { get; set; }
        public int? WRCurrTaskId { get; set; }
        public string WRCurrTasksId { get; set; }
        public int? WRRemainingDuration { get; set; }
        public DateTime? WRCurrTaskStartTime { get; set; }
        public DateTime? WRCurrTaskEndTime { get; set; }
        public DateTime? WRNextTaskStartTime { get; set; }
        public DateTime? WRCurrWindowEndTime { get; set; }
        public DateTime? WRCurrWindowStartTime { get; set; }
        [MaxLength(10)]
        public string IsOnCall { get; set; }
        public string Comment { get; set; }
        public string WasWorking { get; set; }
    }
}