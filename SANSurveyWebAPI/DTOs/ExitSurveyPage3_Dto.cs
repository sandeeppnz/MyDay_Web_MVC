using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitSurveyPage3_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
    }
}