using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ExitSurvey_Page3
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }

    }
}