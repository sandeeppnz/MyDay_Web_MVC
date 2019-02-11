using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class CurrentWorkplace
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int OptionId { get; set; }
        public string OptionValue { get; set; }   
        public bool Ans { get; set; }
    }
}