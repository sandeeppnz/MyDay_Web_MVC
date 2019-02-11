namespace SANSurveyWebAPI.DTOs
{
    public class KidsTravelDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int KidsSurveyId { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public string ModeOfTransport { get; set; }
        public string OtherModeOfTransport { get; set; }
        public int TravelTimeInHours { get; set; }
        public int TravelTimeInMins { get; set; }        
        public string TravelStartedAt { get; set; }
        public string TravelEndedAt { get; set; }
    }
}