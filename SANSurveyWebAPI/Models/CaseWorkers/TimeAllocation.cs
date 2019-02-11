using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class TimeAllocation
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

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