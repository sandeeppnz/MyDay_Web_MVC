using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class MasterDataDto
    {
        public int Id { get; set; }
        public int RecurrentSurveyTimeSlot { get; set; }
        public int RecurrentSurveyTaskSelectionLimit { get; set; }
        public int NoOfSurveyPerParticipant { get; set; }
        public string TaskTypes { get; set; }
        public string SurveyForUsers { get; set; }
        public string SurveyTypes { get; set; }
    }
}