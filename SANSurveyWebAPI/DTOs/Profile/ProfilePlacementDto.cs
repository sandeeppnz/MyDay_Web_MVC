using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfilePlacementDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        public string PlacementStartYear { get; set; }
        public string PlacementStartMonth { get; set; }
        public string PlacementIsInHospital { get; set; }
        public string PlacementHospitalName { get; set; }
        public string PlacementHospitalNameOther { get; set; }
        public string PlacementHospitalStartMonth { get; set; }
        public string PlacementHospitalStartYear { get; set; }
    }
}