using System;

namespace SANSurveyWebAPI.Models
{
    public class WAMFeedback
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Feedback { get; set; }
    }
}