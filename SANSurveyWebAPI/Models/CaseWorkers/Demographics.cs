using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class Demographics
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        //Demographics
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(10)]
        public string BirthYear { get; set; }
        [MaxLength(60)]
        public string MaritalStatus { get; set; }        
        [MaxLength(10)]
        public string IsCaregiverChild { get; set; }
        [MaxLength(10)]
        public string IsCaregiverAdult { get; set; }  
        public string EthnicityOrRace { get; set; }
    }
}