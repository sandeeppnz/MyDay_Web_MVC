using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileTaskTime
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        //TaskTime
        public decimal? ClinicalActualTime { get; set; }
        public decimal? ResearchActualTime { get; set; }
        public decimal? TeachingLearningActualTime { get; set; }
        public decimal? AdminActualTime { get; set; }

        public decimal? ClinicalDesiredTime { get; set; }
        public decimal? ResearchDesiredTime { get; set; }
        public decimal? TeachingLearningDesiredTime { get; set; }
        public decimal? AdminDesiredTime { get; set; }


    }
}