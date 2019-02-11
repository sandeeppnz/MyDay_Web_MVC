using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class SecondJobDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int ExitSurveyId { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
    }
}