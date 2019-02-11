namespace SANSurveyWebAPI.Models
{
    public class KidsEmoStageTracked
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public string TaskWhile { get; set; }
        public string SurveyDate { get; set; }
        public string TaskStartTime { get; set; }
        public string TaskEndTime { get; set; }
        public int? KidsLocationId { get; set; }
        public int? KidsTravelId { get; set; }
        public string LocationName { get; set; }
        public string OtherLocationName { get; set; }
        public string ModeOfTransport { get; set; }
        public string OtherModeOfTransport { get; set; }
        public int? KidsTaskId { get; set; }
        public string TaskPerformed { get; set; }
        public string OtherTask { get; set; }
        public bool IsEmoAffStageCompleted { get; set; }
        public int? SequenceToQEmo { get; set; }
        public string TravelDetails { get; set; }
    }
}