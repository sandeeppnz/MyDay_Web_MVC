using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class EducationBackgroundDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string BachelorsDegree { get; set; }
        public string MasterDegree { get; set; }
        public string PreServiceTraining { get; set; }
    }
}