using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }

        [MaxLength(50)]
        public string Channel { get; set; } //by website, or survey, registration

        public int? ProfileId { get; set; }
        public int? SurveyId { get; set; }
        public string Message { get; set; }

        [MaxLength(50)]
        public string PreferedContact { get; set; } //Email, Phone

        [MaxLength(50)]
        public string PreferedTime { get; set; }

        [MaxLength(256)]
        public string Email { get; set; } 

        [MaxLength(256)]
        public string ContactNumber { get; set; } 
    }
}