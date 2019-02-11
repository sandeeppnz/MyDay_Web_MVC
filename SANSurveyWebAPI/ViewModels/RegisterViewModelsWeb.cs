using SANSurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{

    #region Register VM
    public class EthinicityVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public class WizardViewModel
    {
        public int CurrStep { get; set; }
        public int MaxStep { get; set; }
        public int SurveyType { get; set; }
        // 0 means MyDay and 1 means The Warren and Mahoney (WAM) survey
    }


    [Serializable]
    public class RegisterCompletedViewModel : RegisterVM
    {}


    [Serializable]
    public class RegisterRoster1ViewModel : RegisterVM
    {
        public string YearMonth { get; set; }
    }

    [Serializable]
    public class RegisterFeedbackViewModel : RegisterVM
    {
        public string Comment { get; set; }
    }

    public class RegisterVM
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

    [Serializable]
    public class RegisterTaskTimeViewModel : RegisterVM
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
    public class RegisterScreeningViewModel : RegisterVM
    {
        public string Uid { get; set; }

        public int MaxStep { get; set; }
        public string CurrentLevelOfTraining { get; set; }

        [Display(Name = "Are you currently in a placement?")]
        public string IsCurrentPlacement { get; set; }

    }


    [Serializable]
    public class RegisterWellBeing1ViewModel : RegisterVM
    {
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }
        public string Q3Ans { get; set; }
    }

    [Serializable]
    public class RegisterTask1ViewModel : RegisterVM
    {
        public IList<CheckBoxListItem> FullTaskList { get; set; }
        public RegisterTask1ViewModel()
        {
            FullTaskList = new List<CheckBoxListItem>();
        }
    }

    public class TaskVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Frequency { get; set; }
        //public bool? IsWardRoundIndicator { get; set; }
    }

    public class SelectedTaskVM
    {
        public int ID { get; set; }
        public string Frequency { get; set; }
    }

    public class SelectedSpecialityVM
    {
        public int ID { get; set; }
        public string OtherText { get; set; }
    }


    [Serializable]
    public class RegisterTask2ViewModel : RegisterVM
    {
        //public int[] SelectedTasks { get; set; }
        public string TaskType { get; set; }
        public IList<TaskVM> FullTaskListPatient { get; set; }
        public IList<TaskVM> FullTaskListNonPatient { get; set; }

        public RegisterTask2ViewModel()
        {
            FullTaskListPatient = new List<TaskVM>();
            FullTaskListNonPatient = new List<TaskVM>();
        }
    }


    [Serializable]
    public class RegisterDemographicsViewModel: RegisterVM
    {
        [EmailAddress]
        public string Email { get; set; }

        //[EmailAddress]
        //public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        //[Display(Name = "Month of Birth")]
        //public string BirthMonth { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> BirthMonthList { get; set; }
        
        [Display(Name = "Year of Birth")]
        public string BirthYear { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> BirthYearList { get; set; }

        //[Display(Name = "Other Ethinicity")]
        //public string EthinicityOther { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "MaritialStatus")]
        public string MaritialStatus { get; set; }

        [Display(Name = "IsCaregiverChild")]
        public string IsCaregiverChild { get; set; }

        [Display(Name = "IsCaregiverAdult")]
        public string IsCaregiverAdult { get; set; }

        [Display(Name = "IsUniversityBritish")]
        public string IsUniversityBritish { get; set; }

        [Display(Name = "UniversityAttended")]
        public string UniversityAttended { get; set; }

        [Display(Name = "IsLeadership")]
        public string IsLeadership { get; set; }

        [Display(Name = "UniversityAttendedOtherText")]
        public string UniversityAttendedOtherText { get; set; }

        //public IList<CheckBoxListItem> EthinicityTypeList { get; set; }
        //public int[] SelectedEthinicities { get; set; }
        //public IList<EthinicityVM> EthinicityTypeListDropDown { get; set; }

        //public IList<MedicalUniversityVM> MedicalUniversityTypeListDropDown { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> MedicalUniversityTypeListDropDown { get; set; }

        public RegisterDemographicsViewModel()
        {
            //EthinicityTypeListDropDown = new List<EthinicityVM>();
            //SelectedEthinicities = new int[] { };
            //MedicalUniversityTypeListDropDown = new List<MedicalUniversityVM>();
            //MedicalUniversityTypeListDropDown = new List<MedicalUniversityVM>();
        }
    }

    public class RegisterContractViewModel : RegisterVM
    {

        public string ContractType { get; set; }
        public string WorkingStatus { get; set; }
        public string HoursWorkedLastMonth { get; set; }
    }

    /*  public class RegisterSpecialtyViewModel : RegisterVM
      {

          [Display(Name = "Medical Speciality")]
          public int? SpecialityId { get; set; }
          //public string OtherText { get; set; }

          [Display(Name = "OtherText")]
          public string OtherText { get; set; }


          public IEnumerable<System.Web.Mvc.SelectListItem> SpecialityTypeList { get; set; }
      }*/
    public class RegisterSpecialtyViewModel : RegisterVM
    {

        public int? SpecialityId { get; set; }
        public string OtherText { get; set; }
        
        public IEnumerable<System.Web.Mvc.SelectListItem> SpecialityTypeList { get; set; }
    }


    public class RegisterTrainingViewModel : RegisterVM
    {
        public string TrainingStartYear { get; set; }
        public string IsTrainingBreak { get; set; }
        public decimal? TrainingBreakLengthMonths { get; set; }
    }




    public class RegisterPlacementViewModel : RegisterVM
    {
        public string PlacementStartYear { get; set; }
        public string PlacementStartMonth { get; set; }

        public string PlacementIsInHospital { get; set; }
        public string PlacementHospitalName { get; set; }
        public string PlacementHospitalNameOther { get; set; }

        public string PlacementHospitalStartMonth { get; set; }
        public string PlacementHospitalStartYear { get; set; }

        //public string ContractType { get; set; }
        //public string WorkingStatus { get; set; }
        //public string HoursWorkedLastMonth { get; set; }
        //[Display(Name = "Medical Speciality")]
        //public int SpecialityId { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> SpecialityTypeList { get; set; }
        //public string TrainingStartYear { get; set; }
        //public string IsTrainingBreak { get; set; }
        //public decimal TrainingBreakLengthMonths { get; set; }
        //    [Display(Name = "Highest Qualification")]
        //    public string HighestQualification { get; set; }
        //    //public IEnumerable<System.Web.Mvc.SelectListItem> HighestQualificationList { get; set; }
        //    [Display(Name = "University Attended")]
        //    public string UniversityAttended { get; set; }
        //    //public IEnumerable<System.Web.Mvc.SelectListItem> UniversityAttendedList { get; set; }
        //    [Display(Name = "Current Level of Training")]
        //    public string CurrentLevelOfTraining { get; set; }
        //    //public IEnumerable<System.Web.Mvc.SelectListItem> CurrentLevelOfTrainingList { get; set; }
        //    [Display(Name = "Year of Training")]
        //    public string YearOfTraining { get; set; }
        //    //public IEnumerable<System.Web.Mvc.SelectListItem> YearOfTrainingList { get; set; }
        //    [Display(Name = "Are you currently in a placement?")]
        //    public string CurrentPlacement { get; set; }
    }


    #endregion

    #region WAMDemo

    [Serializable]
    public class RegisterWAMWellbeingVM : RegisterVM
    {
        public string Q1Ans { get; set; }
        public string Q2Ans { get; set; }
        public string Q3Ans { get; set; }
    }
    [Serializable]
    public class RegisterWAMProfileRoleVM : RegisterVM
    {
        public string myProfileRole { get; set; }
        public string roleStartYear { get; set; }
        public List<DisciplineOrRole> disciplineRoleObjList { get; set; }
    }
    [Serializable]
    public class RegisterWAMTasksVM : RegisterVM
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
    public class RegisterWAMDemographicsVM : RegisterVM
    {
        [Display(Name = "Year of Birth")]
        public string BirthYear { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> BirthYearList { get; set; }
        
        [Display(Name = "Gender")]
        public string Gender { get; set; }        

        [Display(Name = "IsCaregiverChild")]
        public string IsCaregiverChild { get; set; }        
    }
    [Serializable]
    public class RegisterWAMIntentionsVM : RegisterVM
    {
        public decimal? SameEmployer { get; set; }
        public decimal? SameIndustry { get; set; }
    }
    [Serializable]
    public class RegisterWAMFeedbackVM : RegisterVM
    {
        public string Feedback { get; set; }
    }
    #endregion

}
