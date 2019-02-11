using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileTaskItemDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int TaskItemId { get; set; }
        public DateTime? CreatedDateTimeUtc { get; set; }
        public string Frequency { get; set; }
    }
}