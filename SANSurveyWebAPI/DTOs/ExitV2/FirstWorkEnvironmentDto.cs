using System.ComponentModel.DataAnnotations;
using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class FirstWorkEnvironmentDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int ExitSurveyId { get; set; }
        public string Qn { get; set; }
        [MaxLength(60)]
        public string Ans { get; set; }
    }
}