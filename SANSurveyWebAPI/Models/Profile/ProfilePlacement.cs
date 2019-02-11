using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class ProfilePlacement
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        //Placement
        [MaxLength(10)]
        public string PlacementStartYear { get; set; }
        [MaxLength(20)]
        public string PlacementStartMonth { get; set; }
        [MaxLength(10)]
        public string PlacementIsInHospital { get; set; }
        [MaxLength(100)]
        public string PlacementHospitalName { get; set; }
        public string PlacementHospitalNameOther { get; set; }
        [MaxLength(20)]
        public string PlacementHospitalStartMonth { get; set; }
        [MaxLength(10)]
        public string PlacementHospitalStartYear { get; set; }


    }
}