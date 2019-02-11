using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileContractDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string ContractType { get; set; }
        public string WorkingType { get; set; }
        public string HoursWorkedLastMonth { get; set; }
     

    }
}