using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SANSurveyWebAPI.Models
{
    public class ProfileTask
    {
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = false)]
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [Required]
        public int TaskItemId { get; set; }
        public virtual TaskItem TaskItem { get; set; }

        [MaxLength(10)]
        public string Frequency { get; set; }

        public DateTime? CreatedDateTimeUtc { get; set; }
        //public DateTime? LastUpdatedDateTimeUtc { get; set; }
    }
}