using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class HangfireStateCustomDto
    {
        public int StateId { get; set; }
        public int JobId { get; set; }
        public string StateName { get; set; }
        public string ReasonName { get; set; }
        public string Data { get; set; }
        public string JobArguments { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }
    }

    public class HangfireStateDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Reason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }

        //
        //public string JobMethod { get; set; }
        //public double RunAfterMin { get; set; }
        //public DateTime? CreatedDateTimeServer { get; set; }
        //public DateTime? CreatedDateTimeUtc { get; set; }
        //public int ProfileRosterId { get; set; }
    }

}