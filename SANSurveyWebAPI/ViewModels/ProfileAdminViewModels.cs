using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace SANSurveyWebAPI.ViewModels.Web
{
    public class ProfileAdminCreateVM
    {
        public int Id { get; set; }
        public int Profile { get; set; }
        public int Shift { get; set; }
        public bool IsScheduled { get; set; }
    }

    public class RecurrentSurveyCreateSurveyVM
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int ProfileRosterId { get; set; }
    }

    //public class RecurrentSurveyCreateProfileRosterVM
    //{
    //    public int Id { get; set; }
    //    public int ProfileId { get; set; }
    //}

}