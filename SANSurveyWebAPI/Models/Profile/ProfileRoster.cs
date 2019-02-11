using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileRoster
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public bool IsAllDay { get; set; }

        //saving the set exct set times  
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        //2017-01-28 support the scheduler
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }


        public int ProfileId { get; set; }
        public string Description { get; set; }

        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }

      

    }
}