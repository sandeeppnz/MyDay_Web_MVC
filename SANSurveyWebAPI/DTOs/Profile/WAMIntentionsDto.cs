using System;
namespace SANSurveyWebAPI.DTOs
{
    public class WAMIntentionsDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public decimal? SameEmployer { get; set; }
        public decimal? SameIndustry { get; set; }
    }
}