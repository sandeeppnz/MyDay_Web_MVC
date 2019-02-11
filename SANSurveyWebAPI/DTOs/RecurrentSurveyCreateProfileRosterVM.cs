using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class RecurrentSurveyCreateProfileRosterVM
    {
        public int ProfileId { get; set; }
        public int OffSetFromUTC { get; set; }



        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public string Description { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
    }
}