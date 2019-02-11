using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileEthinicity
    {
        public int Id { get; set; }

        [Required]
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [Required]
        public int EthinicityId { get; set; }
        public virtual Ethinicity Ethinicity { get; set; }
    }
}