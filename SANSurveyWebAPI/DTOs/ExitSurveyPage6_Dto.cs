using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitSurveyPage6_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q1Other { get; set; } //other text
        public string Q2Other { get; set; } //other text

    }
}