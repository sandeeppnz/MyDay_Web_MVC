using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class AboutYouESDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int ExitSurveyId { get; set; }
        public string Q1_Applicable { get; set; }
        public int Q1_Year { get; set; }
        public string Q2_PTWork { get; set; }
        public string Q2_Other { get; set; }
        public int Q3_NoOfPeople { get; set; }
        public string Q4_Martial { get; set; }
        public string Q5_PartnershipMarried { get; set; }
    }
}