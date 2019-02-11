using System;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.DTOs
{
    public class WAMDemographicsDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        //Demographics
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(60)]
        public string BirthYear { get; set; }
        [MaxLength(10)]
        public string IsCaregiverChild { get; set; }
       
    }
}