using SANSurveyWebAPI.Models.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime? RegisteredDateTimeUtc { get; set; }
        public DateTime? LastUpdatedDateTimeUtc { get; set; }
        public virtual ICollection<ProfileTask> ProfileTasks { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [MaxLength(60)]
        public string RegistrationProgressNext { get; set; }

        [MaxLength(60)]
        public string ExitSurveyProgressNext { get; set; }
        


        public string Uid { get; set; }
        public int MaxStep { get; set; }
        public int OffSetFromUTC { get; set; }
        public int? RegistrationEmailJobId { get; set; }
        public int? RegistrationSmsJobId { get; set; }


        public int MaxStepExitSurvey { get; set; }


        //Screening Module
        [MaxLength(60)]
        public string CurrentLevelOfTraining { get; set; }
        /*
         1. Foundation Year 1
         2. Foundation Year 2
         3. Core medical training year 1
         4. ....
         99. Not in training
              */



        [MaxLength(10)]
        public string IsCurrentPlacement { get; set; }
        /*
         * 1. Yes
         * 2. No
         */

        
        //Tasks
        [MaxLength(20)]
        public string ProfileTaskType { get; set; }
        
        public string Name { get; set; }
        [MaxLength(20)]
        public string MobileNumber { get; set; }

        [MaxLength(256)]
        public string LoginEmail { get; set; }

        public int Incentive { get; set; }

        public int MaxStepKidsSurvey { get; set; }
        public int MaxExitV2Step { get; set; }

        public string ClientName { get; set; }
        public string ClientInitials { get; set; }
    }
}