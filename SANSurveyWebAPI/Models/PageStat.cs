using System;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class PageStat
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string PageName { get; set; }

        [StringLength(20)]
        public string PageType { get; set; }

        [StringLength(20)]
        public string PageAction { get; set; }

        public DateTime? TaskStartDateTime { get; set; }



        public int ProfileId { get; set; }
        public int? SurveyId { get; set; }

        public int? TaskId { get; set; }


        public bool? WholePageIndicator { get; set; } //for get and post methods, enter and exit time measurement indicator

        public DateTime PageDateTimeUtc { get; set; }
        public string Remark { get; set; }
    }
}