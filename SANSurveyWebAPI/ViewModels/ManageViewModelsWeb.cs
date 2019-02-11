using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SANSurveyWebAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SANSurveyWebAPI.ViewModels.Web
{
    public class CalendarListViewModel : UserHeaderMV
    {
        public string YearMonth { get; set; }
        public bool freezeRoster { get; set; }
        public List<int> totalMyDaysurveys { get; set; }
    }

    public class NotificationVM
    {
        public string Message { get; set; }
        public string FlagColor { get; set; }
        public string Link { get; set; }
    }


    public class NotificationListVM : UserHeaderMV
    {

    }

    public class HomeMySurveyVM
    {
        //public DateTime SurveyWindowStartDateTime { get; set; }
        //public DateTime SurveyExpiryDateTime { get; set; }
        public string DateStr { get; set; }
        public string NameStr { get; set; }
        public string StatusStr { get; set; }
        public string ActionStr { get; set; }
    }

    public class HomeMySurveyListVM : UserHeaderMV
    {
        public List<HomeMySurveyVM> surveys { get; set; }
    }


    public class UserHeaderMV
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileEmail { get; set; }
        public int ProfileOffset { get; set; }
        public List<NotificationVM> notifications { get; set; }               
    }

    //public class NotificationDropDownVM
    //{
    //    public NotificationListVM list { get; set; }
    //}
       

    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    
    public class EditProfileViewModel
    {

       
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Display(Name = "Mobile Notifications")]
        public bool MobileNotificationOn { get; set; }


        //[Display(Name = "Month of Birth")]
        //public string BirthMonth { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> BirthMonthList { get; set; }

        //[Display(Name = "Year of Birth")]
        //public string BirthYear { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> BirthYearList { get; set; }

        //[Display(Name = "Gender")]
        //public string Gender { get; set; }

        //public List<CheckBoxListItem> EthinicityTypeList { get; set; }

        //[Display(Name = "Speciality")]
        //public int SpecialityId { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> SpecilityTypeList { get; set; }


        //[Display(Name = "Year of Training")]
        //public string YearOfTraining { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> ExperienceYearList { get; set; }


        //[Display(Name = "Highest Qualification")]
        //public string UniversityAttended { get; set; }
        //public IEnumerable<System.Web.Mvc.SelectListItem> EducationQualificationList { get; set; }


        //public string OtherUniversityAttended { get; set; }



        public EditProfileViewModel()
        {
            //EthinicityTypeList = new List<CheckBoxListItem>();
        }
    }








    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberVM
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

}