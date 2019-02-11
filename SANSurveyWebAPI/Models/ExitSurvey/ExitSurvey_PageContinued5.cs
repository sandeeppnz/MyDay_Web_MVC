using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class ExitSurvey_PageContinued5
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Qn { get; set; }
        [MaxLength(60)]
        public string Ans { get; set; }
    }
}