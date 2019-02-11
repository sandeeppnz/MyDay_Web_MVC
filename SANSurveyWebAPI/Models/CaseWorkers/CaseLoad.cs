using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class CaseLoad
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Option { get; set; }        
    }
}