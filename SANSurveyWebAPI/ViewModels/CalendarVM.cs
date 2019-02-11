using System.Collections.Generic;

namespace SANSurveyWebAPI.ViewModels
{
    /*
     
        User Calendar
     */


    public class CalendarVM
    {
        public string number { get; set; }
        public string badgeClass { get; set; }
        public List<DayEvents> dayEvents { get; set; }
    }


    public class CalendarEditVM
    {
        public string RosterItemId { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        //public List<DayEvents> dayEvents { get; set; }
    }


    public class DayEvents
    {
        public string RosterItemId { get; set; }
        public string Name { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string HasSurvey { get; set; }
    }


}