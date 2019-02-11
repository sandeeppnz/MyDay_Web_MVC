using System;
using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitV2SurveyDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public string Uid { get; set; }
        public DateTime SurveyDate { get; set; }
        public DateTime? EntryStartCurrent { get; set; }
        public DateTime? EndTimeCurrent { get; set; }
        public DateTime? EntryStartUTC { get; set; }
        public DateTime? EndTimeUTC { get; set; }
        public string SurveyProgress { get; set; }
    }
}