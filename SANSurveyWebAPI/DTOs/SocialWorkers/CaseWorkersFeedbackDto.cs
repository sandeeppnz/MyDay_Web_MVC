using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class CaseWorkersFeedbackDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }       
        public string Comments { get; set; }
    }
}