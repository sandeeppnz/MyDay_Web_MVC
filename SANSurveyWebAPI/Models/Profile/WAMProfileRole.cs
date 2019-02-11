using System;

namespace SANSurveyWebAPI.Models
{
    public class WAMProfileRole
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string MyProfileRole { get; set; }
        public string StartYear { get; set; }
    }
}