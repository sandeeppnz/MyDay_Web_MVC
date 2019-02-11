using System;
namespace SANSurveyWebAPI.Models
{
    public class WAMIntentions
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public decimal? SameEmployer { get; set; }
        public decimal? SameIndustry { get; set; }
    }
}