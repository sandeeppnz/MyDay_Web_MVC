using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileWellbeing
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        //Subjective WellbeingModule
        [MaxLength(60)]
        public string SwbLife { get; set; }
        [MaxLength(60)]
        public string SwbHome { get; set; }
        [MaxLength(60)]
        public string SwbJob { get; set; }
    }
}