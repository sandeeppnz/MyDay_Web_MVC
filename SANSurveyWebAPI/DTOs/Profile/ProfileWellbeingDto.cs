using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileWellbeingDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }

        public string SwbLife { get; set; }
        public string SwbHome { get; set; }
        public string SwbJob { get; set; }

    }

   

}