namespace SANSurveyWebAPI.Models
{
    public class KidsFeedback
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public string Comments { get; set; }
    }
}