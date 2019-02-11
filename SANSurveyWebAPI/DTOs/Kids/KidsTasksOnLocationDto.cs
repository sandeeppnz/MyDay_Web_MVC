using SANSurveyWebAPI.Models;
namespace SANSurveyWebAPI.DTOs
{
    public class KidsTasksOnLocationDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public int KidsLocationId { get; set; }
        public string LocationName { get; set; }
        public string OtherLocationName { get; set; }
        public string SpentStartTime { get; set; }
        public string SpentEndTime { get; set; }
        public string TasksDone { get; set; }
        public string TaskOther { get; set; }
        public bool IsEmoStageCompleted { get; set; }
    }
}