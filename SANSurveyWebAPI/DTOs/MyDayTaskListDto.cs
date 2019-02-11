using System;

namespace SANSurveyWebAPI.DTOs
{
    public class MyDayTaskListDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int SurveyId { get; set; }
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public int TaskDuration { get; set; }
        public string TaskCategoryId { get; set; }
        public DateTime? TaskStartDateTimeUtc { get; set; }
        public DateTime? TaskEndDateTimeUtc { get; set; }
        public DateTime? TaskStartDateCurrentTime { get; set; }
        public DateTime? TaskEndDateCurrentTime { get; set; }
        public bool IsRandomlySelected { get; set; }
        public bool IsAffectStageCompleted { get; set; }
    }
}