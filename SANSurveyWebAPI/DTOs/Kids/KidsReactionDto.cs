using System;
using SANSurveyWebAPI.Models;
namespace SANSurveyWebAPI.DTOs
{
    public class KidsReactionDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public string SurveyDate { get; set; }
        public DateTime ResponseStartTime { get; set; }
        public DateTime ResponseEndTime { get; set; }
        public int? KidsLocationId { get; set; }
        public int? KidsTravelId { get; set; }
        public int? KidsTaskId { get; set; }
        public int KidsEmoTrackId { get; set; }
        public string TasksPerformed { get; set; }
        public string QuestionId { get; set; }
        public string Answer { get; set; }
        public string TaskStartTime { get; set; }
        public string TaskEndTime { get; set; }
    }
}