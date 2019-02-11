using System;

namespace SANSurveyWebAPI.DTOs
{
    public class WAMFeedbackDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Feedback { get; set; }
    }
}