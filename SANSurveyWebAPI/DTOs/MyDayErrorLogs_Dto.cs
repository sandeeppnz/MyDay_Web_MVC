using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class MyDayErrorLogs_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string SurveyUID { get; set; }
        public DateTime AccessedDateTime { get; set; }
        public string ErrorMessage { get; set; }
        public string HtmlContent { get; set; }
    }
}