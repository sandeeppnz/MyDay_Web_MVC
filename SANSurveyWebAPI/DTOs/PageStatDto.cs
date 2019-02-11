using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class PageStatDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int? SurveyId { get; set; }
        public int? TaskId { get; set; }
        public bool? WholePageIndicator { get; set; }
        public string PageName { get; set; }
        public string PageType { get; set; }
        public string PageAction { get; set; }
        public string Remark { get; set; }
        public DateTime PageDateTimeUtc { get; set; }

    }
}