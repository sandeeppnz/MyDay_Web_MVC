using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class SurveyDto
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public int? ProfileId { get; set; }
        public int? RosterItemId { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime? SurveyUserStartDateTimeUtc { get; set; }
        public DateTime? SurveyUserCompletedDateTimeUtc { get; set; }
        public string SurveyProgressNext { get; set; }
        public int MaxStep { get; set; }
        public string Status { get; set; }
        public DateTime? SurveyWindowStartDateTime { get; set; }
        public DateTime? SurveyWindowStartDateTimeUtc { get; set; }
        public DateTime? SurveyWindowEndDateTime { get; set; }
        public DateTime? SurveyWindowEndDateTimeUtc { get; set; }
        public DateTime? SurveyExpiryDateTime { get; set; }
        public DateTime? SurveyExpiryDateTimeUtc { get; set; }
        public int? SysGenRandomNumber { get; set; }
        public string SurveyDescription { get; set; }
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
        public string IsOnCall { get; set; }
        public string Comment { get; set; }
        public string WasWorking { get; set; }
    }
}