using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class KidsFeedbackDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public string Comments { get; set; }
    }
}