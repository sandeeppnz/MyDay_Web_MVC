using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ExitSurvey_Page2
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        public string Qn { get; set; }

        [MaxLength(60)]
        public string Ans { get; set; }
        
    }
}