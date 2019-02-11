using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime? LastUpdatedDateTimeUtc { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string LoginEmail { get; set; }
        //public string BirthYear { get; set; }
        //public string Gender { get; set; }
        //public string UniversityAttended { get; set; }
        public string CurrentLevelOfTraining { get; set; }

        //public int? SpecialityId { get; set; }


        public string UserId { get; set; }
        public string RegistrationProgressNext { get; set; }

        public string ExitSurveyProgressNext { get; set; }

        public string Uid { get; set; }
        public int MaxStep { get; set; }
        public int MaxStepExitSurvey { get; set; }


        public int OffSetFromUTC { get; set; }
        public int? RegistrationEmailJobId { get; set; }
        public int? RegistrationSmsJobId { get; set; }
        public DateTime? RegisteredDateTimeUtc { get; set; }

        public string ProfileTaskType { get; set; }
        
        public string IsCurrentPlacement { get; set; }
       
        public int Incentive { get; set; }
        public int MaxStepKidsSurvey { get; set; }

        public int MaxExitV2Step { get; set; }

        public string ClientName { get; set; }
        public string ClientInitials { get; set; }
    }
}