﻿using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class Specialty
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
        public int Sequence { get; set; }

    }
}