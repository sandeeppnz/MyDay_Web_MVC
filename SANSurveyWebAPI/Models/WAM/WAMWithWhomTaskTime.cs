using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class WAMWithWhomTaskTime
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int SurveyId { get; set; }
        public virtual Survey Survey { get; set; }
        public int TaskId { get; set; }
        public int QuestionId { get; set; }
        public string WithWhomOptions { get; set; }
        public string WithWhomOther { get; set; }

        //nEAREST LOCATION        
        public string NearestLocation { get; set; }
        public string NearestOtherLocation { get; set; }
    }
}