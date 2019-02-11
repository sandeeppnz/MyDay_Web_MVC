using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class CurrentWorkplaceContd
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string WorkStatus { get; set; }
        public string WorkPosition { get; set; }
        public string WorkCountry { get; set; }
        public string OtherWorkCountry { get; set; }
    }
}