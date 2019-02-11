using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileDemographic
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        //Demographics
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(60)]
        public string MaritialStatus { get; set; }
        [MaxLength(10)]
        public string BirthYear { get; set; }
        [MaxLength(10)]
        public string IsCaregiverChild { get; set; }
        [MaxLength(10)]
        public string IsCaregiverAdult { get; set; }
        [MaxLength(10)]
        public string IsUniversityBritish { get; set; }
        [MaxLength(60)]
        public string UniversityAttended { get; set; }
        [MaxLength(10)]
        public string IsLeadership { get; set; }

        public string UniversityAttendedOtherText { get; set; }


    }
}