using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitSurveyPage5_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Qn { get; set; }
        public string Ans { get; set; }
    }
}