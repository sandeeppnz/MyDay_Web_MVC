﻿using SANSurveyWebAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.DTOs
{
    public class ThirdJobDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public int ExitSurveyId { get; set; }
        public string Qn { get; set; }
        [MaxLength(60)]
        public string Ans { get; set; }
    }
}