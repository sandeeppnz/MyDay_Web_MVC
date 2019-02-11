using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class MyDayErrorLogs
    {
        public int Id { get; set; }   
        public int ProfileId { get; set; }
        public string SurveyUID { get; set; }
        public DateTime AccessedDateTime { get; set; }
        public string ErrorMessage { get; set; }
        public string HtmlContent { get; set; }
    }
}