using System;

namespace SANSurveyWebAPI.DTOs
{
    public class WAMProfileRoleDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string MyProfileRole { get; set; }
        public string StartYear { get; set; }
    }
}