using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.Models
{
    public class ProximiVisitor
    {
        [Key]
        public int ID { get; set; }
        public string UserId { get; set; }
        public string VisitorId { get; set; }
        public string CreatedDate { get; set; }
        public string TimeZone { get; set; }
    }
}