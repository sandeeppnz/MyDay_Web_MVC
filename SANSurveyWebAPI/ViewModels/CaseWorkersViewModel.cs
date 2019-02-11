using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{
    public class CaseWorkersViewModel
    {       
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmail { get; set; }
        public int ProfileOffset { get; set; }
        public int MaxStep { get; set; }
        public string RegistrationProgressNext { get; set; }
        public DateTime? LastUpdatedDateTimeUtc { get; set; }
        public string ClientInitials { get; set; }
    }

    public class CaseTasksQA
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Frequency { get; set; }
        public bool Ans { get; set; }
    }
   
    [Serializable]
    public class CaseWellBeingVM : CaseWorkersViewModel
    {
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }
        public string Q3Ans { get; set; }
    }
    [Serializable]
    public class CurrentWorkplaceVM : CaseWorkersViewModel
    {        
        public string TaskType { get; set; }
        public IList<CaseTasksQA> QptionsList { get; set; }       

        public CurrentWorkplaceVM()
        {
            QptionsList = new List<CaseTasksQA>();           
        }
        public string HiddenWorkHoursOptionIds { get; set; }
    }
    [Serializable]
    public class CurrentWorkplaceContdVM : CaseWorkersViewModel
    {      
        public string WorkStatus { get; set; }
        public string WorkPosition { get; set; }
        public string WorkCountry { get; set; }
        public string OtherWorkCountry { get; set; }

        public CurrentWorkplaceContdVM()
        {
            
        }
       
    }
    [Serializable]
    public class CaseLoadVM : CaseWorkersViewModel
    {
        public string Q1 { get; set; }

    }
    [Serializable]
    public class TimeAllocationVM : CaseWorkersViewModel
    {
        public decimal? ClinicalActualTime { get; set; }
        public decimal? ResearchActualTime { get; set; }
        public decimal? TeachingLearningActualTime { get; set; }
        public decimal? AdminActualTime { get; set; }

        public decimal? ClinicalDesiredTime { get; set; }
        public decimal? ResearchDesiredTime { get; set; }
        public decimal? TeachingLearningDesiredTime { get; set; }
        public decimal? AdminDesiredTime { get; set; }
    }
    [Serializable]
    public class DemographicsVM : CaseWorkersViewModel
    {
        [Display(Name = "Year of Birth")]
        public string BirthYear { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> BirthYearList { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "MaritialStatus")]
        public string MaritalStatus { get; set; }

        [Display(Name = "IsCaregiverChild")]
        public string IsCaregiverChild { get; set; }

        [Display(Name = "IsCaregiverAdult")]
        public string IsCaregiverAdult { get; set; }

        public string EthnicityOrRace { get; set; }
    }
    [Serializable]
    public class EducationBackgroundVM : CaseWorkersViewModel
    {
        public string BachelorsDegree { get; set; } 
        public string MastersDegree { get; set; }
        public string PreServiceTraining { get; set; }
    }
    [Serializable]
    public class JobIntentionsVM : CaseWorkersViewModel
    {
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }        
    }
    [Serializable]
    public class RosterVM : CaseWorkersViewModel
    {
        public string YearMonth { get; set; }
    }
    [Serializable]
    public class FeedbackVM : CaseWorkersViewModel
    {
        public string Comment { get; set; }
    }
    [Serializable]
    public class CompletedVM : CaseWorkersViewModel
    { }
    [Serializable]
    public class CaseWorkersTaskVM : CaseWorkersViewModel
    {
        //public int[] SelectedTasks { get; set; }
        public string TaskType { get; set; }
        public IList<TaskVM> FullTaskList { get; set; }
        
        public CaseWorkersTaskVM()
        {
            FullTaskList = new List<TaskVM>();
        }
    }

}