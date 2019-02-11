using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SANSurveyWebAPI.Models;

namespace SANSurveyWebAPI.ViewModels
{
    public class TaskCategoryVM
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string TaskType { get; set; }
        public string IsDeleted { get; set; }        
    }
    [Serializable]
    public class TaskCategoryViewVM : TaskCategoryVM
    {
        public TaskCategory TaskCatObj { get; set; }//contains single meeting details
        public List<TaskCategory> TaskCategoryObj { get; set; }//list of meeting
        public string Status { get; set; }//This will help us to handle the view logic                    
        public int hiddenTaskIds { get; set; }
        public int totalRowCount { get; set; }
    }
}