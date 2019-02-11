using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class JobIntentions
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        [MaxLength(60)]
        public string CurrentWorkplace { get; set; }
        [MaxLength(60)]
        public string CurrentIndustry { get; set; }
    }
}