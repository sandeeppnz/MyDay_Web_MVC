using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ExitSurvey_Page11
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int QnId { get; set; }
        public string OtherOption { get; set; }
        public string Options { get; set; }
    }
}