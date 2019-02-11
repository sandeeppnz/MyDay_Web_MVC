using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfileContract
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        [MaxLength(20)]
        public string ContractType { get; set; }
        [MaxLength(20)]
        public string WorkingType { get; set; }
        [MaxLength(60)]
        public string HoursWorkedLastMonth { get; set; }


    




    }
}