using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileTraining
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        [MaxLength(10)]
        public string TrainingStartYear { get; set; }

        [MaxLength(10)]
        public string IsTrainingBreak { get; set; }

        public decimal? TrainingBreakLengthMonths { get; set; }

    }
}