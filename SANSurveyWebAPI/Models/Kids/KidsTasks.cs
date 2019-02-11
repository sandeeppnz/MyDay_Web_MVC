using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class KidsTasks
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public string TaskName { get; set; } //What you did
        public string Venue { get; set; } //Where you did
        public string InOutLocation { get; set; } //Where inside or outside
        public string Travel { get; set; } //How did you reach
        public string People { get; set; } //With Whom you were
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime SurveyDate { get; set; }
        public bool IsEmotionalStageCompleted { get; set; }
        public bool IsRandomlySelected { get; set; }
    }
}