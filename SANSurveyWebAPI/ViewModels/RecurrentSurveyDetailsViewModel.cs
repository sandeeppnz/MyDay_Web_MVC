using SANSurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{

    public class RecurrentSurveyDetailsViewModel
    {
        public int ProfileRosterItemId { get; set; }
        public int ProfileId { get; set; }
        public string Description { get; set; }
        public bool HasSurvey { get; set; }
        public int? SurveyId { get; set; }
        public string SurveyStatus { get; set; }

    }

}