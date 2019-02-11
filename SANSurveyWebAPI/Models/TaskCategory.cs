using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SANSurveyWebAPI.Models
{
    public class TaskCategory
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string TaskType { get; set; }
        public bool IsDeleted { get; set; }
    }
    public enum TaskTypeVal
    {
        MyDay, WAM
    }
}