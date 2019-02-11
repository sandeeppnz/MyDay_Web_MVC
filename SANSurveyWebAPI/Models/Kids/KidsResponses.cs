using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class KidsResponses
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int KidsSurveyId { get; set; }
        public int KidsTaskId { get; set; }
        public string TaskName { get; set; }
        public string QuestionId { get; set; }
        public string Answer { get; set; }
        public DateTime SurveyDate { get; set; }
        public DateTime? ResponseStartTimeUTC { get; set; }
        public DateTime? ResponseEndTimeUTC { get; set; }
    }
}