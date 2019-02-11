using System;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string ShortName { get; set; }
        [StringLength(255)]
        public string LongName { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        public int Sequence { get; set; }
        public bool? WardRoundIndicator { get; set; }
        public bool? OtherTaskIndicator { get; set; }
        public bool? IsWardRoundTask { get; set; }
    }
}