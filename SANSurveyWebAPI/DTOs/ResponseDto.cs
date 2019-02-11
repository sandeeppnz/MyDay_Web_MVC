using System;

namespace SANSurveyWebAPI.Models
{
    public class ResponseDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int ProfileId { get; set; }
        public int TaskId { get; set; }
        public string TaskOther { get; set; }
        public int? PageStatId { get; set; }
        public DateTime? StartResponseDateTimeUtc { get; set; }
        public DateTime? EndResponseDateTimeUtc { get; set; }
        public DateTime? ShiftStartDateTime { get; set; }
        public DateTime? ShiftEndDateTime { get; set; }
        public DateTime? TaskStartDateTime { get; set; }
        public DateTime? TaskEndDateTime { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime? SurveyWindowStartDateTime { get; set; }
        public DateTime? SurveyWindowEndDateTime { get; set; }
        public bool? IsOtherTask { get; set; }
        public bool? IsWardRoundTask { get; set; }
        public int? WardRoundTaskId { get; set; }
        public DateTime? WardRoundWindowStartDateTime { get; set; }
        public DateTime? WardRoundWindowEndDateTime { get; set; }
    }
}