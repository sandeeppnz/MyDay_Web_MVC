using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ExitSurvey_Page8
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; } //approx. years
        public string Q4 { get; set; } // i don't know
        public string Q5 { get; set; } //months
        public string Q6 { get; set; } //weeks
        public string Q7 { get; set; } //days
        public string Q8 { get; set; } //days

    }
}