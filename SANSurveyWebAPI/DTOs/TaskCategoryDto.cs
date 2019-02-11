using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.DTOs
{
    public class TaskCategoryDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string TaskType { get; set; }
        public bool IsDeleted { get; set; }
    }
}