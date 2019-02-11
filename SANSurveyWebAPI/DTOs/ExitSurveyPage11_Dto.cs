using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class ExitSurveyPage11_Dto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int QnId { get; set; }
        public string OtherOption { get; set; }
        public string Options { get; set; }
    }
}