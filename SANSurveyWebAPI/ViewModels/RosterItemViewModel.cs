using SANSurveyWebAPI.Models;
using Kendo.Mvc.UI;
using System;

namespace SANSurveyWebAPI.ViewModels.Web
{
    public class RosterItemViewModel : UserHeaderMV, ISchedulerEvent
    {

        /*
         * 
         * 
         */


        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        private DateTime start;
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.ToUniversalTime();
                //start = value;
            }
        }

        private DateTime end;
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.ToUniversalTime();
                //end = value;
            }
        }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }

        public int? OwnerID { get; set; } //this is the ProfileId

        private string starttimezone;
        public string StartTimezone
        {
            get
            {
                return starttimezone;
            }

            set
            {
                starttimezone = value;
            }
        }

        private string endtimezone;
        public string EndTimezone
        {
            get
            {
                return endtimezone;
            }

            set
            {
                endtimezone = value;
            }
        }

        //public RosterItem ToEntity()
        //{
        //    var entity = new RosterItem
        //    {
        //        Id = TaskID,
        //        Name = Title,
        //        Start = Start,
        //        End = End,
        //        Description = Description,
        //        RecurrenceRule = RecurrenceRule,
        //        RecurrenceException = RecurrenceException,
        //        RecurrenceID = RecurrenceID,
        //        IsAllDay = IsAllDay,
        //        ProfileId = OwnerID.HasValue ? OwnerID.Value : 1,
        //        StartTimezone = StartTimezone,
        //        EndTimezone = EndTimezone
        //    };

        //    return entity;
        //}

        public ProfileRoster ToEntity()
        {
            var entity = new ProfileRoster
            {
                Id = TaskID,
                Name = Title,
                Start = Start,
                End = End,
                Description = Description,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                IsAllDay = IsAllDay,
                ProfileId = OwnerID.HasValue ? OwnerID.Value : 1,
                StartTimezone = StartTimezone,
                EndTimezone = EndTimezone
            };

            return entity;
        }

    }


    public class RosterItemViewModelRevised : UserHeaderMV, ISchedulerEvent
    {
        /*
         *  Start and End 
         */

        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        private DateTime start;
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                //start = value.ToUniversalTime();
                start = value;
            }
        }

        private DateTime end;
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                //end = value.ToUniversalTime();
                end = value;
            }
        }

        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }

        public int? OwnerID { get; set; } //this is the ProfileId

        private string starttimezone;
        public string StartTimezone
        {
            get
            {
                return starttimezone;
            }

            set
            {
                starttimezone = value;
            }
        }

        private string endtimezone;
        public string EndTimezone
        {
            get
            {
                return endtimezone;
            }

            set
            {
                endtimezone = value;
            }
        }

      

        public ProfileRoster ToEntity()
        {
            var entity = new ProfileRoster
            {
                Id = TaskID,
                Name = Title,
                Start = Start,
                End = End,
                Description = Description,
                RecurrenceRule = RecurrenceRule,
                RecurrenceException = RecurrenceException,
                RecurrenceID = RecurrenceID,
                IsAllDay = IsAllDay,
                ProfileId = OwnerID.HasValue ? OwnerID.Value : 1,
                StartTimezone = StartTimezone,
                EndTimezone = EndTimezone
            };

            return entity;
        }

    }

}