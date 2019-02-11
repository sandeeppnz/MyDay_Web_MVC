using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class KidsLocationDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }       
        public int KidsSurveyId { get; set; }
        public virtual KidsSurvey KidsSurvey { get; set; }
        public string Location { get; set; }
        public string OtherLocation { get; set; }
        public int TimeSpentInHours { get; set; }
        public int TimeSpentInMins { get; set; }
        public int LocationSequence { get; set; }
        public string StartedAt { get; set; } // Time at the location
        public string EndedAt { get; set; } //was on the same location till how long
        public bool IsTasksEntered { get; set; }
    }
}