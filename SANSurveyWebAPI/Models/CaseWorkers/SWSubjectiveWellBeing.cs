using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class SWSubjectiveWellBeing
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        [MaxLength(60)]
        public string SwbLife { get; set; }
        [MaxLength(60)]
        public string SwbHome { get; set; }
        [MaxLength(60)]
        public string SwbJob { get; set; }
    }
}