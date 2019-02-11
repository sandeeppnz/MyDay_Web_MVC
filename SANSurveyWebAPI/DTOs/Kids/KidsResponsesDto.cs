using System;
using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class KidsResponsesDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public int KidsTaskId { get; set; }
        public string TaskName { get; set; }
        public string QuestionId { get; set; }
        public string Answer { get; set; }
        public DateTime SurveyDate { get; set; }
        public DateTime? ResponseStartTimeUTC { get; set; }
        public DateTime? ResponseEndTimeUTC { get; set; }
    }
}