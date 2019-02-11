namespace SANSurveyWebAPI.Models
{
    public class FirstTraining
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int ExitSurveyId { get; set; }
        public int QId { get; set; }
        public string Qn { get; set; }
        public string Options { get; set; }
        public string OtherOption { get; set; }
        public string Ans { get; set; }        
    }
}