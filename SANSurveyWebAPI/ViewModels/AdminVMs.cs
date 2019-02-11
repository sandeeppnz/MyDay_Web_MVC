using System;



namespace SANSurveyWebAPI.ViewModels
{
    public class ProfileAdminVM
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string RegistrationProgressNext { get; set; }
        public string Uid { get; set; }
        public string UserId { get; set; }
        public Microsoft.AspNet.Identity.EntityFramework.IdentityUser User { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime? RegisteredDateTime { get; set; }
    }






}