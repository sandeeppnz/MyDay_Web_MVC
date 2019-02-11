using System;

namespace SANSurveyWebAPI.DTOs
{
    public class ProfileCommentDto
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Comment { get; set; }
    }
}