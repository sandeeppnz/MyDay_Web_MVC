using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Areas.CallStaff.ViewModels
{
    public class ProfileViewVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LoginEmail { get; set; }
        public bool Consent { get; set; }

        public DateTime? LastUpdatedDateTimeUtc { get; set; }
        public string RegistrationStatus { get; set; }
        public DateTime? RegisteredDateTimeUtc { get; set; }
        public IList<SurveyProfileVM> Surveys { get; set; }

    }
}