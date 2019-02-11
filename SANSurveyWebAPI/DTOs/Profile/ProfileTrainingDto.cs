using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileTrainingDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }



        public string TrainingStartYear { get; set; }
        public string IsTrainingBreak { get; set; }
        public decimal? TrainingBreakLengthMonths { get; set; }

    }
}