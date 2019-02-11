using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Type { get; set; }        
        public int Sequence { get; set; }
        public bool? WardRoundIndicator { get; set; }
        public bool? OtherTaskIndicator { get; set; }
        public bool? IsWardRoundTask { get; set; }
    }
}