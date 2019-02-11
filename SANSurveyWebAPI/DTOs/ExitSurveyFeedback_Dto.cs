using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitSurveyFeedback_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string feedbackComments { get; set; }
    }

}