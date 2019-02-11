using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileSpecialty
    {
        public int Id { get; set; }

        [Required]
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [Required]
        public int SpecialtyId { get; set; }
        public virtual Specialty Specialty { get; set; }

        public string OtherText { get; set; }

    }
}