using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileDemographicDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        public string BirthYear { get; set; }
        public string Gender { get; set; }
        public string UniversityAttended { get; set; }
        public string UniversityAttendedOtherText { get; set; }



        public string MaritialStatus { get; set; }
        public string IsCaregiverChild { get; set; }
        public string IsCaregiverAdult { get; set; }
        public string IsUniversityBritish { get; set; }
        public string IsLeadership { get; set; }
    }
}