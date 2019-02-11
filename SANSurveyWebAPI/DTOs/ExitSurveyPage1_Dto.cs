using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitSurveyPage1_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Qn { get; set; }
        [MaxLength(60)]
        public string Ans { get; set; }
    }
}