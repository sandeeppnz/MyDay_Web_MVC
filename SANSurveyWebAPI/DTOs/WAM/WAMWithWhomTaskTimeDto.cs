using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class WAMWithWhomTaskTimeDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int SurveyId { get; set; }
        public virtual Survey Survey { get; set; }
        public int TaskId { get; set; }
        public int QuestionId { get; set; }
        public string WithWhomOptions { get; set; }
        public string WithWhomOther { get; set; }

        //nEAREST LOCATION
        public string NearestLocation { get; set; }
    }
}