using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileComment
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        public string Comment { get; set; }

    }
}